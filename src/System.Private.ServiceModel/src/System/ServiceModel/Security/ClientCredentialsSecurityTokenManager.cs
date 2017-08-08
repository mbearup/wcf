// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public class ClientCredentialsSecurityTokenManager : SecurityTokenManager
    {
        private ClientCredentials _parent;

        public ClientCredentialsSecurityTokenManager(ClientCredentials clientCredentials)
        {
            if (clientCredentials == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("clientCredentials");
            }
            _parent = clientCredentials;
        }

        public ClientCredentials ClientCredentials
        {
            get { return _parent; }
        }

        private string GetServicePrincipalName(InitiatorServiceModelSecurityTokenRequirement initiatorRequirement)
        {
            EndpointAddress targetAddress = initiatorRequirement.TargetAddress;
            if (targetAddress == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.Format(SR.TokenRequirementDoesNotSpecifyTargetAddress, initiatorRequirement));
            }
            IdentityVerifier identityVerifier;
            SecurityBindingElement securityBindingElement = initiatorRequirement.SecurityBindingElement;
            if (securityBindingElement != null)
            {
                identityVerifier = securityBindingElement.LocalClientSettings.IdentityVerifier;
            }
            else
            {
                identityVerifier = IdentityVerifier.CreateDefault();
            }
            EndpointIdentity identity;
            identityVerifier.TryGetIdentity(targetAddress, out identity);
            return SecurityUtils.GetSpnFromIdentity(identity, targetAddress);
        }

        private bool IsDigestAuthenticationScheme(SecurityTokenRequirement requirement)
        {
            if (requirement.Properties.ContainsKey(ServiceModelSecurityTokenRequirement.HttpAuthenticationSchemeProperty))
            {
                AuthenticationSchemes authScheme = (AuthenticationSchemes)requirement.Properties[ServiceModelSecurityTokenRequirement.HttpAuthenticationSchemeProperty];

                if (!authScheme.IsSingleton())
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("authScheme", string.Format(SR.HttpRequiresSingleAuthScheme, authScheme));
                }

                return (authScheme == AuthenticationSchemes.Digest);
            }
            else
            {
                return false;
            }
        }

        internal protected bool IsIssuedSecurityTokenRequirement(SecurityTokenRequirement requirement)
        {
            if (requirement != null && requirement.Properties.ContainsKey(ServiceModelSecurityTokenRequirement.IssuerAddressProperty))
            {
                // handle all issued token requirements except for spnego, tlsnego and secure conversation
                if (requirement.TokenType == ServiceModelSecurityTokenTypes.AnonymousSslnego || requirement.TokenType == ServiceModelSecurityTokenTypes.MutualSslnego
                    || requirement.TokenType == ServiceModelSecurityTokenTypes.SecureConversation || requirement.TokenType == ServiceModelSecurityTokenTypes.Spnego)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public override SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement)
        {
            if (tokenRequirement == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenRequirement");
            }

            SecurityTokenProvider result = null;
            if (tokenRequirement is RecipientServiceModelSecurityTokenRequirement && tokenRequirement.TokenType == SecurityTokenTypes.X509Certificate && tokenRequirement.KeyUsage == SecurityKeyUsage.Exchange)
            {
                // this is the uncorrelated duplex case
                if (_parent.ClientCertificate.Certificate == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ClientCertificateNotProvidedOnClientCredentials)));
                }
                result = new X509SecurityTokenProvider(_parent.ClientCertificate.Certificate, _parent.ClientCertificate.CloneCertificate);
            }
            else if (tokenRequirement is InitiatorServiceModelSecurityTokenRequirement)
            {
                InitiatorServiceModelSecurityTokenRequirement initiatorRequirement = tokenRequirement as InitiatorServiceModelSecurityTokenRequirement;
                string tokenType = initiatorRequirement.TokenType;
                if (IsIssuedSecurityTokenRequirement(initiatorRequirement))
                {
                    throw ExceptionHelper.PlatformNotSupported("CreateSecurityTokenProvider (IsIssuedSecurityTokenRequirement(initiatorRequirement)");
                }
                else if (tokenType == SecurityTokenTypes.X509Certificate)
                {
                    if (initiatorRequirement.Properties.ContainsKey(SecurityTokenRequirement.KeyUsageProperty) && initiatorRequirement.KeyUsage == SecurityKeyUsage.Exchange)
                    {
                        throw ExceptionHelper.PlatformNotSupported("CreateSecurityTokenProvider X509Certificate - SecurityKeyUsage.Exchange");
                    }
                    else
                    {
                        if (_parent.ClientCertificate.Certificate == null)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ClientCertificateNotProvidedOnClientCredentials)));
                        }
                        result = new X509SecurityTokenProvider(_parent.ClientCertificate.Certificate, _parent.ClientCertificate.CloneCertificate);
                    }
                }
                else if (tokenType == SecurityTokenTypes.Kerberos)
                {
                    string spn = GetServicePrincipalName(initiatorRequirement);
                    result = new KerberosSecurityTokenProviderWrapper(
                        new KerberosSecurityTokenProvider(spn, _parent.Windows.AllowedImpersonationLevel, SecurityUtils.GetNetworkCredentialOrDefault(_parent.Windows.ClientCredential)));
                }
                else if (tokenType == SecurityTokenTypes.UserName)
                {
                    if (_parent.UserName.UserName == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.UserNamePasswordNotProvidedOnClientCredentials));
                    }
                    result = new UserNameSecurityTokenProvider(_parent.UserName.UserName, _parent.UserName.Password);
                }
                else if (tokenType == ServiceModelSecurityTokenTypes.SspiCredential)
                {
                    if (IsDigestAuthenticationScheme(initiatorRequirement))
                    {
                        result = new SspiSecurityTokenProvider(SecurityUtils.GetNetworkCredentialOrDefault(_parent.HttpDigest.ClientCredential), true, TokenImpersonationLevel.Delegation);
                    }
                    else
                    {
#pragma warning disable 618   // to disable AllowNtlm obsolete wanring.      
                        result = new SspiSecurityTokenProvider(SecurityUtils.GetNetworkCredentialOrDefault(_parent.Windows.ClientCredential),

                            _parent.Windows.AllowNtlm,
                            _parent.Windows.AllowedImpersonationLevel);
#pragma warning restore 618
                    }
                }
            }

            if ((result == null) && !tokenRequirement.IsOptionalToken)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.Format(SR.SecurityTokenManagerCannotCreateProviderForRequirement, tokenRequirement)));
            }

            return result;
        }

        public SecurityTokenSerializer CreateSecurityTokenSerializer(SecurityVersion version)
        {
          if (version == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("version"));
          return this.CreateSecurityTokenSerializer((SecurityTokenVersion) MessageSecurityTokenVersion.GetSecurityTokenVersion(version, true));
        }

#region fromwcf
        public override SecurityTokenSerializer CreateSecurityTokenSerializer(SecurityTokenVersion version)
        {
          if (version == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("version");
          if (this._parent != null && this._parent.UseIdentityConfiguration)
          {  
             throw new NotImplementedException("WrapTokenHandlersAsSecurityTokenSerializer not supported in .NET Core");
             //return this.WrapTokenHandlersAsSecurityTokenSerializer(version);
          }
          MessageSecurityTokenVersion securityTokenVersion = version as MessageSecurityTokenVersion;
          if (securityTokenVersion != null)
          {  
            Console.WriteLine("Using SamlSerializer1 - need to fix.");
            return (SecurityTokenSerializer) new WSSecurityTokenSerializer(securityTokenVersion.SecurityVersion, securityTokenVersion.TrustVersion, securityTokenVersion.SecureConversationVersion, securityTokenVersion.EmitBspRequiredAttributes, (SamlSerializer1) null, (SecurityStateEncoder) null, (IEnumerable<System.Type>) null);
          }
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("SecurityTokenManagerCannotCreateSerializerForVersion", new object[1]
          {
            (object) version
          })));
        }
#endregion

        private X509SecurityTokenAuthenticator CreateServerX509TokenAuthenticator()
        {
            return new X509SecurityTokenAuthenticator(_parent.ServiceCertificate.Authentication.GetCertificateValidator(), false);
        }

        private X509SecurityTokenAuthenticator CreateServerSslX509TokenAuthenticator()
        {
            if (_parent.ServiceCertificate.SslCertificateAuthentication != null)
            {
                return new X509SecurityTokenAuthenticator(_parent.ServiceCertificate.SslCertificateAuthentication.GetCertificateValidator(), false);
            }

            return CreateServerX509TokenAuthenticator();
        }

        public override SecurityTokenAuthenticator CreateSecurityTokenAuthenticator(SecurityTokenRequirement tokenRequirement, out SecurityTokenResolver outOfBandTokenResolver)
        {
            if (tokenRequirement == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenRequirement");
            }

            outOfBandTokenResolver = null;
            SecurityTokenAuthenticator result = null;

            InitiatorServiceModelSecurityTokenRequirement initiatorRequirement = tokenRequirement as InitiatorServiceModelSecurityTokenRequirement;
            if (initiatorRequirement != null)
            {
                string tokenType = initiatorRequirement.TokenType;
                if (IsIssuedSecurityTokenRequirement(initiatorRequirement))
                {
                    throw ExceptionHelper.PlatformNotSupported("CreateSecurityTokenAuthenticator : GenericXmlSecurityTokenAuthenticator");
                }
                else if (tokenType == SecurityTokenTypes.X509Certificate)
                {
                    if (initiatorRequirement.IsOutOfBandToken)
                    {
                        // when the client side soap security asks for a token authenticator, its for doing
                        // identity checks on the out of band server certificate
                        result = new X509SecurityTokenAuthenticator(X509CertificateValidator.None);
                    }
                    else if (initiatorRequirement.PreferSslCertificateAuthenticator)
                    {
                        result = CreateServerSslX509TokenAuthenticator();
                    }
                    else
                    {
                        result = CreateServerX509TokenAuthenticator();
                    }
                }
                else if (tokenType == SecurityTokenTypes.Rsa)
                {
                    throw ExceptionHelper.PlatformNotSupported("CreateSecurityTokenAuthenticator : SecurityTokenTypes.Rsa");
                }
                else if (tokenType == SecurityTokenTypes.Kerberos)
                {
                    throw ExceptionHelper.PlatformNotSupported("CreateSecurityTokenAuthenticator : SecurityTokenTypes.Kerberos");
                }
                else if (tokenType == ServiceModelSecurityTokenTypes.SecureConversation
                    || tokenType == ServiceModelSecurityTokenTypes.MutualSslnego
                    || tokenType == ServiceModelSecurityTokenTypes.AnonymousSslnego
                    || tokenType == ServiceModelSecurityTokenTypes.Spnego)
                {
                    throw ExceptionHelper.PlatformNotSupported("CreateSecurityTokenAuthenticator : GenericXmlSecurityTokenAuthenticator");
                }
            }
            else if ((tokenRequirement is RecipientServiceModelSecurityTokenRequirement) && tokenRequirement.TokenType == SecurityTokenTypes.X509Certificate)
            {
                // uncorrelated duplex case
                result = CreateServerX509TokenAuthenticator();
            }

            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.Format(SR.SecurityTokenManagerCannotCreateAuthenticatorForRequirement, tokenRequirement)));
            }

            return result;
        }
    }

    internal class KerberosSecurityTokenProviderWrapper : CommunicationObjectSecurityTokenProvider
    {
        private KerberosSecurityTokenProvider _innerProvider;

        public KerberosSecurityTokenProviderWrapper(KerberosSecurityTokenProvider innerProvider)
        {
            _innerProvider = innerProvider;
        }

        internal Task<SecurityToken> GetTokenAsync(CancellationToken cancellationToken, ChannelBinding channelbinding)
        {
            return Task.FromResult((SecurityToken)new KerberosRequestorSecurityToken(_innerProvider.ServicePrincipalName,
                _innerProvider.TokenImpersonationLevel, _innerProvider.NetworkCredential,
                SecurityUniqueId.Create().Value));
        }
        protected override Task<SecurityToken> GetTokenCoreAsync(CancellationToken cancellationToken)
        {
            return GetTokenAsync(cancellationToken, null);
        }
    }
}

