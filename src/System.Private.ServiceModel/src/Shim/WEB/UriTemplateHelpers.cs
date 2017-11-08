// Decompiled with JetBrains decompiler
// Type: System.UriTemplateHelpers
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime;
using System.ServiceModel;

namespace System
{
  internal static class UriTemplateHelpers
  {
    private static UriTemplateHelpers.UriTemplateQueryComparer queryComparer = new UriTemplateHelpers.UriTemplateQueryComparer();
    private static UriTemplateHelpers.UriTemplateQueryKeyComparer queryKeyComperar = new UriTemplateHelpers.UriTemplateQueryKeyComparer();

    [Conditional("DEBUG")]
    public static void AssertCanonical(string s)
    {
    }

    public static bool CanMatchQueryInterestingly(UriTemplate ut, NameValueCollection query, bool mustBeEspeciallyInteresting)
    {
      if (ut.queries.Count == 0)
        return false;
      string[] allKeys = query.AllKeys;
      foreach (KeyValuePair<string, UriTemplateQueryValue> query1 in ut.queries)
      {
        string key = query1.Key;
        if (query1.Value.Nature == UriTemplatePartType.Literal)
        {
          bool flag = false;
          for (int index = 0; index < allKeys.Length; ++index)
          {
            if (StringComparer.OrdinalIgnoreCase.Equals(allKeys[index], key))
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            return false;
          if (query1.Value == UriTemplateQueryValue.Empty)
          {
            if (!string.IsNullOrEmpty(query[key]))
              return false;
          }
          else if (((UriTemplateLiteralQueryValue) query1.Value).AsRawUnescapedString() != query[key])
            return false;
        }
        else if (mustBeEspeciallyInteresting && Array.IndexOf<string>(allKeys, key) == -1)
          return false;
      }
      return true;
    }

    public static bool CanMatchQueryTrivially(UriTemplate ut)
    {
      return ut.queries.Count == 0;
    }

    public static void DisambiguateSamePath(UriTemplate[] array, int a, int b, bool allowDuplicateEquivalentUriTemplates)
    {
      Array.Sort<UriTemplate>(array, a, b - a, (IComparer<UriTemplate>) UriTemplateHelpers.queryComparer);
      if (b - a == 1)
        return;
      if (!allowDuplicateEquivalentUriTemplates)
      {
        if (array[a].queries.Count == 0)
          ++a;
        if (array[a].queries.Count == 0)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTTDuplicate", (object) array[a].ToString(), (object) array[a - 1].ToString())));
        if (b - a == 1)
          return;
      }
      else
      {
        while (a < b && array[a].queries.Count == 0)
          ++a;
        if (b - a <= 1)
          return;
      }
      UriTemplateHelpers.EnsureQueriesAreDistinct(array, a, b, allowDuplicateEquivalentUriTemplates);
    }

    public static IEqualityComparer<string> GetQueryKeyComparer()
    {
      return (IEqualityComparer<string>) UriTemplateHelpers.queryKeyComperar;
    }

    public static string GetUriPath(Uri uri)
    {
      return uri.GetComponents(UriComponents.Path | UriComponents.KeepDelimiter, UriFormat.Unescaped);
    }

    public static bool HasQueryLiteralRequirements(UriTemplate ut)
    {
      foreach (UriTemplateQueryValue templateQueryValue in ut.queries.Values)
      {
        if (templateQueryValue.Nature == UriTemplatePartType.Literal)
          return true;
      }
      return false;
    }

    public static UriTemplatePartType IdentifyPartType(string part)
    {
      int num1 = part.IndexOf("{", StringComparison.Ordinal);
      int num2 = part.IndexOf("}", StringComparison.Ordinal);
      if (num1 == -1)
      {
        if (num2 != -1)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new FormatException(SR.GetString("UTInvalidFormatSegmentOrQueryPart", new object[1]
          {
            (object) part
          })));
        return UriTemplatePartType.Literal;
      }
      if (num2 < num1 + 2)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new FormatException(SR.GetString("UTInvalidFormatSegmentOrQueryPart", new object[1]
        {
          (object) part
        })));
      return num1 > 0 || num2 < part.Length - 2 || num2 == part.Length - 2 && !part.EndsWith("/", StringComparison.Ordinal) ? UriTemplatePartType.Compound : UriTemplatePartType.Variable;
    }

    public static bool IsWildcardPath(string path)
    {
      if (path.IndexOf('/') != -1)
        return false;
      UriTemplatePartType type;
      return UriTemplateHelpers.IsWildcardSegment(path, out type);
    }

    public static bool IsWildcardSegment(string segment, out UriTemplatePartType type)
    {
      type = UriTemplateHelpers.IdentifyPartType(segment);
      switch (type)
      {
        case UriTemplatePartType.Literal:
          return string.Compare(segment, "*", StringComparison.Ordinal) == 0;
        case UriTemplatePartType.Compound:
          return false;
        case UriTemplatePartType.Variable:
          if (segment.IndexOf("*", StringComparison.Ordinal) == 1 && !segment.EndsWith("/", StringComparison.Ordinal))
            return segment.Length > "*".Length + 2;
          return false;
        default:
          return false;
      }
    }

    public static NameValueCollection ParseQueryString(string query)
    {
      NameValueCollection queryString = UrlUtility.ParseQueryString(query);
      string str1 = queryString[(string) null];
      if (!string.IsNullOrEmpty(str1))
      {
        queryString.Remove((string) null);
        string str2 = str1;
        char[] chArray = new char[1]{ ',' };
        foreach (string name in str2.Split(chArray))
          queryString.Add(name, (string) null);
      }
      return queryString;
    }

    private static bool AllTemplatesAreEquivalent(IList<UriTemplate> array, int a, int b)
    {
      for (int index = a; index < b - 1; ++index)
      {
        if (!array[index].IsEquivalentTo(array[index + 1]))
          return false;
      }
      return true;
    }

    private static void EnsureQueriesAreDistinct(UriTemplate[] array, int a, int b, bool allowDuplicateEquivalentUriTemplates)
    {
      Dictionary<string, byte> dictionary = new Dictionary<string, byte>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int index = a; index < b; ++index)
      {
        foreach (KeyValuePair<string, UriTemplateQueryValue> query in array[index].queries)
        {
          if (query.Value.Nature == UriTemplatePartType.Literal && !dictionary.ContainsKey(query.Key))
            dictionary.Add(query.Key, (byte) 0);
        }
      }
      Dictionary<string, byte> queryVarNames = new Dictionary<string, byte>((IDictionary<string, byte>) dictionary);
      for (int index = a; index < b; ++index)
      {
        foreach (string key in dictionary.Keys)
        {
          if (!array[index].queries.ContainsKey(key) || array[index].queries[key].Nature != UriTemplatePartType.Literal)
            queryVarNames.Remove(key);
        }
      }
      if (queryVarNames.Count == 0 && (!allowDuplicateEquivalentUriTemplates || !UriTemplateHelpers.AllTemplatesAreEquivalent((IList<UriTemplate>) array, a, b)))
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTTOtherAmbiguousQueries", new object[1]
        {
          (object) array[a].ToString()
        })));
      string[][] strArray = new string[b - a][];
      for (int index = 0; index < b - a; ++index)
        strArray[index] = UriTemplateHelpers.GetQueryLiterals(array[index + a], queryVarNames);
      for (int index1 = 0; index1 < b - a; ++index1)
      {
        for (int index2 = index1 + 1; index2 < b - a; ++index2)
        {
          if (UriTemplateHelpers.Same(strArray[index1], strArray[index2]))
          {
            if (!array[index1 + a].IsEquivalentTo(array[index2 + a]))
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTTAmbiguousQueries", (object) array[a + index1].ToString(), (object) array[index2 + a].ToString())));
            if (!allowDuplicateEquivalentUriTemplates)
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTTDuplicate", (object) array[a + index1].ToString(), (object) array[index2 + a].ToString())));
          }
        }
      }
    }

    private static string[] GetQueryLiterals(UriTemplate up, Dictionary<string, byte> queryVarNames)
    {
      string[] strArray = new string[queryVarNames.Count];
      int index = 0;
      foreach (string key in queryVarNames.Keys)
      {
        UriTemplateQueryValue query = up.queries[key];
        strArray[index] = query != UriTemplateQueryValue.Empty ? ((UriTemplateLiteralQueryValue) query).AsRawUnescapedString() : (string) null;
        ++index;
      }
      return strArray;
    }

    private static bool Same(string[] a, string[] b)
    {
      for (int index = 0; index < a.Length; ++index)
      {
        if (a[index] != b[index])
          return false;
      }
      return true;
    }

    private class UriTemplateQueryComparer : IComparer<UriTemplate>
    {
      public int Compare(UriTemplate x, UriTemplate y)
      {
        return Comparer<int>.Default.Compare(x.queries.Count, y.queries.Count);
      }
    }

    private class UriTemplateQueryKeyComparer : IEqualityComparer<string>
    {
      public bool Equals(string x, string y)
      {
        return string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0;
      }

      public int GetHashCode(string obj)
      {
        if (obj == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("obj");
        return obj.ToUpperInvariant().GetHashCode();
      }
    }
  }
}
