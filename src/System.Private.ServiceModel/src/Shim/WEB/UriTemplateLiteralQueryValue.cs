// Decompiled with JetBrains decompiler
// Type: System.UriTemplateLiteralQueryValue
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Specialized;
using System.Runtime;
using System.Text;

namespace System
{
  internal class UriTemplateLiteralQueryValue : UriTemplateQueryValue, IComparable<UriTemplateLiteralQueryValue>
  {
    private readonly string value;

    private UriTemplateLiteralQueryValue(string value)
      : base(UriTemplatePartType.Literal)
    {
      this.value = value;
    }

    public static UriTemplateLiteralQueryValue CreateFromUriTemplate(string value)
    {
      return new UriTemplateLiteralQueryValue(UrlUtility.UrlDecode(value, Encoding.UTF8));
    }

    public string AsEscapedString()
    {
      return UrlUtility.UrlEncode(this.value, Encoding.UTF8);
    }

    public string AsRawUnescapedString()
    {
      return this.value;
    }

    public override void Bind(string keyName, string[] values, ref int valueIndex, StringBuilder query)
    {
      query.AppendFormat("&{0}={1}", (object) UrlUtility.UrlEncode(keyName, Encoding.UTF8), (object) this.AsEscapedString());
    }

    public int CompareTo(UriTemplateLiteralQueryValue other)
    {
      return string.Compare(this.value, other.value, StringComparison.Ordinal);
    }

    public override bool Equals(object obj)
    {
      UriTemplateLiteralQueryValue literalQueryValue = obj as UriTemplateLiteralQueryValue;
      if (literalQueryValue == null)
        return false;
      return this.value == literalQueryValue.value;
    }

    public override int GetHashCode()
    {
      return this.value.GetHashCode();
    }

    public override bool IsEquivalentTo(UriTemplateQueryValue other)
    {
      if (other == null || other.Nature != UriTemplatePartType.Literal)
        return false;
      return this.CompareTo(other as UriTemplateLiteralQueryValue) == 0;
    }

    public override void Lookup(string value, NameValueCollection boundParameters)
    {
    }
  }
}
