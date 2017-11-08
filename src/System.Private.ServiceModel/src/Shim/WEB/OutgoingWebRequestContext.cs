// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Web.OutgoingWebRequestContext
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.Globalization;
using System.Net;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Web
{
  /// <summary>Provides programmatic access to the context of the outgoing Web request.</summary>
  public class OutgoingWebRequestContext
  {
    private OperationContext operationContext;

    /// <summary>Gets and sets the Accept header value from the outgoing Web request.</summary>
    /// <returns>The Accept header from the outgoing Web request.</returns>
    public string Accept
    {
      get
      {
        return this.MessageProperty.Headers[HttpRequestHeader.Accept];
      }
      set
      {
        this.MessageProperty.Headers[HttpRequestHeader.Accept] = value;
      }
    }

    /// <summary>Gets and sets the content length header value of the outgoing Web request.</summary>
    /// <returns>The content length header of the outgoing Web request.</returns>
    public long ContentLength
    {
      get
      {
        return long.Parse(this.MessageProperty.Headers[HttpRequestHeader.ContentLength], (IFormatProvider) CultureInfo.InvariantCulture);
      }
      set
      {
        this.MessageProperty.Headers[HttpRequestHeader.ContentLength] = value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    /// <summary>Gets and sets the content type header value from the outgoing Web request.</summary>
    /// <returns>The content type header from the outgoing Web request.</returns>
    public string ContentType
    {
      get
      {
        return this.MessageProperty.Headers[HttpRequestHeader.ContentType];
      }
      set
      {
        this.MessageProperty.Headers[HttpRequestHeader.ContentType] = value;
      }
    }

    /// <summary>Gets the headers for the outgoing Web request.</summary>
    /// <returns>A <see cref="T:System.Net.WebHeaderCollection" /> instance that contains the headers of the outgoing Web request.</returns>
    public WebHeaderCollection Headers
    {
      get
      {
        return this.MessageProperty.Headers;
      }
    }

    /// <summary>Gets and sets the IfMatch header value from the outgoing Web request.</summary>
    /// <returns>The IfMatch header from the outgoing Web request.</returns>
    public string IfMatch
    {
      get
      {
        return this.MessageProperty.Headers[HttpRequestHeader.IfMatch];
      }
      set
      {
        this.MessageProperty.Headers[HttpRequestHeader.IfMatch] = value;
      }
    }

    /// <summary>Gets and sets the IfModifiedSince header value from the outgoing Web request.</summary>
    /// <returns>The IfModifiedSince header from the outgoing Web request.</returns>
    public string IfModifiedSince
    {
      get
      {
        return this.MessageProperty.Headers[HttpRequestHeader.IfModifiedSince];
      }
      set
      {
        this.MessageProperty.Headers[HttpRequestHeader.IfModifiedSince] = value;
      }
    }

    /// <summary>Gets and sets the IfNoneMatch header value from the outgoing Web request.</summary>
    /// <returns>The IfNoneMatch header from the outgoing Web request.</returns>
    public string IfNoneMatch
    {
      get
      {
        return this.MessageProperty.Headers[HttpRequestHeader.IfNoneMatch];
      }
      set
      {
        this.MessageProperty.Headers[HttpRequestHeader.IfNoneMatch] = value;
      }
    }

    /// <summary>Gets and sets the IfUnmodifiedSince header value from the outgoing Web request.</summary>
    /// <returns>The IfUnmodifiedSince header from the outgoing Web request.</returns>
    public string IfUnmodifiedSince
    {
      get
      {
        return this.MessageProperty.Headers[HttpRequestHeader.IfUnmodifiedSince];
      }
      set
      {
        this.MessageProperty.Headers[HttpRequestHeader.IfUnmodifiedSince] = value;
      }
    }

    /// <summary>Gets the HTTP method of the outgoing Web request.</summary>
    /// <returns>The HTTP method of the outgoing Web request.</returns>
    public string Method
    {
      get
      {
        return this.MessageProperty.Method;
      }
      set
      {
        this.MessageProperty.Method = value;
      }
    }

    /// <summary>Gets a value that indicates whether Windows Communication Foundation (WCF) omits data that is normally written to the entity body of the response and forces an empty response to be returned.</summary>
    /// <returns>If true, WCF omits any data that is normally written to the entity body of the response and forces an empty response to be returned.</returns>
    public bool SuppressEntityBody
    {
      get
      {
        return this.MessageProperty.SuppressEntityBody;
      }
      set
      {
        this.MessageProperty.SuppressEntityBody = value;
      }
    }

    /// <summary>Gets the user agent header value from the outgoing Web request.</summary>
    /// <returns>The user agent header from the outgoing Web request.</returns>
    public string UserAgent
    {
      get
      {
        return this.MessageProperty.Headers[HttpRequestHeader.UserAgent];
      }
      set
      {
        this.MessageProperty.Headers[HttpRequestHeader.UserAgent] = value;
      }
    }

    private HttpRequestMessageProperty MessageProperty
    {
      get
      {
        if (!this.operationContext.OutgoingMessageProperties.ContainsKey(HttpRequestMessageProperty.Name))
          this.operationContext.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, (object) new HttpRequestMessageProperty());
        return this.operationContext.OutgoingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
      }
    }

    internal OutgoingWebRequestContext(OperationContext operationContext)
    {
      this.operationContext = operationContext;
    }
  }
}
