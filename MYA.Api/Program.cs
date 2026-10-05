using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MYA.Api.Swagger;
using MYA.Business;
using MYA.Business.Auth;
using MYA.Business.Mvs;
using MYA.Data;
using MYA.Models.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));
builder.Services.Configure<MyAuthOptions>(builder.Configuration.GetSection("MyAuth"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<MailApiOptions>(builder.Configuration.GetSection("MailApi"));
builder.Services.Configure<MvsOptions>(builder.Configuration.GetSection("Mvs"));


var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration is missing.");
if (string.IsNullOrWhiteSpace(jwtOptions.Issuer) ||
    string.IsNullOrWhiteSpace(jwtOptions.Audience) ||
    string.IsNullOrWhiteSpace(jwtOptions.SigningKey))
{
    throw new InvalidOperationException("JWT issuer, audience and signing key (at least 32 bytes) must be configured.");
}

var signingKey = Encoding.UTF8.GetBytes(jwtOptions.SigningKey);
if (signingKey.Length < 32)
{
    throw new InvalidOperationException("JWT signing key must be at least 32 bytes for HMAC-SHA256.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKey),
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddDataAccess();
builder.Services.AddBusinessServices();
builder.Services.AddHttpClient<MfaMailApiClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<MailApiOptions>>().Value;
    var timeoutSeconds = Math.Clamp(options.TimeoutSeconds, 1, 120);
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
});

builder.Services
    .AddHttpClient<MvsApiClient>(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                (request, certificate, chain, errors) =>
                {
                    var host = request.RequestUri?.Host;

                    return host is "10.20.3.129" or "10.20.3.131";
                }
        });
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEnd", policy =>
    {
        policy
            .WithOrigins("http://127.0.0.1:5173", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "MYA API",
        Version = "v1",
        Description = "Backend .NET 10 per login, MFA e token MYA."
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Incolla solo l'access token restituito da login o verifyMfa, senza il prefisso Bearer."
    });
    options.OperationFilter<BearerSecurityOperationFilter>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MYA API v1");
        options.RoutePrefix = "swagger";
        options.ConfigObject.AdditionalItems["syntaxHighlight"] = false;
    });
}

if (app.Configuration.GetValue<bool>("HttpsRedirection:Enabled"))
{
    app.UseHttpsRedirection();
}
app.UseCors("FrontEnd");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
