// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.EncryptedType
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.IdentityModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Xml;

namespace System.ServiceModel.Security
{
  [TypeForwardedFrom("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
  public abstract class EncryptedType : ISecurityElement
  {
    public static readonly XmlDictionaryString NamespaceUri = System.IdentityModel.XD.XmlEncryptionDictionary.Namespace;
    internal static readonly XmlDictionaryString EncodingAttribute = System.IdentityModel.XD.XmlEncryptionDictionary.Encoding;
    internal static readonly XmlDictionaryString MimeTypeAttribute = System.IdentityModel.XD.XmlEncryptionDictionary.MimeType;
    internal static readonly XmlDictionaryString TypeAttribute = System.IdentityModel.XD.XmlEncryptionDictionary.Type;
    internal static readonly XmlDictionaryString CipherDataElementName = System.IdentityModel.XD.XmlEncryptionDictionary.CipherData;
    internal static readonly XmlDictionaryString CipherValueElementName = System.IdentityModel.XD.XmlEncryptionDictionary.CipherValue;
    private string encoding;
    private EncryptedType.EncryptionMethodElement encryptionMethod;
    private string id;
    private string wsuId;
    private SecurityKeyIdentifier keyIdentifier;
    private string mimeType;
    private EncryptedType.EncryptionState state;
    private string type;
    private SecurityTokenSerializer tokenSerializer;
    private bool shouldReadXmlReferenceKeyInfoClause;

    public string Encoding
    {
      get
      {
        return this.encoding;
      }
      set
      {
        this.encoding = value;
      }
    }

    public string EncryptionMethod
    {
      get
      {
        return this.encryptionMethod.algorithm;
      }
      set
      {
        this.encryptionMethod.algorithm = value;
      }
    }

    public XmlDictionaryString EncryptionMethodDictionaryString
    {
      get
      {
        return this.encryptionMethod.algorithmDictionaryString;
      }
      set
      {
        this.encryptionMethod.algorithmDictionaryString = value;
      }
    }

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

    public bool ShouldReadXmlReferenceKeyInfoClause
    {
      get
      {
        return this.shouldReadXmlReferenceKeyInfoClause;
      }
      set
      {
        this.shouldReadXmlReferenceKeyInfoClause = value;
      }
    }

    public string WsuId
    {
      get
      {
        return this.wsuId;
      }
      set
      {
        this.wsuId = value;
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

    public string MimeType
    {
      get
      {
        return this.mimeType;
      }
      set
      {
        this.mimeType = value;
      }
    }

    public string Type
    {
      get
      {
        return this.type;
      }
      set
      {
        this.type = value;
      }
    }

    protected abstract XmlDictionaryString OpeningElementName { get; }

    protected EncryptedType.EncryptionState State
    {
      get
      {
        return this.state;
      }
      set
      {
        this.state = value;
      }
    }

    public SecurityTokenSerializer SecurityTokenSerializer
    {
      get
      {
        return this.tokenSerializer;
      }
      set
      {
#if FEATURE_CORECLR
      throw new NotImplementedException("KeyInfoSerializer not supported in .NET Core");
#else
        this.tokenSerializer = value ?? (SecurityTokenSerializer) new KeyInfoSerializer(false);
#endif
      }
    }

    protected EncryptedType()
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("KeyInfoSerializer not supported in .NET Core");
#else
      this.encryptionMethod.Init();
      this.state = EncryptedType.EncryptionState.New;
      this.tokenSerializer = (SecurityTokenSerializer) new KeyInfoSerializer(false);
#endif
    }

    protected abstract void ForceEncryption();

    protected virtual void ReadAdditionalAttributes(XmlDictionaryReader reader)
    {
    }

    protected virtual void ReadAdditionalElements(XmlDictionaryReader reader)
    {
    }

    protected abstract void ReadCipherData(XmlDictionaryReader reader);

    protected abstract void ReadCipherData(XmlDictionaryReader reader, long maxBufferSize);

    public void ReadFrom(XmlReader reader)
    {
      this.ReadFrom(reader, 0L);
    }

    public void ReadFrom(XmlDictionaryReader reader)
    {
      this.ReadFrom(reader, 0L);
    }

    public void ReadFrom(XmlReader reader, long maxBufferSize)
    {
      this.ReadFrom(XmlDictionaryReader.CreateDictionaryReader(reader), maxBufferSize);
    }

    public void ReadFrom(XmlDictionaryReader reader, long maxBufferSize)
    {
      this.ValidateReadState();
      reader.MoveToStartElement(this.OpeningElementName, EncryptedType.NamespaceUri);
      this.encoding = reader.GetAttribute(EncryptedType.EncodingAttribute, (XmlDictionaryString) null);
      this.id = reader.GetAttribute(System.IdentityModel.XD.XmlEncryptionDictionary.Id, (XmlDictionaryString) null) ?? SecurityUniqueId.Create().Value;
      this.wsuId = reader.GetAttribute(System.IdentityModel.XD.XmlEncryptionDictionary.Id, System.IdentityModel.XD.UtilityDictionary.Namespace) ?? SecurityUniqueId.Create().Value;
      this.mimeType = reader.GetAttribute(EncryptedType.MimeTypeAttribute, (XmlDictionaryString) null);
      this.type = reader.GetAttribute(EncryptedType.TypeAttribute, (XmlDictionaryString) null);
      this.ReadAdditionalAttributes(reader);
      reader.Read();
      if (reader.IsStartElement(EncryptedType.EncryptionMethodElement.ElementName, EncryptedType.NamespaceUri))
        this.encryptionMethod.ReadFrom(reader);
      if (this.tokenSerializer.CanReadKeyIdentifier((XmlReader) reader))
      {
        XmlElement xmlElement = (XmlElement) null;
        XmlDictionaryReader dictionaryReader;
        if (this.ShouldReadXmlReferenceKeyInfoClause)
        {
          xmlElement = new XmlDocument().ReadNode((XmlReader) reader) as XmlElement;
          dictionaryReader = XmlDictionaryReader.CreateDictionaryReader((XmlReader) new XmlNodeReader((XmlNode) xmlElement));
        }
        else
          dictionaryReader = reader;
        try
        {
          this.KeyIdentifier = this.tokenSerializer.ReadKeyIdentifier((XmlReader) dictionaryReader);
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex) || !this.ShouldReadXmlReferenceKeyInfoClause)
            throw;
          else
            this.keyIdentifier = this.ReadGenericXmlSecurityKeyIdentifier(XmlDictionaryReader.CreateDictionaryReader((XmlReader) new XmlNodeReader((XmlNode) xmlElement)), ex);
        }
      }
      reader.ReadStartElement(EncryptedType.CipherDataElementName, EncryptedType.NamespaceUri);
      reader.ReadStartElement(EncryptedType.CipherValueElementName, EncryptedType.NamespaceUri);
      if (maxBufferSize == 0L)
        this.ReadCipherData(reader);
      else
        this.ReadCipherData(reader, maxBufferSize);
      reader.ReadEndElement();
      reader.ReadEndElement();
      this.ReadAdditionalElements(reader);
      reader.ReadEndElement();
      this.State = EncryptedType.EncryptionState.Read;
    }

    private SecurityKeyIdentifier ReadGenericXmlSecurityKeyIdentifier(XmlDictionaryReader localReader, Exception previousException)
    {
      if (!localReader.IsStartElement(System.IdentityModel.XD.XmlSignatureDictionary.KeyInfo, System.IdentityModel.XD.XmlSignatureDictionary.Namespace))
        return (SecurityKeyIdentifier) null;
      localReader.ReadStartElement(System.IdentityModel.XD.XmlSignatureDictionary.KeyInfo, System.IdentityModel.XD.XmlSignatureDictionary.Namespace);
      SecurityKeyIdentifier securityKeyIdentifier = new SecurityKeyIdentifier();
      if (localReader.IsStartElement())
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("GenericXmlSecurityKeyIdentifierClause not supported");
#else
        string attribute = localReader.GetAttribute(System.IdentityModel.XD.UtilityDictionary.IdAttribute, System.IdentityModel.XD.UtilityDictionary.Namespace);
        SecurityKeyIdentifierClause clause = (SecurityKeyIdentifierClause) new GenericXmlSecurityKeyIdentifierClause(new XmlDocument().ReadNode((XmlReader) localReader) as XmlElement);
        if (!string.IsNullOrEmpty(attribute))
          clause.Id = attribute;
        securityKeyIdentifier.Add(clause);
#endif
      }
      if (securityKeyIdentifier.Count == 0)
        throw previousException;
      localReader.ReadEndElement();
      return securityKeyIdentifier;
    }

    protected virtual void WriteAdditionalAttributes(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
    }

    protected virtual void WriteAdditionalElements(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
    }

    protected abstract void WriteCipherData(XmlDictionaryWriter writer);

    public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
      if (writer == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
      this.ValidateWriteState();
      writer.WriteStartElement("e", this.OpeningElementName, EncryptedType.NamespaceUri);
      if (this.id != null && this.id.Length != 0)
        writer.WriteAttributeString(System.IdentityModel.XD.XmlEncryptionDictionary.Id, (XmlDictionaryString) null, this.Id);
      if (this.type != null)
        writer.WriteAttributeString(EncryptedType.TypeAttribute, (XmlDictionaryString) null, this.Type);
      if (this.mimeType != null)
        writer.WriteAttributeString(EncryptedType.MimeTypeAttribute, (XmlDictionaryString) null, this.MimeType);
      if (this.encoding != null)
        writer.WriteAttributeString(EncryptedType.EncodingAttribute, (XmlDictionaryString) null, this.Encoding);
      this.WriteAdditionalAttributes(writer, dictionaryManager);
      if (this.encryptionMethod.algorithm != null)
        this.encryptionMethod.WriteTo(writer);
      if (this.KeyIdentifier != null)
        this.tokenSerializer.WriteKeyIdentifier((XmlWriter) writer, this.KeyIdentifier);
      writer.WriteStartElement(EncryptedType.CipherDataElementName, EncryptedType.NamespaceUri);
      writer.WriteStartElement(EncryptedType.CipherValueElementName, EncryptedType.NamespaceUri);
      this.WriteCipherData(writer);
      writer.WriteEndElement();
      writer.WriteEndElement();
      this.WriteAdditionalElements(writer, dictionaryManager);
      writer.WriteEndElement();
    }

    private void ValidateReadState()
    {
      if (this.State != EncryptedType.EncryptionState.New)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new SecurityMessageSerializationException(SR.GetString("BadEncryptionState")));
    }

    private void ValidateWriteState()
    {
      if (this.State == EncryptedType.EncryptionState.EncryptionSetup)
        this.ForceEncryption();
      else if (this.State == EncryptedType.EncryptionState.New)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new SecurityMessageSerializationException(SR.GetString("BadEncryptionState")));
    }

    protected enum EncryptionState
    {
      New,
      Read,
      DecryptionSetup,
      Decrypted,
      EncryptionSetup,
      Encrypted,
    }

    private struct EncryptionMethodElement
    {
      internal static readonly XmlDictionaryString ElementName = System.IdentityModel.XD.XmlEncryptionDictionary.EncryptionMethod;
      internal string algorithm;
      internal XmlDictionaryString algorithmDictionaryString;

      public void Init()
      {
        this.algorithm = (string) null;
      }

      public void ReadFrom(XmlDictionaryReader reader)
      {
        reader.MoveToStartElement(EncryptedType.EncryptionMethodElement.ElementName, System.IdentityModel.XD.XmlEncryptionDictionary.Namespace);
        bool isEmptyElement = reader.IsEmptyElement;
        this.algorithm = reader.GetAttribute(System.IdentityModel.XD.XmlSignatureDictionary.Algorithm, (XmlDictionaryString) null);
        if (this.algorithm == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new SecurityMessageSerializationException(SR.GetString("RequiredAttributeMissing", (object) System.IdentityModel.XD.XmlSignatureDictionary.Algorithm.Value, (object) EncryptedType.EncryptionMethodElement.ElementName.Value)));
        reader.Read();
        if (isEmptyElement)
          return;
        while (reader.IsStartElement())
          reader.Skip();
        reader.ReadEndElement();
      }

      public void WriteTo(XmlDictionaryWriter writer)
      {
        writer.WriteStartElement("e", EncryptedType.EncryptionMethodElement.ElementName, System.IdentityModel.XD.XmlEncryptionDictionary.Namespace);
        if (this.algorithmDictionaryString != null)
        {
          writer.WriteStartAttribute(System.IdentityModel.XD.XmlSignatureDictionary.Algorithm, (XmlDictionaryString) null);
          writer.WriteString(this.algorithmDictionaryString);
          writer.WriteEndAttribute();
        }
        else
          writer.WriteAttributeString(System.IdentityModel.XD.XmlSignatureDictionary.Algorithm, (XmlDictionaryString) null, this.algorithm);
        if (this.algorithm == System.IdentityModel.XD.SecurityAlgorithmDictionary.RsaOaepKeyWrap.Value)
        {
          writer.WriteStartElement("", System.IdentityModel.XD.XmlSignatureDictionary.DigestMethod, System.IdentityModel.XD.XmlSignatureDictionary.Namespace);
          writer.WriteStartAttribute(System.IdentityModel.XD.XmlSignatureDictionary.Algorithm, (XmlDictionaryString) null);
          writer.WriteString(System.IdentityModel.XD.SecurityAlgorithmDictionary.Sha1Digest);
          writer.WriteEndAttribute();
          writer.WriteEndElement();
        }
        writer.WriteEndElement();
      }
    }
  }
}

