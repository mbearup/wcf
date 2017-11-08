// Decompiled with JetBrains decompiler
// Type: System.UriTemplatePathSegment
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace System
{
  [DebuggerDisplay("Segment={originalSegment} Nature={nature}")]
  internal abstract class UriTemplatePathSegment
  {
    private readonly bool endsWithSlash;
    private readonly UriTemplatePartType nature;
    private readonly string originalSegment;

    public bool EndsWithSlash
    {
      get
      {
        return this.endsWithSlash;
      }
    }

    public UriTemplatePartType Nature
    {
      get
      {
        return this.nature;
      }
    }

    public string OriginalSegment
    {
      get
      {
        return this.originalSegment;
      }
    }

    protected UriTemplatePathSegment(string originalSegment, UriTemplatePartType nature, bool endsWithSlash)
    {
      this.originalSegment = originalSegment;
      this.nature = nature;
      this.endsWithSlash = endsWithSlash;
    }

    public static UriTemplatePathSegment CreateFromUriTemplate(string segment, UriTemplate template)
    {
      switch (UriTemplateHelpers.IdentifyPartType(segment))
      {
        case UriTemplatePartType.Literal:
          return (UriTemplatePathSegment) UriTemplateLiteralPathSegment.CreateFromUriTemplate(segment, template);
        case UriTemplatePartType.Compound:
          return (UriTemplatePathSegment) UriTemplateCompoundPathSegment.CreateFromUriTemplate(segment, template);
        case UriTemplatePartType.Variable:
          if (segment.EndsWith("/", StringComparison.Ordinal))
          {
            string varName = template.AddPathVariable(UriTemplatePartType.Variable, segment.Substring(1, segment.Length - 3));
            return (UriTemplatePathSegment) new UriTemplateVariablePathSegment(segment, true, varName);
          }
          string varName1 = template.AddPathVariable(UriTemplatePartType.Variable, segment.Substring(1, segment.Length - 2));
          return (UriTemplatePathSegment) new UriTemplateVariablePathSegment(segment, false, varName1);
        default:
          return (UriTemplatePathSegment) null;
      }
    }

    public abstract void Bind(string[] values, ref int valueIndex, StringBuilder path);

    public abstract bool IsEquivalentTo(UriTemplatePathSegment other, bool ignoreTrailingSlash);

    public bool IsMatch(UriTemplateLiteralPathSegment segment)
    {
      return this.IsMatch(segment, false);
    }

    public abstract bool IsMatch(UriTemplateLiteralPathSegment segment, bool ignoreTrailingSlash);

    public abstract void Lookup(string segment, NameValueCollection boundParameters);
  }
}
