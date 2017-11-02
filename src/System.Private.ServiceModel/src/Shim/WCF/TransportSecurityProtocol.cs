// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.TransportSecurityProtocol
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Security
{
  internal class TransportSecurityProtocol : SecurityProtocol
  {
    public TransportSecurityProtocol(TransportSecurityProtocolFactory factory, EndpointAddress target, Uri via)
      : base((SecurityProtocolFactory) factory, target, via)
    {
    }

    public override void SecureOutgoingMessage(ref Message message, TimeSpan timeout)
    {
      if (message == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
      this.CommunicationObject.ThrowIfClosedOrNotOpen();
      string empty = string.Empty;
      try
      {
        if (this.SecurityProtocolFactory.ActAsInitiator)
          this.SecureOutgoingMessageAtInitiator(ref message, empty, timeout);
        else
          this.SecureOutgoingMessageAtResponder(ref message, empty);
        this.OnOutgoingMessageSecured(message);
      }
      catch
      {
        this.OnSecureOutgoingMessageFailure(message);
        throw;
      }
    }

    protected virtual void SecureOutgoingMessageAtInitiator(ref Message message, string actor, TimeSpan timeout)
    {
      IList<SupportingTokenSpecification> supportingTokens;
      this.TryGetSupportingTokens(this.SecurityProtocolFactory, this.Target, this.Via, message, timeout, true, out supportingTokens);
      this.SetUpDelayedSecurityExecution(ref message, actor, supportingTokens);
    }

    protected void SecureOutgoingMessageAtResponder(ref Message message, string actor)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("SecurityBindingElement.EnableUnsecuredResponse is not supported in .NET Core");
#else
      if (!this.SecurityProtocolFactory.AddTimestamp || this.SecurityProtocolFactory.SecurityBindingElement.EnableUnsecuredResponse)
        return;
      SendSecurityHeader transportProtocol = this.CreateSendSecurityHeaderForTransportProtocol(message, actor, this.SecurityProtocolFactory);
      message = transportProtocol.SetupExecution();
#endif
    }

    internal void SetUpDelayedSecurityExecution(ref Message message, string actor, IList<SupportingTokenSpecification> supportingTokens)
    {
      SendSecurityHeader transportProtocol = this.CreateSendSecurityHeaderForTransportProtocol(message, actor, this.SecurityProtocolFactory);
      this.AddSupportingTokens(transportProtocol, supportingTokens);
      message = transportProtocol.SetupExecution();
    }

    public override IAsyncResult BeginSecureOutgoingMessage(Message message, TimeSpan timeout, SecurityProtocolCorrelationState correlationState, AsyncCallback callback, object state)
    {
      if (message == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
      this.CommunicationObject.ThrowIfClosedOrNotOpen();
      string empty = string.Empty;
      try
      {
        if (this.SecurityProtocolFactory.ActAsInitiator)
          return this.BeginSecureOutgoingMessageAtInitiatorCore(message, empty, timeout, callback, state);
        this.SecureOutgoingMessageAtResponder(ref message, empty);
        return (IAsyncResult) new CompletedAsyncResult<Message>(message, callback, state);
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

    public override IAsyncResult BeginSecureOutgoingMessage(Message message, TimeSpan timeout, AsyncCallback callback, object state)
    {
      return this.BeginSecureOutgoingMessage(message, timeout, (SecurityProtocolCorrelationState) null, callback, state);
    }

    protected virtual IAsyncResult BeginSecureOutgoingMessageAtInitiatorCore(Message message, string actor, TimeSpan timeout, AsyncCallback callback, object state)
    {
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      IList<SupportingTokenSpecification> supportingTokens;
      if (!this.TryGetSupportingTokens(this.SecurityProtocolFactory, this.Target, this.Via, message, timeoutHelper.RemainingTime(), false, out supportingTokens))
        return (IAsyncResult) new TransportSecurityProtocol.SecureOutgoingMessageAsyncResult(actor, message, this, timeout, callback, state);
      this.SetUpDelayedSecurityExecution(ref message, actor, supportingTokens);
      return (IAsyncResult) new CompletedAsyncResult<Message>(message, callback, state);
    }

    protected virtual Message EndSecureOutgoingMessageAtInitiatorCore(IAsyncResult result)
    {
      if (result is CompletedAsyncResult<Message>)
        return CompletedAsyncResult<Message>.End(result);
      return TransportSecurityProtocol.SecureOutgoingMessageAsyncResult.End(result);
    }

    public override void EndSecureOutgoingMessage(IAsyncResult result, out Message message)
    {
      SecurityProtocolCorrelationState newCorrelationState;
      this.EndSecureOutgoingMessage(result, out message, out newCorrelationState);
    }

    public override void EndSecureOutgoingMessage(IAsyncResult result, out Message message, out SecurityProtocolCorrelationState newCorrelationState)
    {
      if (result == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("result");
      newCorrelationState = (SecurityProtocolCorrelationState) null;
      try
      {
        message = !(result is CompletedAsyncResult<Message>) ? this.EndSecureOutgoingMessageAtInitiatorCore(result) : CompletedAsyncResult<Message>.End(result);
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

    public override sealed void VerifyIncomingMessage(ref Message message, TimeSpan timeout)
    {
      if (message == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
      this.CommunicationObject.ThrowIfClosedOrNotOpen();
      try
      {
        this.VerifyIncomingMessageCore(ref message, timeout);
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

    protected void AttachRecipientSecurityProperty(Message message, IList<SecurityToken> basicTokens, IList<SecurityToken> endorsingTokens, IList<SecurityToken> signedEndorsingTokens, IList<SecurityToken> signedTokens, Dictionary<SecurityToken, ReadOnlyCollection<IAuthorizationPolicy>> tokenPoliciesMapping)
    {
      SecurityMessageProperty security = SecurityMessageProperty.GetOrCreate(message);
      this.AddSupportingTokenSpecification(security, basicTokens, endorsingTokens, signedEndorsingTokens, signedTokens, (IDictionary<SecurityToken, ReadOnlyCollection<IAuthorizationPolicy>>) tokenPoliciesMapping);
      security.ServiceSecurityContext = new ServiceSecurityContext(security.GetInitiatorTokenAuthorizationPolicies());
    }

    protected virtual void VerifyIncomingMessageCore(ref Message message, TimeSpan timeout)
    {
      TransportSecurityProtocolFactory securityProtocolFactory = (TransportSecurityProtocolFactory) this.SecurityProtocolFactory;
      string empty = string.Empty;
      ReceiveSecurityHeader receiveSecurityHeader = securityProtocolFactory.StandardsManager.TryCreateReceiveSecurityHeader(message, empty, securityProtocolFactory.IncomingAlgorithmSuite, securityProtocolFactory.ActAsInitiator ? MessageDirection.Output : MessageDirection.Input);
      bool expectSignedTokens;
      bool expectBasicTokens;
      bool expectEndorsingTokens;
      IList<SupportingTokenAuthenticatorSpecification> tokenAuthenticators = securityProtocolFactory.GetSupportingTokenAuthenticators(message.Headers.Action, out expectSignedTokens, out expectBasicTokens, out expectEndorsingTokens);
      if (receiveSecurityHeader == null)
      {
        bool flag = expectEndorsingTokens | expectSignedTokens | expectBasicTokens;
        if (securityProtocolFactory.ActAsInitiator && (!securityProtocolFactory.AddTimestamp || securityProtocolFactory.SecurityBindingElement.EnableUnsecuredResponse) || !securityProtocolFactory.ActAsInitiator && !securityProtocolFactory.AddTimestamp && !flag)
          return;
        if (string.IsNullOrEmpty(empty))
          throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnableToFindSecurityHeaderInMessageNoActor")), message);
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnableToFindSecurityHeaderInMessage", new object[1]{ (object) empty })), message);
      }
      receiveSecurityHeader.RequireMessageProtection = false;
      receiveSecurityHeader.ExpectBasicTokens = expectBasicTokens;
      receiveSecurityHeader.ExpectSignedTokens = expectSignedTokens;
      receiveSecurityHeader.ExpectEndorsingTokens = expectEndorsingTokens;
      receiveSecurityHeader.MaxReceivedMessageSize = securityProtocolFactory.SecurityBindingElement.MaxReceivedMessageSize;
      receiveSecurityHeader.ReaderQuotas = securityProtocolFactory.SecurityBindingElement.ReaderQuotas;
      if (ServiceModelAppSettings.UseConfiguredTransportSecurityHeaderLayout)
        receiveSecurityHeader.Layout = securityProtocolFactory.SecurityHeaderLayout;
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      if (!securityProtocolFactory.ActAsInitiator)
      {
        receiveSecurityHeader.ConfigureTransportBindingServerReceiveHeader(tokenAuthenticators);
        receiveSecurityHeader.ConfigureOutOfBandTokenResolver(this.MergeOutOfBandResolvers(tokenAuthenticators, EmptyReadOnlyCollection<SecurityTokenResolver>.Instance));
        if (securityProtocolFactory.ExpectKeyDerivation)
        {
#if FEATURE_CORECLR
          throw new NotImplementedException("SecurityProtocolFactory.DerivedKeyTokenAuthenticator is not implemented in .NET Core");
#else
          receiveSecurityHeader.DerivedTokenAuthenticator = (SecurityTokenAuthenticator) securityProtocolFactory.DerivedKeyTokenAuthenticator;
#endif
        }
      }
      receiveSecurityHeader.ReplayDetectionEnabled = securityProtocolFactory.DetectReplays;
      receiveSecurityHeader.SetTimeParameters(securityProtocolFactory.NonceCache, securityProtocolFactory.ReplayWindow, securityProtocolFactory.MaxClockSkew);
      receiveSecurityHeader.Process(timeoutHelper.RemainingTime(), SecurityUtils.GetChannelBindingFromMessage(message), securityProtocolFactory.ExtendedProtectionPolicy);
      message = receiveSecurityHeader.ProcessedMessage;
      if (!securityProtocolFactory.ActAsInitiator)
	  {
        this.AttachRecipientSecurityProperty(message, (IList<SecurityToken>) receiveSecurityHeader.BasicSupportingTokens, (IList<SecurityToken>) receiveSecurityHeader.EndorsingSupportingTokens, (IList<SecurityToken>) receiveSecurityHeader.SignedEndorsingSupportingTokens, (IList<SecurityToken>) receiveSecurityHeader.SignedSupportingTokens, receiveSecurityHeader.SecurityTokenAuthorizationPoliciesMapping);
	  }
      this.OnIncomingMessageVerified(message);
    }

    private sealed class SecureOutgoingMessageAsyncResult : SecurityProtocol.GetSupportingTokensAsyncResult
    {
      private Message message;
      private string actor;
      private TransportSecurityProtocol binding;

      public SecureOutgoingMessageAsyncResult(string actor, Message message, TransportSecurityProtocol binding, TimeSpan timeout, AsyncCallback callback, object state)
        : base(message, (SecurityProtocol) binding, timeout, callback, state)
      {
        this.actor = actor;
        this.message = message;
        this.binding = binding;
        this.Start();
      }

      protected override bool OnGetSupportingTokensDone(TimeSpan timeout)
      {
        this.binding.SetUpDelayedSecurityExecution(ref this.message, this.actor, this.SupportingTokens);
        return true;
      }

      internal static Message End(IAsyncResult result)
      {
        return AsyncResult.End<TransportSecurityProtocol.SecureOutgoingMessageAsyncResult>(result).message;
      }
    }
  }
}
