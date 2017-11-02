// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.IssuedTokenClientCredential
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace System.ServiceModel.Security
{
  /// <summary>Represents information used to obtain an issued token from a security token service.</summary>
  public sealed class IssuedTokenClientCredential
  {
    private SecurityKeyEntropyMode defaultKeyEntropyMode = SecurityKeyEntropyMode.CombinedEntropy;
    private bool cacheIssuedTokens = true;
    private TimeSpan maxIssuedTokenCachingTime = TimeSpan.MaxValue; // = IssuanceTokenProviderBase<SspiNegotiationTokenProviderState>.DefaultClientMaxTokenCachingTime;
    private int issuedTokenRenewalThresholdPercentage = 60;
    private KeyedByTypeCollection<IEndpointBehavior> localIssuerChannelBehaviors;
    private Dictionary<Uri, KeyedByTypeCollection<IEndpointBehavior>> issuerChannelBehaviors;
    private EndpointAddress localIssuerAddress;
    private Binding localIssuerBinding;
    private bool isReadOnly;

    /// <summary>Gets or sets the address of the local issuer.</summary>
    /// <returns>The <see cref="T:System.ServiceModel.EndpointAddress" /> of the local issuer.</returns>
    /// <exception cref="T:System.InvalidOperationException">An attempt was made to set a read-only credential.</exception>
    public EndpointAddress LocalIssuerAddress
    {
      get
      {
        return this.localIssuerAddress;
      }
      set
      {
        this.ThrowIfImmutable();
        this.localIssuerAddress = value;
      }
    }

    /// <summary>Gets or sets the binding of the local issuer.</summary>
    /// <returns>The <see cref="T:System.ServiceModel.Channels.Binding" /> of the local issuer.</returns>
    /// <exception cref="T:System.InvalidOperationException">An attempt was made to set a read-only credential.</exception>
    public Binding LocalIssuerBinding
    {
      get
      {
        return this.localIssuerBinding;
      }
      set
      {
        this.ThrowIfImmutable();
        this.localIssuerBinding = value;
      }
    }

    /// <summary>Gets or sets the default value of <see cref="T:System.ServiceModel.Security.SecurityKeyEntropyMode" />.</summary>
    /// <returns>The value of <see cref="T:System.ServiceModel.Security.SecurityKeyEntropyMode" />. The default is <see cref="F:System.ServiceModel.Security.SecurityKeyEntropyMode.CombinedEntropy" />.</returns>
    /// <exception cref="T:System.InvalidOperationException">An attempt was made to set a read-only credential.</exception>
    public SecurityKeyEntropyMode DefaultKeyEntropyMode
    {
      get
      {
        return this.defaultKeyEntropyMode;
      }
      set
      {
        SecurityKeyEntropyModeHelper.Validate(value);
        this.ThrowIfImmutable();
        this.defaultKeyEntropyMode = value;
      }
    }

    /// <summary>Gets or sets a Boolean that specifies whether issued tokens are to be cached by the channel.</summary>
    /// <returns>true if tokens are to be cached; otherwise, false. The default value is true.</returns>
    /// <exception cref="T:System.InvalidOperationException">Attempt to set a read-only credential.</exception>
    public bool CacheIssuedTokens
    {
      get
      {
        return this.cacheIssuedTokens;
      }
      set
      {
        this.ThrowIfImmutable();
        this.cacheIssuedTokens = value;
      }
    }

    /// <summary>Gets or sets the renewal threshold percentage for issued tokens.</summary>
    /// <returns>The renewal threshold percentage for issued tokens. The default value is 60.</returns>
    /// <exception cref="T:System.InvalidOperationException">An attempt was made to set a read-only credential.</exception>
    public int IssuedTokenRenewalThresholdPercentage
    {
      get
      {
        return this.issuedTokenRenewalThresholdPercentage;
      }
      set
      {
        this.ThrowIfImmutable();
        this.issuedTokenRenewalThresholdPercentage = value;
      }
    }

    /// <summary>Gets a collection of issuer channel behaviors.</summary>
    /// <returns>A <see cref="T:System.Collections.Generic.Dictionary`2" /> of <see cref="T:System.ServiceModel.Description.IEndpointBehavior" />s. </returns>
    public Dictionary<Uri, KeyedByTypeCollection<IEndpointBehavior>> IssuerChannelBehaviors
    {
      get
      {
        if (this.issuerChannelBehaviors == null)
          this.issuerChannelBehaviors = new Dictionary<Uri, KeyedByTypeCollection<IEndpointBehavior>>();
        return this.issuerChannelBehaviors;
      }
    }

    /// <summary>Gets a collection of local issuer channel behaviors.</summary>
    /// <returns>A <see cref="T:System.Collections.Generic.KeyedByTypeCollection`1" /> of <see cref="T:System.ServiceModel.Description.IEndpointBehavior" />s.</returns>
    public KeyedByTypeCollection<IEndpointBehavior> LocalIssuerChannelBehaviors
    {
      get
      {
        if (this.localIssuerChannelBehaviors == null)
          this.localIssuerChannelBehaviors = new KeyedByTypeCollection<IEndpointBehavior>();
        return this.localIssuerChannelBehaviors;
      }
    }

    /// <summary>Gets or sets the maximum caching time for an issued token.</summary>
    /// <returns>A <see cref="T:System.TimeSpan" /> that represents the maximum caching time for an issued token. The default value is <see cref="F:System.TimeSpan.MaxValue" />.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The value of set is less than zero.</exception>
    /// <exception cref="T:System.InvalidOperationException">An attempt was made to set a read-only credential.</exception>
    public TimeSpan MaxIssuedTokenCachingTime
    {
      get
      {
        return this.maxIssuedTokenCachingTime;
      }
      set
      {
        if (value < TimeSpan.Zero)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, SR.GetString("SFxTimeoutOutOfRange0")));
        if (TimeoutHelper.IsTooLarge(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, SR.GetString("SFxTimeoutOutOfRangeTooBig")));
        this.ThrowIfImmutable();
        this.maxIssuedTokenCachingTime = value;
      }
    }

    internal IssuedTokenClientCredential()
    {
    }

    internal IssuedTokenClientCredential(IssuedTokenClientCredential other)
    {
      this.defaultKeyEntropyMode = other.defaultKeyEntropyMode;
      this.cacheIssuedTokens = other.cacheIssuedTokens;
      this.issuedTokenRenewalThresholdPercentage = other.issuedTokenRenewalThresholdPercentage;
      this.maxIssuedTokenCachingTime = other.maxIssuedTokenCachingTime;
      this.localIssuerAddress = other.localIssuerAddress;
      this.localIssuerBinding = other.localIssuerBinding != null ? (Binding) new CustomBinding(other.localIssuerBinding) : (Binding) null;
      if (other.localIssuerChannelBehaviors != null)
        this.localIssuerChannelBehaviors = this.GetBehaviorCollection(other.localIssuerChannelBehaviors);
      if (other.issuerChannelBehaviors != null)
      {
        this.issuerChannelBehaviors = new Dictionary<Uri, KeyedByTypeCollection<IEndpointBehavior>>();
        foreach (Uri key in other.issuerChannelBehaviors.Keys)
          this.issuerChannelBehaviors.Add(key, this.GetBehaviorCollection(other.issuerChannelBehaviors[key]));
      }
      this.isReadOnly = other.isReadOnly;
    }

    private KeyedByTypeCollection<IEndpointBehavior> GetBehaviorCollection(KeyedByTypeCollection<IEndpointBehavior> behaviors)
    {
      KeyedByTypeCollection<IEndpointBehavior> byTypeCollection = new KeyedByTypeCollection<IEndpointBehavior>();
      foreach (IEndpointBehavior behavior in (Collection<IEndpointBehavior>) behaviors)
        byTypeCollection.Add(behavior);
      return byTypeCollection;
    }

    internal void MakeReadOnly()
    {
      this.isReadOnly = true;
    }

    private void ThrowIfImmutable()
    {
      if (this.isReadOnly)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("ObjectIsReadOnly")));
    }
  }
}
