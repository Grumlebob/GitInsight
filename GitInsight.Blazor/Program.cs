using System.Reflection;
using GitInsight.Blazor;
using GitInsight.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7273/") });
builder.Services.AddScoped<IAnalysisService, AnalysisService>();


// Add auth services
//builder.Services.AddApiAuthorization();

builder.Services.AddHttpClient( "GitInsight.Blazor.ServerAPI", client => 
        client.BaseAddress = new Uri("https://localhost:7273/"))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("GitInsight.Blazor.ServerAPI"));

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("https://grumlebobgitinsight.onmicrosoft.com/0e737e20-812f-48b4-b1ce-31cf62bd6ac7/API.Access");
});



await builder.Build().RunAsync();
