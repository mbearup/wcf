// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Globalization;
using System.Text;
#region Needed for SecurityToken and securityTokenRequirement classes
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
#endregion

namespace System.ServiceModel.Security.Tokens
{
    public abstract class SecurityTokenParameters
    {
        internal const bool defaultRequireDerivedKeys = true;

        private bool _requireDerivedKeys = defaultRequireDerivedKeys;

        public SecurityTokenInclusionMode InclusionMode {get; set;}

        protected SecurityTokenParameters(SecurityTokenParameters other)
        {
            if (other == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("other");

            _requireDerivedKeys = other._requireDerivedKeys;
        }

        protected SecurityTokenParameters()
        {
            // empty
        }

        internal protected abstract bool HasAsymmetricKey { get; }

        public bool RequireDerivedKeys
        {
            get
            {
                return _requireDerivedKeys;
            }
            set
            {
                _requireDerivedKeys = value;
            }
        }

        internal protected abstract bool SupportsClientAuthentication { get; }
        internal protected abstract bool SupportsServerAuthentication { get; }
        internal protected abstract bool SupportsClientWindowsIdentity { get; }
#if FEATURE_CORECLR
        public virtual SecurityKeyIdentifierClause CreateGenericXmlTokenKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
        {
            GenericXmlSecurityToken xmlSecurityToken = token as GenericXmlSecurityToken;
            if (xmlSecurityToken != null)
            {
              if (referenceStyle == SecurityTokenReferenceStyle.Internal && xmlSecurityToken.InternalTokenReference != null)
                  return xmlSecurityToken.InternalTokenReference;
              if (referenceStyle == SecurityTokenReferenceStyle.External && xmlSecurityToken.ExternalTokenReference != null)
                  return xmlSecurityToken.ExternalTokenReference;
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException("UnableToCreateTokenReference"));
        }

        protected internal virtual bool MatchesKeyIdentifierClause(SecurityToken token, SecurityKeyIdentifierClause keyIdentifierClause, SecurityTokenReferenceStyle referenceStyle)
        {
            throw new NotImplementedException("MatchesKeyIdentifierClause must be overriden");
        }
#endif

        public SecurityTokenParameters Clone()
        {
            SecurityTokenParameters result = this.CloneCore();

            if (result == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SecurityTokenParametersCloneInvalidResult, this.GetType().ToString())));

            return result;
        }

        protected abstract SecurityTokenParameters CloneCore();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "{0}:", this.GetType().ToString()));
            sb.Append(String.Format(CultureInfo.InvariantCulture, "RequireDerivedKeys: {0}", _requireDerivedKeys.ToString()));

            return sb.ToString();
        }

#region Compatibility with classes that inherit from other versions of SecurityTokenParameters
        protected virtual SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
        {
#if FEATURE_CORECLR
            if (token is GenericXmlSecurityToken)
            {
                return this.CreateGenericXmlTokenKeyIdentifierClause(token, referenceStyle);
            }
            return this.CreateKeyIdentifierClause<SecurityContextKeyIdentifierClause, LocalIdKeyIdentifierClause>(token, referenceStyle);
#else
            throw new NotImplementedException("CreateKeyIdentifierClause is not implemented in SecurityTokenParameters class");
#endif
        }
        
#if FEATURE_CORECLR        
        public virtual void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
#else
        protected virtual void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
#endif
        {
            Console.WriteLine("InitializeSecurityTokenRequirement not supported...");
            throw new NotImplementedException("InitializeSecurityTokenRequirement is not implemented in SecurityTokenParameters class");
        }
#endregion

#if FEATURE_CORECLR
        internal SecurityKeyIdentifierClause CreateKeyIdentifierClause<TExternalClause, TInternalClause>(SecurityToken token, SecurityTokenReferenceStyle referenceStyle) where TExternalClause : SecurityKeyIdentifierClause where TInternalClause : SecurityKeyIdentifierClause
        {
          if (token == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
          SecurityKeyIdentifierClause identifierClause;
          if (referenceStyle != SecurityTokenReferenceStyle.Internal)
          {
            if (referenceStyle != SecurityTokenReferenceStyle.External)
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("TokenDoesNotSupportKeyIdentifierClauseCreation", (object) token.GetType().Name, (object) referenceStyle)));
            identifierClause = (SecurityKeyIdentifierClause) token.CreateKeyIdentifierClause<TExternalClause>();
          }
          else
            identifierClause = (SecurityKeyIdentifierClause) token.CreateKeyIdentifierClause<TInternalClause>();
          return identifierClause;
        }
#endif
    }
}
