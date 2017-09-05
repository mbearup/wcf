// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.IChannelSecureConversationSessionSettings
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

namespace System.ServiceModel.Security
{
  internal interface IChannelSecureConversationSessionSettings
  {
    TimeSpan KeyRenewalInterval { get; set; }

    TimeSpan KeyRolloverInterval { get; set; }

    bool TolerateTransportFailures { get; set; }
  }
}
