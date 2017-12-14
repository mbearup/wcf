// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SecurityAppliedMessage
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.IO;
using System.Runtime;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
  internal sealed class SecurityAppliedMessage : DelegatingMessage
  {
    private string bodyPrefix = "s";
    private string bodyId;
    private bool bodyIdInserted;
    private System.ServiceModel.XmlBuffer fullBodyBuffer;
    private ISecurityElement encryptedBodyContent;
    private XmlAttributeHolder[] bodyAttributes;
    private bool delayedApplicationHandled;
    private readonly MessagePartProtectionMode bodyProtectionMode;
    private SecurityAppliedMessage.BodyState state;
    private readonly SendSecurityHeader securityHeader;
    private MemoryStream startBodyFragment;
    private MemoryStream endBodyFragment;
    private byte[] fullBodyFragment;
    private int fullBodyFragmentLength;

    public string BodyId
    {
      get
      {
        return this.bodyId;
      }
    }

    public MessagePartProtectionMode BodyProtectionMode
    {
      get
      {
        return this.bodyProtectionMode;
      }
    }

    internal byte[] PrimarySignatureValue
    {
      get
      {
        return this.securityHeader.PrimarySignatureValue;
      }
    }

    public SecurityAppliedMessage(Message messageToProcess, SendSecurityHeader securityHeader, bool signBody, bool encryptBody)
      : base(messageToProcess)
    {
      this.securityHeader = securityHeader;
#if FEATURE_CORECLR
      // Not implemented
#else
      this.bodyProtectionMode = MessagePartProtectionModeHelper.GetProtectionMode(signBody, encryptBody, securityHeader.SignThenEncrypt);
#endif
    }

    private Exception CreateBadStateException(string operation)
    {
      return (Exception) new InvalidOperationException(SR.GetString("MessageBodyOperationNotValidInBodyState", (object) operation, (object) this.state));
    }

    private void EnsureUniqueSecurityApplication()
    {
      if (this.delayedApplicationHandled)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("DelayedSecurityApplicationAlreadyCompleted")));
      this.delayedApplicationHandled = true;
    }

    protected override void OnBodyToString(XmlDictionaryWriter writer)
    {
      if (this.state == SecurityAppliedMessage.BodyState.Created || this.fullBodyFragment != null)
        base.OnBodyToString(writer);
      else
        this.OnWriteBodyContents(writer);
    }

    protected override void OnClose()
    {
      try
      {
        this.InnerMessage.Close();
      }
      finally
      {
        this.fullBodyBuffer = (System.ServiceModel.XmlBuffer) null;
        this.bodyAttributes = (XmlAttributeHolder[]) null;
        this.encryptedBodyContent = (ISecurityElement) null;
        this.state = SecurityAppliedMessage.BodyState.Disposed;
      }
    }

    protected override void OnWriteStartBody(XmlDictionaryWriter writer)
    {
      if (this.startBodyFragment != null || this.fullBodyFragment != null)
      {
        this.WriteStartInnerMessageWithId(writer);
      }
      else
      {
        switch (this.state)
        {
          case SecurityAppliedMessage.BodyState.Created:
          case SecurityAppliedMessage.BodyState.Encrypted:
            this.InnerMessage.WriteStartBody(writer);
            break;
          case SecurityAppliedMessage.BodyState.Signed:
          case SecurityAppliedMessage.BodyState.EncryptedThenSigned:
            XmlDictionaryReader reader = this.fullBodyBuffer.GetReader(0);
            writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
            writer.WriteAttributes((XmlReader) reader, false);
            reader.Close();
            break;
          case SecurityAppliedMessage.BodyState.SignedThenEncrypted:
#if FEATURE_CORECLR
            // Envelope.DictionaryNamespace not supported
#else
            writer.WriteStartElement(this.bodyPrefix, System.ServiceModel.XD.MessageDictionary.Body, this.Version.Envelope.DictionaryNamespace);
#endif
            if (this.bodyAttributes == null)
              break;
            XmlAttributeHolder.WriteAttributes(this.bodyAttributes, (XmlWriter) writer);
            break;
          default:
            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateBadStateException("OnWriteStartBody"));
        }
      }
    }

    protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
    {
      switch (this.state)
      {
        case SecurityAppliedMessage.BodyState.Created:
          this.InnerMessage.WriteBodyContents(writer);
          break;
        case SecurityAppliedMessage.BodyState.Signed:
        case SecurityAppliedMessage.BodyState.EncryptedThenSigned:
          XmlDictionaryReader reader = this.fullBodyBuffer.GetReader(0);
          reader.ReadStartElement();
          while (reader.NodeType != XmlNodeType.EndElement)
            writer.WriteNode(reader, false);
          reader.ReadEndElement();
          reader.Close();
          break;
        case SecurityAppliedMessage.BodyState.SignedThenEncrypted:
        case SecurityAppliedMessage.BodyState.Encrypted:
#if FEATURE_CORECLR
		  throw new NotImplementedException("ServiceModelDictionaryManager is not implemented in .NET Core");
#else
          this.encryptedBodyContent.WriteTo(writer, ServiceModelDictionaryManager.Instance);
          break;
#endif
        default:
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateBadStateException("OnWriteBodyContents"));
      }
    }

    protected override void OnWriteMessage(XmlDictionaryWriter writer)
    {
      this.AttachChannelBindingTokenIfFound();
      this.EnsureUniqueSecurityApplication();
      SecurityAppliedMessage.MessagePrefixGenerator messagePrefixGenerator = new SecurityAppliedMessage.MessagePrefixGenerator((XmlWriter) writer);
      this.securityHeader.StartSecurityApplication();
      this.Headers.Add((MessageHeader) this.securityHeader);
      this.InnerMessage.WriteStartEnvelope(writer);
      this.Headers.RemoveAt(this.Headers.Count - 1);
      this.securityHeader.ApplyBodySecurity(writer, (IPrefixGenerator) messagePrefixGenerator);
      this.InnerMessage.WriteStartHeaders(writer);
      this.securityHeader.ApplySecurityAndWriteHeaders(this.Headers, writer, (IPrefixGenerator) messagePrefixGenerator);
      this.securityHeader.RemoveSignatureEncryptionIfAppropriate();
      this.securityHeader.CompleteSecurityApplication();
      this.securityHeader.WriteHeader(writer, this.Version);
      writer.WriteEndElement();
      if (this.fullBodyFragment != null)
      {
        ((IFragmentCapableXmlDictionaryWriter) writer).WriteFragment(this.fullBodyFragment, 0, this.fullBodyFragmentLength);
      }
      else
      {
        if (this.startBodyFragment != null)
          ((IFragmentCapableXmlDictionaryWriter) writer).WriteFragment(this.startBodyFragment.GetBuffer(), 0, (int) this.startBodyFragment.Length);
        else
          this.OnWriteStartBody(writer);
        this.OnWriteBodyContents(writer);
        if (this.endBodyFragment != null)
          ((IFragmentCapableXmlDictionaryWriter) writer).WriteFragment(this.endBodyFragment.GetBuffer(), 0, (int) this.endBodyFragment.Length);
        else
          writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }

    private void AttachChannelBindingTokenIfFound()
    {
      ChannelBindingMessageProperty property = (ChannelBindingMessageProperty) null;
      ChannelBindingMessageProperty.TryGet(this.InnerMessage, out property);
      if (property == null || this.securityHeader.ElementContainer == null || this.securityHeader.ElementContainer.EndorsingSupportingTokens == null)
        return;
      foreach (SecurityToken endorsingSupportingToken in this.securityHeader.ElementContainer.EndorsingSupportingTokens)
      {
        ProviderBackedSecurityToken backedSecurityToken = endorsingSupportingToken as ProviderBackedSecurityToken;
        if (backedSecurityToken != null)
          backedSecurityToken.ChannelBinding = property.ChannelBinding;
      }
    }

    private void SetBodyId()
    {
#if FEATURE_CORECLR
	  throw new NotImplementedException("SecurityStandardsManager.IdManager is not supported in .NET Core");
#else
      this.bodyId = this.InnerMessage.GetBodyAttribute("Id", this.securityHeader.StandardsManager.IdManager.DefaultIdNamespaceUri);
      if (this.bodyId != null)
        return;
      this.bodyId = this.securityHeader.GenerateId();
      this.bodyIdInserted = true;
#endif
    }

    public void WriteBodyToEncrypt(EncryptedData encryptedData, SymmetricAlgorithm algorithm)
    {
      encryptedData.Id = this.securityHeader.GenerateId();
      SecurityAppliedMessage.BodyContentHelper bodyContentHelper = new SecurityAppliedMessage.BodyContentHelper();
      this.InnerMessage.WriteBodyContents(bodyContentHelper.CreateWriter());
      encryptedData.SetUpEncryption(algorithm, bodyContentHelper.ExtractResult());
      this.encryptedBodyContent = (ISecurityElement) encryptedData;
      this.state = SecurityAppliedMessage.BodyState.Encrypted;
    }

    public void WriteBodyToEncryptThenSign(Stream canonicalStream, EncryptedData encryptedData, SymmetricAlgorithm algorithm)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("ServiceModelDictionaryManager is not supported in .NET Core");
#else
      encryptedData.Id = this.securityHeader.GenerateId();
      this.SetBodyId();
      XmlDictionaryWriter textWriter = XmlDictionaryWriter.CreateTextWriter(Stream.Null);
      textWriter.WriteStartElement("a");
      MemoryStream memoryStream = new MemoryStream();
      ((IFragmentCapableXmlDictionaryWriter) textWriter).StartFragment((Stream) memoryStream, true);
      this.InnerMessage.WriteBodyContents(textWriter);
      ((IFragmentCapableXmlDictionaryWriter) textWriter).EndFragment();
      textWriter.WriteEndElement();
      memoryStream.Flush();
      encryptedData.SetUpEncryption(algorithm, new ArraySegment<byte>(memoryStream.GetBuffer(), 0, (int) memoryStream.Length));
      this.fullBodyBuffer = new System.ServiceModel.XmlBuffer(int.MaxValue);
      XmlDictionaryWriter writer = this.fullBodyBuffer.OpenSection(XmlDictionaryReaderQuotas.Max);
      writer.StartCanonicalization(canonicalStream, false, (string[]) null);
      this.WriteStartInnerMessageWithId(writer);
      encryptedData.WriteTo(writer, ServiceModelDictionaryManager.Instance);
      writer.WriteEndElement();
      writer.EndCanonicalization();
      writer.Flush();
      this.fullBodyBuffer.CloseSection();
      this.fullBodyBuffer.Close();
      this.state = SecurityAppliedMessage.BodyState.EncryptedThenSigned;
#endif
    }

    public void WriteBodyToSign(Stream canonicalStream)
    {
      this.SetBodyId();
      this.fullBodyBuffer = new System.ServiceModel.XmlBuffer(int.MaxValue);
      XmlDictionaryWriter writer = this.fullBodyBuffer.OpenSection(XmlDictionaryReaderQuotas.Max);
      writer.StartCanonicalization(canonicalStream, false, (string[]) null);
      this.WriteInnerMessageWithId(writer);
      writer.EndCanonicalization();
      writer.Flush();
      this.fullBodyBuffer.CloseSection();
      this.fullBodyBuffer.Close();
      this.state = SecurityAppliedMessage.BodyState.Signed;
    }

    public void WriteBodyToSignThenEncrypt(Stream canonicalStream, EncryptedData encryptedData, SymmetricAlgorithm algorithm)
    {
      System.ServiceModel.XmlBuffer xmlBuffer = new System.ServiceModel.XmlBuffer(int.MaxValue);
      XmlDictionaryWriter writer = xmlBuffer.OpenSection(XmlDictionaryReaderQuotas.Max);
      this.WriteBodyToSignThenEncryptWithFragments(canonicalStream, false, (string[]) null, encryptedData, algorithm, writer);
      ((IFragmentCapableXmlDictionaryWriter) writer).WriteFragment(this.startBodyFragment.GetBuffer(), 0, (int) this.startBodyFragment.Length);
      ((IFragmentCapableXmlDictionaryWriter) writer).WriteFragment(this.endBodyFragment.GetBuffer(), 0, (int) this.endBodyFragment.Length);
      xmlBuffer.CloseSection();
      xmlBuffer.Close();
      this.startBodyFragment = (MemoryStream) null;
      this.endBodyFragment = (MemoryStream) null;
      XmlDictionaryReader reader = xmlBuffer.GetReader(0);
      int content = (int) reader.MoveToContent();
      this.bodyPrefix = reader.Prefix;
      if (reader.HasAttributes)
        this.bodyAttributes = XmlAttributeHolder.ReadAttributes(reader);
      reader.Close();
    }

    public void WriteBodyToSignThenEncryptWithFragments(Stream stream, bool includeComments, string[] inclusivePrefixes, EncryptedData encryptedData, SymmetricAlgorithm algorithm, XmlDictionaryWriter writer)
    {
      IFragmentCapableXmlDictionaryWriter dictionaryWriter = (IFragmentCapableXmlDictionaryWriter) writer;
      this.SetBodyId();
      encryptedData.Id = this.securityHeader.GenerateId();
      this.startBodyFragment = new MemoryStream();
      BufferedOutputStream bufferedOutputStream = (BufferedOutputStream) new System.ServiceModel.Channels.BufferManagerOutputStream("XmlBufferQuotaExceeded", 1024, int.MaxValue, this.securityHeader.StreamBufferManager);
      this.endBodyFragment = new MemoryStream();
      writer.StartCanonicalization(stream, includeComments, inclusivePrefixes);
      dictionaryWriter.StartFragment((Stream) this.startBodyFragment, false);
      this.WriteStartInnerMessageWithId(writer);
      dictionaryWriter.EndFragment();
      dictionaryWriter.StartFragment((Stream) bufferedOutputStream, true);
      this.InnerMessage.WriteBodyContents(writer);
      dictionaryWriter.EndFragment();
      dictionaryWriter.StartFragment((Stream) this.endBodyFragment, false);
      writer.WriteEndElement();
      dictionaryWriter.EndFragment();
      writer.EndCanonicalization();
      int bufferSize;
      byte[] array = bufferedOutputStream.ToArray(out bufferSize);
      encryptedData.SetUpEncryption(algorithm, new ArraySegment<byte>(array, 0, bufferSize));
      this.encryptedBodyContent = (ISecurityElement) encryptedData;
      this.state = SecurityAppliedMessage.BodyState.SignedThenEncrypted;
    }

    public void WriteBodyToSignWithFragments(Stream stream, bool includeComments, string[] inclusivePrefixes, XmlDictionaryWriter writer)
    {
      IFragmentCapableXmlDictionaryWriter dictionaryWriter = (IFragmentCapableXmlDictionaryWriter) writer;
      this.SetBodyId();
      BufferedOutputStream bufferedOutputStream = (BufferedOutputStream) new System.ServiceModel.Channels.BufferManagerOutputStream("XmlBufferQuotaExceeded", 1024, int.MaxValue, this.securityHeader.StreamBufferManager);
      writer.StartCanonicalization(stream, includeComments, inclusivePrefixes);
      dictionaryWriter.StartFragment((Stream) bufferedOutputStream, false);
      this.WriteStartInnerMessageWithId(writer);
      this.InnerMessage.WriteBodyContents(writer);
      writer.WriteEndElement();
      dictionaryWriter.EndFragment();
      writer.EndCanonicalization();
      this.fullBodyFragment = bufferedOutputStream.ToArray(out this.fullBodyFragmentLength);
      this.state = SecurityAppliedMessage.BodyState.Signed;
    }

    private void WriteInnerMessageWithId(XmlDictionaryWriter writer)
    {
      this.WriteStartInnerMessageWithId(writer);
      this.InnerMessage.WriteBodyContents(writer);
      writer.WriteEndElement();
    }

    private void WriteStartInnerMessageWithId(XmlDictionaryWriter writer)
    {
      this.InnerMessage.WriteStartBody(writer);
      if (!this.bodyIdInserted)
        return;
#if FEATURE_CORECLR
	  throw new NotImplementedException("SecurityStandardsManager.IdManager is not supported in .NET Core");
#else
      this.securityHeader.StandardsManager.IdManager.WriteIdAttribute(writer, this.bodyId);
#endif
    }

    private enum BodyState
    {
      Created,
      Signed,
      SignedThenEncrypted,
      EncryptedThenSigned,
      Encrypted,
      Disposed,
    }

    private struct BodyContentHelper
    {
      private MemoryStream stream;
      private XmlDictionaryWriter writer;

      public XmlDictionaryWriter CreateWriter()
      {
        this.stream = new MemoryStream();
        this.writer = XmlDictionaryWriter.CreateTextWriter((Stream) this.stream);
        return this.writer;
      }

      public ArraySegment<byte> ExtractResult()
      {
        this.writer.Flush();
        return new ArraySegment<byte>(this.stream.GetBuffer(), 0, (int) this.stream.Length);
      }
    }

    private sealed class MessagePrefixGenerator : IPrefixGenerator
    {
      private XmlWriter writer;

      public MessagePrefixGenerator(XmlWriter writer)
      {
        this.writer = writer;
      }

      public string GetPrefix(string namespaceUri, int depth, bool isForAttribute)
      {
        return this.writer.LookupPrefix(namespaceUri);
      }
    }
  }
}
