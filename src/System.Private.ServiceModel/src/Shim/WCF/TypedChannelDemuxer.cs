// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.TypedChannelDemuxer
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Diagnostics;
using System.Runtime;
using System.ServiceModel.Diagnostics.Application;

namespace System.ServiceModel.Channels
{
  internal abstract class TypedChannelDemuxer
  {
    internal static void AbortMessage(RequestContext request)
    {
      try
      {
        TypedChannelDemuxer.AbortMessage(request.RequestMessage);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          DiagnosticUtility.TraceHandledException(ex, TraceEventType.Information);
      }
    }

    internal static void AbortMessage(Message message)
    {
      try
      {
        message.Close();
      }
      catch (CommunicationException ex)
      {
        DiagnosticUtility.TraceHandledException((Exception) ex, TraceEventType.Information);
      }
      catch (TimeoutException ex)
      {
        if (TD.CloseTimeoutIsEnabled())
          TD.CloseTimeout(ex.Message);
        DiagnosticUtility.TraceHandledException((Exception) ex, TraceEventType.Information);
      }
    }

    public abstract IChannelListener<TChannel> BuildChannelListener<TChannel>(ChannelDemuxerFilter filter) where TChannel : class, IChannel;
  }
}
