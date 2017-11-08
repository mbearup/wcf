// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Dispatcher.UriTemplateDispatchFormatter
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Dispatcher
{
  internal class UriTemplateDispatchFormatter : IDispatchMessageFormatter
  {
    internal Dictionary<int, string> pathMapping;
    internal Dictionary<int, KeyValuePair<string, System.Type>> queryMapping;
    private Uri baseAddress;
    private IDispatchMessageFormatter inner;
    private string operationName;
    private QueryStringConverter qsc;
    private int totalNumUTVars;
    private UriTemplate uriTemplate;

    public UriTemplateDispatchFormatter(OperationDescription operationDescription, IDispatchMessageFormatter inner, QueryStringConverter qsc, string contractName, Uri baseAddress)
    {
      this.inner = inner;
      this.qsc = qsc;
      this.baseAddress = baseAddress;
      this.operationName = operationDescription.Name;
      UriTemplateClientFormatter.Populate(out this.pathMapping, out this.queryMapping, out this.totalNumUTVars, out this.uriTemplate, operationDescription, qsc, contractName);
    }

    public void DeserializeRequest(Message message, object[] parameters)
    {
      object[] parameters1 = new object[parameters.Length - this.totalNumUTVars];
      if (parameters1.Length != 0)
        this.inner.DeserializeRequest(message, parameters1);
      int index = 0;
      UriTemplateMatch uriTemplateMatch = (UriTemplateMatch) null;
      string name = "UriTemplateMatchResults";
      if (message.Properties.ContainsKey(name))
        uriTemplateMatch = message.Properties[name] as UriTemplateMatch;
      else if (message.Headers.To != (Uri) null && message.Headers.To.IsAbsoluteUri)
        uriTemplateMatch = this.uriTemplate.Match(this.baseAddress, message.Headers.To);
      NameValueCollection nameValueCollection = uriTemplateMatch == null ? new NameValueCollection() : uriTemplateMatch.BoundVariables;
      for (int key = 0; key < parameters.Length; ++key)
      {
        if (this.pathMapping.ContainsKey(key) && uriTemplateMatch != null)
          parameters[key] = (object) nameValueCollection[this.pathMapping[key]];
        else if (this.queryMapping.ContainsKey(key) && uriTemplateMatch != null)
        {
          string parameter = nameValueCollection[this.queryMapping[key].Key];
          parameters[key] = this.qsc.ConvertStringToValue(parameter, this.queryMapping[key].Value);
        }
        else
        {
          parameters[key] = parameters1[index];
          ++index;
        }
      }
      if (!DiagnosticUtility.ShouldTraceInformation || uriTemplateMatch == null)
        return;
      foreach (string key in uriTemplateMatch.QueryParameters.Keys)
      {
        bool flag = true;
        foreach (KeyValuePair<string, System.Type> keyValuePair in this.queryMapping.Values)
        {
          if (string.Compare(key, keyValuePair.Key, StringComparison.OrdinalIgnoreCase) == 0)
          {
            flag = false;
            break;
          }
        }
        if (flag)
        {
#if !FEATURE_CORECLR
          // ISSUE: reference to a compiler-generated method
          TraceUtility.TraceEvent(TraceEventType.Information, 983076, SR2.GetString(SR2.TraceCodeWebRequestUnknownQueryParameterIgnored, new object[2]
          {
            (object) key,
            (object) this.operationName
          }));
#endif
        }
      }
    }

    public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
    {
      // ISSUE: reference to a compiler-generated method
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR2.GetString(SR2.QueryStringFormatterOperationNotSupportedServerSide)));
    }
  }
}
