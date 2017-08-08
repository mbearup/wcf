// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Globalization;
using System.Net.Security;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using System.Text;
using System.Xml;

namespace System.ServiceModel.Channels
{
    public abstract class SecurityBindingElement : BindingElement
    {
        internal const bool defaultIncludeTimestamp = true;
        internal const bool defaultAllowInsecureTransport = false;
        internal const bool defaultRequireSignatureConfirmation = false;
        internal const bool defaultEnableUnsecuredResponse = false;
        internal const bool defaultProtectTokens = false;

        private SupportingTokenParameters _endpointSupportingTokenParameters;
        private bool _includeTimestamp;
        private bool supportsExtendedProtectionPolicy;

        private LocalClientSecuritySettings _localClientSettings;
        private LocalServiceSecuritySettings _localServiceSettings;

        private MessageSecurityVersion _messageSecurityVersion;
        private SecurityHeaderLayout _securityHeaderLayout;
        private long _maxReceivedMessageSize = TransportDefaults.MaxReceivedMessageSize;
        private XmlDictionaryReaderQuotas _readerQuotas;
        private bool _protectTokens = defaultProtectTokens;
#region fromwcf
        internal static readonly SecurityAlgorithmSuite defaultDefaultAlgorithmSuite = SecurityAlgorithmSuite.Default;
        private SecurityAlgorithmSuite _defaultAlgorithmSuite;
#endregion

        internal SecurityBindingElement()
            : base()
        {
            Console.WriteLine("SecurityBindingElement constructor");
            _messageSecurityVersion = MessageSecurityVersion.Default;
            _includeTimestamp = defaultIncludeTimestamp;
            _localClientSettings = new LocalClientSecuritySettings();
            _endpointSupportingTokenParameters = new SupportingTokenParameters();
            _securityHeaderLayout = SecurityProtocolFactory.defaultSecurityHeaderLayout;
            _localServiceSettings = new LocalServiceSecuritySettings();
            _defaultAlgorithmSuite = SecurityBindingElement.defaultDefaultAlgorithmSuite;
        }

        internal SecurityBindingElement(SecurityBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            if (elementToBeCloned == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("elementToBeCloned");
            Console.WriteLine("SecurityBindingElement CLONE constructor");
            _includeTimestamp = elementToBeCloned._includeTimestamp;
            _messageSecurityVersion = elementToBeCloned._messageSecurityVersion;
            _securityHeaderLayout = elementToBeCloned._securityHeaderLayout;
            _endpointSupportingTokenParameters = elementToBeCloned._endpointSupportingTokenParameters.Clone();
            _localClientSettings = elementToBeCloned._localClientSettings.Clone();
            _maxReceivedMessageSize = elementToBeCloned._maxReceivedMessageSize;
            _readerQuotas = elementToBeCloned._readerQuotas;
            _localServiceSettings = elementToBeCloned._localServiceSettings.Clone();
            _defaultAlgorithmSuite = elementToBeCloned.DefaultAlgorithmSuite;
        }

#region FromWCF
    private void SetPrivacyNoticeUriIfRequired(SecurityProtocolFactory factory, Binding binding)
    {
      PrivacyNoticeBindingElement noticeBindingElement = binding.CreateBindingElements().Find<PrivacyNoticeBindingElement>();
      if (noticeBindingElement == null)
        return;
      factory.PrivacyNoticeUri = noticeBindingElement.Url;
      factory.PrivacyNoticeVersion = noticeBindingElement.Version;
    }
    
    public SecurityAlgorithmSuite DefaultAlgorithmSuite
    {
      get
      {
        return this._defaultAlgorithmSuite;
      }
      set
      {
        if (value == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("value"));
        this._defaultAlgorithmSuite = value;
      }
    }

    internal bool SupportsExtendedProtectionPolicy
    {
      get
      {
        return this.supportsExtendedProtectionPolicy;
      }
      set
      {
        this.supportsExtendedProtectionPolicy = value;
      }
    }

    public LocalServiceSecuritySettings LocalServiceSettings
    {
      get
      {
        return this._localServiceSettings;
      }
    }

    internal void ConfigureProtocolFactory(SecurityProtocolFactory factory, SecurityCredentialsManager credentialsManager, bool isForService, BindingContext issuerBindingContext, Binding binding)
    {
      if (factory == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("factory"));
      if (credentialsManager == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("credentialsManager"));
      factory.AddTimestamp = this.IncludeTimestamp;
      if (this.DefaultAlgorithmSuite == null)
      {
          Console.WriteLine("Cannot use a DefaultAlgorithmSuite that is null");
      }
      factory.IncomingAlgorithmSuite = this.DefaultAlgorithmSuite;
      factory.OutgoingAlgorithmSuite = this.DefaultAlgorithmSuite;
      factory.SecurityHeaderLayout = this.SecurityHeaderLayout;
      if (!isForService)
      {
        factory.TimestampValidityDuration = this.LocalClientSettings.TimestampValidityDuration;
        factory.DetectReplays = this.LocalClientSettings.DetectReplays;
        factory.MaxCachedNonces = this.LocalClientSettings.ReplayCacheSize;
        factory.MaxClockSkew = this.LocalClientSettings.MaxClockSkew;
        factory.ReplayWindow = this.LocalClientSettings.ReplayWindow;
        if (this.LocalClientSettings.DetectReplays)
          factory.NonceCache = this.LocalClientSettings.NonceCache;
      }
      else
      {
        factory.TimestampValidityDuration = this.LocalServiceSettings.TimestampValidityDuration;
        factory.DetectReplays = this.LocalServiceSettings.DetectReplays;
        factory.MaxCachedNonces = this.LocalServiceSettings.ReplayCacheSize;
        factory.MaxClockSkew = this.LocalServiceSettings.MaxClockSkew;
        factory.ReplayWindow = this.LocalServiceSettings.ReplayWindow;
        if (this.LocalServiceSettings.DetectReplays)
          factory.NonceCache = this.LocalServiceSettings.NonceCache;
      }
      factory.SecurityBindingElement = (SecurityBindingElement) this.Clone();
      Console.WriteLine("Skipping SecurityBindingElement.SetIssuerBindingContextIfRequired");
      // factory.SecurityBindingElement.SetIssuerBindingContextIfRequired(issuerBindingContext);
      factory.SecurityTokenManager = credentialsManager.CreateSecurityTokenManager();
      SecurityTokenSerializer securityTokenSerializer = factory.SecurityTokenManager.CreateSecurityTokenSerializer(this._messageSecurityVersion.SecurityTokenVersion);
      factory.StandardsManager = new SecurityStandardsManager(this._messageSecurityVersion, securityTokenSerializer);
      if (isForService)
        return;
      this.SetPrivacyNoticeUriIfRequired(factory, binding);
    }

    internal abstract SecurityProtocolFactory CreateSecurityProtocolFactory<TChannel>(BindingContext context, SecurityCredentialsManager credentialsManager, bool isForService, BindingContext issuanceBindingContext);

    internal static ChannelProtectionRequirements ComputeProtectionRequirements(SecurityBindingElement security, BindingParameterCollection parameterCollection, BindingElementCollection bindingElements, bool isForService)
    {
      if (parameterCollection == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameterCollection");
      if (bindingElements == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bindingElements");
      if (security == null)
        return (ChannelProtectionRequirements) null;
      ChannelProtectionRequirements requirements = (ChannelProtectionRequirements) null;
      Console.WriteLine("Skipping AsymmetricSecurityBindingElement because it's not supported");
//      if (security is SymmetricSecurityBindingElement || security is AsymmetricSecurityBindingElement)
      if (security is SymmetricSecurityBindingElement)
      {
        requirements = new ChannelProtectionRequirements();
        ChannelProtectionRequirements protectionRequirements = parameterCollection.Find<ChannelProtectionRequirements>();
        if (protectionRequirements != null)
          requirements.Add(protectionRequirements);
        Console.WriteLine("TODO - AddBindingProtectionRequirements");
        // SecurityBindingElement.AddBindingProtectionRequirements(requirements, bindingElements, !isForService);
      }
      return requirements;
    }

    public static SymmetricSecurityBindingElement CreateIssuedTokenForCertificateBindingElement(IssuedSecurityTokenParameters issuedTokenParameters)
    {
      if (issuedTokenParameters == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("issuedTokenParameters");
#if FEATURE_CORECLR
      throw new NotImplementedException("X509SecurityTokenParameters not supported in .NET Core");
#else
      SymmetricSecurityBindingElement securityBindingElement = new SymmetricSecurityBindingElement((SecurityTokenParameters) new X509SecurityTokenParameters(X509KeyIdentifierClauseType.Thumbprint, SecurityTokenInclusionMode.Never));
      if (issuedTokenParameters.KeyType == SecurityKeyType.BearerKey)
      {
        securityBindingElement.EndpointSupportingTokenParameters.SignedEncrypted.Add((SecurityTokenParameters) issuedTokenParameters);
        securityBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSXDefault;
      }
      else
      {
        securityBindingElement.EndpointSupportingTokenParameters.Endorsing.Add((SecurityTokenParameters) issuedTokenParameters);
        securityBindingElement.MessageSecurityVersion = MessageSecurityVersion.Default;
      }
      securityBindingElement.RequireSignatureConfirmation = true;
      return securityBindingElement;
#endif
    }

    public static SymmetricSecurityBindingElement CreateIssuedTokenForSslBindingElement(IssuedSecurityTokenParameters issuedTokenParameters)
    {
      return SecurityBindingElement.CreateIssuedTokenForSslBindingElement(issuedTokenParameters, false);
    }

    public static SymmetricSecurityBindingElement CreateIssuedTokenForSslBindingElement(IssuedSecurityTokenParameters issuedTokenParameters, bool requireCancellation)
    {
      if (issuedTokenParameters == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("issuedTokenParameters");
      SymmetricSecurityBindingElement securityBindingElement = new SymmetricSecurityBindingElement((SecurityTokenParameters) new SslSecurityTokenParameters(false, requireCancellation));
      if (issuedTokenParameters.KeyType == SecurityKeyType.BearerKey)
      {
        securityBindingElement.EndpointSupportingTokenParameters.SignedEncrypted.Add((SecurityTokenParameters) issuedTokenParameters);
        securityBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSXDefault;
      }
      else
      {
        securityBindingElement.EndpointSupportingTokenParameters.Endorsing.Add((SecurityTokenParameters) issuedTokenParameters);
        securityBindingElement.MessageSecurityVersion = MessageSecurityVersion.Default;
      }
      securityBindingElement.RequireSignatureConfirmation = true;
      return securityBindingElement;
    }

    public static TransportSecurityBindingElement CreateIssuedTokenOverTransportBindingElement(IssuedSecurityTokenParameters issuedTokenParameters)
    {
      if (issuedTokenParameters == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("issuedTokenParameters");
      issuedTokenParameters.RequireDerivedKeys = false;
      TransportSecurityBindingElement securityBindingElement = new TransportSecurityBindingElement();
      if (issuedTokenParameters.KeyType == SecurityKeyType.BearerKey)
      {
        securityBindingElement.EndpointSupportingTokenParameters.Signed.Add((SecurityTokenParameters) issuedTokenParameters);
        securityBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSXDefault;
      }
      else
      {
        securityBindingElement.EndpointSupportingTokenParameters.Endorsing.Add((SecurityTokenParameters) issuedTokenParameters);
        securityBindingElement.MessageSecurityVersion = MessageSecurityVersion.Default;
      }
      securityBindingElement.LocalClientSettings.DetectReplays = false;
      securityBindingElement.LocalServiceSettings.DetectReplays = false;
      securityBindingElement.IncludeTimestamp = true;
      return securityBindingElement;
    }

    public static bool IsIssuedTokenForSslBinding(SecurityBindingElement sbe, out IssuedSecurityTokenParameters issuedTokenParameters)
    {
      return SecurityBindingElement.IsIssuedTokenForSslBinding(sbe, false, out issuedTokenParameters);
    }

    public static bool IsIssuedTokenForSslBinding(SecurityBindingElement sbe, bool requireCancellation, out IssuedSecurityTokenParameters issuedTokenParameters)
    {
      issuedTokenParameters = (IssuedSecurityTokenParameters) null;
      SymmetricSecurityBindingElement securityBindingElement = sbe as SymmetricSecurityBindingElement;
      if (securityBindingElement == null || !securityBindingElement.RequireSignatureConfirmation)
        return false;
      SslSecurityTokenParameters protectionTokenParameters = securityBindingElement.ProtectionTokenParameters as SslSecurityTokenParameters;
      if (protectionTokenParameters == null || protectionTokenParameters.RequireClientCertificate || protectionTokenParameters.RequireCancellation != requireCancellation)
        return false;
      SupportingTokenParameters supportingTokenParameters = securityBindingElement.EndpointSupportingTokenParameters;
      if (supportingTokenParameters.Signed.Count != 0 || supportingTokenParameters.SignedEncrypted.Count == 0 && supportingTokenParameters.Endorsing.Count == 0 || supportingTokenParameters.SignedEndorsing.Count != 0)
        return false;
      if (supportingTokenParameters.SignedEncrypted.Count == 1 && supportingTokenParameters.Endorsing.Count == 0)
      {
        issuedTokenParameters = supportingTokenParameters.SignedEncrypted[0] as IssuedSecurityTokenParameters;
        if (issuedTokenParameters != null && issuedTokenParameters.KeyType != SecurityKeyType.BearerKey)
          return false;
      }
      else if (supportingTokenParameters.Endorsing.Count == 1 && supportingTokenParameters.SignedEncrypted.Count == 0)
      {
        issuedTokenParameters = supportingTokenParameters.Endorsing[0] as IssuedSecurityTokenParameters;
        if (issuedTokenParameters != null && issuedTokenParameters.KeyType != SecurityKeyType.SymmetricKey && issuedTokenParameters.KeyType != SecurityKeyType.AsymmetricKey)
          return false;
      }
      return issuedTokenParameters != null;
    }

    public static bool IsIssuedTokenOverTransportBinding(SecurityBindingElement sbe, out IssuedSecurityTokenParameters issuedTokenParameters)
    {
      issuedTokenParameters = (IssuedSecurityTokenParameters) null;
      if (!(sbe is TransportSecurityBindingElement) || !sbe.IncludeTimestamp)
        return false;
      SupportingTokenParameters supportingTokenParameters = sbe.EndpointSupportingTokenParameters;
      if (supportingTokenParameters.SignedEncrypted.Count != 0 || supportingTokenParameters.Signed.Count == 0 && supportingTokenParameters.Endorsing.Count == 0 || supportingTokenParameters.SignedEndorsing.Count != 0)
        return false;
      if (supportingTokenParameters.Signed.Count == 1 && supportingTokenParameters.Endorsing.Count == 0)
      {
        issuedTokenParameters = supportingTokenParameters.Signed[0] as IssuedSecurityTokenParameters;
        if (issuedTokenParameters != null && issuedTokenParameters.KeyType != SecurityKeyType.BearerKey)
          return false;
      }
      else if (supportingTokenParameters.Endorsing.Count == 1 && supportingTokenParameters.Signed.Count == 0)
      {
        issuedTokenParameters = supportingTokenParameters.Endorsing[0] as IssuedSecurityTokenParameters;
        if (issuedTokenParameters != null && issuedTokenParameters.KeyType != SecurityKeyType.SymmetricKey && issuedTokenParameters.KeyType != SecurityKeyType.AsymmetricKey)
          return false;
      }
      return issuedTokenParameters != null && !issuedTokenParameters.RequireDerivedKeys;
    }

    public static bool IsIssuedTokenForCertificateBinding(SecurityBindingElement sbe, out IssuedSecurityTokenParameters issuedTokenParameters)
    {
      issuedTokenParameters = (IssuedSecurityTokenParameters) null;
      SymmetricSecurityBindingElement securityBindingElement = sbe as SymmetricSecurityBindingElement;
      if (securityBindingElement == null || !securityBindingElement.RequireSignatureConfirmation)
        return false;
#if FEATURE_CORECLR
      throw new NotImplementedException("X509SecurityTokenParameters not implemented in .NET Core");
#else
      X509SecurityTokenParameters protectionTokenParameters = securityBindingElement.ProtectionTokenParameters as X509SecurityTokenParameters;
      if (protectionTokenParameters == null || protectionTokenParameters.X509ReferenceStyle != X509KeyIdentifierClauseType.Thumbprint || protectionTokenParameters.InclusionMode != SecurityTokenInclusionMode.Never)
        return false;
      SupportingTokenParameters supportingTokenParameters = securityBindingElement.EndpointSupportingTokenParameters;
      if (supportingTokenParameters.Signed.Count != 0 || supportingTokenParameters.SignedEncrypted.Count == 0 && supportingTokenParameters.Endorsing.Count == 0 || supportingTokenParameters.SignedEndorsing.Count != 0)
        return false;
      if (supportingTokenParameters.SignedEncrypted.Count == 1 && supportingTokenParameters.Endorsing.Count == 0)
      {
        issuedTokenParameters = supportingTokenParameters.SignedEncrypted[0] as IssuedSecurityTokenParameters;
        if (issuedTokenParameters != null && issuedTokenParameters.KeyType != SecurityKeyType.BearerKey)
          return false;
      }
      else if (supportingTokenParameters.Endorsing.Count == 1 && supportingTokenParameters.SignedEncrypted.Count == 0)
      {
        issuedTokenParameters = supportingTokenParameters.Endorsing[0] as IssuedSecurityTokenParameters;
        if (issuedTokenParameters != null && issuedTokenParameters.KeyType != SecurityKeyType.SymmetricKey && issuedTokenParameters.KeyType != SecurityKeyType.AsymmetricKey)
          return false;
      }
      return issuedTokenParameters != null;
#endif
   }

    public static SymmetricSecurityBindingElement CreateKerberosBindingElement()
    {
      throw new NotImplementedException("KerberosSecurityTokenParameters not supported in .NET Core");
      // SymmetricSecurityBindingElement securityBindingElement = new SymmetricSecurityBindingElement((SecurityTokenParameters) new KerberosSecurityTokenParameters());
      // securityBindingElement.DefaultAlgorithmSuite = SecurityAlgorithmSuite.KerberosDefault;
      // return securityBindingElement;
    }

    public static SymmetricSecurityBindingElement CreateSslNegotiationBindingElement(bool requireClientCertificate)
    {
      return SecurityBindingElement.CreateSslNegotiationBindingElement(requireClientCertificate, false);
    }

    public static SymmetricSecurityBindingElement CreateSslNegotiationBindingElement(bool requireClientCertificate, bool requireCancellation)
    {
      throw new NotImplementedException("SslSecurityTokenParameters not supported in .NET Core");
      // return new SymmetricSecurityBindingElement((SecurityTokenParameters) new SslSecurityTokenParameters(requireClientCertificate, requireCancellation));
    }

    public static SymmetricSecurityBindingElement CreateSspiNegotiationBindingElement()
    {
      return SecurityBindingElement.CreateSspiNegotiationBindingElement(false);
    }

    public static SymmetricSecurityBindingElement CreateSspiNegotiationBindingElement(bool requireCancellation)
    {
      return new SymmetricSecurityBindingElement((SecurityTokenParameters) new SspiSecurityTokenParameters(requireCancellation));
    }

    public static TransportSecurityBindingElement CreateSspiNegotiationOverTransportBindingElement()
    {
      return SecurityBindingElement.CreateSspiNegotiationOverTransportBindingElement(true);
    }

    public static TransportSecurityBindingElement CreateSspiNegotiationOverTransportBindingElement(bool requireCancellation)
    {
      TransportSecurityBindingElement securityBindingElement = new TransportSecurityBindingElement();
      SspiSecurityTokenParameters securityTokenParameters = new SspiSecurityTokenParameters(requireCancellation);
      securityTokenParameters.RequireDerivedKeys = false;
      securityBindingElement.EndpointSupportingTokenParameters.Endorsing.Add((SecurityTokenParameters) securityTokenParameters);
      securityBindingElement.IncludeTimestamp = true;
      securityBindingElement.LocalClientSettings.DetectReplays = false;
      securityBindingElement.LocalServiceSettings.DetectReplays = false;
      securityBindingElement.SupportsExtendedProtectionPolicy = true;
      return securityBindingElement;
    }

    public static SymmetricSecurityBindingElement CreateUserNameForCertificateBindingElement()
    {
      throw new NotImplementedException("X509SecurityTokenParameters not supported in .NET Core");
      // SymmetricSecurityBindingElement securityBindingElement = new SymmetricSecurityBindingElement((SecurityTokenParameters) new X509SecurityTokenParameters(X509KeyIdentifierClauseType.Thumbprint, SecurityTokenInclusionMode.Never));
      // securityBindingElement.EndpointSupportingTokenParameters.SignedEncrypted.Add((SecurityTokenParameters) new UserNameSecurityTokenParameters());
      // securityBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11;
      // return securityBindingElement;
    }

    public static SymmetricSecurityBindingElement CreateUserNameForSslBindingElement()
    {
      return SecurityBindingElement.CreateUserNameForSslBindingElement(false);
    }

    public static SymmetricSecurityBindingElement CreateUserNameForSslBindingElement(bool requireCancellation)
    {
      throw new NotImplementedException("SslSecurityTokenParameters not supported in .NET Core");
      // SymmetricSecurityBindingElement securityBindingElement = new SymmetricSecurityBindingElement((SecurityTokenParameters) new SslSecurityTokenParameters(false, requireCancellation));
      // securityBindingElement.EndpointSupportingTokenParameters.SignedEncrypted.Add((SecurityTokenParameters) new UserNameSecurityTokenParameters());
      // securityBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11;
      // return securityBindingElement;
    }

    public static bool IsAnonymousForCertificateBinding(SecurityBindingElement sbe)
    {
      SymmetricSecurityBindingElement securityBindingElement = sbe as SymmetricSecurityBindingElement;
      if (securityBindingElement == null || !securityBindingElement.RequireSignatureConfirmation)
        return false;
      throw new NotImplementedException("X509SecurityTokenParameters not implemented in .NET Core");
      // X509SecurityTokenParameters protectionTokenParameters = securityBindingElement.ProtectionTokenParameters as X509SecurityTokenParameters;
      // return protectionTokenParameters != null && protectionTokenParameters.X509ReferenceStyle == X509KeyIdentifierClauseType.Thumbprint && (protectionTokenParameters.InclusionMode == SecurityTokenInclusionMode.Never && sbe.EndpointSupportingTokenParameters.IsEmpty());
    }

    public static SymmetricSecurityBindingElement CreateAnonymousForCertificateBindingElement()
    {
      throw new NotImplementedException("X509SecurityTokenParameters is not supported in .NET Core 1.0");
      // SymmetricSecurityBindingElement securityBindingElement = new SymmetricSecurityBindingElement((SecurityTokenParameters) new X509SecurityTokenParameters(X509KeyIdentifierClauseType.Thumbprint, SecurityTokenInclusionMode.Never));
      // securityBindingElement.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11;
      // securityBindingElement.RequireSignatureConfirmation = true;
      // return securityBindingElement;
    }

    public static bool IsUserNameForSslBinding(SecurityBindingElement sbe, bool requireCancellation)
    {
      SymmetricSecurityBindingElement securityBindingElement = sbe as SymmetricSecurityBindingElement;
      if (securityBindingElement == null)
        return false;
      SupportingTokenParameters supportingTokenParameters = sbe.EndpointSupportingTokenParameters;
      if (supportingTokenParameters.Signed.Count != 0 || supportingTokenParameters.SignedEncrypted.Count != 1 || (supportingTokenParameters.Endorsing.Count != 0 || supportingTokenParameters.SignedEndorsing.Count != 0) || !(supportingTokenParameters.SignedEncrypted[0] is UserNameSecurityTokenParameters))
        return false;
      throw new NotImplementedException("SslSecurityTokenParameters not implemented in .NET Core");
      // SslSecurityTokenParameters protectionTokenParameters = securityBindingElement.ProtectionTokenParameters as SslSecurityTokenParameters;
      // if (protectionTokenParameters == null || protectionTokenParameters.RequireCancellation != requireCancellation)
      //   return false;
      // return !protectionTokenParameters.RequireClientCertificate;
    }

    public static bool IsUserNameForCertificateBinding(SecurityBindingElement sbe)
    {
      SymmetricSecurityBindingElement securityBindingElement = sbe as SymmetricSecurityBindingElement;
      if (securityBindingElement == null)
        return false;
      // X509SecurityTokenParameters is not supported in .NET Core
      // X509SecurityTokenParameters protectionTokenParameters = securityBindingElement.ProtectionTokenParameters as X509SecurityTokenParameters;
      // if (protectionTokenParameters == null || protectionTokenParameters.X509ReferenceStyle != X509KeyIdentifierClauseType.Thumbprint || protectionTokenParameters.InclusionMode != SecurityTokenInclusionMode.Never)
      //   return false;
      SupportingTokenParameters supportingTokenParameters = sbe.EndpointSupportingTokenParameters;
      return supportingTokenParameters.Signed.Count == 0 && supportingTokenParameters.SignedEncrypted.Count == 1 && (supportingTokenParameters.Endorsing.Count == 0 && supportingTokenParameters.SignedEndorsing.Count == 0) && supportingTokenParameters.SignedEncrypted[0] is UserNameSecurityTokenParameters;
    }

    public static bool IsSspiNegotiationBinding(SecurityBindingElement sbe, bool requireCancellation)
    {
      SymmetricSecurityBindingElement securityBindingElement = sbe as SymmetricSecurityBindingElement;
      if (securityBindingElement == null || !sbe.EndpointSupportingTokenParameters.IsEmpty())
        return false;
      throw new NotImplementedException("SspiSecurityTokenParameters are not supported in .NET Core");
      // SspiSecurityTokenParameters protectionTokenParameters = securityBindingElement.ProtectionTokenParameters as SspiSecurityTokenParameters;
      // if (protectionTokenParameters == null)
      //   return false;
      // return protectionTokenParameters.RequireCancellation == requireCancellation;
    }

    public static bool IsSspiNegotiationOverTransportBinding(SecurityBindingElement sbe, bool requireCancellation)
    {
      if (!sbe.IncludeTimestamp)
        return false;
      SupportingTokenParameters supportingTokenParameters = sbe.EndpointSupportingTokenParameters;
      if (supportingTokenParameters.Signed.Count != 0 || supportingTokenParameters.SignedEncrypted.Count != 0 || (supportingTokenParameters.Endorsing.Count != 1 || supportingTokenParameters.SignedEndorsing.Count != 0))
        return false;
      throw new NotImplementedException("SspiSecurityTokenParameters are not supported in .NET Core");
      // SspiSecurityTokenParameters securityTokenParameters = supportingTokenParameters.Endorsing[0] as SspiSecurityTokenParameters;
      // return securityTokenParameters != null && !securityTokenParameters.RequireDerivedKeys && (securityTokenParameters.RequireCancellation == requireCancellation && sbe is TransportSecurityBindingElement);
    }

    public static bool IsSslNegotiationBinding(SecurityBindingElement sbe, bool requireClientCertificate, bool requireCancellation)
    {
      SymmetricSecurityBindingElement securityBindingElement = sbe as SymmetricSecurityBindingElement;
      if (securityBindingElement == null || !sbe.EndpointSupportingTokenParameters.IsEmpty())
        return false;
      throw new NotImplementedException("SslSecurityTokenParameters are not supported in .NET Core");
      // SslSecurityTokenParameters protectionTokenParameters = securityBindingElement.ProtectionTokenParameters as SslSecurityTokenParameters;
      // if (protectionTokenParameters == null || protectionTokenParameters.RequireClientCertificate != requireClientCertificate)
      //   return false;
      // return protectionTokenParameters.RequireCancellation == requireCancellation;
    }

    public static bool IsSecureConversationBinding(SecurityBindingElement sbe, out SecurityBindingElement bootstrapSecurity)
    {
      return SecurityBindingElement.IsSecureConversationBinding(sbe, true, out bootstrapSecurity);
    }

    public static bool IsSecureConversationBinding(SecurityBindingElement sbe, bool requireCancellation, out SecurityBindingElement bootstrapSecurity)
    {
      bootstrapSecurity = (SecurityBindingElement) null;
      SymmetricSecurityBindingElement securityBindingElement = sbe as SymmetricSecurityBindingElement;
      if (securityBindingElement != null)
      {
        if (securityBindingElement.RequireSignatureConfirmation)
          return false;
        SecureConversationSecurityTokenParameters protectionTokenParameters = securityBindingElement.ProtectionTokenParameters as SecureConversationSecurityTokenParameters;
        if (protectionTokenParameters == null || protectionTokenParameters.RequireCancellation != requireCancellation)
          return false;
        bootstrapSecurity = protectionTokenParameters.BootstrapSecurityBindingElement;
      }
      else
      {
        if (!sbe.IncludeTimestamp || !(sbe is TransportSecurityBindingElement))
          return false;
        SupportingTokenParameters supportingTokenParameters = sbe.EndpointSupportingTokenParameters;
        if (supportingTokenParameters.Signed.Count != 0 || supportingTokenParameters.SignedEncrypted.Count != 0 || (supportingTokenParameters.Endorsing.Count != 1 || supportingTokenParameters.SignedEndorsing.Count != 0))
          return false;
        SecureConversationSecurityTokenParameters securityTokenParameters = supportingTokenParameters.Endorsing[0] as SecureConversationSecurityTokenParameters;
        if (securityTokenParameters == null || securityTokenParameters.RequireCancellation != requireCancellation)
          return false;
        bootstrapSecurity = securityTokenParameters.BootstrapSecurityBindingElement;
      }
      if (bootstrapSecurity != null && bootstrapSecurity.SecurityHeaderLayout != SecurityHeaderLayout.Strict)
        return false;
      return bootstrapSecurity != null;
    }

    public static bool IsKerberosBinding(SecurityBindingElement sbe)
    {
      SymmetricSecurityBindingElement securityBindingElement = sbe as SymmetricSecurityBindingElement;
      throw new NotImplementedException("KerberosSecurityTokenParameters is not supported in .NET Core");
      // return securityBindingElement != null && securityBindingElement.ProtectionTokenParameters is KerberosSecurityTokenParameters && sbe.EndpointSupportingTokenParameters.IsEmpty();
    }

#endregion

        public SupportingTokenParameters EndpointSupportingTokenParameters
        {
            get
            {
                return _endpointSupportingTokenParameters;
            }
        }

        public SecurityHeaderLayout SecurityHeaderLayout
        {
            get
            {
                return _securityHeaderLayout;
            }
            set
            {
                if (!SecurityHeaderLayoutHelper.IsDefined(value))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));

                _securityHeaderLayout = value;
            }
        }

        public MessageSecurityVersion MessageSecurityVersion
        {
            get
            {
                return _messageSecurityVersion;
            }
            set
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                _messageSecurityVersion = value;
            }
        }

        public bool IncludeTimestamp
        {
            get
            {
                return _includeTimestamp;
            }
            set
            {
                _includeTimestamp = value;
            }
        }

        public LocalClientSecuritySettings LocalClientSettings
        {
            get
            {
                return _localClientSettings;
            }
        }

        internal virtual bool SessionMode
        {
            get { return false; }
        }

        internal virtual bool SupportsDuplex
        {
            get { return false; }
        }

        internal virtual bool SupportsRequestReply
        {
            get { return false; }
        }

        internal long MaxReceivedMessageSize
        {
            get { return _maxReceivedMessageSize; }
            set { _maxReceivedMessageSize = value; }
        }

        internal XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { return _readerQuotas; }
            set { _readerQuotas = value; }
        }

        private void GetSupportingTokensCapabilities(ICollection<SecurityTokenParameters> parameters, out bool supportsClientAuth, out bool supportsWindowsIdentity)
        {
            supportsClientAuth = false;
            supportsWindowsIdentity = false;
            foreach (SecurityTokenParameters p in parameters)
            {
                if (p.SupportsClientAuthentication)
                    supportsClientAuth = true;
                if (p.SupportsClientWindowsIdentity)
                    supportsWindowsIdentity = true;
            }
        }

        private void GetSupportingTokensCapabilities(SupportingTokenParameters requirements, out bool supportsClientAuth, out bool supportsWindowsIdentity)
        {
            supportsClientAuth = false;
            supportsWindowsIdentity = false;
            bool tmpSupportsClientAuth;
            bool tmpSupportsWindowsIdentity;
            this.GetSupportingTokensCapabilities(requirements.Endorsing, out tmpSupportsClientAuth, out tmpSupportsWindowsIdentity);
            supportsClientAuth = supportsClientAuth || tmpSupportsClientAuth;
            supportsWindowsIdentity = supportsWindowsIdentity || tmpSupportsWindowsIdentity;

            this.GetSupportingTokensCapabilities(requirements.SignedEndorsing, out tmpSupportsClientAuth, out tmpSupportsWindowsIdentity);
            supportsClientAuth = supportsClientAuth || tmpSupportsClientAuth;
            supportsWindowsIdentity = supportsWindowsIdentity || tmpSupportsWindowsIdentity;

            this.GetSupportingTokensCapabilities(requirements.SignedEncrypted, out tmpSupportsClientAuth, out tmpSupportsWindowsIdentity);
            supportsClientAuth = supportsClientAuth || tmpSupportsClientAuth;
            supportsWindowsIdentity = supportsWindowsIdentity || tmpSupportsWindowsIdentity;
        }

        internal void GetSupportingTokensCapabilities(out bool supportsClientAuth, out bool supportsWindowsIdentity)
        {
            this.GetSupportingTokensCapabilities(this.EndpointSupportingTokenParameters, out supportsClientAuth, out supportsWindowsIdentity);
        }

        protected static void SetIssuerBindingContextIfRequired(SecurityTokenParameters parameters, BindingContext issuerBindingContext)
        {
            throw ExceptionHelper.PlatformNotSupported("SetIssuerBindingContextIfRequired is not supported.");
        }

        internal bool RequiresChannelDemuxer(SecurityTokenParameters parameters)
        {
            throw ExceptionHelper.PlatformNotSupported("RequiresChannelDemuxer is not supported.");
        }

        internal virtual bool RequiresChannelDemuxer()
        {
            foreach (SecurityTokenParameters parameters in EndpointSupportingTokenParameters.Endorsing)
            {
                if (RequiresChannelDemuxer(parameters))
                {
                    return true;
                }
            }
            foreach (SecurityTokenParameters parameters in EndpointSupportingTokenParameters.SignedEndorsing)
            {
                if (RequiresChannelDemuxer(parameters))
                {
                    return true;
                }
            }

            return false;
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");

            if (!this.CanBuildChannelFactory<TChannel>(context))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.ChannelTypeNotSupported, typeof(TChannel)), "TChannel"));
            }

            _readerQuotas = context.GetInnerProperty<XmlDictionaryReaderQuotas>();
            if (_readerQuotas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.EncodingBindingElementDoesNotHandleReaderQuotas)));
            }

            TransportBindingElement transportBindingElement = null;

            if (context.RemainingBindingElements != null)
                transportBindingElement = context.RemainingBindingElements.Find<TransportBindingElement>();

            if (transportBindingElement != null)
                _maxReceivedMessageSize = transportBindingElement.MaxReceivedMessageSize;

            IChannelFactory<TChannel> result = this.BuildChannelFactoryCore<TChannel>(context);

            return result;
        }

        protected abstract IChannelFactory<TChannel> BuildChannelFactoryCore<TChannel>(BindingContext context);

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");

            if (this.SessionMode)
            {
                return this.CanBuildSessionChannelFactory<TChannel>(context);
            }

            if (!context.CanBuildInnerChannelFactory<TChannel>())
            {
                return false;
            }

            return typeof(TChannel) == typeof(IOutputChannel) || typeof(TChannel) == typeof(IOutputSessionChannel) ||
                (this.SupportsDuplex && (typeof(TChannel) == typeof(IDuplexChannel) || typeof(TChannel) == typeof(IDuplexSessionChannel))) ||
                (this.SupportsRequestReply && (typeof(TChannel) == typeof(IRequestChannel) || typeof(TChannel) == typeof(IRequestSessionChannel)));
        }

        private bool CanBuildSessionChannelFactory<TChannel>(BindingContext context)
        {
            if (!(context.CanBuildInnerChannelFactory<IRequestChannel>()
                || context.CanBuildInnerChannelFactory<IRequestSessionChannel>()
                || context.CanBuildInnerChannelFactory<IDuplexChannel>()
                || context.CanBuildInnerChannelFactory<IDuplexSessionChannel>()))
            {
                return false;
            }

            if (typeof(TChannel) == typeof(IRequestSessionChannel))
            {
                return (context.CanBuildInnerChannelFactory<IRequestChannel>() || context.CanBuildInnerChannelFactory<IRequestSessionChannel>());
            }
            else if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return (context.CanBuildInnerChannelFactory<IDuplexChannel>() || context.CanBuildInnerChannelFactory<IDuplexSessionChannel>());
            }
            else
            {
                return false;
            }
        }

        public virtual void SetKeyDerivation(bool requireDerivedKeys)
        {
            EndpointSupportingTokenParameters.SetKeyDerivation(requireDerivedKeys);
        }

        internal virtual bool IsSetKeyDerivation(bool requireDerivedKeys)
        {
            if (!EndpointSupportingTokenParameters.IsSetKeyDerivation(requireDerivedKeys))
                return false;

            return true;
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T)(object)GetSecurityCapabilities(context);
            }
            else if (typeof(T) == typeof(IdentityVerifier))
            {
                return (T)(object)_localClientSettings.IdentityVerifier;
            }
            else
            {
                return context.GetInnerProperty<T>();
            }
        }

        internal abstract ISecurityCapabilities GetIndividualISecurityCapabilities();

        private ISecurityCapabilities GetSecurityCapabilities(BindingContext context)
        {
            ISecurityCapabilities thisSecurityCapability = this.GetIndividualISecurityCapabilities();
            ISecurityCapabilities lowerSecurityCapability = context.GetInnerProperty<ISecurityCapabilities>();
            if (lowerSecurityCapability == null)
            {
                return thisSecurityCapability;
            }
            else
            {
                bool supportsClientAuth = thisSecurityCapability.SupportsClientAuthentication;
                bool supportsClientWindowsIdentity = thisSecurityCapability.SupportsClientWindowsIdentity;
                bool supportsServerAuth = thisSecurityCapability.SupportsServerAuthentication || lowerSecurityCapability.SupportsServerAuthentication;
                ProtectionLevel requestProtectionLevel = ProtectionLevelHelper.Max(thisSecurityCapability.SupportedRequestProtectionLevel, lowerSecurityCapability.SupportedRequestProtectionLevel);
                ProtectionLevel responseProtectionLevel = ProtectionLevelHelper.Max(thisSecurityCapability.SupportedResponseProtectionLevel, lowerSecurityCapability.SupportedResponseProtectionLevel);
                return new SecurityCapabilities(supportsClientAuth, supportsServerAuth, supportsClientWindowsIdentity, requestProtectionLevel, responseProtectionLevel);
            }
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsMutualCertificateBinding() method.
        static public SecurityBindingElement CreateMutualCertificateBindingElement()
        {
            return CreateMutualCertificateBindingElement(MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11);
        }

        // this method reverses CreateMutualCertificateBindingElement() logic
        public static bool IsMutualCertificateBinding(SecurityBindingElement sbe)
        {
            return IsMutualCertificateBinding(sbe, false);
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsMutualCertificateBinding() method.
        static public SecurityBindingElement CreateMutualCertificateBindingElement(MessageSecurityVersion version)
        {
            return CreateMutualCertificateBindingElement(version, false);
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsMutualCertificateBinding() method.
        static public SecurityBindingElement CreateMutualCertificateBindingElement(MessageSecurityVersion version, bool allowSerializedSigningTokenOnReply)
        {
            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("version");
            }

            throw ExceptionHelper.PlatformNotSupported("SecurityBindingElement.CreateMutualCertificateBindingElement is not supported.");
        }


        // this method reverses CreateMutualCertificateBindingElement() logic
        internal static bool IsMutualCertificateBinding(SecurityBindingElement sbe, bool allowSerializedSigningTokenOnReply)
        {
            throw ExceptionHelper.PlatformNotSupported("SecurityBindingElement.IsMutualCertificateBinding is not supported.");
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsUserNameOverTransportBinding() method.
        static public TransportSecurityBindingElement CreateUserNameOverTransportBindingElement()
        {
            TransportSecurityBindingElement result = new TransportSecurityBindingElement();
            result.EndpointSupportingTokenParameters.SignedEncrypted.Add(
                new UserNameSecurityTokenParameters());
            result.IncludeTimestamp = true;

            return result;
        }

        // this method reverses CreateMutualCertificateBindingElement() logic
        public static bool IsUserNameOverTransportBinding(SecurityBindingElement sbe)
        {
            // do not check local settings: sbe.LocalServiceSettings and sbe.LocalClientSettings
            if (!sbe.IncludeTimestamp)
                return false;

            if (!(sbe is TransportSecurityBindingElement))
                return false;

            SupportingTokenParameters parameters = sbe.EndpointSupportingTokenParameters;
            if (parameters.Signed.Count != 0 || parameters.SignedEncrypted.Count != 1 || parameters.Endorsing.Count != 0 || parameters.SignedEndorsing.Count != 0)
                return false;

            UserNameSecurityTokenParameters userNameParameters = parameters.SignedEncrypted[0] as UserNameSecurityTokenParameters;
            if (userNameParameters == null)
                return false;

            return true;
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsCertificateOverTransportBinding() method.
        static public TransportSecurityBindingElement CreateCertificateOverTransportBindingElement(MessageSecurityVersion version)
        {
            if (version == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("version");
            }

            throw ExceptionHelper.PlatformNotSupported("SecurityBindingElement.CreateCertificateOverTransportBindingElement is not supported.");
        }

        // this method reverses CreateMutualCertificateBindingElement() logic
        public static bool IsCertificateOverTransportBinding(SecurityBindingElement sbe)
        {
            // do not check local settings: sbe.LocalServiceSettings and sbe.LocalClientSettings
            if (!sbe.IncludeTimestamp)
                return false;

            if (!(sbe is TransportSecurityBindingElement))
                return false;

            SupportingTokenParameters parameters = sbe.EndpointSupportingTokenParameters;
            if (parameters.Signed.Count != 0 || parameters.SignedEncrypted.Count != 0 || parameters.Endorsing.Count != 1 || parameters.SignedEndorsing.Count != 0)
                return false;

            throw ExceptionHelper.PlatformNotSupported("SecurityBindingElement.IsCertificateOverTransportBinding is not supported.");
        }

        // If any changes are made to this method, please make sure that they are
        // reflected in the corresponding IsSecureConversationBinding() method.
        static public SecurityBindingElement CreateSecureConversationBindingElement(SecurityBindingElement bootstrapSecurity, bool option = true, ChannelProtectionRequirements bootstrapProtectionRequirements = null)
        {
            if (bootstrapSecurity == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bootstrapBinding");
            SecurityBindingElement securityBindingElement1;
            if (bootstrapSecurity is TransportSecurityBindingElement)
            {
                TransportSecurityBindingElement securityBindingElement2 = new TransportSecurityBindingElement();
                SecureConversationSecurityTokenParameters securityTokenParameters = new SecureConversationSecurityTokenParameters(bootstrapSecurity, option, bootstrapProtectionRequirements);
                securityTokenParameters.RequireDerivedKeys = false;
                securityBindingElement2.EndpointSupportingTokenParameters.Endorsing.Add((SecurityTokenParameters) securityTokenParameters);
                securityBindingElement2.LocalClientSettings.DetectReplays = false;
                securityBindingElement2.LocalServiceSettings.DetectReplays = false;
                securityBindingElement2.IncludeTimestamp = true;
                securityBindingElement1 = (SecurityBindingElement) securityBindingElement2;
            }
            else
            securityBindingElement1 = (SecurityBindingElement) new SymmetricSecurityBindingElement((SecurityTokenParameters) new SecureConversationSecurityTokenParameters(bootstrapSecurity, option, bootstrapProtectionRequirements))
            {
                RequireSignatureConfirmation = false
            };
            return securityBindingElement1;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "{0}:", this.GetType().ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "IncludeTimestamp: {0}", _includeTimestamp.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "MessageSecurityVersion: {0}", this.MessageSecurityVersion.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "SecurityHeaderLayout: {0}", _securityHeaderLayout.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "ProtectTokens: {0}", _protectTokens.ToString()));
            sb.AppendLine("EndpointSupportingTokenParameters:");
            sb.AppendLine("  " + this.EndpointSupportingTokenParameters.ToString().Trim().Replace("\n", "\n  "));

            return sb.ToString().Trim();
        }
    }
}
