// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SecurityVerifiedMessage
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Diagnostics;
using System.IO;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.Xml;

namespace System.ServiceModel.Security
{
  internal sealed class SecurityVerifiedMessage : DelegatingMessage
  {
    private byte[] decryptedBuffer;
    private XmlDictionaryReader cachedDecryptedBodyContentReader;
    private XmlAttributeHolder[] envelopeAttributes;
    private XmlAttributeHolder[] headerAttributes;
    private XmlAttributeHolder[] bodyAttributes;
    private string envelopePrefix;
    private bool bodyDecrypted;
    private SecurityVerifiedMessage.BodyState state;
    private string bodyPrefix;
    private bool isDecryptedBodyStatusDetermined;
    private bool isDecryptedBodyFault;
    private bool isDecryptedBodyEmpty;
    private XmlDictionaryReader cachedReaderAtSecurityHeader;
    private readonly ReceiveSecurityHeader securityHeader;
    private XmlBuffer messageBuffer;
    private bool canDelegateCreateBufferedCopyToInnerMessage;

    public override bool IsEmpty
    {
      get
      {
        if (this.IsDisposed)
          throw TraceUtility.ThrowHelperError(this.CreateMessageDisposedException(), (Message) this);
        if (!this.bodyDecrypted)
          return this.InnerMessage.IsEmpty;
        this.EnsureDecryptedBodyStatusDetermined();
        return this.isDecryptedBodyEmpty;
      }
    }

    public override bool IsFault
    {
      get
      {
        if (this.IsDisposed)
          throw TraceUtility.ThrowHelperError(this.CreateMessageDisposedException(), (Message) this);
        if (!this.bodyDecrypted)
          return this.InnerMessage.IsFault;
        this.EnsureDecryptedBodyStatusDetermined();
        return this.isDecryptedBodyFault;
      }
    }

    internal byte[] PrimarySignatureValue
    {
      get
      {
        return this.securityHeader.PrimarySignatureValue;
      }
    }

    internal ReceiveSecurityHeader ReceivedSecurityHeader
    {
      get
      {
        return this.securityHeader;
      }
    }

    public SecurityVerifiedMessage(Message messageToProcess, ReceiveSecurityHeader securityHeader)
      : base(messageToProcess)
    {
      this.securityHeader = securityHeader;
      if (securityHeader.RequireMessageProtection)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("BufferedMessage not supported in .NET Core");
#else
        BufferedMessage innerMessage = this.InnerMessage as BufferedMessage;
        XmlDictionaryReader reader;
        if (innerMessage != null && this.Headers.ContainsOnlyBufferedMessageHeaders)
        {
          reader = innerMessage.GetMessageReader();
        }
        else
        {
          this.messageBuffer = new XmlBuffer(int.MaxValue);
          this.InnerMessage.WriteMessage(this.messageBuffer.OpenSection(this.securityHeader.ReaderQuotas));
          this.messageBuffer.CloseSection();
          this.messageBuffer.Close();
          reader = this.messageBuffer.GetReader(0);
        }
        this.MoveToSecurityHeader(reader, securityHeader.HeaderIndex, true);
        this.cachedReaderAtSecurityHeader = reader;
        this.state = SecurityVerifiedMessage.BodyState.Buffered;
#endif
      }
      else
      {
        this.envelopeAttributes = XmlAttributeHolder.emptyArray;
        this.headerAttributes = XmlAttributeHolder.emptyArray;
        this.bodyAttributes = XmlAttributeHolder.emptyArray;
        this.canDelegateCreateBufferedCopyToInnerMessage = true;
      }
    }

    private Exception CreateBadStateException(string operation)
    {
      return (Exception) new InvalidOperationException(SR.GetString("MessageBodyOperationNotValidInBodyState", (object) operation, (object) this.state));
    }

    public XmlDictionaryReader CreateFullBodyReader()
    {
      switch (this.state)
      {
        case SecurityVerifiedMessage.BodyState.Buffered:
          return this.CreateFullBodyReaderFromBufferedState();
        case SecurityVerifiedMessage.BodyState.Decrypted:
          return this.CreateFullBodyReaderFromDecryptedState();
        default:
          throw TraceUtility.ThrowHelperError(this.CreateBadStateException("CreateFullBodyReader"), (Message) this);
      }
    }

    private XmlDictionaryReader CreateFullBodyReaderFromBufferedState()
    {
      if (this.messageBuffer == null)
#if FEATURE_CORECLR
        throw new NotImplementedException("BufferedMessage not supported in .NET Core");
#else
        return ((BufferedMessage) this.InnerMessage).GetBufferedReaderAtBody();
#endif
      XmlDictionaryReader reader = this.messageBuffer.GetReader(0);
      this.MoveToBody(reader);
      return reader;
    }

    private XmlDictionaryReader CreateFullBodyReaderFromDecryptedState()
    {
      XmlDictionaryReader textReader = XmlDictionaryReader.CreateTextReader(this.decryptedBuffer, 0, this.decryptedBuffer.Length, this.securityHeader.ReaderQuotas);
      this.MoveToBody(textReader);
      return textReader;
    }

    private void EnsureDecryptedBodyStatusDetermined()
    {
      if (this.isDecryptedBodyStatusDetermined)
        return;
      XmlDictionaryReader fullBodyReader = this.CreateFullBodyReader();
      if (Message.ReadStartBody(fullBodyReader, this.InnerMessage.Version.Envelope, out this.isDecryptedBodyFault, out this.isDecryptedBodyEmpty))
        this.cachedDecryptedBodyContentReader = fullBodyReader;
      else
        fullBodyReader.Close();
      this.isDecryptedBodyStatusDetermined = true;
    }

    public XmlAttributeHolder[] GetEnvelopeAttributes()
    {
      return this.envelopeAttributes;
    }

    public XmlAttributeHolder[] GetHeaderAttributes()
    {
      return this.headerAttributes;
    }

    private XmlDictionaryReader GetReaderAtEnvelope()
    {
      if (this.messageBuffer != null)
        return this.messageBuffer.GetReader(0);
#if FEATURE_CORECLR
      throw new NotImplementedException("Buffered message not supported in .NET Core");
#else
      return ((BufferedMessage) this.InnerMessage).GetMessageReader();
#endif
    }

    public XmlDictionaryReader GetReaderAtFirstHeader()
    {
      XmlDictionaryReader readerAtEnvelope = this.GetReaderAtEnvelope();
      this.MoveToHeaderBlock(readerAtEnvelope, false);
      readerAtEnvelope.ReadStartElement();
      return readerAtEnvelope;
    }

    public XmlDictionaryReader GetReaderAtSecurityHeader()
    {
      if (this.cachedReaderAtSecurityHeader == null)
        return this.Headers.GetReaderAtHeader(this.securityHeader.HeaderIndex);
      XmlDictionaryReader atSecurityHeader = this.cachedReaderAtSecurityHeader;
      this.cachedReaderAtSecurityHeader = (XmlDictionaryReader) null;
      return atSecurityHeader;
    }

    private void MoveToBody(XmlDictionaryReader reader)
    {
      if (reader.NodeType != XmlNodeType.Element)
      {
        int content1 = (int) reader.MoveToContent();
      }
      reader.ReadStartElement();
#if FEATURE_CORECLR
      // Envelope.DictionaryNamespace not supported
#else
      if (reader.IsStartElement(XD.MessageDictionary.Header, this.Version.Envelope.DictionaryNamespace))
        reader.Skip();
#endif
      if (reader.NodeType == XmlNodeType.Element)
        return;
      int content2 = (int) reader.MoveToContent();
    }

    private void MoveToHeaderBlock(XmlDictionaryReader reader, bool captureAttributes)
    {
      if (reader.NodeType != XmlNodeType.Element)
      {
        int content = (int) reader.MoveToContent();
      }
      if (captureAttributes)
      {
        this.envelopePrefix = reader.Prefix;
        this.envelopeAttributes = XmlAttributeHolder.ReadAttributes(reader);
      }
      reader.ReadStartElement();
#if FEATURE_CORECLR
      // Envelope.DictionaryNamespace not supported
#else
      reader.MoveToStartElement(XD.MessageDictionary.Header, this.Version.Envelope.DictionaryNamespace);
#endif
      if (!captureAttributes)
        return;
      this.headerAttributes = XmlAttributeHolder.ReadAttributes(reader);
    }

    private void MoveToSecurityHeader(XmlDictionaryReader reader, int headerIndex, bool captureAttributes)
    {
      this.MoveToHeaderBlock(reader, captureAttributes);
      reader.ReadStartElement();
      while (true)
      {
        if (reader.NodeType != XmlNodeType.Element)
        {
          int content = (int) reader.MoveToContent();
        }
        if (headerIndex != 0)
        {
          reader.Skip();
          --headerIndex;
        }
        else
          break;
      }
    }

    protected override void OnBodyToString(XmlDictionaryWriter writer)
    {
      if (this.state == SecurityVerifiedMessage.BodyState.Created)
        base.OnBodyToString(writer);
      else
        this.OnWriteBodyContents(writer);
    }

    protected override void OnClose()
    {
      if (this.cachedDecryptedBodyContentReader != null)
      {
        try
        {
          this.cachedDecryptedBodyContentReader.Close();
        }
        catch (IOException ex)
        {
          DiagnosticUtility.TraceHandledException((Exception) ex, TraceEventType.Warning);
        }
        finally
        {
          this.cachedDecryptedBodyContentReader = (XmlDictionaryReader) null;
        }
      }
      if (this.cachedReaderAtSecurityHeader != null)
      {
        try
        {
          this.cachedReaderAtSecurityHeader.Close();
        }
        catch (IOException ex)
        {
          DiagnosticUtility.TraceHandledException((Exception) ex, TraceEventType.Warning);
        }
        finally
        {
          this.cachedReaderAtSecurityHeader = (XmlDictionaryReader) null;
        }
      }
      this.messageBuffer = (XmlBuffer) null;
      this.decryptedBuffer = (byte[]) null;
      this.state = SecurityVerifiedMessage.BodyState.Disposed;
      this.InnerMessage.Close();
    }

    protected override XmlDictionaryReader OnGetReaderAtBodyContents()
    {
      if (this.state == SecurityVerifiedMessage.BodyState.Created)
        return this.InnerMessage.GetReaderAtBodyContents();
      if (this.bodyDecrypted)
        this.EnsureDecryptedBodyStatusDetermined();
      if (this.cachedDecryptedBodyContentReader != null)
      {
        XmlDictionaryReader bodyContentReader = this.cachedDecryptedBodyContentReader;
        this.cachedDecryptedBodyContentReader = (XmlDictionaryReader) null;
        return bodyContentReader;
      }
      XmlDictionaryReader fullBodyReader = this.CreateFullBodyReader();
      fullBodyReader.ReadStartElement();
      int content = (int) fullBodyReader.MoveToContent();
      return fullBodyReader;
    }

    protected override MessageBuffer OnCreateBufferedCopy(int maxBufferSize)
    {
#if FEATURE_CORECLR
      // Not implemented
#else
      if (this.canDelegateCreateBufferedCopyToInnerMessage && this.InnerMessage is BufferedMessage)
        return this.InnerMessage.CreateBufferedCopy(maxBufferSize);
#endif
      return base.OnCreateBufferedCopy(maxBufferSize);
    }

    internal void OnMessageProtectionPassComplete(bool atLeastOneHeaderOrBodyEncrypted)
    {
      this.canDelegateCreateBufferedCopyToInnerMessage = !atLeastOneHeaderOrBodyEncrypted;
    }

    internal void OnUnencryptedPart(string name, string ns)
    {
      if (ns == null)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("RequiredMessagePartNotEncrypted", new object[1]{ (object) name })), (Message) this);
      throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("RequiredMessagePartNotEncryptedNs", (object) name, (object) ns)), (Message) this);
    }

    internal void OnUnsignedPart(string name, string ns)
    {
      if (ns == null)
        throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("RequiredMessagePartNotSigned", new object[1]{ (object) name })), (Message) this);
      throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("RequiredMessagePartNotSignedNs", (object) name, (object) ns)), (Message) this);
    }

    protected override void OnWriteStartBody(XmlDictionaryWriter writer)
    {
      if (this.state == SecurityVerifiedMessage.BodyState.Created)
      {
        this.InnerMessage.WriteStartBody(writer);
      }
      else
      {
        XmlDictionaryReader fullBodyReader = this.CreateFullBodyReader();
        int content = (int) fullBodyReader.MoveToContent();
        writer.WriteStartElement(fullBodyReader.Prefix, fullBodyReader.LocalName, fullBodyReader.NamespaceURI);
        writer.WriteAttributes((XmlReader) fullBodyReader, false);
        fullBodyReader.Close();
      }
    }

    protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
    {
      if (this.state == SecurityVerifiedMessage.BodyState.Created)
      {
        this.InnerMessage.WriteBodyContents(writer);
      }
      else
      {
        XmlDictionaryReader fullBodyReader = this.CreateFullBodyReader();
        fullBodyReader.ReadStartElement();
        while (fullBodyReader.NodeType != XmlNodeType.EndElement)
          writer.WriteNode(fullBodyReader, false);
        fullBodyReader.ReadEndElement();
        fullBodyReader.Close();
      }
    }

    public void SetBodyPrefixAndAttributes(XmlDictionaryReader bodyReader)
    {
      this.bodyPrefix = bodyReader.Prefix;
      this.bodyAttributes = XmlAttributeHolder.ReadAttributes(bodyReader);
    }

    public void SetDecryptedBody(byte[] decryptedBodyContent)
    {
      if (this.state != SecurityVerifiedMessage.BodyState.Buffered)
        throw TraceUtility.ThrowHelperError(this.CreateBadStateException("SetDecryptedBody"), (Message) this);
#if FEATURE_CORECLR
      throw new NotImplementedException("ContextImportHelper and Envelope.DictionaryNamespace are not supported in .NET Core");
#else
      MemoryStream memoryStream = new MemoryStream();
      XmlDictionaryWriter textWriter = XmlDictionaryWriter.CreateTextWriter((Stream) memoryStream);
      textWriter.WriteStartElement(this.envelopePrefix, XD.MessageDictionary.Envelope, this.Version.Envelope.DictionaryNamespace);
      textWriter.WriteStartElement(this.bodyPrefix, XD.MessageDictionary.Body, this.Version.Envelope.DictionaryNamespace);
      XmlAttributeHolder.WriteAttributes(this.envelopeAttributes, (XmlWriter) textWriter);
      XmlAttributeHolder.WriteAttributes(this.bodyAttributes, (XmlWriter) textWriter);
      textWriter.WriteString(" ");
      textWriter.WriteEndElement();
      textWriter.WriteEndElement();
      textWriter.Flush();
      this.decryptedBuffer = ContextImportHelper.SpliceBuffers(decryptedBodyContent, memoryStream.GetBuffer(), (int) memoryStream.Length, 2);
      this.bodyDecrypted = true;
      this.state = SecurityVerifiedMessage.BodyState.Decrypted;
#endif
    }

    private enum BodyState
    {
      Created,
      Buffered,
      Decrypted,
      Disposed,
    }
  }
}
