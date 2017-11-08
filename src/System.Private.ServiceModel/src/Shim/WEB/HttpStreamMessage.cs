// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.HttpStreamMessage
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.Runtime;
using System.Xml;

namespace System.ServiceModel.Channels
{
  internal class HttpStreamMessage : Message
  {
    internal const string StreamElementName = "Binary";
    private BodyWriter bodyWriter;
    private MessageHeaders headers;
    private MessageProperties properties;

    public override MessageHeaders Headers
    {
      get
      {
        if (this.IsDisposed)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateDisposedException());
        return this.headers;
      }
    }

    public override bool IsEmpty
    {
      get
      {
        return false;
      }
    }

    public override bool IsFault
    {
      get
      {
        return false;
      }
    }

    public override MessageProperties Properties
    {
      get
      {
        if (this.IsDisposed)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateDisposedException());
        return this.properties;
      }
    }

    public override MessageVersion Version
    {
      get
      {
        if (this.IsDisposed)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateDisposedException());
        return MessageVersion.None;
      }
    }

    public HttpStreamMessage(BodyWriter writer)
    {
      if (writer == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
      this.bodyWriter = writer;
      this.headers = new MessageHeaders(MessageVersion.None, 1);
      this.properties = new MessageProperties();
    }

    public HttpStreamMessage(MessageHeaders headers, MessageProperties properties, BodyWriter bodyWriter)
    {
      if (bodyWriter == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bodyWriter");
      this.headers = new MessageHeaders(headers);
      this.properties = new MessageProperties(properties);
      this.bodyWriter = bodyWriter;
    }

    protected override void OnBodyToString(XmlDictionaryWriter writer)
    {
      if (this.bodyWriter.IsBuffered)
      {
        this.bodyWriter.WriteBodyContents(writer);
      }
      else
      {
        // ISSUE: reference to a compiler-generated method
        writer.WriteString(SR2.GetString(SR2.MessageBodyIsStream));
      }
    }

    protected override void OnClose()
    {
      Exception exception = (Exception) null;
      try
      {
        base.OnClose();
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          exception = ex;
      }
      try
      {
        if (this.properties != null)
          this.properties.Dispose();
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else if (exception == null)
          exception = ex;
      }
      if (exception != null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
      this.bodyWriter = (BodyWriter) null;
    }

    protected override MessageBuffer OnCreateBufferedCopy(int maxBufferSize)
    {
      return (MessageBuffer) new HttpStreamMessage.HttpStreamMessageBuffer(this.Headers, new MessageProperties(this.Properties), !this.bodyWriter.IsBuffered ? this.bodyWriter.CreateBufferedCopy(maxBufferSize) : this.bodyWriter);
    }

    protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
    {
      this.bodyWriter.WriteBodyContents(writer);
    }

    private Exception CreateDisposedException()
    {
      // ISSUE: reference to a compiler-generated method
      return (Exception) new ObjectDisposedException("", SR2.GetString(SR2.MessageClosed));
    }

    private class HttpStreamMessageBuffer : MessageBuffer
    {
      private object thisLock = new object();
      private BodyWriter bodyWriter;
      private bool closed;
      private MessageHeaders headers;
      private MessageProperties properties;

      public override int BufferSize
      {
        get
        {
          return 0;
        }
      }

      private object ThisLock
      {
        get
        {
          return this.thisLock;
        }
      }

      public HttpStreamMessageBuffer(MessageHeaders headers, MessageProperties properties, BodyWriter bodyWriter)
      {
        this.bodyWriter = bodyWriter;
        this.headers = headers;
        this.properties = properties;
      }

      public override void Close()
      {
        lock (this.ThisLock)
        {
          if (this.closed)
            return;
          this.closed = true;
          this.bodyWriter = (BodyWriter) null;
          this.headers = (MessageHeaders) null;
          this.properties = (MessageProperties) null;
        }
      }

      public override Message CreateMessage()
      {
        lock (this.ThisLock)
        {
          if (this.closed)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateDisposedException());
          return (Message) new HttpStreamMessage(this.headers, this.properties, this.bodyWriter);
        }
      }

      private Exception CreateDisposedException()
      {
        // ISSUE: reference to a compiler-generated method
        return (Exception) new ObjectDisposedException("", SR2.GetString(SR2.MessageBufferIsClosed));
      }
    }
  }
}
