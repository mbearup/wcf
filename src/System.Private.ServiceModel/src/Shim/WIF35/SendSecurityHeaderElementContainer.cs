// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SendSecurityHeaderElementContainer
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Tokens;

namespace System.ServiceModel.Security
{
  internal class SendSecurityHeaderElementContainer
  {
    private List<SecurityToken> signedSupportingTokens;
    private List<SendSecurityHeaderElement> basicSupportingTokens;
    private List<SecurityToken> endorsingSupportingTokens;
    private List<SecurityToken> endorsingDerivedSupportingTokens;
    private List<SecurityToken> signedEndorsingSupportingTokens;
    private List<SecurityToken> signedEndorsingDerivedSupportingTokens;
    private List<SendSecurityHeaderElement> signatureConfirmations;
    private List<SendSecurityHeaderElement> endorsingSignatures;
    private Dictionary<SecurityToken, SecurityKeyIdentifierClause> securityTokenMappedToIdentifierClause;
    public SecurityTimestamp Timestamp;
    public SecurityToken PrerequisiteToken;
    public SecurityToken SourceSigningToken;
    public SecurityToken DerivedSigningToken;
    public SecurityToken SourceEncryptionToken;
    public SecurityToken WrappedEncryptionToken;
    public SecurityToken DerivedEncryptionToken;
    public ISecurityElement ReferenceList;
    public SendSecurityHeaderElement PrimarySignature;

    public List<SecurityToken> EndorsingSupportingTokens
    {
      get
      {
        return this.endorsingSupportingTokens;
      }
    }

    private void Add<T>(ref List<T> list, T item)
    {
      if (list == null)
        list = new List<T>();
      list.Add(item);
    }

    public SecurityToken[] GetSignedSupportingTokens()
    {
      if (this.signedSupportingTokens == null)
        return (SecurityToken[]) null;
      return this.signedSupportingTokens.ToArray();
    }

    public void AddSignedSupportingToken(SecurityToken token)
    {
      this.Add<SecurityToken>(ref this.signedSupportingTokens, token);
    }

    public SendSecurityHeaderElement[] GetBasicSupportingTokens()
    {
      if (this.basicSupportingTokens == null)
        return (SendSecurityHeaderElement[]) null;
      return this.basicSupportingTokens.ToArray();
    }

    public void AddBasicSupportingToken(SendSecurityHeaderElement tokenElement)
    {
      this.Add<SendSecurityHeaderElement>(ref this.basicSupportingTokens, tokenElement);
    }

    public SecurityToken[] GetSignedEndorsingSupportingTokens()
    {
      if (this.signedEndorsingSupportingTokens == null)
        return (SecurityToken[]) null;
      return this.signedEndorsingSupportingTokens.ToArray();
    }

    public void AddSignedEndorsingSupportingToken(SecurityToken token)
    {
      this.Add<SecurityToken>(ref this.signedEndorsingSupportingTokens, token);
    }

    public SecurityToken[] GetSignedEndorsingDerivedSupportingTokens()
    {
      if (this.signedEndorsingDerivedSupportingTokens == null)
        return (SecurityToken[]) null;
      return this.signedEndorsingDerivedSupportingTokens.ToArray();
    }

    public void AddSignedEndorsingDerivedSupportingToken(SecurityToken token)
    {
      this.Add<SecurityToken>(ref this.signedEndorsingDerivedSupportingTokens, token);
    }

    public SecurityToken[] GetEndorsingSupportingTokens()
    {
      if (this.endorsingSupportingTokens == null)
        return (SecurityToken[]) null;
      return this.endorsingSupportingTokens.ToArray();
    }

    public void AddEndorsingSupportingToken(SecurityToken token)
    {
      this.Add<SecurityToken>(ref this.endorsingSupportingTokens, token);
    }

    public SecurityToken[] GetEndorsingDerivedSupportingTokens()
    {
      if (this.endorsingDerivedSupportingTokens == null)
        return (SecurityToken[]) null;
      return this.endorsingDerivedSupportingTokens.ToArray();
    }

    public void AddEndorsingDerivedSupportingToken(SecurityToken token)
    {
      this.Add<SecurityToken>(ref this.endorsingDerivedSupportingTokens, token);
    }

    public SendSecurityHeaderElement[] GetSignatureConfirmations()
    {
      if (this.signatureConfirmations == null)
        return (SendSecurityHeaderElement[]) null;
      return this.signatureConfirmations.ToArray();
    }

    public void AddSignatureConfirmation(SendSecurityHeaderElement confirmation)
    {
      this.Add<SendSecurityHeaderElement>(ref this.signatureConfirmations, confirmation);
    }

    public SendSecurityHeaderElement[] GetEndorsingSignatures()
    {
      if (this.endorsingSignatures == null)
        return (SendSecurityHeaderElement[]) null;
      return this.endorsingSignatures.ToArray();
    }

    public void AddEndorsingSignature(SendSecurityHeaderElement signature)
    {
      this.Add<SendSecurityHeaderElement>(ref this.endorsingSignatures, signature);
    }

    public void MapSecurityTokenToStrClause(SecurityToken securityToken, SecurityKeyIdentifierClause keyIdentifierClause)
    {
      if (this.securityTokenMappedToIdentifierClause == null)
        this.securityTokenMappedToIdentifierClause = new Dictionary<SecurityToken, SecurityKeyIdentifierClause>();
      if (this.securityTokenMappedToIdentifierClause.ContainsKey(securityToken))
        return;
      this.securityTokenMappedToIdentifierClause.Add(securityToken, keyIdentifierClause);
    }

    public bool TryGetIdentifierClauseFromSecurityToken(SecurityToken securityToken, out SecurityKeyIdentifierClause keyIdentifierClause)
    {
      keyIdentifierClause = (SecurityKeyIdentifierClause) null;
      return securityToken != null && this.securityTokenMappedToIdentifierClause != null && this.securityTokenMappedToIdentifierClause.TryGetValue(securityToken, out keyIdentifierClause);
    }
  }
}
