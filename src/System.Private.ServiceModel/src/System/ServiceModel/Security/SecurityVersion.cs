// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

using ISignatureValueSecurityElement = System.IdentityModel.ISignatureValueSecurityElement;

namespace System.ServiceModel.Security
{
    public abstract class SecurityVersion
    {
        private readonly XmlDictionaryString _headerName;
        private readonly XmlDictionaryString _headerNamespace;
        private readonly XmlDictionaryString _headerPrefix;

        internal SecurityVersion(XmlDictionaryString headerName, XmlDictionaryString headerNamespace, XmlDictionaryString headerPrefix)
        {
            _headerName = headerName;
            _headerNamespace = headerNamespace;
            _headerPrefix = headerPrefix;
        }

        public XmlDictionaryString HeaderName
        {
            get { return _headerName; }
        }

        public XmlDictionaryString HeaderNamespace
        {
            get { return _headerNamespace; }
        }

        internal XmlDictionaryString HeaderPrefix
        {
            get { return _headerPrefix; }
        }

        public abstract XmlDictionaryString FailedAuthenticationFaultCode
        {
            get;
        }

        public abstract XmlDictionaryString InvalidSecurityTokenFaultCode
        {
            get;
        }

        public abstract XmlDictionaryString InvalidSecurityFaultCode
        {
            get;
        }

        internal virtual bool SupportsSignatureConfirmation
        {
            get { return false; }
        }

        public static SecurityVersion WSSecurity10
        {
            get { return SecurityVersion10.Instance; }
        }

        public static SecurityVersion WSSecurity11
        {
            get { return SecurityVersion11.Instance; }
        }

        public static SecurityVersion Default
        {
            get { return WSSecurity11; }
        }

        internal abstract ReceiveSecurityHeader CreateReceiveSecurityHeader(Message message,
            string actor, bool mustUnderstand, bool relay,
            SecurityStandardsManager standardsManager,
            SecurityAlgorithmSuite algorithmSuite,
            MessageDirection direction,
            int headerIndex);

        public abstract SendSecurityHeader CreateSendSecurityHeader(Message message,
            string actor, bool mustUnderstand, bool relay,
            SecurityStandardsManager standardsManager,
            SecurityAlgorithmSuite algorithmSuite,
            MessageDirection direction);

        public bool DoesMessageContainSecurityHeader(Message message)
        {
            return message.Headers.FindHeader(this.HeaderName.Value, this.HeaderNamespace.Value) >= 0;
        }

        public int FindIndexOfSecurityHeader(Message message, string[] actors)
        {
            return message.Headers.FindHeader(this.HeaderName.Value, this.HeaderNamespace.Value, actors);
        }

        public virtual bool IsReaderAtSignatureConfirmation(XmlDictionaryReader reader)
        {
            return false;
        }

        internal virtual ISignatureValueSecurityElement ReadSignatureConfirmation(XmlDictionaryReader reader)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                SR.SignatureConfirmationNotSupported));
        }

        // The security always look for Empty soap role.  If not found, we will also look for Ultimate actors (next incl).
        // In the future, till we support intermediary scenario, we should refactor this api to do not take actor parameter.
        public ReceiveSecurityHeader TryCreateReceiveSecurityHeader(Message message,
            string actor,
            SecurityStandardsManager standardsManager,
            SecurityAlgorithmSuite algorithmSuite, MessageDirection direction)
        {
            int headerIndex = message.Headers.FindHeader(this.HeaderName.Value, this.HeaderNamespace.Value, actor);
            if (headerIndex < 0 && String.IsNullOrEmpty(actor))
            {
                headerIndex = message.Headers.FindHeader(this.HeaderName.Value, this.HeaderNamespace.Value, message.Version.Envelope.UltimateDestinationActorValues);
            }

            if (headerIndex < 0)
            {
                return null;
            }
            MessageHeaderInfo headerInfo = message.Headers[headerIndex];
            return CreateReceiveSecurityHeader(message,
                headerInfo.Actor, headerInfo.MustUnderstand, headerInfo.Relay,
                standardsManager, algorithmSuite,
                direction, headerIndex);
        }

        internal virtual void WriteSignatureConfirmation(XmlDictionaryWriter writer, string id, byte[] signatureConfirmation)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                SR.SignatureConfirmationNotSupported));
        }

        public void WriteStartHeader(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement(this.HeaderPrefix.Value, this.HeaderName, this.HeaderNamespace);
        }

        internal class SecurityVersion10 : SecurityVersion
        {
            private static readonly SecurityVersion10 s_instance = new SecurityVersion10();

            protected SecurityVersion10()
                : base(XD.SecurityJan2004Dictionary.Security, XD.SecurityJan2004Dictionary.Namespace, XD.SecurityJan2004Dictionary.Prefix)
            {
            }

            public static SecurityVersion10 Instance
            {
                get { return s_instance; }
            }

            public override XmlDictionaryString FailedAuthenticationFaultCode
            {
                get { return XD.SecurityJan2004Dictionary.FailedAuthenticationFaultCode; }
            }

            public override XmlDictionaryString InvalidSecurityTokenFaultCode
            {
                get { return XD.SecurityJan2004Dictionary.InvalidSecurityTokenFaultCode; }
            }

            public override XmlDictionaryString InvalidSecurityFaultCode
            {
                get { return XD.SecurityJan2004Dictionary.InvalidSecurityFaultCode; }
            }

            public override SendSecurityHeader CreateSendSecurityHeader(Message message,
                string actor, bool mustUnderstand, bool relay,
                SecurityStandardsManager standardsManager,
                SecurityAlgorithmSuite algorithmSuite,
                MessageDirection direction)
            {
                return new WSSecurityOneDotZeroSendSecurityHeader(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, direction);
            }

            internal override ReceiveSecurityHeader CreateReceiveSecurityHeader(Message message,
                string actor, bool mustUnderstand, bool relay,
                SecurityStandardsManager standardsManager,
                SecurityAlgorithmSuite algorithmSuite,
                MessageDirection direction,
                int headerIndex)
            {
                return new WSSecurityOneDotZeroReceiveSecurityHeader(
                    message,
                    actor, mustUnderstand, relay,
                    standardsManager,
                    algorithmSuite, headerIndex, direction);
            }

            public override string ToString()
            {
                return "WSSecurity10";
            }
        }

        internal sealed class SecurityVersion11 : SecurityVersion10
        {
            private static readonly SecurityVersion11 s_instance = new SecurityVersion11();

            private SecurityVersion11()
                : base()
            {
            }

            public new static SecurityVersion11 Instance
            {
                get { return s_instance; }
            }

            internal override bool SupportsSignatureConfirmation
            {
                get { return true; }
            }

            internal override ReceiveSecurityHeader CreateReceiveSecurityHeader(Message message,
                string actor, bool mustUnderstand, bool relay,
                SecurityStandardsManager standardsManager,
                SecurityAlgorithmSuite algorithmSuite,
                MessageDirection direction,
                int headerIndex)
            {
                return new WSSecurityOneDotOneReceiveSecurityHeader(
                    message,
                    actor, mustUnderstand, relay,
                    standardsManager,
                    algorithmSuite, headerIndex, direction);
            }

            public override SendSecurityHeader CreateSendSecurityHeader(Message message,
                string actor, bool mustUnderstand, bool relay,
                SecurityStandardsManager standardsManager,
                SecurityAlgorithmSuite algorithmSuite, MessageDirection direction)
            {
                return new WSSecurityOneDotOneSendSecurityHeader(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, direction);
            }

            public override bool IsReaderAtSignatureConfirmation(XmlDictionaryReader reader)
            {
                return reader.IsStartElement(XD.SecurityXXX2005Dictionary.SignatureConfirmation, XD.SecurityXXX2005Dictionary.Namespace);
            }

            internal override ISignatureValueSecurityElement ReadSignatureConfirmation(XmlDictionaryReader reader)
            {
                reader.MoveToStartElement(XD.SecurityXXX2005Dictionary.SignatureConfirmation, XD.SecurityXXX2005Dictionary.Namespace);
                bool isEmptyElement = reader.IsEmptyElement;
                string id = XmlHelper.GetRequiredNonEmptyAttribute(reader, XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace);
                byte[] signatureValue = XmlHelper.GetRequiredBase64Attribute(reader, XD.SecurityXXX2005Dictionary.ValueAttribute, null);
                reader.ReadStartElement();
                if (!isEmptyElement)
                {
                    reader.ReadEndElement();
                }
                return new SignatureConfirmationElement(id, signatureValue, this);
            }

            internal override void WriteSignatureConfirmation(XmlDictionaryWriter writer, string id, byte[] signature)
            {
                if (id == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("id");
                }
                if (signature == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("signature");
                }
                writer.WriteStartElement(XD.SecurityXXX2005Dictionary.Prefix.Value, XD.SecurityXXX2005Dictionary.SignatureConfirmation, XD.SecurityXXX2005Dictionary.Namespace);
                writer.WriteAttributeString(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace, id);
                writer.WriteStartAttribute(XD.SecurityXXX2005Dictionary.ValueAttribute, null);
                writer.WriteBase64(signature, 0, signature.Length);
                writer.WriteEndAttribute();
                writer.WriteEndElement();
            }

            public override string ToString()
            {
                return "WSSecurity11";
            }
        }
    }
}
