using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace GitInsight.Blazor;

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public CustomAuthorizationMessageHandler(IAccessTokenProvider provider,
        NavigationManager navigationManager)
        : base(provider, navigationManager)
    {
        ConfigureHandler(
            authorizedUrls: new[] { "https://localhost:7273/" },
            scopes: new[] { "https://GrumlebobGitInsight.onmicrosoft.com/09ca411b-9cae-4ce4-8e3c-db945e2a2dbf/API.Access" });
    }
}