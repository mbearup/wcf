// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Web.IncomingWebRequestContext
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Web
{
  /// <summary>Provides programmatic access to the context of the incoming Web request.</summary>
  public class IncomingWebRequestContext
  {
    private static readonly string HttpGetMethod = "GET";
    private static readonly string HttpHeadMethod = "HEAD";
    private static readonly string HttpPutMethod = "PUT";
    private static readonly string HttpPostMethod = "POST";
    private static readonly string HttpDeleteMethod = "DELETE";
    private Collection<System.Net.Mime.ContentType> cachedAcceptHeaderElements;
    private string acceptHeaderWhenHeaderElementsCached;
    internal const string UriTemplateMatchResultsPropertyName = "UriTemplateMatchResults";
    private OperationContext operationContext;

    /// <summary>Gets the Accept header value from the incoming Web request.</summary>
    /// <returns>The Accept header from the incoming Web request.</returns>
    public string Accept
    {
      get
      {
        return this.EnsureMessageProperty().Headers[HttpRequestHeader.Accept];
      }
    }

    /// <summary>Gets the ContentLength header value of the incoming Web request.</summary>
    /// <returns>The  ContentLength header of the incoming Web request.</returns>
    public long ContentLength
    {
      get
      {
        return long.Parse(this.EnsureMessageProperty().Headers[HttpRequestHeader.ContentLength], (IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    /// <summary>Gets the ContentType header value from the incoming Web request.</summary>
    /// <returns>The ContentType header from the incoming Web request.</returns>
    public string ContentType
    {
      get
      {
        return this.EnsureMessageProperty().Headers[HttpRequestHeader.ContentType];
      }
    }

    /// <summary>Gets a collection of the items contained in the requests If-Match header.</summary>
    /// <returns>A collection of items contained in the requests If-Match header.</returns>
    public IEnumerable<string> IfMatch
    {
      get
      {
        string header = this.MessageProperty.Headers[HttpRequestHeader.IfMatch];
        if (!string.IsNullOrEmpty(header))
        {
#if FEATURE_CORECLR
          throw new NotImplementedException("Utility is not implemented in .NET Core");
#else
          return (IEnumerable<string>) Utility.QuoteAwareStringSplit(header);
#endif
        }
        return (IEnumerable<string>) null;
      }
    }

    /// <summary>Gets the values contained in the request’s If-None-Match header.</summary>
    /// <returns>The values contained in the request’s If-None-Match header.</returns>
    public IEnumerable<string> IfNoneMatch
    {
      get
      {
        string header = this.MessageProperty.Headers[HttpRequestHeader.IfNoneMatch];
        if (!string.IsNullOrEmpty(header))
        {
#if FEATURE_CORECLR
          throw new NotImplementedException("Utility is not implemented in .NET Core");
#else
          return (IEnumerable<string>) Utility.QuoteAwareStringSplit(header);
#endif
        }
        return (IEnumerable<string>) null;
      }
    }

    /// <summary>Gets the value of the request’s If-Modified-Since header.</summary>
    /// <returns>The request’s If-Modified-Since header value.</returns>
    public DateTime? IfModifiedSince
    {
      get
      {
        string header = this.MessageProperty.Headers[HttpRequestHeader.IfModifiedSince];
        DateTime dtOut;
        if (!string.IsNullOrEmpty(header) && HttpDateParse.ParseHttpDate(header, out dtOut))
          return new DateTime?(dtOut);
        return new DateTime?();
      }
    }

    /// <summary>Gets the value of the request’s If-Unmatched-Since header.</summary>
    /// <returns>The request’s If-Unmatched-Since header..</returns>
    public DateTime? IfUnmodifiedSince
    {
      get
      {
        string header = this.MessageProperty.Headers[HttpRequestHeader.IfUnmodifiedSince];
        DateTime dtOut;
        if (!string.IsNullOrEmpty(header) && HttpDateParse.ParseHttpDate(header, out dtOut))
          return new DateTime?(dtOut);
        return new DateTime?();
      }
    }

    /// <summary>Gets the headers for the incoming Web request.</summary>
    /// <returns>A <see cref="T:System.Net.WebHeaderCollection" /> instance that contains the headers of the incoming Web request.</returns>
    public WebHeaderCollection Headers
    {
      get
      {
        return this.EnsureMessageProperty().Headers;
      }
    }

    /// <summary>Gets the HTTP method of the incoming Web request.</summary>
    /// <returns>The HTTP method of the incoming Web request.</returns>
    public string Method
    {
      get
      {
        return this.EnsureMessageProperty().Method;
      }
    }

    /// <summary>Gets and sets the <see cref="T:System.UriTemplateMatch" /> instance created during the dispatch of the incoming Web request.</summary>
    /// <returns>A <see cref="T:System.UriTemplateMatch" /> instance.</returns>
    public UriTemplateMatch UriTemplateMatch
    {
      get
      {
        if (this.operationContext.IncomingMessageProperties.ContainsKey("UriTemplateMatchResults"))
          return this.operationContext.IncomingMessageProperties["UriTemplateMatchResults"] as UriTemplateMatch;
        return (UriTemplateMatch) null;
      }
      set
      {
        this.operationContext.IncomingMessageProperties["UriTemplateMatchResults"] = (object) value;
      }
    }

    /// <summary>Gets the UserAgent header value from the incoming Web request.</summary>
    /// <returns>The UserAgent header from the incoming Web request.</returns>
    public string UserAgent
    {
      get
      {
        return this.EnsureMessageProperty().Headers[HttpRequestHeader.UserAgent];
      }
    }

    private HttpRequestMessageProperty MessageProperty
    {
      get
      {
        if (this.operationContext.IncomingMessageProperties == null)
          return (HttpRequestMessageProperty) null;
        if (!this.operationContext.IncomingMessageProperties.ContainsKey(HttpRequestMessageProperty.Name))
          return (HttpRequestMessageProperty) null;
        return this.operationContext.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
      }
    }

    internal IncomingWebRequestContext(OperationContext operationContext)
    {
      this.operationContext = operationContext;
    }

    /// <summary>Called when a conditional receive request is made for a resource.</summary>
    /// <param name="entityTag">The entity tag.</param>
    public void CheckConditionalRetrieve(string entityTag)
    {
      this.CheckConditionalRetrieveWithValidatedEtag(OutgoingWebResponseContext.GenerateValidEtagFromString(entityTag));
    }

    /// <summary>Called when a conditional receive request is made for a resource.</summary>
    /// <param name="entityTag">An entity tag.</param>
    public void CheckConditionalRetrieve(int entityTag)
    {
      this.CheckConditionalRetrieveWithValidatedEtag(OutgoingWebResponseContext.GenerateValidEtag((object) entityTag));
    }

    /// <summary>Called when a conditional receive request is made for a resource.</summary>
    /// <param name="entityTag">The entity tag.</param>
    public void CheckConditionalRetrieve(long entityTag)
    {
      this.CheckConditionalRetrieveWithValidatedEtag(OutgoingWebResponseContext.GenerateValidEtag((object) entityTag));
    }

    /// <summary>Called when a conditional receive request is made for a resource.</summary>
    /// <param name="entityTag">An entity tag.</param>
    public void CheckConditionalRetrieve(Guid entityTag)
    {
      this.CheckConditionalRetrieveWithValidatedEtag(OutgoingWebResponseContext.GenerateValidEtag((object) entityTag));
    }

    /// <summary>Called when a conditional receive request is made for a resource.</summary>
    /// <param name="lastModified">The time at which the resource was last modified.</param>
    public void CheckConditionalRetrieve(DateTime lastModified)
    {
      if (!string.Equals(this.Method, IncomingWebRequestContext.HttpGetMethod, StringComparison.OrdinalIgnoreCase) && !string.Equals(this.Method, IncomingWebRequestContext.HttpHeadMethod, StringComparison.OrdinalIgnoreCase))
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.ConditionalRetrieveGetAndHeadOnly, (object) this.Method)));
      }
      DateTime? ifModifiedSince = this.IfModifiedSince;
      if (!ifModifiedSince.HasValue)
        return;
      long ticks1 = lastModified.ToUniversalTime().Ticks;
      DateTime universalTime = ifModifiedSince.Value;
      universalTime = universalTime.ToUniversalTime();
      long ticks2 = universalTime.Ticks;
      if (ticks1 - ticks2 < 10000000L)
      {
        WebOperationContext.Current.OutgoingResponse.LastModified = lastModified;
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new WebFaultException(HttpStatusCode.NotModified));
      }
    }

    /// <summary>Called when a conditional update request is made for a resource.</summary>
    /// <param name="entityTag">The entity tag.</param>
    public void CheckConditionalUpdate(string entityTag)
    {
      this.CheckConditionalUpdateWithValidatedEtag(OutgoingWebResponseContext.GenerateValidEtagFromString(entityTag));
    }

    /// <summary>Called when a conditional update request is made for a resource.</summary>
    /// <param name="entityTag">The entity tag.</param>
    public void CheckConditionalUpdate(int entityTag)
    {
      this.CheckConditionalUpdateWithValidatedEtag(OutgoingWebResponseContext.GenerateValidEtag((object) entityTag));
    }

    /// <summary>Called when a conditional update request is made for a resource.</summary>
    /// <param name="entityTag">The entity tag.</param>
    public void CheckConditionalUpdate(long entityTag)
    {
      this.CheckConditionalUpdateWithValidatedEtag(OutgoingWebResponseContext.GenerateValidEtag((object) entityTag));
    }

    /// <summary>Called when a conditional receive request is made for a resource.</summary>
    /// <param name="entityTag">The entity tag.</param>
    public void CheckConditionalUpdate(Guid entityTag)
    {
      this.CheckConditionalUpdateWithValidatedEtag(OutgoingWebResponseContext.GenerateValidEtag((object) entityTag));
    }

    /// <summary>Gets a collection of the Accept header elements.</summary>
    /// <returns>A collection of the Accept header elements.</returns>
    public Collection<System.Net.Mime.ContentType> GetAcceptHeaderElements()
    {
      string accept = this.Accept;
      if (this.cachedAcceptHeaderElements == null || !string.Equals(this.acceptHeaderWhenHeaderElementsCached, accept, StringComparison.OrdinalIgnoreCase))
      {
        if (string.IsNullOrEmpty(accept))
        {
          this.cachedAcceptHeaderElements = new Collection<System.Net.Mime.ContentType>();
          this.acceptHeaderWhenHeaderElementsCached = accept;
        }
        else
        {
#if FEATURE_CORECLR
          throw new NotImplementedException("Utility is not implemented in .NET Core");
#else
          List<System.Net.Mime.ContentType> contentTypeList = new List<System.Net.Mime.ContentType>();
          int offset = 0;
          while (true)
          {
            System.Net.Mime.ContentType contentTypeOrNull;
            do
            {
              string contentType = Utility.QuoteAwareSubString(accept, ref offset);
              if (contentType != null)
              {
                contentTypeOrNull = Utility.GetContentTypeOrNull(contentType);
              }
              else
                goto label_7;
            }
            while (contentTypeOrNull == null);
            contentTypeList.Add(contentTypeOrNull);
          }
label_7:
          contentTypeList.Sort((IComparer<System.Net.Mime.ContentType>) new AcceptHeaderElementComparer());
          this.cachedAcceptHeaderElements = new Collection<System.Net.Mime.ContentType>((IList<System.Net.Mime.ContentType>) contentTypeList);
          this.acceptHeaderWhenHeaderElementsCached = accept;
#endif
        }
      }
      return this.cachedAcceptHeaderElements;
    }

    private HttpRequestMessageProperty EnsureMessageProperty()
    {
      if (this.MessageProperty == null)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.HttpContextNoIncomingMessageProperty, (object) typeof (HttpRequestMessageProperty).Name)));
      }
      return this.MessageProperty;
    }

    private void CheckConditionalRetrieveWithValidatedEtag(string entityTag)
    {
      if (!string.Equals(this.Method, IncomingWebRequestContext.HttpGetMethod, StringComparison.OrdinalIgnoreCase) && !string.Equals(this.Method, IncomingWebRequestContext.HttpHeadMethod, StringComparison.OrdinalIgnoreCase))
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.ConditionalRetrieveGetAndHeadOnly, (object) this.Method)));
      }
      if (string.IsNullOrEmpty(entityTag))
        return;
      string header = this.Headers[HttpRequestHeader.IfNoneMatch];
      if (!string.IsNullOrEmpty(header) && (IncomingWebRequestContext.IsWildCardCharacter(header) || IncomingWebRequestContext.DoesHeaderContainEtag(header, entityTag)))
      {
        WebOperationContext.Current.OutgoingResponse.ETag = entityTag;
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new WebFaultException(HttpStatusCode.NotModified));
      }
    }

    private void CheckConditionalUpdateWithValidatedEtag(string entityTag)
    {
      bool flag = string.Equals(this.Method, IncomingWebRequestContext.HttpPutMethod, StringComparison.OrdinalIgnoreCase);
      if (!flag && !string.Equals(this.Method, IncomingWebRequestContext.HttpPostMethod, StringComparison.OrdinalIgnoreCase) && !string.Equals(this.Method, IncomingWebRequestContext.HttpDeleteMethod, StringComparison.OrdinalIgnoreCase))
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.ConditionalUpdatePutPostAndDeleteOnly, (object) this.Method)));
      }
      if (flag && string.IsNullOrEmpty(entityTag))
      {
        string header = this.Headers[HttpRequestHeader.IfNoneMatch];
        if (string.IsNullOrEmpty(header) || !IncomingWebRequestContext.IsWildCardCharacter(header))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new WebFaultException(HttpStatusCode.PreconditionFailed));
      }
      else
      {
        string header = this.Headers[HttpRequestHeader.IfMatch];
        if (string.IsNullOrEmpty(header) || !IncomingWebRequestContext.IsWildCardCharacter(header) && !IncomingWebRequestContext.DoesHeaderContainEtag(header, entityTag))
        {
          WebOperationContext.Current.OutgoingResponse.ETag = entityTag;
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new WebFaultException(HttpStatusCode.PreconditionFailed));
        }
      }
    }

    private static bool DoesHeaderContainEtag(string header, string entityTag)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("Utility is not implemented in .NET Core");
#else
      int offset = 0;
      string a;
      do
      {
        a = Utility.QuoteAwareSubString(header, ref offset);
        if (a == null)
          goto label_4;
      }
      while (!string.Equals(a, entityTag, StringComparison.Ordinal));
      return true;
label_4:
      return false;
#endif
    }

    private static bool IsWildCardCharacter(string header)
    {
      return header.Trim() == "*";
    }
  }
}
