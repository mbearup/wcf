// Decompiled with JetBrains decompiler
// Type: System.UriTemplateQueryValue
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Specialized;
using System.Runtime;
using System.ServiceModel;
using System.Text;

namespace System
{
  internal abstract class UriTemplateQueryValue
  {
    private static UriTemplateQueryValue empty = (UriTemplateQueryValue) new UriTemplateQueryValue.EmptyUriTemplateQueryValue();
    private readonly UriTemplatePartType nature;

    public static UriTemplateQueryValue Empty
    {
      get
      {
        return UriTemplateQueryValue.empty;
      }
    }

    public UriTemplatePartType Nature
    {
      get
      {
        return this.nature;
      }
    }

    protected UriTemplateQueryValue(UriTemplatePartType nature)
    {
      this.nature = nature;
    }

    public static UriTemplateQueryValue CreateFromUriTemplate(string value, UriTemplate template)
    {
      if (value == null)
        return UriTemplateQueryValue.Empty;
      switch (UriTemplateHelpers.IdentifyPartType(value))
      {
        case UriTemplatePartType.Literal:
          return (UriTemplateQueryValue) UriTemplateLiteralQueryValue.CreateFromUriTemplate(value);
        case UriTemplatePartType.Compound:
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTQueryCannotHaveCompoundValue", new object[1]
          {
            (object) template.originalTemplate
          })));
        case UriTemplatePartType.Variable:
          return (UriTemplateQueryValue) new UriTemplateVariableQueryValue(template.AddQueryVariable(value.Substring(1, value.Length - 2)));
        default:
          return (UriTemplateQueryValue) null;
      }
    }

    public static bool IsNullOrEmpty(UriTemplateQueryValue utqv)
    {
      return utqv == null || utqv == UriTemplateQueryValue.Empty;
    }

    public abstract void Bind(string keyName, string[] values, ref int valueIndex, StringBuilder query);

    public abstract bool IsEquivalentTo(UriTemplateQueryValue other);

    public abstract void Lookup(string value, NameValueCollection boundParameters);

    private class EmptyUriTemplateQueryValue : UriTemplateQueryValue
    {
      public EmptyUriTemplateQueryValue()
        : base(UriTemplatePartType.Literal)
      {
      }

      public override void Bind(string keyName, string[] values, ref int valueIndex, StringBuilder query)
      {
        query.AppendFormat("&{0}", (object) UrlUtility.UrlEncode(keyName, Encoding.UTF8));
      }

      public override bool IsEquivalentTo(UriTemplateQueryValue other)
      {
        return other == UriTemplateQueryValue.Empty;
      }

      public override void Lookup(string value, NameValueCollection boundParameters)
      {
      }
    }
  }
}
