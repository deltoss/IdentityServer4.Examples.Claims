# Claims Manipulation Example - API Resource

This example demonstrates adding identity claims to the `access token` for the `Password Grant`. This is done via configuring `API Resources` with identity claims, so that whenever a linked API scope is requested, those identity claims automatically gets added to the access token. The code of interest that grants that behavior sits in the [Config.cs](../src/IdentityServer4Example.STS/Config.cs) & [Startup.cs](../src/IdentityServer4Example.STS/Startup.cs) file.

This example is wired up so that when you request the `role` or `email` scope, it'll return the `role` or `email` claims of the user as part of the returned access token.

> Note 📜
>
> All data used by the STS is stored in-memory, populated with dummy data, to keep this example simple as possible.

## Solution Architecture

Project | Description
--- | ---
`IdentityServer4Example.STS` | The STS (Security Token Service) project. AKA authorisation server. It has endpoints that'd provide & validate tokens which can be used to authenticate valid users with.
`IdentityServer4Example.ConsoleClient` | A console application that acts as a HTTP client to contact the STS endpoint to get an access token.

## `IdentityServer4Example.STS` Project

This project is a console application with the following important files:

File | Description
--- | ---
`Config.cs` | Contains the various seed data for the STS. When the STS project starts up, it'd automatically create these seed data entries into our auth data store (which is wired up to be in-memory).
`Program.cs` | The entrypoint of the console application.
`Startup.cs` | The file where you configure various services, including Identity Server 4.

> Developer Tip 💡
>
> The STS application doesn't necessarily have to be a console Application. It could also be a web application if you want a Login UI for certain grant types such as `Authorization Code` flow.

## Testing

1. Run the `IdentityServer4Example.STS` project. This would host your STS, which you can use endpoints to get access tokens from, using valid credentials.

2. To check if the project is running fine, navigate to <https://localhost:5001/.well-known/openid-configuration>.

   > Developer Tip 💡
   >
   > The `<STSUrl>/.well-known/openid-configuration` endpoint is a standard endpoint where it'd show you the `OpenID Connect Discovery Document`. It essentially contains meta data regarding your STS server, such as what grant types it supports, claims that it uses, etc.

3. Obtain an access token. To do this, you have multiple options. See [Obtaining an Access Token from the STS](#obtaining-an-access-token-from-the-sts).

4. You can copy the access token string, and paste it into a JWT decryption service so you may see what's inside the access token. One such service is [jwt.ms](https://jwt.ms/). Notice that you'd be able to see that it has the claims `role` & `email`.

   ![Testing - Access Token Decrypted Response](./assets/Testing%20-%2002%20-%20Access%20Token%20Decrypted%20Response.jpg)

## Obtaining an Access Token from the STS

This project supports the [password grant type](https://oauth.net/2/grant-types/password/). Thus, we'll need to send our client credentials AND the user credentials to the STS server to obtain an access token.

Essentially, you need send a HTTP request with:

- The `POST` verb.
- To the STS token endpoint of `localhost:5001/connect/token`
- With the following fields & values:

  Field Name | Value | Description
  --- | --- | ---
  `grant_type` | password | Defines what grant type (i.e. authorisation workflow) you are using.
  `scope` | weatherforecastapi.read | Defines what set of permissions and/or information you are requesting for.
  `client_id` | client | The unique identifier of the client.
  `client_secret` | secret | The password of the client.
  `username` | scott | The username of the user to log in with.
  `password` | password | The password of the user to log in with.
- The field and values must be send following the `application/x-www-form-urlencoded` format.

To actually send a valid OAuth HTTP token request that meets the above requirements, there are several ways. See below sections.

### Via the Provided Example Console App

Simply start the `IdentityServer4Example.ConsoleClient` project, and you'll see that you'd get a token back.

> Developer Tip 💡
>
> If you use Visual Studio, you can start multiple projects via following this [MSDN article](https://docs.microsoft.com/en-us/visualstudio/ide/how-to-set-multiple-startup-projects?view=vs-2019).

### Via a HTTP Client

HTTP Clients refers to applications such as `Postman`, `Insomnia`, etc. They let you send HTTP requests to an endpoint.

1. Open up your HTTP client of choice.

2. Import the below `CURL` request. HTTP Clients usually have a feature where it allows you to import CURL requests.

   ```bash
   curl --location --request POST 'https://localhost:5001/connect/token' \
   --header 'Content-Type: application/x-www-form-urlencoded' \
   --data-urlencode 'grant_type=password' \
   --data-urlencode 'scope=weatherforecastapi.read' \
   --data-urlencode 'client_id=client' \
   --data-urlencode 'client_secret=secret' \
   --data-urlencode 'username=scott' \
   --data-urlencode 'Password=password'
   ```

3. Execute the request, and you should get a response with an access token.

### Via a Script

If you're on `bash` or a linux based terminal that has the `curl` tool, just run the below script:

```bash
curl --location --request POST 'https://localhost:5001/connect/token' \
--header 'Content-Type: application/x-www-form-urlencoded' \
--data-urlencode 'grant_type=password' \
--data-urlencode 'scope=weatherforecastapi.read' \
--data-urlencode 'client_id=client' \
--data-urlencode 'client_secret=secret' \
--data-urlencode 'username=scott' \
--data-urlencode 'Password=password'
```

Alternatively, if you're on Windows and you use Powershell, you can instead run this script:

```powershell
$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
$headers.Add("Content-Type", "application/x-www-form-urlencoded")

$body = "grant_type=password&scope=weatherforecastapi.read&client_id=client&client_secret=secret&username=scott&Password=password"

$response = Invoke-RestMethod 'https://localhost:5001/connect/token' -Method 'POST' -Headers $headers -Body $body
$response | ConvertTo-Json
```

## Considerations

- This example uses an in-memory data store. For practical use, you'd use a proper data store (e.g. SQL Server database) instead.
