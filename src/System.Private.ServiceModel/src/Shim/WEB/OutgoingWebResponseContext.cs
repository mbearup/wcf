// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Web.OutgoingWebResponseContext
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;

namespace System.ServiceModel.Web
{
  /// <summary>Provides programmatic access to the context of the outgoing Web response.</summary>
  public class OutgoingWebResponseContext
  {
    internal static readonly string WebResponseFormatPropertyName = "WebResponseFormatProperty";
    internal static readonly string AutomatedFormatSelectionContentTypePropertyName = "AutomatedFormatSelectionContentTypePropertyName";
    private Encoding bindingWriteEncoding;
    private OperationContext operationContext;

    /// <summary>Gets and sets the content length header from the outgoing Web response.</summary>
    /// <returns>The content length header of the outgoing Web response.</returns>
    public long ContentLength
    {
      get
      {
        return long.Parse(this.MessageProperty.Headers[HttpResponseHeader.ContentLength], (IFormatProvider) CultureInfo.InvariantCulture);
      }
      set
      {
        this.MessageProperty.Headers[HttpResponseHeader.ContentLength] = value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    /// <summary>Gets and sets the content type header from the outgoing Web response.</summary>
    /// <returns>The content type header of the outgoing Web response.</returns>
    public string ContentType
    {
      get
      {
        return this.MessageProperty.Headers[HttpResponseHeader.ContentType];
      }
      set
      {
        this.MessageProperty.Headers[HttpResponseHeader.ContentType] = value;
      }
    }

    /// <summary>Gets and sets the etag header from the outgoing Web response.</summary>
    /// <returns>The etag header of the outgoing Web response.</returns>
    public string ETag
    {
      get
      {
        return this.MessageProperty.Headers[HttpResponseHeader.ETag];
      }
      set
      {
        this.MessageProperty.Headers[HttpResponseHeader.ETag] = value;
      }
    }

    /// <summary>Gets the headers from the outgoing Web response.</summary>
    /// <returns>A <see cref="T:System.Net.WebHeaderCollection" /> instance that contains the headers from the outgoing Web response.</returns>
    public WebHeaderCollection Headers
    {
      get
      {
        return this.MessageProperty.Headers;
      }
    }

    /// <summary>Gets and sets the last modified header of the outgoing Web response.</summary>
    /// <returns>A <see cref="T:System.DateTime" /> instance that contains the time the requested resource was last modified.</returns>
    public DateTime LastModified
    {
      get
      {
        string header = this.MessageProperty.Headers[HttpRequestHeader.LastModified];
        DateTime result;
        if (!string.IsNullOrEmpty(header) && DateTime.TryParse(header, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
          return result;
        return DateTime.MinValue;
      }
      set
      {
        this.MessageProperty.Headers[HttpResponseHeader.LastModified] = value.Kind == DateTimeKind.Utc ? value.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture) : value.ToUniversalTime().ToString("R", (IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    /// <summary>Gets and sets the location header from the outgoing Web response.</summary>
    /// <returns>The location header from the outgoing Web response.</returns>
    public string Location
    {
      get
      {
        return this.MessageProperty.Headers[HttpResponseHeader.Location];
      }
      set
      {
        this.MessageProperty.Headers[HttpResponseHeader.Location] = value;
      }
    }

    /// <summary>Gets and sets the status code of the outgoing Web response.</summary>
    /// <returns>An <see cref="T:System.Net.HttpStatusCode" /> instance that contains the status code of the outgoing Web response.</returns>
    public HttpStatusCode StatusCode
    {
      get
      {
        return this.MessageProperty.StatusCode;
      }
      set
      {
        this.MessageProperty.StatusCode = value;
      }
    }

    /// <summary>Gets and sets the status description of the outgoing Web response.</summary>
    /// <returns>The status description of the outgoing Web response.</returns>
    public string StatusDescription
    {
      get
      {
        return this.MessageProperty.StatusDescription;
      }
      set
      {
        this.MessageProperty.StatusDescription = value;
      }
    }

    /// <summary>Gets and sets a value that indicates whether Windows Communication Foundation (WCF) omits data that is normally written to the entity body of the response and forces an empty response to be returned.</summary>
    /// <returns>If true, WCF omits any data that is normally written to the entity body of the response and forces an empty response to be returned. The default value is false.</returns>
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

    /// <summary>Gets or sets the web message format</summary>
    /// <returns>The web message format.</returns>
    public WebMessageFormat? Format
    {
      get
      {
        if (!this.operationContext.OutgoingMessageProperties.ContainsKey(OutgoingWebResponseContext.WebResponseFormatPropertyName))
          return new WebMessageFormat?();
        return this.operationContext.OutgoingMessageProperties[OutgoingWebResponseContext.WebResponseFormatPropertyName] as WebMessageFormat?;
      }
      set
      {
        if (value.HasValue)
        {
          if (!WebMessageFormatHelper.IsDefined(value.Value))
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
          this.operationContext.OutgoingMessageProperties[OutgoingWebResponseContext.WebResponseFormatPropertyName] = (object) value.Value;
        }
        else
          this.operationContext.OutgoingMessageProperties[OutgoingWebResponseContext.WebResponseFormatPropertyName] = (object) null;
        this.AutomatedFormatSelectionContentType = (string) null;
      }
    }

    internal string AutomatedFormatSelectionContentType
    {
      get
      {
        if (!this.operationContext.OutgoingMessageProperties.ContainsKey(OutgoingWebResponseContext.AutomatedFormatSelectionContentTypePropertyName))
          return (string) null;
        return this.operationContext.OutgoingMessageProperties[OutgoingWebResponseContext.AutomatedFormatSelectionContentTypePropertyName] as string;
      }
      set
      {
        this.operationContext.OutgoingMessageProperties[OutgoingWebResponseContext.AutomatedFormatSelectionContentTypePropertyName] = (object) value;
      }
    }

    /// <summary>Gets the encoding set on the binding.</summary>
    /// <returns>The encoding.</returns>
    public Encoding BindingWriteEncoding
    {
      get
      {
        if (this.bindingWriteEncoding == null)
        {
#if FEATURE_CORECLR
          throw new NotImplementedException("EndpointDispatcher.Id is not supported in .NET Core");
#else
          string id = this.operationContext.EndpointDispatcher.Id;
          foreach (ServiceEndpoint endpoint in (Collection<ServiceEndpoint>) this.operationContext.Host.Description.Endpoints)
          {
            if (endpoint.Id == id)
            {
              WebMessageEncodingBindingElement encodingBindingElement = endpoint.Binding.CreateBindingElements().Find<WebMessageEncodingBindingElement>();
              if (encodingBindingElement != null)
                this.bindingWriteEncoding = encodingBindingElement.WriteEncoding;
            }
          }
#endif
        }
        return this.bindingWriteEncoding;
      }
    }

    internal HttpResponseMessageProperty MessageProperty
    {
      get
      {
        if (!this.operationContext.OutgoingMessageProperties.ContainsKey(HttpResponseMessageProperty.Name))
          this.operationContext.OutgoingMessageProperties.Add(HttpResponseMessageProperty.Name, (object) new HttpResponseMessageProperty());
        return this.operationContext.OutgoingMessageProperties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
      }
    }

    internal OutgoingWebResponseContext(OperationContext operationContext)
    {
      this.operationContext = operationContext;
    }

    /// <summary>Sets the specified ETag.</summary>
    /// <param name="entityTag">The ETag to set.</param>
    public void SetETag(string entityTag)
    {
      this.ETag = OutgoingWebResponseContext.GenerateValidEtagFromString(entityTag);
    }

    /// <summary>Sets the specified ETag.</summary>
    /// <param name="entityTag">The ETag to set.</param>
    public void SetETag(int entityTag)
    {
      this.ETag = OutgoingWebResponseContext.GenerateValidEtag((object) entityTag);
    }

    /// <summary>Sets the specified ETag.</summary>
    /// <param name="entityTag">The ETag to set.</param>
    public void SetETag(long entityTag)
    {
      this.ETag = OutgoingWebResponseContext.GenerateValidEtag((object) entityTag);
    }

    /// <summary>Sets the specified ETag.</summary>
    /// <param name="entityTag">The ETag to set.</param>
    public void SetETag(Guid entityTag)
    {
      this.ETag = OutgoingWebResponseContext.GenerateValidEtag((object) entityTag);
    }

    /// <summary>Sets the HTTP status code of the outgoing Web response to <see cref="F:System.Net.HttpStatusCode.Created" /> and sets the Location header to the provided URI.</summary>
    /// <param name="locationUri">The <see cref="T:System.Uri" /> instance to the requested resource.</param>
    public void SetStatusAsCreated(Uri locationUri)
    {
      if (locationUri == (Uri) null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("locationUri");
      this.StatusCode = HttpStatusCode.Created;
      this.Location = locationUri.ToString();
    }

    /// <summary>Sets the HTTP status code of the outgoing Web response to <see cref="F:System.Net.HttpStatusCode.NotFound" />.</summary>
    public void SetStatusAsNotFound()
    {
      this.StatusCode = HttpStatusCode.NotFound;
    }

    /// <summary>Sets the HTTP status code of the outgoing Web response to <see cref="F:System.Net.HttpStatusCode.NotFound" /> with the specified description.</summary>
    /// <param name="description">The description of status.</param>
    public void SetStatusAsNotFound(string description)
    {
      this.StatusCode = HttpStatusCode.NotFound;
      this.StatusDescription = description;
    }

    internal static string GenerateValidEtagFromString(string entityTag)
    {
      if (string.IsNullOrEmpty(entityTag))
        return (string) null;
      if (entityTag.StartsWith("W/\"", StringComparison.OrdinalIgnoreCase) && entityTag.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.WeakEntityTagsNotSupported, (object) entityTag)));
      }
      List<int> intList = (List<int>) null;
      int index1 = entityTag.Length - 1;
      bool flag1 = (int) entityTag[0] == 34;
      bool flag2 = (int) entityTag[index1] == 34;
      if (index1 == 0 & flag1)
        flag2 = false;
      bool flag3 = !flag1 || !flag2;
      if (flag1 && !flag2)
      {
        if (intList == null)
          intList = new List<int>();
        intList.Add(0);
      }
      for (int index2 = 1; index2 < index1; ++index2)
      {
        if ((int) entityTag[index2] == 34 && (int) entityTag[index2 - 1] != 92)
        {
          if (intList == null)
            intList = new List<int>();
          intList.Add(index2 + intList.Count);
        }
      }
      if (!flag1 & flag2 && (int) entityTag[index1 - 1] != 92)
      {
        if (intList == null)
          intList = new List<int>();
        intList.Add(index1 + intList.Count);
      }
      if (flag3 || intList != null)
      {
        int num = intList == null ? 0 : intList.Count;
        StringBuilder stringBuilder = new StringBuilder(entityTag, entityTag.Length + num + 2);
        for (int index2 = 0; index2 < num; ++index2)
          stringBuilder.Insert(intList[index2], '\\');
        if (flag3)
        {
          stringBuilder.Insert(entityTag.Length + num, '"');
          stringBuilder.Insert(0, '"');
        }
        entityTag = stringBuilder.ToString();
      }
      return entityTag;
    }

    internal static string GenerateValidEtag(object entityTag)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", new object[1]
      {
        (object) entityTag.ToString()
      });
    }
  }
}
