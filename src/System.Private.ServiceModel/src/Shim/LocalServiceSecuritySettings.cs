// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.LocalServiceSecuritySettings
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Runtime;
using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
  /// <summary>Provides local service security properties that can be set.</summary>
  public sealed class LocalServiceSecuritySettings
  {
    private bool detectReplays;
    private int replayCacheSize;
    private TimeSpan replayWindow;
    private TimeSpan maxClockSkew;
    private TimeSpan issuedCookieLifetime;
    private int maxStatefulNegotiations;
    private TimeSpan negotiationTimeout;
    private int maxCachedCookies;
    private int maxPendingSessions;
    private TimeSpan inactivityTimeout;
    private TimeSpan sessionKeyRenewalInterval;
    private TimeSpan sessionKeyRolloverInterval;
    private bool reconnectTransportOnFailure;
    private TimeSpan timestampValidityDuration;
    private NonceCache nonceCache;

    /// <summary>Gets or sets a value that indicates whether replay detection is enabled on the service. </summary>
    /// <returns>true if replay detection is enabled on the service; otherwise, false. The default is true.</returns>
    public bool DetectReplays
    {
      get
      {
        return this.detectReplays;
      }
      set
      {
        this.detectReplays = value;
      }
    }

    /// <summary>Gets or sets the size of the nonce cache used for replay detection.</summary>
    /// <returns>The size of the nonce cache used for replay detection. The default is 900,000.</returns>
    public int ReplayCacheSize
    {
      get
      {
        return this.replayCacheSize;
      }
      set
      {
        if (value < 0)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("ValueMustBeNonNegative")));
        this.replayCacheSize = value;
      }
    }

    /// <summary>Gets or sets the maximum amount of time within which the service can accept a message.</summary>
    /// <returns>A <see cref="T:System.TimeSpan" /> that represents the maximum amount of time within which the service can accept a message. The default is 5 minutes.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The property is set to a value less than 0.</exception>
    public TimeSpan ReplayWindow
    {
      get
      {
        return this.replayWindow;
      }
      set
      {
        if (value < TimeSpan.Zero)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("SFxTimeoutOutOfRange0")));
        if (TimeoutHelper.IsTooLarge(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("SFxTimeoutOutOfRangeTooBig")));
        this.replayWindow = value;
      }
    }

    /// <summary>Gets or sets the maximum allowable time difference between the system clocks of the two parties that are communicating.</summary>
    /// <returns>A <see cref="T:System.TimeSpan" /> that represents the maximum allowable time difference between the system clocks of the two parties that are communicating. The default is 5 minutes.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The property is set to a value less than 0.</exception>
    public TimeSpan MaxClockSkew
    {
      get
      {
        return this.maxClockSkew;
      }
      set
      {
        if (value < TimeSpan.Zero)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("SFxTimeoutOutOfRange0")));
        if (TimeoutHelper.IsTooLarge(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("SFxTimeoutOutOfRangeTooBig")));
        this.maxClockSkew = value;
      }
    }

    /// <summary>Gets or sets the cache for the local service security settings.</summary>
    /// <returns>The cache for the local service security settings.</returns>
    public NonceCache NonceCache
    {
      get
      {
        return this.nonceCache;
      }
      set
      {
        this.nonceCache = value;
      }
    }

    /// <summary>Gets or sets the lifetime for the <see cref="T:System.ServiceModel.Security.Tokens.SecurityContextSecurityToken" /> that the service issues for the client.</summary>
    /// <returns>A <see cref="T:System.TimeSpan" /> that represents the lifetime for new security cookies. The default is 10 hours.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The property is set to a value less than 0.</exception>
    public TimeSpan IssuedCookieLifetime
    {
      get
      {
        return this.issuedCookieLifetime;
      }
      set
      {
        if (value < TimeSpan.Zero)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("SFxTimeoutOutOfRange0")));
        if (TimeoutHelper.IsTooLarge(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("SFxTimeoutOutOfRangeTooBig")));
        this.issuedCookieLifetime = value;
      }
    }

    /// <summary>Gets or sets the maximum number of concurrent security negotiations with clients that the service can participate in.</summary>
    /// <returns>The maximum number of concurrent security negotiations with clients that the service can participate in. The default is 128.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The property is set to a value less than 0.</exception>
    public int MaxStatefulNegotiations
    {
      get
      {
        return this.maxStatefulNegotiations;
      }
      set
      {
        if (value < 0)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("ValueMustBeNonNegative")));
        this.maxStatefulNegotiations = value;
      }
    }

    /// <summary>Gets or sets the maximum duration of the security negotiation phase between client and service.</summary>
    /// <returns>A <see cref="T:System.TimeSpan" /> that represents the maximum duration of the security negotiation phase between client and service. Any negotiation with the service (for example during message level SPNego or SSL authentication) must complete within this time. The default is 1 minute.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The property is set to a value less than 0.</exception>
    public TimeSpan NegotiationTimeout
    {
      get
      {
        return this.negotiationTimeout;
      }
      set
      {
        if (value < TimeSpan.Zero)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("SFxTimeoutOutOfRange0")));
        if (TimeoutHelper.IsTooLarge(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("SFxTimeoutOutOfRangeTooBig")));
        this.negotiationTimeout = value;
      }
    }

    /// <summary>Gets or sets the maximum number of concurrent security sessions that are established with the server for which it has issued a session token but for which no application messages are sent.</summary>
    /// <returns>The maximum number of concurrent security sessions. The default is 128.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The property is set to a value less than 0.</exception>
    public int MaxPendingSessions
    {
      get
      {
        return this.maxPendingSessions;
      }
      set
      {
        if (value < 0)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("ValueMustBeNonNegative")));
        this.maxPendingSessions = value;
      }
    }

    /// <summary>Gets or sets the duration to wait before the channel is closed due to inactivity.</summary>
    /// <returns>A <see cref="T:System.TimeSpan" /> that represents the duration to wait before the security session with the client is closed due to inactivity. The default is 2 minutes.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The property is set to a value less than 0.</exception>
    public TimeSpan InactivityTimeout
    {
      get
      {
        return this.inactivityTimeout;
      }
      set
      {
        if (value < TimeSpan.Zero)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("SFxTimeoutOutOfRange0")));
        if (TimeoutHelper.IsTooLarge(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("SFxTimeoutOutOfRangeTooBig")));
        this.inactivityTimeout = value;
      }
    }

    /// <summary>Gets or sets the lifetime of a key used in a security session. When this interval expires the key is automatically renewed.</summary>
    /// <returns>The time span after which the service requires that the initiator renew the key used for the security session. If the initiator does not renew the key within this time the service sends back a fault to the initiator. The default is 15 hours.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The property is set to a value less than 0.</exception>
    public TimeSpan SessionKeyRenewalInterval
    {
      get
      {
        return this.sessionKeyRenewalInterval;
      }
      set
      {
        if (value < TimeSpan.Zero)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("SFxTimeoutOutOfRange0")));
        if (TimeoutHelper.IsTooLarge(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("SFxTimeoutOutOfRangeTooBig")));
        this.sessionKeyRenewalInterval = value;
      }
    }

    /// <summary>Gets or sets the time interval after key renewal for which the previous session key is valid on incoming messages during a key renewal.</summary>
    /// <returns>The time interval after key renewal for which the previous session key is valid on incoming messages after a key renewal. The default is 5 minutes.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The property is set to a value less than 0.</exception>
    public TimeSpan SessionKeyRolloverInterval
    {
      get
      {
        return this.sessionKeyRolloverInterval;
      }
      set
      {
        if (value < TimeSpan.Zero)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("SFxTimeoutOutOfRange0")));
        if (TimeoutHelper.IsTooLarge(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("SFxTimeoutOutOfRangeTooBig")));
        this.sessionKeyRolloverInterval = value;
      }
    }

    /// <summary>Gets or sets a value that indicates whether security sessions attempt to reconnect after transport failures.</summary>
    /// <returns>true if security sessions attempt to reconnect after transport failures; otherwise false. The default is true.</returns>
    public bool ReconnectTransportOnFailure
    {
      get
      {
        return this.reconnectTransportOnFailure;
      }
      set
      {
        this.reconnectTransportOnFailure = value;
      }
    }

    /// <summary>Gets or sets the maximum duration of time that messages are valid when sent by the service. If the client receives the service's message after this duration, it discards the message.</summary>
    /// <returns>The maximum duration of time that messages are valid when sent by the service. The default is 5 minutes.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The property is set to a value less than 0.</exception>
    public TimeSpan TimestampValidityDuration
    {
      get
      {
        return this.timestampValidityDuration;
      }
      set
      {
        if (value < TimeSpan.Zero)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("SFxTimeoutOutOfRange0")));
        if (TimeoutHelper.IsTooLarge(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("SFxTimeoutOutOfRangeTooBig")));
        this.timestampValidityDuration = value;
      }
    }

    /// <summary>Gets or sets the maximum number of <see cref="T:System.ServiceModel.Security.Tokens.SecurityContextSecurityToken" />s that the service allows to cache at once.</summary>
    /// <returns>The maximum number of secure conversation cookies that can be cached by the service. When this limit is reached the service removes the oldest cached secure conversation cookies to make room for new secure conversation cookies. The default is 1000.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The property is set to a value less than 0.</exception>
    public int MaxCachedCookies
    {
      get
      {
        return this.maxCachedCookies;
      }
      set
      {
        if (value < 0)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, System.SR.GetString("ValueMustBeNonNegative")));
        this.maxCachedCookies = value;
      }
    }

    private LocalServiceSecuritySettings(LocalServiceSecuritySettings other)
    {
      this.detectReplays = other.detectReplays;
      this.replayCacheSize = other.replayCacheSize;
      this.replayWindow = other.replayWindow;
      this.maxClockSkew = other.maxClockSkew;
      this.issuedCookieLifetime = other.issuedCookieLifetime;
      this.maxStatefulNegotiations = other.maxStatefulNegotiations;
      this.negotiationTimeout = other.negotiationTimeout;
      this.maxPendingSessions = other.maxPendingSessions;
      this.inactivityTimeout = other.inactivityTimeout;
      this.sessionKeyRenewalInterval = other.sessionKeyRenewalInterval;
      this.sessionKeyRolloverInterval = other.sessionKeyRolloverInterval;
      this.reconnectTransportOnFailure = other.reconnectTransportOnFailure;
      this.timestampValidityDuration = other.timestampValidityDuration;
      this.maxCachedCookies = other.maxCachedCookies;
      this.nonceCache = other.nonceCache;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Channels.LocalServiceSecuritySettings" /> class. </summary>
    public LocalServiceSecuritySettings()
    {
      this.DetectReplays = true;
      this.ReplayCacheSize = 900000;
      this.ReplayWindow = SecurityProtocolFactory.defaultReplayWindow;
      this.MaxClockSkew = SecurityProtocolFactory.defaultMaxClockSkew;
      this.MaxStatefulNegotiations = 128;
      this.maxPendingSessions = 128;
#if FEATURE_CORECLR
      // SecuritySessionServerSettings not supported
      this.IssuedCookieLifetime = TimeSpan.Parse("10:00:00");
      this.NegotiationTimeout = TimeSpan.Parse("00:01:00");
#else
      this.IssuedCookieLifetime = NegotiationTokenAuthenticator<NegotiationTokenAuthenticatorState>.defaultServerIssuedTokenLifetime;
      this.NegotiationTimeout = NegotiationTokenAuthenticator<NegotiationTokenAuthenticatorState>.defaultServerMaxNegotiationLifetime;
      this.inactivityTimeout = SecuritySessionServerSettings.defaultInactivityTimeout;
      this.sessionKeyRenewalInterval = SecuritySessionServerSettings.defaultKeyRenewalInterval;
      this.sessionKeyRolloverInterval = SecuritySessionServerSettings.defaultKeyRolloverInterval;
#endif
      this.reconnectTransportOnFailure = true;
      this.TimestampValidityDuration = SecurityProtocolFactory.defaultTimestampValidityDuration;
      this.maxCachedCookies = 1000;
      this.nonceCache = (NonceCache) null;
    }

    /// <summary>Creates a new instance of this class from the current instance.</summary>
    /// <returns>A new instance of <see cref="T:System.ServiceModel.Channels.LocalServiceSecuritySettings" />.</returns>
    public LocalServiceSecuritySettings Clone()
    {
      return new LocalServiceSecuritySettings(this);
    }
  }
}
