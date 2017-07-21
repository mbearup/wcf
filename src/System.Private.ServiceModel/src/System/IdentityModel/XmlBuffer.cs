// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.IO;
using System.Runtime;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace System.IdentityModel
{
    internal class XmlBuffer
    {
#if FEATURE_CORECLR
    private List<XmlBuffer.Section> sections;
    private byte[] buffer;
    private int offset;
    private BufferedOutputStream stream;
    private XmlBuffer.BufferState bufferState;
    private XmlDictionaryWriter writer;
    private XmlDictionaryReaderQuotas quotas;

    public int BufferSize
    {
      get
      {
        return this.buffer.Length;
      }
    }

    public int SectionCount
    {
      get
      {
        return this.sections.Count;
      }
    }

    public XmlBuffer(int maxBufferSize)
    {
      if (maxBufferSize < 0)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("maxBufferSize", (object) maxBufferSize, SR.GetString("ValueMustBeNonNegative")));
      this.stream = (BufferedOutputStream) new BufferManagerOutputStream("XmlBufferQuotaExceeded", Math.Min(512, maxBufferSize), maxBufferSize, BufferManager.CreateBufferManager(0L, int.MaxValue));
      this.sections = new List<XmlBuffer.Section>(1);
    }

    public XmlDictionaryWriter OpenSection(XmlDictionaryReaderQuotas quotas)
    {
      if (this.bufferState != XmlBuffer.BufferState.Created)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateInvalidStateException());
      this.bufferState = XmlBuffer.BufferState.Writing;
      this.quotas = new XmlDictionaryReaderQuotas();
      quotas.CopyTo(this.quotas);
      if (this.writer == null)
        this.writer = XmlDictionaryWriter.CreateBinaryWriter((Stream) this.stream, (IXmlDictionary) XD.Dictionary, (XmlBinaryWriterSession) null, true);
      else
        ((IXmlBinaryWriterInitializer) this.writer).SetOutput((Stream) this.stream, (IXmlDictionary) XD.Dictionary, (XmlBinaryWriterSession) null, true);
      return this.writer;
    }

    public void CloseSection()
    {
      if (this.bufferState != XmlBuffer.BufferState.Writing)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateInvalidStateException());
      this.writer.Close();
      this.bufferState = XmlBuffer.BufferState.Created;
      int size = (int) this.stream.Length - this.offset;
      this.sections.Add(new XmlBuffer.Section(this.offset, size, this.quotas));
      this.offset = this.offset + size;
    }

    public void Close()
    {
      if (this.bufferState != XmlBuffer.BufferState.Created)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateInvalidStateException());
      this.bufferState = XmlBuffer.BufferState.Reading;
      int bufferSize;
      this.buffer = this.stream.ToArray(out bufferSize);
      this.writer = (XmlDictionaryWriter) null;
      this.stream = (BufferedOutputStream) null;
    }

    public XmlDictionaryReader GetReader(int sectionIndex)
    {
      if (this.bufferState != XmlBuffer.BufferState.Reading)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateInvalidStateException());
      XmlBuffer.Section section = this.sections[sectionIndex];
      XmlDictionaryReader binaryReader = XmlDictionaryReader.CreateBinaryReader(this.buffer, section.Offset, section.Size, (IXmlDictionary) XD.Dictionary, section.Quotas, (XmlBinaryReaderSession) null, (OnXmlDictionaryReaderClose) null);
      int content = (int) binaryReader.MoveToContent();
      return binaryReader;
    }

    public void WriteTo(int sectionIndex, XmlWriter writer)
    {
      if (this.bufferState != XmlBuffer.BufferState.Reading)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateInvalidStateException());
      XmlDictionaryReader reader = this.GetReader(sectionIndex);
      try
      {
        writer.WriteNode((XmlReader) reader, false);
      }
      finally
      {
        reader.Close();
      }
    }

#endif
        private enum BufferState
        {
            Created,
            Writing,
            Reading,
        }

        private struct Section
        {
            private int _offset;
            private int _size;
            private XmlDictionaryReaderQuotas _quotas;

            public Section(int offset, int size, XmlDictionaryReaderQuotas quotas)
            {
                _offset = offset;
                _size = size;
                _quotas = quotas;
            }

            public int Offset
            {
                get { return _offset; }
            }

            public int Size
            {
                get { return _size; }
            }

            public XmlDictionaryReaderQuotas Quotas
            {
                get { return _quotas; }
            }
        }


        private Exception CreateInvalidStateException()
        {
            return new InvalidOperationException(SR.XmlBufferInInvalidState);
        }
    }
}
