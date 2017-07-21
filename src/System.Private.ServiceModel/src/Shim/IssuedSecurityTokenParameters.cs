// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.Tokens.IssuedSecurityTokenParameters
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace System.ServiceModel.Security.Tokens
{
  /// <summary>Represents the parameters for a security token issued in a Federated security scenario.</summary>
  public class IssuedSecurityTokenParameters : SecurityTokenParameters
  {
    private static readonly string wsidPPIClaim = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/claims/privatepersonalidentifier", new object[1]{ (object) "http://schemas.xmlsoap.org/ws/2005/05/identity" });
    private Collection<XmlElement> additionalRequestParameters = new Collection<XmlElement>();
    private Collection<IssuedSecurityTokenParameters.AlternativeIssuerEndpoint> alternativeIssuerEndpoints = new Collection<IssuedSecurityTokenParameters.AlternativeIssuerEndpoint>();
    private Collection<ClaimTypeRequirement> claimTypeRequirements = new Collection<ClaimTypeRequirement>();
    private const string wsidPrefix = "wsid";
    private const string wsidNamespace = "http://schemas.xmlsoap.org/ws/2005/05/identity";
    internal const SecurityKeyType defaultKeyType = SecurityKeyType.SymmetricKey;
    internal const bool defaultUseStrTransform = false;
    private MessageSecurityVersion defaultMessageSecurityVersion;
    private EndpointAddress issuerAddress;
    private EndpointAddress issuerMetadataAddress;
    private Binding issuerBinding;
    private int keySize;
    private SecurityKeyType keyType;
    private bool useStrTransform;
    private string tokenType;

    /// <summary>Gets a value that indicates whether the issued token has an asymmetric key.</summary>
    /// <returns>true if the issued token has an asymmetric key; otherwise, false.</returns>
    protected internal override bool HasAsymmetricKey
    {
      get
      {
        return this.KeyType == SecurityKeyType.AsymmetricKey;
      }
    }

    /// <summary>Gets a collection of additional request parameters</summary>
    /// <returns>A <see cref="T:System.Collections.ObjectModel.Collection`1" /> of type <see cref="T:System.Xml.XmlElement" /> that holds the additional request parameters.</returns>
    public Collection<XmlElement> AdditionalRequestParameters
    {
      get
      {
        return this.additionalRequestParameters;
      }
    }

    /// <summary>Gets or sets the default set of security specifications versions.</summary>
    /// <returns>A <see cref="T:System.ServiceModel.MessageSecurityVersion" /> that represents the default set of security specifications versions.</returns>
    public MessageSecurityVersion DefaultMessageSecurityVersion
    {
      get
      {
        return this.defaultMessageSecurityVersion;
      }
      set
      {
        this.defaultMessageSecurityVersion = value;
      }
    }

    public Collection<IssuedSecurityTokenParameters.AlternativeIssuerEndpoint> AlternativeIssuerEndpoints
    {
      get
      {
        return this.alternativeIssuerEndpoints;
      }
    }

    /// <summary>Gets or sets the token issuer's address.</summary>
    /// <returns>The <see cref="T:System.ServiceModel.EndpointAddress" /> of the token issuer.</returns>
    public EndpointAddress IssuerAddress
    {
      get
      {
        return this.issuerAddress;
      }
      set
      {
        this.issuerAddress = value;
      }
    }

    /// <summary>Gets or sets the token issuer's metadata address.</summary>
    /// <returns>The metadata address of the token issuer.</returns>
    public EndpointAddress IssuerMetadataAddress
    {
      get
      {
        return this.issuerMetadataAddress;
      }
      set
      {
        this.issuerMetadataAddress = value;
      }
    }

    /// <summary>Gets or sets the token issuer's binding, to be used by the client.</summary>
    /// <returns>The <see cref="T:System.ServiceModel.Channels.Binding" /> of the token issuer, to be used by the client.</returns>
    public Binding IssuerBinding
    {
      get
      {
        return this.issuerBinding;
      }
      set
      {
        this.issuerBinding = value;
      }
    }

    /// <summary>Gets or sets the issued token key type.</summary>
    /// <returns>One of the <see cref="T:System.IdentityModel.Tokens.SecurityKeyType" /> values.</returns>
    public SecurityKeyType KeyType
    {
      get
      {
        return this.keyType;
      }
      set
      {
        SecurityKeyTypeHelper.Validate(value);
        this.keyType = value;
      }
    }

    /// <summary>Gets or sets the issued token key size.</summary>
    /// <returns>The size of the token key.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">An attempt was made to set a value less than 0.</exception>
    public int KeySize
    {
      get
      {
        return this.keySize;
      }
      set
      {
        if (value < 0)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", SR.GetString("ValueMustBeNonNegative")));
        this.keySize = value;
      }
    }

    /// <summary>Gets or sets a value that indicates whether the issued token parameter uses STR transform.</summary>
    /// <returns>true if the issued token parameter uses STR transform; otherwise, false.</returns>
    public bool UseStrTransform
    {
      get
      {
        return this.useStrTransform;
      }
      set
      {
        this.useStrTransform = value;
      }
    }

    /// <summary>Gets a collection of claim type requirements.</summary>
    /// <returns>A <see cref="T:System.Collections.ObjectModel.Collection`1" /> of type <see cref="T:System.ServiceModel.Security.Tokens.ClaimTypeRequirement" /> that holds the additional claim type requirements.</returns>
    public Collection<ClaimTypeRequirement> ClaimTypeRequirements
    {
      get
      {
        return this.claimTypeRequirements;
      }
    }

    /// <summary>Gets or sets the issued token type.</summary>
    /// <returns>The token type.</returns>
    public string TokenType
    {
      get
      {
        return this.tokenType;
      }
      set
      {
        this.tokenType = value;
      }
    }

    /// <summary>When implemented, gets a value that indicates whether the token supports client authentication.</summary>
    /// <returns>true if the token supports client authentication; otherwise, false.</returns>
    protected internal override bool SupportsClientAuthentication
    {
      get
      {
        return true;
      }
    }

    /// <summary>When implemented, gets a value that indicates whether the token supports server authentication.</summary>
    /// <returns>true if the token supports server authentication; otherwise, false.</returns>
    protected internal override bool SupportsServerAuthentication
    {
      get
      {
        return true;
      }
    }

    /// <summary>When implemented, gets a value that indicates whether the token supports a Windows identity for authentication.</summary>
    /// <returns>true if the token supports a Windows identity for authentication; otherwise, false.</returns>
    protected internal override bool SupportsClientWindowsIdentity
    {
      get
      {
        return false;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.IssuedSecurityTokenParameters" /> class.</summary>
    /// <param name="other">The other instance of this class.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="other" /> is null.</exception>
    protected IssuedSecurityTokenParameters(IssuedSecurityTokenParameters other)
      : base((SecurityTokenParameters) other)
    {
      this.defaultMessageSecurityVersion = other.defaultMessageSecurityVersion;
      this.issuerAddress = other.issuerAddress;
      this.keyType = other.keyType;
      this.tokenType = other.tokenType;
      this.keySize = other.keySize;
      this.useStrTransform = other.useStrTransform;
      foreach (XmlNode requestParameter in other.additionalRequestParameters)
        this.additionalRequestParameters.Add((XmlElement) requestParameter.Clone());
      foreach (ClaimTypeRequirement claimTypeRequirement in other.claimTypeRequirements)
        this.claimTypeRequirements.Add(claimTypeRequirement);
      if (other.issuerBinding != null)
        this.issuerBinding = (Binding) new CustomBinding(other.issuerBinding);
      this.issuerMetadataAddress = other.issuerMetadataAddress;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.IssuedSecurityTokenParameters" /> class.</summary>
    public IssuedSecurityTokenParameters()
      : this((string) null, (EndpointAddress) null, (Binding) null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.IssuedSecurityTokenParameters" /> class using the specified token type.</summary>
    /// <param name="tokenType">The token type.</param>
    public IssuedSecurityTokenParameters(string tokenType)
      : this(tokenType, (EndpointAddress) null, (Binding) null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.IssuedSecurityTokenParameters" /> class using the specified token type and issuer address.</summary>
    /// <param name="tokenType">The token type.</param>
    /// <param name="issuerAddress">The address of the endpoint that issues the token.</param>
    public IssuedSecurityTokenParameters(string tokenType, EndpointAddress issuerAddress)
      : this(tokenType, issuerAddress, (Binding) null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.IssuedSecurityTokenParameters" /> class using the specified token type, issuer address and issuer binding.</summary>
    /// <param name="tokenType">The token type.</param>
    /// <param name="issuerAddress">The address of the endpoint that issues the token.</param>
    /// <param name="issuerBinding">The binding of the issuer.</param>
    public IssuedSecurityTokenParameters(string tokenType, EndpointAddress issuerAddress, Binding issuerBinding)
    {
      this.tokenType = tokenType;
      this.issuerAddress = issuerAddress;
      this.issuerBinding = issuerBinding;
    }

    /// <summary>Clones another instance of this instance of the class.</summary>
    /// <returns>A <see cref="T:System.ServiceModel.Security.Tokens.SecurityTokenParameters" /> that represents the copy.</returns>
    protected override SecurityTokenParameters CloneCore()
    {
      return (SecurityTokenParameters) new IssuedSecurityTokenParameters(this);
    }

    /// <summary>Creates a key identifier clause for a token.</summary>
    /// <param name="token">The token.</param>
    /// <param name="referenceStyle">The reference style of the security token.</param>
    /// <returns>The security key identifier clause.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="token" /> is null.</exception>
    /// <exception cref="T:System.NotSupportedException">
    /// <paramref name="referenceStyle" /> is not External or Internal.</exception>
#if FEATURE_CORECLR
    protected override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
#else
    protected internal override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
#endif
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("CreateGenericXmlTokenKeyIdentifierClause is not supported in .NET Core");
#else
      if (token is GenericXmlSecurityToken)
        return this.CreateGenericXmlTokenKeyIdentifierClause(token, referenceStyle);
      return this.CreateKeyIdentifierClause<SamlAssertionKeyIdentifierClause, SamlAssertionKeyIdentifierClause>(token, referenceStyle);
#endif
    }

    internal void SetRequestParameters(Collection<XmlElement> requestParameters, TrustDriver trustDriver)
    {
      if (requestParameters == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("requestParameters");
      if (trustDriver == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("trustDriver");
      Collection<XmlElement> unknownRequestParameters = new Collection<XmlElement>();
      foreach (XmlElement requestParameter in requestParameters)
      {
        int keySize;
        if (trustDriver.TryParseKeySizeElement(requestParameter, out keySize))
        {
          this.keySize = keySize;
        }
        else
        {
          SecurityKeyType keyType;
          if (trustDriver.TryParseKeyTypeElement(requestParameter, out keyType))
          {
            this.KeyType = keyType;
          }
          else
          {
            string tokenType;
            if (trustDriver.TryParseTokenTypeElement(requestParameter, out tokenType))
              this.TokenType = tokenType;
            else if (trustDriver.StandardsManager.TrustVersion == TrustVersion.WSTrustFeb2005)
            {
              Collection<XmlElement> requiredClaims;
              if (trustDriver.TryParseRequiredClaimsElement(requestParameter, out requiredClaims))
              {
                Collection<XmlElement> collection = new Collection<XmlElement>();
                foreach (XmlElement xmlElement in requiredClaims)
                {
                  if (xmlElement.LocalName == "ClaimType" && xmlElement.NamespaceURI == "http://schemas.xmlsoap.org/ws/2005/05/identity")
                  {
                    string attribute1 = xmlElement.GetAttribute("Uri", string.Empty);
                    if (!string.IsNullOrEmpty(attribute1))
                    {
                      string attribute2 = xmlElement.GetAttribute("Optional", string.Empty);
                      this.claimTypeRequirements.Add(!string.IsNullOrEmpty(attribute2) ? new ClaimTypeRequirement(attribute1, XmlConvert.ToBoolean(attribute2)) : new ClaimTypeRequirement(attribute1));
                    }
                  }
                  else
                    collection.Add(xmlElement);
                }
                if (collection.Count > 0)
                  unknownRequestParameters.Add(trustDriver.CreateRequiredClaimsElement((IEnumerable<XmlElement>) collection));
              }
              else
                unknownRequestParameters.Add(requestParameter);
            }
          }
        }
      }
      Collection<XmlElement> collection1 = trustDriver.ProcessUnknownRequestParameters(unknownRequestParameters, requestParameters);
      if (collection1.Count <= 0)
        return;
      for (int index = 0; index < collection1.Count; ++index)
        this.AdditionalRequestParameters.Add(collection1[index]);
    }

    /// <summary>Creates a collection of issued token request parameter XML elements that get included in the request sent by the client to the security token service.</summary>
    /// <param name="messageSecurityVersion">The message security version.</param>
    /// <param name="securityTokenSerializer">The security token serializer.</param>
    /// <returns>A <see cref="T:System.Collections.ObjectModel.Collection`1" /> that contains XML elements that represent the request parameters.</returns>
    public Collection<XmlElement> CreateRequestParameters(MessageSecurityVersion messageSecurityVersion, SecurityTokenSerializer securityTokenSerializer)
    {
      return this.CreateRequestParameters(System.ServiceModel.Security.SecurityUtils.CreateSecurityStandardsManager(messageSecurityVersion, securityTokenSerializer).TrustDriver);
    }

    internal Collection<XmlElement> CreateRequestParameters(TrustDriver driver)
    {
      if (driver == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("driver");
      Collection<XmlElement> collection1 = new Collection<XmlElement>();
      if (this.tokenType != null)
        collection1.Add(driver.CreateTokenTypeElement(this.tokenType));
      collection1.Add(driver.CreateKeyTypeElement(this.keyType));
      if (this.keySize != 0)
        collection1.Add(driver.CreateKeySizeElement(this.keySize));
      if (this.claimTypeRequirements.Count > 0)
      {
        Collection<XmlElement> collection2 = new Collection<XmlElement>();
        XmlDocument xmlDocument = new XmlDocument();
        foreach (ClaimTypeRequirement claimTypeRequirement in this.claimTypeRequirements)
        {
          XmlElement element = xmlDocument.CreateElement("wsid", "ClaimType", "http://schemas.xmlsoap.org/ws/2005/05/identity");
          XmlAttribute attribute1 = xmlDocument.CreateAttribute("Uri");
          attribute1.Value = claimTypeRequirement.ClaimType;
          element.Attributes.Append(attribute1);
          if (claimTypeRequirement.IsOptional)
          {
            XmlAttribute attribute2 = xmlDocument.CreateAttribute("Optional");
            attribute2.Value = XmlConvert.ToString(claimTypeRequirement.IsOptional);
            element.Attributes.Append(attribute2);
          }
          collection2.Add(element);
        }
        collection1.Add(driver.CreateRequiredClaimsElement((IEnumerable<XmlElement>) collection2));
      }
      if (this.additionalRequestParameters.Count > 0)
      {
        foreach (XmlElement additionalParameter in this.NormalizeAdditionalParameters(this.additionalRequestParameters, driver, this.claimTypeRequirements.Count > 0))
          collection1.Add(additionalParameter);
      }
      return collection1;
    }

    private Collection<XmlElement> NormalizeAdditionalParameters(Collection<XmlElement> additionalParameters, TrustDriver driver, bool clientSideClaimTypeRequirementsSpecified)
    {
      Collection<XmlElement> collection1 = new Collection<XmlElement>();
      foreach (XmlElement additionalParameter in additionalParameters)
        collection1.Add(additionalParameter);
      if (driver.StandardsManager.TrustVersion == TrustVersion.WSTrust13)
      {
        XmlElement xmlElement1 = (XmlElement) null;
        XmlElement xmlElement2 = (XmlElement) null;
        XmlElement xmlElement3 = (XmlElement) null;
        XmlElement xmlElement4 = (XmlElement) null;
        for (int index = 0; index < collection1.Count; ++index)
        {
          string str;
          if (driver.IsEncryptionAlgorithmElement(collection1[index], out str))
            xmlElement1 = collection1[index];
          else if (driver.IsCanonicalizationAlgorithmElement(collection1[index], out str))
            xmlElement2 = collection1[index];
          else if (driver.IsKeyWrapAlgorithmElement(collection1[index], out str))
            xmlElement3 = collection1[index];
          else if (((WSTrustDec2005.DriverDec2005) driver).IsSecondaryParametersElement(collection1[index]))
            xmlElement4 = collection1[index];
        }
        if (xmlElement4 != null)
        {
          foreach (XmlNode childNode in xmlElement4.ChildNodes)
          {
            XmlElement element = childNode as XmlElement;
            if (element != null)
            {
              string str = (string) null;
              if (driver.IsEncryptionAlgorithmElement(element, out str) && xmlElement1 != null)
                collection1.Remove(xmlElement1);
              else if (driver.IsCanonicalizationAlgorithmElement(element, out str) && xmlElement2 != null)
                collection1.Remove(xmlElement2);
              else if (driver.IsKeyWrapAlgorithmElement(element, out str) && xmlElement3 != null)
                collection1.Remove(xmlElement3);
            }
          }
        }
      }
      if ((driver.StandardsManager.TrustVersion != TrustVersion.WSTrustFeb2005 || this.CollectionContainsElementsWithTrustNamespace(additionalParameters, "http://schemas.xmlsoap.org/ws/2005/02/trust")) && (driver.StandardsManager.TrustVersion != TrustVersion.WSTrust13 || this.CollectionContainsElementsWithTrustNamespace(additionalParameters, "http://docs.oasis-open.org/ws-sx/ws-trust/200512")))
        return collection1;
      if (driver.StandardsManager.TrustVersion == TrustVersion.WSTrust13)
      {
        WSTrustFeb2005.DriverFeb2005 trustDriver = (WSTrustFeb2005.DriverFeb2005) SecurityStandardsManager.DefaultInstance.TrustDriver;
        for (int index = 0; index < collection1.Count; ++index)
        {
          string empty = string.Empty;
          if (trustDriver.IsSignWithElement(collection1[index], out empty))
            collection1[index] = driver.CreateSignWithElement(empty);
          else if (trustDriver.IsEncryptWithElement(collection1[index], out empty))
            collection1[index] = driver.CreateEncryptWithElement(empty);
          else if (trustDriver.IsEncryptionAlgorithmElement(collection1[index], out empty))
            collection1[index] = driver.CreateEncryptionAlgorithmElement(empty);
          else if (trustDriver.IsCanonicalizationAlgorithmElement(collection1[index], out empty))
            collection1[index] = driver.CreateCanonicalizationAlgorithmElement(empty);
        }
      }
      else
      {
        Collection<XmlElement> collection2 = (Collection<XmlElement>) null;
        WSTrustDec2005.DriverDec2005 trustDriver = (WSTrustDec2005.DriverDec2005) new SecurityStandardsManager(MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12, (SecurityTokenSerializer) new WSSecurityTokenSerializer(SecurityVersion.WSSecurity11, TrustVersion.WSTrust13, SecureConversationVersion.WSSecureConversation13, true, (SamlSerializer1) null, (SecurityStateEncoder) null, (IEnumerable<System.Type>) null)).TrustDriver;
        foreach (XmlElement element in collection1)
        {
          if (trustDriver.IsSecondaryParametersElement(element))
          {
            collection2 = new Collection<XmlElement>();
            foreach (XmlNode childNode in element.ChildNodes)
            {
              XmlElement innerElement = childNode as XmlElement;
              if (innerElement != null && this.CanPromoteToRoot(innerElement, trustDriver, clientSideClaimTypeRequirementsSpecified))
                collection2.Add(innerElement);
            }
            collection1.Remove(element);
            break;
          }
        }
        if (collection2 != null && collection2.Count > 0)
        {
          XmlElement xmlElement1 = (XmlElement) null;
          string encryptionAlgorithm = string.Empty;
          XmlElement xmlElement2 = (XmlElement) null;
          string canonicalizationAlgorithm = string.Empty;
          XmlElement xmlElement3 = (XmlElement) null;
          Collection<XmlElement> requiredClaims1 = (Collection<XmlElement>) null;
          Collection<XmlElement> collection3 = new Collection<XmlElement>();
          foreach (XmlElement element in collection2)
          {
            if (xmlElement1 == null && trustDriver.IsEncryptionAlgorithmElement(element, out encryptionAlgorithm))
            {
              xmlElement1 = driver.CreateEncryptionAlgorithmElement(encryptionAlgorithm);
              collection3.Add(element);
            }
            else if (xmlElement2 == null && trustDriver.IsCanonicalizationAlgorithmElement(element, out canonicalizationAlgorithm))
            {
              xmlElement2 = driver.CreateCanonicalizationAlgorithmElement(canonicalizationAlgorithm);
              collection3.Add(element);
            }
            else if (xmlElement3 == null && trustDriver.TryParseRequiredClaimsElement(element, out requiredClaims1))
            {
              xmlElement3 = driver.CreateRequiredClaimsElement((IEnumerable<XmlElement>) requiredClaims1);
              collection3.Add(element);
            }
          }
          for (int index = 0; index < collection3.Count; ++index)
            collection2.Remove(collection3[index]);
          XmlElement xmlElement4 = (XmlElement) null;
          for (int index = 0; index < collection1.Count; ++index)
          {
            string str;
            if (trustDriver.IsSignWithElement(collection1[index], out str))
              collection1[index] = driver.CreateSignWithElement(str);
            else if (trustDriver.IsEncryptWithElement(collection1[index], out str))
              collection1[index] = driver.CreateEncryptWithElement(str);
            else if (trustDriver.IsEncryptionAlgorithmElement(collection1[index], out str) && xmlElement1 != null)
            {
              collection1[index] = xmlElement1;
              xmlElement1 = (XmlElement) null;
            }
            else if (trustDriver.IsCanonicalizationAlgorithmElement(collection1[index], out str) && xmlElement2 != null)
            {
              collection1[index] = xmlElement2;
              xmlElement2 = (XmlElement) null;
            }
            else if (trustDriver.IsKeyWrapAlgorithmElement(collection1[index], out str) && xmlElement4 == null)
            {
              xmlElement4 = collection1[index];
            }
            else
            {
              Collection<XmlElement> requiredClaims2;
              if (trustDriver.TryParseRequiredClaimsElement(collection1[index], out requiredClaims2) && xmlElement3 != null)
              {
                collection1[index] = xmlElement3;
                xmlElement3 = (XmlElement) null;
              }
            }
          }
          if (xmlElement4 != null)
            collection1.Remove(xmlElement4);
          if (xmlElement1 != null)
            collection1.Add(xmlElement1);
          if (xmlElement2 != null)
            collection1.Add(xmlElement2);
          if (xmlElement3 != null)
            collection1.Add(xmlElement3);
          if (collection2.Count > 0)
          {
            for (int index = 0; index < collection2.Count; ++index)
              collection1.Add(collection2[index]);
          }
        }
      }
      return collection1;
    }

    private bool CollectionContainsElementsWithTrustNamespace(Collection<XmlElement> collection, string trustNamespace)
    {
      for (int index = 0; index < collection.Count; ++index)
      {
        if (collection[index] != null && collection[index].NamespaceURI == trustNamespace)
          return true;
      }
      return false;
    }

    private bool CanPromoteToRoot(XmlElement innerElement, WSTrustDec2005.DriverDec2005 trust13Driver, bool clientSideClaimTypeRequirementsSpecified)
    {
      Collection<XmlElement> requiredClaims = (Collection<XmlElement>) null;
      if (trust13Driver.TryParseRequiredClaimsElement(innerElement, out requiredClaims))
        return !clientSideClaimTypeRequirementsSpecified;
      SecurityKeyType keyType;
      int keySize;
      string str;
      if (!trust13Driver.TryParseKeyTypeElement(innerElement, out keyType) && !trust13Driver.TryParseKeySizeElement(innerElement, out keySize) && (!trust13Driver.TryParseTokenTypeElement(innerElement, out str) && !trust13Driver.IsSignWithElement(innerElement, out str)) && !trust13Driver.IsEncryptWithElement(innerElement, out str))
        return !trust13Driver.IsKeyWrapAlgorithmElement(innerElement, out str);
      return false;
    }

    public void AddAlgorithmParameters(SecurityAlgorithmSuite algorithmSuite, SecurityStandardsManager standardsManager, SecurityKeyType issuedKeyType)
    {
      this.additionalRequestParameters.Insert(0, standardsManager.TrustDriver.CreateEncryptionAlgorithmElement(algorithmSuite.DefaultEncryptionAlgorithm));
      this.additionalRequestParameters.Insert(0, standardsManager.TrustDriver.CreateCanonicalizationAlgorithmElement(algorithmSuite.DefaultCanonicalizationAlgorithm));
      if (this.keyType == SecurityKeyType.BearerKey)
        return;
      string signatureAlgorithm = this.keyType == SecurityKeyType.SymmetricKey ? algorithmSuite.DefaultSymmetricSignatureAlgorithm : algorithmSuite.DefaultAsymmetricSignatureAlgorithm;
      this.additionalRequestParameters.Insert(0, standardsManager.TrustDriver.CreateSignWithElement(signatureAlgorithm));
      string encryptionAlgorithm = issuedKeyType != SecurityKeyType.SymmetricKey ? algorithmSuite.DefaultAsymmetricKeyWrapAlgorithm : algorithmSuite.DefaultEncryptionAlgorithm;
      this.additionalRequestParameters.Insert(0, standardsManager.TrustDriver.CreateEncryptWithElement(encryptionAlgorithm));
      if (standardsManager.TrustVersion == TrustVersion.WSTrustFeb2005)
        return;
      this.additionalRequestParameters.Insert(0, ((WSTrustDec2005.DriverDec2005) standardsManager.TrustDriver).CreateKeyWrapAlgorithmElement(algorithmSuite.DefaultAsymmetricKeyWrapAlgorithm));
    }

    public bool DoAlgorithmsMatch(SecurityAlgorithmSuite algorithmSuite, SecurityStandardsManager standardsManager, out Collection<XmlElement> otherRequestParameters)
    {
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      bool flag5 = false;
      otherRequestParameters = new Collection<XmlElement>();
      bool flag6 = false;
      Collection<XmlElement> collection;
      if (standardsManager.TrustVersion == TrustVersion.WSTrust13 && this.AdditionalRequestParameters.Count == 1 && ((WSTrustDec2005.DriverDec2005) standardsManager.TrustDriver).IsSecondaryParametersElement(this.AdditionalRequestParameters[0]))
      {
        flag6 = true;
        collection = new Collection<XmlElement>();
        foreach (XmlElement xmlElement in (XmlNode) this.AdditionalRequestParameters[0])
          collection.Add(xmlElement);
      }
      else
        collection = this.AdditionalRequestParameters;
      for (int index = 0; index < collection.Count; ++index)
      {
        XmlElement element = collection[index];
        string str;
        if (standardsManager.TrustDriver.IsCanonicalizationAlgorithmElement(element, out str))
        {
          if (algorithmSuite.DefaultCanonicalizationAlgorithm != str)
            return false;
          flag4 = true;
        }
        else if (standardsManager.TrustDriver.IsSignWithElement(element, out str))
        {
          if (this.keyType == SecurityKeyType.SymmetricKey && str != algorithmSuite.DefaultSymmetricSignatureAlgorithm || this.keyType == SecurityKeyType.AsymmetricKey && str != algorithmSuite.DefaultAsymmetricSignatureAlgorithm)
            return false;
          flag1 = true;
        }
        else if (standardsManager.TrustDriver.IsEncryptWithElement(element, out str))
        {
          if (this.keyType == SecurityKeyType.SymmetricKey && str != algorithmSuite.DefaultEncryptionAlgorithm || this.keyType == SecurityKeyType.AsymmetricKey && str != algorithmSuite.DefaultAsymmetricKeyWrapAlgorithm)
            return false;
          flag2 = true;
        }
        else if (standardsManager.TrustDriver.IsEncryptionAlgorithmElement(element, out str))
        {
          if (str != algorithmSuite.DefaultEncryptionAlgorithm)
            return false;
          flag3 = true;
        }
        else if (standardsManager.TrustDriver.IsKeyWrapAlgorithmElement(element, out str))
        {
          if (str != algorithmSuite.DefaultAsymmetricKeyWrapAlgorithm)
            return false;
          flag5 = true;
        }
        else
          otherRequestParameters.Add(element);
      }
      if (flag6)
        otherRequestParameters = this.AdditionalRequestParameters;
      if (this.keyType == SecurityKeyType.BearerKey)
        return true;
      if (standardsManager.TrustVersion == TrustVersion.WSTrustFeb2005)
        return flag1 & flag4 & flag3 & flag2;
      return flag1 & flag4 & flag3 & flag2 & flag5;
    }

    public static IssuedSecurityTokenParameters CreateInfoCardParameters(SecurityStandardsManager standardsManager, SecurityAlgorithmSuite algorithm)
    {
      IssuedSecurityTokenParameters securityTokenParameters = new IssuedSecurityTokenParameters("http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1");
      securityTokenParameters.KeyType = SecurityKeyType.AsymmetricKey;
      securityTokenParameters.ClaimTypeRequirements.Add(new ClaimTypeRequirement(IssuedSecurityTokenParameters.wsidPPIClaim));
      securityTokenParameters.IssuerAddress = (EndpointAddress) null;
      securityTokenParameters.AddAlgorithmParameters(algorithm, standardsManager, securityTokenParameters.KeyType);
      return securityTokenParameters;
    }

    public static bool IsInfoCardParameters(IssuedSecurityTokenParameters parameters, SecurityStandardsManager standardsManager)
    {
      if (parameters == null || parameters.TokenType != "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1" || parameters.KeyType != SecurityKeyType.AsymmetricKey)
        return false;
      if (parameters.ClaimTypeRequirements.Count == 1)
      {
        ClaimTypeRequirement claimTypeRequirement = parameters.ClaimTypeRequirements[0];
        if (claimTypeRequirement == null || claimTypeRequirement.ClaimType != IssuedSecurityTokenParameters.wsidPPIClaim)
          return false;
      }
      else
      {
        if (parameters.AdditionalRequestParameters == null || parameters.AdditionalRequestParameters.Count <= 0)
          return false;
        bool flag = false;
        XmlElement claimTypeRequirement = IssuedSecurityTokenParameters.GetClaimTypeRequirement(parameters.AdditionalRequestParameters, standardsManager);
        if (claimTypeRequirement != null && claimTypeRequirement.ChildNodes.Count == 1)
        {
          XmlElement childNode = claimTypeRequirement.ChildNodes[0] as XmlElement;
          if (childNode != null)
          {
            XmlNode namedItem = childNode.Attributes.GetNamedItem("Uri");
            if (namedItem != null && namedItem.Value == IssuedSecurityTokenParameters.wsidPPIClaim)
              flag = true;
          }
        }
        if (!flag)
          return false;
      }
      return !(parameters.IssuerAddress != (EndpointAddress) null) && (parameters.AlternativeIssuerEndpoints == null || parameters.AlternativeIssuerEndpoints.Count <= 0);
    }

    internal static XmlElement GetClaimTypeRequirement(Collection<XmlElement> additionalRequestParameters, SecurityStandardsManager standardsManager)
    {
      foreach (XmlElement requestParameter in additionalRequestParameters)
      {
        if (requestParameter.LocalName == ((WSTrust.Driver) standardsManager.TrustDriver).DriverDictionary.Claims.Value && requestParameter.NamespaceURI == ((WSTrust.Driver) standardsManager.TrustDriver).DriverDictionary.Namespace.Value)
          return requestParameter;
        if (requestParameter.LocalName == DXD.TrustDec2005Dictionary.SecondaryParameters.Value && requestParameter.NamespaceURI == DXD.TrustDec2005Dictionary.Namespace.Value)
        {
          Collection<XmlElement> additionalRequestParameters1 = new Collection<XmlElement>();
          foreach (XmlNode childNode in requestParameter.ChildNodes)
          {
            XmlElement xmlElement = childNode as XmlElement;
            if (xmlElement != null)
              additionalRequestParameters1.Add(xmlElement);
          }
          XmlElement claimTypeRequirement = IssuedSecurityTokenParameters.GetClaimTypeRequirement(additionalRequestParameters1, standardsManager);
          if (claimTypeRequirement != null)
            return claimTypeRequirement;
        }
      }
      return (XmlElement) null;
    }

    /// <summary>Displays a text representation of this instance of the class.</summary>
    /// <returns>A text representation of this instance of this class.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(base.ToString());
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TokenType: {0}", new object[1]
      {
        (object) (this.tokenType == null ? "null" : this.tokenType)
      }));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "KeyType: {0}", new object[1]
      {
        (object) this.keyType.ToString()
      }));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "KeySize: {0}", new object[1]
      {
        (object) this.keySize.ToString((IFormatProvider) CultureInfo.InvariantCulture)
      }));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IssuerAddress: {0}", new object[1]
      {
        (object) (this.issuerAddress == (EndpointAddress) null ? "null" : this.issuerAddress.ToString())
      }));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IssuerMetadataAddress: {0}", new object[1]
      {
        (object) (this.issuerMetadataAddress == (EndpointAddress) null ? "null" : this.issuerMetadataAddress.ToString())
      }));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DefaultMessgeSecurityVersion: {0}", new object[1]
      {
        (object) (this.defaultMessageSecurityVersion == null ? "null" : this.defaultMessageSecurityVersion.ToString())
      }));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "UseStrTransform: {0}", new object[1]
      {
        (object) this.useStrTransform.ToString()
      }));
      if (this.issuerBinding == null)
      {
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IssuerBinding: null", new object[0]));
      }
      else
      {
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IssuerBinding:", new object[0]));
        BindingElementCollection bindingElements = this.issuerBinding.CreateBindingElements();
        for (int index = 0; index < bindingElements.Count; ++index)
        {
          stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "  BindingElement[{0}]:", new object[1]
          {
            (object) index.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          }));
          stringBuilder.AppendLine("    " + bindingElements[index].ToString().Trim().Replace("\n", "\n    "));
        }
      }
      if (this.claimTypeRequirements.Count == 0)
      {
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ClaimTypeRequirements: none", new object[0]));
      }
      else
      {
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ClaimTypeRequirements:", new object[0]));
        for (int index = 0; index < this.claimTypeRequirements.Count; ++index)
          stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "  {0}, optional={1}", new object[2]
          {
            (object) this.claimTypeRequirements[index].ClaimType,
            (object) this.claimTypeRequirements[index].IsOptional
          }));
      }
      return stringBuilder.ToString().Trim();
    }

    /// <summary>When implemented, initializes a security token requirement based on the properties set on the IssuedSecurityTokenParameters.</summary>
    /// <param name="requirement">The security token requirement to initialize.</param>
#if FEATURE_CORECLR
    protected override void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
#else
    protected internal override void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
#endif
    {
      requirement.TokenType = this.TokenType;
      requirement.RequireCryptographicToken = true;
      requirement.KeyType = this.KeyType;
      ServiceModelSecurityTokenRequirement tokenRequirement = requirement as ServiceModelSecurityTokenRequirement;
      if (tokenRequirement != null)
        tokenRequirement.DefaultMessageSecurityVersion = this.DefaultMessageSecurityVersion;
      else
        requirement.Properties[ServiceModelSecurityTokenRequirement.DefaultMessageSecurityVersionProperty] = (object) this.DefaultMessageSecurityVersion;
      if (this.KeySize > 0)
        requirement.KeySize = this.KeySize;
      requirement.Properties[ServiceModelSecurityTokenRequirement.IssuerAddressProperty] = (object) this.IssuerAddress;
      if (this.IssuerBinding != null)
        requirement.Properties[ServiceModelSecurityTokenRequirement.IssuerBindingProperty] = (object) this.IssuerBinding;
      requirement.Properties[ServiceModelSecurityTokenRequirement.IssuedSecurityTokenParametersProperty] = (object) this.Clone();
    }

    public struct AlternativeIssuerEndpoint
    {
      public EndpointAddress IssuerAddress;
      public EndpointAddress IssuerMetadataAddress;
      public Binding IssuerBinding;
    }
  }
}
