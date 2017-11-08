// Decompiled with JetBrains decompiler
// Type: System.UriTemplate
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace System
{
  /// <summary>A class that represents a Uniform Resource Identifier (URI) template.</summary>
  //[TypeForwardedFrom("System.ServiceModel.Web, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
  public class UriTemplate
  {
    internal readonly int firstOptionalSegment;
    internal readonly string originalTemplate;
    internal readonly Dictionary<string, UriTemplateQueryValue> queries;
    internal readonly List<UriTemplatePathSegment> segments;
    internal const string WildcardPath = "*";
    private readonly Dictionary<string, string> additionalDefaults;
    private readonly string fragment;
    private readonly bool ignoreTrailingSlash;
    private const string NullableDefault = "null";
    private readonly UriTemplate.WildcardInfo wildcard;
    private IDictionary<string, string> defaults;
    private ConcurrentDictionary<string, string> unescapedDefaults;
    private UriTemplate.VariablesCollection variables;

    /// <summary>Gets a collection of name/value pairs for any default parameter values.</summary>
    /// <returns>A generic dictionary.</returns>
    public IDictionary<string, string> Defaults
    {
      get
      {
        if (this.defaults == null)
          Interlocked.CompareExchange<IDictionary<string, string>>(ref this.defaults, (IDictionary<string, string>) new UriTemplate.UriTemplateDefaults(this), (IDictionary<string, string>) null);
        return this.defaults;
      }
    }

    /// <summary>Specifies whether trailing slashes “/” in the template should be ignored when matching candidate URIs.</summary>
    /// <returns>true if trailing slashes “/” should be ignored, otherwise false.</returns>
    public bool IgnoreTrailingSlash
    {
      get
      {
        return this.ignoreTrailingSlash;
      }
    }

    /// <summary>Gets a collection of variable names used within path segments in the template.</summary>
    /// <returns>A collection of variable names that appear within the template's path segment.</returns>
    public ReadOnlyCollection<string> PathSegmentVariableNames
    {
      get
      {
        if (this.variables == null)
          return UriTemplate.VariablesCollection.EmptyCollection;
        return this.variables.PathSegmentVariableNames;
      }
    }

    /// <summary>Gets a collection of variable names used within the query string in the template.</summary>
    /// <returns>A collection of template variable names that appear in the query portion of the template string.</returns>
    public ReadOnlyCollection<string> QueryValueVariableNames
    {
      get
      {
        if (this.variables == null)
          return UriTemplate.VariablesCollection.EmptyCollection;
        return this.variables.QueryValueVariableNames;
      }
    }

    internal bool HasNoVariables
    {
      get
      {
        return this.variables == null;
      }
    }

    internal bool HasWildcard
    {
      get
      {
        return this.wildcard != null;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.UriTemplate" /> class with the specified template string.</summary>
    /// <param name="template">The template.</param>
    public UriTemplate(string template)
      : this(template, false)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.UriTemplate" /> class.</summary>
    /// <param name="template">The template string.</param>
    /// <param name="ignoreTrailingSlash">A value that specifies whether trailing slash “/” characters should be ignored.</param>
    public UriTemplate(string template, bool ignoreTrailingSlash)
      : this(template, ignoreTrailingSlash, (IDictionary<string, string>) null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.UriTemplate" /> class.</summary>
    /// <param name="template">The template string.</param>
    /// <param name="additionalDefaults">A dictionary that contains a list of default values for the template parameters.</param>
    public UriTemplate(string template, IDictionary<string, string> additionalDefaults)
      : this(template, false, additionalDefaults)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.UriTemplate" /> class.</summary>
    /// <param name="template">The template string.</param>
    /// <param name="ignoreTrailingSlash">true if the trailing slash “/” characters are ignored; otherwise false.</param>
    /// <param name="additionalDefaults">A dictionary that contains a list of default values for the template parameters.</param>
    public UriTemplate(string template, bool ignoreTrailingSlash, IDictionary<string, string> additionalDefaults)
    {
      if (template == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("template");
      this.originalTemplate = template;
      this.ignoreTrailingSlash = ignoreTrailingSlash;
      this.segments = new List<UriTemplatePathSegment>();
      this.queries = new Dictionary<string, UriTemplateQueryValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (template.StartsWith("/", StringComparison.Ordinal))
        template = template.Substring(1);
      int length1 = template.IndexOf('#');
      if (length1 == -1)
      {
        this.fragment = "";
      }
      else
      {
        this.fragment = template.Substring(length1 + 1);
        template = template.Substring(0, length1);
      }
      int length2 = template.IndexOf('?');
      string str1;
      string str2;
      if (length2 == -1)
      {
        str1 = string.Empty;
        str2 = template;
      }
      else
      {
        str1 = template.Substring(length2 + 1);
        str2 = template.Substring(0, length2);
      }
      template = (string) null;
      if (!string.IsNullOrEmpty(str2))
      {
        int startIndex = 0;
        while (startIndex < str2.Length)
        {
          int num = str2.IndexOf('/', startIndex);
          string segment;
          if (num != -1)
          {
            segment = str2.Substring(startIndex, num + 1 - startIndex);
            startIndex = num + 1;
          }
          else
          {
            segment = str2.Substring(startIndex);
            startIndex = str2.Length;
          }
          UriTemplatePartType type;
          if (startIndex == str2.Length && UriTemplateHelpers.IsWildcardSegment(segment, out type))
          {
            if (type != UriTemplatePartType.Literal)
            {
              if (type == UriTemplatePartType.Variable)
                this.wildcard = new UriTemplate.WildcardInfo(this, segment);
            }
            else
              this.wildcard = new UriTemplate.WildcardInfo(this);
          }
          else
            this.segments.Add(UriTemplatePathSegment.CreateFromUriTemplate(segment, this));
        }
      }
      if (!string.IsNullOrEmpty(str1))
      {
        int startIndex1 = 0;
        while (startIndex1 < str1.Length)
        {
          int num1 = str1.IndexOf('&', startIndex1);
          int startIndex2 = startIndex1;
          int num2;
          if (num1 != -1)
          {
            num2 = num1;
            startIndex1 = num1 + 1;
            if (startIndex1 >= str1.Length)
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTQueryCannotEndInAmpersand", new object[1]
              {
                (object) this.originalTemplate
              })));
          }
          else
          {
            num2 = str1.Length;
            startIndex1 = str1.Length;
          }
          int num3 = str1.IndexOf('=', startIndex2, num2 - startIndex2);
          string str3;
          string str4;
          if (num3 >= 0)
          {
            str3 = str1.Substring(startIndex2, num3 - startIndex2);
            str4 = str1.Substring(num3 + 1, num2 - num3 - 1);
          }
          else
          {
            str3 = str1.Substring(startIndex2, num2 - startIndex2);
            str4 = (string) null;
          }
          if (string.IsNullOrEmpty(str3))
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTQueryCannotHaveEmptyName", new object[1]
            {
              (object) this.originalTemplate
            })));
          if (UriTemplateHelpers.IdentifyPartType(str3) != UriTemplatePartType.Literal)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("template", SR.GetString("UTQueryMustHaveLiteralNames", new object[1]
            {
              (object) this.originalTemplate
            }));
          string key = UrlUtility.UrlDecode(str3, Encoding.UTF8);
          if (this.queries.ContainsKey(key))
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTQueryNamesMustBeUnique", new object[1]
            {
              (object) this.originalTemplate
            })));
          this.queries.Add(key, UriTemplateQueryValue.CreateFromUriTemplate(str4, this));
        }
      }
      if (additionalDefaults != null)
      {
        if (this.variables == null)
        {
          if (additionalDefaults.Count > 0)
            this.additionalDefaults = new Dictionary<string, string>(additionalDefaults, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        }
        else
        {
          foreach (KeyValuePair<string, string> additionalDefault in (IEnumerable<KeyValuePair<string, string>>) additionalDefaults)
          {
            string upperInvariant = additionalDefault.Key.ToUpperInvariant();
            if (this.variables.DefaultValues != null && this.variables.DefaultValues.ContainsKey(upperInvariant))
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("additionalDefaults", SR.GetString("UTAdditionalDefaultIsInvalid", (object) additionalDefault.Key, (object) this.originalTemplate));
            if (this.variables.PathSegmentVariableNames.Contains(upperInvariant))
            {
              this.variables.AddDefaultValue(upperInvariant, additionalDefault.Value);
            }
            else
            {
              if (this.variables.QueryValueVariableNames.Contains(upperInvariant))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTDefaultValueToQueryVarFromAdditionalDefaults", (object) this.originalTemplate, (object) upperInvariant)));
              if (string.Compare(additionalDefault.Value, "null", StringComparison.OrdinalIgnoreCase) == 0)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTNullableDefaultAtAdditionalDefaults", (object) this.originalTemplate, (object) upperInvariant)));
              if (this.additionalDefaults == null)
                this.additionalDefaults = new Dictionary<string, string>(additionalDefaults.Count, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
              this.additionalDefaults.Add(additionalDefault.Key, additionalDefault.Value);
            }
          }
        }
      }
      if (this.variables != null && this.variables.DefaultValues != null)
        this.variables.ValidateDefaults(out this.firstOptionalSegment);
      else
        this.firstOptionalSegment = this.segments.Count;
    }

    /// <summary>Creates a new URI from the template and the collection of parameters.</summary>
    /// <param name="baseAddress">The base address.</param>
    /// <param name="parameters">A dictionary that contains a collection of parameter name/value pairs.</param>
    /// <returns>A URI.</returns>
    public Uri BindByName(Uri baseAddress, IDictionary<string, string> parameters)
    {
      return this.BindByName(baseAddress, parameters, false);
    }

    /// <summary>Creates a new URI from the template and the collection of parameters.</summary>
    /// <param name="baseAddress">A URI that contains the base address.</param>
    /// <param name="parameters">A dictionary that contains a collection of parameter name/value pairs.</param>
    /// <param name="omitDefaults">true is the default values are ignored; otherwise false.</param>
    /// <returns>A URI.</returns>
    public Uri BindByName(Uri baseAddress, IDictionary<string, string> parameters, bool omitDefaults)
    {
      if (baseAddress == (Uri) null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("baseAddress");
      if (!baseAddress.IsAbsoluteUri)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("baseAddress", SR.GetString("UTBadBaseAddress"));
      UriTemplate.BindInformation bindInfo = this.variables != null ? this.variables.PrepareBindInformation(parameters, omitDefaults) : this.PrepareBindInformation(parameters, omitDefaults);
      return this.Bind(baseAddress, bindInfo, omitDefaults);
    }

    /// <summary>Creates a new URI from the template and the collection of parameters.</summary>
    /// <param name="baseAddress">The base address.</param>
    /// <param name="parameters">The parameter values.</param>
    /// <returns>A new instance.</returns>
    public Uri BindByName(Uri baseAddress, NameValueCollection parameters)
    {
      return this.BindByName(baseAddress, parameters, false);
    }

    /// <summary>Creates a new URI from the template and the collection of parameters.</summary>
    /// <param name="baseAddress">The base address.</param>
    /// <param name="parameters">A collection of parameter name/value pairs. </param>
    /// <param name="omitDefaults">true if the default values are ignored; otherwise false.</param>
    /// <returns>A URI.</returns>
    public Uri BindByName(Uri baseAddress, NameValueCollection parameters, bool omitDefaults)
    {
      if (baseAddress == (Uri) null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("baseAddress");
      if (!baseAddress.IsAbsoluteUri)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("baseAddress", SR.GetString("UTBadBaseAddress"));
      UriTemplate.BindInformation bindInfo = this.variables != null ? this.variables.PrepareBindInformation(parameters, omitDefaults) : this.PrepareBindInformation(parameters, omitDefaults);
      return this.Bind(baseAddress, bindInfo, omitDefaults);
    }

    /// <summary>Creates a new URI from the template and an array of parameter values.</summary>
    /// <param name="baseAddress">A <see cref="T:System.Uri" /> that contains the base address.</param>
    /// <param name="values">The parameter values.</param>
    /// <returns>A new <see cref="T:System.Uri" /> instance.</returns>
    public Uri BindByPosition(Uri baseAddress, params string[] values)
    {
      if (baseAddress == (Uri) null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("baseAddress");
      if (!baseAddress.IsAbsoluteUri)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("baseAddress", SR.GetString("UTBadBaseAddress"));
      UriTemplate.BindInformation bindInfo;
      if (this.variables == null)
      {
        if (values.Length != 0)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new FormatException(SR.GetString("UTBindByPositionNoVariables", (object) this.originalTemplate, (object) values.Length)));
        bindInfo = new UriTemplate.BindInformation((IDictionary<string, string>) this.additionalDefaults);
      }
      else
        bindInfo = this.variables.PrepareBindInformation(values);
      return this.Bind(baseAddress, bindInfo, false);
    }

    /// <summary>Indicates whether a <see cref="T:System.UriTemplate" /> is structurally equivalent to another.</summary>
    /// <param name="other">The <see cref="T:System.UriTemplate" /> to compare.</param>
    /// <returns>true if the <see cref="T:System.UriTemplate" /> is structurally equivalent to another; otherwise false.</returns>
    public bool IsEquivalentTo(UriTemplate other)
    {
      return other != null && other.segments != null && (other.queries != null && this.IsPathFullyEquivalent(other)) && this.IsQueryEquivalent(other);
    }

    /// <summary>Attempts to match a <see cref="T:System.URI" /> to a <see cref="T:System.UriTemplate" />.</summary>
    /// <param name="baseAddress">The base address.</param>
    /// <param name="candidate">The <see cref="T:System.Uri" /> to match against the template.</param>
    /// <returns>An instance.</returns>
    public UriTemplateMatch Match(Uri baseAddress, Uri candidate)
    {
      if (baseAddress == (Uri) null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("baseAddress");
      if (!baseAddress.IsAbsoluteUri)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("baseAddress", SR.GetString("UTBadBaseAddress"));
      if (candidate == (Uri) null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("candidate");
      if (!candidate.IsAbsoluteUri)
        return (UriTemplateMatch) null;
      string uriPath1 = UriTemplateHelpers.GetUriPath(baseAddress);
      string uriPath2 = UriTemplateHelpers.GetUriPath(candidate);
      if (uriPath2.Length < uriPath1.Length)
        return (UriTemplateMatch) null;
      if (!uriPath2.StartsWith(uriPath1, StringComparison.OrdinalIgnoreCase))
        return (UriTemplateMatch) null;
      int numMatchedSegments;
      Collection<string> relativeSegments;
      if (!this.IsCandidatePathMatch(baseAddress.Segments.Length, candidate.Segments, out numMatchedSegments, out relativeSegments))
        return (UriTemplateMatch) null;
      NameValueCollection nameValueCollection = (NameValueCollection) null;
      if (!UriTemplateHelpers.CanMatchQueryTrivially(this))
      {
        nameValueCollection = UriTemplateHelpers.ParseQueryString(candidate.Query);
        if (!UriTemplateHelpers.CanMatchQueryInterestingly(this, nameValueCollection, false))
          return (UriTemplateMatch) null;
      }
      return this.CreateUriTemplateMatch(baseAddress, candidate, (object) null, numMatchedSegments, relativeSegments, nameValueCollection);
    }

    /// <summary>Returns a string representation of the <see cref="T:System.UriTemplate" /> instance.</summary>
    /// <returns>The representation of the <see cref="T:System.UriTemplate" /> instance.</returns>
    public override string ToString()
    {
      return this.originalTemplate;
    }

    internal string AddPathVariable(UriTemplatePartType sourceNature, string varDeclaration)
    {
      bool hasDefaultValue;
      return this.AddPathVariable(sourceNature, varDeclaration, out hasDefaultValue);
    }

    internal string AddPathVariable(UriTemplatePartType sourceNature, string varDeclaration, out bool hasDefaultValue)
    {
      if (this.variables == null)
        this.variables = new UriTemplate.VariablesCollection(this);
      return this.variables.AddPathVariable(sourceNature, varDeclaration, out hasDefaultValue);
    }

    internal string AddQueryVariable(string varDeclaration)
    {
      if (this.variables == null)
        this.variables = new UriTemplate.VariablesCollection(this);
      return this.variables.AddQueryVariable(varDeclaration);
    }

    internal UriTemplateMatch CreateUriTemplateMatch(Uri baseUri, Uri uri, object data, int numMatchedSegments, Collection<string> relativePathSegments, NameValueCollection uriQuery)
    {
      UriTemplateMatch uriTemplateMatch = new UriTemplateMatch();
      uriTemplateMatch.RequestUri = uri;
      uriTemplateMatch.BaseUri = baseUri;
      if (uriQuery != null)
        uriTemplateMatch.SetQueryParameters(uriQuery);
      uriTemplateMatch.SetRelativePathSegments(relativePathSegments);
      uriTemplateMatch.Data = data;
      uriTemplateMatch.Template = this;
      for (int index = 0; index < numMatchedSegments; ++index)
        this.segments[index].Lookup(uriTemplateMatch.RelativePathSegments[index], uriTemplateMatch.BoundVariables);
      if (this.wildcard != null)
        this.wildcard.Lookup(numMatchedSegments, uriTemplateMatch.RelativePathSegments, uriTemplateMatch.BoundVariables);
      else if (numMatchedSegments < this.segments.Count)
        this.BindTerminalDefaults(numMatchedSegments, uriTemplateMatch.BoundVariables);
      if (this.queries.Count > 0)
      {
        foreach (KeyValuePair<string, UriTemplateQueryValue> query in this.queries)
          query.Value.Lookup(uriTemplateMatch.QueryParameters[query.Key], uriTemplateMatch.BoundVariables);
      }
      if (this.additionalDefaults != null)
      {
        foreach (KeyValuePair<string, string> additionalDefault in this.additionalDefaults)
          uriTemplateMatch.BoundVariables.Add(additionalDefault.Key, this.UnescapeDefaultValue(additionalDefault.Value));
      }
      uriTemplateMatch.SetWildcardPathSegmentsStart(numMatchedSegments);
      return uriTemplateMatch;
    }

    internal bool IsPathPartiallyEquivalentAt(UriTemplate other, int segmentsCount)
    {
      for (int index = 0; index < segmentsCount; ++index)
      {
        if (!this.segments[index].IsEquivalentTo(other.segments[index], index == segmentsCount - 1 && (this.ignoreTrailingSlash || other.ignoreTrailingSlash)))
          return false;
      }
      return true;
    }

    internal bool IsQueryEquivalent(UriTemplate other)
    {
      if (this.queries.Count != other.queries.Count)
        return false;
      foreach (string key in this.queries.Keys)
      {
        UriTemplateQueryValue query = this.queries[key];
        UriTemplateQueryValue other1;
        if (!other.queries.TryGetValue(key, out other1) || !query.IsEquivalentTo(other1))
          return false;
      }
      return true;
    }

    internal static Uri RewriteUri(Uri uri, string host)
    {
      if (string.IsNullOrEmpty(host) || string.Equals(uri.Host + (!uri.IsDefaultPort ? ":" + uri.Port.ToString((IFormatProvider) CultureInfo.InvariantCulture) : string.Empty), host, StringComparison.OrdinalIgnoreCase))
        return uri;
      Uri uri1 = new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}", new object[2]
      {
        (object) uri.Scheme,
        (object) host
      }));
      return new UriBuilder(uri)
      {
        Host = uri1.Host,
        Port = uri1.Port
      }.Uri;
    }

    private Uri Bind(Uri baseAddress, UriTemplate.BindInformation bindInfo, bool omitDefaults)
    {
      UriBuilder uriBuilder = new UriBuilder(baseAddress);
      int valueIndex = 0;
      int num1 = this.variables == null ? -1 : this.variables.PathSegmentVariableNames.Count - 1;
      int num2 = num1 != -1 ? (!omitDefaults ? bindInfo.LastNonNullablePathParameter : bindInfo.LastNonDefaultPathParameter) : -1;
      string[] normalizedParameters = bindInfo.NormalizedParameters;
      IDictionary<string, string> additionalParameters = bindInfo.AdditionalParameters;
      StringBuilder path = new StringBuilder(uriBuilder.Path);
      if ((int) path[path.Length - 1] != 47)
        path.Append('/');
      if (num2 < num1)
      {
        int index = 0;
        while (valueIndex <= num2)
          this.segments[index++].Bind(normalizedParameters, ref valueIndex, path);
        while (this.segments[index].Nature == UriTemplatePartType.Literal)
          this.segments[index++].Bind(normalizedParameters, ref valueIndex, path);
        valueIndex = num1 + 1;
      }
      else if (this.segments.Count > 0 || this.wildcard != null)
      {
        for (int index = 0; index < this.segments.Count; ++index)
          this.segments[index].Bind(normalizedParameters, ref valueIndex, path);
        if (this.wildcard != null)
          this.wildcard.Bind(normalizedParameters, ref valueIndex, path);
      }
      if (this.ignoreTrailingSlash && (int) path[path.Length - 1] == 47)
        path.Remove(path.Length - 1, 1);
      uriBuilder.Path = path.ToString();
      if (this.queries.Count != 0 || additionalParameters != null)
      {
        StringBuilder query = new StringBuilder("");
        foreach (string key in this.queries.Keys)
          this.queries[key].Bind(key, normalizedParameters, ref valueIndex, query);
        if (additionalParameters != null)
        {
          foreach (string key in (IEnumerable<string>) additionalParameters.Keys)
          {
            if (this.queries.ContainsKey(key.ToUpperInvariant()))
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("parameters", SR.GetString("UTBothLiteralAndNameValueCollectionKey", new object[1]
              {
                (object) key
              }));
            string str1 = additionalParameters[key];
            string str2 = string.IsNullOrEmpty(str1) ? string.Empty : UrlUtility.UrlEncode(str1, Encoding.UTF8);
            query.AppendFormat("&{0}={1}", (object) UrlUtility.UrlEncode(key, Encoding.UTF8), (object) str2);
          }
        }
        if (query.Length != 0)
          query.Remove(0, 1);
        uriBuilder.Query = query.ToString();
      }
      if (this.fragment != null)
        uriBuilder.Fragment = this.fragment;
      return uriBuilder.Uri;
    }

    private void BindTerminalDefaults(int numMatchedSegments, NameValueCollection boundParameters)
    {
      for (int index = numMatchedSegments; index < this.segments.Count; ++index)
      {
        if (this.segments[index].Nature == UriTemplatePartType.Variable)
          this.variables.LookupDefault((this.segments[index] as UriTemplateVariablePathSegment).VarName, boundParameters);
      }
    }

    private bool IsCandidatePathMatch(int numSegmentsInBaseAddress, string[] candidateSegments, out int numMatchedSegments, out Collection<string> relativeSegments)
    {
      int num = candidateSegments.Length - numSegmentsInBaseAddress;
      relativeSegments = new Collection<string>();
      bool flag = true;
      int index;
      for (index = 0; flag && index < num; ++index)
      {
        string candidateSegment = candidateSegments[index + numSegmentsInBaseAddress];
        if (index < this.segments.Count)
        {
          bool ignoreTrailingSlash = this.ignoreTrailingSlash && index == num - 1;
          UriTemplateLiteralPathSegment fromWireData = UriTemplateLiteralPathSegment.CreateFromWireData(candidateSegment);
          if (!this.segments[index].IsMatch(fromWireData, ignoreTrailingSlash))
          {
            flag = false;
            break;
          }
          string str = Uri.UnescapeDataString(candidateSegment);
          if (fromWireData.EndsWithSlash)
            str = str.Substring(0, str.Length - 1);
          relativeSegments.Add(str);
        }
        else
        {
          if (!this.HasWildcard)
          {
            flag = false;
            break;
          }
          break;
        }
      }
      if (flag)
      {
        numMatchedSegments = index;
        if (index < num)
        {
          for (; index < num; ++index)
          {
            string str = Uri.UnescapeDataString(candidateSegments[index + numSegmentsInBaseAddress]);
            if (str.EndsWith("/", StringComparison.Ordinal))
              str = str.Substring(0, str.Length - 1);
            relativeSegments.Add(str);
          }
        }
        else if (numMatchedSegments < this.firstOptionalSegment)
          flag = false;
      }
      else
        numMatchedSegments = 0;
      return flag;
    }

    private bool IsPathFullyEquivalent(UriTemplate other)
    {
      if (this.HasWildcard != other.HasWildcard || this.segments.Count != other.segments.Count)
        return false;
      for (int index = 0; index < this.segments.Count; ++index)
      {
        if (!this.segments[index].IsEquivalentTo(other.segments[index], index == this.segments.Count - 1 && !this.HasWildcard && (this.ignoreTrailingSlash || other.ignoreTrailingSlash)))
          return false;
      }
      return true;
    }

    private UriTemplate.BindInformation PrepareBindInformation(IDictionary<string, string> parameters, bool omitDefaults)
    {
      if (parameters == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
      IDictionary<string, string> extraParameters = (IDictionary<string, string>) new Dictionary<string, string>(UriTemplateHelpers.GetQueryKeyComparer());
      foreach (KeyValuePair<string, string> parameter in (IEnumerable<KeyValuePair<string, string>>) parameters)
      {
        if (string.IsNullOrEmpty(parameter.Key))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("parameters", SR.GetString("UTBindByNameCalledWithEmptyKey"));
        extraParameters.Add(parameter);
      }
      UriTemplate.BindInformation bindInfo;
      this.ProcessDefaultsAndCreateBindInfo(omitDefaults, extraParameters, out bindInfo);
      return bindInfo;
    }

    private UriTemplate.BindInformation PrepareBindInformation(NameValueCollection parameters, bool omitDefaults)
    {
      if (parameters == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
      IDictionary<string, string> extraParameters = (IDictionary<string, string>) new Dictionary<string, string>(UriTemplateHelpers.GetQueryKeyComparer());
      foreach (string allKey in parameters.AllKeys)
      {
        if (string.IsNullOrEmpty(allKey))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("parameters", SR.GetString("UTBindByNameCalledWithEmptyKey"));
        extraParameters.Add(allKey, parameters[allKey]);
      }
      UriTemplate.BindInformation bindInfo;
      this.ProcessDefaultsAndCreateBindInfo(omitDefaults, extraParameters, out bindInfo);
      return bindInfo;
    }

    private void ProcessDefaultsAndCreateBindInfo(bool omitDefaults, IDictionary<string, string> extraParameters, out UriTemplate.BindInformation bindInfo)
    {
      if (this.additionalDefaults != null)
      {
        if (omitDefaults)
        {
          foreach (KeyValuePair<string, string> additionalDefault in this.additionalDefaults)
          {
            string strA;
            if (extraParameters.TryGetValue(additionalDefault.Key, out strA) && string.Compare(strA, additionalDefault.Value, StringComparison.Ordinal) == 0)
              extraParameters.Remove(additionalDefault.Key);
          }
        }
        else
        {
          foreach (KeyValuePair<string, string> additionalDefault in this.additionalDefaults)
          {
            if (!extraParameters.ContainsKey(additionalDefault.Key))
              extraParameters.Add(additionalDefault.Key, additionalDefault.Value);
          }
        }
      }
      if (extraParameters.Count == 0)
        extraParameters = (IDictionary<string, string>) null;
      bindInfo = new UriTemplate.BindInformation(extraParameters);
    }

    private string UnescapeDefaultValue(string escapedValue)
    {
      if (string.IsNullOrEmpty(escapedValue))
        return escapedValue;
      if (this.unescapedDefaults == null)
        this.unescapedDefaults = new ConcurrentDictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
      return this.unescapedDefaults.GetOrAdd(escapedValue, new Func<string, string>(Uri.UnescapeDataString));
    }

    private struct BindInformation
    {
      private IDictionary<string, string> additionalParameters;
      private int lastNonDefaultPathParameter;
      private int lastNonNullablePathParameter;
      private string[] normalizedParameters;

      public IDictionary<string, string> AdditionalParameters
      {
        get
        {
          return this.additionalParameters;
        }
      }

      public int LastNonDefaultPathParameter
      {
        get
        {
          return this.lastNonDefaultPathParameter;
        }
      }

      public int LastNonNullablePathParameter
      {
        get
        {
          return this.lastNonNullablePathParameter;
        }
      }

      public string[] NormalizedParameters
      {
        get
        {
          return this.normalizedParameters;
        }
      }

      public BindInformation(string[] normalizedParameters, int lastNonDefaultPathParameter, int lastNonNullablePathParameter, IDictionary<string, string> additionalParameters)
      {
        this.normalizedParameters = normalizedParameters;
        this.lastNonDefaultPathParameter = lastNonDefaultPathParameter;
        this.lastNonNullablePathParameter = lastNonNullablePathParameter;
        this.additionalParameters = additionalParameters;
      }

      public BindInformation(IDictionary<string, string> additionalParameters)
      {
        this.normalizedParameters = (string[]) null;
        this.lastNonDefaultPathParameter = -1;
        this.lastNonNullablePathParameter = -1;
        this.additionalParameters = additionalParameters;
      }
    }

    private class UriTemplateDefaults : IDictionary<string, string>, ICollection<KeyValuePair<string, string>>, IEnumerable<KeyValuePair<string, string>>, IEnumerable
    {
      private Dictionary<string, string> defaults;
      private ReadOnlyCollection<string> keys;
      private ReadOnlyCollection<string> values;

      public int Count
      {
        get
        {
          return this.defaults.Count;
        }
      }

      public bool IsReadOnly
      {
        get
        {
          return true;
        }
      }

      public ICollection<string> Keys
      {
        get
        {
          return (ICollection<string>) this.keys;
        }
      }

      public ICollection<string> Values
      {
        get
        {
          return (ICollection<string>) this.values;
        }
      }

      public string this[string key]
      {
        get
        {
          return this.defaults[key];
        }
        set
        {
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UTDefaultValuesAreImmutable")));
        }
      }

      public UriTemplateDefaults(UriTemplate template)
      {
        this.defaults = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        if (template.variables != null && template.variables.DefaultValues != null)
        {
          foreach (KeyValuePair<string, string> defaultValue in template.variables.DefaultValues)
            this.defaults.Add(defaultValue.Key, defaultValue.Value);
        }
        if (template.additionalDefaults != null)
        {
          foreach (KeyValuePair<string, string> additionalDefault in template.additionalDefaults)
            this.defaults.Add(additionalDefault.Key.ToUpperInvariant(), additionalDefault.Value);
        }
        this.keys = new ReadOnlyCollection<string>((IList<string>) new List<string>((IEnumerable<string>) this.defaults.Keys));
        this.values = new ReadOnlyCollection<string>((IList<string>) new List<string>((IEnumerable<string>) this.defaults.Values));
      }

      public void Add(string key, string value)
      {
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UTDefaultValuesAreImmutable")));
      }

      public void Add(KeyValuePair<string, string> item)
      {
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UTDefaultValuesAreImmutable")));
      }

      public void Clear()
      {
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UTDefaultValuesAreImmutable")));
      }

      public bool Contains(KeyValuePair<string, string> item)
      {
        return ((ICollection<KeyValuePair<string, string>>) this.defaults).Contains(item);
      }

      public bool ContainsKey(string key)
      {
        return this.defaults.ContainsKey(key);
      }

      public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
      {
        ((ICollection<KeyValuePair<string, string>>) this.defaults).CopyTo(array, arrayIndex);
      }

      public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
      {
        return (IEnumerator<KeyValuePair<string, string>>) this.defaults.GetEnumerator();
      }

      public bool Remove(string key)
      {
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UTDefaultValuesAreImmutable")));
      }

      public bool Remove(KeyValuePair<string, string> item)
      {
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("UTDefaultValuesAreImmutable")));
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.defaults.GetEnumerator();
      }

      public bool TryGetValue(string key, out string value)
      {
        return this.defaults.TryGetValue(key, out value);
      }
    }

    private class VariablesCollection
    {
      private readonly UriTemplate owner;
      private static ReadOnlyCollection<string> emptyStringCollection;
      private Dictionary<string, string> defaultValues;
      private int firstNullablePathVariable;
      private List<string> pathSegmentVariableNames;
      private ReadOnlyCollection<string> pathSegmentVariableNamesSnapshot;
      private List<UriTemplatePartType> pathSegmentVariableNature;
      private List<string> queryValueVariableNames;
      private ReadOnlyCollection<string> queryValueVariableNamesSnapshot;

      public static ReadOnlyCollection<string> EmptyCollection
      {
        get
        {
          if (UriTemplate.VariablesCollection.emptyStringCollection == null)
            UriTemplate.VariablesCollection.emptyStringCollection = new ReadOnlyCollection<string>((IList<string>) new List<string>());
          return UriTemplate.VariablesCollection.emptyStringCollection;
        }
      }

      public Dictionary<string, string> DefaultValues
      {
        get
        {
          return this.defaultValues;
        }
      }

      public ReadOnlyCollection<string> PathSegmentVariableNames
      {
        get
        {
          if (this.pathSegmentVariableNamesSnapshot == null)
            Interlocked.CompareExchange<ReadOnlyCollection<string>>(ref this.pathSegmentVariableNamesSnapshot, new ReadOnlyCollection<string>((IList<string>) this.pathSegmentVariableNames), (ReadOnlyCollection<string>) null);
          return this.pathSegmentVariableNamesSnapshot;
        }
      }

      public ReadOnlyCollection<string> QueryValueVariableNames
      {
        get
        {
          if (this.queryValueVariableNamesSnapshot == null)
            Interlocked.CompareExchange<ReadOnlyCollection<string>>(ref this.queryValueVariableNamesSnapshot, new ReadOnlyCollection<string>((IList<string>) this.queryValueVariableNames), (ReadOnlyCollection<string>) null);
          return this.queryValueVariableNamesSnapshot;
        }
      }

      public VariablesCollection(UriTemplate owner)
      {
        this.owner = owner;
        this.pathSegmentVariableNames = new List<string>();
        this.pathSegmentVariableNature = new List<UriTemplatePartType>();
        this.queryValueVariableNames = new List<string>();
        this.firstNullablePathVariable = -1;
      }

      public void AddDefaultValue(string varName, string value)
      {
        int index = this.pathSegmentVariableNames.IndexOf(varName);
        if (this.owner.wildcard != null && this.owner.wildcard.HasVariable && index == this.pathSegmentVariableNames.Count - 1)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTStarVariableWithDefaultsFromAdditionalDefaults", (object) this.owner.originalTemplate, (object) varName)));
        if (this.pathSegmentVariableNature[index] != UriTemplatePartType.Variable)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTDefaultValueToCompoundSegmentVarFromAdditionalDefaults", (object) this.owner.originalTemplate, (object) varName)));
        if (string.IsNullOrEmpty(value) || string.Compare(value, "null", StringComparison.OrdinalIgnoreCase) == 0)
          value = (string) null;
        if (this.defaultValues == null)
          this.defaultValues = new Dictionary<string, string>();
        this.defaultValues.Add(varName, value);
      }

      public string AddPathVariable(UriTemplatePartType sourceNature, string varDeclaration, out bool hasDefaultValue)
      {
        string varName;
        string defaultValue;
        this.ParseVariableDeclaration(varDeclaration, out varName, out defaultValue);
        hasDefaultValue = defaultValue != null;
        if (varName.IndexOf("*", StringComparison.Ordinal) != -1)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new FormatException(SR.GetString("UTInvalidWildcardInVariableOrLiteral", (object) this.owner.originalTemplate, (object) "*")));
        string upperInvariant = varName.ToUpperInvariant();
        if (this.pathSegmentVariableNames.Contains(upperInvariant) || this.queryValueVariableNames.Contains(upperInvariant))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTVarNamesMustBeUnique", (object) this.owner.originalTemplate, (object) varName)));
        this.pathSegmentVariableNames.Add(upperInvariant);
        this.pathSegmentVariableNature.Add(sourceNature);
        if (hasDefaultValue)
        {
          if (defaultValue == string.Empty)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTInvalidDefaultPathValue", (object) this.owner.originalTemplate, (object) varDeclaration, (object) varName)));
          if (string.Compare(defaultValue, "null", StringComparison.OrdinalIgnoreCase) == 0)
            defaultValue = (string) null;
          if (this.defaultValues == null)
            this.defaultValues = new Dictionary<string, string>();
          this.defaultValues.Add(upperInvariant, defaultValue);
        }
        return upperInvariant;
      }

      public string AddQueryVariable(string varDeclaration)
      {
        string varName;
        string defaultValue;
        this.ParseVariableDeclaration(varDeclaration, out varName, out defaultValue);
        if (varName.IndexOf("*", StringComparison.Ordinal) != -1)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new FormatException(SR.GetString("UTInvalidWildcardInVariableOrLiteral", (object) this.owner.originalTemplate, (object) "*")));
        if (defaultValue != null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTDefaultValueToQueryVar", (object) this.owner.originalTemplate, (object) varDeclaration, (object) varName)));
        string upperInvariant = varName.ToUpperInvariant();
        if (this.pathSegmentVariableNames.Contains(upperInvariant) || this.queryValueVariableNames.Contains(upperInvariant))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTVarNamesMustBeUnique", (object) this.owner.originalTemplate, (object) varName)));
        this.queryValueVariableNames.Add(upperInvariant);
        return upperInvariant;
      }

      public void LookupDefault(string varName, NameValueCollection boundParameters)
      {
        boundParameters.Add(varName, this.owner.UnescapeDefaultValue(this.defaultValues[varName]));
      }

      public UriTemplate.BindInformation PrepareBindInformation(IDictionary<string, string> parameters, bool omitDefaults)
      {
        if (parameters == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
        string[] normalizedParameters = this.PrepareNormalizedParameters();
        IDictionary<string, string> extraParameters = (IDictionary<string, string>) null;
        foreach (string key in (IEnumerable<string>) parameters.Keys)
          this.ProcessBindParameter(key, parameters[key], normalizedParameters, ref extraParameters);
        UriTemplate.BindInformation bindInfo;
        this.ProcessDefaultsAndCreateBindInfo(omitDefaults, normalizedParameters, extraParameters, out bindInfo);
        return bindInfo;
      }

      public UriTemplate.BindInformation PrepareBindInformation(NameValueCollection parameters, bool omitDefaults)
      {
        if (parameters == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
        string[] normalizedParameters = this.PrepareNormalizedParameters();
        IDictionary<string, string> extraParameters = (IDictionary<string, string>) null;
        foreach (string allKey in parameters.AllKeys)
          this.ProcessBindParameter(allKey, parameters[allKey], normalizedParameters, ref extraParameters);
        UriTemplate.BindInformation bindInfo;
        this.ProcessDefaultsAndCreateBindInfo(omitDefaults, normalizedParameters, extraParameters, out bindInfo);
        return bindInfo;
      }

      public UriTemplate.BindInformation PrepareBindInformation(params string[] parameters)
      {
        if (parameters == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("values");
        if (parameters.Length < this.pathSegmentVariableNames.Count || parameters.Length > this.pathSegmentVariableNames.Count + this.queryValueVariableNames.Count)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new FormatException(SR.GetString("UTBindByPositionWrongCount", (object) this.owner.originalTemplate, (object) this.pathSegmentVariableNames.Count, (object) this.queryValueVariableNames.Count, (object) parameters.Length)));
        string[] normalizedParameters;
        if (parameters.Length == this.pathSegmentVariableNames.Count + this.queryValueVariableNames.Count)
        {
          normalizedParameters = parameters;
        }
        else
        {
          normalizedParameters = new string[this.pathSegmentVariableNames.Count + this.queryValueVariableNames.Count];
          parameters.CopyTo((Array) normalizedParameters, 0);
          for (int length = parameters.Length; length < normalizedParameters.Length; ++length)
            normalizedParameters[length] = (string) null;
        }
        int lastNonDefaultPathParameter;
        int lastNonNullablePathParameter;
        this.LoadDefaultsAndValidate(normalizedParameters, out lastNonDefaultPathParameter, out lastNonNullablePathParameter);
        return new UriTemplate.BindInformation(normalizedParameters, lastNonDefaultPathParameter, lastNonNullablePathParameter, (IDictionary<string, string>) this.owner.additionalDefaults);
      }

      public void ValidateDefaults(out int firstOptionalSegment)
      {
        for (int index = this.pathSegmentVariableNames.Count - 1; index >= 0 && this.firstNullablePathVariable == -1; --index)
        {
          string str;
          if (!this.defaultValues.TryGetValue(this.pathSegmentVariableNames[index], out str))
            this.firstNullablePathVariable = index + 1;
          else if (str != null)
            this.firstNullablePathVariable = index + 1;
        }
        if (this.firstNullablePathVariable == -1)
          this.firstNullablePathVariable = 0;
        if (this.firstNullablePathVariable > 1)
        {
          for (int index = this.firstNullablePathVariable - 2; index >= 0; --index)
          {
            string segmentVariableName = this.pathSegmentVariableNames[index];
            string str;
            if (this.defaultValues.TryGetValue(segmentVariableName, out str) && str == null)
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTNullableDefaultMustBeFollowedWithNullables", (object) this.owner.originalTemplate, (object) segmentVariableName, (object) this.pathSegmentVariableNames[index + 1])));
          }
        }
        if (this.firstNullablePathVariable < this.pathSegmentVariableNames.Count)
        {
          if (this.owner.HasWildcard)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTNullableDefaultMustNotBeFollowedWithWildcard", (object) this.owner.originalTemplate, (object) this.pathSegmentVariableNames[this.firstNullablePathVariable])));
          for (int index1 = this.pathSegmentVariableNames.Count - 1; index1 >= this.firstNullablePathVariable; --index1)
          {
            int index2 = this.owner.segments.Count - (this.pathSegmentVariableNames.Count - index1);
            if (this.owner.segments[index2].Nature != UriTemplatePartType.Variable)
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTNullableDefaultMustNotBeFollowedWithLiteral", (object) this.owner.originalTemplate, (object) this.pathSegmentVariableNames[this.firstNullablePathVariable], (object) this.owner.segments[index2].OriginalSegment)));
          }
        }
        int num = this.pathSegmentVariableNames.Count - this.firstNullablePathVariable;
        firstOptionalSegment = this.owner.segments.Count - num;
        if (this.owner.HasWildcard)
          return;
        while (firstOptionalSegment > 0)
        {
          UriTemplatePathSegment segment = this.owner.segments[firstOptionalSegment - 1];
          if (segment.Nature != UriTemplatePartType.Variable || !this.defaultValues.ContainsKey((segment as UriTemplateVariablePathSegment).VarName))
            break;
          firstOptionalSegment = firstOptionalSegment - 1;
        }
      }

      private void AddAdditionalDefaults(ref IDictionary<string, string> extraParameters)
      {
        if (extraParameters == null)
        {
          extraParameters = (IDictionary<string, string>) this.owner.additionalDefaults;
        }
        else
        {
          foreach (KeyValuePair<string, string> additionalDefault in this.owner.additionalDefaults)
          {
            if (!extraParameters.ContainsKey(additionalDefault.Key))
              extraParameters.Add(additionalDefault.Key, additionalDefault.Value);
          }
        }
      }

      private void LoadDefaultsAndValidate(string[] normalizedParameters, out int lastNonDefaultPathParameter, out int lastNonNullablePathParameter)
      {
        for (int index = 0; index < this.pathSegmentVariableNames.Count; ++index)
        {
          if (string.IsNullOrEmpty(normalizedParameters[index]) && this.defaultValues != null)
            this.defaultValues.TryGetValue(this.pathSegmentVariableNames[index], out normalizedParameters[index]);
        }
        lastNonDefaultPathParameter = this.pathSegmentVariableNames.Count - 1;
        if (this.defaultValues != null && this.owner.segments[this.owner.segments.Count - 1].Nature != UriTemplatePartType.Literal)
        {
          bool flag = false;
          while (!flag && lastNonDefaultPathParameter >= 0)
          {
            string strB;
            if (this.defaultValues.TryGetValue(this.pathSegmentVariableNames[lastNonDefaultPathParameter], out strB))
            {
              if (string.Compare(normalizedParameters[lastNonDefaultPathParameter], strB, StringComparison.Ordinal) != 0)
                flag = true;
              else
                lastNonDefaultPathParameter = lastNonDefaultPathParameter - 1;
            }
            else
              flag = true;
          }
        }
        lastNonNullablePathParameter = this.firstNullablePathVariable <= lastNonDefaultPathParameter ? lastNonDefaultPathParameter : this.firstNullablePathVariable - 1;
        for (int index = 0; index <= lastNonNullablePathParameter; ++index)
        {
          if ((!this.owner.HasWildcard || !this.owner.wildcard.HasVariable || index != this.pathSegmentVariableNames.Count - 1) && string.IsNullOrEmpty(normalizedParameters[index]))
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("parameters", SR.GetString("BindUriTemplateToNullOrEmptyPathParam", new object[1]
            {
              (object) this.pathSegmentVariableNames[index]
            }));
        }
      }

      private void ParseVariableDeclaration(string varDeclaration, out string varName, out string defaultValue)
      {
        if (varDeclaration.IndexOf('{') != -1 || varDeclaration.IndexOf('}') != -1)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new FormatException(SR.GetString("UTInvalidVarDeclaration", (object) this.owner.originalTemplate, (object) varDeclaration)));
        int length = varDeclaration.IndexOf('=');
        switch (length)
        {
          case -1:
            varName = varDeclaration;
            defaultValue = (string) null;
            break;
          case 0:
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new FormatException(SR.GetString("UTInvalidVarDeclaration", (object) this.owner.originalTemplate, (object) varDeclaration)));
          default:
            varName = varDeclaration.Substring(0, length);
            defaultValue = varDeclaration.Substring(length + 1);
            if (defaultValue.IndexOf('=') == -1)
              break;
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new FormatException(SR.GetString("UTInvalidVarDeclaration", (object) this.owner.originalTemplate, (object) varDeclaration)));
        }
      }

      private string[] PrepareNormalizedParameters()
      {
        string[] strArray = new string[this.pathSegmentVariableNames.Count + this.queryValueVariableNames.Count];
        for (int index = 0; index < strArray.Length; ++index)
          strArray[index] = (string) null;
        return strArray;
      }

      private void ProcessBindParameter(string name, string value, string[] normalizedParameters, ref IDictionary<string, string> extraParameters)
      {
        if (string.IsNullOrEmpty(name))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("parameters", SR.GetString("UTBindByNameCalledWithEmptyKey"));
        string upperInvariant = name.ToUpperInvariant();
        int index = this.pathSegmentVariableNames.IndexOf(upperInvariant);
        if (index != -1)
        {
          normalizedParameters[index] = string.IsNullOrEmpty(value) ? string.Empty : value;
        }
        else
        {
          int num = this.queryValueVariableNames.IndexOf(upperInvariant);
          if (num != -1)
          {
            normalizedParameters[this.pathSegmentVariableNames.Count + num] = string.IsNullOrEmpty(value) ? string.Empty : value;
          }
          else
          {
            if (extraParameters == null)
              extraParameters = (IDictionary<string, string>) new Dictionary<string, string>(UriTemplateHelpers.GetQueryKeyComparer());
            extraParameters.Add(name, value);
          }
        }
      }

      private void ProcessDefaultsAndCreateBindInfo(bool omitDefaults, string[] normalizedParameters, IDictionary<string, string> extraParameters, out UriTemplate.BindInformation bindInfo)
      {
        int lastNonDefaultPathParameter;
        int lastNonNullablePathParameter;
        this.LoadDefaultsAndValidate(normalizedParameters, out lastNonDefaultPathParameter, out lastNonNullablePathParameter);
        if (this.owner.additionalDefaults != null)
        {
          if (omitDefaults)
            this.RemoveAdditionalDefaults(ref extraParameters);
          else
            this.AddAdditionalDefaults(ref extraParameters);
        }
        bindInfo = new UriTemplate.BindInformation(normalizedParameters, lastNonDefaultPathParameter, lastNonNullablePathParameter, extraParameters);
      }

      private void RemoveAdditionalDefaults(ref IDictionary<string, string> extraParameters)
      {
        if (extraParameters == null)
          return;
        foreach (KeyValuePair<string, string> additionalDefault in this.owner.additionalDefaults)
        {
          string strA;
          if (extraParameters.TryGetValue(additionalDefault.Key, out strA) && string.Compare(strA, additionalDefault.Value, StringComparison.Ordinal) == 0)
            extraParameters.Remove(additionalDefault.Key);
        }
        if (extraParameters.Count != 0)
          return;
        extraParameters = (IDictionary<string, string>) null;
      }
    }

    private class WildcardInfo
    {
      private readonly UriTemplate owner;
      private readonly string varName;

      internal bool HasVariable
      {
        get
        {
          return !string.IsNullOrEmpty(this.varName);
        }
      }

      public WildcardInfo(UriTemplate owner)
      {
        this.varName = (string) null;
        this.owner = owner;
      }

      public WildcardInfo(UriTemplate owner, string segment)
      {
        bool hasDefaultValue;
        this.varName = owner.AddPathVariable(UriTemplatePartType.Variable, segment.Substring(1 + "*".Length, segment.Length - 2 - "*".Length), out hasDefaultValue);
        if (hasDefaultValue)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTStarVariableWithDefaults", (object) owner.originalTemplate, (object) segment, (object) this.varName)));
        this.owner = owner;
      }

      public void Bind(string[] values, ref int valueIndex, StringBuilder path)
      {
        if (!this.HasVariable)
          return;
        if (string.IsNullOrEmpty(values[valueIndex]))
        {
          valueIndex = valueIndex + 1;
        }
        else
        {
#if FEATURE_CORECLR
          throw new NotImplementedException("Explicit reference not supported in .NET Core");
#else
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
#endif
        }
      }

      public void Lookup(int numMatchedSegments, Collection<string> relativePathSegments, NameValueCollection boundParameters)
      {
        if (!this.HasVariable)
          return;
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = numMatchedSegments; index < relativePathSegments.Count; ++index)
        {
          if (index < relativePathSegments.Count - 1)
            stringBuilder.AppendFormat("{0}/", (object) relativePathSegments[index]);
          else
            stringBuilder.Append(relativePathSegments[index]);
        }
        boundParameters.Add(this.varName, stringBuilder.ToString());
      }
    }
  }
}
