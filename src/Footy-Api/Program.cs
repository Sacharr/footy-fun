using System;
using Microsoft.Extensions.Configuration;
using FootyApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// Register a typed HttpClient for the external API
builder.Services.AddHttpClient<IFootyApiClient, FootyApiClient>((sp, client) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var baseUrl = cfg["FootyApi:BaseUrl"];
    if (!string.IsNullOrEmpty(baseUrl))
    {
        client.BaseAddress = new Uri(baseUrl);
    }

    // Using dotnet secrets to store the API key, so it won't be in source control.
    var apiKey = builder.Configuration["ApiKey"];
    if (!string.IsNullOrEmpty(apiKey))
    {
        // Use the correct header name required by your API (example: "X-Auth-Token")
        client.DefaultRequestHeaders.Add("X-Auth-Token", apiKey);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
