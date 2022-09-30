using AwsAppConfig.Microsoft.Extensions.Configuration;
using dotenv.net;
using Microsoft.FeatureManagement;

DotEnv.Fluent()
            .WithExceptions()
            .WithEnvFiles("local.env")
            .WithTrimValues()
            .WithDefaultEncoding()
            .Load();

var builder = WebApplication.CreateBuilder(args);

// Add FeatureFlag and AWS Config 
builder.Configuration.AddAwsAppConfig();
builder.Configuration.AddAwsAppConfigFeatureFlags();

builder.Services.AddFeatureManagement()
    .AddAwsAppConfigFeatureFlagsCustomFilters();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
