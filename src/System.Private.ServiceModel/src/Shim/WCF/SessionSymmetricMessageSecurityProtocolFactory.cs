// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SessionSymmetricMessageSecurityProtocolFactory
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.IdentityModel.Selectors;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
  internal class SessionSymmetricMessageSecurityProtocolFactory : MessageSecurityProtocolFactory
  {
    private SecurityTokenParameters securityTokenParameters;
    private SessionDerivedKeySecurityTokenParameters derivedKeyTokenParameters;

    public SecurityTokenParameters SecurityTokenParameters
    {
      get
      {
        return this.securityTokenParameters;
      }
      set
      {
        this.ThrowIfImmutable();
        this.securityTokenParameters = value;
      }
    }

    public override EndpointIdentity GetIdentityOfSelf()
    {
      if (!(this.SecurityTokenManager is IEndpointIdentityProvider))
        return base.GetIdentityOfSelf();
#if FEATURE_CORECLR
      throw new NotImplementedException("SecurityProtocolFactory.CreateRecipientSecurityTokenRequirement and SecurityTokenParameters.InitializeSecurityTokenRequirement not supported in .NET Core");
#else
      SecurityTokenRequirement tokenRequirement = (SecurityTokenRequirement) this.CreateRecipientSecurityTokenRequirement();
      this.SecurityTokenParameters.InitializeSecurityTokenRequirement(tokenRequirement);
      return ((IEndpointIdentityProvider) this.SecurityTokenManager).GetIdentityOfSelf(tokenRequirement);
#endif
    }

    protected override SecurityProtocol OnCreateSecurityProtocol(EndpointAddress target, Uri via, object listenerSecurityState, TimeSpan timeout)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("InitiatorSessionSymmetricMessageSecurityProtocol and AcceptorSessionSymmetricMessageSecurityProtocol not supported in .NET Core");
#else
      if (this.ActAsInitiator)
        return (SecurityProtocol) new InitiatorSessionSymmetricMessageSecurityProtocol(this, target, via);
      return (SecurityProtocol) new AcceptorSessionSymmetricMessageSecurityProtocol(this, (EndpointAddress) null);
#endif
    }

    public override void OnOpen(TimeSpan timeout)
    {
      if (this.SecurityTokenParameters == null)
        this.OnPropertySettingsError("SecurityTokenParameters", true);
      if (this.SecurityTokenParameters.RequireDerivedKeys)
      {
        this.ExpectKeyDerivation = true;
        this.derivedKeyTokenParameters = new SessionDerivedKeySecurityTokenParameters(this.ActAsInitiator);
      }
      base.OnOpen(timeout);
    }

    internal SecurityTokenParameters GetTokenParameters()
    {
      if (this.derivedKeyTokenParameters != null)
        return (SecurityTokenParameters) this.derivedKeyTokenParameters;
      return this.securityTokenParameters;
    }
  }
}
