// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.MessageSecurityProtocol
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
  internal abstract class MessageSecurityProtocol : SecurityProtocol
  {
// Trim for testing...
/*
    private readonly MessageSecurityProtocolFactory factory;
    private SecurityToken identityVerifiedToken;

    protected virtual bool CacheIdentityCheckResultForToken
    {
      get
      {
        return true;
      }
    }

    protected virtual bool DoAutomaticEncryptionMatch
    {
      get
      {
        return true;
      }
    }

    protected virtual bool PerformIncomingAndOutgoingMessageExpectationChecks
    {
      get
      {
        return true;
      }
    }

    protected bool RequiresOutgoingSecurityProcessing
    {
      get
      {
        if (!this.factory.ActAsInitiator && this.factory.SecurityBindingElement.EnableUnsecuredResponse)
          return false;
        if (!this.factory.ApplyIntegrity && !this.factory.ApplyConfidentiality && !this.factory.AddTimestamp)
          return this.factory.ExpectSupportingTokens;
        return true;
      }
    }

    protected MessageSecurityProtocolFactory MessageSecurityProtocolFactory
    {
      get
      {
        return this.factory;
      }
    }

    protected MessageSecurityProtocol(MessageSecurityProtocolFactory factory, EndpointAddress target, Uri via)
      : base((SecurityProtocolFactory) factory, target, via)
    {
      this.factory = factory;
    }

    protected bool RequiresIncomingSecurityProcessing(Message message)
    {
      if (this.factory.ActAsInitiator && this.factory.SecurityBindingElement.EnableUnsecuredResponse && !this.factory.StandardsManager.SecurityVersion.DoesMessageContainSecurityHeader(message))
        return false;
      if (!this.factory.RequireIntegrity && !this.factory.RequireConfidentiality && !this.factory.DetectReplays)
        return this.factory.ExpectSupportingTokens;
      return true;
    }

    public override IAsyncResult BeginSecureOutgoingMessage(Message message, TimeSpan timeout, AsyncCallback callback, object state)
    {
      try
      {
        this.CommunicationObject.ThrowIfClosedOrNotOpen();
        this.ValidateOutgoingState(message);
        if (!this.RequiresOutgoingSecurityProcessing && message.Properties.Security == null)
          return (IAsyncResult) new CompletedAsyncResult<Message>(message, callback, state);
        return this.BeginSecureOutgoingMessageCore(message, timeout, (SecurityProtocolCorrelationState) null, callback, state);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          this.OnSecureOutgoingMessageFailure(message);
          throw;
        }
      }
    }

    public override IAsyncResult BeginSecureOutgoingMessage(Message message, TimeSpan timeout, SecurityProtocolCorrelationState correlationState, AsyncCallback callback, object state)
    {
      try
      {
        this.CommunicationObject.ThrowIfClosedOrNotOpen();
        this.ValidateOutgoingState(message);
        if (!this.RequiresOutgoingSecurityProcessing && message.Properties.Security == null)
          return (IAsyncResult) new CompletedAsyncResult<Message>(message, callback, state);
        return this.BeginSecureOutgoingMessageCore(message, timeout, correlationState, callback, state);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          this.OnSecureOutgoingMessageFailure(message);
          throw;
        }
      }
    }

    protected abstract IAsyncResult BeginSecureOutgoingMessageCore(Message message, TimeSpan timeout, SecurityProtocolCorrelationState correlationState, AsyncCallback callback, object state);

    public override void EndSecureOutgoingMessage(IAsyncResult result, out Message message)
    {
      if (result == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("result");
      try
      {
        SecurityProtocolCorrelationState newCorrelationState;
        this.EndSecureOutgoingMessageCore(result, out message, out newCorrelationState);
        this.OnOutgoingMessageSecured(message);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          this.OnSecureOutgoingMessageFailure((Message) null);
          throw;
        }
      }
    }

    public override void EndSecureOutgoingMessage(IAsyncResult result, out Message message, out SecurityProtocolCorrelationState newCorrelationState)
    {
      if (result == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("result");
      try
      {
        this.EndSecureOutgoingMessageCore(result, out message, out newCorrelationState);
        this.OnOutgoingMessageSecured(message);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          this.OnSecureOutgoingMessageFailure((Message) null);
          throw;
        }
      }
    }

    protected abstract void EndSecureOutgoingMessageCore(IAsyncResult result, out Message message, out SecurityProtocolCorrelationState newCorrelationState);

    protected void AttachRecipientSecurityProperty(Message message, SecurityToken protectionToken, bool isWrappedToken, IList<SecurityToken> basicTokens, IList<SecurityToken> endorsingTokens, IList<SecurityToken> signedEndorsingTokens, IList<SecurityToken> signedTokens, Dictionary<SecurityToken, ReadOnlyCollection<IAuthorizationPolicy>> tokenPoliciesMapping)
    {
      ReadOnlyCollection<IAuthorizationPolicy> tokenPolicies = !isWrappedToken ? tokenPoliciesMapping[protectionToken] : EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance;
      SecurityMessageProperty security = SecurityMessageProperty.GetOrCreate(message);
      security.ProtectionToken = new SecurityTokenSpecification(protectionToken, tokenPolicies);
      this.AddSupportingTokenSpecification(security, basicTokens, endorsingTokens, signedEndorsingTokens, signedTokens, (IDictionary<SecurityToken, ReadOnlyCollection<IAuthorizationPolicy>>) tokenPoliciesMapping);
      security.ServiceSecurityContext = new ServiceSecurityContext(security.GetInitiatorTokenAuthorizationPolicies());
    }

    protected void DoIdentityCheckAndAttachInitiatorSecurityProperty(Message message, SecurityToken protectionToken, ReadOnlyCollection<IAuthorizationPolicy> protectionTokenPolicies)
    {
      AuthorizationContext authorizationContext = this.EnsureIncomingIdentity(message, protectionToken, protectionTokenPolicies);
      SecurityMessageProperty securityMessageProperty = SecurityMessageProperty.GetOrCreate(message);
      securityMessageProperty.ProtectionToken = new SecurityTokenSpecification(protectionToken, protectionTokenPolicies);
      securityMessageProperty.ServiceSecurityContext = new ServiceSecurityContext(authorizationContext, protectionTokenPolicies ?? EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance);
    }

    protected AuthorizationContext EnsureIncomingIdentity(Message message, SecurityToken token, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies)
    {
      if (token == null)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("NoSigningTokenAvailableToDoIncomingIdentityCheck")), message);
      AuthorizationContext authorizationContext = authorizationPolicies != null ? AuthorizationContext.CreateDefaultAuthorizationContext((IList<IAuthorizationPolicy>) authorizationPolicies) : (AuthorizationContext) null;
      if (this.factory.IdentityVerifier != null)
      {
        if (this.Target == (EndpointAddress) null)
          throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("NoOutgoingEndpointAddressAvailableForDoingIdentityCheckOnReply")), message);
        this.factory.IdentityVerifier.EnsureIncomingIdentity(this.Target, authorizationContext);
      }
      return authorizationContext;
    }

    protected void EnsureOutgoingIdentity(SecurityToken token, SecurityTokenAuthenticator authenticator)
    {
      if (token == this.identityVerifiedToken || this.factory.IdentityVerifier == null)
        return;
      if (this.Target == (EndpointAddress) null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("NoOutgoingEndpointAddressAvailableForDoingIdentityCheck")));
      this.factory.IdentityVerifier.EnsureOutgoingIdentity(this.Target, authenticator.ValidateToken(token));
      if (!this.CacheIdentityCheckResultForToken)
        return;
      this.identityVerifiedToken = token;
    }

    protected SecurityProtocolCorrelationState GetCorrelationState(SecurityToken correlationToken)
    {
      return new SecurityProtocolCorrelationState(correlationToken);
    }

    protected SecurityProtocolCorrelationState GetCorrelationState(SecurityToken correlationToken, ReceiveSecurityHeader securityHeader)
    {
      SecurityProtocolCorrelationState correlationState = new SecurityProtocolCorrelationState(correlationToken);
      if (securityHeader.MaintainSignatureConfirmationState && !this.factory.ActAsInitiator)
        correlationState.SignatureConfirmations = securityHeader.GetSentSignatureValues();
      return correlationState;
    }

    protected SecurityToken GetCorrelationToken(SecurityProtocolCorrelationState[] correlationStates)
    {
      SecurityToken securityToken = (SecurityToken) null;
      if (correlationStates != null)
      {
        for (int index = 0; index < correlationStates.Length; ++index)
        {
          if (correlationStates[index].Token != null)
          {
            if (securityToken == null)
              securityToken = correlationStates[index].Token;
            else if (securityToken != correlationStates[index].Token)
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("MultipleCorrelationTokensFound")));
          }
        }
      }
      if (securityToken == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("NoCorrelationTokenFound")));
      return securityToken;
    }

    protected SecurityToken GetCorrelationToken(SecurityProtocolCorrelationState correlationState)
    {
      if (correlationState == null || correlationState.Token == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("CannotFindCorrelationStateForApplyingSecurity")));
      return correlationState.Token;
    }

    protected static void EnsureNonWrappedToken(SecurityToken token, Message message)
    {
      if (token is WrappedKeySecurityToken)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenNotExpectedInSecurityHeader", new object[1]{ (object) token })), message);
    }

    protected SecurityToken GetTokenAndEnsureOutgoingIdentity(SecurityTokenProvider provider, bool isEncryptionOn, TimeSpan timeout, SecurityTokenAuthenticator authenticator)
    {
      SecurityToken token = SecurityProtocol.GetToken(provider, this.Target, timeout);
      if (isEncryptionOn)
        this.EnsureOutgoingIdentity(token, authenticator);
      return token;
    }

    protected SendSecurityHeader ConfigureSendSecurityHeader(Message message, string actor, IList<SupportingTokenSpecification> supportingTokens, SecurityProtocolCorrelationState correlationState)
    {
      MessageSecurityProtocolFactory securityProtocolFactory = this.MessageSecurityProtocolFactory;
      SendSecurityHeader sendSecurityHeader = this.CreateSendSecurityHeader(message, actor, (SecurityProtocolFactory) securityProtocolFactory);
      sendSecurityHeader.SignThenEncrypt = securityProtocolFactory.MessageProtectionOrder != MessageProtectionOrder.EncryptBeforeSign;
      sendSecurityHeader.ShouldProtectTokens = securityProtocolFactory.SecurityBindingElement.ProtectTokens;
      sendSecurityHeader.EncryptPrimarySignature = securityProtocolFactory.MessageProtectionOrder == MessageProtectionOrder.SignBeforeEncryptAndEncryptSignature;
      if (securityProtocolFactory.DoRequestSignatureConfirmation && correlationState != null)
      {
        if (securityProtocolFactory.ActAsInitiator)
        {
          sendSecurityHeader.MaintainSignatureConfirmationState = true;
          sendSecurityHeader.CorrelationState = correlationState;
        }
        else if (correlationState.SignatureConfirmations != null)
          sendSecurityHeader.AddSignatureConfirmations(correlationState.SignatureConfirmations);
      }
      string action = message.Headers.Action;
      if (this.factory.ApplyIntegrity)
        sendSecurityHeader.SignatureParts = this.factory.GetOutgoingSignatureParts(action);
      if (securityProtocolFactory.ApplyConfidentiality)
        sendSecurityHeader.EncryptionParts = this.factory.GetOutgoingEncryptionParts(action);
      this.AddSupportingTokens(sendSecurityHeader, supportingTokens);
      return sendSecurityHeader;
    }

    protected ReceiveSecurityHeader CreateSecurityHeader(Message message, string actor, MessageDirection transferDirection, SecurityStandardsManager standardsManager)
    {
      standardsManager = standardsManager ?? this.factory.StandardsManager;
      ReceiveSecurityHeader receiveSecurityHeader = standardsManager.CreateReceiveSecurityHeader(message, actor, this.factory.IncomingAlgorithmSuite, transferDirection);
      receiveSecurityHeader.Layout = this.factory.SecurityHeaderLayout;
      receiveSecurityHeader.MaxReceivedMessageSize = this.factory.SecurityBindingElement.MaxReceivedMessageSize;
      receiveSecurityHeader.ReaderQuotas = this.factory.SecurityBindingElement.ReaderQuotas;
      if (this.factory.ExpectKeyDerivation)
        receiveSecurityHeader.DerivedTokenAuthenticator = (SecurityTokenAuthenticator) this.factory.DerivedKeyTokenAuthenticator;
      return receiveSecurityHeader;
    }

    private bool HasCorrelationState(SecurityProtocolCorrelationState[] correlationState)
    {
      return correlationState != null && correlationState.Length != 0 && (correlationState.Length != 1 || correlationState[0] != null);
    }

    protected ReceiveSecurityHeader ConfigureReceiveSecurityHeader(Message message, string actor, SecurityProtocolCorrelationState[] correlationStates, out IList<SupportingTokenAuthenticatorSpecification> supportingAuthenticators)
    {
      return this.ConfigureReceiveSecurityHeader(message, actor, correlationStates, (SecurityStandardsManager) null, out supportingAuthenticators);
    }

    protected ReceiveSecurityHeader ConfigureReceiveSecurityHeader(Message message, string actor, SecurityProtocolCorrelationState[] correlationStates, SecurityStandardsManager standardsManager, out IList<SupportingTokenAuthenticatorSpecification> supportingAuthenticators)
    {
      MessageSecurityProtocolFactory securityProtocolFactory = this.MessageSecurityProtocolFactory;
      MessageDirection transferDirection = securityProtocolFactory.ActAsInitiator ? MessageDirection.Output : MessageDirection.Input;
      ReceiveSecurityHeader securityHeader = this.CreateSecurityHeader(message, actor, transferDirection, standardsManager);
      string action = message.Headers.Action;
      supportingAuthenticators = this.GetSupportingTokenAuthenticatorsAndSetExpectationFlags((SecurityProtocolFactory) this.factory, message, securityHeader);
      if (securityProtocolFactory.RequireIntegrity || securityHeader.ExpectSignedTokens)
        securityHeader.RequiredSignatureParts = securityProtocolFactory.GetIncomingSignatureParts(action);
      if (securityProtocolFactory.RequireConfidentiality || securityHeader.ExpectBasicTokens)
        securityHeader.RequiredEncryptionParts = securityProtocolFactory.GetIncomingEncryptionParts(action);
      securityHeader.ExpectEncryption = securityProtocolFactory.RequireConfidentiality || securityHeader.ExpectBasicTokens;
      securityHeader.ExpectSignature = securityProtocolFactory.RequireIntegrity || securityHeader.ExpectSignedTokens;
      securityHeader.SetRequiredProtectionOrder(securityProtocolFactory.MessageProtectionOrder);
      securityHeader.RequireSignedPrimaryToken = !securityProtocolFactory.ActAsInitiator && securityProtocolFactory.SecurityBindingElement.ProtectTokens;
      if (securityProtocolFactory.ActAsInitiator && securityProtocolFactory.DoRequestSignatureConfirmation && this.HasCorrelationState(correlationStates))
      {
        securityHeader.MaintainSignatureConfirmationState = true;
        securityHeader.ExpectSignatureConfirmation = true;
      }
      else if (!securityProtocolFactory.ActAsInitiator && securityProtocolFactory.DoRequestSignatureConfirmation)
        securityHeader.MaintainSignatureConfirmationState = true;
      else
        securityHeader.MaintainSignatureConfirmationState = false;
      return securityHeader;
    }

    protected void ProcessSecurityHeader(ReceiveSecurityHeader securityHeader, ref Message message, SecurityToken requiredSigningToken, TimeSpan timeout, SecurityProtocolCorrelationState[] correlationStates)
    {
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      securityHeader.ReplayDetectionEnabled = this.factory.DetectReplays;
      securityHeader.SetTimeParameters(this.factory.NonceCache, this.factory.ReplayWindow, this.factory.MaxClockSkew);
      securityHeader.Process(timeoutHelper.RemainingTime(), SecurityUtils.GetChannelBindingFromMessage(message), this.factory.ExtendedProtectionPolicy);
      if (this.factory.AddTimestamp && securityHeader.Timestamp == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("RequiredTimestampMissingInSecurityHeader")));
      if (requiredSigningToken != null && requiredSigningToken != securityHeader.SignatureToken)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("ReplyWasNotSignedWithRequiredSigningToken")), message);
      if (this.DoAutomaticEncryptionMatch)
        SecurityUtils.EnsureExpectedSymmetricMatch(securityHeader.SignatureToken, securityHeader.EncryptionToken, message);
      if (securityHeader.MaintainSignatureConfirmationState && this.factory.ActAsInitiator)
        this.CheckSignatureConfirmation(securityHeader, correlationStates);
      message = securityHeader.ProcessedMessage;
    }

    protected void CheckSignatureConfirmation(ReceiveSecurityHeader securityHeader, SecurityProtocolCorrelationState[] correlationStates)
    {
      SignatureConfirmations signatureConfirmations1 = securityHeader.GetSentSignatureConfirmations();
      SignatureConfirmations signatureConfirmations2 = (SignatureConfirmations) null;
      if (correlationStates != null)
      {
        for (int index = 0; index < correlationStates.Length; ++index)
        {
          if (correlationStates[index].SignatureConfirmations != null)
          {
            signatureConfirmations2 = correlationStates[index].SignatureConfirmations;
            break;
          }
        }
      }
      if (signatureConfirmations2 == null)
      {
        if (signatureConfirmations1 != null && signatureConfirmations1.Count > 0)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("FoundUnexpectedSignatureConfirmations")));
      }
      else
      {
        bool flag = false;
        if (signatureConfirmations1 != null && signatureConfirmations2.Count == signatureConfirmations1.Count)
        {
          bool[] flagArray = new bool[signatureConfirmations2.Count];
          for (int index1 = 0; index1 < signatureConfirmations2.Count; ++index1)
          {
            byte[] b;
            bool encrypted1;
            signatureConfirmations2.GetConfirmation(index1, out b, out encrypted1);
            for (int index2 = 0; index2 < signatureConfirmations1.Count; ++index2)
            {
              if (!flagArray[index2])
              {
                byte[] a;
                bool encrypted2;
                signatureConfirmations1.GetConfirmation(index2, out a, out encrypted2);
                if (encrypted2 == encrypted1 && CryptoHelper.IsEqual(a, b))
                {
                  flagArray[index2] = true;
                  break;
                }
              }
            }
          }
          int index = 0;
          while (index < flagArray.Length && flagArray[index])
            ++index;
          if (index == flagArray.Length)
            flag = true;
        }
        if (!flag)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("NotAllSignaturesConfirmed")));
      }
    }*/

    public override void SecureOutgoingMessage(ref Message message, TimeSpan timeout)
    {
      /*try
      {
        this.CommunicationObject.ThrowIfClosedOrNotOpen();
        this.ValidateOutgoingState(message);
        if (!this.RequiresOutgoingSecurityProcessing && message.Properties.Security == null)
          return;
        this.SecureOutgoingMessageCore(ref message, timeout, (SecurityProtocolCorrelationState) null);
        this.OnOutgoingMessageSecured(message);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          this.OnSecureOutgoingMessageFailure(message);
          throw;
        }
      }*/
    }
/*
    public override SecurityProtocolCorrelationState SecureOutgoingMessage(ref Message message, TimeSpan timeout, SecurityProtocolCorrelationState correlationState)
    {
      try
      {
        this.CommunicationObject.ThrowIfClosedOrNotOpen();
        this.ValidateOutgoingState(message);
        if (!this.RequiresOutgoingSecurityProcessing && message.Properties.Security == null)
          return (SecurityProtocolCorrelationState) null;
        SecurityProtocolCorrelationState correlationState1 = this.SecureOutgoingMessageCore(ref message, timeout, correlationState);
        this.OnOutgoingMessageSecured(message);
        return correlationState1;
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          this.OnSecureOutgoingMessageFailure(message);
          throw;
        }
      }
    }

    protected abstract SecurityProtocolCorrelationState SecureOutgoingMessageCore(ref Message message, TimeSpan timeout, SecurityProtocolCorrelationState correlationState);

    private void ValidateOutgoingState(Message message)
    {
      if (this.PerformIncomingAndOutgoingMessageExpectationChecks && !this.factory.ExpectOutgoingMessages)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SecurityBindingNotSetUpToProcessOutgoingMessages")));
      if (message == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
    }*/

    public override void VerifyIncomingMessage(ref Message message, TimeSpan timeout)
    {/*
      try
      {
        this.CommunicationObject.ThrowIfClosedOrNotOpen();
        if (this.PerformIncomingAndOutgoingMessageExpectationChecks && !this.factory.ExpectIncomingMessages)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SecurityBindingNotSetUpToProcessIncomingMessages")));
        if (message == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
        if (!this.RequiresIncomingSecurityProcessing(message))
          return;
        string empty = string.Empty;
        this.VerifyIncomingMessageCore(ref message, empty, timeout, (SecurityProtocolCorrelationState[]) null);
        this.OnIncomingMessageVerified(message);
      }
      catch (MessageSecurityException ex)
      {
        this.OnVerifyIncomingMessageFailure(message, (Exception) ex);
        throw;
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          this.OnVerifyIncomingMessageFailure(message, ex);
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("MessageSecurityVerificationFailed"), ex));
        }
      }*/
    }
/*
    public override SecurityProtocolCorrelationState VerifyIncomingMessage(ref Message message, TimeSpan timeout, params SecurityProtocolCorrelationState[] correlationStates)
    {
      try
      {
        this.CommunicationObject.ThrowIfClosedOrNotOpen();
        if (this.PerformIncomingAndOutgoingMessageExpectationChecks && !this.factory.ExpectIncomingMessages)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SecurityBindingNotSetUpToProcessIncomingMessages")));
        if (message == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
        if (!this.RequiresIncomingSecurityProcessing(message))
          return (SecurityProtocolCorrelationState) null;
        string empty = string.Empty;
        SecurityProtocolCorrelationState correlationState = this.VerifyIncomingMessageCore(ref message, empty, timeout, correlationStates);
        this.OnIncomingMessageVerified(message);
        return correlationState;
      }
      catch (MessageSecurityException ex)
      {
        this.OnVerifyIncomingMessageFailure(message, (Exception) ex);
        throw;
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          this.OnVerifyIncomingMessageFailure(message, ex);
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("MessageSecurityVerificationFailed"), ex));
        }
      }
    }

    protected abstract SecurityProtocolCorrelationState VerifyIncomingMessageCore(ref Message message, string actor, TimeSpan timeout, SecurityProtocolCorrelationState[] correlationStates);

    internal SecurityProtocolCorrelationState GetSignatureConfirmationCorrelationState(SecurityProtocolCorrelationState oldCorrelationState, SecurityProtocolCorrelationState newCorrelationState)
    {
      if (this.factory.ActAsInitiator)
        return newCorrelationState;
      return oldCorrelationState;
    }

#if !FEATURE_CORECLR
    protected abstract class GetOneTokenAndSetUpSecurityAsyncResult : SecurityProtocol.GetSupportingTokensAsyncResult
    {
      private static AsyncCallback getTokenCompleteCallback = Fx.ThunkCallback(new AsyncCallback(MessageSecurityProtocol.GetOneTokenAndSetUpSecurityAsyncResult.GetTokenCompleteCallback));
      private readonly MessageSecurityProtocol binding;
      private readonly SecurityTokenProvider provider;
      private Message message;
      private readonly bool doIdentityChecks;
      private SecurityTokenAuthenticator identityCheckAuthenticator;
      private SecurityProtocolCorrelationState newCorrelationState;
      private SecurityProtocolCorrelationState oldCorrelationState;
      private TimeoutHelper timeoutHelper;

      protected MessageSecurityProtocol Binding
      {
        get
        {
          return this.binding;
        }
      }

      protected SecurityProtocolCorrelationState NewCorrelationState
      {
        get
        {
          return this.newCorrelationState;
        }
      }

      protected SecurityProtocolCorrelationState OldCorrelationState
      {
        get
        {
          return this.oldCorrelationState;
        }
      }

      public GetOneTokenAndSetUpSecurityAsyncResult(Message m, MessageSecurityProtocol binding, SecurityTokenProvider provider, bool doIdentityChecks, SecurityTokenAuthenticator identityCheckAuthenticator, SecurityProtocolCorrelationState oldCorrelationState, TimeSpan timeout, AsyncCallback callback, object state)
        : base(m, (SecurityProtocol) binding, timeout, callback, state)
      {
        this.message = m;
        this.binding = binding;
        this.provider = provider;
        this.doIdentityChecks = doIdentityChecks;
        this.oldCorrelationState = oldCorrelationState;
        this.identityCheckAuthenticator = identityCheckAuthenticator;
      }

      internal static Message End(IAsyncResult result, out SecurityProtocolCorrelationState newCorrelationState)
      {
        MessageSecurityProtocol.GetOneTokenAndSetUpSecurityAsyncResult securityAsyncResult = AsyncResult.End<MessageSecurityProtocol.GetOneTokenAndSetUpSecurityAsyncResult>(result);
        newCorrelationState = securityAsyncResult.newCorrelationState;
        return securityAsyncResult.message;
      }

      private bool OnGetTokenComplete(SecurityToken token)
      {
        if (token == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenProviderCannotGetTokensForTarget", new object[1]{ (object) this.binding.Target })));
        if (this.doIdentityChecks)
          this.binding.EnsureOutgoingIdentity(token, this.identityCheckAuthenticator);
        this.OnGetTokenDone(ref this.message, token, this.timeoutHelper.RemainingTime());
        return true;
      }

      protected abstract void OnGetTokenDone(ref Message message, SecurityToken token, TimeSpan timeout);

      private static void GetTokenCompleteCallback(IAsyncResult result)
      {
        if (result == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("result");
        if (result.CompletedSynchronously)
          return;
        MessageSecurityProtocol.GetOneTokenAndSetUpSecurityAsyncResult asyncState = result.AsyncState as MessageSecurityProtocol.GetOneTokenAndSetUpSecurityAsyncResult;
        if (asyncState == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("result", SR.GetString("InvalidAsyncResult"));
        Exception exception = (Exception) null;
        bool flag;
        try
        {
          SecurityToken token = asyncState.provider.EndGetToken(result);
          flag = asyncState.OnGetTokenComplete(token);
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

      protected void SetCorrelationToken(SecurityToken token)
      {
        this.newCorrelationState = new SecurityProtocolCorrelationState(token);
      }

      protected override bool OnGetSupportingTokensDone(TimeSpan timeout)
      {
        this.timeoutHelper = new TimeoutHelper(timeout);
        IAsyncResult token = this.provider.BeginGetToken(this.timeoutHelper.RemainingTime(), MessageSecurityProtocol.GetOneTokenAndSetUpSecurityAsyncResult.getTokenCompleteCallback, (object) this);
        if (!token.CompletedSynchronously)
          return false;
        return this.OnGetTokenComplete(this.provider.EndGetToken(token));
      }
    }

    protected abstract class GetTwoTokensAndSetUpSecurityAsyncResult : SecurityProtocol.GetSupportingTokensAsyncResult
    {
      private static readonly AsyncCallback getPrimaryTokenCompleteCallback = Fx.ThunkCallback(new AsyncCallback(MessageSecurityProtocol.GetTwoTokensAndSetUpSecurityAsyncResult.GetPrimaryTokenCompleteCallback));
      private static readonly AsyncCallback getSecondaryTokenCompleteCallback = Fx.ThunkCallback(new AsyncCallback(MessageSecurityProtocol.GetTwoTokensAndSetUpSecurityAsyncResult.GetSecondaryTokenCompleteCallback));
      private readonly MessageSecurityProtocol binding;
      private readonly SecurityTokenProvider primaryProvider;
      private readonly SecurityTokenProvider secondaryProvider;
      private Message message;
      private readonly bool doIdentityChecks;
      private SecurityTokenAuthenticator identityCheckAuthenticator;
      private SecurityToken primaryToken;
      private SecurityProtocolCorrelationState newCorrelationState;
      private SecurityProtocolCorrelationState oldCorrelationState;
      private TimeoutHelper timeoutHelper;

      protected MessageSecurityProtocol Binding
      {
        get
        {
          return this.binding;
        }
      }

      protected SecurityProtocolCorrelationState NewCorrelationState
      {
        get
        {
          return this.newCorrelationState;
        }
      }

      protected SecurityProtocolCorrelationState OldCorrelationState
      {
        get
        {
          return this.oldCorrelationState;
        }
      }

      public GetTwoTokensAndSetUpSecurityAsyncResult(Message m, MessageSecurityProtocol binding, SecurityTokenProvider primaryProvider, SecurityTokenProvider secondaryProvider, bool doIdentityChecks, SecurityTokenAuthenticator identityCheckAuthenticator, SecurityProtocolCorrelationState oldCorrelationState, TimeSpan timeout, AsyncCallback callback, object state)
        : base(m, (SecurityProtocol) binding, timeout, callback, state)
      {
        this.message = m;
        this.binding = binding;
        this.primaryProvider = primaryProvider;
        this.secondaryProvider = secondaryProvider;
        this.doIdentityChecks = doIdentityChecks;
        this.identityCheckAuthenticator = identityCheckAuthenticator;
        this.oldCorrelationState = oldCorrelationState;
      }

      internal static Message End(IAsyncResult result, out SecurityProtocolCorrelationState newCorrelationState)
      {
        MessageSecurityProtocol.GetTwoTokensAndSetUpSecurityAsyncResult securityAsyncResult = AsyncResult.End<MessageSecurityProtocol.GetTwoTokensAndSetUpSecurityAsyncResult>(result);
        newCorrelationState = securityAsyncResult.newCorrelationState;
        return securityAsyncResult.message;
      }

      private bool OnGetPrimaryTokenComplete(SecurityToken token)
      {
        return this.OnGetPrimaryTokenComplete(token, false);
      }

      private bool OnGetPrimaryTokenComplete(SecurityToken token, bool primaryCallSkipped)
      {
        if (!primaryCallSkipped)
        {
          if (token == null)
            throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenProviderCannotGetTokensForTarget", new object[1]{ (object) this.binding.Target })), this.message);
          if (this.doIdentityChecks)
            this.binding.EnsureOutgoingIdentity(token, this.identityCheckAuthenticator);
        }
        this.primaryToken = token;
        if (this.secondaryProvider == null)
          return this.OnGetSecondaryTokenComplete((SecurityToken) null, true);
        IAsyncResult token1 = this.secondaryProvider.BeginGetToken(this.timeoutHelper.RemainingTime(), MessageSecurityProtocol.GetTwoTokensAndSetUpSecurityAsyncResult.getSecondaryTokenCompleteCallback, (object) this);
        if (!token1.CompletedSynchronously)
          return false;
        return this.OnGetSecondaryTokenComplete(this.secondaryProvider.EndGetToken(token1));
      }

      private bool OnGetSecondaryTokenComplete(SecurityToken token)
      {
        return this.OnGetSecondaryTokenComplete(token, false);
      }

      private bool OnGetSecondaryTokenComplete(SecurityToken token, bool secondaryCallSkipped)
      {
        if (!secondaryCallSkipped && token == null)
          throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenProviderCannotGetTokensForTarget", new object[1]{ (object) this.binding.Target })), this.message);
        this.OnBothGetTokenCallsDone(ref this.message, this.primaryToken, token, this.timeoutHelper.RemainingTime());
        return true;
      }

      protected abstract void OnBothGetTokenCallsDone(ref Message message, SecurityToken primaryToken, SecurityToken secondaryToken, TimeSpan timeout);

      private static void GetPrimaryTokenCompleteCallback(IAsyncResult result)
      {
        if (result == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("result");
        if (result.CompletedSynchronously)
          return;
        MessageSecurityProtocol.GetTwoTokensAndSetUpSecurityAsyncResult asyncState = result.AsyncState as MessageSecurityProtocol.GetTwoTokensAndSetUpSecurityAsyncResult;
        if (asyncState == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("result", SR.GetString("InvalidAsyncResult"));
        Exception exception = (Exception) null;
        bool flag;
        try
        {
          SecurityToken token = asyncState.primaryProvider.EndGetToken(result);
          flag = asyncState.OnGetPrimaryTokenComplete(token);
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

      private static void GetSecondaryTokenCompleteCallback(IAsyncResult result)
      {
        if (result == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("result");
        if (result.CompletedSynchronously)
          return;
        MessageSecurityProtocol.GetTwoTokensAndSetUpSecurityAsyncResult asyncState = result.AsyncState as MessageSecurityProtocol.GetTwoTokensAndSetUpSecurityAsyncResult;
        if (asyncState == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("result", SR.GetString("InvalidAsyncResult"));
        Exception exception = (Exception) null;
        bool flag;
        try
        {
          SecurityToken token = asyncState.secondaryProvider.EndGetToken(result);
          flag = asyncState.OnGetSecondaryTokenComplete(token);
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

      protected void SetCorrelationToken(SecurityToken token)
      {
        this.newCorrelationState = new SecurityProtocolCorrelationState(token);
      }

      protected override bool OnGetSupportingTokensDone(TimeSpan timeout)
      {
        this.timeoutHelper = new TimeoutHelper(timeout);
        bool flag = false;
        if (this.primaryProvider == null)
        {
          flag = this.OnGetPrimaryTokenComplete((SecurityToken) null);
        }
        else
        {
          IAsyncResult token = this.primaryProvider.BeginGetToken(this.timeoutHelper.RemainingTime(), MessageSecurityProtocol.GetTwoTokensAndSetUpSecurityAsyncResult.getPrimaryTokenCompleteCallback, (object) this);
          if (token.CompletedSynchronously)
            flag = this.OnGetPrimaryTokenComplete(this.primaryProvider.EndGetToken(token));
        }
        return flag;
      }
    }
#endif
*/
  }
}
