// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.ElementWithAlgorithmAttribute
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.ServiceModel;
using System.Security.Cryptography;
using System.Xml;

namespace System.IdentityModel
{
  internal struct ElementWithAlgorithmAttribute
  {
    private readonly XmlDictionaryString elementName;
    private string algorithm;
    private XmlDictionaryString algorithmDictionaryString;
    private string prefix;

    public string Algorithm
    {
      get
      {
        return this.algorithm;
      }
      set
      {
        this.algorithm = value;
      }
    }

    public XmlDictionaryString AlgorithmDictionaryString
    {
      get
      {
        return this.algorithmDictionaryString;
      }
      set
      {
        this.algorithmDictionaryString = value;
      }
    }

    public ElementWithAlgorithmAttribute(XmlDictionaryString elementName)
    {
      if (elementName == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("elementName"));
      this.elementName = elementName;
      this.algorithm = (string) null;
      this.algorithmDictionaryString = (XmlDictionaryString) null;
      this.prefix = "";
    }

    public void ReadFrom(XmlDictionaryReader reader, DictionaryManager dictionaryManager)
    {
      reader.MoveToStartElement(this.elementName, dictionaryManager.XmlSignatureDictionary.Namespace);
      this.prefix = reader.Prefix;
      bool isEmptyElement = reader.IsEmptyElement;
      this.algorithm = reader.GetAttribute(dictionaryManager.XmlSignatureDictionary.Algorithm, (XmlDictionaryString) null);
      if (this.algorithm == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("RequiredAttributeMissing", (object) dictionaryManager.XmlSignatureDictionary.Algorithm, (object) this.elementName)));
      reader.Read();
      int content1 = (int) reader.MoveToContent();
      if (isEmptyElement)
        return;
      int content2 = (int) reader.MoveToContent();
      reader.ReadEndElement();
    }

    public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
      writer.WriteStartElement(this.prefix, this.elementName, dictionaryManager.XmlSignatureDictionary.Namespace);
      writer.WriteStartAttribute(dictionaryManager.XmlSignatureDictionary.Algorithm, (XmlDictionaryString) null);
      if (this.algorithmDictionaryString != null)
        writer.WriteString(this.algorithmDictionaryString);
      else
        writer.WriteString(this.algorithm);
      writer.WriteEndAttribute();
      writer.WriteEndElement();
    }
  }
}
