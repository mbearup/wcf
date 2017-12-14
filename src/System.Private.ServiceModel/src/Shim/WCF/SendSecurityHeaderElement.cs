// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SendSecurityHeaderElement
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.IdentityModel;

namespace System.ServiceModel.Security
{
  internal class SendSecurityHeaderElement
  {
    private string id;
    private ISecurityElement item;
    private bool markedForEncryption;

    public string Id
    {
      get
      {
        return this.id;
      }
    }

    public ISecurityElement Item
    {
      get
      {
        return this.item;
      }
    }

    public bool MarkedForEncryption
    {
      get
      {
        return this.markedForEncryption;
      }
      set
      {
        this.markedForEncryption = value;
      }
    }

    public SendSecurityHeaderElement(string id, ISecurityElement item)
    {
      this.id = id;
      this.item = item;
      this.markedForEncryption = false;
    }

    public bool IsSameItem(ISecurityElement item)
    {
      if (this.item != item)
        return this.item.Equals((object) item);
      return true;
    }

    public void Replace(string id, ISecurityElement item)
    {
      this.item = item;
      this.id = id;
    }
  }
}
