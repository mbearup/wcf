// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.DoneReceivingAsyncResult
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Runtime;

namespace System.ServiceModel.Channels
{
  internal class DoneReceivingAsyncResult : CompletedAsyncResult
  {
    internal DoneReceivingAsyncResult(AsyncCallback callback, object state)
      : base(callback, state)
    {
    }

    internal static bool End(DoneReceivingAsyncResult result, out Message message)
    {
      message = (Message) null;
      return true;
    }

    internal static bool End(DoneReceivingAsyncResult result, out RequestContext requestContext)
    {
      requestContext = (RequestContext) null;
      return true;
    }

    internal static bool End(DoneReceivingAsyncResult result)
    {
      return true;
    }
  }
}
