// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Security;
using System.IdentityModel.Selectors;

namespace System.ServiceModel
{
    public abstract class MessageSecurityVersion
    {
        public static MessageSecurityVersion WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11
        {
            get
            {
                return WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11MessageSecurityVersion.Instance;
            }
        }

        public static MessageSecurityVersion WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10
        {
            get
            {
                return WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10MessageSecurityVersion.Instance;
            }
        }

        public static MessageSecurityVersion WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10
        {
            get
            {
                return WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10MessageSecurityVersion.Instance;
            }
        }

        public static MessageSecurityVersion WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12
        {
            get
            {
                return WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12MessageSecurityVersion.Instance;
            }
        }

        public static MessageSecurityVersion WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10
        {
            get
            {
                return WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10MessageSecurityVersion.Instance;
            }
        }

        public static MessageSecurityVersion WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10
        {
            get
            {
                return WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10MessageSecurityVersion.Instance;
            }
        }

        public static MessageSecurityVersion Default
        {
            get
            {
                return WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11MessageSecurityVersion.Instance;
            }
        }

        internal static MessageSecurityVersion WSSXDefault
        {
            get
            {
                return WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12MessageSecurityVersion.Instance;
            }
        }

        internal MessageSecurityVersion() { }

        public SecurityVersion SecurityVersion
        {
            get
            {
                return MessageSecurityTokenVersion.SecurityVersion;
            }
        }

        public TrustVersion TrustVersion
        {
            get
            {
                return MessageSecurityTokenVersion.TrustVersion;
            }
        }

        public SecureConversationVersion SecureConversationVersion
        {
            get
            {
                return MessageSecurityTokenVersion.SecureConversationVersion;
            }
        }

        public SecurityTokenVersion SecurityTokenVersion
        {
            get
            {
                return MessageSecurityTokenVersion;
            }
        }

        public abstract SecurityPolicyVersion SecurityPolicyVersion { get; }
        public abstract BasicSecurityProfileVersion BasicSecurityProfileVersion { get; }
        public abstract MessageSecurityTokenVersion MessageSecurityTokenVersion { get; }

        internal class WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11MessageSecurityVersion : MessageSecurityVersion
        {
            private static MessageSecurityVersion s_instance = new WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11MessageSecurityVersion();

            public static MessageSecurityVersion Instance
            {
                get { return s_instance; }
            }

            public override BasicSecurityProfileVersion BasicSecurityProfileVersion
            {
                get { return null; }
            }

            public override MessageSecurityTokenVersion MessageSecurityTokenVersion
            {
                get { return MessageSecurityTokenVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005; }
            }

            public override SecurityPolicyVersion SecurityPolicyVersion
            {
                get { return SecurityPolicyVersion.WSSecurityPolicy11; }
            }

            public override string ToString()
            {
                return "WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11";
            }
        }

        internal class WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10MessageSecurityVersion : MessageSecurityVersion
        {
            private static MessageSecurityVersion s_instance = new WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10MessageSecurityVersion();

            public static MessageSecurityVersion Instance
            {
                get { return s_instance; }
            }

            public override BasicSecurityProfileVersion BasicSecurityProfileVersion
            {
                get { return BasicSecurityProfileVersion.BasicSecurityProfile10; }
            }

            public override MessageSecurityTokenVersion MessageSecurityTokenVersion
            {
                get { return MessageSecurityTokenVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005BasicSecurityProfile10; }
            }

            public override SecurityPolicyVersion SecurityPolicyVersion
            {
                get { return SecurityPolicyVersion.WSSecurityPolicy11; }
            }

            public override string ToString()
            {
                return "WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10";
            }
        }

        internal class WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10MessageSecurityVersion : MessageSecurityVersion
        {
            private static MessageSecurityVersion s_instance = new WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10MessageSecurityVersion();

            public static MessageSecurityVersion Instance
            {
                get { return s_instance; }
            }

            public override SecurityPolicyVersion SecurityPolicyVersion
            {
                get { return SecurityPolicyVersion.WSSecurityPolicy11; }
            }

            public override BasicSecurityProfileVersion BasicSecurityProfileVersion
            {
                get { return BasicSecurityProfileVersion.BasicSecurityProfile10; }
            }

            public override MessageSecurityTokenVersion MessageSecurityTokenVersion
            {
                get { return MessageSecurityTokenVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005BasicSecurityProfile10; }
            }

            public override string ToString()
            {
                return "WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10";
            }
        }

        internal class WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10MessageSecurityVersion : MessageSecurityVersion
        {
            private static MessageSecurityVersion s_instance = new WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10MessageSecurityVersion();

            public static MessageSecurityVersion Instance
            {
                get { return s_instance; }
            }

            public override SecurityPolicyVersion SecurityPolicyVersion
            {
                get { return SecurityPolicyVersion.WSSecurityPolicy12; }
            }

            public override BasicSecurityProfileVersion BasicSecurityProfileVersion
            {
                get { return null; }
            }

            public override MessageSecurityTokenVersion MessageSecurityTokenVersion
            {
                get { return MessageSecurityTokenVersion.WSSecurity10WSTrust13WSSecureConversation13BasicSecurityProfile10; }
            }

            public override string ToString()
            {
                return "WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10";
            }
        }

        internal class WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12MessageSecurityVersion : MessageSecurityVersion
        {
            private static MessageSecurityVersion s_instance = new WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12MessageSecurityVersion();

            public static MessageSecurityVersion Instance
            {
                get { return s_instance; }
            }

            public override SecurityPolicyVersion SecurityPolicyVersion
            {
                get { return SecurityPolicyVersion.WSSecurityPolicy12; }
            }

            public override BasicSecurityProfileVersion BasicSecurityProfileVersion
            {
                get { return null; }
            }

            public override MessageSecurityTokenVersion MessageSecurityTokenVersion
            {
                get { return MessageSecurityTokenVersion.WSSecurity11WSTrust13WSSecureConversation13; }
            }

            public override string ToString()
            {
                return "WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12";
            }
        }

        internal class WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10MessageSecurityVersion : MessageSecurityVersion
        {
            private static MessageSecurityVersion s_instance = new WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10MessageSecurityVersion();

            public static MessageSecurityVersion Instance
            {
                get { return s_instance; }
            }

            public override SecurityPolicyVersion SecurityPolicyVersion
            {
                get { return SecurityPolicyVersion.WSSecurityPolicy12; }
            }

            public override BasicSecurityProfileVersion BasicSecurityProfileVersion
            {
                get { return null; }
            }

            public override MessageSecurityTokenVersion MessageSecurityTokenVersion
            {
                get { return MessageSecurityTokenVersion.WSSecurity11WSTrust13WSSecureConversation13BasicSecurityProfile10; }
            }

            public override string ToString()
            {
                return "WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10";
            }
        }
    }
}
