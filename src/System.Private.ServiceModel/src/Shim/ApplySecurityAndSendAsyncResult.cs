// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.ApplySecurityAndSendAsyncResult`1
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Runtime;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Security
{
  internal abstract class ApplySecurityAndSendAsyncResult<MessageSenderType> : AsyncResult where MessageSenderType : class
  {
    private static AsyncCallback sharedCallback = Fx.ThunkCallback(new AsyncCallback(ApplySecurityAndSendAsyncResult<MessageSenderType>.SharedCallback));
    private readonly MessageSenderType channel;
    private readonly SecurityProtocol binding;
    private volatile bool secureOutgoingMessageDone;
    private SecurityProtocolCorrelationState newCorrelationState;
    private TimeoutHelper timeoutHelper;

    protected SecurityProtocolCorrelationState CorrelationState
    {
      get
      {
        return this.newCorrelationState;
      }
    }

    protected SecurityProtocol SecurityProtocol
    {
      get
      {
        return this.binding;
      }
    }

    public ApplySecurityAndSendAsyncResult(SecurityProtocol binding, MessageSenderType channel, TimeSpan timeout, AsyncCallback callback, object state)
      : base(callback, state)
    {
      this.binding = binding;
      this.channel = channel;
      this.timeoutHelper = new TimeoutHelper(timeout);
    }

    protected void Begin(Message message, SecurityProtocolCorrelationState correlationState)
    {
      IAsyncResult result = this.binding.BeginSecureOutgoingMessage(message, this.timeoutHelper.RemainingTime(), correlationState, ApplySecurityAndSendAsyncResult<MessageSenderType>.sharedCallback, (object) this);
      if (!result.CompletedSynchronously)
        return;
      this.binding.EndSecureOutgoingMessage(result, out message, out this.newCorrelationState);
      if (!this.OnSecureOutgoingMessageComplete(message))
        return;
      this.Complete(true);
    }

    protected static void OnEnd(ApplySecurityAndSendAsyncResult<MessageSenderType> self)
    {
      AsyncResult.End<ApplySecurityAndSendAsyncResult<MessageSenderType>>((IAsyncResult) self);
    }

    private bool OnSecureOutgoingMessageComplete(Message message)
    {
      if (message == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("message"));
      this.secureOutgoingMessageDone = true;
      IAsyncResult result = this.BeginSendCore(this.channel, message, this.timeoutHelper.RemainingTime(), ApplySecurityAndSendAsyncResult<MessageSenderType>.sharedCallback, (object) this);
      if (!result.CompletedSynchronously)
        return false;
      this.EndSendCore(this.channel, result);
      return this.OnSendComplete();
    }

    protected abstract IAsyncResult BeginSendCore(MessageSenderType channel, Message message, TimeSpan timeout, AsyncCallback callback, object state);

    protected abstract void EndSendCore(MessageSenderType channel, IAsyncResult result);

    private bool OnSendComplete()
    {
      this.OnSendCompleteCore(this.timeoutHelper.RemainingTime());
      return true;
    }

    protected abstract void OnSendCompleteCore(TimeSpan timeout);

    private static void SharedCallback(IAsyncResult result)
    {
      if (result == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("result"));
      if (result.CompletedSynchronously)
        return;
      ApplySecurityAndSendAsyncResult<MessageSenderType> asyncState = result.AsyncState as ApplySecurityAndSendAsyncResult<MessageSenderType>;
      if (asyncState == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR.GetString("InvalidAsyncResult"), "result"));
      Exception exception = (Exception) null;
      bool flag;
      try
      {
        if (!asyncState.secureOutgoingMessageDone)
        {
          Message message;
          asyncState.binding.EndSecureOutgoingMessage(result, out message, out asyncState.newCorrelationState);
          flag = asyncState.OnSecureOutgoingMessageComplete(message);
        }
        else
        {
          asyncState.EndSendCore(asyncState.channel, result);
          flag = asyncState.OnSendComplete();
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
  }
}
