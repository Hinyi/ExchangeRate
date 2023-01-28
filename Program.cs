using System.Reflection;
using ExchangeRate;
using ExchangeRate.Entities;
using ExchangeRate.Interface;
using ExchangeRate.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Adding http client
builder.Services.AddHttpClient("ECB_SDMX", configure =>
{
    configure.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ExchangeRateAPI"));
});
// Add services to the container.
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddControllers();
builder.Services.AddScoped<DbSeeder>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ExchangeRateDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("ExchangeDbContext")));

var app = builder.Build();

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
await seeder.Seed();

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
