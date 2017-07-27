// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security.Tokens;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Xml;
using System.Runtime.CompilerServices;

namespace System.ServiceModel.Security
{
    public class SecurityStandardsManager
    {
#pragma warning disable 0649 // Remove this once we do real implementation, this prevents "field is never assigned to" warning
        private static SecurityStandardsManager s_instance;
        private readonly MessageSecurityVersion _messageSecurityVersion;
        private readonly TrustDriver _trustDriver;
#if FEATURE_CORECLR
        private readonly SecureConversationDriver _secureConversationDriver;
        private readonly SecurityTokenSerializer _tokenSerializer;
        private WSSecurityTokenSerializer wsSecurityTokenSerializer;

        internal SecureConversationDriver SecureConversationDriver
        {
          get
          {
            return this._secureConversationDriver;
          }
        }

#endif
#pragma warning restore 0649


        [MethodImpl(MethodImplOptions.NoInlining)]
        public SecurityStandardsManager()
            : this(WSSecurityTokenSerializer.DefaultInstance)
        {
        }

        public SecurityStandardsManager(SecurityTokenSerializer tokenSerializer)
            : this(MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11, tokenSerializer)
        {
        }

        public SecurityStandardsManager(MessageSecurityVersion messageSecurityVersion, SecurityTokenSerializer tokenSerializer)
        {
#if FEATURE_CORECLR
            throw ExceptionHelper.PlatformNotSupported();
#else
            _tokenSerializer = tokenSerializer;
            _messageSecurityVersion = messageSecurityVersion;
            this._secureConversationDriver = messageSecurityVersion.SecureConversationVersion != SecureConversationVersion.WSSecureConversation13 ? (SecureConversationDriver) new WSSecureConversationFeb2005.DriverFeb2005() : (SecureConversationDriver) new WSSecureConversationDec2005.DriverDec2005();
#endif
        }

        public static SecurityStandardsManager DefaultInstance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new SecurityStandardsManager();
                return s_instance;
            }
        }

#if FEATURE_CORECLR

        private WSSecurityTokenSerializer WSSecurityTokenSerializer
        {
          get
          {
            if (this.wsSecurityTokenSerializer == null)
              this.wsSecurityTokenSerializer = this._tokenSerializer as WSSecurityTokenSerializer ?? new WSSecurityTokenSerializer(this.SecurityVersion);
            return this.wsSecurityTokenSerializer;
          }
        }

        internal SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXml(XmlElement element, SecurityTokenReferenceStyle tokenReferenceStyle)
        {
          return this.WSSecurityTokenSerializer.CreateKeyIdentifierClauseFromTokenXml(element, tokenReferenceStyle);
        }

        internal bool TryCreateKeyIdentifierClauseFromTokenXml(XmlElement element, SecurityTokenReferenceStyle tokenReferenceStyle, out SecurityKeyIdentifierClause securityKeyIdentifierClause)
        {
          return this.WSSecurityTokenSerializer.TryCreateKeyIdentifierClauseFromTokenXml(element, tokenReferenceStyle, out securityKeyIdentifierClause);
        }

        internal SecurityTokenSerializer SecurityTokenSerializer
        {
          get
          {
            return this._tokenSerializer;
          }
        }

        internal TrustVersion TrustVersion
        {
          get
          {
            return this._messageSecurityVersion.TrustVersion;
          }
        }
#endif

        public SecurityVersion SecurityVersion
        {
            get { return _messageSecurityVersion == null ? null : _messageSecurityVersion.SecurityVersion; }
        }

        public MessageSecurityVersion MessageSecurityVersion
        {
            get { return _messageSecurityVersion; }
        }

        internal TrustDriver TrustDriver
        {
            get { return _trustDriver; }
        }
    }
}
