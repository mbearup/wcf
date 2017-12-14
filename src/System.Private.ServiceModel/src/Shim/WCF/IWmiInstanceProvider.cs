// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Administration.IWmiInstanceProvider
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

namespace System.ServiceModel.Administration
{
  internal interface IWmiInstanceProvider
  {
    string GetInstanceType();

    void FillInstance(IWmiInstance wmiInstance);
  }
}
