// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Web.WebFaultException
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.Net;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace System.ServiceModel.Web
{
  /// <summary>Represents a fault that can have an associated HTTP status code. </summary>
  [Serializable]
  public class WebFaultException : FaultException, IWebFaultException
  {
    internal const string WebFaultCodeNamespace = "http://schemas.microsoft.com/2009/WebFault";

    /// <summary>Gets the <see cref="T:System.Net.HttpStatusCode" /> associated with the fault.</summary>
    /// <returns>The HTTP status code associated with the fault.</returns>
    public HttpStatusCode StatusCode { get; private set; }

    Type IWebFaultException.DetailType
    {
      get
      {
        return (Type) null;
      }
    }

    object IWebFaultException.DetailObject
    {
      get
      {
        return (object) null;
      }
    }

    Type[] IWebFaultException.KnownTypes
    {
      get
      {
        return (Type[]) null;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Web.WebFaultException" /> class with the specified <see cref="T:System.Net.HttpStatusCode" />.</summary>
    /// <param name="statusCode">The HTTP status code to return to the caller.</param>
    public WebFaultException(HttpStatusCode statusCode)
      : base(WebFaultException.GetDefaultReason(statusCode), WebFaultException.GetFaultCode(statusCode))
    {
      this.StatusCode = statusCode;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Web.WebFaultException" /> class with the specified <see cref="T:System.Runtime.Serialization.SerializationInfo" /> and <see cref="T:System.Runtime.Serialization.StreamingContext" />.</summary>
    /// <param name="info">The serialization information.</param>
    /// <param name="context">The streaming context.</param>
    protected WebFaultException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.StatusCode = (HttpStatusCode) info.GetValue("statusCode", typeof (HttpStatusCode));
    }

    /// <summary>An implementation of the <see cref="M:System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)" /> method that is called when an object is serialized to a stream. </summary>
    /// <param name="info">The serialization information to which the object data is added when serialized.</param>
    /// <param name="context">The destination for the serialized object.</param>
    [SecurityCritical]
    [SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("statusCode", (object) this.StatusCode);
    }

    internal static FaultCode GetFaultCode(HttpStatusCode statusCode)
    {
      if (statusCode >= HttpStatusCode.InternalServerError)
        return FaultCode.CreateReceiverFaultCode(statusCode.ToString(), "http://schemas.microsoft.com/2009/WebFault");
      return FaultCode.CreateSenderFaultCode(statusCode.ToString(), "http://schemas.microsoft.com/2009/WebFault");
    }

    internal static string GetDefaultReason(HttpStatusCode statusCode)
    {
      switch (statusCode)
      {
        case HttpStatusCode.MultipleChoices:
          return "Multiple Choices";
        case HttpStatusCode.MovedPermanently:
          return "Moved Permanently";
        case HttpStatusCode.Found:
          return "Found";
        case HttpStatusCode.SeeOther:
          return "See Other";
        case HttpStatusCode.NotModified:
          return "Not Modified";
        case HttpStatusCode.UseProxy:
          return "Use Proxy";
        case HttpStatusCode.TemporaryRedirect:
          return "Temporary Redirect";
        case HttpStatusCode.BadRequest:
          return "Bad Request";
        case HttpStatusCode.Unauthorized:
          return "Unauthorized";
        case HttpStatusCode.PaymentRequired:
          return "Payment Required";
        case HttpStatusCode.Forbidden:
          return "Forbidden";
        case HttpStatusCode.NotFound:
          return "Not Found";
        case HttpStatusCode.MethodNotAllowed:
          return "Method Not Allowed";
        case HttpStatusCode.NotAcceptable:
          return "Not Acceptable";
        case HttpStatusCode.ProxyAuthenticationRequired:
          return "Proxy Authentication Required";
        case HttpStatusCode.RequestTimeout:
          return "Request Time-out";
        case HttpStatusCode.Conflict:
          return "Conflict";
        case HttpStatusCode.Gone:
          return "Gone";
        case HttpStatusCode.LengthRequired:
          return "Length Required";
        case HttpStatusCode.PreconditionFailed:
          return "Precondition Failed";
        case HttpStatusCode.RequestEntityTooLarge:
          return "Request Entity Too Large";
        case HttpStatusCode.RequestUriTooLong:
          return "Request-URI Too Large";
        case HttpStatusCode.UnsupportedMediaType:
          return "Unsupported Media Type";
        case HttpStatusCode.RequestedRangeNotSatisfiable:
          return "Requested range not satisfiable";
        case HttpStatusCode.ExpectationFailed:
          return "Expectation Failed";
        case HttpStatusCode.InternalServerError:
          return "Internal Server Error";
        case HttpStatusCode.NotImplemented:
          return "Not Implemented";
        case HttpStatusCode.BadGateway:
          return "Bad Gateway";
        case HttpStatusCode.ServiceUnavailable:
          return "Service Unavailable";
        case HttpStatusCode.GatewayTimeout:
          return "Gateway Time-out";
        case HttpStatusCode.HttpVersionNotSupported:
          return "HTTP Version not supported";
        case HttpStatusCode.Continue:
          return "Continue";
        case HttpStatusCode.SwitchingProtocols:
          return "Switching Protocols";
        case HttpStatusCode.OK:
          return "OK";
        case HttpStatusCode.Created:
          return "Created";
        case HttpStatusCode.Accepted:
          return "Accepted";
        case HttpStatusCode.NonAuthoritativeInformation:
          return "Non-Authoritative Information";
        case HttpStatusCode.NoContent:
          return "No Content";
        case HttpStatusCode.ResetContent:
          return "Reset Content";
        case HttpStatusCode.PartialContent:
          return "Partial Content";
        default:
          switch ((int) statusCode / 100)
          {
            case 1:
              return "Informational";
            case 2:
              return "Success";
            case 3:
              return "Redirection";
            case 4:
              return "Client Error";
            case 5:
              return "Server Error";
            default:
              return (string) null;
          }
      }
    }
  }
}
