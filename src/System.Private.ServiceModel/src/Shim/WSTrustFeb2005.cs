// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.WSTrustFeb2005
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.Net.Security;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
  internal class WSTrustFeb2005 : WSTrust
  {
    public override TrustDictionary SerializerDictionary
    {
      get
      {
        return (TrustDictionary) XD.TrustFeb2005Dictionary;
      }
    }

    public WSTrustFeb2005(WSSecurityTokenSerializer tokenSerializer)
      : base(tokenSerializer)
    {
    }

    public class DriverFeb2005 : WSTrust.Driver
    {
      public override TrustDictionary DriverDictionary
      {
        get
        {
          return (TrustDictionary) XD.TrustFeb2005Dictionary;
        }
      }

      public override XmlDictionaryString RequestSecurityTokenResponseFinalAction
      {
        get
        {
          return XD.TrustFeb2005Dictionary.RequestSecurityTokenIssuanceResponse;
        }
      }

      public override bool IsSessionSupported
      {
        get
        {
          return true;
        }
      }

      public override bool IsIssuedTokensSupported
      {
        get
        {
          return true;
        }
      }

      public override string IssuedTokensHeaderName
      {
        get
        {
          return this.DriverDictionary.IssuedTokensHeader.Value;
        }
      }

      public override string IssuedTokensHeaderNamespace
      {
        get
        {
          return this.DriverDictionary.Namespace.Value;
        }
      }

      public override string RequestTypeRenew
      {
        get
        {
          return this.DriverDictionary.RequestTypeRenew.Value;
        }
      }

      public override string RequestTypeClose
      {
        get
        {
          return this.DriverDictionary.RequestTypeClose.Value;
        }
      }

      public DriverFeb2005(SecurityStandardsManager standardsManager)
        : base(standardsManager)
      {
      }

      public override Collection<XmlElement> ProcessUnknownRequestParameters(Collection<XmlElement> unknownRequestParameters, Collection<XmlElement> originalRequestParameters)
      {
        return unknownRequestParameters;
      }

      protected override void ReadReferences(XmlElement rstrXml, out SecurityKeyIdentifierClause requestedAttachedReference, out SecurityKeyIdentifierClause requestedUnattachedReference)
      {
        XmlElement element = (XmlElement) null;
        requestedAttachedReference = (SecurityKeyIdentifierClause) null;
        requestedUnattachedReference = (SecurityKeyIdentifierClause) null;
        for (int index = 0; index < rstrXml.ChildNodes.Count; ++index)
        {
          XmlElement childNode = rstrXml.ChildNodes[index] as XmlElement;
          if (childNode != null)
          {
            if (childNode.LocalName == this.DriverDictionary.RequestedSecurityToken.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
              element = XmlHelper.GetChildElement(childNode);
            else if (childNode.LocalName == this.DriverDictionary.RequestedAttachedReference.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
              requestedAttachedReference = this.GetKeyIdentifierXmlReferenceClause(XmlHelper.GetChildElement(childNode));
            else if (childNode.LocalName == this.DriverDictionary.RequestedUnattachedReference.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
              requestedUnattachedReference = this.GetKeyIdentifierXmlReferenceClause(XmlHelper.GetChildElement(childNode));
          }
        }
        try
        {
          if (element == null)
            return;
          if (requestedAttachedReference == null)
            this.StandardsManager.TryCreateKeyIdentifierClauseFromTokenXml(element, SecurityTokenReferenceStyle.Internal, out requestedAttachedReference);
          if (requestedUnattachedReference != null)
            return;
          this.StandardsManager.TryCreateKeyIdentifierClauseFromTokenXml(element, SecurityTokenReferenceStyle.External, out requestedUnattachedReference);
        }
        catch (XmlException)
        {
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new XmlException(SR.GetString("TrustDriverIsUnableToCreatedNecessaryAttachedOrUnattachedReferences", new object[1]{ (object) element.ToString() })));
        }
      }

      protected override bool ReadRequestedTokenClosed(XmlElement rstrXml)
      {
        for (int index = 0; index < rstrXml.ChildNodes.Count; ++index)
        {
          XmlElement childNode = rstrXml.ChildNodes[index] as XmlElement;
          if (childNode != null && childNode.LocalName == this.DriverDictionary.RequestedTokenClosed.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
            return true;
        }
        return false;
      }

      protected override void ReadTargets(XmlElement rstXml, out SecurityKeyIdentifierClause renewTarget, out SecurityKeyIdentifierClause closeTarget)
      {
        renewTarget = (SecurityKeyIdentifierClause) null;
        closeTarget = (SecurityKeyIdentifierClause) null;
        for (int index = 0; index < rstXml.ChildNodes.Count; ++index)
        {
          XmlElement childNode = rstXml.ChildNodes[index] as XmlElement;
          if (childNode != null)
          {
            if (childNode.LocalName == this.DriverDictionary.RenewTarget.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
              renewTarget = this.StandardsManager.SecurityTokenSerializer.ReadKeyIdentifierClause((XmlReader) new XmlNodeReader(childNode.FirstChild));
            else if (childNode.LocalName == this.DriverDictionary.CloseTarget.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
              closeTarget = this.StandardsManager.SecurityTokenSerializer.ReadKeyIdentifierClause((XmlReader) new XmlNodeReader(childNode.FirstChild));
          }
        }
      }

      protected override void WriteReferences(RequestSecurityTokenResponse rstr, XmlDictionaryWriter writer)
      {
        if (rstr.RequestedAttachedReference != null)
        {
          writer.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.RequestedAttachedReference, this.DriverDictionary.Namespace);
          this.StandardsManager.SecurityTokenSerializer.WriteKeyIdentifierClause((XmlWriter) writer, rstr.RequestedAttachedReference);
          writer.WriteEndElement();
        }
        if (rstr.RequestedUnattachedReference == null)
          return;
        writer.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.RequestedUnattachedReference, this.DriverDictionary.Namespace);
        this.StandardsManager.SecurityTokenSerializer.WriteKeyIdentifierClause((XmlWriter) writer, rstr.RequestedUnattachedReference);
        writer.WriteEndElement();
      }

      protected override void WriteRequestedTokenClosed(RequestSecurityTokenResponse rstr, XmlDictionaryWriter writer)
      {
        if (!rstr.IsRequestedTokenClosed)
          return;
        writer.WriteElementString(this.DriverDictionary.RequestedTokenClosed, this.DriverDictionary.Namespace, string.Empty);
      }

      protected override void WriteTargets(RequestSecurityToken rst, XmlDictionaryWriter writer)
      {
        if (rst.RenewTarget != null)
        {
          writer.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.RenewTarget, this.DriverDictionary.Namespace);
          this.StandardsManager.SecurityTokenSerializer.WriteKeyIdentifierClause((XmlWriter) writer, rst.RenewTarget);
          writer.WriteEndElement();
        }
        if (rst.CloseTarget == null)
          return;
        writer.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.CloseTarget, this.DriverDictionary.Namespace);
        this.StandardsManager.SecurityTokenSerializer.WriteKeyIdentifierClause((XmlWriter) writer, rst.CloseTarget);
        writer.WriteEndElement();
      }

      public override IChannelFactory<IRequestChannel> CreateFederationProxy(EndpointAddress address, Binding binding, KeyedByTypeCollection<IEndpointBehavior> channelBehaviors)
      {
        if (channelBehaviors == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("channelBehaviors");
        ChannelFactory<WSTrustFeb2005.DriverFeb2005.IWsTrustFeb2005SecurityTokenService> innerChannelFactory = new ChannelFactory<WSTrustFeb2005.DriverFeb2005.IWsTrustFeb2005SecurityTokenService>(binding, address);
        this.SetProtectionLevelForFederation(innerChannelFactory.Endpoint.Contract.Operations);
#if FEATURE_CORECLR
          // ServiceEndpoint.Behaviors is not supported
#else
        innerChannelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
        for (int index = 0; index < channelBehaviors.Count; ++index)
          innerChannelFactory.Endpoint.Behaviors.Add(channelBehaviors[index]);
        innerChannelFactory.Endpoint.Behaviors.Add((IEndpointBehavior) new WSTrustFeb2005.DriverFeb2005.InteractiveInitializersRemovingBehavior());
#endif
        return (IChannelFactory<IRequestChannel>) new WSTrustFeb2005.DriverFeb2005.RequestChannelFactory<WSTrustFeb2005.DriverFeb2005.IWsTrustFeb2005SecurityTokenService>(innerChannelFactory);
      }

      [ServiceContract]
      internal interface IWsTrustFeb2005SecurityTokenService
      {
        [OperationContract(Action = "http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue", IsOneWay = false, ReplyAction = "http://schemas.xmlsoap.org/ws/2005/02/trust/RSTR/Issue")]
#if FEATURE_CORECLR
        // ProtectionLevel not supported
#else
        [FaultContract(typeof (string), Action = "*", ProtectionLevel = ProtectionLevel.Sign)]
#endif
        Message RequestToken(Message message);
      }

#if FEATURE_CORECLR
      // IEndpointBehavior pulls in a conflicting version of EndpointDispatcher
      public class InteractiveInitializersRemovingBehavior
#else
      public class InteractiveInitializersRemovingBehavior : IEndpointBehavior
#endif
      {
        public void Validate(ServiceEndpoint serviceEndpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint serviceEndpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime behavior)
        {
          if (behavior == null || behavior.InteractiveChannelInitializers == null)
            return;
          behavior.InteractiveChannelInitializers.Clear();
        }
      }

      public class RequestChannelFactory<TokenService> : ChannelFactoryBase, IChannelFactory<IRequestChannel>, IChannelFactory, ICommunicationObject
      {
        private ChannelFactory<TokenService> innerChannelFactory;

        public RequestChannelFactory(ChannelFactory<TokenService> innerChannelFactory)
        {
          this.innerChannelFactory = innerChannelFactory;
        }

        public IRequestChannel CreateChannel(EndpointAddress address)
        {
#if FEATURE_CORECLR
          throw new NotImplementedException("ChannelFactory.CreateChannle cannot be used with type arguments in .NET Core");
#else
          return this.innerChannelFactory.CreateChannel<IRequestChannel>(address);
#endif
        }

        public IRequestChannel CreateChannel(EndpointAddress address, Uri via)
        {
#if FEATURE_CORECLR
          throw new NotImplementedException("ChannelFactory.CreateChannle cannot be used with type arguments in .NET Core");
#else
          return this.innerChannelFactory.CreateChannel<IRequestChannel>(address, via);
#endif
        }

        protected override void OnAbort()
        {
          this.innerChannelFactory.Abort();
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
          return this.innerChannelFactory.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
          this.innerChannelFactory.EndOpen(result);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
          return this.innerChannelFactory.BeginClose(timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
          this.innerChannelFactory.EndClose(result);
        }

        protected override void OnClose(TimeSpan timeout)
        {
          this.innerChannelFactory.Close(timeout);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
          this.innerChannelFactory.Open(timeout);
        }

        public override T GetProperty<T>()
        {
          return this.innerChannelFactory.GetProperty<T>();
        }
      }
    }
  }
}
