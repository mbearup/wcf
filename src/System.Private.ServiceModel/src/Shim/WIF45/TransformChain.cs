// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.TransformChain
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Security.Cryptography;
using System.ServiceModel;
using System.Xml;

namespace System.IdentityModel
{
  internal sealed class TransformChain
  {
    private string prefix = "";
    private MostlySingletonList<Transform> transforms;

    public int TransformCount
    {
      get
      {
        return this.transforms.Count;
      }
    }

    public Transform this[int index]
    {
      get
      {
        return this.transforms[index];
      }
    }

    public bool NeedsInclusiveContext
    {
      get
      {
        for (int index = 0; index < this.TransformCount; ++index)
        {
          if (this[index].NeedsInclusiveContext)
            return true;
        }
        return false;
      }
    }

    public void Add(Transform transform)
    {
      this.transforms.Add(transform);
    }

    public void ReadFrom(XmlDictionaryReader reader, TransformFactory transformFactory, DictionaryManager dictionaryManager, bool preserveComments)
    {
      reader.MoveToStartElement(dictionaryManager.XmlSignatureDictionary.Transforms, dictionaryManager.XmlSignatureDictionary.Namespace);
      this.prefix = reader.Prefix;
      reader.Read();
      while (reader.IsStartElement(dictionaryManager.XmlSignatureDictionary.Transform, dictionaryManager.XmlSignatureDictionary.Namespace))
      {
        string attribute = reader.GetAttribute(dictionaryManager.XmlSignatureDictionary.Algorithm, (XmlDictionaryString) null);
        Transform transform = transformFactory.CreateTransform(attribute);
        transform.ReadFrom(reader, dictionaryManager, preserveComments);
        this.Add(transform);
      }
      int content = (int) reader.MoveToContent();
      reader.ReadEndElement();
      if (this.TransformCount == 0)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("AtLeastOneTransformRequired")));
    }

    public byte[] TransformToDigest(object data, SignatureResourcePool resourcePool, string digestMethod, DictionaryManager dictionaryManager)
    {
      for (int index = 0; index < this.TransformCount - 1; ++index)
        data = this[index].Process(data, resourcePool, dictionaryManager);
      return this[this.TransformCount - 1].ProcessAndDigest(data, resourcePool, digestMethod, dictionaryManager);
    }

    public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
      writer.WriteStartElement(this.prefix, dictionaryManager.XmlSignatureDictionary.Transforms, dictionaryManager.XmlSignatureDictionary.Namespace);
      for (int index = 0; index < this.TransformCount; ++index)
        this[index].WriteTo(writer, dictionaryManager);
      writer.WriteEndElement();
    }
  }
}
