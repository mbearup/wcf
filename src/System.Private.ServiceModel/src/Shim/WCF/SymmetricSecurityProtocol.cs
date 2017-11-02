// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SymmetricSecurityProtocol
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
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
  internal sealed class SymmetricSecurityProtocol : MessageSecurityProtocol
  {
// Trim for testing...

    private SecurityTokenProvider initiatorSymmetricTokenProvider;
    private SecurityTokenProvider initiatorAsymmetricTokenProvider;
    private SecurityTokenAuthenticator initiatorTokenAuthenticator;

    private SymmetricSecurityProtocolFactory Factory
    {
      get
      {
        return (SymmetricSecurityProtocolFactory) this.MessageSecurityProtocolFactory;
      }
    }

    public SecurityTokenProvider InitiatorSymmetricTokenProvider
    {
      get
      {
        this.CommunicationObject.ThrowIfNotOpened();
        return this.initiatorSymmetricTokenProvider;
      }
    }

    public SecurityTokenProvider InitiatorAsymmetricTokenProvider
    {
      get
      {
        this.CommunicationObject.ThrowIfNotOpened();
        return this.initiatorAsymmetricTokenProvider;
      }
    }

    public SecurityTokenAuthenticator InitiatorTokenAuthenticator
    {
      get
      {
        this.CommunicationObject.ThrowIfNotOpened();
        return this.initiatorTokenAuthenticator;
      }
    }

    public SymmetricSecurityProtocol(SymmetricSecurityProtocolFactory factory, EndpointAddress target, Uri via)
      : base((MessageSecurityProtocolFactory) factory, target, via)
    {
    }

    private InitiatorServiceModelSecurityTokenRequirement CreateInitiatorTokenRequirement()
    {
      InitiatorServiceModelSecurityTokenRequirement tokenRequirement = this.CreateInitiatorSecurityTokenRequirement();
      this.Factory.SecurityTokenParameters.InitializeSecurityTokenRequirement((SecurityTokenRequirement) tokenRequirement);
      tokenRequirement.KeyUsage = this.Factory.SecurityTokenParameters.HasAsymmetricKey ? SecurityKeyUsage.Exchange : SecurityKeyUsage.Signature;
      tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.MessageDirectionProperty] = (object) MessageDirection.Output;
      if (this.Factory.SecurityTokenParameters.HasAsymmetricKey)
        tokenRequirement.IsOutOfBandToken = true;
      return tokenRequirement;
    }

    public override void OnOpen(TimeSpan timeout)
    {
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      base.OnOpen(timeoutHelper.RemainingTime());
      if (!this.Factory.ActAsInitiator)
        return;
      SecurityTokenProvider securityTokenProvider = this.Factory.SecurityTokenManager.CreateSecurityTokenProvider((SecurityTokenRequirement) this.CreateInitiatorTokenRequirement());
      SecurityUtils.OpenTokenProviderIfRequired(securityTokenProvider, timeoutHelper.RemainingTime());
      if (this.Factory.SecurityTokenParameters.HasAsymmetricKey)
        this.initiatorAsymmetricTokenProvider = securityTokenProvider;
      else
        this.initiatorSymmetricTokenProvider = securityTokenProvider;
      SecurityTokenResolver outOfBandTokenResolver;
      this.initiatorTokenAuthenticator = this.Factory.SecurityTokenManager.CreateSecurityTokenAuthenticator((SecurityTokenRequirement) this.CreateInitiatorTokenRequirement(), out outOfBandTokenResolver);
      SecurityUtils.OpenTokenAuthenticatorIfRequired(this.initiatorTokenAuthenticator, timeoutHelper.RemainingTime());
    }

    public override void OnAbort()
    {
      if (this.Factory.ActAsInitiator)
      {
        SecurityTokenProvider tokenProvider = this.initiatorSymmetricTokenProvider ?? this.initiatorAsymmetricTokenProvider;
        if (tokenProvider != null)
          SecurityUtils.AbortTokenProviderIfRequired(tokenProvider);
        if (this.initiatorTokenAuthenticator != null)
          SecurityUtils.AbortTokenAuthenticatorIfRequired(this.initiatorTokenAuthenticator);
      }
      base.OnAbort();
    }

    public override void OnClose(TimeSpan timeout)
    {
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      if (this.Factory.ActAsInitiator)
      {
        SecurityTokenProvider tokenProvider = this.initiatorSymmetricTokenProvider ?? this.initiatorAsymmetricTokenProvider;
        if (tokenProvider != null)
          SecurityUtils.CloseTokenProviderIfRequired(tokenProvider, timeoutHelper.RemainingTime());
        if (this.initiatorTokenAuthenticator != null)
          SecurityUtils.CloseTokenAuthenticatorIfRequired(this.initiatorTokenAuthenticator, timeoutHelper.RemainingTime());
      }
      base.OnClose(timeoutHelper.RemainingTime());
    }

    private SecurityTokenProvider GetTokenProvider()
    {
      if (this.Factory.ActAsInitiator)
        return this.initiatorSymmetricTokenProvider ?? this.initiatorAsymmetricTokenProvider;
      return this.Factory.RecipientAsymmetricTokenProvider;
    }

    protected override IAsyncResult BeginSecureOutgoingMessageCore(Message message, TimeSpan timeout, SecurityProtocolCorrelationState correlationState, AsyncCallback callback, object state)
    {
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      SecurityToken token;
      SecurityTokenParameters tokenParameters;
      SecurityToken prerequisiteWrappingToken;
      IList<SupportingTokenSpecification> supportingTokens;
      SecurityProtocolCorrelationState newCorrelationState;
      if (this.TryGetTokenSynchronouslyForOutgoingSecurity(message, correlationState, false, timeoutHelper.RemainingTime(), out token, out tokenParameters, out prerequisiteWrappingToken, out supportingTokens, out newCorrelationState))
      {
        this.SetUpDelayedSecurityExecution(ref message, prerequisiteWrappingToken, token, tokenParameters, supportingTokens, this.GetSignatureConfirmationCorrelationState(correlationState, newCorrelationState));
        return (IAsyncResult) new CompletedAsyncResult<Message, SecurityProtocolCorrelationState>(message, newCorrelationState, callback, state);
      }
      if (!this.Factory.ActAsInitiator)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("ProtocolMustBeInitiator", new object[1]{ (object) this.GetType().ToString() })));
      SecurityTokenProvider tokenProvider = this.GetTokenProvider();
      return (IAsyncResult) new SymmetricSecurityProtocol.SecureOutgoingMessageAsyncResult(message, this, tokenProvider, this.Factory.ApplyConfidentiality, this.initiatorTokenAuthenticator, correlationState, timeoutHelper.RemainingTime(), callback, state);
    }

    protected override SecurityProtocolCorrelationState SecureOutgoingMessageCore(ref Message message, TimeSpan timeout, SecurityProtocolCorrelationState correlationState)
    {
      SecurityToken token;
      SecurityTokenParameters tokenParameters;
      SecurityToken prerequisiteWrappingToken;
      IList<SupportingTokenSpecification> supportingTokens;
      SecurityProtocolCorrelationState newCorrelationState;
      this.TryGetTokenSynchronouslyForOutgoingSecurity(message, correlationState, true, timeout, out token, out tokenParameters, out prerequisiteWrappingToken, out supportingTokens, out newCorrelationState);
      this.SetUpDelayedSecurityExecution(ref message, prerequisiteWrappingToken, token, tokenParameters, supportingTokens, this.GetSignatureConfirmationCorrelationState(correlationState, newCorrelationState));
      return newCorrelationState;
    }

    private void SetUpDelayedSecurityExecution(ref Message message, SecurityToken prerequisiteToken, SecurityToken primaryToken, SecurityTokenParameters primaryTokenParameters, IList<SupportingTokenSpecification> supportingTokens, SecurityProtocolCorrelationState correlationState)
    {
      string empty = string.Empty;
      SendSecurityHeader sendSecurityHeader = this.ConfigureSendSecurityHeader(message, empty, supportingTokens, correlationState);
      if (prerequisiteToken != null)
        sendSecurityHeader.AddPrerequisiteToken(prerequisiteToken);
      if (this.Factory.ApplyIntegrity || sendSecurityHeader.HasSignedTokens)
      {
        if (!this.Factory.ApplyIntegrity)
          sendSecurityHeader.SignatureParts = MessagePartSpecification.NoParts;
        sendSecurityHeader.SetSigningToken(primaryToken, primaryTokenParameters);
      }
      if (this.Factory.ApplyConfidentiality || sendSecurityHeader.HasEncryptedTokens)
      {
        if (!this.Factory.ApplyConfidentiality)
          sendSecurityHeader.EncryptionParts = MessagePartSpecification.NoParts;
        sendSecurityHeader.SetEncryptionToken(primaryToken, primaryTokenParameters);
      }
      message = sendSecurityHeader.SetupExecution();
    }

    protected override void EndSecureOutgoingMessageCore(IAsyncResult result, out Message message, out SecurityProtocolCorrelationState newCorrelationState)
    {
      if (result is CompletedAsyncResult<Message, SecurityProtocolCorrelationState>)
        message = CompletedAsyncResult<Message, SecurityProtocolCorrelationState>.End(result, out newCorrelationState);
      else
      { 
#if FEATURE_CORECLR
        throw new NotImplementedException("GetOneTokenAndSetUpSecurityAsyncResult not supported in .NET core");
#else
        message = MessageSecurityProtocol.GetOneTokenAndSetUpSecurityAsyncResult.End(result, out newCorrelationState);
#endif
      }
    }

    private WrappedKeySecurityToken CreateWrappedKeyToken(SecurityToken wrappingToken, SecurityTokenParameters wrappingTokenParameters, SecurityTokenReferenceStyle wrappingTokenReferenceStyle)
    {
      int keyLength = Math.Max(128, this.Factory.OutgoingAlgorithmSuite.DefaultSymmetricKeyLength);
#if FEATURE_CORECLR
      throw new NotImplementedException("CryptoHelper.ValidateSymmetricKeyLength is not supported in .NET Core");
#else
      CryptoHelper.ValidateSymmetricKeyLength(keyLength, this.Factory.OutgoingAlgorithmSuite);
      byte[] numArray = new byte[keyLength / 8];
      CryptoHelper.FillRandomBytes(numArray);
      string id = SecurityUtils.GenerateId();
      string keyWrapAlgorithm = this.Factory.OutgoingAlgorithmSuite.DefaultAsymmetricKeyWrapAlgorithm;
      SecurityKeyIdentifierClause identifierClause = wrappingTokenParameters.CreateKeyIdentifierClause(wrappingToken, wrappingTokenReferenceStyle);
      return new WrappedKeySecurityToken(id, numArray, keyWrapAlgorithm, wrappingToken, new SecurityKeyIdentifier() { identifierClause });
#endif
    }

    private SecurityToken GetInitiatorToken(SecurityToken providerToken, Message message, TimeSpan timeout, out SecurityTokenParameters tokenParameters, out SecurityToken prerequisiteWrappingToken)
    {
      tokenParameters = (SecurityTokenParameters) null;
      prerequisiteWrappingToken = (SecurityToken) null;
      SecurityToken securityToken;
      if (this.Factory.SecurityTokenParameters.HasAsymmetricKey)
      {
        SecurityToken wrappingToken = providerToken;
        bool flag = SendSecurityHeader.ShouldSerializeToken(this.Factory.SecurityTokenParameters, MessageDirection.Input);
        if (flag)
          prerequisiteWrappingToken = wrappingToken;
        securityToken = (SecurityToken) this.CreateWrappedKeyToken(wrappingToken, this.Factory.SecurityTokenParameters, flag ? SecurityTokenReferenceStyle.Internal : SecurityTokenReferenceStyle.External);
      }
      else
        securityToken = providerToken;
      tokenParameters = this.Factory.GetProtectionTokenParameters();
      return securityToken;
    }

    private bool TryGetTokenSynchronouslyForOutgoingSecurity(Message message, SecurityProtocolCorrelationState correlationState, bool isBlockingCall, TimeSpan timeout, out SecurityToken token, out SecurityTokenParameters tokenParameters, out SecurityToken prerequisiteWrappingToken, out IList<SupportingTokenSpecification> supportingTokens, out SecurityProtocolCorrelationState newCorrelationState)
    {
      SymmetricSecurityProtocolFactory factory = this.Factory;
      supportingTokens = (IList<SupportingTokenSpecification>) null;
      prerequisiteWrappingToken = (SecurityToken) null;
      token = (SecurityToken) null;
      tokenParameters = (SecurityTokenParameters) null;
      newCorrelationState = (SecurityProtocolCorrelationState) null;
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      if (factory.ApplyIntegrity || factory.ApplyConfidentiality)
      {
        if (factory.ActAsInitiator)
        {
          if (!isBlockingCall || !this.TryGetSupportingTokens((SecurityProtocolFactory) factory, this.Target, this.Via, message, timeoutHelper.RemainingTime(), isBlockingCall, out supportingTokens))
            return false;
          SecurityToken outgoingIdentity = this.GetTokenAndEnsureOutgoingIdentity(this.GetTokenProvider(), factory.ApplyConfidentiality, timeoutHelper.RemainingTime(), this.initiatorTokenAuthenticator);
          token = this.GetInitiatorToken(outgoingIdentity, message, timeoutHelper.RemainingTime(), out tokenParameters, out prerequisiteWrappingToken);
          newCorrelationState = this.GetCorrelationState(token);
        }
        else
        {
          token = this.GetCorrelationToken(correlationState);
          tokenParameters = this.Factory.GetProtectionTokenParameters();
        }
      }
      return true;
    }

    private SecurityToken GetCorrelationToken(SecurityProtocolCorrelationState[] correlationStates, out SecurityTokenParameters correlationTokenParameters)
    {
      SecurityToken correlationToken = this.GetCorrelationToken(correlationStates);
      correlationTokenParameters = this.Factory.GetProtectionTokenParameters();
      return correlationToken;
    }

    private void EnsureWrappedToken(SecurityToken token, Message message)
    {
      if (!(token is WrappedKeySecurityToken))
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("IncomingSigningTokenMustBeAnEncryptedKey")), message);
    }

    protected override SecurityProtocolCorrelationState VerifyIncomingMessageCore(ref Message message, string actor, TimeSpan timeout, SecurityProtocolCorrelationState[] correlationStates)
    {
      SymmetricSecurityProtocolFactory factory = this.Factory;
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      IList<SupportingTokenAuthenticatorSpecification> supportingAuthenticators;
      ReceiveSecurityHeader securityHeader = this.ConfigureReceiveSecurityHeader(message, string.Empty, correlationStates, out supportingAuthenticators);
      SecurityToken requiredSigningToken = (SecurityToken) null;
      if (this.Factory.ActAsInitiator)
      {
        SecurityTokenParameters correlationTokenParameters;
        SecurityToken correlationToken = this.GetCorrelationToken(correlationStates, out correlationTokenParameters);
        securityHeader.ConfigureSymmetricBindingClientReceiveHeader(correlationToken, correlationTokenParameters);
        requiredSigningToken = correlationToken;
      }
      else
      {
        if (factory.RecipientSymmetricTokenAuthenticator != null)
        {
          securityHeader.ConfigureSymmetricBindingServerReceiveHeader(this.Factory.RecipientSymmetricTokenAuthenticator, this.Factory.SecurityTokenParameters, supportingAuthenticators);
        }
        else
        {
          securityHeader.ConfigureSymmetricBindingServerReceiveHeader(this.Factory.RecipientAsymmetricTokenProvider.GetToken(timeoutHelper.RemainingTime()), this.Factory.SecurityTokenParameters, supportingAuthenticators);
          securityHeader.WrappedKeySecurityTokenAuthenticator = this.Factory.WrappedKeySecurityTokenAuthenticator;
        }
        securityHeader.ConfigureOutOfBandTokenResolver(this.MergeOutOfBandResolvers(supportingAuthenticators, this.Factory.RecipientOutOfBandTokenResolverList));
      }
      this.ProcessSecurityHeader(securityHeader, ref message, requiredSigningToken, timeoutHelper.RemainingTime(), correlationStates);
      SecurityToken signatureToken = securityHeader.SignatureToken;
      if (factory.RequireIntegrity)
      {
        if (factory.SecurityTokenParameters.HasAsymmetricKey)
          this.EnsureWrappedToken(signatureToken, message);
        else
          MessageSecurityProtocol.EnsureNonWrappedToken(signatureToken, message);
        if (factory.ActAsInitiator)
        {
          if (!factory.SecurityTokenParameters.HasAsymmetricKey)
          {
            ReadOnlyCollection<IAuthorizationPolicy> protectionTokenPolicies = this.initiatorTokenAuthenticator.ValidateToken(signatureToken);
            this.DoIdentityCheckAndAttachInitiatorSecurityProperty(message, signatureToken, protectionTokenPolicies);
          }
          else
          {
            ReadOnlyCollection<IAuthorizationPolicy> protectionTokenPolicies = this.initiatorTokenAuthenticator.ValidateToken((signatureToken as WrappedKeySecurityToken).WrappingToken);
            this.DoIdentityCheckAndAttachInitiatorSecurityProperty(message, signatureToken, protectionTokenPolicies);
          }
        }
        else
          this.AttachRecipientSecurityProperty(message, signatureToken, this.Factory.SecurityTokenParameters.HasAsymmetricKey, (IList<SecurityToken>) securityHeader.BasicSupportingTokens, (IList<SecurityToken>) securityHeader.EndorsingSupportingTokens, (IList<SecurityToken>) securityHeader.SignedEndorsingSupportingTokens, (IList<SecurityToken>) securityHeader.SignedSupportingTokens, securityHeader.SecurityTokenAuthorizationPoliciesMapping);
      }
      return this.GetCorrelationState(signatureToken, securityHeader);
    }

    private sealed class SecureOutgoingMessageAsyncResult : MessageSecurityProtocol.GetOneTokenAndSetUpSecurityAsyncResult
    {
      private SymmetricSecurityProtocol symmetricBinding;

      public SecureOutgoingMessageAsyncResult(Message m, SymmetricSecurityProtocol binding, SecurityTokenProvider provider, bool doIdentityChecks, SecurityTokenAuthenticator identityCheckAuthenticator, SecurityProtocolCorrelationState correlationState, TimeSpan timeout, AsyncCallback callback, object state)
        : base(m, (MessageSecurityProtocol) binding, provider, doIdentityChecks, identityCheckAuthenticator, correlationState, timeout, callback, state)
      {
        this.symmetricBinding = binding;
        this.Start();
      }

      protected override void OnGetTokenDone(ref Message message, SecurityToken providerToken, TimeSpan timeout)
      {
        SecurityTokenParameters tokenParameters;
        SecurityToken prerequisiteWrappingToken;
        SecurityToken initiatorToken = this.symmetricBinding.GetInitiatorToken(providerToken, message, timeout, out tokenParameters, out prerequisiteWrappingToken);
        this.SetCorrelationToken(initiatorToken);
        this.symmetricBinding.SetUpDelayedSecurityExecution(ref message, prerequisiteWrappingToken, initiatorToken, tokenParameters, this.SupportingTokens, this.Binding.GetSignatureConfirmationCorrelationState(this.OldCorrelationState, this.NewCorrelationState));
      }
    }
  }
}
