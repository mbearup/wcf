// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.EncryptedKey
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.IdentityModel;
using System.Runtime.CompilerServices;
using System.Xml;

namespace System.ServiceModel.Security
{
  [TypeForwardedFrom("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
  public sealed class EncryptedKey : EncryptedType
  {
    internal static readonly XmlDictionaryString CarriedKeyElementName = System.IdentityModel.XD.XmlEncryptionDictionary.CarriedKeyName;
    public static readonly XmlDictionaryString ElementName = System.IdentityModel.XD.XmlEncryptionDictionary.EncryptedKey;
    internal static readonly XmlDictionaryString RecipientAttribute = System.IdentityModel.XD.XmlEncryptionDictionary.Recipient;
    private string carriedKeyName;
    private string recipient;
    private ReferenceList referenceList;
    private byte[] wrappedKey;

    public string CarriedKeyName
    {
      get
      {
        return this.carriedKeyName;
      }
      set
      {
        this.carriedKeyName = value;
      }
    }

    public string Recipient
    {
      get
      {
        return this.recipient;
      }
      set
      {
        this.recipient = value;
      }
    }

    public ReferenceList ReferenceList
    {
      get
      {
        return this.referenceList;
      }
      set
      {
        this.referenceList = value;
      }
    }

    protected override XmlDictionaryString OpeningElementName
    {
      get
      {
        return EncryptedKey.ElementName;
      }
    }

    protected override void ForceEncryption()
    {
    }

    public byte[] GetWrappedKey()
    {
      if (this.State == EncryptedType.EncryptionState.New)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("BadEncryptionState")));
      return this.wrappedKey;
    }

    public void SetUpKeyWrap(byte[] wrappedKey)
    {
      if (this.State != EncryptedType.EncryptionState.New)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("BadEncryptionState")));
      if (wrappedKey == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("wrappedKey");
      this.wrappedKey = wrappedKey;
      this.State = EncryptedType.EncryptionState.Encrypted;
    }

    protected override void ReadAdditionalAttributes(XmlDictionaryReader reader)
    {
      this.recipient = reader.GetAttribute(EncryptedKey.RecipientAttribute, (XmlDictionaryString) null);
    }

    protected override void ReadAdditionalElements(XmlDictionaryReader reader)
    {
      if (reader.IsStartElement(ReferenceList.ElementName, EncryptedType.NamespaceUri))
      {
        this.referenceList = new ReferenceList();
        this.referenceList.ReadFrom(reader);
      }
      if (!reader.IsStartElement(EncryptedKey.CarriedKeyElementName, EncryptedType.NamespaceUri))
        return;
      reader.ReadStartElement(EncryptedKey.CarriedKeyElementName, EncryptedType.NamespaceUri);
      this.carriedKeyName = reader.ReadString();
      reader.ReadEndElement();
    }

    protected override void ReadCipherData(XmlDictionaryReader reader)
    {
      this.wrappedKey = reader.ReadContentAsBase64();
    }

    protected override void ReadCipherData(XmlDictionaryReader reader, long maxBufferSize)
    {
      this.wrappedKey = System.IdentityModel.SecurityUtils.ReadContentAsBase64(reader, maxBufferSize);
    }

    protected override void WriteAdditionalAttributes(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
      if (this.recipient == null)
        return;
      writer.WriteAttributeString(EncryptedKey.RecipientAttribute, (XmlDictionaryString) null, this.recipient);
    }

    protected override void WriteAdditionalElements(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
      if (this.carriedKeyName != null)
      {
        writer.WriteStartElement(EncryptedKey.CarriedKeyElementName, EncryptedType.NamespaceUri);
        writer.WriteString(this.carriedKeyName);
        writer.WriteEndElement();
      }
      if (this.referenceList == null)
        return;
      this.referenceList.WriteTo(writer, dictionaryManager);
    }

    protected override void WriteCipherData(XmlDictionaryWriter writer)
    {
      writer.WriteBase64(this.wrappedKey, 0, this.wrappedKey.Length);
    }
  }
}

