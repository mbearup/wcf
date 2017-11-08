// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.WebHttpSecurity
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.ComponentModel;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
  /// <summary>Specifies the types of security available to a service endpoint configured to receive HTTP requests.</summary>
  public sealed class WebHttpSecurity
  {
    internal const WebHttpSecurityMode DefaultMode = WebHttpSecurityMode.None;
    private WebHttpSecurityMode mode;
    private HttpTransportSecurity transportSecurity;
    private bool isModeSet;

    /// <summary>Gets or sets the mode of security that is used by an endpoint configured to receive HTTP requests with a <see cref="T:System.ServiceModel.WebHttpBinding" />.</summary>
    /// <returns>A value of the <see cref="T:System.ServiceModel.WebHttpSecurityMode" /> that indicates whether transport-level security or no security is used by an endpoint. The default value is <see cref="F:System.ServiceModel.WebHttpSecurityMode.None" />.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The value is not a valid <see cref="T:System.ServiceModel.WebHttpSecurityMode" />.</exception>
    public WebHttpSecurityMode Mode
    {
      get
      {
        return this.mode;
      }
      set
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("WebHttpSecurityModeHelper is not implemented in .NET Core");
#else
        if (!WebHttpSecurityModeHelper.IsDefined(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.mode = value;
        this.isModeSet = true;
#endif
      }
    }

    internal bool IsModeSet
    {
      get
      {
        return this.isModeSet;
      }
    }

    /// <summary>Gets an object that contains the transport-level security settings for this binding.</summary>
    /// <returns>The <see cref="T:System.ServiceModel.HttpTransportSecurity" /> for this binding. The default values set are a <see cref="P:System.ServiceModel.HttpTransportSecurity.ClientCredentialType" /> of <see cref="F:System.ServiceModel.HttpClientCredentialType.None" />, a <see cref="P:System.ServiceModel.HttpTransportSecurity.ProxyCredentialType" /> of <see cref="F:System.ServiceModel.HttpProxyCredentialType.None" />, and <see cref="P:System.ServiceModel.HttpTransportSecurity.Realm" /> = "".</returns>
    public HttpTransportSecurity Transport
    {
      get
      {
        return this.transportSecurity;
      }
      set
      {
        this.transportSecurity = value == null ? new HttpTransportSecurity() : value;
      }
    }

    /// <summary>Creates a new instance of the <see cref="T:System.ServiceModel.WebHttpSecurity" /> class.</summary>
    public WebHttpSecurity()
    {
      this.transportSecurity = new HttpTransportSecurity();
    }

    internal void DisableTransportAuthentication(HttpTransportBindingElement http)
    {
      this.transportSecurity.DisableTransportAuthentication(http);
    }

    internal void EnableTransportAuthentication(HttpTransportBindingElement http)
    {
      this.transportSecurity.ConfigureTransportAuthentication(http);
    }

    internal void EnableTransportSecurity(HttpsTransportBindingElement https)
    {
      this.transportSecurity.ConfigureTransportProtectionAndAuthentication(https);
    }

    internal bool InternalShouldSerialize()
    {
      if (!this.ShouldSerializeMode())
        return this.ShouldSerializeTransport();
      return true;
    }

    /// <summary>Specifies whether the <see cref="P:System.ServiceModel.WebHttpSecurity.Mode" /> property has changed from its default and should be serialized. This is used for XAML integration.</summary>
    /// <returns>true if the <see cref="P:System.ServiceModel.WebHttpSecurity.Mode" /> property value should be serialized; otherwise, false.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeMode()
    {
      return (uint) this.Mode > 0U;
    }

    /// <summary>Returns a value that indicates whether the Transport property has changed from its default value and should be serialized. This is used by WCF for XAML integration.</summary>
    /// <returns>true if the <see cref="P:System.ServiceModel.WebHttpSecurity.Transport" /> property value should be serialized; otherwise, false.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeTransport()
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("HttpTransportSecurity.InternalShouldSerialize is not supported in .NET Core");
#else
      return this.Transport.InternalShouldSerialize();
#endif
    }
  }
}
