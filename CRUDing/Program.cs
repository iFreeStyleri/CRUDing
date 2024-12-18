using CRUDing.Core.DI;
using CRUDing.DAL.DI;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Enrichers.Span;
using System.Reflection;
using System.Text;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
); ;

var services = builder.Services;
var configuration = builder.Configuration;
services
    .ConfigureDAL(configuration)
    .ConfigureServices();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.WithSpan()
    .WriteTo.File($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/logs/log.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.ConfigureLogging(logging =>
{
    logging.AddSerilog();
}).UseSerilog();

services.AddStackExchangeRedisCache(options =>
{
    var redisOpt = configuration.GetSection("Redis");
    options.Configuration = redisOpt["Configuration"];
    options.InstanceName = redisOpt["InstanceName"];
    options.ConfigurationOptions = new ConfigurationOptions
    {
        ClientName = redisOpt["ClientName"],
        Password = redisOpt["Password"]
    };

});
services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
}); 
services.AddEndpointsApiExplorer();
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        var jwtOptions = configuration.GetSection("JwtOptions");
        opt.RequireHttpsMetadata = false;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions["ISSUER"],
            ValidateAudience = true,
            ValidAudience = jwtOptions["AUDIENCE"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions["KEY"]))
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                       ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
