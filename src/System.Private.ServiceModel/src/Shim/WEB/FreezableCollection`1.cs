// Decompiled with JetBrains decompiler
// Type: System.Collections.ObjectModel.FreezableCollection`1
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.ServiceModel;

namespace System.Collections.ObjectModel
{
  internal class FreezableCollection<T> : Collection<T>, ICollection<T>, IEnumerable<T>, IEnumerable
  {
    private bool frozen;

    public bool IsFrozen
    {
      get
      {
        return this.frozen;
      }
    }

    bool ICollection<T>.IsReadOnly
    {
      get
      {
        return this.frozen;
      }
    }

    public FreezableCollection()
    {
    }

    public FreezableCollection(IList<T> list)
      : base(list)
    {
    }

    public void Freeze()
    {
      this.frozen = true;
    }

    protected override void ClearItems()
    {
      this.ThrowIfFrozen();
      base.ClearItems();
    }

    protected override void InsertItem(int index, T item)
    {
      this.ThrowIfFrozen();
      base.InsertItem(index, item);
    }

    protected override void RemoveItem(int index)
    {
      this.ThrowIfFrozen();
      base.RemoveItem(index);
    }

    protected override void SetItem(int index, T item)
    {
      this.ThrowIfFrozen();
      base.SetItem(index, item);
    }

    private void ThrowIfFrozen()
    {
      if (this.frozen)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException("ObjectIsReadOnly"));
    }
  }
}
