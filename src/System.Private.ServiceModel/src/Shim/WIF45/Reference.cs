// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.Reference
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Security.Cryptography;
using System.ServiceModel;
using System.Xml;

namespace System.IdentityModel
{
  internal sealed class Reference
  {
    private string prefix = "";
    private readonly TransformChain transformChain = new TransformChain();
    private ElementWithAlgorithmAttribute digestMethodElement;
    private Reference.DigestValueElement digestValueElement;
    private string id;
    private object resolvedXmlSource;
    private string type;
    private string uri;
    private SignatureResourcePool resourcePool;
    private bool verified;
    private string referredId;
    private DictionaryManager dictionaryManager;

    public string DigestMethod
    {
      get
      {
        return this.digestMethodElement.Algorithm;
      }
      set
      {
        this.digestMethodElement.Algorithm = value;
      }
    }

    public XmlDictionaryString DigestMethodDictionaryString
    {
      get
      {
        return this.digestMethodElement.AlgorithmDictionaryString;
      }
      set
      {
        this.digestMethodElement.AlgorithmDictionaryString = value;
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

    public SignatureResourcePool ResourcePool
    {
      get
      {
        return this.resourcePool;
      }
      set
      {
        this.resourcePool = value;
      }
    }

    public TransformChain TransformChain
    {
      get
      {
        return this.transformChain;
      }
    }

    public int TransformCount
    {
      get
      {
        return this.transformChain.TransformCount;
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

    public string Uri
    {
      get
      {
        return this.uri;
      }
      set
      {
        this.uri = value;
      }
    }

    public bool Verified
    {
      get
      {
        return this.verified;
      }
    }

    public Reference(DictionaryManager dictionaryManager)
      : this(dictionaryManager, (string) null)
    {
    }

    public Reference(DictionaryManager dictionaryManager, string uri)
      : this(dictionaryManager, uri, (object) null)
    {
    }

    public Reference(DictionaryManager dictionaryManager, string uri, object resolvedXmlSource)
    {
      if (dictionaryManager == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("dictionaryManager");
      this.dictionaryManager = dictionaryManager;
      this.digestMethodElement = new ElementWithAlgorithmAttribute(dictionaryManager.XmlSignatureDictionary.DigestMethod);
      this.uri = uri;
      this.resolvedXmlSource = resolvedXmlSource;
    }

    public void AddTransform(Transform transform)
    {
      this.transformChain.Add(transform);
    }

    public void EnsureDigestValidity(string id, byte[] computedDigest)
    {
      if (!this.EnsureDigestValidityIfIdMatches(id, computedDigest))
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("RequiredTargetNotSigned", new object[1]{ (object) id })));
    }

    public void EnsureDigestValidity(string id, object resolvedXmlSource)
    {
      if (!this.EnsureDigestValidityIfIdMatches(id, resolvedXmlSource))
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("RequiredTargetNotSigned", new object[1]{ (object) id })));
    }

    public bool EnsureDigestValidityIfIdMatches(string id, byte[] computedDigest)
    {
      if (this.verified || id != this.ExtractReferredId())
        return false;
      if (!CryptoHelper.IsEqual(computedDigest, this.GetDigestValue()))
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("DigestVerificationFailedForReference", new object[1]{ (object) this.uri })));
      this.verified = true;
      return true;
    }

    public bool EnsureDigestValidityIfIdMatches(string id, object resolvedXmlSource)
    {
      if (this.verified || id != this.ExtractReferredId() && !this.IsStrTranform())
        return false;
      this.resolvedXmlSource = resolvedXmlSource;
      if (!this.CheckDigest())
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("DigestVerificationFailedForReference", new object[1]{ (object) this.uri })));
      this.verified = true;
      return true;
    }

    public bool IsStrTranform()
    {
      if (this.TransformChain.TransformCount == 1)
        return this.TransformChain[0].Algorithm == "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#STR-Transform";
      return false;
    }

    public string ExtractReferredId()
    {
      if (this.referredId == null)
      {
        if (StringComparer.OrdinalIgnoreCase.Equals(this.uri, string.Empty))
          return string.Empty;
        if (this.uri == null || this.uri.Length < 2 || (int) this.uri[0] != 35)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("UnableToResolveReferenceUriForSignature", new object[1]{ (object) this.uri })));
        this.referredId = this.uri.Substring(1);
      }
      return this.referredId;
    }

    private static bool ShouldPreserveComments(string uri)
    {
      bool flag = false;
      if (!string.IsNullOrEmpty(uri))
      {
        string str = uri.Substring(1);
        if (str == "xpointer(/)")
          flag = true;
        else if (str.StartsWith("xpointer(id(", StringComparison.Ordinal) && str.IndexOf(")", StringComparison.Ordinal) > 0)
          flag = true;
      }
      return flag;
    }

    public bool CheckDigest()
    {
      return CryptoHelper.IsEqual(this.ComputeDigest(), this.GetDigestValue());
    }

    public void ComputeAndSetDigest()
    {
      this.digestValueElement.Value = this.ComputeDigest();
    }

    public byte[] ComputeDigest()
    {
      if (this.transformChain.TransformCount == 0)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("EmptyTransformChainNotSupported")));
      if (this.resolvedXmlSource == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("UnableToResolveReferenceUriForSignature", new object[1]{ (object) this.uri })));
      return this.transformChain.TransformToDigest(this.resolvedXmlSource, this.ResourcePool, this.DigestMethod, this.dictionaryManager);
    }

    public byte[] GetDigestValue()
    {
      return this.digestValueElement.Value;
    }

    public void ReadFrom(XmlDictionaryReader reader, TransformFactory transformFactory, DictionaryManager dictionaryManager)
    {
      reader.MoveToStartElement(dictionaryManager.XmlSignatureDictionary.Reference, dictionaryManager.XmlSignatureDictionary.Namespace);
      this.prefix = reader.Prefix;
      this.Id = reader.GetAttribute("Id", (string) null);
      this.Uri = reader.GetAttribute(dictionaryManager.XmlSignatureDictionary.URI, (XmlDictionaryString) null);
      this.Type = reader.GetAttribute(dictionaryManager.XmlSignatureDictionary.Type, (XmlDictionaryString) null);
      reader.Read();
      if (reader.IsStartElement(dictionaryManager.XmlSignatureDictionary.Transforms, dictionaryManager.XmlSignatureDictionary.Namespace))
        this.transformChain.ReadFrom(reader, transformFactory, dictionaryManager, Reference.ShouldPreserveComments(this.Uri));
      this.digestMethodElement.ReadFrom(reader, dictionaryManager);
      this.digestValueElement.ReadFrom(reader, dictionaryManager);
      int content = (int) reader.MoveToContent();
      reader.ReadEndElement();
    }

    public void SetResolvedXmlSource(object resolvedXmlSource)
    {
      this.resolvedXmlSource = resolvedXmlSource;
    }

    public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
      writer.WriteStartElement(this.prefix, dictionaryManager.XmlSignatureDictionary.Reference, dictionaryManager.XmlSignatureDictionary.Namespace);
      if (this.id != null)
        writer.WriteAttributeString(dictionaryManager.UtilityDictionary.IdAttribute, (XmlDictionaryString) null, this.id);
      if (this.uri != null)
        writer.WriteAttributeString(dictionaryManager.XmlSignatureDictionary.URI, (XmlDictionaryString) null, this.uri);
      if (this.type != null)
        writer.WriteAttributeString(dictionaryManager.XmlSignatureDictionary.Type, (XmlDictionaryString) null, this.type);
      if (this.transformChain.TransformCount > 0)
        this.transformChain.WriteTo(writer, dictionaryManager);
      this.digestMethodElement.WriteTo(writer, dictionaryManager);
      this.digestValueElement.WriteTo(writer, dictionaryManager);
      writer.WriteEndElement();
    }

    private struct DigestValueElement
    {
      private byte[] digestValue;
      private string digestText;
      private string prefix;

      internal byte[] Value
      {
        get
        {
          return this.digestValue;
        }
        set
        {
          this.digestValue = value;
          this.digestText = (string) null;
        }
      }

      public void ReadFrom(XmlDictionaryReader reader, DictionaryManager dictionaryManager)
      {
        reader.MoveToStartElement(dictionaryManager.XmlSignatureDictionary.DigestValue, dictionaryManager.XmlSignatureDictionary.Namespace);
        this.prefix = reader.Prefix;
        reader.Read();
        int content1 = (int) reader.MoveToContent();
        this.digestText = reader.ReadString();
        this.digestValue = Convert.FromBase64String(this.digestText.Trim());
        int content2 = (int) reader.MoveToContent();
        reader.ReadEndElement();
      }

      public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
      {
        writer.WriteStartElement(this.prefix ?? "", dictionaryManager.XmlSignatureDictionary.DigestValue, dictionaryManager.XmlSignatureDictionary.Namespace);
        if (this.digestText != null)
          writer.WriteString(this.digestText);
        else
          writer.WriteBase64(this.digestValue, 0, this.digestValue.Length);
        writer.WriteEndElement();
      }
    }
  }
}
