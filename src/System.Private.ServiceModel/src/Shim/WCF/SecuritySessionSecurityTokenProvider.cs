// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SecuritySessionSecurityTokenProvider
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IdentityModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net;
using System.Runtime;
using System.Runtime.Diagnostics;
using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Diagnostics.Application;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security.Tokens;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Security
{
  internal class SecuritySessionSecurityTokenProvider : CommunicationObjectSecurityTokenProvider
  {
    private static readonly MessageOperationFormatter operationFormatter = new MessageOperationFormatter();
    private object thisLock = new object();
    private BindingContext issuerBindingContext;
    private IChannelFactory<IRequestChannel> rstChannelFactory;
    private SecurityAlgorithmSuite securityAlgorithmSuite;
    private SecurityStandardsManager standardsManager;
    private SecurityKeyEntropyMode keyEntropyMode;
    private SecurityTokenParameters issuedTokenParameters;
    private bool requiresManualReplyAddressing;
    private EndpointAddress targetAddress;
    private SecurityBindingElement bootstrapSecurityBindingElement;
    private Uri via;
    private string sctUri;
    private Uri privacyNoticeUri;
    private int privacyNoticeVersion;
    private MessageVersion messageVersion;
    private EndpointAddress localAddress;
    private ChannelParameterCollection channelParameters;
    private System.IdentityModel.SafeFreeCredentials credentialsHandle;
    private bool ownCredentialsHandle;
    private WebHeaderCollection webHeaderCollection;

    public WebHeaderCollection WebHeaders
    {
      get
      {
        return this.webHeaderCollection;
      }
      set
      {
        this.CommunicationObject.ThrowIfDisposedOrImmutable();
        this.webHeaderCollection = value;
      }
    }

    public SecurityAlgorithmSuite SecurityAlgorithmSuite
    {
      get
      {
        return this.securityAlgorithmSuite;
      }
      set
      {
        this.CommunicationObject.ThrowIfDisposedOrImmutable();
        this.securityAlgorithmSuite = value;
      }
    }

    public SecurityKeyEntropyMode KeyEntropyMode
    {
      get
      {
        return this.keyEntropyMode;
      }
      set
      {
        this.CommunicationObject.ThrowIfDisposedOrImmutable();
        SecurityKeyEntropyModeHelper.Validate(value);
        this.keyEntropyMode = value;
      }
    }

    private MessageVersion MessageVersion
    {
      get
      {
        return this.messageVersion;
      }
    }

    public EndpointAddress TargetAddress
    {
      get
      {
        return this.targetAddress;
      }
      set
      {
        this.CommunicationObject.ThrowIfDisposedOrImmutable();
        this.targetAddress = value;
      }
    }

    public EndpointAddress LocalAddress
    {
      get
      {
        return this.localAddress;
      }
      set
      {
        this.CommunicationObject.ThrowIfDisposedOrImmutable();
        this.localAddress = value;
      }
    }

    public Uri Via
    {
      get
      {
        return this.via;
      }
      set
      {
        this.CommunicationObject.ThrowIfDisposedOrImmutable();
        this.via = value;
      }
    }

    public BindingContext IssuerBindingContext
    {
      get
      {
        return this.issuerBindingContext;
      }
      set
      {
        this.CommunicationObject.ThrowIfDisposedOrImmutable();
        if (value == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
        this.issuerBindingContext = value.Clone();
      }
    }

    public SecurityBindingElement BootstrapSecurityBindingElement
    {
      get
      {
        return this.bootstrapSecurityBindingElement;
      }
      set
      {
        this.CommunicationObject.ThrowIfDisposedOrImmutable();
        if (value == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
        this.bootstrapSecurityBindingElement = (SecurityBindingElement) value.Clone();
      }
    }

    public SecurityStandardsManager StandardsManager
    {
      get
      {
        return this.standardsManager;
      }
      set
      {
        this.CommunicationObject.ThrowIfDisposedOrImmutable();
        if (value == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("value"));
        if (value.TrustDriver == null)
        {
            throw new NullReferenceException("value.TrustDriver is NULL");
        }
        if (!value.TrustDriver.IsSessionSupported)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR.GetString("TrustDriverVersionDoesNotSupportSession"), "value"));
        if (!value.SecureConversationDriver.IsSessionSupported)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR.GetString("SecureConversationDriverVersionDoesNotSupportSession"), "value"));
        this.standardsManager = value;
      }
    }

    public SecurityTokenParameters IssuedSecurityTokenParameters
    {
      get
      {
        return this.issuedTokenParameters;
      }
      set
      {
        this.CommunicationObject.ThrowIfDisposedOrImmutable();
        this.issuedTokenParameters = value;
      }
    }

    public Uri PrivacyNoticeUri
    {
      get
      {
        return this.privacyNoticeUri;
      }
      set
      {
        this.CommunicationObject.ThrowIfDisposedOrImmutable();
        this.privacyNoticeUri = value;
      }
    }

    public ChannelParameterCollection ChannelParameters
    {
      get
      {
        return this.channelParameters;
      }
      set
      {
        this.CommunicationObject.ThrowIfDisposedOrImmutable();
        this.channelParameters = value;
      }
    }

    public int PrivacyNoticeVersion
    {
      get
      {
        return this.privacyNoticeVersion;
      }
      set
      {
        this.CommunicationObject.ThrowIfDisposedOrImmutable();
        this.privacyNoticeVersion = value;
      }
    }

    public virtual XmlDictionaryString IssueAction
    {
      get
      {
        return this.standardsManager.SecureConversationDriver.IssueAction;
      }
    }

    public virtual XmlDictionaryString IssueResponseAction
    {
      get
      {
        return this.standardsManager.SecureConversationDriver.IssueResponseAction;
      }
    }

    public virtual XmlDictionaryString RenewAction
    {
      get
      {
        return this.standardsManager.SecureConversationDriver.RenewAction;
      }
    }

    public virtual XmlDictionaryString RenewResponseAction
    {
      get
      {
        return this.standardsManager.SecureConversationDriver.RenewResponseAction;
      }
    }

    public virtual XmlDictionaryString CloseAction
    {
      get
      {
        return this.standardsManager.SecureConversationDriver.CloseAction;
      }
    }

    public virtual XmlDictionaryString CloseResponseAction
    {
      get
      {
        return this.standardsManager.SecureConversationDriver.CloseResponseAction;
      }
    }

    public SecuritySessionSecurityTokenProvider(System.IdentityModel.SafeFreeCredentials credentialsHandle)
    {
      this.credentialsHandle = credentialsHandle;
      this.standardsManager = SecurityStandardsManager.DefaultInstance;
      this.keyEntropyMode = SecurityKeyEntropyMode.CombinedEntropy;
    }

    public override void OnAbort()
    {
      if (this.rstChannelFactory != null)
      {
        this.rstChannelFactory.Abort();
        this.rstChannelFactory = (IChannelFactory<IRequestChannel>) null;
      }
      this.FreeCredentialsHandle();
    }

#region FROMWCF
    protected override Task<SecurityToken> GetTokenCoreAsync(CancellationToken cancellationToken)
    {
      throw new NotImplementedException("GetTokenCoreAsync not supported");
    }
#endregion

    public override void OnOpen(TimeSpan timeout)
    {
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      if (this.targetAddress == (EndpointAddress) null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("TargetAddressIsNotSet", new object[1]{ (object) this.GetType() })));
      if (this.IssuerBindingContext == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("IssuerBuildContextNotSet", new object[1]{ (object) this.GetType() })));
      if (this.IssuedSecurityTokenParameters == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("IssuedSecurityTokenParametersNotSet", new object[1]{ (object) this.GetType() })));
      if (this.BootstrapSecurityBindingElement == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("BootstrapSecurityBindingElementNotSet", new object[1]{ (object) this.GetType() })));
      if (this.SecurityAlgorithmSuite == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SecurityAlgorithmSuiteNotSet", new object[1]{ (object) this.GetType() })));
      this.InitializeFactories();
      this.rstChannelFactory.Open(timeoutHelper.RemainingTime());
      this.sctUri = this.StandardsManager.SecureConversationDriver.TokenTypeUri;
    }

    public override void OnOpening()
    {
      base.OnOpening();
      if (this.credentialsHandle != null)
        return;
      if (this.IssuerBindingContext == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("IssuerBuildContextNotSet", new object[1]{ (object) this.GetType() })));
      if (this.BootstrapSecurityBindingElement == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("BootstrapSecurityBindingElementNotSet", new object[1]{ (object) this.GetType() })));
#if FEATURE_CORECLR
      CompatibilityShim.Log("TODO skipping CredentialsHandle as it should NOT be needed");
      this.ownCredentialsHandle = false;
#else
      this.credentialsHandle = SecurityUtils.GetCredentialsHandle(this.bootstrapSecurityBindingElement, this.issuerBindingContext);
      this.ownCredentialsHandle = true;
#endif
    }

    public override void OnClose(TimeSpan timeout)
    {
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      if (this.rstChannelFactory != null)
      {
        this.rstChannelFactory.Close(timeoutHelper.RemainingTime());
        this.rstChannelFactory = (IChannelFactory<IRequestChannel>) null;
      }
      this.FreeCredentialsHandle();
    }

    private void FreeCredentialsHandle()
    {
      if (this.credentialsHandle == null)
        return;
      if (this.ownCredentialsHandle)
        this.credentialsHandle.Close();
      this.credentialsHandle = (System.IdentityModel.SafeFreeCredentials) null;
    }

    private void InitializeFactories()
    {
      ISecurityCapabilities property = this.BootstrapSecurityBindingElement.GetProperty<ISecurityCapabilities>(this.IssuerBindingContext);
      SecurityCredentialsManager credentialsManager = this.IssuerBindingContext.BindingParameters.Find<SecurityCredentialsManager>() ?? (SecurityCredentialsManager) ClientCredentials.CreateDefaultCredentials();
      BindingContext issuerBindingContext = this.IssuerBindingContext;
      this.bootstrapSecurityBindingElement.ReaderQuotas = issuerBindingContext.GetInnerProperty<XmlDictionaryReaderQuotas>();
      if (this.bootstrapSecurityBindingElement.ReaderQuotas == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("EncodingBindingElementDoesNotHandleReaderQuotas")));
      TransportBindingElement transportBindingElement = issuerBindingContext.RemainingBindingElements.Find<TransportBindingElement>();
      if (transportBindingElement != null)
        this.bootstrapSecurityBindingElement.MaxReceivedMessageSize = transportBindingElement.MaxReceivedMessageSize;
      SecurityProtocolFactory securityProtocolFactory1 = this.BootstrapSecurityBindingElement.CreateSecurityProtocolFactory<IRequestChannel>(this.IssuerBindingContext.Clone(), credentialsManager, false, this.IssuerBindingContext.Clone());
      if (securityProtocolFactory1 is MessageSecurityProtocolFactory)
      {
        MessageSecurityProtocolFactory securityProtocolFactory2 = securityProtocolFactory1 as MessageSecurityProtocolFactory;
        securityProtocolFactory2.ApplyConfidentiality = securityProtocolFactory2.ApplyIntegrity = securityProtocolFactory2.RequireConfidentiality = securityProtocolFactory2.RequireIntegrity = true;
        securityProtocolFactory2.ProtectionRequirements.IncomingSignatureParts.ChannelParts.IsBodyIncluded = true;
        securityProtocolFactory2.ProtectionRequirements.OutgoingSignatureParts.ChannelParts.IsBodyIncluded = true;
        MessagePartSpecification parts = new MessagePartSpecification(true);
        securityProtocolFactory2.ProtectionRequirements.IncomingSignatureParts.AddParts(parts, this.IssueAction);
        securityProtocolFactory2.ProtectionRequirements.IncomingEncryptionParts.AddParts(parts, this.IssueAction);
        securityProtocolFactory2.ProtectionRequirements.IncomingSignatureParts.AddParts(parts, this.RenewAction);
        securityProtocolFactory2.ProtectionRequirements.IncomingEncryptionParts.AddParts(parts, this.RenewAction);
        securityProtocolFactory2.ProtectionRequirements.OutgoingSignatureParts.AddParts(parts, this.IssueResponseAction);
        securityProtocolFactory2.ProtectionRequirements.OutgoingEncryptionParts.AddParts(parts, this.IssueResponseAction);
        securityProtocolFactory2.ProtectionRequirements.OutgoingSignatureParts.AddParts(parts, this.RenewResponseAction);
        securityProtocolFactory2.ProtectionRequirements.OutgoingEncryptionParts.AddParts(parts, this.RenewResponseAction);
      }
      securityProtocolFactory1.PrivacyNoticeUri = this.PrivacyNoticeUri;
      securityProtocolFactory1.PrivacyNoticeVersion = this.privacyNoticeVersion;
      if (this.localAddress != (EndpointAddress) null)
      {
        MessageFilter filter = (MessageFilter) new SessionActionFilter(this.standardsManager, new string[2]{ this.IssueResponseAction.Value, this.RenewResponseAction.Value });
        issuerBindingContext.BindingParameters.Add((object) new LocalAddressProvider(this.localAddress, filter));
      }
      ChannelBuilder channelBuilder = new ChannelBuilder(issuerBindingContext, true);
      IChannelFactory<IRequestChannel> channelFactory;
      if (channelBuilder.CanBuildChannelFactory<IRequestChannel>())
      {
        channelFactory = channelBuilder.BuildChannelFactory<IRequestChannel>();
        this.requiresManualReplyAddressing = true;
      }
      else
      {
        ServiceChannelFactory serviceChannelFactory = ServiceChannelFactory.BuildChannelFactory(channelBuilder, new ClientRuntime("RequestSecuritySession", "http://tempuri.org/") { UseSynchronizationContext = false, AddTransactionFlowProperties = false, ValidateMustUnderstand = false });
        serviceChannelFactory.ClientRuntime.Operations.Add(new ClientOperation(serviceChannelFactory.ClientRuntime, "Issue", this.IssueAction.Value)
        {
          Formatter = (IClientMessageFormatter) SecuritySessionSecurityTokenProvider.operationFormatter
        });
        serviceChannelFactory.ClientRuntime.Operations.Add(new ClientOperation(serviceChannelFactory.ClientRuntime, "Renew", this.RenewAction.Value)
        {
          Formatter = (IClientMessageFormatter) SecuritySessionSecurityTokenProvider.operationFormatter
        });
        channelFactory = (IChannelFactory<IRequestChannel>) new SecuritySessionSecurityTokenProvider.RequestChannelFactory(serviceChannelFactory);
        this.requiresManualReplyAddressing = false;
      }
      SecurityChannelFactory<IRequestChannel> securityChannelFactory = new SecurityChannelFactory<IRequestChannel>(property, this.IssuerBindingContext, channelBuilder, securityProtocolFactory1, (IChannelFactory) channelFactory);
      if (transportBindingElement != null && securityChannelFactory.SecurityProtocolFactory != null)
        securityChannelFactory.SecurityProtocolFactory.ExtendedProtectionPolicy = transportBindingElement.GetProperty<ExtendedProtectionPolicy>(issuerBindingContext);
      this.rstChannelFactory = (IChannelFactory<IRequestChannel>) securityChannelFactory;
      this.messageVersion = securityChannelFactory.MessageVersion;
    }

    protected override IAsyncResult BeginGetTokenCore(TimeSpan timeout, AsyncCallback callback, object state)
    {
      this.CommunicationObject.ThrowIfClosedOrNotOpen();
      return (IAsyncResult) new SecuritySessionSecurityTokenProvider.SessionOperationAsyncResult(this, SecuritySessionOperation.Issue, this.TargetAddress, this.Via, (SecurityToken) null, timeout, callback, state);
    }

    protected override SecurityToken EndGetTokenCore(IAsyncResult result)
    {
      return SecuritySessionSecurityTokenProvider.SessionOperationAsyncResult.End(result);
    }

    protected override SecurityToken GetTokenCore(TimeSpan timeout)
    {
      this.CommunicationObject.ThrowIfClosedOrNotOpen();
      return (SecurityToken) this.DoOperation(SecuritySessionOperation.Issue, this.targetAddress, this.via, (SecurityToken) null, timeout);
    }

    protected override IAsyncResult BeginRenewTokenCore(TimeSpan timeout, SecurityToken tokenToBeRenewed, AsyncCallback callback, object state)
    {
      this.CommunicationObject.ThrowIfClosedOrNotOpen();
      return (IAsyncResult) new SecuritySessionSecurityTokenProvider.SessionOperationAsyncResult(this, SecuritySessionOperation.Renew, this.TargetAddress, this.Via, tokenToBeRenewed, timeout, callback, state);
    }

    protected override SecurityToken EndRenewTokenCore(IAsyncResult result)
    {
      return SecuritySessionSecurityTokenProvider.SessionOperationAsyncResult.End(result);
    }

    protected override SecurityToken RenewTokenCore(TimeSpan timeout, SecurityToken tokenToBeRenewed)
    {
      this.CommunicationObject.ThrowIfClosedOrNotOpen();
      return (SecurityToken) this.DoOperation(SecuritySessionOperation.Renew, this.targetAddress, this.via, tokenToBeRenewed, timeout);
    }

    private IRequestChannel CreateChannel(SecuritySessionOperation operation, EndpointAddress target, Uri via)
    {
      if (operation != SecuritySessionOperation.Issue && operation != SecuritySessionOperation.Renew)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
      IChannelFactory<IRequestChannel> rstChannelFactory = this.rstChannelFactory;
      IRequestChannel requestChannel = !(via != (Uri) null) ? rstChannelFactory.CreateChannel(target) : rstChannelFactory.CreateChannel(target, via);
      if (this.channelParameters != null)
        this.channelParameters.PropagateChannelParameters((IChannel) requestChannel);
      if (this.ownCredentialsHandle)
      {
        ChannelParameterCollection property = requestChannel.GetProperty<ChannelParameterCollection>();
        if (property != null)
        {  
#if FEATURE_CORECLR
          throw new NotImplementedException("SspiIssuanceChannelParameter not supported in .NET Core");
#else
          property.Add((object) new SspiIssuanceChannelParameter(true, this.credentialsHandle));
#endif
        }
      }
      return requestChannel;
    }

    private Message CreateRequest(SecuritySessionOperation operation, EndpointAddress target, SecurityToken currentToken, out object requestState)
    {
      if (operation == SecuritySessionOperation.Issue)
        return this.CreateIssueRequest(target, out requestState);
      if (operation == SecuritySessionOperation.Renew)
        return this.CreateRenewRequest(target, currentToken, out requestState);
      throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
    }

    private GenericXmlSecurityToken ProcessReply(Message reply, SecuritySessionOperation operation, object requestState)
    {
      SecuritySessionSecurityTokenProvider.ThrowIfFault(reply, this.targetAddress);
      GenericXmlSecurityToken xmlSecurityToken = (GenericXmlSecurityToken) null;
      if (operation == SecuritySessionOperation.Issue)
        xmlSecurityToken = this.ProcessIssueResponse(reply, requestState);
      else if (operation == SecuritySessionOperation.Renew)
        xmlSecurityToken = this.ProcessRenewResponse(reply, requestState);
      return xmlSecurityToken;
    }

    private void OnOperationSuccess(SecuritySessionOperation operation, EndpointAddress target, SecurityToken issuedToken, SecurityToken currentToken)
    {
#if !FEATURE_CORECLR
      SecurityTraceRecordHelper.TraceSecuritySessionOperationSuccess(operation, target, currentToken, issuedToken);
#endif
    }

    private void OnOperationFailure(SecuritySessionOperation operation, EndpointAddress target, SecurityToken currentToken, Exception e, IChannel channel)
    {
#if !FEATURE_CORECLR
      SecurityTraceRecordHelper.TraceSecuritySessionOperationFailure(operation, target, currentToken, e);
#endif
      if (channel == null)
        return;
      channel.Abort();
    }

    private GenericXmlSecurityToken DoOperation(SecuritySessionOperation operation, EndpointAddress target, Uri via, SecurityToken currentToken, TimeSpan timeout)
    {
      if (target == (EndpointAddress) null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("target");
      if (operation == SecuritySessionOperation.Renew && currentToken == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("currentToken");
      IRequestChannel requestChannel = (IRequestChannel) null;
      try
      {
#if !FEATURE_CORECLR
        SecurityTraceRecordHelper.TraceBeginSecuritySessionOperation(operation, target, currentToken);
#endif
        requestChannel = this.CreateChannel(operation, target, via);
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        requestChannel.Open(timeoutHelper.RemainingTime());
        object requestState;
        GenericXmlSecurityToken issuedToken;
        using (Message request = this.CreateRequest(operation, target, currentToken, out requestState))
        {
          EventTraceActivity eventTraceActivity = (EventTraceActivity) null;
          if (TD.MessageReceivedFromTransportIsEnabled())
            eventTraceActivity = EventTraceActivityHelper.TryExtractActivity(request);
          TraceUtility.ProcessOutgoingMessage(request, eventTraceActivity);
          using (Message message = requestChannel.Request(request, timeoutHelper.RemainingTime()))
          {
            if (message == null)
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CommunicationException(SR.GetString("FailToRecieveReplyFromNegotiation")));
            if (eventTraceActivity == null && TD.MessageReceivedFromTransportIsEnabled())
              eventTraceActivity = EventTraceActivityHelper.TryExtractActivity(message);
            TraceUtility.ProcessIncomingMessage(message, eventTraceActivity);
            SecuritySessionSecurityTokenProvider.ThrowIfFault(message, this.targetAddress);
            issuedToken = this.ProcessReply(message, operation, requestState);
            this.ValidateKeySize(issuedToken);
          }
        }
        requestChannel.Close(timeoutHelper.RemainingTime());
        this.OnOperationSuccess(operation, target, (SecurityToken) issuedToken, currentToken);
        return issuedToken;
      }
      catch (Exception ex)
      {
        Exception exception = ex;
        if (Fx.IsFatal(exception))
        {
          throw;
        }
        else
        {
          if (exception is TimeoutException)
            exception = (Exception) new TimeoutException(SR.GetString("ClientSecuritySessionRequestTimeout", new object[1]
            {
              (object) timeout
            }), exception);
          this.OnOperationFailure(operation, target, currentToken, exception, (IChannel) requestChannel);
          throw;
        }
      }
    }

    private byte[] GenerateEntropy(int entropySize)
    {
#if FEATURE_CORECLR
      byte[] buffer = Fx.AllocateByteArray(entropySize / 8);
#else
      byte[] buffer = System.ServiceModel.DiagnosticUtility.Utility.AllocateByteArray(entropySize / 8);
#endif
      CryptoHelper.FillRandomBytes(buffer);
      return buffer;
    }

    private RequestSecurityToken CreateRst(EndpointAddress target, out object requestState)
    {
      RequestSecurityToken requestSecurityToken = new RequestSecurityToken(this.standardsManager);
      requestSecurityToken.KeySize = this.SecurityAlgorithmSuite.DefaultSymmetricKeyLength;
      requestSecurityToken.TokenType = this.sctUri;
      if (this.KeyEntropyMode == SecurityKeyEntropyMode.ClientEntropy || this.KeyEntropyMode == SecurityKeyEntropyMode.CombinedEntropy)
      {
        byte[] entropy = this.GenerateEntropy(requestSecurityToken.KeySize);
        requestSecurityToken.SetRequestorEntropy(entropy);
        requestState = (object) entropy;
      }
      else
        requestState = (object) null;
      return requestSecurityToken;
    }

    private void PrepareRequest(Message message)
    {
      RequestReplyCorrelator.PrepareRequest(message);
      if (this.requiresManualReplyAddressing)
        message.Headers.ReplyTo = !(this.localAddress != (EndpointAddress) null) ? EndpointAddress.AnonymousAddress : this.LocalAddress;
      if (this.webHeaderCollection == null || this.webHeaderCollection.Count <= 0)
        return;
      object obj = (object) null;
      HttpRequestMessageProperty requestMessageProperty;
      if (message.Properties.TryGetValue(HttpRequestMessageProperty.Name, out obj))
      {
        requestMessageProperty = obj as HttpRequestMessageProperty;
      }
      else
      {
        requestMessageProperty = new HttpRequestMessageProperty();
        message.Properties.Add(HttpRequestMessageProperty.Name, (object) requestMessageProperty);
      }
      if (requestMessageProperty == null || requestMessageProperty.Headers == null)
        return;
      requestMessageProperty.Headers.Add((NameValueCollection) this.webHeaderCollection);
    }

    protected virtual Message CreateIssueRequest(EndpointAddress target, out object requestState)
    {
      this.CommunicationObject.ThrowIfClosedOrNotOpen();
      RequestSecurityToken rst = this.CreateRst(target, out requestState);
      rst.RequestType = this.StandardsManager.TrustDriver.RequestTypeIssue;
      rst.MakeReadOnly();
      Message message = Message.CreateMessage(this.MessageVersion, ActionHeader.Create(this.IssueAction, this.MessageVersion.Addressing), (BodyWriter) rst);
      this.PrepareRequest(message);
      return message;
    }

    private GenericXmlSecurityToken ExtractToken(Message response, object requestState)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("RequestSecurityTokenResponse.GetIssuedToken is not supported in .NET Core");
#else
      SecurityMessageProperty security = response.Properties.Security;
      ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies = security == null || security.ServiceSecurityContext == null ? EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance : security.ServiceSecurityContext.AuthorizationPolicies;
      RequestSecurityTokenResponse securityTokenResponse = (RequestSecurityTokenResponse) null;
      XmlDictionaryReader readerAtBodyContents = response.GetReaderAtBodyContents();
      using (readerAtBodyContents)
      {
        if (this.StandardsManager.MessageSecurityVersion.TrustVersion == TrustVersion.WSTrustFeb2005)
        {
          securityTokenResponse = this.StandardsManager.TrustDriver.CreateRequestSecurityTokenResponse((XmlReader) readerAtBodyContents);
        }
        else
        {
          if (this.StandardsManager.MessageSecurityVersion.TrustVersion != TrustVersion.WSTrust13)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
          foreach (RequestSecurityTokenResponse rstr in this.StandardsManager.TrustDriver.CreateRequestSecurityTokenResponseCollection((XmlReader) readerAtBodyContents).RstrCollection)
          {
            if (securityTokenResponse != null)
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("MoreThanOneRSTRInRSTRC")));
            securityTokenResponse = rstr;
          }
        }
        response.ReadFromBodyContentsToEnd(readerAtBodyContents);
      }
      byte[] requestorEntropy = requestState == null ? (byte[]) null : (byte[]) requestState;
      return securityTokenResponse.GetIssuedToken((SecurityTokenResolver) null, (IList<SecurityTokenAuthenticator>) null, this.KeyEntropyMode, requestorEntropy, this.sctUri, authorizationPolicies, this.SecurityAlgorithmSuite.DefaultSymmetricKeyLength, false);
#endif
    }

    protected virtual GenericXmlSecurityToken ProcessIssueResponse(Message response, object requestState)
    {
      this.CommunicationObject.ThrowIfClosedOrNotOpen();
      return this.ExtractToken(response, requestState);
    }

    protected virtual Message CreateRenewRequest(EndpointAddress target, SecurityToken currentSessionToken, out object requestState)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("IssuedSecurityTokenParameters.CreateKeyIdentifierClause has an issue with the supplied parameters");
#else
      this.CommunicationObject.ThrowIfClosedOrNotOpen();
      RequestSecurityToken rst = this.CreateRst(target, out requestState);
      rst.RequestType = this.StandardsManager.TrustDriver.RequestTypeRenew;
      rst.RenewTarget = this.IssuedSecurityTokenParameters.CreateKeyIdentifierClause(currentSessionToken, SecurityTokenReferenceStyle.External);
      rst.MakeReadOnly();
      Message message = Message.CreateMessage(this.MessageVersion, ActionHeader.Create(this.RenewAction, this.MessageVersion.Addressing), (BodyWriter) rst);
      message.Properties.Security = new SecurityMessageProperty()
      {
        OutgoingSupportingTokens = {
          new SupportingTokenSpecification(currentSessionToken, EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance, SecurityTokenAttachmentMode.Endorsing, this.IssuedSecurityTokenParameters)
        }
      };
      this.PrepareRequest(message);
      return message;
#endif
    }

    protected virtual GenericXmlSecurityToken ProcessRenewResponse(Message response, object requestState)
    {
      this.CommunicationObject.ThrowIfClosedOrNotOpen();
      if (response.Headers.Action != this.RenewResponseAction.Value)
        throw TraceUtility.ThrowHelperError((Exception) new SecurityNegotiationException(SR.GetString("InvalidRenewResponseAction", new object[1]{ (object) response.Headers.Action })), response);
      return this.ExtractToken(response, requestState);
    }

    protected static void ThrowIfFault(Message message, EndpointAddress target)
    {
      System.ServiceModel.Security.SecurityUtils.ThrowIfNegotiationFault(message, target);
    }

    protected void ValidateKeySize(GenericXmlSecurityToken issuedToken)
    {
      this.CommunicationObject.ThrowIfClosedOrNotOpen();
      ReadOnlyCollection<SecurityKey> securityKeys = issuedToken.SecurityKeys;
      if (securityKeys == null || securityKeys.Count != 1)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new SecurityNegotiationException(SR.GetString("CannotObtainIssuedTokenKeySize")));
      SymmetricSecurityKey symmetricSecurityKey = securityKeys[0] as SymmetricSecurityKey;
      if (symmetricSecurityKey != null && !this.SecurityAlgorithmSuite.IsSymmetricKeyLengthSupported(symmetricSecurityKey.KeySize))
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new SecurityNegotiationException(SR.GetString("InvalidIssuedTokenKeySize", new object[1]{ (object) symmetricSecurityKey.KeySize })));
    }

    private class SessionOperationAsyncResult : System.Runtime.AsyncResult
    {
      private static AsyncCallback openChannelCallback = Fx.ThunkCallback(new AsyncCallback(SecuritySessionSecurityTokenProvider.SessionOperationAsyncResult.OpenChannelCallback));
      private static AsyncCallback closeChannelCallback = Fx.ThunkCallback(new AsyncCallback(SecuritySessionSecurityTokenProvider.SessionOperationAsyncResult.CloseChannelCallback));
      private SecuritySessionSecurityTokenProvider requestor;
      private SecuritySessionOperation operation;
      private EndpointAddress target;
      private Uri via;
      private SecurityToken currentToken;
      private GenericXmlSecurityToken issuedToken;
      private IRequestChannel channel;
      private TimeoutHelper timeoutHelper;

      public SessionOperationAsyncResult(SecuritySessionSecurityTokenProvider requestor, SecuritySessionOperation operation, EndpointAddress target, Uri via, SecurityToken currentToken, TimeSpan timeout, AsyncCallback callback, object state)
        : base(callback, state)
      {
        this.requestor = requestor;
        this.operation = operation;
        this.target = target;
        this.via = via;
        this.currentToken = currentToken;
        this.timeoutHelper = new TimeoutHelper(timeout);
#if !FEATURE_CORECLR
        SecurityTraceRecordHelper.TraceBeginSecuritySessionOperation(operation, target, currentToken);
#endif
        bool flag;
        try
        {
          flag = this.StartOperation();
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
          {
            throw;
          }
          else
          {
            this.OnOperationFailure(ex);
            throw;
          }
        }
        if (!flag)
          return;
        this.OnOperationComplete();
        this.Complete(true);
      }

      private bool StartOperation()
      {
        this.channel = this.requestor.CreateChannel(this.operation, this.target, this.via);
        IAsyncResult result = this.channel.BeginOpen(this.timeoutHelper.RemainingTime(), SecuritySessionSecurityTokenProvider.SessionOperationAsyncResult.openChannelCallback, (object) this);
        if (!result.CompletedSynchronously)
          return false;
        this.channel.EndOpen(result);
        return this.OnChannelOpened();
      }

      private static void OpenChannelCallback(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        SecuritySessionSecurityTokenProvider.SessionOperationAsyncResult asyncState = (SecuritySessionSecurityTokenProvider.SessionOperationAsyncResult) result.AsyncState;
        Exception exception = (Exception) null;
        bool flag;
        try
        {
          asyncState.channel.EndOpen(result);
          flag = asyncState.OnChannelOpened();
          if (flag)
            asyncState.OnOperationComplete();
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
          {
            throw;
          }
          else
          {
            flag = true;
            exception = ex;
            asyncState.OnOperationFailure(exception);
          }
        }
        if (!flag)
          return;
        asyncState.Complete(false, exception);
      }

      private bool OnChannelOpened()
      {
        object requestState;
        Message request = this.requestor.CreateRequest(this.operation, this.target, this.currentToken, out requestState);
        if (request == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("NullSessionRequestMessage", new object[1]{ (object) this.operation.ToString() })));
        SecuritySessionSecurityTokenProvider.ChannelOpenAsyncResultWrapper asyncResultWrapper = new SecuritySessionSecurityTokenProvider.ChannelOpenAsyncResultWrapper();
        asyncResultWrapper.Message = request;
        asyncResultWrapper.RequestState = requestState;
        bool flag = true;
        try
        {
          IAsyncResult result = this.channel.BeginRequest(request, this.timeoutHelper.RemainingTime(), Fx.ThunkCallback(new AsyncCallback(this.RequestCallback)), (object) asyncResultWrapper);
          if (result.CompletedSynchronously)
            return this.OnReplyReceived(this.channel.EndRequest(result), requestState);
          flag = false;
          return false;
        }
        finally
        {
          if (flag)
            asyncResultWrapper.Message.Close();
        }
      }

      private void RequestCallback(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        SecuritySessionSecurityTokenProvider.ChannelOpenAsyncResultWrapper asyncState = (SecuritySessionSecurityTokenProvider.ChannelOpenAsyncResultWrapper) result.AsyncState;
        object requestState = asyncState.RequestState;
        bool flag = false;
        Exception exception = (Exception) null;
        try
        {
          flag = this.OnReplyReceived(this.channel.EndRequest(result), requestState);
          if (flag)
            this.OnOperationComplete();
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
          {
            throw;
          }
          else
          {
            flag = true;
            exception = ex;
            this.OnOperationFailure(ex);
          }
        }
        finally
        {
          if (asyncState.Message != null)
            asyncState.Message.Close();
        }
        if (!flag)
          return;
        this.Complete(false, exception);
      }

      private bool OnReplyReceived(Message reply, object requestState)
      {
        if (reply == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CommunicationException(SR.GetString("FailToRecieveReplyFromNegotiation")));
        using (reply)
        {
          this.issuedToken = this.requestor.ProcessReply(reply, this.operation, requestState);
          this.requestor.ValidateKeySize(this.issuedToken);
        }
        return this.OnReplyProcessed();
      }

      private bool OnReplyProcessed()
      {
        IAsyncResult result = this.channel.BeginClose(this.timeoutHelper.RemainingTime(), SecuritySessionSecurityTokenProvider.SessionOperationAsyncResult.closeChannelCallback, (object) this);
        if (!result.CompletedSynchronously)
          return false;
        this.channel.EndClose(result);
        return true;
      }

      private static void CloseChannelCallback(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        SecuritySessionSecurityTokenProvider.SessionOperationAsyncResult asyncState = (SecuritySessionSecurityTokenProvider.SessionOperationAsyncResult) result.AsyncState;
        Exception exception = (Exception) null;
        try
        {
          asyncState.channel.EndClose(result);
          asyncState.OnOperationComplete();
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
          {
            throw;
          }
          else
          {
            exception = ex;
            asyncState.OnOperationFailure(exception);
          }
        }
        asyncState.Complete(false, exception);
      }

      private void OnOperationFailure(Exception e)
      {
        try
        {
          this.requestor.OnOperationFailure(this.operation, this.target, this.currentToken, e, (IChannel) this.channel);
        }
        catch (CommunicationException ex)
        {
          System.ServiceModel.DiagnosticUtility.TraceHandledException((Exception) ex, TraceEventType.Information);
        }
      }

      private void OnOperationComplete()
      {
        this.requestor.OnOperationSuccess(this.operation, this.target, (SecurityToken) this.issuedToken, this.currentToken);
      }

      public static SecurityToken End(IAsyncResult result)
      {
        return (SecurityToken) System.Runtime.AsyncResult.End<SecuritySessionSecurityTokenProvider.SessionOperationAsyncResult>(result).issuedToken;
      }
    }

    private class ChannelOpenAsyncResultWrapper
    {
      public object RequestState;
      public Message Message;
    }

    internal class RequestChannelFactory : ChannelFactoryBase<IRequestChannel>
    {
      private ServiceChannelFactory serviceChannelFactory;

      public RequestChannelFactory(ServiceChannelFactory serviceChannelFactory)
      {
        this.serviceChannelFactory = serviceChannelFactory;
      }

      protected override IRequestChannel OnCreateChannel(EndpointAddress address, Uri via)
      {
        return this.serviceChannelFactory.CreateChannel<IRequestChannel>(address, via);
      }

      protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
      {
        return this.serviceChannelFactory.BeginOpen(timeout, callback, state);
      }

      protected override void OnEndOpen(IAsyncResult result)
      {
        this.serviceChannelFactory.EndOpen(result);
      }

      protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("Cannot access protected member 'ChannelFactoryBase<IRequestChannel>.OnBeginClose");
#else
        return (IAsyncResult) new ChainedCloseAsyncResult(timeout, callback, state, new ChainedBeginHandler(((ChannelFactoryBase<IRequestChannel>) this).OnBeginClose), new ChainedEndHandler(((ChannelFactoryBase<IRequestChannel>) this).OnEndClose), new ICommunicationObject[1]{ (ICommunicationObject) this.serviceChannelFactory });
#endif
      }

      protected override void OnEndClose(IAsyncResult result)
      {
        ChainedAsyncResult.End(result);
      }

      protected override void OnClose(TimeSpan timeout)
      {
        base.OnClose(timeout);
        this.serviceChannelFactory.Close(timeout);
      }

      protected override void OnOpen(TimeSpan timeout)
      {
        this.serviceChannelFactory.Open(timeout);
      }

      protected override void OnAbort()
      {
        this.serviceChannelFactory.Abort();
        base.OnAbort();
      }

      public override T GetProperty<T>()
      {
        return this.serviceChannelFactory.GetProperty<T>();
      }
    }
  }
}
