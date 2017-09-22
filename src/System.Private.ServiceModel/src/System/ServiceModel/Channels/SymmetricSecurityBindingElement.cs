// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.SymmetricSecurityBindingElement
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Globalization;
using System.Net.Security;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Text;

namespace System.ServiceModel.Channels
{
  /// <summary>Represents a custom binding element that supports channel security using symmetric encryption.</summary>
  public sealed class SymmetricSecurityBindingElement : SecurityBindingElement, IPolicyExportExtension
  {
    private MessageProtectionOrder messageProtectionOrder;
    private SecurityTokenParameters protectionTokenParameters;
    private bool requireSignatureConfirmation;

    /// <summary>Gets or sets a value that indicates whether message signatures must be confirmed. </summary>
    /// <returns>true if message signatures must be confirmed; otherwise, false. The default is false.</returns>
    public bool RequireSignatureConfirmation
    {
      get
      {
        return this.requireSignatureConfirmation;
      }
      set
      {
        this.requireSignatureConfirmation = value;
      }
    }

    /// <summary>Gets or sets the order of message encryption and signing for this binding.</summary>
    /// <returns>The <see cref="T:System.ServiceModel.Security.MessageProtectionOrder" /> that specifies how the message is protected. The default is <see cref="F:System.ServiceModel.Security.MessageProtectionOrder.SignBeforeEncrypt" />.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">set and value is undefined.</exception>
    public MessageProtectionOrder MessageProtectionOrder
    {
      get
      {
        return this.messageProtectionOrder;
      }
      set
      {
        if (!MessageProtectionOrderHelper.IsDefined(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.messageProtectionOrder = value;
      }
    }

    /// <summary>Gets or sets the protection token parameters.</summary>
    /// <returns>The <see cref="T:System.ServiceModel.Security.Tokens.SecurityTokenParameters" />.</returns>
    public SecurityTokenParameters ProtectionTokenParameters
    {
      get
      {
        return this.protectionTokenParameters;
      }
      set
      {
        this.protectionTokenParameters = value;
      }
    }

    internal override bool SessionMode
    {
      get
      {
        SecureConversationSecurityTokenParameters protectionTokenParameters = this.ProtectionTokenParameters as SecureConversationSecurityTokenParameters;
        if (protectionTokenParameters != null)
        {
          // Not supported
          //return protectionTokenParameters.RequireCancellation;
          return false;
        }
        return false;
      }
    }

    internal override bool SupportsDuplex
    {
      get
      {
        return this.SessionMode;
      }
    }

    internal override bool SupportsRequestReply
    {
      get
      {
        return true;
      }
    }

    private SymmetricSecurityBindingElement(SymmetricSecurityBindingElement elementToBeCloned)
      : base((SecurityBindingElement) elementToBeCloned)
    {
      this.messageProtectionOrder = elementToBeCloned.messageProtectionOrder;
      if (elementToBeCloned.protectionTokenParameters != null)
        this.protectionTokenParameters = elementToBeCloned.protectionTokenParameters.Clone();
      this.requireSignatureConfirmation = elementToBeCloned.requireSignatureConfirmation;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Channels.SymmetricSecurityBindingElement" /> class.  </summary>
    public SymmetricSecurityBindingElement()
      : this((SecurityTokenParameters) null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Channels.SymmetricSecurityBindingElement" /> class using specified security token parameters. </summary>
    /// <param name="protectionTokenParameters">The <see cref="T:System.ServiceModel.Security.Tokens.SecurityTokenParameters" />.</param>
    public SymmetricSecurityBindingElement(SecurityTokenParameters protectionTokenParameters)
      : base()
    {
      this.messageProtectionOrder = MessageProtectionOrder.SignBeforeEncryptAndEncryptSignature;
      this.requireSignatureConfirmation = false;
      this.protectionTokenParameters = protectionTokenParameters;
    }

    internal override ISecurityCapabilities GetIndividualISecurityCapabilities()
    {
      bool supportsServerAuth = false;
      bool supportsClientAuth;
      bool supportsWindowsIdentity;
      this.GetSupportingTokensCapabilities(out supportsClientAuth, out supportsWindowsIdentity);
      if (this.ProtectionTokenParameters != null)
      {
        supportsClientAuth = supportsClientAuth || this.ProtectionTokenParameters.SupportsClientAuthentication;
        supportsWindowsIdentity = supportsWindowsIdentity || this.ProtectionTokenParameters.SupportsClientWindowsIdentity;
        supportsServerAuth = !this.ProtectionTokenParameters.HasAsymmetricKey ? this.ProtectionTokenParameters.SupportsServerAuthentication : this.ProtectionTokenParameters.SupportsClientAuthentication;
      }
      return (ISecurityCapabilities) new SecurityCapabilities(supportsClientAuth, supportsServerAuth, supportsWindowsIdentity, ProtectionLevel.EncryptAndSign, ProtectionLevel.EncryptAndSign);
    }
    /*
    /// <summary>Sets a value that indicates whether derived keys are required.</summary>
    /// <param name="requireDerivedKeys">true to indicate that derived keys are required; otherwise, false.</param>
    public override void SetKeyDerivation(bool requireDerivedKeys)
    {
      base.SetKeyDerivation(requireDerivedKeys);
      if (this.protectionTokenParameters == null)
        return;
      this.protectionTokenParameters.RequireDerivedKeys = requireDerivedKeys;
    }

    internal override bool IsSetKeyDerivation(bool requireDerivedKeys)
    {
      return base.IsSetKeyDerivation(requireDerivedKeys) && (this.protectionTokenParameters == null || this.protectionTokenParameters.RequireDerivedKeys == requireDerivedKeys);
    }*/

    // Nothing to override
    internal override SecurityProtocolFactory CreateSecurityProtocolFactory<TChannel>(BindingContext context, SecurityCredentialsManager credentialsManager, bool isForService, BindingContext issuerBindingContext)
    {
      if (context == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
      if (credentialsManager == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("credentialsManager");
      if (this.ProtectionTokenParameters == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SymmetricSecurityBindingElementNeedsProtectionTokenParameters", new object[1]{ (object) this.ToString() })));
      SymmetricSecurityProtocolFactory securityProtocolFactory = new SymmetricSecurityProtocolFactory();
      if (isForService)
        this.ApplyAuditBehaviorSettings(context, (SecurityProtocolFactory) securityProtocolFactory);
      securityProtocolFactory.SecurityTokenParameters = this.ProtectionTokenParameters.Clone();
      SecurityBindingElement.SetIssuerBindingContextIfRequired(securityProtocolFactory.SecurityTokenParameters, issuerBindingContext);
      securityProtocolFactory.ApplyConfidentiality = true;
      securityProtocolFactory.RequireConfidentiality = true;
      securityProtocolFactory.ApplyIntegrity = true;
      securityProtocolFactory.RequireIntegrity = true;
      securityProtocolFactory.IdentityVerifier = this.LocalClientSettings.IdentityVerifier;
      securityProtocolFactory.DoRequestSignatureConfirmation = this.RequireSignatureConfirmation;
      securityProtocolFactory.MessageProtectionOrder = this.MessageProtectionOrder;
      securityProtocolFactory.ProtectionRequirements.Add(SecurityBindingElement.ComputeProtectionRequirements((SecurityBindingElement) this, context.BindingParameters, context.Binding.Elements, isForService));
      this.ConfigureProtocolFactory((SecurityProtocolFactory) securityProtocolFactory, credentialsManager, isForService, issuerBindingContext, (Binding) context.Binding);
      return (SecurityProtocolFactory) securityProtocolFactory;
    }

    /*internal override bool RequiresChannelDemuxer()
    {
      if (!base.RequiresChannelDemuxer())
        return this.RequiresChannelDemuxer(this.ProtectionTokenParameters);
      return true;
    }*/

    protected override IChannelFactory<TChannel> BuildChannelFactoryCore<TChannel>(BindingContext context)
    {
      ISecurityCapabilities property = this.GetProperty<ISecurityCapabilities>(context);
      SecurityCredentialsManager credentialsManager = context.BindingParameters.Find<SecurityCredentialsManager>() ?? (SecurityCredentialsManager) ClientCredentials.CreateDefaultCredentials();
      bool addChannelDemuxerIfRequired = this.RequiresChannelDemuxer();
      ChannelBuilder channelBuilder = new ChannelBuilder(context, addChannelDemuxerIfRequired);
      if (addChannelDemuxerIfRequired)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("ApplyPropertiesOnDemuxer is not supported in .NET Core");
#else
        this.ApplyPropertiesOnDemuxer(channelBuilder, context);
#endif
      }
      BindingContext bindingContext1 = context.Clone();
      SecurityChannelFactory<TChannel> securityChannelFactory;
      if (this.ProtectionTokenParameters is SecureConversationSecurityTokenParameters)
      {
        SecureConversationSecurityTokenParameters protectionTokenParameters = (SecureConversationSecurityTokenParameters) this.ProtectionTokenParameters;
        if (protectionTokenParameters.BootstrapSecurityBindingElement == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SecureConversationSecurityTokenParametersRequireBootstrapBinding")));
        BindingContext bindingContext2 = bindingContext1.Clone();
        bindingContext2.BindingParameters.Remove<ChannelProtectionRequirements>();
// BootstrapProtectionRequirements not supported
//         bindingContext2.BindingParameters.Add((object) protectionTokenParameters.BootstrapProtectionRequirements);
        if (protectionTokenParameters.RequireCancellation)
        {
          SessionSymmetricMessageSecurityProtocolFactory securityProtocolFactory = new SessionSymmetricMessageSecurityProtocolFactory();
          securityProtocolFactory.SecurityTokenParameters = protectionTokenParameters.Clone();
          ((SecureConversationSecurityTokenParameters) securityProtocolFactory.SecurityTokenParameters).IssuerBindingContext = bindingContext2;
          securityProtocolFactory.ApplyConfidentiality = true;
          securityProtocolFactory.RequireConfidentiality = true;
          securityProtocolFactory.ApplyIntegrity = true;
          securityProtocolFactory.RequireIntegrity = true;
          securityProtocolFactory.IdentityVerifier = this.LocalClientSettings.IdentityVerifier;
          securityProtocolFactory.DoRequestSignatureConfirmation = this.RequireSignatureConfirmation;
          securityProtocolFactory.MessageProtectionOrder = this.MessageProtectionOrder;
          securityProtocolFactory.IdentityVerifier = this.LocalClientSettings.IdentityVerifier;
          securityProtocolFactory.ProtectionRequirements.Add(SecurityBindingElement.ComputeProtectionRequirements((SecurityBindingElement) this, context.BindingParameters, context.Binding.Elements, false));
          this.ConfigureProtocolFactory((SecurityProtocolFactory) securityProtocolFactory, credentialsManager, false, bindingContext1, (Binding) context.Binding);
          SecuritySessionClientSettings<TChannel> sessionClientSettings = new SecuritySessionClientSettings<TChannel>();
          sessionClientSettings.ChannelBuilder = channelBuilder;
          sessionClientSettings.KeyRenewalInterval = this.LocalClientSettings.SessionKeyRenewalInterval;
          sessionClientSettings.CanRenewSession = protectionTokenParameters.CanRenewSession;
          sessionClientSettings.KeyRolloverInterval = this.LocalClientSettings.SessionKeyRolloverInterval;
          sessionClientSettings.TolerateTransportFailures = this.LocalClientSettings.ReconnectTransportOnFailure;
          sessionClientSettings.IssuedSecurityTokenParameters = protectionTokenParameters.Clone();
          ((SecureConversationSecurityTokenParameters) sessionClientSettings.IssuedSecurityTokenParameters).IssuerBindingContext = bindingContext1;
          sessionClientSettings.SecurityStandardsManager = securityProtocolFactory.StandardsManager;
          sessionClientSettings.SessionProtocolFactory = (SecurityProtocolFactory) securityProtocolFactory;
          securityChannelFactory = new SecurityChannelFactory<TChannel>(property, context, sessionClientSettings);
        }
        else
        {
#if FEATURE_CORECLR
          throw new NotImplementedException("SymmetricSecurityProtocolFactory not supported in .NET Core");
#else
          SymmetricSecurityProtocolFactory securityProtocolFactory = new SymmetricSecurityProtocolFactory();
          securityProtocolFactory.SecurityTokenParameters = protectionTokenParameters.Clone();
          ((SecureConversationSecurityTokenParameters) securityProtocolFactory.SecurityTokenParameters).IssuerBindingContext = bindingContext2;
          securityProtocolFactory.ApplyConfidentiality = true;
          securityProtocolFactory.RequireConfidentiality = true;
          securityProtocolFactory.ApplyIntegrity = true;
          securityProtocolFactory.RequireIntegrity = true;
          securityProtocolFactory.IdentityVerifier = this.LocalClientSettings.IdentityVerifier;
          securityProtocolFactory.DoRequestSignatureConfirmation = this.RequireSignatureConfirmation;
          securityProtocolFactory.MessageProtectionOrder = this.MessageProtectionOrder;
          securityProtocolFactory.ProtectionRequirements.Add(SecurityBindingElement.ComputeProtectionRequirements((SecurityBindingElement) this, context.BindingParameters, context.Binding.Elements, false));
          this.ConfigureProtocolFactory((SecurityProtocolFactory) securityProtocolFactory, credentialsManager, false, bindingContext1, (Binding) context.Binding);
          securityChannelFactory = new SecurityChannelFactory<TChannel>(property, context, channelBuilder, (SecurityProtocolFactory) securityProtocolFactory);
#endif
        }
      }
      else
      {
        SecurityProtocolFactory securityProtocolFactory = this.CreateSecurityProtocolFactory<TChannel>(context, credentialsManager, false, bindingContext1);
        securityChannelFactory = new SecurityChannelFactory<TChannel>(property, context, channelBuilder, securityProtocolFactory);
      }
      return (IChannelFactory<TChannel>) securityChannelFactory;
    }

    /*
    // Nothing to override
    // protected override IChannelListener<TChannel> BuildChannelListenerCore<TChannel>(BindingContext context)
    public IChannelListener<TChannel> BuildChannelListenerCore<TChannel>(BindingContext context) where TChannel : class, IChannel
    {
      SecurityChannelListener<TChannel> securityChannelListener = new SecurityChannelListener<TChannel>((SecurityBindingElement) this, context);
      SecurityCredentialsManager credentialsManager = context.BindingParameters.Find<SecurityCredentialsManager>() ?? (SecurityCredentialsManager) ServiceCredentials.CreateDefaultCredentials();
      bool addChannelDemuxerIfRequired = this.RequiresChannelDemuxer();
      ChannelBuilder channelBuilder = new ChannelBuilder(context, addChannelDemuxerIfRequired);
      if (addChannelDemuxerIfRequired)
        this.ApplyPropertiesOnDemuxer(channelBuilder, context);
      BindingContext bindingContext = context.Clone();
      if (this.ProtectionTokenParameters is SecureConversationSecurityTokenParameters)
      {
        SecureConversationSecurityTokenParameters protectionTokenParameters = (SecureConversationSecurityTokenParameters) this.ProtectionTokenParameters;
        if (protectionTokenParameters.BootstrapSecurityBindingElement == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SecureConversationSecurityTokenParametersRequireBootstrapBinding")));
        BindingContext secureConversationBindingContext = bindingContext.Clone();
        secureConversationBindingContext.BindingParameters.Remove<ChannelProtectionRequirements>();
        secureConversationBindingContext.BindingParameters.Add((object) protectionTokenParameters.BootstrapProtectionRequirements);
        IMessageFilterTable<EndpointAddress> messageFilterTable = context.BindingParameters.Find<IMessageFilterTable<EndpointAddress>>();
        this.AddDemuxerForSecureConversation(channelBuilder, secureConversationBindingContext);
        if (protectionTokenParameters.RequireCancellation)
        {
          SessionSymmetricMessageSecurityProtocolFactory securityProtocolFactory = new SessionSymmetricMessageSecurityProtocolFactory();
          this.ApplyAuditBehaviorSettings(context, (SecurityProtocolFactory) securityProtocolFactory);
          securityProtocolFactory.SecurityTokenParameters = protectionTokenParameters.Clone();
          ((SecureConversationSecurityTokenParameters) securityProtocolFactory.SecurityTokenParameters).IssuerBindingContext = secureConversationBindingContext;
          securityProtocolFactory.ApplyConfidentiality = true;
          securityProtocolFactory.RequireConfidentiality = true;
          securityProtocolFactory.ApplyIntegrity = true;
          securityProtocolFactory.RequireIntegrity = true;
          securityProtocolFactory.IdentityVerifier = this.LocalClientSettings.IdentityVerifier;
          securityProtocolFactory.DoRequestSignatureConfirmation = this.RequireSignatureConfirmation;
          securityProtocolFactory.MessageProtectionOrder = this.MessageProtectionOrder;
          securityProtocolFactory.IdentityVerifier = this.LocalClientSettings.IdentityVerifier;
          securityProtocolFactory.ProtectionRequirements.Add(SecurityBindingElement.ComputeProtectionRequirements((SecurityBindingElement) this, context.BindingParameters, context.Binding.Elements, true));
          this.ConfigureProtocolFactory((SecurityProtocolFactory) securityProtocolFactory, credentialsManager, true, bindingContext, (Binding) context.Binding);
          securityChannelListener.SessionMode = true;
          securityChannelListener.SessionServerSettings.InactivityTimeout = this.LocalServiceSettings.InactivityTimeout;
          securityChannelListener.SessionServerSettings.KeyRolloverInterval = this.LocalServiceSettings.SessionKeyRolloverInterval;
          securityChannelListener.SessionServerSettings.MaximumPendingSessions = this.LocalServiceSettings.MaxPendingSessions;
          securityChannelListener.SessionServerSettings.MaximumKeyRenewalInterval = this.LocalServiceSettings.SessionKeyRenewalInterval;
          securityChannelListener.SessionServerSettings.TolerateTransportFailures = this.LocalServiceSettings.ReconnectTransportOnFailure;
          securityChannelListener.SessionServerSettings.CanRenewSession = protectionTokenParameters.CanRenewSession;
          securityChannelListener.SessionServerSettings.IssuedSecurityTokenParameters = protectionTokenParameters.Clone();
          ((SecureConversationSecurityTokenParameters) securityChannelListener.SessionServerSettings.IssuedSecurityTokenParameters).IssuerBindingContext = secureConversationBindingContext;
          securityChannelListener.SessionServerSettings.SecurityStandardsManager = securityProtocolFactory.StandardsManager;
          securityChannelListener.SessionServerSettings.SessionProtocolFactory = (SecurityProtocolFactory) securityProtocolFactory;
          securityChannelListener.SessionServerSettings.SessionProtocolFactory.EndpointFilterTable = messageFilterTable;
          if (context.BindingParameters != null && context.BindingParameters.Find<IChannelDemuxFailureHandler>() == null && !this.IsUnderlyingListenerDuplex<TChannel>(context))
            context.BindingParameters.Add((object) new SecuritySessionServerSettings.SecuritySessionDemuxFailureHandler(securityProtocolFactory.StandardsManager));
        }
        else
        {
          SymmetricSecurityProtocolFactory securityProtocolFactory = new SymmetricSecurityProtocolFactory();
          this.ApplyAuditBehaviorSettings(context, (SecurityProtocolFactory) securityProtocolFactory);
          securityProtocolFactory.SecurityTokenParameters = protectionTokenParameters.Clone();
          ((SecureConversationSecurityTokenParameters) securityProtocolFactory.SecurityTokenParameters).IssuerBindingContext = secureConversationBindingContext;
          securityProtocolFactory.ApplyConfidentiality = true;
          securityProtocolFactory.RequireConfidentiality = true;
          securityProtocolFactory.ApplyIntegrity = true;
          securityProtocolFactory.RequireIntegrity = true;
          securityProtocolFactory.IdentityVerifier = this.LocalClientSettings.IdentityVerifier;
          securityProtocolFactory.DoRequestSignatureConfirmation = this.RequireSignatureConfirmation;
          securityProtocolFactory.MessageProtectionOrder = this.MessageProtectionOrder;
          securityProtocolFactory.ProtectionRequirements.Add(SecurityBindingElement.ComputeProtectionRequirements((SecurityBindingElement) this, context.BindingParameters, context.Binding.Elements, true));
          securityProtocolFactory.EndpointFilterTable = messageFilterTable;
          this.ConfigureProtocolFactory((SecurityProtocolFactory) securityProtocolFactory, credentialsManager, true, bindingContext, (Binding) context.Binding);
          securityChannelListener.SecurityProtocolFactory = (SecurityProtocolFactory) securityProtocolFactory;
        }
      }
      else
      {
        SecurityProtocolFactory securityProtocolFactory = this.CreateSecurityProtocolFactory<TChannel>(context, credentialsManager, true, bindingContext);
        securityChannelListener.SecurityProtocolFactory = securityProtocolFactory;
      }
      securityChannelListener.InitializeListener(channelBuilder);
      return (IChannelListener<TChannel>) securityChannelListener;
    }

    /// <summary>Gets a specified object from the <see cref="T:System.ServiceModel.Channels.BindingContext" />.</summary>
    /// <param name="context">A <see cref="T:System.ServiceModel.Channels.BindingContext" />.</param>
    /// <typeparam name="T">The type of the object to get.</typeparam>
    /// <returns>The specified object of type <paramref name="T" /> from the <see cref="T:System.ServiceModel.Channels.BindingContext" />, or null if the object is not found.</returns>
    public override T GetProperty<T>(BindingContext context)
    {
      if (context == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
      if (!(typeof (T) == typeof (ChannelProtectionRequirements)))
        return base.GetProperty<T>(context);
      AddressingVersion addressing = MessageVersion.Default.Addressing;
      MessageEncodingBindingElement encodingBindingElement = context.Binding.Elements.Find<MessageEncodingBindingElement>();
      if (encodingBindingElement != null)
        addressing = encodingBindingElement.MessageVersion.Addressing;
      ChannelProtectionRequirements protectionRequirements = this.GetProtectionRequirements(addressing, ProtectionLevel.EncryptAndSign);
      protectionRequirements.Add(context.GetInnerProperty<ChannelProtectionRequirements>() ?? new ChannelProtectionRequirements());
      return (T) protectionRequirements;
    }

    /// <summary>Returns a string that represents this <see cref="T:System.ServiceModel.Channels.SymmetricSecurityBindingElement" /> instance.</summary>
    /// <returns>A string that represents this <see cref="T:System.ServiceModel.Channels.SymmetricSecurityBindingElement" /> instance.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(base.ToString());
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "MessageProtectionOrder: {0}", new object[1]
      {
        (object) this.messageProtectionOrder.ToString()
      }));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "RequireSignatureConfirmation: {0}", new object[1]
      {
        (object) this.requireSignatureConfirmation.ToString()
      }));
      stringBuilder.Append("ProtectionTokenParameters: ");
      if (this.protectionTokenParameters != null)
        stringBuilder.AppendLine(this.protectionTokenParameters.ToString().Trim().Replace("\n", "\n  "));
      else
        stringBuilder.AppendLine("null");
      return stringBuilder.ToString().Trim();
    }*/

    /// <summary>Creates a new instance of this class initialized from the current one.</summary>
    /// <returns>A <see cref="T:System.ServiceModel.Channels.BindingElement" /> object with property values equal to those of the current instance.</returns>
    public override BindingElement Clone()
    {
      return (BindingElement) new SymmetricSecurityBindingElement(this);
    }

    // Not supported in .NET Core
    // void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
    // {
    //  SecurityBindingElement.ExportPolicy(exporter, context);
    // }
  }
}
