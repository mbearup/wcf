// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.WebBodyFormatMessageProperty
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.Globalization;

namespace System.ServiceModel.Channels
{
  /// <summary>Stores and retrieves the message encoding format of incoming and outgoing messages for the composite Web message encoder.</summary>
  public sealed class WebBodyFormatMessageProperty : IMessageProperty
  {
    private WebContentFormat format;
    private static WebBodyFormatMessageProperty jsonProperty;
    /// <summary>Returns the name of the property.</summary>
    /// <returns>Returns: "WebBodyFormatMessageProperty".</returns>
    public const string Name = "WebBodyFormatMessageProperty";
    private static WebBodyFormatMessageProperty xmlProperty;
    private static WebBodyFormatMessageProperty rawProperty;

    /// <summary>Gets the format used for the message body.</summary>
    /// <returns>The <see cref="T:System.ServiceModel.Channels.WebContentFormat" /> that specifies the format used for the message body.</returns>
    public WebContentFormat Format
    {
      get
      {
        return this.format;
      }
    }

    internal static WebBodyFormatMessageProperty JsonProperty
    {
      get
      {
        if (WebBodyFormatMessageProperty.jsonProperty == null)
          WebBodyFormatMessageProperty.jsonProperty = new WebBodyFormatMessageProperty(WebContentFormat.Json);
        return WebBodyFormatMessageProperty.jsonProperty;
      }
    }

    internal static WebBodyFormatMessageProperty XmlProperty
    {
      get
      {
        if (WebBodyFormatMessageProperty.xmlProperty == null)
          WebBodyFormatMessageProperty.xmlProperty = new WebBodyFormatMessageProperty(WebContentFormat.Xml);
        return WebBodyFormatMessageProperty.xmlProperty;
      }
    }

    internal static WebBodyFormatMessageProperty RawProperty
    {
      get
      {
        if (WebBodyFormatMessageProperty.rawProperty == null)
          WebBodyFormatMessageProperty.rawProperty = new WebBodyFormatMessageProperty(WebContentFormat.Raw);
        return WebBodyFormatMessageProperty.rawProperty;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Channels.WebBodyFormatMessageProperty" /> class with a specified format.</summary>
    /// <param name="format">The <see cref="T:System.ServiceModel.Channels.WebContentFormat" /> of the message body.</param>
    /// <exception cref="T:System.ArgumentException">The format cannot be set to the <see cref="F:System.ServiceModel.Channels.WebContentFormat.Default" /> value in the constructor.</exception>
    public WebBodyFormatMessageProperty(WebContentFormat format)
    {
      if (format == WebContentFormat.Default)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR2.GetString(SR2.DefaultContentFormatNotAllowedInProperty)));
      }
      this.format = format;
    }

    /// <summary>Returns the current instance of the current property.</summary>
    /// <returns>An instance of the <see cref="T:System.ServiceModel.Channels.IMessageProperty" /> interface that is a copy of the current <see cref="T:System.ServiceModel.Channels.WebBodyFormatMessageProperty" />.</returns>
    public IMessageProperty CreateCopy()
    {
      return (IMessageProperty) this;
    }

    /// <summary>Returns the name of the property and the encoding format used when constructed.</summary>
    /// <returns>Returns "WebBodyFormatMessageProperty: EncodingFormat={0}", where {0} is WebContentFormat.ToString(), which specifies the encoding format used.</returns>
    public override string ToString()
    {
      // ISSUE: reference to a compiler-generated method
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR2.GetString(SR2.WebBodyFormatPropertyToString, (object) this.Format.ToString()), new object[0]);
    }
  }
}
