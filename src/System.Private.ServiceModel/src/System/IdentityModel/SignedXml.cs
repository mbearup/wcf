// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.SignedXml
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Xml;

namespace System.IdentityModel
{
  internal sealed class SignedXml : ISignatureValueSecurityElement, ISecurityElement
  {
    internal const string DefaultPrefix = "";
    private SecurityTokenSerializer tokenSerializer;
    private readonly Signature signature;
    private TransformFactory transformFactory;
    private DictionaryManager dictionaryManager;

    public bool HasId
    {
      get
      {
        return true;
      }
    }

    public string Id
    {
      get
      {
        return this.signature.Id;
      }
      set
      {
        this.signature.Id = value;
      }
    }

    public SecurityTokenSerializer SecurityTokenSerializer
    {
      get
      {
        return this.tokenSerializer;
      }
    }

    public Signature Signature
    {
      get
      {
        return this.signature;
      }
    }

    public TransformFactory TransformFactory
    {
      get
      {
        return this.transformFactory;
      }
      set
      {
        this.transformFactory = value;
      }
    }

    public SignedXml(DictionaryManager dictionaryManager, SecurityTokenSerializer tokenSerializer)
      : this((SignedInfo) new StandardSignedInfo(dictionaryManager), dictionaryManager, tokenSerializer)
    {
    }

    internal SignedXml(SignedInfo signedInfo, DictionaryManager dictionaryManager, SecurityTokenSerializer tokenSerializer)
    {
      if (signedInfo == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("signedInfo"));
      if (dictionaryManager == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("dictionaryManager");
      if (tokenSerializer == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenSerializer");
      this.transformFactory = (TransformFactory) StandardTransformFactory.Instance;
      this.tokenSerializer = tokenSerializer;
      this.signature = new Signature(this, signedInfo);
      this.dictionaryManager = dictionaryManager;
    }

    private void ComputeSignature(HashAlgorithm hash, AsymmetricSignatureFormatter formatter, string signatureMethod)
    {
      this.Signature.SignedInfo.ComputeReferenceDigests();
      this.Signature.SignedInfo.ComputeHash(hash);
      byte[] signature;
      if (SecurityUtils.RequiresFipsCompliance && signatureMethod == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256")
      {
        formatter.SetHashAlgorithm("SHA256");
        signature = formatter.CreateSignature(hash.Hash);
      }
      else
        signature = formatter.CreateSignature(hash);
      this.Signature.SetSignatureValue(signature);
    }

    private void ComputeSignature(KeyedHashAlgorithm hash)
    {
      this.Signature.SignedInfo.ComputeReferenceDigests();
      this.Signature.SignedInfo.ComputeHash((HashAlgorithm) hash);
      this.Signature.SetSignatureValue(hash.Hash);
    }

    public void ComputeSignature(SecurityKey signingKey)
    {
      string signatureMethod = this.Signature.SignedInfo.SignatureMethod;
      SymmetricSecurityKey symmetricSecurityKey = signingKey as SymmetricSecurityKey;
      if (symmetricSecurityKey != null)
      {
        using (KeyedHashAlgorithm keyedHashAlgorithm = symmetricSecurityKey.GetKeyedHashAlgorithm(signatureMethod))
        {
          if (keyedHashAlgorithm == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UnableToCreateKeyedHashAlgorithm", (object) symmetricSecurityKey, (object) signatureMethod)));
          this.ComputeSignature(keyedHashAlgorithm);
        }
      }
      else
      {
        AsymmetricSecurityKey asymmetricSecurityKey = signingKey as AsymmetricSecurityKey;
        if (asymmetricSecurityKey == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UnknownICryptoType", new object[1]{ (object) signingKey })));
        using (HashAlgorithm algorithmForSignature = asymmetricSecurityKey.GetHashAlgorithmForSignature(signatureMethod))
        {
          if (algorithmForSignature == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UnableToCreateHashAlgorithmFromAsymmetricCrypto", (object) signatureMethod, (object) asymmetricSecurityKey)));
          AsymmetricSignatureFormatter signatureFormatter = asymmetricSecurityKey.GetSignatureFormatter(signatureMethod);
          if (signatureFormatter == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UnableToCreateSignatureFormatterFromAsymmetricCrypto", (object) signatureMethod, (object) asymmetricSecurityKey)));
          this.ComputeSignature(algorithmForSignature, signatureFormatter, signatureMethod);
        }
      }
    }

    public void CompleteSignatureVerification()
    {
      this.Signature.SignedInfo.EnsureAllReferencesVerified();
    }

    public void EnsureDigestValidity(string id, object resolvedXmlSource)
    {
      this.Signature.SignedInfo.EnsureDigestValidity(id, resolvedXmlSource);
    }

    public bool EnsureDigestValidityIfIdMatches(string id, object resolvedXmlSource)
    {
      return this.Signature.SignedInfo.EnsureDigestValidityIfIdMatches(id, resolvedXmlSource);
    }

    public byte[] GetSignatureValue()
    {
      return this.Signature.GetSignatureBytes();
    }

    public void ReadFrom(XmlReader reader)
    {
      this.ReadFrom(XmlDictionaryReader.CreateDictionaryReader(reader));
    }

    public void ReadFrom(XmlDictionaryReader reader)
    {
      this.signature.ReadFrom(reader, this.dictionaryManager);
    }

    private void VerifySignature(KeyedHashAlgorithm hash)
    {
      this.Signature.SignedInfo.ComputeHash((HashAlgorithm) hash);
      if (!CryptoHelper.IsEqual(hash.Hash, this.GetSignatureValue()))
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("SignatureVerificationFailed")));
    }

    private void VerifySignature(HashAlgorithm hash, AsymmetricSignatureDeformatter deformatter, string signatureMethod)
    {
      this.Signature.SignedInfo.ComputeHash(hash);
      bool flag;
#if FEATURE_CORECLR
      throw new NotImplementedException("SecurityUtils.RequiresFipsCompliance is not supported in .NET Core");
#else
      if (SecurityUtils.RequiresFipsCompliance && signatureMethod == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256")
      {
        deformatter.SetHashAlgorithm("SHA256");
        flag = deformatter.VerifySignature(hash.Hash, this.GetSignatureValue());
      }
      else
        flag = deformatter.VerifySignature(hash, this.GetSignatureValue());
      if (!flag)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("SignatureVerificationFailed")));
#endif
    }

    public void StartSignatureVerification(SecurityKey verificationKey)
    {
      string signatureMethod = this.Signature.SignedInfo.SignatureMethod;
      SymmetricSecurityKey symmetricSecurityKey = verificationKey as SymmetricSecurityKey;
      if (symmetricSecurityKey != null)
      {
        using (KeyedHashAlgorithm keyedHashAlgorithm = symmetricSecurityKey.GetKeyedHashAlgorithm(signatureMethod))
        {
          if (keyedHashAlgorithm == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("UnableToCreateKeyedHashAlgorithmFromSymmetricCrypto", (object) signatureMethod, (object) symmetricSecurityKey)));
          this.VerifySignature(keyedHashAlgorithm);
        }
      }
      else
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("AsymmetricSecurityKey is not supported in .NET Core");
#else
        AsymmetricSecurityKey asymmetricSecurityKey = verificationKey as AsymmetricSecurityKey;
        if (asymmetricSecurityKey == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UnknownICryptoType", new object[1]{ (object) verificationKey })));
        using (HashAlgorithm algorithmForSignature = asymmetricSecurityKey.GetHashAlgorithmForSignature(signatureMethod))
        {
          if (algorithmForSignature == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("UnableToCreateHashAlgorithmFromAsymmetricCrypto", (object) signatureMethod, (object) asymmetricSecurityKey)));
          AsymmetricSignatureDeformatter signatureDeformatter = asymmetricSecurityKey.GetSignatureDeformatter(signatureMethod);
          if (signatureDeformatter == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("UnableToCreateSignatureDeformatterFromAsymmetricCrypto", (object) signatureMethod, (object) asymmetricSecurityKey)));
          this.VerifySignature(algorithmForSignature, signatureDeformatter, signatureMethod);
        }
#endif
      }
    }

    public void WriteTo(XmlDictionaryWriter writer)
    {
      this.WriteTo(writer, this.dictionaryManager);
    }

    public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
      this.signature.WriteTo(writer, dictionaryManager);
    }
  }
}
