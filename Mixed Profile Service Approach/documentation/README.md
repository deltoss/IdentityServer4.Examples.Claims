# Claims Manipulation Example - ProfileService with UserInfo and/or API Resource

This example demonstrates adding identity claims to the `access token`. This example is wired up so that when you send a token request with the `role`, it'll return the `role` claim of the user as part of the returned access token. If you need additional user claims, you can send a request to the [UserInfo endpoint](https://identityserver4.readthedocs.io/en/latest/endpoints/userinfo.html) to retrieve the additional claims (i.e. `email` in this example).

This behavior is accomplished via implementing `IProfileService` in such a way where it'd maintain the default behavior where it adds claims based on API Resources (if you're requesting an access token), or Identity Resources (if you're requesting an identity token). See [ProfileService](../src/IdentityServer4Example.STS/IdentityServer4/Services/ProfileService.cs) for the implementation, where it adds additional claims.

> Developer Tip 💡
>
> The profile service isn't just called when access tokens are requested. It's also called for the [UserInfo](https://identityserver4.readthedocs.io/en/latest/endpoints/userinfo.html) & [Authorize](https://identityserver4.readthedocs.io/en/latest/endpoints/authorize.html) endpoints (for `Identity Tokens`). This means you can adjust the profile service to customise the claims returned for the those endpoints too. For more information [StackOverflow - IdentityServer4 and UserInfo endpoint customization](https://stackoverflow.com/questions/56172654/identityserver4-and-userinfo-endpoint-customization).
>
> Developer Tip 💡
>
> In addition to profile service, you may also want to override the validators to first check if the user exists in your user store. For more information, see:
>
> - [Identity Server 4 Official Documentation - Custom Token Request Validation and Issuance](https://identityserver4.readthedocs.io/en/latest/topics/custom_token_request_validation.html)
> - [Identity Server 4 Official Documentation - Resource Owner Password Validation](https://identityserver4.readthedocs.io/en/latest/topics/resource_owner.html)
> - [StackOverflow - IdentityServer4 register UserService and get users from database in asp.net core](https://stackoverflow.com/a/35306021/4229687)
>
> Note 📜
>
> All data used by the STS is stored in-memory, populated with dummy data, to keep this example simple as possible.

## Solution Architecture

Project | Description
--- | ---
`IdentityServer4Example.STS` | The STS (Security Token Service) project. AKA authorisation server. It has endpoints that'd provide & validate tokens which can be used to authenticate valid users with.
`IdentityServer4Example.ConsoleClient` | A console application that acts as a HTTP client to contact the STS endpoint to get an access token.

## Considerations

- This example uses an in-memory data store. For practical use, you'd use a proper data store (e.g. SQL Server database) instead.
