using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;

using Microsoft.AspNetCore.ResponseCompression;
using Tecnocasa2024.Server.Models;
using Tecnocasa2024.Server.Models.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http.Json;
using Tecnocasa2024.Server.Models.Servicios.Interfaces;
using Tecnocasa2024.Server.Models.Servicios;


var builder = WebApplication.CreateBuilder(args);

//.....servicios propios......
// 1º) servicio de acceso a BD usando MongoDB
builder.Services.AddScoped<IDBAccess, MongoDBAccess>();


// 2º) configuracion autentificacion y autorizacion de clientes para uso de servicio REST mediante JWT...
// o proveeddores externos como Google....
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                        (JwtBearerOptions opciones) => {

                            byte[] _bytesClaveFirma = System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:clavefirma"]);

                            opciones.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters { 
                                ValidateLifetime = true,
                                ValidateIssuer=true,
                                ValidateIssuerSigningKey=true,
                                ValidateAudience=false,
                                ValidIssuer = builder.Configuration["JWT:issuer"],
                                IssuerSigningKey=new SymmetricSecurityKey(_bytesClaveFirma)
                            };
                        }
                    )
                .AddGoogle(
                        (GoogleOptions opciones) =>
                        {
                            opciones.ClientId = builder.Configuration["Google:client_id"];
                            opciones.ClientSecret = builder.Configuration["Google:client_secret"];
                        }
                    );

builder.Services.AddScoped<ICorreo, MailJetService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

//------------------ añadimos middleware a la pipeline del servidor donde corre servicio rest para habilitar autentificacion por JWT ----
app.UseAuthentication();
app.UseAuthorization();
// -------------------------------------------------------------------------------------------------------------------------------------

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
