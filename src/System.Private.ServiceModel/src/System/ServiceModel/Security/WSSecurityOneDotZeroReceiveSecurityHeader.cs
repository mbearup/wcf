// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.WSSecurityOneDotZeroReceiveSecurityHeader
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Diagnostics.Application;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
  internal class WSSecurityOneDotZeroReceiveSecurityHeader : ReceiveSecurityHeader
  {
    private WrappedKeySecurityToken pendingDecryptionToken;
    private ReferenceList pendingReferenceList;
    private SignedXml pendingSignature;
    private List<string> earlyDecryptedDataReferences;

    public WSSecurityOneDotZeroReceiveSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay, SecurityStandardsManager standardsManager, SecurityAlgorithmSuite algorithmSuite, int headerIndex, MessageDirection transferDirection)
      : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, headerIndex, transferDirection)
    {
    }

    protected static SymmetricAlgorithm CreateDecryptionAlgorithm(SecurityToken token, string encryptionMethod, SecurityAlgorithmSuite suite)
    {
      if (encryptionMethod == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("EncryptionMethodMissingInEncryptedData")));
#if FEATURE_CORECLR
	  throw new NotImplementedException("SecurityAlgorithmSuite.EnsureAcceptableEncryptionAlgorithm is not supported in .NET Core");
#else
      suite.EnsureAcceptableEncryptionAlgorithm(encryptionMethod);
      SymmetricSecurityKey securityKey = SecurityUtils.GetSecurityKey<SymmetricSecurityKey>(token);
      if (securityKey == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenCannotCreateSymmetricCrypto", new object[1]{ (object) token })));
      suite.EnsureAcceptableDecryptionSymmetricKeySize(securityKey, token);
      SymmetricAlgorithm symmetricAlgorithm = securityKey.GetSymmetricAlgorithm(encryptionMethod);
      if (symmetricAlgorithm == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnableToCreateSymmetricAlgorithmFromToken", new object[1]{ (object) encryptionMethod })));
      return symmetricAlgorithm;
#endif
    }

    private void DecryptBody(XmlDictionaryReader bodyContentReader, SecurityToken token)
    {
      EncryptedData encryptedData = new EncryptedData();
      encryptedData.ShouldReadXmlReferenceKeyInfoClause = this.MessageDirection == MessageDirection.Output;
      encryptedData.SecurityTokenSerializer = this.StandardsManager.SecurityTokenSerializer;
      encryptedData.ReadFrom(bodyContentReader, this.MaxReceivedMessageSize);
      if (!bodyContentReader.EOF && bodyContentReader.NodeType != XmlNodeType.EndElement)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new FormatException(SR.GetString("BadEncryptedBody")));
      if (token == null)
        token = WSSecurityOneDotZeroReceiveSecurityHeader.ResolveKeyIdentifier(encryptedData.KeyIdentifier, (SecurityTokenResolver) this.PrimaryTokenResolver, false);
      this.RecordEncryptionToken(token);
      using (SymmetricAlgorithm decryptionAlgorithm = WSSecurityOneDotZeroReceiveSecurityHeader.CreateDecryptionAlgorithm(token, encryptedData.EncryptionMethod, this.AlgorithmSuite))
      {
        encryptedData.SetUpDecryption(decryptionAlgorithm);
        this.SecurityVerifiedMessage.SetDecryptedBody(encryptedData.GetDecryptedBuffer());
      }
    }

    /*protected virtual DecryptedHeader DecryptHeader(XmlDictionaryReader reader, WrappedKeySecurityToken wrappedKeyToken)
    {
      throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("HeaderDecryptionNotSupportedInWsSecurityJan2004")));
    }*/

    internal override byte[] DecryptSecurityHeaderElement(EncryptedData encryptedData, WrappedKeySecurityToken wrappedKeyToken, out SecurityToken encryptionToken)
    {
      if (encryptedData.KeyIdentifier != null || wrappedKeyToken == null)
      {
        encryptionToken = WSSecurityOneDotZeroReceiveSecurityHeader.ResolveKeyIdentifier(encryptedData.KeyIdentifier, this.CombinedPrimaryTokenResolver, false);
        if (wrappedKeyToken != null && wrappedKeyToken.ReferenceList != null && (encryptedData.HasId && wrappedKeyToken.ReferenceList.ContainsReferredId(encryptedData.Id)) && wrappedKeyToken != encryptionToken)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("EncryptedKeyWasNotEncryptedWithTheRequiredEncryptingToken", new object[1]{ (object) wrappedKeyToken })));
      }
      else
        encryptionToken = (SecurityToken) wrappedKeyToken;
      using (SymmetricAlgorithm decryptionAlgorithm = WSSecurityOneDotZeroReceiveSecurityHeader.CreateDecryptionAlgorithm(encryptionToken, encryptedData.EncryptionMethod, this.AlgorithmSuite))
      {
        encryptedData.SetUpDecryption(decryptionAlgorithm);
        return encryptedData.GetDecryptedBuffer();
      }
    }

    protected override WrappedKeySecurityToken DecryptWrappedKey(XmlDictionaryReader reader)
    {
#if FEATURE_CORECLR
	  throw new NotImplementedException("SecurityAlgorithmSuite.EnsureAcceptableDigestAlgorithm is not supported in .NET Core");
#else
      if (TD.WrappedKeyDecryptionStartIsEnabled())
        TD.WrappedKeyDecryptionStart(this.EventTraceActivity);
      WrappedKeySecurityToken keySecurityToken = (WrappedKeySecurityToken) this.StandardsManager.SecurityTokenSerializer.ReadToken((XmlReader) reader, (SecurityTokenResolver) this.PrimaryTokenResolver);
      this.AlgorithmSuite.EnsureAcceptableKeyWrapAlgorithm(keySecurityToken.WrappingAlgorithm, keySecurityToken.WrappingSecurityKey is AsymmetricSecurityKey);
	  if (TD.WrappedKeyDecryptionSuccessIsEnabled())
        TD.WrappedKeyDecryptionSuccess(this.EventTraceActivity);
      return keySecurityToken;
#endif
    }

    private bool EnsureDigestValidityIfIdMatches(SignedInfo signedInfo, string id, XmlDictionaryReader reader, bool doSoapAttributeChecks, MessagePartSpecification signatureParts, MessageHeaderInfo info, bool checkForTokensAtHeaders)
    {
      if (signedInfo == null)
        return false;
      if (doSoapAttributeChecks)
        this.VerifySoapAttributeMatchForHeader(info, signatureParts, reader);
      bool flag1 = checkForTokensAtHeaders && this.StandardsManager.SecurityTokenSerializer.CanReadToken((XmlReader) reader);
      bool flag2;
      try
      {
        flag2 = signedInfo.EnsureDigestValidityIfIdMatches(id, (object) reader);
      }
      catch (CryptographicException ex)
      {
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("FailedSignatureVerification"), (Exception) ex));
      }
      if (flag2 & flag1)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("SecurityTokenFoundOutsideSecurityHeader", (object) info.Namespace, (object) info.Name)));
      return flag2;
    }

    protected override void ExecuteMessageProtectionPass(bool hasAtLeastOneSupportingTokenExpectedToBeSigned)
    {
      SignatureTargetIdManager idManager = this.StandardsManager.IdManager;
      MessagePartSpecification partSpecification = this.RequiredEncryptionParts ?? MessagePartSpecification.NoParts;
      MessagePartSpecification signatureParts = this.RequiredSignatureParts ?? MessagePartSpecification.NoParts;
      bool checkForTokensAtHeaders = hasAtLeastOneSupportingTokenExpectedToBeSigned;
      bool doSoapAttributeChecks = !signatureParts.IsBodyIncluded;
      bool encryptBeforeSignMode = this.EncryptBeforeSignMode;
      SignedInfo signedInfo = this.pendingSignature != null ? this.pendingSignature.Signature.SignedInfo : (SignedInfo) null;
      SignatureConfirmations signatureConfirmations = this.GetSentSignatureConfirmations();
      if (signatureConfirmations != null && signatureConfirmations.Count > 0 && signatureConfirmations.IsMarkedForEncryption)
        this.VerifySignatureEncryption();
      MessageHeaders headers = this.SecurityVerifiedMessage.Headers;
      XmlDictionaryReader readerAtFirstHeader = this.SecurityVerifiedMessage.GetReaderAtFirstHeader();
      bool flag1 = false;
      for (int headerIndex = 0; headerIndex < headers.Count; ++headerIndex)
      {
        if (readerAtFirstHeader.NodeType != XmlNodeType.Element)
        {
          int content = (int) readerAtFirstHeader.MoveToContent();
        }
        if (headerIndex == this.HeaderIndex)
        {
          readerAtFirstHeader.Skip();
        }
        else
        {
          bool flag2 = false;
          string id1 = idManager.ExtractId(readerAtFirstHeader);
          if (id1 != null)
            flag2 = this.TryDeleteReferenceListEntry(id1);
          if (!flag2 && readerAtFirstHeader.IsStartElement("EncryptedHeader", "http://docs.oasis-open.org/wss/oasis-wss-wssecurity-secext-1.1.xsd"))
          {
            XmlDictionaryReader readerAtHeader = headers.GetReaderAtHeader(headerIndex);
            readerAtHeader.ReadStartElement("EncryptedHeader", "http://docs.oasis-open.org/wss/oasis-wss-wssecurity-secext-1.1.xsd");
#if FEATURE_CORECLR
			throw new NotImplementedException("XD.XmlEncryptionDictionary is not supported in .NET Core");
#else
            if (readerAtHeader.IsStartElement(EncryptedData.ElementName, System.ServiceModel.XD.XmlEncryptionDictionary.Namespace))
            {
              string attribute = readerAtHeader.GetAttribute(System.ServiceModel.XD.XmlEncryptionDictionary.Id, (XmlDictionaryString) null);
              if (attribute != null && this.TryDeleteReferenceListEntry(attribute))
                flag2 = true;
            }
#endif
          }
          this.ElementManager.VerifyUniquenessAndSetHeaderId(id1, headerIndex);
          MessageHeaderInfo info = headers[headerIndex];
          if (!flag2 && partSpecification.IsHeaderIncluded(info.Name, info.Namespace))
            this.SecurityVerifiedMessage.OnUnencryptedPart(info.Name, info.Namespace);
          bool flag3 = !flag2 | encryptBeforeSignMode && id1 != null && this.EnsureDigestValidityIfIdMatches(signedInfo, id1, readerAtFirstHeader, doSoapAttributeChecks, signatureParts, info, checkForTokensAtHeaders);
          if (flag2)
          {
#if FEATURE_CORECLR
			throw new NotImplementedException("DecryptedHeader is not supported in .NET Core");
#else
            XmlDictionaryReader reader = flag3 ? headers.GetReaderAtHeader(headerIndex) : readerAtFirstHeader;
            DecryptedHeader decryptedHeader = this.DecryptHeader(reader, this.pendingDecryptionToken);
            info = (MessageHeaderInfo) decryptedHeader;
            string id2 = decryptedHeader.Id;
            this.ElementManager.VerifyUniquenessAndSetDecryptedHeaderId(id2, headerIndex);
            headers.ReplaceAt(headerIndex, (MessageHeader) decryptedHeader);
            if (reader != readerAtFirstHeader)
              reader.Close();
            if (!encryptBeforeSignMode && id2 != null)
            {
              XmlDictionaryReader headerReader = decryptedHeader.GetHeaderReader();
              flag3 = this.EnsureDigestValidityIfIdMatches(signedInfo, id2, headerReader, doSoapAttributeChecks, signatureParts, info, checkForTokensAtHeaders);
              headerReader.Close();
            }
#endif
          }
          if (!flag3 && signatureParts.IsHeaderIncluded(info.Name, info.Namespace))
            this.SecurityVerifiedMessage.OnUnsignedPart(info.Name, info.Namespace);
          if (flag3 & flag2)
            this.VerifySignatureEncryption();
          if (flag2 && !flag3)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("EncryptedHeaderNotSigned", (object) info.Name, (object) info.Namespace)));
          if (!flag3 && !flag2)
            readerAtFirstHeader.Skip();
          flag1 |= flag2;
        }
      }
      readerAtFirstHeader.ReadEndElement();
      if (readerAtFirstHeader.NodeType != XmlNodeType.Element)
      {
        int content1 = (int) readerAtFirstHeader.MoveToContent();
      }
      string id3 = idManager.ExtractId(readerAtFirstHeader);
      this.ElementManager.VerifyUniquenessAndSetBodyId(id3);
      this.SecurityVerifiedMessage.SetBodyPrefixAndAttributes(readerAtFirstHeader);
      bool flag4 = partSpecification.IsBodyIncluded || this.HasPendingDecryptionItem();
      bool flag5 = !flag4 | encryptBeforeSignMode && id3 != null && this.EnsureDigestValidityIfIdMatches(signedInfo, id3, readerAtFirstHeader, false, (MessagePartSpecification) null, (MessageHeaderInfo) null, false);
      bool flag6;
      if (flag4)
      {
        XmlDictionaryReader dictionaryReader = flag5 ? this.SecurityVerifiedMessage.CreateFullBodyReader() : readerAtFirstHeader;
        dictionaryReader.ReadStartElement();
        string id1 = idManager.ExtractId(dictionaryReader);
        this.ElementManager.VerifyUniquenessAndSetBodyContentId(id1);
        flag6 = id1 != null && this.TryDeleteReferenceListEntry(id1);
        if (flag6)
          this.DecryptBody(dictionaryReader, (SecurityToken) this.pendingDecryptionToken);
        if (dictionaryReader != readerAtFirstHeader)
          dictionaryReader.Close();
        if (!encryptBeforeSignMode && signedInfo != null && signedInfo.HasUnverifiedReference(id3))
        {
          XmlDictionaryReader fullBodyReader = this.SecurityVerifiedMessage.CreateFullBodyReader();
          flag5 = this.EnsureDigestValidityIfIdMatches(signedInfo, id3, fullBodyReader, false, (MessagePartSpecification) null, (MessageHeaderInfo) null, false);
          fullBodyReader.Close();
        }
      }
      else
        flag6 = false;
      if (flag5 & flag6)
        this.VerifySignatureEncryption();
      readerAtFirstHeader.Close();
      if (this.pendingSignature != null)
      {
        this.pendingSignature.CompleteSignatureVerification();
        this.pendingSignature = (SignedXml) null;
      }
      this.pendingDecryptionToken = (WrappedKeySecurityToken) null;
      bool atLeastOneHeaderOrBodyEncrypted = flag1 | flag6;
      if (!flag5 && signatureParts.IsBodyIncluded)
	  {
#if FEATURE_CORECLR
		throw new NotImplementedException("XD.XmlEncryptionDictionary is not supported in .NET Core");
#else
        this.SecurityVerifiedMessage.OnUnsignedPart(System.ServiceModel.XD.MessageDictionary.Body.Value, this.Version.Envelope.Namespace);
#endif
	  }
      if (!flag6 && partSpecification.IsBodyIncluded)
	  {
#if FEATURE_CORECLR
		throw new NotImplementedException("XD.XmlEncryptionDictionary is not supported in .NET Core");
#else
        this.SecurityVerifiedMessage.OnUnencryptedPart(System.ServiceModel.XD.MessageDictionary.Body.Value, this.Version.Envelope.Namespace);
#endif
	  }
      this.SecurityVerifiedMessage.OnMessageProtectionPassComplete(atLeastOneHeaderOrBodyEncrypted);
    }

    protected override bool IsReaderAtEncryptedData(XmlDictionaryReader reader)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("XD.XmlEncryptionDictionary is not supported in .NET Core");
#else
      bool flag = reader.IsStartElement(EncryptedData.ElementName, System.ServiceModel.XD.XmlEncryptionDictionary.Namespace);
      if (flag)
        this.HasAtLeastOneItemInsideSecurityHeaderEncrypted = true;
      return flag;
#endif
    }

    protected override bool IsReaderAtEncryptedKey(XmlDictionaryReader reader)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("XD.XmlEncryptionDictionary is not supported in .NET Core");
#else
      return reader.IsStartElement(EncryptedKey.ElementName, System.ServiceModel.XD.XmlEncryptionDictionary.Namespace);
#endif
    }

    protected override bool IsReaderAtReferenceList(XmlDictionaryReader reader)
    {
      return reader.IsStartElement(ReferenceList.ElementName, ReferenceList.NamespaceUri);
    }

    protected override bool IsReaderAtSignature(XmlDictionaryReader reader)
    {
      return reader.IsStartElement(System.ServiceModel.XD.XmlSignatureDictionary.Signature, System.ServiceModel.XD.XmlSignatureDictionary.Namespace);
    }

    protected override bool IsReaderAtSecurityTokenReference(XmlDictionaryReader reader)
    {
      return reader.IsStartElement(System.ServiceModel.XD.SecurityJan2004Dictionary.SecurityTokenReference, System.ServiceModel.XD.SecurityJan2004Dictionary.Namespace);
    }

    protected override void ProcessReferenceListCore(ReferenceList referenceList, WrappedKeySecurityToken wrappedKeyToken)
    {
      this.pendingReferenceList = referenceList;
      this.pendingDecryptionToken = wrappedKeyToken;
    }

    protected override ReferenceList ReadReferenceListCore(XmlDictionaryReader reader)
    {
      ReferenceList referenceList = new ReferenceList();
      referenceList.ReadFrom(reader);
      return referenceList;
    }

    internal override EncryptedData ReadSecurityHeaderEncryptedItem(XmlDictionaryReader reader, bool readXmlreferenceKeyInfoClause)
    {
      EncryptedData encryptedData = new EncryptedData();
      encryptedData.ShouldReadXmlReferenceKeyInfoClause = readXmlreferenceKeyInfoClause;
      encryptedData.SecurityTokenSerializer = this.StandardsManager.SecurityTokenSerializer;
      encryptedData.ReadFrom(reader);
      return encryptedData;
    }

    internal override SignedXml ReadSignatureCore(XmlDictionaryReader signatureReader)
    {
      SignedXml signedXml = new SignedXml(ServiceModelDictionaryManager.Instance, this.StandardsManager.SecurityTokenSerializer);
      signedXml.Signature.SignedInfo.ResourcePool = this.ResourcePool;
      signedXml.ReadFrom(signatureReader);
      return signedXml;
    }

    protected static bool TryResolveKeyIdentifier(SecurityKeyIdentifier keyIdentifier, SecurityTokenResolver resolver, bool isFromSignature, out SecurityToken token)
    {
      if (keyIdentifier != null)
        return resolver.TryResolveToken(keyIdentifier, out token);
      if (isFromSignature)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("NoKeyInfoInSignatureToFindVerificationToken")));
      throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("NoKeyInfoInEncryptedItemToFindDecryptingToken")));
    }

    protected static SecurityToken ResolveKeyIdentifier(SecurityKeyIdentifier keyIdentifier, SecurityTokenResolver resolver, bool isFromSignature)
    {
      SecurityToken token;
      if (WSSecurityOneDotZeroReceiveSecurityHeader.TryResolveKeyIdentifier(keyIdentifier, resolver, isFromSignature, out token))
        return token;
      if (isFromSignature)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnableToResolveKeyInfoForVerifyingSignature", (object) keyIdentifier, (object) resolver)));
      throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnableToResolveKeyInfoForDecryption", (object) keyIdentifier, (object) resolver)));
    }

    private SecurityToken ResolveSignatureToken(SecurityKeyIdentifier keyIdentifier, SecurityTokenResolver resolver, bool isPrimarySignature)
    {
      SecurityToken token;
      WSSecurityOneDotZeroReceiveSecurityHeader.TryResolveKeyIdentifier(keyIdentifier, resolver, true, out token);
#if FEATURE_CORECLR
	  throw new NotImplementedException("RsaKeyIdentifierClause is not supported in .NET Core");
#else
      RsaKeyIdentifierClause clause;
      if (token == null && !isPrimarySignature && (keyIdentifier.Count == 1 && keyIdentifier.TryFind<RsaKeyIdentifierClause>(out clause)))
      {
        RsaSecurityTokenAuthenticator allowedAuthenticator = this.FindAllowedAuthenticator<RsaSecurityTokenAuthenticator>(false);
        if (allowedAuthenticator != null)
        {
          token = (SecurityToken) new RsaSecurityToken(clause.Rsa);
          ReadOnlyCollection<IAuthorizationPolicy> readOnlyCollection = allowedAuthenticator.ValidateToken(token);
          SupportingTokenAuthenticatorSpecification spec;
          TokenTracker supportingTokenTracker = this.GetSupportingTokenTracker((SecurityTokenAuthenticator) allowedAuthenticator, out spec);
          if (supportingTokenTracker == null)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("UnknownTokenAuthenticatorUsedInTokenProcessing", new object[1]{ (object) allowedAuthenticator })));
          supportingTokenTracker.RecordToken(token);
          this.SecurityTokenAuthorizationPoliciesMapping.Add(token, readOnlyCollection);
        }
      }
      if (token == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnableToResolveKeyInfoForVerifyingSignature", (object) keyIdentifier, (object) resolver)));
      return token;
#endif
    }

    protected override void ReadSecurityTokenReference(XmlDictionaryReader reader)
    {
      string attribute = reader.GetAttribute(System.ServiceModel.XD.UtilityDictionary.IdAttribute, System.ServiceModel.XD.UtilityDictionary.Namespace);
      SecurityKeyIdentifierClause strClause = this.StandardsManager.SecurityTokenSerializer.ReadKeyIdentifierClause((XmlReader) reader);
      if (string.IsNullOrEmpty(strClause.Id))
        strClause.Id = attribute;
      if (string.IsNullOrEmpty(strClause.Id))
        return;
      this.ElementManager.AppendSecurityTokenReference(strClause, strClause.Id);
    }

    private bool HasPendingDecryptionItem()
    {
      if (this.pendingReferenceList != null)
        return this.pendingReferenceList.DataReferenceCount > 0;
      return false;
    }

    protected override bool TryDeleteReferenceListEntry(string id)
    {
      if (this.pendingReferenceList != null)
        return this.pendingReferenceList.TryRemoveReferredId(id);
      return false;
    }

    protected override void EnsureDecryptionComplete()
    {
      if (this.earlyDecryptedDataReferences != null)
      {
        for (int index = 0; index < this.earlyDecryptedDataReferences.Count; ++index)
        {
          if (!this.TryDeleteReferenceListEntry(this.earlyDecryptedDataReferences[index]))
            throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnexpectedEncryptedElementInSecurityHeader")), this.Message);
        }
      }
      if (this.HasPendingDecryptionItem())
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnableToResolveDataReference", new object[1]{ (object) this.pendingReferenceList.GetReferredId(0) })), this.Message);
    }

    protected override void OnDecryptionOfSecurityHeaderItemRequiringReferenceListEntry(string id)
    {
      if (this.TryDeleteReferenceListEntry(id))
        return;
      if (this.earlyDecryptedDataReferences == null)
        this.earlyDecryptedDataReferences = new List<string>(4);
      this.earlyDecryptedDataReferences.Add(id);
    }

    internal override SecurityToken VerifySignature(SignedXml signedXml, bool isPrimarySignature, SecurityHeaderTokenResolver resolver, object signatureTarget, string id)
    {
#if FEATURE_CORECLR
	  throw new NotImplementedException("LocalAppContextSwitches is not supported in .NET Core");
#else
      if (TD.SignatureVerificationStartIsEnabled())
        TD.SignatureVerificationStart(this.EventTraceActivity);
      SecurityToken token = this.ResolveSignatureToken(signedXml.Signature.KeyIdentifier, (SecurityTokenResolver) resolver, isPrimarySignature);
      if (isPrimarySignature)
        this.RecordSignatureToken(token);
      ReadOnlyCollection<SecurityKey> securityKeys = token.SecurityKeys;
      SecurityKey securityKey = securityKeys == null || securityKeys.Count <= 0 ? (SecurityKey) null : securityKeys[0];
      if (securityKey == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnableToCreateICryptoFromTokenForSignatureVerification", new object[1]{ (object) token })));
      this.AlgorithmSuite.EnsureAcceptableSignatureKeySize(securityKey, token);
      this.AlgorithmSuite.EnsureAcceptableSignatureAlgorithm(securityKey, signedXml.Signature.SignedInfo.SignatureMethod);
      signedXml.StartSignatureVerification(securityKey);
      this.ValidateDigestsOfTargetsInSecurityHeader((StandardSignedInfo) signedXml.Signature.SignedInfo, this.Timestamp, isPrimarySignature, signatureTarget, id);
      if (!isPrimarySignature)
      {
        if (!this.RequireMessageProtection && securityKey is AsymmetricSecurityKey && this.Version.Addressing != AddressingVersion.None)
        {
          int header = this.Message.Headers.FindHeader(System.ServiceModel.XD.AddressingDictionary.To.Value, this.Message.Version.Addressing.Namespace);
          if (header == -1)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TransportSecuredMessageMissingToHeader")));
          XmlDictionaryReader readerAtHeader = this.Message.Headers.GetReaderAtHeader(header);
          id = readerAtHeader.GetAttribute(System.ServiceModel.XD.UtilityDictionary.IdAttribute, System.ServiceModel.XD.UtilityDictionary.Namespace);
          if (System.ServiceModel.LocalAppContextSwitches.AllowUnsignedToHeader)
          {
            if (id != null)
              signedXml.EnsureDigestValidityIfIdMatches(id, (object) readerAtHeader);
          }
          else
          {
            if (id == null)
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnsignedToHeaderInTransportSecuredMessage")));
            signedXml.EnsureDigestValidity(id, (object) readerAtHeader);
          }
        }
        signedXml.CompleteSignatureVerification();
        return token;
      }
      this.pendingSignature = signedXml;
      if (TD.SignatureVerificationSuccessIsEnabled())
        TD.SignatureVerificationSuccess(this.EventTraceActivity);
      return token;
#endif
    }

    private void ValidateDigestsOfTargetsInSecurityHeader(StandardSignedInfo signedInfo, SecurityTimestamp timestamp, bool isPrimarySignature, object signatureTarget, string id)
    {
#if FEATURE_CORECLR
	  throw new NotImplementedException("SecurityAlgorithmSuite.EnsureAcceptableDigestAlgorithm is not supported in .NET Core");
#else
      for (int index1 = 0; index1 < signedInfo.ReferenceCount; ++index1)
      {
        Reference reference = signedInfo[index1];
        this.AlgorithmSuite.EnsureAcceptableDigestAlgorithm(reference.DigestMethod);
        string referredId = reference.ExtractReferredId();
        if (isPrimarySignature || id == referredId)
        {
          if (timestamp != null && timestamp.Id == referredId && (!reference.TransformChain.NeedsInclusiveContext && timestamp.DigestAlgorithm == reference.DigestMethod) && timestamp.GetDigest() != null)
          {
            reference.EnsureDigestValidity(referredId, timestamp.GetDigest());
            this.ElementManager.SetTimestampSigned(referredId);
          }
          else if (signatureTarget != null)
          {
            reference.EnsureDigestValidity(id, signatureTarget);
          }
          else
          {
            int index2 = -1;
            XmlDictionaryReader dictionaryReader = (XmlDictionaryReader) null;
            if (reference.IsStrTranform())
            {
              if (this.ElementManager.TryGetTokenElementIndexFromStrId(referredId, out index2))
              {
                ReceiveSecurityHeaderEntry element;
                this.ElementManager.GetElementEntry(index2, out element);
                bool requiresEncryptedFormReader = element.bindingMode == ReceiveSecurityHeaderBindingModes.Signed || element.bindingMode == ReceiveSecurityHeaderBindingModes.SignedEndorsing;
                if (!this.ElementManager.IsPrimaryTokenSigned)
                  this.ElementManager.IsPrimaryTokenSigned = element.bindingMode == ReceiveSecurityHeaderBindingModes.Primary && element.elementCategory == ReceiveSecurityHeaderElementCategory.Token;
                this.ElementManager.SetSigned(index2);
                dictionaryReader = this.ElementManager.GetReader(index2, requiresEncryptedFormReader);
              }
            }
            else
              dictionaryReader = this.ElementManager.GetSignatureVerificationReader(referredId, this.EncryptBeforeSignMode);
            if (dictionaryReader != null)
            {
              reference.EnsureDigestValidity(referredId, (object) dictionaryReader);
              dictionaryReader.Close();
            }
          }
          if (!isPrimarySignature)
            break;
        }
      }
      if (isPrimarySignature && this.RequireSignedPrimaryToken && !this.ElementManager.IsPrimaryTokenSigned)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("SupportingTokenIsNotSigned", new object[1]{ (object) new IssuedSecurityTokenParameters() })));
#endif
    }

    private void VerifySoapAttributeMatchForHeader(MessageHeaderInfo info, MessagePartSpecification signatureParts, XmlDictionaryReader reader)
    {
      if (!signatureParts.IsHeaderIncluded(info.Name, info.Namespace))
        return;
      EnvelopeVersion envelope = this.Version.Envelope;
      EnvelopeVersion envelopeVersion = envelope == EnvelopeVersion.Soap11 ? EnvelopeVersion.Soap12 : EnvelopeVersion.Soap11;
      bool flag1 = reader.GetAttribute(System.ServiceModel.XD.MessageDictionary.MustUnderstand, envelope.DictionaryNamespace) != null;
      if (reader.GetAttribute(System.ServiceModel.XD.MessageDictionary.MustUnderstand, envelopeVersion.DictionaryNamespace) != null && !flag1)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("InvalidAttributeInSignedHeader", (object) info.Name, (object) info.Namespace, (object) System.ServiceModel.XD.MessageDictionary.MustUnderstand, (object) envelopeVersion.DictionaryNamespace, (object) System.ServiceModel.XD.MessageDictionary.MustUnderstand, (object) envelope.DictionaryNamespace)), (Message) this.SecurityVerifiedMessage);
      bool flag2 = reader.GetAttribute(envelope.DictionaryActor, envelope.DictionaryNamespace) != null;
      if (reader.GetAttribute(envelopeVersion.DictionaryActor, envelopeVersion.DictionaryNamespace) != null && !flag2)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("InvalidAttributeInSignedHeader", (object) info.Name, (object) info.Namespace, (object) envelopeVersion.DictionaryActor, (object) envelopeVersion.DictionaryNamespace, (object) envelope.DictionaryActor, (object) envelope.DictionaryNamespace)), (Message) this.SecurityVerifiedMessage);
    }
  }
}
