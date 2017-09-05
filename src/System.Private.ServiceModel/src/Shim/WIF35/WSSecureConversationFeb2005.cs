// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.WSSecureConversationFeb2005
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
  internal class WSSecureConversationFeb2005 : WSSecureConversation
  {
    private SecurityStateEncoder securityStateEncoder;
    private IList<Type> knownClaimTypes;

    public override SecureConversationDictionary SerializerDictionary
    {
      get
      {
        return (SecureConversationDictionary) XD.SecureConversationFeb2005Dictionary;
      }
    }

    public WSSecureConversationFeb2005(WSSecurityTokenSerializer tokenSerializer, SecurityStateEncoder securityStateEncoder, IEnumerable<Type> knownTypes, int maxKeyDerivationOffset, int maxKeyDerivationLabelLength, int maxKeyDerivationNonceLength)
      : base(tokenSerializer, maxKeyDerivationOffset, maxKeyDerivationLabelLength, maxKeyDerivationNonceLength)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("DataProtectionSecurityStateEncoder not supported in .NET Core");
#else
      this.securityStateEncoder = securityStateEncoder == null ? (SecurityStateEncoder) new DataProtectionSecurityStateEncoder() : securityStateEncoder;
      this.knownClaimTypes = (IList<Type>) new List<Type>();
      if (knownTypes == null)
        return;
      foreach (Type knownType in knownTypes)
        this.knownClaimTypes.Add(knownType);
#endif
    }

    public override void PopulateTokenEntries(IList<WSSecurityTokenSerializer.TokenEntry> tokenEntryList)
    {
      base.PopulateTokenEntries(tokenEntryList);
      tokenEntryList.Add((WSSecurityTokenSerializer.TokenEntry) new WSSecureConversationFeb2005.SecurityContextTokenEntryFeb2005(this, this.securityStateEncoder, this.knownClaimTypes));
    }

    private class SecurityContextTokenEntryFeb2005 : WSSecureConversation.SecurityContextTokenEntry
    {
      public SecurityContextTokenEntryFeb2005(WSSecureConversationFeb2005 parent, SecurityStateEncoder securityStateEncoder, IList<Type> knownClaimTypes)
        : base((WSSecureConversation) parent, securityStateEncoder, knownClaimTypes)
      {
      }

      protected override bool CanReadGeneration(XmlDictionaryReader reader)
      {
        return reader.IsStartElement(DXD.SecureConversationDec2005Dictionary.Instance, XD.SecureConversationFeb2005Dictionary.Namespace);
      }

      protected override bool CanReadGeneration(XmlElement element)
      {
        if (element.LocalName == DXD.SecureConversationDec2005Dictionary.Instance.Value)
          return element.NamespaceURI == XD.SecureConversationFeb2005Dictionary.Namespace.Value;
        return false;
      }

      protected override UniqueId ReadGeneration(XmlDictionaryReader reader)
      {
        return reader.ReadElementContentAsUniqueId();
      }

      protected override UniqueId ReadGeneration(XmlElement element)
      {
        return XmlHelper.ReadTextElementAsUniqueId(element);
      }

      protected override void WriteGeneration(XmlDictionaryWriter writer, SecurityContextSecurityToken sct)
      {
        if (!(sct.KeyGeneration != (UniqueId) null))
          return;
        writer.WriteStartElement(XD.SecureConversationFeb2005Dictionary.Prefix.Value, DXD.SecureConversationDec2005Dictionary.Instance, XD.SecureConversationFeb2005Dictionary.Namespace);
        XmlHelper.WriteStringAsUniqueId(writer, sct.KeyGeneration);
        writer.WriteEndElement();
      }
    }

    public class DriverFeb2005 : WSSecureConversation.Driver
    {
      protected override SecureConversationDictionary DriverDictionary
      {
        get
        {
          return (SecureConversationDictionary) XD.SecureConversationFeb2005Dictionary;
        }
      }

      public override XmlDictionaryString CloseAction
      {
        get
        {
          return XD.SecureConversationFeb2005Dictionary.RequestSecurityContextClose;
        }
      }

      public override XmlDictionaryString CloseResponseAction
      {
        get
        {
          return XD.SecureConversationFeb2005Dictionary.RequestSecurityContextCloseResponse;
        }
      }

      public override bool IsSessionSupported
      {
        get
        {
          return true;
        }
      }

      public override XmlDictionaryString RenewAction
      {
        get
        {
          return XD.SecureConversationFeb2005Dictionary.RequestSecurityContextRenew;
        }
      }

      public override XmlDictionaryString RenewResponseAction
      {
        get
        {
          return XD.SecureConversationFeb2005Dictionary.RequestSecurityContextRenewResponse;
        }
      }

      public override XmlDictionaryString Namespace
      {
        get
        {
          return XD.SecureConversationFeb2005Dictionary.Namespace;
        }
      }

      public override string TokenTypeUri
      {
        get
        {
          return XD.SecureConversationFeb2005Dictionary.SecurityContextTokenType.Value;
        }
      }
    }
  }
}
