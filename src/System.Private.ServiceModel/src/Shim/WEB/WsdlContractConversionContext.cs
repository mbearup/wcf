// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Description.WsdlContractConversionContext
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#if !FEATURE_CORECLR
using System.Web.Services.Description;
#endif

namespace System.ServiceModel.Description
{
  /// <summary>Passed to custom WSDL exporters and importers to enable customization of the metadata export and import processes for a contract.</summary>
  public class WsdlContractConversionContext
  {
#if !FEATURE_CORECLR
    private readonly ContractDescription contract;
    private readonly PortType wsdlPortType;
    private readonly Dictionary<OperationDescription, Operation> wsdlOperations;
    private readonly Dictionary<Operation, OperationDescription> operationDescriptions;
    private readonly Dictionary<MessageDescription, OperationMessage> wsdlOperationMessages;
    private readonly Dictionary<FaultDescription, OperationFault> wsdlOperationFaults;
    private readonly Dictionary<OperationMessage, MessageDescription> messageDescriptions;
    private readonly Dictionary<OperationFault, FaultDescription> faultDescriptions;
    private readonly Dictionary<Operation, Collection<OperationBinding>> operationBindings;

    internal IEnumerable<IWsdlExportExtension> ExportExtensions
    {
      get
      {
        foreach (IWsdlExportExtension wsdlExportExtension in this.contract.Behaviors.FindAll<IWsdlExportExtension>())
          yield return wsdlExportExtension;
        foreach (OperationDescription operation in (Collection<OperationDescription>) this.contract.Operations)
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

    /// <summary>Gets the <see cref="T:System.ServiceModel.Description.ContractDescription" /> being exported or imported.</summary>
    /// <returns>The <see cref="T:System.ServiceModel.Description.ContractDescription" /> being exported or imported.</returns>
    public ContractDescription Contract
    {
      get
      {
        return this.contract;
      }
    }

    /// <summary>Gets the <see cref="T:System.Web.Services.Description.PortType" /> that represents the contract.</summary>
    /// <returns>The <see cref="T:System.Web.Services.Description.PortType" /> that represents the contract.</returns>
    public PortType WsdlPortType
    {
      get
      {
        return this.wsdlPortType;
      }
    }

    internal WsdlContractConversionContext(ContractDescription contract, PortType wsdlPortType)
    {
      this.contract = contract;
      this.wsdlPortType = wsdlPortType;
      this.wsdlOperations = new Dictionary<OperationDescription, Operation>();
      this.operationDescriptions = new Dictionary<Operation, OperationDescription>();
      this.wsdlOperationMessages = new Dictionary<MessageDescription, OperationMessage>();
      this.messageDescriptions = new Dictionary<OperationMessage, MessageDescription>();
      this.wsdlOperationFaults = new Dictionary<FaultDescription, OperationFault>();
      this.faultDescriptions = new Dictionary<OperationFault, FaultDescription>();
      this.operationBindings = new Dictionary<Operation, Collection<OperationBinding>>();
    }

    /// <summary>Returns the operation for the specified operation description.</summary>
    /// <param name="operation">The <see cref="T:System.ServiceModel.Description.OperationDescription" /> for the requested <see cref="T:System.Web.Services.Description.Operation" />.</param>
    /// <returns>The operation for the specified operation description.</returns>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The value was not found.</exception>
    /// <exception cref="T:System.ArgumentNullException">The value is null.</exception>
    public Operation GetOperation(OperationDescription operation)
    {
      return this.wsdlOperations[operation];
    }

    /// <summary>Gets a <see cref="T:System.Web.Services.Description.OperationMessage" /> object for the specified <paramref name="message" /> that represents a message type passed by the action of an XML Web service.</summary>
    /// <param name="message">The <see cref="T:System.ServiceModel.Description.MessageDescription" /> for the requested <see cref="T:System.Web.Services.Description.OperationMessage" />.</param>
    /// <returns>A <see cref="T:System.Web.Services.Description.OperationMessage" /> object that represents a message type passed by the action of an XML Web service.</returns>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The value was not found.</exception>
    /// <exception cref="T:System.ArgumentNullException">The value is null.</exception>
    public OperationMessage GetOperationMessage(MessageDescription message)
    {
      return this.wsdlOperationMessages[message];
    }

    /// <summary>Returns the <see cref="T:System.Web.Services.Description.OperationFault" /> for the requested <see cref="T:System.ServiceModel.Description.FaultDescription" />.</summary>
    /// <param name="fault">The <see cref="T:System.ServiceModel.Description.FaultDescription" /> for the requested <see cref="T:System.Web.Services.Description.OperationFault" />.</param>
    /// <returns>The <see cref="T:System.Web.Services.Description.OperationFault" /> for the requested <see cref="T:System.ServiceModel.Description.FaultDescription" />.</returns>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The value was not found.</exception>
    /// <exception cref="T:System.ArgumentNullException">The value is null.</exception>
    public OperationFault GetOperationFault(FaultDescription fault)
    {
      return this.wsdlOperationFaults[fault];
    }

    /// <summary>Returns the operation description associated with the operation.</summary>
    /// <param name="operation">The <see cref="T:System.Web.Services.Description.Operation" /> for the requested <see cref="T:System.ServiceModel.Description.OperationDescription" />.</param>
    /// <returns>The operation description associated with the operation.</returns>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The value was not found.</exception>
    /// <exception cref="T:System.ArgumentNullException">The value is null.</exception>
    public OperationDescription GetOperationDescription(Operation operation)
    {
      return this.operationDescriptions[operation];
    }

    /// <summary>Returns the message description for the specified message.</summary>
    /// <param name="operationMessage">The <see cref="T:System.Web.Services.Description.OperationMessage" /> for the requested <see cref="T:System.ServiceModel.Description.MessageDescription" />.</param>
    /// <returns>The message description for the specified message.</returns>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The value was not found.</exception>
    /// <exception cref="T:System.ArgumentNullException">The value is null.</exception>
    public MessageDescription GetMessageDescription(OperationMessage operationMessage)
    {
      return this.messageDescriptions[operationMessage];
    }

    /// <summary>Returns the fault description for the specified fault.</summary>
    /// <param name="operationFault">The <see cref="T:System.Web.Services.Description.OperationFault" /> for the requested <see cref="T:System.ServiceModel.Description.FaultDescription" />.</param>
    /// <returns>The fault description for the specified fault.</returns>
    /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The value was not found.</exception>
    /// <exception cref="T:System.ArgumentNullException">The value is null.</exception>
    public FaultDescription GetFaultDescription(OperationFault operationFault)
    {
      return this.faultDescriptions[operationFault];
    }

    internal void AddOperation(OperationDescription operationDescription, Operation wsdlOperation)
    {
      this.wsdlOperations.Add(operationDescription, wsdlOperation);
      this.operationDescriptions.Add(wsdlOperation, operationDescription);
    }

    internal void AddMessage(MessageDescription messageDescription, OperationMessage wsdlOperationMessage)
    {
      this.wsdlOperationMessages.Add(messageDescription, wsdlOperationMessage);
      this.messageDescriptions.Add(wsdlOperationMessage, messageDescription);
    }

    internal void AddFault(FaultDescription faultDescription, OperationFault wsdlOperationFault)
    {
      this.wsdlOperationFaults.Add(faultDescription, wsdlOperationFault);
      this.faultDescriptions.Add(wsdlOperationFault, faultDescription);
    }

    internal Collection<OperationBinding> GetOperationBindings(Operation operation)
    {
      Collection<OperationBinding> collection;
      if (!this.operationBindings.TryGetValue(operation, out collection))
      {
        collection = new Collection<OperationBinding>();
        foreach (System.Web.Services.Description.ServiceDescription serviceDescription in (CollectionBase) this.WsdlPortType.ServiceDescription.ServiceDescriptions)
        {
          foreach (Binding binding in (CollectionBase) serviceDescription.Bindings)
          {
            if (binding.Type.Name == this.WsdlPortType.Name && binding.Type.Namespace == this.WsdlPortType.ServiceDescription.TargetNamespace)
            {
              foreach (OperationBinding operation1 in (CollectionBase) binding.Operations)
              {
                if (WsdlImporter.Binding2DescriptionHelper.Match(operation1, operation) != WsdlImporter.Binding2DescriptionHelper.MatchResult.None)
                {
                  collection.Add(operation1);
                  break;
                }
              }
            }
          }
        }
        this.operationBindings.Add(operation, collection);
      }
      return collection;
    }
#endif
  }
}
