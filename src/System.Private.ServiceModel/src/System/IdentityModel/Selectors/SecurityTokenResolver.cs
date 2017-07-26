// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.Xml;

namespace System.IdentityModel.Selectors
{
    public abstract class SecurityTokenResolver
    {
        private class SimpleTokenResolver : SecurityTokenResolver
        {
            private ReadOnlyCollection<SecurityToken> _tokens;
            private bool _canMatchLocalId;

            public SimpleTokenResolver(ReadOnlyCollection<SecurityToken> tokens, bool canMatchLocalId)
            {
                if (tokens == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokens");

                _tokens = tokens;
                _canMatchLocalId = canMatchLocalId;
            }
        }

        public SecurityToken ResolveToken(SecurityKeyIdentifier keyIdentifier)
        {
            if (keyIdentifier == null)
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifier");
            SecurityToken token;
            if (!this.TryResolveTokenCore(keyIdentifier, out token))
               throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR.GetString("UnableToResolveTokenReference", new object[1]{ (object) keyIdentifier })));
            return token;
    }

        public static SecurityTokenResolver CreateDefaultSecurityTokenResolver(ReadOnlyCollection<SecurityToken> tokens, bool canMatchLocalId)
        {
            return (SecurityTokenResolver) new SecurityTokenResolver.SimpleTokenResolver(tokens, canMatchLocalId);
        }

        protected virtual bool TryResolveTokenCore(SecurityKeyIdentifier keyIdentifier, out SecurityToken token)
        {
            token = null;
            return false;
        }

        protected virtual bool TryResolveTokenCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityToken token)
        {
            token = null;
            return false;
        }

        protected virtual bool TryResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key)
        {
            key = null;
            return false;
        }

        public bool TryResolveToken(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityToken token)
        {
            if (keyIdentifierClause == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifierClause");
            return this.TryResolveTokenCore(keyIdentifierClause, out token);
        }

    public bool TryResolveToken(SecurityKeyIdentifier keyIdentifier, out SecurityToken token)
    {
      if (keyIdentifier == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifier");
      return this.TryResolveTokenCore(keyIdentifier, out token);
    }

    public bool TryResolveSecurityKey(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key)
    {
      if (keyIdentifierClause == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifierClause");
      return this.TryResolveSecurityKeyCore(keyIdentifierClause, out key);
    }

    }
}
