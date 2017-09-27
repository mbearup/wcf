// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.SignedInfo
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.IO;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Xml;

namespace System.IdentityModel
{
  internal abstract class SignedInfo : ISecurityElement
  {
    private readonly ExclusiveCanonicalizationTransform canonicalizationMethodElement = new ExclusiveCanonicalizationTransform(true);
    private bool sendSide = true;
    private string id;
    private ElementWithAlgorithmAttribute signatureMethodElement;
    private SignatureResourcePool resourcePool;
    private DictionaryManager dictionaryManager;
    private MemoryStream canonicalStream;
    private ISignatureReaderProvider readerProvider;
    private object signatureReaderProviderCallbackContext;

    protected DictionaryManager DictionaryManager
    {
      get
      {
        return this.dictionaryManager;
      }
    }

    protected MemoryStream CanonicalStream
    {
      get
      {
        return this.canonicalStream;
      }
      set
      {
        this.canonicalStream = value;
      }
    }

    protected bool SendSide
    {
      get
      {
        return this.sendSide;
      }
      set
      {
        this.sendSide = value;
      }
    }

    public ISignatureReaderProvider ReaderProvider
    {
      get
      {
        return this.readerProvider;
      }
      set
      {
        this.readerProvider = value;
      }
    }

    public object SignatureReaderProviderCallbackContext
    {
      get
      {
        return this.signatureReaderProviderCallbackContext;
      }
      set
      {
        this.signatureReaderProviderCallbackContext = value;
      }
    }

    public string CanonicalizationMethod
    {
      get
      {
        return this.canonicalizationMethodElement.Algorithm;
      }
      set
      {
        if (value != this.canonicalizationMethodElement.Algorithm)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnsupportedTransformAlgorithm")));
      }
    }

    public XmlDictionaryString CanonicalizationMethodDictionaryString
    {
      set
      {
        if (value != null && value.Value != this.canonicalizationMethodElement.Algorithm)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnsupportedTransformAlgorithm")));
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

    public abstract int ReferenceCount { get; }

    public string SignatureMethod
    {
      get
      {
        return this.signatureMethodElement.Algorithm;
      }
      set
      {
        this.signatureMethodElement.Algorithm = value;
      }
    }

    public XmlDictionaryString SignatureMethodDictionaryString
    {
      get
      {
        return this.signatureMethodElement.AlgorithmDictionaryString;
      }
      set
      {
        this.signatureMethodElement.AlgorithmDictionaryString = value;
      }
    }

    public SignatureResourcePool ResourcePool
    {
      get
      {
        if (this.resourcePool == null)
          this.resourcePool = new SignatureResourcePool();
        return this.resourcePool;
      }
      set
      {
        this.resourcePool = value;
      }
    }

    protected SignedInfo(DictionaryManager dictionaryManager)
    {
      if (dictionaryManager == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("dictionaryManager");
      this.signatureMethodElement = new ElementWithAlgorithmAttribute(dictionaryManager.XmlSignatureDictionary.SignatureMethod);
      this.dictionaryManager = dictionaryManager;
    }

    public void ComputeHash(HashAlgorithm algorithm)
    {
      if (this.CanonicalizationMethod != "http://www.w3.org/2001/10/xml-exc-c14n#" && this.CanonicalizationMethod != "http://www.w3.org/2001/10/xml-exc-c14n#WithComments")
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("UnsupportedTransformAlgorithm")));
#if FEATURE_CORECLR
        throw new NotImplementedException("SignatureResourcePool is not fully implemented in .NET Core");
#else
      HashStream hashStream = this.ResourcePool.TakeHashStream(algorithm);
      this.ComputeHash(hashStream);
      hashStream.FlushHash();
#endif
    }

    protected virtual void ComputeHash(HashStream hashStream)
    {
      if (this.sendSide)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("SignatureResourcePool is not fully implemented in .NET Core");
#else
        XmlDictionaryWriter utf8Writer = this.ResourcePool.TakeUtf8Writer();
        utf8Writer.StartCanonicalization((Stream) hashStream, false, (string[]) null);
        this.WriteTo(utf8Writer, this.dictionaryManager);
        utf8Writer.EndCanonicalization();
#endif
      }
      else if (this.canonicalStream != null)
      {
        this.canonicalStream.WriteTo((Stream) hashStream);
      }
      else
      {
        if (this.readerProvider == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("InclusiveNamespacePrefixRequiresSignatureReader")));
        XmlDictionaryReader reader = this.readerProvider.GetReader(this.signatureReaderProviderCallbackContext);
        if (!reader.CanCanonicalize)
        {
          MemoryStream memoryStream = new MemoryStream();
          XmlDictionaryWriter binaryWriter = XmlDictionaryWriter.CreateBinaryWriter((Stream) memoryStream, this.DictionaryManager.ParentDictionary);
          string[] inclusivePrefixes = this.GetInclusivePrefixes();
          if (inclusivePrefixes != null)
          {
            binaryWriter.WriteStartElement("a");
            for (int index = 0; index < inclusivePrefixes.Length; ++index)
            {
              string forInclusivePrefix = this.GetNamespaceForInclusivePrefix(inclusivePrefixes[index]);
              if (forInclusivePrefix != null)
                binaryWriter.WriteXmlnsAttribute(inclusivePrefixes[index], forInclusivePrefix);
            }
          }
          int content = (int) reader.MoveToContent();
          binaryWriter.WriteNode(reader, false);
          if (inclusivePrefixes != null)
            binaryWriter.WriteEndElement();
          binaryWriter.Flush();
          byte[] array = memoryStream.ToArray();
          int length = (int) memoryStream.Length;
          binaryWriter.Close();
          reader.Close();
          reader = XmlDictionaryReader.CreateBinaryReader(array, 0, length, this.DictionaryManager.ParentDictionary, XmlDictionaryReaderQuotas.Max);
          if (inclusivePrefixes != null)
            reader.ReadStartElement("a");
        }
        reader.ReadStartElement(this.dictionaryManager.XmlSignatureDictionary.Signature, this.dictionaryManager.XmlSignatureDictionary.Namespace);
        reader.MoveToStartElement(this.dictionaryManager.XmlSignatureDictionary.SignedInfo, this.dictionaryManager.XmlSignatureDictionary.Namespace);
        reader.StartCanonicalization((Stream) hashStream, false, this.GetInclusivePrefixes());
        reader.Skip();
        reader.EndCanonicalization();
        reader.Close();
      }
    }

    public abstract void ComputeReferenceDigests();

    protected string[] GetInclusivePrefixes()
    {
      return this.canonicalizationMethodElement.GetInclusivePrefixes();
    }

    protected virtual string GetNamespaceForInclusivePrefix(string prefix)
    {
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
    }

    public abstract void EnsureAllReferencesVerified();

    public void EnsureDigestValidity(string id, object resolvedXmlSource)
    {
      if (!this.EnsureDigestValidityIfIdMatches(id, resolvedXmlSource))
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("RequiredTargetNotSigned", new object[1]{ (object) id })));
    }

    public abstract bool EnsureDigestValidityIfIdMatches(string id, object resolvedXmlSource);

    public virtual bool HasUnverifiedReference(string id)
    {
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
    }

    protected void ReadCanonicalizationMethod(XmlDictionaryReader reader, DictionaryManager dictionaryManager)
    {
      this.canonicalizationMethodElement.ReadFrom(reader, dictionaryManager, false);
    }

    public abstract void ReadFrom(XmlDictionaryReader reader, TransformFactory transformFactory, DictionaryManager dictionaryManager);

    protected void ReadSignatureMethod(XmlDictionaryReader reader, DictionaryManager dictionaryManager)
    {
      this.signatureMethodElement.ReadFrom(reader, dictionaryManager);
    }

    protected void WriteCanonicalizationMethod(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
      this.canonicalizationMethodElement.WriteTo(writer, dictionaryManager);
    }

    protected void WriteSignatureMethod(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
      this.signatureMethodElement.WriteTo(writer, dictionaryManager);
    }

    public abstract void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager);
  }
}
