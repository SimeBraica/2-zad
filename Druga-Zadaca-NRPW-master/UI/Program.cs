using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UI;
using UI.Services;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Captcha;
using Blazorise.Captcha.ReCaptcha;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddBlazorise(options => {
        options.Immediate = true;
    })
    .AddBootstrapProviders() 
    .AddBlazoriseGoogleReCaptcha(reCaptchaOptions => {
        reCaptchaOptions.SiteKey = "6Lc1-HUqAAAAAOlwH177GLu1BPJN0O05ABebCtHd"; 
    });

builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<PetService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://druga-zadaca-nrpw.onrender.com/") });

await builder.Build().RunAsync();
