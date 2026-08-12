using MYA.Business;
using MYA.Business.Auth;
using MYA.Data;
using MYA.Models.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));
builder.Services.Configure<MyAuthOptions>(builder.Configuration.GetSection("MyAuth"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<MailApiOptions>(builder.Configuration.GetSection("MailApi"));

builder.Services.AddDataAccess();
builder.Services.AddBusinessServices();
builder.Services.AddHttpClient<IMfaMailSender, HttpMfaMailSender>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<MailApiOptions>>().Value;
    var timeoutSeconds = Math.Clamp(options.TimeoutSeconds, 1, 120);
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
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
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MYA API v1");
        options.RoutePrefix = "swagger";
    });
}

if (app.Configuration.GetValue<bool>("HttpsRedirection:Enabled"))
{
    app.UseHttpsRedirection();
}
app.UseCors("FrontEnd");
app.UseAuthorization();
app.MapControllers();

app.Run();
