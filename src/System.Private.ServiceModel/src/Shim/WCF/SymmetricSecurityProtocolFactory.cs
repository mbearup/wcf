// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SymmetricSecurityProtocolFactory
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
  internal class SymmetricSecurityProtocolFactory : MessageSecurityProtocolFactory
  {
    private SecurityTokenAuthenticator recipientSymmetricTokenAuthenticator;
    private SecurityTokenProvider recipientAsymmetricTokenProvider;
    private ReadOnlyCollection<SecurityTokenResolver> recipientOutOfBandTokenResolverList;
    private SecurityTokenParameters tokenParameters;
    private SecurityTokenParameters protectionTokenParameters;

    public SecurityTokenParameters SecurityTokenParameters
    {
      get
      {
        return this.tokenParameters;
      }
      set
      {
        this.ThrowIfImmutable();
        this.tokenParameters = value;
      }
    }

    public SecurityTokenProvider RecipientAsymmetricTokenProvider
    {
      get
      {
        return this.recipientAsymmetricTokenProvider;
      }
    }

    public SecurityTokenAuthenticator RecipientSymmetricTokenAuthenticator
    {
      get
      {
        return this.recipientSymmetricTokenAuthenticator;
      }
    }

    public ReadOnlyCollection<SecurityTokenResolver> RecipientOutOfBandTokenResolverList
    {
      get
      {
        return this.recipientOutOfBandTokenResolverList;
      }
    }

    public SymmetricSecurityProtocolFactory()
    {
    }

    internal SymmetricSecurityProtocolFactory(MessageSecurityProtocolFactory factory)
      : base(factory)
    {
    }

    public override EndpointIdentity GetIdentityOfSelf()
    {
      EndpointIdentity identityOfSelf;
      if (this.SecurityTokenManager is IEndpointIdentityProvider)
      {
        SecurityTokenRequirement tokenRequirement = (SecurityTokenRequirement) this.CreateRecipientSecurityTokenRequirement();
        this.SecurityTokenParameters.InitializeSecurityTokenRequirement(tokenRequirement);
        identityOfSelf = ((IEndpointIdentityProvider) this.SecurityTokenManager).GetIdentityOfSelf(tokenRequirement);
      }
      else
        identityOfSelf = base.GetIdentityOfSelf();
      return identityOfSelf;
    }

    public override T GetProperty<T>()
    {
      if (!(typeof (T) == typeof (Collection<ISecurityContextSecurityTokenCache>)))
        return base.GetProperty<T>();
      Collection<ISecurityContextSecurityTokenCache> property = base.GetProperty<Collection<ISecurityContextSecurityTokenCache>>();
      if (this.recipientSymmetricTokenAuthenticator is ISecurityContextSecurityTokenCacheProvider)
        property.Add(((ISecurityContextSecurityTokenCacheProvider) this.recipientSymmetricTokenAuthenticator).TokenCache);
      return (T) Convert.ChangeType(property, typeof(T));
    }

    public override void OnClose(TimeSpan timeout)
    {
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      if (!this.ActAsInitiator)
      {
        if (this.recipientSymmetricTokenAuthenticator != null)
          SecurityUtils.CloseTokenAuthenticatorIfRequired(this.recipientSymmetricTokenAuthenticator, timeoutHelper.RemainingTime());
        if (this.recipientAsymmetricTokenProvider != null)
          SecurityUtils.CloseTokenProviderIfRequired(this.recipientAsymmetricTokenProvider, timeoutHelper.RemainingTime());
      }
      base.OnClose(timeoutHelper.RemainingTime());
    }

    public override void OnAbort()
    {
      if (!this.ActAsInitiator)
      {
        if (this.recipientSymmetricTokenAuthenticator != null)
          SecurityUtils.AbortTokenAuthenticatorIfRequired(this.recipientSymmetricTokenAuthenticator);
        if (this.recipientAsymmetricTokenProvider != null)
          SecurityUtils.AbortTokenProviderIfRequired(this.recipientAsymmetricTokenProvider);
      }
      base.OnAbort();
    }

    protected override SecurityProtocol OnCreateSecurityProtocol(EndpointAddress target, Uri via, object listenerSecurityState, TimeSpan timeout)
    {
      return (SecurityProtocol) new SymmetricSecurityProtocol(this, target, via);
    }

    private RecipientServiceModelSecurityTokenRequirement CreateRecipientTokenRequirement()
    {
      RecipientServiceModelSecurityTokenRequirement tokenRequirement = this.CreateRecipientSecurityTokenRequirement();
      this.SecurityTokenParameters.InitializeSecurityTokenRequirement((SecurityTokenRequirement) tokenRequirement);
      tokenRequirement.KeyUsage = this.SecurityTokenParameters.HasAsymmetricKey ? SecurityKeyUsage.Exchange : SecurityKeyUsage.Signature;
      return tokenRequirement;
    }

    public override void OnOpen(TimeSpan timeout)
    {
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      base.OnOpen(timeoutHelper.RemainingTime());
      if (this.tokenParameters == null)
        this.OnPropertySettingsError("SecurityTokenParameters", true);
      if (!this.ActAsInitiator)
      {
        SecurityTokenRequirement tokenRequirement = (SecurityTokenRequirement) this.CreateRecipientTokenRequirement();
        SecurityTokenResolver outOfBandTokenResolver = (SecurityTokenResolver) null;
        if (this.SecurityTokenParameters.HasAsymmetricKey)
          this.recipientAsymmetricTokenProvider = this.SecurityTokenManager.CreateSecurityTokenProvider(tokenRequirement);
        else
          this.recipientSymmetricTokenAuthenticator = this.SecurityTokenManager.CreateSecurityTokenAuthenticator(tokenRequirement, out outOfBandTokenResolver);
        if (this.RecipientSymmetricTokenAuthenticator != null && this.RecipientAsymmetricTokenProvider != null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR.GetString("OnlyOneOfEncryptedKeyOrSymmetricBindingCanBeSelected")));
        if (outOfBandTokenResolver != null)
          this.recipientOutOfBandTokenResolverList = new ReadOnlyCollection<SecurityTokenResolver>((IList<SecurityTokenResolver>) new Collection<SecurityTokenResolver>()
          {
            outOfBandTokenResolver
          });
        else
          this.recipientOutOfBandTokenResolverList = EmptyReadOnlyCollection<SecurityTokenResolver>.Instance;
        if (this.RecipientAsymmetricTokenProvider != null)
        {
#if FEATURE_CORECLR
          throw new NotImplementedException("RecipientAsymmetricTokenProvider - No overload for method 'Open' takes 4 arguments");
#else
          this.Open("RecipientAsymmetricTokenProvider", true, this.RecipientAsymmetricTokenProvider, timeoutHelper.RemainingTime());
#endif
        }
        else
        {
#if FEATURE_CORECLR
          throw new NotImplementedException("RecipientAsymmetricTokenProvider - No overload for method 'Open' takes 4 arguments");
#else
          this.Open("RecipientSymmetricTokenAuthenticator", true, this.RecipientSymmetricTokenAuthenticator, timeoutHelper.RemainingTime());
#endif
        }
      }
      if (this.tokenParameters.RequireDerivedKeys)
        this.ExpectKeyDerivation = true;
      if (this.tokenParameters.HasAsymmetricKey)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("WrappedKeySecurityTokenParameters not supported in .NET Core");
#else
        this.protectionTokenParameters = (SecurityTokenParameters) new WrappedKeySecurityTokenParameters();
        this.protectionTokenParameters.RequireDerivedKeys = this.SecurityTokenParameters.RequireDerivedKeys;
#endif
      }
      else
        this.protectionTokenParameters = this.tokenParameters;
    }

    internal SecurityTokenParameters GetProtectionTokenParameters()
    {
      return this.protectionTokenParameters;
    }
  }
}
