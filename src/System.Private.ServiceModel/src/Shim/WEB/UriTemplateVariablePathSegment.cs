// Decompiled with JetBrains decompiler
// Type: System.UriTemplateVariablePathSegment
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Specialized;
using System.Text;

namespace System
{
  internal class UriTemplateVariablePathSegment : UriTemplatePathSegment
  {
    private readonly string varName;

    public string VarName
    {
      get
      {
        return this.varName;
      }
    }

    public UriTemplateVariablePathSegment(string originalSegment, bool endsWithSlash, string varName)
      : base(originalSegment, UriTemplatePartType.Variable, endsWithSlash)
    {
      this.varName = varName;
    }

    public override void Bind(string[] values, ref int valueIndex, StringBuilder path)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("Issue with explicit reference");
#else
      if (this.EndsWithSlash)
      {
        StringBuilder stringBuilder = path;
        string format = "{0}/";
        string[] strArray = values;
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        int& local = @valueIndex;
        int num1 = valueIndex;
        int num2 = num1 + 1;
        // ISSUE: explicit reference operation
        ^local = num2;
        int index = num1;
        string str = strArray[index];
        stringBuilder.AppendFormat(format, (object) str);
      }
      else
      {

        StringBuilder stringBuilder = path;
        string[] strArray = values;
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        int& local = @valueIndex;
        int num1 = valueIndex;
        int num2 = num1 + 1;
        // ISSUE: explicit reference operation
        ^local = num2;
        int index = num1;
        string str = strArray[index];
        stringBuilder.Append(str);
      }
#endif
    }

    public override bool IsEquivalentTo(UriTemplatePathSegment other, bool ignoreTrailingSlash)
    {
      if (other == null || !ignoreTrailingSlash && this.EndsWithSlash != other.EndsWithSlash)
        return false;
      return other.Nature == UriTemplatePartType.Variable;
    }

    public override bool IsMatch(UriTemplateLiteralPathSegment segment, bool ignoreTrailingSlash)
    {
      if (!ignoreTrailingSlash && this.EndsWithSlash != segment.EndsWithSlash)
        return false;
      return !segment.IsNullOrEmpty();
    }

    public override void Lookup(string segment, NameValueCollection boundParameters)
    {
      boundParameters.Add(this.varName, segment);
    }
  }
}
