// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.WSSecurityJan2004
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
  internal class WSSecurityJan2004
  {
    public class IdManager : SignatureTargetIdManager
    {
      private static readonly WSSecurityJan2004.IdManager instance = new WSSecurityJan2004.IdManager();

      public override string DefaultIdNamespacePrefix
      {
        get
        {
          return "u";
        }
      }

      public override string DefaultIdNamespaceUri
      {
        get
        {
          return "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
        }
      }

      internal static WSSecurityJan2004.IdManager Instance
      {
        get
        {
          return WSSecurityJan2004.IdManager.instance;
        }
      }

      private IdManager()
      {
      }

      public override string ExtractId(XmlDictionaryReader reader)
      {
        if (reader == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");
#if FEATURE_CORECLR
        throw new NotImplementedException("XD.XmlEncryptionDictionary is not supported in .NET Core");
#else
        if (reader.IsStartElement(EncryptedData.ElementName, XD.XmlEncryptionDictionary.Namespace))
          return reader.GetAttribute(XD.XmlEncryptionDictionary.Id, (XmlDictionaryString) null);
        return reader.GetAttribute(XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace);
#endif
      }

      public override void WriteIdAttribute(XmlDictionaryWriter writer, string id)
      {
        if (writer == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
        writer.WriteAttributeString(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace, id);
      }
    }
  }
}
