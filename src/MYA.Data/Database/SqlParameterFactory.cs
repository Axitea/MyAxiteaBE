using System.Data;
using Microsoft.Data.SqlClient;

namespace MYA.Data.Database;

internal static class SqlParameterFactory
{
    public static SqlParameter BigInt(string name, long value)
    {
        return new SqlParameter(name, SqlDbType.BigInt) { Value = value };
    }

    public static SqlParameter Int(string name, int value)
    {
        return new SqlParameter(name, SqlDbType.Int) { Value = value };
    }

    public static SqlParameter DateTime(string name, DateTimeOffset value)
    {
        return new SqlParameter(name, SqlDbType.DateTime) { Value = value.UtcDateTime };
    }

    public static SqlParameter NChar(string name, string? value, int size)
    {
        return new SqlParameter(name, SqlDbType.NChar, size) { Value = DbValue(value) };
    }

    public static SqlParameter NVarChar(string name, string? value, int size)
    {
        return new SqlParameter(name, SqlDbType.NVarChar, size) { Value = DbValue(value) };
    }

    public static SqlParameter NVarCharMax(string name, string? value)
    {
        return new SqlParameter(name, SqlDbType.NVarChar, -1) { Value = DbValue(value) };
    }

    public static SqlParameter VarChar(string name, string? value, int size)
    {
        return new SqlParameter(name, SqlDbType.VarChar, size) { Value = DbValue(value) };
    }

    private static object DbValue(string? value)
    {
        return value is null ? DBNull.Value : value;
    }
}
