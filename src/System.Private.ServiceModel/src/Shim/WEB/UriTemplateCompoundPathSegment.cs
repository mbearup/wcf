// Decompiled with JetBrains decompiler
// Type: System.UriTemplateCompoundPathSegment
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ServiceModel;
using System.Text;

namespace System
{
  internal class UriTemplateCompoundPathSegment : UriTemplatePathSegment, IComparable<UriTemplateCompoundPathSegment>
  {
    private readonly string firstLiteral;
    private readonly List<UriTemplateCompoundPathSegment.VarAndLitPair> varLitPairs;
    private UriTemplateCompoundPathSegment.CompoundSegmentClass csClass;

    private UriTemplateCompoundPathSegment(string originalSegment, bool endsWithSlash, string firstLiteral)
      : base(originalSegment, UriTemplatePartType.Compound, endsWithSlash)
    {
      this.firstLiteral = firstLiteral;
      this.varLitPairs = new List<UriTemplateCompoundPathSegment.VarAndLitPair>();
    }

    public static new UriTemplateCompoundPathSegment CreateFromUriTemplate(string segment, UriTemplate template)
    {
      string originalSegment = segment;
      bool endsWithSlash = segment.EndsWith("/", StringComparison.Ordinal);
      if (endsWithSlash)
        segment = segment.Remove(segment.Length - 1);
      int length = segment.IndexOf("{", StringComparison.Ordinal);
      string stringToUnescape1 = length > 0 ? segment.Substring(0, length) : string.Empty;
      if (stringToUnescape1.IndexOf("*", StringComparison.Ordinal) != -1)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new FormatException(SR.GetString("UTInvalidWildcardInVariableOrLiteral", (object) template.originalTemplate, (object) "*")));
      UriTemplateCompoundPathSegment compoundPathSegment = new UriTemplateCompoundPathSegment(originalSegment, endsWithSlash, stringToUnescape1 != string.Empty ? Uri.UnescapeDataString(stringToUnescape1) : string.Empty);
      do
      {
        int num = segment.IndexOf("}", length + 1, StringComparison.Ordinal);
        if (num < length + 2)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new FormatException(SR.GetString("UTInvalidFormatSegmentOrQueryPart", new object[1]
          {
            (object) segment
          })));
        bool hasDefaultValue;
        string varName = template.AddPathVariable(UriTemplatePartType.Compound, segment.Substring(length + 1, num - length - 1), out hasDefaultValue);
        if (hasDefaultValue)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTDefaultValueToCompoundSegmentVar", (object) template, (object) originalSegment, (object) varName)));
        length = segment.IndexOf("{", num + 1, StringComparison.Ordinal);
        string stringToUnescape2;
        if (length > 0)
        {
          if (length == num + 1)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("template", SR.GetString("UTDoesNotSupportAdjacentVarsInCompoundSegment", (object) template, (object) segment));
          stringToUnescape2 = segment.Substring(num + 1, length - num - 1);
        }
        else
          stringToUnescape2 = num + 1 >= segment.Length ? string.Empty : segment.Substring(num + 1);
        if (stringToUnescape2.IndexOf("*", StringComparison.Ordinal) != -1)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new FormatException(SR.GetString("UTInvalidWildcardInVariableOrLiteral", (object) template.originalTemplate, (object) "*")));
        if (stringToUnescape2.IndexOf('}') != -1)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new FormatException(SR.GetString("UTInvalidFormatSegmentOrQueryPart", new object[1]
          {
            (object) segment
          })));
        compoundPathSegment.varLitPairs.Add(new UriTemplateCompoundPathSegment.VarAndLitPair(varName, stringToUnescape2 == string.Empty ? string.Empty : Uri.UnescapeDataString(stringToUnescape2)));
      }
      while (length > 0);
      if (string.IsNullOrEmpty(compoundPathSegment.firstLiteral))
      {
        UriTemplateCompoundPathSegment.VarAndLitPair varLitPair = compoundPathSegment.varLitPairs[compoundPathSegment.varLitPairs.Count - 1];
        compoundPathSegment.csClass = !string.IsNullOrEmpty(varLitPair.Literal) ? UriTemplateCompoundPathSegment.CompoundSegmentClass.HasOnlySuffix : UriTemplateCompoundPathSegment.CompoundSegmentClass.HasNoPrefixNorSuffix;
      }
      else
      {
        UriTemplateCompoundPathSegment.VarAndLitPair varLitPair = compoundPathSegment.varLitPairs[compoundPathSegment.varLitPairs.Count - 1];
        compoundPathSegment.csClass = !string.IsNullOrEmpty(varLitPair.Literal) ? UriTemplateCompoundPathSegment.CompoundSegmentClass.HasPrefixAndSuffix : UriTemplateCompoundPathSegment.CompoundSegmentClass.HasOnlyPrefix;
      }
      return compoundPathSegment;
    }

    public override void Bind(string[] values, ref int valueIndex, StringBuilder path)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("Issue with explicit reference");
#else
      path.Append(this.firstLiteral);
      for (int index1 = 0; index1 < this.varLitPairs.Count; ++index1)
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
        int index2 = num1;
        string str = strArray[index2];
        stringBuilder.Append(str);
        path.Append(this.varLitPairs[index1].Literal);
      }
      if (!this.EndsWithSlash)
        return;
      path.Append("/");
#endif
    }

    public override bool IsEquivalentTo(UriTemplatePathSegment other, bool ignoreTrailingSlash)
    {
      if (other == null || !ignoreTrailingSlash && this.EndsWithSlash != other.EndsWithSlash)
        return false;
      UriTemplateCompoundPathSegment compoundPathSegment = other as UriTemplateCompoundPathSegment;
      if (compoundPathSegment == null || this.varLitPairs.Count != compoundPathSegment.varLitPairs.Count || StringComparer.OrdinalIgnoreCase.Compare(this.firstLiteral, compoundPathSegment.firstLiteral) != 0)
        return false;
      for (int index = 0; index < this.varLitPairs.Count; ++index)
      {
        if (StringComparer.OrdinalIgnoreCase.Compare(this.varLitPairs[index].Literal, compoundPathSegment.varLitPairs[index].Literal) != 0)
          return false;
      }
      return true;
    }

    public override bool IsMatch(UriTemplateLiteralPathSegment segment, bool ignoreTrailingSlash)
    {
      if (!ignoreTrailingSlash && this.EndsWithSlash != segment.EndsWithSlash)
        return false;
      return this.TryLookup(segment.AsUnescapedString(), (NameValueCollection) null);
    }

    public override void Lookup(string segment, NameValueCollection boundParameters)
    {
      if (!this.TryLookup(segment, boundParameters))
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTCSRLookupBeforeMatch")));
    }

    private bool TryLookup(string segment, NameValueCollection boundParameters)
    {
      int startIndex = 0;
      if (!string.IsNullOrEmpty(this.firstLiteral))
      {
        if (!segment.StartsWith(this.firstLiteral, StringComparison.Ordinal))
          return false;
        startIndex = this.firstLiteral.Length;
      }
      for (int index = 0; index < this.varLitPairs.Count - 1; ++index)
      {
        int num = segment.IndexOf(this.varLitPairs[index].Literal, startIndex, StringComparison.Ordinal);
        if (num < startIndex + 1)
          return false;
        if (boundParameters != null)
        {
          string str = segment.Substring(startIndex, num - startIndex);
          boundParameters.Add(this.varLitPairs[index].VarName, str);
        }
        startIndex = num + this.varLitPairs[index].Literal.Length;
      }
      if (startIndex >= segment.Length)
        return false;
      if (string.IsNullOrEmpty(this.varLitPairs[this.varLitPairs.Count - 1].Literal))
      {
        if (boundParameters != null)
          boundParameters.Add(this.varLitPairs[this.varLitPairs.Count - 1].VarName, segment.Substring(startIndex));
        return true;
      }
      if (startIndex + this.varLitPairs[this.varLitPairs.Count - 1].Literal.Length >= segment.Length || !segment.EndsWith(this.varLitPairs[this.varLitPairs.Count - 1].Literal, StringComparison.Ordinal))
        return false;
      if (boundParameters != null)
        boundParameters.Add(this.varLitPairs[this.varLitPairs.Count - 1].VarName, segment.Substring(startIndex, segment.Length - startIndex - this.varLitPairs[this.varLitPairs.Count - 1].Literal.Length));
      return true;
    }

    int IComparable<UriTemplateCompoundPathSegment>.CompareTo(UriTemplateCompoundPathSegment other)
    {
      switch (this.csClass)
      {
        case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasPrefixAndSuffix:
          switch (other.csClass)
          {
            case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasPrefixAndSuffix:
              return this.CompareToOtherThatHasPrefixAndSuffix(other);
            case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasOnlyPrefix:
            case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasOnlySuffix:
            case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasNoPrefixNorSuffix:
              return -1;
            default:
              return 0;
          }
        case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasOnlyPrefix:
          switch (other.csClass)
          {
            case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasPrefixAndSuffix:
              return 1;
            case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasOnlyPrefix:
              return this.CompareToOtherThatHasOnlyPrefix(other);
            case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasOnlySuffix:
            case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasNoPrefixNorSuffix:
              return -1;
            default:
              return 0;
          }
        case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasOnlySuffix:
          switch (other.csClass)
          {
            case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasPrefixAndSuffix:
            case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasOnlyPrefix:
              return 1;
            case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasOnlySuffix:
              return this.CompareToOtherThatHasOnlySuffix(other);
            case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasNoPrefixNorSuffix:
              return -1;
            default:
              return 0;
          }
        case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasNoPrefixNorSuffix:
          switch (other.csClass)
          {
            case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasPrefixAndSuffix:
            case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasOnlyPrefix:
            case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasOnlySuffix:
              return 1;
            case UriTemplateCompoundPathSegment.CompoundSegmentClass.HasNoPrefixNorSuffix:
              return this.CompareToOtherThatHasNoPrefixNorSuffix(other);
            default:
              return 0;
          }
        default:
          return 0;
      }
    }

    private int CompareToOtherThatHasPrefixAndSuffix(UriTemplateCompoundPathSegment other)
    {
      int otherPrefix = this.ComparePrefixToOtherPrefix(other);
      if (otherPrefix != 0)
        return otherPrefix;
      int otherSuffix = this.CompareSuffixToOtherSuffix(other);
      if (otherSuffix == 0)
        return other.varLitPairs.Count - this.varLitPairs.Count;
      return otherSuffix;
    }

    private int CompareToOtherThatHasOnlyPrefix(UriTemplateCompoundPathSegment other)
    {
      int otherPrefix = this.ComparePrefixToOtherPrefix(other);
      if (otherPrefix == 0)
        return other.varLitPairs.Count - this.varLitPairs.Count;
      return otherPrefix;
    }

    private int CompareToOtherThatHasOnlySuffix(UriTemplateCompoundPathSegment other)
    {
      int otherSuffix = this.CompareSuffixToOtherSuffix(other);
      if (otherSuffix == 0)
        return other.varLitPairs.Count - this.varLitPairs.Count;
      return otherSuffix;
    }

    private int CompareToOtherThatHasNoPrefixNorSuffix(UriTemplateCompoundPathSegment other)
    {
      return other.varLitPairs.Count - this.varLitPairs.Count;
    }

    private int ComparePrefixToOtherPrefix(UriTemplateCompoundPathSegment other)
    {
      return string.Compare(other.firstLiteral, this.firstLiteral, StringComparison.OrdinalIgnoreCase);
    }

    private int CompareSuffixToOtherSuffix(UriTemplateCompoundPathSegment other)
    {
      string strB = UriTemplateCompoundPathSegment.ReverseString(this.varLitPairs[this.varLitPairs.Count - 1].Literal);
      return string.Compare(UriTemplateCompoundPathSegment.ReverseString(other.varLitPairs[other.varLitPairs.Count - 1].Literal), strB, StringComparison.OrdinalIgnoreCase);
    }

    private static string ReverseString(string stringToReverse)
    {
      char[] chArray = new char[stringToReverse.Length];
      for (int index = 0; index < stringToReverse.Length; ++index)
        chArray[index] = stringToReverse[stringToReverse.Length - index - 1];
      return new string(chArray);
    }

    private enum CompoundSegmentClass
    {
      Undefined,
      HasPrefixAndSuffix,
      HasOnlyPrefix,
      HasOnlySuffix,
      HasNoPrefixNorSuffix,
    }

    private struct VarAndLitPair
    {
      private readonly string literal;
      private readonly string varName;

      public string Literal
      {
        get
        {
          return this.literal;
        }
      }

      public string VarName
      {
        get
        {
          return this.varName;
        }
      }

      public VarAndLitPair(string varName, string literal)
      {
        this.varName = varName;
        this.literal = literal;
      }
    }
  }
}
