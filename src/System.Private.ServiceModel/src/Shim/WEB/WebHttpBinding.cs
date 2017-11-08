// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.WebHttpBinding
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.ComponentModel;
using System.Configuration;
using System.ServiceModel.Channels;
#if !FEATURE_CORECLR
using System.ServiceModel.Configuration;
#endif
using System.Text;
using System.Xml;

namespace System.ServiceModel
{
  /// <summary>A binding used to configure endpoints for Windows Communication Foundation (WCF) Web services that are exposed through HTTP requests instead of SOAP messages.</summary>
  public class WebHttpBinding : Binding, IBindingRuntimePreferences
  {
    private WebHttpSecurity security = new WebHttpSecurity();
    private HttpsTransportBindingElement httpsTransportBindingElement;
    private HttpTransportBindingElement httpTransportBindingElement;
    private WebMessageEncodingBindingElement webMessageEncodingBindingElement;

    /// <summary>Gets or sets a value that indicates whether the client accepts cookies and propagates them on future requests.</summary>
    /// <returns>true if cookies are allowed; otherwise, false. The default is false.</returns>
    [DefaultValue(false)]
    public bool AllowCookies
    {
      get
      {
        return this.httpTransportBindingElement.AllowCookies;
      }
      set
      {
        this.httpTransportBindingElement.AllowCookies = value;
        this.httpsTransportBindingElement.AllowCookies = value;
      }
    }

    /// <summary>Gets or sets a value that indicates whether to bypass the proxy server for local addresses.</summary>
    /// <returns>true to bypass the proxy server for local addresses; otherwise, false. The default value is false.</returns>
    [DefaultValue(false)]
    public bool BypassProxyOnLocal
    {
      get
      {
        return this.httpTransportBindingElement.BypassProxyOnLocal;
      }
      set
      {
        this.httpTransportBindingElement.BypassProxyOnLocal = value;
        this.httpsTransportBindingElement.BypassProxyOnLocal = value;
      }
    }

    /// <summary>Gets the envelope version that is used by endpoints that are configured by this binding to receive HTTP requests.</summary>
    /// <returns>The <see cref="T:System.ServiceModel.EnvelopeVersion" /> with the <see cref="P:System.ServiceModel.EnvelopeVersion.None" /> property that is used with endpoints configured with this binding to receive HTTP requests. </returns>
    public EnvelopeVersion EnvelopeVersion
    {
      get
      {
        return EnvelopeVersion.None;
      }
    }

    /// <summary>Gets or sets a value that indicates whether the hostname is used to reach the service when matching the URI.</summary>
    /// <returns>The <see cref="P:System.ServiceModel.Configuration.WSDualHttpBindingElement.HostNameComparisonMode" /> value that indicates whether the hostname is used to reach the service when matching on the URI. The default value is <see cref="F:System.ServiceModel.HostNameComparisonMode.StrongWildcard" />, which ignores the hostname in the match.</returns>
    /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The value set is not a valid <see cref="P:System.ServiceModel.Configuration.WSDualHttpBindingElement.HostNameComparisonMode" /> value.</exception>
    [DefaultValue(HostNameComparisonMode.StrongWildcard)]
    public HostNameComparisonMode HostNameComparisonMode
    {
      get
      {
        return this.httpTransportBindingElement.HostNameComparisonMode;
      }
      set
      {
        this.httpTransportBindingElement.HostNameComparisonMode = value;
        this.httpsTransportBindingElement.HostNameComparisonMode = value;
      }
    }

    /// <summary>Gets or sets the maximum amount of memory allocated, in bytes, for the buffer manager that manages the buffers required by endpoints that use this binding.</summary>
    /// <returns>The maximum size, in bytes, for the pool of buffers used by an endpoint configured with this binding. The default value is 65,536 bytes.</returns>
    [DefaultValue(524288)]
    public long MaxBufferPoolSize
    {
      get
      {
        return this.httpTransportBindingElement.MaxBufferPoolSize;
      }
      set
      {
        this.httpTransportBindingElement.MaxBufferPoolSize = value;
        this.httpsTransportBindingElement.MaxBufferPoolSize = value;
      }
    }

    /// <summary>Gets or sets the maximum amount of memory, in bytes, that is allocated for use by the manager of the message buffers that receive messages from the channel.</summary>
    /// <returns>The maximum amount of memory, in bytes, available for use by the message buffer manager. The default value is 524,288 (0x80000) bytes.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The value set is less than or equal to zero.</exception>
    [DefaultValue(65536)]
    public int MaxBufferSize
    {
      get
      {
        return this.httpTransportBindingElement.MaxBufferSize;
      }
      set
      {
        this.httpTransportBindingElement.MaxBufferSize = value;
        this.httpsTransportBindingElement.MaxBufferSize = value;
      }
    }

    /// <summary>Gets or sets the maximum size, in bytes, for a message that can be processed by the binding.</summary>
    /// <returns>The maximum size, in bytes, for a message that is processed by the binding. The default value is 65,536 bytes.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The value is less than zero.</exception>
    /// <exception cref="T:System.ServiceModel.QuotaExceededException">A message exceeded the maximum size allocated.</exception>
    [DefaultValue(65536)]
    public long MaxReceivedMessageSize
    {
      get
      {
        return this.httpTransportBindingElement.MaxReceivedMessageSize;
      }
      set
      {
        this.httpTransportBindingElement.MaxReceivedMessageSize = value;
        this.httpsTransportBindingElement.MaxReceivedMessageSize = value;
      }
    }

    /// <summary>Gets or sets the URI address of the HTTP proxy.</summary>
    /// <returns>A <see cref="T:System.Uri" /> that serves as the address of the HTTP proxy. The default value is null.</returns>
    [DefaultValue(null)]
    public Uri ProxyAddress
    {
      get
      {
        return this.httpTransportBindingElement.ProxyAddress;
      }
      set
      {
        this.httpTransportBindingElement.ProxyAddress = value;
        this.httpsTransportBindingElement.ProxyAddress = value;
      }
    }

    /// <summary>Gets or sets constraints on the complexity of SOAP messages that can be processed by endpoints configured with this binding.</summary>
    /// <returns>The <see cref="T:System.Xml.XmlDictionaryReaderQuotas" /> that specifies the complexity constraints.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value set is null.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The quota values of <see cref="T:System.Xml.XmlDictionaryReaderQuotas" /> are read only.</exception>
    /// <exception cref="T:System.ArgumentException">The quotas set must be positive.</exception>
    public XmlDictionaryReaderQuotas ReaderQuotas
    {
      get
      {
        return this.webMessageEncodingBindingElement.ReaderQuotas;
      }
      set
      {
        if (value == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
        value.CopyTo(this.webMessageEncodingBindingElement.ReaderQuotas);
      }
    }

    /// <summary>Gets the URI transport scheme for the channels and listeners that are configured with this binding.</summary>
    /// <returns>"https" if the <see cref="P:System.ServiceModel.WebHttpBinding.Security" /> is set to <see cref="F:System.ServiceModel.WebHttpSecurityMode.Transport" />; "http" if it is set to <see cref="F:System.ServiceModel.WebHttpSecurityMode.None" />.</returns>
    public override string Scheme
    {
      get
      {
        return this.GetTransport().Scheme;
      }
    }

    /// <summary>Gets the security settings used with this binding.  </summary>
    /// <returns>The <see cref="T:System.ServiceModel.WebHttpSecurity" /> that is used with this binding. The default value is <see cref="F:System.ServiceModel.WebHttpSecurityMode.None" />. </returns>
    public WebHttpSecurity Security
    {
      get
      {
        return this.security;
      }
      set
      {
        if (value == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
        this.security = value;
      }
    }

    /// <summary>Gets or sets a value that indicates whether the service configured with the binding uses streamed or buffered (or both) modes of message transfer.</summary>
    /// <returns>The <see cref="T:System.ServiceModel.TransferMode" /> value that indicates whether the service configured with the binding uses streamed or buffered (or both) modes of message transfer. The default value is <see cref="F:System.ServiceModel.TransferMode.Buffered" />.</returns>
    /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The value set is not a valid <see cref="T:System.ServiceModel.TransferMode" /> value.</exception>
    [DefaultValue(TransferMode.Buffered)]
    public TransferMode TransferMode
    {
      get
      {
        return this.httpTransportBindingElement.TransferMode;
      }
      set
      {
        this.httpTransportBindingElement.TransferMode = value;
        this.httpsTransportBindingElement.TransferMode = value;
      }
    }

    /// <summary>Gets or sets a value that indicates whether the auto-configured HTTP proxy of the system should be used, if available.</summary>
    /// <returns>true if the auto-configured HTTP proxy of the system should be used, if available; otherwise, false. The default value is true.  </returns>
    [DefaultValue(true)]
    public bool UseDefaultWebProxy
    {
      get
      {
        return this.httpTransportBindingElement.UseDefaultWebProxy;
      }
      set
      {
        this.httpTransportBindingElement.UseDefaultWebProxy = value;
        this.httpsTransportBindingElement.UseDefaultWebProxy = value;
      }
    }

    /// <summary>Gets or sets the character encoding that is used for the message text.</summary>
    /// <returns>The <see cref="T:System.Text.Encoding" /> that indicates the character encoding that is used. The default is <see cref="T:System.Text.UTF8Encoding" />.</returns>
    /// <exception cref="T:System.ArgumentNullException">The value set is null.</exception>
#if !FEATURE_CORECLR
    [TypeConverter(typeof (EncodingConverter))]
#endif
    public Encoding WriteEncoding
    {
      get
      {
        return this.webMessageEncodingBindingElement.WriteEncoding;
      }
      set
      {
        this.webMessageEncodingBindingElement.WriteEncoding = value;
      }
    }

    /// <summary>Gets or sets the content type mapper.</summary>
    /// <returns>The content type mapper.</returns>
    public WebContentTypeMapper ContentTypeMapper
    {
      get
      {
        return this.webMessageEncodingBindingElement.ContentTypeMapper;
      }
      set
      {
        this.webMessageEncodingBindingElement.ContentTypeMapper = value;
      }
    }

    /// <summary>Gets or sets a value that determines if cross domain script access is enabled.</summary>
    /// <returns>true if cross domain scripting is enabled; otherwise false.</returns>
    public bool CrossDomainScriptAccessEnabled
    {
      get
      {
        return this.webMessageEncodingBindingElement.CrossDomainScriptAccessEnabled;
      }
      set
      {
        this.webMessageEncodingBindingElement.CrossDomainScriptAccessEnabled = value;
      }
    }

    bool IBindingRuntimePreferences.ReceiveSynchronously
    {
      get
      {
        return false;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.WebHttpBinding" /> class. </summary>
    public WebHttpBinding()
    {
      this.Initialize();
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.WebHttpBinding" /> class with a binding specified by its configuration name.</summary>
    /// <param name="configurationName">The binding configuration name for the <see cref="T:System.ServiceModel.Configuration.WebHttpBindingElement" />.</param>
    /// <exception cref="T:System.Configuration.ConfigurationErrorsException">The binding element with the name <paramref name="configurationName" /> was not found.</exception>
    public WebHttpBinding(string configurationName)
      : this()
    {
      this.ApplyConfiguration(configurationName);
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.WebHttpBinding" /> class with the type of security used by the binding explicitly specified.</summary>
    /// <param name="securityMode">The value of <see cref="T:System.ServiceModel.WebHttpSecurityMode" /> that specifies the type of security that is used to configure a service endpoint to receive HTTP requests.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="securityMode" /> specified is not a valid <see cref="T:System.ServiceModel.WebHttpSecurityMode" />.</exception>
    public WebHttpBinding(WebHttpSecurityMode securityMode)
    {
      this.Initialize();
      this.security.Mode = securityMode;
    }

    /// <summary>Builds the channel factory stack on the client that creates a specified type of channel and that satisfies the features specified by a collection of binding parameters.</summary>
    /// <param name="parameters">The <see cref="T:System.ServiceModel.Channels.BindingParameterCollection" /> that specifies requirements for the channel factory built.</param>
    /// <typeparam name="TChannel">The type of channel the channel factory produces.</typeparam>
    /// <returns>An <see cref="T:System.ServiceModel.Channels.IChannelFactory`1" /> of type TChannel that satisfies the features specified by the collection.</returns>
    public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingParameterCollection parameters)
    {
      if ((this.security.Mode == WebHttpSecurityMode.Transport || this.security.Mode == WebHttpSecurityMode.TransportCredentialOnly) && this.security.Transport.ClientCredentialType == HttpClientCredentialType.InheritedFromHost)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("HttpClientCredentialTypeInvalid", new object[1]
        {
          (object) this.security.Transport.ClientCredentialType
        })));
      return base.BuildChannelFactory<TChannel>(parameters);
    }

    /// <summary>Returns an ordered collection of binding elements contained in the current binding.</summary>
    /// <returns>A <see cref="T:System.ServiceModel.Channels.BindingElementCollection" /> that contains the <see cref="T:System.ServiceModel.Channels.BindingElement" /> objects for the binding.</returns>
    public override BindingElementCollection CreateBindingElements()
    {
      BindingElementCollection elementCollection = new BindingElementCollection();
      elementCollection.Add((BindingElement) this.webMessageEncodingBindingElement);
      elementCollection.Add((BindingElement) this.GetTransport());
      return elementCollection.Clone();
    }

    private void ApplyConfiguration(string configurationName)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("WebHttpBindingElement is not implemented in .NET Core");
#else
      WebHttpBindingElement binding = WebHttpBindingCollectionElement.GetBindingCollectionElement().Bindings[(object) configurationName];
      if (binding == null)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ConfigurationErrorsException(SR2.GetString(SR2.ConfigInvalidBindingConfigurationName, new object[2]
        {
          (object) configurationName,
          (object) "webHttpBinding"
        })));
      }
      binding.ApplyConfiguration((Binding) this);
#endif
    }

    private TransportBindingElement GetTransport()
    {
      if (this.security.Mode == WebHttpSecurityMode.Transport)
      {
        this.security.EnableTransportSecurity(this.httpsTransportBindingElement);
        return (TransportBindingElement) this.httpsTransportBindingElement;
      }
      if (this.security.Mode == WebHttpSecurityMode.TransportCredentialOnly)
      {
        this.security.EnableTransportAuthentication(this.httpTransportBindingElement);
        return (TransportBindingElement) this.httpTransportBindingElement;
      }
      this.security.DisableTransportAuthentication(this.httpTransportBindingElement);
      return (TransportBindingElement) this.httpTransportBindingElement;
    }

    private void Initialize()
    {
      this.httpTransportBindingElement = new HttpTransportBindingElement();
      this.httpsTransportBindingElement = new HttpsTransportBindingElement();
      this.httpTransportBindingElement.ManualAddressing = true;
      this.httpsTransportBindingElement.ManualAddressing = true;
      this.webMessageEncodingBindingElement = new WebMessageEncodingBindingElement();
      this.webMessageEncodingBindingElement.MessageVersion = MessageVersion.None;
    }

    /// <summary>Determines if reader quotas should be serialized.</summary>
    /// <returns>true if reader quotas should be serialized; otherwise false.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeReaderQuotas()
    {
      return !EncoderDefaults.IsDefaultReaderQuotas(this.ReaderQuotas);
    }

    /// <summary>Determines if the encoding used for serialization should be serialized.</summary>
    /// <returns>true if the encoding should be serialized; otherwise false.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeWriteEncoding()
    {
      return this.WriteEncoding != TextEncoderDefaults.Encoding;
    }

    /// <summary>Determines if security settings should be serialized.</summary>
    /// <returns>true if security settings should be serialized; otherwise false.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeSecurity()
    {
      return this.Security.InternalShouldSerialize();
    }

    internal static class WebHttpBindingConfigurationStrings
    {
      internal const string WebHttpBindingCollectionElementName = "webHttpBinding";
    }
  }
}
