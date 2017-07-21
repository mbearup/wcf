// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.Tokens.BufferedGenericXmlSecurityToken
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Collections.ObjectModel;
using System.IdentityModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Xml;

namespace System.ServiceModel.Security.Tokens
{
  [TypeForwardedFrom("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
  internal class BufferedGenericXmlSecurityToken : GenericXmlSecurityToken
  {
    private System.IdentityModel.XmlBuffer tokenXmlBuffer;

    public System.IdentityModel.XmlBuffer TokenXmlBuffer
    {
      get
      {
        return this.tokenXmlBuffer;
      }
    }

    public BufferedGenericXmlSecurityToken(XmlElement tokenXml, SecurityToken proofToken, DateTime effectiveTime, DateTime expirationTime, SecurityKeyIdentifierClause internalTokenReference, SecurityKeyIdentifierClause externalTokenReference, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies, System.IdentityModel.XmlBuffer tokenXmlBuffer)
      : base(tokenXml, proofToken, effectiveTime, expirationTime, internalTokenReference, externalTokenReference, authorizationPolicies)
    {
      this.tokenXmlBuffer = tokenXmlBuffer;
    }
  }
}
