// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.Tokens.X509AsymmetricSecurityKey
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.ServiceModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
// using System.Security.Cryptography.Xml;

namespace System.IdentityModel.Tokens
{
  /// <summary>Represents an asymmetric key for X.509 certificates.</summary>
  public class X509AsymmetricSecurityKey : AsymmetricSecurityKey
  {
    private object thisLock = new object();
    private X509Certificate2 certificate;
    private AsymmetricAlgorithm privateKey;
    private bool privateKeyAvailabilityDetermined;
    private PublicKey publicKey;

    /// <summary>Gets the size, in bits, of the public key associated with the X.509 certificate.</summary>
    /// <returns>The size, in bits, of the public key associated with the X.509 certificate.</returns>
    public override int KeySize
    {
      get
      {
        return this.PublicKey.Key.KeySize;
      }
    }

    private AsymmetricAlgorithm PrivateKey
    {
      get
      {
        if (!this.privateKeyAvailabilityDetermined)
        {
          lock (this.ThisLock)
          {
            if (!this.privateKeyAvailabilityDetermined)
            {
              this.privateKey = this.certificate.PrivateKey;
              this.privateKeyAvailabilityDetermined = true;
            }
          }
        }
        return this.privateKey;
      }
    }

    private PublicKey PublicKey
    {
      get
      {
        if (this.publicKey == null)
        {
          lock (this.ThisLock)
          {
            if (this.publicKey == null)
              this.publicKey = this.certificate.PublicKey;
          }
        }
        return this.publicKey;
      }
    }

    private object ThisLock
    {
      get
      {
        return this.thisLock;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.IdentityModel.Tokens.X509AsymmetricSecurityKey" /> class using the specified X.509 certificate. </summary>
    /// <param name="certificate">The <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> that represents the X.509 certificate.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="certificate" /> is null.</exception>
    public X509AsymmetricSecurityKey(X509Certificate2 certificate)
    {
      if (certificate == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("certificate");
      this.certificate = certificate;
    }

    /// <summary>Decrypts the specified encrypted key using the specified cryptographic algorithm.</summary>
    /// <param name="algorithm">The cryptographic algorithm to decrypt the key.</param>
    /// <param name="keyData">An array of <see cref="T:System.Byte" /> that contains the encrypted key.</param>
    /// <returns>An array of <see cref="T:System.Byte" /> that contains the decrypted key.</returns>
    /// <exception cref="T:System.NotSupportedException">The X.509 certificate specified in the constructor does not have a private key.-or-The X.509 certificate has a private key, but it was not generated using the <see cref="T:System.Security.Cryptography.RSA" /> algorithm.-or-The X.509 certificate has a private key, it was generated using the <see cref="T:System.Security.Cryptography.RSA" /> algorithm, but the <see cref="P:System.Security.Cryptography.AsymmetricAlgorithm.KeyExchangeAlgorithm" /> property is null.-or-The <paramref name="algorithm" /> parameter is not supported. The supported algorithms are <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSA15Url" /> and <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSAOAEPUrl" />.</exception>
    public override byte[] DecryptKey(string algorithm, byte[] keyData)
    {
      if (this.PrivateKey == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("MissingPrivateKey")));
      RSA privateKey = this.PrivateKey as RSA;
      if (privateKey == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("PrivateKeyNotRSA")));
      if (privateKey.KeyExchangeAlgorithm == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("PrivateKeyExchangeNotSupported")));
#if FEATURE_CORECLR
      throw new NotImplementedException("EncryptedXml is not supported in .NET Core");
#else
      if (algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-1_5")
        return EncryptedXml.DecryptKey(keyData, privateKey, false);
      if (algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p" || this.IsSupportedAlgorithm(algorithm))
        return EncryptedXml.DecryptKey(keyData, privateKey, true);
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnsupportedCryptoAlgorithm", new object[1]{ (object) algorithm })));
#endif
    }

    /// <summary>Encrypts the specified encrypted key using the specified cryptographic algorithm.</summary>
    /// <param name="algorithm">The cryptographic algorithm to encrypt the key.</param>
    /// <param name="keyData">An array of <see cref="T:System.Byte" /> that contains the key to encrypt.</param>
    /// <returns>An array of <see cref="T:System.Byte" /> that contains the encrypted key.</returns>
    /// <exception cref="T:System.NotSupportedException">The X.509 certificate specified in the constructor has a public key that was not generated using the <see cref="T:System.Security.Cryptography.RSA" /> algorithm.-or-The <paramref name="algorithm" /> parameter is not supported. The supported algorithms are <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSA15Url" /> and <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSAOAEPUrl" />.</exception>
    public override byte[] EncryptKey(string algorithm, byte[] keyData)
    {
      RSA key = this.PublicKey.Key as RSA;
      if (key == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("PublicKeyNotRSA")));
#if FEATURE_CORECLR
      throw new NotImplementedException("EncryptedXml is not supported in .NET Core");
#else
      if (algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-1_5")
        return EncryptedXml.EncryptKey(keyData, key, false);
      if (algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p" || this.IsSupportedAlgorithm(algorithm))
        return EncryptedXml.EncryptKey(keyData, key, true);
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnsupportedCryptoAlgorithm", new object[1]{ (object) algorithm })));
#endif
    }

    /// <summary>Gets the specified asymmetric cryptographic algorithm.</summary>
    /// <param name="algorithm">The asymmetric algorithm to create.</param>
    /// <param name="privateKey">true when a private key is required to create the algorithm; otherwise, false. </param>
    /// <returns>An <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> that represents the specified asymmetric cryptographic algorithm.</returns>
    /// <exception cref="T:System.NotSupportedException">
    /// <paramref name="privateKey" /> is true and the X.509 certificate specified in the constructor does not have a private key.-or-<paramref name="algorithm" /> is <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigDSAUrl" /> and the public or private key for the X.509 certificate specified in the constructor is not of type <see cref="T:System.Security.Cryptography.DSA" />. -or-<paramref name="algorithm" /> is <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSA15Url" />, <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSAOAEPUrl" />, <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url" /> or <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" /> and the public or private key for the X.509 certificate specified in the constructor is not of type <see cref="T:System.Security.Cryptography.RSA" />. -or-<paramref name="algorithm" /> is not supported. The supported algorithms are <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigDSAUrl" />, <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSA15Url" />, <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSAOAEPUrl" />, <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url" />, and <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" />.</exception>
    public override AsymmetricAlgorithm GetAsymmetricAlgorithm(string algorithm, bool privateKey)
    {
      if (privateKey)
      {
        if (this.PrivateKey == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("MissingPrivateKey")));
        if (string.IsNullOrEmpty(algorithm))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(algorithm, SR.GetString("EmptyOrNullArgumentString", new object[1]{ (object) "algorithm" }));
        if (!(algorithm == "http://www.w3.org/2000/09/xmldsig#dsa-sha1"))
        {
          if (algorithm == "http://www.w3.org/2000/09/xmldsig#rsa-sha1" || algorithm == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256" || (algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-1_5" || algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p"))
          {
            if (this.PrivateKey is RSA)
              return (AsymmetricAlgorithm) (this.PrivateKey as RSA);
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("AlgorithmAndPrivateKeyMisMatch")));
          }
          if (this.IsSupportedAlgorithm(algorithm))
            return this.PrivateKey;
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnsupportedCryptoAlgorithm", new object[1]{ (object) algorithm })));
        }
        if (this.PrivateKey is DSA)
          return (AsymmetricAlgorithm) (this.PrivateKey as DSA);
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("AlgorithmAndPrivateKeyMisMatch")));
      }
      if (!(algorithm == "http://www.w3.org/2000/09/xmldsig#dsa-sha1"))
      {
        if (algorithm == "http://www.w3.org/2000/09/xmldsig#rsa-sha1" || algorithm == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256" || (algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-1_5" || algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p"))
        {
          if (this.PublicKey.Key is RSA)
            return (AsymmetricAlgorithm) (this.PublicKey.Key as RSA);
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("AlgorithmAndPublicKeyMisMatch")));
        }
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnsupportedCryptoAlgorithm", new object[1]{ (object) algorithm })));
      }
      if (this.PublicKey.Key is DSA)
        return (AsymmetricAlgorithm) (this.PublicKey.Key as DSA);
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("AlgorithmAndPublicKeyMisMatch")));
    }

    /// <summary>Gets a cryptographic algorithm that generates a hash for a digital signature.</summary>
    /// <param name="algorithm">The hash algorithm.</param>
    /// <returns>A <see cref="T:System.Security.Cryptography.HashAlgorithm" /> that generates hashes for digital signatures.</returns>
    /// <exception cref="T:System.NotSupportedException">
    /// <paramref name="algorithm" /> is not supported. The supported algorithms are <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigDSAUrl" />, <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url" />, and <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" />.</exception>
    public override HashAlgorithm GetHashAlgorithmForSignature(string algorithm)
    {
      if (string.IsNullOrEmpty(algorithm))
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(algorithm, SR.GetString("EmptyOrNullArgumentString", new object[1]{ (object) "algorithm" }));
      object algorithmFromConfig = CryptoHelper.GetAlgorithmFromConfig(algorithm);
      if (algorithmFromConfig != null)
      {
        SignatureDescription signatureDescription = algorithmFromConfig as SignatureDescription;
        if (signatureDescription != null)
          return signatureDescription.CreateDigest();
        HashAlgorithm hashAlgorithm = algorithmFromConfig as HashAlgorithm;
        if (hashAlgorithm != null)
          return hashAlgorithm;
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("UnsupportedAlgorithmForCryptoOperation", (object) algorithm, (object) "CreateDigest")));
      }
      if (algorithm == "http://www.w3.org/2000/09/xmldsig#dsa-sha1" || algorithm == "http://www.w3.org/2000/09/xmldsig#rsa-sha1")
        return CryptoHelper.NewSha1HashAlgorithm();
      if (algorithm == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256")
        return CryptoHelper.NewSha256HashAlgorithm();
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnsupportedCryptoAlgorithm", new object[1]{ (object) algorithm })));
    }

    /// <summary>Gets the de-formatter algorithm for the digital signature.</summary>
    /// <param name="algorithm">The de-formatter algorithm for the digital signature to get an instance of.</param>
    /// <returns>An <see cref="T:System.Security.Cryptography.AsymmetricSignatureDeformatter" /> that represents the de-formatter algorithm for the digital signature.</returns>
    /// <exception cref="T:System.NotSupportedException">
    /// <paramref name="algorithm" /> is <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigDSAUrl" /> and the public key for the X.509 certificate specified in the constructor is not of type <see cref="T:System.Security.Cryptography.DSA" />.-or-<paramref name="algorithm" /> is <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url" /> or <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" /> and the public key for the X.509 certificate specified in the constructor is not of type <see cref="T:System.Security.Cryptography.RSA" />.-or-<paramref name="algorithm" /> is not supported. The supported algorithms are <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigDSAUrl" />,<see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url" />, and <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" />.</exception>
    public override AsymmetricSignatureDeformatter GetSignatureDeformatter(string algorithm)
    {
      if (string.IsNullOrEmpty(algorithm))
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(algorithm, SR.GetString("EmptyOrNullArgumentString", new object[1]{ (object) "algorithm" }));
#if FEATURE_CORECLR
      throw new NotImplementedException("CryptoHelper.GetAlgorithmFromConfig is not supported in .NET Core");
#else
      object algorithmFromConfig = CryptoHelper.GetAlgorithmFromConfig(algorithm);
      if (algorithmFromConfig != null)
      {
        SignatureDescription signatureDescription = algorithmFromConfig as SignatureDescription;
        if (signatureDescription != null)
          return signatureDescription.CreateDeformatter(this.PublicKey.Key);
        try
        {
          AsymmetricSignatureDeformatter signatureDeformatter = algorithmFromConfig as AsymmetricSignatureDeformatter;
          if (signatureDeformatter != null)
          {
            signatureDeformatter.SetKey(this.PublicKey.Key);
            return signatureDeformatter;
          }
        }
        catch (InvalidCastException ex)
        {
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("AlgorithmAndPublicKeyMisMatch"), (Exception) ex));
        }
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("UnsupportedAlgorithmForCryptoOperation", (object) algorithm, (object) "GetSignatureDeformatter")));
      }
      if (!(algorithm == "http://www.w3.org/2000/09/xmldsig#dsa-sha1"))
      {
        if (algorithm == "http://www.w3.org/2000/09/xmldsig#rsa-sha1" || algorithm == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256")
        {
          RSA key = this.PublicKey.Key as RSA;
          if (key == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("PublicKeyNotRSA")));
          return (AsymmetricSignatureDeformatter) new RSAPKCS1SignatureDeformatter((AsymmetricAlgorithm) key);
        }
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnsupportedCryptoAlgorithm", new object[1]{ (object) algorithm })));
      }
      DSA key1 = this.PublicKey.Key as DSA;
      if (key1 == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("PublicKeyNotDSA")));
      return (AsymmetricSignatureDeformatter) new DSASignatureDeformatter((AsymmetricAlgorithm) key1);
#endif
    }

    /// <summary>Gets the formatter algorithm for the digital signature.</summary>
    /// <param name="algorithm">The formatter algorithm for the digital signature to get an instance of.</param>
    /// <returns>An <see cref="T:System.Security.Cryptography.AsymmetricSignatureDeformatter" /> that represents the formatter algorithm for the digital signature.</returns>
    /// <exception cref="T:System.NotSupportedException">The X.509 certificate specified in the constructor does not have a private key.-or-<paramref name="algorithm" /> is <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigDSAUrl" /> and the private key for the X.509 certificate specified in the constructor is not of type <see cref="T:System.Security.Cryptography.DSA" />.-or-<paramref name="algorithm" /> is <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url" /> or <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" /> and the private key for the X.509 certificate specified in the constructor is not of type <see cref="T:System.Security.Cryptography.RSA" />.-or-<paramref name="algorithm" /> is not supported. The supported algorithms are <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigDSAUrl" />,<see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url" />, and <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" />.</exception>
    public override AsymmetricSignatureFormatter GetSignatureFormatter(string algorithm)
    {
      if (this.PrivateKey == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("MissingPrivateKey")));
      if (string.IsNullOrEmpty(algorithm))
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(algorithm, SR.GetString("EmptyOrNullArgumentString", new object[1]{ (object) "algorithm" }));
      AsymmetricAlgorithm key = X509AsymmetricSecurityKey.LevelUpRsa(this.PrivateKey, algorithm);
      object algorithmFromConfig = CryptoHelper.GetAlgorithmFromConfig(algorithm);
      if (algorithmFromConfig != null)
      {
        SignatureDescription signatureDescription = algorithmFromConfig as SignatureDescription;
        if (signatureDescription != null)
          return signatureDescription.CreateFormatter(key);
        try
        {
          AsymmetricSignatureFormatter signatureFormatter = algorithmFromConfig as AsymmetricSignatureFormatter;
          if (signatureFormatter != null)
          {
            signatureFormatter.SetKey(key);
            return signatureFormatter;
          }
        }
        catch (InvalidCastException ex)
        {
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("AlgorithmAndPrivateKeyMisMatch"), (Exception) ex));
        }
#if FEATURE_CORECLR
        Console.WriteLine("Would normally throw an unsupported error - continuing... {0}", algorithm);
#else
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("UnsupportedAlgorithmForCryptoOperation", (object) algorithm, (object) "GetSignatureFormatter")));
#endif
      }
      if (!(algorithm == "http://www.w3.org/2000/09/xmldsig#dsa-sha1"))
      {
        if (!(algorithm == "http://www.w3.org/2000/09/xmldsig#rsa-sha1"))
        {
          if (algorithm == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256")
          {
            RSA rsa = key as RSA;
            if (rsa == null)
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("PrivateKeyNotRSA")));
            return (AsymmetricSignatureFormatter) new RSAPKCS1SignatureFormatter((AsymmetricAlgorithm) rsa);
          }
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnsupportedCryptoAlgorithm", new object[1]{ (object) algorithm })));
        }
        RSA privateKey = this.PrivateKey as RSA;
        if (privateKey == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("PrivateKeyNotRSA")));
        return (AsymmetricSignatureFormatter) new RSAPKCS1SignatureFormatter((AsymmetricAlgorithm) privateKey);
      }
      DSA privateKey1 = this.PrivateKey as DSA;
      if (privateKey1 == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("PrivateKeyNotDSA")));
#if FEATURE_CORECLR
      throw new NotSupportedException("DSASignatureFormatter uses a broken Cryptographic Algorithm");
#else
      return (AsymmetricSignatureFormatter) new DSASignatureFormatter((AsymmetricAlgorithm) privateKey1);
#endif
    }

    private static AsymmetricAlgorithm LevelUpRsa(AsymmetricAlgorithm asymmetricAlgorithm, string algorithm)
    {
#if FEATURE_CORECLR
      Console.WriteLine("TODO - skipping LocalAppContextSwitches.DisableUpdatingRsaProviderType");
#else
      if (System.IdentityModel.LocalAppContextSwitches.DisableUpdatingRsaProviderType)
        return asymmetricAlgorithm;
#endif
      if (asymmetricAlgorithm == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("asymmetricAlgorithm"));
      if (string.IsNullOrEmpty(algorithm))
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(algorithm, SR.GetString("EmptyOrNullArgumentString", new object[1]{ (object) "algorithm" }));
      if (!string.Equals(algorithm, "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"))
        return asymmetricAlgorithm;
      RSACryptoServiceProvider cryptoServiceProvider = asymmetricAlgorithm as RSACryptoServiceProvider;
      if (cryptoServiceProvider == null)
        return asymmetricAlgorithm;
      if (cryptoServiceProvider.CspKeyContainerInfo.ProviderType != 1 && cryptoServiceProvider.CspKeyContainerInfo.ProviderType != 12 || cryptoServiceProvider.CspKeyContainerInfo.HardwareDevice)
        return (AsymmetricAlgorithm) cryptoServiceProvider;
      CspParameters parameters = new CspParameters();
      parameters.ProviderType = 24;
      parameters.KeyContainerName = cryptoServiceProvider.CspKeyContainerInfo.KeyContainerName;
      parameters.KeyNumber = (int) cryptoServiceProvider.CspKeyContainerInfo.KeyNumber;
      if (cryptoServiceProvider.CspKeyContainerInfo.MachineKeyStore)
        parameters.Flags = CspProviderFlags.UseMachineKeyStore;
      parameters.Flags |= CspProviderFlags.UseExistingKey;
      return (AsymmetricAlgorithm) new RSACryptoServiceProvider(parameters);
    }

    /// <summary>Gets a value that indicates whether the private key is a available. </summary>
    /// <returns>true when the private key is available; otherwise, false.</returns>
    public override bool HasPrivateKey()
    {
      return this.PrivateKey != null;
    }

    /// <summary>Gets a value that indicates whether the specified algorithm uses asymmetric keys.</summary>
    /// <param name="algorithm">The cryptographic algorithm.</param>
    /// <returns>true when the specified algorithm is <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.DsaSha1Signature" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha1Signature" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaOaepKeyWrap" />, or <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaV15KeyWrap" />; otherwise, false.</returns>
    public override bool IsAsymmetricAlgorithm(string algorithm)
    {
      if (string.IsNullOrEmpty(algorithm))
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(algorithm, SR.GetString("EmptyOrNullArgumentString", new object[1]{ (object) "algorithm" }));
      return CryptoHelper.IsAsymmetricAlgorithm(algorithm);
    }

    /// <summary>Gets a value that indicates whether the specified algorithm is supported by this class. </summary>
    /// <param name="algorithm">The cryptographic algorithm.</param>
    /// <returns>true when the specified algorithm is <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigDSAUrl" />, <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSA15Url" />, <see cref="F:System.Security.Cryptography.Xml.EncryptedXml.XmlEncRSAOAEPUrl" />, <see cref="F:System.Security.Cryptography.Xml.SignedXml.XmlDsigRSASHA1Url" />, or <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature" /> and the public key is of the right type; otherwise, false. See the remarks for details.</returns>
    public override bool IsSupportedAlgorithm(string algorithm)
    {
      if (string.IsNullOrEmpty(algorithm))
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(algorithm, SR.GetString("EmptyOrNullArgumentString", new object[1]{ (object) "algorithm" }));
      object obj = (object) null;
      try
      {
        obj = CryptoHelper.GetAlgorithmFromConfig(algorithm);
        
      }
      catch (InvalidOperationException ex)
      {
        algorithm = (string) null;
      }
      if (obj != null)
      {
        if (obj is SignatureDescription || obj is AsymmetricAlgorithm)
        {
            return true;
        }
      }
      if (algorithm == "http://www.w3.org/2000/09/xmldsig#dsa-sha1")
        return this.PublicKey.Key is DSA;
      if (algorithm == "http://www.w3.org/2000/09/xmldsig#rsa-sha1" || algorithm == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256" || (algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-1_5" || algorithm == "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p"))
      {
          if (this.PublicKey.Key is RSA)
          {
              return true;
          }
      }
      return false;
    }

    /// <summary>Gets a value that indicates whether the specified algorithm uses symmetric keys.</summary>
    /// <param name="algorithm">The cryptographic algorithm.</param>
    /// <returns>true when the specified algorithm is <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.HmacSha1Signature" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes128Encryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes192Encryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes256Encryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.TripleDesEncryption" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes128KeyWrap" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes192KeyWrap" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Aes256KeyWrap" />, <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.TripleDesKeyWrap" />, or <see cref="F:System.IdentityModel.Tokens.SecurityAlgorithms.Psha1KeyDerivation" />; otherwise, false.</returns>
    public override bool IsSymmetricAlgorithm(string algorithm)
    {
      return CryptoHelper.IsSymmetricAlgorithm(algorithm);
    }
  }
}
