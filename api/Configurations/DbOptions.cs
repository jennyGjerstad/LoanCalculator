using System.Text.Json.Serialization;

namespace LoanCalculator.Configurations;

public class DbOptions
{
    public const string name = "DbConfiguration";
    public DbType DbType { get; set; } = DbType.SQLITE;
    public string ConnectionString { get; set; } = "";
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DbType
{
    SQLITE,
    SQLSERVER,
    MYSQL
}