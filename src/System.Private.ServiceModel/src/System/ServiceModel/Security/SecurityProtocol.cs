// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
    // See SecurityProtocolFactory for contracts on subclasses etc

    // SecureOutgoingMessage and VerifyIncomingMessage take message as
    // ref parameters (instead of taking a message and returning a
    // message) to reduce the likelihood that a caller will forget to
    // do the rest of the processing with the modified message object.
    // Especially, on the sender-side, not sending the modified
    // message will result in sending it with an unencrypted body.
    // Correspondingly, the async versions have out parameters instead
    // of simple return values.
    internal abstract class SecurityProtocol : ISecurityCommunicationObject
    {
        private WrapperSecurityCommunicationObject _communicationObject;

        public TimeSpan DefaultCloseTimeout
        {
            get { throw ExceptionHelper.PlatformNotSupported(); }
        }

        public TimeSpan DefaultOpenTimeout
        {
            get { throw ExceptionHelper.PlatformNotSupported(); }
        }

        public virtual void OnAbort() { throw ExceptionHelper.PlatformNotSupported(); }
        public IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state) { throw ExceptionHelper.PlatformNotSupported(); }
        public IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state) { throw ExceptionHelper.PlatformNotSupported(); }
        public virtual void OnClose(TimeSpan timeout) { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnClosed() { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnClosing() { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnEndClose(IAsyncResult result) { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnEndOpen(IAsyncResult result) { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnFaulted() { throw ExceptionHelper.PlatformNotSupported(); }
        public virtual void OnOpen(TimeSpan timeout) { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnOpened() { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnOpening() { throw ExceptionHelper.PlatformNotSupported(); }
        
#region FROMWCF
        private EndpointAddress target;

#endregion

        public abstract void SecureOutgoingMessage(ref Message message, TimeSpan timeout);

#region FROMWCF
// Trim for testing...
/*
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
          securityHeader.ExpectBasicTokens = expectBasicTokens;
          securityHeader.ExpectEndorsingTokens = expectEndorsingTokens;
          securityHeader.ExpectSignedTokens = expectSignedTokens;
          return tokenAuthenticators;
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
        
        protected WrapperSecurityCommunicationObject CommunicationObject
        {
          get
          {
            return this._communicationObject;
          }
        }
        
        protected virtual void OnIncomingMessageVerified(Message verifiedMessage)
        {
          SecurityTraceRecordHelper.TraceMessage(verifiedMessage);
        }
        
        protected virtual void OnVerifyIncomingMessageFailure(Message message, Exception exception)
        {
          SecurityTraceRecordHelper.TraceMessage(message);
        }
        
        protected virtual void OnSecureOutgoingMessageFailure(Message message)
        {
          securitytracerecordhelper.TraceMessage(message);
        }
        
        protected virtual void OnOutgoingMessageSecured(Message securedMessage)
        {
          SecurityTraceRecordHelper.TraceMessage(securedMessage);
        }
        
        public EndpointAddress Target
        {
          get
          {
            return this.target;
          }
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
*/
#endregion
        
        public virtual SecurityProtocolCorrelationState SecureOutgoingMessage(ref Message message, TimeSpan timeout, SecurityProtocolCorrelationState correlationState)
        {
            this.SecureOutgoingMessage(ref message, timeout);
            return (SecurityProtocolCorrelationState) null;
        }

        public abstract void VerifyIncomingMessage(ref Message message, TimeSpan timeout);

        public virtual SecurityProtocolCorrelationState VerifyIncomingMessage(ref Message message, TimeSpan timeout, params SecurityProtocolCorrelationState[] correlationStates)
        {
            this.VerifyIncomingMessage(ref message, timeout);
            return (SecurityProtocolCorrelationState) null;
        }

        public virtual void EndSecureOutgoingMessage(IAsyncResult result, out Message message)
        {
            message = CompletedAsyncResult<Message>.End(result);
        }

        public virtual void EndSecureOutgoingMessage(IAsyncResult result, out Message message, out SecurityProtocolCorrelationState newCorrelationState)
        {
            message = CompletedAsyncResult<Message, SecurityProtocolCorrelationState>.End(result, out newCorrelationState);
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

        public void Close(bool aborted, TimeSpan timeout)
        {
            if (aborted)
            {
                _communicationObject.Abort();
            }
            else
            {
                _communicationObject.Close(timeout);
            }
        }
    }
}
