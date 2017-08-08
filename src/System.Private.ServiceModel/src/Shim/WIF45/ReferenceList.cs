// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.ReferenceList
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Collections.Generic;
using System.IdentityModel;
using System.Runtime.CompilerServices;
using System.Xml;

namespace System.ServiceModel.Security
{
  [TypeForwardedFrom("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
  public sealed class ReferenceList : ISecurityElement
  {
    public static readonly XmlDictionaryString ElementName = System.IdentityModel.XD.XmlEncryptionDictionary.ReferenceList;
    internal static readonly XmlDictionaryString NamespaceUri = EncryptedType.NamespaceUri;
    internal static readonly XmlDictionaryString UriAttribute = System.IdentityModel.XD.XmlEncryptionDictionary.URI;
    private List<string> referredIds = new List<string>();
    private const string NamespacePrefix = "e";

    public int DataReferenceCount
    {
      get
      {
        return this.referredIds.Count;
      }
    }

    public bool HasId
    {
      get
      {
        return false;
      }
    }

    public string Id
    {
      get
      {
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
      }
    }

    public void AddReferredId(string id)
    {
      if (id == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("id"));
      this.referredIds.Add(id);
    }

    public bool ContainsReferredId(string id)
    {
      if (id == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("id"));
      return this.referredIds.Contains(id);
    }

    public string GetReferredId(int index)
    {
      return this.referredIds[index];
    }

    public void ReadFrom(XmlDictionaryReader reader)
    {
      reader.ReadStartElement(ReferenceList.ElementName, ReferenceList.NamespaceUri);
      while (reader.IsStartElement())
      {
        string str = ReferenceList.DataReference.ReadFrom(reader);
        if (this.referredIds.Contains(str))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new SecurityMessageSerializationException(SR.GetString("InvalidDataReferenceInReferenceList", new object[1]{ (object) ("#" + str) })));
        this.referredIds.Add(str);
      }
      reader.ReadEndElement();
      if (this.DataReferenceCount == 0)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new SecurityMessageSerializationException(SR.GetString("ReferenceListCannotBeEmpty")));
    }

    public bool TryRemoveReferredId(string id)
    {
      if (id == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("id"));
      return this.referredIds.Remove(id);
    }

    public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
    {
      if (this.DataReferenceCount == 0)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("ReferenceListCannotBeEmpty")));
      writer.WriteStartElement("e", ReferenceList.ElementName, ReferenceList.NamespaceUri);
      for (int index = 0; index < this.DataReferenceCount; ++index)
        ReferenceList.DataReference.WriteTo(writer, this.referredIds[index]);
      writer.WriteEndElement();
    }

    private static class DataReference
    {
      internal static readonly XmlDictionaryString ElementName = System.IdentityModel.XD.XmlEncryptionDictionary.DataReference;
      internal static readonly XmlDictionaryString NamespaceUri = EncryptedType.NamespaceUri;

      public static string ReadFrom(XmlDictionaryReader reader)
      {
        string prefix;
        string str = XmlHelper.ReadEmptyElementAndRequiredAttribute(reader, ReferenceList.DataReference.ElementName, ReferenceList.DataReference.NamespaceUri, ReferenceList.UriAttribute, out prefix);
        if (str.Length < 2 || (int) str[0] != 35)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new SecurityMessageSerializationException(SR.GetString("InvalidDataReferenceInReferenceList", new object[1]{ (object) str })));
        return str.Substring(1);
      }

      public static void WriteTo(XmlDictionaryWriter writer, string referredId)
      {
        writer.WriteStartElement(System.IdentityModel.XD.XmlEncryptionDictionary.Prefix.Value, ReferenceList.DataReference.ElementName, ReferenceList.DataReference.NamespaceUri);
        writer.WriteStartAttribute(ReferenceList.UriAttribute, (XmlDictionaryString) null);
        writer.WriteString("#");
        writer.WriteString(referredId);
        writer.WriteEndAttribute();
        writer.WriteEndElement();
      }
    }
  }
}

