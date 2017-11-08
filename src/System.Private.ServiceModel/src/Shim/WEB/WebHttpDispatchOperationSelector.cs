// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Dispatcher.WebHttpDispatchOperationSelector
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
#if !FEATURE_CORECLR
using System.ServiceModel.Activation;
#endif
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Dispatcher
{
  /// <summary>The operation selector that supports the Web programming model.</summary>
  public class WebHttpDispatchOperationSelector : IDispatchOperationSelector
  {
    private string catchAllOperationName = "";
    /// <summary>A string used as a key for storing the value that indicates whether a call to a service operation was matched by the URI but not by the HTTP method.</summary>
    public const string HttpOperationSelectorUriMatchedPropertyName = "UriMatched";
    internal const string HttpOperationSelectorDataPropertyName = "HttpOperationSelectorData";
    /// <summary>The name of the message property on the request message that provides the name of the selected operation for the request.</summary>
    public const string HttpOperationNamePropertyName = "HttpOperationName";
    internal const string redirectOperationName = "";
    internal const string RedirectPropertyName = "WebHttpRedirect";
    private Dictionary<string, UriTemplateTable> methodSpecificTables;
    private UriTemplateTable wildcardTable;
    private Dictionary<string, UriTemplate> templates;
    private UriTemplateTable helpUriTable;

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Dispatcher.WebHttpDispatchOperationSelector" /> with the specified endpoint.</summary>
    /// <param name="endpoint">The service endpoint.</param>
    public WebHttpDispatchOperationSelector(ServiceEndpoint endpoint)
    {
      if (endpoint == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpoint");
      if (endpoint.Address == (EndpointAddress) null)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.EndpointAddressCannotBeNull)));
      }
      Uri uri = endpoint.Address.Uri;
      this.methodSpecificTables = new Dictionary<string, UriTemplateTable>();
      this.templates = new Dictionary<string, UriTemplate>();
      WebHttpBehavior webHttpBehavior = endpoint.Behaviors.Find<WebHttpBehavior>();
      if (webHttpBehavior != null && webHttpBehavior.HelpEnabled)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("HelpPage is not implemented in .NET Core");
#else
        this.helpUriTable = new UriTemplateTable(endpoint.ListenUri, HelpPage.GetOperationTemplatePairs());
#endif
      }
      Dictionary<WebHttpDispatchOperationSelector.WCFKey, string> dictionary = new Dictionary<WebHttpDispatchOperationSelector.WCFKey, string>();
      foreach (OperationDescription operation in (Collection<OperationDescription>) endpoint.Contract.Operations)
      {
        if (operation.Messages[0].Direction == MessageDirection.Input)
        {
          string webMethod = WebHttpBehavior.GetWebMethod(operation);
          string utStringOrDefault = UriTemplateClientFormatter.GetUTStringOrDefault(operation);
          if (UriTemplateHelpers.IsWildcardPath(utStringOrDefault) && webMethod == "*")
          {
            if (this.catchAllOperationName != "")
            {
              // ISSUE: reference to a compiler-generated method
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.MultipleOperationsInContractWithPathMethod, (object) endpoint.Contract.Name, (object) utStringOrDefault, (object) webMethod)));
            }
            this.catchAllOperationName = operation.Name;
          }
          UriTemplate uriTemplate = new UriTemplate(utStringOrDefault);
          WebHttpDispatchOperationSelector.WCFKey key = new WebHttpDispatchOperationSelector.WCFKey(uriTemplate, webMethod);
          if (dictionary.ContainsKey(key))
          {
            // ISSUE: reference to a compiler-generated method
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.MultipleOperationsInContractWithPathMethod, (object) endpoint.Contract.Name, (object) utStringOrDefault, (object) webMethod)));
          }
          dictionary.Add(key, operation.Name);
          UriTemplateTable uriTemplateTable;
          if (!this.methodSpecificTables.TryGetValue(webMethod, out uriTemplateTable))
          {
            uriTemplateTable = new UriTemplateTable(uri);
            this.methodSpecificTables.Add(webMethod, uriTemplateTable);
          }
          uriTemplateTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(uriTemplate, (object) operation.Name));
          this.templates.Add(operation.Name, uriTemplate);
        }
      }
      if (this.methodSpecificTables.Count == 0)
      {
        this.methodSpecificTables = (Dictionary<string, UriTemplateTable>) null;
      }
      else
      {
        foreach (UriTemplateTable uriTemplateTable in this.methodSpecificTables.Values)
          uriTemplateTable.MakeReadOnly(true);
        if (this.methodSpecificTables.TryGetValue("*", out this.wildcardTable))
          return;
        this.wildcardTable = (UriTemplateTable) null;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Dispatcher.WebHttpDispatchOperationSelector" />.</summary>
    protected WebHttpDispatchOperationSelector()
    {
    }

    /// <summary>Gets the <see cref="T:System.UriTemplate" /> associated with the specified operation name.</summary>
    /// <param name="operationName">The operation.</param>
    /// <returns>The <see cref="T:System.UriTemplate" /> for the specified operation.</returns>
    public virtual UriTemplate GetUriTemplate(string operationName)
    {
      if (operationName == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("operationName");
      UriTemplate uriTemplate;
      if (!this.templates.TryGetValue(operationName, out uriTemplate))
        return (UriTemplate) null;
      return uriTemplate;
    }

    /// <summary>Selects the service operation to call.</summary>
    /// <param name="message">The <see cref="T:System.ServiceModel.Channels.Message" /> object sent to invoke a service operation.</param>
    /// <returns>The name of the service operation to call.</returns>
    public string SelectOperation(ref Message message)
    {
      if (message == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
      bool uriMatched;
      string str = this.SelectOperation(ref message, out uriMatched);
      message.Properties.Add("UriMatched", (object) uriMatched);
      if (str != null)
      {
        message.Properties.Add("HttpOperationName", (object) str);
        if (DiagnosticUtility.ShouldTraceInformation)
        {
#if !FEATURE_CORECLR
          // ISSUE: reference to a compiler-generated method
          TraceUtility.TraceEvent(TraceEventType.Information, 983077, SR2.GetString(SR2.TraceCodeWebRequestMatchesOperation, new object[2]
          {
            (object) message.Headers.To,
            (object) str
          }));
#endif
        }
      }
      return str;
    }

    /// <summary>Selects the service operation to call.</summary>
    /// <param name="message">The <see cref="T:System.ServiceModel.Channels.Message" /> object sent to invoke a service operation.</param>
    /// <param name="uriMatched">A value that specifies whether the URI matched a specific service operation.</param>
    /// <returns>The name of the service operation to call.</returns>
    protected virtual string SelectOperation(ref Message message, out bool uriMatched)
    {
      if (message == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
      uriMatched = false;
      if (this.methodSpecificTables == null || !message.Properties.ContainsKey(HttpRequestMessageProperty.Name))
        return this.catchAllOperationName;
      HttpRequestMessageProperty property = message.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
      if (property == null)
        return this.catchAllOperationName;
      string method = property.Method;
      Uri to = message.Headers.To;
      if (to == (Uri) null)
        return this.catchAllOperationName;
      if (this.helpUriTable != null)
      {
        UriTemplateMatch match = this.helpUriTable.MatchSingle(to);
        if (match != null)
        {
          uriMatched = true;
          this.AddUriTemplateMatch(match, property, message);
          if (method == "GET")
            return "HelpPageInvoke";
#if FEATURE_CORECLR
        throw new NotImplementedException("WebHttpDispatchOperationSelectorData is not implemented in .NET Core");
#else
          message.Properties.Add("HttpOperationSelectorData", (object) new WebHttpDispatchOperationSelectorData()
          {
            AllowedMethods = new List<string>() { "GET" }
          });
          return this.catchAllOperationName;
#endif
        }
      }
      UriTemplateTable methodSpecificTable1;
      if (this.methodSpecificTables.TryGetValue(method, out methodSpecificTable1))
      {
        string operationName;
        uriMatched = this.CanUriMatch(methodSpecificTable1, to, property, message, out operationName);
        if (uriMatched)
          return operationName;
      }
      if (this.wildcardTable != null)
      {
        string operationName;
        uriMatched = this.CanUriMatch(this.wildcardTable, to, property, message, out operationName);
        if (uriMatched)
          return operationName;
      }
      if (this.ShouldRedirectToUriWithSlashAtTheEnd(methodSpecificTable1, message, to))
        return "";
      List<string> stringList = (List<string>) null;
      foreach (KeyValuePair<string, UriTemplateTable> methodSpecificTable2 in this.methodSpecificTables)
      {
        if (!(methodSpecificTable2.Key == method) && !(methodSpecificTable2.Key == "*") && methodSpecificTable2.Value.MatchSingle(to) != null)
        {
          if (stringList == null)
            stringList = new List<string>();
          if (!stringList.Contains(methodSpecificTable2.Key))
            stringList.Add(methodSpecificTable2.Key);
        }
      }
      if (stringList != null)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("WebHttpDispatchOperationSelectorData is not implemented in .NET Core");
#else
        uriMatched = true;
        message.Properties.Add("HttpOperationSelectorData", (object) new WebHttpDispatchOperationSelectorData()
        {
          AllowedMethods = stringList
        });
#endif
      }
      return this.catchAllOperationName;
    }

    private bool CanUriMatch(UriTemplateTable methodSpecificTable, Uri to, HttpRequestMessageProperty prop, Message message, out string operationName)
    {
      operationName = (string) null;
      UriTemplateMatch match = methodSpecificTable.MatchSingle(to);
      if (match == null)
        return false;
      operationName = match.Data as string;
      this.AddUriTemplateMatch(match, prop, message);
      return true;
    }

    private void AddUriTemplateMatch(UriTemplateMatch match, HttpRequestMessageProperty requestProp, Message message)
    {
      match.SetBaseUri(match.BaseUri, requestProp);
      message.Properties.Add("UriTemplateMatchResults", (object) match);
    }

    private bool ShouldRedirectToUriWithSlashAtTheEnd(UriTemplateTable methodSpecificTable, Message message, Uri to)
    {
      UriBuilder uriBuilder = new UriBuilder(to);
      if (uriBuilder.Path.EndsWith("/", StringComparison.Ordinal))
        return false;
      uriBuilder.Path = uriBuilder.Path + "/";
      Uri uri1 = uriBuilder.Uri;
      bool flag = false;
      if (methodSpecificTable != null && methodSpecificTable.MatchSingle(uri1) != null)
      {
        flag = true;
      }
      else
      {
        foreach (KeyValuePair<string, UriTemplateTable> methodSpecificTable1 in this.methodSpecificTables)
        {
          UriTemplateTable uriTemplateTable = methodSpecificTable1.Value;
          if (uriTemplateTable != methodSpecificTable && uriTemplateTable.MatchSingle(uri1) != null)
          {
            flag = true;
            break;
          }
        }
      }
      if (flag)
      {
        string authority = WebHttpDispatchOperationSelector.GetAuthority(message);
        Uri uri2 = UriTemplate.RewriteUri(uriBuilder.Uri, authority);
        message.Properties.Add("WebHttpRedirect", (object) uri2);
      }
      return flag;
    }

    private static string GetAuthority(Message message)
    {
      string str = (string) null;
      HttpRequestMessageProperty property;
      if (message.Properties.TryGetValue<HttpRequestMessageProperty>(HttpRequestMessageProperty.Name, out property))
      {
        str = property.Headers[HttpRequestHeader.Host];
        if (!string.IsNullOrEmpty(str))
          return str;
      }
#if FEATURE_CORECLR
        throw new NotImplementedException("AspNetEnvironment is not implemented in .NET Core");
#else
      IAspNetMessageProperty hostingProperty = AspNetEnvironment.Current.GetHostingProperty(message);
      if (hostingProperty != null)
        str = hostingProperty.OriginalRequestUri.Authority;
      return str;
#endif
    }

    private class WCFKey
    {
      private string method;
      private UriTemplate uriTemplate;

      public WCFKey(UriTemplate uriTemplate, string method)
      {
        this.uriTemplate = uriTemplate;
        this.method = method;
      }

      public override bool Equals(object obj)
      {
        WebHttpDispatchOperationSelector.WCFKey wcfKey = obj as WebHttpDispatchOperationSelector.WCFKey;
        if (wcfKey == null || !this.uriTemplate.IsEquivalentTo(wcfKey.uriTemplate))
          return false;
        return this.method == wcfKey.method;
      }

      public override int GetHashCode()
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("UriTemplateEquivalenceComparer is not implemented in .NET Core");
#else
        return UriTemplateEquivalenceComparer.Instance.GetHashCode(this.uriTemplate);
#endif
      }
    }
  }
}
