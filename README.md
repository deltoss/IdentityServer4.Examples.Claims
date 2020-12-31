# Ways To Add Claims to Tokens with Identity Server 4

## What are Claims?

When you're requesting access tokens from the STS (Security Token Service), i.e. Identity Server 4, as part of the token you get a number of **claims** back.

`Claims` are pieces of information that describes a given entity. It may describe a client, if you're using [Client Credentials](https://oauth.net/2/grant-types/client-credentials/). Or, it could describe a user, if you're using [Password Grant](https://oauth.net/2/grant-types/password/) or [Authorisation Code](https://oauth.net/2/grant-types/authorization-code/) flow.

For user focused grant types such as [Password Grant](https://oauth.net/2/grant-types/password/) or [Authorisation Code](https://oauth.net/2/grant-types/authorization-code/) flow, the `claims` can be used to identify a user, or even be used as part of the process to authenticate/authorise users.

You can request a JWT `access token` from the [/connect/token](https://identityserver4.readthedocs.io/en/latest/endpoints/token.html) endpoint. That access token, if you decrypt it would contain many claims that describes the user.

For example, lets say you have the below access token:

```text
eyJhbGciOiJSUzI1NiIsImtpZCI6Ijc3NENEQTM3QjRDOEM0OTNFQjRGOUQ1NTZBRDUwQzQ2IiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2MDkwNzQ5NzksImV4cCI6MTYwOTA3ODU3OSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsImF1ZCI6WyJ3ZWF0aGVyZm9yZWNhc3RhcGkiLCJodHRwczovL2xvY2FsaG9zdDo1MDAxL3Jlc291cmNlcyJdLCJjbGllbnRfaWQiOiJjbGllbnQiLCJzdWIiOiI1QkU4NjM1OS0wNzNDLTQzNEItQUQyRC1BMzkzMjIyMkRBQkUiLCJhdXRoX3RpbWUiOjE2MDkwNzQ5NzksImlkcCI6ImxvY2FsIiwianRpIjoiNTQwMzU5MEE0RDY4NjUxRTIwODEyNkE0NzJENjYwRkQiLCJpYXQiOjE2MDkwNzQ5NzksInNjb3BlIjpbIndlYXRoZXJmb3JlY2FzdGFwaS5yZWFkIl0sImFtciI6WyJwd2QiXX0.mmcHxbVSS7nmGbUWyAhgYqDw6V2bIj87gTTcN15LrMJcVxEoV4RvSDGYJS1_3-2BPP_ceIMWg1ZbGVaLtY3jjTJmNkErIXWGC2DyMKzTnvUhjiRu-HoqByFov6p9AOV-uUZDKM-qwaaA-zgLET2voza_VdyaPpXsWIKxV7fVU7Rq_Cp5thkk-ueaQZJCVhWV_RpXT5zZ_LCgOWgdjw1a5CdXyOCs_R-V6QfN5ZABRiWVztqzfUas0UVApZqS6dgWTHB8rVu4v-Umdqux1TT1CC7FnLGTeaDM3_Sa_u0iutivKeUWxXVi2TdrAmESDgBjYxd1F5rIHK3X8xE485Z8pA
```

If you copy and paste the above access token, and paste it into a JWT decryption service, such as [jwt.ms](https://jwt.ms/), you'd see something like below:

```jwt
{
  "alg": "RS256",
  "kid": "774CDA37B4C8C493EB4F9D556AD50C46",
  "typ": "at+jwt"
}.{
  "nbf": 1609074979,
  "exp": 1609078579,
  "iss": "https://localhost:5001",
  "aud": [
    "weatherforecastapi",
    "https://localhost:5001/resources"
  ],
  "client_id": "client",
  "sub": "5BE86359-073C-434B-AD2D-A3932222DABE",
  "auth_time": 1609074979,
  "idp": "local",
  "jti": "5403590A4D68651E208126A472D660FD",
  "iat": 1609074979,
  "scope": [
    "weatherforecastapi.read"
  ],
  "amr": [
    "pwd"
  ]
}.[Signature]
```

The claims are key value pairs. For example, you'd see there's a claim of `sub`. This claim tells you who's the subject of the access token. Usually, it corresponds to the ID in the database for a given user. For the above, the value of that claim is `5BE86359-073C-434B-AD2D-A3932222DABE`.

## What's the Problem?

For general use cases, you want to add **additional claims** to the above token. So that you can identity a user, or for authentication/authorisation purposes.

For example, you may want to have the below claims added to the token:

Claim Type | Description
--- | ---
`email` | The user's email.
`role` | The role(s) the user have.

With `Identity Server 4`, how do we go about adding these additional claims to our token?

## Solutions

Below are some approaches you can take.

### Via UserInfo Endpoint

You can use the [UserInfo Endpoint](https://identityserver4.readthedocs.io/en/latest/endpoints/userinfo.html) to get the additional claims for a given access token. This is another HTTP request to the server, but it lets you request claims only when you need it.

See the [UserInfo Example](./UserInfo%20Endpoint%20Approach/documentation/README.md) for more information and example.

### Via API Resources

You can define `API Resources` with their associated claims in your database. That way, whenever you request an access token that corresponds to the API resource, you'd get the claims included as part of that access token.

See the [API Resource Example](./Api%20Resource%20Approach/documentation/README.md) for more information and example.

### Via Profile Service

You can implement Identity Server's [IProfileService](https://identityserver4.readthedocs.io/en/latest/reference/profileservice.html) interface. With this approach, it gives you full control, and you can add claims conditionally to the token for both access tokens and identity tokens. You can load data from databases to create your claims and attach it to the token.

> Note 📜
>
> Taking this approach means you're basically bypassing user claims associated with API Resources or Identity Resources.

See the [Profile Service Example](./Profile%20Service%20Approach/documentation/README.md) for more information and example.

### Via Profile Service with API Resources and/or UserInfo Endpoint

You may combine implementing the [IProfileService](https://identityserver4.readthedocs.io/en/latest/reference/profileservice.html) so that you can load claims from your own data store, but you leave whether the claims gets added to the access token or identity token based on the Identity Server logic (i.e. based on `API Resources` for access tokens, and `Identity Resources` for identity tokens).

This means essentially, you provide all the claims (loading them from data store(s) if needed), and Identity Server would figure out whether to add them to the token based on:

1. If it's an `access token`, then find the corresponding `API Resource`. If the `API Resource` has user claims defined and the claims exists, add the claim to the token.
2. If it's an `identity token`, then find the corresponding `Identity Resource`. If the `Identity Resource` has user claims defined and the claims exists, add the claim to the token.

This approach grants you flexibility, but also allows you to adjust what claims gets returned for each client via API resources and/or Identity Resources, potentially via a database or even an Admin UI.

See the [Mixed Profile Service Example](./Mixed%20Profile%20Service%20Approach/documentation/README.md) for more information and example.

## Additional Resources

- [IdentityServer4 Authorization and Working with Claims](https://code-maze.com/identityserver4-authorization/)
- [IdentityServer4, OAuth, OpenID Connect Series](https://code-maze.com/identityserver-4-series/)
