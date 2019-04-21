using System;
using System.ComponentModel;
using System.Globalization;

namespace SharpConfig
{
  internal sealed class FallbackStringConverter : ITypeStringConverter
  {
    public string ConvertToString(object value)
    {
      try
      {
        var converter = TypeDescriptor.GetConverter(value);
        return converter.ConvertToString(null, Configuration.CultureInfo, value);
      }
      catch (Exception ex)
      {
        throw SettingValueCastException.Create(value.ToString(), value.GetType(), ex);
      }
    }

    public object ConvertFromString(string value, Type hint)
    {
      try
      {
        var converter = TypeDescriptor.GetConverter(hint);
        return converter.ConvertFrom(null, Configuration.CultureInfo, value);
      }
      catch (Exception ex)
      {
        throw SettingValueCastException.Create(value, hint, ex);
      }
    }

    public Type ConvertibleType => null;

    public object TryConvertFromString(string value, Type hint)
    {
      // Just call ConvertFromString since implementation is already in a try-catch block.
      return ConvertFromString(value, hint);
    }
  }

  internal sealed class BoolStringConverter : TypeStringConverter<bool>
  {
    public override string ConvertToString(object value)
    {
      return value.ToString();
    }

    public override object ConvertFromString(string value, Type hint)
    {
      return TryConvertFromString(value, hint) ?? throw SettingValueCastException.Create(value, hint, null);
    }
    
    public override object TryConvertFromString(string value, Type hint)
    {
      switch (value.ToLowerInvariant())
      {
        case "false":
        case "off":
        case "no":
        case "n":
        case "0":
          return false;
        case "true":
        case "on":
        case "yes":
        case "y":
        case "1":
          return true;
        default:
          return null;
      }
    }
  }

  internal sealed class ByteStringConverter : TypeStringConverter<byte>
  {
    public override string ConvertToString(object value)
    {
      return value.ToString();
    }

    public override object ConvertFromString(string value, Type hint)
    {
      return byte.Parse(value, Configuration.CultureInfo.NumberFormat);
    }
    
    public override object TryConvertFromString(string value, Type hint)
    {
      if (!byte.TryParse(value, NumberStyles.Integer, Configuration.CultureInfo.NumberFormat, out var result))
        return null;
      return result;
    }
  }

  internal sealed class CharStringConverter : TypeStringConverter<char>
  {
    public override string ConvertToString(object value)
    {
      return value.ToString();
    }

    public override object ConvertFromString(string value, Type hint)
    {
      return char.Parse(value);
    }
    
    public override object TryConvertFromString(string value, Type hint)
    {
      if (!char.TryParse(value, out var result))
        return null;
      return result;
    }
  }

  internal sealed class DateTimeStringConverter : TypeStringConverter<DateTime>
  {
    public override string ConvertToString(object value)
    {
      return ((DateTime)value).ToString(Configuration.CultureInfo.DateTimeFormat);
    }

    public override object ConvertFromString(string value, Type hint)
    {
      return DateTime.Parse(value, Configuration.CultureInfo.DateTimeFormat);
    }
    
    public override object TryConvertFromString(string value, Type hint)
    {
      if (!DateTime.TryParse(value, Configuration.CultureInfo.DateTimeFormat, DateTimeStyles.None, out var result))
        return null;
      return result;
    }
  }

  internal sealed class DecimalStringConverter : TypeStringConverter<decimal>
  {
    public override string ConvertToString(object value)
    {
      return ((decimal)value).ToString(Configuration.CultureInfo.NumberFormat);
    }

    public override object ConvertFromString(string value, Type hint)
    {
      return decimal.Parse(value, Configuration.CultureInfo.NumberFormat);
    }
    
    public override object TryConvertFromString(string value, Type hint)
    {
      if (!decimal.TryParse(value, NumberStyles.Number, Configuration.CultureInfo.NumberFormat, out var result))
        return null;
      return result;
    }
  }

  internal sealed class DoubleStringConverter : TypeStringConverter<double>
  {
    public override string ConvertToString(object value)
    {
      return ((double)value).ToString(Configuration.CultureInfo.NumberFormat);
    }

    public override object ConvertFromString(string value, Type hint)
    {
      return double.Parse(value, Configuration.CultureInfo.NumberFormat);
    }
    
    public override object TryConvertFromString(string value, Type hint)
    {
      if (!double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, Configuration.CultureInfo.NumberFormat, out var result))
        return null;
      return result;
    }
  }

  internal sealed class EnumStringConverter : TypeStringConverter<Enum>
  {
    public override string ConvertToString(object value)
    {
      return value.ToString();
    }

    public override object ConvertFromString(string value, Type hint)
    {
      value = RemoveTypeNames(value);
      return Enum.Parse(hint, value);
    }
    
    public override object TryConvertFromString(string value, Type hint)
    {
      value = RemoveTypeNames(value);
      return Enum.IsDefined(hint, value) ? Enum.Parse(hint, value) : null;
    }

    /// <summary>
    /// Removes possible type names from a string value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks>
    /// It's possible that the value is something like:
    /// UriFormat.Unescaped
    /// We, and especially Enum.Parse do not want this format. Instead, it wants the clean name like:
    /// Unescaped
    /// </remarks>
    private static string RemoveTypeNames(string value)
    {
      var indexOfLastDot = value.LastIndexOf('.');
      if (indexOfLastDot >= 0)
        value = value.Substring(indexOfLastDot + 1, value.Length - indexOfLastDot - 1).Trim();
      return value;
    }
  }

  internal sealed class Int16StringConverter : TypeStringConverter<short>
  {
    public override string ConvertToString(object value)
    {
      return ((short)value).ToString(Configuration.CultureInfo.NumberFormat);
    }

    public override object ConvertFromString(string value, Type hint)
    {
      return short.Parse(value, Configuration.CultureInfo.NumberFormat);
    }
    
    public override object TryConvertFromString(string value, Type hint)
    {
      if (!short.TryParse(value, NumberStyles.Integer, Configuration.CultureInfo.NumberFormat, out var result))
        return null;
      return result;
    }
  }

  internal sealed class Int32StringConverter : TypeStringConverter<int>
  {
    public override string ConvertToString(object value)
    {
      return ((int)value).ToString(Configuration.CultureInfo.NumberFormat);
    }

    public override object ConvertFromString(string value, Type hint)
    {
      return int.Parse(value, Configuration.CultureInfo.NumberFormat);
    }
    
    public override object TryConvertFromString(string value, Type hint)
    {
      if (!int.TryParse(value, NumberStyles.Integer, Configuration.CultureInfo.NumberFormat, out var result))
        return null;
      return result;
    }
  }

  internal sealed class Int64StringConverter : TypeStringConverter<long>
  {
    public override string ConvertToString(object value)
    {
      return ((long)value).ToString(Configuration.CultureInfo.NumberFormat);
    }

    public override object ConvertFromString(string value, Type hint)
    {
      return long.Parse(value, Configuration.CultureInfo.NumberFormat);
    }
    
    public override object TryConvertFromString(string value, Type hint)
    {
      if (!long.TryParse(value, NumberStyles.Integer, Configuration.CultureInfo.NumberFormat, out var result))
        return null;
      return result;
    }
  }

  internal sealed class SByteStringConverter : TypeStringConverter<sbyte>
  {
    public override string ConvertToString(object value)
    {
      return ((sbyte)value).ToString(Configuration.CultureInfo.NumberFormat);
    }

    public override object ConvertFromString(string value, Type hint)
    {
      return sbyte.Parse(value, Configuration.CultureInfo.NumberFormat);
    }
    
    public override object TryConvertFromString(string value, Type hint)
    {
      if (!sbyte.TryParse(value, NumberStyles.Integer, Configuration.CultureInfo.NumberFormat, out var result))
        return null;
      return result;
    }
  }

  internal sealed class SingleStringConverter : TypeStringConverter<float>
  {
    public override string ConvertToString(object value)
    {
      return ((float)value).ToString(Configuration.CultureInfo.NumberFormat);
    }

    public override object ConvertFromString(string value, Type hint)
    {
      return float.Parse(value, Configuration.CultureInfo.NumberFormat);
    }
    
    public override object TryConvertFromString(string value, Type hint)
    {
      if (!float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, Configuration.CultureInfo.NumberFormat, out var result))
        return null;
      return result;
    }
  }

  internal sealed class StringStringConverter : TypeStringConverter<string>
  {
    public override string ConvertToString(object value)
    {
      return value.ToString().Trim('\"');
    }

    public override object ConvertFromString(string value, Type hint)
    {
      return value.Trim('\"');
    }
    
    public override object TryConvertFromString(string value, Type hint)
    {
      return ConvertFromString(value, hint);
    }
  }

  internal sealed class UInt16StringConverter : TypeStringConverter<ushort>
  {
    public override string ConvertToString(object value)
    {
      return ((ushort)value).ToString(Configuration.CultureInfo.NumberFormat);
    }

    public override object ConvertFromString(string value, Type hint)
    {
      return ushort.Parse(value, Configuration.CultureInfo.NumberFormat);
    }
    
    public override object TryConvertFromString(string value, Type hint)
    {
      if (!ushort.TryParse(value, NumberStyles.Integer, Configuration.CultureInfo.NumberFormat, out var result))
        return null;
      return result;
    }
  }

  internal sealed class UInt32StringConverter : TypeStringConverter<uint>
  {
    public override string ConvertToString(object value)
    {
      return ((uint)value).ToString(Configuration.CultureInfo.NumberFormat);
    }

    public override object ConvertFromString(string value, Type hint)
    {
      return uint.Parse(value, Configuration.CultureInfo.NumberFormat);
    }
    
    public override object TryConvertFromString(string value, Type hint)
    {
      if (!uint.TryParse(value, NumberStyles.Integer, Configuration.CultureInfo.NumberFormat, out var result))
        return null;
      return result;
    }
  }

  internal sealed class UInt64StringConverter : TypeStringConverter<ulong>
  {
    public override string ConvertToString(object value)
    {
      return ((ulong)value).ToString(Configuration.CultureInfo.NumberFormat);
    }

    public override object ConvertFromString(string value, Type hint)
    {
      return ulong.Parse(value, Configuration.CultureInfo.NumberFormat);
    }
    
    public override object TryConvertFromString(string value, Type hint)
    {
      if (!ulong.TryParse(value, NumberStyles.Integer, Configuration.CultureInfo.NumberFormat, out var result))
        return null;
      return result;
    }
  }
}
