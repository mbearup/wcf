// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Web.WebChannelFactory`1
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace System.ServiceModel.Web
{
  /// <summary>A class for accessing Windows Communication Foundation (WCF) Web services on a client.</summary>
  /// <typeparam name="TChannel">The type of channel to create.</typeparam>
  public class WebChannelFactory<TChannel> : ChannelFactory<TChannel> where TChannel : class
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Web.WebChannelFactory`1" /> class.</summary>
    public WebChannelFactory()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Web.WebChannelFactory`1" /> class.</summary>
    /// <param name="binding">The binding to use when creating the channel.</param>
    public WebChannelFactory(Binding binding)
      : base(binding)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Web.WebChannelFactory`1" /> class.</summary>
    /// <param name="endpoint">The endpoint to use when creating the channel.</param>
    public WebChannelFactory(ServiceEndpoint endpoint)
      : base(endpoint)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Web.WebChannelFactory`1" /> class.</summary>
    /// <param name="endpointConfigurationName">The name within the application configuration file where the channel is configured.</param>
    public WebChannelFactory(string endpointConfigurationName)
      : base(endpointConfigurationName)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Web.WebChannelFactory`1" /> class.</summary>
    /// <param name="channelType">The channel type to use.</param>
    public WebChannelFactory(System.Type channelType)
      : base(channelType)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Web.WebChannelFactory`1" /> class with the specified <see cref="T:System.Uri" />.</summary>
    /// <param name="remoteAddress">The URI of the Web service that is called.</param>
    public WebChannelFactory(Uri remoteAddress)
      : this(WebChannelFactory<TChannel>.GetDefaultBinding(remoteAddress), remoteAddress)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Web.WebChannelFactory`1" /> class with the specified binding and <see cref="T:System.Uri" />.</summary>
    /// <param name="binding">The binding to use.</param>
    /// <param name="remoteAddress">The URI of the Web service that is called.</param>
    public WebChannelFactory(Binding binding, Uri remoteAddress)
      : base(binding, remoteAddress != (Uri) null ? new EndpointAddress(remoteAddress, new AddressHeader[0]) : (EndpointAddress) null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Web.WebChannelFactory`1" /> class with the specified endpoint configuration and <see cref="T:System.Uri" />.</summary>
    /// <param name="endpointConfigurationName">The name within the application configuration file where the channel is configured.</param>
    /// <param name="remoteAddress">The URI of the Web service that is called.</param>
    public WebChannelFactory(string endpointConfigurationName, Uri remoteAddress)
      : base(endpointConfigurationName, remoteAddress != (Uri) null ? new EndpointAddress(remoteAddress, new AddressHeader[0]) : (EndpointAddress) null)
    {
    }

    /// <summary>This method is called when the <see cref="T:System.ServiceModel.Web.WebChannelFactory`1" /> is opened.</summary>
    protected override void OnOpening()
    {
      if (this.Endpoint == null)
        return;
      if (this.Endpoint.Binding == null && this.Endpoint.Address != (EndpointAddress) null)
        this.Endpoint.Binding = WebChannelFactory<TChannel>.GetDefaultBinding(this.Endpoint.Address.Uri);
#if FEATURE_CORECLR
      throw new NotImplementedException("WebServiceHost is not implemented in .NET Core");
#else
      WebServiceHost.SetRawContentTypeMapperIfNecessary(this.Endpoint, false);
      if (this.Endpoint.Behaviors.Find<WebHttpBehavior>() == null)
        this.Endpoint.Behaviors.Add((IEndpointBehavior) new WebHttpBehavior());
      base.OnOpening();
#endif
    }

    private static Binding GetDefaultBinding(Uri remoteAddress)
    {
      if (remoteAddress == (Uri) null || remoteAddress.Scheme != Uri.UriSchemeHttp && remoteAddress.Scheme != Uri.UriSchemeHttps)
        return (Binding) null;
      if (remoteAddress.Scheme == Uri.UriSchemeHttp)
        return (Binding) new WebHttpBinding();
      return (Binding) new WebHttpBinding()
      {
        Security = {
          Mode = WebHttpSecurityMode.Transport,
          Transport = {
            ClientCredentialType = HttpClientCredentialType.None
          }
        }
      };
    }
  }
}
