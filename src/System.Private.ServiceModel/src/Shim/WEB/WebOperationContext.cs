// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Web.WebOperationContext
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
#if !FEATURE_CORECLR
using System.ServiceModel.Syndication;
#endif
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace System.ServiceModel.Web
{
  /// <summary>A helper class that provides easy access to contextual properties of Web requests and responses.</summary>
  public class WebOperationContext : IExtension<OperationContext>
  {
    internal static readonly string DefaultTextMediaType = "text/plain";
    internal static readonly string DefaultJsonMediaType = "application/json";
    internal static readonly string DefaultXmlMediaType = "application/xml";
    internal static readonly string DefaultAtomMediaType = "application/atom+xml";
    internal static readonly string DefaultStreamMediaType = WebHttpBehavior.defaultStreamContentType;
    private OperationContext operationContext;

    /// <summary>Gets the current Web operation context.</summary>
    /// <returns>A <see cref="T:System.ServiceModel.Web.WebOperationContext" /> instance.</returns>
    public static WebOperationContext Current
    {
      get
      {
        if (OperationContext.Current == null)
          return (WebOperationContext) null;
        return OperationContext.Current.Extensions.Find<WebOperationContext>() ?? new WebOperationContext(OperationContext.Current);
      }
    }

    /// <summary>Gets the Web request context for the request being received.</summary>
    /// <returns>An <see cref="T:System.ServiceModel.Web.IncomingWebRequestContext" /> instance.</returns>
    public IncomingWebRequestContext IncomingRequest
    {
      get
      {
        return new IncomingWebRequestContext(this.operationContext);
      }
    }

    /// <summary>Gets the Web response context for the request being received.</summary>
    /// <returns>An <see cref="T:System.ServiceModel.Web.IncomingWebResponseContext" /> instance.</returns>
    public IncomingWebResponseContext IncomingResponse
    {
      get
      {
        return new IncomingWebResponseContext(this.operationContext);
      }
    }

    /// <summary>Gets the Web request context for the request being sent.</summary>
    /// <returns>An <see cref="T:System.ServiceModel.Web.OutgoingWebRequestContext" /> instance.</returns>
    public OutgoingWebRequestContext OutgoingRequest
    {
      get
      {
        return new OutgoingWebRequestContext(this.operationContext);
      }
    }

    /// <summary>Gets the Web response context for the response being sent.</summary>
    /// <returns>An <see cref="T:System.ServiceModel.Web.OutgoingWebResponseContext" /> instance.</returns>
    public OutgoingWebResponseContext OutgoingResponse
    {
      get
      {
        return new OutgoingWebResponseContext(this.operationContext);
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Web.WebOperationContext" /> class with the specified <see cref="T:System.ServiceModel.OperationContext" /> instance.</summary>
    /// <param name="operationContext">The operation context.</param>
    public WebOperationContext(OperationContext operationContext)
    {
      if (operationContext == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("operationContext");
      this.operationContext = operationContext;
      if (operationContext.Extensions.Find<WebOperationContext>() != null)
        return;
      operationContext.Extensions.Add((IExtension<OperationContext>) this);
    }

    /// <summary>Attaches the current <see cref="T:System.ServiceModel.Web.WebOperationContext" /> instance to the specified <see cref="T:System.ServiceModel.OperationContext" /> instance.</summary>
    /// <param name="owner">The <see cref="T:System.ServiceModel.OperationContext" /> to attach to.</param>
    public void Attach(OperationContext owner)
    {
    }

    /// <summary>Detaches the current <see cref="T:System.ServiceModel.Web.WebOperationContext" /> instance from the specified <see cref="T:System.ServiceModel.OperationContext" /> instance.</summary>
    /// <param name="owner">The <see cref="T:System.ServiceModel.OperationContext" /> to detach from.</param>
    public void Detach(OperationContext owner)
    {
    }

    /// <summary>Creates a JSON formatted message.</summary>
    /// <param name="instance">The object to write to the message.</param>
    /// <typeparam name="T">The type of object to write to the message.</typeparam>
    /// <returns>A JSON formatted message.</returns>
    public Message CreateJsonResponse<T>(T instance)
    {
      DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (T));
      return this.CreateJsonResponse<T>(instance, serializer);
    }

    /// <summary>Creates a JSON formatted message.</summary>
    /// <param name="instance">The object to write to the message.</param>
    /// <param name="serializer">The serializer to use.</param>
    /// <typeparam name="T">The type of object to write to the message.</typeparam>
    /// <returns>A JSON formatted message.</returns>
    public Message CreateJsonResponse<T>(T instance, DataContractJsonSerializer serializer)
    {
      if (serializer == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serializer");
      Message message = Message.CreateMessage(MessageVersion.None, (string) null, (object) instance, (XmlObjectSerializer) serializer);
      message.Properties.Add("WebBodyFormatMessageProperty", (object) WebBodyFormatMessageProperty.JsonProperty);
      this.AddContentType(WebOperationContext.DefaultJsonMediaType, this.OutgoingResponse.BindingWriteEncoding);
      return message;
    }

    /// <summary>Creates an XML formatted message.</summary>
    /// <param name="instance">The object to write to write to the message.</param>
    /// <typeparam name="T">The type of object to write to the message.</typeparam>
    /// <returns>An XML formatted message.</returns>
    public Message CreateXmlResponse<T>(T instance)
    {
      DataContractSerializer contractSerializer = new DataContractSerializer(typeof (T));
      return this.CreateXmlResponse<T>(instance, (XmlObjectSerializer) contractSerializer);
    }

    /// <summary>Creates an XML formatted message.</summary>
    /// <param name="instance">The object to write to the message.</param>
    /// <param name="serializer">The serializer to use.</param>
    /// <typeparam name="T">The type of object to write to the message.</typeparam>
    /// <returns>An XML formatted message.</returns>
    public Message CreateXmlResponse<T>(T instance, XmlObjectSerializer serializer)
    {
      if (serializer == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serializer");
      Message message = Message.CreateMessage(MessageVersion.None, (string) null, (object) instance, serializer);
      message.Properties.Add("WebBodyFormatMessageProperty", (object) WebBodyFormatMessageProperty.XmlProperty);
      this.AddContentType(WebOperationContext.DefaultXmlMediaType, this.OutgoingResponse.BindingWriteEncoding);
      return message;
    }

    /// <summary>Creates an XML formatted message.</summary>
    /// <param name="instance">The object to write to the message.</param>
    /// <param name="serializer">The serializer to use.</param>
    /// <typeparam name="T">The type of object to write to the message.</typeparam>
    /// <returns>An XML formatted message.</returns>
    public Message CreateXmlResponse<T>(T instance, XmlSerializer serializer)
    {
      if (serializer == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serializer");
      Message message = Message.CreateMessage(MessageVersion.None, (string) null, (BodyWriter) new WebOperationContext.XmlSerializerBodyWriter((object) instance, serializer));
      message.Properties.Add("WebBodyFormatMessageProperty", (object) WebBodyFormatMessageProperty.XmlProperty);
      this.AddContentType(WebOperationContext.DefaultXmlMediaType, this.OutgoingResponse.BindingWriteEncoding);
      return message;
    }

    /// <summary>Creates an XML formatted message.</summary>
    /// <param name="document">The data to write to the message.</param>
    /// <returns>An XML formatted message.</returns>
    public Message CreateXmlResponse(XDocument document)
    {
      if (document == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("document");
      Message message = document.FirstNode != null ? Message.CreateMessage(MessageVersion.None, (string) null, document.CreateReader()) : Message.CreateMessage(MessageVersion.None, (string) null);
      message.Properties.Add("WebBodyFormatMessageProperty", (object) WebBodyFormatMessageProperty.XmlProperty);
      this.AddContentType(WebOperationContext.DefaultXmlMediaType, this.OutgoingResponse.BindingWriteEncoding);
      return message;
    }

    /// <summary>Creates an XML formatted message.</summary>
    /// <param name="element">The data to write to the message.</param>
    /// <returns>An XML formatted message.</returns>
    public Message CreateXmlResponse(XElement element)
    {
      if (element == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
      Message message = Message.CreateMessage(MessageVersion.None, (string) null, element.CreateReader());
      message.Properties.Add("WebBodyFormatMessageProperty", (object) WebBodyFormatMessageProperty.XmlProperty);
      this.AddContentType(WebOperationContext.DefaultXmlMediaType, this.OutgoingResponse.BindingWriteEncoding);
      return message;
    }

    /// <summary>Creates a message formatted according to the Atom 1.0 specification with the specified content.</summary>
    /// <param name="item">The content to write to the message.</param>
    /// <returns>A message in Atom 1.0 format.</returns>
#if !FEATURE_CORECLR
    public Message CreateAtom10Response(SyndicationItem item)
    {
      if (item == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
      Message message = Message.CreateMessage(MessageVersion.None, (string) null, (object) item.GetAtom10Formatter());
      message.Properties.Add("WebBodyFormatMessageProperty", (object) WebBodyFormatMessageProperty.XmlProperty);
      this.AddContentType(WebOperationContext.DefaultAtomMediaType, this.OutgoingResponse.BindingWriteEncoding);
      return message;
    }
#endif

    /// <summary>Creates a message formatted according to the Atom 1.0 specification with the specified content.</summary>
    /// <param name="feed">The content to write to the message.</param>
    /// <returns>A message in Atom 1.0 format.</returns>
#if !FEATURE_CORECLR
    public Message CreateAtom10Response(SyndicationFeed feed)
    {
      if (feed == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("feed");
      Message message = Message.CreateMessage(MessageVersion.None, (string) null, (object) feed.GetAtom10Formatter());
      message.Properties.Add("WebBodyFormatMessageProperty", (object) WebBodyFormatMessageProperty.XmlProperty);
      this.AddContentType(WebOperationContext.DefaultAtomMediaType, this.OutgoingResponse.BindingWriteEncoding);
      return message;
    }
#endif

    /// <summary>Creates a message formatted according to the Atom 1.0 specification with the specified content.</summary>
    /// <param name="document">The content to write to the message.</param>
    /// <returns>A message in Atom 1.0 format.</returns>
#if !FEATURE_CORECLR
    public Message CreateAtom10Response(ServiceDocument document)
    {
      if (document == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("document");
      Message message = Message.CreateMessage(MessageVersion.None, (string) null, (object) document.GetFormatter());
      message.Properties.Add("WebBodyFormatMessageProperty", (object) WebBodyFormatMessageProperty.XmlProperty);
      this.AddContentType(WebOperationContext.DefaultAtomMediaType, this.OutgoingResponse.BindingWriteEncoding);
      return message;
    }
#endif

    /// <summary>Creates a text formatted response message.</summary>
    /// <param name="text">The text to write to the message.</param>
    /// <returns>A text formatted message.</returns>
    public Message CreateTextResponse(string text)
    {
      return this.CreateTextResponse(text, WebOperationContext.DefaultTextMediaType, Encoding.UTF8);
    }

    /// <summary>Creates a text formatted message.</summary>
    /// <param name="text">The text to write to the message.</param>
    /// <param name="contentType">The content type of the message.</param>
    /// <returns>A text formatted message.</returns>
    public Message CreateTextResponse(string text, string contentType)
    {
      return this.CreateTextResponse(text, contentType, Encoding.UTF8);
    }

    /// <summary>Creates a text formatted message.</summary>
    /// <param name="text">The text to write to the message.</param>
    /// <param name="contentType">The content type of the message.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>A text formatted message.</returns>
    public Message CreateTextResponse(string text, string contentType, Encoding encoding)
    {
      if (text == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("text");
      if (contentType == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("contentType");
      if (encoding == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("encoding");
      Message message = (Message) new HttpStreamMessage((BodyWriter) StreamBodyWriter.CreateStreamBodyWriter((Action<Stream>) (stream =>
      {
        byte[] preamble = encoding.GetPreamble();
        if (preamble.Length != 0)
          stream.Write(preamble, 0, preamble.Length);
        byte[] bytes = encoding.GetBytes(text);
        stream.Write(bytes, 0, bytes.Length);
        stream.Flush();
      })));
      message.Properties.Add("WebBodyFormatMessageProperty", (object) WebBodyFormatMessageProperty.RawProperty);
      this.AddContentType(contentType, (Encoding) null);
      return message;
    }

    /// <summary>Creates a text formatted message</summary>
    /// <param name="textWriter">A delegate that writes the text data.</param>
    /// <param name="contentType">The content type for the message.</param>
    /// <returns>A text formatted message.</returns>
    public Message CreateTextResponse(Action<TextWriter> textWriter, string contentType)
    {
      Encoding encoding = this.OutgoingResponse.BindingWriteEncoding ?? Encoding.UTF8;
      return this.CreateTextResponse(textWriter, contentType, encoding);
    }

    /// <summary>Creates a text formatted message</summary>
    /// <param name="textWriter">A delegate that writes the text data.</param>
    /// <param name="contentType">The content type of the message.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>A text formatted message.</returns>
    public Message CreateTextResponse(Action<TextWriter> textWriter, string contentType, Encoding encoding)
    {
      if (textWriter == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("textWriter");
      if (contentType == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("contentType");
      if (encoding == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("encoding");
      Message message = (Message) new HttpStreamMessage((BodyWriter) StreamBodyWriter.CreateStreamBodyWriter((Action<Stream>) (stream =>
      {
        using (TextWriter textWriter1 = (TextWriter) new StreamWriter(stream, encoding))
          textWriter(textWriter1);
      })));
      message.Properties.Add("WebBodyFormatMessageProperty", (object) WebBodyFormatMessageProperty.RawProperty);
      this.AddContentType(contentType, (Encoding) null);
      return message;
    }

    /// <summary>Creates a stream formatted message.</summary>
    /// <param name="stream">The stream containing the data to write to the stream.</param>
    /// <param name="contentType">The content type of the message.</param>
    /// <returns>A stream formatted message.</returns>
    public Message CreateStreamResponse(Stream stream, string contentType)
    {
      if (stream == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
      if (contentType == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("contentType");
#if FEATURE_CORECLR
      throw new NotImplementedException("ByteStreamMessage is not supported in .NET Core");
#else
      Message message = ByteStreamMessage.CreateMessage(stream);
      message.Properties.Add("WebBodyFormatMessageProperty", (object) WebBodyFormatMessageProperty.RawProperty);
      this.AddContentType(contentType, (Encoding) null);
      return message;
#endif
    }

    /// <summary>Creates a stream formatted message.</summary>
    /// <param name="bodyWriter">The stream body writer containing the data to write to the message..</param>
    /// <param name="contentType">The content type of the message</param>
    /// <returns>A stream formatted message.</returns>
#if !FEATURE_CORECLR
    public Message CreateStreamResponse(StreamBodyWriter bodyWriter, string contentType)
    {
      if (bodyWriter == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bodyWriter");
      if (contentType == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("contentType");
      Message message = (Message) new HttpStreamMessage((BodyWriter) bodyWriter);
      message.Properties.Add("WebBodyFormatMessageProperty", (object) WebBodyFormatMessageProperty.RawProperty);
      this.AddContentType(contentType, (Encoding) null);
      return message;
    }
#endif

    /// <summary>Creates a stream formatted message.</summary>
    /// <param name="streamWriter">The stream writer containg the data to write to the stream.</param>
    /// <param name="contentType">The content type for the message.</param>
    /// <returns>A stream formatted message.</returns>
    public Message CreateStreamResponse(Action<Stream> streamWriter, string contentType)
    {
      if (streamWriter == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("streamWriter");
      if (contentType == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("contentType");
      Message message = (Message) new HttpStreamMessage((BodyWriter) StreamBodyWriter.CreateStreamBodyWriter(streamWriter));
      message.Properties.Add("WebBodyFormatMessageProperty", (object) WebBodyFormatMessageProperty.RawProperty);
      this.AddContentType(contentType, (Encoding) null);
      return message;
    }

    /// <summary>Gets the URI template associated with the specified operation.</summary>
    /// <param name="operationName">The operation.</param>
    /// <returns>A URI template.</returns>
    public UriTemplate GetUriTemplate(string operationName)
    {
      if (string.IsNullOrEmpty(operationName))
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("operationName");
#if FEATURE_CORECLR
      throw new NotImplementedException("DispatchRuntime.OperationSelector is not supported in .NET Core");
#else
      WebHttpDispatchOperationSelector operationSelector = OperationContext.Current.EndpointDispatcher.DispatchRuntime.OperationSelector as WebHttpDispatchOperationSelector;
      if (operationSelector == null)
      {
        // ISSUE: reference to a compiler-generated method
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR2.GetString(SR2.OperationSelectorNotWebSelector, (object) typeof (WebHttpDispatchOperationSelector))));
      }
      return operationSelector.GetUriTemplate(operationName);
#endif
    }

    private void AddContentType(string contentType, Encoding encoding)
    {
      if (!string.IsNullOrEmpty(this.OutgoingResponse.ContentType))
        return;
      if (encoding != null)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("WebMessageEncoderFactory is not supported in .NET Core");
#else
        contentType = WebMessageEncoderFactory.GetContentType(contentType, encoding);
#endif
      }
      this.OutgoingResponse.ContentType = contentType;
    }

    private class XmlSerializerBodyWriter : BodyWriter
    {
      private object instance;
      private XmlSerializer serializer;

      public XmlSerializerBodyWriter(object instance, XmlSerializer serializer)
        : base(false)
      {
        if (instance == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("instance");
        if (serializer == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serializer");
        this.instance = instance;
        this.serializer = serializer;
      }

      protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
      {
        this.serializer.Serialize((XmlWriter) writer, this.instance);
      }
    }
  }
}
