using LoanCalculator.Configurations;
using LoanCalculator.Data;
using LoanCalculator.Factories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var allowedHosts = builder.Configuration.GetSection("AllowedHosts").Get<string>();
if (allowedHosts == "*")
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: "AllowAll",
            builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    });
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<LoanCalculatorFactory>();

BuildConfiguration();
ConfigureUrl();
ConfigureDb();

var app = builder.Build();

SeedDatabase(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

if (allowedHosts == "*")
{
    app.UseCors("AllowAll");
}

app.MapControllers();

app.Run();

#region configurations
void BuildConfiguration()
{
    builder.Services
        .Configure<ApiOptions>(builder.Configuration.GetSection(ApiOptions.name));
}
void ConfigureUrl()
{
    var apiOptions = builder.Configuration.GetSection(ApiOptions.name).Get<ApiOptions>()
        ?? throw new Exception($"Configuration for: {ApiOptions.name} configuration missing.");

    builder.WebHost.UseUrls(apiOptions.ApiUrl);
}
void ConfigureDb()
{
    var dbOptions = builder.Configuration.GetSection(DbOptions.name).Get<DbOptions>()
        ?? throw new Exception($"Configuration for: {DbOptions.name} is missing.");

    builder.Services.AddSingleton(dbOptions);
    switch (dbOptions.DbType)
    {
        case DbType.SQLITE:
            ConfigureSQLite(dbOptions);
            break;
        case DbType.SQLSERVER:
            ConfigureSQLServer(dbOptions);
            break;
        default:
            throw new Exception($"Unrecognized db-configuration: {dbOptions.DbType}");
    }
}
void ConfigureSQLite(DbOptions dbOptions)
{
    builder.Services.AddDbContext<Context>(options => {
        options.UseSqlite(dbOptions.ConnectionString);
    });
}

void ConfigureSQLServer(DbOptions dbOptions)
{
    builder.Services.AddDbContext<Context>(options => {
        options.UseSqlServer(dbOptions.ConnectionString);
    });
}
#endregion

void SeedDatabase(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<Context>();
        context.SeedDatabase().Wait();
    }
}