using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Text.Json;
using Tecnocasa2024.Client;
using Tecnocasa2024.Client.Models;
using Tecnocasa2024.Client.Models.Interfaces;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("Tecnocasa2024.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Tecnocasa2024.ServerAPI"));


//....servicios inyectados propios...
builder.Services.AddScoped<IAjaxServices,AjaxService>();
builder.Services.AddScoped<IStorageService,SubjectsStorage>();



await builder.Build().RunAsync();

