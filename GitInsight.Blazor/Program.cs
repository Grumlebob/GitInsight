using System.Reflection;
using GitInsight.Blazor;
using GitInsight.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("https://GrumlebobGitInsight.onmicrosoft.com/09ca411b-9cae-4ce4-8e3c-db945e2a2dbf/API.Access");
});

builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

builder.Services.AddHttpClient<AnalysisService>( "GitInsight.Blazor.ServerAPI", client => 
        client.BaseAddress = new Uri("https://localhost:7273/"))
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddScoped<IAnalysisService, AnalysisService>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("GitInsight.Blazor.ServerAPI"));

await builder.Build().RunAsync();
