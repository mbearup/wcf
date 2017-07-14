// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Xml;

namespace System.Runtime.Diagnostics
{
    public class StringTraceRecord : TraceRecord
    {
        private string _elementName;
        private string _content;

        public StringTraceRecord(string elementName, string content)
        {
            _elementName = elementName;
            _content = content;
        }

        public override string EventId
        {
            get { return BuildEventId("String"); }
        }

        public override void WriteTo(XmlWriter writer)
        {
            writer.WriteElementString(_elementName, _content);
        }
    }
}
