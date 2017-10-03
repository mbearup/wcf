// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.PreDigestedSignedInfo
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.IO;
using System.ServiceModel;
using System.Text;
using System.Xml;

namespace System.IdentityModel
{
  internal sealed class PreDigestedSignedInfo : SignedInfo
  {
    private const int InitialReferenceArraySize = 8;
    private bool addEnvelopedSignatureTransform;
    private int count;
    private string digestMethod;
    private XmlDictionaryString digestMethodDictionaryString;
    private PreDigestedSignedInfo.ReferenceEntry[] references;

    public bool AddEnvelopedSignatureTransform
    {
      get
      {
        return this.addEnvelopedSignatureTransform;
      }
      set
      {
        this.addEnvelopedSignatureTransform = value;
      }
    }

    public string DigestMethod
    {
      get
      {
        return this.digestMethod;
      }
      set
      {
        this.digestMethod = value;
      }
    }

    public override int ReferenceCount
    {
      get
      {
        return this.count;
      }
    }

    public PreDigestedSignedInfo(DictionaryManager dictionaryManager)
      : base(dictionaryManager)
    {
      this.references = new PreDigestedSignedInfo.ReferenceEntry[8];
    }

    public PreDigestedSignedInfo(DictionaryManager dictionaryManager, string canonicalizationMethod, XmlDictionaryString canonicalizationMethodDictionaryString, string digestMethod, XmlDictionaryString digestMethodDictionaryString, string signatureMethod, XmlDictionaryString signatureMethodDictionaryString)
      : base(dictionaryManager)
    {
      this.references = new PreDigestedSignedInfo.ReferenceEntry[8];
      this.CanonicalizationMethod = canonicalizationMethod;
      this.CanonicalizationMethodDictionaryString = canonicalizationMethodDictionaryString;
      this.DigestMethod = digestMethod;
      this.digestMethodDictionaryString = digestMethodDictionaryString;
      this.SignatureMethod = signatureMethod;
      this.SignatureMethodDictionaryString = signatureMethodDictionaryString;
    }

    public void AddReference(string id, byte[] digest)
    {
      this.AddReference(id, digest, false);
    }

    public void AddReference(string id, byte[] digest, bool useStrTransform)
    {
      if (this.count == this.references.Length)
      {
        PreDigestedSignedInfo.ReferenceEntry[] referenceEntryArray = new PreDigestedSignedInfo.ReferenceEntry[this.references.Length * 2];
        Array.Copy((Array) this.references, 0, (Array) referenceEntryArray, 0, this.count);
        this.references = referenceEntryArray;
      }
      PreDigestedSignedInfo.ReferenceEntry[] references = this.references;
      int count = this.count;
      this.count = count + 1;
      int index = count;
      references[index].Set(id, digest, useStrTransform);
    }

    protected override void ComputeHash(HashStream hashStream)
    {
      if (this.AddEnvelopedSignatureTransform)
        base.ComputeHash(hashStream);
      else
      {
        PreDigestedSignedInfo.SignedInfoCanonicalFormWriter.Instance.WriteSignedInfoCanonicalForm((Stream) hashStream, this.SignatureMethod, this.DigestMethod, this.references, this.count, this.ResourcePool.TakeEncodingBuffer(), this.ResourcePool.TakeBase64Buffer());
      }
    }

    public override void ComputeReferenceDigests()
    {
    }

    public override void ReadFrom(XmlDictionaryReader reader, TransformFactory transformFactory, DictionaryManager dictionaryManager)
    {
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
    }

    public override void EnsureAllReferencesVerified()
    {
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
    }

    public override bool EnsureDigestValidityIfIdMatches(string id, object resolvedXmlSource)
    {
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
    }

    public override void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
      string prefix = "";
      XmlDictionaryString namespaceUri = dictionaryManager.XmlSignatureDictionary.Namespace;
      writer.WriteStartElement(prefix, dictionaryManager.XmlSignatureDictionary.SignedInfo, namespaceUri);
      if (this.Id != null)
        writer.WriteAttributeString(dictionaryManager.UtilityDictionary.IdAttribute, (XmlDictionaryString) null, this.Id);
      this.WriteCanonicalizationMethod(writer, dictionaryManager);
      this.WriteSignatureMethod(writer, dictionaryManager);
      for (int index = 0; index < this.count; ++index)
      {
        writer.WriteStartElement(prefix, dictionaryManager.XmlSignatureDictionary.Reference, namespaceUri);
        writer.WriteStartAttribute(dictionaryManager.XmlSignatureDictionary.URI, (XmlDictionaryString) null);
        writer.WriteString("#");
        writer.WriteString(this.references[index].id);
        writer.WriteEndAttribute();
        writer.WriteStartElement(prefix, dictionaryManager.XmlSignatureDictionary.Transforms, namespaceUri);
        if (this.addEnvelopedSignatureTransform)
        {
          writer.WriteStartElement(prefix, dictionaryManager.XmlSignatureDictionary.Transform, namespaceUri);
          writer.WriteStartAttribute(dictionaryManager.XmlSignatureDictionary.Algorithm, (XmlDictionaryString) null);
          writer.WriteString(dictionaryManager.XmlSignatureDictionary.EnvelopedSignature);
          writer.WriteEndAttribute();
          writer.WriteEndElement();
        }
        if (this.references[index].useStrTransform)
        {
          writer.WriteStartElement(prefix, dictionaryManager.XmlSignatureDictionary.Transform, namespaceUri);
          writer.WriteStartAttribute(dictionaryManager.XmlSignatureDictionary.Algorithm, (XmlDictionaryString) null);
          writer.WriteString("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#STR-Transform");
          writer.WriteEndAttribute();
          writer.WriteStartElement("o", "TransformationParameters", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
          writer.WriteStartElement(prefix, dictionaryManager.XmlSignatureDictionary.CanonicalizationMethod, namespaceUri);
          writer.WriteStartAttribute(dictionaryManager.XmlSignatureDictionary.Algorithm, (XmlDictionaryString) null);
          writer.WriteString(dictionaryManager.SecurityAlgorithmDictionary.ExclusiveC14n);
          writer.WriteEndAttribute();
          writer.WriteEndElement();
          writer.WriteEndElement();
          writer.WriteEndElement();
        }
        else
        {
          writer.WriteStartElement(prefix, dictionaryManager.XmlSignatureDictionary.Transform, namespaceUri);
          writer.WriteStartAttribute(dictionaryManager.XmlSignatureDictionary.Algorithm, (XmlDictionaryString) null);
          writer.WriteString(dictionaryManager.SecurityAlgorithmDictionary.ExclusiveC14n);
          writer.WriteEndAttribute();
          writer.WriteEndElement();
        }
        writer.WriteEndElement();
        writer.WriteStartElement(prefix, dictionaryManager.XmlSignatureDictionary.DigestMethod, namespaceUri);
        writer.WriteStartAttribute(dictionaryManager.XmlSignatureDictionary.Algorithm, (XmlDictionaryString) null);
        if (this.digestMethodDictionaryString != null)
          writer.WriteString(this.digestMethodDictionaryString);
        else
          writer.WriteString(this.digestMethod);
        writer.WriteEndAttribute();
        writer.WriteEndElement();
        byte[] digest = this.references[index].digest;
        writer.WriteStartElement(prefix, dictionaryManager.XmlSignatureDictionary.DigestValue, namespaceUri);
        writer.WriteBase64(digest, 0, digest.Length);
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }

    private struct ReferenceEntry
    {
      internal string id;
      internal byte[] digest;
      internal bool useStrTransform;

      public void Set(string id, byte[] digest, bool useStrTransform)
      {
        if (useStrTransform && string.IsNullOrEmpty(id))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException(id));
        this.id = id;
        this.digest = digest;
        this.useStrTransform = useStrTransform;
      }
    }

    private sealed class SignedInfoCanonicalFormWriter : CanonicalFormWriter
    {
      private static readonly PreDigestedSignedInfo.SignedInfoCanonicalFormWriter instance = new PreDigestedSignedInfo.SignedInfoCanonicalFormWriter();
      private const string xml1 = "<SignedInfo xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><CanonicalizationMethod Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\"></CanonicalizationMethod><SignatureMethod Algorithm=\"";
      private const string xml2 = "\"></SignatureMethod>";
      private const string xml3 = "<Reference URI=\"#";
      private const string xml4 = "\"><Transforms><Transform Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\"></Transform></Transforms><DigestMethod Algorithm=\"";
      private const string xml4WithStrTransform = "\"><Transforms><Transform Algorithm=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#STR-Transform\"><o:TransformationParameters xmlns:o=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\"><CanonicalizationMethod Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\"></CanonicalizationMethod></o:TransformationParameters></Transform></Transforms><DigestMethod Algorithm=\"";
      private const string xml5 = "\"></DigestMethod><DigestValue>";
      private const string xml6 = "</DigestValue></Reference>";
      private const string xml7 = "</SignedInfo>";
      private readonly byte[] fragment1;
      private readonly byte[] fragment2;
      private readonly byte[] fragment3;
      private readonly byte[] fragment4;
      private readonly byte[] fragment4StrTransform;
      private readonly byte[] fragment5;
      private readonly byte[] fragment6;
      private readonly byte[] fragment7;
      private readonly byte[] sha1Digest;
      private readonly byte[] sha256Digest;
      private readonly byte[] hmacSha1Signature;
      private readonly byte[] rsaSha1Signature;

      public static PreDigestedSignedInfo.SignedInfoCanonicalFormWriter Instance
      {
        get
        {
          return PreDigestedSignedInfo.SignedInfoCanonicalFormWriter.instance;
        }
      }

      private SignedInfoCanonicalFormWriter()
      {
        UTF8Encoding utf8WithoutPreamble = (UTF8Encoding) CanonicalFormWriter.Utf8WithoutPreamble;
        this.fragment1 = utf8WithoutPreamble.GetBytes("<SignedInfo xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><CanonicalizationMethod Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\"></CanonicalizationMethod><SignatureMethod Algorithm=\"");
        this.fragment2 = utf8WithoutPreamble.GetBytes("\"></SignatureMethod>");
        this.fragment3 = utf8WithoutPreamble.GetBytes("<Reference URI=\"#");
        this.fragment4 = utf8WithoutPreamble.GetBytes("\"><Transforms><Transform Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\"></Transform></Transforms><DigestMethod Algorithm=\"");
        this.fragment4StrTransform = utf8WithoutPreamble.GetBytes("\"><Transforms><Transform Algorithm=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#STR-Transform\"><o:TransformationParameters xmlns:o=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\"><CanonicalizationMethod Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\"></CanonicalizationMethod></o:TransformationParameters></Transform></Transforms><DigestMethod Algorithm=\"");
        this.fragment5 = utf8WithoutPreamble.GetBytes("\"></DigestMethod><DigestValue>");
        this.fragment6 = utf8WithoutPreamble.GetBytes("</DigestValue></Reference>");
        this.fragment7 = utf8WithoutPreamble.GetBytes("</SignedInfo>");
        this.sha1Digest = utf8WithoutPreamble.GetBytes("http://www.w3.org/2000/09/xmldsig#sha1");
        this.sha256Digest = utf8WithoutPreamble.GetBytes("http://www.w3.org/2001/04/xmlenc#sha256");
        this.hmacSha1Signature = utf8WithoutPreamble.GetBytes("http://www.w3.org/2000/09/xmldsig#hmac-sha1");
        this.rsaSha1Signature = utf8WithoutPreamble.GetBytes("http://www.w3.org/2000/09/xmldsig#rsa-sha1");
      }

      private byte[] EncodeDigestAlgorithm(string algorithm)
      {
        if (algorithm == "http://www.w3.org/2000/09/xmldsig#sha1")
          return this.sha1Digest;
        if (algorithm == "http://www.w3.org/2001/04/xmlenc#sha256")
          return this.sha256Digest;
        return CanonicalFormWriter.Utf8WithoutPreamble.GetBytes(algorithm);
      }

      private byte[] EncodeSignatureAlgorithm(string algorithm)
      {
        if (algorithm == "http://www.w3.org/2000/09/xmldsig#hmac-sha1")
          return this.hmacSha1Signature;
        if (algorithm == "http://www.w3.org/2000/09/xmldsig#rsa-sha1")
          return this.rsaSha1Signature;
        return CanonicalFormWriter.Utf8WithoutPreamble.GetBytes(algorithm);
      }

      public void WriteSignedInfoCanonicalForm(Stream stream, string signatureMethod, string digestMethod, PreDigestedSignedInfo.ReferenceEntry[] references, int referenceCount, byte[] workBuffer, char[] base64WorkBuffer)
      {
        stream.Write(this.fragment1, 0, this.fragment1.Length);
        byte[] buffer1 = this.EncodeSignatureAlgorithm(signatureMethod);
        stream.Write(buffer1, 0, buffer1.Length);
        stream.Write(this.fragment2, 0, this.fragment2.Length);
        byte[] buffer2 = this.EncodeDigestAlgorithm(digestMethod);
        for (int index = 0; index < referenceCount; ++index)
        {
          stream.Write(this.fragment3, 0, this.fragment3.Length);
          CanonicalFormWriter.EncodeAndWrite(stream, workBuffer, references[index].id);
          if (references[index].useStrTransform)
            stream.Write(this.fragment4StrTransform, 0, this.fragment4StrTransform.Length);
          else
            stream.Write(this.fragment4, 0, this.fragment4.Length);
          stream.Write(buffer2, 0, buffer2.Length);
          stream.Write(this.fragment5, 0, this.fragment5.Length);

          CanonicalFormWriter.Base64EncodeAndWrite(stream, workBuffer, base64WorkBuffer, references[index].digest);
          stream.Write(this.fragment6, 0, this.fragment6.Length);
        }
        stream.Write(this.fragment7, 0, this.fragment7.Length);
      }
    }
  }
}
