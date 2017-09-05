// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.WSTrustDec2005
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.Net.Security;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace System.ServiceModel.Security
{
  internal class WSTrustDec2005 : WSTrustFeb2005
  {
    public override TrustDictionary SerializerDictionary
    {
      get
      {
        return (TrustDictionary) DXD.TrustDec2005Dictionary;
      }
    }

    public WSTrustDec2005(WSSecurityTokenSerializer tokenSerializer)
      : base(tokenSerializer)
    {
    }

    public class DriverDec2005 : WSTrustFeb2005.DriverFeb2005
    {
      public override TrustDictionary DriverDictionary
      {
        get
        {
          return (TrustDictionary) DXD.TrustDec2005Dictionary;
        }
      }

      public override XmlDictionaryString RequestSecurityTokenResponseFinalAction
      {
        get
        {
          return DXD.TrustDec2005Dictionary.RequestSecurityTokenCollectionIssuanceFinalResponse;
        }
      }

      public DriverDec2005(SecurityStandardsManager standardsManager)
        : base(standardsManager)
      {
      }

      public override XmlElement CreateKeyTypeElement(SecurityKeyType keyType)
      {
        if (keyType != SecurityKeyType.BearerKey)
          return base.CreateKeyTypeElement(keyType);
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement element = xmlDocument.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.KeyType.Value, this.DriverDictionary.Namespace.Value);
        element.AppendChild((XmlNode) xmlDocument.CreateTextNode(DXD.TrustDec2005Dictionary.BearerKeyType.Value));
        return element;
      }

      public override bool TryParseKeyTypeElement(XmlElement element, out SecurityKeyType keyType)
      {
        if (element == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
        if (!(element.LocalName == this.DriverDictionary.KeyType.Value) || !(element.NamespaceURI == this.DriverDictionary.Namespace.Value) || !(element.InnerText == DXD.TrustDec2005Dictionary.BearerKeyType.Value))
          return base.TryParseKeyTypeElement(element, out keyType);
        keyType = SecurityKeyType.BearerKey;
        return true;
      }

      public override XmlElement CreateRequiredClaimsElement(IEnumerable<XmlElement> claimsList)
      {
        XmlElement requiredClaimsElement = base.CreateRequiredClaimsElement(claimsList);
        XmlAttribute attribute = requiredClaimsElement.OwnerDocument.CreateAttribute(DXD.TrustDec2005Dictionary.Dialect.Value);
        attribute.Value = DXD.TrustDec2005Dictionary.DialectType.Value;
        requiredClaimsElement.Attributes.Append(attribute);
        return requiredClaimsElement;
      }

      public override IChannelFactory<IRequestChannel> CreateFederationProxy(EndpointAddress address, Binding binding, KeyedByTypeCollection<IEndpointBehavior> channelBehaviors)
      {
        if (channelBehaviors == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("channelBehaviors");
        ChannelFactory<WSTrustDec2005.DriverDec2005.IWsTrustDec2005SecurityTokenService> innerChannelFactory = new ChannelFactory<WSTrustDec2005.DriverDec2005.IWsTrustDec2005SecurityTokenService>(binding, address);
        this.SetProtectionLevelForFederation(innerChannelFactory.Endpoint.Contract.Operations);
#if FEATURE_CORECLR
          // ServiceEndpoint.Behaviors is not supported
#else
        innerChannelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
        for (int index = 0; index < channelBehaviors.Count; ++index)
          innerChannelFactory.Endpoint.Behaviors.Add(channelBehaviors[index]);
        innerChannelFactory.Endpoint.Behaviors.Add((IEndpointBehavior) new WSTrustFeb2005.DriverFeb2005.InteractiveInitializersRemovingBehavior());
#endif
        return (IChannelFactory<IRequestChannel>) new WSTrustFeb2005.DriverFeb2005.RequestChannelFactory<WSTrustDec2005.DriverDec2005.IWsTrustDec2005SecurityTokenService>(innerChannelFactory);
      }

      public override Collection<XmlElement> ProcessUnknownRequestParameters(Collection<XmlElement> unknownRequestParameters, Collection<XmlElement> originalRequestParameters)
      {
        if (originalRequestParameters == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("originalRequestParameters");
        if (originalRequestParameters.Count <= 0 || originalRequestParameters[0] == null || originalRequestParameters[0].OwnerDocument == null)
          return originalRequestParameters;
        XmlElement element = originalRequestParameters[0].OwnerDocument.CreateElement(DXD.TrustDec2005Dictionary.Prefix.Value, DXD.TrustDec2005Dictionary.SecondaryParameters.Value, DXD.TrustDec2005Dictionary.Namespace.Value);
        for (int index = 0; index < originalRequestParameters.Count; ++index)
          element.AppendChild((XmlNode) originalRequestParameters[index]);
        return new Collection<XmlElement>() { element };
      }

      internal virtual bool IsSecondaryParametersElement(XmlElement element)
      {
        if (element.LocalName == DXD.TrustDec2005Dictionary.SecondaryParameters.Value)
          return element.NamespaceURI == DXD.TrustDec2005Dictionary.Namespace.Value;
        return false;
      }

      public virtual XmlElement CreateKeyWrapAlgorithmElement(string keyWrapAlgorithm)
      {
        if (keyWrapAlgorithm == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyWrapAlgorithm");
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement element = xmlDocument.CreateElement(DXD.TrustDec2005Dictionary.Prefix.Value, DXD.TrustDec2005Dictionary.KeyWrapAlgorithm.Value, DXD.TrustDec2005Dictionary.Namespace.Value);
        element.AppendChild((XmlNode) xmlDocument.CreateTextNode(keyWrapAlgorithm));
        return element;
      }

      internal override bool IsKeyWrapAlgorithmElement(XmlElement element, out string keyWrapAlgorithm)
      {
        return WSTrust.CheckElement(element, DXD.TrustDec2005Dictionary.KeyWrapAlgorithm.Value, DXD.TrustDec2005Dictionary.Namespace.Value, out keyWrapAlgorithm);
      }

      [ServiceContract]
      internal interface IWsTrustDec2005SecurityTokenService
      {
        [OperationContract(Action = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue", IsOneWay = false, ReplyAction = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/RSTRC/IssueFinal")]
#if FEATURE_CORECLR
        // Protectionlevel not supported
#else
        [FaultContract(typeof (string), Action = "*", ProtectionLevel = ProtectionLevel.Sign)]
#endif
        Message RequestToken(Message message);
      }
    }
  }
}
