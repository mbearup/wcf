// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.Tokens.GenericXmlSecurityToken
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Collections.ObjectModel;
using System.Globalization;
using System.IdentityModel.Policy;
using System.IO;
using System.Xml;

namespace System.IdentityModel.Tokens
{
  /// <summary>Represents a security token that is based upon XML.</summary>
  public class GenericXmlSecurityToken : SecurityToken
  {
    private const int SupportedPersistanceVersion = 1;
    private string id;
    private SecurityToken proofToken;
    private SecurityKeyIdentifierClause internalTokenReference;
    private SecurityKeyIdentifierClause externalTokenReference;
    private XmlElement tokenXml;
    private ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies;
    private DateTime effectiveTime;
    private DateTime expirationTime;

    /// <summary>Gets a unique identifier of the security token.</summary>
    /// <returns>The unique identifier of the security token.</returns>
    public override string Id
    {
      get
      {
        return this.id;
      }
    }

    /// <summary>Gets the first instant in time at which this security token is valid.</summary>
    /// <returns>A <see cref="T:System.DateTime" /> that represents the first instant in time at which this security token is valid.</returns>
    public override DateTime ValidFrom
    {
      get
      {
        return this.effectiveTime;
      }
    }

    /// <summary>Gets the last instant in time at which this security token is valid.</summary>
    /// <returns>A <see cref="T:System.DateTime" /> that represents the last instant in time at which this security token is valid.</returns>
    public override DateTime ValidTo
    {
      get
      {
        return this.expirationTime;
      }
    }

    /// <summary>Gets a security key identifier clause that references this security token when this security token is included in the SOAP message in which it is referenced.</summary>
    /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> that represents a reference to this security token when it is included in a SOAP message in which it is referenced.</returns>
    public SecurityKeyIdentifierClause InternalTokenReference
    {
      get
      {
        return this.internalTokenReference;
      }
    }

    /// <summary>Gets a security key identifier clause that references this security token when this security token is not included in the SOAP message in which it is referenced.</summary>
    /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> that represents a reference to this security token when it is not included in a SOAP message in which it is referenced.</returns>
    public SecurityKeyIdentifierClause ExternalTokenReference
    {
      get
      {
        return this.externalTokenReference;
      }
    }

    /// <summary>Gets the XML that is associated with the security token. </summary>
    /// <returns>An <see cref="T:System.Xml.XmlElement" /> that represents the XML that is associated with the security token.</returns>
    public XmlElement TokenXml
    {
      get
      {
        return this.tokenXml;
      }
    }

    /// <summary>Gets the proof token for the security token.</summary>
    /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityToken" /> that represents the proof token for the security token.</returns>
    public SecurityToken ProofToken
    {
      get
      {
        return this.proofToken;
      }
    }

    /// <summary>Gets the collection of authorization policies for this security token.</summary>
    /// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of type <see cref="T:System.IdentityModel.Policy.IAuthorizationPolicy" /> that contains the set authorization policies for this security token.</returns>
    public ReadOnlyCollection<IAuthorizationPolicy> AuthorizationPolicies
    {
      get
      {
        return this.authorizationPolicies;
      }
    }

    /// <summary>Gets the cryptographic keys associated with the proof token.</summary>
    /// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of type <see cref="T:System.IdentityModel.Tokens.SecurityKey" /> that contains the set of keys associated with the proof token.</returns>
    public override ReadOnlyCollection<SecurityKey> SecurityKeys
    {
      get
      {
        if (this.proofToken != null)
          return this.proofToken.SecurityKeys;
        return EmptyReadOnlyCollection<SecurityKey>.Instance;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.GenericXmlSecurityToken" /> class.  </summary>
    /// <param name="tokenXml">An <see cref="T:System.Xml.XmlElement" /> that represents the XML that is associated with the security token. Sets the <see cref="P:System.IdentityModel.Tokens.GenericXmlSecurityToken.TokenXml" /> property.</param>
    /// <param name="proofToken">A <see cref="T:System.IdentityModel.Tokens.SecurityToken" /> that represents the proof token for the security token. Sets the <see cref="P:System.IdentityModel.Tokens.GenericXmlSecurityToken.ProofToken" /> property.</param>
    /// <param name="effectiveTime">A <see cref="T:System.DateTime" /> that represents the first instant in time at which this security token is valid. Sets the <see cref="P:System.IdentityModel.Tokens.GenericXmlSecurityToken.ValidFrom" /> property.</param>
    /// <param name="expirationTime">A <see cref="T:System.DateTime" /> that represents the last instant in time at which this security token is valid. Sets the <see cref="P:System.IdentityModel.Tokens.GenericXmlSecurityToken.ValidFrom" /> property.</param>
    /// <param name="internalTokenReference">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> that represents a reference to this security token when it is included in a SOAP message in which it is referenced. Sets the <see cref="P:System.IdentityModel.Tokens.GenericXmlSecurityToken.InternalTokenReference" /> property.</param>
    /// <param name="externalTokenReference">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> that represents a reference to this security token when it is not included in a SOAP message in which it is referenced. Sets the <see cref="P:System.IdentityModel.Tokens.GenericXmlSecurityToken.ValidFrom" /> property.</param>
    /// <param name="authorizationPolicies">A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of type <see cref="T:System.IdentityModel.Policy.IAuthorizationPolicy" /> that contains the set authorization policies for this security token.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="tokenXml" /> is null.-or-<paramref name="proofToken" /> is null.</exception>
    public GenericXmlSecurityToken(XmlElement tokenXml, SecurityToken proofToken, DateTime effectiveTime, DateTime expirationTime, SecurityKeyIdentifierClause internalTokenReference, SecurityKeyIdentifierClause externalTokenReference, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies)
    {
      if (tokenXml == null)
        throw new ArgumentNullException("tokenXml");
      this.id = GenericXmlSecurityToken.GetId(tokenXml);
      this.tokenXml = tokenXml;
      this.proofToken = proofToken;
      this.effectiveTime = effectiveTime.ToUniversalTime();
      this.expirationTime = expirationTime.ToUniversalTime();
      this.internalTokenReference = internalTokenReference;
      this.externalTokenReference = externalTokenReference;
      this.authorizationPolicies = authorizationPolicies ?? EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance;
    }

    /// <summary>Returns the current object.</summary>
    /// <returns>The current object.</returns>
    public override string ToString()
    {
      StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      stringWriter.WriteLine("Generic XML token:");
      stringWriter.WriteLine("   validFrom: {0}", (object) this.ValidFrom);
      stringWriter.WriteLine("   validTo: {0}", (object) this.ValidTo);
      if (this.internalTokenReference != null)
        stringWriter.WriteLine("   InternalTokenReference: {0}", (object) this.internalTokenReference);
      if (this.externalTokenReference != null)
        stringWriter.WriteLine("   ExternalTokenReference: {0}", (object) this.externalTokenReference);
      stringWriter.WriteLine("   Token Element: ({0}, {1})", (object) this.tokenXml.LocalName, (object) this.tokenXml.NamespaceURI);
      return stringWriter.ToString();
    }

    private static string GetId(XmlElement tokenXml)
    {
      if (tokenXml != null)
      {
        string attribute = tokenXml.GetAttribute("Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
        if (string.IsNullOrEmpty(attribute))
        {
          attribute = tokenXml.GetAttribute("AssertionID");
          if (string.IsNullOrEmpty(attribute))
            attribute = tokenXml.GetAttribute("Id");
          if (string.IsNullOrEmpty(attribute))
            attribute = tokenXml.GetAttribute("ID");
        }
        if (!string.IsNullOrEmpty(attribute))
          return attribute;
      }
      return (string) null;
    }

    /// <summary>Gets a value that indicates whether this security token is capable of creating the specified key identifier clause.</summary>
    /// <typeparam name="T">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> that specifies the key identifier to create.</typeparam>
    /// <returns>true when <paramref name="T" /> is not null and the same type as either the <see cref="P:System.IdentityModel.Tokens.GenericXmlSecurityToken.InternalTokenReference" /> or <see cref="P:System.IdentityModel.Tokens.GenericXmlSecurityToken.ExternalTokenReference" /> property values; otherwise, false.</returns>
    public override bool CanCreateKeyIdentifierClause<T>()
    {
      return this.internalTokenReference != null && typeof (T) == this.internalTokenReference.GetType() || this.externalTokenReference != null && typeof (T) == this.externalTokenReference.GetType();
    }

    /// <summary>Creates the specified key identifier clause.</summary>
    /// <typeparam name="T">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> that specifies the key identifier to create.</typeparam>
    /// <returns>A <see cref="T:System.IdentityModel.Tokens.SamlAssertionKeyIdentifierClause" /> that is a key identifier clause for a <see cref="T:System.IdentityModel.Tokens.GenericXmlSecurityToken" /> security token.</returns>
    /// <exception cref="T:System.IdentityModel.Tokens.SecurityTokenException">
    /// <paramref name="T" /> is not null and not the same type as one of the <see cref="P:System.IdentityModel.Tokens.GenericXmlSecurityToken.InternalTokenReference" /> or <see cref="P:System.IdentityModel.Tokens.GenericXmlSecurityToken.ExternalTokenReference" /> property values.</exception>
    public override T CreateKeyIdentifierClause<T>()
    {
      if (this.internalTokenReference != null && typeof (T) == this.internalTokenReference.GetType())
        return (T) this.internalTokenReference;
      if (this.externalTokenReference != null && typeof (T) == this.externalTokenReference.GetType())
        return (T) this.externalTokenReference;
      throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new SecurityTokenException(SR.GetString("UnableToCreateTokenReference")));
    }

    /// <summary>Returns a value that indicates whether the key identifier for this instance is equal to the specified key identifier.</summary>
    /// <param name="keyIdentifierClause">An <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" /> to compare to this instance.</param>
    /// <returns>true when <paramref name="keyIdentifierClause" /> is not null and matches either the <see cref="P:System.IdentityModel.Tokens.GenericXmlSecurityToken.InternalTokenReference" /> or <see cref="P:System.IdentityModel.Tokens.GenericXmlSecurityToken.ExternalTokenReference" /> property values; otherwise, false.</returns>
    public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
    {
      return this.internalTokenReference != null && this.internalTokenReference.Matches(keyIdentifierClause) || this.externalTokenReference != null && this.externalTokenReference.Matches(keyIdentifierClause);
    }
  }
}
