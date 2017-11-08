// Decompiled with JetBrains decompiler
// Type: System.UriTemplateLiteralPathSegment
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Specialized;
using System.ServiceModel;
using System.Text;

namespace System
{
  internal class UriTemplateLiteralPathSegment : UriTemplatePathSegment, IComparable<UriTemplateLiteralPathSegment>
  {
    private static Uri dummyUri = new Uri("http://localhost");
    private readonly string segment;

    private UriTemplateLiteralPathSegment(string segment)
      : base(segment, UriTemplatePartType.Literal, segment.EndsWith("/", StringComparison.Ordinal))
    {
      if (this.EndsWithSlash)
        this.segment = segment.Remove(segment.Length - 1);
      else
        this.segment = segment;
    }

    public static new UriTemplateLiteralPathSegment CreateFromUriTemplate(string segment, UriTemplate template)
    {
      if (string.Compare(segment, "/", StringComparison.Ordinal) == 0)
        return new UriTemplateLiteralPathSegment("/");
      if (segment.IndexOf("*", StringComparison.Ordinal) != -1)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new FormatException(SR.GetString("UTInvalidWildcardInVariableOrLiteral", (object) template.originalTemplate, (object) "*")));
      segment = segment.Replace("%2a", "*").Replace("%2A", "*");
      string segment1 = new UriBuilder(UriTemplateLiteralPathSegment.dummyUri)
      {
        Path = segment
      }.Uri.AbsolutePath.Substring(1);
      if (segment1 == string.Empty)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("segment", SR.GetString("UTInvalidFormatSegmentOrQueryPart", new object[1]
        {
          (object) segment
        }));
      return new UriTemplateLiteralPathSegment(segment1);
    }

    public static UriTemplateLiteralPathSegment CreateFromWireData(string segment)
    {
      return new UriTemplateLiteralPathSegment(segment);
    }

    public string AsUnescapedString()
    {
      return Uri.UnescapeDataString(this.segment);
    }

    public override void Bind(string[] values, ref int valueIndex, StringBuilder path)
    {
      if (this.EndsWithSlash)
        path.AppendFormat("{0}/", (object) this.AsUnescapedString());
      else
        path.Append(this.AsUnescapedString());
    }

    public int CompareTo(UriTemplateLiteralPathSegment other)
    {
      return StringComparer.OrdinalIgnoreCase.Compare(this.segment, other.segment);
    }

    public override bool Equals(object obj)
    {
      UriTemplateLiteralPathSegment literalPathSegment = obj as UriTemplateLiteralPathSegment;
      if (literalPathSegment == null || this.EndsWithSlash != literalPathSegment.EndsWithSlash)
        return false;
      return StringComparer.OrdinalIgnoreCase.Equals(this.segment, literalPathSegment.segment);
    }

    public override int GetHashCode()
    {
      return StringComparer.OrdinalIgnoreCase.GetHashCode(this.segment);
    }

    public override bool IsEquivalentTo(UriTemplatePathSegment other, bool ignoreTrailingSlash)
    {
      if (other == null || other.Nature != UriTemplatePartType.Literal)
        return false;
      return this.IsMatch(other as UriTemplateLiteralPathSegment, ignoreTrailingSlash);
    }

    public override bool IsMatch(UriTemplateLiteralPathSegment segment, bool ignoreTrailingSlash)
    {
      if (!ignoreTrailingSlash && segment.EndsWithSlash != this.EndsWithSlash)
        return false;
      return this.CompareTo(segment) == 0;
    }

    public bool IsNullOrEmpty()
    {
      return string.IsNullOrEmpty(this.segment);
    }

    public override void Lookup(string segment, NameValueCollection boundParameters)
    {
    }
  }
}
