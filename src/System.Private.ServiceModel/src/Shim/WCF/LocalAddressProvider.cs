// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.LocalAddressProvider
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Channels
{
  internal class LocalAddressProvider
  {
    private EndpointAddress localAddress;
    private MessageFilter filter;
    private int priority;

    public EndpointAddress LocalAddress
    {
      get
      {
        return this.localAddress;
      }
    }

    public MessageFilter Filter
    {
      get
      {
        return this.filter;
      }
    }

    public int Priority
    {
      get
      {
        return this.priority;
      }
    }

    public LocalAddressProvider(EndpointAddress localAddress, MessageFilter filter)
    {
      if (localAddress == (EndpointAddress) null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("localAddress");
      if (filter == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("filter");
      this.localAddress = localAddress;
      this.filter = filter;
      if (localAddress.Headers.FindHeader(XD.UtilityDictionary.UniqueEndpointHeaderName.Value, XD.UtilityDictionary.UniqueEndpointHeaderNamespace.Value) == null)
        this.priority = 2147483646;
      else
        this.priority = int.MaxValue;
    }
  }
}
