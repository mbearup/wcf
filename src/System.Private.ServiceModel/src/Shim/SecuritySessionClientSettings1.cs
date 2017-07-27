// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SecuritySessionClientSettings`1
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IdentityModel.Claims;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Diagnostics.Application;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
  internal sealed class SecuritySessionClientSettings<TChannel> : IChannelSecureConversationSessionSettings, ISecurityCommunicationObject
  {
    private bool canRenewSession = true;
    private object thisLock = new object();
    private SecurityProtocolFactory sessionProtocolFactory;
    private TimeSpan keyRenewalInterval;
    private TimeSpan keyRolloverInterval;
    private bool tolerateTransportFailures;
    private System.ServiceModel.Channels.SecurityChannelFactory<TChannel> securityChannelFactory;
    private IChannelFactory innerChannelFactory;
    private ChannelBuilder channelBuilder;
    private WrapperSecurityCommunicationObject communicationObject;
    private SecurityStandardsManager standardsManager;
    private SecurityTokenParameters issuedTokenParameters;
    private int issuedTokenRenewalThreshold;

    private IChannelFactory InnerChannelFactory
    {
      get
      {
        return this.innerChannelFactory;
      }
    }

    internal ChannelBuilder ChannelBuilder
    {
      get
      {
        return this.channelBuilder;
      }
      set
      {
        this.channelBuilder = value;
      }
    }

    private System.ServiceModel.Channels.SecurityChannelFactory<TChannel> SecurityChannelFactory
    {
      get
      {
        return this.securityChannelFactory;
      }
    }

    public SecurityProtocolFactory SessionProtocolFactory
    {
      get
      {
        return this.sessionProtocolFactory;
      }
      set
      {
        this.communicationObject.ThrowIfDisposedOrImmutable();
        this.sessionProtocolFactory = value;
      }
    }

    public TimeSpan KeyRenewalInterval
    {
      get
      {
        return this.keyRenewalInterval;
      }
      set
      {
        if (value <= TimeSpan.Zero)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", SR.GetString("TimeSpanMustbeGreaterThanTimeSpanZero")));
        this.communicationObject.ThrowIfDisposedOrImmutable();
        this.keyRenewalInterval = value;
      }
    }

    public TimeSpan KeyRolloverInterval
    {
      get
      {
        return this.keyRolloverInterval;
      }
      set
      {
        if (value <= TimeSpan.Zero)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", SR.GetString("TimeSpanMustbeGreaterThanTimeSpanZero")));
        this.communicationObject.ThrowIfDisposedOrImmutable();
        this.keyRolloverInterval = value;
      }
    }

    public bool TolerateTransportFailures
    {
      get
      {
        return this.tolerateTransportFailures;
      }
      set
      {
        this.communicationObject.ThrowIfDisposedOrImmutable();
        this.tolerateTransportFailures = value;
      }
    }

    public bool CanRenewSession
    {
      get
      {
        return this.canRenewSession;
      }
      set
      {
        this.canRenewSession = value;
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
        this.communicationObject.ThrowIfDisposedOrImmutable();
        this.issuedTokenParameters = value;
      }
    }

    public SecurityStandardsManager SecurityStandardsManager
    {
      get
      {
        return this.standardsManager;
      }
      set
      {
        this.communicationObject.ThrowIfDisposedOrImmutable();
        this.standardsManager = value;
      }
    }

    public TimeSpan DefaultOpenTimeout
    {
      get
      {
        return ServiceDefaults.OpenTimeout;
      }
    }

    public TimeSpan DefaultCloseTimeout
    {
      get
      {
        return ServiceDefaults.CloseTimeout;
      }
    }

    public SecuritySessionClientSettings()
    {
      this.keyRenewalInterval = SecuritySessionClientSettings.defaultKeyRenewalInterval;
      this.keyRolloverInterval = SecuritySessionClientSettings.defaultKeyRolloverInterval;
      this.tolerateTransportFailures = true;
      this.communicationObject = new WrapperSecurityCommunicationObject((ISecurityCommunicationObject) this);
    }

    internal IChannelFactory CreateInnerChannelFactory()
    {
      if (this.ChannelBuilder.CanBuildChannelFactory<IDuplexSessionChannel>())
        return (IChannelFactory) this.ChannelBuilder.BuildChannelFactory<IDuplexSessionChannel>();
      if (this.ChannelBuilder.CanBuildChannelFactory<IDuplexChannel>())
        return (IChannelFactory) this.ChannelBuilder.BuildChannelFactory<IDuplexChannel>();
      if (this.ChannelBuilder.CanBuildChannelFactory<IRequestChannel>())
        return (IChannelFactory) this.ChannelBuilder.BuildChannelFactory<IRequestChannel>();
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
    }

    public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
    {
      return this.communicationObject.BeginClose(timeout, callback, state);
    }

    public void EndClose(IAsyncResult result)
    {
      this.communicationObject.EndClose(result);
    }

    IAsyncResult ISecurityCommunicationObject.OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("Cannot convert OperationWithTimeoutCallback to IAsyncResult");
#else
      var ret = new OperationWithTimeoutAsyncResult(new OperationWithTimeoutCallback(this.OnClose), timeout, callback, state);
      return (IAsyncResult) Convert.ChangeType(ret, typeof(IAsyncResult));
#endif
    }

    IAsyncResult ISecurityCommunicationObject.OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("Cannot convert OperationWithTimeoutCallback to IAsyncResult");
#else
      var ret =  new OperationWithTimeoutAsyncResult(new OperationWithTimeoutCallback(this.OnOpen), timeout, callback, state);
      return (IAsyncResult) Convert.ChangeType(ret, typeof(IAsyncResult));
#endif
    }

    public void OnClosed()
    {
    }

    public void OnClosing()
    {
    }

    void ISecurityCommunicationObject.OnEndClose(IAsyncResult result)
    {
      OperationWithTimeoutAsyncResult.End(result);
    }

    void ISecurityCommunicationObject.OnEndOpen(IAsyncResult result)
    {
      OperationWithTimeoutAsyncResult.End(result);
    }

    public void OnFaulted()
    {
    }

    public void OnOpened()
    {
    }

    public void OnOpening()
    {
    }

    public void OnClose(TimeSpan timeout)
    {
      if (this.sessionProtocolFactory == null)
        return;
      this.sessionProtocolFactory.Close(false, timeout);
    }

    public void OnAbort()
    {
      if (this.sessionProtocolFactory == null)
        return;
      this.sessionProtocolFactory.Close(true, TimeSpan.Zero);
    }

    public void OnOpen(TimeSpan timeout)
    {
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      if (this.sessionProtocolFactory == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SecuritySessionProtocolFactoryShouldBeSetBeforeThisOperation")));
      if (this.standardsManager == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SecurityStandardsManagerNotSet", new object[1]{ (object) this.GetType().ToString() })));
      if (this.issuedTokenParameters == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("IssuedSecurityTokenParametersNotSet", new object[1]{ (object) this.GetType() })));
      if (this.keyRenewalInterval < this.keyRolloverInterval)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("KeyRolloverGreaterThanKeyRenewal")));
      this.issuedTokenRenewalThreshold = this.sessionProtocolFactory.SecurityBindingElement.LocalClientSettings.CookieRenewalThresholdPercentage;
      this.ConfigureSessionProtocolFactory();
      this.sessionProtocolFactory.Open(true, timeoutHelper.RemainingTime());
    }

    internal void Close(TimeSpan timeout)
    {
      this.communicationObject.Close(timeout);
    }

    internal void Abort()
    {
      this.communicationObject.Abort();
    }

    internal void Open(System.ServiceModel.Channels.SecurityChannelFactory<TChannel> securityChannelFactory, IChannelFactory innerChannelFactory, ChannelBuilder channelBuilder, TimeSpan timeout)
    {
      this.securityChannelFactory = securityChannelFactory;
      this.innerChannelFactory = innerChannelFactory;
      this.channelBuilder = channelBuilder;
      this.communicationObject.Open(timeout);
    }

#if !FEATURE_CORECLR
    // MessageFilter is not supported
    internal TChannel OnCreateChannel(EndpointAddress remoteAddress, Uri via)
    {
      return this.OnCreateChannel(remoteAddress, via, (MessageFilter) null);
    }

    internal TChannel OnCreateChannel(EndpointAddress remoteAddress, Uri via, MessageFilter filter)
    {
      this.communicationObject.ThrowIfClosed();
      if (filter != null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
      if (typeof (TChannel) == typeof (IRequestSessionChannel))
        return (TChannel) new SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel(this, remoteAddress, via);
      if (typeof (TChannel) == typeof (IDuplexSessionChannel))
        return (TChannel) new SecuritySessionClientSettings<TChannel>.ClientSecurityDuplexSessionChannel(this, remoteAddress, via);
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR.GetString("ChannelTypeNotSupported", new object[1]{ (object) typeof (TChannel) }), "TChannel"));
    }
#endif

    private void ConfigureSessionProtocolFactory()
    {
      if (this.sessionProtocolFactory is SessionSymmetricMessageSecurityProtocolFactory)
      {
        AddressingVersion addressing = MessageVersion.Default.Addressing;
        if (this.channelBuilder != null)
        {
          MessageEncodingBindingElement encodingBindingElement = this.channelBuilder.Binding.Elements.Find<MessageEncodingBindingElement>();
          if (encodingBindingElement != null)
            addressing = encodingBindingElement.MessageVersion.Addressing;
        }
        if (addressing != AddressingVersion.WSAddressing10 && addressing != AddressingVersion.WSAddressingAugust2004)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ProtocolException(SR.GetString("AddressingVersionNotSupported", new object[1]{ (object) addressing })));
        SessionSymmetricMessageSecurityProtocolFactory sessionProtocolFactory = (SessionSymmetricMessageSecurityProtocolFactory) this.sessionProtocolFactory;
        if (!sessionProtocolFactory.ApplyIntegrity || !sessionProtocolFactory.RequireIntegrity)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SecuritySessionRequiresMessageIntegrity")));
        MessagePartSpecification parts = new MessagePartSpecification(true);
        sessionProtocolFactory.ProtectionRequirements.OutgoingSignatureParts.AddParts(parts, this.SecurityStandardsManager.SecureConversationDriver.CloseResponseAction);
        sessionProtocolFactory.ProtectionRequirements.OutgoingSignatureParts.AddParts(parts, this.SecurityStandardsManager.SecureConversationDriver.CloseAction);
        sessionProtocolFactory.ProtectionRequirements.OutgoingSignatureParts.AddParts(parts, addressing.FaultAction);
        sessionProtocolFactory.ProtectionRequirements.OutgoingSignatureParts.AddParts(parts, addressing.DefaultFaultAction);
        sessionProtocolFactory.ProtectionRequirements.OutgoingSignatureParts.AddParts(parts, "http://schemas.microsoft.com/ws/2006/05/security/SecureConversationFault");
        sessionProtocolFactory.ProtectionRequirements.IncomingSignatureParts.AddParts(parts, this.SecurityStandardsManager.SecureConversationDriver.CloseAction);
        sessionProtocolFactory.ProtectionRequirements.IncomingSignatureParts.AddParts(parts, this.SecurityStandardsManager.SecureConversationDriver.CloseResponseAction);
        if (sessionProtocolFactory.ApplyConfidentiality)
        {
          sessionProtocolFactory.ProtectionRequirements.IncomingEncryptionParts.AddParts(MessagePartSpecification.NoParts, this.SecurityStandardsManager.SecureConversationDriver.CloseAction);
          sessionProtocolFactory.ProtectionRequirements.IncomingEncryptionParts.AddParts(MessagePartSpecification.NoParts, this.SecurityStandardsManager.SecureConversationDriver.CloseResponseAction);
        }
        if (!sessionProtocolFactory.RequireConfidentiality)
          return;
        sessionProtocolFactory.ProtectionRequirements.OutgoingEncryptionParts.AddParts(MessagePartSpecification.NoParts, this.SecurityStandardsManager.SecureConversationDriver.CloseResponseAction);
        sessionProtocolFactory.ProtectionRequirements.OutgoingEncryptionParts.AddParts(MessagePartSpecification.NoParts, this.SecurityStandardsManager.SecureConversationDriver.CloseAction);
        sessionProtocolFactory.ProtectionRequirements.OutgoingEncryptionParts.AddParts(parts, addressing.FaultAction);
        sessionProtocolFactory.ProtectionRequirements.OutgoingEncryptionParts.AddParts(parts, addressing.DefaultFaultAction);
        sessionProtocolFactory.ProtectionRequirements.OutgoingEncryptionParts.AddParts(parts, "http://schemas.microsoft.com/ws/2006/05/security/SecureConversationFault");
      }
      else
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("SessionSymmetricTransportSecurityProtocolFactory not supported in .NET Core");
#else
        if (!(this.sessionProtocolFactory is SessionSymmetricTransportSecurityProtocolFactory))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
        SessionSymmetricTransportSecurityProtocolFactory sessionProtocolFactory = (SessionSymmetricTransportSecurityProtocolFactory) this.sessionProtocolFactory;
        sessionProtocolFactory.AddTimestamp = true;
        sessionProtocolFactory.SecurityTokenParameters.RequireDerivedKeys = false;
#endif
      }
    }

    private abstract class ClientSecuritySessionChannel : ChannelBase
    {
      private InterruptibleWaitObject inputSessionClosedHandle = new InterruptibleWaitObject(false);
      private InterruptibleWaitObject outputSessionCloseHandle = new InterruptibleWaitObject(true);
      private EndpointAddress to;
      private Uri via;
      private IClientReliableChannelBinder channelBinder;
      private ChannelParameterCollection channelParameters;
      private SecurityToken currentSessionToken;
      private SecurityToken previousSessionToken;
      private DateTime keyRenewalTime;
      private DateTime keyRolloverTime;
      private SecurityProtocol securityProtocol;
      private SecuritySessionClientSettings<TChannel> settings;
      private SecurityTokenProvider sessionTokenProvider;
      private bool isKeyRenewalOngoing;
      private InterruptibleWaitObject keyRenewalCompletedEvent;
      private bool sentClose;
      private bool receivedClose;
      private volatile bool isOutputClosed;
      private volatile bool isInputClosed;
      private bool sendCloseHandshake;
      private MessageVersion messageVersion;
      private bool isCompositeDuplexConnection;
      private Message closeResponse;
      private WebHeaderCollection webHeaderCollection;

      protected SecuritySessionClientSettings<TChannel> Settings
      {
        get
        {
          return this.settings;
        }
      }

      protected IClientReliableChannelBinder ChannelBinder
      {
        get
        {
          return this.channelBinder;
        }
      }

      public EndpointAddress RemoteAddress
      {
        get
        {
          return this.to;
        }
      }

      public Uri Via
      {
        get
        {
          return this.via;
        }
      }

      protected bool SendCloseHandshake
      {
        get
        {
          return this.sendCloseHandshake;
        }
      }

      protected EndpointAddress InternalLocalAddress
      {
        get
        {
          if (this.channelBinder != null)
            return this.channelBinder.LocalAddress;
          return (EndpointAddress) null;
        }
      }

      protected virtual bool CanDoSecurityCorrelation
      {
        get
        {
          return false;
        }
      }

      public MessageVersion MessageVersion
      {
        get
        {
          return this.messageVersion;
        }
      }

      protected bool IsInputClosed
      {
        get
        {
          return this.isInputClosed;
        }
      }

      protected bool IsOutputClosed
      {
        get
        {
          return this.isOutputClosed;
        }
      }

      protected abstract bool ExpectClose { get; }

      protected abstract string SessionId { get; }

      protected ClientSecuritySessionChannel(SecuritySessionClientSettings<TChannel> settings, EndpointAddress to, Uri via)
        : base((ChannelManagerBase) settings.SecurityChannelFactory)
      {
        this.settings = settings;
        this.to = to;
        this.via = via;
        this.keyRenewalCompletedEvent = new InterruptibleWaitObject(false);
        this.messageVersion = settings.SecurityChannelFactory.MessageVersion;
        this.channelParameters = new ChannelParameterCollection((IChannel) this);
        this.InitializeChannelBinder();
        this.webHeaderCollection = new WebHeaderCollection();
      }

      public override T GetProperty<T>()
      {
        if (typeof (T) == typeof (ChannelParameterCollection))
          return this.channelParameters as T;
        if (typeof (T) == typeof (FaultConverter) && this.channelBinder != null)
          return new SecurityChannelFaultConverter(this.channelBinder.Channel) as T;
        if (typeof (T) == typeof (WebHeaderCollection))
          return (T) Convert.ChangeType(this.webHeaderCollection, typeof(T));
        T property = base.GetProperty<T>();
        if ((object) property == null && this.channelBinder != null && this.channelBinder.Channel != null)
          property = this.channelBinder.Channel.GetProperty<T>();
        return property;
      }

      protected abstract void InitializeSession(SecurityToken sessionToken);

      private void InitializeSecurityState(SecurityToken sessionToken)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("GenericXmlSecurityTokenAuthenticator not supported in .NET Core");
#else
        this.InitializeSession(sessionToken);
        this.currentSessionToken = sessionToken;
        this.previousSessionToken = (SecurityToken) null;
        List<SecurityToken> tokens = new List<SecurityToken>(1);
        tokens.Add(sessionToken);
        ((IInitiatorSecuritySessionProtocol) this.securityProtocol).SetIdentityCheckAuthenticator((SecurityTokenAuthenticator) new GenericXmlSecurityTokenAuthenticator());
        ((IInitiatorSecuritySessionProtocol) this.securityProtocol).SetIncomingSessionTokens(tokens);
        ((IInitiatorSecuritySessionProtocol) this.securityProtocol).SetOutgoingSessionToken(sessionToken);
        if (this.CanDoSecurityCorrelation)
          ((IInitiatorSecuritySessionProtocol) this.securityProtocol).ReturnCorrelationState = true;
        this.keyRenewalTime = this.GetKeyRenewalTime(sessionToken);
#endif
      }

      private void SetupSessionTokenProvider()
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("SecurityTokenParameters.InitializeSecurityTokenRequirement is inaccessible");
#else
        InitiatorServiceModelSecurityTokenRequirement tokenRequirement = new InitiatorServiceModelSecurityTokenRequirement();
        this.Settings.IssuedSecurityTokenParameters.InitializeSecurityTokenRequirement((SecurityTokenRequirement) tokenRequirement);
        tokenRequirement.KeyUsage = SecurityKeyUsage.Signature;
        tokenRequirement.SupportSecurityContextCancellation = true;
        tokenRequirement.SecurityAlgorithmSuite = this.Settings.SessionProtocolFactory.OutgoingAlgorithmSuite;
        tokenRequirement.SecurityBindingElement = this.Settings.SessionProtocolFactory.SecurityBindingElement;
        tokenRequirement.TargetAddress = this.to;
        tokenRequirement.Via = this.Via;
        tokenRequirement.MessageSecurityVersion = this.Settings.SessionProtocolFactory.MessageSecurityVersion.SecurityTokenVersion;
        tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.PrivacyNoticeUriProperty] = (object) this.Settings.SessionProtocolFactory.PrivacyNoticeUri;
        tokenRequirement.WebHeaders = this.webHeaderCollection;
        if (this.channelParameters != null)
          tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty] = (object) this.channelParameters;
        tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.PrivacyNoticeVersionProperty] = (object) this.Settings.SessionProtocolFactory.PrivacyNoticeVersion;
        if (this.channelBinder.LocalAddress != (EndpointAddress) null)
          tokenRequirement.DuplexClientLocalAddress = this.channelBinder.LocalAddress;
        this.sessionTokenProvider = this.Settings.SessionProtocolFactory.SecurityTokenManager.CreateSecurityTokenProvider((SecurityTokenRequirement) tokenRequirement);
#endif
      }

      private void OpenCore(SecurityToken sessionToken, TimeSpan timeout)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("SecurityProtocol.Open is not supported in .NET Core");
#else
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        this.securityProtocol = this.Settings.SessionProtocolFactory.CreateSecurityProtocol(this.to, this.Via, (object) null, true, timeoutHelper.RemainingTime());
        if (!(this.securityProtocol is IInitiatorSecuritySessionProtocol))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("ProtocolMisMatch", (object) "IInitiatorSecuritySessionProtocol", (object) this.GetType().ToString())));
        this.securityProtocol.Open(timeoutHelper.RemainingTime());
        this.channelBinder.Open(timeoutHelper.RemainingTime());
        this.InitializeSecurityState(sessionToken);
#endif
      }

      protected override void OnFaulted()
      {
        this.AbortCore();
        this.inputSessionClosedHandle.Fault((CommunicationObject) this);
        this.keyRenewalCompletedEvent.Fault((CommunicationObject) this);
        this.outputSessionCloseHandle.Fault((CommunicationObject) this);
        base.OnFaulted();
      }

      protected override void OnOpen(TimeSpan timeout)
      {
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        this.SetupSessionTokenProvider();
        SecurityUtils.OpenTokenProviderIfRequired(this.sessionTokenProvider, timeoutHelper.RemainingTime());
        using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity() : (ServiceModelActivity) null)
        {
          if (DiagnosticUtility.ShouldUseActivity)
            ServiceModelActivity.Start(activity, SR.GetString("ActivitySecuritySetup"), ActivityType.SecuritySetup);
          SecurityToken token = this.sessionTokenProvider.GetToken(timeoutHelper.RemainingTime());
          this.sendCloseHandshake = true;
          this.OpenCore(token, timeoutHelper.RemainingTime());
        }
      }

      protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
      {
        ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateAsyncActivity() : (ServiceModelActivity) null;
        using (ServiceModelActivity.BoundOperation(activity, true))
        {
          if (DiagnosticUtility.ShouldUseActivity)
            ServiceModelActivity.Start(activity, SR.GetString("ActivitySecuritySetup"), ActivityType.SecuritySetup);
          return (IAsyncResult) new SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.OpenAsyncResult(this, timeout, callback, state);
        }
      }

      protected override void OnEndOpen(IAsyncResult result)
      {
        SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.OpenAsyncResult.End(result);
      }

      private void InitializeChannelBinder()
      {
        ChannelBuilder channelBuilder = this.Settings.ChannelBuilder;
        TolerateFaultsMode faultMode = this.Settings.TolerateTransportFailures ? TolerateFaultsMode.Always : TolerateFaultsMode.Never;
#if FEATURE_CORECLR
        throw new NotImplementedException("ClientReliableChannelBinder not implemented in .NET Core");
#else
        if (channelBuilder.CanBuildChannelFactory<IDuplexSessionChannel>())
          this.channelBinder = ClientReliableChannelBinder<IDuplexSessionChannel>.CreateBinder(this.RemoteAddress, this.Via, (IChannelFactory<IDuplexSessionChannel>) this.Settings.InnerChannelFactory, MaskingMode.None, faultMode, this.channelParameters, this.DefaultCloseTimeout, this.DefaultSendTimeout);
        else if (channelBuilder.CanBuildChannelFactory<IDuplexChannel>())
        {
          this.channelBinder = ClientReliableChannelBinder<IDuplexChannel>.CreateBinder(this.RemoteAddress, this.Via, (IChannelFactory<IDuplexChannel>) this.Settings.InnerChannelFactory, MaskingMode.None, faultMode, this.channelParameters, this.DefaultCloseTimeout, this.DefaultSendTimeout);
          this.isCompositeDuplexConnection = true;
        }
        else if (channelBuilder.CanBuildChannelFactory<IRequestChannel>())
          this.channelBinder = ClientReliableChannelBinder<IRequestChannel>.CreateBinder(this.RemoteAddress, this.Via, (IChannelFactory<IRequestChannel>) this.Settings.InnerChannelFactory, MaskingMode.None, faultMode, this.channelParameters, this.DefaultCloseTimeout, this.DefaultSendTimeout);
        else if (channelBuilder.CanBuildChannelFactory<IRequestSessionChannel>())
          this.channelBinder = ClientReliableChannelBinder<IRequestSessionChannel>.CreateBinder(this.RemoteAddress, this.Via, (IChannelFactory<IRequestSessionChannel>) this.Settings.InnerChannelFactory, MaskingMode.None, faultMode, this.channelParameters, this.DefaultCloseTimeout, this.DefaultSendTimeout);
        this.channelBinder.Faulted += new BinderExceptionHandler(this.OnInnerFaulted);
#endif
      }

      private void OnInnerFaulted(IReliableChannelBinder sender, Exception exception)
      {
        this.Fault(exception);
      }

      protected virtual bool OnCloseResponseReceived()
      {
        bool flag1 = false;
        bool flag2 = false;
        lock (this.ThisLock)
        {
          flag2 = this.sentClose;
          if (flag2)
          {
            if (!this.isInputClosed)
            {
              this.isInputClosed = true;
              flag1 = true;
            }
          }
        }
        if (!flag2)
        {
          this.Fault((Exception) new ProtocolException(SR.GetString("UnexpectedSecuritySessionCloseResponse")));
          return false;
        }
        if (flag1)
          this.inputSessionClosedHandle.Set();
        return true;
      }

      protected virtual bool OnCloseReceived()
      {
        if (!this.ExpectClose)
        {
          this.Fault((Exception) new ProtocolException(SR.GetString("UnexpectedSecuritySessionClose")));
          return false;
        }
        bool flag = false;
        lock (this.ThisLock)
        {
          if (!this.isInputClosed)
          {
            this.isInputClosed = true;
            this.receivedClose = true;
            flag = true;
          }
        }
        if (flag)
          this.inputSessionClosedHandle.Set();
        return true;
      }

      private Message PrepareCloseMessage()
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("SecurityTokenParameters.CreateKeyIdentifierClause is inaccessible");
#else
        SecurityToken currentSessionToken;
        lock (this.ThisLock)
          currentSessionToken = this.currentSessionToken;
        RequestSecurityToken requestSecurityToken = new RequestSecurityToken(this.Settings.SecurityStandardsManager);
        requestSecurityToken.RequestType = this.Settings.SecurityStandardsManager.TrustDriver.RequestTypeClose;
        requestSecurityToken.CloseTarget = this.Settings.IssuedSecurityTokenParameters.CreateKeyIdentifierClause(currentSessionToken, SecurityTokenReferenceStyle.External);
        requestSecurityToken.MakeReadOnly();
        Message message = Message.CreateMessage(this.MessageVersion, ActionHeader.Create(this.Settings.SecurityStandardsManager.SecureConversationDriver.CloseAction, this.MessageVersion.Addressing), (BodyWriter) requestSecurityToken);
        RequestReplyCorrelator.PrepareRequest(message);
        if (this.webHeaderCollection != null && this.webHeaderCollection.Count > 0)
        {
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
          if (requestMessageProperty != null && requestMessageProperty.Headers != null)
            requestMessageProperty.Headers.Add((NameValueCollection) this.webHeaderCollection);
        }
        if (this.InternalLocalAddress != (EndpointAddress) null)
          message.Headers.ReplyTo = this.InternalLocalAddress;
        else if (message.Version.Addressing == AddressingVersion.WSAddressing10)
          message.Headers.ReplyTo = (EndpointAddress) null;
        else if (message.Version.Addressing == AddressingVersion.WSAddressingAugust2004)
          message.Headers.ReplyTo = EndpointAddress.AnonymousAddress;
        else
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ProtocolException(SR.GetString("AddressingVersionNotSupported", new object[1]{ (object) message.Version.Addressing })));
        if (TraceUtility.PropagateUserActivity || TraceUtility.ShouldPropagateActivity)
          TraceUtility.AddAmbientActivityToMessage(message);
        return message;
#endif
      }

      protected SecurityProtocolCorrelationState SendCloseMessage(TimeSpan timeout)
      {
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        Message message = this.PrepareCloseMessage();
        SecurityProtocolCorrelationState correlationState;
        try
        {
          correlationState = this.securityProtocol.SecureOutgoingMessage(ref message, timeoutHelper.RemainingTime(), (SecurityProtocolCorrelationState) null);
          this.ChannelBinder.Send(message, timeoutHelper.RemainingTime());
        }
        finally
        {
          message.Close();
        }
        SecurityTraceRecordHelper.TraceCloseMessageSent(this.currentSessionToken, this.RemoteAddress);
        return correlationState;
      }

      protected void SendCloseResponseMessage(TimeSpan timeout)
      {
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        Message message = (Message) null;
        try
        {
          message = this.closeResponse;
          this.securityProtocol.SecureOutgoingMessage(ref message, timeoutHelper.RemainingTime(), (SecurityProtocolCorrelationState) null);
          this.ChannelBinder.Send(message, timeoutHelper.RemainingTime());
          SecurityTraceRecordHelper.TraceCloseResponseMessageSent(this.currentSessionToken, this.RemoteAddress);
        }
        finally
        {
          message.Close();
        }
      }

      private IAsyncResult BeginSendCloseMessage(TimeSpan timeout, AsyncCallback callback, object state)
      {
        using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity() : (ServiceModelActivity) null)
        {
          if (DiagnosticUtility.ShouldUseActivity)
            ServiceModelActivity.Start(activity, SR.GetString("ActivitySecurityClose"), ActivityType.SecuritySetup);
          return (IAsyncResult) new SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResult(this.PrepareCloseMessage(), this, timeout, callback, state, true);
        }
      }

      private SecurityProtocolCorrelationState EndSendCloseMessage(IAsyncResult result)
      {
        SecurityProtocolCorrelationState correlationState = SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResult.End(result);
        SecurityTraceRecordHelper.TraceCloseMessageSent(this.currentSessionToken, this.RemoteAddress);
        return correlationState;
      }

      private IAsyncResult BeginSendCloseResponseMessage(TimeSpan timeout, AsyncCallback callback, object state)
      {
        return (IAsyncResult) new SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResult(this.closeResponse, this, timeout, callback, state, true);
      }

      private void EndSendCloseResponseMessage(IAsyncResult result)
      {
        SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResult.End(result);
        SecurityTraceRecordHelper.TraceCloseResponseMessageSent(this.currentSessionToken, this.RemoteAddress);
      }

      private MessageFault GetProtocolFault(ref Message message, out bool isKeyRenewalFault, out bool isSessionAbortedFault)
      {
        isKeyRenewalFault = false;
        isSessionAbortedFault = false;
        MessageFault messageFault = (MessageFault) null;
        using (MessageBuffer bufferedCopy = message.CreateBufferedCopy(int.MaxValue))
        {
          message = bufferedCopy.CreateMessage();
          MessageFault fault = MessageFault.CreateFault(bufferedCopy.CreateMessage(), 16384);
          if (fault.Code.IsSenderFault)
          {
            FaultCode subCode = fault.Code.SubCode;
            if (subCode != null)
            {
#if FEATURE_CORECLR
              throw new NotImplementedException("SecurityProtocol.SecurityProtocolFactory is not supported in .NET Core");
#else
              SecureConversationDriver conversationDriver = this.securityProtocol.SecurityProtocolFactory.StandardsManager.SecureConversationDriver;
              if (subCode.Namespace == conversationDriver.Namespace.Value && subCode.Name == conversationDriver.RenewNeededFaultCode.Value)
              {
                messageFault = fault;
                isKeyRenewalFault = true;
              }
              else if (subCode.Namespace == "http://schemas.microsoft.com/ws/2006/05/security")
              {
                if (subCode.Name == "SecuritySessionAborted")
                {
                  messageFault = fault;
                  isSessionAbortedFault = true;
                }
              }
#endif
            }
          }
        }
        return messageFault;
      }

      private void ProcessKeyRenewalFault()
      {
        SecurityTraceRecordHelper.TraceSessionKeyRenewalFault(this.currentSessionToken, this.RemoteAddress);
        lock (this.ThisLock)
          this.keyRenewalTime = DateTime.UtcNow;
      }

      private void ProcessSessionAbortedFault(MessageFault sessionAbortedFault)
      {
        SecurityTraceRecordHelper.TraceRemoteSessionAbortedFault(this.currentSessionToken, this.RemoteAddress);
        this.Fault((Exception) new FaultException(sessionAbortedFault));
      }

      private void ProcessCloseResponse(Message response)
      {
        if (response.Headers.Action != this.Settings.SecurityStandardsManager.SecureConversationDriver.CloseResponseAction.Value)
          throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("InvalidCloseResponseAction", new object[1]{ (object) response.Headers.Action })), response);
        RequestSecurityTokenResponse securityTokenResponse = (RequestSecurityTokenResponse) null;
        XmlDictionaryReader readerAtBodyContents = response.GetReaderAtBodyContents();
        using (readerAtBodyContents)
        {
          if (this.Settings.SecurityStandardsManager.MessageSecurityVersion.TrustVersion == TrustVersion.WSTrustFeb2005)
          {
            securityTokenResponse = this.Settings.SecurityStandardsManager.TrustDriver.CreateRequestSecurityTokenResponse((XmlReader) readerAtBodyContents);
          }
          else
          {
            if (this.Settings.SecurityStandardsManager.MessageSecurityVersion.TrustVersion != TrustVersion.WSTrust13)
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
            foreach (RequestSecurityTokenResponse rstr in this.Settings.SecurityStandardsManager.TrustDriver.CreateRequestSecurityTokenResponseCollection((XmlReader) readerAtBodyContents).RstrCollection)
            {
              if (securityTokenResponse != null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("MoreThanOneRSTRInRSTRC")));
              securityTokenResponse = rstr;
            }
          }
          response.ReadFromBodyContentsToEnd(readerAtBodyContents);
        }
        if (!securityTokenResponse.IsRequestedTokenClosed)
          throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("SessionTokenWasNotClosed")), response);
      }

      private void PrepareReply(Message request, Message reply)
      {
        if (request.Headers.ReplyTo != (EndpointAddress) null)
          request.Headers.ReplyTo.ApplyTo(reply);
        else if (request.Headers.From != (EndpointAddress) null)
          request.Headers.From.ApplyTo(reply);
        if (request.Headers.MessageId != (UniqueId) null)
          reply.Headers.RelatesTo = request.Headers.MessageId;
        TraceUtility.CopyActivity(request, reply);
        if (!TraceUtility.PropagateUserActivity && !TraceUtility.ShouldPropagateActivity)
          return;
        TraceUtility.AddActivityHeader(reply);
      }

      private bool DoesSkiClauseMatchSigningToken(SecurityContextKeyIdentifierClause skiClause, Message request)
      {
        if (this.SessionId == null)
          return false;
        return skiClause.ContextId.ToString() == this.SessionId;
      }

      private void ProcessCloseMessage(Message message)
      {
        XmlDictionaryReader readerAtBodyContents = message.GetReaderAtBodyContents();
        RequestSecurityToken requestSecurityToken;
        using (readerAtBodyContents)
        {
          requestSecurityToken = this.Settings.SecurityStandardsManager.TrustDriver.CreateRequestSecurityToken((XmlReader) readerAtBodyContents);
          message.ReadFromBodyContentsToEnd(readerAtBodyContents);
        }
        if (requestSecurityToken.RequestType != null && requestSecurityToken.RequestType != this.Settings.SecurityStandardsManager.TrustDriver.RequestTypeClose)
          throw TraceUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("InvalidRstRequestType", new object[1]{ (object) requestSecurityToken.RequestType })), message);
        if (requestSecurityToken.CloseTarget == null)
          throw TraceUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("NoCloseTargetSpecified")), message);
        SecurityContextKeyIdentifierClause closeTarget = requestSecurityToken.CloseTarget as SecurityContextKeyIdentifierClause;
        if (closeTarget == null || !this.DoesSkiClauseMatchSigningToken(closeTarget, message))
          throw TraceUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("BadCloseTarget", new object[1]{ (object) requestSecurityToken.CloseTarget })), message);
        RequestSecurityTokenResponse securityTokenResponse = new RequestSecurityTokenResponse(this.Settings.SecurityStandardsManager);
        securityTokenResponse.Context = requestSecurityToken.Context;
        securityTokenResponse.IsRequestedTokenClosed = true;
        securityTokenResponse.MakeReadOnly();
        Message message1;
        if (this.Settings.SecurityStandardsManager.MessageSecurityVersion.TrustVersion == TrustVersion.WSTrustFeb2005)
        {
          message1 = Message.CreateMessage(message.Version, ActionHeader.Create(this.Settings.SecurityStandardsManager.SecureConversationDriver.CloseResponseAction, message.Version.Addressing), (BodyWriter) securityTokenResponse);
        }
        else
        {
          if (this.Settings.SecurityStandardsManager.MessageSecurityVersion.TrustVersion != TrustVersion.WSTrust13)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
          RequestSecurityTokenResponseCollection responseCollection = new RequestSecurityTokenResponseCollection((IEnumerable<RequestSecurityTokenResponse>) new List<RequestSecurityTokenResponse>() { securityTokenResponse }, this.Settings.SecurityStandardsManager);
          message1 = Message.CreateMessage(message.Version, ActionHeader.Create(this.Settings.SecurityStandardsManager.SecureConversationDriver.CloseResponseAction, message.Version.Addressing), (BodyWriter) responseCollection);
        }
        this.PrepareReply(message, message1);
        this.closeResponse = message1;
      }

      private bool ShouldWrapException(Exception e)
      {
        if (!(e is FormatException))
          return e is XmlException;
        return true;
      }

      protected Message ProcessIncomingMessage(Message message, TimeSpan timeout, SecurityProtocolCorrelationState correlationState, out MessageFault protocolFault)
      {
        protocolFault = (MessageFault) null;
        lock (this.ThisLock)
          this.DoKeyRolloverIfNeeded();
        try
        {
          this.VerifyIncomingMessage(ref message, timeout, correlationState);
          string action = message.Headers.Action;
          if (action == this.Settings.SecurityStandardsManager.SecureConversationDriver.CloseResponseAction.Value)
          {
            SecurityTraceRecordHelper.TraceCloseResponseReceived(this.currentSessionToken, this.RemoteAddress);
            this.ProcessCloseResponse(message);
            this.OnCloseResponseReceived();
          }
          else if (action == this.Settings.SecurityStandardsManager.SecureConversationDriver.CloseAction.Value)
          {
            SecurityTraceRecordHelper.TraceCloseMessageReceived(this.currentSessionToken, this.RemoteAddress);
            this.ProcessCloseMessage(message);
            this.OnCloseReceived();
          }
          else
          {
            if (!(action == "http://schemas.microsoft.com/ws/2006/05/security/SecureConversationFault"))
              return message;
            bool isKeyRenewalFault;
            bool isSessionAbortedFault;
            protocolFault = this.GetProtocolFault(ref message, out isKeyRenewalFault, out isSessionAbortedFault);
            if (isKeyRenewalFault)
            {
              this.ProcessKeyRenewalFault();
            }
            else
            {
              if (!isSessionAbortedFault)
                return message;
              this.ProcessSessionAbortedFault(protocolFault);
            }
          }
        }
        catch (Exception ex)
        {
          if (!(ex is CommunicationException) && !(ex is TimeoutException) && (!Fx.IsFatal(ex) && this.ShouldWrapException(ex)))
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("MessageSecurityVerificationFailed"), ex));
          throw;
        }
        message.Close();
        return (Message) null;
      }

      protected Message ProcessRequestContext(System.ServiceModel.Channels.RequestContext requestContext, TimeSpan timeout, SecurityProtocolCorrelationState correlationState)
      {
        if (requestContext == null)
          return (Message) null;
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        Message requestMessage = requestContext.RequestMessage;
        Message message = requestMessage;
        try
        {
          Exception exception = (Exception) null;
          try
          {
            MessageFault protocolFault;
            return this.ProcessIncomingMessage(requestMessage, timeoutHelper.RemainingTime(), correlationState, out protocolFault);
          }
          catch (MessageSecurityException ex)
          {
            if (!this.isCompositeDuplexConnection)
            {
              if (message.IsFault)
              {
                MessageFault fault = MessageFault.CreateFault(message, 16384);
                if (SecurityUtils.IsSecurityFault(fault, this.settings.sessionProtocolFactory.StandardsManager))
                  exception = SecurityUtils.CreateSecurityFaultException(fault);
              }
              else
                exception = (Exception) ex;
            }
          }
          if (exception != null)
          {
            this.Fault(exception);
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
          }
          return (Message) null;
        }
        finally
        {
          requestContext.Close(timeoutHelper.RemainingTime());
        }
      }

      private void DoKeyRolloverIfNeeded()
      {
        if (!(DateTime.UtcNow >= this.keyRolloverTime) || this.previousSessionToken == null)
          return;
        SecurityTraceRecordHelper.TracePreviousSessionKeyDiscarded(this.previousSessionToken, this.currentSessionToken, this.RemoteAddress);
        this.previousSessionToken = (SecurityToken) null;
        ((IInitiatorSecuritySessionProtocol) this.securityProtocol).SetIncomingSessionTokens(new List<SecurityToken>(1)
        {
          this.currentSessionToken
        });
      }

      private DateTime GetKeyRenewalTime(SecurityToken token)
      {
        DateTime dateTime1 = token.ValidTo;
        long ticks1 = dateTime1.Ticks;
        dateTime1 = token.ValidFrom;
        long ticks2 = dateTime1.Ticks;
        TimeSpan timeout = TimeSpan.FromTicks((ticks1 - ticks2) * (long) this.settings.issuedTokenRenewalThreshold / 100L);
        DateTime dateTime2 = TimeoutHelper.Add(token.ValidFrom, timeout);
        DateTime dateTime3 = TimeoutHelper.Add(token.ValidFrom, this.settings.keyRenewalInterval);
        if (dateTime2 < dateTime3)
          return dateTime2;
        return dateTime3;
      }

      private bool IsKeyRenewalNeeded()
      {
        return DateTime.UtcNow >= this.keyRenewalTime;
      }

      private void UpdateSessionTokens(SecurityToken newToken)
      {
        lock (this.ThisLock)
        {
          this.previousSessionToken = this.currentSessionToken;
          this.keyRolloverTime = TimeoutHelper.Add(DateTime.UtcNow, this.Settings.KeyRolloverInterval);
          this.currentSessionToken = newToken;
          this.keyRenewalTime = this.GetKeyRenewalTime(newToken);
          ((IInitiatorSecuritySessionProtocol) this.securityProtocol).SetIncomingSessionTokens(new List<SecurityToken>(2)
          {
            this.previousSessionToken,
            this.currentSessionToken
          });
          ((IInitiatorSecuritySessionProtocol) this.securityProtocol).SetOutgoingSessionToken(this.currentSessionToken);
          SecurityTraceRecordHelper.TraceSessionKeyRenewed(this.currentSessionToken, this.previousSessionToken, this.RemoteAddress);
        }
      }

      private void RenewKey(TimeSpan timeout)
      {
        if (!this.settings.CanRenewSession)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new SessionKeyExpiredException(SR.GetString("SessionKeyRenewalNotSupported")));
        bool flag;
        lock (this.ThisLock)
        {
          if (!this.isKeyRenewalOngoing)
          {
            this.isKeyRenewalOngoing = true;
            this.keyRenewalCompletedEvent.Reset();
            flag = true;
          }
          else
            flag = false;
        }
        if (flag)
        {
          try
          {
            using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity() : (ServiceModelActivity) null)
            {
              if (DiagnosticUtility.ShouldUseActivity)
                ServiceModelActivity.Start(activity, SR.GetString("ActivitySecurityRenew"), ActivityType.SecuritySetup);
              this.UpdateSessionTokens(this.sessionTokenProvider.RenewToken(timeout, this.currentSessionToken));
            }
          }
          finally
          {
            lock (this.ThisLock)
            {
              this.isKeyRenewalOngoing = false;
              this.keyRenewalCompletedEvent.Set();
            }
          }
        }
        else
        {
          this.keyRenewalCompletedEvent.Wait(timeout);
          lock (this.ThisLock)
          {
            if (this.IsKeyRenewalNeeded())
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new SessionKeyExpiredException(SR.GetString("UnableToRenewSessionKey")));
          }
        }
      }

      private bool CheckIfKeyRenewalNeeded()
      {
        bool flag = false;
        lock (this.ThisLock)
        {
          flag = this.IsKeyRenewalNeeded();
          this.DoKeyRolloverIfNeeded();
        }
        return flag;
      }

      protected IAsyncResult BeginSecureOutgoingMessage(Message message, TimeSpan timeout, AsyncCallback callback, object state)
      {
        if (this.CheckIfKeyRenewalNeeded())
          return (IAsyncResult) new SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.KeyRenewalAsyncResult(message, this, timeout, callback, state);
        SecurityProtocolCorrelationState parameter = this.securityProtocol.SecureOutgoingMessage(ref message, timeout, (SecurityProtocolCorrelationState) null);
        return (IAsyncResult) new CompletedAsyncResult<Message, SecurityProtocolCorrelationState>(message, parameter, callback, state);
      }

      protected Message EndSecureOutgoingMessage(IAsyncResult result, out SecurityProtocolCorrelationState correlationState)
      {
        if (result is CompletedAsyncResult<Message, SecurityProtocolCorrelationState>)
          return CompletedAsyncResult<Message, SecurityProtocolCorrelationState>.End(result, out correlationState);
        TimeSpan remainingTime;
        Message message = SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.KeyRenewalAsyncResult.End(result, out remainingTime);
        correlationState = this.securityProtocol.SecureOutgoingMessage(ref message, remainingTime, (SecurityProtocolCorrelationState) null);
        return message;
      }

      protected SecurityProtocolCorrelationState SecureOutgoingMessage(ref Message message, TimeSpan timeout)
      {
        bool flag = this.CheckIfKeyRenewalNeeded();
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        if (flag)
          this.RenewKey(timeoutHelper.RemainingTime());
        return this.securityProtocol.SecureOutgoingMessage(ref message, timeoutHelper.RemainingTime(), (SecurityProtocolCorrelationState) null);
      }

      protected void VerifyIncomingMessage(ref Message message, TimeSpan timeout, SecurityProtocolCorrelationState correlationState)
      {
        this.securityProtocol.VerifyIncomingMessage(ref message, timeout, correlationState);
      }

      protected virtual void AbortCore()
      {
        if (this.channelBinder != null)
          this.channelBinder.Abort();
        if (this.sessionTokenProvider == null)
          return;
        SecurityUtils.AbortTokenProviderIfRequired(this.sessionTokenProvider);
      }

      protected virtual void CloseCore(TimeSpan timeout)
      {
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        try
        {
          if (this.channelBinder != null)
            this.channelBinder.Close(timeoutHelper.RemainingTime());
          if (this.sessionTokenProvider != null)
            SecurityUtils.CloseTokenProviderIfRequired(this.sessionTokenProvider, timeoutHelper.RemainingTime());
          this.keyRenewalCompletedEvent.Abort((CommunicationObject) this);
          this.inputSessionClosedHandle.Abort((CommunicationObject) this);
        }
        catch (CommunicationObjectAbortedException)
        {
          if (this.State == CommunicationState.Closed)
            return;
          throw;
        }
      }

      protected virtual IAsyncResult BeginCloseCore(TimeSpan timeout, AsyncCallback callback, object state)
      {
        return (IAsyncResult) new SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseCoreAsyncResult(this, timeout, callback, state);
      }

      protected virtual void EndCloseCore(IAsyncResult result)
      {
        SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseCoreAsyncResult.End(result);
      }

      protected IAsyncResult BeginReceiveInternal(TimeSpan timeout, SecurityProtocolCorrelationState correlationState, AsyncCallback callback, object state)
      {
        return (IAsyncResult) new SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.ReceiveAsyncResult(this, timeout, correlationState, callback, state);
      }

      protected Message EndReceiveInternal(IAsyncResult result)
      {
        return SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.ReceiveAsyncResult.End(result);
      }

      protected Message ReceiveInternal(TimeSpan timeout, SecurityProtocolCorrelationState correlationState)
      {
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        while (!this.isInputClosed)
        {
          System.ServiceModel.Channels.RequestContext requestContext;
          if (this.ChannelBinder.TryReceive(timeoutHelper.RemainingTime(), out requestContext))
          {
            if (requestContext == null)
              return (Message) null;
            Message message = this.ProcessRequestContext(requestContext, timeoutHelper.RemainingTime(), correlationState);
            if (message != null)
              return message;
          }
          if (timeoutHelper.RemainingTime() == TimeSpan.Zero)
            break;
        }
        return (Message) null;
      }

      protected bool CloseSession(TimeSpan timeout, out bool wasAborted)
      {
        using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity() : (ServiceModelActivity) null)
        {
          if (DiagnosticUtility.ShouldUseActivity)
            ServiceModelActivity.Start(activity, SR.GetString("ActivitySecurityClose"), ActivityType.SecuritySetup);
          TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
          wasAborted = false;
          try
          {
            this.CloseOutputSession(timeoutHelper.RemainingTime());
            return this.inputSessionClosedHandle.Wait(timeoutHelper.RemainingTime(), false);
          }
          catch (CommunicationObjectAbortedException)
          {
            if (this.State != CommunicationState.Closed)
              throw;
            else
              wasAborted = true;
          }
          return false;
        }
      }

      protected IAsyncResult BeginCloseSession(TimeSpan timeout, AsyncCallback callback, object state)
      {
        using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateAsyncActivity() : (ServiceModelActivity) null)
        {
          if (DiagnosticUtility.ShouldUseActivity)
            ServiceModelActivity.Start(activity, SR.GetString("ActivitySecurityClose"), ActivityType.SecuritySetup);
          return (IAsyncResult) new SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseSessionAsyncResult(timeout, this, callback, state);
        }
      }

      protected bool EndCloseSession(IAsyncResult result, out bool wasAborted)
      {
        return SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseSessionAsyncResult.End(result, out wasAborted);
      }

      private void DetermineCloseMessageToSend(out bool sendClose, out bool sendCloseResponse)
      {
        sendClose = false;
        sendCloseResponse = false;
        lock (this.ThisLock)
        {
          if (this.isOutputClosed)
            return;
          this.isOutputClosed = true;
          if (this.receivedClose)
          {
            sendCloseResponse = true;
          }
          else
          {
            sendClose = true;
            this.sentClose = true;
          }
          this.outputSessionCloseHandle.Reset();
        }
      }

      protected virtual SecurityProtocolCorrelationState CloseOutputSession(TimeSpan timeout)
      {
        this.ThrowIfFaulted();
        if (!this.SendCloseHandshake)
          return (SecurityProtocolCorrelationState) null;
        bool sendClose;
        bool sendCloseResponse;
        this.DetermineCloseMessageToSend(out sendClose, out sendCloseResponse);
        if (!(sendClose | sendCloseResponse))
          return (SecurityProtocolCorrelationState) null;
        try
        {
          if (sendClose)
            return this.SendCloseMessage(timeout);
          this.SendCloseResponseMessage(timeout);
          return (SecurityProtocolCorrelationState) null;
        }
        finally
        {
          this.outputSessionCloseHandle.Set();
        }
      }

      protected virtual IAsyncResult BeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state)
      {
        this.ThrowIfFaulted();
        if (!this.SendCloseHandshake)
          return (IAsyncResult) new CompletedAsyncResult(callback, state);
        bool sendClose;
        bool sendCloseResponse;
        this.DetermineCloseMessageToSend(out sendClose, out sendCloseResponse);
        if (!(sendClose | sendCloseResponse))
          return (IAsyncResult) new CompletedAsyncResult(callback, state);
        bool flag = true;
        try
        {
          IAsyncResult asyncResult = !sendClose ? this.BeginSendCloseResponseMessage(timeout, callback, state) : this.BeginSendCloseMessage(timeout, callback, state);
          flag = false;
          return asyncResult;
        }
        finally
        {
          if (flag)
            this.outputSessionCloseHandle.Set();
        }
      }

      protected virtual SecurityProtocolCorrelationState EndCloseOutputSession(IAsyncResult result)
      {
        if (result is CompletedAsyncResult)
        {
          CompletedAsyncResult.End(result);
          return (SecurityProtocolCorrelationState) null;
        }
        bool sentClose;
        lock (this.ThisLock)
          sentClose = this.sentClose;
        try
        {
          if (sentClose)
            return this.EndSendCloseMessage(result);
          this.EndSendCloseResponseMessage(result);
          return (SecurityProtocolCorrelationState) null;
        }
        finally
        {
          this.outputSessionCloseHandle.Set();
        }
      }

      protected void CheckOutputOpen()
      {
        this.ThrowIfClosedOrNotOpen();
        lock (this.ThisLock)
        {
          if (this.isOutputClosed)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new CommunicationException(SR.GetString("OutputNotExpected")));
        }
      }

      protected override void OnAbort()
      {
        this.AbortCore();
        this.inputSessionClosedHandle.Abort((CommunicationObject) this);
        this.keyRenewalCompletedEvent.Abort((CommunicationObject) this);
        this.outputSessionCloseHandle.Abort((CommunicationObject) this);
      }

      protected override void OnClose(TimeSpan timeout)
      {
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        if (this.SendCloseHandshake)
        {
          bool wasAborted;
          bool flag = this.CloseSession(timeout, out wasAborted);
          if (wasAborted)
            return;
          if (!flag)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new TimeoutException(SR.GetString("ClientSecurityCloseTimeout", new object[1]{ (object) timeout })));
          try
          {
            if (!this.outputSessionCloseHandle.Wait(timeoutHelper.RemainingTime(), false))
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new TimeoutException(SR.GetString("ClientSecurityOutputSessionCloseTimeout", new object[1]{ (object) timeoutHelper.OriginalTimeout })));
          }
          catch (CommunicationObjectAbortedException)
          {
            if (this.State == CommunicationState.Closed)
              return;
            throw;
          }
        }
        this.CloseCore(timeoutHelper.RemainingTime());
      }

      protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
      {
        return (IAsyncResult) new SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseAsyncResult(this, timeout, callback, state);
      }

      protected override void OnEndClose(IAsyncResult result)
      {
        SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseAsyncResult.End(result);
      }

      private class CloseCoreAsyncResult : TraceAsyncResult
      {
        private static AsyncCallback closeChannelBinderCallback = Fx.ThunkCallback(new AsyncCallback(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseCoreAsyncResult.ChannelBinderCloseCallback));
        private static AsyncCallback closeTokenProviderCallback = Fx.ThunkCallback(new AsyncCallback(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseCoreAsyncResult.CloseTokenProviderCallback));
        private TimeoutHelper timeoutHelper;
        private SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel channel;

        public CloseCoreAsyncResult(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
          : base(callback, state)
        {
          this.channel = channel;
          this.timeoutHelper = new TimeoutHelper(timeout);
          bool flag = false;
          if (channel.channelBinder != null)
          {
            try
            {
              IAsyncResult result = this.channel.channelBinder.BeginClose(this.timeoutHelper.RemainingTime(), SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseCoreAsyncResult.closeChannelBinderCallback, (object) this);
              if (!result.CompletedSynchronously)
                return;
              this.channel.channelBinder.EndClose(result);
            }
            catch (CommunicationObjectAbortedException)
            {
              if (this.channel.State != CommunicationState.Closed)
                throw;
              else
                flag = true;
            }
          }
          if (!flag)
            flag = this.OnChannelBinderClosed();
          if (!flag)
            return;
          this.Complete(true);
        }

        private static void ChannelBinderCloseCallback(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseCoreAsyncResult asyncState = (SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseCoreAsyncResult) result.AsyncState;
          Exception exception = (Exception) null;
          bool flag = false;
          try
          {
            try
            {
              asyncState.channel.channelBinder.EndClose(result);
            }
            catch (CommunicationObjectAbortedException)
            {
              if (asyncState.channel.State != CommunicationState.Closed)
                throw;
              else
                flag = true;
            }
            if (!flag)
              flag = asyncState.OnChannelBinderClosed();
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
            }
          }
          if (!flag)
            return;
          asyncState.Complete(false, exception);
        }

        private bool OnChannelBinderClosed()
        {
          if (this.channel.sessionTokenProvider != null)
          {
            try
            {
              IAsyncResult result = SecurityUtils.BeginCloseTokenProviderIfRequired(this.channel.sessionTokenProvider, this.timeoutHelper.RemainingTime(), SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseCoreAsyncResult.closeTokenProviderCallback, (object) this);
              if (!result.CompletedSynchronously)
                return false;
              SecurityUtils.EndCloseTokenProviderIfRequired(result);
            }
            catch (CommunicationObjectAbortedException)
            {
              if (this.channel.State == CommunicationState.Closed)
                return true;
              throw;
            }
          }
          return this.OnTokenProviderClosed();
        }

        private static void CloseTokenProviderCallback(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseCoreAsyncResult asyncState = (SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseCoreAsyncResult) result.AsyncState;
          Exception exception = (Exception) null;
          bool flag = false;
          try
          {
            try
            {
              SecurityUtils.EndCloseTokenProviderIfRequired(result);
            }
            catch (CommunicationObjectAbortedException)
            {
              if (asyncState.channel.State != CommunicationState.Closed)
                throw;
              else
                flag = true;
            }
            if (!flag)
              flag = asyncState.OnTokenProviderClosed();
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
            }
          }
          if (!flag)
            return;
          asyncState.Complete(false, exception);
        }

        private bool OnTokenProviderClosed()
        {
          this.channel.keyRenewalCompletedEvent.Abort((CommunicationObject) this.channel);
          this.channel.inputSessionClosedHandle.Abort((CommunicationObject) this.channel);
          return true;
        }

        public static void End(IAsyncResult result)
        {
          AsyncResult.End<SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseCoreAsyncResult>(result);
        }
      }

      private class ReceiveAsyncResult : TraceAsyncResult
      {
        private static AsyncCallback onReceive = Fx.ThunkCallback(new AsyncCallback(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.ReceiveAsyncResult.OnReceive));
        private SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel channel;
        private Message message;
        private SecurityProtocolCorrelationState correlationState;
        private TimeoutHelper timeoutHelper;

        public ReceiveAsyncResult(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel channel, TimeSpan timeout, SecurityProtocolCorrelationState correlationState, AsyncCallback callback, object state)
          : base(callback, state)
        {
          this.channel = channel;
          this.correlationState = correlationState;
          this.timeoutHelper = new TimeoutHelper(timeout);
          IAsyncResult result = channel.ChannelBinder.BeginTryReceive(this.timeoutHelper.RemainingTime(), SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.ReceiveAsyncResult.onReceive, (object) this);
          if (!result.CompletedSynchronously || !this.CompleteReceive(result))
            return;
          this.Complete(true);
        }

        private bool CompleteReceive(IAsyncResult result)
        {
          while (!this.channel.isInputClosed)
          {
            System.ServiceModel.Channels.RequestContext requestContext;
            if (this.channel.ChannelBinder.EndTryReceive(result, out requestContext))
            {
              if (requestContext != null)
              {
                this.message = this.channel.ProcessRequestContext(requestContext, this.timeoutHelper.RemainingTime(), this.correlationState);
                if (this.message != null || this.channel.isInputClosed)
                  break;
              }
              else
                break;
            }
            if (!(this.timeoutHelper.RemainingTime() == TimeSpan.Zero))
            {
              result = this.channel.ChannelBinder.BeginTryReceive(this.timeoutHelper.RemainingTime(), SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.ReceiveAsyncResult.onReceive, (object) this);
              if (!result.CompletedSynchronously)
                return false;
            }
            else
              break;
          }
          return true;
        }

        public static Message End(IAsyncResult result)
        {
          return AsyncResult.End<SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.ReceiveAsyncResult>(result).message;
        }

        private static void OnReceive(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.ReceiveAsyncResult asyncState = (SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.ReceiveAsyncResult) result.AsyncState;
          Exception exception = (Exception) null;
          bool flag;
          try
          {
            flag = asyncState.CompleteReceive(result);
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
            }
          }
          if (!flag)
            return;
          asyncState.Complete(false, exception);
        }
      }

      private class OpenAsyncResult : TraceAsyncResult
      {
        private static readonly AsyncCallback getTokenCallback = Fx.ThunkCallback(new AsyncCallback(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.OpenAsyncResult.GetTokenCallback));
        private static readonly AsyncCallback openTokenProviderCallback = Fx.ThunkCallback(new AsyncCallback(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.OpenAsyncResult.OpenTokenProviderCallback));
        private SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel sessionChannel;
        private TimeoutHelper timeoutHelper;

        public OpenAsyncResult(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel sessionChannel, TimeSpan timeout, AsyncCallback callback, object state)
          : base(callback, state)
        {
          this.timeoutHelper = new TimeoutHelper(timeout);
          this.sessionChannel = sessionChannel;
          this.sessionChannel.SetupSessionTokenProvider();
          IAsyncResult result = SecurityUtils.BeginOpenTokenProviderIfRequired(this.sessionChannel.sessionTokenProvider, this.timeoutHelper.RemainingTime(), SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.OpenAsyncResult.openTokenProviderCallback, (object) this);
          if (!result.CompletedSynchronously)
            return;
          SecurityUtils.EndOpenTokenProviderIfRequired(result);
          if (!this.OnTokenProviderOpened())
            return;
          this.Complete(true);
        }

        private static void OpenTokenProviderCallback(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.OpenAsyncResult asyncState = (SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.OpenAsyncResult) result.AsyncState;
          Exception exception = (Exception) null;
          bool flag;
          try
          {
            SecurityUtils.EndOpenTokenProviderIfRequired(result);
            flag = asyncState.OnTokenProviderOpened();
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
            }
          }
          if (!flag)
            return;
          asyncState.Complete(false, exception);
        }

        private bool OnTokenProviderOpened()
        {
          IAsyncResult token = this.sessionChannel.sessionTokenProvider.BeginGetToken(this.timeoutHelper.RemainingTime(), SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.OpenAsyncResult.getTokenCallback, (object) this);
          if (!token.CompletedSynchronously)
            return false;
          return this.OnTokenObtained(this.sessionChannel.sessionTokenProvider.EndGetToken(token));
        }

        private bool OnTokenObtained(SecurityToken sessionToken)
        {
          this.sessionChannel.sendCloseHandshake = true;
          this.sessionChannel.OpenCore(sessionToken, this.timeoutHelper.RemainingTime());
          return true;
        }

        private static void GetTokenCallback(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.OpenAsyncResult asyncState = (SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.OpenAsyncResult) result.AsyncState;
          try
          {
            using (ServiceModelActivity.BoundOperation(asyncState.CallbackActivity))
            {
              Exception exception = (Exception) null;
              bool flag;
              try
              {
                SecurityToken token = asyncState.sessionChannel.sessionTokenProvider.EndGetToken(result);
                flag = asyncState.OnTokenObtained(token);
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
                }
              }
              if (!flag)
                return;
              asyncState.Complete(false, exception);
            }
          }
          finally
          {
            if (asyncState.CallbackActivity != null)
              asyncState.CallbackActivity.Dispose();
          }
        }

        public static void End(IAsyncResult result)
        {
          AsyncResult.End<SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.OpenAsyncResult>(result);
          ServiceModelActivity.Stop(((TraceAsyncResult) result).CallbackActivity);
        }
      }

      private class CloseSessionAsyncResult : TraceAsyncResult
      {
        private static readonly AsyncCallback closeOutputSessionCallback = Fx.ThunkCallback(new AsyncCallback(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseSessionAsyncResult.CloseOutputSessionCallback));
        private static readonly AsyncCallback shutdownWaitCallback = Fx.ThunkCallback(new AsyncCallback(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseSessionAsyncResult.ShutdownWaitCallback));
        private SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel sessionChannel;
        private bool closeCompleted;
        private bool wasAborted;
        private TimeoutHelper timeoutHelper;

        public CloseSessionAsyncResult(TimeSpan timeout, SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel sessionChannel, AsyncCallback callback, object state)
          : base(callback, state)
        {
          this.timeoutHelper = new TimeoutHelper(timeout);
          this.sessionChannel = sessionChannel;
          bool flag = false;
          try
          {
            IAsyncResult result = this.sessionChannel.BeginCloseOutputSession(this.timeoutHelper.RemainingTime(), SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseSessionAsyncResult.closeOutputSessionCallback, (object) this);
            if (!result.CompletedSynchronously)
              return;
            this.sessionChannel.EndCloseOutputSession(result);
          }
          catch (CommunicationObjectAbortedException)
          {
            if (this.sessionChannel.State != CommunicationState.Closed)
            {
              throw;
            }
            else
            {
              flag = true;
              this.wasAborted = true;
            }
          }
          if (!this.wasAborted)
            flag = this.OnOutputSessionClosed();
          if (!flag)
            return;
          this.Complete(true);
        }

        private static void CloseOutputSessionCallback(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseSessionAsyncResult asyncState = (SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseSessionAsyncResult) result.AsyncState;
          bool flag = false;
          Exception exception = (Exception) null;
          try
          {
            try
            {
              asyncState.sessionChannel.EndCloseOutputSession(result);
            }
            catch (CommunicationObjectAbortedException)
            {
              if (asyncState.sessionChannel.State != CommunicationState.Closed)
              {
                throw;
              }
              else
              {
                asyncState.wasAborted = true;
                flag = true;
              }
            }
            if (!asyncState.wasAborted)
              flag = asyncState.OnOutputSessionClosed();
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
            }
          }
          if (!flag)
            return;
          asyncState.Complete(false, exception);
        }

        private bool OnOutputSessionClosed()
        {
          try
          {
            IAsyncResult result = this.sessionChannel.inputSessionClosedHandle.BeginWait(this.timeoutHelper.RemainingTime(), true, SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseSessionAsyncResult.shutdownWaitCallback, (object) this);
            if (!result.CompletedSynchronously)
              return false;
            this.sessionChannel.inputSessionClosedHandle.EndWait(result);
            this.closeCompleted = true;
          }
          catch (CommunicationObjectAbortedException)
          {
            if (this.sessionChannel.State != CommunicationState.Closed)
              throw;
            else
              this.wasAborted = true;
          }
          catch (TimeoutException)
          {
            this.closeCompleted = false;
          }
          return true;
        }

        private static void ShutdownWaitCallback(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseSessionAsyncResult asyncState = (SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseSessionAsyncResult) result.AsyncState;
          Exception exception = (Exception) null;
          try
          {
            asyncState.sessionChannel.inputSessionClosedHandle.EndWait(result);
            asyncState.closeCompleted = true;
          }
          catch (CommunicationObjectAbortedException)
          {
            if (asyncState.sessionChannel.State != CommunicationState.Closed)
              throw;
            else
              asyncState.wasAborted = true;
          }
          catch (TimeoutException)
          {
            asyncState.closeCompleted = false;
          }
          catch (Exception ex)
          {
            if (Fx.IsFatal(ex))
              throw;
            else
              exception = ex;
          }
          asyncState.Complete(false, exception);
        }

        public static bool End(IAsyncResult result, out bool wasAborted)
        {
          SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseSessionAsyncResult sessionAsyncResult = AsyncResult.End<SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseSessionAsyncResult>(result);
          wasAborted = sessionAsyncResult.wasAborted;
          ServiceModelActivity.Stop(sessionAsyncResult.CallbackActivity);
          return sessionAsyncResult.closeCompleted;
        }
      }

      private class CloseAsyncResult : TraceAsyncResult
      {
        private static readonly AsyncCallback closeSessionCallback = Fx.ThunkCallback(new AsyncCallback(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseAsyncResult.CloseSessionCallback));
        private static readonly AsyncCallback outputSessionClosedCallback = Fx.ThunkCallback(new AsyncCallback(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseAsyncResult.OutputSessionClosedCallback));
        private static readonly AsyncCallback closeCoreCallback = Fx.ThunkCallback(new AsyncCallback(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseAsyncResult.CloseCoreCallback));
        private SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel sessionChannel;
        private TimeoutHelper timeoutHelper;

        public CloseAsyncResult(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel sessionChannel, TimeSpan timeout, AsyncCallback callback, object state)
          : base(callback, state)
        {
          sessionChannel.ThrowIfFaulted();
          this.timeoutHelper = new TimeoutHelper(timeout);
          this.sessionChannel = sessionChannel;
          if (!sessionChannel.SendCloseHandshake)
          {
            if (!this.CloseCore())
              return;
            this.Complete(true);
          }
          else
          {
            bool wasAborted = false;
            IAsyncResult result = this.sessionChannel.BeginCloseSession(this.timeoutHelper.RemainingTime(), SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseAsyncResult.closeSessionCallback, (object) this);
            if (!result.CompletedSynchronously)
              return;
            bool flag1 = this.sessionChannel.EndCloseSession(result, out wasAborted);
            if (wasAborted)
            {
              this.Complete(true);
            }
            else
            {
              if (!flag1)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new TimeoutException(SR.GetString("ClientSecurityCloseTimeout", new object[1]{ (object) timeout })));
              bool flag2 = this.OnWaitForOutputSessionClose(out wasAborted);
              if (!(wasAborted | flag2))
                return;
              this.Complete(true);
            }
          }
        }

        private static void CloseSessionCallback(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseAsyncResult asyncState = (SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseAsyncResult) result.AsyncState;
          Exception exception = (Exception) null;
          bool flag1;
          try
          {
            bool wasAborted;
            bool flag2 = asyncState.sessionChannel.EndCloseSession(result, out wasAborted);
            if (wasAborted)
            {
              flag1 = true;
            }
            else
            {
              if (!flag2)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new TimeoutException(SR.GetString("ClientSecurityCloseTimeout", new object[1]{ (object) asyncState.timeoutHelper.OriginalTimeout })));
              flag1 = asyncState.OnWaitForOutputSessionClose(out wasAborted);
              if (wasAborted)
                flag1 = true;
            }
          }
          catch (Exception ex)
          {
            if (Fx.IsFatal(ex))
            {
              throw;
            }
            else
            {
              flag1 = true;
              exception = ex;
            }
          }
          if (!flag1)
            return;
          asyncState.Complete(false, exception);
        }

        private bool OnWaitForOutputSessionClose(out bool wasAborted)
        {
          wasAborted = false;
          bool flag = false;
          try
          {
            IAsyncResult result = this.sessionChannel.outputSessionCloseHandle.BeginWait(this.timeoutHelper.RemainingTime(), true, SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseAsyncResult.outputSessionClosedCallback, (object) this);
            if (!result.CompletedSynchronously)
              return false;
            this.sessionChannel.outputSessionCloseHandle.EndWait(result);
            flag = true;
          }
          catch (TimeoutException)
          {
            flag = false;
          }
          catch (CommunicationObjectAbortedException)
          {
            if (this.sessionChannel.State == CommunicationState.Closed)
              wasAborted = true;
            else
              throw;
          }
          if (wasAborted)
            return true;
          if (!flag)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new TimeoutException(SR.GetString("ClientSecurityOutputSessionCloseTimeout", new object[1]{ (object) this.timeoutHelper.OriginalTimeout })));
          return this.CloseCore();
        }

        private static void OutputSessionClosedCallback(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseAsyncResult asyncState = (SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseAsyncResult) result.AsyncState;
          Exception exception = (Exception) null;
          bool flag1;
          try
          {
            bool flag2 = false;
            bool flag3 = false;
            try
            {
              asyncState.sessionChannel.outputSessionCloseHandle.EndWait(result);
              flag2 = true;
            }
            catch (TimeoutException)
            {
              flag2 = false;
            }
            catch (CommunicationObjectFaultedException)
            {
              if (asyncState.sessionChannel.State == CommunicationState.Closed)
                flag3 = true;
              else
                throw;
            }
            if (!flag3)
            {
              if (!flag2)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new TimeoutException(SR.GetString("ClientSecurityOutputSessionCloseTimeout", new object[1]{ (object) asyncState.timeoutHelper.OriginalTimeout })));
              flag1 = asyncState.CloseCore();
            }
            else
              flag1 = true;
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
              flag1 = true;
            }
          }
          if (!flag1)
            return;
          asyncState.Complete(false, exception);
        }

        private bool CloseCore()
        {
          IAsyncResult result = this.sessionChannel.BeginCloseCore(this.timeoutHelper.RemainingTime(), SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseAsyncResult.closeCoreCallback, (object) this);
          if (!result.CompletedSynchronously)
            return false;
          this.sessionChannel.EndCloseCore(result);
          return true;
        }

        private static void CloseCoreCallback(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseAsyncResult asyncState = (SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseAsyncResult) result.AsyncState;
          Exception exception = (Exception) null;
          try
          {
            asyncState.sessionChannel.EndCloseCore(result);
          }
          catch (Exception ex)
          {
            if (Fx.IsFatal(ex))
              throw;
            else
              exception = ex;
          }
          asyncState.Complete(false, exception);
        }

        public static void End(IAsyncResult result)
        {
          AsyncResult.End<SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.CloseAsyncResult>(result);
        }
      }

      private class KeyRenewalAsyncResult : TraceAsyncResult
      {
        private static readonly Action<object> renewKeyCallback = new Action<object>(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.KeyRenewalAsyncResult.RenewKeyCallback);
        private Message message;
        private SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel sessionChannel;
        private TimeoutHelper timeoutHelper;

        public KeyRenewalAsyncResult(Message message, SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel sessionChannel, TimeSpan timeout, AsyncCallback callback, object state)
          : base(callback, state)
        {
          this.timeoutHelper = new TimeoutHelper(timeout);
          this.message = message;
          this.sessionChannel = sessionChannel;
          ActionItem.Schedule(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.KeyRenewalAsyncResult.renewKeyCallback, (object) this);
        }

        private static void RenewKeyCallback(object state)
        {
          SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.KeyRenewalAsyncResult renewalAsyncResult = (SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.KeyRenewalAsyncResult) state;
          Exception exception = (Exception) null;
          try
          {
            using (renewalAsyncResult.CallbackActivity == null ? (Activity) null : ServiceModelActivity.BoundOperation(renewalAsyncResult.CallbackActivity))
              renewalAsyncResult.sessionChannel.RenewKey(renewalAsyncResult.timeoutHelper.RemainingTime());
          }
          catch (Exception ex)
          {
            if (Fx.IsFatal(ex))
              throw;
            else
              exception = ex;
          }
          renewalAsyncResult.Complete(false, exception);
        }

        public static Message End(IAsyncResult result, out TimeSpan remainingTime)
        {
          SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.KeyRenewalAsyncResult renewalAsyncResult = AsyncResult.End<SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.KeyRenewalAsyncResult>(result);
          remainingTime = renewalAsyncResult.timeoutHelper.RemainingTime();
          return renewalAsyncResult.message;
        }
      }

      internal abstract class SecureSendAsyncResultBase : TraceAsyncResult
      {
        private static readonly AsyncCallback secureOutgoingMessageCallback = Fx.ThunkCallback(new AsyncCallback(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResultBase.SecureOutgoingMessageCallback));
        private Message message;
        private SecurityProtocolCorrelationState correlationState;
        private SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel sessionChannel;
        private bool didSecureOutgoingMessageCompleteSynchronously;
        private TimeoutHelper timeoutHelper;

        protected bool DidSecureOutgoingMessageCompleteSynchronously
        {
          get
          {
            return this.didSecureOutgoingMessageCompleteSynchronously;
          }
        }

        protected TimeoutHelper TimeoutHelper
        {
          get
          {
            return this.timeoutHelper;
          }
        }

        protected IClientReliableChannelBinder ChannelBinder
        {
          get
          {
            return this.sessionChannel.ChannelBinder;
          }
        }

        protected Message Message
        {
          get
          {
            return this.message;
          }
        }

        protected SecurityProtocolCorrelationState SecurityCorrelationState
        {
          get
          {
            return this.correlationState;
          }
        }

        protected SecureSendAsyncResultBase(Message message, SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel sessionChannel, TimeSpan timeout, AsyncCallback callback, object state)
          : base(callback, state)
        {
          this.message = message;
          this.sessionChannel = sessionChannel;
          this.timeoutHelper = new TimeoutHelper(timeout);
          IAsyncResult result = this.sessionChannel.BeginSecureOutgoingMessage(message, this.timeoutHelper.RemainingTime(), SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResultBase.secureOutgoingMessageCallback, (object) this);
          if (!result.CompletedSynchronously)
            return;
          this.message = this.sessionChannel.EndSecureOutgoingMessage(result, out this.correlationState);
          this.didSecureOutgoingMessageCompleteSynchronously = true;
        }

        protected abstract bool OnMessageSecured();

        private static void SecureOutgoingMessageCallback(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResultBase asyncState = (SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResultBase) result.AsyncState;
          Exception exception = (Exception) null;
          bool flag;
          try
          {
            asyncState.message = asyncState.sessionChannel.EndSecureOutgoingMessage(result, out asyncState.correlationState);
            flag = asyncState.OnMessageSecured();
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
            }
          }
          if (!flag)
            return;
          asyncState.Complete(false, exception);
        }
      }

      internal sealed class SecureSendAsyncResult : SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResultBase
      {
        private static readonly AsyncCallback sendCallback = Fx.ThunkCallback(new AsyncCallback(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResult.SendCallback));
        private bool autoCloseMessage;

        public SecureSendAsyncResult(Message message, SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel sessionChannel, TimeSpan timeout, AsyncCallback callback, object state, bool autoCloseMessage)
          : base(message, sessionChannel, timeout, callback, state)
        {
          this.autoCloseMessage = autoCloseMessage;
          if (!this.DidSecureOutgoingMessageCompleteSynchronously || !this.OnMessageSecured())
            return;
          this.Complete(true);
        }

        protected override bool OnMessageSecured()
        {
          bool flag = true;
          try
          {
            IAsyncResult result = this.ChannelBinder.BeginSend(this.Message, this.TimeoutHelper.RemainingTime(), SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResult.sendCallback, (object) this);
            if (!result.CompletedSynchronously)
            {
              flag = false;
              return false;
            }
            this.ChannelBinder.EndSend(result);
            return true;
          }
          finally
          {
            if (flag && this.autoCloseMessage && this.Message != null)
              this.Message.Close();
          }
        }

        private static void SendCallback(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResult asyncState = (SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResult) result.AsyncState;
          Exception exception = (Exception) null;
          try
          {
            asyncState.ChannelBinder.EndSend(result);
          }
          catch (Exception ex)
          {
            if (Fx.IsFatal(ex))
              throw;
            else
              exception = ex;
          }
          finally
          {
            if (asyncState.autoCloseMessage && asyncState.Message != null)
              asyncState.Message.Close();
            if (asyncState.CallbackActivity != null && DiagnosticUtility.ShouldUseActivity)
              asyncState.CallbackActivity.Stop();
          }
          asyncState.Complete(false, exception);
        }

        public static SecurityProtocolCorrelationState End(IAsyncResult result)
        {
          return AsyncResult.End<SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResult>(result).SecurityCorrelationState;
        }
      }

      protected class SoapSecurityOutputSession : ISecureConversationSession, ISecuritySession, ISession, IOutputSession
      {
        private SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel channel;
        private EndpointIdentity remoteIdentity;
        private UniqueId sessionId;
        private SecurityKeyIdentifierClause sessionTokenIdentifier;
        private SecurityStandardsManager standardsManager;

        public string Id
        {
          get
          {
            if (this.sessionId == (UniqueId) null)
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("ChannelMustBeOpenedToGetSessionId")));
            return this.sessionId.ToString();
          }
        }

        public EndpointIdentity RemoteIdentity
        {
          get
          {
            return this.remoteIdentity;
          }
        }

        public SoapSecurityOutputSession(SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel channel)
        {
          this.channel = channel;
        }

        internal void Initialize(SecurityToken sessionToken, SecuritySessionClientSettings<TChannel> settings)
        {
          if (sessionToken == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("sessionToken");
          if (settings == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("settings");
          Claim primaryIdentityClaim = SecurityUtils.GetPrimaryIdentityClaim(((GenericXmlSecurityToken) sessionToken).AuthorizationPolicies);
          if (primaryIdentityClaim != null)
            this.remoteIdentity = EndpointIdentity.CreateIdentity(primaryIdentityClaim);
          this.standardsManager = settings.SessionProtocolFactory.StandardsManager;
          this.sessionId = this.GetSessionId(sessionToken, this.standardsManager);
#if FEATURE_CORECLR
          throw new NotImplementedException("SecurityTokenParameters.CreateKeyIdentifierClause is inaccessible");
#else
          this.sessionTokenIdentifier = settings.IssuedSecurityTokenParameters.CreateKeyIdentifierClause(sessionToken, SecurityTokenReferenceStyle.External);
#endif
        }

        private UniqueId GetSessionId(SecurityToken sessionToken, SecurityStandardsManager standardsManager)
        {
          GenericXmlSecurityToken xmlSecurityToken = sessionToken as GenericXmlSecurityToken;
          if (xmlSecurityToken == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("SessionTokenIsNotGenericXmlToken", (object) sessionToken, (object) typeof (GenericXmlSecurityToken))));
          return standardsManager.SecureConversationDriver.GetSecurityContextTokenId(XmlDictionaryReader.CreateDictionaryReader((XmlReader) new XmlNodeReader((XmlNode) xmlSecurityToken.TokenXml)));
        }

        public void WriteSessionTokenIdentifier(XmlDictionaryWriter writer)
        {
          this.channel.ThrowIfDisposedOrNotOpen();
          this.standardsManager.SecurityTokenSerializer.WriteKeyIdentifierClause((XmlWriter) writer, this.sessionTokenIdentifier);
        }

        public bool TryReadSessionTokenIdentifier(XmlReader reader)
        {
          this.channel.ThrowIfDisposedOrNotOpen();
          if (!this.standardsManager.SecurityTokenSerializer.CanReadKeyIdentifierClause(reader))
            return false;
          SecurityContextKeyIdentifierClause identifierClause = this.standardsManager.SecurityTokenSerializer.ReadKeyIdentifierClause(reader) as SecurityContextKeyIdentifierClause;
          if (identifierClause != null)
            return identifierClause.Matches(this.sessionId, (UniqueId) null);
          return false;
        }
      }
    }

    private abstract class ClientSecuritySimplexSessionChannel : SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel
    {
      private SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SoapSecurityOutputSession outputSession;

      public IOutputSession Session
      {
        get
        {
          return (IOutputSession) this.outputSession;
        }
      }

      protected override bool ExpectClose
      {
        get
        {
          return false;
        }
      }

      protected override string SessionId
      {
        get
        {
          return this.Session.Id;
        }
      }

      protected ClientSecuritySimplexSessionChannel(SecuritySessionClientSettings<TChannel> settings, EndpointAddress to, Uri via)
        : base(settings, to, via)
      {
        this.outputSession = new SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SoapSecurityOutputSession((SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel) this);
      }

      protected override void InitializeSession(SecurityToken sessionToken)
      {
        this.outputSession.Initialize(sessionToken, this.Settings);
      }
    }

    private sealed class SecurityRequestSessionChannel : SecuritySessionClientSettings<TChannel>.ClientSecuritySimplexSessionChannel, IRequestSessionChannel, IRequestChannel, IChannel, ICommunicationObject, ISessionChannel<IOutputSession>
    {
      protected override bool CanDoSecurityCorrelation
      {
        get
        {
          return true;
        }
      }

      public SecurityRequestSessionChannel(SecuritySessionClientSettings<TChannel> settings, EndpointAddress to, Uri via)
        : base(settings, to, via)
      {
      }

      protected override SecurityProtocolCorrelationState CloseOutputSession(TimeSpan timeout)
      {
        this.ThrowIfFaulted();
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        SecurityProtocolCorrelationState correlationState = base.CloseOutputSession(timeoutHelper.RemainingTime());
        Message message = this.ReceiveInternal(timeoutHelper.RemainingTime(), correlationState);
        if (message == null)
          return (SecurityProtocolCorrelationState) null;
        using (message)
          throw TraceUtility.ThrowHelperWarning((Exception) ProtocolException.ReceiveShutdownReturnedNonNull(message), message);
      }

      protected override IAsyncResult BeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state)
      {
        this.ThrowIfFaulted();
        return (IAsyncResult) new SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.CloseOutputSessionAsyncResult(this, timeout, callback, state);
      }

      protected override SecurityProtocolCorrelationState EndCloseOutputSession(IAsyncResult result)
      {
        SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.CloseOutputSessionAsyncResult.End(result);
        return (SecurityProtocolCorrelationState) null;
      }

      private IAsyncResult BeginBaseCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state)
      {
        return base.BeginCloseOutputSession(timeout, callback, state);
      }

      private SecurityProtocolCorrelationState EndBaseCloseOutputSession(IAsyncResult result)
      {
        return base.EndCloseOutputSession(result);
      }

      public Message Request(Message message)
      {
        return this.Request(message, this.DefaultSendTimeout);
      }

      private Message ProcessReply(Message reply, TimeSpan timeout, SecurityProtocolCorrelationState correlationState)
      {
        if (reply == null)
          return (Message) null;
        Message message1 = reply;
        Message message2 = (Message) null;
        MessageFault protocolFault = (MessageFault) null;
        Exception exception = (Exception) null;
        try
        {
          message2 = this.ProcessIncomingMessage(reply, timeout, correlationState, out protocolFault);
        }
        catch (MessageSecurityException)
        {
          if (message1.IsFault)
          {
            MessageFault fault = MessageFault.CreateFault(message1, 16384);
            if (SecurityUtils.IsSecurityFault(fault, this.Settings.standardsManager))
              exception = SecurityUtils.CreateSecurityFaultException(fault);
          }
          if (exception == null)
            throw;
        }
        if (exception != null)
        {
          this.Fault(exception);
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
        }
        if (message2 == null && protocolFault != null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("SecuritySessionFaultReplyWasSent"), (Exception) new FaultException(protocolFault)));
        return message2;
      }

      public Message Request(Message message, TimeSpan timeout)
      {
        this.ThrowIfFaulted();
        this.CheckOutputOpen();
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        SecurityProtocolCorrelationState correlationState = this.SecureOutgoingMessage(ref message, timeoutHelper.RemainingTime());
        return this.ProcessReply(this.ChannelBinder.Request(message, timeoutHelper.RemainingTime()), timeoutHelper.RemainingTime(), correlationState);
      }

      public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state)
      {
        return this.BeginRequest(message, this.DefaultSendTimeout, callback, state);
      }

      public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
      {
        this.ThrowIfFaulted();
        this.CheckOutputOpen();
        return (IAsyncResult) new SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.SecureRequestAsyncResult(message, (SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel) this, timeout, callback, state);
      }

      public Message EndRequest(IAsyncResult result)
      {
        SecurityProtocolCorrelationState correlationState;
        TimeSpan remainingTime;
        return this.ProcessReply(SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.SecureRequestAsyncResult.EndAsReply(result, out correlationState, out remainingTime), remainingTime, correlationState);
      }

      private sealed class SecureRequestAsyncResult : SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResultBase
      {
        private static readonly AsyncCallback requestCallback = Fx.ThunkCallback(new AsyncCallback(SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.SecureRequestAsyncResult.RequestCallback));
        private Message reply;

        public SecureRequestAsyncResult(Message request, SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel sessionChannel, TimeSpan timeout, AsyncCallback callback, object state)
          : base(request, sessionChannel, timeout, callback, state)
        {
          if (!this.DidSecureOutgoingMessageCompleteSynchronously || !this.OnMessageSecured())
            return;
          this.Complete(true);
        }

        protected override bool OnMessageSecured()
        {
          IAsyncResult result = this.ChannelBinder.BeginRequest(this.Message, this.TimeoutHelper.RemainingTime(), SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.SecureRequestAsyncResult.requestCallback, (object) this);
          if (!result.CompletedSynchronously)
            return false;
          this.reply = this.ChannelBinder.EndRequest(result);
          return true;
        }

        private static void RequestCallback(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.SecureRequestAsyncResult asyncState = (SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.SecureRequestAsyncResult) result.AsyncState;
          Exception exception = (Exception) null;
          try
          {
            asyncState.reply = asyncState.ChannelBinder.EndRequest(result);
          }
          catch (Exception ex)
          {
            if (Fx.IsFatal(ex))
              throw;
            else
              exception = ex;
          }
          asyncState.Complete(false, exception);
        }

        public static Message EndAsReply(IAsyncResult result, out SecurityProtocolCorrelationState correlationState, out TimeSpan remainingTime)
        {
          SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.SecureRequestAsyncResult requestAsyncResult = AsyncResult.End<SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.SecureRequestAsyncResult>(result);
          correlationState = requestAsyncResult.SecurityCorrelationState;
          remainingTime = requestAsyncResult.TimeoutHelper.RemainingTime();
          return requestAsyncResult.reply;
        }
      }

      private class CloseOutputSessionAsyncResult : TraceAsyncResult
      {
        private static readonly AsyncCallback baseCloseOutputSessionCallback = Fx.ThunkCallback(new AsyncCallback(SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.CloseOutputSessionAsyncResult.BaseCloseOutputSessionCallback));
        private static readonly AsyncCallback receiveInternalCallback = Fx.ThunkCallback(new AsyncCallback(SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.CloseOutputSessionAsyncResult.ReceiveInternalCallback));
        private SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel requestChannel;
        private SecurityProtocolCorrelationState correlationState;
        private TimeoutHelper timeoutHelper;

        public CloseOutputSessionAsyncResult(SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel requestChannel, TimeSpan timeout, AsyncCallback callback, object state)
          : base(callback, state)
        {
          this.timeoutHelper = new TimeoutHelper(timeout);
          this.requestChannel = requestChannel;
          IAsyncResult result = this.requestChannel.BeginBaseCloseOutputSession(this.timeoutHelper.RemainingTime(), SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.CloseOutputSessionAsyncResult.baseCloseOutputSessionCallback, (object) this);
          if (!result.CompletedSynchronously)
            return;
          this.correlationState = this.requestChannel.EndBaseCloseOutputSession(result);
          if (!this.OnBaseOutputSessionClosed())
            return;
          this.Complete(true);
        }

        private static void BaseCloseOutputSessionCallback(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.CloseOutputSessionAsyncResult asyncState = (SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.CloseOutputSessionAsyncResult) result.AsyncState;
          Exception exception = (Exception) null;
          bool flag;
          try
          {
            asyncState.correlationState = asyncState.requestChannel.EndBaseCloseOutputSession(result);
            flag = asyncState.OnBaseOutputSessionClosed();
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
            }
          }
          if (!flag)
            return;
          asyncState.Complete(false, exception);
        }

        private bool OnBaseOutputSessionClosed()
        {
          IAsyncResult result = this.requestChannel.BeginReceiveInternal(this.timeoutHelper.RemainingTime(), this.correlationState, SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.CloseOutputSessionAsyncResult.receiveInternalCallback, (object) this);
          if (!result.CompletedSynchronously)
            return false;
          return this.OnMessageReceived(this.requestChannel.EndReceiveInternal(result));
        }

        private static void ReceiveInternalCallback(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.CloseOutputSessionAsyncResult asyncState = (SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.CloseOutputSessionAsyncResult) result.AsyncState;
          Exception exception = (Exception) null;
          bool flag;
          try
          {
            Message message = asyncState.requestChannel.EndReceiveInternal(result);
            flag = asyncState.OnMessageReceived(message);
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
            }
          }
          if (!flag)
            return;
          asyncState.Complete(false, exception);
        }

        private bool OnMessageReceived(Message message)
        {
          if (message == null)
            return true;
          using (message)
            throw TraceUtility.ThrowHelperWarning((Exception) ProtocolException.ReceiveShutdownReturnedNonNull(message), message);
        }

        public static void End(IAsyncResult result)
        {
          AsyncResult.End<SecuritySessionClientSettings<TChannel>.SecurityRequestSessionChannel.CloseOutputSessionAsyncResult>(result);
        }
      }
    }

    private class ClientSecurityDuplexSessionChannel : SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel, IDuplexSessionChannel, IDuplexChannel, IInputChannel, IChannel, ICommunicationObject, IOutputChannel, ISessionChannel<IDuplexSession>
    {
      private static AsyncCallback onReceive = Fx.ThunkCallback(new AsyncCallback(SecuritySessionClientSettings<TChannel>.ClientSecurityDuplexSessionChannel.OnReceive));
      private SecuritySessionClientSettings<TChannel>.ClientSecurityDuplexSessionChannel.SoapSecurityClientDuplexSession session;
      private InputQueue<Message> queue;
      private Action startReceiving;
      private Action<object> completeLater;

      public EndpointAddress LocalAddress
      {
        get
        {
          return this.InternalLocalAddress;
        }
      }

      public IDuplexSession Session
      {
        get
        {
          return (IDuplexSession) this.session;
        }
      }

      protected override bool ExpectClose
      {
        get
        {
          return true;
        }
      }

      protected override string SessionId
      {
        get
        {
          return this.session.Id;
        }
      }

      public ClientSecurityDuplexSessionChannel(SecuritySessionClientSettings<TChannel> settings, EndpointAddress to, Uri via)
        : base(settings, to, via)
      {
        this.session = new SecuritySessionClientSettings<TChannel>.ClientSecurityDuplexSessionChannel.SoapSecurityClientDuplexSession(this);
        this.queue = TraceUtility.CreateInputQueue<Message>();
        this.startReceiving = new Action(this.StartReceiving);
        this.completeLater = new Action<object>(this.CompleteLater);
      }

      public Message Receive()
      {
        return this.Receive(this.DefaultReceiveTimeout);
      }

      public Message Receive(TimeSpan timeout)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("InputChannel not supported in .NET Core");
#else
        return InputChannel.HelpReceive((IInputChannel) this, timeout);
#endif
      }

      public IAsyncResult BeginReceive(AsyncCallback callback, object state)
      {
        return this.BeginReceive(this.DefaultReceiveTimeout, callback, state);
      }

      public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("InputChannel not supported in .NET Core");
#else
        return InputChannel.HelpBeginReceive((IInputChannel) this, timeout, callback, state);
#endif
      }

      public Message EndReceive(IAsyncResult result)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("InputChannel not supported in .NET Core");
#else
        return InputChannel.HelpEndReceive(result);
#endif
      }

      public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
      {
        this.ThrowIfFaulted();
        return this.queue.BeginDequeue(timeout, callback, state);
      }

      public bool EndTryReceive(IAsyncResult result, out Message message)
      {
        bool flag = this.queue.EndDequeue(result, out message);
        if (message == null)
          this.ThrowIfFaulted();
        return flag;
      }

      protected override void OnOpened()
      {
        base.OnOpened();
        this.StartReceiving();
      }

      public bool TryReceive(TimeSpan timeout, out Message message)
      {
        this.ThrowIfFaulted();
        bool flag = this.queue.Dequeue(timeout, out message);
        if (message == null)
          this.ThrowIfFaulted();
        return flag;
      }

      public void Send(Message message)
      {
        this.Send(message, this.DefaultSendTimeout);
      }

      public void Send(Message message, TimeSpan timeout)
      {
        this.ThrowIfFaulted();
        this.CheckOutputOpen();
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        this.SecureOutgoingMessage(ref message, timeoutHelper.RemainingTime());
        this.ChannelBinder.Send(message, timeoutHelper.RemainingTime());
      }

      public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
      {
        return this.BeginSend(message, this.DefaultSendTimeout, callback, state);
      }

      public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
      {
        this.ThrowIfFaulted();
        this.CheckOutputOpen();
        return (IAsyncResult) new SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResult(message, (SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel) this, timeout, callback, state, false);
      }

      public void EndSend(IAsyncResult result)
      {
        SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SecureSendAsyncResult.End(result);
      }

      protected override void InitializeSession(SecurityToken sessionToken)
      {
        this.session.Initialize(sessionToken, this.Settings);
      }

      private void StartReceiving()
      {
        IAsyncResult asyncResult = this.IssueReceive();
        if (asyncResult == null || !asyncResult.CompletedSynchronously)
          return;
        ActionItem.Schedule(this.completeLater, (object) asyncResult);
      }

      private IAsyncResult IssueReceive()
      {
        while (this.State != CommunicationState.Closed && this.State != CommunicationState.Faulted)
        {
          if (!this.IsInputClosed)
          {
            try
            {
              return this.BeginReceiveInternal(TimeSpan.MaxValue, (SecurityProtocolCorrelationState) null, SecuritySessionClientSettings<TChannel>.ClientSecurityDuplexSessionChannel.onReceive, (object) this);
            }
            catch (CommunicationException ex)
            {
              DiagnosticUtility.TraceHandledException((Exception) ex, TraceEventType.Information);
            }
            catch (TimeoutException ex)
            {
              if (TD.ReceiveTimeoutIsEnabled())
                TD.ReceiveTimeout(ex.Message);
              DiagnosticUtility.TraceHandledException((Exception) ex, TraceEventType.Information);
            }
          }
          else
            break;
        }
        return (IAsyncResult) null;
      }

      private void CompleteLater(object obj)
      {
        this.CompleteReceive((IAsyncResult) obj);
      }

      private static void OnReceive(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        ((SecuritySessionClientSettings<TChannel>.ClientSecurityDuplexSessionChannel) result.AsyncState).CompleteReceive(result);
      }

      private void CompleteReceive(IAsyncResult result)
      {
        Message message = (Message) null;
        bool flag;
        try
        {
          message = this.EndReceiveInternal(result);
          flag = true;
        }
        catch (MessageSecurityException)
        {
          flag = false;
        }
        catch (CommunicationException ex)
        {
          flag = true;
          DiagnosticUtility.TraceHandledException((Exception) ex, TraceEventType.Information);
        }
        catch (TimeoutException ex)
        {
          flag = true;
          if (TD.ReceiveTimeoutIsEnabled())
            TD.ReceiveTimeout(ex.Message);
          DiagnosticUtility.TraceHandledException((Exception) ex, TraceEventType.Information);
        }
        if (flag)
        {
          IAsyncResult asyncResult = this.IssueReceive();
          if (asyncResult != null && asyncResult.CompletedSynchronously)
            ActionItem.Schedule(this.completeLater, (object) asyncResult);
        }
        if (message == null)
          return;
        try
        {
          this.queue.EnqueueAndDispatch(message);
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else
            DiagnosticUtility.TraceHandledException(ex, TraceEventType.Warning);
        }
      }

      protected override void AbortCore()
      {
        try
        {
          this.queue.Dispose();
        }
        catch (CommunicationException ex)
        {
          DiagnosticUtility.TraceHandledException((Exception) ex, TraceEventType.Information);
        }
        catch (TimeoutException ex)
        {
          if (TD.CloseTimeoutIsEnabled())
            TD.CloseTimeout(ex.Message);
          DiagnosticUtility.TraceHandledException((Exception) ex, TraceEventType.Information);
        }
        base.AbortCore();
      }

      public bool WaitForMessage(TimeSpan timeout)
      {
        return this.queue.WaitForItem(timeout);
      }

      public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
      {
        return this.queue.BeginWaitForItem(timeout, callback, state);
      }

      public bool EndWaitForMessage(IAsyncResult result)
      {
        return this.queue.EndWaitForItem(result);
      }

      protected override void OnFaulted()
      {
        this.queue.Shutdown((Func<Exception>) (() => this.GetPendingException()));
        base.OnFaulted();
      }

      protected override bool OnCloseResponseReceived()
      {
        if (!base.OnCloseResponseReceived())
          return false;
        this.queue.Shutdown();
        return true;
      }

      protected override bool OnCloseReceived()
      {
        if (!base.OnCloseReceived())
          return false;
        this.queue.Shutdown();
        return true;
      }

      private class SoapSecurityClientDuplexSession : SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel.SoapSecurityOutputSession, IDuplexSession, IInputSession, ISession, IOutputSession
      {
        private SecuritySessionClientSettings<TChannel>.ClientSecurityDuplexSessionChannel channel;
        private bool initialized;

        public SoapSecurityClientDuplexSession(SecuritySessionClientSettings<TChannel>.ClientSecurityDuplexSessionChannel channel)
          : base((SecuritySessionClientSettings<TChannel>.ClientSecuritySessionChannel) channel)
        {
          this.channel = channel;
        }

        internal new void Initialize(SecurityToken sessionToken, SecuritySessionClientSettings<TChannel> settings)
        {
          base.Initialize(sessionToken, settings);
          this.initialized = true;
        }

        private void CheckInitialized()
        {
          if (!this.initialized)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("ChannelNotOpen")));
        }

        public void CloseOutputSession()
        {
          this.CloseOutputSession(this.channel.DefaultCloseTimeout);
        }

        public void CloseOutputSession(TimeSpan timeout)
        {
          this.CheckInitialized();
          this.channel.ThrowIfFaulted();
          this.channel.ThrowIfNotOpened();
          Exception exception = (Exception) null;
          try
          {
            this.channel.CloseOutputSession(timeout);
          }
          catch (CommunicationObjectAbortedException)
          {
            if (this.channel.State != CommunicationState.Closed)
              throw;
          }
          catch (Exception ex)
          {
            if (Fx.IsFatal(ex))
              throw;
            else
              exception = ex;
          }
          if (exception != null)
          {
            this.channel.Fault(exception);
            throw exception;
          }
        }

        public IAsyncResult BeginCloseOutputSession(AsyncCallback callback, object state)
        {
          return this.BeginCloseOutputSession(this.channel.DefaultCloseTimeout, callback, state);
        }

        public IAsyncResult BeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state)
        {
          this.CheckInitialized();
          this.channel.ThrowIfFaulted();
          this.channel.ThrowIfNotOpened();
          Exception exception;
          try
          {
            return this.channel.BeginCloseOutputSession(timeout, callback, state);
          }
          catch (CommunicationObjectAbortedException)
          {
            if (this.channel.State == CommunicationState.Closed)
              return (IAsyncResult) new CompletedAsyncResult(callback, state);
            throw;
          }
          catch (Exception ex)
          {
            if (Fx.IsFatal(ex))
              throw;
            else
              exception = ex;
          }
          if (exception == null)
            return (IAsyncResult) null;
          this.channel.Fault(exception);
          if (exception is CommunicationException)
            throw exception;
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
        }

        public void EndCloseOutputSession(IAsyncResult result)
        {
          if (result is CompletedAsyncResult)
          {
            CompletedAsyncResult.End(result);
          }
          else
          {
            Exception exception = (Exception) null;
            try
            {
              this.channel.EndCloseOutputSession(result);
            }
            catch (CommunicationObjectAbortedException)
            {
              if (this.channel.State != CommunicationState.Closed)
                throw;
            }
            catch (Exception ex)
            {
              if (Fx.IsFatal(ex))
                throw;
              else
                exception = ex;
            }
            if (exception == null)
              return;
            this.channel.Fault(exception);
            if (exception is CommunicationException)
              throw exception;
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
          }
        }
      }
    }
  }
}
