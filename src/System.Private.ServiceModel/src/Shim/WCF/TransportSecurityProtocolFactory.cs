// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.TransportSecurityProtocolFactory
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

namespace System.ServiceModel.Security
{
  internal class TransportSecurityProtocolFactory : SecurityProtocolFactory
  {
    public override bool SupportsDuplex
    {
      get
      {
        return true;
      }
    }

#if FEATURE_CORECLR
    public bool SupportsReplayDetection
#else
    public override bool SupportsReplayDetection
#endif
    {
      get
      {
        return false;
      }
    }

    public TransportSecurityProtocolFactory()
    {
    }

    internal TransportSecurityProtocolFactory(TransportSecurityProtocolFactory factory)
      : base((SecurityProtocolFactory) factory)
    {
    }

    protected override SecurityProtocol OnCreateSecurityProtocol(EndpointAddress target, Uri via, object listenerSecurityState, TimeSpan timeout)
    {
      return (SecurityProtocol) new TransportSecurityProtocol(this, target, via);
    }
  }
}
