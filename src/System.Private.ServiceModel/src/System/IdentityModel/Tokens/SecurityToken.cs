// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.ObjectModel;
using System.Runtime;

namespace System.IdentityModel.Tokens
{
    public abstract class SecurityToken
    {
        public abstract string Id { get; }
        public abstract ReadOnlyCollection<SecurityKey> SecurityKeys { get; }
        public abstract DateTime ValidFrom { get; }
        public abstract DateTime ValidTo { get; }

        public virtual bool CanCreateKeyIdentifierClause<T>() where T : SecurityKeyIdentifierClause
        {
          if (typeof (T) == typeof (LocalIdKeyIdentifierClause))
            return this.CanCreateLocalKeyIdentifierClause();
          return false;
        }

        public virtual T CreateKeyIdentifierClause<T>() where T : SecurityKeyIdentifierClause
        {
          if (typeof (T) == typeof (LocalIdKeyIdentifierClause) && this.CanCreateLocalKeyIdentifierClause())
            return new LocalIdKeyIdentifierClause(this.Id, this.GetType()) as T;
          throw new NotSupportedException("TokenDoesNotSupportKeyIdentifierClauseCreation");
        }

        public virtual bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
          LocalIdKeyIdentifierClause identifierClause = keyIdentifierClause as LocalIdKeyIdentifierClause;
          if (identifierClause != null)
            return identifierClause.Matches(this.Id, this.GetType());
          return false;
        }
        
        public virtual SecurityKey ResolveKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
          if (this.SecurityKeys.Count != 0 && this.MatchesKeyIdentifierClause(keyIdentifierClause))
            return this.SecurityKeys[0];
          return (SecurityKey) null;
        }

        private bool CanCreateLocalKeyIdentifierClause()
        {
          return this.Id != null;
        }
    }
}
