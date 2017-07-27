// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.MessageSecurityProtocolFactory
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Selectors;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
  internal abstract class MessageSecurityProtocolFactory : SecurityProtocolFactory
  {
    private bool applyIntegrity = true;
    private bool applyConfidentiality = true;
    private ChannelProtectionRequirements protectionRequirements = new ChannelProtectionRequirements();
    private bool requireIntegrity = true;
    private bool requireConfidentiality = true;
    internal const MessageProtectionOrder defaultMessageProtectionOrder = MessageProtectionOrder.SignBeforeEncrypt;
    internal const bool defaultDoRequestSignatureConfirmation = false;
    private bool doRequestSignatureConfirmation;
    private IdentityVerifier identityVerifier;
    private MessageProtectionOrder messageProtectionOrder;
    private List<SecurityTokenAuthenticator> wrappedKeyTokenAuthenticator;

    public bool ApplyConfidentiality
    {
      get
      {
        return this.applyConfidentiality;
      }
      set
      {
        this.ThrowIfImmutable();
        this.applyConfidentiality = value;
      }
    }

    public bool ApplyIntegrity
    {
      get
      {
        return this.applyIntegrity;
      }
      set
      {
        this.ThrowIfImmutable();
        this.applyIntegrity = value;
      }
    }

    public bool DoRequestSignatureConfirmation
    {
      get
      {
        return this.doRequestSignatureConfirmation;
      }
      set
      {
        this.ThrowIfImmutable();
        this.doRequestSignatureConfirmation = value;
      }
    }

    public IdentityVerifier IdentityVerifier
    {
      get
      {
        return this.identityVerifier;
      }
      set
      {
        this.ThrowIfImmutable();
        this.identityVerifier = value;
      }
    }

    public ChannelProtectionRequirements ProtectionRequirements
    {
      get
      {
        return this.protectionRequirements;
      }
    }

    public MessageProtectionOrder MessageProtectionOrder
    {
      get
      {
        return this.messageProtectionOrder;
      }
      set
      {
        this.ThrowIfImmutable();
        this.messageProtectionOrder = value;
      }
    }

    public bool RequireIntegrity
    {
      get
      {
        return this.requireIntegrity;
      }
      set
      {
        this.ThrowIfImmutable();
        this.requireIntegrity = value;
      }
    }

    public bool RequireConfidentiality
    {
      get
      {
        return this.requireConfidentiality;
      }
      set
      {
        this.ThrowIfImmutable();
        this.requireConfidentiality = value;
      }
    }

    internal List<SecurityTokenAuthenticator> WrappedKeySecurityTokenAuthenticator
    {
      get
      {
        return this.wrappedKeyTokenAuthenticator;
      }
    }

    protected MessageSecurityProtocolFactory()
    {
    }

    internal MessageSecurityProtocolFactory(MessageSecurityProtocolFactory factory)
      : base((SecurityProtocolFactory) factory)
    {
      if (factory == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("factory");
      this.applyIntegrity = factory.applyIntegrity;
      this.applyConfidentiality = factory.applyConfidentiality;
      this.identityVerifier = factory.identityVerifier;
      this.protectionRequirements = new ChannelProtectionRequirements(factory.protectionRequirements);
      this.messageProtectionOrder = factory.messageProtectionOrder;
      this.requireIntegrity = factory.requireIntegrity;
      this.requireConfidentiality = factory.requireConfidentiality;
      this.doRequestSignatureConfirmation = factory.doRequestSignatureConfirmation;
    }

    protected virtual void ValidateCorrelationSecuritySettings()
    {
      if (!this.ActAsInitiator || !this.SupportsRequestReply || !((!this.ApplyIntegrity && !this.ApplyConfidentiality) & (this.RequireIntegrity || this.RequireConfidentiality)))
        return;
      this.OnPropertySettingsError("ApplyIntegrity", false);
    }

    public override void OnOpen(TimeSpan timeout)
    {
      base.OnOpen(timeout);
      this.protectionRequirements.MakeReadOnly();
      if (this.DetectReplays && !this.RequireIntegrity)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("RequireIntegrity", SR.GetString("ForReplayDetectionToBeDoneRequireIntegrityMustBeSet"));
      if (this.DoRequestSignatureConfirmation)
      {
        if (!this.SupportsRequestReply)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString("SignatureConfirmationRequiresRequestReply"));
        if (!this.StandardsManager.SecurityVersion.SupportsSignatureConfirmation)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString("SecurityVersionDoesNotSupportSignatureConfirmation", new object[1]{ (object) this.StandardsManager.SecurityVersion }));
      }
#if FEATURE_CORECLR
      throw new NotImplementedException("WrappedKeySecurityToken not supported in .NET Core");
#else
      this.wrappedKeyTokenAuthenticator = new List<SecurityTokenAuthenticator>(1);
      this.wrappedKeyTokenAuthenticator.Add((SecurityTokenAuthenticator) new NonValidatingSecurityTokenAuthenticator<WrappedKeySecurityToken>());
      this.ValidateCorrelationSecuritySettings();
#endif
    }

    private static MessagePartSpecification ExtractMessageParts(string action, ScopedMessagePartSpecification scopedParts, bool isForSignature)
    {
      MessagePartSpecification parts = (MessagePartSpecification) null;
      if (scopedParts.TryGetParts(action, out parts) || scopedParts.TryGetParts("*", out parts))
        return parts;
      SecurityVersion securityVersion = MessageSecurityVersion.Default.SecurityVersion;
      MessageFault fault = MessageFault.CreateFault(FaultCode.CreateSenderFaultCode(new FaultCode(securityVersion.InvalidSecurityFaultCode.Value, securityVersion.HeaderNamespace.Value)), new FaultReason(SR.GetString("InvalidOrUnrecognizedAction", new object[1]{ (object) action }), CultureInfo.CurrentCulture));
      if (isForSignature)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("NoSignaturePartsSpecified", new object[1]{ (object) action }), (Exception) null, fault));
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("NoEncryptionPartsSpecified", new object[1]{ (object) action }), (Exception) null, fault));
    }

    internal MessagePartSpecification GetIncomingEncryptionParts(string action)
    {
      if (!this.RequireConfidentiality)
        return MessagePartSpecification.NoParts;
      if (this.IsDuplexReply)
        return MessageSecurityProtocolFactory.ExtractMessageParts(action, this.ProtectionRequirements.OutgoingEncryptionParts, false);
      return MessageSecurityProtocolFactory.ExtractMessageParts(action, this.ActAsInitiator ? this.ProtectionRequirements.OutgoingEncryptionParts : this.ProtectionRequirements.IncomingEncryptionParts, false);
    }

    internal MessagePartSpecification GetIncomingSignatureParts(string action)
    {
      if (!this.RequireIntegrity)
        return MessagePartSpecification.NoParts;
      if (this.IsDuplexReply)
        return MessageSecurityProtocolFactory.ExtractMessageParts(action, this.ProtectionRequirements.OutgoingSignatureParts, true);
      return MessageSecurityProtocolFactory.ExtractMessageParts(action, this.ActAsInitiator ? this.ProtectionRequirements.OutgoingSignatureParts : this.ProtectionRequirements.IncomingSignatureParts, true);
    }

    internal MessagePartSpecification GetOutgoingEncryptionParts(string action)
    {
      if (!this.ApplyConfidentiality)
        return MessagePartSpecification.NoParts;
      if (this.IsDuplexReply)
        return MessageSecurityProtocolFactory.ExtractMessageParts(action, this.ProtectionRequirements.OutgoingEncryptionParts, false);
      return MessageSecurityProtocolFactory.ExtractMessageParts(action, this.ActAsInitiator ? this.ProtectionRequirements.IncomingEncryptionParts : this.ProtectionRequirements.OutgoingEncryptionParts, false);
    }

    internal MessagePartSpecification GetOutgoingSignatureParts(string action)
    {
      if (!this.ApplyIntegrity)
        return MessagePartSpecification.NoParts;
      if (this.IsDuplexReply)
        return MessageSecurityProtocolFactory.ExtractMessageParts(action, this.ProtectionRequirements.OutgoingSignatureParts, true);
      return MessageSecurityProtocolFactory.ExtractMessageParts(action, this.ActAsInitiator ? this.ProtectionRequirements.IncomingSignatureParts : this.ProtectionRequirements.OutgoingSignatureParts, true);
    }
  }
}
