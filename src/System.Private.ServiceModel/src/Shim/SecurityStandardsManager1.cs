// There are duplicate implementations of SecurityStandardsManager in WCF and WIF
// Downstream applications can use this class to indirectly create a SecureStandardsManager from WCF
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security.Tokens;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Xml;
using System.Runtime.CompilerServices;

namespace System.ServiceModel.Security
{
  public class SecurityStandardsManager1 : SecurityStandardsManager
  {
        public SecurityStandardsManager1()
            : base(WSSecurityTokenSerializer.DefaultInstance)
        { }

        public SecurityStandardsManager1(SecurityTokenSerializer tokenSerializer)
            : base(MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11, tokenSerializer)
        { }

        public SecurityStandardsManager1(MessageSecurityVersion messageSecurityVersion, SecurityTokenSerializer tokenSerializer)
            : base(messageSecurityVersion, tokenSerializer)
        { }
  }
}
