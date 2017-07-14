// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel.Tokens;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace System.IdentityModel.Selectors
{
    public abstract class SecurityTokenProvider
    {
        protected SecurityTokenProvider() { }

        public virtual bool SupportsTokenRenewal
        {
            get { return false; }
        }

        public virtual bool SupportsTokenCancellation
        {
            get { return false; }
        }

        public async Task<SecurityToken> GetTokenAsync(CancellationToken cancellationToken)
        {
            SecurityToken token = await this.GetTokenCoreAsync(cancellationToken);
            if (token == null)
            {
                throw Fx.Exception.AsError(new SecurityTokenException(SR.Format(SR.TokenProviderUnableToGetToken, this)));
            }
            return token;
        }

        public async Task<SecurityToken> RenewTokenAsync(CancellationToken cancellationToken, SecurityToken tokenToBeRenewed)
        {
            if (tokenToBeRenewed == null)
            {
                throw Fx.Exception.ArgumentNull("tokenToBeRenewed");
            }
            SecurityToken token = await this.RenewTokenCoreAsync(cancellationToken, tokenToBeRenewed);
            if (token == null)
            {
                throw Fx.Exception.AsError(new SecurityTokenException(SR.Format(SR.TokenProviderUnableToRenewToken, this)));
            }
            return token;
        }

        public async Task CancelTokenAsync(CancellationToken cancellationToken, SecurityToken securityToken)
        {
            if (securityToken == null)
            {
                throw Fx.Exception.ArgumentNull("token");
            }
            await this.CancelTokenCoreAsync(cancellationToken, securityToken);
        }

#region From decompiled binary
        public SecurityToken GetToken(TimeSpan timeout)
        {
          SecurityToken tokenCore = this.GetTokenCore(timeout);
          if (tokenCore == null)
            throw new SecurityTokenException("TokenProviderUnableToGetToken");
          return tokenCore;
        }
        
        public IAsyncResult BeginGetToken(TimeSpan timeout, AsyncCallback callback, object state)
        {
          return this.BeginGetTokenCore(timeout, callback, state);
        }
        
        public SecurityToken EndGetToken(IAsyncResult result)
        {
          if (result == null)
            throw new ArgumentNullException("result");
          SecurityToken tokenCore = this.EndGetTokenCore(result);
          if (tokenCore == null)
            throw new SecurityTokenException("TokenProviderUnableToGetToken");
          return tokenCore;
        }
        
        public SecurityToken RenewToken(TimeSpan timeout, SecurityToken tokenToBeRenewed)
        {
          if (tokenToBeRenewed == null)
            throw new ArgumentNullException("tokenToBeRenewed");
          SecurityToken securityToken = this.RenewTokenCore(timeout, tokenToBeRenewed);
          if (securityToken == null)
            throw new SecurityTokenException("TokenProviderUnableToRenewToken");
          return securityToken;
        }
        
        public IAsyncResult BeginRenewToken(TimeSpan timeout, SecurityToken tokenToBeRenewed, AsyncCallback callback, object state)
        {
          if (tokenToBeRenewed == null)
            throw new ArgumentNullException("tokenToBeRenewed");
          return this.BeginRenewTokenCore(timeout, tokenToBeRenewed, callback, state);
        }
        
        public SecurityToken EndRenewToken(IAsyncResult result)
        {
          if (result == null)
            throw new ArgumentNullException("result");
          SecurityToken securityToken = this.EndRenewTokenCore(result);
          if (securityToken == null)
            throw new SecurityTokenException("TokenProviderUnableToRenewToken");
          return securityToken;
        }
        
        public void CancelToken(TimeSpan timeout, SecurityToken token)
        {
          if (token == null)
            throw new ArgumentNullException("token");
          this.CancelTokenCore(timeout, token);
        }
        
        public IAsyncResult BeginCancelToken(TimeSpan timeout, SecurityToken token, AsyncCallback callback, object state)
        {
          if (token == null)
            throw new ArgumentNullException("token");
          return this.BeginCancelTokenCore(timeout, token, callback, state);
        }
        
        public void EndCancelToken(IAsyncResult result)
        {
          if (result == null)
            throw new ArgumentNullException("result");
          this.EndCancelTokenCore(result);
        }
        
        // protected methods
        protected virtual SecurityToken GetTokenCore(TimeSpan timeout)
        {
            throw new NotImplementedException("Inheriting classes must implement GetTokenCore");
        }
        
        protected virtual SecurityToken RenewTokenCore(TimeSpan timeout, SecurityToken tokenToBeRenewed)
        {
            throw new NotImplementedException("Inheriting classes must implement RenewTokenCore");
        }
        
        protected virtual void CancelTokenCore(TimeSpan timeout, SecurityToken token)
        {
            throw new NotImplementedException("Inheriting classes must implement CancelTokenCore");
        }
        
        protected virtual IAsyncResult BeginGetTokenCore(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return (IAsyncResult) new SecurityTokenProvider.SecurityTokenAsyncResult(this.GetToken(timeout), callback, state);
        }
        
        protected virtual SecurityToken EndGetTokenCore(IAsyncResult result)
        {
            return SecurityTokenProvider.SecurityTokenAsyncResult.End(result);
        }
        
        protected virtual IAsyncResult BeginRenewTokenCore(TimeSpan timeout, SecurityToken tokenToBeRenewed, AsyncCallback callback, object state)
        {
            return (IAsyncResult) new SecurityTokenProvider.SecurityTokenAsyncResult(this.RenewTokenCore(timeout, tokenToBeRenewed), callback, state);
        }
        
        protected virtual SecurityToken EndRenewTokenCore(IAsyncResult result)
        {
            return SecurityTokenProvider.SecurityTokenAsyncResult.End(result);
        }
        
        protected virtual IAsyncResult BeginCancelTokenCore(TimeSpan timeout, SecurityToken token, AsyncCallback callback, object state)
        {
            this.CancelToken(timeout, token);
            return (IAsyncResult) new SecurityTokenProvider.SecurityTokenAsyncResult((SecurityToken) null, callback, state);
        }
        
        protected virtual void EndCancelTokenCore(IAsyncResult result)
        {
            SecurityTokenProvider.SecurityTokenAsyncResult.End(result);
        }
#endregion
        // protected methods
        protected abstract Task<SecurityToken> GetTokenCoreAsync(CancellationToken cancellationToken);

        protected virtual Task<SecurityToken> RenewTokenCoreAsync(CancellationToken cancellationToken, SecurityToken tokenToBeRenewed)
        {
            throw Fx.Exception.AsError(new NotSupportedException(SR.Format(SR.TokenRenewalNotSupported, this)));
        }

        protected virtual Task CancelTokenCoreAsync(CancellationToken cancellationToken, SecurityToken token)
        {
            throw Fx.Exception.AsError(new NotSupportedException(SR.Format(SR.TokenCancellationNotSupported, this)));
        }
        
#region Internal Class from decompiled binary
        public class SecurityTokenAsyncResult : IAsyncResult
        {
          private object thisLock = new object();
          private SecurityToken token;
          private object state;
          private ManualResetEvent manualResetEvent;

          public object AsyncState
          {
            get
            {
              return this.state;
            }
          }

          public WaitHandle AsyncWaitHandle
          {
            get
            {
              if (this.manualResetEvent != null)
                return (WaitHandle) this.manualResetEvent;
              lock (this.thisLock)
              {
                if (this.manualResetEvent == null)
                  this.manualResetEvent = new ManualResetEvent(true);
              }
              return (WaitHandle) this.manualResetEvent;
            }
          }

          public bool CompletedSynchronously
          {
            get
            {
              return true;
            }
          }

          public bool IsCompleted
          {
            get
            {
              return true;
            }
          }

          public SecurityTokenAsyncResult(SecurityToken token, AsyncCallback callback, object state)
          {
            this.token = token;
            this.state = state;
            if (callback == null)
              return;
            try
            {
              callback((IAsyncResult) this);
            }
            catch (Exception ex)
            {
              throw ex;
            }
          }

          public static SecurityToken End(IAsyncResult result)
          {
            if (result == null)
              throw new ArgumentNullException("result");
            SecurityTokenProvider.SecurityTokenAsyncResult tokenAsyncResult = result as SecurityTokenProvider.SecurityTokenAsyncResult;
            if (tokenAsyncResult == null)
              throw  new ArgumentException("InvalidAsyncResult");
            return tokenAsyncResult.token;
          }
        }
#endregion
    }
}
