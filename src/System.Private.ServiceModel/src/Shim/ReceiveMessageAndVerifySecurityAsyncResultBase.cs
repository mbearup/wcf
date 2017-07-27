// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.ReceiveMessageAndVerifySecurityAsyncResultBase
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Runtime;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Security
{
  internal abstract class ReceiveMessageAndVerifySecurityAsyncResultBase : AsyncResult
  {
    private static AsyncCallback innerTryReceiveCompletedCallback = Fx.ThunkCallback(new AsyncCallback(ReceiveMessageAndVerifySecurityAsyncResultBase.InnerTryReceiveCompletedCallback));
    private Message message;
    private bool receiveCompleted;
    private TimeoutHelper timeoutHelper;
    private IInputChannel innerChannel;

    protected ReceiveMessageAndVerifySecurityAsyncResultBase(IInputChannel innerChannel, TimeSpan timeout, AsyncCallback callback, object state)
      : base(callback, state)
    {
      this.timeoutHelper = new TimeoutHelper(timeout);
      this.innerChannel = innerChannel;
    }

    public void Start()
    {
      IAsyncResult result = this.innerChannel.BeginTryReceive(this.timeoutHelper.RemainingTime(), ReceiveMessageAndVerifySecurityAsyncResultBase.innerTryReceiveCompletedCallback, (object) this);
      if (!result.CompletedSynchronously)
        return;
      if (!this.innerChannel.EndTryReceive(result, out this.message))
      {
        this.receiveCompleted = false;
      }
      else
      {
        this.receiveCompleted = true;
        if (!this.OnInnerReceiveDone(ref this.message, this.timeoutHelper.RemainingTime()))
          return;
      }
      this.Complete(true);
    }

    private static void InnerTryReceiveCompletedCallback(IAsyncResult result)
    {
      if (result.CompletedSynchronously)
        return;
      ReceiveMessageAndVerifySecurityAsyncResultBase asyncState = (ReceiveMessageAndVerifySecurityAsyncResultBase) result.AsyncState;
      Exception exception = (Exception) null;
      bool flag;
      try
      {
        if (!asyncState.innerChannel.EndTryReceive(result, out asyncState.message))
        {
          asyncState.receiveCompleted = false;
          flag = true;
        }
        else
        {
          asyncState.receiveCompleted = true;
          flag = asyncState.OnInnerReceiveDone(ref asyncState.message, asyncState.timeoutHelper.RemainingTime());
        }
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          flag = true;
          exception = ex;
        }
      }
      if (!flag)
        return;
      asyncState.Complete(false, exception);
    }

    protected abstract bool OnInnerReceiveDone(ref Message message, TimeSpan timeout);

    public static bool End(IAsyncResult result, out Message message)
    {
      ReceiveMessageAndVerifySecurityAsyncResultBase securityAsyncResultBase = AsyncResult.End<ReceiveMessageAndVerifySecurityAsyncResultBase>(result);
      message = securityAsyncResultBase.message;
      return securityAsyncResultBase.receiveCompleted;
    }
  }
}
