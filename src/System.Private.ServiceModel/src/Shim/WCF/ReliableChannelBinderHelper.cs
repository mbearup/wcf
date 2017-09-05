// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.ReliableChannelBinderHelper
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Runtime;

namespace System.ServiceModel.Channels
{
  internal static class ReliableChannelBinderHelper
  {
    internal static IAsyncResult BeginCloseDuplexSessionChannel(ReliableChannelBinder<IDuplexSessionChannel> binder, IDuplexSessionChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
    {
      return (IAsyncResult) new ReliableChannelBinderHelper.CloseDuplexSessionChannelAsyncResult(binder, channel, timeout, callback, state);
    }

    internal static IAsyncResult BeginCloseReplySessionChannel(ReliableChannelBinder<IReplySessionChannel> binder, IReplySessionChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
    {
      return (IAsyncResult) new ReliableChannelBinderHelper.CloseReplySessionChannelAsyncResult(binder, channel, timeout, callback, state);
    }

    internal static void CloseDuplexSessionChannel(ReliableChannelBinder<IDuplexSessionChannel> binder, IDuplexSessionChannel channel, TimeSpan timeout)
    {
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      channel.Session.CloseOutputSession(timeoutHelper.RemainingTime());
      binder.WaitForPendingOperations(timeoutHelper.RemainingTime());
      TimeSpan timeout1 = timeoutHelper.RemainingTime();
      bool flag1 = timeout1 == TimeSpan.Zero;
      while (true)
      {
        Message message = (Message) null;
        bool flag2 = true;
        try
        {
          bool flag3 = channel.TryReceive(timeout1, out message);
          flag2 = false;
          if (flag3)
          {
            if (message == null)
            {
              channel.Close(timeoutHelper.RemainingTime());
              return;
            }
          }
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else if (flag2)
          {
            if (!ReliableChannelBinderHelper.MaskHandled(binder.DefaultMaskingMode) || !binder.IsHandleable(ex))
              throw;
            else
              flag2 = false;
          }
          else
            throw;
        }
        finally
        {
          if (message != null)
            message.Close();
          if (flag2)
            channel.Abort();
        }
        if (!flag1 && channel.State == CommunicationState.Opened)
        {
          timeout1 = timeoutHelper.RemainingTime();
          flag1 = timeout1 == TimeSpan.Zero;
        }
        else
          break;
      }
      channel.Abort();
    }

    internal static void CloseReplySessionChannel(ReliableChannelBinder<IReplySessionChannel> binder, IReplySessionChannel channel, TimeSpan timeout)
    {
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      binder.WaitForPendingOperations(timeoutHelper.RemainingTime());
      TimeSpan timeout1 = timeoutHelper.RemainingTime();
      bool flag1 = timeout1 == TimeSpan.Zero;
      while (true)
      {
        RequestContext context = (RequestContext) null;
        bool flag2 = true;
        try
        {
          bool request = channel.TryReceiveRequest(timeout1, out context);
          flag2 = false;
          if (request)
          {
            if (context == null)
            {
              channel.Close(timeoutHelper.RemainingTime());
              return;
            }
          }
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else if (flag2)
          {
            if (!ReliableChannelBinderHelper.MaskHandled(binder.DefaultMaskingMode) || !binder.IsHandleable(ex))
              throw;
            else
              flag2 = false;
          }
          else
            throw;
        }
        finally
        {
          if (context != null)
          {
            context.RequestMessage.Close();
            context.Close();
          }
          if (flag2)
            channel.Abort();
        }
        if (!flag1 && channel.State == CommunicationState.Opened)
        {
          timeout1 = timeoutHelper.RemainingTime();
          flag1 = timeout1 == TimeSpan.Zero;
        }
        else
          break;
      }
      channel.Abort();
    }

    internal static void EndCloseDuplexSessionChannel(IDuplexSessionChannel channel, IAsyncResult result)
    {
      ReliableChannelBinderHelper.CloseDuplexSessionChannelAsyncResult.End(result);
    }

    internal static void EndCloseReplySessionChannel(IReplySessionChannel channel, IAsyncResult result)
    {
      ReliableChannelBinderHelper.CloseReplySessionChannelAsyncResult.End(result);
    }

    internal static bool MaskHandled(MaskingMode maskingMode)
    {
      return (maskingMode & MaskingMode.Handled) == MaskingMode.Handled;
    }

    internal static bool MaskUnhandled(MaskingMode maskingMode)
    {
      return (maskingMode & MaskingMode.Unhandled) == MaskingMode.Unhandled;
    }

    private abstract class CloseInputSessionChannelAsyncResult<TChannel, TItem> : AsyncResult where TChannel : class, IChannel where TItem : class
    {
      private static AsyncCallback onChannelCloseCompleteStatic = Fx.ThunkCallback(new AsyncCallback(ReliableChannelBinderHelper.CloseInputSessionChannelAsyncResult<TChannel, TItem>.OnChannelCloseCompleteStatic));
      private static AsyncCallback onInputCompleteStatic = Fx.ThunkCallback(new AsyncCallback(ReliableChannelBinderHelper.CloseInputSessionChannelAsyncResult<TChannel, TItem>.OnInputCompleteStatic));
      private static AsyncCallback onWaitForPendingOperationsCompleteStatic = Fx.ThunkCallback(new AsyncCallback(ReliableChannelBinderHelper.CloseInputSessionChannelAsyncResult<TChannel, TItem>.OnWaitForPendingOperationsCompleteStatic));
      private ReliableChannelBinder<TChannel> binder;
      private TChannel channel;
      private bool lastReceive;
      private TimeoutHelper timeoutHelper;

      protected TChannel Channel
      {
        get
        {
          return this.channel;
        }
      }

      protected TimeSpan RemainingTime
      {
        get
        {
          return this.timeoutHelper.RemainingTime();
        }
      }

      protected CloseInputSessionChannelAsyncResult(ReliableChannelBinder<TChannel> binder, TChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
        : base(callback, state)
      {
        this.binder = binder;
        this.channel = channel;
        this.timeoutHelper = new TimeoutHelper(timeout);
      }

      protected bool Begin()
      {
        bool flag = false;
        IAsyncResult result = this.binder.BeginWaitForPendingOperations(this.RemainingTime, ReliableChannelBinderHelper.CloseInputSessionChannelAsyncResult<TChannel, TItem>.onWaitForPendingOperationsCompleteStatic, (object) this);
        if (result.CompletedSynchronously)
          flag = this.HandleWaitForPendingOperationsComplete(result);
        return flag;
      }

      protected abstract IAsyncResult BeginTryInput(TimeSpan timeout, AsyncCallback callback, object state);

      protected abstract void DisposeItem(TItem item);

      protected abstract bool EndTryInput(IAsyncResult result, out TItem item);

      private void HandleChannelCloseComplete(IAsyncResult result)
      {
        this.channel.EndClose(result);
      }

      private bool HandleInputComplete(IAsyncResult result, out bool gotEof)
      {
        TItem obj = default (TItem);
        bool flag1 = true;
        gotEof = false;
        try
        {
          bool flag2 = this.EndTryInput(result, out obj);
          flag1 = false;
          if (!flag2 || (object) obj != null)
          {
            if (!this.lastReceive && this.channel.State == CommunicationState.Opened)
              return false;
            this.channel.Abort();
            return true;
          }
          gotEof = true;
          result = this.channel.BeginClose(this.RemainingTime, ReliableChannelBinderHelper.CloseInputSessionChannelAsyncResult<TChannel, TItem>.onChannelCloseCompleteStatic, (object) this);
          if (!result.CompletedSynchronously)
            return false;
          this.HandleChannelCloseComplete(result);
          return true;
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else if (flag1)
          {
            if (!ReliableChannelBinderHelper.MaskHandled(this.binder.DefaultMaskingMode) || !this.binder.IsHandleable(ex))
            {
              throw;
            }
            else
            {
              if (!this.lastReceive && this.channel.State == CommunicationState.Opened)
                return false;
              this.channel.Abort();
              return true;
            }
          }
          else
            throw;
        }
        finally
        {
          if ((object) obj != null)
            this.DisposeItem(obj);
          if (flag1)
            this.channel.Abort();
        }
      }

      private bool HandleWaitForPendingOperationsComplete(IAsyncResult result)
      {
        this.binder.EndWaitForPendingOperations(result);
        return this.WaitForEof();
      }

      private static void OnChannelCloseCompleteStatic(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        ReliableChannelBinderHelper.CloseInputSessionChannelAsyncResult<TChannel, TItem> asyncState = (ReliableChannelBinderHelper.CloseInputSessionChannelAsyncResult<TChannel, TItem>) result.AsyncState;
        Exception exception = (Exception) null;
        try
        {
          asyncState.HandleChannelCloseComplete(result);
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else
            exception = ex;
        }
        asyncState.Complete(false, exception);
      }

      private static void OnInputCompleteStatic(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        ReliableChannelBinderHelper.CloseInputSessionChannelAsyncResult<TChannel, TItem> asyncState = (ReliableChannelBinderHelper.CloseInputSessionChannelAsyncResult<TChannel, TItem>) result.AsyncState;
        bool flag = false;
        Exception exception = (Exception) null;
        try
        {
          bool gotEof;
          flag = asyncState.HandleInputComplete(result, out gotEof);
          if (!flag)
          {
            if (!gotEof)
              flag = asyncState.WaitForEof();
          }
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else
            exception = ex;
        }
        if (!flag && exception == null)
          return;
        asyncState.Complete(false, exception);
      }

      private static void OnWaitForPendingOperationsCompleteStatic(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        ReliableChannelBinderHelper.CloseInputSessionChannelAsyncResult<TChannel, TItem> asyncState = (ReliableChannelBinderHelper.CloseInputSessionChannelAsyncResult<TChannel, TItem>) result.AsyncState;
        bool flag = false;
        Exception exception = (Exception) null;
        try
        {
          flag = asyncState.HandleWaitForPendingOperationsComplete(result);
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else
            exception = ex;
        }
        if (!flag && exception == null)
          return;
        asyncState.Complete(false, exception);
      }

      private bool WaitForEof()
      {
        TimeSpan remainingTime = this.RemainingTime;
        this.lastReceive = remainingTime == TimeSpan.Zero;
        bool flag;
        while (true)
        {
          IAsyncResult result = (IAsyncResult) null;
          try
          {
            result = this.BeginTryInput(remainingTime, ReliableChannelBinderHelper.CloseInputSessionChannelAsyncResult<TChannel, TItem>.onInputCompleteStatic, (object) this);
          }
          catch (Exception ex)
          {
            if (Fx.IsFatal(ex))
            {
              throw;
            }
            else
            {
              if (ReliableChannelBinderHelper.MaskHandled(this.binder.DefaultMaskingMode))
              {
                if (this.binder.IsHandleable(ex))
                  goto label_8;
              }
              throw;
            }
          }
label_8:
          if (result != null)
          {
            if (result.CompletedSynchronously)
            {
              bool gotEof;
              flag = this.HandleInputComplete(result, out gotEof);
              if (flag | gotEof)
                break;
            }
            else
              goto label_12;
          }
          if (!this.lastReceive && this.channel.State == CommunicationState.Opened)
          {
            remainingTime = this.RemainingTime;
            this.lastReceive = remainingTime == TimeSpan.Zero;
          }
          else
            goto label_14;
        }
        return flag;
label_12:
        return false;
label_14:
        this.channel.Abort();
        return true;
      }
    }

    private sealed class CloseDuplexSessionChannelAsyncResult : ReliableChannelBinderHelper.CloseInputSessionChannelAsyncResult<IDuplexSessionChannel, Message>
    {
      private static AsyncCallback onCloseOutputSessionCompleteStatic = Fx.ThunkCallback(new AsyncCallback(ReliableChannelBinderHelper.CloseDuplexSessionChannelAsyncResult.OnCloseOutputSessionCompleteStatic));

      public CloseDuplexSessionChannelAsyncResult(ReliableChannelBinder<IDuplexSessionChannel> binder, IDuplexSessionChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
        : base(binder, channel, timeout, callback, state)
      {
        bool flag = false;
        IAsyncResult result = this.Channel.Session.BeginCloseOutputSession(this.RemainingTime, ReliableChannelBinderHelper.CloseDuplexSessionChannelAsyncResult.onCloseOutputSessionCompleteStatic, (object) this);
        if (result.CompletedSynchronously)
          flag = this.HandleCloseOutputSessionComplete(result);
        if (!flag)
          return;
        this.Complete(true);
      }

      protected override IAsyncResult BeginTryInput(TimeSpan timeout, AsyncCallback callback, object state)
      {
        return this.Channel.BeginTryReceive(timeout, callback, state);
      }

      protected override void DisposeItem(Message item)
      {
        item.Close();
      }

      public static void End(IAsyncResult result)
      {
        AsyncResult.End<ReliableChannelBinderHelper.CloseDuplexSessionChannelAsyncResult>(result);
      }

      protected override bool EndTryInput(IAsyncResult result, out Message item)
      {
        return this.Channel.EndTryReceive(result, out item);
      }

      private bool HandleCloseOutputSessionComplete(IAsyncResult result)
      {
        this.Channel.Session.EndCloseOutputSession(result);
        return this.Begin();
      }

      private static void OnCloseOutputSessionCompleteStatic(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        ReliableChannelBinderHelper.CloseDuplexSessionChannelAsyncResult asyncState = (ReliableChannelBinderHelper.CloseDuplexSessionChannelAsyncResult) result.AsyncState;
        bool flag = false;
        Exception exception = (Exception) null;
        try
        {
          flag = asyncState.HandleCloseOutputSessionComplete(result);
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else
            exception = ex;
        }
        if (!flag && exception == null)
          return;
        asyncState.Complete(false, exception);
      }
    }

    private sealed class CloseReplySessionChannelAsyncResult : ReliableChannelBinderHelper.CloseInputSessionChannelAsyncResult<IReplySessionChannel, RequestContext>
    {
      public CloseReplySessionChannelAsyncResult(ReliableChannelBinder<IReplySessionChannel> binder, IReplySessionChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
        : base(binder, channel, timeout, callback, state)
      {
        if (!this.Begin())
          return;
        this.Complete(true);
      }

      protected override IAsyncResult BeginTryInput(TimeSpan timeout, AsyncCallback callback, object state)
      {
        return this.Channel.BeginTryReceiveRequest(timeout, callback, state);
      }

      protected override void DisposeItem(RequestContext item)
      {
        item.RequestMessage.Close();
        item.Close();
      }

      public static void End(IAsyncResult result)
      {
        AsyncResult.End<ReliableChannelBinderHelper.CloseReplySessionChannelAsyncResult>(result);
      }

      protected override bool EndTryInput(IAsyncResult result, out RequestContext item)
      {
        return this.Channel.EndTryReceiveRequest(result, out item);
      }
    }
  }
}
