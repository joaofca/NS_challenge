using System.Reflection;
using CollisionEvents.Api.Configurations;
using CollisionEvents.Api.Configurations.Swagger;
using CollisionEvents.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.OperationFilter<AddRequiredOperatorIdParameterAttribute>();

    // Show Enums descriptions
    options.AddEnumsWithValuesFixFilters(x =>
    {
        x.IncludeDescriptions = true;
        x.XEnumNamesAlias = "x-enum-varnames";
        x.XEnumDescriptionsAlias = "x-enum-descriptions";
    });
});

builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0);
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// 
/// </summary>
public partial class Program { }