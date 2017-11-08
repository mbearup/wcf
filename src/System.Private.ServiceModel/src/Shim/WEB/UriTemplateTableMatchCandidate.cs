// Decompiled with JetBrains decompiler
// Type: System.UriTemplateTableMatchCandidate
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

namespace System
{
  internal struct UriTemplateTableMatchCandidate
  {
    private readonly object data;
    private readonly int segmentsCount;
    private readonly UriTemplate template;

    public object Data
    {
      get
      {
        return this.data;
      }
    }

    public int SegmentsCount
    {
      get
      {
        return this.segmentsCount;
      }
    }

    public UriTemplate Template
    {
      get
      {
        return this.template;
      }
    }

    public UriTemplateTableMatchCandidate(UriTemplate template, int segmentsCount, object data)
    {
      this.template = template;
      this.segmentsCount = segmentsCount;
      this.data = data;
    }
  }
}
