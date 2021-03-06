// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel.Channels;
using System.Net;

namespace System.ServiceModel
{
    public sealed class HttpTransportSecurity
    {
        internal const HttpClientCredentialType DefaultClientCredentialType = HttpClientCredentialType.None;
        internal const HttpProxyCredentialType DefaultProxyCredentialType = HttpProxyCredentialType.None;
        internal const string DefaultRealm = HttpTransportDefaults.Realm;

        private HttpClientCredentialType _clientCredentialType;
        private HttpProxyCredentialType _proxyCredentialType;
        private string _realm;
        private ExtendedProtectionPolicy extendedProtectionPolicy = null;

        public HttpTransportSecurity()
        {
            _clientCredentialType = DefaultClientCredentialType;
            _proxyCredentialType = DefaultProxyCredentialType;
            _realm = DefaultRealm;
            extendedProtectionPolicy = ChannelBindingUtility.DefaultPolicy;
        }

#region FromWCF
        public bool ShouldSerializeProxyCredentialType()
        {
            return (uint) this._proxyCredentialType > 0U;
        }

        public bool ShouldSerializeRealm()
        {
            return this._realm != "";
        }

    public ExtendedProtectionPolicy ExtendedProtectionPolicy
    {
      get
      {
        return this.extendedProtectionPolicy;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        // if (value.PolicyEnforcement == PolicyEnforcement.Always && !ExtendedProtectionPolicy.OSSupportsExtendedProtection)
        //   throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new PlatformNotSupportedException(SR.GetString("ExtendedProtectionNotSupported")));
        this.extendedProtectionPolicy = value;
      }
    }
#endregion

        public HttpClientCredentialType ClientCredentialType
        {
            get { return _clientCredentialType; }
            set
            {
                if (!HttpClientCredentialTypeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }

                _clientCredentialType = value;
            }
        }

        public HttpProxyCredentialType ProxyCredentialType
        {
            get { return _proxyCredentialType; }
            set
            {
                if (!HttpProxyCredentialTypeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }

                _proxyCredentialType = value;
            }
        }

        public string Realm
        {
            get { return _realm; }
            set { _realm = value; }
        }


        public void ConfigureTransportProtectionOnly(HttpsTransportBindingElement https)
        {
            DisableAuthentication(https);
            https.RequireClientCertificate = false;
        }

        private void ConfigureAuthentication(HttpTransportBindingElement http)
        {
            http.AuthenticationScheme = HttpClientCredentialTypeHelper.MapToAuthenticationScheme(_clientCredentialType);
            http.Realm = this.Realm;
#region FromWCF
            http.ExtendedProtectionPolicy = this.extendedProtectionPolicy;
#endregion
        }

        private static void ConfigureAuthentication(HttpTransportBindingElement http, HttpTransportSecurity transportSecurity)
        {
            transportSecurity._clientCredentialType = HttpClientCredentialTypeHelper.MapToClientCredentialType(http.AuthenticationScheme);
            transportSecurity.Realm = http.Realm;
#region FromWCF
            transportSecurity.extendedProtectionPolicy = http.ExtendedProtectionPolicy;
#endregion
        }

        private void DisableAuthentication(HttpTransportBindingElement http)
        {
            http.AuthenticationScheme = AuthenticationSchemes.Anonymous;
            http.Realm = DefaultRealm;
#region FromWCF
            http.ExtendedProtectionPolicy = this.extendedProtectionPolicy;
#endregion
            //ExtendedProtectionPolicy is always copied - even for security mode None, Message and TransportWithMessageCredential,
            //because the settings for ExtendedProtectionPolicy are always below the <security><transport> element
            //http.ExtendedProtectionPolicy = this.extendedProtectionPolicy;
        }

        private static bool IsDisabledAuthentication(HttpTransportBindingElement http)
        {
            return http.AuthenticationScheme == AuthenticationSchemes.Anonymous && http.Realm == DefaultRealm;
        }

        public void ConfigureTransportProtectionAndAuthentication(HttpsTransportBindingElement https)
        {
            ConfigureAuthentication(https);
            https.RequireClientCertificate = (_clientCredentialType == HttpClientCredentialType.Certificate);
        }

        public static void ConfigureTransportProtectionAndAuthentication(HttpsTransportBindingElement https, HttpTransportSecurity transportSecurity)
        {
            ConfigureAuthentication(https, transportSecurity);
            if (https.RequireClientCertificate)
                transportSecurity.ClientCredentialType = HttpClientCredentialType.Certificate;
        }

        internal void ConfigureTransportAuthentication(HttpTransportBindingElement http)
        {
            if (_clientCredentialType == HttpClientCredentialType.Certificate)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.CertificateUnsupportedForHttpTransportCredentialOnly));
            }
            ConfigureAuthentication(http);
        }

        internal static bool IsConfiguredTransportAuthentication(HttpTransportBindingElement http, HttpTransportSecurity transportSecurity)
        {
            if (HttpClientCredentialTypeHelper.MapToClientCredentialType(http.AuthenticationScheme) == HttpClientCredentialType.Certificate)
                return false;
            ConfigureAuthentication(http, transportSecurity);
            return true;
        }

        internal void DisableTransportAuthentication(HttpTransportBindingElement http)
        {
            DisableAuthentication(http);
        }

        internal static bool IsDisabledTransportAuthentication(HttpTransportBindingElement http)
        {
            return IsDisabledAuthentication(http);
        }
    }
}
