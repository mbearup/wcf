// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Description.WsdlExporter
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Runtime;
using System.ServiceModel.Channels;
#if !FEATURE_CORECLR
using System.Web.Services.Description;
#endif
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.ServiceModel.Description
{
  /// <summary>Converts service, contract, and endpoint information into metadata documents.</summary>
#if FEATURE_CORECLR
  public class WsdlExporter
#else
  public class WsdlExporter : MetadataExporter
#endif
  {
#if !FEATURE_CORECLR
    private ServiceDescriptionCollection wsdlDocuments = new ServiceDescriptionCollection();
    private XmlSchemaSet xmlSchemas = WsdlExporter.GetEmptySchemaSet();
    private Dictionary<ContractDescription, WsdlContractConversionContext> exportedContracts = new Dictionary<ContractDescription, WsdlContractConversionContext>();
    private Dictionary<WsdlExporter.BindingDictionaryKey, WsdlEndpointConversionContext> exportedBindings = new Dictionary<WsdlExporter.BindingDictionaryKey, WsdlEndpointConversionContext>();
    private Dictionary<WsdlExporter.EndpointDictionaryKey, ServiceEndpoint> exportedEndpoints = new Dictionary<WsdlExporter.EndpointDictionaryKey, ServiceEndpoint>();
    private static XmlDocument xmlDocument;
    private bool isFaulted;

    /// <summary>Gets a collection of <see cref="T:System.Web.Services.Description.ServiceDescription" /> objects after calling one of the export methods. </summary>
    /// <returns>A collection of <see cref="T:System.Web.Services.Description.ServiceDescription" /> objects.</returns>
    public ServiceDescriptionCollection GeneratedWsdlDocuments
    {
      get
      {
        return this.wsdlDocuments;
      }
    }

    /// <summary>Gets a set of <see cref="T:System.Xml.Schema.XmlSchema" /> objects after calling one of the export methods.</summary>
    /// <returns>A set of <see cref="T:System.Xml.Schema.XmlSchema" /> objects.</returns>
    public XmlSchemaSet GeneratedXmlSchemas
    {
      get
      {
        return this.xmlSchemas;
      }
    }

    private static XmlDocument XmlDoc
    {
      get
      {
        if (WsdlExporter.xmlDocument == null)
        {
          System.Xml.NameTable nameTable = new System.Xml.NameTable();
          nameTable.Add("Policy");
          nameTable.Add("All");
          nameTable.Add("ExactlyOne");
          nameTable.Add("PolicyURIs");
          nameTable.Add("Id");
          nameTable.Add("UsingAddressing");
          nameTable.Add("UsingAddressing");
          nameTable.Add("Addressing");
          nameTable.Add("AnonymousResponses");
          nameTable.Add("NonAnonymousResponses");
          WsdlExporter.xmlDocument = new XmlDocument((XmlNameTable) nameTable);
        }
        return WsdlExporter.xmlDocument;
      }
    }

    /// <summary>Exports metadata that describes only the contract information from the specified contract description.</summary>
    /// <param name="contract">The <see cref="T:System.ServiceModel.Description.ContractDescription" /> to generate metadata from.</param>
    /// <exception cref="T:System.InvalidOperationException">The exporter encountered an error.</exception>
    /// <exception cref="T:System.ArgumentNullException">The contract is null.</exception>
    public override void ExportContract(ContractDescription contract)
    {
      if (this.isFaulted)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("WsdlExporterIsFaulted")));
      if (contract == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("contract");
      if (this.exportedContracts.ContainsKey(contract))
        return;
      try
      {
        PortType wsdlPortType = this.CreateWsdlPortType(contract);
        WsdlContractConversionContext contractContext = new WsdlContractConversionContext(contract, wsdlPortType);
        foreach (OperationDescription operation in (Collection<OperationDescription>) contract.Operations)
        {
          bool isWildcardAction;
          if (!WsdlExporter.OperationIsExportable(operation, out isWildcardAction))
          {
            string warningMessage;
            if (!isWildcardAction)
              warningMessage = SR.GetString("WarnSkippingOpertationWithSessionOpenNotificationEnabled", (object) "Action", (object) "http://schemas.microsoft.com/2011/02/session/onopen", (object) contract.Name, (object) contract.Namespace, (object) operation.Name);
            else
              warningMessage = SR.GetString("WarnSkippingOpertationWithWildcardAction", (object) contract.Name, (object) contract.Namespace, (object) operation.Name);
            this.LogExportWarning(warningMessage);
          }
          else
          {
            System.Web.Services.Description.Operation wsdlOperation = this.CreateWsdlOperation(operation, contract);
            wsdlPortType.Operations.Add(wsdlOperation);
            contractContext.AddOperation(operation, wsdlOperation);
            foreach (MessageDescription message in (Collection<MessageDescription>) operation.Messages)
            {
              OperationMessage operationMessage = this.CreateWsdlOperationMessage(message);
              wsdlOperation.Messages.Add(operationMessage);
              contractContext.AddMessage(message, operationMessage);
            }
            foreach (FaultDescription fault in (Collection<FaultDescription>) operation.Faults)
            {
              OperationFault wsdlOperationFault = this.CreateWsdlOperationFault(fault);
              wsdlOperation.Faults.Add(wsdlOperationFault);
              contractContext.AddFault(fault, wsdlOperationFault);
            }
          }
        }
        this.CallExportContract(contractContext);
        this.exportedContracts.Add(contract, contractContext);
      }
      catch
      {
        this.isFaulted = true;
        throw;
      }
    }

    /// <summary>Generates metadata about the specified endpoint.</summary>
    /// <param name="endpoint">The <see cref="T:System.ServiceModel.Description.ServiceEndpoint" /> about which to return metadata.</param>
    /// <exception cref="T:System.InvalidOperationException">The exporter encountered an error.</exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="endpoint" /> is null.</exception>
    /// <exception cref="T:System.ArgumentException">The binding is null.</exception>
    public override void ExportEndpoint(ServiceEndpoint endpoint)
    {
      if (this.isFaulted)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("WsdlExporterIsFaulted")));
      if (endpoint == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpoint");
      this.ExportEndpoint(endpoint, new XmlQualifiedName("service", "http://tempuri.org/"), (BindingParameterCollection) null);
    }

    /// <summary>Generates metadata about a group of endpoints from a specified service.</summary>
    /// <param name="endpoints">The <see cref="T:System.ServiceModel.Description.ServiceEndpoint" /> objects about which to generate metadata.</param>
    /// <param name="wsdlServiceQName">The name of the service.</param>
    /// <exception cref="T:System.InvalidOperationException">The exporter encountered an error.</exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="endpoints" /> is null.</exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="wsdlServiceQName" /> is null.</exception>
    /// <exception cref="T:System.ArgumentException">A binding is null.</exception>
    public void ExportEndpoints(IEnumerable<ServiceEndpoint> endpoints, XmlQualifiedName wsdlServiceQName)
    {
      this.ExportEndpoints(endpoints, wsdlServiceQName, (BindingParameterCollection) null);
    }

    internal void ExportEndpoints(IEnumerable<ServiceEndpoint> endpoints, XmlQualifiedName wsdlServiceQName, BindingParameterCollection bindingParameters)
    {
      if (this.isFaulted)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("WsdlExporterIsFaulted")));
      if (endpoints == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpoints");
      if (wsdlServiceQName == (XmlQualifiedName) null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("wsdlServiceQName");
      foreach (ServiceEndpoint endpoint in endpoints)
        this.ExportEndpoint(endpoint, wsdlServiceQName, bindingParameters);
    }

    /// <summary>Returns an enumerable collection of generated <see cref="T:System.ServiceModel.Description.MetadataSection" /> objects.</summary>
    /// <returns>An enumerable collection of generated <see cref="T:System.ServiceModel.Description.MetadataSection" /> objects that represents the metadata generated as a result of calls to <see cref="M:System.ServiceModel.Description.WsdlExporter.ExportContract(System.ServiceModel.Description.ContractDescription)" />, <see cref="M:System.ServiceModel.Description.WsdlExporter.ExportEndpoint(System.ServiceModel.Description.ServiceEndpoint)" />, or <see cref="M:System.ServiceModel.Description.WsdlExporter.ExportEndpoints(System.Collections.Generic.IEnumerable{System.ServiceModel.Description.ServiceEndpoint},System.Xml.XmlQualifiedName)" />.</returns>
    public override MetadataSet GetGeneratedMetadata()
    {
      MetadataSet metadataSet = new MetadataSet();
      foreach (System.Web.Services.Description.ServiceDescription wsdlDocument in (CollectionBase) this.wsdlDocuments)
        metadataSet.MetadataSections.Add(MetadataSection.CreateFromServiceDescription(wsdlDocument));
      foreach (System.Xml.Schema.XmlSchema schema in (IEnumerable) this.xmlSchemas.Schemas())
        metadataSet.MetadataSections.Add(MetadataSection.CreateFromSchema(schema));
      return metadataSet;
    }

    private void ExportEndpoint(ServiceEndpoint endpoint, XmlQualifiedName wsdlServiceQName, BindingParameterCollection bindingParameters)
    {
      if (endpoint.Binding == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR.GetString("EndpointsMustHaveAValidBinding1", new object[1]{ (object) endpoint.Name })));
      WsdlExporter.EndpointDictionaryKey key = new WsdlExporter.EndpointDictionaryKey(endpoint, wsdlServiceQName);
      try
      {
        if (this.exportedEndpoints.ContainsKey(key))
          return;
        this.ExportContract(endpoint.Contract);
        WsdlContractConversionContext exportedContract = this.exportedContracts[endpoint.Contract];
        Port wsdlPort;
        bool newBinding;
        bool bindingNameWasUniquified;
        System.Web.Services.Description.Binding wsdlBindingAndPort = this.CreateWsdlBindingAndPort(endpoint, wsdlServiceQName, out wsdlPort, out newBinding, out bindingNameWasUniquified);
        if (!newBinding && wsdlPort == null)
          return;
        WsdlEndpointConversionContext endpointContext;
        if (newBinding)
        {
          endpointContext = new WsdlEndpointConversionContext(exportedContract, endpoint, wsdlBindingAndPort, wsdlPort);
          foreach (OperationDescription operation in (Collection<OperationDescription>) endpoint.Contract.Operations)
          {
            if (WsdlExporter.OperationIsExportable(operation))
            {
              OperationBinding operationBinding = this.CreateWsdlOperationBinding(endpoint.Contract, operation);
              wsdlBindingAndPort.Operations.Add(operationBinding);
              endpointContext.AddOperationBinding(operation, operationBinding);
              foreach (MessageDescription message in (Collection<MessageDescription>) operation.Messages)
              {
                MessageBinding wsdlMessageBinding = this.CreateWsdlMessageBinding(message, endpoint.Binding, operationBinding);
                endpointContext.AddMessageBinding(message, wsdlMessageBinding);
              }
              foreach (FaultDescription fault in (Collection<FaultDescription>) operation.Faults)
              {
                FaultBinding wsdlFaultBinding = this.CreateWsdlFaultBinding(fault, endpoint.Binding, operationBinding);
                endpointContext.AddFaultBinding(fault, wsdlFaultBinding);
              }
            }
          }
          PolicyConversionContext policyContext = bindingParameters != null ? this.ExportPolicy(endpoint, bindingParameters) : this.ExportPolicy(endpoint);
          new WsdlExporter.WSPolicyAttachmentHelper(this.PolicyVersion).AttachPolicy(endpoint, endpointContext, policyContext);
          this.exportedBindings.Add(new WsdlExporter.BindingDictionaryKey(endpoint.Contract, endpoint.Binding), endpointContext);
        }
        else
          endpointContext = new WsdlEndpointConversionContext(this.exportedBindings[new WsdlExporter.BindingDictionaryKey(endpoint.Contract, endpoint.Binding)], endpoint, wsdlPort);
        this.CallExportEndpoint(endpointContext);
        this.exportedEndpoints.Add(key, endpoint);
        if (!bindingNameWasUniquified)
          return;
        this.Errors.Add(new MetadataConversionError(SR.GetString("WarnDuplicateBindingQNameNameOnExport", (object) endpoint.Binding.Name, (object) endpoint.Binding.Namespace, (object) endpoint.Contract.Name), true));
      }
      catch
      {
        this.isFaulted = true;
        throw;
      }
    }

    private void CallExportEndpoint(WsdlEndpointConversionContext endpointContext)
    {
      foreach (IWsdlExportExtension exportExtension in endpointContext.ExportExtensions)
        this.CallExtension(endpointContext, exportExtension);
    }

    private void CallExportContract(WsdlContractConversionContext contractContext)
    {
      foreach (IWsdlExportExtension exportExtension in contractContext.ExportExtensions)
        this.CallExtension(contractContext, exportExtension);
    }

    private PortType CreateWsdlPortType(ContractDescription contract)
    {
      XmlQualifiedName portTypeQname = WsdlExporter.WsdlNamingHelper.GetPortTypeQName(contract);
      System.Web.Services.Description.ServiceDescription wsdl = this.GetOrCreateWsdl(portTypeQname.Namespace);
      PortType portType = new PortType();
      portType.Name = portTypeQname.Name;
      if (wsdl.PortTypes[portType.Name] != null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR.GetString("DuplicateContractQNameNameOnExport", (object) contract.Name, (object) contract.Namespace)));
      WsdlExporter.NetSessionHelper.AddUsingSessionAttributeIfNeeded(portType, contract);
      wsdl.PortTypes.Add(portType);
      return portType;
    }

    private System.Web.Services.Description.Operation CreateWsdlOperation(OperationDescription operation, ContractDescription contract)
    {
      System.Web.Services.Description.Operation wsdlOperation = new System.Web.Services.Description.Operation();
      wsdlOperation.Name = WsdlExporter.WsdlNamingHelper.GetWsdlOperationName(operation, contract);
      WsdlExporter.NetSessionHelper.AddInitiatingTerminatingAttributesIfNeeded(wsdlOperation, operation, contract);
      return wsdlOperation;
    }

    private OperationMessage CreateWsdlOperationMessage(MessageDescription message)
    {
      OperationMessage wsdlOperationMessage = message.Direction != MessageDirection.Input ? (OperationMessage) new OperationOutput() : (OperationMessage) new OperationInput();
      if (!XmlName.IsNullOrEmpty(message.MessageName))
        wsdlOperationMessage.Name = message.MessageName.EncodedName;
      WsdlExporter.WSAddressingHelper.AddActionAttribute(message.Action, wsdlOperationMessage, this.PolicyVersion);
      return wsdlOperationMessage;
    }

    private OperationFault CreateWsdlOperationFault(FaultDescription fault)
    {
      OperationFault operationFault = new OperationFault();
      operationFault.Name = fault.Name;
      WsdlExporter.WSAddressingHelper.AddActionAttribute(fault.Action, (OperationMessage) operationFault, this.PolicyVersion);
      return operationFault;
    }

    private System.Web.Services.Description.Binding CreateWsdlBindingAndPort(ServiceEndpoint endpoint, XmlQualifiedName wsdlServiceQName, out Port wsdlPort, out bool newBinding, out bool bindingNameWasUniquified)
    {
      bool flag = WsdlExporter.IsWsdlExportable(endpoint.Binding);
      WsdlEndpointConversionContext conversionContext;
      XmlQualifiedName xmlQualifiedName1;
      System.Web.Services.Description.Binding binding;
      if (!this.exportedBindings.TryGetValue(new WsdlExporter.BindingDictionaryKey(endpoint.Contract, endpoint.Binding), out conversionContext))
      {
        xmlQualifiedName1 = WsdlExporter.WsdlNamingHelper.GetBindingQName(endpoint, this, out bindingNameWasUniquified);
        System.Web.Services.Description.ServiceDescription wsdl = this.GetOrCreateWsdl(xmlQualifiedName1.Namespace);
        binding = new System.Web.Services.Description.Binding();
        binding.Name = xmlQualifiedName1.Name;
        newBinding = true;
        PortType wsdlPortType = this.exportedContracts[endpoint.Contract].WsdlPortType;
        XmlQualifiedName xmlQualifiedName2 = new XmlQualifiedName(wsdlPortType.Name, wsdlPortType.ServiceDescription.TargetNamespace);
        binding.Type = xmlQualifiedName2;
        if (flag)
          wsdl.Bindings.Add(binding);
        WsdlExporter.EnsureWsdlContainsImport(wsdl, xmlQualifiedName2.Namespace);
      }
      else
      {
        xmlQualifiedName1 = new XmlQualifiedName(conversionContext.WsdlBinding.Name, conversionContext.WsdlBinding.ServiceDescription.TargetNamespace);
        bindingNameWasUniquified = false;
        binding = this.wsdlDocuments[xmlQualifiedName1.Namespace].Bindings[xmlQualifiedName1.Name];
        XmlQualifiedName type = binding.Type;
        newBinding = false;
      }
      if (endpoint.Address != (EndpointAddress) null)
      {
        Service wsdlService = this.GetOrCreateWsdlService(wsdlServiceQName);
        wsdlPort = new Port();
        string portName = WsdlExporter.WsdlNamingHelper.GetPortName(endpoint, wsdlService);
        wsdlPort.Name = portName;
        wsdlPort.Binding = xmlQualifiedName1;
        SoapAddressBinding soapAddressBinding = SoapHelper.GetOrCreateSoapAddressBinding(binding, wsdlPort, this);
        if (soapAddressBinding != null)
          soapAddressBinding.Location = endpoint.Address.Uri.AbsoluteUri;
        WsdlExporter.EnsureWsdlContainsImport(wsdlService.ServiceDescription, xmlQualifiedName1.Namespace);
        if (flag)
          wsdlService.Ports.Add(wsdlPort);
      }
      else
        wsdlPort = (Port) null;
      return binding;
    }

    private OperationBinding CreateWsdlOperationBinding(ContractDescription contract, OperationDescription operation)
    {
      OperationBinding operationBinding = new OperationBinding();
      operationBinding.Name = WsdlExporter.WsdlNamingHelper.GetWsdlOperationName(operation, contract);
      return operationBinding;
    }

    private MessageBinding CreateWsdlMessageBinding(MessageDescription messageDescription, System.ServiceModel.Channels.Binding binding, OperationBinding wsdlOperationBinding)
    {
      MessageBinding messageBinding;
      if (messageDescription.Direction == MessageDirection.Input)
      {
        wsdlOperationBinding.Input = new InputBinding();
        messageBinding = (MessageBinding) wsdlOperationBinding.Input;
      }
      else
      {
        wsdlOperationBinding.Output = new OutputBinding();
        messageBinding = (MessageBinding) wsdlOperationBinding.Output;
      }
      if (!XmlName.IsNullOrEmpty(messageDescription.MessageName))
        messageBinding.Name = messageDescription.MessageName.EncodedName;
      return messageBinding;
    }

    private FaultBinding CreateWsdlFaultBinding(FaultDescription faultDescription, System.ServiceModel.Channels.Binding binding, OperationBinding wsdlOperationBinding)
    {
      FaultBinding bindingOperationFault = new FaultBinding();
      wsdlOperationBinding.Faults.Add(bindingOperationFault);
      if (faultDescription.Name != null)
        bindingOperationFault.Name = faultDescription.Name;
      return bindingOperationFault;
    }

    internal static bool OperationIsExportable(OperationDescription operation)
    {
      bool isWildcardAction;
      return WsdlExporter.OperationIsExportable(operation, out isWildcardAction);
    }

    internal static bool OperationIsExportable(OperationDescription operation, out bool isWildcardAction)
    {
      isWildcardAction = false;
      if (operation.IsSessionOpenNotificationEnabled)
        return false;
      for (int index = 0; index < operation.Messages.Count; ++index)
      {
        if (operation.Messages[index].Action == "*")
        {
          isWildcardAction = true;
          return false;
        }
      }
      return true;
    }

    internal static bool IsBuiltInOperationBehavior(IWsdlExportExtension extension)
    {
      DataContractSerializerOperationBehavior operationBehavior1 = extension as DataContractSerializerOperationBehavior;
      if (operationBehavior1 != null)
        return operationBehavior1.IsBuiltInOperationBehavior;
      XmlSerializerOperationBehavior operationBehavior2 = extension as XmlSerializerOperationBehavior;
      if (operationBehavior2 != null)
        return operationBehavior2.IsBuiltInOperationBehavior;
      return false;
    }

    internal System.Web.Services.Description.ServiceDescription GetOrCreateWsdl(string ns)
    {
      ServiceDescriptionCollection wsdlDocuments = this.wsdlDocuments;
      System.Web.Services.Description.ServiceDescription serviceDescription = wsdlDocuments[ns];
      if (serviceDescription == null)
      {
        serviceDescription = new System.Web.Services.Description.ServiceDescription();
        serviceDescription.TargetNamespace = ns;
        XmlSerializerNamespaces serializerNamespaces = new XmlSerializerNamespaces(new WsdlExporter.WsdlNamespaceHelper(this.PolicyVersion).SerializerNamespaces);
        if (!string.IsNullOrEmpty(serviceDescription.TargetNamespace))
          serializerNamespaces.Add("tns", serviceDescription.TargetNamespace);
        serviceDescription.Namespaces = serializerNamespaces;
        wsdlDocuments.Add(serviceDescription);
      }
      return serviceDescription;
    }

    private Service GetOrCreateWsdlService(XmlQualifiedName wsdlServiceQName)
    {
      System.Web.Services.Description.ServiceDescription wsdl = this.GetOrCreateWsdl(wsdlServiceQName.Namespace);
      Service service = wsdl.Services[wsdlServiceQName.Name];
      if (service == null)
      {
        service = new Service();
        service.Name = wsdlServiceQName.Name;
        if (string.IsNullOrEmpty(wsdl.Name))
          wsdl.Name = service.Name;
        wsdl.Services.Add(service);
      }
      return service;
    }

    private static void EnsureWsdlContainsImport(System.Web.Services.Description.ServiceDescription srcWsdl, string target)
    {
      if (srcWsdl.TargetNamespace == target)
        return;
      foreach (Import import in (CollectionBase) srcWsdl.Imports)
      {
        if (import.Namespace == target)
          return;
      }
      srcWsdl.Imports.Add(new Import()
      {
        Location = (string) null,
        Namespace = target
      });
      WsdlExporter.WsdlNamespaceHelper.FindOrCreatePrefix("i", target, (DocumentableItem) srcWsdl);
    }

    private void LogExportWarning(string warningMessage)
    {
      this.Errors.Add(new MetadataConversionError(warningMessage, true));
    }

    internal static XmlSchemaSet GetEmptySchemaSet()
    {
      return new XmlSchemaSet() { XmlResolver = (XmlResolver) null };
    }

    private static bool IsWsdlExportable(System.ServiceModel.Channels.Binding binding)
    {
      BindingElementCollection bindingElements = binding.CreateBindingElements();
      if (bindingElements == null)
        return true;
      foreach (BindingElement bindingElement in (Collection<BindingElement>) bindingElements)
      {
        MessageEncodingBindingElement encodingBindingElement = bindingElement as MessageEncodingBindingElement;
        if (encodingBindingElement != null && !encodingBindingElement.IsWsdlExportable)
          return false;
      }
      return true;
    }

    private void CallExtension(WsdlContractConversionContext contractContext, IWsdlExportExtension extension)
    {
      try
      {
        extension.ExportContract(this, contractContext);
      }
      catch (Exception ex)
      {
        if (!Fx.IsFatal(ex))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.ThrowExtensionException(contractContext.Contract, extension, ex));
        throw;
      }
    }

    private void CallExtension(WsdlEndpointConversionContext endpointContext, IWsdlExportExtension extension)
    {
      try
      {
        extension.ExportEndpoint(this, endpointContext);
      }
      catch (Exception ex)
      {
        if (!Fx.IsFatal(ex))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.ThrowExtensionException(endpointContext.Endpoint, extension, ex));
        throw;
      }
    }

    private Exception ThrowExtensionException(ContractDescription contract, IWsdlExportExtension exporter, Exception e)
    {
      string str = new XmlQualifiedName(contract.Name, contract.Namespace).ToString();
      return (Exception) new InvalidOperationException(SR.GetString("WsdlExtensionContractExportError", (object) exporter.GetType(), (object) str), e);
    }

    private Exception ThrowExtensionException(ServiceEndpoint endpoint, IWsdlExportExtension exporter, Exception e)
    {
      string str;
      if (endpoint.Address != (EndpointAddress) null && endpoint.Address.Uri != (Uri) null)
        str = endpoint.Address.Uri.ToString();
      else
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Contract={1}:{0} ,Binding={3}:{2}", (object) endpoint.Contract.Name, (object) endpoint.Contract.Namespace, (object) endpoint.Binding.Name, (object) endpoint.Binding.Namespace);
      return (Exception) new InvalidOperationException(SR.GetString("WsdlExtensionEndpointExportError", (object) exporter.GetType(), (object) str), e);
    }

    internal static class WSAddressingHelper
    {
      internal static void AddActionAttribute(string actionUri, OperationMessage wsdlOperationMessage, PolicyVersion policyVersion)
      {
        XmlAttribute xmlAttribute = policyVersion != PolicyVersion.Policy12 ? WsdlExporter.XmlDoc.CreateAttribute("wsam", "Action", "http://www.w3.org/2007/05/addressing/metadata") : WsdlExporter.XmlDoc.CreateAttribute("wsaw", "Action", "http://www.w3.org/2006/05/addressing/wsdl");
        xmlAttribute.Value = actionUri;
        wsdlOperationMessage.ExtensibleAttributes = new XmlAttribute[1]
        {
          xmlAttribute
        };
      }

      internal static void AddAddressToWsdlPort(Port wsdlPort, EndpointAddress addr, AddressingVersion addressing)
      {
        if (addressing == AddressingVersion.None)
          return;
        MemoryStream memoryStream = new MemoryStream();
        XmlWriter writer = XmlWriter.Create((Stream) memoryStream);
        writer.WriteStartElement("temp");
        if (addressing == AddressingVersion.WSAddressing10)
          writer.WriteAttributeString("xmlns", "wsa10", (string) null, "http://www.w3.org/2005/08/addressing");
        else if (addressing == AddressingVersion.WSAddressingAugust2004)
          writer.WriteAttributeString("xmlns", "wsa", (string) null, "http://schemas.xmlsoap.org/ws/2004/08/addressing");
        else
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("AddressingVersionNotSupported", new object[1]{ (object) addressing })));
        addr.WriteTo(addressing, writer);
        writer.WriteEndElement();
        writer.Flush();
        memoryStream.Seek(0L, SeekOrigin.Begin);
        XmlReader reader = XmlReader.Create((Stream) memoryStream);
        int content = (int) reader.MoveToContent();
        XmlElement childNode = (XmlElement) WsdlExporter.XmlDoc.ReadNode(reader).ChildNodes[0];
        wsdlPort.Extensions.Add((object) childNode);
      }

      internal static void AddWSAddressingAssertion(MetadataExporter exporter, PolicyConversionContext context, AddressingVersion addressVersion)
      {
        XmlElement xmlElement;
        if (addressVersion == AddressingVersion.WSAddressingAugust2004)
          xmlElement = WsdlExporter.XmlDoc.CreateElement("wsap", "UsingAddressing", "http://schemas.xmlsoap.org/ws/2004/08/addressing/policy");
        else if (addressVersion == AddressingVersion.WSAddressing10)
        {
          if (exporter.PolicyVersion == PolicyVersion.Policy12)
          {
            xmlElement = WsdlExporter.XmlDoc.CreateElement("wsaw", "UsingAddressing", "http://www.w3.org/2006/05/addressing/wsdl");
          }
          else
          {
            xmlElement = WsdlExporter.XmlDoc.CreateElement("wsam", "Addressing", "http://www.w3.org/2007/05/addressing/metadata");
            SupportedAddressingMode supportedAddressingMode = SupportedAddressingMode.Anonymous;
            string name = typeof (SupportedAddressingMode).Name;
            if (exporter.State.ContainsKey((object) name) && exporter.State[(object) name] is SupportedAddressingMode)
            {
              supportedAddressingMode = (SupportedAddressingMode) exporter.State[(object) name];
              if (!SupportedAddressingModeHelper.IsDefined(supportedAddressingMode))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SupportedAddressingModeNotSupported", new object[1]{ (object) supportedAddressingMode })));
            }
            if (supportedAddressingMode != SupportedAddressingMode.Mixed)
            {
              string localName = supportedAddressingMode != SupportedAddressingMode.Anonymous ? "NonAnonymousResponses" : "AnonymousResponses";
              XmlElement element1 = WsdlExporter.XmlDoc.CreateElement("wsp", "Policy", "http://www.w3.org/ns/ws-policy");
              XmlElement element2 = WsdlExporter.XmlDoc.CreateElement("wsam", localName, "http://www.w3.org/2007/05/addressing/metadata");
              element1.AppendChild((XmlNode) element2);
              xmlElement.AppendChild((XmlNode) element1);
            }
          }
        }
        else if (addressVersion == AddressingVersion.None)
          xmlElement = (XmlElement) null;
        else
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("AddressingVersionNotSupported", new object[1]{ (object) addressVersion })));
        if (xmlElement == null)
          return;
        context.GetBindingAssertions().Add(xmlElement);
      }
    }

    private class WSPolicyAttachmentHelper
    {
      private PolicyVersion policyVersion;

      internal WSPolicyAttachmentHelper(PolicyVersion policyVersion)
      {
        this.policyVersion = policyVersion;
      }

      internal void AttachPolicy(ServiceEndpoint endpoint, WsdlEndpointConversionContext endpointContext, PolicyConversionContext policyContext)
      {
        SortedList<string, string> policyKeys = new SortedList<string, string>();
        NamingHelper.DoesNameExist doesNameExist = (NamingHelper.DoesNameExist) ((name, nameCollection) => policyKeys.ContainsKey(name));
        System.Web.Services.Description.ServiceDescription serviceDescription = endpointContext.WsdlBinding.ServiceDescription;
        ICollection<XmlElement> bindingAssertions1 = (ICollection<XmlElement>) policyContext.GetBindingAssertions();
        System.Web.Services.Description.Binding wsdlBinding = endpointContext.WsdlBinding;
        if (bindingAssertions1.Count > 0)
        {
          string uniqueName = NamingHelper.GetUniqueName(WsdlExporter.WSPolicyAttachmentHelper.CreateBindingPolicyKey(wsdlBinding), doesNameExist, (object) null);
          policyKeys.Add(uniqueName, uniqueName);
          this.AttachItemPolicy(bindingAssertions1, uniqueName, serviceDescription, (DocumentableItem) wsdlBinding);
        }
        foreach (OperationDescription operation in (Collection<OperationDescription>) endpoint.Contract.Operations)
        {
          if (WsdlExporter.OperationIsExportable(operation))
          {
            ICollection<XmlElement> bindingAssertions2 = (ICollection<XmlElement>) policyContext.GetOperationBindingAssertions(operation);
            if (bindingAssertions2.Count > 0)
            {
              OperationBinding operationBinding = endpointContext.GetOperationBinding(operation);
              string uniqueName = NamingHelper.GetUniqueName(WsdlExporter.WSPolicyAttachmentHelper.CreateOperationBindingPolicyKey(operationBinding), doesNameExist, (object) null);
              policyKeys.Add(uniqueName, uniqueName);
              this.AttachItemPolicy(bindingAssertions2, uniqueName, serviceDescription, (DocumentableItem) operationBinding);
            }
            foreach (MessageDescription message in (Collection<MessageDescription>) operation.Messages)
            {
              ICollection<XmlElement> bindingAssertions3 = (ICollection<XmlElement>) policyContext.GetMessageBindingAssertions(message);
              if (bindingAssertions3.Count > 0)
              {
                MessageBinding messageBinding = endpointContext.GetMessageBinding(message);
                string uniqueName = NamingHelper.GetUniqueName(WsdlExporter.WSPolicyAttachmentHelper.CreateMessageBindingPolicyKey(messageBinding, message.Direction), doesNameExist, (object) null);
                policyKeys.Add(uniqueName, uniqueName);
                this.AttachItemPolicy(bindingAssertions3, uniqueName, serviceDescription, (DocumentableItem) messageBinding);
              }
            }
            foreach (FaultDescription fault in (Collection<FaultDescription>) operation.Faults)
            {
              ICollection<XmlElement> bindingAssertions3 = (ICollection<XmlElement>) policyContext.GetFaultBindingAssertions(fault);
              if (bindingAssertions3.Count > 0)
              {
                FaultBinding faultBinding = endpointContext.GetFaultBinding(fault);
                string uniqueName = NamingHelper.GetUniqueName(WsdlExporter.WSPolicyAttachmentHelper.CreateFaultBindingPolicyKey(faultBinding), doesNameExist, (object) null);
                policyKeys.Add(uniqueName, uniqueName);
                this.AttachItemPolicy(bindingAssertions3, uniqueName, serviceDescription, (DocumentableItem) faultBinding);
              }
            }
          }
        }
      }

      private void AttachItemPolicy(ICollection<XmlElement> assertions, string key, System.Web.Services.Description.ServiceDescription policyWsdl, DocumentableItem item)
      {
        this.InsertPolicyReference(this.InsertPolicy(key, policyWsdl, assertions), item);
      }

      private void InsertPolicyReference(string policyKey, DocumentableItem item)
      {
        XmlElement element = WsdlExporter.XmlDoc.CreateElement("wsp", "PolicyReference", this.policyVersion.Namespace);
        XmlAttribute attribute = WsdlExporter.XmlDoc.CreateAttribute("URI");
        attribute.Value = policyKey;
        element.Attributes.Append(attribute);
        item.Extensions.Add((object) element);
      }

      private string InsertPolicy(string key, System.Web.Services.Description.ServiceDescription policyWsdl, ICollection<XmlElement> assertions)
      {
        XmlElement policyElement = this.CreatePolicyElement(assertions);
        XmlAttribute attribute = WsdlExporter.XmlDoc.CreateAttribute("wsu", "Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
        attribute.Value = key;
        policyElement.SetAttributeNode(attribute);
        if (policyWsdl != null)
          policyWsdl.Extensions.Add((object) policyElement);
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "#{0}", new object[1]{ (object) key });
      }

      private XmlElement CreatePolicyElement(ICollection<XmlElement> assertions)
      {
        XmlElement element1 = WsdlExporter.XmlDoc.CreateElement("wsp", "Policy", this.policyVersion.Namespace);
        XmlElement element2 = WsdlExporter.XmlDoc.CreateElement("wsp", "ExactlyOne", this.policyVersion.Namespace);
        element1.AppendChild((XmlNode) element2);
        XmlElement element3 = WsdlExporter.XmlDoc.CreateElement("wsp", "All", this.policyVersion.Namespace);
        element2.AppendChild((XmlNode) element3);
        foreach (XmlNode assertion in (IEnumerable<XmlElement>) assertions)
        {
          XmlNode newChild = WsdlExporter.XmlDoc.ImportNode(assertion, true);
          element3.AppendChild(newChild);
        }
        return element1;
      }

      private static string CreateBindingPolicyKey(System.Web.Services.Description.Binding wsdlBinding)
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_policy", new object[1]{ (object) wsdlBinding.Name });
      }

      private static string CreateOperationBindingPolicyKey(OperationBinding wsdlOperationBinding)
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}_policy", new object[2]{ (object) wsdlOperationBinding.Binding.Name, (object) wsdlOperationBinding.Name });
      }

      private static string CreateMessageBindingPolicyKey(MessageBinding wsdlMessageBinding, MessageDirection direction)
      {
        OperationBinding operationBinding = wsdlMessageBinding.OperationBinding;
        System.Web.Services.Description.Binding binding = operationBinding.Binding;
        if (direction == MessageDirection.Input)
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}_Input_policy", new object[2]{ (object) binding.Name, (object) operationBinding.Name });
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}_output_policy", new object[2]{ (object) binding.Name, (object) operationBinding.Name });
      }

      private static string CreateFaultBindingPolicyKey(FaultBinding wsdlFaultBinding)
      {
        OperationBinding operationBinding = wsdlFaultBinding.OperationBinding;
        System.Web.Services.Description.Binding binding = operationBinding.Binding;
        if (string.IsNullOrEmpty(wsdlFaultBinding.Name))
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}_Fault", new object[2]{ (object) binding.Name, (object) operationBinding.Name });
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}_{2}_Fault", new object[3]{ (object) binding.Name, (object) operationBinding.Name, (object) wsdlFaultBinding.Name });
      }
    }

    private class WsdlNamespaceHelper
    {
      private XmlSerializerNamespaces xmlSerializerNamespaces;
      private PolicyVersion policyVersion;

      internal XmlSerializerNamespaces SerializerNamespaces
      {
        get
        {
          if (this.xmlSerializerNamespaces == null)
          {
            WsdlExporter.WsdlNamespaceHelper.XmlSerializerNamespaceWrapper namespaceWrapper = new WsdlExporter.WsdlNamespaceHelper.XmlSerializerNamespaceWrapper();
            namespaceWrapper.Add("wsdl", "http://schemas.xmlsoap.org/wsdl/");
            namespaceWrapper.Add("xsd", "http://www.w3.org/2001/XMLSchema");
            namespaceWrapper.Add("wsp", this.policyVersion.Namespace);
            namespaceWrapper.Add("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
            namespaceWrapper.Add("wsa", "http://schemas.xmlsoap.org/ws/2004/08/addressing");
            namespaceWrapper.Add("wsap", "http://schemas.xmlsoap.org/ws/2004/08/addressing/policy");
            namespaceWrapper.Add("wsa10", "http://www.w3.org/2005/08/addressing");
            namespaceWrapper.Add("wsaw", "http://www.w3.org/2006/05/addressing/wsdl");
            namespaceWrapper.Add("wsam", "http://www.w3.org/2007/05/addressing/metadata");
            namespaceWrapper.Add("wsx", "http://schemas.xmlsoap.org/ws/2004/09/mex");
            namespaceWrapper.Add("msc", "http://schemas.microsoft.com/ws/2005/12/wsdl/contract");
            namespaceWrapper.Add("soapenc", "http://schemas.xmlsoap.org/soap/encoding/");
            namespaceWrapper.Add("soap12", "http://schemas.xmlsoap.org/wsdl/soap12/");
            namespaceWrapper.Add("soap", "http://schemas.xmlsoap.org/wsdl/soap/");
            this.xmlSerializerNamespaces = namespaceWrapper.GetNamespaces();
          }
          return this.xmlSerializerNamespaces;
        }
      }

      internal WsdlNamespaceHelper(PolicyVersion policyVersion)
      {
        this.policyVersion = policyVersion;
      }

      internal static string FindOrCreatePrefix(string prefixBase, string ns, params DocumentableItem[] scopes)
      {
        if (scopes.Length == 0)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "You must pass at least one namespaceScope", new object[0])));
        string prefix1 = (string) null;
        string prefix2;
        if (string.IsNullOrEmpty(ns))
        {
          prefix2 = string.Empty;
        }
        else
        {
          for (int index = 0; index < scopes.Length; ++index)
          {
            if (WsdlExporter.WsdlNamespaceHelper.TryMatchNamespace(scopes[index].Namespaces.ToArray(), ns, out prefix1))
              return prefix1;
          }
          int num1 = 0;
          string str1;
          string str2;
          for (prefix2 = prefixBase + num1.ToString((IFormatProvider) CultureInfo.InvariantCulture); WsdlExporter.WsdlNamespaceHelper.PrefixExists(scopes[0].Namespaces.ToArray(), prefix2); prefix2 = str1 + str2)
          {
            str1 = prefixBase;
            int num2 = num1 + 1;
            num1 = num2;
            str2 = num2.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          }
        }
        scopes[0].Namespaces.Add(prefix2, ns);
        return prefix2;
      }

      private static bool PrefixExists(XmlQualifiedName[] prefixDefinitions, string prefix)
      {
        return Array.Exists<XmlQualifiedName>(prefixDefinitions, (Predicate<XmlQualifiedName>) (prefixDef => prefixDef.Name == prefix));
      }

      private static bool TryMatchNamespace(XmlQualifiedName[] prefixDefinitions, string ns, out string prefix)
      {
        string foundPrefix = (string) null;
        Array.Find<XmlQualifiedName>(prefixDefinitions, (Predicate<XmlQualifiedName>) (prefixDef =>
        {
          if (!(prefixDef.Namespace == ns))
            return false;
          foundPrefix = prefixDef.Name;
          return true;
        }));
        prefix = foundPrefix;
        return foundPrefix != null;
      }

      private class XmlSerializerNamespaceWrapper
      {
        private readonly XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
        private readonly Dictionary<string, string> lookup = new Dictionary<string, string>();

        internal void Add(string prefix, string namespaceUri)
        {
          if (this.lookup.ContainsKey(prefix))
            return;
          this.namespaces.Add(prefix, namespaceUri);
          this.lookup.Add(prefix, namespaceUri);
        }

        internal XmlSerializerNamespaces GetNamespaces()
        {
          return this.namespaces;
        }
      }
    }

    internal static class WsdlNamingHelper
    {
      internal static XmlQualifiedName GetPortTypeQName(ContractDescription contract)
      {
        return new XmlQualifiedName(contract.Name, contract.Namespace);
      }

      internal static XmlQualifiedName GetBindingQName(ServiceEndpoint endpoint, WsdlExporter exporter, out bool wasUniquified)
      {
        string name = endpoint.Name;
        string str = endpoint.Binding.Namespace;
        string uniqueName = NamingHelper.GetUniqueName(name, WsdlExporter.WsdlNamingHelper.WsdlBindingQNameExists(exporter, str), (object) null);
        wasUniquified = name != uniqueName;
        return new XmlQualifiedName(uniqueName, str);
      }

      private static NamingHelper.DoesNameExist WsdlBindingQNameExists(WsdlExporter exporter, string bindingWsdlNamespace)
      {
        return (NamingHelper.DoesNameExist) ((localName, nameCollection) =>
        {
          XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(localName, bindingWsdlNamespace);
          System.Web.Services.Description.ServiceDescription wsdlDocument = exporter.wsdlDocuments[bindingWsdlNamespace];
          return wsdlDocument != null && wsdlDocument.Bindings[localName] != null;
        });
      }

      internal static string GetPortName(ServiceEndpoint endpoint, Service wsdlService)
      {
        return NamingHelper.GetUniqueName(endpoint.Name, WsdlExporter.WsdlNamingHelper.ServiceContainsPort(wsdlService), (object) null);
      }

      private static NamingHelper.DoesNameExist ServiceContainsPort(Service service)
      {
        return (NamingHelper.DoesNameExist) ((portName, nameCollection) =>
        {
          foreach (NamedItem port in (CollectionBase) service.Ports)
          {
            if (port.Name == portName)
              return true;
          }
          return false;
        });
      }

      internal static string GetWsdlOperationName(OperationDescription operationDescription, ContractDescription parentContractDescription)
      {
        return operationDescription.Name;
      }
    }

    internal static class NetSessionHelper
    {
      internal const string NamespaceUri = "http://schemas.microsoft.com/ws/2005/12/wsdl/contract";
      internal const string Prefix = "msc";
      internal const string UsingSession = "usingSession";
      internal const string IsInitiating = "isInitiating";
      internal const string IsTerminating = "isTerminating";
      internal const string True = "true";
      internal const string False = "false";

      internal static void AddUsingSessionAttributeIfNeeded(PortType wsdlPortType, ContractDescription contract)
      {
        bool b;
        if (contract.SessionMode == SessionMode.Required)
        {
          b = true;
        }
        else
        {
          if (contract.SessionMode != SessionMode.NotAllowed)
            return;
          b = false;
        }
        wsdlPortType.ExtensibleAttributes = WsdlExporter.NetSessionHelper.CloneAndAddToAttributes(wsdlPortType.ExtensibleAttributes, "msc", "usingSession", "http://schemas.microsoft.com/ws/2005/12/wsdl/contract", WsdlExporter.NetSessionHelper.ToValue(b));
      }

      internal static void AddInitiatingTerminatingAttributesIfNeeded(System.Web.Services.Description.Operation wsdlOperation, OperationDescription operation, ContractDescription contract)
      {
        if (contract.SessionMode != SessionMode.Required)
          return;
        WsdlExporter.NetSessionHelper.AddInitiatingAttribute(wsdlOperation, operation.IsInitiating);
        WsdlExporter.NetSessionHelper.AddTerminatingAttribute(wsdlOperation, operation.IsTerminating);
      }

      private static void AddInitiatingAttribute(System.Web.Services.Description.Operation wsdlOperation, bool isInitiating)
      {
        wsdlOperation.ExtensibleAttributes = WsdlExporter.NetSessionHelper.CloneAndAddToAttributes(wsdlOperation.ExtensibleAttributes, "msc", "isInitiating", "http://schemas.microsoft.com/ws/2005/12/wsdl/contract", WsdlExporter.NetSessionHelper.ToValue(isInitiating));
      }

      private static void AddTerminatingAttribute(System.Web.Services.Description.Operation wsdlOperation, bool isTerminating)
      {
        wsdlOperation.ExtensibleAttributes = WsdlExporter.NetSessionHelper.CloneAndAddToAttributes(wsdlOperation.ExtensibleAttributes, "msc", "isTerminating", "http://schemas.microsoft.com/ws/2005/12/wsdl/contract", WsdlExporter.NetSessionHelper.ToValue(isTerminating));
      }

      private static XmlAttribute[] CloneAndAddToAttributes(XmlAttribute[] originalAttributes, string prefix, string localName, string ns, string value)
      {
        XmlAttribute attribute = WsdlExporter.XmlDoc.CreateAttribute(prefix, localName, ns);
        attribute.Value = value;
        int num = 0;
        if (originalAttributes != null)
          num = originalAttributes.Length;
        XmlAttribute[] xmlAttributeArray = new XmlAttribute[num + 1];
        if (originalAttributes != null)
          originalAttributes.CopyTo((Array) xmlAttributeArray, 0);
        xmlAttributeArray[xmlAttributeArray.Length - 1] = attribute;
        return xmlAttributeArray;
      }

      private static string ToValue(bool b)
      {
        return !b ? "false" : "true";
      }
    }

    private sealed class BindingDictionaryKey
    {
      public readonly ContractDescription Contract;
      public readonly System.ServiceModel.Channels.Binding Binding;

      public BindingDictionaryKey(ContractDescription contract, System.ServiceModel.Channels.Binding binding)
      {
        this.Contract = contract;
        this.Binding = binding;
      }

      public override bool Equals(object obj)
      {
        WsdlExporter.BindingDictionaryKey bindingDictionaryKey = obj as WsdlExporter.BindingDictionaryKey;
        return bindingDictionaryKey != null && bindingDictionaryKey.Binding == this.Binding && bindingDictionaryKey.Contract == this.Contract;
      }

      public override int GetHashCode()
      {
        return this.Contract.GetHashCode() ^ this.Binding.GetHashCode();
      }
    }

    private sealed class EndpointDictionaryKey
    {
      public readonly ServiceEndpoint Endpoint;
      public readonly XmlQualifiedName ServiceQName;

      public EndpointDictionaryKey(ServiceEndpoint endpoint, XmlQualifiedName serviceQName)
      {
        this.Endpoint = endpoint;
        this.ServiceQName = serviceQName;
      }

      public override bool Equals(object obj)
      {
        WsdlExporter.EndpointDictionaryKey endpointDictionaryKey = obj as WsdlExporter.EndpointDictionaryKey;
        return endpointDictionaryKey != null && endpointDictionaryKey.Endpoint == this.Endpoint && endpointDictionaryKey.ServiceQName == this.ServiceQName;
      }

      public override int GetHashCode()
      {
        return this.Endpoint.GetHashCode() ^ this.ServiceQName.GetHashCode();
      }
    }
#endif
  }
}
