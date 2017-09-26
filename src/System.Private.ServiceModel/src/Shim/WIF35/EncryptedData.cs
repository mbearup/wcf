// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.EncryptedData
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Security.Cryptography;
using System.Xml;

namespace System.ServiceModel.Security
{
  internal class EncryptedData : EncryptedType
  {
    internal static readonly XmlDictionaryString ElementName = System.IdentityModel.XD.XmlEncryptionDictionary.EncryptedData;
    internal static readonly string ElementType = "http://www.w3.org/2001/04/xmlenc#Element";
    internal static readonly string ContentType = "http://www.w3.org/2001/04/xmlenc#Content";
    private SymmetricAlgorithm algorithm;
    private byte[] decryptedBuffer;
    private ArraySegment<byte> buffer;
    private byte[] iv;
    private byte[] cipherText;

    protected override XmlDictionaryString OpeningElementName
    {
      get
      {
        return EncryptedData.ElementName;
      }
    }

    private void EnsureDecryptionSet()
    {
      if (this.State == EncryptedType.EncryptionState.DecryptionSetup)
        this.SetPlainText();
      else if (this.State != EncryptedType.EncryptionState.Decrypted)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("BadEncryptionState")));
    }

    protected override void ForceEncryption()
    {
#if FEATURE_CORECLR
	  throw new NotImplementedException("CryptoHelper is not supported in .NET Core");
#else
      CryptoHelper.GenerateIVAndEncrypt(this.algorithm, this.buffer, out this.iv, out this.cipherText);
      this.State = EncryptedType.EncryptionState.Encrypted;
      this.buffer = new ArraySegment<byte>(CryptoHelper.EmptyBuffer);
#endif
    }

    public byte[] GetDecryptedBuffer()
    {
      this.EnsureDecryptionSet();
      return this.decryptedBuffer;
    }

    protected override void ReadCipherData(XmlDictionaryReader reader)
    {
      this.cipherText = reader.ReadContentAsBase64();
    }

    protected override void ReadCipherData(XmlDictionaryReader reader, long maxBufferSize)
    {
#if FEATURE_CORECLR
	  throw new NotImplementedException("SecurityUtils.ReadContentAsBase64 is not supported in .NET Core");
#else
      this.cipherText = SecurityUtils.ReadContentAsBase64(reader, maxBufferSize);
#endif
    }

    private void SetPlainText()
    {
#if FEATURE_CORECLR
	  throw new NotImplementedException("CryptoHelper is not supported in .NET Core");
#else
      this.decryptedBuffer = CryptoHelper.ExtractIVAndDecrypt(this.algorithm, this.cipherText, 0, this.cipherText.Length);
      this.State = EncryptedType.EncryptionState.Decrypted;
#endif
    }

    public void SetUpDecryption(SymmetricAlgorithm algorithm)
    {
      if (this.State != EncryptedType.EncryptionState.Read)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("BadEncryptionState")));
      if (algorithm == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("algorithm");
      this.algorithm = algorithm;
      this.State = EncryptedType.EncryptionState.DecryptionSetup;
    }

    public void SetUpEncryption(SymmetricAlgorithm algorithm, ArraySegment<byte> buffer)
    {
      if (this.State != EncryptedType.EncryptionState.New)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("BadEncryptionState")));
      if (algorithm == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("algorithm");
      this.algorithm = algorithm;
      this.buffer = buffer;
      this.State = EncryptedType.EncryptionState.EncryptionSetup;
    }

    protected override void WriteCipherData(XmlDictionaryWriter writer)
    {
      writer.WriteBase64(this.iv, 0, this.iv.Length);
      writer.WriteBase64(this.cipherText, 0, this.cipherText.Length);
    }
  }
}
