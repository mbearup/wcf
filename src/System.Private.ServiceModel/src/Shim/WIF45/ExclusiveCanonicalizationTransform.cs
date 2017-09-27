// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.ExclusiveCanonicalizationTransform
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.ServiceModel;
using System.IO;
using System.Security.Cryptography;
using System.Xml;

namespace System.IdentityModel
{
  internal class ExclusiveCanonicalizationTransform : Transform
  {
    private string inclusiveListElementPrefix = "ec";
    private string prefix = "";
    private bool includeComments;
    private string algorithm;
    private string inclusiveNamespacesPrefixList;
    private string[] inclusivePrefixes;
    private readonly bool isCanonicalizationMethod;

    public override string Algorithm
    {
      get
      {
        return this.algorithm;
      }
    }

    public bool IncludeComments
    {
      get
      {
        return this.includeComments;
      }
    }

    public string InclusiveNamespacesPrefixList
    {
      get
      {
        return this.inclusiveNamespacesPrefixList;
      }
      set
      {
        this.inclusiveNamespacesPrefixList = value;
        this.inclusivePrefixes = ExclusiveCanonicalizationTransform.TokenizeInclusivePrefixList(value);
      }
    }

    public override bool NeedsInclusiveContext
    {
      get
      {
        return this.GetInclusivePrefixes() != null;
      }
    }

    public ExclusiveCanonicalizationTransform()
      : this(false)
    {
    }

    public ExclusiveCanonicalizationTransform(bool isCanonicalizationMethod)
      : this(isCanonicalizationMethod, false)
    {
    }

    public ExclusiveCanonicalizationTransform(bool isCanonicalizationMethod, bool includeComments)
    {
      this.isCanonicalizationMethod = isCanonicalizationMethod;
      this.includeComments = includeComments;
      this.algorithm = includeComments ? XD.SecurityAlgorithmDictionary.ExclusiveC14nWithComments.Value : XD.SecurityAlgorithmDictionary.ExclusiveC14n.Value;
    }

    public string[] GetInclusivePrefixes()
    {
      return this.inclusivePrefixes;
    }

#if !FEATURE_CORECLR
    private CanonicalizationDriver GetConfiguredDriver(SignatureResourcePool resourcePool)
    {
      CanonicalizationDriver canonicalizationDriver = resourcePool.TakeCanonicalizationDriver();
      canonicalizationDriver.IncludeComments = this.IncludeComments;
      canonicalizationDriver.SetInclusivePrefixes(this.inclusivePrefixes);
      return canonicalizationDriver;
    }
#endif

    public override object Process(object input, SignatureResourcePool resourcePool, DictionaryManager dictionaryManager)
    {
      if (input is XmlReader)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("GetConfiguredDriver is not supported in .NET Core");
#else
        CanonicalizationDriver configuredDriver = this.GetConfiguredDriver(resourcePool);
        configuredDriver.SetInput(input as XmlReader);
        return (object) configuredDriver.GetMemoryStream();
#endif
      }
      if (input is ISecurityElement)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("SignatureResourcePool is not fully implemented in .NET Core");
#else
        MemoryStream memoryStream = new MemoryStream();
        XmlDictionaryWriter utf8Writer = resourcePool.TakeUtf8Writer();
        utf8Writer.StartCanonicalization((Stream) memoryStream, false, (string[]) null);
        (input as ISecurityElement).WriteTo(utf8Writer, dictionaryManager);
        utf8Writer.EndCanonicalization();
        memoryStream.Seek(0L, SeekOrigin.Begin);
        return (object) memoryStream;
#endif
      }
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnsupportedInputTypeForTransform", new object[1]{ (object) input.GetType() })));
    }

    public override byte[] ProcessAndDigest(object input, SignatureResourcePool resourcePool, string digestAlgorithm, DictionaryManager dictionaryManager)
    {
#if FEATURE_CORECLR
        throw new NotImplementedException("SignatureResourcePool is not fully implemented in .NET Core");
#else
      HashAlgorithm hashAlgorithm = resourcePool.TakeHashAlgorithm(digestAlgorithm);
      this.ProcessAndDigest(input, resourcePool, hashAlgorithm, dictionaryManager);
      return hashAlgorithm.Hash;
#endif
    }

    public void ProcessAndDigest(object input, SignatureResourcePool resourcePool, HashAlgorithm hash, DictionaryManager dictionaryManger)
    {
#if FEATURE_CORECLR
        throw new NotImplementedException("SignatureResourcePool is not fully implemented in .NET Core");
#else
      HashStream hashStream = resourcePool.TakeHashStream(hash);
      XmlReader reader = input as XmlReader;
      if (reader != null)
        this.ProcessReaderInput(reader, resourcePool, hashStream);
      else if (input is ISecurityElement)
      {
        XmlDictionaryWriter utf8Writer = resourcePool.TakeUtf8Writer();
        utf8Writer.StartCanonicalization((Stream) hashStream, this.IncludeComments, this.GetInclusivePrefixes());
        (input as ISecurityElement).WriteTo(utf8Writer, dictionaryManger);
        utf8Writer.EndCanonicalization();
      }
      else
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnsupportedInputTypeForTransform", new object[1]{ (object) input.GetType() })));
      hashStream.FlushHash();
#endif
    }

    private void ProcessReaderInput(XmlReader reader, SignatureResourcePool resourcePool, HashStream hashStream)
    {
      int content = (int) reader.MoveToContent();
      XmlDictionaryReader dictionaryReader = reader as XmlDictionaryReader;
      if (dictionaryReader != null && dictionaryReader.CanCanonicalize)
      {
        dictionaryReader.StartCanonicalization((Stream) hashStream, this.IncludeComments, this.GetInclusivePrefixes());
        dictionaryReader.Skip();
        dictionaryReader.EndCanonicalization();
      }
      else
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("GetConfiguredDriver is not supported in .NET Core");
#else
        CanonicalizationDriver configuredDriver = this.GetConfiguredDriver(resourcePool);
        configuredDriver.SetInput(reader);
        configuredDriver.WriteTo((Stream) hashStream);
#endif
      }
    }

    public override void ReadFrom(XmlDictionaryReader reader, DictionaryManager dictionaryManager, bool preserveComments)
    {
      XmlDictionaryString localName = this.isCanonicalizationMethod ? dictionaryManager.XmlSignatureDictionary.CanonicalizationMethod : dictionaryManager.XmlSignatureDictionary.Transform;
      reader.MoveToStartElement(localName, dictionaryManager.XmlSignatureDictionary.Namespace);
      this.prefix = reader.Prefix;
      bool isEmptyElement1 = reader.IsEmptyElement;
      this.algorithm = reader.GetAttribute(dictionaryManager.XmlSignatureDictionary.Algorithm, (XmlDictionaryString) null);
      if (string.IsNullOrEmpty(this.algorithm))
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("ID0001", (object) dictionaryManager.XmlSignatureDictionary.Algorithm, (object) reader.LocalName)));
      if (this.algorithm == dictionaryManager.SecurityAlgorithmDictionary.ExclusiveC14nWithComments.Value)
        this.includeComments = preserveComments;
      else if (this.algorithm == dictionaryManager.SecurityAlgorithmDictionary.ExclusiveC14n.Value)
        this.includeComments = false;
      else
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("ID6005", new object[1]{ (object) this.algorithm })));
      reader.Read();
      int content1 = (int) reader.MoveToContent();
      if (isEmptyElement1)
        return;
      if (reader.IsStartElement(dictionaryManager.ExclusiveC14NDictionary.InclusiveNamespaces, dictionaryManager.ExclusiveC14NDictionary.Namespace))
      {
        reader.MoveToStartElement(dictionaryManager.ExclusiveC14NDictionary.InclusiveNamespaces, dictionaryManager.ExclusiveC14NDictionary.Namespace);
        this.inclusiveListElementPrefix = reader.Prefix;
        bool isEmptyElement2 = reader.IsEmptyElement;
        this.InclusiveNamespacesPrefixList = reader.GetAttribute(dictionaryManager.ExclusiveC14NDictionary.PrefixList, (XmlDictionaryString) null);
        reader.Read();
        if (!isEmptyElement2)
          reader.ReadEndElement();
      }
      int content2 = (int) reader.MoveToContent();
      reader.ReadEndElement();
    }

    public override void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
      XmlDictionaryString localName = this.isCanonicalizationMethod ? dictionaryManager.XmlSignatureDictionary.CanonicalizationMethod : dictionaryManager.XmlSignatureDictionary.Transform;
      writer.WriteStartElement(this.prefix, localName, dictionaryManager.XmlSignatureDictionary.Namespace);
      writer.WriteAttributeString(dictionaryManager.XmlSignatureDictionary.Algorithm, (XmlDictionaryString) null, this.algorithm);
      if (this.InclusiveNamespacesPrefixList != null)
      {
        writer.WriteStartElement(this.inclusiveListElementPrefix, dictionaryManager.ExclusiveC14NDictionary.InclusiveNamespaces, dictionaryManager.ExclusiveC14NDictionary.Namespace);
        writer.WriteAttributeString(dictionaryManager.ExclusiveC14NDictionary.PrefixList, (XmlDictionaryString) null, this.InclusiveNamespacesPrefixList);
        writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }

    private static string[] TokenizeInclusivePrefixList(string prefixList)
    {
      if (prefixList == null)
        return (string[]) null;
      string[] strArray1 = prefixList.Split((char[]) null);
      int length = 0;
      for (int index = 0; index < strArray1.Length; ++index)
      {
        string str = strArray1[index];
        if (str == "#default")
          strArray1[length++] = string.Empty;
        else if (str.Length > 0)
          strArray1[length++] = str;
      }
      if (length == 0)
        return (string[]) null;
      if (length == strArray1.Length)
        return strArray1;
      string[] strArray2 = new string[length];
      Array.Copy((Array) strArray1, (Array) strArray2, length);
      return strArray2;
    }
  }
}
