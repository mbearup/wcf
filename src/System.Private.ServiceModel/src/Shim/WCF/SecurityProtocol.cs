// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SecurityProtocol
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IdentityModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
  internal abstract class SecurityProtocol : ISecurityCommunicationObject
  {
    private static ReadOnlyCollection<SupportingTokenProviderSpecification> emptyTokenProviders;
    private ICollection<SupportingTokenProviderSpecification> channelSupportingTokenProviderSpecification;
    private Dictionary<string, ICollection<SupportingTokenProviderSpecification>> scopedSupportingTokenProviderSpecification;
    private Dictionary<string, Collection<SupportingTokenProviderSpecification>> mergedSupportingTokenProvidersMap;
    private SecurityProtocolFactory factory;
    private EndpointAddress target;
    private Uri via;
    private WrapperSecurityCommunicationObject communicationObject;
    private ChannelParameterCollection channelParameters;

    protected WrapperSecurityCommunicationObject CommunicationObject
    {
      get
      {
        return this.communicationObject;
      }
    }

    public SecurityProtocolFactory SecurityProtocolFactory
    {
      get
      {
        return this.factory;
      }
    }

    public EndpointAddress Target
    {
      get
      {
        return this.target;
      }
    }

    public Uri Via
    {
      get
      {
        return this.via;
      }
    }

    public ICollection<SupportingTokenProviderSpecification> ChannelSupportingTokenProviderSpecification
    {
      get
      {
        return this.channelSupportingTokenProviderSpecification;
      }
    }

    public Dictionary<string, ICollection<SupportingTokenProviderSpecification>> ScopedSupportingTokenProviderSpecification
    {
      get
      {
        return this.scopedSupportingTokenProviderSpecification;
      }
    }

    private static ReadOnlyCollection<SupportingTokenProviderSpecification> EmptyTokenProviders
    {
      get
      {
        if (SecurityProtocol.emptyTokenProviders == null)
          SecurityProtocol.emptyTokenProviders = new ReadOnlyCollection<SupportingTokenProviderSpecification>((IList<SupportingTokenProviderSpecification>) new List<SupportingTokenProviderSpecification>());
        return SecurityProtocol.emptyTokenProviders;
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
        this.communicationObject.ThrowIfDisposedOrImmutable();
        this.channelParameters = value;
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

    protected SecurityProtocol(SecurityProtocolFactory factory, EndpointAddress target, Uri via)
    {
      this.factory = factory;
      this.target = target;
      this.via = via;
      this.communicationObject = new WrapperSecurityCommunicationObject((ISecurityCommunicationObject) this);
    }

    public IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("SecurityProtocol.OnBeginClose is not supported in .NET Core");
#else
      return (IAsyncResult) new OperationWithTimeoutAsyncResult(new OperationWithTimeoutCallback(this.OnClose), timeout, callback, state);
#endif
    }

    public IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("SecurityProtocol.OnBeginOpen is not supported in .NET Core");
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

    internal IList<SupportingTokenProviderSpecification> GetSupportingTokenProviders(string action)
    {
      if (this.mergedSupportingTokenProvidersMap != null && this.mergedSupportingTokenProvidersMap.Count > 0)
      {
        if (action != null && this.mergedSupportingTokenProvidersMap.ContainsKey(action))
          return (IList<SupportingTokenProviderSpecification>) this.mergedSupportingTokenProvidersMap[action];
        if (this.mergedSupportingTokenProvidersMap.ContainsKey("*"))
          return (IList<SupportingTokenProviderSpecification>) this.mergedSupportingTokenProvidersMap["*"];
      }
      if (this.channelSupportingTokenProviderSpecification != SecurityProtocol.EmptyTokenProviders)
        return (IList<SupportingTokenProviderSpecification>) this.channelSupportingTokenProviderSpecification;
      return (IList<SupportingTokenProviderSpecification>) null;
    }

    protected InitiatorServiceModelSecurityTokenRequirement CreateInitiatorSecurityTokenRequirement()
    {
      InitiatorServiceModelSecurityTokenRequirement tokenRequirement = new InitiatorServiceModelSecurityTokenRequirement();
      tokenRequirement.TargetAddress = this.Target;
      tokenRequirement.Via = this.via;
      tokenRequirement.SecurityBindingElement = this.factory.SecurityBindingElement;
      tokenRequirement.SecurityAlgorithmSuite = this.factory.OutgoingAlgorithmSuite;
      tokenRequirement.MessageSecurityVersion = this.factory.MessageSecurityVersion.SecurityTokenVersion;
      if (this.factory.PrivacyNoticeUri != (Uri) null)
        tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.PrivacyNoticeUriProperty] = (object) this.factory.PrivacyNoticeUri;
      if (this.channelParameters != null)
        tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty] = (object) this.channelParameters;
      tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.PrivacyNoticeVersionProperty] = (object) this.factory.PrivacyNoticeVersion;
      return tokenRequirement;
    }

    private InitiatorServiceModelSecurityTokenRequirement CreateInitiatorSecurityTokenRequirement(SecurityTokenParameters parameters, SecurityTokenAttachmentMode attachmentMode)
    {
      InitiatorServiceModelSecurityTokenRequirement tokenRequirement = this.CreateInitiatorSecurityTokenRequirement();
      parameters.InitializeSecurityTokenRequirement((SecurityTokenRequirement) tokenRequirement);
      tokenRequirement.KeyUsage = SecurityKeyUsage.Signature;
      tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.MessageDirectionProperty] = (object) MessageDirection.Output;
      tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.SupportingTokenAttachmentModeProperty] = (object) attachmentMode;
      return tokenRequirement;
    }

    private void AddSupportingTokenProviders(SupportingTokenParameters supportingTokenParameters, bool isOptional, IList<SupportingTokenProviderSpecification> providerSpecList)
    {
      if (supportingTokenParameters == null)
      {
          throw new ArgumentNullException("supportingTokenParameters");
      }
      else if (supportingTokenParameters.Endorsing == null)
      {
          throw new ArgumentNullException("supportingTokenParameters.Endorsing");
      }
      for (int index = 0; index < supportingTokenParameters.Endorsing.Count; ++index)
      {
        SecurityTokenRequirement tokenRequirement = (SecurityTokenRequirement) this.CreateInitiatorSecurityTokenRequirement(supportingTokenParameters.Endorsing[index], SecurityTokenAttachmentMode.Endorsing);
        try
        {
          if (isOptional)
            tokenRequirement.IsOptionalToken = true;
          SecurityTokenProvider securityTokenProvider = this.factory.SecurityTokenManager.CreateSecurityTokenProvider(tokenRequirement);
          if (securityTokenProvider != null)
          {
            SupportingTokenProviderSpecification providerSpecification = new SupportingTokenProviderSpecification(securityTokenProvider, SecurityTokenAttachmentMode.Endorsing, supportingTokenParameters.Endorsing[index]);
            providerSpecList.Add(providerSpecification);
          }
        }
        catch (Exception ex)
        {
          if (isOptional)
          {
            if (!Fx.IsFatal(ex))
              continue;
          }
          throw;
        }
      }
      for (int index = 0; index < supportingTokenParameters.SignedEndorsing.Count; ++index)
      {
        SecurityTokenRequirement tokenRequirement = (SecurityTokenRequirement) this.CreateInitiatorSecurityTokenRequirement(supportingTokenParameters.SignedEndorsing[index], SecurityTokenAttachmentMode.SignedEndorsing);
        try
        {
          if (isOptional)
            tokenRequirement.IsOptionalToken = true;
          SecurityTokenProvider securityTokenProvider = this.factory.SecurityTokenManager.CreateSecurityTokenProvider(tokenRequirement);
          if (securityTokenProvider != null)
          {
            SupportingTokenProviderSpecification providerSpecification = new SupportingTokenProviderSpecification(securityTokenProvider, SecurityTokenAttachmentMode.SignedEndorsing, supportingTokenParameters.SignedEndorsing[index]);
            providerSpecList.Add(providerSpecification);
          }
        }
        catch (Exception ex)
        {
          if (isOptional)
          {
            if (!Fx.IsFatal(ex))
              continue;
          }
          throw;
        }
      }
      for (int index = 0; index < supportingTokenParameters.SignedEncrypted.Count; ++index)
      {
        SecurityTokenRequirement tokenRequirement = (SecurityTokenRequirement) this.CreateInitiatorSecurityTokenRequirement(supportingTokenParameters.SignedEncrypted[index], SecurityTokenAttachmentMode.SignedEncrypted);
        try
        {
          if (isOptional)
            tokenRequirement.IsOptionalToken = true;
          SecurityTokenProvider securityTokenProvider = this.factory.SecurityTokenManager.CreateSecurityTokenProvider(tokenRequirement);
          if (securityTokenProvider != null)
          {
            SupportingTokenProviderSpecification providerSpecification = new SupportingTokenProviderSpecification(securityTokenProvider, SecurityTokenAttachmentMode.SignedEncrypted, supportingTokenParameters.SignedEncrypted[index]);
            providerSpecList.Add(providerSpecification);
          }
        }
        catch (Exception ex)
        {
          if (isOptional)
          {
            if (!Fx.IsFatal(ex))
              continue;
          }
          throw;
        }
      }
      for (int index = 0; index < supportingTokenParameters.Signed.Count; ++index)
      {
        SecurityTokenRequirement tokenRequirement = (SecurityTokenRequirement) this.CreateInitiatorSecurityTokenRequirement(supportingTokenParameters.Signed[index], SecurityTokenAttachmentMode.Signed);
        try
        {
          if (isOptional)
            tokenRequirement.IsOptionalToken = true;
          SecurityTokenProvider securityTokenProvider = this.factory.SecurityTokenManager.CreateSecurityTokenProvider(tokenRequirement);
          if (securityTokenProvider != null)
          {
            SupportingTokenProviderSpecification providerSpecification = new SupportingTokenProviderSpecification(securityTokenProvider, SecurityTokenAttachmentMode.Signed, supportingTokenParameters.Signed[index]);
            providerSpecList.Add(providerSpecification);
          }
        }
        catch (Exception ex)
        {
          if (isOptional)
          {
            if (!Fx.IsFatal(ex))
              continue;
          }
          throw;
        }
      }
    }

    private void MergeSupportingTokenProviders(TimeSpan timeout)
    {
      if (this.ScopedSupportingTokenProviderSpecification.Count == 0)
      {
        this.mergedSupportingTokenProvidersMap = (Dictionary<string, Collection<SupportingTokenProviderSpecification>>) null;
      }
      else
      {
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        this.factory.ExpectSupportingTokens = true;
        this.mergedSupportingTokenProvidersMap = new Dictionary<string, Collection<SupportingTokenProviderSpecification>>();
        foreach (string key in this.ScopedSupportingTokenProviderSpecification.Keys)
        {
          ICollection<SupportingTokenProviderSpecification> providerSpecifications = this.ScopedSupportingTokenProviderSpecification[key];
          if (providerSpecifications != null && providerSpecifications.Count != 0)
          {
            Collection<SupportingTokenProviderSpecification> collection = new Collection<SupportingTokenProviderSpecification>();
            foreach (SupportingTokenProviderSpecification providerSpecification in (IEnumerable<SupportingTokenProviderSpecification>) this.channelSupportingTokenProviderSpecification)
              collection.Add(providerSpecification);
            foreach (SupportingTokenProviderSpecification providerSpecification in (IEnumerable<SupportingTokenProviderSpecification>) providerSpecifications)
            {
              SecurityUtils.OpenTokenProviderIfRequired(providerSpecification.TokenProvider, timeoutHelper.RemainingTime());
#if FEATURE_CORECLR
              // TokenParameters.HasAsymmetricKey not supported
#else
              if ((providerSpecification.SecurityTokenAttachmentMode == SecurityTokenAttachmentMode.Endorsing || providerSpecification.SecurityTokenAttachmentMode == SecurityTokenAttachmentMode.SignedEndorsing) && (providerSpecification.TokenParameters.RequireDerivedKeys && !providerSpecification.TokenParameters.HasAsymmetricKey))
                this.factory.ExpectKeyDerivation = true;
#endif
              collection.Add(providerSpecification);
            }
            this.mergedSupportingTokenProvidersMap.Add(key, collection);
          }
        }
      }
    }

    public void Open(TimeSpan timeout)
    {
      this.communicationObject.Open(timeout);
    }

    public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
    {
      return this.communicationObject.BeginOpen(timeout, callback, state);
    }

    public void EndOpen(IAsyncResult result)
    {
      this.communicationObject.EndOpen(result);
    }

    public virtual void OnOpen(TimeSpan timeout)
    {
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      if (!this.factory.ActAsInitiator)
        return;
      this.channelSupportingTokenProviderSpecification = (ICollection<SupportingTokenProviderSpecification>) new Collection<SupportingTokenProviderSpecification>();
      this.scopedSupportingTokenProviderSpecification = new Dictionary<string, ICollection<SupportingTokenProviderSpecification>>();
      this.factory.SecurityBindingElement.SetEndpointSupportingTokenParametersIfNull();
      this.AddSupportingTokenProviders(this.factory.SecurityBindingElement.EndpointSupportingTokenParameters, false, (IList<SupportingTokenProviderSpecification>) this.channelSupportingTokenProviderSpecification);
      this.AddSupportingTokenProviders(this.factory.SecurityBindingElement.OptionalEndpointSupportingTokenParameters, true, (IList<SupportingTokenProviderSpecification>) this.channelSupportingTokenProviderSpecification);
      foreach (string key in (IEnumerable<string>) this.factory.SecurityBindingElement.OperationSupportingTokenParameters.Keys)
      {
        Collection<SupportingTokenProviderSpecification> collection = new Collection<SupportingTokenProviderSpecification>();
        this.AddSupportingTokenProviders(this.factory.SecurityBindingElement.OperationSupportingTokenParameters[key], false, (IList<SupportingTokenProviderSpecification>) collection);
        this.scopedSupportingTokenProviderSpecification.Add(key, (ICollection<SupportingTokenProviderSpecification>) collection);
      }
      foreach (string key in (IEnumerable<string>) this.factory.SecurityBindingElement.OptionalOperationSupportingTokenParameters.Keys)
      {
        ICollection<SupportingTokenProviderSpecification> providerSpecifications;
        Collection<SupportingTokenProviderSpecification> collection;
        if (this.scopedSupportingTokenProviderSpecification.TryGetValue(key, out providerSpecifications))
        {
          collection = (Collection<SupportingTokenProviderSpecification>) providerSpecifications;
        }
        else
        {
          collection = new Collection<SupportingTokenProviderSpecification>();
          this.scopedSupportingTokenProviderSpecification.Add(key, (ICollection<SupportingTokenProviderSpecification>) collection);
        }
        this.AddSupportingTokenProviders(this.factory.SecurityBindingElement.OptionalOperationSupportingTokenParameters[key], true, (IList<SupportingTokenProviderSpecification>) collection);
      }
      if (!this.channelSupportingTokenProviderSpecification.IsReadOnly)
      {
        if (this.channelSupportingTokenProviderSpecification.Count == 0)
        {
          this.channelSupportingTokenProviderSpecification = (ICollection<SupportingTokenProviderSpecification>) SecurityProtocol.EmptyTokenProviders;
        }
        else
        {
          this.factory.ExpectSupportingTokens = true;
          foreach (SupportingTokenProviderSpecification providerSpecification in (IEnumerable<SupportingTokenProviderSpecification>) this.channelSupportingTokenProviderSpecification)
          {
            SecurityUtils.OpenTokenProviderIfRequired(providerSpecification.TokenProvider, timeoutHelper.RemainingTime());
            if ((providerSpecification.SecurityTokenAttachmentMode == SecurityTokenAttachmentMode.Endorsing || providerSpecification.SecurityTokenAttachmentMode == SecurityTokenAttachmentMode.SignedEndorsing) && (providerSpecification.TokenParameters.RequireDerivedKeys && !providerSpecification.TokenParameters.HasAsymmetricKey))
              this.factory.ExpectKeyDerivation = true;
          }
          this.channelSupportingTokenProviderSpecification = (ICollection<SupportingTokenProviderSpecification>) new ReadOnlyCollection<SupportingTokenProviderSpecification>((IList<SupportingTokenProviderSpecification>) this.channelSupportingTokenProviderSpecification);
        }
      }
      this.MergeSupportingTokenProviders(timeoutHelper.RemainingTime());
    }

    public void Close(bool aborted, TimeSpan timeout)
    {
      if (aborted)
        this.communicationObject.Abort();
      else
        this.communicationObject.Close(timeout);
    }

    public virtual void OnAbort()
    {
      if (!this.factory.ActAsInitiator)
        return;
      foreach (SupportingTokenProviderSpecification providerSpecification in (IEnumerable<SupportingTokenProviderSpecification>) this.channelSupportingTokenProviderSpecification)
        SecurityUtils.AbortTokenProviderIfRequired(providerSpecification.TokenProvider);
      foreach (string key in this.scopedSupportingTokenProviderSpecification.Keys)
      {
        foreach (SupportingTokenProviderSpecification providerSpecification in (IEnumerable<SupportingTokenProviderSpecification>) this.scopedSupportingTokenProviderSpecification[key])
          SecurityUtils.AbortTokenProviderIfRequired(providerSpecification.TokenProvider);
      }
    }

    public virtual void OnClose(TimeSpan timeout)
    {
      if (!this.factory.ActAsInitiator)
        return;
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      foreach (SupportingTokenProviderSpecification providerSpecification in (IEnumerable<SupportingTokenProviderSpecification>) this.channelSupportingTokenProviderSpecification)
        SecurityUtils.CloseTokenProviderIfRequired(providerSpecification.TokenProvider, timeoutHelper.RemainingTime());
      foreach (string key in this.scopedSupportingTokenProviderSpecification.Keys)
      {
        foreach (SupportingTokenProviderSpecification providerSpecification in (IEnumerable<SupportingTokenProviderSpecification>) this.scopedSupportingTokenProviderSpecification[key])
          SecurityUtils.CloseTokenProviderIfRequired(providerSpecification.TokenProvider, timeoutHelper.RemainingTime());
      }
    }

    public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
    {
      return this.communicationObject.BeginClose(timeout, callback, state);
    }

    public void EndClose(IAsyncResult result)
    {
      this.communicationObject.EndClose(result);
    }

    private static void SetSecurityHeaderId(SendSecurityHeader securityHeader, Message message)
    {
#if FEATURE_CORECLR
      SecurityMessageProperty security = null;
#else
      SecurityMessageProperty security = message.Properties.Security;
#endif
      if (security == null)
        return;
      securityHeader.IdPrefix = security.SenderIdPrefix;
    }

    private void AddSupportingTokenSpecification(SecurityMessageProperty security, IList<SecurityToken> tokens, SecurityTokenAttachmentMode attachmentMode, IDictionary<SecurityToken, ReadOnlyCollection<IAuthorizationPolicy>> tokenPoliciesMapping)
    {
      if (tokens == null || tokens.Count == 0)
        return;
      for (int index = 0; index < tokens.Count; ++index)
        security.IncomingSupportingTokens.Add(new SupportingTokenSpecification(tokens[index], tokenPoliciesMapping[tokens[index]], attachmentMode));
    }

    protected void AddSupportingTokenSpecification(SecurityMessageProperty security, IList<SecurityToken> basicTokens, IList<SecurityToken> endorsingTokens, IList<SecurityToken> signedEndorsingTokens, IList<SecurityToken> signedTokens, IDictionary<SecurityToken, ReadOnlyCollection<IAuthorizationPolicy>> tokenPoliciesMapping)
    {
      this.AddSupportingTokenSpecification(security, basicTokens, SecurityTokenAttachmentMode.SignedEncrypted, tokenPoliciesMapping);
      this.AddSupportingTokenSpecification(security, endorsingTokens, SecurityTokenAttachmentMode.Endorsing, tokenPoliciesMapping);
      this.AddSupportingTokenSpecification(security, signedEndorsingTokens, SecurityTokenAttachmentMode.SignedEndorsing, tokenPoliciesMapping);
      this.AddSupportingTokenSpecification(security, signedTokens, SecurityTokenAttachmentMode.Signed, tokenPoliciesMapping);
    }

    protected SendSecurityHeader CreateSendSecurityHeader(Message message, string actor, SecurityProtocolFactory factory)
    {
      return this.CreateSendSecurityHeader(message, actor, factory, true);
    }

    protected SendSecurityHeader CreateSendSecurityHeaderForTransportProtocol(Message message, string actor, SecurityProtocolFactory factory)
    {
      return this.CreateSendSecurityHeader(message, actor, factory, false);
    }

    private SendSecurityHeader CreateSendSecurityHeader(Message message, string actor, SecurityProtocolFactory factory, bool requireMessageProtection)
    {
      MessageDirection direction = factory.ActAsInitiator ? MessageDirection.Input : MessageDirection.Output;
      SendSecurityHeader sendSecurityHeader = factory.StandardsManager.CreateSendSecurityHeader(message, actor, true, false, factory.OutgoingAlgorithmSuite, direction);
      sendSecurityHeader.Layout = factory.SecurityHeaderLayout;
      sendSecurityHeader.RequireMessageProtection = requireMessageProtection;
      SecurityProtocol.SetSecurityHeaderId(sendSecurityHeader, message);
      if (factory.AddTimestamp)
        sendSecurityHeader.AddTimestamp(factory.TimestampValidityDuration);
      sendSecurityHeader.StreamBufferManager = factory.StreamBufferManager;
      return sendSecurityHeader;
    }

    internal void AddMessageSupportingTokens(Message message, ref IList<SupportingTokenSpecification> supportingTokens)
    {
#if FEATURE_CORECLR
        // Not implemented
      SecurityMessageProperty security = null;
#else
      SecurityMessageProperty security = message.Properties.Security;
#endif
      if (security == null || !security.HasOutgoingSupportingTokens)
        return;
      if (supportingTokens == null)
        supportingTokens = (IList<SupportingTokenSpecification>) new Collection<SupportingTokenSpecification>();
      for (int index = 0; index < security.OutgoingSupportingTokens.Count; ++index)
      {
        SupportingTokenSpecification outgoingSupportingToken = security.OutgoingSupportingTokens[index];
        if (outgoingSupportingToken.SecurityTokenParameters == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("SenderSideSupportingTokensMustSpecifySecurityTokenParameters")));
        supportingTokens.Add(outgoingSupportingToken);
      }
    }

    internal bool TryGetSupportingTokens(SecurityProtocolFactory factory, EndpointAddress target, Uri via, Message message, TimeSpan timeout, bool isBlockingCall, out IList<SupportingTokenSpecification> supportingTokens)
    {
      if (!factory.ActAsInitiator)
      {
        supportingTokens = (IList<SupportingTokenSpecification>) null;
        return true;
      }
      if (message == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      supportingTokens = (IList<SupportingTokenSpecification>) null;
      IList<SupportingTokenProviderSpecification> supportingTokenProviders = this.GetSupportingTokenProviders(message.Headers.Action);
      if (supportingTokenProviders != null && supportingTokenProviders.Count > 0)
      {
        if (!isBlockingCall)
          return false;
        supportingTokens = (IList<SupportingTokenSpecification>) new Collection<SupportingTokenSpecification>();
        for (int index = 0; index < supportingTokenProviders.Count; ++index)
        {
          SupportingTokenProviderSpecification providerSpecification = supportingTokenProviders[index];
          CompatibilityShim.Log("Skipping KerberosSecurityTokenParameters - should not be needed");
//           SecurityToken token = !(this is TransportSecurityProtocol) || !(providerSpecification.TokenParameters is KerberosSecurityTokenParameters) ? providerSpecification.TokenProvider.GetToken(timeoutHelper.RemainingTime()) : (SecurityToken) new ProviderBackedSecurityToken(providerSpecification.TokenProvider, timeoutHelper.RemainingTime());
          SecurityToken token = !(this is TransportSecurityProtocol) ? providerSpecification.TokenProvider.GetToken(timeoutHelper.RemainingTime()) : (SecurityToken) new ProviderBackedSecurityToken(providerSpecification.TokenProvider, timeoutHelper.RemainingTime());
          supportingTokens.Add(new SupportingTokenSpecification(token, EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance, providerSpecification.SecurityTokenAttachmentMode, providerSpecification.TokenParameters));
        }
      }
      this.AddMessageSupportingTokens(message, ref supportingTokens);
      return true;
    }

    protected IList<SupportingTokenAuthenticatorSpecification> GetSupportingTokenAuthenticatorsAndSetExpectationFlags(SecurityProtocolFactory factory, Message message, ReceiveSecurityHeader securityHeader)
    {
      if (factory.ActAsInitiator)
        return (IList<SupportingTokenAuthenticatorSpecification>) null;
      if (message == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
      bool expectSignedTokens;
      bool expectBasicTokens;
      bool expectEndorsingTokens;
      IList<SupportingTokenAuthenticatorSpecification> tokenAuthenticators = factory.GetSupportingTokenAuthenticators(message.Headers.Action, out expectSignedTokens, out expectBasicTokens, out expectEndorsingTokens);
#if FEATURE_CORECLR
      throw new NotImplementedException("SecurityHeader.expect*Tokens is not supported in .NET Core");
#else
      securityHeader.ExpectBasicTokens = expectBasicTokens;
      securityHeader.ExpectEndorsingTokens = expectEndorsingTokens;
      securityHeader.ExpectSignedTokens = expectSignedTokens;
      return tokenAuthenticators;
#endif
    }

    protected ReadOnlyCollection<SecurityTokenResolver> MergeOutOfBandResolvers(IList<SupportingTokenAuthenticatorSpecification> supportingAuthenticators, ReadOnlyCollection<SecurityTokenResolver> primaryResolvers)
    {
      Collection<SecurityTokenResolver> collection = (Collection<SecurityTokenResolver>) null;
      if (supportingAuthenticators != null && supportingAuthenticators.Count > 0)
      {
        for (int index = 0; index < supportingAuthenticators.Count; ++index)
        {
          if (supportingAuthenticators[index].TokenResolver != null)
          {
            collection = collection ?? new Collection<SecurityTokenResolver>();
            collection.Add(supportingAuthenticators[index].TokenResolver);
          }
        }
      }
      if (collection == null)
        return primaryResolvers ?? EmptyReadOnlyCollection<SecurityTokenResolver>.Instance;
      if (primaryResolvers != null)
      {
        for (int index = 0; index < primaryResolvers.Count; ++index)
          collection.Insert(0, primaryResolvers[index]);
      }
      return new ReadOnlyCollection<SecurityTokenResolver>((IList<SecurityTokenResolver>) collection);
    }
	
    protected void AddSupportingTokens(SendSecurityHeader securityHeader, IList<SupportingTokenSpecification> supportingTokens)
    {
      if (supportingTokens == null)
        return;
      for (int index = 0; index < supportingTokens.Count; ++index)
      {
        SecurityToken securityToken = supportingTokens[index].SecurityToken;
        SecurityTokenParameters securityTokenParameters = supportingTokens[index].SecurityTokenParameters;
        switch (supportingTokens[index].SecurityTokenAttachmentMode)
        {
          case SecurityTokenAttachmentMode.Signed:
            securityHeader.AddSignedSupportingToken(securityToken, securityTokenParameters);
            break;
          case SecurityTokenAttachmentMode.Endorsing:
            securityHeader.AddEndorsingSupportingToken(securityToken, securityTokenParameters);
            break;
          case SecurityTokenAttachmentMode.SignedEndorsing:
            securityHeader.AddSignedEndorsingSupportingToken(securityToken, securityTokenParameters);
            break;
          case SecurityTokenAttachmentMode.SignedEncrypted:
            securityHeader.AddBasicSupportingToken(securityToken, securityTokenParameters);
            break;
          default:
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnknownTokenAttachmentMode", new object[1]{ (object) supportingTokens[index].SecurityTokenAttachmentMode.ToString() })));
        }
      }
    }

    public virtual IAsyncResult BeginSecureOutgoingMessage(Message message, TimeSpan timeout, AsyncCallback callback, object state)
    {
      this.SecureOutgoingMessage(ref message, timeout);
      return (IAsyncResult) new CompletedAsyncResult<Message>(message, callback, state);
    }

    public virtual IAsyncResult BeginSecureOutgoingMessage(Message message, TimeSpan timeout, SecurityProtocolCorrelationState correlationState, AsyncCallback callback, object state)
    {
      SecurityProtocolCorrelationState parameter = this.SecureOutgoingMessage(ref message, timeout, correlationState);
      return (IAsyncResult) new CompletedAsyncResult<Message, SecurityProtocolCorrelationState>(message, parameter, callback, state);
    }

    public virtual IAsyncResult BeginVerifyIncomingMessage(Message message, TimeSpan timeout, AsyncCallback callback, object state)
    {
      this.VerifyIncomingMessage(ref message, timeout);
      return (IAsyncResult) new CompletedAsyncResult<Message>(message, callback, state);
    }

    public virtual IAsyncResult BeginVerifyIncomingMessage(Message message, TimeSpan timeout, SecurityProtocolCorrelationState[] correlationStates, AsyncCallback callback, object state)
    {
      SecurityProtocolCorrelationState parameter = this.VerifyIncomingMessage(ref message, timeout, correlationStates);
      return (IAsyncResult) new CompletedAsyncResult<Message, SecurityProtocolCorrelationState>(message, parameter, callback, state);
    }

    public virtual void EndSecureOutgoingMessage(IAsyncResult result, out Message message)
    {
      message = CompletedAsyncResult<Message>.End(result);
    }

    public virtual void EndSecureOutgoingMessage(IAsyncResult result, out Message message, out SecurityProtocolCorrelationState newCorrelationState)
    {
      message = CompletedAsyncResult<Message, SecurityProtocolCorrelationState>.End(result, out newCorrelationState);
    }

    public virtual void EndVerifyIncomingMessage(IAsyncResult result, out Message message)
    {
      message = CompletedAsyncResult<Message>.End(result);
    }

    public virtual void EndVerifyIncomingMessage(IAsyncResult result, out Message message, out SecurityProtocolCorrelationState newCorrelationState)
    {
      message = CompletedAsyncResult<Message, SecurityProtocolCorrelationState>.End(result, out newCorrelationState);
    }

    internal static SecurityToken GetToken(SecurityTokenProvider provider, EndpointAddress target, TimeSpan timeout)
    {
      if (provider == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenProviderCannotGetTokensForTarget", new object[1]{ (object) target })));
      try
      {
        return provider.GetToken(timeout);
      }
      catch (SecurityTokenException ex)
      {
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenProviderCannotGetTokensForTarget", new object[1]{ (object) target }), (Exception) ex));
      }
      catch (SecurityNegotiationException ex)
      {
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new SecurityNegotiationException(SR.GetString("TokenProviderCannotGetTokensForTarget", new object[1]{ (object) target }), (Exception) ex));
      }
    }

    public abstract void SecureOutgoingMessage(ref Message message, TimeSpan timeout);

    public virtual SecurityProtocolCorrelationState SecureOutgoingMessage(ref Message message, TimeSpan timeout, SecurityProtocolCorrelationState correlationState)
    {
      this.SecureOutgoingMessage(ref message, timeout);
      return (SecurityProtocolCorrelationState) null;
    }

    protected virtual void OnOutgoingMessageSecured(Message securedMessage)
    {
      SecurityTraceRecordHelper.TraceOutgoingMessageSecured(this, securedMessage);
    }

    protected virtual void OnSecureOutgoingMessageFailure(Message message)
    {
      SecurityTraceRecordHelper.TraceSecureOutgoingMessageFailure(this, message);
    }

    public abstract void VerifyIncomingMessage(ref Message message, TimeSpan timeout);

    public virtual SecurityProtocolCorrelationState VerifyIncomingMessage(ref Message message, TimeSpan timeout, params SecurityProtocolCorrelationState[] correlationStates)
    {
      this.VerifyIncomingMessage(ref message, timeout);
      return (SecurityProtocolCorrelationState) null;
    }

    protected virtual void OnIncomingMessageVerified(Message verifiedMessage)
    {
      SecurityTraceRecordHelper.TraceIncomingMessageVerified(this, verifiedMessage);
      if (AuditLevel.Success != (this.factory.MessageAuthenticationAuditLevel & AuditLevel.Success))
        return;
#if FEATURE_CORECLR
        // Not implemented
#else
      SecurityAuditHelper.WriteMessageAuthenticationSuccessEvent(this.factory.AuditLogLocation, this.factory.SuppressAuditFailure, verifiedMessage, verifiedMessage.Headers.To, verifiedMessage.Headers.Action, SecurityUtils.GetIdentityNamesFromContext(verifiedMessage.Properties.Security.ServiceSecurityContext.AuthorizationContext));
#endif
    }

    protected virtual void OnVerifyIncomingMessageFailure(Message message, Exception exception)
    {
      SecurityTraceRecordHelper.TraceVerifyIncomingMessageFailure(this, message);
#if !FEATURE_CORECLR
      // Skip performance counters in .NET Core
      if (PerformanceCounters.PerformanceCountersEnabled && (Uri) null != this.factory.ListenUri && (exception.GetType() == typeof (MessageSecurityException) || exception.GetType().IsSubclassOf(typeof (MessageSecurityException)) || (exception.GetType() == typeof (SecurityTokenException) || exception.GetType().IsSubclassOf(typeof (SecurityTokenException)))))
        PerformanceCounters.AuthenticationFailed(message, this.factory.ListenUri);
#endif
      if (AuditLevel.Failure != (this.factory.MessageAuthenticationAuditLevel & AuditLevel.Failure))
        return;
      try
      {
#if FEATURE_CORECLR
        // Not implemented
        string clientIdentity = SecurityUtils.AnonymousIdentity.Name;
#else
        SecurityMessageProperty security = message.Properties.Security;
        string clientIdentity = security == null || security.ServiceSecurityContext == null ? SecurityUtils.AnonymousIdentity.Name : SecurityUtils.GetIdentityNamesFromContext(security.ServiceSecurityContext.AuthorizationContext);
        SecurityAuditHelper.WriteMessageAuthenticationFailureEvent(this.factory.AuditLogLocation, this.factory.SuppressAuditFailure, message, message.Headers.To, message.Headers.Action, clientIdentity, exception);
#endif
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          DiagnosticUtility.TraceHandledException(ex, TraceEventType.Error);
      }
    }

    protected abstract class GetSupportingTokensAsyncResult : System.Runtime.AsyncResult
    {
      private static AsyncCallback getSupportingTokensCallback = Fx.ThunkCallback(new AsyncCallback(SecurityProtocol.GetSupportingTokensAsyncResult.GetSupportingTokenCallback));
      private SecurityProtocol binding;
      private Message message;
      private IList<SupportingTokenSpecification> supportingTokens;
      private int currentTokenProviderIndex;
      private IList<SupportingTokenProviderSpecification> supportingTokenProviders;
      private TimeoutHelper timeoutHelper;

      protected IList<SupportingTokenSpecification> SupportingTokens
      {
        get
        {
          return this.supportingTokens;
        }
      }

      public GetSupportingTokensAsyncResult(Message m, SecurityProtocol binding, TimeSpan timeout, AsyncCallback callback, object state)
        : base(callback, state)
      {
        this.message = m;
        this.binding = binding;
        this.timeoutHelper = new TimeoutHelper(timeout);
      }

      protected abstract bool OnGetSupportingTokensDone(TimeSpan timeout);

      private static void GetSupportingTokenCallback(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        SecurityProtocol.GetSupportingTokensAsyncResult asyncState = (SecurityProtocol.GetSupportingTokensAsyncResult) result.AsyncState;
        Exception exception = (Exception) null;
        bool flag;
        try
        {
          asyncState.AddSupportingToken(result);
          flag = asyncState.AddSupportingTokens();
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

      private void AddSupportingToken(IAsyncResult result)
      {
        SupportingTokenProviderSpecification supportingTokenProvider = this.supportingTokenProviders[this.currentTokenProviderIndex];
        if (result is SecurityTokenProvider.SecurityTokenAsyncResult)
          this.supportingTokens.Add(new SupportingTokenSpecification(SecurityTokenProvider.SecurityTokenAsyncResult.End(result), EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance, supportingTokenProvider.SecurityTokenAttachmentMode, supportingTokenProvider.TokenParameters));
        else
          this.supportingTokens.Add(new SupportingTokenSpecification(supportingTokenProvider.TokenProvider.EndGetToken(result), EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance, supportingTokenProvider.SecurityTokenAttachmentMode, supportingTokenProvider.TokenParameters));
        this.currentTokenProviderIndex = this.currentTokenProviderIndex + 1;
      }

      private bool AddSupportingTokens()
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("KerberosSecurityTokenParameters is not supported in .NET Core");
#else
        while (this.currentTokenProviderIndex < this.supportingTokenProviders.Count)
        {
          SupportingTokenProviderSpecification supportingTokenProvider = this.supportingTokenProviders[this.currentTokenProviderIndex];
          IAsyncResult result = !(this.binding is TransportSecurityProtocol) || !(supportingTokenProvider.TokenParameters is KerberosSecurityTokenParameters) ? supportingTokenProvider.TokenProvider.BeginGetToken(this.timeoutHelper.RemainingTime(), SecurityProtocol.GetSupportingTokensAsyncResult.getSupportingTokensCallback, (object) this) : (IAsyncResult) new SecurityTokenProvider.SecurityTokenAsyncResult((SecurityToken) new ProviderBackedSecurityToken(supportingTokenProvider.TokenProvider, this.timeoutHelper.RemainingTime()), (AsyncCallback) null, (object) this);
          if (!result.CompletedSynchronously)
            return false;
          this.AddSupportingToken(result);
        }
        this.binding.AddMessageSupportingTokens(this.message, ref this.supportingTokens);
        return this.OnGetSupportingTokensDone(this.timeoutHelper.RemainingTime());
#endif
      }

      protected void Start()
      {
        bool flag;
        if (this.binding.TryGetSupportingTokens(this.binding.SecurityProtocolFactory, this.binding.Target, this.binding.Via, this.message, this.timeoutHelper.RemainingTime(), false, out this.supportingTokens))
        {
          flag = this.OnGetSupportingTokensDone(this.timeoutHelper.RemainingTime());
        }
        else
        {
          this.supportingTokens = (IList<SupportingTokenSpecification>) new Collection<SupportingTokenSpecification>();
          this.supportingTokenProviders = this.binding.GetSupportingTokenProviders(this.message.Headers.Action);
          if (this.supportingTokenProviders == null || this.supportingTokenProviders.Count <= 0)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException("There must be at least 1 supporting token provider"));
          flag = this.AddSupportingTokens();
        }
        if (!flag)
          return;
        this.Complete(true);
      }
    }
  }
}
