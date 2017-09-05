// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.InterruptibleWaitObject
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Runtime;
using System.Threading;

namespace System.ServiceModel.Channels
{
  internal class InterruptibleWaitObject
  {
    private object thisLock = new object();
    private bool throwTimeoutByDefault = true;
    private bool aborted;
    private CommunicationObject communicationObject;
    private ManualResetEvent handle;
    private bool set;
    private int syncWaiters;

    private event WaitAsyncResult.AbortHandler Aborted;

    private event WaitAsyncResult.AbortHandler Faulted;

    private event WaitAsyncResult.SignaledHandler Signaled;

    public InterruptibleWaitObject(bool signaled)
      : this(signaled, true)
    {
    }

    public InterruptibleWaitObject(bool signaled, bool throwTimeoutByDefault)
    {
      this.set = signaled;
      this.throwTimeoutByDefault = throwTimeoutByDefault;
    }

    public void Abort(CommunicationObject communicationObject)
    {
      if (communicationObject == null)
        throw Fx.AssertAndThrow("Argument communicationObject cannot be null.");
      lock (this.thisLock)
      {
        if (this.aborted)
          return;
        this.communicationObject = communicationObject;
        this.aborted = true;
        this.InternalSet();
      }
      // ISSUE: reference to a compiler-generated field
      WaitAsyncResult.AbortHandler aborted = this.Aborted;
      if (aborted == null)
        return;
      aborted(communicationObject);
    }

    public void Fault(CommunicationObject communicationObject)
    {
      if (communicationObject == null)
        throw Fx.AssertAndThrow("Argument communicationObject cannot be null.");
      lock (this.thisLock)
      {
        if (this.aborted)
          return;
        this.communicationObject = communicationObject;
        this.aborted = false;
        this.InternalSet();
      }
      // ISSUE: reference to a compiler-generated field
      WaitAsyncResult.AbortHandler faulted = this.Faulted;
      if (faulted == null)
        return;
      faulted(communicationObject);
    }

    public IAsyncResult BeginWait(TimeSpan timeout, AsyncCallback callback, object state)
    {
      return this.BeginWait(timeout, this.throwTimeoutByDefault, callback, state);
    }

    public IAsyncResult BeginWait(TimeSpan timeout, bool throwTimeoutException, AsyncCallback callback, object state)
    {
      Exception exception = (Exception) null;
      lock (this.thisLock)
      {
        if (!this.set)
        {
          WaitAsyncResult waitAsyncResult = new WaitAsyncResult(timeout, throwTimeoutException, callback, state);
          this.Aborted += new WaitAsyncResult.AbortHandler(waitAsyncResult.OnAborted);
          this.Faulted += new WaitAsyncResult.AbortHandler(waitAsyncResult.OnFaulted);
          this.Signaled += new WaitAsyncResult.SignaledHandler(waitAsyncResult.OnSignaled);
          waitAsyncResult.Begin();
          return (IAsyncResult) waitAsyncResult;
        }
        if (this.communicationObject != null)
          exception = this.GetException();
      }
      if (exception != null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
      return (IAsyncResult) new CompletedAsyncResult(callback, state);
    }

    public IAsyncResult BeginTryWait(TimeSpan timeout, AsyncCallback callback, object state)
    {
      return this.BeginWait(timeout, false, callback, state);
    }

    public void EndWait(IAsyncResult result)
    {
      this.EndTryWait(result);
    }

    public bool EndTryWait(IAsyncResult result)
    {
      if (!(result is CompletedAsyncResult))
        return WaitAsyncResult.End(result);
      CompletedAsyncResult.End(result);
      return true;
    }

    private Exception GetException()
    {
      CommunicationObject communicationObject = this.communicationObject;
      if (!this.aborted)
        return this.communicationObject.GetTerminalException();
      return this.communicationObject.CreateAbortedException();
    }

    private void InternalSet()
    {
      lock (this.thisLock)
      {
        this.set = true;
        if (this.handle == null)
          return;
        this.handle.Set();
      }
    }

    public void Reset()
    {
      lock (this.thisLock)
      {
        this.communicationObject = (CommunicationObject) null;
        this.aborted = false;
        this.set = false;
        if (this.handle == null)
          return;
        this.handle.Reset();
      }
    }

    public void Set()
    {
      this.InternalSet();
      // ISSUE: reference to a compiler-generated field
      WaitAsyncResult.SignaledHandler signaled = this.Signaled;
      if (signaled == null)
        return;
      signaled();
    }

    public bool Wait(TimeSpan timeout)
    {
      return this.Wait(timeout, this.throwTimeoutByDefault);
    }

    public bool Wait(TimeSpan timeout, bool throwTimeoutException)
    {
      lock (this.thisLock)
      {
        if (this.set)
        {
          if (this.communicationObject != null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.GetException());
          return true;
        }
        if (this.handle == null)
          this.handle = new ManualResetEvent(false);
        this.syncWaiters = this.syncWaiters + 1;
      }
      try
      {
        if (!System.Runtime.TimeoutHelper.WaitOne((WaitHandle) this.handle, timeout))
        {
          if (throwTimeoutException)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new TimeoutException(SR.GetString("TimeoutOnOperation", new object[1]{ (object) timeout })));
          return false;
        }
      }
      finally
      {
        lock (this.thisLock)
        {
          this.syncWaiters = this.syncWaiters - 1;
          if (this.syncWaiters == 0)
          {
            this.handle.Close();
            this.handle = (ManualResetEvent) null;
          }
        }
      }
      if (this.communicationObject != null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.GetException());
      return true;
    }
  }
}
