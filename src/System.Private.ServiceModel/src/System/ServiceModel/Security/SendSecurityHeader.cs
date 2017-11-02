// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SendSecurityHeader
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
  public abstract class SendSecurityHeader : SecurityHeader, IMessageHeaderWithSharedNamespace
  {
    private static readonly string[] ids = new string[10]{ "_0", "_1", "_2", "_3", "_4", "_5", "_6", "_7", "_8", "_9" };
    private bool signThenEncrypt = true;
    private bool basicTokenEncrypted;
    private SendSecurityHeaderElementContainer elementContainer;
    private bool primarySignatureDone;
    private bool encryptSignature;
    private SignatureConfirmations signatureValuesGenerated;
    private SignatureConfirmations signatureConfirmationsToSend;
    private int idCounter;
    private string idPrefix;
    private bool hasSignedTokens;
    private bool hasEncryptedTokens;
    private MessagePartSpecification signatureParts;
    private MessagePartSpecification encryptionParts;
    private SecurityTokenParameters signingTokenParameters;
    private SecurityTokenParameters encryptingTokenParameters;
    private List<SecurityToken> basicTokens;
    private List<SecurityTokenParameters> basicSupportingTokenParameters;
    private List<SecurityTokenParameters> endorsingTokenParameters;
    private List<SecurityTokenParameters> signedEndorsingTokenParameters;
    private List<SecurityTokenParameters> signedTokenParameters;
    private SecurityToken encryptingToken;
    private bool skipKeyInfoForEncryption;
    private byte[] primarySignatureValue;
    private bool shouldProtectTokens;
    private System.ServiceModel.Channels.BufferManager bufferManager;
    private bool shouldSignToHeader;
    private SecurityProtocolCorrelationState correlationState;

    internal SendSecurityHeaderElementContainer ElementContainer
    {
      get
      {
        return this.elementContainer;
      }
    }

    internal SecurityProtocolCorrelationState CorrelationState
    {
      get
      {
        return this.correlationState;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.correlationState = value;
      }
    }

    public System.ServiceModel.Channels.BufferManager StreamBufferManager
    {
      get
      {
        if (this.bufferManager == null)
          this.bufferManager = System.ServiceModel.Channels.BufferManager.CreateBufferManager(0L, int.MaxValue);
        return this.bufferManager;
      }
      set
      {
        this.bufferManager = value;
      }
    }

    public MessagePartSpecification EncryptionParts
    {
      get
      {
        return this.encryptionParts;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        if (value == null)
          throw TraceUtility.ThrowHelperError((Exception) new ArgumentNullException("value"), this.Message);
        if (!value.IsReadOnly)
          throw TraceUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("MessagePartSpecificationMustBeImmutable")), this.Message);
        this.encryptionParts = value;
      }
    }

    public bool EncryptPrimarySignature
    {
      get
      {
        return this.encryptSignature;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.encryptSignature = value;
      }
    }

    internal byte[] PrimarySignatureValue
    {
      get
      {
        return this.primarySignatureValue;
      }
    }

    protected internal SecurityTokenParameters SigningTokenParameters
    {
      get
      {
        return this.signingTokenParameters;
      }
    }

    protected bool ShouldSignToHeader
    {
      get
      {
        return this.shouldSignToHeader;
      }
    }

    public string IdPrefix
    {
      get
      {
        return this.idPrefix;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.idPrefix = string.IsNullOrEmpty(value) || value == "_" ? (string) null : value;
      }
    }

    public override string Name
    {
      get
      {
        return this.StandardsManager.SecurityVersion.HeaderName.Value;
      }
    }

    public override string Namespace
    {
      get
      {
        return this.StandardsManager.SecurityVersion.HeaderNamespace.Value;
      }
    }

    internal SecurityAppliedMessage SecurityAppliedMessage
    {
      get
      {
        return (SecurityAppliedMessage) this.Message;
      }
    }

    public bool SignThenEncrypt
    {
      get
      {
        return this.signThenEncrypt;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.signThenEncrypt = value;
      }
    }

    public bool ShouldProtectTokens
    {
      get
      {
        return this.shouldProtectTokens;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.shouldProtectTokens = value;
      }
    }

    public MessagePartSpecification SignatureParts
    {
      get
      {
        return this.signatureParts;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        if (value == null)
          throw TraceUtility.ThrowHelperError((Exception) new ArgumentNullException("value"), this.Message);
        if (!value.IsReadOnly)
          throw TraceUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("MessagePartSpecificationMustBeImmutable")), this.Message);
        this.signatureParts = value;
      }
    }

    internal SecurityTimestamp Timestamp
    {
      get
      {
        return this.elementContainer.Timestamp;
      }
    }

    public bool HasSignedTokens
    {
      get
      {
        return this.hasSignedTokens;
      }
    }

    public bool HasEncryptedTokens
    {
      get
      {
        return this.hasEncryptedTokens;
      }
    }

    protected virtual bool HasSignedEncryptedMessagePart
    {
      get
      {
        return false;
      }
    }

    XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedNamespace
    {
      get
      {
        return System.ServiceModel.XD.UtilityDictionary.Namespace;
      }
    }

    XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedPrefix
    {
      get
      {
        return System.ServiceModel.XD.UtilityDictionary.Prefix;
      }
    }

    protected SendSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay, SecurityStandardsManager standardsManager, SecurityAlgorithmSuite algorithmSuite, MessageDirection transferDirection)
      : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, transferDirection)
    {
      this.elementContainer = new SendSecurityHeaderElementContainer();
    }

    public void AddPrerequisiteToken(SecurityToken token)
    {
      this.ThrowIfProcessingStarted();
      if (token == null)
        throw TraceUtility.ThrowHelperArgumentNull("token", this.Message);
      this.elementContainer.PrerequisiteToken = token;
    }

    private void AddParameters(ref List<SecurityTokenParameters> list, SecurityTokenParameters item)
    {
      if (list == null)
        list = new List<SecurityTokenParameters>();
      list.Add(item);
    }

    public abstract void ApplyBodySecurity(XmlDictionaryWriter writer, IPrefixGenerator prefixGenerator);

    public abstract void ApplySecurityAndWriteHeaders(MessageHeaders headers, XmlDictionaryWriter writer, IPrefixGenerator prefixGenerator);

    public void SetSigningToken(SecurityToken token, SecurityTokenParameters tokenParameters)
    {
      this.ThrowIfProcessingStarted();
      if (token == null && tokenParameters != null || token != null && tokenParameters == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR.GetString("TokenMustBeNullWhenTokenParametersAre")));
      this.elementContainer.SourceSigningToken = token;
      this.signingTokenParameters = tokenParameters;
    }

    public void SetEncryptionToken(SecurityToken token, SecurityTokenParameters tokenParameters)
    {
      this.ThrowIfProcessingStarted();
      if (token == null && tokenParameters != null || token != null && tokenParameters == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR.GetString("TokenMustBeNullWhenTokenParametersAre")));
      this.elementContainer.SourceEncryptionToken = token;
      this.encryptingTokenParameters = tokenParameters;
    }

    public void AddBasicSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
    {
      if (token == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
      if (parameters == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
      this.ThrowIfProcessingStarted();
      this.elementContainer.AddBasicSupportingToken(new SendSecurityHeaderElement(token.Id, (ISecurityElement) new TokenElement(token, this.StandardsManager))
      {
        MarkedForEncryption = true
      });
      this.hasEncryptedTokens = true;
      this.hasSignedTokens = true;
      this.AddParameters(ref this.basicSupportingTokenParameters, parameters);
      if (this.basicTokens == null)
        this.basicTokens = new List<SecurityToken>();
      this.basicTokens.Add(token);
    }

    public void AddEndorsingSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
    {
      if (token == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
      if (parameters == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
      this.ThrowIfProcessingStarted();
      this.elementContainer.AddEndorsingSupportingToken(token);
      this.shouldSignToHeader = System.ServiceModel.CompatibilityShim.ShouldSignHeader;
      if (!(token is ProviderBackedSecurityToken))
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("SecurityUtils.GetSecurityKey is not supported in .NET Core");
#else
        this.shouldSignToHeader = ((this.shouldSignToHeader ? 1 : 0) | (this.RequireMessageProtection ? 0 : (SecurityUtils.GetSecurityKey<AsymmetricSecurityKey>(token) != null ? 1 : 0))) != 0;
#endif
      }
      this.AddParameters(ref this.endorsingTokenParameters, parameters);
    }

    public void AddSignedEndorsingSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
    {
      if (token == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
      if (parameters == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
      this.ThrowIfProcessingStarted();
      this.elementContainer.AddSignedEndorsingSupportingToken(token);
      this.hasSignedTokens = true;
#if FEATURE_CORECLR
      throw new NotImplementedException("SecurityUtils.GetSecurityKey is not supported in .NET Core");
#else
      this.shouldSignToHeader = ((this.shouldSignToHeader ? 1 : 0) | (this.RequireMessageProtection ? 0 : (SecurityUtils.GetSecurityKey<AsymmetricSecurityKey>(token) != null ? 1 : 0))) != 0;
      this.AddParameters(ref this.signedEndorsingTokenParameters, parameters);
#endif
    }

    public void AddSignedSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
    {
      if (token == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
      if (parameters == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
      this.ThrowIfProcessingStarted();
      this.elementContainer.AddSignedSupportingToken(token);
      this.hasSignedTokens = true;
      this.AddParameters(ref this.signedTokenParameters, parameters);
    }

    public void AddSignatureConfirmations(SignatureConfirmations confirmations)
    {
      this.ThrowIfProcessingStarted();
      this.signatureConfirmationsToSend = confirmations;
    }

    public void AddTimestamp(TimeSpan timestampValidityDuration)
    {
      DateTime utcNow = DateTime.UtcNow;
      string id = this.RequireMessageProtection ? SecurityUtils.GenerateId() : this.GenerateId();
      this.AddTimestamp(new SecurityTimestamp(utcNow, utcNow + timestampValidityDuration, id));
    }

    internal void AddTimestamp(SecurityTimestamp timestamp)
    {
      this.ThrowIfProcessingStarted();
      if (this.elementContainer.Timestamp != null)
        throw TraceUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("TimestampAlreadySetForSecurityHeader")), this.Message);
      if (timestamp == null)
        throw TraceUtility.ThrowHelperArgumentNull("timestamp", this.Message);
      this.elementContainer.Timestamp = timestamp;
    }

    internal virtual ISignatureValueSecurityElement[] CreateSignatureConfirmationElements(SignatureConfirmations signatureConfirmations)
    {
      throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SignatureConfirmationNotSupported")));
    }

    private void StartEncryption()
    {
      if (this.elementContainer.SourceEncryptionToken == null)
        return;
#if FEATURE_CORECLR
      throw new NotImplementedException("AlgorithmSuite.GetEncryptionKeyDerivationLength is not supported in .NET Core");
#else
      SecurityTokenReferenceStyle tokenReferenceStyle = this.GetTokenReferenceStyle(this.encryptingTokenParameters);
      bool flag = tokenReferenceStyle == SecurityTokenReferenceStyle.Internal;
      SecurityKeyIdentifierClause identifierClause1 = this.encryptingTokenParameters.CreateKeyIdentifierClause(this.elementContainer.SourceEncryptionToken, tokenReferenceStyle);
      if (identifierClause1 == null)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenManagerCannotCreateTokenReference")), this.Message);
      SecurityToken securityToken;
      SecurityKeyIdentifierClause tokenToDeriveIdentifier;
      if (!SecurityUtils.HasSymmetricSecurityKey(this.elementContainer.SourceEncryptionToken))
      {

        int keyLength = Math.Max(128, this.AlgorithmSuite.DefaultSymmetricKeyLength);
        CryptoHelper.ValidateSymmetricKeyLength(keyLength, this.AlgorithmSuite);
        byte[] numArray = new byte[keyLength / 8];
        CryptoHelper.FillRandomBytes(numArray);
        string keyWrapAlgorithm;
        XmlDictionaryString keyWrapAlgorithmDictionaryString;
        this.AlgorithmSuite.GetKeyWrapAlgorithm(this.elementContainer.SourceEncryptionToken, out keyWrapAlgorithm, out keyWrapAlgorithmDictionaryString);
        WrappedKeySecurityToken keySecurityToken = new WrappedKeySecurityToken(this.GenerateId(), numArray, keyWrapAlgorithm, keyWrapAlgorithmDictionaryString, this.elementContainer.SourceEncryptionToken, new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[1]{ identifierClause1 }));
        this.elementContainer.WrappedEncryptionToken = (SecurityToken) keySecurityToken;
        securityToken = (SecurityToken) keySecurityToken;
        tokenToDeriveIdentifier = (SecurityKeyIdentifierClause) new LocalIdKeyIdentifierClause(keySecurityToken.Id, keySecurityToken.GetType());
        flag = true;
      }
      else
      {
        securityToken = this.elementContainer.SourceEncryptionToken;
        tokenToDeriveIdentifier = identifierClause1;
      }
      SecurityKeyIdentifierClause identifierClause2;
      if (this.encryptingTokenParameters.RequireDerivedKeys)
      {
        string derivationAlgorithm1 = this.AlgorithmSuite.GetEncryptionKeyDerivationAlgorithm(securityToken, this.StandardsManager.MessageSecurityVersion.SecureConversationVersion);
        string derivationAlgorithm2 = SecurityUtils.GetKeyDerivationAlgorithm(this.StandardsManager.MessageSecurityVersion.SecureConversationVersion);
        if (derivationAlgorithm1 == derivationAlgorithm2)
        {

          DerivedKeySecurityToken keySecurityToken = new DerivedKeySecurityToken(-1, 0, this.AlgorithmSuite.GetEncryptionKeyDerivationLength(securityToken, this.StandardsManager.MessageSecurityVersion.SecureConversationVersion), (string) null, 16, securityToken, tokenToDeriveIdentifier, derivationAlgorithm1, this.GenerateId());
          this.encryptingToken = this.elementContainer.DerivedEncryptionToken = (SecurityToken) keySecurityToken;
          identifierClause2 = (SecurityKeyIdentifierClause) new LocalIdKeyIdentifierClause(keySecurityToken.Id, keySecurityToken.GetType());
        }
        else
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnsupportedCryptoAlgorithm", new object[1]{ (object) derivationAlgorithm1 })));
      }
      else
      {
        this.encryptingToken = securityToken;
        identifierClause2 = tokenToDeriveIdentifier;
      }
      this.skipKeyInfoForEncryption = flag && this.EncryptedKeyContainsReferenceList && this.encryptingToken is WrappedKeySecurityToken && this.signThenEncrypt;
      SecurityKeyIdentifier keyIdentifier;
      if (this.skipKeyInfoForEncryption)
        keyIdentifier = (SecurityKeyIdentifier) null;
      else
        keyIdentifier = new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[1]
        {
          identifierClause2
        });
      this.StartEncryptionCore(this.encryptingToken, keyIdentifier);
#endif
    }

    private void CompleteEncryption()
    {
      ISecurityElement securityElement = this.CompleteEncryptionCore(this.elementContainer.PrimarySignature, this.elementContainer.GetBasicSupportingTokens(), this.elementContainer.GetSignatureConfirmations(), this.elementContainer.GetEndorsingSignatures());
      if (securityElement == null)
      {
        this.elementContainer.SourceEncryptionToken = (SecurityToken) null;
        this.elementContainer.WrappedEncryptionToken = (SecurityToken) null;
        this.elementContainer.DerivedEncryptionToken = (SecurityToken) null;
      }
      else
      {
        if (this.skipKeyInfoForEncryption)
        {
          WrappedKeySecurityToken encryptingToken = this.encryptingToken as WrappedKeySecurityToken;
          encryptingToken.EnsureEncryptedKeySetUp();
          encryptingToken.EncryptedKey.ReferenceList = (ReferenceList) securityElement;
        }
        else
          this.elementContainer.ReferenceList = securityElement;
        this.basicTokenEncrypted = true;
      }
    }

    internal void StartSecurityApplication()
    {
      if (this.SignThenEncrypt)
      {
        this.StartSignature();
        this.StartEncryption();
      }
      else
      {
        this.StartEncryption();
        this.StartSignature();
      }
    }

    internal void CompleteSecurityApplication()
    {
      if (this.SignThenEncrypt)
      {
        this.CompleteSignature();
        this.SignWithSupportingTokens();
        this.CompleteEncryption();
      }
      else
      {
        this.CompleteEncryption();
        this.CompleteSignature();
        this.SignWithSupportingTokens();
      }
      if (this.correlationState == null)
        return;
      this.correlationState.SignatureConfirmations = this.GetSignatureValues();
    }

    public void RemoveSignatureEncryptionIfAppropriate()
    {
      if (!this.SignThenEncrypt || !this.EncryptPrimarySignature || this.SecurityAppliedMessage.BodyProtectionMode == MessagePartProtectionMode.SignThenEncrypt || this.basicSupportingTokenParameters != null && this.basicSupportingTokenParameters.Count != 0 || (this.signatureConfirmationsToSend != null && this.signatureConfirmationsToSend.Count != 0 && this.signatureConfirmationsToSend.IsMarkedForEncryption || this.HasSignedEncryptedMessagePart))
        return;
      this.encryptSignature = false;
    }

    public string GenerateId()
    {
      int idCounter = this.idCounter;
      this.idCounter = idCounter + 1;
      int index = idCounter;
      if (this.idPrefix != null)
        return this.idPrefix + (object) index;
      if (index < SendSecurityHeader.ids.Length)
        return SendSecurityHeader.ids[index];
      return "_" + (object) index;
    }

    private SignatureConfirmations GetSignatureValues()
    {
      return this.signatureValuesGenerated;
    }

    protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
    {
      this.StandardsManager.SecurityVersion.WriteStartHeader(writer);
      this.WriteHeaderAttributes(writer, messageVersion);
    }

    internal static bool ShouldSerializeToken(SecurityTokenParameters parameters, MessageDirection transferDirection)
    {
      switch (parameters.InclusionMode)
      {
        case SecurityTokenInclusionMode.AlwaysToRecipient:
        case SecurityTokenInclusionMode.Once:
          return transferDirection == MessageDirection.Input;
        case SecurityTokenInclusionMode.Never:
          return false;
        case SecurityTokenInclusionMode.AlwaysToInitiator:
          return transferDirection == MessageDirection.Output;
        default:
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnsupportedTokenInclusionMode", new object[1]{ (object) parameters.InclusionMode })));
      }
    }

    protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
    {
      if (this.basicSupportingTokenParameters != null && this.basicSupportingTokenParameters.Count > 0 && (this.RequireMessageProtection && !this.basicTokenEncrypted))
        throw TraceUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("BasicTokenCannotBeWrittenWithoutEncryption")), this.Message);
      if (this.elementContainer.Timestamp != null && this.Layout != SecurityHeaderLayout.LaxTimestampLast)
      {
        this.StandardsManager.WSUtilitySpecificationVersion.WriteTimestamp(writer, this.elementContainer.Timestamp);
      }
      if (this.elementContainer.PrerequisiteToken != null)
        this.StandardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) writer, this.elementContainer.PrerequisiteToken);
      if (this.elementContainer.SourceSigningToken != null && SendSecurityHeader.ShouldSerializeToken(this.signingTokenParameters, this.MessageDirection))
      {
        this.StandardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) writer, this.elementContainer.SourceSigningToken);
        if (this.ShouldProtectTokens)
          this.WriteSecurityTokenReferencyEntry(writer, this.elementContainer.SourceSigningToken, this.signingTokenParameters);
      }
      if (this.elementContainer.DerivedSigningToken != null)
        this.StandardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) writer, this.elementContainer.DerivedSigningToken);
      if (this.elementContainer.SourceEncryptionToken != null && this.elementContainer.SourceEncryptionToken != this.elementContainer.SourceSigningToken && SendSecurityHeader.ShouldSerializeToken(this.encryptingTokenParameters, this.MessageDirection))
        this.StandardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) writer, this.elementContainer.SourceEncryptionToken);
      if (this.elementContainer.WrappedEncryptionToken != null)
        this.StandardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) writer, this.elementContainer.WrappedEncryptionToken);
      if (this.elementContainer.DerivedEncryptionToken != null)
        this.StandardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) writer, this.elementContainer.DerivedEncryptionToken);
      if (this.SignThenEncrypt && this.elementContainer.ReferenceList != null)
        this.elementContainer.ReferenceList.WriteTo(writer, ServiceModelDictionaryManager.Instance);
      SecurityToken[] supportingTokens1 = this.elementContainer.GetSignedSupportingTokens();
      if (supportingTokens1 != null)
      {
        for (int index = 0; index < supportingTokens1.Length; ++index)
        {
          this.StandardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) writer, supportingTokens1[index]);
          this.WriteSecurityTokenReferencyEntry(writer, supportingTokens1[index], this.signedTokenParameters[index]);
        }
      }
      SendSecurityHeaderElement[] supportingTokens2 = this.elementContainer.GetBasicSupportingTokens();
      if (supportingTokens2 != null)
      {
        for (int index = 0; index < supportingTokens2.Length; ++index)
        {
          supportingTokens2[index].Item.WriteTo(writer, ServiceModelDictionaryManager.Instance);
          if (this.SignThenEncrypt)
            this.WriteSecurityTokenReferencyEntry(writer, this.basicTokens[index], this.basicSupportingTokenParameters[index]);
        }
      }
      SecurityToken[] supportingTokens3 = this.elementContainer.GetEndorsingSupportingTokens();
      if (supportingTokens3 != null)
      {
        for (int index = 0; index < supportingTokens3.Length; ++index)
        {
          if (SendSecurityHeader.ShouldSerializeToken(this.endorsingTokenParameters[index], this.MessageDirection))
            this.StandardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) writer, supportingTokens3[index]);
        }
      }
      SecurityToken[] supportingTokens4 = this.elementContainer.GetEndorsingDerivedSupportingTokens();
      if (supportingTokens4 != null)
      {
        for (int index = 0; index < supportingTokens4.Length; ++index)
          this.StandardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) writer, supportingTokens4[index]);
      }
      SecurityToken[] supportingTokens5 = this.elementContainer.GetSignedEndorsingSupportingTokens();
      if (supportingTokens5 != null)
      {
        for (int index = 0; index < supportingTokens5.Length; ++index)
        {
          this.StandardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) writer, supportingTokens5[index]);
          this.WriteSecurityTokenReferencyEntry(writer, supportingTokens5[index], this.signedEndorsingTokenParameters[index]);
        }
      }
      SecurityToken[] supportingTokens6 = this.elementContainer.GetSignedEndorsingDerivedSupportingTokens();
      if (supportingTokens6 != null)
      {
        for (int index = 0; index < supportingTokens6.Length; ++index)
          this.StandardsManager.SecurityTokenSerializer.WriteToken((XmlWriter) writer, supportingTokens6[index]);
      }
      SendSecurityHeaderElement[] signatureConfirmations = this.elementContainer.GetSignatureConfirmations();
      if (signatureConfirmations != null)
      {
        for (int index = 0; index < signatureConfirmations.Length; ++index)
          signatureConfirmations[index].Item.WriteTo(writer, ServiceModelDictionaryManager.Instance);
      }
      if (this.elementContainer.PrimarySignature != null && this.elementContainer.PrimarySignature.Item != null)
        this.elementContainer.PrimarySignature.Item.WriteTo(writer, ServiceModelDictionaryManager.Instance);
      SendSecurityHeaderElement[] endorsingSignatures = this.elementContainer.GetEndorsingSignatures();
      if (endorsingSignatures != null)
      {
        for (int index = 0; index < endorsingSignatures.Length; ++index)
          endorsingSignatures[index].Item.WriteTo(writer, ServiceModelDictionaryManager.Instance);
      }
      if (!this.SignThenEncrypt && this.elementContainer.ReferenceList != null)
        this.elementContainer.ReferenceList.WriteTo(writer, ServiceModelDictionaryManager.Instance);
      if (this.elementContainer.Timestamp == null || this.Layout != SecurityHeaderLayout.LaxTimestampLast)
        return;
      this.StandardsManager.WSUtilitySpecificationVersion.WriteTimestamp(writer, this.elementContainer.Timestamp);
    }

    protected abstract void WriteSecurityTokenReferencyEntry(XmlDictionaryWriter writer, SecurityToken securityToken, SecurityTokenParameters securityTokenParameters);

    public Message SetupExecution()
    {
      this.ThrowIfProcessingStarted();
      this.SetProcessingStarted();
      bool signBody = false;
      if (this.elementContainer.SourceSigningToken != null)
      {
        if (this.signatureParts == null)
          throw TraceUtility.ThrowHelperError((Exception) new ArgumentNullException("SignatureParts"), this.Message);
        signBody = this.signatureParts.IsBodyIncluded;
      }
      bool encryptBody = false;
      if (this.elementContainer.SourceEncryptionToken != null)
      {
        if (this.encryptionParts == null)
          throw TraceUtility.ThrowHelperError((Exception) new ArgumentNullException("EncryptionParts"), this.Message);
        encryptBody = this.encryptionParts.IsBodyIncluded;
      }
      SecurityAppliedMessage securityAppliedMessage = new SecurityAppliedMessage(this.Message, this, signBody, encryptBody);
      this.Message = (Message) securityAppliedMessage;
      return (Message) securityAppliedMessage;
    }

    protected internal SecurityTokenReferenceStyle GetTokenReferenceStyle(SecurityTokenParameters parameters)
    {
      return !SendSecurityHeader.ShouldSerializeToken(parameters, this.MessageDirection) ? SecurityTokenReferenceStyle.External : SecurityTokenReferenceStyle.Internal;
    }

    private void StartSignature()
    {
      if (this.elementContainer.SourceSigningToken == null)
        return;
#if FEATURE_CORECLR
        throw new NotImplementedException("SecurityAlgorithmSuite.GetSignatureKeyDerivationLength is not supported in .NET Core");
#else
      SecurityKeyIdentifierClause identifierClause1 = this.signingTokenParameters.CreateKeyIdentifierClause(this.elementContainer.SourceSigningToken, this.GetTokenReferenceStyle(this.signingTokenParameters));
      if (identifierClause1 == null)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenManagerCannotCreateTokenReference")), this.Message);
      SecurityToken token;
      SecurityKeyIdentifierClause identifierClause2;
      if (this.signingTokenParameters.RequireDerivedKeys && !this.signingTokenParameters.HasAsymmetricKey)
      {
        string derivationAlgorithm1 = this.AlgorithmSuite.GetSignatureKeyDerivationAlgorithm(this.elementContainer.SourceSigningToken, this.StandardsManager.MessageSecurityVersion.SecureConversationVersion);
        string derivationAlgorithm2 = SecurityUtils.GetKeyDerivationAlgorithm(this.StandardsManager.MessageSecurityVersion.SecureConversationVersion);
        if (derivationAlgorithm1 == derivationAlgorithm2)
        {
          token = this.elementContainer.DerivedSigningToken = (SecurityToken) new DerivedKeySecurityToken(-1, 0, this.AlgorithmSuite.GetSignatureKeyDerivationLength(this.elementContainer.SourceSigningToken, this.StandardsManager.MessageSecurityVersion.SecureConversationVersion), (string) null, 16, this.elementContainer.SourceSigningToken, identifierClause1, derivationAlgorithm1, this.GenerateId());
          identifierClause2 = (SecurityKeyIdentifierClause) new LocalIdKeyIdentifierClause(token.Id, token.GetType());
        }
        else
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnsupportedCryptoAlgorithm", new object[1]{ (object) derivationAlgorithm1 })));
      }
      else
      {
        token = this.elementContainer.SourceSigningToken;
        identifierClause2 = identifierClause1;
      }
      SecurityKeyIdentifier identifier = new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[1]{ identifierClause2 });
      if (this.signatureConfirmationsToSend != null && this.signatureConfirmationsToSend.Count > 0)
      {
        ISecurityElement[] confirmationElements = (ISecurityElement[]) this.CreateSignatureConfirmationElements(this.signatureConfirmationsToSend);
        for (int index = 0; index < confirmationElements.Length; ++index)
          this.elementContainer.AddSignatureConfirmation(new SendSecurityHeaderElement(confirmationElements[index].Id, confirmationElements[index])
          {
            MarkedForEncryption = this.signatureConfirmationsToSend.IsMarkedForEncryption
          });
      }
      bool generateTargettablePrimarySignature = this.endorsingTokenParameters != null || this.signedEndorsingTokenParameters != null;
      this.StartPrimarySignatureCore(token, identifier, this.signatureParts, generateTargettablePrimarySignature);
#endif
    }

    private void CompleteSignature()
    {
      ISignatureValueSecurityElement valueSecurityElement = this.CompletePrimarySignatureCore(this.elementContainer.GetSignatureConfirmations(), this.elementContainer.GetSignedEndorsingSupportingTokens(), this.elementContainer.GetSignedSupportingTokens(), this.elementContainer.GetBasicSupportingTokens(), true);
      if (valueSecurityElement == null)
        return;
      this.elementContainer.PrimarySignature = new SendSecurityHeaderElement(valueSecurityElement.Id, (ISecurityElement) valueSecurityElement);
      this.elementContainer.PrimarySignature.MarkedForEncryption = this.encryptSignature;
      this.AddGeneratedSignatureValue(valueSecurityElement.GetSignatureValue(), this.EncryptPrimarySignature);
      this.primarySignatureDone = true;
      this.primarySignatureValue = valueSecurityElement.GetSignatureValue();
    }

    protected abstract void StartPrimarySignatureCore(SecurityToken token, SecurityKeyIdentifier identifier, MessagePartSpecification signatureParts, bool generateTargettablePrimarySignature);

    internal abstract ISignatureValueSecurityElement CompletePrimarySignatureCore(SendSecurityHeaderElement[] signatureConfirmations, SecurityToken[] signedEndorsingTokens, SecurityToken[] signedTokens, SendSecurityHeaderElement[] basicTokens, bool isPrimarySignature);

    internal abstract ISignatureValueSecurityElement CreateSupportingSignature(SecurityToken token, SecurityKeyIdentifier identifier);

    internal abstract ISignatureValueSecurityElement CreateSupportingSignature(SecurityToken token, SecurityKeyIdentifier identifier, ISecurityElement primarySignature);

    protected abstract void StartEncryptionCore(SecurityToken token, SecurityKeyIdentifier keyIdentifier);

    internal abstract ISecurityElement CompleteEncryptionCore(SendSecurityHeaderElement primarySignature, SendSecurityHeaderElement[] basicTokens, SendSecurityHeaderElement[] signatureConfirmations, SendSecurityHeaderElement[] endorsingSignatures);

    private void SignWithSupportingToken(SecurityToken token, SecurityKeyIdentifierClause identifierClause)
    {
      if (token == null)
        throw TraceUtility.ThrowHelperArgumentNull("token", this.Message);
      if (identifierClause == null)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenManagerCannotCreateTokenReference")), this.Message);
      if (!this.RequireMessageProtection)
      {
        if (this.elementContainer.Timestamp == null)
          throw TraceUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SigningWithoutPrimarySignatureRequiresTimestamp")), this.Message);
      }
      else
      {
        if (!this.primarySignatureDone)
          throw TraceUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("PrimarySignatureMustBeComputedBeforeSupportingTokenSignatures")), this.Message);
        if (this.elementContainer.PrimarySignature.Item == null)
          throw TraceUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SupportingTokenSignaturesNotExpected")), this.Message);
      }
      SecurityKeyIdentifier identifier = new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[1]{ identifierClause });
      ISignatureValueSecurityElement valueSecurityElement = this.RequireMessageProtection ? this.CreateSupportingSignature(token, identifier, this.elementContainer.PrimarySignature.Item) : this.CreateSupportingSignature(token, identifier);
      this.AddGeneratedSignatureValue(valueSecurityElement.GetSignatureValue(), this.encryptSignature);
      this.elementContainer.AddEndorsingSignature(new SendSecurityHeaderElement(valueSecurityElement.Id, (ISecurityElement) valueSecurityElement)
      {
        MarkedForEncryption = this.encryptSignature
      });
    }

    private void SignWithSupportingTokens()
    {
      SecurityToken[] supportingTokens1 = this.elementContainer.GetEndorsingSupportingTokens();
      if (supportingTokens1 != null)
      {
        for (int index = 0; index < supportingTokens1.Length; ++index)
        {
          SecurityToken securityToken = supportingTokens1[index];
          SecurityKeyIdentifierClause identifierClause1 = this.endorsingTokenParameters[index].CreateKeyIdentifierClausePublic(securityToken, this.GetTokenReferenceStyle(this.endorsingTokenParameters[index]));
          if (identifierClause1 == null)
            throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenManagerCannotCreateTokenReference")), this.Message);
          SecurityToken token;
          SecurityKeyIdentifierClause identifierClause2;
          if (this.endorsingTokenParameters[index].RequireDerivedKeys && !this.endorsingTokenParameters[index].HasAsymmetricKey)
          {
            string derivationAlgorithm = SecurityUtils.GetKeyDerivationAlgorithm(this.StandardsManager.MessageSecurityVersion.SecureConversationVersion);
            DerivedKeySecurityToken keySecurityToken = new DerivedKeySecurityToken(-1, 0, this.AlgorithmSuite.GetSignatureKeyDerivationLength(securityToken, this.StandardsManager.MessageSecurityVersion.SecureConversationVersion), (string) null, 16, securityToken, identifierClause1, derivationAlgorithm, this.GenerateId());
            token = (SecurityToken) keySecurityToken;
            identifierClause2 = (SecurityKeyIdentifierClause) new LocalIdKeyIdentifierClause(keySecurityToken.Id, keySecurityToken.GetType());
            this.elementContainer.AddEndorsingDerivedSupportingToken((SecurityToken) keySecurityToken);
          }
          else
          {
            token = securityToken;
            identifierClause2 = identifierClause1;
          }
          this.SignWithSupportingToken(token, identifierClause2);
        }
      }
      SecurityToken[] supportingTokens2 = this.elementContainer.GetSignedEndorsingSupportingTokens();
      if (supportingTokens2 == null)
        return;
      for (int index = 0; index < supportingTokens2.Length; ++index)
      {
        SecurityToken securityToken = supportingTokens2[index];
        SecurityKeyIdentifierClause identifierClause1 = this.signedEndorsingTokenParameters[index].CreateKeyIdentifierClausePublic(securityToken, this.GetTokenReferenceStyle(this.signedEndorsingTokenParameters[index]));
        if (identifierClause1 == null)
          throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenManagerCannotCreateTokenReference")), this.Message);
        SecurityToken token;
        SecurityKeyIdentifierClause identifierClause2;
        if (this.signedEndorsingTokenParameters[index].RequireDerivedKeys && !this.signedEndorsingTokenParameters[index].HasAsymmetricKey)
        {
          string derivationAlgorithm = SecurityUtils.GetKeyDerivationAlgorithm(this.StandardsManager.MessageSecurityVersion.SecureConversationVersion);
          DerivedKeySecurityToken keySecurityToken = new DerivedKeySecurityToken(-1, 0, this.AlgorithmSuite.GetSignatureKeyDerivationLength(securityToken, this.StandardsManager.MessageSecurityVersion.SecureConversationVersion), (string) null, 16, securityToken, identifierClause1, derivationAlgorithm, this.GenerateId());
          token = (SecurityToken) keySecurityToken;
          identifierClause2 = (SecurityKeyIdentifierClause) new LocalIdKeyIdentifierClause(keySecurityToken.Id, keySecurityToken.GetType());
          this.elementContainer.AddSignedEndorsingDerivedSupportingToken((SecurityToken) keySecurityToken);
        }
        else
        {
          token = securityToken;
          identifierClause2 = identifierClause1;
        }
        this.SignWithSupportingToken(token, identifierClause2);
      }
    }

    protected bool ShouldUseStrTransformForToken(SecurityToken securityToken, int position, SecurityTokenAttachmentMode mode, out SecurityKeyIdentifierClause keyIdentifierClause)
    {
      keyIdentifierClause = (SecurityKeyIdentifierClause) null;
      IssuedSecurityTokenParameters securityTokenParameters;
      switch (mode)
      {
        case SecurityTokenAttachmentMode.Signed:
          securityTokenParameters = this.signedTokenParameters[position] as IssuedSecurityTokenParameters;
          break;
        case SecurityTokenAttachmentMode.SignedEndorsing:
          securityTokenParameters = this.signedEndorsingTokenParameters[position] as IssuedSecurityTokenParameters;
          break;
        case SecurityTokenAttachmentMode.SignedEncrypted:
          securityTokenParameters = this.basicSupportingTokenParameters[position] as IssuedSecurityTokenParameters;
          break;
        default:
          return false;
      }
      if (securityTokenParameters == null || !securityTokenParameters.UseStrTransform)
        return false;
#if FEATURE_CORECLR
      throw new NotImplementedException("CreateKeyIdentifierClause");
#else
      keyIdentifierClause = securityTokenParameters.CreateKeyIdentifierClause(securityToken, this.GetTokenReferenceStyle((SecurityTokenParameters) securityTokenParameters));
      if (keyIdentifierClause == null)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenManagerCannotCreateTokenReference")), this.Message);
      return true;
#endif
    }

    private void AddGeneratedSignatureValue(byte[] signatureValue, bool wasEncrypted)
    {
      if (!this.MaintainSignatureConfirmationState || this.signatureConfirmationsToSend != null)
        return;
      if (this.signatureValuesGenerated == null)
        this.signatureValuesGenerated = new SignatureConfirmations();
      this.signatureValuesGenerated.AddConfirmation(signatureValue, wasEncrypted);
    }
  }
}
