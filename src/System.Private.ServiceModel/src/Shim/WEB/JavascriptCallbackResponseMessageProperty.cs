// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.JavascriptCallbackResponseMessageProperty
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.Net;

namespace System.ServiceModel.Channels
{
  /// <summary>Enables the use of a JavaScript callback in a service operation response using JSON Padding (JSONP).</summary>
  public sealed class JavascriptCallbackResponseMessageProperty
  {
    private static readonly string JavascriptCallbackResponseMessagePropertyName = "javascriptCallbackResponse";

    /// <summary>Gets the message property name used to add a JavaScript callback message property to a service operation response using JSONP.</summary>
    /// <returns>The message property name.</returns>
    public static string Name
    {
      get
      {
        return JavascriptCallbackResponseMessageProperty.JavascriptCallbackResponseMessagePropertyName;
      }
    }

    /// <summary>Gets or sets the name of the callback function used with JSONP.</summary>
    /// <returns>The name of the callback function.</returns>
    public string CallbackFunctionName { get; set; }

    /// <summary>Gets or sets the HTTP status code.</summary>
    /// <returns>The HTTP status code.</returns>
    public HttpStatusCode? StatusCode { get; set; }
  }
}
