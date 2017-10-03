// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.StandardSignedInfo
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Xml;

namespace System.IdentityModel
{
  internal class StandardSignedInfo : SignedInfo
  {
    private string prefix = "";
    private List<Reference> references;
    private Dictionary<string, string> context;

    public override int ReferenceCount
    {
      get
      {
        return this.references.Count;
      }
    }

    public Reference this[int index]
    {
      get
      {
        return this.references[index];
      }
    }

    protected string Prefix
    {
      get
      {
        return this.prefix;
      }
      set
      {
        this.prefix = value;
      }
    }

    protected Dictionary<string, string> Context
    {
      get
      {
        return this.context;
      }
      set
      {
        this.context = value;
      }
    }

    public StandardSignedInfo(DictionaryManager dictionaryManager)
      : base(dictionaryManager)
    {
      this.references = new List<Reference>();
    }

    public void AddReference(Reference reference)
    {
      reference.ResourcePool = this.ResourcePool;
      this.references.Add(reference);
    }

    public override void EnsureAllReferencesVerified()
    {
      for (int index = 0; index < this.references.Count; ++index)
      {
        if (!this.references[index].Verified)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("UnableToResolveReferenceUriForSignature", new object[1]{ (object) this.references[index].Uri })));
      }
    }

    public override bool EnsureDigestValidityIfIdMatches(string id, object resolvedXmlSource)
    {
      for (int index = 0; index < this.references.Count; ++index)
      {
        if (this.references[index].EnsureDigestValidityIfIdMatches(id, resolvedXmlSource))
          return true;
      }
      return false;
    }

    public override bool HasUnverifiedReference(string id)
    {
      for (int index = 0; index < this.references.Count; ++index)
      {
        if (!this.references[index].Verified && this.references[index].ExtractReferredId() == id)
          return true;
      }
      return false;
    }

    public override void ComputeReferenceDigests()
    {
      if (this.references.Count == 0)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("AtLeastOneReferenceRequired")));
      for (int index = 0; index < this.references.Count; ++index)
        this.references[index].ComputeAndSetDigest();
    }

    public override void ReadFrom(XmlDictionaryReader reader, TransformFactory transformFactory, DictionaryManager dictionaryManager)
    {
      this.SendSide = false;
      if (reader.CanCanonicalize)
      {
        this.CanonicalStream = new MemoryStream();
        reader.StartCanonicalization((Stream) this.CanonicalStream, false, (string[]) null);
      }
      reader.MoveToStartElement(dictionaryManager.XmlSignatureDictionary.SignedInfo, dictionaryManager.XmlSignatureDictionary.Namespace);
      this.prefix = reader.Prefix;
      this.Id = reader.GetAttribute(dictionaryManager.UtilityDictionary.IdAttribute, (XmlDictionaryString) null);
      reader.Read();
      this.ReadCanonicalizationMethod(reader, dictionaryManager);
      this.ReadSignatureMethod(reader, dictionaryManager);
      while (reader.IsStartElement(dictionaryManager.XmlSignatureDictionary.Reference, dictionaryManager.XmlSignatureDictionary.Namespace))
      {
        Reference reference = new Reference(dictionaryManager);
        reference.ReadFrom(reader, transformFactory, dictionaryManager);
        this.AddReference(reference);
      }
      reader.ReadEndElement();
      if (reader.CanCanonicalize)
        reader.EndCanonicalization();
      string[] inclusivePrefixes = this.GetInclusivePrefixes();
      if (inclusivePrefixes == null)
        return;
      this.CanonicalStream = (MemoryStream) null;
      this.context = new Dictionary<string, string>(inclusivePrefixes.Length);
      for (int index = 0; index < inclusivePrefixes.Length; ++index)
        this.context.Add(inclusivePrefixes[index], reader.LookupNamespace(inclusivePrefixes[index]));
    }

    public override void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
      writer.WriteStartElement(this.prefix, dictionaryManager.XmlSignatureDictionary.SignedInfo, dictionaryManager.XmlSignatureDictionary.Namespace);
      if (this.Id != null)
        writer.WriteAttributeString(dictionaryManager.UtilityDictionary.IdAttribute, (XmlDictionaryString) null, this.Id);
      this.WriteCanonicalizationMethod(writer, dictionaryManager);
      this.WriteSignatureMethod(writer, dictionaryManager);
      for (int index = 0; index < this.references.Count; ++index)
        this.references[index].WriteTo(writer, dictionaryManager);
      writer.WriteEndElement();
    }

    protected override string GetNamespaceForInclusivePrefix(string prefix)
    {
      if (this.context == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException());
      if (prefix == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("prefix");
      return this.context[prefix];
    }
  }
}
