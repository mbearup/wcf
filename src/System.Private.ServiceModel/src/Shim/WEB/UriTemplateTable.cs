// Decompiled with JetBrains decompiler
// Type: System.UriTemplateTable
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.ServiceModel;

namespace System
{
  /// <summary>A class that represents an associative set of <see cref="T:System.UriTemplate" /> objects.</summary>
  [TypeForwardedFrom("System.ServiceModel.Web, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
  public class UriTemplateTable
  {
    private Uri baseAddress;
    private string basePath;
    private Dictionary<string, UriTemplateTable.FastPathInfo> fastPathTable;
    private bool noTemplateHasQueryPart;
    private int numSegmentsInBaseAddress;
    private Uri originalUncanonicalizedBaseAddress;
#if !FEATURE_CORECLR
    private UriTemplateTrieNode rootNode;
#endif
    private UriTemplateTable.UriTemplatesCollection templates;
    private object thisLock;
    private bool addTrailingSlashToBaseAddress;

    /// <summary>Gets and sets the base address for the <see cref="T:System.UriTemplateTable" /> instance.</summary>
    /// <returns>A <see cref="T:System.Uri" /> that contains the base address.</returns>
    public Uri BaseAddress
    {
      get
      {
        return this.baseAddress;
      }
      set
      {
        if (value == (Uri) null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
        lock (this.thisLock)
        {
          if (this.IsReadOnly)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTTCannotChangeBaseAddress")));
          if (!value.IsAbsoluteUri)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", SR.GetString("UTTBaseAddressMustBeAbsolute"));
          this.originalUncanonicalizedBaseAddress = value;
          this.baseAddress = value;
          this.NormalizeBaseAddress();
        }
      }
    }

    /// <summary>Gets the original base address.</summary>
    /// <returns>The original base address.</returns>
    public Uri OriginalBaseAddress
    {
      get
      {
        return this.originalUncanonicalizedBaseAddress;
      }
    }

    /// <summary>Gets a value that specifies whether the <see cref="T:System.UriTemplateTable" /> is read only.</summary>
    /// <returns>true if the <see cref="T:System.UriTemplateTable" /> property is read only; otherwise false.</returns>
    public bool IsReadOnly
    {
      get
      {
        return this.templates.IsFrozen;
      }
    }

    /// <summary>Gets a collection of key/value pairs that consist of <see cref="T:System.UriTemplate" /> objects and their associated data.</summary>
    /// <returns>A collection of key/value pairs that consist of <see cref="T:System.UriTemplate" /> objects and their associated data.</returns>
    public IList<KeyValuePair<UriTemplate, object>> KeyValuePairs
    {
      get
      {
        return (IList<KeyValuePair<UriTemplate, object>>) this.templates;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.UriTemplateTable" /> class.</summary>
    public UriTemplateTable()
      : this((Uri) null, (IEnumerable<KeyValuePair<UriTemplate, object>>) null, true)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.UriTemplateTable" /> class with the specified collection of key/value pairs.</summary>
    /// <param name="keyValuePairs">A collection of key/value pairs that consist of URI templates and associated data.</param>
    public UriTemplateTable(IEnumerable<KeyValuePair<UriTemplate, object>> keyValuePairs)
      : this((Uri) null, keyValuePairs, true)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.UriTemplateTable" /> class with the specified base address.</summary>
    /// <param name="baseAddress">A <see cref="T:System.Uri" /> instance that contains the base address.</param>
    public UriTemplateTable(Uri baseAddress)
      : this(baseAddress, (IEnumerable<KeyValuePair<UriTemplate, object>>) null, true)
    {
    }

    internal UriTemplateTable(Uri baseAddress, bool addTrailingSlashToBaseAddress)
      : this(baseAddress, (IEnumerable<KeyValuePair<UriTemplate, object>>) null, addTrailingSlashToBaseAddress)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.UriTemplateTable" /> class with the specified base address and collection of key/value pairs.</summary>
    /// <param name="baseAddress">A <see cref="T:System.Uri" /> instance that contains the base address.</param>
    /// <param name="keyValuePairs">A collection of key/value pairs that consist of URI templates and associated data.</param>
    public UriTemplateTable(Uri baseAddress, IEnumerable<KeyValuePair<UriTemplate, object>> keyValuePairs)
      : this(baseAddress, keyValuePairs, true)
    {
    }

    internal UriTemplateTable(Uri baseAddress, IEnumerable<KeyValuePair<UriTemplate, object>> keyValuePairs, bool addTrailingSlashToBaseAddress)
    {
      if (baseAddress != (Uri) null && !baseAddress.IsAbsoluteUri)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("baseAddress", SR.GetString("UTTMustBeAbsolute"));
      this.addTrailingSlashToBaseAddress = addTrailingSlashToBaseAddress;
      this.originalUncanonicalizedBaseAddress = baseAddress;
      this.templates = keyValuePairs == null ? new UriTemplateTable.UriTemplatesCollection() : new UriTemplateTable.UriTemplatesCollection(keyValuePairs);
      this.thisLock = new object();
      this.baseAddress = baseAddress;
      this.NormalizeBaseAddress();
    }

    /// <summary>Makes the <see cref="T:System.UriTemplateTable" /> read only.</summary>
    /// <param name="allowDuplicateEquivalentUriTemplates">Specifies whether to allow duplicate equivalent <see cref="T:System.UriTemplate" /> instances in the <see cref="T:System.UriTemplateTable" />.</param>
    public void MakeReadOnly(bool allowDuplicateEquivalentUriTemplates)
    {
      lock (this.thisLock)
      {
        if (this.IsReadOnly)
          return;
        this.templates.Freeze();
        this.Validate(allowDuplicateEquivalentUriTemplates);
        this.ConstructFastPathTable();
      }
    }

    /// <summary>Attempts to match a candidate <see cref="T:System.Uri" /> to the <see cref="T:System.UriTemplateTable" />.</summary>
    /// <param name="uri">The candidate URI.</param>
    /// <returns>A collection of <see cref="T:System.UriTemplateMatch" /> instances.</returns>
    public Collection<UriTemplateMatch> Match(Uri uri)
    {
      if (uri == (Uri) null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("uri");
      if (!uri.IsAbsoluteUri)
        return UriTemplateTable.None();
      this.MakeReadOnly(true);
      Collection<string> relativePathSegments;
      IList<UriTemplateTableMatchCandidate> candidates;
      if (!this.FastComputeRelativeSegmentsAndLookup(uri, out relativePathSegments, out candidates))
        return UriTemplateTable.None();
      NameValueCollection nameValueCollection = (NameValueCollection) null;
      if (!this.noTemplateHasQueryPart && UriTemplateTable.AtLeastOneCandidateHasQueryPart(candidates))
      {
        Collection<UriTemplateTableMatchCandidate> collection = new Collection<UriTemplateTableMatchCandidate>();
        nameValueCollection = UriTemplateHelpers.ParseQueryString(uri.Query);
        bool mustBeEspeciallyInteresting = UriTemplateTable.NoCandidateHasQueryLiteralRequirementsAndThereIsAnEmptyFallback(candidates);
        for (int index = 0; index < candidates.Count; ++index)
        {
          if (UriTemplateHelpers.CanMatchQueryInterestingly(candidates[index].Template, nameValueCollection, mustBeEspeciallyInteresting))
            collection.Add(candidates[index]);
        }
        int count1 = collection.Count;
        if (collection.Count == 0)
        {
          for (int index = 0; index < candidates.Count; ++index)
          {
            if (UriTemplateHelpers.CanMatchQueryTrivially(candidates[index].Template))
              collection.Add(candidates[index]);
          }
        }
        if (collection.Count == 0)
          return UriTemplateTable.None();
        int count2 = collection.Count;
        candidates = (IList<UriTemplateTableMatchCandidate>) collection;
      }
      if (UriTemplateTable.NotAllCandidatesArePathFullyEquivalent(candidates))
      {
        Collection<UriTemplateTableMatchCandidate> collection = new Collection<UriTemplateTableMatchCandidate>();
        int num = -1;
        for (int index = 0; index < candidates.Count; ++index)
        {
          UriTemplateTableMatchCandidate tableMatchCandidate = candidates[index];
          if (num == -1)
          {
            num = tableMatchCandidate.Template.segments.Count;
            collection.Add(tableMatchCandidate);
          }
          else if (tableMatchCandidate.Template.segments.Count < num)
          {
            num = tableMatchCandidate.Template.segments.Count;
            collection.Clear();
            collection.Add(tableMatchCandidate);
          }
          else if (tableMatchCandidate.Template.segments.Count == num)
            collection.Add(tableMatchCandidate);
        }
        candidates = (IList<UriTemplateTableMatchCandidate>) collection;
      }
      Collection<UriTemplateMatch> collection1 = new Collection<UriTemplateMatch>();
      for (int index = 0; index < candidates.Count; ++index)
      {
        UriTemplateTableMatchCandidate tableMatchCandidate = candidates[index];
        UriTemplateMatch uriTemplateMatch = tableMatchCandidate.Template.CreateUriTemplateMatch(this.originalUncanonicalizedBaseAddress, uri, tableMatchCandidate.Data, tableMatchCandidate.SegmentsCount, relativePathSegments, nameValueCollection);
        collection1.Add(uriTemplateMatch);
      }
      return collection1;
    }

    /// <summary>Attempts to match a candidate <see cref="T:System.Uri" /> to the <see cref="T:System.UriTemplateTable" />.</summary>
    /// <param name="uri">The candidate URI.</param>
    /// <returns>A single <see cref="T:System.UriTemplateMatch" /> instance.</returns>
    public UriTemplateMatch MatchSingle(Uri uri)
    {
      Collection<UriTemplateMatch> collection = this.Match(uri);
      if (collection.Count == 0)
        return (UriTemplateMatch) null;
      if (collection.Count == 1)
        return collection[0];
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new UriTemplateMatchException(SR.GetString("UTTMultipleMatches")));
    }

    private static bool AllEquivalent(IList<UriTemplateTableMatchCandidate> list, int a, int b)
    {
      for (int index = a; index < b - 1; ++index)
      {
        if (!list[index].Template.IsPathPartiallyEquivalentAt(list[index + 1].Template, list[index].SegmentsCount) || !list[index].Template.IsQueryEquivalent(list[index + 1].Template))
          return false;
      }
      return true;
    }

    private static bool AtLeastOneCandidateHasQueryPart(IList<UriTemplateTableMatchCandidate> candidates)
    {
      for (int index = 0; index < candidates.Count; ++index)
      {
        if (!UriTemplateHelpers.CanMatchQueryTrivially(candidates[index].Template))
          return true;
      }
      return false;
    }

    private static bool NoCandidateHasQueryLiteralRequirementsAndThereIsAnEmptyFallback(IList<UriTemplateTableMatchCandidate> candidates)
    {
      bool flag = false;
      for (int index = 0; index < candidates.Count; ++index)
      {
        UriTemplateTableMatchCandidate candidate = candidates[index];
        if (UriTemplateHelpers.HasQueryLiteralRequirements(candidate.Template))
          return false;
        candidate = candidates[index];
        if (candidate.Template.queries.Count == 0)
          flag = true;
      }
      return flag;
    }

    private static Collection<UriTemplateMatch> None()
    {
      return new Collection<UriTemplateMatch>();
    }

    private static bool NotAllCandidatesArePathFullyEquivalent(IList<UriTemplateTableMatchCandidate> candidates)
    {
      if (candidates.Count <= 1)
        return false;
      int num1 = -1;
      UriTemplateTableMatchCandidate candidate;
      for (int index = 0; index < candidates.Count; ++index)
      {
        if (num1 == -1)
        {
          candidate = candidates[index];
          num1 = candidate.Template.segments.Count;
        }
        else
        {
          int num2 = num1;
          candidate = candidates[index];
          int count = candidate.Template.segments.Count;
          if (num2 != count)
            return true;
        }
      }
      return false;
    }

    private bool ComputeRelativeSegmentsAndLookup(Uri uri, ICollection<string> relativePathSegments, ICollection<UriTemplateTableMatchCandidate> candidates)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("UriTemplateTrieNode is not supported in .NET Core");
#else
      string[] segments = uri.Segments;
      int length = segments.Length - this.numSegmentsInBaseAddress;
      UriTemplateLiteralPathSegment[] wireData = new UriTemplateLiteralPathSegment[length];
      for (int index = 0; index < length; ++index)
      {
        string str1 = segments[index + this.numSegmentsInBaseAddress];
        UriTemplateLiteralPathSegment fromWireData = UriTemplateLiteralPathSegment.CreateFromWireData(str1);
        wireData[index] = fromWireData;
        string str2 = Uri.UnescapeDataString(str1);
        if (fromWireData.EndsWithSlash)
          str2 = str2.Substring(0, str2.Length - 1);
        relativePathSegments.Add(str2);
      }
      return this.rootNode.Match(wireData, candidates);
#endif
    }

    private void ConstructFastPathTable()
    {
      this.noTemplateHasQueryPart = true;
      foreach (KeyValuePair<UriTemplate, object> template in (Collection<KeyValuePair<UriTemplate, object>>) this.templates)
      {
        UriTemplate key = template.Key;
        if (!UriTemplateHelpers.CanMatchQueryTrivially(key))
          this.noTemplateHasQueryPart = false;
        if (key.HasNoVariables && !key.HasWildcard)
        {
          if (this.fastPathTable == null)
            this.fastPathTable = new Dictionary<string, UriTemplateTable.FastPathInfo>();
          Uri uri = key.BindByPosition(this.originalUncanonicalizedBaseAddress);
          string uriPath = UriTemplateHelpers.GetUriPath(uri);
          if (!this.fastPathTable.ContainsKey(uriPath))
          {
            UriTemplateTable.FastPathInfo fastPathInfo = new UriTemplateTable.FastPathInfo();
            if (this.ComputeRelativeSegmentsAndLookup(uri, (ICollection<string>) fastPathInfo.RelativePathSegments, (ICollection<UriTemplateTableMatchCandidate>) fastPathInfo.Candidates))
            {
              fastPathInfo.Freeze();
              this.fastPathTable.Add(uriPath, fastPathInfo);
            }
          }
        }
      }
    }

    private bool FastComputeRelativeSegmentsAndLookup(Uri uri, out Collection<string> relativePathSegments, out IList<UriTemplateTableMatchCandidate> candidates)
    {
      string uriPath = UriTemplateHelpers.GetUriPath(uri);
      UriTemplateTable.FastPathInfo fastPathInfo = (UriTemplateTable.FastPathInfo) null;
      if (this.fastPathTable != null && this.fastPathTable.TryGetValue(uriPath, out fastPathInfo))
      {
        relativePathSegments = fastPathInfo.RelativePathSegments;
        candidates = (IList<UriTemplateTableMatchCandidate>) fastPathInfo.Candidates;
        return true;
      }
      relativePathSegments = new Collection<string>();
      candidates = (IList<UriTemplateTableMatchCandidate>) new Collection<UriTemplateTableMatchCandidate>();
      return this.SlowComputeRelativeSegmentsAndLookup(uri, uriPath, relativePathSegments, (ICollection<UriTemplateTableMatchCandidate>) candidates);
    }

    private void NormalizeBaseAddress()
    {
      if (!(this.baseAddress != (Uri) null))
        return;
      UriBuilder uriBuilder = new UriBuilder(this.baseAddress);
      if (this.addTrailingSlashToBaseAddress && !uriBuilder.Path.EndsWith("/", StringComparison.Ordinal))
        uriBuilder.Path = uriBuilder.Path + "/";
      uriBuilder.Host = "localhost";
      uriBuilder.Port = -1;
      uriBuilder.UserName = (string) null;
      uriBuilder.Password = (string) null;
      uriBuilder.Path = uriBuilder.Path.ToUpperInvariant();
      uriBuilder.Scheme = Uri.UriSchemeHttp;
      this.baseAddress = uriBuilder.Uri;
      this.basePath = UriTemplateHelpers.GetUriPath(this.baseAddress);
    }

    private bool SlowComputeRelativeSegmentsAndLookup(Uri uri, string uriPath, Collection<string> relativePathSegments, ICollection<UriTemplateTableMatchCandidate> candidates)
    {
      if (uriPath.Length < this.basePath.Length || !uriPath.StartsWith(this.basePath, StringComparison.OrdinalIgnoreCase) || uriPath.Length > this.basePath.Length && !this.basePath.EndsWith("/", StringComparison.Ordinal) && (int) uriPath[this.basePath.Length] != 47)
        return false;
      return this.ComputeRelativeSegmentsAndLookup(uri, (ICollection<string>) relativePathSegments, candidates);
    }

    private void Validate(bool allowDuplicateEquivalentUriTemplates)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("UriTemplateTrieNode is not supported in .NET Core");
#else
      if (this.baseAddress == (Uri) null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTTBaseAddressNotSet")));
      this.numSegmentsInBaseAddress = this.baseAddress.Segments.Length;
      if (this.templates.Count == 0)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("UTTEmptyKeyValuePairs")));
      this.rootNode = UriTemplateTrieNode.Make((IEnumerable<KeyValuePair<UriTemplate, object>>) this.templates, allowDuplicateEquivalentUriTemplates);
#endif
    }

    [Conditional("DEBUG")]
    private void VerifyThatFastPathAndSlowPathHaveSameResults(Uri uri, Collection<string> fastPathRelativePathSegments, IList<UriTemplateTableMatchCandidate> fastPathCandidates)
    {
      Collection<string> relativePathSegments = new Collection<string>();
      List<UriTemplateTableMatchCandidate> tableMatchCandidateList = new List<UriTemplateTableMatchCandidate>();
      this.SlowComputeRelativeSegmentsAndLookup(uri, UriTemplateHelpers.GetUriPath(uri), relativePathSegments, (ICollection<UriTemplateTableMatchCandidate>) tableMatchCandidateList);
      int count1 = fastPathRelativePathSegments.Count;
      int count2 = relativePathSegments.Count;
      for (int index = 0; index < fastPathRelativePathSegments.Count; ++index)
      {
        int num = fastPathRelativePathSegments[index] != relativePathSegments[index] ? 1 : 0;
      }
      int count3 = fastPathCandidates.Count;
      int count4 = tableMatchCandidateList.Count;
      for (int index = 0; index < fastPathCandidates.Count; ++index)
        tableMatchCandidateList.Contains(fastPathCandidates[index]);
    }

    private class FastPathInfo
    {
      private FreezableCollection<UriTemplateTableMatchCandidate> candidates;
      private FreezableCollection<string> relativePathSegments;

      public Collection<UriTemplateTableMatchCandidate> Candidates
      {
        get
        {
          return (Collection<UriTemplateTableMatchCandidate>) this.candidates;
        }
      }

      public Collection<string> RelativePathSegments
      {
        get
        {
          return (Collection<string>) this.relativePathSegments;
        }
      }

      public FastPathInfo()
      {
        this.relativePathSegments = new FreezableCollection<string>();
        this.candidates = new FreezableCollection<UriTemplateTableMatchCandidate>();
      }

      public void Freeze()
      {
        this.relativePathSegments.Freeze();
        this.candidates.Freeze();
      }
    }

    private class UriTemplatesCollection : FreezableCollection<KeyValuePair<UriTemplate, object>>
    {
      public UriTemplatesCollection()
      {
      }

      public UriTemplatesCollection(IEnumerable<KeyValuePair<UriTemplate, object>> keyValuePairs)
      {
        foreach (KeyValuePair<UriTemplate, object> keyValuePair in keyValuePairs)
        {
          UriTemplateTable.UriTemplatesCollection.ThrowIfInvalid(keyValuePair.Key, "keyValuePairs");
          this.Add(keyValuePair);
        }
      }

      protected override void InsertItem(int index, KeyValuePair<UriTemplate, object> item)
      {
        UriTemplateTable.UriTemplatesCollection.ThrowIfInvalid(item.Key, "item");
        base.InsertItem(index, item);
      }

      protected override void SetItem(int index, KeyValuePair<UriTemplate, object> item)
      {
        UriTemplateTable.UriTemplatesCollection.ThrowIfInvalid(item.Key, "item");
        base.SetItem(index, item);
      }

      private static void ThrowIfInvalid(UriTemplate template, string argName)
      {
        if (template == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(argName, SR.GetString("UTTNullTemplateKey"));
        if (template.IgnoreTrailingSlash)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(argName, SR.GetString("UTTInvalidTemplateKey", new object[1]
          {
            (object) template
          }));
      }
    }
  }
}
