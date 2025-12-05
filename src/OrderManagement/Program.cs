using OrderManagement.Data;
using OrderManagement.Repositories;
using OrderManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Order Management API",
        Version = "v1",
        Description = "REST API for Order Management with Dataverse integration"
    });
});

// Configure Dataverse settings
var dataverseSettings = builder.Configuration.GetSection("Dataverse").Get<DataverseSettings>() 
    ?? new DataverseSettings();
builder.Services.AddSingleton(dataverseSettings);

// Register Dataverse connection as singleton
builder.Services.AddSingleton<IDataverseConnection, DataverseConnection>();

// Register Repository and Service
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Initialize Dataverse connection on startup
var dataverseConnection = app.Services.GetRequiredService<IDataverseConnection>();
try
{
    await dataverseConnection.ConnectAsync();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Failed to connect to Dataverse on startup. Connection will be retried on first request.");
}

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
