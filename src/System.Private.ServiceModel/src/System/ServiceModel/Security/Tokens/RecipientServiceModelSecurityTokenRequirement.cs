// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Security.Tokens
{
    public sealed class RecipientServiceModelSecurityTokenRequirement : ServiceModelSecurityTokenRequirement
    {
        public RecipientServiceModelSecurityTokenRequirement()
            : base()
        {
            Properties.Add(IsInitiatorProperty, (object)false);
        }

        public Uri ListenUri
        {
            get
            {
                return GetPropertyOrDefault<Uri>(ListenUriProperty, null);
            }
            set
            {
                this.Properties[ListenUriProperty] = value;
            }
        }

        public override string ToString()
        {
            return InternalToString();
        }
        
        public AuditLogLocation AuditLogLocation
        {
          get
          {
            return this.GetPropertyOrDefault<AuditLogLocation>(ServiceModelSecurityTokenRequirement.AuditLogLocationProperty, AuditLogLocation.Default);
          }
          set
          {
            this.Properties[ServiceModelSecurityTokenRequirement.AuditLogLocationProperty] = (object) value;
          }
        }
        
        public bool SuppressAuditFailure
        {
          get
          {
            return this.GetPropertyOrDefault<bool>(ServiceModelSecurityTokenRequirement.SuppressAuditFailureProperty, true);
          }
          set
          {
            this.Properties[ServiceModelSecurityTokenRequirement.SuppressAuditFailureProperty] = (object) value;
          }
        }
        
        public AuditLevel MessageAuthenticationAuditLevel
        {
          get
          {
            return this.GetPropertyOrDefault<AuditLevel>(ServiceModelSecurityTokenRequirement.MessageAuthenticationAuditLevelProperty, AuditLevel.None);
          }
          set
          {
            this.Properties[ServiceModelSecurityTokenRequirement.MessageAuthenticationAuditLevelProperty] = (object) value;
          }
        }
    }
}
