// Decompiled with JetBrains decompiler
// Type: System.UriTemplateMatch
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;

namespace System
{
  /// <summary>A class that represents the results of a match operation on a <see cref="T:System.UriTemplate" /> instance.</summary>
  [TypeForwardedFrom("System.ServiceModel.Web, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
  public class UriTemplateMatch
  {
    private int wildcardSegmentsStartOffset = -1;
    private Uri baseUri;
    private NameValueCollection boundVariables;
    private object data;
    private NameValueCollection queryParameters;
    private Collection<string> relativePathSegments;
    private Uri requestUri;
    private UriTemplate template;
    private Collection<string> wildcardPathSegments;
    private Uri originalBaseUri;
    private HttpRequestMessageProperty requestProp;

    /// <summary>Gets and sets the base URI for the template match.</summary>
    /// <returns>A <see cref="T:System.Uri" /> instance that represents the base URI for the template match.</returns>
    public Uri BaseUri
    {
      get
      {
        if (this.baseUri == (Uri) null && this.originalBaseUri != (Uri) null)
          this.baseUri = UriTemplate.RewriteUri(this.originalBaseUri, this.requestProp.Headers[HttpRequestHeader.Host]);
        return this.baseUri;
      }
      set
      {
        this.baseUri = value;
        this.originalBaseUri = (Uri) null;
        this.requestProp = (HttpRequestMessageProperty) null;
      }
    }

    /// <summary>Gets the BoundVariables collection for the template match.</summary>
    /// <returns>A <see cref="T:System.Collections.Specialized.NameValueCollection" /> instance that contains template variable values extracted from the URI during the match.</returns>
    public NameValueCollection BoundVariables
    {
      get
      {
        if (this.boundVariables == null)
          this.boundVariables = new NameValueCollection();
        return this.boundVariables;
      }
    }

    /// <summary>Gets and sets the object associated with the <see cref="T:System.UriTemplateMatch" /> instance.</summary>
    /// <returns>An <see cref="T:System.Object" /> instance.</returns>
    public object Data
    {
      get
      {
        return this.data;
      }
      set
      {
        this.data = value;
      }
    }

    /// <summary>Gets a collection of query string parameters and their values.</summary>
    /// <returns>A <see cref="T:System.Collections.Specialized.NameValueCollection" /> instance that contains the query string parameters and their values.</returns>
    public NameValueCollection QueryParameters
    {
      get
      {
        if (this.queryParameters == null)
          this.PopulateQueryParameters();
        return this.queryParameters;
      }
    }

    /// <summary>Gets a collection of relative path segments.</summary>
    /// <returns>A collection of relative path segments.</returns>
    public Collection<string> RelativePathSegments
    {
      get
      {
        if (this.relativePathSegments == null)
          this.relativePathSegments = new Collection<string>();
        return this.relativePathSegments;
      }
    }

    /// <summary>Gets and sets the matched URI.</summary>
    /// <returns>A <see cref="T:System.Uri" /> instance.</returns>
    public Uri RequestUri
    {
      get
      {
        return this.requestUri;
      }
      set
      {
        this.requestUri = value;
      }
    }

    /// <summary>Gets and sets the <see cref="T:System.UriTemplate" /> instance associated with this <see cref="T:System.UriTemplateMatch" /> instance.</summary>
    /// <returns>A <see cref="T:System.UriTemplate" /> instance.</returns>
    public UriTemplate Template
    {
      get
      {
        return this.template;
      }
      set
      {
        this.template = value;
      }
    }

    /// <summary>Gets a collection of path segments that are matched by a wildcard in the URI template.</summary>
    /// <returns>A collection of path segments that are matched by a wildcard in the URI template.</returns>
    public Collection<string> WildcardPathSegments
    {
      get
      {
        if (this.wildcardPathSegments == null)
          this.PopulateWildcardSegments();
        return this.wildcardPathSegments;
      }
    }

    internal void SetQueryParameters(NameValueCollection queryParameters)
    {
      this.queryParameters = new NameValueCollection(queryParameters);
    }

    internal void SetRelativePathSegments(Collection<string> segments)
    {
      this.relativePathSegments = segments;
    }

    internal void SetWildcardPathSegmentsStart(int startOffset)
    {
      this.wildcardSegmentsStartOffset = startOffset;
    }

    internal void SetBaseUri(Uri originalBaseUri, HttpRequestMessageProperty requestProp)
    {
      this.baseUri = (Uri) null;
      this.originalBaseUri = originalBaseUri;
      this.requestProp = requestProp;
    }

    private void PopulateQueryParameters()
    {
      if (this.requestUri != (Uri) null)
        this.queryParameters = UriTemplateHelpers.ParseQueryString(this.requestUri.Query);
      else
        this.queryParameters = new NameValueCollection();
    }

    private void PopulateWildcardSegments()
    {
      if (this.wildcardSegmentsStartOffset != -1)
      {
        this.wildcardPathSegments = new Collection<string>();
        for (int segmentsStartOffset = this.wildcardSegmentsStartOffset; segmentsStartOffset < this.RelativePathSegments.Count; ++segmentsStartOffset)
          this.wildcardPathSegments.Add(this.RelativePathSegments[segmentsStartOffset]);
      }
      else
        this.wildcardPathSegments = new Collection<string>();
    }
  }
}
