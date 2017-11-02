// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.ReceiveSecurityHeader
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.Runtime.Diagnostics;
using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Diagnostics.Application;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
  public abstract class ReceiveSecurityHeader : SecurityHeader
  {
    private bool expectEncryption = true;
    private bool expectSignature = true;
    private bool enforceDerivedKeyRequirement = true;
    private long maxReceivedMessageSize = 65536;
    private SecurityTokenAuthenticator primaryTokenAuthenticator;
    private bool allowFirstTokenMismatch;
    private SecurityToken outOfBandPrimaryToken;
    private IList<SecurityToken> outOfBandPrimaryTokenCollection;
    private SecurityTokenParameters primaryTokenParameters;
    private TokenTracker primaryTokenTracker;
    private SecurityToken wrappingToken;
    private SecurityTokenParameters wrappingTokenParameters;
    private SecurityToken expectedEncryptionToken;
    private SecurityTokenParameters expectedEncryptionTokenParameters;
    private SecurityTokenAuthenticator derivedTokenAuthenticator;
    private IList<SupportingTokenAuthenticatorSpecification> supportingTokenAuthenticators;
    private ChannelBinding channelBinding;
    private ExtendedProtectionPolicy extendedProtectionPolicy;
    private bool expectBasicTokens;
    private bool expectSignedTokens;
    private bool expectEndorsingTokens;
    private bool requireSignedPrimaryToken;
    private bool expectSignatureConfirmation;
    private List<TokenTracker> supportingTokenTrackers;
    private SignatureConfirmations receivedSignatureValues;
    private SignatureConfirmations receivedSignatureConfirmations;
    private List<SecurityTokenAuthenticator> allowedAuthenticators;
    private SecurityTokenAuthenticator pendingSupportingTokenAuthenticator;
    private WrappedKeySecurityToken wrappedKeyToken;
    private Collection<SecurityToken> basicTokens;
    private Collection<SecurityToken> signedTokens;
    private Collection<SecurityToken> endorsingTokens;
    private Collection<SecurityToken> signedEndorsingTokens;
    private Dictionary<SecurityToken, ReadOnlyCollection<IAuthorizationPolicy>> tokenPoliciesMapping;
    private List<SecurityTokenAuthenticator> wrappedKeyAuthenticator;
    private SecurityTimestamp timestamp;
    private SecurityHeaderTokenResolver universalTokenResolver;
    private SecurityHeaderTokenResolver primaryTokenResolver;
    private ReadOnlyCollection<SecurityTokenResolver> outOfBandTokenResolver;
    private SecurityTokenResolver combinedUniversalTokenResolver;
    private SecurityTokenResolver combinedPrimaryTokenResolver;
    private readonly int headerIndex;
    private System.ServiceModel.Channels.XmlAttributeHolder[] securityElementAttributes;
    private ReceiveSecurityHeader.OrderTracker orderTracker;
    private ReceiveSecurityHeader.OperationTracker signatureTracker;
    private ReceiveSecurityHeader.OperationTracker encryptionTracker;
    private ReceiveSecurityHeaderElementManager elementManager;
    private int maxDerivedKeys;
    private int numDerivedKeys;
    private int maxDerivedKeyLength;
    private NonceCache nonceCache;
    private TimeSpan replayWindow;
    private TimeSpan clockSkew;
    private byte[] primarySignatureValue;
    private TimeoutHelper timeoutHelper;
    private SecurityVerifiedMessage securityVerifiedMessage;
    private XmlDictionaryReaderQuotas readerQuotas;
    private MessageProtectionOrder protectionOrder;
    private bool hasAtLeastOneSupportingTokenExpectedToBeSigned;
    private bool hasEndorsingOrSignedEndorsingSupportingTokens;
    private SignatureResourcePool resourcePool;
    private bool replayDetectionEnabled;
    private bool hasAtLeastOneItemInsideSecurityHeaderEncrypted;
    private const int AppendPosition = -1;
    private EventTraceActivity eventTraceActivity;

    public Collection<SecurityToken> BasicSupportingTokens
    {
      get
      {
        return this.basicTokens;
      }
    }

    public Collection<SecurityToken> SignedSupportingTokens
    {
      get
      {
        return this.signedTokens;
      }
    }

    public Collection<SecurityToken> EndorsingSupportingTokens
    {
      get
      {
        return this.endorsingTokens;
      }
    }

    internal ReceiveSecurityHeaderElementManager ElementManager
    {
      get
      {
        return this.elementManager;
      }
    }

    public Collection<SecurityToken> SignedEndorsingSupportingTokens
    {
      get
      {
        return this.signedEndorsingTokens;
      }
    }

    public SecurityTokenAuthenticator DerivedTokenAuthenticator
    {
      get
      {
        return this.derivedTokenAuthenticator;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.derivedTokenAuthenticator = value;
      }
    }

    public List<SecurityTokenAuthenticator> WrappedKeySecurityTokenAuthenticator
    {
      get
      {
        return this.wrappedKeyAuthenticator;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.wrappedKeyAuthenticator = value;
      }
    }

    public bool EnforceDerivedKeyRequirement
    {
      get
      {
        return this.enforceDerivedKeyRequirement;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.enforceDerivedKeyRequirement = value;
      }
    }

    public byte[] PrimarySignatureValue
    {
      get
      {
        return this.primarySignatureValue;
      }
    }

    public bool EncryptBeforeSignMode
    {
      get
      {
        return this.orderTracker.EncryptBeforeSignMode;
      }
    }

    public SecurityToken EncryptionToken
    {
      get
      {
        return this.encryptionTracker.Token;
      }
    }

    public bool ExpectBasicTokens
    {
      get
      {
        return this.expectBasicTokens;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.expectBasicTokens = value;
      }
    }

    public bool ReplayDetectionEnabled
    {
      get
      {
        return this.replayDetectionEnabled;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.replayDetectionEnabled = value;
      }
    }

    public bool ExpectEncryption
    {
      get
      {
        return this.expectEncryption;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.expectEncryption = value;
      }
    }

    public bool ExpectSignature
    {
      get
      {
        return this.expectSignature;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.expectSignature = value;
      }
    }

    public bool ExpectSignatureConfirmation
    {
      get
      {
        return this.expectSignatureConfirmation;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.expectSignatureConfirmation = value;
      }
    }

    public bool ExpectSignedTokens
    {
      get
      {
        return this.expectSignedTokens;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.expectSignedTokens = value;
      }
    }

    public bool RequireSignedPrimaryToken
    {
      get
      {
        return this.requireSignedPrimaryToken;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.requireSignedPrimaryToken = value;
      }
    }

    public bool ExpectEndorsingTokens
    {
      get
      {
        return this.expectEndorsingTokens;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.expectEndorsingTokens = value;
      }
    }

    public bool HasAtLeastOneItemInsideSecurityHeaderEncrypted
    {
      get
      {
        return this.hasAtLeastOneItemInsideSecurityHeaderEncrypted;
      }
      set
      {
        this.hasAtLeastOneItemInsideSecurityHeaderEncrypted = value;
      }
    }

    internal SecurityHeaderTokenResolver PrimaryTokenResolver
    {
      get
      {
        return this.primaryTokenResolver;
      }
    }

    public SecurityTokenResolver CombinedUniversalTokenResolver
    {
      get
      {
        return this.combinedUniversalTokenResolver;
      }
    }

    public SecurityTokenResolver CombinedPrimaryTokenResolver
    {
      get
      {
        return this.combinedPrimaryTokenResolver;
      }
    }

    protected EventTraceActivity EventTraceActivity
    {
      get
      {
        if (this.eventTraceActivity == null && FxTrace.Trace.IsEnd2EndActivityTracingEnabled)
          this.eventTraceActivity = EventTraceActivityHelper.TryExtractActivity(OperationContext.Current != null ? OperationContext.Current.IncomingMessage : (Message) null);
        return this.eventTraceActivity;
      }
    }

    internal int HeaderIndex
    {
      get
      {
        return this.headerIndex;
      }
    }

    internal long MaxReceivedMessageSize
    {
      get
      {
        return this.maxReceivedMessageSize;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        this.maxReceivedMessageSize = value;
      }
    }

    internal XmlDictionaryReaderQuotas ReaderQuotas
    {
      get
      {
        return this.readerQuotas;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        if (value == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
        this.readerQuotas = value;
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

    public Message ProcessedMessage
    {
      get
      {
        return this.Message;
      }
    }

    public MessagePartSpecification RequiredEncryptionParts
    {
      get
      {
        return this.encryptionTracker.Parts;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        if (value == null)
          throw TraceUtility.ThrowHelperError((Exception) new ArgumentNullException("value"), this.Message);
        if (!value.IsReadOnly)
          throw TraceUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("MessagePartSpecificationMustBeImmutable")), this.Message);
        this.encryptionTracker.Parts = value;
      }
    }

    public MessagePartSpecification RequiredSignatureParts
    {
      get
      {
        return this.signatureTracker.Parts;
      }
      set
      {
        this.ThrowIfProcessingStarted();
        if (value == null)
          throw TraceUtility.ThrowHelperError((Exception) new ArgumentNullException("value"), this.Message);
        if (!value.IsReadOnly)
          throw TraceUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("MessagePartSpecificationMustBeImmutable")), this.Message);
        this.signatureTracker.Parts = value;
      }
    }

    internal SignatureResourcePool ResourcePool
    {
      get
      {
        if (this.resourcePool == null)
          this.resourcePool = new SignatureResourcePool();
        return this.resourcePool;
      }
    }

    internal SecurityVerifiedMessage SecurityVerifiedMessage
    {
      get
      {
        return this.securityVerifiedMessage;
      }
    }

    public SecurityToken SignatureToken
    {
      get
      {
        return this.signatureTracker.Token;
      }
    }

    public Dictionary<SecurityToken, ReadOnlyCollection<IAuthorizationPolicy>> SecurityTokenAuthorizationPoliciesMapping
    {
      get
      {
        if (this.tokenPoliciesMapping == null)
          this.tokenPoliciesMapping = new Dictionary<SecurityToken, ReadOnlyCollection<IAuthorizationPolicy>>();
        return this.tokenPoliciesMapping;
      }
    }

    internal SecurityTimestamp Timestamp
    {
      get
      {
        return this.timestamp;
      }
    }

    public int MaxDerivedKeyLength
    {
      get
      {
        return this.maxDerivedKeyLength;
      }
    }

    protected ReceiveSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay, SecurityStandardsManager standardsManager, SecurityAlgorithmSuite algorithmSuite, int headerIndex, MessageDirection direction)
      : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, direction)
    {
      this.headerIndex = headerIndex;
      this.elementManager = new ReceiveSecurityHeaderElementManager(this);
    }

    protected void VerifySignatureEncryption()
    {
      if (this.protectionOrder == MessageProtectionOrder.SignBeforeEncryptAndEncryptSignature && !this.orderTracker.AllSignaturesEncrypted)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("PrimarySignatureIsRequiredToBeEncrypted")));
    }

    internal XmlDictionaryReader CreateSecurityHeaderReader()
    {
      return this.securityVerifiedMessage.GetReaderAtSecurityHeader();
    }

    public SignatureConfirmations GetSentSignatureConfirmations()
    {
      return this.receivedSignatureConfirmations;
    }

    internal void ConfigureSymmetricBindingServerReceiveHeader(SecurityTokenAuthenticator primaryTokenAuthenticator, SecurityTokenParameters primaryTokenParameters, IList<SupportingTokenAuthenticatorSpecification> supportingTokenAuthenticators)
    {
      this.primaryTokenAuthenticator = primaryTokenAuthenticator;
      this.primaryTokenParameters = primaryTokenParameters;
      this.supportingTokenAuthenticators = supportingTokenAuthenticators;
    }

    internal void ConfigureSymmetricBindingServerReceiveHeader(SecurityToken wrappingToken, SecurityTokenParameters wrappingTokenParameters, IList<SupportingTokenAuthenticatorSpecification> supportingTokenAuthenticators)
    {
      this.wrappingToken = wrappingToken;
      this.wrappingTokenParameters = wrappingTokenParameters;
      this.supportingTokenAuthenticators = supportingTokenAuthenticators;
    }

    internal void ConfigureAsymmetricBindingServerReceiveHeader(SecurityTokenAuthenticator primaryTokenAuthenticator, SecurityTokenParameters primaryTokenParameters, SecurityToken wrappingToken, SecurityTokenParameters wrappingTokenParameters, IList<SupportingTokenAuthenticatorSpecification> supportingTokenAuthenticators)
    {
      this.primaryTokenAuthenticator = primaryTokenAuthenticator;
      this.primaryTokenParameters = primaryTokenParameters;
      this.wrappingToken = wrappingToken;
      this.wrappingTokenParameters = wrappingTokenParameters;
      this.supportingTokenAuthenticators = supportingTokenAuthenticators;
    }

    internal void ConfigureTransportBindingServerReceiveHeader(IList<SupportingTokenAuthenticatorSpecification> supportingTokenAuthenticators)
    {
      this.supportingTokenAuthenticators = supportingTokenAuthenticators;
    }

    public void ConfigureAsymmetricBindingClientReceiveHeader(SecurityToken primaryToken, SecurityTokenParameters primaryTokenParameters, SecurityToken encryptionToken, SecurityTokenParameters encryptionTokenParameters, SecurityTokenAuthenticator primaryTokenAuthenticator)
    {
#if FEATURE_CORECLR
	  throw new NotImplementedException("SecurityUtils.HasSymmetricSecurityKey is not supported in .NET Core");
#else
      this.outOfBandPrimaryToken = primaryToken;
      this.primaryTokenParameters = primaryTokenParameters;
      this.primaryTokenAuthenticator = primaryTokenAuthenticator;
      this.allowFirstTokenMismatch = primaryTokenAuthenticator != null;
      if (encryptionToken != null && !SecurityUtils.HasSymmetricSecurityKey(encryptionToken))
      {
        this.wrappingToken = encryptionToken;
        this.wrappingTokenParameters = encryptionTokenParameters;
      }
      else
      {
        this.expectedEncryptionToken = encryptionToken;
        this.expectedEncryptionTokenParameters = encryptionTokenParameters;
      }
#endif
    }

    public void ConfigureSymmetricBindingClientReceiveHeader(SecurityToken primaryToken, SecurityTokenParameters primaryTokenParameters)
    {
      this.outOfBandPrimaryToken = primaryToken;
      this.primaryTokenParameters = primaryTokenParameters;
    }

    public void ConfigureSymmetricBindingClientReceiveHeader(IList<SecurityToken> primaryTokens, SecurityTokenParameters primaryTokenParameters)
    {
      this.outOfBandPrimaryTokenCollection = primaryTokens;
      this.primaryTokenParameters = primaryTokenParameters;
    }

    public void ConfigureOutOfBandTokenResolver(ReadOnlyCollection<SecurityTokenResolver> outOfBandResolvers)
    {
      if (outOfBandResolvers == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("outOfBandResolvers");
      if (outOfBandResolvers.Count == 0)
        return;
      this.outOfBandTokenResolver = outOfBandResolvers;
    }

    internal abstract EncryptedData ReadSecurityHeaderEncryptedItem(XmlDictionaryReader reader, bool readXmlreferenceKeyInfoClause);

    internal abstract byte[] DecryptSecurityHeaderElement(EncryptedData encryptedData, WrappedKeySecurityToken wrappedKeyToken, out SecurityToken encryptionToken);

    protected abstract WrappedKeySecurityToken DecryptWrappedKey(XmlDictionaryReader reader);

    public SignatureConfirmations GetSentSignatureValues()
    {
      return this.receivedSignatureValues;
    }

    protected abstract bool IsReaderAtEncryptedKey(XmlDictionaryReader reader);

    protected abstract bool IsReaderAtEncryptedData(XmlDictionaryReader reader);

    protected abstract bool IsReaderAtReferenceList(XmlDictionaryReader reader);

    protected abstract bool IsReaderAtSignature(XmlDictionaryReader reader);

    protected abstract bool IsReaderAtSecurityTokenReference(XmlDictionaryReader reader);

    protected abstract void OnDecryptionOfSecurityHeaderItemRequiringReferenceListEntry(string id);

    private void MarkHeaderAsUnderstood()
    {
      this.Message.Headers.UnderstoodHeaders.Add(this.Message.Headers[this.headerIndex]);
    }

    protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
    {
      this.StandardsManager.SecurityVersion.WriteStartHeader(writer);
      System.ServiceModel.Channels.XmlAttributeHolder[] elementAttributes = this.securityElementAttributes;
      for (int index = 0; index < elementAttributes.Length; ++index)
        writer.WriteAttributeString(elementAttributes[index].Prefix, elementAttributes[index].LocalName, elementAttributes[index].NamespaceUri, elementAttributes[index].Value);
    }

    protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
    {
      XmlDictionaryReader atSecurityHeader = this.GetReaderAtSecurityHeader();
      atSecurityHeader.ReadStartElement();
      for (int index = 0; index < this.ElementManager.Count; ++index)
      {
        ReceiveSecurityHeaderEntry element;
        this.ElementManager.GetElementEntry(index, out element);
        if (element.encrypted)
        {
          XmlDictionaryReader reader = this.ElementManager.GetReader(index, false);
          writer.WriteNode(reader, false);
          reader.Close();
          atSecurityHeader.Skip();
        }
        else
          writer.WriteNode(atSecurityHeader, false);
      }
      atSecurityHeader.Close();
    }

    private XmlDictionaryReader GetReaderAtSecurityHeader()
    {
      XmlDictionaryReader readerAtFirstHeader = this.SecurityVerifiedMessage.GetReaderAtFirstHeader();
      for (int index = 0; index < this.HeaderIndex; ++index)
        readerAtFirstHeader.Skip();
      return readerAtFirstHeader;
    }

    private Collection<SecurityToken> EnsureSupportingTokens(ref Collection<SecurityToken> list)
    {
      if (list == null)
        list = new Collection<SecurityToken>();
      return list;
    }

    private void VerifySupportingToken(TokenTracker tracker)
    {
      if (tracker == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tracker");
      SupportingTokenAuthenticatorSpecification spec = tracker.spec;
      if (tracker.token == null)
      {
        if (!spec.IsTokenOptional)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("SupportingTokenNotProvided", (object) spec.TokenParameters, (object) spec.SecurityTokenAttachmentMode)));
      }
      else
      {
        switch (spec.SecurityTokenAttachmentMode)
        {
          case SecurityTokenAttachmentMode.Signed:
            if (!tracker.IsSigned && this.RequireMessageProtection)
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("SupportingTokenIsNotSigned", new object[1]{ (object) spec.TokenParameters })));
            this.EnsureSupportingTokens(ref this.signedTokens).Add(tracker.token);
            break;
          case SecurityTokenAttachmentMode.Endorsing:
            if (!tracker.IsEndorsing)
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("SupportingTokenIsNotEndorsing", new object[1]{ (object) spec.TokenParameters })));
#if FEATURE_CORECLR
            // TokenParameters.HasAsymmetricKey not supported
#else
            if (this.EnforceDerivedKeyRequirement && spec.TokenParameters.RequireDerivedKeys && (!spec.TokenParameters.HasAsymmetricKey && !tracker.IsDerivedFrom))
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("SupportingSignatureIsNotDerivedFrom", new object[1]{ (object) spec.TokenParameters })));
#endif
            this.EnsureSupportingTokens(ref this.endorsingTokens).Add(tracker.token);
            break;
          case SecurityTokenAttachmentMode.SignedEndorsing:
            if (!tracker.IsSigned && this.RequireMessageProtection)
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("SupportingTokenIsNotSigned", new object[1]{ (object) spec.TokenParameters })));
            if (!tracker.IsEndorsing)
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("SupportingTokenIsNotEndorsing", new object[1]{ (object) spec.TokenParameters })));
#if FEATURE_CORECLR
            // TokenParameters.HasAsymmetricKey not supported
#else
            if (this.EnforceDerivedKeyRequirement && spec.TokenParameters.RequireDerivedKeys && (!spec.TokenParameters.HasAsymmetricKey && !tracker.IsDerivedFrom))
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("SupportingSignatureIsNotDerivedFrom", new object[1]{ (object) spec.TokenParameters })));
#endif
            this.EnsureSupportingTokens(ref this.signedEndorsingTokens).Add(tracker.token);
            break;
          case SecurityTokenAttachmentMode.SignedEncrypted:
            if (!tracker.IsSigned && this.RequireMessageProtection)
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("SupportingTokenIsNotSigned", new object[1]{ (object) spec.TokenParameters })));
            if (!tracker.IsEncrypted && this.RequireMessageProtection)
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("SupportingTokenIsNotEncrypted", new object[1]{ (object) spec.TokenParameters })));
            this.EnsureSupportingTokens(ref this.basicTokens).Add(tracker.token);
            break;
          default:
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UnknownTokenAttachmentMode", new object[1]{ (object) spec.SecurityTokenAttachmentMode })));
        }
      }
    }

    public void SetTimeParameters(NonceCache nonceCache, TimeSpan replayWindow, TimeSpan clockSkew)
    {
      this.nonceCache = nonceCache;
      this.replayWindow = replayWindow;
      this.clockSkew = clockSkew;
    }

    public void Process(TimeSpan timeout, ChannelBinding channelBinding, ExtendedProtectionPolicy extendedProtectionPolicy)
    {
      MessageProtectionOrder protectionOrder = this.protectionOrder;
      bool flag = false;
      if (this.protectionOrder == MessageProtectionOrder.SignBeforeEncryptAndEncryptSignature && (this.RequiredEncryptionParts == null || !this.RequiredEncryptionParts.IsBodyIncluded))
      {
        protectionOrder = MessageProtectionOrder.SignBeforeEncrypt;
        flag = true;
      }
      this.channelBinding = channelBinding;
      this.extendedProtectionPolicy = extendedProtectionPolicy;
      this.orderTracker.SetRequiredProtectionOrder(protectionOrder);
      this.SetProcessingStarted();
      this.timeoutHelper = new TimeoutHelper(timeout);
      this.Message = (Message) (this.securityVerifiedMessage = new SecurityVerifiedMessage(this.Message, this));
      XmlDictionaryReader securityHeaderReader = this.CreateSecurityHeaderReader();
      securityHeaderReader.MoveToStartElement();
      if (securityHeaderReader.IsEmptyElement)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("SecurityHeaderIsEmpty")), this.Message);
      this.securityElementAttributes = !this.RequireMessageProtection ? System.ServiceModel.Channels.XmlAttributeHolder.emptyArray : System.ServiceModel.Channels.XmlAttributeHolder.ReadAttributes(securityHeaderReader);
      securityHeaderReader.ReadStartElement();
      if (this.primaryTokenParameters != null)
        this.primaryTokenTracker = new TokenTracker((SupportingTokenAuthenticatorSpecification) null, this.outOfBandPrimaryToken, this.allowFirstTokenMismatch);
      this.universalTokenResolver = new SecurityHeaderTokenResolver(this);
      this.primaryTokenResolver = new SecurityHeaderTokenResolver(this);
      if (this.outOfBandPrimaryToken != null)
      {
        this.universalTokenResolver.Add(this.outOfBandPrimaryToken, SecurityTokenReferenceStyle.External, this.primaryTokenParameters);
        this.primaryTokenResolver.Add(this.outOfBandPrimaryToken, SecurityTokenReferenceStyle.External, this.primaryTokenParameters);
      }
      else if (this.outOfBandPrimaryTokenCollection != null)
      {
        for (int index = 0; index < this.outOfBandPrimaryTokenCollection.Count; ++index)
        {
          this.universalTokenResolver.Add(this.outOfBandPrimaryTokenCollection[index], SecurityTokenReferenceStyle.External, this.primaryTokenParameters);
          this.primaryTokenResolver.Add(this.outOfBandPrimaryTokenCollection[index], SecurityTokenReferenceStyle.External, this.primaryTokenParameters);
        }
      }
      if (this.wrappingToken != null)
      {
        this.universalTokenResolver.ExpectedWrapper = this.wrappingToken;
        this.universalTokenResolver.ExpectedWrapperTokenParameters = this.wrappingTokenParameters;
        this.primaryTokenResolver.ExpectedWrapper = this.wrappingToken;
        this.primaryTokenResolver.ExpectedWrapperTokenParameters = this.wrappingTokenParameters;
      }
      else if (this.expectedEncryptionToken != null)
      {
        this.universalTokenResolver.Add(this.expectedEncryptionToken, SecurityTokenReferenceStyle.External, this.expectedEncryptionTokenParameters);
        this.primaryTokenResolver.Add(this.expectedEncryptionToken, SecurityTokenReferenceStyle.External, this.expectedEncryptionTokenParameters);
      }
      if (this.outOfBandTokenResolver == null)
      {
        this.combinedUniversalTokenResolver = (SecurityTokenResolver) this.universalTokenResolver;
        this.combinedPrimaryTokenResolver = (SecurityTokenResolver) this.primaryTokenResolver;
      }
      else
      {
#if FEATURE_CORECLR
	    throw new NotImplementedException("AggregateSecurityHeaderTokenResolver is not supported in .NET Core");
#else
        this.combinedUniversalTokenResolver = (SecurityTokenResolver) new AggregateSecurityHeaderTokenResolver(this.universalTokenResolver, this.outOfBandTokenResolver);
        this.combinedPrimaryTokenResolver = (SecurityTokenResolver) new AggregateSecurityHeaderTokenResolver(this.primaryTokenResolver, this.outOfBandTokenResolver);
#endif
      }
      this.allowedAuthenticators = new List<SecurityTokenAuthenticator>();
      if (this.primaryTokenAuthenticator != null)
        this.allowedAuthenticators.Add(this.primaryTokenAuthenticator);
      if (this.DerivedTokenAuthenticator != null)
        this.allowedAuthenticators.Add(this.DerivedTokenAuthenticator);
      this.pendingSupportingTokenAuthenticator = (SecurityTokenAuthenticator) null;
      int num = 0;
      if (this.supportingTokenAuthenticators != null && this.supportingTokenAuthenticators.Count > 0)
      {
        this.supportingTokenTrackers = new List<TokenTracker>(this.supportingTokenAuthenticators.Count);
        for (int index = 0; index < this.supportingTokenAuthenticators.Count; ++index)
        {
          SupportingTokenAuthenticatorSpecification tokenAuthenticator = this.supportingTokenAuthenticators[index];
          switch (tokenAuthenticator.SecurityTokenAttachmentMode)
          {
            case SecurityTokenAttachmentMode.Signed:
              this.hasAtLeastOneSupportingTokenExpectedToBeSigned = true;
              break;
            case SecurityTokenAttachmentMode.Endorsing:
              this.hasEndorsingOrSignedEndorsingSupportingTokens = true;
              break;
            case SecurityTokenAttachmentMode.SignedEndorsing:
              this.hasEndorsingOrSignedEndorsingSupportingTokens = true;
              this.hasAtLeastOneSupportingTokenExpectedToBeSigned = true;
              break;
            case SecurityTokenAttachmentMode.SignedEncrypted:
              this.hasAtLeastOneSupportingTokenExpectedToBeSigned = true;
              break;
          }
          if (this.primaryTokenAuthenticator != null && this.primaryTokenAuthenticator.GetType().Equals(tokenAuthenticator.TokenAuthenticator.GetType()))
            this.pendingSupportingTokenAuthenticator = tokenAuthenticator.TokenAuthenticator;
          else
            this.allowedAuthenticators.Add(tokenAuthenticator.TokenAuthenticator);
#if FEATURE_CORECLR
          // TokenParameters.HasAsymmetricKey is not supported
#else
          if (tokenAuthenticator.TokenParameters.RequireDerivedKeys && !tokenAuthenticator.TokenParameters.HasAsymmetricKey && (tokenAuthenticator.SecurityTokenAttachmentMode == SecurityTokenAttachmentMode.Endorsing || tokenAuthenticator.SecurityTokenAttachmentMode == SecurityTokenAttachmentMode.SignedEndorsing))
            ++num;
#endif
          this.supportingTokenTrackers.Add(new TokenTracker(tokenAuthenticator));
        }
      }
      if (this.DerivedTokenAuthenticator != null)
      {
        this.maxDerivedKeyLength = (this.AlgorithmSuite.DefaultEncryptionKeyDerivationLength >= this.AlgorithmSuite.DefaultSignatureKeyDerivationLength ? this.AlgorithmSuite.DefaultEncryptionKeyDerivationLength : this.AlgorithmSuite.DefaultSignatureKeyDerivationLength) / 8;
        this.maxDerivedKeys = (2 + num) * 2;
      }
#if FEATURE_CORECLR
      // SecurityHeaderElementInferenceEngine is not supported
#else
      SecurityHeaderElementInferenceEngine.GetInferenceEngine(this.Layout).ExecuteProcessingPasses(this, securityHeaderReader);
#endif
      if (this.RequireMessageProtection)
      {
        this.ElementManager.EnsureAllRequiredSecurityHeaderTargetsWereProtected();
        this.ExecuteMessageProtectionPass(this.hasAtLeastOneSupportingTokenExpectedToBeSigned);
        if (this.RequiredSignatureParts != null && this.SignatureToken == null)
          throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("RequiredSignatureMissing")), this.Message);
      }
      this.EnsureDecryptionComplete();
      this.signatureTracker.SetDerivationSourceIfRequired();
      this.encryptionTracker.SetDerivationSourceIfRequired();
      if (this.EncryptionToken != null)
      {
        if (this.wrappingToken != null)
        {
          if (!(this.EncryptionToken is WrappedKeySecurityToken) || ((WrappedKeySecurityToken) this.EncryptionToken).WrappingToken != this.wrappingToken)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("EncryptedKeyWasNotEncryptedWithTheRequiredEncryptingToken", new object[1]{ (object) this.wrappingToken })));
        }
        else if (this.expectedEncryptionToken != null)
        {
          if (this.EncryptionToken != this.expectedEncryptionToken)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("MessageWasNotEncryptedWithTheRequiredEncryptingToken")));
        }
        else if (this.SignatureToken != null && this.EncryptionToken != this.SignatureToken)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("SignatureAndEncryptionTokenMismatch", (object) this.SignatureToken, (object) this.EncryptionToken)));
      }
      if (this.EnforceDerivedKeyRequirement)
      {
#if FEATURE_CORECLR
          // TokenParameters.HasAsymmetricKey not supported
#else
        if (this.SignatureToken != null)
        {
          if (this.primaryTokenParameters != null)
          {
            if (this.primaryTokenParameters.RequireDerivedKeys && !this.primaryTokenParameters.HasAsymmetricKey && !this.primaryTokenTracker.IsDerivedFrom)
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("PrimarySignatureWasNotSignedByDerivedKey", new object[1]{ (object) this.primaryTokenParameters })));
          }
          else if (this.wrappingTokenParameters != null && this.wrappingTokenParameters.RequireDerivedKeys && !this.signatureTracker.IsDerivedToken)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("PrimarySignatureWasNotSignedByDerivedWrappedKey", new object[1]{ (object) this.wrappingTokenParameters })));
        }
        if (this.EncryptionToken != null)
        {
          if (this.wrappingTokenParameters != null)
          {
            if (this.wrappingTokenParameters.RequireDerivedKeys && !this.encryptionTracker.IsDerivedToken)
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("MessageWasNotEncryptedByDerivedWrappedKey", new object[1]{ (object) this.wrappingTokenParameters })));
          }
          else if (this.expectedEncryptionTokenParameters != null)
          {
            if (this.expectedEncryptionTokenParameters.RequireDerivedKeys && !this.encryptionTracker.IsDerivedToken)
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("MessageWasNotEncryptedByDerivedEncryptionToken", new object[1]{ (object) this.expectedEncryptionTokenParameters })));
          }
          else if (this.primaryTokenParameters != null && !this.primaryTokenParameters.HasAsymmetricKey && (this.primaryTokenParameters.RequireDerivedKeys && !this.encryptionTracker.IsDerivedToken))
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("MessageWasNotEncryptedByDerivedEncryptionToken", new object[1]{ (object) this.primaryTokenParameters })));
        }
#endif
      }
      if (flag && this.BasicSupportingTokens != null && this.BasicSupportingTokens.Count > 0)
        this.VerifySignatureEncryption();
      if (this.supportingTokenTrackers != null)
      {
        for (int index = 0; index < this.supportingTokenTrackers.Count; ++index)
          this.VerifySupportingToken(this.supportingTokenTrackers[index]);
      }
      if (this.replayDetectionEnabled)
      {
        if (this.timestamp == null)
          throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("NoTimestampAvailableInSecurityHeaderToDoReplayDetection")), this.Message);
        if (this.primarySignatureValue == null)
          throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("NoSignatureAvailableInSecurityHeaderToDoReplayDetection")), this.Message);
        ReceiveSecurityHeader.AddNonce(this.nonceCache, this.primarySignatureValue);
        this.timestamp.ValidateFreshness(this.replayWindow, this.clockSkew);
      }
      if (this.ExpectSignatureConfirmation)
        this.ElementManager.VerifySignatureConfirmationWasFound();
      this.MarkHeaderAsUnderstood();
    }

    private static void AddNonce(NonceCache cache, byte[] nonce)
    {
      if (!cache.TryAddNonce(nonce))
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("InvalidOrReplayedNonce"), true));
    }

    private static void CheckNonce(NonceCache cache, byte[] nonce)
    {
      if (cache.CheckNonce(nonce))
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("InvalidOrReplayedNonce"), true));
    }

    protected abstract void EnsureDecryptionComplete();

    protected abstract void ExecuteMessageProtectionPass(bool hasAtLeastOneSupportingTokenExpectedToBeSigned);

    internal void ExecuteSignatureEncryptionProcessingPass()
    {
      for (int index = 0; index < this.elementManager.Count; ++index)
      {
        ReceiveSecurityHeaderEntry element1;
        this.elementManager.GetElementEntry(index, out element1);
        switch (element1.elementCategory)
        {
          case ReceiveSecurityHeaderElementCategory.Signature:
            if (element1.bindingMode == ReceiveSecurityHeaderBindingModes.Primary)
            {
              this.ProcessPrimarySignature((SignedXml) element1.element, element1.encrypted);
              break;
            }
            this.ProcessSupportingSignature((SignedXml) element1.element, element1.encrypted);
            break;
          case ReceiveSecurityHeaderElementCategory.ReferenceList:
            this.ProcessReferenceList((ReferenceList) element1.element);
            break;
          case ReceiveSecurityHeaderElementCategory.Token:
            WrappedKeySecurityToken element2 = element1.element as WrappedKeySecurityToken;
            if (element2 != null && element2.ReferenceList != null)
            {
              this.ProcessReferenceList(element2.ReferenceList, element2);
              break;
            }
            break;
        }
      }
    }

    internal void ExecuteSubheaderDecryptionPass()
    {
      for (int index = 0; index < this.elementManager.Count; ++index)
      {
        if (this.elementManager.GetElementCategory(index) == ReceiveSecurityHeaderElementCategory.EncryptedData)
        {
          EncryptedData element = this.elementManager.GetElement<EncryptedData>(index);
          bool primarySignatureFound = false;
          this.ProcessEncryptedData(element, this.timeoutHelper.RemainingTime(), index, false, ref primarySignatureFound);
        }
      }
    }

    internal void ExecuteReadingPass(XmlDictionaryReader reader)
    {
      int num = 0;
      while (reader.IsStartElement())
      {
        if (this.IsReaderAtSignature(reader))
          this.ReadSignature(reader, -1, (byte[]) null);
        else if (this.IsReaderAtReferenceList(reader))
          this.ReadReferenceList(reader);
        else if (this.StandardsManager.WSUtilitySpecificationVersion.IsReaderAtTimestamp(reader))
          this.ReadTimestamp(reader);
        else if (this.IsReaderAtEncryptedKey(reader))
          this.ReadEncryptedKey(reader, false);
        else if (this.IsReaderAtEncryptedData(reader))
          this.ReadEncryptedData(reader);
        else if (this.StandardsManager.SecurityVersion.IsReaderAtSignatureConfirmation(reader))
          this.ReadSignatureConfirmation(reader, -1, (byte[]) null);
        else if (this.IsReaderAtSecurityTokenReference(reader))
          this.ReadSecurityTokenReference(reader);
        else
          this.ReadToken(reader, -1, (byte[]) null, (SecurityToken) null, (string) null, this.timeoutHelper.RemainingTime());
        ++num;
      }
      reader.ReadEndElement();
      reader.Close();
    }

    internal void ExecuteFullPass(XmlDictionaryReader reader)
    {
      bool primarySignatureFound = !this.RequireMessageProtection;
      int num = 0;
      while (reader.IsStartElement())
      {
        if (this.IsReaderAtSignature(reader))
        {
          SignedXml signedXml = this.ReadSignature(reader, -1, (byte[]) null);
          if (primarySignatureFound)
          {
            this.elementManager.SetBindingMode(num, ReceiveSecurityHeaderBindingModes.Endorsing);
            this.ProcessSupportingSignature(signedXml, false);
          }
          else
          {
            primarySignatureFound = true;
            this.elementManager.SetBindingMode(num, ReceiveSecurityHeaderBindingModes.Primary);
            this.ProcessPrimarySignature(signedXml, false);
          }
        }
        else if (this.IsReaderAtReferenceList(reader))
          this.ProcessReferenceList(this.ReadReferenceList(reader));
        else if (this.StandardsManager.WSUtilitySpecificationVersion.IsReaderAtTimestamp(reader))
          this.ReadTimestamp(reader);
        else if (this.IsReaderAtEncryptedKey(reader))
          this.ReadEncryptedKey(reader, true);
        else if (this.IsReaderAtEncryptedData(reader))
          this.ProcessEncryptedData(this.ReadEncryptedData(reader), this.timeoutHelper.RemainingTime(), num, true, ref primarySignatureFound);
        else if (this.StandardsManager.SecurityVersion.IsReaderAtSignatureConfirmation(reader))
          this.ReadSignatureConfirmation(reader, -1, (byte[]) null);
        else if (this.IsReaderAtSecurityTokenReference(reader))
          this.ReadSecurityTokenReference(reader);
        else
          this.ReadToken(reader, -1, (byte[]) null, (SecurityToken) null, (string) null, this.timeoutHelper.RemainingTime());
        ++num;
      }
      reader.ReadEndElement();
      reader.Close();
    }

    internal void EnsureDerivedKeyLimitNotReached()
    {
      this.numDerivedKeys = this.numDerivedKeys + 1;
      if (this.numDerivedKeys > this.maxDerivedKeys)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("DerivedKeyLimitExceeded", new object[1]{ (object) this.maxDerivedKeys })));
    }

    internal void ExecuteDerivedKeyTokenStubPass(bool isFinalPass)
    {
#if FEATURE_CORECLR
	  throw new NotImplementedException("DerivedKeySecurityTokenStub is not supported in .NET Core");
#else
      for (int index = 0; index < this.elementManager.Count; ++index)
      {
        if (this.elementManager.GetElementCategory(index) == ReceiveSecurityHeaderElementCategory.Token)
        {
          DerivedKeySecurityTokenStub element = this.elementManager.GetElement(index) as DerivedKeySecurityTokenStub;
          if (element != null)
          {
            SecurityToken token1 = (SecurityToken) null;
            this.universalTokenResolver.TryResolveToken(element.TokenToDeriveIdentifier, out token1);
            if (token1 != null)
            {
              this.EnsureDerivedKeyLimitNotReached();
              DerivedKeySecurityToken token2 = element.CreateToken(token1, this.maxDerivedKeyLength);
              this.elementManager.SetElement(index, (object) token2);
              this.AddDerivedKeyTokenToResolvers((SecurityToken) token2);
            }
            else if (isFinalPass)
              throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnableToResolveKeyInfoClauseInDerivedKeyToken", new object[1]{ (object) element.TokenToDeriveIdentifier })), this.Message);
          }
        }
      }
#endif
    }

    private SecurityToken GetRootToken(SecurityToken token)
    {
      if (token is DerivedKeySecurityToken)
        return ((DerivedKeySecurityToken) token).TokenToDerive;
      return token;
    }

    private void RecordEncryptionTokenAndRemoveReferenceListEntry(string id, SecurityToken encryptionToken)
    {
      if (id == null)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("MissingIdInEncryptedElement")), this.Message);
      this.OnDecryptionOfSecurityHeaderItemRequiringReferenceListEntry(id);
      this.RecordEncryptionToken(encryptionToken);
    }

    private EncryptedData ReadEncryptedData(XmlDictionaryReader reader)
    {
      EncryptedData encryptedData = this.ReadSecurityHeaderEncryptedItem(reader, this.MessageDirection == MessageDirection.Output);
      this.elementManager.AppendEncryptedData(encryptedData);
      return encryptedData;
    }

    internal XmlDictionaryReader CreateDecryptedReader(byte[] decryptedBuffer)
    {
#if FEATURE_CORECLR
        throw new NotImplementedException("ContextImportHelper is not supported in .NET Core");
#else
      return ContextImportHelper.CreateSplicedReader(decryptedBuffer, this.SecurityVerifiedMessage.GetEnvelopeAttributes(), this.SecurityVerifiedMessage.GetHeaderAttributes(), this.securityElementAttributes, this.ReaderQuotas);
#endif
    }

    private void ProcessEncryptedData(EncryptedData encryptedData, TimeSpan timeout, int position, bool eagerMode, ref bool primarySignatureFound)
    {
#if !FEATURE_CORECLR
	  // Skip tracing in .NET Core
      if (TD.EncryptedDataProcessingStartIsEnabled())
        TD.EncryptedDataProcessingStart(this.EventTraceActivity);
#endif
      string id = encryptedData.Id;
      SecurityToken encryptionToken1;
      byte[] decryptedBuffer1 = this.DecryptSecurityHeaderElement(encryptedData, this.wrappedKeyToken, out encryptionToken1);
      XmlDictionaryReader decryptedReader = this.CreateDecryptedReader(decryptedBuffer1);
      if (this.IsReaderAtSignature(decryptedReader))
      {
        this.RecordEncryptionTokenAndRemoveReferenceListEntry(id, encryptionToken1);
        SignedXml signedXml = this.ReadSignature(decryptedReader, position, decryptedBuffer1);
        if (eagerMode)
        {
          if (primarySignatureFound)
          {
            this.elementManager.SetBindingMode(position, ReceiveSecurityHeaderBindingModes.Endorsing);
            this.ProcessSupportingSignature(signedXml, true);
          }
          else
          {
            primarySignatureFound = true;
            this.elementManager.SetBindingMode(position, ReceiveSecurityHeaderBindingModes.Primary);
            this.ProcessPrimarySignature(signedXml, true);
          }
        }
      }
      else if (this.StandardsManager.SecurityVersion.IsReaderAtSignatureConfirmation(decryptedReader))
      {
        this.RecordEncryptionTokenAndRemoveReferenceListEntry(id, encryptionToken1);
        this.ReadSignatureConfirmation(decryptedReader, position, decryptedBuffer1);
      }
      else if (this.IsReaderAtEncryptedData(decryptedReader))
      {
        EncryptedData encryptedData1 = this.ReadSecurityHeaderEncryptedItem(decryptedReader, false);
        SecurityToken encryptionToken2;
        byte[] decryptedBuffer2 = this.DecryptSecurityHeaderElement(encryptedData1, this.wrappedKeyToken, out encryptionToken2);
        this.ReadToken(this.CreateDecryptedReader(decryptedBuffer2), position, decryptedBuffer2, encryptionToken1, id, timeout);
        ReceiveSecurityHeaderEntry element;
        this.ElementManager.GetElementEntry(position, out element);
        if (this.EncryptBeforeSignMode)
        {
          element.encryptedFormId = encryptedData.Id;
          element.encryptedFormWsuId = encryptedData.WsuId;
        }
        else
        {
          element.encryptedFormId = encryptedData1.Id;
          element.encryptedFormWsuId = encryptedData1.WsuId;
        }
        element.decryptedBuffer = decryptedBuffer1;
        element.doubleEncrypted = true;
        this.ElementManager.ReplaceHeaderEntry(position, element);
      }
      else
        this.ReadToken(decryptedReader, position, decryptedBuffer1, encryptionToken1, id, timeout);
#if !FEATURE_CORECLR
	  // Skip tracing in .NET Core
      if (!TD.EncryptedDataProcessingSuccessIsEnabled())
        return;
      TD.EncryptedDataProcessingSuccess(this.EventTraceActivity);
#endif
    }

    private void ReadEncryptedKey(XmlDictionaryReader reader, bool processReferenceListIfPresent)
    {
      this.orderTracker.OnEncryptedKey();
      WrappedKeySecurityToken wrappedKeyToken = this.DecryptWrappedKey(reader);
      if (wrappedKeyToken.WrappingToken != this.wrappingToken)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("EncryptedKeyWasNotEncryptedWithTheRequiredEncryptingToken", new object[1]{ (object) this.wrappingToken })));
      this.universalTokenResolver.Add((SecurityToken) wrappedKeyToken);
      this.primaryTokenResolver.Add((SecurityToken) wrappedKeyToken);
      if (wrappedKeyToken.ReferenceList != null)
      {
        if (!this.EncryptedKeyContainsReferenceList)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("EncryptedKeyWithReferenceListNotAllowed")));
        if (!this.ExpectEncryption)
          throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("EncryptionNotExpected")), this.Message);
        if (processReferenceListIfPresent)
          this.ProcessReferenceList(wrappedKeyToken.ReferenceList, wrappedKeyToken);
        this.wrappedKeyToken = wrappedKeyToken;
      }
      this.elementManager.AppendToken((SecurityToken) wrappedKeyToken, ReceiveSecurityHeaderBindingModes.Primary, (TokenTracker) null);
    }

    private ReferenceList ReadReferenceList(XmlDictionaryReader reader)
    {
      if (!this.ExpectEncryption)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("EncryptionNotExpected")), this.Message);
      ReferenceList referenceList = this.ReadReferenceListCore(reader);
      this.elementManager.AppendReferenceList(referenceList);
      return referenceList;
    }

    protected abstract ReferenceList ReadReferenceListCore(XmlDictionaryReader reader);

    private void ProcessReferenceList(ReferenceList referenceList)
    {
      this.ProcessReferenceList(referenceList, (WrappedKeySecurityToken) null);
    }

    private void ProcessReferenceList(ReferenceList referenceList, WrappedKeySecurityToken wrappedKeyToken)
    {
      this.orderTracker.OnProcessReferenceList();
      this.ProcessReferenceListCore(referenceList, wrappedKeyToken);
    }

    protected abstract void ProcessReferenceListCore(ReferenceList referenceList, WrappedKeySecurityToken wrappedKeyToken);

    private SignedXml ReadSignature(XmlDictionaryReader reader, int position, byte[] decryptedBuffer)
    {
      if (!this.ExpectSignature)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("SignatureNotExpected")), this.Message);
      SignedXml signedXml = this.ReadSignatureCore(reader);
      signedXml.Signature.SignedInfo.ReaderProvider = (ISignatureReaderProvider) this.ElementManager;
      int num;
      if (decryptedBuffer == null)
      {
        this.elementManager.AppendSignature(signedXml);
        num = this.elementManager.Count - 1;
      }
      else
      {
        this.elementManager.SetSignatureAfterDecryption(position, signedXml, decryptedBuffer);
        num = position;
      }
      signedXml.Signature.SignedInfo.SignatureReaderProviderCallbackContext = (object) num;
      return signedXml;
    }

    protected abstract void ReadSecurityTokenReference(XmlDictionaryReader reader);

    private void ProcessPrimarySignature(SignedXml signedXml, bool isFromDecryptedSource)
    {
      this.orderTracker.OnProcessSignature(isFromDecryptedSource);
      this.primarySignatureValue = signedXml.GetSignatureValue();
      if (this.replayDetectionEnabled)
        ReceiveSecurityHeader.CheckNonce(this.nonceCache, this.primarySignatureValue);
      SecurityToken token = this.VerifySignature(signedXml, true, this.primaryTokenResolver, (object) null, (string) null);
      SecurityToken rootToken = this.GetRootToken(token);
      bool flag = token is DerivedKeySecurityToken;
      if (this.primaryTokenTracker != null)
      {
        this.primaryTokenTracker.RecordToken(rootToken);
        this.primaryTokenTracker.IsDerivedFrom = flag;
      }
      this.AddIncomingSignatureValue(signedXml.GetSignatureValue(), isFromDecryptedSource);
    }

    private void ReadSignatureConfirmation(XmlDictionaryReader reader, int position, byte[] decryptedBuffer)
    {
      if (!this.ExpectSignatureConfirmation)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("SignatureConfirmationsNotExpected")), this.Message);
      if (this.orderTracker.PrimarySignatureDone)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("SignatureConfirmationsOccursAfterPrimarySignature")), this.Message);
#if FEATURE_CORECLR
      throw new NotImplementedException("SecurityVersion.ReadSignatureConfirmation is inaccessible in .NET Core");
#else
      ISignatureValueSecurityElement signatureConfirmationElement = this.StandardsManager.SecurityVersion.ReadSignatureConfirmation(reader);
      if (decryptedBuffer == null)
      {
        this.AddIncomingSignatureConfirmation(signatureConfirmationElement.GetSignatureValue(), false);
        this.elementManager.AppendSignatureConfirmation(signatureConfirmationElement);
      }
      else
      {
        this.AddIncomingSignatureConfirmation(signatureConfirmationElement.GetSignatureValue(), true);
        this.elementManager.SetSignatureConfirmationAfterDecryption(position, signatureConfirmationElement, decryptedBuffer);
      }
#endif
    }

    internal TokenTracker GetSupportingTokenTracker(SecurityToken token)
    {
      if (this.supportingTokenTrackers == null)
        return (TokenTracker) null;
      for (int index = 0; index < this.supportingTokenTrackers.Count; ++index)
      {
        if (this.supportingTokenTrackers[index].token == token)
          return this.supportingTokenTrackers[index];
      }
      return (TokenTracker) null;
    }

    internal TokenTracker GetSupportingTokenTracker(SecurityTokenAuthenticator tokenAuthenticator, out SupportingTokenAuthenticatorSpecification spec)
    {
      spec = (SupportingTokenAuthenticatorSpecification) null;
      if (this.supportingTokenAuthenticators == null)
        return (TokenTracker) null;
      for (int index = 0; index < this.supportingTokenAuthenticators.Count; ++index)
      {
        if (this.supportingTokenAuthenticators[index].TokenAuthenticator == tokenAuthenticator)
        {
          spec = this.supportingTokenAuthenticators[index];
          return this.supportingTokenTrackers[index];
        }
      }
      return (TokenTracker) null;
    }

    protected TAuthenticator FindAllowedAuthenticator<TAuthenticator>(bool removeIfPresent) where TAuthenticator : SecurityTokenAuthenticator
    {
      if (this.allowedAuthenticators == null)
        return default (TAuthenticator);
      for (int index = 0; index < this.allowedAuthenticators.Count; ++index)
      {
        if (this.allowedAuthenticators[index] is TAuthenticator)
        {
          TAuthenticator allowedAuthenticator = (TAuthenticator) this.allowedAuthenticators[index];
          if (removeIfPresent)
            this.allowedAuthenticators.RemoveAt(index);
          return allowedAuthenticator;
        }
      }
      return default (TAuthenticator);
    }

    private void ProcessSupportingSignature(SignedXml signedXml, bool isFromDecryptedSource)
    {
      if (!this.ExpectEndorsingTokens)
        throw TraceUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SupportingTokenSignaturesNotExpected")), this.Message);
      XmlDictionaryReader reader;
      string id;
      object signatureTarget;
      if (!this.RequireMessageProtection)
      {
        if (this.timestamp == null)
          throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("SigningWithoutPrimarySignatureRequiresTimestamp")), this.Message);
        reader = (XmlDictionaryReader) null;
        id = this.timestamp.Id;
        signatureTarget = (object) null;
      }
      else
      {
        this.elementManager.GetPrimarySignature(out reader, out id);
        if (reader == null)
          throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("NoPrimarySignatureAvailableForSupportingTokenSignatureVerification")), this.Message);
        signatureTarget = (object) reader;
      }
      SecurityToken token = this.VerifySignature(signedXml, false, this.universalTokenResolver, signatureTarget, id);
      if (reader != null)
        reader.Close();
      if (token == null)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("SignatureVerificationFailed")), this.Message);
      TokenTracker supportingTokenTracker = this.GetSupportingTokenTracker(this.GetRootToken(token));
      if (supportingTokenTracker == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("UnknownSupportingToken", new object[1]{ (object) token })));
      if (supportingTokenTracker.AlreadyReadEndorsingSignature)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("MoreThanOneSupportingSignature", new object[1]{ (object) token })));
      supportingTokenTracker.IsEndorsing = true;
      supportingTokenTracker.AlreadyReadEndorsingSignature = true;
      supportingTokenTracker.IsDerivedFrom = token is DerivedKeySecurityToken;
      this.AddIncomingSignatureValue(signedXml.GetSignatureValue(), isFromDecryptedSource);
    }

    private void ReadTimestamp(XmlDictionaryReader reader)
    {
      if (this.timestamp != null)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("DuplicateTimestampInSecurityHeader")), this.Message);
      bool flag = this.RequireMessageProtection || this.hasEndorsingOrSignedEndorsingSupportingTokens;
      string digestAlgorithm = flag ? this.AlgorithmSuite.DefaultDigestAlgorithm : (string) null;
      SignatureResourcePool resourcePool = flag ? this.ResourcePool : (SignatureResourcePool) null;
      this.timestamp = this.StandardsManager.WSUtilitySpecificationVersion.ReadTimestamp(reader, digestAlgorithm, resourcePool);
      this.timestamp.ValidateRangeAndFreshness(this.replayWindow, this.clockSkew);
      this.elementManager.AppendTimestamp(this.timestamp);
    }

    private bool IsPrimaryToken(SecurityToken token)
    {
      bool flag = token == this.outOfBandPrimaryToken || this.primaryTokenTracker != null && token == this.primaryTokenTracker.token || token == this.expectedEncryptionToken || token is WrappedKeySecurityToken && ((WrappedKeySecurityToken) token).WrappingToken == this.wrappingToken;
      if (!flag && this.outOfBandPrimaryTokenCollection != null)
      {
        for (int index = 0; index < this.outOfBandPrimaryTokenCollection.Count; ++index)
        {
          if (this.outOfBandPrimaryTokenCollection[index] == token)
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    private void ReadToken(XmlDictionaryReader reader, int position, byte[] decryptedBuffer, SecurityToken encryptionToken, string idInEncryptedForm, TimeSpan timeout)
    {
#if FEATURE_CORECLR
	  throw new NotImplementedException("SspiNegotiationTokenAuthenticator is not supported in .NET Core");
#else
      string localName = reader.LocalName;
      string namespaceUri = reader.NamespaceURI;
      string attribute = reader.GetAttribute(System.ServiceModel.XD.SecurityJan2004Dictionary.ValueType, (XmlDictionaryString) null);
      SecurityTokenAuthenticator usedTokenAuthenticator;
      SecurityToken token = this.ReadToken((XmlReader) reader, this.CombinedUniversalTokenResolver, (IList<SecurityTokenAuthenticator>) this.allowedAuthenticators, out usedTokenAuthenticator);
      if (token == null)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("TokenManagerCouldNotReadToken", (object) localName, (object) namespaceUri, (object) attribute)), this.Message);
      DerivedKeySecurityToken keySecurityToken = token as DerivedKeySecurityToken;
      if (keySecurityToken != null)
      {
        this.EnsureDerivedKeyLimitNotReached();
        keySecurityToken.InitializeDerivedKey(this.maxDerivedKeyLength);
      }
      if (usedTokenAuthenticator is SspiNegotiationTokenAuthenticator || usedTokenAuthenticator == this.primaryTokenAuthenticator)
        this.allowedAuthenticators.Remove(usedTokenAuthenticator);
      TokenTracker supportingTokenTracker = (TokenTracker) null;
      ReceiveSecurityHeaderBindingModes mode;
      if (usedTokenAuthenticator == this.primaryTokenAuthenticator)
      {
        this.universalTokenResolver.Add(token, SecurityTokenReferenceStyle.Internal, this.primaryTokenParameters);
        this.primaryTokenResolver.Add(token, SecurityTokenReferenceStyle.Internal, this.primaryTokenParameters);
        if (this.pendingSupportingTokenAuthenticator != null)
        {
          this.allowedAuthenticators.Add(this.pendingSupportingTokenAuthenticator);
          this.pendingSupportingTokenAuthenticator = (SecurityTokenAuthenticator) null;
        }
        this.primaryTokenTracker.RecordToken(token);
        mode = ReceiveSecurityHeaderBindingModes.Primary;
      }
      else if (usedTokenAuthenticator == this.DerivedTokenAuthenticator)
      {
        if (token is DerivedKeySecurityTokenStub)
        {
          if (this.Layout == SecurityHeaderLayout.Strict)
            throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnableToResolveKeyInfoClauseInDerivedKeyToken", new object[1]{ (object) ((DerivedKeySecurityTokenStub) token).TokenToDeriveIdentifier })), this.Message);
        }
        else
          this.AddDerivedKeyTokenToResolvers(token);
        mode = ReceiveSecurityHeaderBindingModes.Unknown;
      }
      else
      {
        SupportingTokenAuthenticatorSpecification spec;
        supportingTokenTracker = this.GetSupportingTokenTracker(usedTokenAuthenticator, out spec);
        if (supportingTokenTracker == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("UnknownTokenAuthenticatorUsedInTokenProcessing", new object[1]{ (object) usedTokenAuthenticator })));
        if (supportingTokenTracker.token != null)
        {
          supportingTokenTracker = new TokenTracker(spec);
          this.supportingTokenTrackers.Add(supportingTokenTracker);
        }
        supportingTokenTracker.RecordToken(token);
        if (encryptionToken != null)
          supportingTokenTracker.IsEncrypted = true;
        bool isBasic;
        bool isSignedButNotBasic;
        SecurityTokenAttachmentModeHelper.Categorize(spec.SecurityTokenAttachmentMode, out isBasic, out isSignedButNotBasic, out mode);
        if (isBasic)
        {
          if (!this.ExpectBasicTokens)
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("BasicTokenNotExpected")));
          if (this.RequireMessageProtection && encryptionToken != null)
            this.RecordEncryptionTokenAndRemoveReferenceListEntry(idInEncryptedForm, encryptionToken);
        }
        if (isSignedButNotBasic && !this.ExpectSignedTokens)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new MessageSecurityException(SR.GetString("SignedSupportingTokenNotExpected")));
        this.universalTokenResolver.Add(token, SecurityTokenReferenceStyle.Internal, spec.TokenParameters);
      }
      if (position == -1)
        this.elementManager.AppendToken(token, mode, supportingTokenTracker);
      else
        this.elementManager.SetTokenAfterDecryption(position, token, mode, decryptedBuffer, supportingTokenTracker);
#endif
    }

    private SecurityToken ReadToken(XmlReader reader, SecurityTokenResolver tokenResolver, IList<SecurityTokenAuthenticator> allowedTokenAuthenticators, out SecurityTokenAuthenticator usedTokenAuthenticator)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("ServiceCredentialsSecurityTokenManager and DerivedKeySecurityTokenStub are not supported in .NET Core");
#else
      SecurityToken securityToken = this.StandardsManager.SecurityTokenSerializer.ReadToken(reader, tokenResolver);
      if (securityToken is DerivedKeySecurityTokenStub)
      {
        if (this.DerivedTokenAuthenticator == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnableToFindTokenAuthenticator", new object[1]{ (object) typeof (DerivedKeySecurityToken) })));
        usedTokenAuthenticator = this.DerivedTokenAuthenticator;
        return securityToken;
      }
      for (int index = 0; index < allowedTokenAuthenticators.Count; ++index)
      {
        SecurityTokenAuthenticator tokenAuthenticator = allowedTokenAuthenticators[index];
        if (tokenAuthenticator.CanValidateToken(securityToken))
        {

          ServiceCredentialsSecurityTokenManager.KerberosSecurityTokenAuthenticatorWrapper authenticatorWrapper = tokenAuthenticator as ServiceCredentialsSecurityTokenManager.KerberosSecurityTokenAuthenticatorWrapper;
          ReadOnlyCollection<IAuthorizationPolicy> readOnlyCollection = authenticatorWrapper == null ? tokenAuthenticator.ValidateToken(securityToken) : authenticatorWrapper.ValidateToken(securityToken, this.channelBinding, this.extendedProtectionPolicy);
          this.SecurityTokenAuthorizationPoliciesMapping.Add(securityToken, readOnlyCollection);
          usedTokenAuthenticator = tokenAuthenticator;
          return securityToken;

        }
      }
      throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("UnableToFindTokenAuthenticator", new object[1]{ (object) securityToken.GetType() })));
#endif
    }

    private void AddDerivedKeyTokenToResolvers(SecurityToken token)
    {
      this.universalTokenResolver.Add(token);
      if (!this.IsPrimaryToken(this.GetRootToken(token)))
        return;
      this.primaryTokenResolver.Add(token);
    }

    private void AddIncomingSignatureConfirmation(byte[] signatureValue, bool isFromDecryptedSource)
    {
      if (!this.MaintainSignatureConfirmationState)
        return;
      if (this.receivedSignatureConfirmations == null)
        this.receivedSignatureConfirmations = new SignatureConfirmations();
      this.receivedSignatureConfirmations.AddConfirmation(signatureValue, isFromDecryptedSource);
    }

    private void AddIncomingSignatureValue(byte[] signatureValue, bool isFromDecryptedSource)
    {
      if (!this.MaintainSignatureConfirmationState || this.ExpectSignatureConfirmation)
        return;
      if (this.receivedSignatureValues == null)
        this.receivedSignatureValues = new SignatureConfirmations();
      this.receivedSignatureValues.AddConfirmation(signatureValue, isFromDecryptedSource);
    }

    protected void RecordEncryptionToken(SecurityToken token)
    {
      this.encryptionTracker.RecordToken(token);
    }

    protected void RecordSignatureToken(SecurityToken token)
    {
      this.signatureTracker.RecordToken(token);
    }

    public void SetRequiredProtectionOrder(MessageProtectionOrder protectionOrder)
    {
      this.ThrowIfProcessingStarted();
      this.protectionOrder = protectionOrder;
    }

    internal abstract SignedXml ReadSignatureCore(XmlDictionaryReader signatureReader);

    internal abstract SecurityToken VerifySignature(SignedXml signedXml, bool isPrimarySignature, SecurityHeaderTokenResolver resolver, object signatureTarget, string id);

    protected abstract bool TryDeleteReferenceListEntry(string id);

    private struct OrderTracker
    {
      private static readonly ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder[] stateTransitionTableOnDecrypt = new ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder[6]{ ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder.Decrypt, ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder.VerifyDecrypt, ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder.Decrypt, ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder.Mixed, ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder.VerifyDecrypt, ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder.Mixed };
      private static readonly ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder[] stateTransitionTableOnVerify = new ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder[6]{ ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder.Verify, ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder.Verify, ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder.DecryptVerify, ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder.DecryptVerify, ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder.Mixed, ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder.Mixed };
      private const int MaxAllowedWrappedKeys = 1;
      private int referenceListCount;
      private ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder state;
      private int signatureCount;
      private int unencryptedSignatureCount;
      private int numWrappedKeys;
      private MessageProtectionOrder protectionOrder;
      private bool enforce;

      public bool AllSignaturesEncrypted
      {
        get
        {
          return this.unencryptedSignatureCount == 0;
        }
      }

      public bool EncryptBeforeSignMode
      {
        get
        {
          if (this.enforce)
            return this.protectionOrder == MessageProtectionOrder.EncryptBeforeSign;
          return false;
        }
      }

      public bool EncryptBeforeSignOrderRequirementMet
      {
        get
        {
          if (this.state != ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder.DecryptVerify)
            return this.state != ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder.Mixed;
          return false;
        }
      }

      public bool PrimarySignatureDone
      {
        get
        {
          return this.signatureCount > 0;
        }
      }

      public bool SignBeforeEncryptOrderRequirementMet
      {
        get
        {
          if (this.state != ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder.VerifyDecrypt)
            return this.state != ReceiveSecurityHeader.OrderTracker.ReceiverProcessingOrder.Mixed;
          return false;
        }
      }

      private void EnforceProtectionOrder()
      {
        switch (this.protectionOrder)
        {
          case MessageProtectionOrder.SignBeforeEncrypt:
            if (this.SignBeforeEncryptOrderRequirementMet)
              break;
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("MessageProtectionOrderMismatch", new object[1]{ (object) this.protectionOrder })));
          case MessageProtectionOrder.SignBeforeEncryptAndEncryptSignature:
            if (!this.AllSignaturesEncrypted)
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("PrimarySignatureIsRequiredToBeEncrypted")));
            goto case MessageProtectionOrder.SignBeforeEncrypt;
          case MessageProtectionOrder.EncryptBeforeSign:
            if (this.EncryptBeforeSignOrderRequirementMet)
              break;
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("MessageProtectionOrderMismatch", new object[1]{ (object) this.protectionOrder })));
        }
      }

      public void OnProcessReferenceList()
      {
        if (this.referenceListCount > 0)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("AtMostOneReferenceListIsSupportedWithDefaultPolicyCheck")));
        this.referenceListCount = this.referenceListCount + 1;
        this.state = ReceiveSecurityHeader.OrderTracker.stateTransitionTableOnDecrypt[(int) this.state];
        if (!this.enforce)
          return;
        this.EnforceProtectionOrder();
      }

      public void OnProcessSignature(bool isEncrypted)
      {
        if (this.signatureCount > 0)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("AtMostOneSignatureIsSupportedWithDefaultPolicyCheck")));
        this.signatureCount = this.signatureCount + 1;
        if (!isEncrypted)
          this.unencryptedSignatureCount = this.unencryptedSignatureCount + 1;
        this.state = ReceiveSecurityHeader.OrderTracker.stateTransitionTableOnVerify[(int) this.state];
        if (!this.enforce)
          return;
        this.EnforceProtectionOrder();
      }

      public void OnEncryptedKey()
      {
        if (this.numWrappedKeys == 1)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("WrappedKeyLimitExceeded", new object[1]{ (object) this.numWrappedKeys })));
        this.numWrappedKeys = this.numWrappedKeys + 1;
      }

      public void SetRequiredProtectionOrder(MessageProtectionOrder protectionOrder)
      {
        this.protectionOrder = protectionOrder;
        this.enforce = true;
      }

      private enum ReceiverProcessingOrder
      {
        None,
        Verify,
        Decrypt,
        DecryptVerify,
        VerifyDecrypt,
        Mixed,
      }
    }

    private struct OperationTracker
    {
      private MessagePartSpecification parts;
      private SecurityToken token;
      private bool isDerivedToken;

      public MessagePartSpecification Parts
      {
        get
        {
          return this.parts;
        }
        set
        {
          this.parts = value;
        }
      }

      public SecurityToken Token
      {
        get
        {
          return this.token;
        }
      }

      public bool IsDerivedToken
      {
        get
        {
          return this.isDerivedToken;
        }
      }

      public void RecordToken(SecurityToken token)
      {
        if (this.token == null)
          this.token = token;
        else if (this.token != token)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("MismatchInSecurityOperationToken")));
      }

      public void SetDerivationSourceIfRequired()
      {
        DerivedKeySecurityToken token = this.token as DerivedKeySecurityToken;
        if (token == null)
          return;
        this.token = token.TokenToDerive;
        this.isDerivedToken = true;
      }
    }
  }
}
