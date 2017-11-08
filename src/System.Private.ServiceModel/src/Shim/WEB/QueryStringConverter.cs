// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Dispatcher.QueryStringConverter
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Xml;

namespace System.ServiceModel.Dispatcher
{
  /// <summary>This class converts a parameter in a query string to an object of the appropriate type. It can also convert a parameter from an object to its query string representation. </summary>
  public class QueryStringConverter
  {
    private Hashtable defaultSupportedQueryStringTypes;
    private Hashtable typeConverterCache;

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Dispatcher.QueryStringConverter" /> class.</summary>
    public QueryStringConverter()
    {
      this.defaultSupportedQueryStringTypes = new Hashtable();
      this.defaultSupportedQueryStringTypes.Add((object) typeof (byte), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (sbyte), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (short), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (int), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (long), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (ushort), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (uint), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (ulong), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (float), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (double), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (bool), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (char), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (Decimal), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (string), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (object), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (DateTime), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (TimeSpan), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (byte[]), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (Guid), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (Uri), (object) null);
      this.defaultSupportedQueryStringTypes.Add((object) typeof (DateTimeOffset), (object) null);
      this.typeConverterCache = new Hashtable();
    }

    /// <summary>Determines whether the specified type can be converted to and from a string representation.</summary>
    /// <param name="type">The <see cref="T:System.Type" /> to convert.</param>
    /// <returns>A value that specifies whether the type can be converted.</returns>
    public virtual bool CanConvert(Type type)
    {
      if (this.defaultSupportedQueryStringTypes.ContainsKey((object) type) || typeof (Enum).IsAssignableFrom(type))
        return true;
      return this.GetStringConverter(type) != null;
    }

    /// <summary>Converts a query string parameter to the specified type.</summary>
    /// <param name="parameter">The string form of the parameter and value.</param>
    /// <param name="parameterType">The <see cref="T:System.Type" /> to convert the parameter to.</param>
    /// <returns>The converted parameter.</returns>
    /// <exception cref="T:System.FormatException">The provided string does not have the correct format.</exception>
    public virtual object ConvertStringToValue(string parameter, Type parameterType)
    {
      if (parameterType == (Type) null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameterType");
      switch (Type.GetTypeCode(parameterType))
      {
        case TypeCode.Boolean:
#if FEATURE_CORECLR
          throw new NotImplementedException("Cannot convert int to bool");
#else
          return (object) (bool) (parameter == null ? 0 : (Convert.ToBoolean(parameter, (IFormatProvider) CultureInfo.InvariantCulture) ? 1 : 0));
#endif
        case TypeCode.Char:
          return (object) (char) (parameter == null ? 0 : (int) XmlConvert.ToChar(parameter));
        case TypeCode.SByte:
          return (object) (sbyte) (parameter == null ? 0 : (int) XmlConvert.ToSByte(parameter));
        case TypeCode.Byte:
          return (object) (byte) (parameter == null ? 0 : (int) XmlConvert.ToByte(parameter));
        case TypeCode.Int16:
          return (object) (short) (parameter == null ? 0 : (int) XmlConvert.ToInt16(parameter));
        case TypeCode.UInt16:
          return (object) (ushort) (parameter == null ? 0 : (int) XmlConvert.ToUInt16(parameter));
        case TypeCode.Int32:
          if (typeof (Enum).IsAssignableFrom(parameterType))
            return Enum.Parse(parameterType, parameter, true);
          return (object) (parameter == null ? 0 : XmlConvert.ToInt32(parameter));
        case TypeCode.UInt32:
          return (object) (uint) (parameter == null ? 0 : (int) XmlConvert.ToUInt32(parameter));
        case TypeCode.Int64:
          return (object) (parameter == null ? 0L : XmlConvert.ToInt64(parameter));
        case TypeCode.UInt64:
          return (object) (ulong) (parameter == null ? 0L : (long) XmlConvert.ToUInt64(parameter));
        case TypeCode.Single:
          return (object) (float) (parameter == null ? 0.0 : (double) XmlConvert.ToSingle(parameter));
        case TypeCode.Double:
          return (object) (parameter == null ? 0.0 : XmlConvert.ToDouble(parameter));
        case TypeCode.Decimal:
          return (object) (parameter == null ? Decimal.Zero : XmlConvert.ToDecimal(parameter));
        case TypeCode.DateTime:
          return (object) (parameter == null ? new DateTime() : DateTime.Parse(parameter, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));
        case TypeCode.String:
          return (object) parameter;
        default:
          if (parameterType == typeof (TimeSpan))
          {
            TimeSpan result;
            if (!TimeSpan.TryParse(parameter, out result))
              result = parameter == null ? new TimeSpan() : XmlConvert.ToTimeSpan(parameter);
            return (object) result;
          }
          if (parameterType == typeof (Guid))
            return (object) (parameter == null ? new Guid() : XmlConvert.ToGuid(parameter));
          if (parameterType == typeof (DateTimeOffset))
            return (object) (parameter == null ? new DateTimeOffset() : DateTimeOffset.Parse(parameter, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind));
          if (parameterType == typeof (byte[]))
          {
            if (string.IsNullOrEmpty(parameter))
              return (object) new byte[0];
            return (object) Convert.FromBase64String(parameter);
          }
          if (parameterType == typeof (Uri))
          {
            if (string.IsNullOrEmpty(parameter))
              return (object) null;
            return (object) new Uri(parameter, UriKind.RelativeOrAbsolute);
          }
          if (parameterType == typeof (object))
            return (object) parameter;
          TypeConverter stringConverter = this.GetStringConverter(parameterType);
          if (stringConverter == null)
          {
            // ISSUE: reference to a compiler-generated method
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR2.GetString(SR2.TypeNotSupportedByQueryStringConverter, new object[2]
            {
              (object) parameterType.ToString(),
              (object) this.GetType().Name
            })));
          }
          return stringConverter.ConvertFromInvariantString(parameter);
      }
    }

    /// <summary>Converts a parameter to a query string representation.</summary>
    /// <param name="parameter">The parameter to convert.</param>
    /// <param name="parameterType">The <see cref="T:System.Type" /> of the parameter to convert.</param>
    /// <returns>The parameter name and value.</returns>
    public virtual string ConvertValueToString(object parameter, Type parameterType)
    {
      if (parameterType == (Type) null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameterType");
      if (parameterType.IsValueType && parameter == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameter");
      switch (Type.GetTypeCode(parameterType))
      {
        case TypeCode.Boolean:
          return XmlConvert.ToString((bool) parameter);
        case TypeCode.Char:
          return XmlConvert.ToString((char) parameter);
        case TypeCode.SByte:
          return XmlConvert.ToString((sbyte) parameter);
        case TypeCode.Byte:
          return XmlConvert.ToString((byte) parameter);
        case TypeCode.Int16:
          return XmlConvert.ToString((short) parameter);
        case TypeCode.UInt16:
          return XmlConvert.ToString((ushort) parameter);
        case TypeCode.Int32:
          if (typeof (Enum).IsAssignableFrom(parameterType))
            return Enum.Format(parameterType, parameter, "G");
          return XmlConvert.ToString((int) parameter);
        case TypeCode.UInt32:
          return XmlConvert.ToString((uint) parameter);
        case TypeCode.Int64:
          return XmlConvert.ToString((long) parameter);
        case TypeCode.UInt64:
          return XmlConvert.ToString((ulong) parameter);
        case TypeCode.Single:
          return XmlConvert.ToString((float) parameter);
        case TypeCode.Double:
          return XmlConvert.ToString((double) parameter);
        case TypeCode.Decimal:
          return XmlConvert.ToString((Decimal) parameter);
        case TypeCode.DateTime:
          return XmlConvert.ToString((DateTime) parameter, XmlDateTimeSerializationMode.RoundtripKind);
        case TypeCode.String:
          return (string) parameter;
        default:
          if (parameterType == typeof (TimeSpan))
            return XmlConvert.ToString((TimeSpan) parameter);
          if (parameterType == typeof (Guid))
            return XmlConvert.ToString((Guid) parameter);
          if (parameterType == typeof (DateTimeOffset))
            return XmlConvert.ToString((DateTimeOffset) parameter);
          if (parameterType == typeof (byte[]))
          {
            if (parameter == null)
              return (string) null;
            return Convert.ToBase64String((byte[]) parameter, Base64FormattingOptions.None);
          }
          if (parameterType == typeof (Uri) || parameterType == typeof (object))
          {
            if (parameter == null)
              return (string) null;
            return Convert.ToString(parameter, (IFormatProvider) CultureInfo.InvariantCulture);
          }
          TypeConverter stringConverter = this.GetStringConverter(parameterType);
          if (stringConverter == null)
          {
            // ISSUE: reference to a compiler-generated method
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR2.GetString(SR2.TypeNotSupportedByQueryStringConverter, new object[2]
            {
              (object) parameterType.ToString(),
              (object) this.GetType().Name
            })));
          }
          return stringConverter.ConvertToInvariantString(parameter);
      }
    }

    private TypeConverter GetStringConverter(Type parameterType)
    {
      if (this.typeConverterCache.ContainsKey((object) parameterType))
        return (TypeConverter) this.typeConverterCache[(object) parameterType];
      TypeConverterAttribute[] customAttributes = parameterType.GetCustomAttributes(typeof (TypeConverterAttribute), true) as TypeConverterAttribute[];
      if (customAttributes != null)
      {
        foreach (TypeConverterAttribute converterAttribute in customAttributes)
        {
          Type type = Type.GetType(converterAttribute.ConverterTypeName, false, true);
          if (type != (Type) null)
          {
            TypeConverter typeConverter = (TypeConverter) null;
            Exception exception = (Exception) null;
            try
            {
              typeConverter = (TypeConverter) Activator.CreateInstance(type);
            }
            catch (TargetInvocationException ex)
            {
              exception = (Exception) ex;
            }
            catch (MemberAccessException ex)
            {
              exception = (Exception) ex;
            }
            catch (TypeLoadException ex)
            {
              exception = (Exception) ex;
            }
            catch (COMException ex)
            {
              exception = (Exception) ex;
            }
            catch (InvalidComObjectException ex)
            {
              exception = (Exception) ex;
            }
            finally
            {
              if (exception != null)
              {
                if (Fx.IsFatal(exception))
                  throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
                DiagnosticUtility.TraceHandledException(exception, TraceEventType.Warning);
              }
            }
            if (typeConverter != null && typeConverter.CanConvertTo(typeof (string)) && typeConverter.CanConvertFrom(typeof (string)))
            {
              this.typeConverterCache.Add((object) parameterType, (object) typeConverter);
              return typeConverter;
            }
          }
        }
      }
      return (TypeConverter) null;
    }
  }
}
