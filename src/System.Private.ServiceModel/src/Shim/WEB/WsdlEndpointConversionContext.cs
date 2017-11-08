// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Description.WsdlEndpointConversionContext
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
#if !FEATURE_CORECLR
using System.Web.Services.Description;
#endif

namespace System.ServiceModel.Description
{
  /// <summary>Passed to custom WSDL exporters and importers to enable customization of the metadata export and import processes for a WSDL endpoint.</summary>
  public class WsdlEndpointConversionContext
  {
#if !FEATURE_CORECLR
    private readonly ServiceEndpoint endpoint;
    private readonly Binding wsdlBinding;
    private readonly Port wsdlPort;
    private readonly WsdlContractConversionContext contractContext;
    private readonly Dictionary<OperationDescription, OperationBinding> wsdlOperationBindings;
    private readonly Dictionary<OperationBinding, OperationDescription> operationDescriptionBindings;
    private readonly Dictionary<MessageDescription, MessageBinding> wsdlMessageBindings;
    private readonly Dictionary<FaultDescription, FaultBinding> wsdlFaultBindings;
    private readonly Dictionary<MessageBinding, MessageDescription> messageDescriptionBindings;
    private readonly Dictionary<FaultBinding, FaultDescription> faultDescriptionBindings;

    internal IEnumerable<IWsdlExportExtension> ExportExtensions
    {
      get
      {
        foreach (IWsdlExportExtension wsdlExportExtension in this.endpoint.Behaviors.FindAll<IWsdlExportExtension>())
          yield return wsdlExportExtension;
        foreach (IWsdlExportExtension wsdlExportExtension in this.endpoint.Binding.CreateBindingElements().FindAll<IWsdlExportExtension>())
          yield return wsdlExportExtension;
        foreach (IWsdlExportExtension wsdlExportExtension in this.endpoint.Contract.Behaviors.FindAll<IWsdlExportExtension>())
          yield return wsdlExportExtension;
        foreach (OperationDescription operation in (Collection<OperationDescription>) this.endpoint.Contract.Operations)
        {
          if (WsdlExporter.OperationIsExportable(operation))
          {
            Collection<IWsdlExportExtension> extensions = operation.Behaviors.FindAll<IWsdlExportExtension>();
            int i = 0;
            while (i < extensions.Count)
            {
              if (WsdlExporter.IsBuiltInOperationBehavior(extensions[i]))
              {
                yield return extensions[i];
                extensions.RemoveAt(i);
              }
              else
                ++i;
            }
            foreach (IWsdlExportExtension wsdlExportExtension in extensions)
              yield return wsdlExportExtension;
            extensions = (Collection<IWsdlExportExtension>) null;
          }
        }
      }
    }

    /// <summary>Gets the <see cref="T:System.ServiceModel.Description.ServiceEndpoint" /> being exported or imported.</summary>
    /// <returns>The <see cref="T:System.ServiceModel.Description.ServiceEndpoint" /> being exported or imported.</returns>
    public ServiceEndpoint Endpoint
    {
      get
      {
        return this.endpoint;
      }
    }

    /// <summary>Gets the <see cref="T:System.Web.Services.Description.Binding" /> for the WSDL endpoint.</summary>
    /// <returns>The <see cref="T:System.Web.Services.Description.Binding" /> for the WSDL endpoint.</returns>
    public Binding WsdlBinding
    {
      get
      {
        return this.wsdlBinding;
      }
    }

    /// <summary>Gets the <see cref="T:System.Web.Services.Description.Port" /> for the WSDL endpoint.</summary>
    /// <returns>The <see cref="T:System.Web.Services.Description.Port" /> for the WSDL endpoint.</returns>
    public Port WsdlPort
    {
      get
      {
        return this.wsdlPort;
      }
    }

    /// <summary>Gets the <see cref="T:System.ServiceModel.Description.WsdlContractConversionContext" /> being exported or imported.</summary>
    /// <returns>The <see cref="T:System.ServiceModel.Description.WsdlContractConversionContext" /> being exported or imported. </returns>
    public WsdlContractConversionContext ContractConversionContext
    {
      get
      {
        return this.contractContext;
      }
    }

    internal WsdlEndpointConversionContext(WsdlContractConversionContext contractContext, ServiceEndpoint endpoint, Binding wsdlBinding, Port wsdlport)
    {
      this.endpoint = endpoint;
      this.wsdlBinding = wsdlBinding;
      this.wsdlPort = wsdlport;
      this.contractContext = contractContext;
      this.wsdlOperationBindings = new Dictionary<OperationDescription, OperationBinding>();
      this.operationDescriptionBindings = new Dictionary<OperationBinding, OperationDescription>();
      this.wsdlMessageBindings = new Dictionary<MessageDescription, MessageBinding>();
      this.messageDescriptionBindings = new Dictionary<MessageBinding, MessageDescription>();
      this.wsdlFaultBindings = new Dictionary<FaultDescription, FaultBinding>();
      this.faultDescriptionBindings = new Dictionary<FaultBinding, FaultDescription>();
    }

    internal WsdlEndpointConversionContext(WsdlEndpointConversionContext bindingContext, ServiceEndpoint endpoint, Port wsdlport)
    {
      this.endpoint = endpoint;
      this.wsdlBinding = bindingContext.WsdlBinding;
      this.wsdlPort = wsdlport;
      this.contractContext = bindingContext.contractContext;
      this.wsdlOperationBindings = bindingContext.wsdlOperationBindings;
      this.operationDescriptionBindings = bindingContext.operationDescriptionBindings;
      this.wsdlMessageBindings = bindingContext.wsdlMessageBindings;
      this.messageDescriptionBindings = bindingContext.messageDescriptionBindings;
      this.wsdlFaultBindings = bindingContext.wsdlFaultBindings;
      this.faultDescriptionBindings = bindingContext.faultDescriptionBindings;
    }

    /// <summary>Gets the WSDL binding for the operation specified by the description.</summary>
    /// <param name="operation">The <see cref="T:System.ServiceModel.Description.OperationDescription" /> of the operation associated with the binding.</param>
    /// <returns>The <see cref="T:System.Web.Services.Description.OperationBinding" /> for the operation specified by the description.</returns>
    public OperationBinding GetOperationBinding(OperationDescription operation)
    {
      return this.wsdlOperationBindings[operation];
    }

    /// <summary>Gets the WSDL binding for the message specified by the description.</summary>
    /// <param name="message">The <see cref="T:System.ServiceModel.Description.MessageDescription" /> associated with the description.</param>
    /// <returns>The <see cref="T:System.Web.Services.Description.MessageBinding" /> for the message specified by the description.</returns>
    public MessageBinding GetMessageBinding(MessageDescription message)
    {
      return this.wsdlMessageBindings[message];
    }

    /// <summary>Gets the WSDL binding associated with the fault.</summary>
    /// <param name="fault">The <see cref="T:System.ServiceModel.Description.FaultDescription" /> for the fault associated with the WSDL binding.</param>
    /// <returns>The WSDL <see cref="T:System.Web.Services.Description.FaultBinding" /> associated with the fault.</returns>
    public FaultBinding GetFaultBinding(FaultDescription fault)
    {
      return this.wsdlFaultBindings[fault];
    }

    /// <summary>Returns the operation description of the operation associated with the WSDL binding.</summary>
    /// <param name="operationBinding">The <see cref="T:System.Web.Services.Description.OperationBinding" /> associated with the description.</param>
    /// <returns>The <see cref="T:System.ServiceModel.Description.OperationDescription" /> of the operation associated with the binding.</returns>
    public OperationDescription GetOperationDescription(OperationBinding operationBinding)
    {
      return this.operationDescriptionBindings[operationBinding];
    }

    /// <summary>Gets the message description for the message specified by the WSDL binding.</summary>
    /// <param name="messageBinding">The <see cref="T:System.Web.Services.Description.MessageBinding" /> associated with the message.</param>
    /// <returns>The <see cref="T:System.ServiceModel.Description.MessageDescription" /> for the message specified by the binding.</returns>
    public MessageDescription GetMessageDescription(MessageBinding messageBinding)
    {
      return this.messageDescriptionBindings[messageBinding];
    }

    /// <summary>Gets the description for the fault associated with the WSDL fault binding.</summary>
    /// <param name="faultBinding">The <see cref="T:System.Web.Services.Description.FaultBinding" /> associated with the WSDL fault binding.</param>
    /// <returns>The <see cref="T:System.ServiceModel.Description.FaultDescription" /> for the fault associated with the WSDL binding.</returns>
    public FaultDescription GetFaultDescription(FaultBinding faultBinding)
    {
      return this.faultDescriptionBindings[faultBinding];
    }

    internal void AddOperationBinding(OperationDescription operationDescription, OperationBinding wsdlOperationBinding)
    {
      this.wsdlOperationBindings.Add(operationDescription, wsdlOperationBinding);
      this.operationDescriptionBindings.Add(wsdlOperationBinding, operationDescription);
    }

    internal void AddMessageBinding(MessageDescription messageDescription, MessageBinding wsdlMessageBinding)
    {
      this.wsdlMessageBindings.Add(messageDescription, wsdlMessageBinding);
      this.messageDescriptionBindings.Add(wsdlMessageBinding, messageDescription);
    }

    internal void AddFaultBinding(FaultDescription faultDescription, FaultBinding wsdlFaultBinding)
    {
      this.wsdlFaultBindings.Add(faultDescription, wsdlFaultBinding);
      this.faultDescriptionBindings.Add(wsdlFaultBinding, faultDescription);
    }
#endif
  }
}
