// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.WSTrust
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net.Security;
using System.Runtime;
#if FEATURE_CORECLR
using System.ServiceModel;
// not supported
#else
using System.Runtime.Remoting.Metadata.W3cXsd2001;
#endif
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Xml;

namespace System.ServiceModel.Security
{
  internal abstract class WSTrust : WSSecurityTokenSerializer.SerializerEntries
  {
    private WSSecurityTokenSerializer tokenSerializer;

    public WSSecurityTokenSerializer WSSecurityTokenSerializer
    {
      get
      {
        return this.tokenSerializer;
      }
    }

    public abstract System.ServiceModel.TrustDictionary SerializerDictionary { get; }

    public WSTrust(WSSecurityTokenSerializer tokenSerializer)
    {
      this.tokenSerializer = tokenSerializer;
    }

    public override void PopulateTokenEntries(IList<WSSecurityTokenSerializer.TokenEntry> tokenEntryList)
    {
      tokenEntryList.Add((WSSecurityTokenSerializer.TokenEntry) new WSTrust.BinarySecretTokenEntry(this));
    }

    protected static bool CheckElement(XmlElement element, string name, string ns, out string value)
    {
      value = (string) null;
      if (element.LocalName != name || element.NamespaceURI != ns || !(element.FirstChild is XmlText))
        return false;
      value = element.FirstChild.Value;
      return true;
    }

    private class BinarySecretTokenEntry : WSSecurityTokenSerializer.TokenEntry
    {
      private WSTrust parent;
      private System.ServiceModel.TrustDictionary otherDictionary;

      protected override XmlDictionaryString LocalName
      {
        get
        {
          return this.parent.SerializerDictionary.BinarySecret;
        }
      }

      protected override XmlDictionaryString NamespaceUri
      {
        get
        {
          return this.parent.SerializerDictionary.Namespace;
        }
      }

      public override string TokenTypeUri
      {
        get
        {
          return (string) null;
        }
      }

      protected override string ValueTypeUri
      {
        get
        {
          return (string) null;
        }
      }

      public BinarySecretTokenEntry(WSTrust parent)
      {
        this.parent = parent;
        this.otherDictionary = (System.ServiceModel.TrustDictionary) null;
        if (parent.SerializerDictionary is System.ServiceModel.TrustDec2005Dictionary)
          this.otherDictionary = (System.ServiceModel.TrustDictionary) System.ServiceModel.XD.TrustFeb2005Dictionary;
        if (parent.SerializerDictionary is System.ServiceModel.TrustFeb2005Dictionary)
          this.otherDictionary = (System.ServiceModel.TrustDictionary) DXD.TrustDec2005Dictionary;
        if (this.otherDictionary != null)
          return;
        this.otherDictionary = this.parent.SerializerDictionary;
      }

      protected override Type[] GetTokenTypesCore()
      {
        return new Type[1]{ typeof (BinarySecretSecurityToken) };
      }

      public override bool CanReadTokenCore(XmlElement element)
      {
        string str = (string) null;
        if (element.HasAttribute("ValueType", (string) null))
          str = element.GetAttribute("ValueType", (string) null);
        if (element.LocalName == this.LocalName.Value && (element.NamespaceURI == this.NamespaceUri.Value || element.NamespaceURI == this.otherDictionary.Namespace.Value))
          return str == this.ValueTypeUri;
        return false;
      }

      public override bool CanReadTokenCore(XmlDictionaryReader reader)
      {
        if (reader.IsStartElement(this.LocalName, this.NamespaceUri) || reader.IsStartElement(this.LocalName, this.otherDictionary.Namespace))
          return reader.GetAttribute(System.ServiceModel.XD.SecurityJan2004Dictionary.ValueType, (XmlDictionaryString) null) == this.ValueTypeUri;
        return false;
      }

      public override SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXmlCore(XmlElement issuedTokenXml, SecurityTokenReferenceStyle tokenReferenceStyle)
      {
        TokenReferenceStyleHelper.Validate(tokenReferenceStyle);
        if (tokenReferenceStyle == SecurityTokenReferenceStyle.Internal)
          return WSSecurityTokenSerializer.TokenEntry.CreateDirectReference(issuedTokenXml, "Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", typeof (GenericXmlSecurityToken));
        if (tokenReferenceStyle == SecurityTokenReferenceStyle.External)
          return (SecurityKeyIdentifierClause) null;
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("tokenReferenceStyle"));
      }

      public override SecurityToken ReadTokenCore(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver)
      {
        string attribute1 = reader.GetAttribute(System.ServiceModel.XD.SecurityJan2004Dictionary.TypeAttribute, (XmlDictionaryString) null);
        string attribute2 = reader.GetAttribute(System.ServiceModel.XD.UtilityDictionary.IdAttribute, System.ServiceModel.XD.UtilityDictionary.Namespace);
        bool flag = false;
        if (attribute1 != null && attribute1.Length > 0)
        {
          if (attribute1 == this.parent.SerializerDictionary.NonceBinarySecret.Value || attribute1 == this.otherDictionary.NonceBinarySecret.Value)
            flag = true;
          else if (attribute1 != this.parent.SerializerDictionary.SymmetricKeyBinarySecret.Value && attribute1 != this.otherDictionary.SymmetricKeyBinarySecret.Value)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnexpectedBinarySecretType", (object) this.parent.SerializerDictionary.SymmetricKeyBinarySecret.Value, (object) attribute1)));
        }
        byte[] key = reader.ReadElementContentAsBase64();
        if (flag)
          return (SecurityToken) new NonceToken(attribute2, key);
        return (SecurityToken) new BinarySecretSecurityToken(attribute2, key);
      }

      public override void WriteTokenCore(XmlDictionaryWriter writer, SecurityToken token)
      {
        BinarySecretSecurityToken secretSecurityToken = token as BinarySecretSecurityToken;
        byte[] keyBytes = secretSecurityToken.GetKeyBytes();
        writer.WriteStartElement(this.parent.SerializerDictionary.Prefix.Value, this.parent.SerializerDictionary.BinarySecret, this.parent.SerializerDictionary.Namespace);
        if (secretSecurityToken.Id != null)
          writer.WriteAttributeString(System.ServiceModel.XD.UtilityDictionary.Prefix.Value, System.ServiceModel.XD.UtilityDictionary.IdAttribute, System.ServiceModel.XD.UtilityDictionary.Namespace, secretSecurityToken.Id);
        if (token is NonceToken)
          writer.WriteAttributeString(System.ServiceModel.XD.SecurityJan2004Dictionary.TypeAttribute, (XmlDictionaryString) null, this.parent.SerializerDictionary.NonceBinarySecret.Value);
        writer.WriteBase64(keyBytes, 0, keyBytes.Length);
        writer.WriteEndElement();
      }
    }

    public abstract class Driver : TrustDriver
    {
      private static readonly string base64Uri = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary";
      private static readonly string hexBinaryUri = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#HexBinary";
      private SecurityStandardsManager standardsManager;
      private List<SecurityTokenAuthenticator> entropyAuthenticators;

      public abstract System.ServiceModel.TrustDictionary DriverDictionary { get; }

#if FEATURE_CORECLR
      public override Collection<XmlElement> ProcessUnknownRequestParameters(Collection<XmlElement> unknownRequestParameters, Collection<XmlElement> originalRequestParameters)
      {
         throw new NotImplementedException("TrustDriver.ProcessUnknownRequestParameters not implemented in .NET Core");
      }
#endif

      public override XmlDictionaryString RequestSecurityTokenAction
      {
        get
        {
          return this.DriverDictionary.RequestSecurityTokenIssuance;
        }
      }

      public override XmlDictionaryString RequestSecurityTokenResponseAction
      {
        get
        {
          return this.DriverDictionary.RequestSecurityTokenIssuanceResponse;
        }
      }

      public override string RequestTypeIssue
      {
        get
        {
          return this.DriverDictionary.RequestTypeIssue.Value;
        }
      }

      public override string ComputedKeyAlgorithm
      {
        get
        {
          return this.DriverDictionary.Psha1ComputedKeyUri.Value;
        }
      }

      public override SecurityStandardsManager StandardsManager
      {
        get
        {
          return this.standardsManager;
        }
      }

      public override XmlDictionaryString Namespace
      {
        get
        {
          return this.DriverDictionary.Namespace;
        }
      }

      public Driver(SecurityStandardsManager standardsManager)
      {
        this.standardsManager = standardsManager;
        this.entropyAuthenticators = new List<SecurityTokenAuthenticator>(2);
      }

      public override RequestSecurityToken CreateRequestSecurityToken(XmlReader xmlReader)
      {
        XmlDictionaryReader dictionaryReader = XmlDictionaryReader.CreateDictionaryReader(xmlReader);
        dictionaryReader.MoveToStartElement(this.DriverDictionary.RequestSecurityToken, this.DriverDictionary.Namespace);
        string context = (string) null;
        string tokenType = (string) null;
        string requestType = (string) null;
        int keySize = 0;
        XmlElement rstXml = new XmlDocument().ReadNode((XmlReader) dictionaryReader) as XmlElement;
        SecurityKeyIdentifierClause renewTarget = (SecurityKeyIdentifierClause) null;
        SecurityKeyIdentifierClause closeTarget = (SecurityKeyIdentifierClause) null;
        for (int index = 0; index < rstXml.Attributes.Count; ++index)
        {
          XmlAttribute attribute = rstXml.Attributes[index];
          if (attribute.LocalName == this.DriverDictionary.Context.Value)
            context = attribute.Value;
        }
        for (int index = 0; index < rstXml.ChildNodes.Count; ++index)
        {
          XmlElement childNode = rstXml.ChildNodes[index] as XmlElement;
          if (childNode != null)
          {
            if (childNode.LocalName == this.DriverDictionary.TokenType.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
              tokenType = XmlHelper.ReadTextElementAsTrimmedString(childNode);
            else if (childNode.LocalName == this.DriverDictionary.RequestType.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
              requestType = XmlHelper.ReadTextElementAsTrimmedString(childNode);
            else if (childNode.LocalName == this.DriverDictionary.KeySize.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
              keySize = int.Parse(XmlHelper.ReadTextElementAsTrimmedString(childNode), (IFormatProvider) NumberFormatInfo.InvariantInfo);
          }
        }
        this.ReadTargets(rstXml, out renewTarget, out closeTarget);
        return new RequestSecurityToken(this.standardsManager, rstXml, context, tokenType, requestType, keySize, renewTarget, closeTarget);
      }

      private System.IdentityModel.XmlBuffer GetIssuedTokenBuffer(System.IdentityModel.XmlBuffer rstrBuffer)
      {
        System.IdentityModel.XmlBuffer xmlBuffer = (System.IdentityModel.XmlBuffer) null;
        using (XmlDictionaryReader reader = rstrBuffer.GetReader(0))
        {
          reader.ReadFullStartElement();
          while (reader.IsStartElement())
          {
            if (reader.IsStartElement(this.DriverDictionary.RequestedSecurityToken, this.DriverDictionary.Namespace))
            {
              reader.ReadStartElement();
              int content = (int) reader.MoveToContent();
              xmlBuffer = new System.IdentityModel.XmlBuffer(int.MaxValue);
              using (XmlDictionaryWriter dictionaryWriter = xmlBuffer.OpenSection(reader.Quotas))
              {
                dictionaryWriter.WriteNode(reader, false);
                xmlBuffer.CloseSection();
                xmlBuffer.Close();
              }
              reader.ReadEndElement();
              break;
            }
            reader.Skip();
          }
        }
        return xmlBuffer;
      }

      public override RequestSecurityTokenResponse CreateRequestSecurityTokenResponse(XmlReader xmlReader)
      {
        if (xmlReader == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("xmlReader");
        XmlDictionaryReader dictionaryReader = XmlDictionaryReader.CreateDictionaryReader(xmlReader);
        if (!dictionaryReader.IsStartElement(this.DriverDictionary.RequestSecurityTokenResponse, this.DriverDictionary.Namespace))
          XmlHelper.OnRequiredElementMissing(this.DriverDictionary.RequestSecurityTokenResponse.Value, this.DriverDictionary.Namespace.Value);
        System.IdentityModel.XmlBuffer rstrBuffer = new System.IdentityModel.XmlBuffer(int.MaxValue);
        using (XmlDictionaryWriter dictionaryWriter = rstrBuffer.OpenSection(dictionaryReader.Quotas))
        {
          dictionaryWriter.WriteNode(dictionaryReader, false);
          rstrBuffer.CloseSection();
          rstrBuffer.Close();
        }
        XmlElement rstrXml;
        using (XmlReader reader = (XmlReader) rstrBuffer.GetReader(0))
          rstrXml = new XmlDocument().ReadNode(reader) as XmlElement;
        System.IdentityModel.XmlBuffer issuedTokenBuffer = this.GetIssuedTokenBuffer(rstrBuffer);
        string context = (string) null;
        string tokenType = (string) null;
        int keySize = 0;
        SecurityKeyIdentifierClause requestedAttachedReference = (SecurityKeyIdentifierClause) null;
        SecurityKeyIdentifierClause requestedUnattachedReference = (SecurityKeyIdentifierClause) null;
        bool computeKey = false;
        DateTime validFrom = DateTime.UtcNow;
        DateTime validTo = SecurityUtils.MaxUtcDateTime;
        for (int index = 0; index < rstrXml.Attributes.Count; ++index)
        {
          XmlAttribute attribute = rstrXml.Attributes[index];
          if (attribute.LocalName == this.DriverDictionary.Context.Value)
            context = attribute.Value;
        }
        for (int index = 0; index < rstrXml.ChildNodes.Count; ++index)
        {
          XmlElement childNode = rstrXml.ChildNodes[index] as XmlElement;
          if (childNode != null)
          {
            if (childNode.LocalName == this.DriverDictionary.TokenType.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
              tokenType = XmlHelper.ReadTextElementAsTrimmedString(childNode);
            else if (childNode.LocalName == this.DriverDictionary.KeySize.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
              keySize = int.Parse(XmlHelper.ReadTextElementAsTrimmedString(childNode), (IFormatProvider) NumberFormatInfo.InvariantInfo);
            else if (childNode.LocalName == this.DriverDictionary.RequestedProofToken.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
            {
              XmlElement childElement = XmlHelper.GetChildElement(childNode);
              if (childElement.LocalName == this.DriverDictionary.ComputedKey.Value && childElement.NamespaceURI == this.DriverDictionary.Namespace.Value)
              {
                string str = XmlHelper.ReadTextElementAsTrimmedString(childElement);
                if (str != this.DriverDictionary.Psha1ComputedKeyUri.Value)
                  throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new SecurityNegotiationException(SR.GetString("UnknownComputedKeyAlgorithm", new object[1]{ (object) str })));
                computeKey = true;
              }
            }
            else if (childNode.LocalName == this.DriverDictionary.Lifetime.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
            {
              XmlElement childElement1 = XmlHelper.GetChildElement(childNode, "Created", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
              if (childElement1 != null)
                validFrom = DateTime.ParseExact(XmlHelper.ReadTextElementAsTrimmedString(childElement1), WSUtilitySpecificationVersion.AcceptedDateTimeFormats, (IFormatProvider) DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None).ToUniversalTime();
              XmlElement childElement2 = XmlHelper.GetChildElement(childNode, "Expires", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
              if (childElement2 != null)
                validTo = DateTime.ParseExact(XmlHelper.ReadTextElementAsTrimmedString(childElement2), WSUtilitySpecificationVersion.AcceptedDateTimeFormats, (IFormatProvider) DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None).ToUniversalTime();
            }
          }
        }
        bool isRequestedTokenClosed = this.ReadRequestedTokenClosed(rstrXml);
        this.ReadReferences(rstrXml, out requestedAttachedReference, out requestedUnattachedReference);
        return new RequestSecurityTokenResponse(this.standardsManager, rstrXml, context, tokenType, keySize, requestedAttachedReference, requestedUnattachedReference, computeKey, validFrom, validTo, isRequestedTokenClosed, issuedTokenBuffer);
      }

      public override RequestSecurityTokenResponseCollection CreateRequestSecurityTokenResponseCollection(XmlReader xmlReader)
      {
        XmlDictionaryReader dictionaryReader = XmlDictionaryReader.CreateDictionaryReader(xmlReader);
        List<RequestSecurityTokenResponse> securityTokenResponseList = new List<RequestSecurityTokenResponse>(2);
        string name = dictionaryReader.Name;
        dictionaryReader.ReadStartElement(this.DriverDictionary.RequestSecurityTokenResponseCollection, this.DriverDictionary.Namespace);
        while (dictionaryReader.IsStartElement(this.DriverDictionary.RequestSecurityTokenResponse.Value, this.DriverDictionary.Namespace.Value))
        {
          RequestSecurityTokenResponse securityTokenResponse = this.CreateRequestSecurityTokenResponse((XmlReader) dictionaryReader);
          securityTokenResponseList.Add(securityTokenResponse);
        }
        dictionaryReader.ReadEndElement();
        if (securityTokenResponseList.Count == 0)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new XmlException(SR.GetString("NoRequestSecurityTokenResponseElements")));
        return new RequestSecurityTokenResponseCollection((IEnumerable<RequestSecurityTokenResponse>) securityTokenResponseList.AsReadOnly(), this.StandardsManager);
      }

      private XmlElement GetAppliesToElement(XmlElement rootElement)
      {
        if (rootElement == null)
          return (XmlElement) null;
        for (int index = 0; index < rootElement.ChildNodes.Count; ++index)
        {
          XmlElement childNode = rootElement.ChildNodes[index] as XmlElement;
          if (childNode != null && childNode.LocalName == this.DriverDictionary.AppliesTo.Value && childNode.NamespaceURI == "http://schemas.xmlsoap.org/ws/2004/09/policy")
            return childNode;
        }
        return (XmlElement) null;
      }

      private T GetAppliesTo<T>(XmlElement rootXml, XmlObjectSerializer serializer)
      {
        XmlElement appliesToElement = this.GetAppliesToElement(rootXml);
        if (appliesToElement == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("NoAppliesToPresent")));
        using (XmlReader reader = (XmlReader) new XmlNodeReader((XmlNode) appliesToElement))
        {
          reader.ReadStartElement();
          lock (serializer)
            return (T) serializer.ReadObject(reader);
        }
      }

      public override T GetAppliesTo<T>(RequestSecurityToken rst, XmlObjectSerializer serializer)
      {
        if (rst == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rst");
        return this.GetAppliesTo<T>(rst.RequestSecurityTokenXml, serializer);
      }

      public override T GetAppliesTo<T>(RequestSecurityTokenResponse rstr, XmlObjectSerializer serializer)
      {
        if (rstr == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rstr");
        return this.GetAppliesTo<T>(rstr.RequestSecurityTokenResponseXml, serializer);
      }

      public override bool IsAppliesTo(string localName, string namespaceUri)
      {
        if (localName == this.DriverDictionary.AppliesTo.Value)
          return namespaceUri == "http://schemas.xmlsoap.org/ws/2004/09/policy";
        return false;
      }

      private void GetAppliesToQName(XmlElement rootElement, out string localName, out string namespaceUri)
      {
        localName = namespaceUri = (string) null;
        XmlElement appliesToElement = this.GetAppliesToElement(rootElement);
        if (appliesToElement == null)
          return;
        using (XmlReader xmlReader = (XmlReader) new XmlNodeReader((XmlNode) appliesToElement))
        {
          xmlReader.ReadStartElement();
          int content = (int) xmlReader.MoveToContent();
          localName = xmlReader.LocalName;
          namespaceUri = xmlReader.NamespaceURI;
        }
      }

      public override void GetAppliesToQName(RequestSecurityToken rst, out string localName, out string namespaceUri)
      {
        if (rst == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rst");
        this.GetAppliesToQName(rst.RequestSecurityTokenXml, out localName, out namespaceUri);
      }

      public override void GetAppliesToQName(RequestSecurityTokenResponse rstr, out string localName, out string namespaceUri)
      {
        if (rstr == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rstr");
        this.GetAppliesToQName(rstr.RequestSecurityTokenResponseXml, out localName, out namespaceUri);
      }

      public override byte[] GetAuthenticator(RequestSecurityTokenResponse rstr)
      {
        if (rstr != null && rstr.RequestSecurityTokenResponseXml != null && rstr.RequestSecurityTokenResponseXml.ChildNodes != null)
        {
          for (int index = 0; index < rstr.RequestSecurityTokenResponseXml.ChildNodes.Count; ++index)
          {
            XmlElement childNode = rstr.RequestSecurityTokenResponseXml.ChildNodes[index] as XmlElement;
            if (childNode != null && childNode.LocalName == this.DriverDictionary.Authenticator.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
            {
              XmlElement childElement = XmlHelper.GetChildElement(childNode);
              if (childElement.LocalName == this.DriverDictionary.CombinedHash.Value && childElement.NamespaceURI == this.DriverDictionary.Namespace.Value)
                return Convert.FromBase64String(XmlHelper.ReadTextElementAsTrimmedString(childElement));
            }
          }
        }
        return (byte[]) null;
      }

      public override BinaryNegotiation GetBinaryNegotiation(RequestSecurityTokenResponse rstr)
      {
        if (rstr == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rstr");
        return this.GetBinaryNegotiation(rstr.RequestSecurityTokenResponseXml);
      }

      public override BinaryNegotiation GetBinaryNegotiation(RequestSecurityToken rst)
      {
        if (rst == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rst");
        return this.GetBinaryNegotiation(rst.RequestSecurityTokenXml);
      }

      private BinaryNegotiation GetBinaryNegotiation(XmlElement rootElement)
      {
        if (rootElement == null)
          return (BinaryNegotiation) null;
        for (int index = 0; index < rootElement.ChildNodes.Count; ++index)
        {
          XmlElement childNode = rootElement.ChildNodes[index] as XmlElement;
          if (childNode != null && childNode.LocalName == this.DriverDictionary.BinaryExchange.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
            return WSTrust.Driver.ReadBinaryNegotiation(childNode);
        }
        return (BinaryNegotiation) null;
      }

      public override SecurityToken GetEntropy(RequestSecurityToken rst, SecurityTokenResolver resolver)
      {
        if (rst == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rst");
        return this.GetEntropy(rst.RequestSecurityTokenXml, resolver);
      }

      public override SecurityToken GetEntropy(RequestSecurityTokenResponse rstr, SecurityTokenResolver resolver)
      {
        if (rstr == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rstr");
        return this.GetEntropy(rstr.RequestSecurityTokenResponseXml, resolver);
      }

      private SecurityToken GetEntropy(XmlElement rootElement, SecurityTokenResolver resolver)
      {
        if (rootElement == null || rootElement.ChildNodes == null)
          return (SecurityToken) null;
        for (int index = 0; index < rootElement.ChildNodes.Count; ++index)
        {
          XmlElement childNode = rootElement.ChildNodes[index] as XmlElement;
          if (childNode != null && childNode.LocalName == this.DriverDictionary.Entropy.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
          {
            XmlElement childElement = XmlHelper.GetChildElement(childNode);
            if (childNode.GetAttribute("ValueType").Length == 0)
            { } 
            return this.standardsManager.SecurityTokenSerializer.ReadToken((XmlReader) new XmlNodeReader((XmlNode) childElement), resolver);
          }
        }
        return (SecurityToken) null;
      }

      private void GetIssuedAndProofXml(RequestSecurityTokenResponse rstr, out XmlElement issuedTokenXml, out XmlElement proofTokenXml)
      {
        issuedTokenXml = (XmlElement) null;
        proofTokenXml = (XmlElement) null;
        if (rstr.RequestSecurityTokenResponseXml == null || rstr.RequestSecurityTokenResponseXml.ChildNodes == null)
          return;
        for (int index = 0; index < rstr.RequestSecurityTokenResponseXml.ChildNodes.Count; ++index)
        {
          XmlElement childNode = rstr.RequestSecurityTokenResponseXml.ChildNodes[index] as XmlElement;
          if (childNode != null)
          {
            if (childNode.LocalName == this.DriverDictionary.RequestedSecurityToken.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
            {
              if (issuedTokenXml != null)
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("RstrHasMultipleIssuedTokens")));
              issuedTokenXml = XmlHelper.GetChildElement(childNode);
            }
            else if (childNode.LocalName == this.DriverDictionary.RequestedProofToken.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
            {
              if (proofTokenXml != null)
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("RstrHasMultipleProofTokens")));
              proofTokenXml = XmlHelper.GetChildElement(childNode);
            }
          }
        }
      }

#if FEATURE_CORECLR
      public GenericXmlSecurityToken GetIssuedToken(RequestSecurityTokenResponse rstr, SecurityTokenResolver resolver, IList<SecurityTokenAuthenticator> allowedAuthenticators, SecurityKeyEntropyMode keyEntropyMode, byte[] requestorEntropy, string expectedTokenType, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies, int defaultKeySize, bool isBearerKeyType)
#else
      public override GenericXmlSecurityToken GetIssuedToken(RequestSecurityTokenResponse rstr, SecurityTokenResolver resolver, IList<SecurityTokenAuthenticator> allowedAuthenticators, SecurityKeyEntropyMode keyEntropyMode, byte[] requestorEntropy, string expectedTokenType, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies, int defaultKeySize, bool isBearerKeyType)
#endif
      {
        SecurityKeyEntropyModeHelper.Validate(keyEntropyMode);
        if (defaultKeySize < 0)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("defaultKeySize", SR.GetString("ValueMustBeNonNegative")));
        if (rstr == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rstr");
        string str;
        if (rstr.TokenType != null)
        {
          if (expectedTokenType != null && expectedTokenType != rstr.TokenType)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("BadIssuedTokenType", (object) rstr.TokenType, (object) expectedTokenType)));
          str = rstr.TokenType;
        }
        else
          str = expectedTokenType;
        DateTime validFrom = rstr.ValidFrom;
        DateTime validTo = rstr.ValidTo;
        XmlElement issuedTokenXml;
        XmlElement proofTokenXml;
        this.GetIssuedAndProofXml(rstr, out issuedTokenXml, out proofTokenXml);
        if (issuedTokenXml == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("NoLicenseXml")));
        if (isBearerKeyType)
        {
          if (proofTokenXml != null)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("BearerKeyTypeCannotHaveProofKey")));
          return new GenericXmlSecurityToken(issuedTokenXml, (SecurityToken) null, validFrom, validTo, rstr.RequestedAttachedReference, rstr.RequestedUnattachedReference, authorizationPolicies);
        }
        SecurityToken entropy = this.GetEntropy(rstr, resolver);
        SecurityToken proofToken;
        if (keyEntropyMode == SecurityKeyEntropyMode.ClientEntropy)
        {
          if (requestorEntropy == null)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("EntropyModeRequiresRequestorEntropy", new object[1]{ (object) keyEntropyMode })));
          if (proofTokenXml != null || entropy != null)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("EntropyModeCannotHaveProofTokenOrIssuerEntropy", new object[1]{ (object) keyEntropyMode })));
          proofToken = (SecurityToken) new BinarySecretSecurityToken(requestorEntropy);
        }
        else if (keyEntropyMode == SecurityKeyEntropyMode.ServerEntropy)
        {
          if (requestorEntropy != null)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("EntropyModeCannotHaveRequestorEntropy", new object[1]{ (object) keyEntropyMode })));
          if (rstr.ComputeKey || entropy != null)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("EntropyModeCannotHaveComputedKey", new object[1]{ (object) keyEntropyMode })));
          if (proofTokenXml == null)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("EntropyModeRequiresProofToken", new object[1]{ (object) keyEntropyMode })));
          if (proofTokenXml.GetAttribute("ValueType").Length == 0)
          { }
          proofToken = this.standardsManager.SecurityTokenSerializer.ReadToken((XmlReader) new XmlNodeReader((XmlNode) proofTokenXml), resolver);
        }
        else
        {
          if (!rstr.ComputeKey)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("EntropyModeRequiresComputedKey", new object[1]{ (object) keyEntropyMode })));
          if (entropy == null)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("EntropyModeRequiresIssuerEntropy", new object[1]{ (object) keyEntropyMode })));
          if (requestorEntropy == null)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("EntropyModeRequiresRequestorEntropy", new object[1]{ (object) keyEntropyMode })));
          if (rstr.KeySize == 0 && defaultKeySize == 0)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("RstrKeySizeNotProvided")));
          int keySizeInBits = rstr.KeySize != 0 ? rstr.KeySize : defaultKeySize;
          byte[] issuerEntropy;
          if (entropy is BinarySecretSecurityToken)
          {
            issuerEntropy = ((BinarySecretSecurityToken) entropy).GetKeyBytes();
          }
          else
          {
#if FEATURE_CORECLR
            throw new NotImplementedException("WrappedKeySecurityToken is not supported in .NET Core");
#else
            if (!(entropy is WrappedKeySecurityToken))
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnsupportedIssuerEntropyType")));
            issuerEntropy = ((WrappedKeySecurityToken) entropy).GetWrappedKey();
#endif
          }
          proofToken = (SecurityToken) new BinarySecretSecurityToken(RequestSecurityTokenResponse.ComputeCombinedKey(requestorEntropy, issuerEntropy, keySizeInBits));
        }
        SecurityKeyIdentifierClause attachedReference = rstr.RequestedAttachedReference;
        SecurityKeyIdentifierClause unattachedReference = rstr.RequestedUnattachedReference;
        return (GenericXmlSecurityToken) new BufferedGenericXmlSecurityToken(issuedTokenXml, proofToken, validFrom, validTo, attachedReference, unattachedReference, authorizationPolicies, rstr.IssuedTokenBuffer);
      }

#if FEATURE_CORECLR
      public GenericXmlSecurityToken GetIssuedToken(RequestSecurityTokenResponse rstr, string expectedTokenType, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies, RSA clientKey)
#else
      public override GenericXmlSecurityToken GetIssuedToken(RequestSecurityTokenResponse rstr, string expectedTokenType, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies, RSA clientKey)
#endif
      {
        if (rstr == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("rstr"));
        string str;
        if (rstr.TokenType != null)
        {
          if (expectedTokenType != null && expectedTokenType != rstr.TokenType)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("BadIssuedTokenType", (object) rstr.TokenType, (object) expectedTokenType)));
          str = rstr.TokenType;
        }
        else
          str = expectedTokenType;
        DateTime validFrom = rstr.ValidFrom;
        DateTime validTo = rstr.ValidTo;
        XmlElement issuedTokenXml;
        XmlElement proofTokenXml;
        this.GetIssuedAndProofXml(rstr, out issuedTokenXml, out proofTokenXml);
        if (issuedTokenXml == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("NoLicenseXml")));
        if (proofTokenXml != null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("ProofTokenXmlUnexpectedInRstr")));
        SecurityKeyIdentifierClause attachedReference = rstr.RequestedAttachedReference;
        SecurityKeyIdentifierClause unattachedReference = rstr.RequestedUnattachedReference;
#if FEATURE_CORECLR
        throw new NotImplementedException("RsaSecurityToken is not supported in .NET Core");
#else
        SecurityToken proofToken = (SecurityToken) new RsaSecurityToken(clientKey);
        return (GenericXmlSecurityToken) new BufferedGenericXmlSecurityToken(issuedTokenXml, proofToken, validFrom, validTo, attachedReference, unattachedReference, authorizationPolicies, rstr.IssuedTokenBuffer);
#endif
      }

      public override bool IsAtRequestSecurityTokenResponse(XmlReader reader)
      {
        if (reader == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");
        return reader.IsStartElement(this.DriverDictionary.RequestSecurityTokenResponse.Value, this.DriverDictionary.Namespace.Value);
      }

      public override bool IsAtRequestSecurityTokenResponseCollection(XmlReader reader)
      {
        if (reader == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");
        return reader.IsStartElement(this.DriverDictionary.RequestSecurityTokenResponseCollection.Value, this.DriverDictionary.Namespace.Value);
      }

      public override bool IsRequestedSecurityTokenElement(string name, string nameSpace)
      {
        if (name == this.DriverDictionary.RequestedSecurityToken.Value)
          return nameSpace == this.DriverDictionary.Namespace.Value;
        return false;
      }

      public override bool IsRequestedProofTokenElement(string name, string nameSpace)
      {
        if (name == this.DriverDictionary.RequestedProofToken.Value)
          return nameSpace == this.DriverDictionary.Namespace.Value;
        return false;
      }

      public static BinaryNegotiation ReadBinaryNegotiation(XmlElement elem)
      {
        if (elem == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("elem");
        string str = (string) null;
        string valueTypeUri = (string) null;
        if (elem.Attributes != null)
        {
          for (int index = 0; index < elem.Attributes.Count; ++index)
          {
            XmlAttribute attribute = elem.Attributes[index];
            if (attribute.LocalName == "EncodingType" && attribute.NamespaceURI.Length == 0)
            {
              str = attribute.Value;
              if (str != WSTrust.Driver.base64Uri && str != WSTrust.Driver.hexBinaryUri)
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new XmlException(SR.GetString("UnsupportedBinaryEncoding", new object[1]{ (object) str })));
            }
            else if (attribute.LocalName == "ValueType" && attribute.NamespaceURI.Length == 0)
              valueTypeUri = attribute.Value;
          }
        }
        if (str == null)
          XmlHelper.OnRequiredAttributeMissing("EncodingType", elem.Name);
        if (valueTypeUri == null)
          XmlHelper.OnRequiredAttributeMissing("ValueType", elem.Name);
        string s = XmlHelper.ReadTextElementAsTrimmedString(elem);
#if FEATURE_CORECLR
        throw new NotImplementedException("SoapHexBinary is not supported in .NET Core");
#else
        byte[] negotiationData = !(str == WSTrust.Driver.base64Uri) ? SoapHexBinary.Parse(s).Value : Convert.FromBase64String(s);
        return new BinaryNegotiation(valueTypeUri, negotiationData);
#endif
      }

      protected virtual void ReadReferences(XmlElement rstrXml, out SecurityKeyIdentifierClause requestedAttachedReference, out SecurityKeyIdentifierClause requestedUnattachedReference)
      {
        XmlElement element = (XmlElement) null;
        requestedAttachedReference = (SecurityKeyIdentifierClause) null;
        requestedUnattachedReference = (SecurityKeyIdentifierClause) null;
        for (int index = 0; index < rstrXml.ChildNodes.Count; ++index)
        {
          XmlElement childNode = rstrXml.ChildNodes[index] as XmlElement;
          if (childNode != null)
          {
            if (childNode.LocalName == this.DriverDictionary.RequestedSecurityToken.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
              element = XmlHelper.GetChildElement(childNode);
            else if (childNode.LocalName == this.DriverDictionary.RequestedTokenReference.Value && childNode.NamespaceURI == this.DriverDictionary.Namespace.Value)
              requestedUnattachedReference = this.GetKeyIdentifierXmlReferenceClause(XmlHelper.GetChildElement(childNode));
          }
        }
        if (element == null)
          return;
        requestedAttachedReference = this.standardsManager.CreateKeyIdentifierClauseFromTokenXml(element, SecurityTokenReferenceStyle.Internal);
        if (requestedUnattachedReference != null)
          return;
        try
        {
          requestedUnattachedReference = this.standardsManager.CreateKeyIdentifierClauseFromTokenXml(element, SecurityTokenReferenceStyle.External);
        }
        catch (XmlException)
        {
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new XmlException(SR.GetString("TrustDriverIsUnableToCreatedNecessaryAttachedOrUnattachedReferences", new object[1]{ (object) element.ToString() })));
        }
      }

      internal bool TryReadKeyIdentifierClause(XmlNodeReader reader, out SecurityKeyIdentifierClause keyIdentifierClause)
      {
        keyIdentifierClause = (SecurityKeyIdentifierClause) null;
        try
        {
          keyIdentifierClause = this.standardsManager.SecurityTokenSerializer.ReadKeyIdentifierClause((XmlReader) reader);
        }
        catch (XmlException ex)
        {
          if (Fx.IsFatal((Exception) ex))
          {
            throw;
          }
          else
          {
            keyIdentifierClause = (SecurityKeyIdentifierClause) null;
            return false;
          }
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
          {
            throw;
          }
          else
          {
            keyIdentifierClause = (SecurityKeyIdentifierClause) null;
            return false;
          }
        }
        return true;
      }

      internal SecurityKeyIdentifierClause CreateGenericXmlSecurityKeyIdentifierClause(XmlNodeReader reader, XmlElement keyIdentifierReferenceXmlElement)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("GenericXmlSecurityKeyIdentifierClause is not supported in .NET Core");
#else
        string attribute = XmlDictionaryReader.CreateDictionaryReader((XmlReader) reader).GetAttribute(System.ServiceModel.XD.UtilityDictionary.IdAttribute, System.ServiceModel.XD.UtilityDictionary.Namespace);
        SecurityKeyIdentifierClause identifierClause = (SecurityKeyIdentifierClause) new GenericXmlSecurityKeyIdentifierClause(keyIdentifierReferenceXmlElement);
        if (!string.IsNullOrEmpty(attribute))
          identifierClause.Id = attribute;
        return identifierClause;
#endif
      }

      internal SecurityKeyIdentifierClause GetKeyIdentifierXmlReferenceClause(XmlElement keyIdentifierReferenceXmlElement)
      {
        SecurityKeyIdentifierClause keyIdentifierClause = (SecurityKeyIdentifierClause) null;
        if (!this.TryReadKeyIdentifierClause(new XmlNodeReader((XmlNode) keyIdentifierReferenceXmlElement), out keyIdentifierClause))
          keyIdentifierClause = this.CreateGenericXmlSecurityKeyIdentifierClause(new XmlNodeReader((XmlNode) keyIdentifierReferenceXmlElement), keyIdentifierReferenceXmlElement);
        return keyIdentifierClause;
      }

      protected virtual bool ReadRequestedTokenClosed(XmlElement rstrXml)
      {
        return false;
      }

      protected virtual void ReadTargets(XmlElement rstXml, out SecurityKeyIdentifierClause renewTarget, out SecurityKeyIdentifierClause closeTarget)
      {
        renewTarget = (SecurityKeyIdentifierClause) null;
        closeTarget = (SecurityKeyIdentifierClause) null;
      }

      public override void OnRSTRorRSTRCMissingException()
      {
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new XmlException(SR.GetString("ExpectedOneOfTwoElementsFromNamespace", (object) this.DriverDictionary.RequestSecurityTokenResponse, (object) this.DriverDictionary.RequestSecurityTokenResponseCollection, (object) this.DriverDictionary.Namespace)));
      }

      private void WriteAppliesTo(object appliesTo, Type appliesToType, XmlObjectSerializer serializer, XmlWriter xmlWriter)
      {
        XmlDictionaryWriter dictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(xmlWriter);
        dictionaryWriter.WriteStartElement("wsp", this.DriverDictionary.AppliesTo.Value, "http://schemas.xmlsoap.org/ws/2004/09/policy");
        lock (serializer)
          serializer.WriteObject(dictionaryWriter, appliesTo);
        dictionaryWriter.WriteEndElement();
      }

      public void WriteBinaryNegotiation(BinaryNegotiation negotiation, XmlWriter xmlWriter)
      {
        if (negotiation == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("negotiation");
        XmlDictionaryWriter dictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(xmlWriter);
        negotiation.WriteTo(dictionaryWriter, this.DriverDictionary.Prefix.Value, this.DriverDictionary.BinaryExchange, this.DriverDictionary.Namespace, System.ServiceModel.XD.SecurityJan2004Dictionary.ValueType, (XmlDictionaryString) null);
      }

      public override void WriteRequestSecurityToken(RequestSecurityToken rst, XmlWriter xmlWriter)
      {
        if (rst == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rst");
        if (xmlWriter == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("xmlWriter");
        XmlDictionaryWriter dictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(xmlWriter);
        if (rst.IsReceiver)
        {
          rst.WriteTo((XmlWriter) dictionaryWriter);
        }
        else
        {
          dictionaryWriter.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.RequestSecurityToken, this.DriverDictionary.Namespace);
          XmlHelper.AddNamespaceDeclaration(dictionaryWriter, this.DriverDictionary.Prefix.Value, this.DriverDictionary.Namespace);
          if (rst.Context != null)
            dictionaryWriter.WriteAttributeString(this.DriverDictionary.Context, (XmlDictionaryString) null, rst.Context);
          rst.OnWriteCustomAttributes((XmlWriter) dictionaryWriter);
          if (rst.TokenType != null)
          {
            dictionaryWriter.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.TokenType, this.DriverDictionary.Namespace);
            dictionaryWriter.WriteString(rst.TokenType);
            dictionaryWriter.WriteEndElement();
          }
          if (rst.RequestType != null)
          {
            dictionaryWriter.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.RequestType, this.DriverDictionary.Namespace);
            dictionaryWriter.WriteString(rst.RequestType);
            dictionaryWriter.WriteEndElement();
          }
          if (rst.AppliesTo != null)
            this.WriteAppliesTo(rst.AppliesTo, rst.AppliesToType, (XmlObjectSerializer) rst.AppliesToSerializer, (XmlWriter) dictionaryWriter);
          SecurityToken requestorEntropy = rst.GetRequestorEntropy();
          if (requestorEntropy != null)
          {
            dictionaryWriter.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.Entropy, this.DriverDictionary.Namespace);
            this.standardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) dictionaryWriter, requestorEntropy);
            dictionaryWriter.WriteEndElement();
          }
          if (rst.KeySize != 0)
          {
            dictionaryWriter.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.KeySize, this.DriverDictionary.Namespace);
            dictionaryWriter.WriteValue(rst.KeySize);
            dictionaryWriter.WriteEndElement();
          }
          BinaryNegotiation binaryNegotiation = rst.GetBinaryNegotiation();
          if (binaryNegotiation != null)
            this.WriteBinaryNegotiation(binaryNegotiation, (XmlWriter) dictionaryWriter);
          this.WriteTargets(rst, dictionaryWriter);
          if (rst.RequestProperties != null)
          {
            foreach (XmlNode requestProperty in rst.RequestProperties)
              requestProperty.WriteTo((XmlWriter) dictionaryWriter);
          }
          rst.OnWriteCustomElements((XmlWriter) dictionaryWriter);
          dictionaryWriter.WriteEndElement();
        }
      }

      protected virtual void WriteTargets(RequestSecurityToken rst, XmlDictionaryWriter writer)
      {
      }

      protected virtual void WriteReferences(RequestSecurityTokenResponse rstr, XmlDictionaryWriter writer)
      {
        if (rstr.RequestedUnattachedReference == null)
          return;
        writer.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.RequestedTokenReference, this.DriverDictionary.Namespace);
        this.standardsManager.SecurityTokenSerializer.WriteKeyIdentifierClause((XmlWriter) writer, rstr.RequestedUnattachedReference);
        writer.WriteEndElement();
      }

      protected virtual void WriteRequestedTokenClosed(RequestSecurityTokenResponse rstr, XmlDictionaryWriter writer)
      {
      }

      public override void WriteRequestSecurityTokenResponse(RequestSecurityTokenResponse rstr, XmlWriter xmlWriter)
      {
        if (rstr == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rstr");
        if (xmlWriter == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("xmlWriter");
        XmlDictionaryWriter dictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(xmlWriter);
        if (rstr.IsReceiver)
        {
          rstr.WriteTo((XmlWriter) dictionaryWriter);
        }
        else
        {
          dictionaryWriter.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.RequestSecurityTokenResponse, this.DriverDictionary.Namespace);
          if (rstr.Context != null)
            dictionaryWriter.WriteAttributeString(this.DriverDictionary.Context, (XmlDictionaryString) null, rstr.Context);
          XmlHelper.AddNamespaceDeclaration(dictionaryWriter, "u", System.ServiceModel.XD.UtilityDictionary.Namespace);
          rstr.OnWriteCustomAttributes((XmlWriter) dictionaryWriter);
          if (rstr.TokenType != null)
            dictionaryWriter.WriteElementString(this.DriverDictionary.Prefix.Value, this.DriverDictionary.TokenType, this.DriverDictionary.Namespace, rstr.TokenType);
          if (rstr.RequestedSecurityToken != null)
          {
            dictionaryWriter.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.RequestedSecurityToken, this.DriverDictionary.Namespace);
            this.standardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) dictionaryWriter, rstr.RequestedSecurityToken);
            dictionaryWriter.WriteEndElement();
          }
          if (rstr.AppliesTo != null)
            this.WriteAppliesTo(rstr.AppliesTo, rstr.AppliesToType, rstr.AppliesToSerializer, (XmlWriter) dictionaryWriter);
          this.WriteReferences(rstr, dictionaryWriter);
          if (rstr.ComputeKey || rstr.RequestedProofToken != null)
          {
            dictionaryWriter.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.RequestedProofToken, this.DriverDictionary.Namespace);
            if (rstr.ComputeKey)
              dictionaryWriter.WriteElementString(this.DriverDictionary.Prefix.Value, this.DriverDictionary.ComputedKey, this.DriverDictionary.Namespace, this.DriverDictionary.Psha1ComputedKeyUri.Value);
            else
              this.standardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) dictionaryWriter, rstr.RequestedProofToken);
            dictionaryWriter.WriteEndElement();
          }
          SecurityToken issuerEntropy = rstr.GetIssuerEntropy();
          if (issuerEntropy != null)
          {
            dictionaryWriter.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.Entropy, this.DriverDictionary.Namespace);
            this.standardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) dictionaryWriter, issuerEntropy);
            dictionaryWriter.WriteEndElement();
          }
          if (rstr.IsLifetimeSet || rstr.RequestedSecurityToken != null)
          {
            DateTime dateTime1 = SecurityUtils.MinUtcDateTime;
            DateTime dateTime2 = SecurityUtils.MaxUtcDateTime;
            if (rstr.IsLifetimeSet)
            {
              dateTime1 = rstr.ValidFrom.ToUniversalTime();
              dateTime2 = rstr.ValidTo.ToUniversalTime();
            }
            else if (rstr.RequestedSecurityToken != null)
            {
              dateTime1 = rstr.RequestedSecurityToken.ValidFrom.ToUniversalTime();
              dateTime2 = rstr.RequestedSecurityToken.ValidTo.ToUniversalTime();
            }
            dictionaryWriter.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.Lifetime, this.DriverDictionary.Namespace);
            dictionaryWriter.WriteStartElement(System.ServiceModel.XD.UtilityDictionary.Prefix.Value, System.ServiceModel.XD.UtilityDictionary.CreatedElement, System.ServiceModel.XD.UtilityDictionary.Namespace);
            dictionaryWriter.WriteString(dateTime1.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", (IFormatProvider) CultureInfo.InvariantCulture.DateTimeFormat));
            dictionaryWriter.WriteEndElement();
            dictionaryWriter.WriteStartElement(System.ServiceModel.XD.UtilityDictionary.Prefix.Value, System.ServiceModel.XD.UtilityDictionary.ExpiresElement, System.ServiceModel.XD.UtilityDictionary.Namespace);
            dictionaryWriter.WriteString(dateTime2.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", (IFormatProvider) CultureInfo.InvariantCulture.DateTimeFormat));
            dictionaryWriter.WriteEndElement();
            dictionaryWriter.WriteEndElement();
          }
          byte[] authenticator = rstr.GetAuthenticator();
          if (authenticator != null)
          {
            dictionaryWriter.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.Authenticator, this.DriverDictionary.Namespace);
            dictionaryWriter.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.CombinedHash, this.DriverDictionary.Namespace);
            dictionaryWriter.WriteBase64(authenticator, 0, authenticator.Length);
            dictionaryWriter.WriteEndElement();
            dictionaryWriter.WriteEndElement();
          }
          if (rstr.KeySize > 0)
          {
            dictionaryWriter.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.KeySize, this.DriverDictionary.Namespace);
            dictionaryWriter.WriteValue(rstr.KeySize);
            dictionaryWriter.WriteEndElement();
          }
          this.WriteRequestedTokenClosed(rstr, dictionaryWriter);
          BinaryNegotiation binaryNegotiation = rstr.GetBinaryNegotiation();
          if (binaryNegotiation != null)
            this.WriteBinaryNegotiation(binaryNegotiation, (XmlWriter) dictionaryWriter);
          rstr.OnWriteCustomElements((XmlWriter) dictionaryWriter);
          dictionaryWriter.WriteEndElement();
        }
      }

      public override void WriteRequestSecurityTokenResponseCollection(RequestSecurityTokenResponseCollection rstrCollection, XmlWriter xmlWriter)
      {
        if (rstrCollection == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rstrCollection");
        XmlDictionaryWriter dictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(xmlWriter);
        dictionaryWriter.WriteStartElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.RequestSecurityTokenResponseCollection, this.DriverDictionary.Namespace);
        foreach (RequestSecurityTokenResponse rstr in rstrCollection.RstrCollection)
          rstr.WriteTo((XmlWriter) dictionaryWriter);
        dictionaryWriter.WriteEndElement();
      }

      protected void SetProtectionLevelForFederation(OperationDescriptionCollection operations)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("MessagePartDescriptionCollection not fully implemented in .NET Core");
#else
        foreach (OperationDescription operation in (Collection<OperationDescription>) operations)
        {
          foreach (MessageDescription message in (Collection<MessageDescription>) operation.Messages)
          {
            if (message.Body.Parts.Count > 0)
            {
              foreach (MessagePartDescription part in (Collection<MessagePartDescription>) message.Body.Parts)
                part.ProtectionLevel = ProtectionLevel.EncryptAndSign;
            }
            if (OperationFormatter.IsValidReturnValue(message.Body.ReturnValue))
              message.Body.ReturnValue.ProtectionLevel = ProtectionLevel.EncryptAndSign;
          }
        }
#endif
      }

      public override bool TryParseKeySizeElement(XmlElement element, out int keySize)
      {
        if (element == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
        if (element.LocalName == this.DriverDictionary.KeySize.Value && element.NamespaceURI == this.DriverDictionary.Namespace.Value)
        {
          keySize = int.Parse(XmlHelper.ReadTextElementAsTrimmedString(element), (IFormatProvider) NumberFormatInfo.InvariantInfo);
          return true;
        }
        keySize = 0;
        return false;
      }

      public override XmlElement CreateKeySizeElement(int keySize)
      {
        if (keySize < 0)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("keySize", SR.GetString("ValueMustBeNonNegative")));
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement element = xmlDocument.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.KeySize.Value, this.DriverDictionary.Namespace.Value);
        element.AppendChild((XmlNode) xmlDocument.CreateTextNode(keySize.ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat)));
        return element;
      }

      public override XmlElement CreateKeyTypeElement(SecurityKeyType keyType)
      {
        if (keyType == SecurityKeyType.SymmetricKey)
          return this.CreateSymmetricKeyTypeElement();
        if (keyType == SecurityKeyType.AsymmetricKey)
          return this.CreatePublicKeyTypeElement();
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UnableToCreateKeyTypeElementForUnknownKeyType", new object[1]{ (object) keyType.ToString() })));
      }

      public override bool TryParseKeyTypeElement(XmlElement element, out SecurityKeyType keyType)
      {
        if (element == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
        if (this.TryParseSymmetricKeyElement(element))
        {
          keyType = SecurityKeyType.SymmetricKey;
          return true;
        }
        if (this.TryParsePublicKeyElement(element))
        {
          keyType = SecurityKeyType.AsymmetricKey;
          return true;
        }
        keyType = SecurityKeyType.SymmetricKey;
        return false;
      }

      public bool TryParseSymmetricKeyElement(XmlElement element)
      {
        if (element == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
        if (element.LocalName == this.DriverDictionary.KeyType.Value && element.NamespaceURI == this.DriverDictionary.Namespace.Value)
          return element.InnerText == this.DriverDictionary.SymmetricKeyType.Value;
        return false;
      }

      private XmlElement CreateSymmetricKeyTypeElement()
      {
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement element = xmlDocument.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.KeyType.Value, this.DriverDictionary.Namespace.Value);
        element.AppendChild((XmlNode) xmlDocument.CreateTextNode(this.DriverDictionary.SymmetricKeyType.Value));
        return element;
      }

      private bool TryParsePublicKeyElement(XmlElement element)
      {
        if (element == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
        if (element.LocalName == this.DriverDictionary.KeyType.Value && element.NamespaceURI == this.DriverDictionary.Namespace.Value)
          return element.InnerText == this.DriverDictionary.PublicKeyType.Value;
        return false;
      }

      private XmlElement CreatePublicKeyTypeElement()
      {
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement element = xmlDocument.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.KeyType.Value, this.DriverDictionary.Namespace.Value);
        element.AppendChild((XmlNode) xmlDocument.CreateTextNode(this.DriverDictionary.PublicKeyType.Value));
        return element;
      }

      public override bool TryParseTokenTypeElement(XmlElement element, out string tokenType)
      {
        if (element == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
        if (element.LocalName == this.DriverDictionary.TokenType.Value && element.NamespaceURI == this.DriverDictionary.Namespace.Value)
        {
          tokenType = element.InnerText;
          return true;
        }
        tokenType = (string) null;
        return false;
      }

      public override XmlElement CreateTokenTypeElement(string tokenTypeUri)
      {
        if (tokenTypeUri == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenTypeUri");
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement element = xmlDocument.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.TokenType.Value, this.DriverDictionary.Namespace.Value);
        element.AppendChild((XmlNode) xmlDocument.CreateTextNode(tokenTypeUri));
        return element;
      }

#if FEATURE_CORECLR
      public XmlElement CreateUseKeyElement(SecurityKeyIdentifier keyIdentifier, SecurityStandardsManager standardsManager)
#else
      public override XmlElement CreateUseKeyElement(SecurityKeyIdentifier keyIdentifier, SecurityStandardsManager standardsManager)
#endif
      {
        if (keyIdentifier == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifier");
        if (standardsManager == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("standardsManager");
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement element = xmlDocument.CreateElement(this.DriverDictionary.UseKey.Value, this.DriverDictionary.Namespace.Value);
        MemoryStream memoryStream = new MemoryStream();
        using (XmlDictionaryWriter dictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter((XmlWriter) new XmlTextWriter((Stream) memoryStream, Encoding.UTF8)))
        {
          standardsManager.SecurityTokenSerializer.WriteKeyIdentifier((XmlWriter) dictionaryWriter, keyIdentifier);
          dictionaryWriter.Flush();
          memoryStream.Seek(0L, SeekOrigin.Begin);
          XmlNode newChild;
          using (XmlDictionaryReader dictionaryReader = XmlDictionaryReader.CreateDictionaryReader((XmlReader) new XmlTextReader((Stream) memoryStream) { DtdProcessing = DtdProcessing.Prohibit }))
          {
            int content = (int) dictionaryReader.MoveToContent();
            newChild = xmlDocument.ReadNode((XmlReader) dictionaryReader);
          }
          element.AppendChild(newChild);
        }
        return element;
      }

      public override XmlElement CreateSignWithElement(string signatureAlgorithm)
      {
        if (signatureAlgorithm == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("signatureAlgorithm");
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement element = xmlDocument.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.SignWith.Value, this.DriverDictionary.Namespace.Value);
        element.AppendChild((XmlNode) xmlDocument.CreateTextNode(signatureAlgorithm));
        return element;
      }

      internal override bool IsSignWithElement(XmlElement element, out string signatureAlgorithm)
      {
        return WSTrust.CheckElement(element, this.DriverDictionary.SignWith.Value, this.DriverDictionary.Namespace.Value, out signatureAlgorithm);
      }

      public override XmlElement CreateEncryptWithElement(string encryptionAlgorithm)
      {
        if (encryptionAlgorithm == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("encryptionAlgorithm");
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement element = xmlDocument.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.EncryptWith.Value, this.DriverDictionary.Namespace.Value);
        element.AppendChild((XmlNode) xmlDocument.CreateTextNode(encryptionAlgorithm));
        return element;
      }

      public override XmlElement CreateEncryptionAlgorithmElement(string encryptionAlgorithm)
      {
        if (encryptionAlgorithm == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("encryptionAlgorithm");
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement element = xmlDocument.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.EncryptionAlgorithm.Value, this.DriverDictionary.Namespace.Value);
        element.AppendChild((XmlNode) xmlDocument.CreateTextNode(encryptionAlgorithm));
        return element;
      }

      internal override bool IsEncryptWithElement(XmlElement element, out string encryptWithAlgorithm)
      {
        return WSTrust.CheckElement(element, this.DriverDictionary.EncryptWith.Value, this.DriverDictionary.Namespace.Value, out encryptWithAlgorithm);
      }

      internal override bool IsEncryptionAlgorithmElement(XmlElement element, out string encryptionAlgorithm)
      {
        return WSTrust.CheckElement(element, this.DriverDictionary.EncryptionAlgorithm.Value, this.DriverDictionary.Namespace.Value, out encryptionAlgorithm);
      }

#if FEATURE_CORECLR
      public XmlElement CreateComputedKeyAlgorithmElement(string algorithm)
#else
      public override XmlElement CreateComputedKeyAlgorithmElement(string algorithm)
#endif
      {
        if (algorithm == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("algorithm");
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement element = xmlDocument.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.ComputedKeyAlgorithm.Value, this.DriverDictionary.Namespace.Value);
        element.AppendChild((XmlNode) xmlDocument.CreateTextNode(algorithm));
        return element;
      }

      public override XmlElement CreateCanonicalizationAlgorithmElement(string algorithm)
      {
        if (algorithm == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("algorithm");
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement element = xmlDocument.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.CanonicalizationAlgorithm.Value, this.DriverDictionary.Namespace.Value);
        element.AppendChild((XmlNode) xmlDocument.CreateTextNode(algorithm));
        return element;
      }

      internal override bool IsCanonicalizationAlgorithmElement(XmlElement element, out string canonicalizationAlgorithm)
      {
        return WSTrust.CheckElement(element, this.DriverDictionary.CanonicalizationAlgorithm.Value, this.DriverDictionary.Namespace.Value, out canonicalizationAlgorithm);
      }

      public override bool TryParseRequiredClaimsElement(XmlElement element, out Collection<XmlElement> requiredClaims)
      {
        if (element == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
        if (element.LocalName == this.DriverDictionary.Claims.Value && element.NamespaceURI == this.DriverDictionary.Namespace.Value)
        {
          requiredClaims = new Collection<XmlElement>();
          foreach (XmlNode childNode in element.ChildNodes)
          {
            if (childNode is XmlElement)
              requiredClaims.Add((XmlElement) childNode);
          }
          return true;
        }
        requiredClaims = (Collection<XmlElement>) null;
        return false;
      }

      public override XmlElement CreateRequiredClaimsElement(IEnumerable<XmlElement> claimsList)
      {
        if (claimsList == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("claimsList");
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement element = xmlDocument.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.Claims.Value, this.DriverDictionary.Namespace.Value);
        foreach (XmlElement claims in claimsList)
        {
          XmlElement xmlElement = (XmlElement) xmlDocument.ImportNode((XmlNode) claims, true);
          element.AppendChild((XmlNode) xmlElement);
        }
        return element;
      }

      internal static void ValidateRequestedKeySize(int keySize, SecurityAlgorithmSuite algorithmSuite)
      {
        if (keySize % 8 != 0 || !algorithmSuite.IsSymmetricKeyLengthSupported(keySize))
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new SecurityNegotiationException(SR.GetString("InvalidKeyLengthRequested", new object[1]{ (object) keySize })));
      }

      private static void ValidateRequestorEntropy(SecurityToken entropy, SecurityKeyEntropyMode mode)
      {
        if ((mode == SecurityKeyEntropyMode.ClientEntropy || mode == SecurityKeyEntropyMode.CombinedEntropy) && entropy == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR.GetString("EntropyModeRequiresRequestorEntropy", new object[1]{ (object) mode })));
        if (mode == SecurityKeyEntropyMode.ServerEntropy && entropy != null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR.GetString("EntropyModeCannotHaveRequestorEntropy", new object[1]{ (object) mode })));
      }

      internal static void ProcessRstAndIssueKey(RequestSecurityToken requestSecurityToken, SecurityTokenResolver resolver, SecurityKeyEntropyMode keyEntropyMode, SecurityAlgorithmSuite algorithmSuite, out int issuedKeySize, out byte[] issuerEntropy, out byte[] proofKey, out SecurityToken proofToken)
      {
        SecurityToken requestorEntropy1 = requestSecurityToken.GetRequestorEntropy(resolver);
        WSTrust.Driver.ValidateRequestorEntropy(requestorEntropy1, keyEntropyMode);
        byte[] requestorEntropy2;
        if (requestorEntropy1 != null)
        {
          if (requestorEntropy1 is BinarySecretSecurityToken)
          {
            requestorEntropy2 = ((BinarySecretSecurityToken) requestorEntropy1).GetKeyBytes();
          }
#if !FEATURE_CORECLR
          else if (requestorEntropy1 is WrappedKeySecurityToken)
            requestorEntropy2 = ((WrappedKeySecurityToken) requestorEntropy1).GetWrappedKey();
#endif
          else
          {
            Console.WriteLine("TODO WrappedKeySecurityToken not supported in .NET Core!");
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR.GetString("TokenCannotCreateSymmetricCrypto", new object[1]{ (object) requestorEntropy1 })));
          }
        }
        else
          requestorEntropy2 = (byte[]) null;
        if (keyEntropyMode == SecurityKeyEntropyMode.ClientEntropy)
        {
          if (requestorEntropy2 != null)
            WSTrust.Driver.ValidateRequestedKeySize(requestorEntropy2.Length * 8, algorithmSuite);
          proofKey = requestorEntropy2;
          issuerEntropy = (byte[]) null;
          issuedKeySize = 0;
          proofToken = (SecurityToken) null;
        }
        else
        {
          if (requestSecurityToken.KeySize != 0)
          {
            WSTrust.Driver.ValidateRequestedKeySize(requestSecurityToken.KeySize, algorithmSuite);
            issuedKeySize = requestSecurityToken.KeySize;
          }
          else
            issuedKeySize = algorithmSuite.DefaultSymmetricKeyLength;
          RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider();
          if (keyEntropyMode == SecurityKeyEntropyMode.ServerEntropy)
          {
            proofKey = new byte[issuedKeySize / 8];
            cryptoServiceProvider.GetNonZeroBytes(proofKey);
            issuerEntropy = (byte[]) null;
            proofToken = (SecurityToken) new BinarySecretSecurityToken(proofKey);
          }
          else
          {
            issuerEntropy = new byte[issuedKeySize / 8];
            cryptoServiceProvider.GetNonZeroBytes(issuerEntropy);
            proofKey = RequestSecurityTokenResponse.ComputeCombinedKey(requestorEntropy2, issuerEntropy, issuedKeySize);
            proofToken = (SecurityToken) null;
          }
        }
      }
    }
  }
}
