using System.Globalization;
using Microsoft.Data.SqlClient;

namespace MYA.Data.Common;

internal static class SqlDataReaderExtensions
{
    public static string? GetNullableString(this SqlDataReader reader, string name)
    {
        var value = reader.GetValueOrNull(name);
        return value is null ? null : Convert.ToString(value, CultureInfo.InvariantCulture);
    }

    public static string? GetNullableTrimmedString(this SqlDataReader reader, string name)
    {
        return reader.GetNullableString(name)?.Trim();
    }

    public static string GetStringOrEmpty(this SqlDataReader reader, string name)
    {
        return reader.GetNullableString(name) ?? string.Empty;
    }

    public static string GetTrimmedStringOrEmpty(this SqlDataReader reader, string name)
    {
        return reader.GetNullableTrimmedString(name) ?? string.Empty;
    }

    public static string GetStringRequired(this SqlDataReader reader, string name)
    {
        return reader.GetNullableString(name)
            ?? throw new InvalidOperationException($"Column {name} is null.");
    }

    public static int? GetNullableInt32(this SqlDataReader reader, string name)
    {
        var value = reader.GetValueOrNull(name);
        return value is null ? null : Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    public static double? GetNullableDouble(this SqlDataReader reader, string name)
    {
        var value = reader.GetValueOrNull(name);
        if (value is null)
        {
            return null;
        }

        if (value is string stringValue)
        {
            return double.TryParse(
                stringValue,
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out var result)
                ? result
                : null;
        }

        return Convert.ToDouble(value, CultureInfo.InvariantCulture);
    }

    public static long? GetNullableInt64(this SqlDataReader reader, string name)
    {
        var value = reader.GetValueOrNull(name);
        return value is null ? null : Convert.ToInt64(value, CultureInfo.InvariantCulture);
    }

    public static long GetInt64Required(this SqlDataReader reader, string name)
    {
        return reader.GetNullableInt64(name)
            ?? throw new InvalidOperationException($"Column {name} is null.");
    }

    public static int GetInt32Required(this SqlDataReader reader, string name)
    {
        return reader.GetNullableInt32(name)
            ?? throw new InvalidOperationException($"Column {name} is null.");
    }

    public static DateTime GetDateTimeRequired(this SqlDataReader reader, string name)
    {
        var value = reader.GetValueOrNull(name)
            ?? throw new InvalidOperationException($"Column {name} is null.");

        return Convert.ToDateTime(value, CultureInfo.InvariantCulture);
    }

    public static bool? GetNullableBoolean(this SqlDataReader reader, string name)
    {
        var value = reader.GetValueOrNull(name);

        return value switch
        {
            null => null,
            bool boolValue => boolValue,
            byte byteValue => byteValue != 0,
            short shortValue => shortValue != 0,
            int intValue => intValue != 0,
            long longValue => longValue != 0,
            string stringValue => stringValue.Trim() is "1" or "S" or "s" or "Y" or "y" or "true" or "True",
            _ => Convert.ToBoolean(value, CultureInfo.InvariantCulture)
        };
    }

    public static DateTime? GetNullableDateTime(this SqlDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);

        return reader.IsDBNull(ordinal)
            ? null
            : Convert.ToDateTime(reader.GetValue(ordinal));
    }

    private static object? GetValueOrNull(this SqlDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal);
    }
}
