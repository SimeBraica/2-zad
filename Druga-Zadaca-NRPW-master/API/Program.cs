using API;
using API.Controllers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options => options.Cookie.Name = "token") 
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"], 
        ValidAudience = builder.Configuration["Jwt:Audience"], 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) 
    };
    options.Events = new JwtBearerEvents {
        OnMessageReceived = context => {
            context.Token = context.Request.Cookies["token"]; 
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddScoped<OwnerController>();
builder.Services.AddScoped<PetController>();
builder.Services.AddScoped<HttpClient>();

builder.Services.AddHttpContextAccessor();

string corsPolicyName = "cors";
builder.Services.AddCors(options => {
    options.AddPolicy(corsPolicyName, policy => {
        policy.WithOrigins("http://localhost:420") 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddDbContext<NrppwDrugaContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database_Conn")));

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting(); 
app.UseCors(corsPolicyName); 
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => {
    endpoints.MapControllers(); 
    endpoints.MapFallbackToFile("index.html"); 
});

app.Run(); 
