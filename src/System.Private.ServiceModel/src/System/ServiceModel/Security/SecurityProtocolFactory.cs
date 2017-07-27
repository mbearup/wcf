// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.Globalization;
using System.IdentityModel.Selectors;
using System.Security.Authentication.ExtendedProtection;

namespace System.ServiceModel.Security
{
    internal class SecurityProtocolFactory
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

    public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
    {
      return this.CommunicationObject.BeginClose(timeout, callback, state);
    }

#region fromwcf

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
      communicationObject = new WrapperSecurityCommunicationObject((ISecurityCommunicationObject) this);
    }

    internal SecurityProtocolFactory(SecurityProtocolFactory factory)
      : this()
    {
      if (factory == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("factory");
      this.actAsInitiator = factory.actAsInitiator;
      //this.addTimestamp = factory.addTimestamp;
      this.detectReplays = factory.detectReplays;
      // this.incomingAlgorithmSuite = factory.incomingAlgorithmSuite;
      // this.maxCachedNonces = factory.maxCachedNonces;
      // this.maxClockSkew = factory.maxClockSkew;
      // this.outgoingAlgorithmSuite = factory.outgoingAlgorithmSuite;
      // this.replayWindow = factory.replayWindow;
      // this.channelSupportingTokenAuthenticatorSpecification = (ICollection<SupportingTokenAuthenticatorSpecification>) new Collection<SupportingTokenAuthenticatorSpecification>((IList<SupportingTokenAuthenticatorSpecification>) new List<SupportingTokenAuthenticatorSpecification>((IEnumerable<SupportingTokenAuthenticatorSpecification>) factory.channelSupportingTokenAuthenticatorSpecification));
      // this.scopedSupportingTokenAuthenticatorSpecification = new Dictionary<string, ICollection<SupportingTokenAuthenticatorSpecification>>((IDictionary<string, ICollection<SupportingTokenAuthenticatorSpecification>>) factory.scopedSupportingTokenAuthenticatorSpecification);
      this.standardsManager = factory.standardsManager;
      // this.timestampValidityDuration = factory.timestampValidityDuration;
      // this.auditLogLocation = factory.auditLogLocation;
      // this.suppressAuditFailure = factory.suppressAuditFailure;
      // this.serviceAuthorizationAuditLevel = factory.serviceAuthorizationAuditLevel;
      // this.messageAuthenticationAuditLevel = factory.messageAuthenticationAuditLevel;
      if (factory.securityBindingElement != null)
        this.securityBindingElement = (SecurityBindingElement) factory.securityBindingElement.Clone();
      this.securityTokenManager = factory.securityTokenManager;
      this.privacyNoticeUri = factory.privacyNoticeUri;
      this.privacyNoticeVersion = factory.privacyNoticeVersion;
      //  this.endpointFilterTable = factory.endpointFilterTable;
      this.extendedProtectionPolicy = factory.extendedProtectionPolicy;
      // this.nonceCache = factory.nonceCache;
    }

    public virtual void OnOpen(TimeSpan timeout)
    {
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
      this.communicationObject.ThrowIfDisposedOrImmutable();
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
