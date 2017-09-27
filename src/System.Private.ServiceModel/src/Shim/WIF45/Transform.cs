// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.Transform
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Xml;

namespace System.IdentityModel
{
  internal abstract class Transform
  {
    public abstract string Algorithm { get; }

    public virtual bool NeedsInclusiveContext
    {
      get
      {
        return false;
      }
    }

    public abstract object Process(object input, SignatureResourcePool resourcePool, DictionaryManager dictionaryManager);

    public abstract byte[] ProcessAndDigest(object input, SignatureResourcePool resourcePool, string digestAlgorithm, DictionaryManager dictionaryManager);

    public abstract void ReadFrom(XmlDictionaryReader reader, DictionaryManager dictionaryManager, bool preserveComments);

    public abstract void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager);
  }
}
