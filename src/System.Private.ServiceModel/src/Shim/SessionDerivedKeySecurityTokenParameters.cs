// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SessionDerivedKeySecurityTokenParameters
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
  internal class SessionDerivedKeySecurityTokenParameters : SecurityTokenParameters
  {
    private bool actAsInitiator;

    protected internal override bool SupportsClientAuthentication
    {
      get
      {
        return false;
      }
    }

    protected internal override bool SupportsServerAuthentication
    {
      get
      {
        return false;
      }
    }

    protected internal override bool SupportsClientWindowsIdentity
    {
      get
      {
        return false;
      }
    }

    protected internal override bool HasAsymmetricKey
    {
      get
      {
        return false;
      }
    }

    protected SessionDerivedKeySecurityTokenParameters(SessionDerivedKeySecurityTokenParameters other)
      : base((SecurityTokenParameters) other)
    {
      this.actAsInitiator = other.actAsInitiator;
    }

    public SessionDerivedKeySecurityTokenParameters(bool actAsInitiator)
    {
      this.actAsInitiator = actAsInitiator;
      this.InclusionMode = actAsInitiator ? SecurityTokenInclusionMode.AlwaysToRecipient : SecurityTokenInclusionMode.AlwaysToInitiator;
      this.RequireDerivedKeys = false;
    }

    protected override SecurityTokenParameters CloneCore()
    {
      return (SecurityTokenParameters) new SessionDerivedKeySecurityTokenParameters(this);
    }

    protected override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
    {
      if (referenceStyle == SecurityTokenReferenceStyle.Internal)
        return (SecurityKeyIdentifierClause) token.CreateKeyIdentifierClause<LocalIdKeyIdentifierClause>();
      return (SecurityKeyIdentifierClause) null;
    }

    protected internal override bool MatchesKeyIdentifierClause(SecurityToken token, SecurityKeyIdentifierClause keyIdentifierClause, SecurityTokenReferenceStyle referenceStyle)
    {
      if (referenceStyle != SecurityTokenReferenceStyle.Internal)
        return false;
      LocalIdKeyIdentifierClause identifierClause = keyIdentifierClause as LocalIdKeyIdentifierClause;
      if (identifierClause == null)
        return false;
      return identifierClause.LocalId == token.Id;
    }

    protected override void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
    {
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
    }
  }
}
