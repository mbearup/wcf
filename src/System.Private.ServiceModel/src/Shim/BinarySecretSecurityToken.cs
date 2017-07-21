// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.Tokens.BinarySecretSecurityToken
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Collections.ObjectModel;
using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.Runtime.CompilerServices;

namespace System.ServiceModel.Security.Tokens
{
  /// <summary>Represents a binary secret security token.</summary>
  [TypeForwardedFrom("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
  public class BinarySecretSecurityToken : SecurityToken
  {
    private string id;
    private DateTime effectiveTime;
    private byte[] key;
    private ReadOnlyCollection<SecurityKey> securityKeys;

    /// <summary>Gets the token ID.</summary>
    /// <returns>The token ID.</returns>
    public override string Id
    {
      get
      {
        return this.id;
      }
    }

    /// <summary>Gets the token effective start time.</summary>
    /// <returns>A <see cref="T:System.DateTime" /> that represents the token effective start time. </returns>
    public override DateTime ValidFrom
    {
      get
      {
        return this.effectiveTime;
      }
    }

    /// <summary>Gets the token effective start time.</summary>
    /// <returns>A <see cref="T:System.DateTime" /> that represents the token effective start time.</returns>
    public override DateTime ValidTo
    {
      get
      {
        return DateTime.MaxValue;
      }
    }

    /// <summary>Gets the token key size.</summary>
    /// <returns>The token key size.</returns>
    public int KeySize
    {
      get
      {
        return this.key.Length * 8;
      }
    }

    /// <summary>Gets a collection of security keys.</summary>
    /// <returns>A collection of  <see cref="T:System.IdentityModel.Tokens.SecurityKey" />s.</returns>
    public override ReadOnlyCollection<SecurityKey> SecurityKeys
    {
      get
      {
        return this.securityKeys;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.BinarySecretSecurityToken" /> class.  </summary>
    /// <param name="keySizeInBits">The key size in bits.</param>
    public BinarySecretSecurityToken(int keySizeInBits)
      : this(SecurityUniqueId.Create().Value, keySizeInBits)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.BinarySecretSecurityToken" /> class.  </summary>
    /// <param name="id">The token ID.</param>
    /// <param name="keySizeInBits">The key size in bits.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Either <paramref name="keySizeInBits" /> is less than or equal to zero, or it is greater than or equal to 512, or it is not a multiple of 8.</exception>
    public BinarySecretSecurityToken(string id, int keySizeInBits)
      : this(id, keySizeInBits, true)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.BinarySecretSecurityToken" /> class.  </summary>
    /// <param name="key">A byte-array that represents the key.</param>
    public BinarySecretSecurityToken(byte[] key)
      : this(SecurityUniqueId.Create().Value, key)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.BinarySecretSecurityToken" /> class.  </summary>
    /// <param name="id">The token ID.</param>
    /// <param name="key">A byte-array that represents the key.</param>
    public BinarySecretSecurityToken(string id, byte[] key)
      : this(id, key, true)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.BinarySecretSecurityToken" /> class.  </summary>
    /// <param name="id">The token ID.</param>
    /// <param name="keySizeInBits">The key size in bits.</param>
    /// <param name="allowCrypto">A <see cref="T:System.Boolean" /> that indicates whether to allow cryptography.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Either <paramref name="keySizeInBits" /> is less than or equal to zero, or it is greater than or equal to 512, or it is not a multiple of 8.</exception>
    protected BinarySecretSecurityToken(string id, int keySizeInBits, bool allowCrypto)
    {
      if (keySizeInBits <= 0 || keySizeInBits >= 512)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("keySizeInBits", SR.GetString("ValueMustBeInRange", (object) 0, (object) 512)));
      if (keySizeInBits % 8 != 0)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("keySizeInBits", SR.GetString("KeyLengthMustBeMultipleOfEight", new object[1]{ (object) keySizeInBits })));
      this.id = id;
      this.effectiveTime = DateTime.UtcNow;
      this.key = new byte[keySizeInBits / 8];
      CryptoHelper.FillRandomBytes(this.key);
      if (allowCrypto)
        this.securityKeys = System.IdentityModel.SecurityUtils.CreateSymmetricSecurityKeys(this.key);
      else
        this.securityKeys = EmptyReadOnlyCollection<SecurityKey>.Instance;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Security.Tokens.BinarySecretSecurityToken" /> class.  </summary>
    /// <param name="id">The token ID.</param>
    /// <param name="key">A byte-array that represents the key.</param>
    /// <param name="allowCrypto">A <see cref="T:System.Boolean" /> that indicates whether to allow cryptography.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="key" /> is null.</exception>
    protected BinarySecretSecurityToken(string id, byte[] key, bool allowCrypto)
    {
      if (key == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("key");
      this.id = id;
      this.effectiveTime = DateTime.UtcNow;
      this.key = new byte[key.Length];
      Buffer.BlockCopy((Array) key, 0, (Array) this.key, 0, key.Length);
      if (allowCrypto)
        this.securityKeys = System.IdentityModel.SecurityUtils.CreateSymmetricSecurityKeys(this.key);
      else
        this.securityKeys = EmptyReadOnlyCollection<SecurityKey>.Instance;
    }

    /// <summary>Gets the bytes that represent the key.</summary>
    /// <returns>The key.</returns>
    public byte[] GetKeyBytes()
    {
      return System.IdentityModel.SecurityUtils.CloneBuffer(this.key);
    }
  }
}
