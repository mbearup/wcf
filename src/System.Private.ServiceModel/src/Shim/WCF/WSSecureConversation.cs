// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.WSSecureConversation
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
  internal abstract class WSSecureConversation : WSSecurityTokenSerializer.SerializerEntries
  {
    private WSSecurityTokenSerializer tokenSerializer;
    private WSSecureConversation.DerivedKeyTokenEntry derivedKeyEntry;

    public abstract SecureConversationDictionary SerializerDictionary { get; }

    public WSSecurityTokenSerializer WSSecurityTokenSerializer
    {
      get
      {
        return this.tokenSerializer;
      }
    }

    public virtual string DerivationAlgorithm
    {
      get
      {
        return "http://schemas.xmlsoap.org/ws/2005/02/sc/dk/p_sha1";
      }
    }

    protected WSSecureConversation(WSSecurityTokenSerializer tokenSerializer, int maxKeyDerivationOffset, int maxKeyDerivationLabelLength, int maxKeyDerivationNonceLength)
    {
      if (tokenSerializer == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenSerializer");
      this.tokenSerializer = tokenSerializer;
      this.derivedKeyEntry = new WSSecureConversation.DerivedKeyTokenEntry(this, maxKeyDerivationOffset, maxKeyDerivationLabelLength, maxKeyDerivationNonceLength);
    }

    public override void PopulateTokenEntries(IList<WSSecurityTokenSerializer.TokenEntry> tokenEntryList)
    {
      if (tokenEntryList == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenEntryList");
      tokenEntryList.Add((WSSecurityTokenSerializer.TokenEntry) this.derivedKeyEntry);
    }

    public virtual bool IsAtDerivedKeyToken(XmlDictionaryReader reader)
    {
      return this.derivedKeyEntry.CanReadTokenCore(reader);
    }

    public virtual void ReadDerivedKeyTokenParameters(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver, out string id, out string derivationAlgorithm, out string label, out int length, out byte[] nonce, out int offset, out int generation, out SecurityKeyIdentifierClause tokenToDeriveIdentifier, out SecurityToken tokenToDerive)
    {
      this.derivedKeyEntry.ReadDerivedKeyTokenParameters(reader, tokenResolver, out id, out derivationAlgorithm, out label, out length, out nonce, out offset, out generation, out tokenToDeriveIdentifier, out tokenToDerive);
    }

    public virtual SecurityToken CreateDerivedKeyToken(string id, string derivationAlgorithm, string label, int length, byte[] nonce, int offset, int generation, SecurityKeyIdentifierClause tokenToDeriveIdentifier, SecurityToken tokenToDerive)
    {
      return this.derivedKeyEntry.CreateDerivedKeyToken(id, derivationAlgorithm, label, length, nonce, offset, generation, tokenToDeriveIdentifier, tokenToDerive);
    }

    protected class DerivedKeyTokenEntry : WSSecurityTokenSerializer.TokenEntry
    {
      public const string DefaultLabel = "WS-SecureConversation";
      private WSSecureConversation parent;
      private int maxKeyDerivationOffset;
      private int maxKeyDerivationLabelLength;
      private int maxKeyDerivationNonceLength;

      protected override XmlDictionaryString LocalName
      {
        get
        {
          return this.parent.SerializerDictionary.DerivedKeyToken;
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
          return this.parent.SerializerDictionary.DerivedKeyTokenType.Value;
        }
      }

      protected override string ValueTypeUri
      {
        get
        {
          return (string) null;
        }
      }

      public DerivedKeyTokenEntry(WSSecureConversation parent, int maxKeyDerivationOffset, int maxKeyDerivationLabelLength, int maxKeyDerivationNonceLength)
      {
        if (parent == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parent");
        this.parent = parent;
        this.maxKeyDerivationOffset = maxKeyDerivationOffset;
        this.maxKeyDerivationLabelLength = maxKeyDerivationLabelLength;
        this.maxKeyDerivationNonceLength = maxKeyDerivationNonceLength;
      }

      protected override Type[] GetTokenTypesCore()
      {
        return new Type[1]{ typeof (DerivedKeySecurityToken) };
      }

      public override SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXmlCore(XmlElement issuedTokenXml, SecurityTokenReferenceStyle tokenReferenceStyle)
      {
        TokenReferenceStyleHelper.Validate(tokenReferenceStyle);
        if (tokenReferenceStyle == SecurityTokenReferenceStyle.Internal)
          return WSSecurityTokenSerializer.TokenEntry.CreateDirectReference(issuedTokenXml, "Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", typeof (DerivedKeySecurityToken));
        if (tokenReferenceStyle == SecurityTokenReferenceStyle.External)
          return (SecurityKeyIdentifierClause) null;
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("tokenReferenceStyle"));
      }

      public virtual void ReadDerivedKeyTokenParameters(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver, out string id, out string derivationAlgorithm, out string label, out int length, out byte[] nonce, out int offset, out int generation, out SecurityKeyIdentifierClause tokenToDeriveIdentifier, out SecurityToken tokenToDerive)
      {
        if (tokenResolver == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenResolver");
        id = reader.GetAttribute(XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace);
        derivationAlgorithm = reader.GetAttribute(XD.XmlSignatureDictionary.Algorithm, (XmlDictionaryString) null);
        if (derivationAlgorithm == null)
          derivationAlgorithm = this.parent.DerivationAlgorithm;
        reader.ReadStartElement();
        tokenToDeriveIdentifier = (SecurityKeyIdentifierClause) null;
        tokenToDerive = (SecurityToken) null;
        if (!reader.IsStartElement(XD.SecurityJan2004Dictionary.SecurityTokenReference, XD.SecurityJan2004Dictionary.Namespace))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new XmlException(SR.GetString("DerivedKeyTokenRequiresTokenReference")));
        tokenToDeriveIdentifier = this.parent.WSSecurityTokenSerializer.ReadKeyIdentifierClause((XmlReader) reader);
        tokenResolver.TryResolveToken(tokenToDeriveIdentifier, out tokenToDerive);
        generation = -1;
        if (reader.IsStartElement(this.parent.SerializerDictionary.Generation, this.parent.SerializerDictionary.Namespace))
        {
          reader.ReadStartElement();
          generation = reader.ReadContentAsInt();
          reader.ReadEndElement();
          if (generation < 0)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new XmlException(SR.GetString("DerivedKeyInvalidGenerationSpecified", new object[1]{ (object) generation })));
        }
        offset = -1;
        if (reader.IsStartElement(this.parent.SerializerDictionary.Offset, this.parent.SerializerDictionary.Namespace))
        {
          reader.ReadStartElement();
          offset = reader.ReadContentAsInt();
          reader.ReadEndElement();
          if (offset < 0)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new XmlException(SR.GetString("DerivedKeyInvalidOffsetSpecified", new object[1]{ (object) offset })));
        }
        length = 32;
        if (reader.IsStartElement(this.parent.SerializerDictionary.Length, this.parent.SerializerDictionary.Namespace))
        {
          reader.ReadStartElement();
          length = reader.ReadContentAsInt();
          reader.ReadEndElement();
        }
        if (offset == -1 && generation == -1)
          offset = 0;
        DerivedKeySecurityToken.EnsureAcceptableOffset(offset, generation, length, this.maxKeyDerivationOffset);
        label = (string) null;
        if (reader.IsStartElement(this.parent.SerializerDictionary.Label, this.parent.SerializerDictionary.Namespace))
        {
          reader.ReadStartElement();
          label = reader.ReadString();
          reader.ReadEndElement();
        }
        if (label != null && label.Length > this.maxKeyDerivationLabelLength)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("DerivedKeyTokenLabelTooLong", (object) label.Length, (object) this.maxKeyDerivationLabelLength)));
        nonce = (byte[]) null;
        reader.ReadStartElement(this.parent.SerializerDictionary.Nonce, this.parent.SerializerDictionary.Namespace);
        nonce = reader.ReadContentAsBase64();
        reader.ReadEndElement();
        if (nonce != null && nonce.Length > this.maxKeyDerivationNonceLength)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("DerivedKeyTokenNonceTooLong", (object) nonce.Length, (object) this.maxKeyDerivationNonceLength)));
        reader.ReadEndElement();
      }

      public virtual SecurityToken CreateDerivedKeyToken(string id, string derivationAlgorithm, string label, int length, byte[] nonce, int offset, int generation, SecurityKeyIdentifierClause tokenToDeriveIdentifier, SecurityToken tokenToDerive)
      {

        if (tokenToDerive == null)
		{
#if FEATURE_CORECLR
			throw new NotImplementedException("DerivedKeySecurityTokenStub not supported in .NET Core");
#else
			return (SecurityToken) new DerivedKeySecurityTokenStub(generation, offset, length, label, nonce, tokenToDeriveIdentifier, derivationAlgorithm, id);
#endif
		}
        return (SecurityToken) new DerivedKeySecurityToken(generation, offset, length, label, nonce, tokenToDerive, tokenToDeriveIdentifier, derivationAlgorithm, id);
      }

      public override SecurityToken ReadTokenCore(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver)
      {
        string id;
        string derivationAlgorithm;
        string label;
        int length;
        byte[] nonce;
        int offset;
        int generation;
        SecurityKeyIdentifierClause tokenToDeriveIdentifier;
        SecurityToken tokenToDerive;
        this.ReadDerivedKeyTokenParameters(reader, tokenResolver, out id, out derivationAlgorithm, out label, out length, out nonce, out offset, out generation, out tokenToDeriveIdentifier, out tokenToDerive);
        return this.CreateDerivedKeyToken(id, derivationAlgorithm, label, length, nonce, offset, generation, tokenToDeriveIdentifier, tokenToDerive);
      }

      public override void WriteTokenCore(XmlDictionaryWriter writer, SecurityToken token)
      {
        DerivedKeySecurityToken keySecurityToken = token as DerivedKeySecurityToken;
        if (keySecurityToken == null)
        {
            string tokenType = token.GetType().ToString();
            throw new Exception("token type is incompatible with DerivedKeySecurityToken:  " + tokenType);
        }
       
        string prefix = this.parent.SerializerDictionary.Prefix.Value;
        writer.WriteStartElement(prefix, this.parent.SerializerDictionary.DerivedKeyToken, this.parent.SerializerDictionary.Namespace);
        if (keySecurityToken.Id != null)
          writer.WriteAttributeString(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace, keySecurityToken.Id);
        if (keySecurityToken.KeyDerivationAlgorithm != this.parent.DerivationAlgorithm)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnsupportedKeyDerivationAlgorithm", new object[1]{ (object) keySecurityToken.KeyDerivationAlgorithm })));
        this.parent.WSSecurityTokenSerializer.WriteKeyIdentifierClause((XmlWriter) writer, keySecurityToken.TokenToDeriveIdentifier);
        if (keySecurityToken.Generation > 0 || keySecurityToken.Offset > 0 || keySecurityToken.Length != 32)
        {
          if (keySecurityToken.Generation >= 0 && keySecurityToken.Offset >= 0)
          {
            writer.WriteStartElement(prefix, this.parent.SerializerDictionary.Generation, this.parent.SerializerDictionary.Namespace);
            writer.WriteValue(keySecurityToken.Generation);
            writer.WriteEndElement();
          }
          else if (keySecurityToken.Generation != -1)
          {
            writer.WriteStartElement(prefix, this.parent.SerializerDictionary.Generation, this.parent.SerializerDictionary.Namespace);
            writer.WriteValue(keySecurityToken.Generation);
            writer.WriteEndElement();
          }
          else if (keySecurityToken.Offset != -1)
          {
            writer.WriteStartElement(prefix, this.parent.SerializerDictionary.Offset, this.parent.SerializerDictionary.Namespace);
            writer.WriteValue(keySecurityToken.Offset);
            writer.WriteEndElement();
          }
          if (keySecurityToken.Length != 32)
          {
            writer.WriteStartElement(prefix, this.parent.SerializerDictionary.Length, this.parent.SerializerDictionary.Namespace);
            writer.WriteValue(keySecurityToken.Length);
            writer.WriteEndElement();
          }
        }
        if (keySecurityToken.Label != null)
        {
          writer.WriteStartElement(prefix, this.parent.SerializerDictionary.Generation, this.parent.SerializerDictionary.Namespace);
          writer.WriteString(keySecurityToken.Label);
          writer.WriteEndElement();
        }
        writer.WriteStartElement(prefix, this.parent.SerializerDictionary.Nonce, this.parent.SerializerDictionary.Namespace);
        writer.WriteBase64(keySecurityToken.Nonce, 0, keySecurityToken.Nonce.Length);
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
    }

    protected abstract class SecurityContextTokenEntry : WSSecurityTokenSerializer.TokenEntry
    {
      private WSSecureConversation parent;
      private SecurityContextCookieSerializer cookieSerializer;

      protected WSSecureConversation Parent
      {
        get
        {
          return this.parent;
        }
      }

      protected override XmlDictionaryString LocalName
      {
        get
        {
          return this.parent.SerializerDictionary.SecurityContextToken;
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
          return this.parent.SerializerDictionary.SecurityContextTokenType.Value;
        }
      }

      protected override string ValueTypeUri
      {
        get
        {
          return (string) null;
        }
      }

      public SecurityContextTokenEntry(WSSecureConversation parent, SecurityStateEncoder securityStateEncoder, IList<Type> knownClaimTypes)
      {
        this.parent = parent;
        this.cookieSerializer = new SecurityContextCookieSerializer(securityStateEncoder, knownClaimTypes);
      }

      protected override Type[] GetTokenTypesCore()
      {
        return new Type[1]{ typeof (SecurityContextSecurityToken) };
      }

      public override SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXmlCore(XmlElement issuedTokenXml, SecurityTokenReferenceStyle tokenReferenceStyle)
      {
        TokenReferenceStyleHelper.Validate(tokenReferenceStyle);
        if (tokenReferenceStyle == SecurityTokenReferenceStyle.Internal)
          return WSSecurityTokenSerializer.TokenEntry.CreateDirectReference(issuedTokenXml, "Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", typeof (SecurityContextSecurityToken));
        if (tokenReferenceStyle != SecurityTokenReferenceStyle.External)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("tokenReferenceStyle"));
        UniqueId contextId = (UniqueId) null;
        UniqueId generation = (UniqueId) null;
        foreach (XmlNode childNode in issuedTokenXml.ChildNodes)
        {
          XmlElement element = childNode as XmlElement;
          if (element != null)
          {
            if (element.LocalName == this.parent.SerializerDictionary.Identifier.Value && element.NamespaceURI == this.parent.SerializerDictionary.Namespace.Value)
              contextId = XmlHelper.ReadTextElementAsUniqueId(element);
            else if (this.CanReadGeneration(element))
              generation = this.ReadGeneration(element);
          }
        }
        return (SecurityKeyIdentifierClause) new SecurityContextKeyIdentifierClause(contextId, generation);
      }

      protected abstract bool CanReadGeneration(XmlDictionaryReader reader);

      protected abstract bool CanReadGeneration(XmlElement element);

      protected abstract UniqueId ReadGeneration(XmlDictionaryReader reader);

      protected abstract UniqueId ReadGeneration(XmlElement element);

      private SecurityContextSecurityToken TryResolveSecurityContextToken(UniqueId contextId, UniqueId generation, string id, SecurityTokenResolver tokenResolver, out ISecurityContextSecurityTokenCache sctCache)
      {
        SecurityContextSecurityToken sourceToken = (SecurityContextSecurityToken) null;
        sctCache = (ISecurityContextSecurityTokenCache) null;
        if (tokenResolver is ISecurityContextSecurityTokenCache)
        {
          sctCache = (ISecurityContextSecurityTokenCache) tokenResolver;
          sourceToken = sctCache.GetContext(contextId, generation);
        }
        else 
        {
#if FEATURE_CORECLR
            throw new NotImplementedException("AggregateSecurityHeaderTokenResolver not supported in .NET Core");
#else
            if (tokenResolver is AggregateSecurityHeaderTokenResolver)
            {
              AggregateSecurityHeaderTokenResolver headerTokenResolver = tokenResolver as AggregateSecurityHeaderTokenResolver;
              for (int index = 0; index < headerTokenResolver.TokenResolvers.Count; ++index)
              {
                ISecurityContextSecurityTokenCache tokenResolver1 = headerTokenResolver.TokenResolvers[index] as ISecurityContextSecurityTokenCache;
                if (tokenResolver1 != null)
                {
                  if (sctCache == null)
                    sctCache = tokenResolver1;
                  sourceToken = tokenResolver1.GetContext(contextId, generation);
                  if (sourceToken != null)
                    break;
                }
              }
            }
#endif
        }
        
        if (sourceToken == null)
          return (SecurityContextSecurityToken) null;
        if (sourceToken.Id == id)
          return sourceToken;
        return new SecurityContextSecurityToken(sourceToken, id);
      }

      public override SecurityToken ReadTokenCore(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver)
      {
        UniqueId generation = (UniqueId) null;
        string attribute = reader.GetAttribute(XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace);
        reader.ReadFullStartElement();
        reader.MoveToStartElement(this.parent.SerializerDictionary.Identifier, this.parent.SerializerDictionary.Namespace);
        UniqueId contextId = reader.ReadElementContentAsUniqueId();
        if (this.CanReadGeneration(reader))
          generation = this.ReadGeneration(reader);
#if FEATURE_CORECLR
        throw new NotImplementedException("DotNetSecurityDictionary not implemented in .NET Core");
#else
        bool flag = false;
        SecurityContextSecurityToken token = (SecurityContextSecurityToken) null;
        if (reader.IsStartElement(this.parent.SerializerDictionary.Cookie, DotNetSecurityDictionary.Namespace))
        {
          flag = true;
          ISecurityContextSecurityTokenCache sctCache;
          token = this.TryResolveSecurityContextToken(contextId, generation, attribute, tokenResolver, out sctCache);
          if (token == null)
          {
            byte[] encodedCookie = reader.ReadElementContentAsBase64();
            if (encodedCookie != null)
            {
              token = this.cookieSerializer.CreateSecurityContextFromCookie(encodedCookie, contextId, generation, attribute, reader.Quotas);
              if (sctCache != null)
                sctCache.AddContext(token);
            }
          }
          else
            reader.Skip();
        }
        reader.ReadEndElement();
        if (contextId == (UniqueId) null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("NoSecurityContextIdentifier")));
        if (token == null && !flag)
        {
          ISecurityContextSecurityTokenCache sctCache;
          token = this.TryResolveSecurityContextToken(contextId, generation, attribute, tokenResolver, out sctCache);
        }
        if (token == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new SecurityContextTokenValidationException(SR.GetString("SecurityContextNotRegistered", (object) contextId, (object) generation)));
        return (SecurityToken) token;
#endif
      }

      protected virtual void WriteGeneration(XmlDictionaryWriter writer, SecurityContextSecurityToken sct)
      {
      }

      public override void WriteTokenCore(XmlDictionaryWriter writer, SecurityToken token)
      {
        SecurityContextSecurityToken sct = token as SecurityContextSecurityToken;
        writer.WriteStartElement(this.parent.SerializerDictionary.Prefix.Value, this.parent.SerializerDictionary.SecurityContextToken, this.parent.SerializerDictionary.Namespace);
        if (sct.Id != null)
          writer.WriteAttributeString(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace, sct.Id);
        writer.WriteStartElement(this.parent.SerializerDictionary.Prefix.Value, this.parent.SerializerDictionary.Identifier, this.parent.SerializerDictionary.Namespace);
        XmlHelper.WriteStringAsUniqueId(writer, sct.ContextId);
        writer.WriteEndElement();
        this.WriteGeneration(writer, sct);
        if (sct.IsCookieMode)
        {
          if (sct.CookieBlob == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("NoCookieInSct")));
#if FEATURE_CORECLR
          throw new NotImplementedException("XD.DotNetSecurityDictionary is not supported in .NET Core");
#else
          writer.WriteStartElement(XD.DotNetSecurityDictionary.Prefix.Value, this.parent.SerializerDictionary.Cookie, XD.DotNetSecurityDictionary.Namespace);
          writer.WriteBase64(sct.CookieBlob, 0, sct.CookieBlob.Length);
          writer.WriteEndElement();
#endif
        }
        writer.WriteEndElement();
      }
    }

    public abstract class Driver : SecureConversationDriver
    {
      protected abstract SecureConversationDictionary DriverDictionary { get; }

      public override XmlDictionaryString IssueAction
      {
        get
        {
          return this.DriverDictionary.RequestSecurityContextIssuance;
        }
      }

      public override XmlDictionaryString IssueResponseAction
      {
        get
        {
          return this.DriverDictionary.RequestSecurityContextIssuanceResponse;
        }
      }

      public override XmlDictionaryString RenewNeededFaultCode
      {
        get
        {
          return this.DriverDictionary.RenewNeededFaultCode;
        }
      }

      public override XmlDictionaryString BadContextTokenFaultCode
      {
        get
        {
          return this.DriverDictionary.BadContextTokenFaultCode;
        }
      }

      public override UniqueId GetSecurityContextTokenId(XmlDictionaryReader reader)
      {
        if (reader == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");
        reader.ReadStartElement(this.DriverDictionary.SecurityContextToken, this.DriverDictionary.Namespace);
        UniqueId uniqueId = XmlHelper.ReadElementStringAsUniqueId(reader, this.DriverDictionary.Identifier, this.DriverDictionary.Namespace);
        while (reader.IsStartElement())
          reader.Skip();
        reader.ReadEndElement();
        return uniqueId;
      }

      public override bool IsAtSecurityContextToken(XmlDictionaryReader reader)
      {
        if (reader == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");
        return reader.IsStartElement(this.DriverDictionary.SecurityContextToken, this.DriverDictionary.Namespace);
      }
    }
  }
}
