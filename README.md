# IS3PostLogoutRedirectExample

Exhibits a problem with identity server 3 where the post logout redirect stops at the IS3 logout page instead of redirecting to the client's specified redirect.

In OwinConfigure.cs you need to put your AAD Client ID
