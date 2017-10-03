// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.Signature
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.IdentityModel.Tokens;
using System.Xml;

namespace System.IdentityModel
{
  internal sealed class Signature
  {
    private string prefix = "";
    private readonly Signature.SignatureValueElement signatureValueElement = new Signature.SignatureValueElement();
    private SignedXml signedXml;
    private string id;
    private SecurityKeyIdentifier keyIdentifier;
    private readonly SignedInfo signedInfo;

    public string Id
    {
      get
      {
        return this.id;
      }
      set
      {
        this.id = value;
      }
    }

    public SecurityKeyIdentifier KeyIdentifier
    {
      get
      {
        return this.keyIdentifier;
      }
      set
      {
        this.keyIdentifier = value;
      }
    }

    public SignedInfo SignedInfo
    {
      get
      {
        return this.signedInfo;
      }
    }

    public ISignatureValueSecurityElement SignatureValue
    {
      get
      {
        return (ISignatureValueSecurityElement) this.signatureValueElement;
      }
    }

    public Signature(SignedXml signedXml, SignedInfo signedInfo)
    {
      this.signedXml = signedXml;
      this.signedInfo = signedInfo;
    }

    public byte[] GetSignatureBytes()
    {
      return this.signatureValueElement.Value;
    }

    public void ReadFrom(XmlDictionaryReader reader, DictionaryManager dictionaryManager)
    {
      reader.MoveToStartElement(dictionaryManager.XmlSignatureDictionary.Signature, dictionaryManager.XmlSignatureDictionary.Namespace);
      this.prefix = reader.Prefix;
      this.Id = reader.GetAttribute(dictionaryManager.UtilityDictionary.IdAttribute, (XmlDictionaryString) null);
      reader.Read();
      this.signedInfo.ReadFrom(reader, this.signedXml.TransformFactory, dictionaryManager);
      this.signatureValueElement.ReadFrom(reader, dictionaryManager);
      if (this.signedXml.SecurityTokenSerializer.CanReadKeyIdentifier((XmlReader) reader))
        this.keyIdentifier = this.signedXml.SecurityTokenSerializer.ReadKeyIdentifier((XmlReader) reader);
      reader.ReadEndElement();
    }

    public void SetSignatureValue(byte[] signatureValue)
    {
      this.signatureValueElement.Value = signatureValue;
    }

    public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
      writer.WriteStartElement(this.prefix, dictionaryManager.XmlSignatureDictionary.Signature, dictionaryManager.XmlSignatureDictionary.Namespace);
      if (this.id != null)
        writer.WriteAttributeString(dictionaryManager.UtilityDictionary.IdAttribute, (XmlDictionaryString) null, this.id);
      this.signedInfo.WriteTo(writer, dictionaryManager);
      this.signatureValueElement.WriteTo(writer, dictionaryManager);
      if (this.keyIdentifier != null)
        this.signedXml.SecurityTokenSerializer.WriteKeyIdentifier((XmlWriter) writer, this.keyIdentifier);
      writer.WriteEndElement();
    }

    private sealed class SignatureValueElement : ISignatureValueSecurityElement, ISecurityElement
    {
      private string prefix = "";
      private string id;
      private byte[] signatureValue;
      private string signatureText;

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
          return this.id;
        }
        set
        {
          this.id = value;
        }
      }

      internal byte[] Value
      {
        get
        {
          return this.signatureValue;
        }
        set
        {
          this.signatureValue = value;
          this.signatureText = (string) null;
        }
      }

      public void ReadFrom(XmlDictionaryReader reader, DictionaryManager dictionaryManager)
      {
        reader.MoveToStartElement(dictionaryManager.XmlSignatureDictionary.SignatureValue, dictionaryManager.XmlSignatureDictionary.Namespace);
        this.prefix = reader.Prefix;
        this.Id = reader.GetAttribute("Id", (string) null);
        reader.Read();
        this.signatureText = reader.ReadString();
        this.signatureValue = Convert.FromBase64String(this.signatureText.Trim());
        reader.ReadEndElement();
      }

      public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
      {
        writer.WriteStartElement(this.prefix, dictionaryManager.XmlSignatureDictionary.SignatureValue, dictionaryManager.XmlSignatureDictionary.Namespace);
        if (this.id != null)
          writer.WriteAttributeString(dictionaryManager.UtilityDictionary.IdAttribute, (XmlDictionaryString) null, this.id);
        if (this.signatureText != null)
          writer.WriteString(this.signatureText);
        else
          writer.WriteBase64(this.signatureValue, 0, this.signatureValue.Length);
        writer.WriteEndElement();
      }

      byte[] ISignatureValueSecurityElement.GetSignatureValue()
      {
        return this.Value;
      }
    }
  }
}
