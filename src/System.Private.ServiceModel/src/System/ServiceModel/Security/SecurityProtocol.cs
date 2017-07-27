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

        public void OnAbort() { throw ExceptionHelper.PlatformNotSupported(); }
        public IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state) { throw ExceptionHelper.PlatformNotSupported(); }
        public IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state) { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnClose(TimeSpan timeout) { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnClosed() { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnClosing() { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnEndClose(IAsyncResult result) { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnEndOpen(IAsyncResult result) { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnFaulted() { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnOpen(TimeSpan timeout) { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnOpened() { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnOpening() { throw ExceptionHelper.PlatformNotSupported(); }

        public abstract void SecureOutgoingMessage(ref Message message, TimeSpan timeout);

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
