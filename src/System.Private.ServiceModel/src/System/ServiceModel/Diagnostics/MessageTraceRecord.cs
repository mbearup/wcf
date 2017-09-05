// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Diagnostics;
using System.Runtime.Diagnostics;
using System.ServiceModel.Channels;
using System.Xml;

namespace System.ServiceModel.Diagnostics
{
    public class MessageTraceRecord : TraceRecord
    {
        private string message;

        public MessageTraceRecord(string _message)
        {
            this.message = _message;
        }

        public MessageTraceRecord(Message _message)
        {
            this.message = "";
        }


        public MessageTraceRecord()
        {}
    }
}
