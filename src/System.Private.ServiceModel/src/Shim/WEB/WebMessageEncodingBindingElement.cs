// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.WebMessageEncodingBindingElement
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.ServiceModel.Administration;
using System.ServiceModel.Description;
using System.Text;
using System.Xml;

namespace System.ServiceModel.Channels
{
  /// <summary>Enables plain-text XML, JavaScript Object Notation (JSON) message encodings and "raw" binary content to be read and written when used in a Windows Communication Foundation (WCF) binding.</summary>
  public sealed class WebMessageEncodingBindingElement : MessageEncodingBindingElement, IWsdlExportExtension, IWmiInstanceProvider
  {
    private WebContentTypeMapper contentTypeMapper;
    private int maxReadPoolSize;
    private int maxWritePoolSize;
    private XmlDictionaryReaderQuotas readerQuotas;
    private Encoding writeEncoding;

    /// <summary>Gets or sets how the content type of an incoming message is mapped to a format.</summary>
    /// <returns>The <see cref="T:System.ServiceModel.Channels.WebContentTypeMapper" /> that indicates the format for the content type of the incoming message.</returns>
    public WebContentTypeMapper ContentTypeMapper
    {
      get
      {
        return this.contentTypeMapper;
      }
      set
      {
        this.contentTypeMapper = value;
      }
    }

    /// <summary>Gets or sets a value that specifies the maximum number of readers that is allocated to a pool and that is available to process incoming messages without allocating new readers.</summary>
    /// <returns>The maximum number of readers available to process incoming messages. The default value is 64 readers of each type.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The value is less than or equal to zero.</exception>
    public int MaxReadPoolSize
    {
      get
      {
        return this.maxReadPoolSize;
      }
      set
      {
        if (value <= 0)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, SR2.GetString(SR2.ValueMustBePositive)));
        this.maxReadPoolSize = value;
      }
    }

    /// <summary>Gets or sets a value that specifies the maximum number of writers that is allocated to a pool and that is available to process outgoing messages without allocating new writers.</summary>
    /// <returns>The maximum number of writers available to process outgoing messages. The default is 16 writers of each type.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The value is less than or equal to zero.</exception>
    public int MaxWritePoolSize
    {
      get
      {
        return this.maxWritePoolSize;
      }
      set
      {
        if (value <= 0)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, SR2.GetString(SR2.ValueMustBePositive)));
        this.maxWritePoolSize = value;
      }
    }

    /// <summary>Gets or sets the message version that indicates that the binding element does not use SOAP or WS-Addressing.</summary>
    /// <returns>
    ///   <see cref="P:System.ServiceModel.Channels.MessageVersion.None" />
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">The value set is null.</exception>
    /// <exception cref="T:System.ArgumentException">The value set is neither null nor <see cref="P:System.ServiceModel.Channels.MessageVersion.None" />.</exception>
    public override MessageVersion MessageVersion
    {
      get
      {
        return MessageVersion.None;
      }
      set
      {
        if (value == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
        if (value != MessageVersion.None)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", SR2.GetString(SR2.JsonOnlySupportsMessageVersionNone));
      }
    }

    internal override bool IsWsdlExportable
    {
      get
      {
        return false;
      }
    }

    /// <summary>Gets constraints on the complexity of SOAP messages that can be processed by endpoints configured with this binding.</summary>
    /// <returns>The <see cref="T:System.Xml.XmlDictionaryReaderQuotas" /> that specifies the complexity constraints on SOAP messages that are exchanged. The default values for these constraints are provided in the following Remarks section.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value set is null.</exception>
    public XmlDictionaryReaderQuotas ReaderQuotas
    {
      get
      {
        return this.readerQuotas;
      }
    }

    /// <summary>Gets or sets the character encoding that is used to write the message text.</summary>
    /// <returns>The <see cref="T:System.Text.Encoding" /> that indicates the character encoding that is used to write the message text. The default is <see cref="T:System.Text.UTF8Encoding" />.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value set is null.</exception>
    public Encoding WriteEncoding
    {
      get
      {
        return this.writeEncoding;
      }
      set
      {
        if (value == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
        TextEncoderDefaults.ValidateEncoding(value);
        this.writeEncoding = value;
      }
    }

    /// <summary>Gets or sets a value that determines if cross domain script access is enabled.</summary>
    /// <returns>true if cross domain script access is enabled; otherwise, false.</returns>
    public bool CrossDomainScriptAccessEnabled { get; set; }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Channels.WebMessageEncodingBindingElement" /> class. </summary>
    public WebMessageEncodingBindingElement()
      : this(TextEncoderDefaults.Encoding)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Channels.WebMessageEncodingBindingElement" /> class with a specified write character encoding. </summary>
    /// <param name="writeEncoding">The <see cref="T:System.Text.Encoding" /> to be used to write characters in a message.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="writeEncoding" /> is null.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="writeEncoding" /> is not a supported message text encoding.</exception>
    public WebMessageEncodingBindingElement(Encoding writeEncoding)
    {
      if (writeEncoding == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writeEncoding");
      TextEncoderDefaults.ValidateEncoding(writeEncoding);
      this.maxReadPoolSize = 64;
      this.maxWritePoolSize = 16;
      this.readerQuotas = new XmlDictionaryReaderQuotas();
      EncoderDefaults.ReaderQuotas.CopyTo(this.readerQuotas);
      this.writeEncoding = writeEncoding;
    }

    private WebMessageEncodingBindingElement(WebMessageEncodingBindingElement elementToBeCloned)
      : base((MessageEncodingBindingElement) elementToBeCloned)
    {
      this.maxReadPoolSize = elementToBeCloned.maxReadPoolSize;
      this.maxWritePoolSize = elementToBeCloned.maxWritePoolSize;
      this.readerQuotas = new XmlDictionaryReaderQuotas();
      elementToBeCloned.readerQuotas.CopyTo(this.readerQuotas);
      this.writeEncoding = elementToBeCloned.writeEncoding;
      this.contentTypeMapper = elementToBeCloned.contentTypeMapper;
      this.CrossDomainScriptAccessEnabled = elementToBeCloned.CrossDomainScriptAccessEnabled;
    }

    /// <summary>Builds the channel factory stack on the client that creates a specified type of channel for a specified context.</summary>
    /// <param name="context">The <see cref="T:System.ServiceModel.Channels.BindingContext" /> for the channel.</param>
    /// <typeparam name="TChannel">The type of channel the channel factory produces.</typeparam>
    /// <returns>An <see cref="T:System.ServiceModel.Channels.IChannelFactory`1" /> of type <paramref name="TChannel" /> for the specified context.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value set is null.</exception>
    public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
    {
      return this.InternalBuildChannelFactory<TChannel>(context);
    }

    /// <summary>Builds the channel listener stack on the client that accepts a specified type of channel for a specified context.</summary>
    /// <param name="context">The <see cref="T:System.ServiceModel.Channels.BindingContext" /> for the listener.</param>
    /// <typeparam name="TChannel">The type of channel the channel listener accepts.</typeparam>
    /// <returns>An <see cref="T:System.ServiceModel.Channels.IChannelListener`1" /> of type <paramref name="TChannel" /> for the specified context.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value set is null.</exception>
    public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("InternalBuildChannelListener is not supported in .NET Core");
#else
      return this.InternalBuildChannelListener<TChannel>(context);
#endif
    }

    /// <summary>Returns a value that indicates whether the current binding can build a listener for a specified type of channel and context.</summary>
    /// <param name="context">The <see cref="T:System.ServiceModel.Channels.BindingContext" /> for the listener.</param>
    /// <typeparam name="TChannel">The type of channel the channel listener accepts.</typeparam>
    /// <returns>true if the specified channel listener stack can be built on the service; otherwise, false.</returns>
    public override bool CanBuildChannelListener<TChannel>(BindingContext context)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("InternalCanBuildChannelListener is not supported in .NET Core");
#else
      return this.InternalCanBuildChannelListener<TChannel>(context);
#endif
    }

    /// <summary>Creates a new <see cref="T:System.ServiceModel.Channels.WebMessageEncodingBindingElement" /> object initialized from the current one.</summary>
    /// <returns>A <see cref="T:System.ServiceModel.Channels.WebMessageEncodingBindingElement" /> object with property values equal to those of the current element.</returns>
    public override BindingElement Clone()
    {
      return (BindingElement) new WebMessageEncodingBindingElement(this);
    }

    /// <summary>Creates a message encoder factory that produces message encoders that can write either JavaScript Object Notation (JSON) or XML messages.</summary>
    /// <returns>The <see cref="T:System.ServiceModel.Channels.MessageEncoderFactory" /> that encodes JSON, XML or "raw" binary messages.</returns>
    public override MessageEncoderFactory CreateMessageEncoderFactory()
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("WebMessageEncoderFactory is not supported in .NET Core");
#else
      return (MessageEncoderFactory) new WebMessageEncoderFactory(this.WriteEncoding, this.MaxReadPoolSize, this.MaxWritePoolSize, this.ReaderQuotas, this.ContentTypeMapper, this.CrossDomainScriptAccessEnabled);
#endif
    }

    /// <summary>Returns the object of the type requested, if present, from the appropriate layer in the channel stack, or null if it is not present.</summary>
    /// <param name="context">The <see cref="T:System.ServiceModel.Channels.BindingContext" /> for the current binding element.</param>
    /// <typeparam name="T">The typed object for which the method is querying.</typeparam>
    /// <returns>The typed object <paramref name="T" /> requested if it is present or null if it is not.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="context" /> set is null.</exception>
    public override T GetProperty<T>(BindingContext context)
    {
      if (context == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
      if (typeof (T) == typeof (XmlDictionaryReaderQuotas))
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("Cannot convert XmlDictionaryReaderQuotas to 'T'");
#else
        return (T) this.readerQuotas;
#endif
      }
      return base.GetProperty<T>(context);
    }

    void IWmiInstanceProvider.FillInstance(IWmiInstance wmiInstance)
    {
      wmiInstance.SetProperty("MessageVersion", (object) this.MessageVersion.ToString());
      wmiInstance.SetProperty("Encoding", (object) this.writeEncoding.WebName);
      wmiInstance.SetProperty("MaxReadPoolSize", (object) this.maxReadPoolSize);
      wmiInstance.SetProperty("MaxWritePoolSize", (object) this.maxWritePoolSize);
      if (this.ReaderQuotas == null)
        return;
      IWmiInstance wmiInstance1 = wmiInstance.NewInstance("XmlDictionaryReaderQuotas");
      wmiInstance1.SetProperty("MaxArrayLength", (object) this.readerQuotas.MaxArrayLength);
      wmiInstance1.SetProperty("MaxBytesPerRead", (object) this.readerQuotas.MaxBytesPerRead);
      wmiInstance1.SetProperty("MaxDepth", (object) this.readerQuotas.MaxDepth);
      wmiInstance1.SetProperty("MaxNameTableCharCount", (object) this.readerQuotas.MaxNameTableCharCount);
      wmiInstance1.SetProperty("MaxStringContentLength", (object) this.readerQuotas.MaxStringContentLength);
      wmiInstance.SetProperty("ReaderQuotas", (object) wmiInstance1);
    }

    string IWmiInstanceProvider.GetInstanceType()
    {
      return typeof (WebMessageEncodingBindingElement).Name;
    }

    void IWsdlExportExtension.ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
    {
    }

    void IWsdlExportExtension.ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
    {
      if (context == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
#if FEATURE_CORECLR
      throw new NotImplementedException("SoapHelper is not supported in .NET Core");
#else
      SoapHelper.SetSoapVersion(context, exporter, this.MessageVersion.Envelope);
#endif
    }

    internal override bool CheckEncodingVersion(EnvelopeVersion version)
    {
      return this.MessageVersion.Envelope == version;
    }

#if FEATURE_CORECLR
    public override bool IsMatch(BindingElement b)
#else
    internal override bool IsMatch(BindingElement b)
#endif
    {
      if (!base.IsMatch(b))
        return false;
      WebMessageEncodingBindingElement encodingBindingElement = b as WebMessageEncodingBindingElement;
      return encodingBindingElement != null && this.maxReadPoolSize == encodingBindingElement.MaxReadPoolSize && (this.maxWritePoolSize == encodingBindingElement.MaxWritePoolSize && this.readerQuotas.MaxStringContentLength == encodingBindingElement.ReaderQuotas.MaxStringContentLength) && (this.readerQuotas.MaxArrayLength == encodingBindingElement.ReaderQuotas.MaxArrayLength && this.readerQuotas.MaxBytesPerRead == encodingBindingElement.ReaderQuotas.MaxBytesPerRead && (this.readerQuotas.MaxDepth == encodingBindingElement.ReaderQuotas.MaxDepth && this.readerQuotas.MaxNameTableCharCount == encodingBindingElement.ReaderQuotas.MaxNameTableCharCount)) && (!(this.WriteEncoding.EncodingName != encodingBindingElement.WriteEncoding.EncodingName) && this.MessageVersion.IsMatch(encodingBindingElement.MessageVersion) && this.ContentTypeMapper == encodingBindingElement.ContentTypeMapper);
    }
  }
}
