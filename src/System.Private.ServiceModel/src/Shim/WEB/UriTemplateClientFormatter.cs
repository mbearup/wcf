// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Dispatcher.UriTemplateClientFormatter
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;

namespace System.ServiceModel.Dispatcher
{
  internal class UriTemplateClientFormatter : IClientMessageFormatter
  {
    internal Dictionary<int, string> pathMapping;
    internal Dictionary<int, KeyValuePair<string, System.Type>> queryMapping;
    private Uri baseUri;
    private IClientMessageFormatter inner;
    private bool innerIsUntypedMessage;
    private bool isGet;
    private string method;
    private QueryStringConverter qsc;
    private int totalNumUTVars;
    private UriTemplate uriTemplate;

    public UriTemplateClientFormatter(OperationDescription operationDescription, IClientMessageFormatter inner, QueryStringConverter qsc, Uri baseUri, bool innerIsUntypedMessage, string contractName)
    {
      this.inner = inner;
      this.qsc = qsc;
      this.baseUri = baseUri;
      this.innerIsUntypedMessage = innerIsUntypedMessage;
      UriTemplateClientFormatter.Populate(out this.pathMapping, out this.queryMapping, out this.totalNumUTVars, out this.uriTemplate, operationDescription, qsc, contractName);
      this.method = WebHttpBehavior.GetWebMethod(operationDescription);
      this.isGet = this.method == "GET";
    }

    public object DeserializeReply(Message message, object[] parameters)
    {
      // ISSUE: reference to a compiler-generated method
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR2.GetString(SR2.QueryStringFormatterOperationNotSupportedClientSide)));
    }

    public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
    {
      object[] parameters1 = new object[parameters.Length - this.totalNumUTVars];
      NameValueCollection parameters2 = new NameValueCollection();
      int index = 0;
      for (int key = 0; key < parameters.Length; ++key)
      {
        if (this.pathMapping.ContainsKey(key))
          parameters2[this.pathMapping[key]] = parameters[key] as string;
        else if (this.queryMapping.ContainsKey(key))
        {
          if (parameters[key] != null)
            parameters2[this.queryMapping[key].Key] = this.qsc.ConvertValueToString(parameters[key], this.queryMapping[key].Value);
        }
        else
        {
          parameters1[index] = parameters[key];
          ++index;
        }
      }
      Message message = this.inner.SerializeRequest(messageVersion, parameters1);
      if ((!this.innerIsUntypedMessage || !(message.Headers.To != (Uri) null)) && (OperationContext.Current == null || !(OperationContext.Current.OutgoingMessageHeaders.To != (Uri) null)))
        message.Headers.To = this.uriTemplate.BindByName(this.baseUri, parameters2);
      if (WebOperationContext.Current != null)
      {
        if (this.isGet)
          WebOperationContext.Current.OutgoingRequest.SuppressEntityBody = true;
        if (this.method != "*" && WebOperationContext.Current.OutgoingRequest.Method != null)
          WebOperationContext.Current.OutgoingRequest.Method = this.method;
      }
      else
      {
        HttpRequestMessageProperty requestMessageProperty;
        if (message.Properties.ContainsKey(HttpRequestMessageProperty.Name))
        {
          requestMessageProperty = message.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
        }
        else
        {
          requestMessageProperty = new HttpRequestMessageProperty();
          message.Properties.Add(HttpRequestMessageProperty.Name, (object) requestMessageProperty);
        }
        if (this.isGet)
          requestMessageProperty.SuppressEntityBody = true;
        if (this.method != "*")
          requestMessageProperty.Method = this.method;
      }
      return message;
    }

    internal static string GetUTStringOrDefault(OperationDescription operationDescription)
    {
      string str = WebHttpBehavior.GetWebUriTemplate(operationDescription);
      if (str == null && WebHttpBehavior.GetWebMethod(operationDescription) == "GET")
        str = UriTemplateClientFormatter.MakeDefaultGetUTString(operationDescription);
      if (str == null)
        str = operationDescription.Name;
      return str;
    }

    internal static void Populate(out Dictionary<int, string> pathMapping, out Dictionary<int, KeyValuePair<string, System.Type>> queryMapping, out int totalNumUTVars, out UriTemplate uriTemplate, OperationDescription operationDescription, QueryStringConverter qsc, string contractName)
    {
      pathMapping = new Dictionary<int, string>();
      queryMapping = new Dictionary<int, KeyValuePair<string, System.Type>>();
      string utStringOrDefault = UriTemplateClientFormatter.GetUTStringOrDefault(operationDescription);
      uriTemplate = new UriTemplate(utStringOrDefault);
      List<string> stringList1 = new List<string>((IEnumerable<string>) uriTemplate.PathSegmentVariableNames);
      List<string> stringList2 = new List<string>((IEnumerable<string>) uriTemplate.QueryValueVariableNames);
      Dictionary<string, byte> dictionary = new Dictionary<string, byte>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      totalNumUTVars = stringList1.Count + stringList2.Count;
      for (int key = 0; key < operationDescription.Messages[0].Body.Parts.Count; ++key)
      {
        MessagePartDescription part = operationDescription.Messages[0].Body.Parts[key];
        string decodedName = part.XmlName.DecodedName;
        if (dictionary.ContainsKey(decodedName))
        {
          // ISSUE: reference to a compiler-generated method
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.UriTemplateVarCaseDistinction, (object) operationDescription.XmlName.DecodedName, (object) contractName, (object) decodedName)));
        }
        foreach (string strB in new List<string>((IEnumerable<string>) stringList1))
        {
          if (string.Compare(decodedName, strB, StringComparison.OrdinalIgnoreCase) == 0)
          {
            if (part.Type != typeof (string))
            {
              // ISSUE: reference to a compiler-generated method
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.UriTemplatePathVarMustBeString, (object) operationDescription.XmlName.DecodedName, (object) contractName, (object) decodedName)));
            }
            pathMapping.Add(key, decodedName);
            dictionary.Add(decodedName, (byte) 0);
            stringList1.Remove(strB);
          }
        }
        foreach (string strB in new List<string>((IEnumerable<string>) stringList2))
        {
          if (string.Compare(decodedName, strB, StringComparison.OrdinalIgnoreCase) == 0)
          {
            if (!qsc.CanConvert(part.Type))
            {
              // ISSUE: reference to a compiler-generated method
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.UriTemplateQueryVarMustBeConvertible, (object) operationDescription.XmlName.DecodedName, (object) contractName, (object) decodedName, (object) part.Type, (object) qsc.GetType().Name)));
            }
            queryMapping.Add(key, new KeyValuePair<string, System.Type>(decodedName, part.Type));
            dictionary.Add(decodedName, (byte) 0);
            stringList2.Remove(strB);
          }
        }
      }
      if (stringList1.Count != 0)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.UriTemplateMissingVar, (object) operationDescription.XmlName.DecodedName, (object) contractName, (object) stringList1[0])));
      }
      if (stringList2.Count != 0)
      {
        // ISSUE: reference to a compiler-generated method
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.UriTemplateMissingVar, (object) operationDescription.XmlName.DecodedName, (object) contractName, (object) stringList2[0])));
      }
    }

    private static string MakeDefaultGetUTString(OperationDescription od)
    {
      StringBuilder stringBuilder = new StringBuilder(od.XmlName.DecodedName);
      if (!WebHttpBehavior.IsUntypedMessage(od.Messages[0]))
      {
        stringBuilder.Append("?");
        foreach (MessagePartDescription part in (Collection<MessagePartDescription>) od.Messages[0].Body.Parts)
        {
          string decodedName = part.XmlName.DecodedName;
          stringBuilder.Append(decodedName);
          stringBuilder.Append("={");
          stringBuilder.Append(decodedName);
          stringBuilder.Append("}&");
        }
        stringBuilder.Remove(stringBuilder.Length - 1, 1);
      }
      return stringBuilder.ToString();
    }
  }
}
