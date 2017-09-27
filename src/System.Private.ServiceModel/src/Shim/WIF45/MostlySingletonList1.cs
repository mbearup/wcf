// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.MostlySingletonList`1
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Collections.Generic;
using System.ServiceModel;

namespace System.IdentityModel
{
  internal struct MostlySingletonList<T> where T : class
  {
    private int count;
    private T singleton;
    private List<T> list;

    public T this[int index]
    {
      get
      {
        if (this.list != null)
          return this.list[index];
        this.EnsureValidSingletonIndex(index);
        return this.singleton;
      }
    }

    public int Count
    {
      get
      {
        return this.count;
      }
    }

    public void Add(T item)
    {
      if (this.list == null)
      {
        if (this.count == 0)
        {
          this.singleton = item;
          this.count = 1;
          return;
        }
        this.list = new List<T>();
        this.list.Add(this.singleton);
        this.singleton = default (T);
      }
      this.list.Add(item);
      this.count = this.count + 1;
    }

    private static bool Compare(T x, T y)
    {
      if ((object) x != null)
        return x.Equals((object) y);
      return (object) y == null;
    }

    public bool Contains(T item)
    {
      return this.IndexOf(item) >= 0;
    }

    private void EnsureValidSingletonIndex(int index)
    {
      if (this.count != 1)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("count", SR.GetString("ValueMustBeOne")));
      if (index != 0)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("index", SR.GetString("ValueMustBeZero")));
    }

    private bool MatchesSingleton(T item)
    {
      if (this.count == 1)
        return MostlySingletonList<T>.Compare(this.singleton, item);
      return false;
    }

    public int IndexOf(T item)
    {
      if (this.list != null)
        return this.list.IndexOf(item);
      return !this.MatchesSingleton(item) ? -1 : 0;
    }

    public bool Remove(T item)
    {
      if (this.list == null)
      {
        if (!this.MatchesSingleton(item))
          return false;
        this.singleton = default (T);
        this.count = 0;
        return true;
      }
      bool flag = this.list.Remove(item);
      if (flag)
        this.count = this.count - 1;
      return flag;
    }

    public void RemoveAt(int index)
    {
      if (this.list == null)
      {
        this.EnsureValidSingletonIndex(index);
        this.singleton = default (T);
        this.count = 0;
      }
      else
      {
        this.list.RemoveAt(index);
        this.count = this.count - 1;
      }
    }
  }
}
