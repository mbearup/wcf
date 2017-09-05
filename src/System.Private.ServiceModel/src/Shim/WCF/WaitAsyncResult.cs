// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.WaitAsyncResult
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Runtime;

namespace System.ServiceModel.Channels
{
  internal class WaitAsyncResult : AsyncResult
  {
    private object thisLock = new object();
    private bool completed;
    private bool throwTimeoutException;
    private bool timedOut;
    private TimeSpan timeout;
    private IOThreadTimer timer;

    public WaitAsyncResult(TimeSpan timeout, bool throwTimeoutException, AsyncCallback callback, object state)
      : base(callback, state)
    {
      this.timeout = timeout;
      this.throwTimeoutException = throwTimeoutException;
    }

    public void Begin()
    {
      lock (this.thisLock)
      {
        if (this.completed || !(this.timeout != TimeSpan.MaxValue))
          return;
        this.timer = new IOThreadTimer(new Action<object>(this.OnTimerElapsed), (object) null, true);
        this.timer.Set(this.timeout);
      }
    }

    public static bool End(IAsyncResult result)
    {
      return !AsyncResult.End<WaitAsyncResult>(result).timedOut;
    }

    protected virtual string GetTimeoutString(TimeSpan timeout)
    {
      return SR.GetString("TimeoutOnOperation", new object[1]{ (object) timeout });
    }

    public void OnAborted(CommunicationObject communicationObject)
    {
      if (!this.ShouldComplete(false))
        return;
      this.Complete(false, communicationObject.CreateClosedException());
    }

    public void OnFaulted(CommunicationObject communicationObject)
    {
      if (!this.ShouldComplete(false))
        return;
      this.Complete(false, communicationObject.GetTerminalException());
    }

    public void OnSignaled()
    {
      if (!this.ShouldComplete(false))
        return;
      this.Complete(false);
    }

    protected virtual void OnTimerElapsed(object state)
    {
      if (!this.ShouldComplete(true))
        return;
      if (this.throwTimeoutException)
        this.Complete(false, (Exception) new TimeoutException(this.GetTimeoutString(this.timeout)));
      else
        this.Complete(false);
    }

    private bool ShouldComplete(bool timedOut)
    {
      lock (this.thisLock)
      {
        if (!this.completed)
        {
          this.completed = true;
          this.timedOut = timedOut;
          if (!timedOut && this.timer != null)
            this.timer.Cancel();
          return true;
        }
      }
      return false;
    }

    public delegate void AbortHandler(CommunicationObject communicationObject);

    public delegate void SignaledHandler();
  }
}
