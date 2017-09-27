// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.WSSecurityOneDotZeroSendSecurityHeader
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
  internal class WSSecurityOneDotZeroSendSecurityHeader : SendSecurityHeader
  {
    private HashStream hashStream;
    private PreDigestedSignedInfo signedInfo;
    private SignedXml signedXml;
    private SecurityKey signatureKey;
    private MessagePartSpecification effectiveSignatureParts;
    private SymmetricAlgorithm encryptingSymmetricAlgorithm;
    private ReferenceList referenceList;
    private SecurityKeyIdentifier encryptionKeyIdentifier;
    private bool hasSignedEncryptedMessagePart;
    private byte[] toHeaderHash;
    private string toHeaderId;

    protected string EncryptionAlgorithm
    {
      get
      {
        return this.AlgorithmSuite.DefaultEncryptionAlgorithm;
      }
    }

    protected XmlDictionaryString EncryptionAlgorithmDictionaryString
    {
      get
      {
        return this.AlgorithmSuite.DefaultEncryptionAlgorithmDictionaryString;
      }
    }

    protected override bool HasSignedEncryptedMessagePart
    {
      get
      {
        return this.hasSignedEncryptedMessagePart;
      }
    }

    public WSSecurityOneDotZeroSendSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay, SecurityStandardsManager standardsManager, SecurityAlgorithmSuite algorithmSuite, MessageDirection direction)
      : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, direction)
    {
    }

    private void AddEncryptionReference(MessageHeader header, string headerId, IPrefixGenerator prefixGenerator, bool sign, out MemoryStream plainTextStream, out string encryptedDataId)
    {
      plainTextStream = new MemoryStream();
      XmlDictionaryWriter textWriter = XmlDictionaryWriter.CreateTextWriter((Stream) plainTextStream);
      if (sign)
      {
        this.AddSignatureReference(header, headerId, prefixGenerator, textWriter);
      }
      else
      {
        header.WriteHeader(textWriter, this.Version);
        textWriter.Flush();
      }
      encryptedDataId = this.GenerateId();
      this.referenceList.AddReferredId(encryptedDataId);
    }

    private void AddSignatureReference(SecurityToken token, int position, SecurityTokenAttachmentMode mode)
    {
      SecurityKeyIdentifierClause keyIdentifierClause = (SecurityKeyIdentifierClause) null;
      bool strTransformEnabled = this.ShouldUseStrTransformForToken(token, position, mode, out keyIdentifierClause);
      this.AddTokenSignatureReference(token, keyIdentifierClause, strTransformEnabled);
    }

    private void AddPrimaryTokenSignatureReference(SecurityToken token, SecurityTokenParameters securityTokenParameters)
    {
      IssuedSecurityTokenParameters securityTokenParameters1 = securityTokenParameters as IssuedSecurityTokenParameters;
      if (securityTokenParameters1 == null)
        return;
      bool strTransformEnabled = securityTokenParameters1 != null && securityTokenParameters1.UseStrTransform;
      SecurityKeyIdentifierClause keyIdentifierClause = (SecurityKeyIdentifierClause) null;
      if (!SendSecurityHeader.ShouldSerializeToken(securityTokenParameters, this.MessageDirection))
        return;
      if (strTransformEnabled)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("Vague parameters for CreateKeyIdentifierClause");
#else
        keyIdentifierClause = securityTokenParameters.CreateKeyIdentifierClause(token, this.GetTokenReferenceStyle(securityTokenParameters));
#endif
      }
      this.AddTokenSignatureReference(token, keyIdentifierClause, strTransformEnabled);
    }

    private void AddTokenSignatureReference(SecurityToken token, SecurityKeyIdentifierClause keyIdentifierClause, bool strTransformEnabled)
    {
      if (!strTransformEnabled && token.Id == null)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("ElementToSignMustHaveId")), this.Message);
      HashStream hashStream = this.TakeHashStream();
      XmlDictionaryWriter utf8Writer = this.TakeUtf8Writer();
      utf8Writer.StartCanonicalization((Stream) hashStream, false, (string[]) null);
      this.StandardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) utf8Writer, token);
      utf8Writer.EndCanonicalization();
      if (strTransformEnabled)
      {
        if (keyIdentifierClause == null)
          throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenManagerCannotCreateTokenReference")), this.Message);
        if (string.IsNullOrEmpty(keyIdentifierClause.Id))
          keyIdentifierClause.Id = SecurityUniqueId.Create().Value;
        this.ElementContainer.MapSecurityTokenToStrClause(token, keyIdentifierClause);
        this.signedInfo.AddReference(keyIdentifierClause.Id, hashStream.FlushHashAndGetValue(), true);
      }
      else
        this.signedInfo.AddReference(token.Id, hashStream.FlushHashAndGetValue());
    }

    private void AddSignatureReference(SendSecurityHeaderElement[] elements)
    {
      if (elements == null)
        return;
      for (int position = 0; position < elements.Length; ++position)
      {
        SecurityKeyIdentifierClause keyIdentifierClause = (SecurityKeyIdentifierClause) null;
        TokenElement tokenElement = elements[position].Item as TokenElement;
        bool flag = tokenElement != null && this.SignThenEncrypt && this.ShouldUseStrTransformForToken(tokenElement.Token, position, SecurityTokenAttachmentMode.SignedEncrypted, out keyIdentifierClause);
        if (!flag && elements[position].Id == null)
          throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("ElementToSignMustHaveId")), this.Message);
        HashStream hashStream = this.TakeHashStream();
        XmlDictionaryWriter utf8Writer = this.TakeUtf8Writer();
        utf8Writer.StartCanonicalization((Stream) hashStream, false, (string[]) null);
        elements[position].Item.WriteTo(utf8Writer, ServiceModelDictionaryManager.Instance);
        utf8Writer.EndCanonicalization();
        if (flag)
        {
          if (keyIdentifierClause == null)
            throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenManagerCannotCreateTokenReference")), this.Message);
          if (string.IsNullOrEmpty(keyIdentifierClause.Id))
            keyIdentifierClause.Id = SecurityUniqueId.Create().Value;
          this.ElementContainer.MapSecurityTokenToStrClause(tokenElement.Token, keyIdentifierClause);
          this.signedInfo.AddReference(keyIdentifierClause.Id, hashStream.FlushHashAndGetValue(), true);
        }
        else
          this.signedInfo.AddReference(elements[position].Id, hashStream.FlushHashAndGetValue());
      }
    }

    private void AddSignatureReference(SecurityToken[] tokens, SecurityTokenAttachmentMode mode)
    {
      if (tokens == null)
        return;
      for (int position = 0; position < tokens.Length; ++position)
        this.AddSignatureReference(tokens[position], position, mode);
    }

    private string GetSignatureHash(MessageHeader header, string headerId, IPrefixGenerator prefixGenerator, XmlDictionaryWriter writer, out byte[] hash)
    {
      HashStream hashStream = this.TakeHashStream();
      System.ServiceModel.XmlBuffer xmlBuffer = (System.ServiceModel.XmlBuffer) null;
      XmlDictionaryWriter writer1;
      if (writer.CanCanonicalize)
      {
        writer1 = writer;
      }
      else
      {
        xmlBuffer = new System.ServiceModel.XmlBuffer(int.MaxValue);
        writer1 = xmlBuffer.OpenSection(XmlDictionaryReaderQuotas.Max);
      }
      writer1.StartCanonicalization((Stream) hashStream, false, (string[]) null);
      header.WriteStartHeader(writer1, this.Version);
      if (headerId == null)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("SecurityStandardsManager.IdManager is not supported in .NET Core");
#else
        headerId = this.GenerateId();
        this.StandardsManager.IdManager.WriteIdAttribute(writer1, headerId);
#endif
      }
      header.WriteHeaderContents(writer1, this.Version);
      writer1.WriteEndElement();
      writer1.EndCanonicalization();
      writer1.Flush();
      if (writer1 != writer)
      {
        xmlBuffer.CloseSection();
        xmlBuffer.Close();
        XmlDictionaryReader reader = xmlBuffer.GetReader(0);
        writer.WriteNode(reader, false);
        reader.Close();
      }
      hash = hashStream.FlushHashAndGetValue();
      return headerId;
    }

    private void AddSignatureReference(MessageHeader header, string headerId, IPrefixGenerator prefixGenerator, XmlDictionaryWriter writer)
    {
      byte[] hash;
      headerId = this.GetSignatureHash(header, headerId, prefixGenerator, writer, out hash);
      this.signedInfo.AddReference(headerId, hash);
    }

    private void ApplySecurityAndWriteHeader(MessageHeader header, string headerId, XmlDictionaryWriter writer, IPrefixGenerator prefixGenerator)
    {
      if (!this.RequireMessageProtection && this.ShouldSignToHeader && (header.Name == System.ServiceModel.XD.AddressingDictionary.To.Value && header.Namespace == this.Message.Version.Addressing.Namespace))
      {
        if (this.toHeaderHash != null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TransportSecuredMessageHasMoreThanOneToHeader")));
        byte[] hash;
        headerId = this.GetSignatureHash(header, headerId, prefixGenerator, writer, out hash);
        this.toHeaderHash = hash;
        this.toHeaderId = headerId;
      }
      else
      {
        switch (this.GetProtectionMode(header))
        {
          case MessagePartProtectionMode.None:
            header.WriteHeader(writer, this.Version);
            break;
          case MessagePartProtectionMode.Sign:
            this.AddSignatureReference(header, headerId, prefixGenerator, writer);
            break;
          case MessagePartProtectionMode.Encrypt:
            MemoryStream plainTextStream1;
            string encryptedDataId1;
            this.AddEncryptionReference(header, headerId, prefixGenerator, false, out plainTextStream1, out encryptedDataId1);
            this.EncryptAndWriteHeader(header, encryptedDataId1, plainTextStream1, writer);
            break;
          case MessagePartProtectionMode.SignThenEncrypt:
            MemoryStream plainTextStream2;
            string encryptedDataId2;
            this.AddEncryptionReference(header, headerId, prefixGenerator, true, out plainTextStream2, out encryptedDataId2);
            this.EncryptAndWriteHeader(header, encryptedDataId2, plainTextStream2, writer);
            this.hasSignedEncryptedMessagePart = true;
            break;
          case MessagePartProtectionMode.EncryptThenSign:
#if FEATURE_CORECLR
            throw new NotImplementedException("EncryptHeader is not supported in .NET Core");
#else
            MemoryStream plainTextStream3;
            string encryptedDataId3;
            this.AddEncryptionReference(header, headerId, prefixGenerator, false, out plainTextStream3, out encryptedDataId3);
            this.AddSignatureReference((MessageHeader) this.EncryptHeader(header, this.encryptingSymmetricAlgorithm, this.encryptionKeyIdentifier, this.Version, encryptedDataId3, plainTextStream3), encryptedDataId3, prefixGenerator, writer);
            break;
#endif
        }
      }
    }

    public override void ApplySecurityAndWriteHeaders(MessageHeaders headers, XmlDictionaryWriter writer, IPrefixGenerator prefixGenerator)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("SecurityStandardsManager.IdManager is not supported in .NET Core");
#else
      string[] strArray = this.RequireMessageProtection || this.ShouldSignToHeader ? headers.GetHeaderAttributes("Id", this.StandardsManager.IdManager.DefaultIdNamespaceUri) : (string[]) null;
      for (int index = 0; index < headers.Count; ++index)
      {
        MessageHeader messageHeader = headers.GetMessageHeader(index);
        if ((this.Version.Addressing != AddressingVersion.None || !(messageHeader.Namespace == AddressingVersion.None.Namespace)) && messageHeader != this)
          this.ApplySecurityAndWriteHeader(messageHeader, strArray == null ? (string) null : strArray[index], writer, prefixGenerator);
      }
#endif
    }

    private static bool CanCanonicalizeAndFragment(XmlDictionaryWriter writer)
    {
      if (!writer.CanCanonicalize)
        return false;
      IFragmentCapableXmlDictionaryWriter dictionaryWriter = writer as IFragmentCapableXmlDictionaryWriter;
      if (dictionaryWriter != null)
        return dictionaryWriter.CanFragment;
      return false;
    }

    public override void ApplyBodySecurity(XmlDictionaryWriter writer, IPrefixGenerator prefixGenerator)
    {
      SecurityAppliedMessage securityAppliedMessage = this.SecurityAppliedMessage;
      switch (securityAppliedMessage.BodyProtectionMode)
      {
        case MessagePartProtectionMode.Sign:
          HashStream hashStream1 = this.TakeHashStream();
          if (WSSecurityOneDotZeroSendSecurityHeader.CanCanonicalizeAndFragment(writer))
            securityAppliedMessage.WriteBodyToSignWithFragments((Stream) hashStream1, false, (string[]) null, writer);
          else
            securityAppliedMessage.WriteBodyToSign((Stream) hashStream1);
          this.signedInfo.AddReference(securityAppliedMessage.BodyId, hashStream1.FlushHashAndGetValue());
          break;
        case MessagePartProtectionMode.Encrypt:
          EncryptedData encryptedDataForBody1 = this.CreateEncryptedDataForBody();
          securityAppliedMessage.WriteBodyToEncrypt(encryptedDataForBody1, this.encryptingSymmetricAlgorithm);
          this.referenceList.AddReferredId(encryptedDataForBody1.Id);
          break;
        case MessagePartProtectionMode.SignThenEncrypt:
          HashStream hashStream2 = this.TakeHashStream();
          EncryptedData encryptedDataForBody2 = this.CreateEncryptedDataForBody();
          if (WSSecurityOneDotZeroSendSecurityHeader.CanCanonicalizeAndFragment(writer))
            securityAppliedMessage.WriteBodyToSignThenEncryptWithFragments((Stream) hashStream2, false, (string[]) null, encryptedDataForBody2, this.encryptingSymmetricAlgorithm, writer);
          else
            securityAppliedMessage.WriteBodyToSignThenEncrypt((Stream) hashStream2, encryptedDataForBody2, this.encryptingSymmetricAlgorithm);
          this.signedInfo.AddReference(securityAppliedMessage.BodyId, hashStream2.FlushHashAndGetValue());
          this.referenceList.AddReferredId(encryptedDataForBody2.Id);
          this.hasSignedEncryptedMessagePart = true;
          break;
        case MessagePartProtectionMode.EncryptThenSign:
          HashStream hashStream3 = this.TakeHashStream();
          EncryptedData encryptedDataForBody3 = this.CreateEncryptedDataForBody();
          securityAppliedMessage.WriteBodyToEncryptThenSign((Stream) hashStream3, encryptedDataForBody3, this.encryptingSymmetricAlgorithm);
          this.signedInfo.AddReference(securityAppliedMessage.BodyId, hashStream3.FlushHashAndGetValue());
          this.referenceList.AddReferredId(encryptedDataForBody3.Id);
          break;
      }
    }

    protected static MemoryStream CaptureToken(SecurityToken token, SecurityStandardsManager serializer)
    {
      MemoryStream memoryStream = new MemoryStream();
      XmlDictionaryWriter textWriter = XmlDictionaryWriter.CreateTextWriter((Stream) memoryStream);
      serializer.SecurityTokenSerializer.WriteToken((XmlWriter) textWriter, token);
      textWriter.Flush();
      memoryStream.Seek(0L, SeekOrigin.Begin);
      return memoryStream;
    }

    protected static MemoryStream CaptureSecurityElement(ISecurityElement element)
    {
      MemoryStream memoryStream = new MemoryStream();
      XmlDictionaryWriter textWriter = XmlDictionaryWriter.CreateTextWriter((Stream) memoryStream);
      element.WriteTo(textWriter, ServiceModelDictionaryManager.Instance);
      textWriter.Flush();
      memoryStream.Seek(0L, SeekOrigin.Begin);
      return memoryStream;
    }

    internal override ISecurityElement CompleteEncryptionCore(SendSecurityHeaderElement primarySignature, SendSecurityHeaderElement[] basicTokens, SendSecurityHeaderElement[] signatureConfirmations, SendSecurityHeaderElement[] endorsingSignatures)
    {
      if (this.referenceList == null)
        return (ISecurityElement) null;
      if (primarySignature != null && primarySignature.Item != null && primarySignature.MarkedForEncryption)
        this.EncryptElement(primarySignature);
      if (basicTokens != null)
      {
        for (int index = 0; index < basicTokens.Length; ++index)
        {
          if (basicTokens[index].MarkedForEncryption)
            this.EncryptElement(basicTokens[index]);
        }
      }
      if (signatureConfirmations != null)
      {
        for (int index = 0; index < signatureConfirmations.Length; ++index)
        {
          if (signatureConfirmations[index].MarkedForEncryption)
            this.EncryptElement(signatureConfirmations[index]);
        }
      }
      if (endorsingSignatures != null)
      {
        for (int index = 0; index < endorsingSignatures.Length; ++index)
        {
          if (endorsingSignatures[index].MarkedForEncryption)
            this.EncryptElement(endorsingSignatures[index]);
        }
      }
      try
      {
        return this.referenceList.DataReferenceCount > 0 ? (ISecurityElement) this.referenceList : (ISecurityElement) null;
      }
      finally
      {
        this.referenceList = (ReferenceList) null;
        this.encryptingSymmetricAlgorithm = (SymmetricAlgorithm) null;
        this.encryptionKeyIdentifier = (SecurityKeyIdentifier) null;
      }
    }

    internal override ISignatureValueSecurityElement CompletePrimarySignatureCore(SendSecurityHeaderElement[] signatureConfirmations, SecurityToken[] signedEndorsingTokens, SecurityToken[] signedTokens, SendSecurityHeaderElement[] basicTokens, bool isPrimarySignature)
    {
      if (this.signedXml == null)
        return (ISignatureValueSecurityElement) null;
#if FEATURE_CORECLR
      throw new NotImplementedException("SignedXml.ComputeSignature is not supported in .NET Core");
#else
      SecurityTimestamp timestamp = this.Timestamp;
      if (timestamp != null)
      {
        if (timestamp.Id == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("TimestampToSignHasNoId")));
        HashStream hashStream = this.TakeHashStream();
        this.StandardsManager.WSUtilitySpecificationVersion.WriteTimestampCanonicalForm((Stream) hashStream, timestamp, this.signedInfo.ResourcePool.TakeEncodingBuffer());
        this.signedInfo.AddReference(timestamp.Id, hashStream.FlushHashAndGetValue());
      }
      if (this.ShouldSignToHeader && this.signatureKey is AsymmetricSecurityKey && this.Version.Addressing != AddressingVersion.None)
      {
        if (this.toHeaderHash == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("TransportSecurityRequireToHeader")));
        this.signedInfo.AddReference(this.toHeaderId, this.toHeaderHash);
      }
      this.AddSignatureReference(signatureConfirmations);
      if (isPrimarySignature && this.ShouldProtectTokens)
        this.AddPrimaryTokenSignatureReference(this.ElementContainer.SourceSigningToken, this.SigningTokenParameters);
      if (this.RequireMessageProtection)
      {
        this.AddSignatureReference(signedEndorsingTokens, SecurityTokenAttachmentMode.SignedEndorsing);
        this.AddSignatureReference(signedTokens, SecurityTokenAttachmentMode.Signed);
        this.AddSignatureReference(basicTokens);
      }
      if (this.signedInfo.ReferenceCount == 0)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("NoPartsOfMessageMatchedPartsToSign")), this.Message);
      try
      {
        this.signedXml.ComputeSignature(this.signatureKey);
        return (ISignatureValueSecurityElement) this.signedXml;
      }
      finally
      {
        this.hashStream = (HashStream) null;
        this.signedInfo = (PreDigestedSignedInfo) null;
        this.signedXml = (SignedXml) null;
        this.signatureKey = (SecurityKey) null;
        this.effectiveSignatureParts = (MessagePartSpecification) null;
      }
#endif
    }

    private EncryptedData CreateEncryptedData()
    {
      EncryptedData encryptedData = new EncryptedData();
      encryptedData.SecurityTokenSerializer = this.StandardsManager.SecurityTokenSerializer;
      encryptedData.KeyIdentifier = this.encryptionKeyIdentifier;
      encryptedData.EncryptionMethod = this.EncryptionAlgorithm;
      encryptedData.EncryptionMethodDictionaryString = this.EncryptionAlgorithmDictionaryString;
      return encryptedData;
    }

    private EncryptedData CreateEncryptedData(MemoryStream stream, string id, bool typeElement)
    {
      EncryptedData encryptedData = this.CreateEncryptedData();
      encryptedData.Id = id;
      encryptedData.SetUpEncryption(this.encryptingSymmetricAlgorithm, new ArraySegment<byte>(stream.GetBuffer(), 0, (int) stream.Length));
      if (typeElement)
        encryptedData.Type = EncryptedData.ElementType;
      return encryptedData;
    }

    private EncryptedData CreateEncryptedDataForBody()
    {
      EncryptedData encryptedData = this.CreateEncryptedData();
      encryptedData.Type = EncryptedData.ContentType;
      return encryptedData;
    }

    private void EncryptAndWriteHeader(MessageHeader plainTextHeader, string id, MemoryStream stream, XmlDictionaryWriter writer)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("EncryptHeader is not supported in .NET Core");
#else
      this.EncryptHeader(plainTextHeader, this.encryptingSymmetricAlgorithm, this.encryptionKeyIdentifier, this.Version, id, stream).WriteHeader(writer, this.Version);
#endif
    }

    private void EncryptElement(SendSecurityHeaderElement element)
    {
      string id = this.GenerateId();
      ISecurityElement encryptedData = (ISecurityElement) this.CreateEncryptedData(WSSecurityOneDotZeroSendSecurityHeader.CaptureSecurityElement(element.Item), id, true);
      this.referenceList.AddReferredId(id);
      element.Replace(id, encryptedData);
    }

#if !FEATURE_CORECLR
    internal virtual EncryptedHeader EncryptHeader(MessageHeader plainTextHeader, SymmetricAlgorithm algorithm, SecurityKeyIdentifier keyIdentifier, MessageVersion version, string id, MemoryStream stream)
    {
      throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("HeaderEncryptionNotSupportedInWsSecurityJan2004", (object) plainTextHeader.Name, (object) plainTextHeader.Namespace)));
    }
#endif

    private HashStream TakeHashStream()
    {
      HashStream hashStream;
      if (this.hashStream == null)
      {
        this.hashStream = hashStream = new HashStream(CryptoHelper.CreateHashAlgorithm(this.AlgorithmSuite.DefaultDigestAlgorithm));
      }
      else
      {
        hashStream = this.hashStream;
        hashStream.Reset();
      }
      return hashStream;
    }

    private XmlDictionaryWriter TakeUtf8Writer()
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("SignatureResourcePool.TakeUtf8Writer is not supported in .NET Core");
#else
      return this.signedInfo.ResourcePool.TakeUtf8Writer();
#endif
    }

    private MessagePartProtectionMode GetProtectionMode(MessageHeader header)
    {
      if (!this.RequireMessageProtection)
        return MessagePartProtectionMode.None;
#if FEATURE_CORECLR
      throw new NotImplementedException("MessagePartProtectionModeHelper is not supported in .NET Core");
#else
      return MessagePartProtectionModeHelper.GetProtectionMode(this.signedInfo != null && this.effectiveSignatureParts.IsHeaderIncluded(header), this.referenceList != null && this.EncryptionParts.IsHeaderIncluded(header), this.SignThenEncrypt);
#endif
    }

    protected override void StartEncryptionCore(SecurityToken token, SecurityKeyIdentifier keyIdentifier)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("SecurityUtils.GetSymmetricAlgorithm is not supported in .NET Core");
#else
      this.encryptingSymmetricAlgorithm = SecurityUtils.GetSymmetricAlgorithm(this.EncryptionAlgorithm, token);
      if (this.encryptingSymmetricAlgorithm == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnableToCreateSymmetricAlgorithmFromToken", new object[1]{ (object) this.EncryptionAlgorithm })));
      this.encryptionKeyIdentifier = keyIdentifier;
      this.referenceList = new ReferenceList();
#endif
    }

    protected override void StartPrimarySignatureCore(SecurityToken token, SecurityKeyIdentifier keyIdentifier, MessagePartSpecification signatureParts, bool generateTargettableSignature)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("SignedXml is not supported in .NET Core");
#else
      SecurityAlgorithmSuite algorithmSuite = this.AlgorithmSuite;
      string canonicalizationAlgorithm = algorithmSuite.DefaultCanonicalizationAlgorithm;
      XmlDictionaryString dictionaryString1 = algorithmSuite.DefaultCanonicalizationAlgorithmDictionaryString;
      if (canonicalizationAlgorithm != "http://www.w3.org/2001/10/xml-exc-c14n#")
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnsupportedCanonicalizationAlgorithm", new object[1]{ (object) algorithmSuite.DefaultCanonicalizationAlgorithm })));
      string signatureAlgorithm;
      XmlDictionaryString signatureAlgorithmDictionaryString;
      algorithmSuite.GetSignatureAlgorithmAndKey(token, out signatureAlgorithm, out this.signatureKey, out signatureAlgorithmDictionaryString);
      string defaultDigestAlgorithm = algorithmSuite.DefaultDigestAlgorithm;
      XmlDictionaryString dictionaryString2 = algorithmSuite.DefaultDigestAlgorithmDictionaryString;
      this.signedInfo = new PreDigestedSignedInfo(ServiceModelDictionaryManager.Instance, canonicalizationAlgorithm, dictionaryString1, defaultDigestAlgorithm, dictionaryString2, signatureAlgorithm, signatureAlgorithmDictionaryString);
      this.signedXml = new SignedXml((SignedInfo) this.signedInfo, ServiceModelDictionaryManager.Instance, this.StandardsManager.SecurityTokenSerializer);
      if (keyIdentifier != null)
        this.signedXml.Signature.KeyIdentifier = keyIdentifier;
      if (generateTargettableSignature)
        this.signedXml.Id = this.GenerateId();
      this.effectiveSignatureParts = signatureParts;
      this.hashStream = this.signedInfo.ResourcePool.TakeHashStream(defaultDigestAlgorithm);
#endif
    }

    internal override ISignatureValueSecurityElement CreateSupportingSignature(SecurityToken token, SecurityKeyIdentifier identifier)
    {
      this.StartPrimarySignatureCore(token, identifier, MessagePartSpecification.NoParts, false);
      return this.CompletePrimarySignatureCore((SendSecurityHeaderElement[]) null, (SecurityToken[]) null, (SecurityToken[]) null, (SendSecurityHeaderElement[]) null, false);
    }

    internal override ISignatureValueSecurityElement CreateSupportingSignature(SecurityToken token, SecurityKeyIdentifier identifier, ISecurityElement elementToSign)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("SignedXml is not supported in .NET Core");
#else
      SecurityAlgorithmSuite algorithmSuite = this.AlgorithmSuite;
      string signatureAlgorithm;
      SecurityKey key;
      XmlDictionaryString signatureAlgorithmDictionaryString;
      algorithmSuite.GetSignatureAlgorithmAndKey(token, out signatureAlgorithm, out key, out signatureAlgorithmDictionaryString);
      SignedXml signedXml = new SignedXml(ServiceModelDictionaryManager.Instance, this.StandardsManager.SecurityTokenSerializer);
      SignedInfo signedInfo = signedXml.Signature.SignedInfo;
      signedInfo.CanonicalizationMethod = algorithmSuite.DefaultCanonicalizationAlgorithm;
      signedInfo.CanonicalizationMethodDictionaryString = algorithmSuite.DefaultCanonicalizationAlgorithmDictionaryString;
      signedInfo.SignatureMethod = signatureAlgorithm;
      signedInfo.SignatureMethodDictionaryString = signatureAlgorithmDictionaryString;
      if (elementToSign.Id == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("ElementToSignMustHaveId")));
      Reference reference = new Reference(ServiceModelDictionaryManager.Instance, "#" + elementToSign.Id, (object) elementToSign);
      reference.DigestMethod = algorithmSuite.DefaultDigestAlgorithm;
      reference.DigestMethodDictionaryString = algorithmSuite.DefaultDigestAlgorithmDictionaryString;
      reference.AddTransform((Transform) new ExclusiveCanonicalizationTransform());
      ((StandardSignedInfo) signedInfo).AddReference(reference);
      signedXml.ComputeSignature(key);
      if (identifier != null)
        signedXml.Signature.KeyIdentifier = identifier;
      return (ISignatureValueSecurityElement) signedXml;
#endif
    }

    protected override void WriteSecurityTokenReferencyEntry(XmlDictionaryWriter writer, SecurityToken securityToken, SecurityTokenParameters securityTokenParameters)
    {
      SecurityKeyIdentifierClause keyIdentifierClause = (SecurityKeyIdentifierClause) null;
      IssuedSecurityTokenParameters securityTokenParameters1 = securityTokenParameters as IssuedSecurityTokenParameters;
      if (securityTokenParameters1 == null || !securityTokenParameters1.UseStrTransform || !this.ElementContainer.TryGetIdentifierClauseFromSecurityToken(securityToken, out keyIdentifierClause))
        return;
      if (keyIdentifierClause == null || string.IsNullOrEmpty(keyIdentifierClause.Id))
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenManagerCannotCreateTokenReference")), this.Message);
#if FEATURE_CORECLR
      throw new NotImplementedException("WrappedXmlDictionaryWriter is not supported in .NET Core");
#else
      this.StandardsManager.SecurityTokenSerializer.WriteKeyIdentifierClause((XmlWriter) new WrappedXmlDictionaryWriter(writer, keyIdentifierClause.Id), keyIdentifierClause);
#endif
    }
  }
}
