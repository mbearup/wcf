// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime;
using System.ServiceModel.Channels;
using System.Globalization;
using System.IdentityModel.Selectors;
using System.Security.Authentication.ExtendedProtection;
#if FEATURE_CORECLR
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using System.ServiceModel.Description;
#endif

namespace System.ServiceModel.Security
{
    internal class SecurityProtocolFactory : ISecurityCommunicationObject
    {
        
        internal const bool defaultAddTimestamp = true;
        internal const bool defaultDeriveKeys = true;
        internal const bool defaultDetectReplays = true;
        internal const string defaultMaxClockSkewString = "00:05:00";
        internal const string defaultReplayWindowString = "00:05:00";
        internal static readonly TimeSpan defaultMaxClockSkew = TimeSpan.Parse(defaultMaxClockSkewString, CultureInfo.InvariantCulture);
        internal static readonly TimeSpan defaultReplayWindow = TimeSpan.Parse(defaultReplayWindowString, CultureInfo.InvariantCulture);
        internal const int defaultMaxCachedNonces = 900000;
        internal const string defaultTimestampValidityDurationString = "00:05:00";
        internal static readonly TimeSpan defaultTimestampValidityDuration = TimeSpan.Parse(defaultTimestampValidityDurationString, CultureInfo.InvariantCulture);
        internal const SecurityHeaderLayout defaultSecurityHeaderLayout = SecurityHeaderLayout.Strict;
        private ExtendedProtectionPolicy extendedProtectionPolicy;
        private MessageSecurityVersion messageSecurityVersion;
        private bool actAsInitiator;
        private WrapperSecurityCommunicationObject communicationObject;
        private SecurityAlgorithmSuite outgoingAlgorithmSuite = SecurityAlgorithmSuite.Default;
        private Uri privacyNoticeUri;
        private int privacyNoticeVersion;
        private SecurityBindingElement securityBindingElement;
        private SecurityTokenManager securityTokenManager;
        private SecurityStandardsManager standardsManager = SecurityStandardsManager.DefaultInstance;
        private BufferManager streamBufferManager;
        private bool isDuplexReply;
        private bool detectReplays = true;
        private string requestReplyErrorPropertyName;
        private bool expectKeyDerivation;
#region fromwcf
        private bool expectSupportingTokens;
        private bool addTimestamp = true;
        private SecurityAlgorithmSuite incomingAlgorithmSuite = SecurityAlgorithmSuite.Default;
        private int maxCachedNonces = 900000;
        private NonceCache nonceCache;
        private TimeSpan maxClockSkew = SecurityProtocolFactory.defaultMaxClockSkew;
        private TimeSpan replayWindow = SecurityProtocolFactory.defaultReplayWindow;
        private SecurityHeaderLayout securityHeaderLayout;
        private TimeSpan timestampValidityDuration = SecurityProtocolFactory.defaultTimestampValidityDuration;
        private Uri listenUri;
        private AuditLogLocation auditLogLocation;
        private ICollection<SupportingTokenAuthenticatorSpecification> channelSupportingTokenAuthenticatorSpecification;
        private bool suppressAuditFailure;
        private AuditLevel messageAuthenticationAuditLevel;
#endregion

    public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
    {
      return this.CommunicationObject.BeginClose(timeout, callback, state);
    }

#region fromwcf
    internal IList<SupportingTokenAuthenticatorSpecification> GetSupportingTokenAuthenticators(string action, out bool expectSignedTokens, out bool expectBasicTokens, out bool expectEndorsingTokens)
    {
      throw new NotImplementedException("GetSupportingTokenAuthenticators is not supported in .NET Core");
    }
    internal bool ExpectSupportingTokens
    {
      get
      {
        return this.expectSupportingTokens;
      }
      set
      {
        this.expectSupportingTokens = value;
      }
    }

    public AuditLevel MessageAuthenticationAuditLevel
    {
      get
      {
        return this.messageAuthenticationAuditLevel;
      }
      set
      {
        this.ThrowIfImmutable();
        AuditLevelHelper.Validate(value);
        this.messageAuthenticationAuditLevel = value;
      }
    }
    
    public bool SuppressAuditFailure
    {
      get
      {
        return this.suppressAuditFailure;
      }
      set
      {
        this.ThrowIfImmutable();
        this.suppressAuditFailure = value;
      }
    }
    
    public ICollection<SupportingTokenAuthenticatorSpecification> ChannelSupportingTokenAuthenticatorSpecification
    {
      get
      {
        return this.channelSupportingTokenAuthenticatorSpecification;
      }
    }
    
    public virtual T GetProperty<T>() where T : class
    {
      if (!(typeof (T) == typeof (Collection<ISecurityContextSecurityTokenCache>)))
        return default (T);
      this.ThrowIfNotOpen();
      Collection<ISecurityContextSecurityTokenCache> collection = new Collection<ISecurityContextSecurityTokenCache>();
      if (this.channelSupportingTokenAuthenticatorSpecification != null)
      {
        foreach (SupportingTokenAuthenticatorSpecification authenticatorSpecification in (IEnumerable<SupportingTokenAuthenticatorSpecification>) this.channelSupportingTokenAuthenticatorSpecification)
        {
          if (authenticatorSpecification.TokenAuthenticator is ISecurityContextSecurityTokenCacheProvider)
            collection.Add(((ISecurityContextSecurityTokenCacheProvider) authenticatorSpecification.TokenAuthenticator).TokenCache);
        }
      }
#if FEATURE_CORECLR
      return (T) Convert.ChangeType(collection, typeof(T));
#else
      return (T) collection;
#endif
    }
    
    protected RecipientServiceModelSecurityTokenRequirement CreateRecipientSecurityTokenRequirement()
    {
      RecipientServiceModelSecurityTokenRequirement tokenRequirement = new RecipientServiceModelSecurityTokenRequirement();
      tokenRequirement.SecurityBindingElement = this.securityBindingElement;
      tokenRequirement.SecurityAlgorithmSuite = this.IncomingAlgorithmSuite;
      tokenRequirement.ListenUri = this.listenUri;
      tokenRequirement.MessageSecurityVersion = this.MessageSecurityVersion.SecurityTokenVersion;
      tokenRequirement.AuditLogLocation = this.auditLogLocation;
      tokenRequirement.SuppressAuditFailure = this.suppressAuditFailure;
      tokenRequirement.MessageAuthenticationAuditLevel = this.messageAuthenticationAuditLevel;
      tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.ExtendedProtectionPolicy] = (object) this.extendedProtectionPolicy;
#if FEATURE_CORECLR
      Console.WriteLine("TODO - skipping endpointFilterTable");
#else
      if (this.endpointFilterTable != null)
        tokenRequirement.Properties.Add(ServiceModelSecurityTokenRequirement.EndpointFilterTableProperty, (object) this.endpointFilterTable);
#endif
      return tokenRequirement;
    }

    public Uri ListenUri
    {
      get
      {
        return this.listenUri;
      }
      set
      {
        this.ThrowIfImmutable();
        this.listenUri = value;
      }
    }
    
    private RecipientServiceModelSecurityTokenRequirement CreateRecipientSecurityTokenRequirement(SecurityTokenParameters parameters, SecurityTokenAttachmentMode attachmentMode)
    {
      RecipientServiceModelSecurityTokenRequirement tokenRequirement = this.CreateRecipientSecurityTokenRequirement();
      parameters.InitializeSecurityTokenRequirement((SecurityTokenRequirement) tokenRequirement);
      tokenRequirement.KeyUsage = SecurityKeyUsage.Signature;
      tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.MessageDirectionProperty] = (object) MessageDirection.Input;
      tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.SupportingTokenAttachmentModeProperty] = (object) attachmentMode;
      tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.ExtendedProtectionPolicy] = (object) this.extendedProtectionPolicy;
      return tokenRequirement;
    }

    public IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("OperationWithTimeoutAsyncResult constructor requires System.Action<System.TimeSpan>");
#else
      return (IAsyncResult) new OperationWithTimeoutAsyncResult(new OperationWithTimeoutCallback(this.OnClose), timeout, callback, state);
#endif
    }

    public IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("OperationWithTimeoutAsyncResult constructor requires System.Action<System.TimeSpan>");
#else
      return (IAsyncResult) new OperationWithTimeoutAsyncResult(new OperationWithTimeoutCallback(this.OnOpen), timeout, callback, state);
#endif
    }

    public void OnClosed()
    {
    }

    public void OnClosing()
    {
    }

    public void OnEndClose(IAsyncResult result)
    {
      OperationWithTimeoutAsyncResult.End(result);
    }

    public void OnEndOpen(IAsyncResult result)
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
    
    public virtual void OnAbort()
    {
      if (this.actAsInitiator)
        return;
#if FEATURE_CORECLR
      throw new NotImplementedException("scopedSupportingTokenAuthenticatorSpecification not supported in .NET Core");
#else
      foreach (SupportingTokenAuthenticatorSpecification authenticatorSpecification in (IEnumerable<SupportingTokenAuthenticatorSpecification>) this.channelSupportingTokenAuthenticatorSpecification)
        SecurityUtils.AbortTokenAuthenticatorIfRequired(authenticatorSpecification.TokenAuthenticator);
      foreach (string key in this.scopedSupportingTokenAuthenticatorSpecification.Keys)
      {
        foreach (SupportingTokenAuthenticatorSpecification authenticatorSpecification in (IEnumerable<SupportingTokenAuthenticatorSpecification>) this.scopedSupportingTokenAuthenticatorSpecification[key])
          SecurityUtils.AbortTokenAuthenticatorIfRequired(authenticatorSpecification.TokenAuthenticator);
      }
#endif
    }

    public virtual void OnClose(TimeSpan timeout)
    {
      if (this.actAsInitiator)
        return;
#if FEATURE_CORECLR
      throw new NotImplementedException("scopedSupportingTokenAuthenticatorSpecification not supported in .NET Core");
#else
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      foreach (SupportingTokenAuthenticatorSpecification authenticatorSpecification in (IEnumerable<SupportingTokenAuthenticatorSpecification>) this.channelSupportingTokenAuthenticatorSpecification)
        SecurityUtils.CloseTokenAuthenticatorIfRequired(authenticatorSpecification.TokenAuthenticator, timeoutHelper.RemainingTime());
      foreach (string key in this.scopedSupportingTokenAuthenticatorSpecification.Keys)
      {
        foreach (SupportingTokenAuthenticatorSpecification authenticatorSpecification in (IEnumerable<SupportingTokenAuthenticatorSpecification>) this.scopedSupportingTokenAuthenticatorSpecification[key])
          SecurityUtils.CloseTokenAuthenticatorIfRequired(authenticatorSpecification.TokenAuthenticator, timeoutHelper.RemainingTime());
      }
#endif
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
    
    public TimeSpan TimestampValidityDuration
    {
      get
      {
        return this.timestampValidityDuration;
      }
      set
      {
        this.ThrowIfImmutable();
        if (value <= TimeSpan.Zero)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", SR.GetString("TimeSpanMustbeGreaterThanTimeSpanZero")));
        this.timestampValidityDuration = value;
      }
    }
    
    public SecurityHeaderLayout SecurityHeaderLayout
    {
      get
      {
        return this.securityHeaderLayout;
      }
      set
      {
        this.ThrowIfImmutable();
        this.securityHeaderLayout = value;
      }
    }

    public TimeSpan ReplayWindow
    {
      get
      {
        return this.replayWindow;
      }
      set
      {
        this.ThrowIfImmutable();
        if (value <= TimeSpan.Zero)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", SR.GetString("TimeSpanMustbeGreaterThanTimeSpanZero")));
        this.replayWindow = value;
      }
    }

    public NonceCache NonceCache
    {
      get
      {
        return this.nonceCache;
      }
      set
      {
        this.ThrowIfImmutable();
        this.nonceCache = value;
      }
    }

    public TimeSpan MaxClockSkew
    {
      get
      {
        return this.maxClockSkew;
      }
      set
      {
        this.ThrowIfImmutable();
        if (value < TimeSpan.Zero)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.maxClockSkew = value;
      }
    }

    public int MaxCachedNonces
    {
      get
      {
        return this.maxCachedNonces;
      }
      set
      {
        this.ThrowIfImmutable();
        if (value <= 0)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.maxCachedNonces = value;
      }
    }
    
    public bool AddTimestamp
    {
      get
      {
        return this.addTimestamp;
      }
      set
      {
        this.ThrowIfImmutable();
        this.addTimestamp = value;
      }
    }
    
    public SecurityAlgorithmSuite IncomingAlgorithmSuite
    {
      get
      {
        return this.incomingAlgorithmSuite;
      }
      set
      {
        this.ThrowIfImmutable();
        if (value == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("value"));
        this.incomingAlgorithmSuite = value;
      }
    }

    internal bool ExpectKeyDerivation
    {
      get
      {
        return this.expectKeyDerivation;
      }
      set
      {
        this.expectKeyDerivation = value;
      }
    }

    public SecurityProtocolFactory()
    {
      this.channelSupportingTokenAuthenticatorSpecification = (ICollection<SupportingTokenAuthenticatorSpecification>) new Collection<SupportingTokenAuthenticatorSpecification>();
      // this.scopedSupportingTokenAuthenticatorSpecification = new Dictionary<string, ICollection<SupportingTokenAuthenticatorSpecification>>();
      Console.WriteLine("SecurityProtocolFactory Constructor");
      communicationObject = new WrapperSecurityCommunicationObject(this);
    }

    internal SecurityProtocolFactory(SecurityProtocolFactory factory)
      : this()
    {
      if (factory == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("factory");
      this.actAsInitiator = factory.actAsInitiator;
      this.addTimestamp = factory.addTimestamp;
      this.detectReplays = factory.detectReplays;
      this.incomingAlgorithmSuite = factory.incomingAlgorithmSuite;
      this.maxCachedNonces = factory.maxCachedNonces;
      this.maxClockSkew = factory.maxClockSkew;
      this.outgoingAlgorithmSuite = factory.outgoingAlgorithmSuite;
      this.replayWindow = factory.replayWindow;
      // this.channelSupportingTokenAuthenticatorSpecification = (ICollection<SupportingTokenAuthenticatorSpecification>) new Collection<SupportingTokenAuthenticatorSpecification>((IList<SupportingTokenAuthenticatorSpecification>) new List<SupportingTokenAuthenticatorSpecification>((IEnumerable<SupportingTokenAuthenticatorSpecification>) factory.channelSupportingTokenAuthenticatorSpecification));
      // this.scopedSupportingTokenAuthenticatorSpecification = new Dictionary<string, ICollection<SupportingTokenAuthenticatorSpecification>>((IDictionary<string, ICollection<SupportingTokenAuthenticatorSpecification>>) factory.scopedSupportingTokenAuthenticatorSpecification);
      this.standardsManager = factory.standardsManager;
      // this.timestampValidityDuration = factory.timestampValidityDuration;
      // this.auditLogLocation = factory.auditLogLocation;
      this.suppressAuditFailure = factory.suppressAuditFailure;
      // this.serviceAuthorizationAuditLevel = factory.serviceAuthorizationAuditLevel;
      this.messageAuthenticationAuditLevel = factory.messageAuthenticationAuditLevel;
      if (factory.securityBindingElement != null)
        this.securityBindingElement = (SecurityBindingElement) factory.securityBindingElement.Clone();
      this.securityTokenManager = factory.securityTokenManager;
      this.privacyNoticeUri = factory.privacyNoticeUri;
      this.privacyNoticeVersion = factory.privacyNoticeVersion;
      //  this.endpointFilterTable = factory.endpointFilterTable;
      this.extendedProtectionPolicy = factory.extendedProtectionPolicy;
      this.nonceCache = factory.nonceCache;
    }

    public virtual void OnOpen(TimeSpan timeout)
    {
      Console.WriteLine("TODO - fill in SecurityProtocolFactory.OnOpen");
      this.messageSecurityVersion = this.standardsManager.MessageSecurityVersion;
    }

    public virtual EndpointIdentity GetIdentityOfSelf()
    {
      throw new NotImplementedException("GetIdentityOfSelf not supported");
    }

    public bool ActAsInitiator
    {
      get
      {
        return this.actAsInitiator;
      }
    }

    internal bool IsDuplexReply
    {
      get
      {
        return this.isDuplexReply;
      }
      set
      {
        this.isDuplexReply = value;
      }
    }

    public bool DetectReplays
    {
      get
      {
        return this.detectReplays;
      }
      set
      {
        this.ThrowIfImmutable();
        this.detectReplays = value;
      }
    }

    internal void OnPropertySettingsError(string propertyName, bool requiredForForwardDirection)
    {
      if (requiredForForwardDirection)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR.GetString("PropertySettingErrorOnProtocolFactory", (object) propertyName, (object) this), propertyName));
      if (this.requestReplyErrorPropertyName != null)
        return;
      this.requestReplyErrorPropertyName = propertyName;
    }


#endregion

    internal void ThrowIfImmutable()
    {
      // this.communicationObject.ThrowIfDisposedOrImmutable();
    }

    public BufferManager StreamBufferManager
    {
      get
      {
        if (this.streamBufferManager == null)
          this.streamBufferManager = BufferManager.CreateBufferManager(0L, int.MaxValue);
        return this.streamBufferManager;
      }
      set
      {
        this.streamBufferManager = value;
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
        this.ThrowIfImmutable();
        if (value == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("value"));
        this.standardsManager = value;
      }
    }

    private void ThrowIfNotOpen()
    {
      this.communicationObject.ThrowIfNotOpened();
    }

    protected virtual SecurityProtocol OnCreateSecurityProtocol(EndpointAddress target, Uri via, object listenerSecurityState, TimeSpan timeout)
    {
      throw new NotImplementedException("OnCreateSecurityProtocol not implemented in .NET Core");
    }

    public virtual bool SupportsDuplex
    {
      get
      {
        return false;
      }
    }

    public virtual bool SupportsRequestReply
    {
      get
      {
        return true;
      }
    }

    public SecurityTokenManager SecurityTokenManager
    {
      get
      {
        return this.securityTokenManager;
      }
      set
      {
        this.ThrowIfImmutable();
        this.securityTokenManager = value;
      }
    }

    public SecurityBindingElement SecurityBindingElement
    {
      get
      {
        return this.securityBindingElement;
      }
      set
      {
        Console.WriteLine("SET SecurityBindingElement");
        if (value == null)
        { Console.WriteLine("Cannot set SecurityBindingElement to NULL"); }
        this.ThrowIfImmutable();
        if (value != null)
          value = (SecurityBindingElement) value.Clone();
        this.securityBindingElement = value;
      }
    }

    protected WrapperSecurityCommunicationObject CommunicationObject
    {
      get
      {
        return this.communicationObject;
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
        this.ThrowIfImmutable();
        this.privacyNoticeVersion = value;
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
        this.ThrowIfImmutable();
        this.privacyNoticeUri = value;
      }
    }

    public void Open(bool actAsInitiator, TimeSpan timeout)
    {
      if (this.communicationObject == null)
      { Console.WriteLine("CommunicationObject is NULL");}
      else
      { Console.WriteLine("CommunicationObject is NOT NULL");}
      this.actAsInitiator = actAsInitiator;
      this.communicationObject.Open(timeout);
    }

    public void Close(bool aborted, TimeSpan timeout)
    {
      if (aborted)
        this.CommunicationObject.Abort();
      else
        this.CommunicationObject.Close(timeout);
    }

    public SecurityProtocol CreateSecurityProtocol(EndpointAddress target, Uri via, object listenerSecurityState, bool isReturnLegSecurityRequired, TimeSpan timeout)
    {
      this.ThrowIfNotOpen();
      SecurityProtocol securityProtocol = this.OnCreateSecurityProtocol(target, via, listenerSecurityState, timeout);
      if (securityProtocol == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("ProtocolFactoryCouldNotCreateProtocol")));
      return securityProtocol;
    }

    public SecurityAlgorithmSuite OutgoingAlgorithmSuite
    {
      get
      {
        return this.outgoingAlgorithmSuite;
      }
      set
      {
        this.ThrowIfImmutable();
        if (value == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("value"));
        this.outgoingAlgorithmSuite = value;
      }
    }

    public void EndClose(IAsyncResult result)
    {
      this.CommunicationObject.EndClose(result);
    }

    public ExtendedProtectionPolicy ExtendedProtectionPolicy
    {
      get
      {
        return this.extendedProtectionPolicy;
      }
      set
      {
        this.extendedProtectionPolicy = value;
      }
    }

    internal MessageSecurityVersion MessageSecurityVersion
    {
      get
      {
        return this.messageSecurityVersion;
      }
    }

    }
}
