// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.Tokens.WrappedKeySecurityToken
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Collections.ObjectModel;
using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Xml;

namespace System.ServiceModel.Security.Tokens
{
  /// <summary>Represents a security token whose key is wrapped inside another token.</summary>
  [TypeForwardedFrom("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
  public class WrappedKeySecurityToken : SecurityToken
  {
    private string id;
    private DateTime effectiveTime;
    private EncryptedKey encryptedKey;
    private ReadOnlyCollection<SecurityKey> securityKey;
    private byte[] wrappedKey;
    private string wrappingAlgorithm;
    private ISspiNegotiation wrappingSspiContext;
    private SecurityToken wrappingToken;
    private SecurityKey wrappingSecurityKey;
    private SecurityKeyIdentifier wrappingTokenReference;
    private bool serializeCarriedKeyName;
    private byte[] wrappedKeyHash;
    private XmlDictionaryString wrappingAlgorithmDictionaryString;

    /// <summary>Gets the token ID.</summary>
    /// <returns>The token ID.</returns>
    public override string Id
    {
      get
      {
        return this.id;
      }
    }

    /// <summary>Gets the token effective start date.</summary>
    /// <returns>A <see cref="T:System.DateTime" /> that represents the token effective start date.</returns>
    public override DateTime ValidFrom
    {
      get
      {
        return this.effectiveTime;
      }
    }

    /// <summary>Gets the token expiration date.</summary>
    /// <returns>A <see cref="T:System.DateTime" /> that represents the token expiration date.</returns>
    public override DateTime ValidTo
    {
      get
      {
        return DateTime.MaxValue;
      }
    }

    public EncryptedKey EncryptedKey
    {
      get
      {
        return this.encryptedKey;
      }
      set
      {
        this.encryptedKey = value;
      }
    }

    public ReferenceList ReferenceList
    {
      get
      {
        if (this.encryptedKey != null)
          return this.encryptedKey.ReferenceList;
        return (ReferenceList) null;
      }
    }

    /// <summary>Gets the wrapping algorithm.</summary>
    /// <returns>A <see cref="T:System.String" /> that specifies the wrapping algorithm or the algorithm used to encrypt the symmetric key.</returns>
    public string WrappingAlgorithm
    {
      get
      {
        return this.wrappingAlgorithm;
      }
    }

    internal SecurityKey WrappingSecurityKey
    {
      get
      {
        return this.wrappingSecurityKey;
      }
    }

    /// <summary>Gets the wrapping token.</summary>
    /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityToken" /> that represents the wrapping token.</returns>
    public SecurityToken WrappingToken
    {
      get
      {
        return this.wrappingToken;
      }
    }

    /// <summary>Gets the wrapping token reference.</summary>
    /// <returns>A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifier" /> that represents a reference to the wrapping token.</returns>
    public SecurityKeyIdentifier WrappingTokenReference
    {
      get
      {
        return this.wrappingTokenReference;
      }
    }

    internal string CarriedKeyName
    {
      get
      {
        return (string) null;
      }
    }

    /// <summary>Gets a collection of security keys.</summary>
    /// <returns>A collection of <see cref="T:System.IdentityModel.Tokens.SecurityKey" />.</returns>
    public override ReadOnlyCollection<SecurityKey> SecurityKeys
    {
      get
      {
        return this.securityKey;
      }
    }

    public WrappedKeySecurityToken(string id, byte[] keyToWrap, ISspiNegotiation wrappingSspiContext)
      : this(id, keyToWrap, wrappingSspiContext != null ? wrappingSspiContext.KeyEncryptionAlgorithm : (string) null, wrappingSspiContext, (byte[]) null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.WrappedKeySecurityToken" /> class.  </summary>
    /// <param name="id">The ID of the key token.</param>
    /// <param name="keyToWrap">The key to be wrapped.</param>
    /// <param name="wrappingAlgorithm">The algorithm used to do the wrapping.</param>
    /// <param name="wrappingToken">A <see cref="T:System.IdentityModel.Tokens.SecurityToken" /> that represents the wrapping token.</param>
    /// <param name="wrappingTokenReference">A <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifier" /> that represents a reference to the wrapping token.</param>
    public WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, SecurityToken wrappingToken, SecurityKeyIdentifier wrappingTokenReference)
      : this(id, keyToWrap, wrappingAlgorithm, (XmlDictionaryString) null, wrappingToken, wrappingTokenReference)
    {
    }

    public WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, XmlDictionaryString wrappingAlgorithmDictionaryString, SecurityToken wrappingToken, SecurityKeyIdentifier wrappingTokenReference)
      : this(id, keyToWrap, wrappingAlgorithm, wrappingAlgorithmDictionaryString, wrappingToken, wrappingTokenReference, (byte[]) null, (SecurityKey) null)
    {
    }

    public WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, ISspiNegotiation wrappingSspiContext, byte[] wrappedKey)
      : this(id, keyToWrap, wrappingAlgorithm, (XmlDictionaryString) null)
    {
      if (wrappingSspiContext == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("wrappingSspiContext");
      this.wrappingSspiContext = wrappingSspiContext;
      this.wrappedKey = wrappedKey != null ? wrappedKey : wrappingSspiContext.Encrypt(keyToWrap);
      this.serializeCarriedKeyName = false;
    }

    public WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, SecurityToken wrappingToken, SecurityKeyIdentifier wrappingTokenReference, byte[] wrappedKey, SecurityKey wrappingSecurityKey)
      : this(id, keyToWrap, wrappingAlgorithm, (XmlDictionaryString) null, wrappingToken, wrappingTokenReference, wrappedKey, wrappingSecurityKey)
    {
    }

    public WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, XmlDictionaryString wrappingAlgorithmDictionaryString, SecurityToken wrappingToken, SecurityKeyIdentifier wrappingTokenReference, byte[] wrappedKey, SecurityKey wrappingSecurityKey)
      : this(id, keyToWrap, wrappingAlgorithm, wrappingAlgorithmDictionaryString)
    {
      if (wrappingToken == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("wrappingToken");
      this.wrappingToken = wrappingToken;
      this.wrappingTokenReference = wrappingTokenReference;
      this.wrappedKey = wrappedKey != null ? wrappedKey : System.IdentityModel.SecurityUtils.EncryptKey(wrappingToken, wrappingAlgorithm, keyToWrap);
      this.wrappingSecurityKey = wrappingSecurityKey;
      this.serializeCarriedKeyName = true;
    }

    private WrappedKeySecurityToken(string id, byte[] keyToWrap, string wrappingAlgorithm, XmlDictionaryString wrappingAlgorithmDictionaryString)
    {
      if (id == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("id");
      if (wrappingAlgorithm == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("wrappingAlgorithm");
      if (keyToWrap == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("securityKeyToWrap");
      this.id = id;
      this.effectiveTime = DateTime.UtcNow;
      this.securityKey = System.IdentityModel.SecurityUtils.CreateSymmetricSecurityKeys(keyToWrap);
      this.wrappingAlgorithm = wrappingAlgorithm;
      this.wrappingAlgorithmDictionaryString = wrappingAlgorithmDictionaryString;
    }

    internal byte[] GetHash()
    {
      if (this.wrappedKeyHash == null)
      {
        this.EnsureEncryptedKeySetUp();
        using (HashAlgorithm hashAlgorithm = CryptoHelper.NewSha1HashAlgorithm())
          this.wrappedKeyHash = hashAlgorithm.ComputeHash(this.encryptedKey.GetWrappedKey());
      }
      return this.wrappedKeyHash;
    }

    /// <summary>Gets the wrapped key.</summary>
    /// <returns>The wrapped key.</returns>
    public byte[] GetWrappedKey()
    {
      return System.IdentityModel.SecurityUtils.CloneBuffer(this.wrappedKey);
    }

    public void EnsureEncryptedKeySetUp()
    {
      if (this.encryptedKey != null)
        return;
      EncryptedKey encryptedKey = new EncryptedKey();
      encryptedKey.Id = this.Id;
      encryptedKey.CarriedKeyName = !this.serializeCarriedKeyName ? (string) null : this.CarriedKeyName;
      encryptedKey.EncryptionMethod = this.WrappingAlgorithm;
      encryptedKey.EncryptionMethodDictionaryString = this.wrappingAlgorithmDictionaryString;
      encryptedKey.SetUpKeyWrap(this.wrappedKey);
      if (this.WrappingTokenReference != null)
        encryptedKey.KeyIdentifier = this.WrappingTokenReference;
      this.encryptedKey = encryptedKey;
    }

    /// <summary>Gets a value that indicates whether the token can create a key identifier clause.</summary>
    /// <typeparam name="T">The type of the <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" />.</typeparam>
    /// <returns>true if the token can create a key identifier clause; otherwise, false. The default is false.</returns>
    public override bool CanCreateKeyIdentifierClause<T>()
    {
      if (typeof (T) == typeof (EncryptedKeyHashIdentifierClause))
        return true;
      return base.CanCreateKeyIdentifierClause<T>();
    }

    /// <summary>Create a key identifier clause.</summary>
    /// <typeparam name="T">The type of the <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" />.</typeparam>
    /// <returns>The type of the <see cref="T:System.IdentityModel.Tokens.SecurityKeyIdentifierClause" />.</returns>
    public override T CreateKeyIdentifierClause<T>()
    {
      if (typeof (T) == typeof (EncryptedKeyHashIdentifierClause))
        return new EncryptedKeyHashIdentifierClause(this.GetHash()) as T;
      return base.CreateKeyIdentifierClause<T>();
    }

    /// <summary>Compares the current security key identifier clause to a specified one for equality.</summary>
    /// <param name="keyIdentifierClause">The specified security key identifier clause.</param>
    /// <returns>true if the current security key identifier clause equals the specified one; otherwise, false. The default is false.</returns>
    public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
    {
      EncryptedKeyHashIdentifierClause identifierClause = keyIdentifierClause as EncryptedKeyHashIdentifierClause;
      if (identifierClause != null)
        return identifierClause.Matches(this.GetHash());
      return base.MatchesKeyIdentifierClause(keyIdentifierClause);
    }
  }
}
