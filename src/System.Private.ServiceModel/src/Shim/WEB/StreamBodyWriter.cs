// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.StreamBodyWriter
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.IO;
using System.Xml;

namespace System.ServiceModel.Channels
{
  /// <summary>An abstract base class used to create custom <see cref="T:System.ServiceModel.Channels.BodyWriter" /> classes that can be used to a message body as a stream.</summary>
  public abstract class StreamBodyWriter : BodyWriter
  {
    private readonly bool isQuirkedTo40Behavior;

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Channels.StreamBodyWriter" /> class.</summary>
    /// <param name="isBuffered">true if the stream is buffered; otherwise false.</param>
    protected StreamBodyWriter(bool isBuffered)
#if FEATURE_CORECLR
      : this(isBuffered, false)
#else
      : this(isBuffered, !OSEnvironmentHelper.IsApplicationTargeting45)
#endif
    {
    }

    internal StreamBodyWriter(bool isBuffered, bool isQuirkedTo40Behavior)
      : base(isBuffered)
    {
      this.isQuirkedTo40Behavior = isQuirkedTo40Behavior;
    }

    internal static StreamBodyWriter CreateStreamBodyWriter(Action<Stream> streamAction)
    {
      if (streamAction == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("actionOfStream");
      return (StreamBodyWriter) new StreamBodyWriter.ActionOfStreamBodyWriter(streamAction);
    }

    /// <summary>Override this method to handle writing the message body contents.</summary>
    /// <param name="stream">The stream to write to.</param>
    protected abstract void OnWriteBodyContents(Stream stream);

    /// <summary>Override this method to create a buffered copy of the stream.</summary>
    /// <param name="maxBufferSize">The maximum buffer size.</param>
    /// <returns>A body writer.</returns>
    protected override BodyWriter OnCreateBufferedCopy(int maxBufferSize)
    {
      using (BufferManagerOutputStream managerOutputStream = new BufferManagerOutputStream(SR2.MaxReceivedMessageSizeExceeded, maxBufferSize))
      {
        this.OnWriteBodyContents((Stream) managerOutputStream);
        int bufferSize;
        return (BodyWriter) new StreamBodyWriter.BufferedBytesStreamBodyWriter(managerOutputStream.ToArray(out bufferSize), bufferSize);
      }
    }

    /// <summary>Override this method to handle writing the message body contents.</summary>
    /// <param name="writer">The writer to write to.</param>
    protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
    {
      using (StreamBodyWriter.XmlWriterBackedStream writerBackedStream = new StreamBodyWriter.XmlWriterBackedStream((XmlWriter) writer, this.isQuirkedTo40Behavior))
        this.OnWriteBodyContents((Stream) writerBackedStream);
    }

    private class XmlWriterBackedStream : Stream
    {
      private const string StreamElementName = "Binary";
      private readonly bool isQuirkedTo40Behavior;
      private XmlWriter writer;

      public override bool CanRead
      {
        get
        {
          return false;
        }
      }

      public override bool CanSeek
      {
        get
        {
          return false;
        }
      }

      public override bool CanWrite
      {
        get
        {
          return true;
        }
      }

      public override long Length
      {
        get
        {
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR2.GetString(SR2.XmlWriterBackedStreamPropertyGetNotSupported, (object) "Length")));
        }
      }

      public override long Position
      {
        get
        {
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR2.GetString(SR2.XmlWriterBackedStreamPropertyGetNotSupported, (object) "Position")));
        }
        set
        {
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR2.GetString(SR2.XmlWriterBackedStreamPropertySetNotSupported, (object) "Position")));
        }
      }

      public XmlWriterBackedStream(XmlWriter writer, bool isQuirkedTo40Behavior)
      {
        if (writer == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
        this.writer = writer;
        this.isQuirkedTo40Behavior = isQuirkedTo40Behavior;
      }

      public override void Flush()
      {
        this.writer.Flush();
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR2.GetString(SR2.XmlWriterBackedStreamMethodNotSupported, (object) "Read")));
      }

      public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR2.GetString(SR2.XmlWriterBackedStreamMethodNotSupported, (object) "BeginRead")));
      }

      public override int EndRead(IAsyncResult asyncResult)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR2.GetString(SR2.XmlWriterBackedStreamMethodNotSupported, (object) "EndRead")));
      }

      public override int ReadByte()
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR2.GetString(SR2.XmlWriterBackedStreamMethodNotSupported, (object) "ReadByte")));
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR2.GetString(SR2.XmlWriterBackedStreamMethodNotSupported, (object) "Seek")));
      }

      public override void SetLength(long value)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR2.GetString(SR2.XmlWriterBackedStreamMethodNotSupported, (object) "SetLength")));
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        if (this.writer.WriteState == WriteState.Content || this.isQuirkedTo40Behavior)
        {
          this.writer.WriteBase64(buffer, offset, count);
        }
        else
        {
          if (this.writer.WriteState != WriteState.Start)
            return;
          this.writer.WriteStartElement("Binary", string.Empty);
          this.writer.WriteBase64(buffer, offset, count);
        }
      }
    }

    private class BufferedBytesStreamBodyWriter : StreamBodyWriter
    {
      private byte[] array;
      private int size;

      public BufferedBytesStreamBodyWriter(byte[] array, int size)
        : base(true, false)
      {
        this.array = array;
        this.size = size;
      }

      protected override void OnWriteBodyContents(Stream stream)
      {
        stream.Write(this.array, 0, this.size);
      }
    }

    private class ActionOfStreamBodyWriter : StreamBodyWriter
    {
      private Action<Stream> actionOfStream;

      public ActionOfStreamBodyWriter(Action<Stream> actionOfStream)
        : base(false, false)
      {
        this.actionOfStream = actionOfStream;
      }

      protected override void OnWriteBodyContents(Stream stream)
      {
        this.actionOfStream(stream);
      }
    }
  }
}
