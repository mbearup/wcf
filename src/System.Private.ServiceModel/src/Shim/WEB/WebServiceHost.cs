// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Web.WebServiceHost
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
#if !FEATURE_CORECLR
using System.ServiceModel.Activation;
using System.ServiceModel.Configuration;
using System.ServiceModel.Web.Configuration;
#endif

namespace System.ServiceModel.Web
{
  /// <summary>A <see cref="T:System.ServiceModel.ServiceHost" /> derived class that compliments the Windows Communication Foundation (WCF) REST programming model.</summary>
#if FEATURE_CORECLR
  public class WebServiceHost
#else
  public class WebServiceHost : ServiceHost
#endif
  {

#if !FEATURE_CORECLR
    private static readonly System.Type WebHttpBindingType = typeof (WebHttpBinding);
    private static readonly string WebHttpEndpointKind = "webHttpEndpoint";

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Web.WebServiceHost" /> class.</summary>
    public WebServiceHost()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Web.WebServiceHost" /> class with the specified singleton server instance and base address.</summary>
    /// <param name="singletonInstance">A service instance to be used as the singleton instance.</param>
    /// <param name="baseAddresses">The base address of the service.</param>
    public WebServiceHost(object singletonInstance, params Uri[] baseAddresses)
      : base(singletonInstance, baseAddresses)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Web.WebServiceHost" /> class with the specified service type and base address.</summary>
    /// <param name="serviceType">The service type.</param>
    /// <param name="baseAddresses">The base address of the service.</param>
    public WebServiceHost(System.Type serviceType, params Uri[] baseAddresses)
      : base(serviceType, baseAddresses)
    {
    }

    internal static void AddAutomaticWebHttpBindingEndpoints(ServiceHost host, IDictionary<string, ContractDescription> implementedContracts, string multipleContractsErrorMessage, string noContractErrorMessage, string standardEndpointKind)
    {
      bool endpointsCompatibility = AppSettings.EnableAutomaticEndpointsCompatibility;
      if (host.Description.Endpoints != null && host.Description.Endpoints.Count > 0 && !endpointsCompatibility)
        return;
      AuthenticationSchemes supportedSchemes = AuthenticationSchemes.None;
      if (host.BaseAddresses.Count > 0)
      {
        supportedSchemes = AspNetEnvironment.Current.GetAuthenticationSchemes(host.BaseAddresses[0]);
        if (AspNetEnvironment.Current.IsSimpleApplicationHost && supportedSchemes == (AuthenticationSchemes.Ntlm | AuthenticationSchemes.Anonymous))
          supportedSchemes = !AspNetEnvironment.Current.IsWindowsAuthenticationConfigured() ? AuthenticationSchemes.Anonymous : AuthenticationSchemes.Ntlm;
      }
      System.Type type = (System.Type) null;
      foreach (Uri baseAddress in host.BaseAddresses)
      {
        string scheme = baseAddress.Scheme;
        if ((object) scheme == (object) Uri.UriSchemeHttp || (object) scheme == (object) Uri.UriSchemeHttps)
        {
          bool flag1 = false;
          foreach (ServiceEndpoint endpoint in (Collection<ServiceEndpoint>) host.Description.Endpoints)
          {
            if (endpoint.Address != (EndpointAddress) null && EndpointAddress.UriEquals(endpoint.Address.Uri, baseAddress, true, false))
            {
              flag1 = true;
              break;
            }
          }
          if (!flag1)
          {
            if (type == (System.Type) null)
            {
              if (implementedContracts.Count > 1)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(multipleContractsErrorMessage));
              if (implementedContracts.Count == 0)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(noContractErrorMessage));
              using (IEnumerator<ContractDescription> enumerator = implementedContracts.Values.GetEnumerator())
              {
                if (enumerator.MoveNext())
                  type = enumerator.Current.ContractType;
              }
            }
            ConfigLoader configLoader = new ConfigLoader(host.GetContractResolver(implementedContracts));
            ServiceEndpointElement serviceEndpointElement = new ServiceEndpointElement();
            serviceEndpointElement.Contract = type.FullName;
            ProtocolMappingItem protocolMappingItem = ConfigLoader.LookupProtocolMapping(baseAddress.Scheme);
            if (protocolMappingItem != null && string.Equals(protocolMappingItem.Binding, "webHttpBinding", StringComparison.Ordinal))
              serviceEndpointElement.BindingConfiguration = protocolMappingItem.BindingConfiguration;
            serviceEndpointElement.Kind = standardEndpointKind;
            ServiceEndpoint serviceEndpoint = configLoader.LookupEndpoint(serviceEndpointElement, (ContextInformation) null, (ServiceHostBase) host, host.Description, true);
            WebHttpBinding binding = serviceEndpoint.Binding as WebHttpBinding;
            bool flag2 = !binding.Security.IsModeSet;
            if (flag2)
              binding.Security.Mode = (object) scheme != (object) Uri.UriSchemeHttps ? (supportedSchemes == AuthenticationSchemes.None || supportedSchemes == AuthenticationSchemes.Anonymous ? WebHttpSecurityMode.None : WebHttpSecurityMode.TransportCredentialOnly) : WebHttpSecurityMode.Transport;
            if (flag2 && AspNetEnvironment.Enabled)
              WebServiceHost.SetBindingCredentialBasedOnHostedEnvironment(serviceEndpoint, supportedSchemes);
            ConfigLoader.ConfigureEndpointAddress(serviceEndpointElement, (ServiceHostBase) host, serviceEndpoint);
            ConfigLoader.ConfigureEndpointListenUri(serviceEndpointElement, (ServiceHostBase) host, serviceEndpoint);
            host.AddServiceEndpoint(serviceEndpoint);
          }
        }
      }
    }
#endif

    internal static void SetRawContentTypeMapperIfNecessary(ServiceEndpoint endpoint, bool isDispatch)
    {
      Binding binding = endpoint.Binding;
      ContractDescription contract = endpoint.Contract;
      if (binding == null)
        return;
      CustomBinding customBinding = new CustomBinding(binding);
      WebMessageEncodingBindingElement encodingBindingElement = customBinding.Elements.Find<WebMessageEncodingBindingElement>();
      if (encodingBindingElement == null || encodingBindingElement.ContentTypeMapper != null)
        return;
      bool flag = true;
      int numStreamOperations = 0;
      foreach (OperationDescription operation in (Collection<OperationDescription>) contract.Operations)
      {
        if (!(isDispatch ? WebServiceHost.IsRawContentMapperCompatibleDispatchOperation(operation, ref numStreamOperations) : WebServiceHost.IsRawContentMapperCompatibleClientOperation(operation, ref numStreamOperations)))
        {
          flag = false;
          break;
        }
      }
      if (!flag || numStreamOperations <= 0)
        return;
#if FEATURE_CORECLR
      throw new NotImplementedException("RawContentTypeMapper is not implemented in .NET Core");
#else
      encodingBindingElement.ContentTypeMapper = (WebContentTypeMapper) RawContentTypeMapper.Instance;
      endpoint.Binding = (Binding) customBinding;
#endif
    }

#if !FEATURE_CORECLR
    /// <summary>Called when the <see cref="T:System.ServiceModel.Web.WebServiceHost" /> instance opens.</summary>
    protected override void OnOpening()
    {
      if (this.Description == null)
        return;
      ServiceDebugBehavior serviceDebugBehavior = this.Description.Behaviors.Find<ServiceDebugBehavior>();
      if (serviceDebugBehavior != null)
      {
        serviceDebugBehavior.HttpHelpPageEnabled = false;
        serviceDebugBehavior.HttpsHelpPageEnabled = false;
      }
      ServiceMetadataBehavior metadataBehavior = this.Description.Behaviors.Find<ServiceMetadataBehavior>();
      if (metadataBehavior != null)
      {
        metadataBehavior.HttpGetEnabled = false;
        metadataBehavior.HttpsGetEnabled = false;
      }
      // ISSUE: reference to a compiler-generated method
      // ISSUE: reference to a compiler-generated method
      WebServiceHost.AddAutomaticWebHttpBindingEndpoints((ServiceHost) this, this.ImplementedContracts, SR2.GetString(SR2.HttpTransferServiceHostMultipleContracts, (object) this.Description.Name), SR2.GetString(SR2.HttpTransferServiceHostNoContract, (object) this.Description.Name), WebServiceHost.WebHttpEndpointKind);
      foreach (ServiceEndpoint endpoint in (Collection<ServiceEndpoint>) this.Description.Endpoints)
      {
        if (endpoint.Binding != null && endpoint.Binding.CreateBindingElements().Find<WebMessageEncodingBindingElement>() != null)
        {
          WebServiceHost.SetRawContentTypeMapperIfNecessary(endpoint, true);
          if (endpoint.Behaviors.Find<WebHttpBehavior>() == null)
          {
            ConfigLoader.LoadDefaultEndpointBehaviors(endpoint);
            if (endpoint.Behaviors.Find<WebHttpBehavior>() == null)
              endpoint.Behaviors.Add((IEndpointBehavior) new WebHttpBehavior());
          }
        }
      }
      base.OnOpening();
    }
#endif

    private static bool IsRawContentMapperCompatibleClientOperation(OperationDescription operation, ref int numStreamOperations)
    {
      return !(operation.Messages.Count > 1 & !WebServiceHost.IsResponseStreamOrVoid(operation, ref numStreamOperations));
    }

    private static bool IsRawContentMapperCompatibleDispatchOperation(OperationDescription operation, ref int numStreamOperations)
    {
      UriTemplateDispatchFormatter throwAway = new UriTemplateDispatchFormatter(operation, (IDispatchMessageFormatter) null, new QueryStringConverter(), operation.DeclaringContract.Name, new Uri("http://localhost"));
      int num = throwAway.pathMapping.Count + throwAway.queryMapping.Count;
      bool isRequestCompatible = false;
      if (num > 0)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("WebHttpBehavior.HideRequestUriTemplateParameters is not implemented in .NET Core");
#else
        int tmp = 0;
        WebHttpBehavior.HideRequestUriTemplateParameters(operation, throwAway, (WebHttpBehavior.Effect) (() => isRequestCompatible = WebServiceHost.IsRequestStreamOrVoid(operation, ref tmp)));
        numStreamOperations = numStreamOperations + tmp;
#endif
      }
      else
        isRequestCompatible = WebServiceHost.IsRequestStreamOrVoid(operation, ref numStreamOperations);
      return isRequestCompatible;
    }

    private static bool IsRequestStreamOrVoid(OperationDescription operation, ref int numStreamOperations)
    {
      MessageDescription message = operation.Messages[0];
      if (WebHttpBehavior.IsTypedMessage(message) || WebHttpBehavior.IsUntypedMessage(message))
        return false;
      if (message.Body.Parts.Count == 0)
        return true;
      if (message.Body.Parts.Count == 1)
      {
        if (WebServiceHost.IsStreamPart(message.Body.Parts[0].Type))
        {
          numStreamOperations = numStreamOperations + 1;
          return true;
        }
        if (WebServiceHost.IsVoidPart(message.Body.Parts[0].Type))
          return true;
      }
      return false;
    }

    private static bool IsResponseStreamOrVoid(OperationDescription operation, ref int numStreamOperations)
    {
      if (operation.Messages.Count <= 1)
        return true;
      MessageDescription message = operation.Messages[1];
      if (WebHttpBehavior.IsTypedMessage(message) || WebHttpBehavior.IsUntypedMessage(message) || message.Body.Parts.Count != 0)
        return false;
      if (message.Body.ReturnValue == null || WebServiceHost.IsVoidPart(message.Body.ReturnValue.Type))
        return true;
      if (!WebServiceHost.IsStreamPart(message.Body.ReturnValue.Type))
        return false;
      numStreamOperations = numStreamOperations + 1;
      return true;
    }

    private static bool IsStreamPart(System.Type type)
    {
      return type == typeof (Stream);
    }

    private static bool IsVoidPart(System.Type type)
    {
      if (!(type == (System.Type) null))
        return type == typeof (void);
      return true;
    }

#if !FEATURE_CORECLR
    private static void SetBindingCredentialBasedOnHostedEnvironment(ServiceEndpoint serviceEndpoint, AuthenticationSchemes supportedSchemes)
    {
      WebHttpBinding binding = serviceEndpoint.Binding as WebHttpBinding;
      switch (supportedSchemes)
      {
        case AuthenticationSchemes.Digest:
          binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Digest;
          break;
        case AuthenticationSchemes.Negotiate:
        case AuthenticationSchemes.IntegratedWindowsAuthentication:
          binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
          break;
        case AuthenticationSchemes.Ntlm:
          binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
          break;
        case AuthenticationSchemes.Basic:
          binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
          break;
        case AuthenticationSchemes.Anonymous:
          binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
          break;
        default:
          binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.InheritedFromHost;
          break;
      }
    }
#endif
  }
}
