// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace System.ServiceModel.Security
{
    internal class WSSecurityOneDotZeroSendSecurityHeader : SendSecurityHeader
    {
        public WSSecurityOneDotZeroSendSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay,
            SecurityStandardsManager standardsManager,
            SecurityAlgorithmSuite algorithmSuite,
            MessageDirection direction)
            : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, direction)
        {
        }

        internal override void ApplyBodySecurity(XmlDictionaryWriter writer, IPrefixGenerator prefixGenerator)
        {
            throw new NotImplementedException("ApplyBodySecurity is not supported in .NET Core");
        }

        internal override void ApplySecurityAndWriteHeaders(MessageHeaders headers, XmlDictionaryWriter writer, IPrefixGenerator prefixGenerator)
        {
            throw new NotImplementedException("ApplySecurityAndWriteHeaders is not supported in .NET Core");
        }

    }
}
