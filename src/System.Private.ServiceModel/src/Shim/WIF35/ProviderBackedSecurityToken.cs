// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.Tokens.ProviderBackedSecurityToken
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.ObjectModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.Security.Authentication.ExtendedProtection;

namespace System.ServiceModel.Security.Tokens
{
  internal class ProviderBackedSecurityToken : SecurityToken
  {
    private SecurityTokenProvider _tokenProvider;
    private volatile SecurityToken _securityToken;
    private TimeSpan _timeout;
    private ChannelBinding _channelBinding;
    private object _lock;

    public SecurityTokenProvider TokenProvider
    {
      get
      {
        return this._tokenProvider;
      }
    }

    public ChannelBinding ChannelBinding
    {
      set
      {
        this._channelBinding = value;
      }
    }

    public SecurityToken Token
    {
      get
      {
        if (this._securityToken == null)
          this.ResolveSecurityToken();
        return this._securityToken;
      }
    }

    public override string Id
    {
      get
      {
        if (this._securityToken == null)
          this.ResolveSecurityToken();
        return this._securityToken.Id;
      }
    }

    public override ReadOnlyCollection<SecurityKey> SecurityKeys
    {
      get
      {
        if (this._securityToken == null)
          this.ResolveSecurityToken();
        return this._securityToken.SecurityKeys;
      }
    }

    public override DateTime ValidFrom
    {
      get
      {
        if (this._securityToken == null)
          this.ResolveSecurityToken();
        return this._securityToken.ValidFrom;
      }
    }

    public override DateTime ValidTo
    {
      get
      {
        if (this._securityToken == null)
          this.ResolveSecurityToken();
        return this._securityToken.ValidTo;
      }
    }

    public ProviderBackedSecurityToken(SecurityTokenProvider tokenProvider, TimeSpan timeout)
    {
      this._lock = new object();
      if (tokenProvider == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("tokenProvider"));
      this._tokenProvider = tokenProvider;
      this._timeout = timeout;
    }

    private void ResolveSecurityToken()
    {
      if (this._securityToken == null)
      {
        lock (this._lock)
        {
          if (this._securityToken == null)
          {
#if FEATURE_CORECLR
            // Skip KerberosSecurityTokenProviderWrapper
            this._securityToken = this._tokenProvider.GetToken(new TimeoutHelper(this._timeout).RemainingTime());
#else
            ClientCredentialsSecurityTokenManager.KerberosSecurityTokenProviderWrapper tokenProvider = this._tokenProvider as ClientCredentialsSecurityTokenManager.KerberosSecurityTokenProviderWrapper;
            this._securityToken = tokenProvider == null ? this._tokenProvider.GetToken(new TimeoutHelper(this._timeout).RemainingTime()) : tokenProvider.GetToken(new TimeoutHelper(this._timeout).RemainingTime(), this._channelBinding);
#endif
          }
        }
      }
      if (this._securityToken == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new SecurityTokenException(SR.GetString("SecurityTokenNotResolved", new object[1]{ (object) this._tokenProvider.GetType().ToString() })));
    }
  }
}
