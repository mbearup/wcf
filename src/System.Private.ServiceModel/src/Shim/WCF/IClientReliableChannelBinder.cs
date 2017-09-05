// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.IClientReliableChannelBinder
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

namespace System.ServiceModel.Channels
{
  internal interface IClientReliableChannelBinder : IReliableChannelBinder
  {
    Uri Via { get; }

    event EventHandler ConnectionLost;

    bool EnsureChannelForRequest();

    IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state);

    IAsyncResult BeginRequest(Message message, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state);

    Message EndRequest(IAsyncResult result);

    Message Request(Message message, TimeSpan timeout);

    Message Request(Message message, TimeSpan timeout, MaskingMode maskingMode);
  }
}
