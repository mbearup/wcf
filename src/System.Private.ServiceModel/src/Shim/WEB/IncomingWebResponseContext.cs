// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Web.IncomingWebResponseContext
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.Globalization;
using System.Net;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Web
{
  /// <summary>Provides programmatic access to the context of the incoming Web response.</summary>
  public class IncomingWebResponseContext
  {
    private OperationContext operationContext;

    /// <summary>Gets the content length header from the incoming Web response.</summary>
    /// <returns>The content length of the incoming Web response.</returns>
    public long ContentLength
    {
      get
      {
        return long.Parse(this.EnsureMessageProperty().Headers[HttpResponseHeader.ContentLength], (IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    /// <summary>Gets the content type header from the incoming Web response.</summary>
    /// <returns>The content type header of the incoming Web response.</returns>
    public string ContentType
    {
      get
      {
        return this.EnsureMessageProperty().Headers[HttpResponseHeader.ContentType];
      }
    }

    /// <summary>Gets the etag header from the incoming Web response.</summary>
    /// <returns>The etag header of the incoming Web response.</returns>
    public string ETag
    {
      get
      {
        return this.EnsureMessageProperty().Headers[HttpResponseHeader.ETag];
      }
    }

    /// <summary>Gets the headers from the incoming Web response.</summary>
    /// <returns>A <see cref="T:System.Net.WebHeaderCollection" /> instance that contains the headers from the incoming Web response.</returns>
    public WebHeaderCollection Headers
    {
      get
      {
        return this.EnsureMessageProperty().Headers;
      }
    }

    /// <summary>Gets the location header from the incoming Web response.</summary>
    /// <returns>The location header from the incoming Web response.</returns>
    public string Location
    {
      get
      {
        return this.EnsureMessageProperty().Headers[HttpResponseHeader.Location];
      }
    }

    /// <summary>Gets the status code of the incoming Web response.</summary>
    /// <returns>A <see cref="T:System.Net.HttpStatusCode" /> instance that contains the status code of the incoming Web response.</returns>
    public HttpStatusCode StatusCode
    {
      get
      {
        return this.EnsureMessageProperty().StatusCode;
      }
    }

    /// <summary>Gets the status description of the incoming Web response.</summary>
    /// <returns>The status description of the incoming Web response.</returns>
    public string StatusDescription
    {
      get
      {
        return this.EnsureMessageProperty().StatusDescription;
      }
    }

    private HttpResponseMessageProperty MessageProperty
    {
      get
      {
        if (this.operationContext.IncomingMessageProperties == null)
          return (HttpResponseMessageProperty) null;
        if (!this.operationContext.IncomingMessageProperties.ContainsKey(HttpResponseMessageProperty.Name))
          return (HttpResponseMessageProperty) null;
        return this.operationContext.IncomingMessageProperties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
      }
    }

    internal IncomingWebResponseContext(OperationContext operationContext)
    {
      this.operationContext = operationContext;
    }

    private HttpResponseMessageProperty EnsureMessageProperty()
    {
      if (this.MessageProperty == null)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.HttpContextNoIncomingMessageProperty, (object) typeof (HttpResponseMessageProperty).Name)));
      }
      return this.MessageProperty;
    }
  }
}
