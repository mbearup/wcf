// Decompiled with JetBrains decompiler
// Type: System.UriTemplateVariableQueryValue
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Specialized;
using System.Runtime;
using System.Text;

namespace System
{
  internal class UriTemplateVariableQueryValue : UriTemplateQueryValue
  {
    private readonly string varName;

    public UriTemplateVariableQueryValue(string varName)
      : base(UriTemplatePartType.Variable)
    {
      this.varName = varName;
    }

    public override void Bind(string keyName, string[] values, ref int valueIndex, StringBuilder query)
    {
      if (values[valueIndex] == null)
      {
        valueIndex = valueIndex + 1;
      }
      else
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("Issue with explicit reference");
#else
        StringBuilder stringBuilder = query;
        string format = "&{0}={1}";
        string str1 = UrlUtility.UrlEncode(keyName, Encoding.UTF8);
        string[] strArray = values;
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        int& local = @valueIndex;
        int num1 = valueIndex;
        int num2 = num1 + 1;
        // ISSUE: explicit reference operation
        ^local = num2;
        int index = num1;
        string str2 = UrlUtility.UrlEncode(strArray[index], Encoding.UTF8);
        stringBuilder.AppendFormat(format, (object) str1, (object) str2);
#endif
      }
    }

    public override bool IsEquivalentTo(UriTemplateQueryValue other)
    {
      if (other == null)
        return false;
      return other.Nature == UriTemplatePartType.Variable;
    }

    public override void Lookup(string value, NameValueCollection boundParameters)
    {
      boundParameters.Add(this.varName, value);
    }
  }
}
