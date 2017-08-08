// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.ReliableChannelBinder`1
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Runtime;
using System.Threading;

namespace System.ServiceModel.Channels
{
  internal abstract class ReliableChannelBinder<TChannel> : IReliableChannelBinder where TChannel : class, IChannel
  {
    private object thisLock = new object();
    private bool aborted;
    private TimeSpan defaultCloseTimeout;
    private MaskingMode defaultMaskingMode;
    private TimeSpan defaultSendTimeout;
    private AsyncCallback onCloseChannelComplete;
    private CommunicationState state;
    private ReliableChannelBinder<TChannel>.ChannelSynchronizer synchronizer;

    protected abstract bool CanGetChannelForReceive { get; }

    public abstract bool CanSendAsynchronously { get; }

    public virtual ChannelParameterCollection ChannelParameters
    {
      get
      {
        return (ChannelParameterCollection) null;
      }
    }

    public IChannel Channel
    {
      get
      {
        return (IChannel) this.synchronizer.CurrentChannel;
      }
    }

    public bool Connected
    {
      get
      {
        return this.synchronizer.Connected;
      }
    }

    public MaskingMode DefaultMaskingMode
    {
      get
      {
        return this.defaultMaskingMode;
      }
    }

    public TimeSpan DefaultSendTimeout
    {
      get
      {
        return this.defaultSendTimeout;
      }
    }

    public abstract bool HasSession { get; }

    public abstract EndpointAddress LocalAddress { get; }

    protected abstract bool MustCloseChannel { get; }

    protected abstract bool MustOpenChannel { get; }

    public abstract EndpointAddress RemoteAddress { get; }

    public CommunicationState State
    {
      get
      {
        return this.state;
      }
    }

    protected ReliableChannelBinder<TChannel>.ChannelSynchronizer Synchronizer
    {
      get
      {
        return this.synchronizer;
      }
    }

    protected object ThisLock
    {
      get
      {
        return this.thisLock;
      }
    }

    private bool TolerateFaults
    {
      get
      {
        return this.synchronizer.TolerateFaults;
      }
    }

    public event EventHandler ConnectionLost;

    public event BinderExceptionHandler Faulted;

    public event BinderExceptionHandler OnException;

    protected ReliableChannelBinder(TChannel channel, MaskingMode maskingMode, TolerateFaultsMode faultMode, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
    {
      if (maskingMode != MaskingMode.None && maskingMode != MaskingMode.All)
        throw Fx.AssertAndThrow("ReliableChannelBinder was implemented with only 2 default masking modes, None and All.");
      this.defaultMaskingMode = maskingMode;
      this.defaultCloseTimeout = defaultCloseTimeout;
      this.defaultSendTimeout = defaultSendTimeout;
      this.synchronizer = new ReliableChannelBinder<TChannel>.ChannelSynchronizer(this, channel, faultMode);
    }

    public void Abort()
    {
      TChannel channel;
      lock (this.ThisLock)
      {
        this.aborted = true;
        if (this.state == CommunicationState.Closed)
          return;
        this.state = CommunicationState.Closing;
        channel = this.synchronizer.StopSynchronizing(true);
        if (!this.MustCloseChannel)
          channel = default (TChannel);
      }
      this.synchronizer.UnblockWaiters();
      this.OnShutdown();
      this.OnAbort();
      if ((object) channel != null)
        channel.Abort();
      this.TransitionToClosed();
    }

    protected virtual void AddOutputHeaders(Message message)
    {
    }

    public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
    {
      return this.BeginClose(timeout, this.defaultMaskingMode, callback, state);
    }

    public IAsyncResult BeginClose(TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
    {
      this.ThrowIfTimeoutNegative(timeout);
      TChannel channel;
      if (this.CloseCore(out channel))
        return (IAsyncResult) new CompletedAsyncResult(callback, state);
      return (IAsyncResult) new ReliableChannelBinder<TChannel>.CloseAsyncResult(this, channel, timeout, maskingMode, callback, state);
    }

    protected virtual IAsyncResult BeginCloseChannel(TChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
    {
      return channel.BeginClose(timeout, callback, state);
    }

    public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
    {
      this.ThrowIfTimeoutNegative(timeout);
      if (this.OnOpening(this.defaultMaskingMode))
      {
        try
        {
          return this.OnBeginOpen(timeout, callback, state);
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
          {
            throw;
          }
          else
          {
            this.Fault((Exception) null);
            if (this.defaultMaskingMode == MaskingMode.None)
              throw;
            else
              this.RaiseOnException(ex);
          }
        }
      }
      return (IAsyncResult) new ReliableChannelBinder<TChannel>.BinderCompletedAsyncResult(callback, state);
    }

    public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
    {
      return this.BeginSend(message, timeout, this.defaultMaskingMode, callback, state);
    }

    public IAsyncResult BeginSend(Message message, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
    {
      ReliableChannelBinder<TChannel>.SendAsyncResult sendAsyncResult = new ReliableChannelBinder<TChannel>.SendAsyncResult(this, callback, state);
      sendAsyncResult.Start(message, timeout, maskingMode);
      return (IAsyncResult) sendAsyncResult;
    }

    protected abstract IAsyncResult BeginTryGetChannel(TimeSpan timeout, AsyncCallback callback, object state);

    public virtual IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
    {
      return this.BeginTryReceive(timeout, this.defaultMaskingMode, callback, state);
    }

    public virtual IAsyncResult BeginTryReceive(TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
    {
      if (this.ValidateInputOperation(timeout))
        return (IAsyncResult) new ReliableChannelBinder<TChannel>.TryReceiveAsyncResult(this, timeout, maskingMode, callback, state);
      return (IAsyncResult) new CompletedAsyncResult(callback, state);
    }

    internal IAsyncResult BeginWaitForPendingOperations(TimeSpan timeout, AsyncCallback callback, object state)
    {
      return this.synchronizer.BeginWaitForPendingOperations(timeout, callback, state);
    }

    private bool CloseCore(out TChannel channel)
    {
      channel = default (TChannel);
      bool flag1 = true;
      bool flag2 = false;
      lock (this.ThisLock)
      {
        if (this.state == CommunicationState.Closing || this.state == CommunicationState.Closed)
          return true;
        if (this.state == CommunicationState.Opened)
        {
          this.state = CommunicationState.Closing;
          channel = this.synchronizer.StopSynchronizing(true);
          flag1 = false;
          if (!this.MustCloseChannel)
            channel = default (TChannel);
          if ((object) channel != null)
          {
            switch (channel.State)
            {
              case CommunicationState.Created:
              case CommunicationState.Opening:
              case CommunicationState.Faulted:
                flag2 = true;
                break;
              case CommunicationState.Closing:
              case CommunicationState.Closed:
                channel = default (TChannel);
                break;
            }
          }
        }
      }
      this.synchronizer.UnblockWaiters();
      if (flag1)
      {
        this.Abort();
        return true;
      }
      if (flag2)
      {
        channel.Abort();
        channel = default (TChannel);
      }
      return false;
    }

    public void Close(TimeSpan timeout)
    {
      this.Close(timeout, this.defaultMaskingMode);
    }

    public void Close(TimeSpan timeout, MaskingMode maskingMode)
    {
      this.ThrowIfTimeoutNegative(timeout);
      System.Runtime.TimeoutHelper timeoutHelper = new System.Runtime.TimeoutHelper(timeout);
      TChannel channel;
      if (this.CloseCore(out channel))
        return;
      try
      {
        this.OnShutdown();
        this.OnClose(timeoutHelper.RemainingTime());
        if ((object) channel != null)
          this.CloseChannel(channel, timeoutHelper.RemainingTime());
        this.TransitionToClosed();
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          this.Abort();
          if (this.HandleException(ex, maskingMode))
            return;
          throw;
        }
      }
    }

    private void CloseChannel(TChannel channel)
    {
      if (!this.MustCloseChannel)
        throw Fx.AssertAndThrow("MustCloseChannel is false when there is no receive loop and this method is called when there is a receive loop.");
      if (this.onCloseChannelComplete == null)
        this.onCloseChannelComplete = Fx.ThunkCallback(new AsyncCallback(this.OnCloseChannelComplete));
      try
      {
        IAsyncResult result = channel.BeginClose(this.onCloseChannelComplete, (object) channel);
        if (!result.CompletedSynchronously)
          return;
        channel.EndClose(result);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          this.HandleException(ex, MaskingMode.All);
      }
    }

    protected virtual void CloseChannel(TChannel channel, TimeSpan timeout)
    {
      channel.Close(timeout);
    }

    public void EndClose(IAsyncResult result)
    {
      ReliableChannelBinder<TChannel>.CloseAsyncResult closeAsyncResult = result as ReliableChannelBinder<TChannel>.CloseAsyncResult;
      if (closeAsyncResult != null)
        closeAsyncResult.End();
      else
        CompletedAsyncResult.End(result);
    }

    protected virtual void EndCloseChannel(TChannel channel, IAsyncResult result)
    {
      channel.EndClose(result);
    }

    public void EndOpen(IAsyncResult result)
    {
      ReliableChannelBinder<TChannel>.BinderCompletedAsyncResult completedAsyncResult = result as ReliableChannelBinder<TChannel>.BinderCompletedAsyncResult;
      if (completedAsyncResult != null)
      {
        completedAsyncResult.End();
      }
      else
      {
        try
        {
          this.OnEndOpen(result);
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
          {
            throw;
          }
          else
          {
            this.Fault((Exception) null);
            if (this.defaultMaskingMode == MaskingMode.None)
            {
              throw;
            }
            else
            {
              this.RaiseOnException(ex);
              return;
            }
          }
        }
        this.synchronizer.StartSynchronizing();
        this.OnOpened();
      }
    }

    public void EndSend(IAsyncResult result)
    {
      ReliableChannelBinder<TChannel>.SendAsyncResult.End(result);
    }

    protected abstract bool EndTryGetChannel(IAsyncResult result);

    public virtual bool EndTryReceive(IAsyncResult result, out RequestContext requestContext)
    {
      ReliableChannelBinder<TChannel>.TryReceiveAsyncResult receiveAsyncResult = result as ReliableChannelBinder<TChannel>.TryReceiveAsyncResult;
      if (receiveAsyncResult != null)
        return receiveAsyncResult.End(out requestContext);
      CompletedAsyncResult.End(result);
      requestContext = (RequestContext) null;
      return true;
    }

    public void EndWaitForPendingOperations(IAsyncResult result)
    {
      this.synchronizer.EndWaitForPendingOperations(result);
    }

    protected void Fault(Exception e)
    {
      lock (this.ThisLock)
      {
        if (this.state == CommunicationState.Created)
          throw Fx.AssertAndThrow("The binder should not detect the inner channel's faults until after the binder is opened.");
        if (this.state == CommunicationState.Faulted || this.state == CommunicationState.Closed)
          return;
        this.state = CommunicationState.Faulted;
        this.synchronizer.StopSynchronizing(false);
      }
      this.synchronizer.UnblockWaiters();
      // ISSUE: reference to a compiler-generated field
      BinderExceptionHandler faulted = this.Faulted;
      if (faulted == null)
        return;
      faulted((IReliableChannelBinder) this, e);
    }

    private Exception GetClosedException(MaskingMode maskingMode)
    {
      if (ReliableChannelBinderHelper.MaskHandled(maskingMode))
        return (Exception) null;
      if (!this.aborted)
        return (Exception) new ObjectDisposedException(this.GetType().ToString());
      return (Exception) new CommunicationObjectAbortedException(SR.GetString("CommunicationObjectAborted1", new object[1]{ (object) this.GetType().ToString() }));
    }

    private Exception GetClosedOrFaultedException(MaskingMode maskingMode)
    {
      if (this.state == CommunicationState.Faulted)
        return this.GetFaultedException(maskingMode);
      if (this.state == CommunicationState.Closing || this.state == CommunicationState.Closed)
        return this.GetClosedException(maskingMode);
      throw Fx.AssertAndThrow("Caller is attempting to get a terminal exception in a non-terminal state.");
    }

    private Exception GetFaultedException(MaskingMode maskingMode)
    {
      if (ReliableChannelBinderHelper.MaskHandled(maskingMode))
        return (Exception) null;
      return (Exception) new CommunicationObjectFaultedException(SR.GetString("CommunicationObjectFaulted1", new object[1]{ (object) this.GetType().ToString() }));
    }

    public abstract ISession GetInnerSession();

    public void HandleException(Exception e)
    {
      this.HandleException(e, MaskingMode.All);
    }

    protected bool HandleException(Exception e, MaskingMode maskingMode)
    {
      if (this.TolerateFaults && e is CommunicationObjectFaultedException)
        return true;
      if (this.IsHandleable(e))
        return ReliableChannelBinderHelper.MaskHandled(maskingMode);
      bool flag = ReliableChannelBinderHelper.MaskUnhandled(maskingMode);
      if (flag)
        this.RaiseOnException(e);
      return flag;
    }

    protected bool HandleException(Exception e, MaskingMode maskingMode, bool autoAborted)
    {
      if (this.TolerateFaults & autoAborted && e is CommunicationObjectAbortedException)
        return true;
      return this.HandleException(e, maskingMode);
    }

    protected abstract bool HasSecuritySession(TChannel channel);

    public bool IsHandleable(Exception e)
    {
      if (e is ProtocolException)
        return false;
      if (!(e is CommunicationException))
        return e is TimeoutException;
      return true;
    }

    protected abstract void OnAbort();

    protected abstract IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state);

    protected abstract IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state);

    protected virtual IAsyncResult OnBeginSend(TChannel channel, Message message, TimeSpan timeout, AsyncCallback callback, object state)
    {
      throw Fx.AssertAndThrow("The derived class does not support the BeginSend operation.");
    }

    protected virtual IAsyncResult OnBeginTryReceive(TChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
    {
      throw Fx.AssertAndThrow("The derived class does not support the BeginTryReceive operation.");
    }

    protected abstract void OnClose(TimeSpan timeout);

    private void OnCloseChannelComplete(IAsyncResult result)
    {
      if (result.CompletedSynchronously)
        return;
      TChannel asyncState = (TChannel) result.AsyncState;
      try
      {
        asyncState.EndClose(result);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          this.HandleException(ex, MaskingMode.All);
      }
    }

    protected abstract void OnEndClose(IAsyncResult result);

    protected abstract void OnEndOpen(IAsyncResult result);

    protected virtual void OnEndSend(TChannel channel, IAsyncResult result)
    {
      throw Fx.AssertAndThrow("The derived class does not support the EndSend operation.");
    }

    protected virtual bool OnEndTryReceive(TChannel channel, IAsyncResult result, out RequestContext requestContext)
    {
      throw Fx.AssertAndThrow("The derived class does not support the EndTryReceive operation.");
    }

    private void OnInnerChannelFaulted()
    {
      if (!this.TolerateFaults)
        return;
      // ISSUE: reference to a compiler-generated field
      EventHandler connectionLost = this.ConnectionLost;
      if (connectionLost == null)
        return;
      connectionLost((object) this, EventArgs.Empty);
    }

    protected abstract void OnOpen(TimeSpan timeout);

    private void OnOpened()
    {
      lock (this.ThisLock)
      {
        if (this.state != CommunicationState.Opening)
          return;
        this.state = CommunicationState.Opened;
      }
    }

    private bool OnOpening(MaskingMode maskingMode)
    {
      lock (this.ThisLock)
      {
        if (this.state != CommunicationState.Created)
        {
          Exception exception = (Exception) null;
          if (this.state == CommunicationState.Opening || this.state == CommunicationState.Opened)
          {
            if (!ReliableChannelBinderHelper.MaskUnhandled(maskingMode))
              exception = (Exception) new InvalidOperationException(SR.GetString("CommunicationObjectCannotBeModifiedInState", (object) this.GetType().ToString(), (object) this.state.ToString()));
          }
          else
            exception = this.GetClosedOrFaultedException(maskingMode);
          if (exception != null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
          return false;
        }
        this.state = CommunicationState.Opening;
        return true;
      }
    }

    protected virtual void OnShutdown()
    {
    }

    protected virtual void OnSend(TChannel channel, Message message, TimeSpan timeout)
    {
      throw Fx.AssertAndThrow("The derived class does not support the Send operation.");
    }

    protected virtual bool OnTryReceive(TChannel channel, TimeSpan timeout, out RequestContext requestContext)
    {
      throw Fx.AssertAndThrow("The derived class does not support the TryReceive operation.");
    }

    public void Open(TimeSpan timeout)
    {
      this.ThrowIfTimeoutNegative(timeout);
      if (!this.OnOpening(this.defaultMaskingMode))
        return;
      try
      {
        this.OnOpen(timeout);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          this.Fault((Exception) null);
          if (this.defaultMaskingMode == MaskingMode.None)
          {
            throw;
          }
          else
          {
            this.RaiseOnException(ex);
            return;
          }
        }
      }
      this.synchronizer.StartSynchronizing();
      this.OnOpened();
    }

    private void RaiseOnException(Exception e)
    {
      // ISSUE: reference to a compiler-generated field
      BinderExceptionHandler onException = this.OnException;
      if (onException == null)
        return;
      onException((IReliableChannelBinder) this, e);
    }

    public void Send(Message message, TimeSpan timeout)
    {
      this.Send(message, timeout, this.defaultMaskingMode);
    }

    public void Send(Message message, TimeSpan timeout, MaskingMode maskingMode)
    {
      if (!this.ValidateOutputOperation(message, timeout, maskingMode))
        return;
      bool autoAborted = false;
      try
      {
        System.Runtime.TimeoutHelper timeoutHelper = new System.Runtime.TimeoutHelper(timeout);
        TChannel channel;
        if (!this.synchronizer.TryGetChannelForOutput(timeoutHelper.RemainingTime(), maskingMode, out channel))
        {
          if (!ReliableChannelBinderHelper.MaskHandled(maskingMode))
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new TimeoutException(SR.GetString("TimeoutOnSend", new object[1]{ (object) timeout })));
        }
        else
        {
          if ((object) channel == null)
            return;
          this.AddOutputHeaders(message);
          try
          {
            this.OnSend(channel, message, timeoutHelper.RemainingTime());
          }
          finally
          {
            autoAborted = this.Synchronizer.Aborting;
            this.synchronizer.ReturnChannel();
          }
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
          if (this.HandleException(ex, maskingMode, autoAborted))
            return;
          throw;
        }
      }
    }

    public void SetMaskingMode(RequestContext context, MaskingMode maskingMode)
    {
      ((ReliableChannelBinder<TChannel>.BinderRequestContext) context).SetMaskingMode(maskingMode);
    }

    private bool ThrowIfNotOpenedAndNotMasking(MaskingMode maskingMode, bool throwDisposed)
    {
      lock (this.ThisLock)
      {
        if (this.State == CommunicationState.Created)
          throw Fx.AssertAndThrow("Messaging operations cannot be called when the binder is in the Created state.");
        if (this.State == CommunicationState.Opening)
          throw Fx.AssertAndThrow("Messaging operations cannot be called when the binder is in the Opening state.");
        if (this.State == CommunicationState.Opened)
          return true;
        if (throwDisposed)
        {
          Exception faultedException = this.GetClosedOrFaultedException(maskingMode);
          if (faultedException != null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(faultedException);
        }
        return false;
      }
    }

    private void ThrowIfTimeoutNegative(TimeSpan timeout)
    {
      if (timeout < TimeSpan.Zero)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("timeout", (object) timeout, "SFxTimeoutOutOfRange0"));
    }

    private void TransitionToClosed()
    {
      lock (this.ThisLock)
      {
        if (this.state != CommunicationState.Closing && this.state != CommunicationState.Closed && this.state != CommunicationState.Faulted)
          throw Fx.AssertAndThrow("Caller cannot transition to the Closed state from a non-terminal state.");
        this.state = CommunicationState.Closed;
      }
    }

    protected abstract bool TryGetChannel(TimeSpan timeout);

    public virtual bool TryReceive(TimeSpan timeout, out RequestContext requestContext)
    {
      return this.TryReceive(timeout, out requestContext, this.defaultMaskingMode);
    }

    public virtual bool TryReceive(TimeSpan timeout, out RequestContext requestContext, MaskingMode maskingMode)
    {
      if (maskingMode != MaskingMode.None)
        throw Fx.AssertAndThrow("This method was implemented only for the case where we do not mask exceptions.");
      if (!this.ValidateInputOperation(timeout))
      {
        requestContext = (RequestContext) null;
        return true;
      }
      System.Runtime.TimeoutHelper timeoutHelper = new System.Runtime.TimeoutHelper(timeout);
      while (true)
      {
        bool autoAborted = false;
        try
        {
          TChannel channel;
          bool flag1 = !this.synchronizer.TryGetChannelForInput(this.CanGetChannelForReceive, timeoutHelper.RemainingTime(), out channel);
          if ((object) channel == null)
          {
            requestContext = (RequestContext) null;
            return flag1;
          }
          try
          {
            bool flag2 = this.OnTryReceive(channel, timeoutHelper.RemainingTime(), out requestContext);
            if (!flag2 || requestContext != null)
              return flag2;
            this.synchronizer.OnReadEof();
          }
          finally
          {
            autoAborted = this.Synchronizer.Aborting;
            this.synchronizer.ReturnChannel();
          }
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else if (!this.HandleException(ex, maskingMode, autoAborted))
            throw;
        }
      }
    }

    protected bool ValidateInputOperation(TimeSpan timeout)
    {
      if (timeout < TimeSpan.Zero)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("timeout", (object) timeout, "SFxTimeoutOutOfRange0"));
      return this.ThrowIfNotOpenedAndNotMasking(MaskingMode.All, false);
    }

    protected bool ValidateOutputOperation(Message message, TimeSpan timeout, MaskingMode maskingMode)
    {
      if (message == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
      if (timeout < TimeSpan.Zero)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("timeout", (object) timeout, "SFxTimeoutOutOfRange0"));
      return this.ThrowIfNotOpenedAndNotMasking(maskingMode, true);
    }

    internal void WaitForPendingOperations(TimeSpan timeout)
    {
      this.synchronizer.WaitForPendingOperations(timeout);
    }

    protected RequestContext WrapMessage(Message message)
    {
      if (message == null)
        return (RequestContext) null;
      return (RequestContext) new ReliableChannelBinder<TChannel>.MessageRequestContext(this, message);
    }

    public RequestContext WrapRequestContext(RequestContext context)
    {
      if (context == null)
        return (RequestContext) null;
      if (!this.TolerateFaults && this.defaultMaskingMode == MaskingMode.None)
        return context;
      return (RequestContext) new ReliableChannelBinder<TChannel>.RequestRequestContext(this, context, context.RequestMessage);
    }

    private sealed class BinderCompletedAsyncResult : CompletedAsyncResult
    {
      public BinderCompletedAsyncResult(AsyncCallback callback, object state)
        : base(callback, state)
      {
      }

      public void End()
      {
        CompletedAsyncResult.End((IAsyncResult) this);
      }
    }

    private abstract class BinderRequestContext : RequestContextBase
    {
      private ReliableChannelBinder<TChannel> binder;
      private MaskingMode maskingMode;

      protected ReliableChannelBinder<TChannel> Binder
      {
        get
        {
          return this.binder;
        }
      }

      protected MaskingMode MaskingMode
      {
        get
        {
          return this.maskingMode;
        }
      }

      public BinderRequestContext(ReliableChannelBinder<TChannel> binder, Message message)
        : base(message, binder.defaultCloseTimeout, binder.defaultSendTimeout)
      {
        this.binder = binder;
        this.maskingMode = binder.defaultMaskingMode;
      }

      public void SetMaskingMode(MaskingMode maskingMode)
      {
        if (this.binder.defaultMaskingMode != MaskingMode.All)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
        this.maskingMode = maskingMode;
      }
    }

    protected class ChannelSynchronizer
    {
      private static Action<object> asyncGetChannelCallback = new Action<object>(ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncGetChannelCallback);
      private bool tolerateFaults = true;
      private object thisLock = new object();
      private bool aborting;
      private ReliableChannelBinder<TChannel> binder;
      private int count;
      private TChannel currentChannel;
      private InterruptibleWaitObject drainEvent;
      private TolerateFaultsMode faultMode;
      private Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter> getChannelQueue;
      private bool innerChannelFaulted;
      private EventHandler onChannelFaulted;
      private ReliableChannelBinder<TChannel>.ChannelSynchronizer.State state;
      private Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter> waitQueue;

      public bool Aborting
      {
        get
        {
          return this.aborting;
        }
      }

      public bool Connected
      {
        get
        {
          if (this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpened)
            return this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpening;
          return true;
        }
      }

      public TChannel CurrentChannel
      {
        get
        {
          return this.currentChannel;
        }
      }

      private object ThisLock
      {
        get
        {
          return this.thisLock;
        }
      }

      public bool TolerateFaults
      {
        get
        {
          return this.tolerateFaults;
        }
      }

      public ChannelSynchronizer(ReliableChannelBinder<TChannel> binder, TChannel channel, TolerateFaultsMode faultMode)
      {
        this.binder = binder;
        this.currentChannel = channel;
        this.faultMode = faultMode;
      }

      public TChannel AbortCurentChannel()
      {
        lock (this.ThisLock)
        {
          if (!this.tolerateFaults)
            throw Fx.AssertAndThrow("It is only valid to abort the current channel when masking faults");
          if (this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpening)
          {
            this.aborting = true;
          }
          else
          {
            if (this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpened)
              return default (TChannel);
            if (this.count == 0)
            {
              this.state = ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.NoChannel;
            }
            else
            {
              this.aborting = true;
              this.state = ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelClosing;
            }
          }
          return this.currentChannel;
        }
      }

      private static void AsyncGetChannelCallback(object state)
      {
        ((ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter) state).GetChannel(false);
      }

      public IAsyncResult BeginTryGetChannelForInput(bool canGetChannel, TimeSpan timeout, AsyncCallback callback, object state)
      {
        return this.BeginTryGetChannel(canGetChannel, false, timeout, MaskingMode.All, callback, state);
      }

      public IAsyncResult BeginTryGetChannelForOutput(TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
      {
        return this.BeginTryGetChannel(true, true, timeout, maskingMode, callback, state);
      }

      private IAsyncResult BeginTryGetChannel(bool canGetChannel, bool canCauseFault, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
      {
        TChannel data = default (TChannel);
        ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter asyncWaiter = (ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter) null;
        bool flag1 = false;
        bool flag2 = false;
        lock (this.ThisLock)
        {
          if (!this.ThrowIfNecessary(maskingMode))
            data = default (TChannel);
          else if (this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpened)
          {
            if ((object) this.currentChannel == null)
              throw Fx.AssertAndThrow("Field currentChannel cannot be null in the ChannelOpened state.");
            this.count = this.count + 1;
            data = this.currentChannel;
          }
          else if (!this.tolerateFaults && (this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.NoChannel || this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelClosing))
          {
            if (canCauseFault)
              flag2 = true;
            data = default (TChannel);
          }
          else if (!canGetChannel || this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpening || this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelClosing)
          {
            asyncWaiter = new ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter(this, canGetChannel, default (TChannel), timeout, maskingMode, this.binder.ChannelParameters, callback, state);
            this.GetQueue(canGetChannel).Enqueue((ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter) asyncWaiter);
          }
          else
          {
            if (this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.NoChannel)
              throw Fx.AssertAndThrow("The state must be NoChannel.");
            asyncWaiter = new ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter(this, canGetChannel, this.GetCurrentChannelIfCreated(), timeout, maskingMode, this.binder.ChannelParameters, callback, state);
            this.state = ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpening;
            flag1 = true;
          }
        }
        if (flag2)
          this.binder.Fault((Exception) null);
        if (asyncWaiter == null)
          return (IAsyncResult) new CompletedAsyncResult<TChannel>(data, callback, state);
        if (flag1)
          asyncWaiter.GetChannel(true);
        else
          asyncWaiter.Wait();
        return (IAsyncResult) asyncWaiter;
      }

      public IAsyncResult BeginWaitForPendingOperations(TimeSpan timeout, AsyncCallback callback, object state)
      {
        lock (this.ThisLock)
        {
          if (this.drainEvent != null)
            throw Fx.AssertAndThrow("The WaitForPendingOperations operation may only be invoked once.");
          if (this.count > 0)
            this.drainEvent = new InterruptibleWaitObject(false, false);
        }
        if (this.drainEvent != null)
          return this.drainEvent.BeginWait(timeout, callback, state);
        return (IAsyncResult) new ReliableChannelBinder<TChannel>.ChannelSynchronizer.SynchronizerCompletedAsyncResult(callback, state);
      }

      private bool CompleteSetChannel(ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter waiter, out TChannel channel)
      {
        if (waiter == null)
          throw Fx.AssertAndThrow("Argument waiter cannot be null.");
        bool flag = false;
        lock (this.ThisLock)
        {
          if (this.ValidateOpened())
          {
            channel = this.currentChannel;
            return true;
          }
          channel = default (TChannel);
          flag = this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.Closed;
        }
        if (flag)
          waiter.Close();
        else
          waiter.Fault();
        return false;
      }

      public bool EndTryGetChannel(IAsyncResult result, out TChannel channel)
      {
        ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter asyncWaiter = result as ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter;
        if (asyncWaiter != null)
          return asyncWaiter.End(out channel);
        channel = CompletedAsyncResult<TChannel>.End(result);
        return true;
      }

      public void EndWaitForPendingOperations(IAsyncResult result)
      {
        ReliableChannelBinder<TChannel>.ChannelSynchronizer.SynchronizerCompletedAsyncResult completedAsyncResult = result as ReliableChannelBinder<TChannel>.ChannelSynchronizer.SynchronizerCompletedAsyncResult;
        if (completedAsyncResult != null)
          completedAsyncResult.End();
        else
          this.drainEvent.EndWait(result);
      }

      public bool EnsureChannel()
      {
        bool flag = false;
        lock (this.ThisLock)
        {
          if (this.ValidateOpened())
          {
            if (this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpened)
              return true;
            if (this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.NoChannel)
              throw Fx.AssertAndThrow("The caller may only invoke this EnsureChannel during the CreateSequence negotiation. ChannelOpening and ChannelClosing are invalid states during this phase of the negotiation.");
            if (!this.tolerateFaults)
            {
              flag = true;
            }
            else
            {
              if ((object) this.GetCurrentChannelIfCreated() != null)
                return true;
              if (this.binder.TryGetChannel(TimeSpan.Zero))
                return (object) this.currentChannel != null;
            }
          }
        }
        if (flag)
          this.binder.Fault((Exception) null);
        return false;
      }

      private ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter GetChannelWaiter()
      {
        if (this.getChannelQueue == null || this.getChannelQueue.Count == 0)
          return (ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter) null;
        return this.getChannelQueue.Dequeue();
      }

      private TChannel GetCurrentChannelIfCreated()
      {
        if (this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.NoChannel)
          throw Fx.AssertAndThrow("This method may only be called in the NoChannel state.");
        if ((object) this.currentChannel != null && this.currentChannel.State == CommunicationState.Created)
          return this.currentChannel;
        return default (TChannel);
      }

      private Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter> GetQueue(bool canGetChannel)
      {
        if (canGetChannel)
        {
          if (this.getChannelQueue == null)
            this.getChannelQueue = new Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter>();
          return this.getChannelQueue;
        }
        if (this.waitQueue == null)
          this.waitQueue = new Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter>();
        return this.waitQueue;
      }

      private void OnChannelFaulted(object sender, EventArgs e)
      {
        TChannel channel = (TChannel) sender;
        bool flag1 = false;
        bool flag2 = false;
        lock (this.ThisLock)
        {
          if ((object) this.currentChannel != (object) channel || !this.ValidateOpened())
            return;
          if (this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpened)
          {
            if (this.count == 0)
              channel.Faulted -= this.onChannelFaulted;
            flag1 = !this.tolerateFaults;
            this.state = ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelClosing;
            this.innerChannelFaulted = true;
            if (!flag1)
            {
              if (this.count == 0)
              {
                this.state = ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.NoChannel;
                this.aborting = false;
                flag2 = true;
                this.innerChannelFaulted = false;
              }
            }
          }
        }
        if (flag1)
          this.binder.Fault((Exception) null);
        channel.Abort();
        if (!flag2)
          return;
        this.binder.OnInnerChannelFaulted();
      }

      private bool OnChannelOpened(ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter waiter)
      {
        if (waiter == null)
          throw Fx.AssertAndThrow("Argument waiter cannot be null.");
        bool flag1 = false;
        bool flag2 = false;
        Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter> waiters1 = (Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter>) null;
        Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter> waiters2 = (Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter>) null;
        TChannel channel = default (TChannel);
        lock (this.ThisLock)
        {
          if ((object) this.currentChannel == null)
            throw Fx.AssertAndThrow("Caller must ensure that field currentChannel is set before opening the channel.");
          if (this.ValidateOpened())
          {
            if (this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpening)
              throw Fx.AssertAndThrow("This method may only be called in the ChannelOpening state.");
            this.state = ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpened;
            this.SetTolerateFaults();
            this.count = this.count + 1;
            this.count = this.count + (this.getChannelQueue == null ? 0 : this.getChannelQueue.Count);
            this.count = this.count + (this.waitQueue == null ? 0 : this.waitQueue.Count);
            waiters1 = this.getChannelQueue;
            waiters2 = this.waitQueue;
            channel = this.currentChannel;
            this.getChannelQueue = (Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter>) null;
            this.waitQueue = (Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter>) null;
          }
          else
          {
            flag1 = this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.Closed;
            flag2 = this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.Faulted;
          }
        }
        if (flag1)
        {
          waiter.Close();
          return false;
        }
        if (flag2)
        {
          waiter.Fault();
          return false;
        }
        this.SetWaiters(waiters1, channel);
        this.SetWaiters(waiters2, channel);
        return true;
      }

      private void OnGetChannelFailed()
      {
        ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter waiter = (ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter) null;
        lock (this.ThisLock)
        {
          if (!this.ValidateOpened())
            return;
          if (this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpening)
            throw Fx.AssertAndThrow("The state must be set to ChannelOpening before the caller attempts to open the channel.");
          waiter = this.GetChannelWaiter();
          if (waiter == null)
          {
            this.state = ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.NoChannel;
            return;
          }
        }
        if (waiter is ReliableChannelBinder<TChannel>.ChannelSynchronizer.SyncWaiter)
          waiter.GetChannel(false);
        else
          ActionItem.Schedule(ReliableChannelBinder<TChannel>.ChannelSynchronizer.asyncGetChannelCallback, (object) waiter);
      }

      public void OnReadEof()
      {
        lock (this.ThisLock)
        {
          if (this.count <= 0)
            throw Fx.AssertAndThrow("Caller must ensure that OnReadEof is called before ReturnChannel.");
          if (!this.ValidateOpened())
            return;
          if (this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpened && this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelClosing)
            throw Fx.AssertAndThrow("Since count is positive, the only valid states are ChannelOpened and ChannelClosing.");
          if (this.currentChannel.State == CommunicationState.Faulted)
            return;
          this.state = ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelClosing;
        }
      }

      private bool RemoveWaiter(ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter waiter)
      {
        Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter> waiterQueue = waiter.CanGetChannel ? this.getChannelQueue : this.waitQueue;
        bool flag = false;
        lock (this.ThisLock)
        {
          if (!this.ValidateOpened())
            return false;
          for (int count = waiterQueue.Count; count > 0; --count)
          {
            ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter waiter1 = waiterQueue.Dequeue();
            if (waiter == waiter1)
              flag = true;
            else
              waiterQueue.Enqueue(waiter1);
          }
        }
        return flag;
      }

      public void ReturnChannel()
      {
        TChannel channel = default (TChannel);
        ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter waiter = (ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter) null;
        bool flag1 = false;
        bool flag2 = false;
        bool flag3;
        lock (this.ThisLock)
        {
          if (this.count <= 0)
            throw Fx.AssertAndThrow("Method ReturnChannel() can only be called after TryGetChannel or EndTryGetChannel returns a channel.");
          this.count = this.count - 1;
          flag3 = this.count == 0 && this.drainEvent != null;
          if (this.ValidateOpened())
          {
            if (this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpened && this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelClosing)
              throw Fx.AssertAndThrow("ChannelOpened and ChannelClosing are the only 2 valid states when count is positive.");
            if (this.currentChannel.State == CommunicationState.Faulted)
            {
              flag1 = !this.tolerateFaults;
              this.innerChannelFaulted = true;
              this.state = ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelClosing;
            }
            if (!flag1)
            {
              if (this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelClosing)
              {
                if (this.count == 0)
                {
                  channel = this.currentChannel;
                  flag2 = this.innerChannelFaulted;
                  this.innerChannelFaulted = false;
                  this.state = ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.NoChannel;
                  this.aborting = false;
                  waiter = this.GetChannelWaiter();
                  if (waiter != null)
                    this.state = ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpening;
                }
              }
            }
          }
        }
        if (flag1)
          this.binder.Fault((Exception) null);
        if (flag3)
          this.drainEvent.Set();
        if ((object) channel != null)
        {
          channel.Faulted -= this.onChannelFaulted;
          if (channel.State == CommunicationState.Opened)
            this.binder.CloseChannel(channel);
          else
            channel.Abort();
          if (waiter != null)
            waiter.GetChannel(false);
        }
        if (!flag2)
          return;
        this.binder.OnInnerChannelFaulted();
      }

      public bool SetChannel(TChannel channel)
      {
        lock (this.ThisLock)
        {
          if (this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpening && this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.NoChannel)
            throw Fx.AssertAndThrow("SetChannel is only valid in the NoChannel and ChannelOpening states");
          if (!this.tolerateFaults)
            throw Fx.AssertAndThrow("SetChannel is only valid when masking faults");
          if (!this.ValidateOpened())
            return false;
          this.currentChannel = channel;
          return true;
        }
      }

      private void SetTolerateFaults()
      {
        if (this.faultMode == TolerateFaultsMode.Never)
          this.tolerateFaults = false;
        else if (this.faultMode == TolerateFaultsMode.IfNotSecuritySession)
          this.tolerateFaults = !this.binder.HasSecuritySession(this.currentChannel);
        if (this.onChannelFaulted == null)
          this.onChannelFaulted = new EventHandler(this.OnChannelFaulted);
        this.currentChannel.Faulted += this.onChannelFaulted;
      }

      private void SetWaiters(Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter> waiters, TChannel channel)
      {
        if (waiters == null || waiters.Count <= 0)
          return;
        foreach (ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter waiter in waiters)
          waiter.Set(channel);
      }

      public void StartSynchronizing()
      {
        lock (this.ThisLock)
        {
          if (this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.Created)
          {
            this.state = ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.NoChannel;
            if ((object) this.currentChannel == null && !this.binder.TryGetChannel(TimeSpan.Zero) || ((object) this.currentChannel == null || this.binder.MustOpenChannel))
              return;
            this.state = ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpened;
            this.SetTolerateFaults();
          }
          else if (this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.Closed)
            throw Fx.AssertAndThrow("Abort is the only operation that can race with Open.");
        }
      }

      public TChannel StopSynchronizing(bool close)
      {
        lock (this.ThisLock)
        {
          if (this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.Faulted && this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.Closed)
          {
            this.state = close ? ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.Closed : ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.Faulted;
            if ((object) this.currentChannel != null && this.onChannelFaulted != null)
              this.currentChannel.Faulted -= this.onChannelFaulted;
          }
          return this.currentChannel;
        }
      }

      private bool ThrowIfNecessary(MaskingMode maskingMode)
      {
        if (this.ValidateOpened())
          return true;
        Exception exception = this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.Closed ? this.binder.GetFaultedException(maskingMode) : this.binder.GetClosedException(maskingMode);
        if (exception != null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
        return false;
      }

      public bool TryGetChannelForInput(bool canGetChannel, TimeSpan timeout, out TChannel channel)
      {
        return this.TryGetChannel(canGetChannel, false, timeout, MaskingMode.All, out channel);
      }

      public bool TryGetChannelForOutput(TimeSpan timeout, MaskingMode maskingMode, out TChannel channel)
      {
        return this.TryGetChannel(true, true, timeout, maskingMode, out channel);
      }

      private bool TryGetChannel(bool canGetChannel, bool canCauseFault, TimeSpan timeout, MaskingMode maskingMode, out TChannel channel)
      {
        ReliableChannelBinder<TChannel>.ChannelSynchronizer.SyncWaiter syncWaiter = (ReliableChannelBinder<TChannel>.ChannelSynchronizer.SyncWaiter) null;
        bool flag1 = false;
        bool flag2 = false;
        lock (this.ThisLock)
        {
          if (!this.ThrowIfNecessary(maskingMode))
          {
            channel = default (TChannel);
            return true;
          }
          if (this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpened)
          {
            if ((object) this.currentChannel == null)
              throw Fx.AssertAndThrow("Field currentChannel cannot be null in the ChannelOpened state.");
            this.count = this.count + 1;
            channel = this.currentChannel;
            return true;
          }
          if (!this.tolerateFaults && (this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelClosing || this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.NoChannel))
          {
            if (!canCauseFault)
            {
              channel = default (TChannel);
              return true;
            }
            flag1 = true;
          }
          else if (!canGetChannel || this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpening || this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelClosing)
          {
            syncWaiter = new ReliableChannelBinder<TChannel>.ChannelSynchronizer.SyncWaiter(this, canGetChannel, default (TChannel), timeout, maskingMode, this.binder.ChannelParameters);
            this.GetQueue(canGetChannel).Enqueue((ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter) syncWaiter);
          }
          else
          {
            if (this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.NoChannel)
              throw Fx.AssertAndThrow("The state must be NoChannel.");
            syncWaiter = new ReliableChannelBinder<TChannel>.ChannelSynchronizer.SyncWaiter(this, canGetChannel, this.GetCurrentChannelIfCreated(), timeout, maskingMode, this.binder.ChannelParameters);
            this.state = ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.ChannelOpening;
            flag2 = true;
          }
        }
        if (flag1)
        {
          this.binder.Fault((Exception) null);
          channel = default (TChannel);
          return true;
        }
        if (flag2)
          syncWaiter.GetChannel(true);
        return syncWaiter.TryWait(out channel);
      }

      public void UnblockWaiters()
      {
        Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter> getChannelQueue;
        Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter> waitQueue;
        lock (this.ThisLock)
        {
          getChannelQueue = this.getChannelQueue;
          waitQueue = this.waitQueue;
          this.getChannelQueue = (Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter>) null;
          this.waitQueue = (Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter>) null;
        }
        bool close = this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.Closed;
        this.UnblockWaiters(getChannelQueue, close);
        this.UnblockWaiters(waitQueue, close);
      }

      private void UnblockWaiters(Queue<ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter> waiters, bool close)
      {
        if (waiters == null || waiters.Count <= 0)
          return;
        foreach (ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter waiter in waiters)
        {
          if (close)
            waiter.Close();
          else
            waiter.Fault();
        }
      }

      private bool ValidateOpened()
      {
        if (this.state == ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.Created)
          throw Fx.AssertAndThrow("This operation expects that the synchronizer has been opened.");
        if (this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.Closed)
          return this.state != ReliableChannelBinder<TChannel>.ChannelSynchronizer.State.Faulted;
        return false;
      }

      public void WaitForPendingOperations(TimeSpan timeout)
      {
        lock (this.ThisLock)
        {
          if (this.drainEvent != null)
            throw Fx.AssertAndThrow("The WaitForPendingOperations operation may only be invoked once.");
          if (this.count > 0)
            this.drainEvent = new InterruptibleWaitObject(false, false);
        }
        if (this.drainEvent == null)
          return;
        this.drainEvent.Wait(timeout);
      }

      private enum State
      {
        Created,
        NoChannel,
        ChannelOpening,
        ChannelOpened,
        ChannelClosing,
        Faulted,
        Closed,
      }

      public interface IWaiter
      {
        bool CanGetChannel { get; }

        void Close();

        void Fault();

        void GetChannel(bool onUserThread);

        void Set(TChannel channel);
      }

      public sealed class AsyncWaiter : AsyncResult, ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter
      {
        private static AsyncCallback onOpenComplete = Fx.ThunkCallback(new AsyncCallback(ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter.OnOpenComplete));
        private static Action<object> onTimeoutElapsed = new Action<object>(ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter.OnTimeoutElapsed);
        private static AsyncCallback onTryGetChannelComplete = Fx.ThunkCallback(new AsyncCallback(ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter.OnTryGetChannelComplete));
        private bool isSynchronous = true;
        private bool canGetChannel;
        private TChannel channel;
        private ChannelParameterCollection channelParameters;
        private MaskingMode maskingMode;
        private bool timedOut;
        private ReliableChannelBinder<TChannel>.ChannelSynchronizer synchronizer;
        private System.Runtime.TimeoutHelper timeoutHelper;
        private IOThreadTimer timer;
        private bool timerCancelled;

        public bool CanGetChannel
        {
          get
          {
            return this.canGetChannel;
          }
        }

        private object ThisLock
        {
          get
          {
            return (object) this;
          }
        }

        public AsyncWaiter(ReliableChannelBinder<TChannel>.ChannelSynchronizer synchronizer, bool canGetChannel, TChannel channel, TimeSpan timeout, MaskingMode maskingMode, ChannelParameterCollection channelParameters, AsyncCallback callback, object state)
          : base(callback, state)
        {
          if (!canGetChannel && (object) channel != null)
            throw Fx.AssertAndThrow("This waiter must wait for a channel thus argument channel must be null.");
          this.synchronizer = synchronizer;
          this.canGetChannel = canGetChannel;
          this.channel = channel;
          this.timeoutHelper = new System.Runtime.TimeoutHelper(timeout);
          this.maskingMode = maskingMode;
          this.channelParameters = channelParameters;
        }

        private void CancelTimer()
        {
          lock (this.ThisLock)
          {
            if (this.timerCancelled)
              return;
            if (this.timer != null)
              this.timer.Cancel();
            this.timerCancelled = true;
          }
        }

        public void Close()
        {
          this.CancelTimer();
          this.channel = default (TChannel);
          this.Complete(false, this.synchronizer.binder.GetClosedException(this.maskingMode));
        }

        private bool CompleteOpen(IAsyncResult result)
        {
          this.channel.EndOpen(result);
          return this.OnChannelOpened();
        }

        private bool CompleteTryGetChannel(IAsyncResult result)
        {
          if (!this.synchronizer.binder.EndTryGetChannel(result))
          {
            this.timedOut = true;
            this.OnGetChannelFailed();
            return true;
          }
          if (this.synchronizer.CompleteSetChannel((ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter) this, out this.channel))
            return this.OpenChannel();
          if (!this.IsCompleted)
            throw Fx.AssertAndThrow("CompleteSetChannel must complete the IWaiter if it returns false.");
          return false;
        }

        public bool End(out TChannel channel)
        {
          AsyncResult.End<ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter>((IAsyncResult) this);
          channel = this.channel;
          return !this.timedOut;
        }

        public void Fault()
        {
          this.CancelTimer();
          this.channel = default (TChannel);
          this.Complete(false, this.synchronizer.binder.GetFaultedException(this.maskingMode));
        }

        private bool GetChannel()
        {
          if ((object) this.channel != null)
            return this.OpenChannel();
          IAsyncResult channel = this.synchronizer.binder.BeginTryGetChannel(this.timeoutHelper.RemainingTime(), ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter.onTryGetChannelComplete, (object) this);
          if (channel.CompletedSynchronously)
            return this.CompleteTryGetChannel(channel);
          return false;
        }

        public void GetChannel(bool onUserThread)
        {
          if (!this.CanGetChannel)
            throw Fx.AssertAndThrow("This waiter must wait for a channel thus the caller cannot attempt to get a channel.");
          this.isSynchronous = onUserThread;
          if (onUserThread)
          {
            bool flag = true;
            try
            {
              if (this.GetChannel())
                this.Complete(true);
              flag = false;
            }
            finally
            {
              if (flag)
                this.OnGetChannelFailed();
            }
          }
          else
          {
            bool flag = false;
            Exception exception = (Exception) null;
            try
            {
              this.CancelTimer();
              flag = this.GetChannel();
            }
            catch (Exception ex)
            {
              if (Fx.IsFatal(ex))
              {
                throw;
              }
              else
              {
                this.OnGetChannelFailed();
                exception = ex;
              }
            }
            if (!flag && exception == null)
              return;
            this.Complete(false, exception);
          }
        }

        private bool OnChannelOpened()
        {
          if (this.synchronizer.OnChannelOpened((ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter) this))
            return true;
          if (!this.IsCompleted)
            throw Fx.AssertAndThrow("OnChannelOpened must complete the IWaiter if it returns false.");
          return false;
        }

        private void OnGetChannelFailed()
        {
          if ((object) this.channel != null)
            this.channel.Abort();
          this.synchronizer.OnGetChannelFailed();
        }

        private static void OnOpenComplete(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter asyncState = (ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter) result.AsyncState;
          bool flag = false;
          Exception exception = (Exception) null;
          asyncState.isSynchronous = false;
          try
          {
            flag = asyncState.CompleteOpen(result);
          }
          catch (Exception ex)
          {
            if (Fx.IsFatal(ex))
              throw;
            else
              exception = ex;
          }
          if (flag)
          {
            asyncState.Complete(false);
          }
          else
          {
            if (exception == null)
              return;
            asyncState.OnGetChannelFailed();
            asyncState.Complete(false, exception);
          }
        }

        private void OnTimeoutElapsed()
        {
          if (!this.synchronizer.RemoveWaiter((ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter) this))
            return;
          this.timedOut = true;
          this.Complete(this.isSynchronous, (Exception) null);
        }

        private static void OnTimeoutElapsed(object state)
        {
          ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter asyncWaiter = (ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter) state;
          asyncWaiter.isSynchronous = false;
          asyncWaiter.OnTimeoutElapsed();
        }

        private static void OnTryGetChannelComplete(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter asyncState = (ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter) result.AsyncState;
          asyncState.isSynchronous = false;
          bool flag = false;
          Exception exception = (Exception) null;
          try
          {
            flag = asyncState.CompleteTryGetChannel(result);
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
          if (exception != null)
            asyncState.OnGetChannelFailed();
          asyncState.Complete(asyncState.isSynchronous, exception);
        }

        private bool OpenChannel()
        {
          if (!this.synchronizer.binder.MustOpenChannel)
            return this.OnChannelOpened();
          if (this.channelParameters != null)
            this.channelParameters.PropagateChannelParameters((IChannel) this.channel);
          IAsyncResult result = this.channel.BeginOpen(this.timeoutHelper.RemainingTime(), ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter.onOpenComplete, (object) this);
          if (result.CompletedSynchronously)
            return this.CompleteOpen(result);
          return false;
        }

        public void Set(TChannel channel)
        {
          this.CancelTimer();
          this.channel = channel;
          this.Complete(false);
        }

        public void Wait()
        {
          lock (this.ThisLock)
          {
            if (this.timerCancelled)
              return;
            if (this.timeoutHelper.RemainingTime() > TimeSpan.Zero)
            {
              this.timer = new IOThreadTimer(ReliableChannelBinder<TChannel>.ChannelSynchronizer.AsyncWaiter.onTimeoutElapsed, (object) this, true);
              this.timer.Set(this.timeoutHelper.RemainingTime());
              return;
            }
          }
          this.OnTimeoutElapsed();
        }
      }

      private sealed class SynchronizerCompletedAsyncResult : CompletedAsyncResult
      {
        public SynchronizerCompletedAsyncResult(AsyncCallback callback, object state)
          : base(callback, state)
        {
        }

        public void End()
        {
          CompletedAsyncResult.End((IAsyncResult) this);
        }
      }

      private sealed class SyncWaiter : ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter
      {
        private AutoResetEvent completeEvent = new AutoResetEvent(false);
        private bool canGetChannel;
        private TChannel channel;
        private ChannelParameterCollection channelParameters;
        private Exception exception;
        private bool getChannel;
        private MaskingMode maskingMode;
        private ReliableChannelBinder<TChannel>.ChannelSynchronizer synchronizer;
        private System.Runtime.TimeoutHelper timeoutHelper;

        public bool CanGetChannel
        {
          get
          {
            return this.canGetChannel;
          }
        }

        public SyncWaiter(ReliableChannelBinder<TChannel>.ChannelSynchronizer synchronizer, bool canGetChannel, TChannel channel, TimeSpan timeout, MaskingMode maskingMode, ChannelParameterCollection channelParameters)
        {
          if (!canGetChannel && (object) channel != null)
            throw Fx.AssertAndThrow("This waiter must wait for a channel thus argument channel must be null.");
          this.synchronizer = synchronizer;
          this.canGetChannel = canGetChannel;
          this.channel = channel;
          this.timeoutHelper = new System.Runtime.TimeoutHelper(timeout);
          this.maskingMode = maskingMode;
          this.channelParameters = channelParameters;
        }

        public void Close()
        {
          this.exception = this.synchronizer.binder.GetClosedException(this.maskingMode);
          this.completeEvent.Set();
        }

        public void Fault()
        {
          this.exception = this.synchronizer.binder.GetFaultedException(this.maskingMode);
          this.completeEvent.Set();
        }

        public void GetChannel(bool onUserThread)
        {
          if (!this.CanGetChannel)
            throw Fx.AssertAndThrow("This waiter must wait for a channel thus the caller cannot attempt to get a channel.");
          this.getChannel = true;
          this.completeEvent.Set();
        }

        public void Set(TChannel channel)
        {
          if ((object) channel == null)
            throw Fx.AssertAndThrow("Argument channel cannot be null. Caller must call Fault or Close instead.");
          this.channel = channel;
          this.completeEvent.Set();
        }

        private bool TryGetChannel()
        {
          TChannel channel;
          if ((object) this.channel != null)
            channel = this.channel;
          else if (this.synchronizer.binder.TryGetChannel(this.timeoutHelper.RemainingTime()))
          {
            if (!this.synchronizer.CompleteSetChannel((ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter) this, out channel))
              return true;
          }
          else
          {
            this.synchronizer.OnGetChannelFailed();
            return false;
          }
          if (this.synchronizer.binder.MustOpenChannel)
          {
            bool flag = true;
            if (this.channelParameters != null)
              this.channelParameters.PropagateChannelParameters((IChannel) channel);
            try
            {
              channel.Open(this.timeoutHelper.RemainingTime());
              flag = false;
            }
            finally
            {
              if (flag)
              {
                channel.Abort();
                this.synchronizer.OnGetChannelFailed();
              }
            }
          }
          if (this.synchronizer.OnChannelOpened((ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter) this))
            this.Set(channel);
          return true;
        }

        public bool TryWait(out TChannel channel)
        {
          if (!this.Wait())
          {
            channel = default (TChannel);
            return false;
          }
          if (this.getChannel && !this.TryGetChannel())
          {
            channel = default (TChannel);
            return false;
          }
          this.completeEvent.Close();
          if (this.exception != null)
          {
            if ((object) this.channel != null)
              throw Fx.AssertAndThrow("User of IWaiter called both Set and Fault or Close.");
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.exception);
          }
          channel = this.channel;
          return true;
        }

        private bool Wait()
        {
          if (!System.Runtime.TimeoutHelper.WaitOne((WaitHandle) this.completeEvent, this.timeoutHelper.RemainingTime()))
          {
            if (this.synchronizer.RemoveWaiter((ReliableChannelBinder<TChannel>.ChannelSynchronizer.IWaiter) this))
              return false;
            System.Runtime.TimeoutHelper.WaitOne((WaitHandle) this.completeEvent, TimeSpan.MaxValue);
          }
          return true;
        }
      }
    }

    private sealed class CloseAsyncResult : AsyncResult
    {
      private static AsyncCallback onBinderCloseComplete = Fx.ThunkCallback(new AsyncCallback(ReliableChannelBinder<TChannel>.CloseAsyncResult.OnBinderCloseComplete));
      private static AsyncCallback onChannelCloseComplete = Fx.ThunkCallback(new AsyncCallback(ReliableChannelBinder<TChannel>.CloseAsyncResult.OnChannelCloseComplete));
      private ReliableChannelBinder<TChannel> binder;
      private TChannel channel;
      private MaskingMode maskingMode;
      private System.Runtime.TimeoutHelper timeoutHelper;

      public CloseAsyncResult(ReliableChannelBinder<TChannel> binder, TChannel channel, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
        : base(callback, state)
      {
        this.binder = binder;
        this.channel = channel;
        this.timeoutHelper = new System.Runtime.TimeoutHelper(timeout);
        this.maskingMode = maskingMode;
        bool flag = false;
        try
        {
          this.binder.OnShutdown();
          IAsyncResult result = this.binder.OnBeginClose(timeout, ReliableChannelBinder<TChannel>.CloseAsyncResult.onBinderCloseComplete, (object) this);
          if (result.CompletedSynchronously)
            flag = this.CompleteBinderClose(true, result);
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
          {
            throw;
          }
          else
          {
            this.binder.Abort();
            if (!this.binder.HandleException(ex, this.maskingMode))
              throw;
            else
              flag = true;
          }
        }
        if (!flag)
          return;
        this.Complete(true);
      }

      private bool CompleteBinderClose(bool synchronous, IAsyncResult result)
      {
        this.binder.OnEndClose(result);
        if ((object) this.channel != null)
        {
          result = this.binder.BeginCloseChannel(this.channel, this.timeoutHelper.RemainingTime(), ReliableChannelBinder<TChannel>.CloseAsyncResult.onChannelCloseComplete, (object) this);
          if (result.CompletedSynchronously)
            return this.CompleteChannelClose(synchronous, result);
          return false;
        }
        this.binder.TransitionToClosed();
        return true;
      }

      private bool CompleteChannelClose(bool synchronous, IAsyncResult result)
      {
        this.binder.EndCloseChannel(this.channel, result);
        this.binder.TransitionToClosed();
        return true;
      }

      public void End()
      {
        AsyncResult.End<ReliableChannelBinder<TChannel>.CloseAsyncResult>((IAsyncResult) this);
      }

      private Exception HandleAsyncException(Exception e)
      {
        this.binder.Abort();
        if (this.binder.HandleException(e, this.maskingMode))
          return (Exception) null;
        return e;
      }

      private static void OnBinderCloseComplete(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        ReliableChannelBinder<TChannel>.CloseAsyncResult asyncState = (ReliableChannelBinder<TChannel>.CloseAsyncResult) result.AsyncState;
        bool flag;
        Exception exception;
        try
        {
          flag = asyncState.CompleteBinderClose(false, result);
          exception = (Exception) null;
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
        if (exception != null)
          exception = asyncState.HandleAsyncException(exception);
        asyncState.Complete(false, exception);
      }

      private static void OnChannelCloseComplete(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        ReliableChannelBinder<TChannel>.CloseAsyncResult asyncState = (ReliableChannelBinder<TChannel>.CloseAsyncResult) result.AsyncState;
        bool flag;
        Exception exception;
        try
        {
          flag = asyncState.CompleteChannelClose(false, result);
          exception = (Exception) null;
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
        if (exception != null)
          exception = asyncState.HandleAsyncException(exception);
        asyncState.Complete(false, exception);
      }
    }

    protected abstract class InputAsyncResult<TBinder> : AsyncResult where TBinder : ReliableChannelBinder<TChannel>
    {
      private static AsyncCallback onInputComplete = Fx.ThunkCallback(new AsyncCallback(ReliableChannelBinder<TChannel>.InputAsyncResult<TBinder>.OnInputCompleteStatic));
      private static AsyncCallback onTryGetChannelComplete = Fx.ThunkCallback(new AsyncCallback(ReliableChannelBinder<TChannel>.InputAsyncResult<TBinder>.OnTryGetChannelCompleteStatic));
      private bool isSynchronous = true;
      private bool autoAborted;
      private TBinder binder;
      private bool canGetChannel;
      private TChannel channel;
      private MaskingMode maskingMode;
      private bool success;
      private System.Runtime.TimeoutHelper timeoutHelper;

      public InputAsyncResult(TBinder binder, bool canGetChannel, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
        : base(callback, state)
      {
        this.binder = binder;
        this.canGetChannel = canGetChannel;
        this.timeoutHelper = new System.Runtime.TimeoutHelper(timeout);
        this.maskingMode = maskingMode;
      }

      protected abstract IAsyncResult BeginInput(TBinder binder, TChannel channel, TimeSpan timeout, AsyncCallback callback, object state);

      private bool CompleteInput(IAsyncResult result)
      {
        bool complete;
        try
        {
          this.success = this.EndInput(this.binder, this.channel, result, out complete);
        }
        finally
        {
          this.autoAborted = this.binder.Synchronizer.Aborting;
          this.binder.synchronizer.ReturnChannel();
        }
        return !complete;
      }

      private bool CompleteTryGetChannel(IAsyncResult result, out bool complete)
      {
        complete = false;
        this.success = this.binder.synchronizer.EndTryGetChannel(result, out this.channel);
        if ((object) this.channel == null)
        {
          complete = true;
          return false;
        }
        bool flag = true;
        IAsyncResult result1 = (IAsyncResult) null;
        try
        {
          result1 = this.BeginInput(this.binder, this.channel, this.timeoutHelper.RemainingTime(), ReliableChannelBinder<TChannel>.InputAsyncResult<TBinder>.onInputComplete, (object) this);
          flag = false;
        }
        finally
        {
          if (flag)
          {
            this.autoAborted = this.binder.Synchronizer.Aborting;
            this.binder.synchronizer.ReturnChannel();
          }
        }
        if (result1.CompletedSynchronously)
        {
          if (this.CompleteInput(result1))
          {
            complete = false;
            return true;
          }
          complete = true;
          return false;
        }
        complete = false;
        return false;
      }

      public bool End()
      {
        AsyncResult.End<ReliableChannelBinder<TChannel>.InputAsyncResult<TBinder>>((IAsyncResult) this);
        return this.success;
      }

      protected abstract bool EndInput(TBinder binder, TChannel channel, IAsyncResult result, out bool complete);

      private void OnInputComplete(IAsyncResult result)
      {
        this.isSynchronous = false;
        Exception exception = (Exception) null;
        bool flag;
        try
        {
          flag = this.CompleteInput(result);
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else if (!this.binder.HandleException(ex, this.maskingMode, this.autoAborted))
          {
            exception = ex;
            flag = false;
          }
          else
            flag = true;
        }
        if (flag)
          this.StartOnNonUserThread();
        else
          this.Complete(this.isSynchronous, exception);
      }

      private static void OnInputCompleteStatic(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        ((ReliableChannelBinder<TChannel>.InputAsyncResult<TBinder>) result.AsyncState).OnInputComplete(result);
      }

      private void OnTryGetChannelComplete(IAsyncResult result)
      {
        this.isSynchronous = false;
        bool complete = false;
        Exception exception = (Exception) null;
        bool flag;
        try
        {
          flag = this.CompleteTryGetChannel(result, out complete);
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else if (!this.binder.HandleException(ex, this.maskingMode, this.autoAborted))
          {
            exception = ex;
            flag = false;
          }
          else
            flag = true;
        }
        if (complete & flag)
          throw Fx.AssertAndThrow("The derived class' implementation of CompleteTryGetChannel() cannot indicate that the asynchronous operation should complete and retry.");
        if (flag)
        {
          this.StartOnNonUserThread();
        }
        else
        {
          if (!complete && exception == null)
            return;
          this.Complete(this.isSynchronous, exception);
        }
      }

      private static void OnTryGetChannelCompleteStatic(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        ((ReliableChannelBinder<TChannel>.InputAsyncResult<TBinder>) result.AsyncState).OnTryGetChannelComplete(result);
      }

      protected bool Start()
      {
        bool flag;
        bool complete;
        do
        {
          flag = false;
          complete = false;
          this.autoAborted = false;
          try
          {
            IAsyncResult channelForInput = this.binder.synchronizer.BeginTryGetChannelForInput(this.canGetChannel, this.timeoutHelper.RemainingTime(), ReliableChannelBinder<TChannel>.InputAsyncResult<TBinder>.onTryGetChannelComplete, (object) this);
            if (channelForInput.CompletedSynchronously)
              flag = this.CompleteTryGetChannel(channelForInput, out complete);
          }
          catch (Exception ex)
          {
            if (Fx.IsFatal(ex))
              throw;
            else if (!this.binder.HandleException(ex, this.maskingMode, this.autoAborted))
              throw;
            else
              flag = true;
          }
          if (complete & flag)
            throw Fx.AssertAndThrow("The derived class' implementation of CompleteTryGetChannel() cannot indicate that the asynchronous operation should complete and retry.");
        }
        while (flag);
        return complete;
      }

      private void StartOnNonUserThread()
      {
        bool flag = false;
        Exception exception = (Exception) null;
        try
        {
          flag = this.Start();
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
        this.Complete(false, exception);
      }
    }

    private sealed class MessageRequestContext : ReliableChannelBinder<TChannel>.BinderRequestContext
    {
      public MessageRequestContext(ReliableChannelBinder<TChannel> binder, Message message)
        : base(binder, message)
      {
      }

      protected override void OnAbort()
      {
      }

      protected override void OnClose(TimeSpan timeout)
      {
      }

      protected override IAsyncResult OnBeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
      {
        return (IAsyncResult) new ReliableChannelBinder<TChannel>.MessageRequestContext.ReplyAsyncResult(this, message, timeout, callback, state);
      }

      protected override void OnEndReply(IAsyncResult result)
      {
        ReliableChannelBinder<TChannel>.MessageRequestContext.ReplyAsyncResult.End(result);
      }

      protected override void OnReply(Message message, TimeSpan timeout)
      {
        if (message == null)
          return;
        this.Binder.Send(message, timeout, this.MaskingMode);
      }

      private class ReplyAsyncResult : AsyncResult
      {
        private static AsyncCallback onSend;
        private ReliableChannelBinder<TChannel>.MessageRequestContext context;

        public ReplyAsyncResult(ReliableChannelBinder<TChannel>.MessageRequestContext context, Message message, TimeSpan timeout, AsyncCallback callback, object state)
          : base(callback, state)
        {
          if (message != null)
          {
            if (ReliableChannelBinder<TChannel>.MessageRequestContext.ReplyAsyncResult.onSend == null)
              ReliableChannelBinder<TChannel>.MessageRequestContext.ReplyAsyncResult.onSend = Fx.ThunkCallback(new AsyncCallback(ReliableChannelBinder<TChannel>.MessageRequestContext.ReplyAsyncResult.OnSend));
            this.context = context;
            IAsyncResult result = context.Binder.BeginSend(message, timeout, context.MaskingMode, ReliableChannelBinder<TChannel>.MessageRequestContext.ReplyAsyncResult.onSend, (object) this);
            if (!result.CompletedSynchronously)
              return;
            context.Binder.EndSend(result);
          }
          this.Complete(true);
        }

        public static void End(IAsyncResult result)
        {
          AsyncResult.End<ReliableChannelBinder<TChannel>.MessageRequestContext.ReplyAsyncResult>(result);
        }

        private static void OnSend(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          Exception exception = (Exception) null;
          ReliableChannelBinder<TChannel>.MessageRequestContext.ReplyAsyncResult asyncState = (ReliableChannelBinder<TChannel>.MessageRequestContext.ReplyAsyncResult) result.AsyncState;
          try
          {
            asyncState.context.Binder.EndSend(result);
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
      }
    }

    protected abstract class OutputAsyncResult<TBinder> : AsyncResult where TBinder : ReliableChannelBinder<TChannel>
    {
      private static AsyncCallback onTryGetChannelComplete = Fx.ThunkCallback(new AsyncCallback(ReliableChannelBinder<TChannel>.OutputAsyncResult<TBinder>.OnTryGetChannelCompleteStatic));
      private static AsyncCallback onOutputComplete = Fx.ThunkCallback(new AsyncCallback(ReliableChannelBinder<TChannel>.OutputAsyncResult<TBinder>.OnOutputCompleteStatic));
      private bool autoAborted;
      private TBinder binder;
      private TChannel channel;
      private bool hasChannel;
      private MaskingMode maskingMode;
      private Message message;
      private TimeSpan timeout;
      private System.Runtime.TimeoutHelper timeoutHelper;

      public MaskingMode MaskingMode
      {
        get
        {
          return this.maskingMode;
        }
      }

      public OutputAsyncResult(TBinder binder, AsyncCallback callback, object state)
        : base(callback, state)
      {
        this.binder = binder;
      }

      protected abstract IAsyncResult BeginOutput(TBinder binder, TChannel channel, Message message, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state);

      private void Cleanup()
      {
        if (!this.hasChannel)
          return;
        this.autoAborted = this.binder.Synchronizer.Aborting;
        this.binder.synchronizer.ReturnChannel();
      }

      private bool CompleteOutput(IAsyncResult result)
      {
        this.EndOutput(this.binder, this.channel, this.maskingMode, result);
        this.Cleanup();
        return true;
      }

      private bool CompleteTryGetChannel(IAsyncResult result)
      {
        bool flag = !this.binder.synchronizer.EndTryGetChannel(result, out this.channel);
        if (flag || (object) this.channel == null)
        {
          this.Cleanup();
          if (flag && !ReliableChannelBinderHelper.MaskHandled(this.maskingMode))
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new TimeoutException(this.GetTimeoutString(this.timeout)));
          return true;
        }
        this.hasChannel = true;
        result = this.BeginOutput(this.binder, this.channel, this.message, this.timeoutHelper.RemainingTime(), this.maskingMode, ReliableChannelBinder<TChannel>.OutputAsyncResult<TBinder>.onOutputComplete, (object) this);
        if (result.CompletedSynchronously)
          return this.CompleteOutput(result);
        return false;
      }

      protected abstract void EndOutput(TBinder binder, TChannel channel, MaskingMode maskingMode, IAsyncResult result);

      protected abstract string GetTimeoutString(TimeSpan timeout);

      private void OnOutputComplete(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        Exception exception = (Exception) null;
        bool flag;
        try
        {
          flag = this.CompleteOutput(result);
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
          {
            throw;
          }
          else
          {
            this.Cleanup();
            flag = true;
            if (!this.binder.HandleException(ex, this.maskingMode, this.autoAborted))
              exception = ex;
          }
        }
        if (!flag)
          return;
        this.Complete(false, exception);
      }

      private static void OnOutputCompleteStatic(IAsyncResult result)
      {
        ((ReliableChannelBinder<TChannel>.OutputAsyncResult<TBinder>) result.AsyncState).OnOutputComplete(result);
      }

      private void OnTryGetChannelComplete(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        Exception exception = (Exception) null;
        bool flag;
        try
        {
          flag = this.CompleteTryGetChannel(result);
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
          {
            throw;
          }
          else
          {
            this.Cleanup();
            flag = true;
            if (!this.binder.HandleException(ex, this.maskingMode, this.autoAborted))
              exception = ex;
          }
        }
        if (!flag)
          return;
        this.Complete(false, exception);
      }

      private static void OnTryGetChannelCompleteStatic(IAsyncResult result)
      {
        ((ReliableChannelBinder<TChannel>.OutputAsyncResult<TBinder>) result.AsyncState).OnTryGetChannelComplete(result);
      }

      public void Start(Message message, TimeSpan timeout, MaskingMode maskingMode)
      {
        if (!this.binder.ValidateOutputOperation(message, timeout, maskingMode))
        {
          this.Complete(true);
        }
        else
        {
          this.message = message;
          this.timeout = timeout;
          this.timeoutHelper = new System.Runtime.TimeoutHelper(timeout);
          this.maskingMode = maskingMode;
          bool flag = false;
          try
          {
            IAsyncResult channelForOutput = this.binder.synchronizer.BeginTryGetChannelForOutput(this.timeoutHelper.RemainingTime(), this.maskingMode, ReliableChannelBinder<TChannel>.OutputAsyncResult<TBinder>.onTryGetChannelComplete, (object) this);
            if (channelForOutput.CompletedSynchronously)
              flag = this.CompleteTryGetChannel(channelForOutput);
          }
          catch (Exception ex)
          {
            if (Fx.IsFatal(ex))
            {
              throw;
            }
            else
            {
              this.Cleanup();
              if (this.binder.HandleException(ex, this.maskingMode, this.autoAborted))
                flag = true;
              else
                throw;
            }
          }
          if (!flag)
            return;
          this.Complete(true);
        }
      }
    }

    private sealed class RequestRequestContext : ReliableChannelBinder<TChannel>.BinderRequestContext
    {
      private RequestContext innerContext;

      public RequestRequestContext(ReliableChannelBinder<TChannel> binder, RequestContext innerContext, Message message)
        : base(binder, message)
      {
        if (binder.defaultMaskingMode != MaskingMode.All && !binder.TolerateFaults)
          throw Fx.AssertAndThrow("This request context is designed to catch exceptions. Thus it cannot be used if the caller expects no exception handling.");
        if (innerContext == null)
          throw Fx.AssertAndThrow("Argument innerContext cannot be null.");
        this.innerContext = innerContext;
      }

      protected override void OnAbort()
      {
        this.innerContext.Abort();
      }

      protected override IAsyncResult OnBeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
      {
        try
        {
          if (message != null)
            this.Binder.AddOutputHeaders(message);
          return this.innerContext.BeginReply(message, timeout, callback, state);
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else if (!this.Binder.HandleException(ex, this.MaskingMode))
            throw;
          else
            this.innerContext.Abort();
        }
        return (IAsyncResult) new ReliableChannelBinder<TChannel>.BinderCompletedAsyncResult(callback, state);
      }

      protected override void OnClose(TimeSpan timeout)
      {
        try
        {
          this.innerContext.Close(timeout);
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else if (!this.Binder.HandleException(ex, this.MaskingMode))
            throw;
          else
            this.innerContext.Abort();
        }
      }

      protected override void OnEndReply(IAsyncResult result)
      {
        ReliableChannelBinder<TChannel>.BinderCompletedAsyncResult completedAsyncResult = result as ReliableChannelBinder<TChannel>.BinderCompletedAsyncResult;
        if (completedAsyncResult != null)
        {
          completedAsyncResult.End();
        }
        else
        {
          try
          {
            this.innerContext.EndReply(result);
          }
          catch (ObjectDisposedException)
          {
          }
          catch (Exception ex)
          {
            if (Fx.IsFatal(ex))
              throw;
            else if (!this.Binder.HandleException(ex, this.MaskingMode))
              throw;
            else
              this.innerContext.Abort();
          }
        }
      }

      protected override void OnReply(Message message, TimeSpan timeout)
      {
        try
        {
          if (message != null)
            this.Binder.AddOutputHeaders(message);
          this.innerContext.Reply(message, timeout);
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else if (!this.Binder.HandleException(ex, this.MaskingMode))
            throw;
          else
            this.innerContext.Abort();
        }
      }
    }

    private sealed class SendAsyncResult : ReliableChannelBinder<TChannel>.OutputAsyncResult<ReliableChannelBinder<TChannel>>
    {
      public SendAsyncResult(ReliableChannelBinder<TChannel> binder, AsyncCallback callback, object state)
        : base(binder, callback, state)
      {
      }

      protected override IAsyncResult BeginOutput(ReliableChannelBinder<TChannel> binder, TChannel channel, Message message, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
      {
        binder.AddOutputHeaders(message);
        return binder.OnBeginSend(channel, message, timeout, callback, state);
      }

      public static void End(IAsyncResult result)
      {
        AsyncResult.End<ReliableChannelBinder<TChannel>.SendAsyncResult>(result);
      }

      protected override void EndOutput(ReliableChannelBinder<TChannel> binder, TChannel channel, MaskingMode maskingMode, IAsyncResult result)
      {
        binder.OnEndSend(channel, result);
      }

      protected override string GetTimeoutString(TimeSpan timeout)
      {
        return SR.GetString("TimeoutOnSend", new object[1]{ (object) timeout });
      }
    }

    private sealed class TryReceiveAsyncResult : ReliableChannelBinder<TChannel>.InputAsyncResult<ReliableChannelBinder<TChannel>>
    {
      private RequestContext requestContext;

      public TryReceiveAsyncResult(ReliableChannelBinder<TChannel> binder, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
        : base(binder, binder.CanGetChannelForReceive, timeout, maskingMode, callback, state)
      {
        if (!this.Start())
          return;
        this.Complete(true);
      }

      protected override IAsyncResult BeginInput(ReliableChannelBinder<TChannel> binder, TChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
      {
        return binder.OnBeginTryReceive(channel, timeout, callback, state);
      }

      public bool End(out RequestContext requestContext)
      {
        requestContext = this.requestContext;
        return this.End();
      }

      protected override bool EndInput(ReliableChannelBinder<TChannel> binder, TChannel channel, IAsyncResult result, out bool complete)
      {
        bool flag = binder.OnEndTryReceive(channel, result, out this.requestContext);
        complete = !flag || this.requestContext != null;
        if (!complete)
          binder.synchronizer.OnReadEof();
        return flag;
      }
    }
  }
}
