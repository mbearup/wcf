// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Description.WebHttpBehavior
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Administration;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

namespace System.ServiceModel.Description
{
  /// <summary>Enables the Web programming model for a Windows Communication Foundation (WCF) service.</summary>
  public class WebHttpBehavior : IEndpointBehavior, IWmiInstanceProvider
  {
    internal static readonly string defaultStreamContentType = "application/octet-stream";
    internal static readonly string defaultCallbackParameterName = "callback";
    internal const string GET = "GET";
    internal const string POST = "POST";
    internal const string WildcardAction = "*";
    internal const string WildcardMethod = "*";
    private const string AddressPropertyName = "Address";
#if !FEATURE_CORECLR
    private WebMessageBodyStyle defaultBodyStyle;
    private WebMessageFormat defaultOutgoingReplyFormat;
    private WebMessageFormat defaultOutgoingRequestFormat;
    private XmlSerializerOperationBehavior.Reflector reflector;
    private UnwrappedTypesXmlSerializerManager xmlSerializerManager;

    /// <summary>Gets and sets the default message body style.</summary>
    /// <returns>One of the values defined in the <see cref="T:System.ServiceModel.Web.WebMessageBodyStyle" /> enumeration.</returns>
    public virtual WebMessageBodyStyle DefaultBodyStyle
    {
      get
      {
        return this.defaultBodyStyle;
      }
      set
      {
        if (!WebMessageBodyStyleHelper.IsDefined(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.defaultBodyStyle = value;
      }
    }

    /// <summary>Gets and sets the default outgoing request format.</summary>
    /// <returns>One of the values defined in the <see cref="T:System.ServiceModel.Web.WebMessageFormat" /> enumeration.</returns>
    public virtual WebMessageFormat DefaultOutgoingRequestFormat
    {
      get
      {
        return this.defaultOutgoingRequestFormat;
      }
      set
      {
        if (!WebMessageFormatHelper.IsDefined(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.defaultOutgoingRequestFormat = value;
      }
    }

    /// <summary>Gets and sets the default outgoing response format.</summary>
    /// <returns>One of the values defined in the <see cref="T:System.ServiceModel.Web.WebMessageFormat" /> enumeration.</returns>
    public virtual WebMessageFormat DefaultOutgoingResponseFormat
    {
      get
      {
        return this.defaultOutgoingReplyFormat;
      }
      set
      {
        if (!WebMessageFormatHelper.IsDefined(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.defaultOutgoingReplyFormat = value;
      }
    }

    /// <summary>Gets or sets a value that determines if the WCF Help page is enabled.</summary>
    /// <returns>true if the WCFHelp page is enabled; otherwise false.</returns>
#endif
    public virtual bool HelpEnabled { get; set; }

    /// <summary>Gets or sets a value that determines if automatic format selection is enabled.</summary>
    /// <returns>true if automatic format selection is enabled; otherwise false.</returns>
    public virtual bool AutomaticFormatSelectionEnabled { get; set; }

    /// <summary>Gets or sets the flag that specifies whether a FaultException is generated when an internal server error (HTTP status code: 500) occurs.</summary>
    /// <returns>Returns true if the flag is enabled; otherwise returns false.</returns>
    public virtual bool FaultExceptionEnabled { get; set; }

    internal Uri HelpUri { get; set; }

    /// <summary>Gets or sets the JavaScript callback parameter name.</summary>
    /// <returns>The JavaScript callback parameter name.</returns>
    protected internal string JavascriptCallbackParameterName { get; set; }

#if !FEATURE_CORECLR
    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Description.WebHttpBehavior" /> class.</summary>
    public WebHttpBehavior()
    {
      this.defaultOutgoingRequestFormat = WebMessageFormat.Xml;
      this.defaultOutgoingReplyFormat = WebMessageFormat.Xml;
      this.defaultBodyStyle = WebMessageBodyStyle.Bare;
      this.xmlSerializerManager = new UnwrappedTypesXmlSerializerManager();
    }
#endif

    /// <summary>Implements the <see cref="M:System.ServiceModel.Description.IEndpointBehavior.AddBindingParameters(System.ServiceModel.Description.ServiceEndpoint,System.ServiceModel.Channels.BindingParameterCollection)" /> method to pass data at runtime to bindings to support custom behavior.</summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="bindingParameters">The binding parameters that support modifying the bindings.</param>
    public virtual void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
    }

    /// <summary>Implements the <see cref="M:System.ServiceModel.Description.IEndpointBehavior.ApplyClientBehavior(System.ServiceModel.Description.ServiceEndpoint,System.ServiceModel.Dispatcher.ClientRuntime)" /> method to support modification or extension of the client across an endpoint.</summary>
    /// <param name="endpoint">The endpoint that exposes the contract.</param>
    /// <param name="clientRuntime">The client to which the custom behavior is applied.</param>
    public virtual void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("WebHttpBehavior is not implemented in .NET Core");
#else
      if (endpoint == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpoint");
      if (clientRuntime == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("clientRuntime");
      WebMessageEncodingBindingElement encodingBindingElement = endpoint.Binding.CreateBindingElements().Find<WebMessageEncodingBindingElement>();
      if (encodingBindingElement != null && encodingBindingElement.CrossDomainScriptAccessEnabled)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR2.CrossDomainJavascriptNotsupported));
      this.reflector = new XmlSerializerOperationBehavior.Reflector(endpoint.Contract.Namespace, (System.Type) null);
      foreach (OperationDescription operation1 in (Collection<OperationDescription>) endpoint.Contract.Operations)
      {
        if (clientRuntime.Operations.Contains(operation1.Name))
        {
          ClientOperation operation2 = clientRuntime.Operations[operation1.Name];
          IClientMessageFormatter requestClientFormatter = this.GetRequestClientFormatter(operation1, endpoint);
          IClientMessageFormatter replyClientFormatter = this.GetReplyClientFormatter(operation1, endpoint);
          operation2.Formatter = (IClientMessageFormatter) new CompositeClientFormatter(requestClientFormatter, replyClientFormatter);
          operation2.SerializeRequest = true;
          operation2.DeserializeReply = operation1.Messages.Count > 1 && !WebHttpBehavior.IsUntypedMessage(operation1.Messages[1]);
        }
      }
      this.AddClientErrorInspector(endpoint, clientRuntime);
#endif
    }

    /// <summary>Implements the <see cref="M:System.ServiceModel.Description.IEndpointBehavior.ApplyDispatchBehavior(System.ServiceModel.Description.ServiceEndpoint,System.ServiceModel.Dispatcher.EndpointDispatcher)" /> method to support modification or extension of the client across an endpoint.</summary>
    /// <param name="endpoint">The endpoint that exposes the contract.</param>
    /// <param name="endpointDispatcher">The endpoint dispatcher to which the behavior is applied.</param>
    public virtual void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("WebHttpBehavior is not implemented in .NET Core");
#else
      if (endpoint == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpoint");
      if (endpointDispatcher == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpointDispatcher");
      WebMessageEncodingBindingElement encodingBindingElement = endpoint.Binding.CreateBindingElements().Find<WebMessageEncodingBindingElement>();
      if (encodingBindingElement != null && encodingBindingElement.CrossDomainScriptAccessEnabled)
      {
        if (endpoint.Binding.GetProperty<ISecurityCapabilities>(new BindingParameterCollection()).SupportsClientAuthentication)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR2.CrossDomainJavascriptAuthNotSupported));
        this.JavascriptCallbackParameterName = !endpoint.Contract.Behaviors.Contains(typeof (JavascriptCallbackBehaviorAttribute)) ? WebHttpBehavior.defaultCallbackParameterName : (endpoint.Contract.Behaviors[typeof (JavascriptCallbackBehaviorAttribute)] as JavascriptCallbackBehaviorAttribute).UrlParameterName;
        endpointDispatcher.DispatchRuntime.MessageInspectors.Add((IDispatchMessageInspector) new JavascriptCallbackMessageInspector(this.JavascriptCallbackParameterName));
      }
      if (this.HelpEnabled)
        this.HelpUri = new UriTemplate("help").BindByPosition(endpoint.ListenUri);
      this.reflector = new XmlSerializerOperationBehavior.Reflector(endpoint.Contract.Namespace, (System.Type) null);
      endpointDispatcher.AddressFilter = (MessageFilter) new PrefixEndpointAddressMessageFilter(endpoint.Address);
      endpointDispatcher.ContractFilter = (MessageFilter) new MatchAllMessageFilter();
      endpointDispatcher.DispatchRuntime.OperationSelector = (IDispatchOperationSelector) this.GetOperationSelector(endpoint);
      string str = (string) null;
      foreach (OperationDescription operation in (Collection<OperationDescription>) endpoint.Contract.Operations)
      {
        if (operation.Messages[0].Direction == MessageDirection.Input && operation.Messages[0].Action == "*")
        {
          str = operation.Name;
          break;
        }
      }
      if (str != null)
        endpointDispatcher.DispatchRuntime.Operations.Add(endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation);
      FormatSelectingMessageInspector messageInspector = (FormatSelectingMessageInspector) null;
      string contentType1;
      string contentType2;
      if (encodingBindingElement != null)
      {
        XmlFormatMapping xmlFormatMapping = new XmlFormatMapping(encodingBindingElement.WriteEncoding, encodingBindingElement.ContentTypeMapper);
        JsonFormatMapping jsonFormatMapping = new JsonFormatMapping(encodingBindingElement.WriteEncoding, encodingBindingElement.ContentTypeMapper);
        contentType1 = xmlFormatMapping.DefaultContentType.ToString();
        contentType2 = jsonFormatMapping.DefaultContentType.ToString();
        if (this.AutomaticFormatSelectionEnabled)
        {
          messageInspector = new FormatSelectingMessageInspector(this, new List<MultiplexingFormatMapping>()
          {
            (MultiplexingFormatMapping) xmlFormatMapping,
            (MultiplexingFormatMapping) jsonFormatMapping
          });
          endpointDispatcher.DispatchRuntime.MessageInspectors.Add((IDispatchMessageInspector) messageInspector);
        }
      }
      else
      {
        contentType1 = TextMessageEncoderFactory.GetContentType(XmlFormatMapping.defaultMediaType, TextEncoderDefaults.Encoding);
        contentType2 = JsonMessageEncoderFactory.GetContentType((WebMessageEncodingBindingElement) null);
      }
      endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation = new DispatchOperation(endpointDispatcher.DispatchRuntime, "*", "*", "*");
      endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation.DeserializeRequest = false;
      endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation.SerializeReply = false;
      endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation.Invoker = (IOperationInvoker) new HttpUnhandledOperationInvoker()
      {
        HelpUri = this.HelpUri
      };
      foreach (OperationDescription operation in (Collection<OperationDescription>) endpoint.Contract.Operations)
      {
        DispatchOperation dispatchOperation = (DispatchOperation) null;
        if (endpointDispatcher.DispatchRuntime.Operations.Contains(operation.Name))
          dispatchOperation = endpointDispatcher.DispatchRuntime.Operations[operation.Name];
        else if (endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation.Name == operation.Name)
          dispatchOperation = endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation;
        if (dispatchOperation != null)
        {
          IDispatchMessageFormatter dispatchFormatter1 = this.GetRequestDispatchFormatter(operation, endpoint);
          IDispatchMessageFormatter dispatchFormatter2 = this.GetReplyDispatchFormatter(operation, endpoint);
          MultiplexingDispatchMessageFormatter formatter = dispatchFormatter2 as MultiplexingDispatchMessageFormatter;
          if (formatter != null)
          {
            formatter.DefaultContentTypes.Add(WebMessageFormat.Xml, contentType1);
            formatter.DefaultContentTypes.Add(WebMessageFormat.Json, contentType2);
            if (messageInspector != null)
              messageInspector.RegisterOperation(operation.Name, formatter);
          }
          dispatchOperation.Formatter = (IDispatchMessageFormatter) new CompositeDispatchFormatter(dispatchFormatter1, dispatchFormatter2);
          dispatchOperation.FaultFormatter = (IDispatchFaultFormatter) new WebFaultFormatter(dispatchOperation.FaultFormatter);
          dispatchOperation.DeserializeRequest = dispatchFormatter1 != null;
          dispatchOperation.SerializeReply = operation.Messages.Count > 1 && dispatchFormatter2 != null;
        }
      }
      if (this.HelpEnabled)
      {
        HelpPage helpPage = new HelpPage(this, endpoint.Contract);
        DispatchOperation dispatchOperation1 = new DispatchOperation(endpointDispatcher.DispatchRuntime, "HelpPageInvoke", (string) null, (string) null);
        dispatchOperation1.DeserializeRequest = false;
        dispatchOperation1.SerializeReply = false;
        HelpOperationInvoker operationInvoker = new HelpOperationInvoker(helpPage, endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation.Invoker);
        dispatchOperation1.Invoker = (IOperationInvoker) operationInvoker;
        DispatchOperation dispatchOperation2 = dispatchOperation1;
        endpointDispatcher.DispatchRuntime.Operations.Add(dispatchOperation2);
      }
      this.AddServerErrorHandlers(endpoint, endpointDispatcher);
#endif
    }

#if !FEATURE_CORECLR
    internal virtual Dictionary<string, string> GetWmiProperties()
    {
      return new Dictionary<string, string>()
      {
        {
          "DefaultBodyStyle",
          this.DefaultBodyStyle.ToString()
        },
        {
          "DefaultOutgoingRequestFormat",
          this.DefaultOutgoingRequestFormat.ToString()
        },
        {
          "DefaultOutgoingResponseFormat",
          this.DefaultOutgoingResponseFormat.ToString()
        }
      };
    }

    internal virtual string GetWmiTypeName()
    {
      return "WebHttpBehavior";
    }
#endif

    void IWmiInstanceProvider.FillInstance(IWmiInstance wmiInstance)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("WebHttpBehavior is not implemented in .NET Core");
#else
      if (wmiInstance == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("wmiInstance");
      Dictionary<string, string> wmiProperties = this.GetWmiProperties();
      foreach (string key in wmiProperties.Keys)
        wmiInstance.SetProperty(key, (object) wmiProperties[key]);
#endif
    }

    string IWmiInstanceProvider.GetInstanceType()
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("WebHttpBehavior is not implemented in .NET Core");
#else
      return this.GetWmiTypeName();
#endif
    }

    /// <summary>Confirms that the endpoint meets the requirements for the Web programming model.</summary>
    /// <param name="endpoint">The service endpoint.</param>
    public virtual void Validate(ServiceEndpoint endpoint)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("WebHttpBehavior is not implemented in .NET Core");
#else
      if (endpoint == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpoint");
      this.ValidateNoMessageHeadersPresent(endpoint);
      this.ValidateBinding(endpoint);
      this.ValidateContract(endpoint);
#endif
    }

#if !FEATURE_CORECLR
    private void ValidateNoMessageHeadersPresent(ServiceEndpoint endpoint)
    {
      if (endpoint == null || endpoint.Address == (EndpointAddress) null)
        return;
      EndpointAddress address = endpoint.Address;
      if (address.Headers.Count > 0)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.WebHttpServiceEndpointCannotHaveMessageHeaders, (object) address)));
      }
    }

    /// <summary>Ensures the binding is valid for use with the WCF Web Programming Model.</summary>
    /// <param name="endpoint">The service endpoint.</param>
    protected virtual void ValidateBinding(ServiceEndpoint endpoint)
    {
      WebHttpBehavior.ValidateIsWebHttpBinding(endpoint, this.GetType().ToString());
    }
#endif

    internal static string GetWebMethod(OperationDescription od)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("WebHttpBehavior is not implemented in .NET Core");
#else
      WebGetAttribute wga = od.Behaviors.Find<WebGetAttribute>();
      WebInvokeAttribute wia = od.Behaviors.Find<WebInvokeAttribute>();
      WebHttpBehavior.EnsureOk(wga, wia, od);
      if (wga != null)
        return "GET";
      if (wia != null)
        return wia.Method ?? "POST";
      return "POST";
#endif
    }

    internal static string GetWebUriTemplate(OperationDescription od)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("WebHttpBehavior is not implemented in .NET Core");
#else
      WebGetAttribute wga = od.Behaviors.Find<WebGetAttribute>();
      WebInvokeAttribute wia = od.Behaviors.Find<WebInvokeAttribute>();
      WebHttpBehavior.EnsureOk(wga, wia, od);
      if (wga != null)
        return wga.UriTemplate;
      if (wia != null)
        return wia.UriTemplate;
      return (string) null;
#endif
    }

#if !FEATURE_CORECLR
    internal static string GetDescription(OperationDescription od)
    {
      object[] objArray = (object[]) null;
      if (od.SyncMethod != (MethodInfo) null)
        objArray = od.SyncMethod.GetCustomAttributes(typeof (DescriptionAttribute), true);
      else if (od.BeginMethod != (MethodInfo) null)
        objArray = od.BeginMethod.GetCustomAttributes(typeof (DescriptionAttribute), true);
      if (objArray != null && objArray.Length != 0)
        return ((DescriptionAttribute) objArray[0]).Description;
      return string.Empty;
    }

    internal static bool IsTypedMessage(MessageDescription message)
    {
      if (message != null)
        return message.MessageType != (System.Type) null;
      return false;
    }
#endif

    internal static bool IsUntypedMessage(MessageDescription message)
    {
      if (message == null)
        return false;
      if (message.Body.ReturnValue != null && message.Body.Parts.Count == 0 && message.Body.ReturnValue.Type == typeof (Message))
        return true;
      if (message.Body.ReturnValue == null && message.Body.Parts.Count == 1)
        return message.Body.Parts[0].Type == typeof (Message);
      return false;
    }

#if !FEATURE_CORECLR
    internal static MessageDescription MakeDummyMessageDescription(MessageDirection direction)
    {
      return new MessageDescription("urn:dummyAction", direction);
    }

    internal static bool SupportsJsonFormat(OperationDescription od)
    {
      return od.Behaviors.Find<DataContractSerializerOperationBehavior>() != null;
    }

    internal static void ValidateIsWebHttpBinding(ServiceEndpoint serviceEndpoint, string behaviorName)
    {
      Binding binding = serviceEndpoint.Binding;
      if (binding.Scheme != "http" && binding.Scheme != "https")
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.WCFBindingCannotBeUsedWithUriOperationSelectorBehaviorBadScheme, new object[2]
        {
          (object) serviceEndpoint.Contract.Name,
          (object) behaviorName
        })));
      }
      if (binding.MessageVersion != MessageVersion.None)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.WCFBindingCannotBeUsedWithUriOperationSelectorBehaviorBadMessageVersion, new object[2]
        {
          (object) serviceEndpoint.Address.Uri.AbsoluteUri,
          (object) behaviorName
        })));
      }
      TransportBindingElement transportBindingElement = binding.CreateBindingElements().Find<TransportBindingElement>();
      if (transportBindingElement != null && !transportBindingElement.ManualAddressing)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.ManualAddressingCannotBeFalseWithTransportBindingElement, (object) serviceEndpoint.Address.Uri.AbsoluteUri, (object) behaviorName, (object) transportBindingElement.GetType().Name)));
      }
    }

    internal WebMessageBodyStyle GetBodyStyle(OperationDescription od)
    {
      WebGetAttribute wga = od.Behaviors.Find<WebGetAttribute>();
      WebInvokeAttribute wia = od.Behaviors.Find<WebInvokeAttribute>();
      WebHttpBehavior.EnsureOk(wga, wia, od);
      if (wga != null)
        return wga.GetBodyStyleOrDefault(this.DefaultBodyStyle);
      if (wia != null)
        return wia.GetBodyStyleOrDefault(this.DefaultBodyStyle);
      return this.DefaultBodyStyle;
    }

    internal IClientMessageFormatter GetDefaultClientFormatter(OperationDescription od, bool useJson, bool isWrapped)
    {
      DataContractSerializerOperationBehavior dcsob = od.Behaviors.Find<DataContractSerializerOperationBehavior>();
      if (useJson)
      {
        if (dcsob == null)
        {
          // ISSUE: reference to a compiler-generated method
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.JsonFormatRequiresDataContract, (object) od.Name, (object) od.DeclaringContract.Name, (object) od.DeclaringContract.Namespace)));
        }
        return (IClientMessageFormatter) this.CreateDataContractJsonSerializerOperationFormatter(od, dcsob, isWrapped);
      }
      ClientOperation clientOperation = new ClientOperation(new ClientRuntime("name", ""), "dummyClient", "urn:dummy");
      clientOperation.Formatter = (IClientMessageFormatter) null;
      if (dcsob != null)
      {
        ((IOperationBehavior) dcsob).ApplyClientBehavior(od, clientOperation);
        return clientOperation.Formatter;
      }
      XmlSerializerOperationBehavior operationBehavior = od.Behaviors.Find<XmlSerializerOperationBehavior>();
      if (operationBehavior == null)
        return (IClientMessageFormatter) null;
      ((IOperationBehavior) new XmlSerializerOperationBehavior(od, operationBehavior.XmlSerializerFormatAttribute, this.reflector)).ApplyClientBehavior(od, clientOperation);
      return clientOperation.Formatter;
    }

    /// <summary>Adds a client error inspector to the specified service endpoint.</summary>
    /// <param name="endpoint">The service endpoint.</param>
    /// <param name="clientRuntime">The client runtime.</param>
    protected virtual void AddClientErrorInspector(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
      if (!this.FaultExceptionEnabled)
        clientRuntime.MessageInspectors.Add((IClientMessageInspector) new WebFaultClientMessageInspector());
      else
        clientRuntime.MessageVersionNoneFaultsEnabled = true;
    }

    /// <summary>Override this method to change the way errors that occur on the service are handled.</summary>
    /// <param name="endpoint">The service endpoint.</param>
    /// <param name="endpointDispatcher">The endpoint dispatcher.</param>
    protected virtual void AddServerErrorHandlers(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    {
      if (this.FaultExceptionEnabled)
        return;
      WebErrorHandler webErrorHandler = new WebErrorHandler(this, endpoint.Contract, endpointDispatcher.DispatchRuntime.ChannelDispatcher.IncludeExceptionDetailInFaults);
      endpointDispatcher.DispatchRuntime.ChannelDispatcher.ErrorHandlers.Add((IErrorHandler) webErrorHandler);
    }

    /// <summary>Creates a new <see cref="T:System.ServiceModel.Dispatcher.WebHttpDispatchOperationSelector" /> object.</summary>
    /// <param name="endpoint">The endpoint that exposes the contract.</param>
    /// <returns>An instance of <see cref="T:System.ServiceModel.Dispatcher.WebHttpDispatchOperationSelector" /> that contains the operation selector for the specified endpoint.</returns>
    protected virtual WebHttpDispatchOperationSelector GetOperationSelector(ServiceEndpoint endpoint)
    {
      return new WebHttpDispatchOperationSelector(endpoint);
    }

    /// <summary>Gets the query string converter.</summary>
    /// <param name="operationDescription">The service operation.</param>
    /// <returns>A <see cref="T:System.ServiceModel.Dispatcher.QueryStringConverter" /> instance.</returns>
    protected virtual QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
    {
      return new QueryStringConverter();
    }

    /// <summary>Gets the reply formatter on the client for the specified endpoint and service operation.</summary>
    /// <param name="operationDescription">The service operation.</param>
    /// <param name="endpoint">The service endpoint.</param>
    /// <returns>An <see cref="T:System.ServiceModel.Dispatcher.IClientMessageFormatter" /> reference to the reply formatter on the client for the specified operation and endpoint.</returns>
    protected virtual IClientMessageFormatter GetReplyClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
    {
      if (operationDescription.Messages.Count < 2)
        return (IClientMessageFormatter) null;
      this.ValidateBodyParameters(operationDescription, false);
      System.Type type;
      if (WebHttpBehavior.TryGetStreamParameterType(operationDescription.Messages[1], operationDescription, false, out type))
        return (IClientMessageFormatter) new HttpStreamFormatter(operationDescription);
      if (WebHttpBehavior.IsUntypedMessage(operationDescription.Messages[1]))
        return (IClientMessageFormatter) new WebHttpBehavior.MessagePassthroughFormatter();
      WebMessageBodyStyle bodyStyle = this.GetBodyStyle(operationDescription);
      System.Type parameterType;
      if (this.UseBareReplyFormatter(bodyStyle, operationDescription, this.GetResponseFormat(operationDescription), out parameterType))
        return SingleBodyParameterMessageFormatter.CreateXmlAndJsonClientFormatter(operationDescription, parameterType, false, this.xmlSerializerManager);
      MessageDescription message = operationDescription.Messages[0];
      operationDescription.Messages[0] = WebHttpBehavior.MakeDummyMessageDescription(MessageDirection.Input);
      IClientMessageFormatter jsonClientFormatter = this.GetDefaultXmlAndJsonClientFormatter(operationDescription, !WebHttpBehavior.IsBareResponse(bodyStyle));
      operationDescription.Messages[0] = message;
      return jsonClientFormatter;
    }

    internal virtual bool UseBareReplyFormatter(WebMessageBodyStyle style, OperationDescription operationDescription, WebMessageFormat responseFormat, out System.Type parameterType)
    {
      parameterType = (System.Type) null;
      if (WebHttpBehavior.IsBareResponse(style))
        return WebHttpBehavior.TryGetNonMessageParameterType(operationDescription.Messages[1], operationDescription, false, out parameterType);
      return false;
    }

    /// <summary>Gets the reply formatter on the service for the specified endpoint and service operation.</summary>
    /// <param name="operationDescription">The service operation.</param>
    /// <param name="endpoint">The service endpoint.</param>
    /// <returns>An <see cref="T:System.ServiceModel.Dispatcher.IDispatchMessageFormatter" /> reference to the reply formatter on the service for the specified operation and endpoint.</returns>
    protected virtual IDispatchMessageFormatter GetReplyDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
    {
      if (operationDescription.Messages.Count < 2)
        return (IDispatchMessageFormatter) null;
      this.ValidateBodyParameters(operationDescription, false);
      WebMessageFormat responseFormat = this.GetResponseFormat(operationDescription);
      bool flag = responseFormat == WebMessageFormat.Json || WebHttpBehavior.SupportsJsonFormat(operationDescription);
      System.Type type;
      IDispatchMessageFormatter messageFormatter;
      if (WebHttpBehavior.TryGetStreamParameterType(operationDescription.Messages[1], operationDescription, false, out type))
        messageFormatter = (IDispatchMessageFormatter) new ContentTypeSettingDispatchMessageFormatter(WebHttpBehavior.defaultStreamContentType, (IDispatchMessageFormatter) new HttpStreamFormatter(operationDescription));
      else if (WebHttpBehavior.IsUntypedMessage(operationDescription.Messages[1]))
      {
        messageFormatter = (IDispatchMessageFormatter) new WebHttpBehavior.MessagePassthroughFormatter();
      }
      else
      {
        WebMessageBodyStyle bodyStyle = this.GetBodyStyle(operationDescription);
        Dictionary<WebMessageFormat, IDispatchMessageFormatter> formatters = new Dictionary<WebMessageFormat, IDispatchMessageFormatter>();
        System.Type parameterType;
        if (this.UseBareReplyFormatter(bodyStyle, operationDescription, responseFormat, out parameterType))
        {
          formatters.Add(WebMessageFormat.Xml, SingleBodyParameterMessageFormatter.CreateDispatchFormatter(operationDescription, parameterType, false, false, this.xmlSerializerManager, (string) null));
          if (flag)
            formatters.Add(WebMessageFormat.Json, SingleBodyParameterMessageFormatter.CreateDispatchFormatter(operationDescription, parameterType, false, true, this.xmlSerializerManager, this.JavascriptCallbackParameterName));
        }
        else
        {
          MessageDescription message = operationDescription.Messages[0];
          operationDescription.Messages[0] = WebHttpBehavior.MakeDummyMessageDescription(MessageDirection.Input);
          formatters.Add(WebMessageFormat.Xml, this.GetDefaultDispatchFormatter(operationDescription, false, !WebHttpBehavior.IsBareResponse(bodyStyle)));
          if (flag)
            formatters.Add(WebMessageFormat.Json, this.GetDefaultDispatchFormatter(operationDescription, true, !WebHttpBehavior.IsBareResponse(bodyStyle)));
          operationDescription.Messages[0] = message;
        }
        messageFormatter = (IDispatchMessageFormatter) new MultiplexingDispatchMessageFormatter(formatters, responseFormat);
      }
      return messageFormatter;
    }

    /// <summary>Gets the request formatter on the client for the specified service operation and endpoint.</summary>
    /// <param name="operationDescription">The service operation.</param>
    /// <param name="endpoint">The service endpoint.</param>
    /// <returns>An <see cref="T:System.ServiceModel.Dispatcher.IClientMessageFormatter" /> reference to the request formatter on the client for the specified operation and endpoint.</returns>
    protected virtual IClientMessageFormatter GetRequestClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
    {
      bool useJson = this.GetRequestFormat(operationDescription) == WebMessageFormat.Json;
      WebMessageEncodingBindingElement webEncoding = useJson ? endpoint.Binding.CreateBindingElements().Find<WebMessageEncodingBindingElement>() : (WebMessageEncodingBindingElement) null;
      IClientMessageFormatter innerFormatter = (IClientMessageFormatter) null;
      if (endpoint.Address == (EndpointAddress) null)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.ServiceEndpointMustHaveNonNullAddress, (object) typeof (ServiceEndpoint), (object) typeof (ChannelFactory), (object) typeof (WebHttpEndpoint), (object) "Address", (object) typeof (ServiceEndpoint))));
      }
      UriTemplateClientFormatter throwAway = new UriTemplateClientFormatter(operationDescription, (IClientMessageFormatter) null, this.GetQueryStringConverter(operationDescription), endpoint.Address.Uri, false, endpoint.Contract.Name);
      int numUriVariables = throwAway.pathMapping.Count + throwAway.queryMapping.Count;
      bool isStream = false;
      WebHttpBehavior.HideReplyMessage(operationDescription, (WebHttpBehavior.Effect) (() =>
      {
        WebMessageBodyStyle style = this.GetBodyStyle(operationDescription);
        bool isUntypedWhenUriParamsNotConsidered = false;
        WebHttpBehavior.Effect doBodyFormatter = (WebHttpBehavior.Effect) (() =>
        {
          if (numUriVariables != 0)
            WebHttpBehavior.EnsureNotUntypedMessageNorMessageContract(operationDescription);
          this.ValidateBodyParameters(operationDescription, true);
          System.Type type;
          IClientMessageFormatter messageFormatter;
          if (WebHttpBehavior.TryGetStreamParameterType(operationDescription.Messages[0], operationDescription, true, out type))
          {
            isStream = true;
            messageFormatter = (IClientMessageFormatter) new HttpStreamFormatter(operationDescription);
          }
          else
            messageFormatter = !this.UseBareRequestFormatter(style, operationDescription, out type) ? this.GetDefaultClientFormatter(operationDescription, useJson, !WebHttpBehavior.IsBareRequest(style)) : SingleBodyParameterMessageFormatter.CreateClientFormatter(operationDescription, type, true, useJson, this.xmlSerializerManager);
          innerFormatter = messageFormatter;
          isUntypedWhenUriParamsNotConsidered = WebHttpBehavior.IsUntypedMessage(operationDescription.Messages[0]);
        });
        if (numUriVariables == 0)
        {
          if (WebHttpBehavior.IsUntypedMessage(operationDescription.Messages[0]))
          {
            this.ValidateBodyParameters(operationDescription, true);
            innerFormatter = (IClientMessageFormatter) new WebHttpBehavior.MessagePassthroughFormatter();
            isUntypedWhenUriParamsNotConsidered = true;
          }
          else if (WebHttpBehavior.IsTypedMessage(operationDescription.Messages[0]))
          {
            this.ValidateBodyParameters(operationDescription, true);
            innerFormatter = this.GetDefaultClientFormatter(operationDescription, useJson, !WebHttpBehavior.IsBareRequest(style));
          }
          else
            doBodyFormatter();
        }
        else
          WebHttpBehavior.HideRequestUriTemplateParameters(operationDescription, throwAway, (WebHttpBehavior.Effect) (() => WebHttpBehavior.CloneMessageDescriptionsBeforeActing(operationDescription, (WebHttpBehavior.Effect) (() => doBodyFormatter()))));
        innerFormatter = (IClientMessageFormatter) new UriTemplateClientFormatter(operationDescription, innerFormatter, this.GetQueryStringConverter(operationDescription), endpoint.Address.Uri, isUntypedWhenUriParamsNotConsidered, endpoint.Contract.Name);
      }));
      string defaultContentType = this.GetDefaultContentType(isStream, useJson, webEncoding);
      if (!string.IsNullOrEmpty(defaultContentType))
        innerFormatter = (IClientMessageFormatter) new ContentTypeSettingClientMessageFormatter(defaultContentType, innerFormatter);
      return innerFormatter;
    }

    /// <summary>Gets the request formatter on the service for the given service operation and service endpoint.</summary>
    /// <param name="operationDescription">The service operation.</param>
    /// <param name="endpoint">The service endpoint.</param>
    /// <returns>An <see cref="T:System.ServiceModel.Dispatcher.IDispatchMessageFormatter" /> reference to the request formatter on the service for the specified operation and endpoint.</returns>
    protected virtual IDispatchMessageFormatter GetRequestDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
    {
      IDispatchMessageFormatter result = (IDispatchMessageFormatter) null;
      UriTemplateDispatchFormatter throwAway = new UriTemplateDispatchFormatter(operationDescription, (IDispatchMessageFormatter) null, this.GetQueryStringConverter(operationDescription), endpoint.Contract.Name, endpoint.Address.Uri);
      int numUriVariables = throwAway.pathMapping.Count + throwAway.queryMapping.Count;
      WebHttpBehavior.HideReplyMessage(operationDescription, (WebHttpBehavior.Effect) (() =>
      {
        WebMessageBodyStyle style = this.GetBodyStyle(operationDescription);
        WebHttpBehavior.Effect doBodyFormatter = (WebHttpBehavior.Effect) (() =>
        {
          if (numUriVariables != 0)
            WebHttpBehavior.EnsureNotUntypedMessageNorMessageContract(operationDescription);
          this.ValidateBodyParameters(operationDescription, true);
          System.Type type;
          if (WebHttpBehavior.TryGetStreamParameterType(operationDescription.Messages[0], operationDescription, true, out type))
          {
            result = (IDispatchMessageFormatter) new HttpStreamFormatter(operationDescription);
          }
          else
          {
            System.Type parameterType;
            if (this.UseBareRequestFormatter(style, operationDescription, out parameterType))
              result = SingleBodyParameterMessageFormatter.CreateXmlAndJsonDispatchFormatter(operationDescription, parameterType, true, this.xmlSerializerManager, this.JavascriptCallbackParameterName);
            else
              result = this.GetDefaultXmlAndJsonDispatchFormatter(operationDescription, !WebHttpBehavior.IsBareRequest(style));
          }
        });
        if (numUriVariables == 0)
        {
          if (WebHttpBehavior.IsUntypedMessage(operationDescription.Messages[0]))
          {
            this.ValidateBodyParameters(operationDescription, true);
            result = (IDispatchMessageFormatter) new WebHttpBehavior.MessagePassthroughFormatter();
          }
          else if (WebHttpBehavior.IsTypedMessage(operationDescription.Messages[0]))
          {
            this.ValidateBodyParameters(operationDescription, true);
            result = this.GetDefaultXmlAndJsonDispatchFormatter(operationDescription, !WebHttpBehavior.IsBareRequest(style));
          }
          else
            doBodyFormatter();
        }
        else
          WebHttpBehavior.HideRequestUriTemplateParameters(operationDescription, throwAway, (WebHttpBehavior.Effect) (() => WebHttpBehavior.CloneMessageDescriptionsBeforeActing(operationDescription, (WebHttpBehavior.Effect) (() => doBodyFormatter()))));
        result = (IDispatchMessageFormatter) new UriTemplateDispatchFormatter(operationDescription, result, this.GetQueryStringConverter(operationDescription), endpoint.Contract.Name, endpoint.Address.Uri);
      }));
      return result;
    }

    private static void CloneMessageDescriptionsBeforeActing(OperationDescription operationDescription, WebHttpBehavior.Effect effect)
    {
      MessageDescription message = operationDescription.Messages[0];
      bool flag = operationDescription.Messages.Count > 1;
      MessageDescription messageDescription = flag ? operationDescription.Messages[1] : (MessageDescription) null;
      operationDescription.Messages[0] = message.Clone();
      if (flag)
        operationDescription.Messages[1] = messageDescription.Clone();
      effect();
      operationDescription.Messages[0] = message;
      if (!flag)
        return;
      operationDescription.Messages[1] = messageDescription;
    }

    internal virtual bool UseBareRequestFormatter(WebMessageBodyStyle style, OperationDescription operationDescription, out System.Type parameterType)
    {
      parameterType = (System.Type) null;
      if (WebHttpBehavior.IsBareRequest(style))
        return WebHttpBehavior.TryGetNonMessageParameterType(operationDescription.Messages[0], operationDescription, true, out parameterType);
      return false;
    }

    private static Collection<MessagePartDescription> CloneParts(MessageDescription md)
    {
      MessagePartDescriptionCollection parts = md.Body.Parts;
      Collection<MessagePartDescription> collection = new Collection<MessagePartDescription>();
      for (int index = 0; index < parts.Count; ++index)
      {
        MessagePartDescription messagePartDescription = parts[index].Clone();
        collection.Add(messagePartDescription);
      }
      return collection;
    }

    private static void EnsureNotUntypedMessageNorMessageContract(OperationDescription operationDescription)
    {
      bool flag = false;
      if (WebHttpBehavior.GetWebMethod(operationDescription) == "GET" && WebHttpBehavior.GetWebUriTemplate(operationDescription) == null)
        flag = true;
      if (WebHttpBehavior.IsTypedMessage(operationDescription.Messages[0]))
      {
        if (flag)
        {
          // ISSUE: reference to a compiler-generated method
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.GETCannotHaveMCParameter, (object) operationDescription.Name, (object) operationDescription.DeclaringContract.Name, (object) operationDescription.Messages[0].MessageType.Name)));
        }
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.UTParamsDoNotComposeWithMessageContract, new object[2]
        {
          (object) operationDescription.Name,
          (object) operationDescription.DeclaringContract.Name
        })));
      }
      if (WebHttpBehavior.IsUntypedMessage(operationDescription.Messages[0]))
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.UTParamsDoNotComposeWithMessage, new object[2]
        {
          (object) operationDescription.Name,
          (object) operationDescription.DeclaringContract.Name
        })));
      }
    }

    private static void EnsureOk(WebGetAttribute wga, WebInvokeAttribute wia, OperationDescription od)
    {
      if (wga != null && wia != null)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.MultipleWebAttributes, new object[2]
        {
          (object) od.Name,
          (object) od.DeclaringContract.Name
        })));
      }
    }

    private static void HideReplyMessage(OperationDescription operationDescription, WebHttpBehavior.Effect effect)
    {
      MessageDescription messageDescription = (MessageDescription) null;
      if (operationDescription.Messages.Count > 1)
      {
        messageDescription = operationDescription.Messages[1];
        operationDescription.Messages[1] = WebHttpBehavior.MakeDummyMessageDescription(MessageDirection.Output);
      }
      effect();
      if (operationDescription.Messages.Count <= 1)
        return;
      operationDescription.Messages[1] = messageDescription;
    }

    private static void HideRequestUriTemplateParameters(OperationDescription operationDescription, UriTemplateClientFormatter throwAway, WebHttpBehavior.Effect effect)
    {
      WebHttpBehavior.HideRequestUriTemplateParameters(operationDescription, throwAway.pathMapping, throwAway.queryMapping, effect);
    }

    internal static void HideRequestUriTemplateParameters(OperationDescription operationDescription, UriTemplateDispatchFormatter throwAway, WebHttpBehavior.Effect effect)
    {
      WebHttpBehavior.HideRequestUriTemplateParameters(operationDescription, throwAway.pathMapping, throwAway.queryMapping, effect);
    }

    private static void HideRequestUriTemplateParameters(OperationDescription operationDescription, Dictionary<int, string> pathMapping, Dictionary<int, KeyValuePair<string, System.Type>> queryMapping, WebHttpBehavior.Effect effect)
    {
      Collection<MessagePartDescription> collection1 = WebHttpBehavior.CloneParts(operationDescription.Messages[0]);
      Collection<MessagePartDescription> collection2 = WebHttpBehavior.CloneParts(operationDescription.Messages[0]);
      operationDescription.Messages[0].Body.Parts.Clear();
      int num = 0;
      for (int key = 0; key < collection2.Count; ++key)
      {
        if (!pathMapping.ContainsKey(key) && !queryMapping.ContainsKey(key))
        {
          operationDescription.Messages[0].Body.Parts.Add(collection2[key]);
          collection2[key].Index = num++;
        }
      }
      effect();
      operationDescription.Messages[0].Body.Parts.Clear();
      for (int index = 0; index < collection1.Count; ++index)
        operationDescription.Messages[0].Body.Parts.Add(collection1[index]);
    }

    private static bool IsBareRequest(WebMessageBodyStyle style)
    {
      if (style != WebMessageBodyStyle.Bare)
        return style == WebMessageBodyStyle.WrappedResponse;
      return true;
    }

    private static bool IsBareResponse(WebMessageBodyStyle style)
    {
      if (style != WebMessageBodyStyle.Bare)
        return style == WebMessageBodyStyle.WrappedRequest;
      return true;
    }

    internal static bool TryGetNonMessageParameterType(MessageDescription message, OperationDescription declaringOperation, bool isRequest, out System.Type type)
    {
      type = (System.Type) null;
      if (message == null)
        return true;
      if (WebHttpBehavior.IsTypedMessage(message) || WebHttpBehavior.IsUntypedMessage(message))
        return false;
      if (isRequest)
      {
        if (message.Body.Parts.Count > 1)
        {
          // ISSUE: reference to a compiler-generated method
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.AtMostOneRequestBodyParameterAllowedForUnwrappedMessages, new object[2]
          {
            (object) declaringOperation.Name,
            (object) declaringOperation.DeclaringContract.Name
          })));
        }
        if (message.Body.Parts.Count == 1 && message.Body.Parts[0].Type != typeof (void))
          type = message.Body.Parts[0].Type;
        return true;
      }
      if (message.Body.Parts.Count > 0)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.OnlyReturnValueBodyParameterAllowedForUnwrappedMessages, new object[2]
        {
          (object) declaringOperation.Name,
          (object) declaringOperation.DeclaringContract.Name
        })));
      }
      if (message.Body.ReturnValue != null && message.Body.ReturnValue.Type != typeof (void))
        type = message.Body.ReturnValue.Type;
      return true;
    }

    private static bool TryGetStreamParameterType(MessageDescription message, OperationDescription declaringOperation, bool isRequest, out System.Type type)
    {
      type = (System.Type) null;
      if (message == null || WebHttpBehavior.IsTypedMessage(message) || WebHttpBehavior.IsUntypedMessage(message))
        return false;
      if (isRequest)
      {
        bool flag = false;
        for (int index = 0; index < message.Body.Parts.Count; ++index)
        {
          if (typeof (Stream) == message.Body.Parts[index].Type)
          {
            type = message.Body.Parts[index].Type;
            flag = true;
            break;
          }
        }
        if (flag && message.Body.Parts.Count > 1)
        {
          // ISSUE: reference to a compiler-generated method
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR2.GetString(SR2.AtMostOneRequestBodyParameterAllowedForStream, new object[2]
          {
            (object) declaringOperation.Name,
            (object) declaringOperation.DeclaringContract.Name
          })));
        }
        return flag;
      }
      for (int index = 0; index < message.Body.Parts.Count; ++index)
      {
        if (typeof (Stream) == message.Body.Parts[index].Type)
        {
          // ISSUE: reference to a compiler-generated method
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR2.GetString(SR2.NoOutOrRefStreamParametersAllowed, (object) message.Body.Parts[index].Name, (object) declaringOperation.Name, (object) declaringOperation.DeclaringContract.Name)));
        }
      }
      if (message.Body.ReturnValue == null || !(typeof (Stream) == message.Body.ReturnValue.Type))
        return false;
      if (message.Body.Parts.Count > 0)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR2.GetString(SR2.NoOutOrRefParametersAllowedWithStreamResult, new object[2]
        {
          (object) declaringOperation.Name,
          (object) declaringOperation.DeclaringContract.Name
        })));
      }
      type = message.Body.ReturnValue.Type;
      return true;
    }

    private static void ValidateAtMostOneStreamParameter(OperationDescription operation, bool request)
    {
      System.Type type;
      if (request)
      {
        WebHttpBehavior.TryGetStreamParameterType(operation.Messages[0], operation, true, out type);
      }
      else
      {
        if (operation.Messages.Count <= 1)
          return;
        WebHttpBehavior.TryGetStreamParameterType(operation.Messages[1], operation, false, out type);
      }
    }

    private string GetDefaultContentType(bool isStream, bool useJson, WebMessageEncodingBindingElement webEncoding)
    {
      if (isStream)
        return WebHttpBehavior.defaultStreamContentType;
      if (useJson)
        return JsonMessageEncoderFactory.GetContentType(webEncoding);
      return (string) null;
    }

    private IDispatchMessageFormatter GetDefaultDispatchFormatter(OperationDescription od, bool useJson, bool isWrapped)
    {
      DataContractSerializerOperationBehavior dcsob = od.Behaviors.Find<DataContractSerializerOperationBehavior>();
      if (useJson)
      {
        if (dcsob == null)
        {
          // ISSUE: reference to a compiler-generated method
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.JsonFormatRequiresDataContract, (object) od.Name, (object) od.DeclaringContract.Name, (object) od.DeclaringContract.Namespace)));
        }
        return (IDispatchMessageFormatter) this.CreateDataContractJsonSerializerOperationFormatter(od, dcsob, isWrapped);
      }
      DispatchOperation dispatchOperation = new DispatchOperation(new EndpointDispatcher(new EndpointAddress("http://localhost/"), "name", "").DispatchRuntime, "dummyDispatch", "urn:dummy");
      dispatchOperation.Formatter = (IDispatchMessageFormatter) null;
      if (dcsob != null)
      {
        ((IOperationBehavior) dcsob).ApplyDispatchBehavior(od, dispatchOperation);
        return dispatchOperation.Formatter;
      }
      XmlSerializerOperationBehavior operationBehavior = od.Behaviors.Find<XmlSerializerOperationBehavior>();
      if (operationBehavior == null)
        return (IDispatchMessageFormatter) null;
      ((IOperationBehavior) new XmlSerializerOperationBehavior(od, operationBehavior.XmlSerializerFormatAttribute, this.reflector)).ApplyDispatchBehavior(od, dispatchOperation);
      return dispatchOperation.Formatter;
    }

    internal virtual DataContractJsonSerializerOperationFormatter CreateDataContractJsonSerializerOperationFormatter(OperationDescription od, DataContractSerializerOperationBehavior dcsob, bool isWrapped)
    {
      return new DataContractJsonSerializerOperationFormatter(od, dcsob.MaxItemsInObjectGraph, dcsob.IgnoreExtensionDataObject, dcsob.DataContractSurrogate, isWrapped, false, this.JavascriptCallbackParameterName);
    }

    private IClientMessageFormatter GetDefaultXmlAndJsonClientFormatter(OperationDescription od, bool isWrapped)
    {
      IClientMessageFormatter defaultClientFormatter1 = this.GetDefaultClientFormatter(od, false, isWrapped);
      if (!WebHttpBehavior.SupportsJsonFormat(od))
        return defaultClientFormatter1;
      IClientMessageFormatter defaultClientFormatter2 = this.GetDefaultClientFormatter(od, true, isWrapped);
      return (IClientMessageFormatter) new DemultiplexingClientMessageFormatter((IDictionary<WebContentFormat, IClientMessageFormatter>) new Dictionary<WebContentFormat, IClientMessageFormatter>()
      {
        {
          WebContentFormat.Xml,
          defaultClientFormatter1
        },
        {
          WebContentFormat.Json,
          defaultClientFormatter2
        }
      }, defaultClientFormatter1);
    }

    private IDispatchMessageFormatter GetDefaultXmlAndJsonDispatchFormatter(OperationDescription od, bool isWrapped)
    {
      IDispatchMessageFormatter dispatchFormatter1 = this.GetDefaultDispatchFormatter(od, false, isWrapped);
      if (!WebHttpBehavior.SupportsJsonFormat(od))
        return dispatchFormatter1;
      IDispatchMessageFormatter dispatchFormatter2 = this.GetDefaultDispatchFormatter(od, true, isWrapped);
      return (IDispatchMessageFormatter) new DemultiplexingDispatchMessageFormatter((IDictionary<WebContentFormat, IDispatchMessageFormatter>) new Dictionary<WebContentFormat, IDispatchMessageFormatter>()
      {
        {
          WebContentFormat.Xml,
          dispatchFormatter1
        },
        {
          WebContentFormat.Json,
          dispatchFormatter2
        }
      }, dispatchFormatter1);
    }

    internal WebMessageFormat GetRequestFormat(OperationDescription od)
    {
      WebGetAttribute wga = od.Behaviors.Find<WebGetAttribute>();
      WebInvokeAttribute wia = od.Behaviors.Find<WebInvokeAttribute>();
      WebHttpBehavior.EnsureOk(wga, wia, od);
      if (wga != null)
      {
        if (!wga.IsRequestFormatSetExplicitly)
          return this.DefaultOutgoingRequestFormat;
        return wga.RequestFormat;
      }
      if (wia != null && wia.IsRequestFormatSetExplicitly)
        return wia.RequestFormat;
      return this.DefaultOutgoingRequestFormat;
    }

    internal WebMessageFormat GetResponseFormat(OperationDescription od)
    {
      WebGetAttribute wga = od.Behaviors.Find<WebGetAttribute>();
      WebInvokeAttribute wia = od.Behaviors.Find<WebInvokeAttribute>();
      WebHttpBehavior.EnsureOk(wga, wia, od);
      if (wga != null)
      {
        if (!wga.IsResponseFormatSetExplicitly)
          return this.DefaultOutgoingResponseFormat;
        return wga.ResponseFormat;
      }
      if (wia != null && wia.IsResponseFormatSetExplicitly)
        return wia.ResponseFormat;
      return this.DefaultOutgoingResponseFormat;
    }

    private void ValidateBodyParameters(OperationDescription operation, bool request)
    {
      string webMethod = WebHttpBehavior.GetWebMethod(operation);
      if (request)
        this.ValidateGETHasNoBody(operation, webMethod);
      this.ValidateBodyStyle(operation, request);
      WebHttpBehavior.ValidateAtMostOneStreamParameter(operation, request);
    }

    private void ValidateBodyStyle(OperationDescription operation, bool request)
    {
      WebMessageBodyStyle bodyStyle = this.GetBodyStyle(operation);
      System.Type type;
      if (request && WebHttpBehavior.IsBareRequest(bodyStyle))
        WebHttpBehavior.TryGetNonMessageParameterType(operation.Messages[0], operation, true, out type);
      if (request || operation.Messages.Count <= 1 || !WebHttpBehavior.IsBareResponse(bodyStyle))
        return;
      WebHttpBehavior.TryGetNonMessageParameterType(operation.Messages[1], operation, false, out type);
    }

    private void ValidateGETHasNoBody(OperationDescription operation, string method)
    {
      if (!(method == "GET") || WebHttpBehavior.IsUntypedMessage(operation.Messages[0]) || operation.Messages[0].Body.Parts.Count == 0)
        return;
      if (!WebHttpBehavior.IsTypedMessage(operation.Messages[0]))
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.GETCannotHaveBody, (object) operation.Name, (object) operation.DeclaringContract.Name, (object) operation.Messages[0].Body.Parts[0].Name)));
      }
      // ISSUE: reference to a compiler-generated method
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.GETCannotHaveMCParameter, (object) operation.Name, (object) operation.DeclaringContract.Name, (object) operation.Messages[0].MessageType.Name)));
    }

    private void ValidateContract(ServiceEndpoint endpoint)
    {
      foreach (OperationDescription operation in (Collection<OperationDescription>) endpoint.Contract.Operations)
      {
        this.ValidateNoOperationHasEncodedXmlSerializer(operation);
        this.ValidateNoMessageContractHeaders(operation.Messages[0], operation.Name, endpoint.Contract.Name);
        this.ValidateNoBareMessageContractWithMultipleParts(operation.Messages[0], operation.Name, endpoint.Contract.Name);
        this.ValidateNoMessageContractWithStream(operation.Messages[0], operation.Name, endpoint.Contract.Name);
        if (operation.Messages.Count > 1)
        {
          this.ValidateNoMessageContractHeaders(operation.Messages[1], operation.Name, endpoint.Contract.Name);
          this.ValidateNoBareMessageContractWithMultipleParts(operation.Messages[1], operation.Name, endpoint.Contract.Name);
          this.ValidateNoMessageContractWithStream(operation.Messages[1], operation.Name, endpoint.Contract.Name);
        }
      }
    }

    internal static bool IsXmlSerializerFaultFormat(OperationDescription operationDescription)
    {
      XmlSerializerOperationBehavior operationBehavior = operationDescription.Behaviors.Find<XmlSerializerOperationBehavior>();
      if (operationBehavior != null)
        return operationBehavior.XmlSerializerFormatAttribute.SupportFaults;
      return false;
    }

    private void ValidateNoMessageContractWithStream(MessageDescription md, string opName, string contractName)
    {
      if (!WebHttpBehavior.IsTypedMessage(md))
        return;
      foreach (MessagePartDescription part in (Collection<MessagePartDescription>) md.Body.Parts)
      {
        if (part.Type == typeof (Stream))
        {
          // ISSUE: reference to a compiler-generated method
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.StreamBodyMemberNotSupported, (object) this.GetType().ToString(), (object) contractName, (object) opName, (object) md.MessageType.ToString(), (object) part.Name)));
        }
      }
    }

    private void ValidateNoOperationHasEncodedXmlSerializer(OperationDescription od)
    {
      XmlSerializerOperationBehavior operationBehavior = od.Behaviors.Find<XmlSerializerOperationBehavior>();
      if (operationBehavior != null && (operationBehavior.XmlSerializerFormatAttribute.Style == OperationFormatStyle.Rpc || operationBehavior.XmlSerializerFormatAttribute.IsEncoded))
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.RpcEncodedNotSupportedForNoneMessageVersion, (object) od.Name, (object) od.DeclaringContract.Name, (object) od.DeclaringContract.Namespace)));
      }
    }

    private void ValidateNoBareMessageContractWithMultipleParts(MessageDescription md, string opName, string contractName)
    {
      if (!WebHttpBehavior.IsTypedMessage(md) || md.Body.WrapperName != null)
        return;
      if (md.Body.Parts.Count > 1)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.InvalidMessageContractWithoutWrapperName, (object) opName, (object) contractName, (object) md.MessageType)));
      }
      if (md.Body.Parts.Count == 1 && md.Body.Parts[0].Multiple)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.MCAtMostOneRequestBodyParameterAllowedForUnwrappedMessages, (object) opName, (object) contractName, (object) md.MessageType)));
      }
    }

    private void ValidateNoMessageContractHeaders(MessageDescription md, string opName, string contractName)
    {
      if (md.Headers.Count != 0)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.InvalidMethodWithSOAPHeaders, new object[2]
        {
          (object) opName,
          (object) contractName
        })));
      }
    }
#endif

    internal static JavascriptCallbackResponseMessageProperty TrySetupJavascriptCallback(string callbackParameterName)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("WebHttpBehavior is not implemented in .NET Core");
#else
      JavascriptCallbackResponseMessageProperty property = (JavascriptCallbackResponseMessageProperty) null;
      if (!string.IsNullOrEmpty(callbackParameterName) && !OperationContext.Current.OutgoingMessageProperties.TryGetValue<JavascriptCallbackResponseMessageProperty>(JavascriptCallbackResponseMessageProperty.Name, out property))
      {
        UriTemplateMatch uriTemplateMatch = WebOperationContext.Current.IncomingRequest.UriTemplateMatch;
        if (uriTemplateMatch != null && ((IEnumerable<string>) uriTemplateMatch.QueryParameters.AllKeys).Contains<string>(callbackParameterName))
        {
          string queryParameter = uriTemplateMatch.QueryParameters[callbackParameterName];
          if (!string.IsNullOrEmpty(queryParameter))
          {
            property = new JavascriptCallbackResponseMessageProperty()
            {
              CallbackFunctionName = queryParameter
            };
            OperationContext.Current.OutgoingMessageProperties.Add(JavascriptCallbackResponseMessageProperty.Name, (object) property);
          }
        }
      }
      return property;
#endif
    }

#if !FEATURE_CORECLR
    internal delegate void Effect();

    internal class MessagePassthroughFormatter : IClientMessageFormatter, IDispatchMessageFormatter
    {
      public object DeserializeReply(Message message, object[] parameters)
      {
        return (object) message;
      }

      public void DeserializeRequest(Message message, object[] parameters)
      {
        parameters[0] = (object) message;
      }

      public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
      {
        return result as Message;
      }

      public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
      {
        return parameters[0] as Message;
      }
    }
#endif
  }
}
