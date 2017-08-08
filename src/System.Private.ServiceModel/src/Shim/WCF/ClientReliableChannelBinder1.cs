// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.ClientReliableChannelBinder`1
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Runtime;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
  internal abstract class ClientReliableChannelBinder<TChannel> : ReliableChannelBinder<TChannel>, IClientReliableChannelBinder, IReliableChannelBinder where TChannel : class, IChannel
  {
    private ChannelParameterCollection channelParameters;
    private IChannelFactory<TChannel> factory;
    private EndpointAddress to;
    private Uri via;

    protected override bool CanGetChannelForReceive
    {
      get
      {
        return false;
      }
    }

    public override bool CanSendAsynchronously
    {
      get
      {
        return true;
      }
    }

    public override ChannelParameterCollection ChannelParameters
    {
      get
      {
        return this.channelParameters;
      }
    }

    protected override bool MustCloseChannel
    {
      get
      {
        return true;
      }
    }

    protected override bool MustOpenChannel
    {
      get
      {
        return true;
      }
    }

    public Uri Via
    {
      get
      {
        return this.via;
      }
    }

    protected ClientReliableChannelBinder(EndpointAddress to, Uri via, IChannelFactory<TChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode, ChannelParameterCollection channelParameters, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
      : base(factory.CreateChannel(to, via), maskingMode, faultMode, defaultCloseTimeout, defaultSendTimeout)
    {
      if (channelParameters == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("channelParameters");
      this.to = to;
      this.via = via;
      this.factory = factory;
      this.channelParameters = channelParameters;
    }

    public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
    {
      return this.BeginRequest(message, timeout, this.DefaultMaskingMode, callback, state);
    }

    public IAsyncResult BeginRequest(Message message, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
    {
      ClientReliableChannelBinder<TChannel>.RequestAsyncResult requestAsyncResult = new ClientReliableChannelBinder<TChannel>.RequestAsyncResult(this, callback, state);
      requestAsyncResult.Start(message, timeout, maskingMode);
      return (IAsyncResult) requestAsyncResult;
    }

    protected override IAsyncResult BeginTryGetChannel(TimeSpan timeout, AsyncCallback callback, object state)
    {
      TChannel data;
      switch (this.State)
      {
        case CommunicationState.Created:
        case CommunicationState.Opening:
        case CommunicationState.Opened:
          data = this.factory.CreateChannel(this.to, this.via);
          break;
        default:
          data = default (TChannel);
          break;
      }
      return (IAsyncResult) new CompletedAsyncResult<TChannel>(data, callback, state);
    }

    public static IClientReliableChannelBinder CreateBinder(EndpointAddress to, Uri via, IChannelFactory<TChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode, ChannelParameterCollection channelParameters, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
    {
      System.Type type = typeof (TChannel);
      if (type == typeof (IDuplexChannel))
        return (IClientReliableChannelBinder) new ClientReliableChannelBinder<TChannel>.DuplexClientReliableChannelBinder(to, via, (IChannelFactory<IDuplexChannel>) factory, maskingMode, channelParameters, defaultCloseTimeout, defaultSendTimeout);
      if (type == typeof (IDuplexSessionChannel))
        return (IClientReliableChannelBinder) new ClientReliableChannelBinder<TChannel>.DuplexSessionClientReliableChannelBinder(to, via, (IChannelFactory<IDuplexSessionChannel>) factory, maskingMode, faultMode, channelParameters, defaultCloseTimeout, defaultSendTimeout);
      if (type == typeof (IRequestChannel))
        return (IClientReliableChannelBinder) new ClientReliableChannelBinder<TChannel>.RequestClientReliableChannelBinder(to, via, (IChannelFactory<IRequestChannel>) factory, maskingMode, channelParameters, defaultCloseTimeout, defaultSendTimeout);
      if (type == typeof (IRequestSessionChannel))
        return (IClientReliableChannelBinder) new ClientReliableChannelBinder<TChannel>.RequestSessionClientReliableChannelBinder(to, via, (IChannelFactory<IRequestSessionChannel>) factory, maskingMode, faultMode, channelParameters, defaultCloseTimeout, defaultSendTimeout);
      throw Fx.AssertAndThrow("ClientReliableChannelBinder supports creation of IDuplexChannel, IDuplexSessionChannel, IRequestChannel, and IRequestSessionChannel only.");
    }

    public Message EndRequest(IAsyncResult result)
    {
      return ClientReliableChannelBinder<TChannel>.RequestAsyncResult.End(result);
    }

    protected override bool EndTryGetChannel(IAsyncResult result)
    {
      TChannel channel = CompletedAsyncResult<TChannel>.End(result);
      if ((object) channel != null && !this.Synchronizer.SetChannel(channel))
        channel.Abort();
      return true;
    }

    public bool EnsureChannelForRequest()
    {
      return this.Synchronizer.EnsureChannel();
    }

    protected override void OnAbort()
    {
    }

    protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
    {
      return (IAsyncResult) new CompletedAsyncResult(callback, state);
    }

    protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
    {
      return (IAsyncResult) new CompletedAsyncResult(callback, state);
    }

    protected virtual IAsyncResult OnBeginRequest(TChannel channel, Message message, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
    {
      throw Fx.AssertAndThrow("The derived class does not support the OnBeginRequest operation.");
    }

    protected override void OnClose(TimeSpan timeout)
    {
    }

    protected override void OnEndClose(IAsyncResult result)
    {
      CompletedAsyncResult.End(result);
    }

    protected override void OnEndOpen(IAsyncResult result)
    {
      CompletedAsyncResult.End(result);
    }

    protected virtual Message OnEndRequest(TChannel channel, MaskingMode maskingMode, IAsyncResult result)
    {
      throw Fx.AssertAndThrow("The derived class does not support the OnEndRequest operation.");
    }

    protected override void OnOpen(TimeSpan timeout)
    {
    }

    protected virtual Message OnRequest(TChannel channel, Message message, TimeSpan timeout, MaskingMode maskingMode)
    {
      throw Fx.AssertAndThrow("The derived class does not support the OnRequest operation.");
    }

    public Message Request(Message message, TimeSpan timeout)
    {
      return this.Request(message, timeout, this.DefaultMaskingMode);
    }

    public Message Request(Message message, TimeSpan timeout, MaskingMode maskingMode)
    {
      if (!this.ValidateOutputOperation(message, timeout, maskingMode))
        return (Message) null;
      bool autoAborted = false;
      try
      {
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        TChannel channel;
        if (!this.Synchronizer.TryGetChannelForOutput(timeoutHelper.RemainingTime(), maskingMode, out channel))
        {
          if (!ReliableChannelBinderHelper.MaskHandled(maskingMode))
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new TimeoutException(SR.GetString("TimeoutOnRequest", new object[1]{ (object) timeout })));
          return (Message) null;
        }
        if ((object) channel == null)
          return (Message) null;
        try
        {
          return this.OnRequest(channel, message, timeoutHelper.RemainingTime(), maskingMode);
        }
        finally
        {
          autoAborted = this.Synchronizer.Aborting;
          this.Synchronizer.ReturnChannel();
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
            return (Message) null;
          throw;
        }
      }
    }

    protected override bool TryGetChannel(TimeSpan timeout)
    {
      CommunicationState state = this.State;
      // CS0219
      // TChannel channel1 = default (TChannel);
      if (state == CommunicationState.Created || state == CommunicationState.Opening || state == CommunicationState.Opened)
      {
        TChannel channel2 = this.factory.CreateChannel(this.to, this.via);
        if (!this.Synchronizer.SetChannel(channel2))
          channel2.Abort();
      }
      // else
      //   channel1 = default (TChannel);
      return true;
    }

    private abstract class DuplexClientReliableChannelBinder<TDuplexChannel> : ClientReliableChannelBinder<TDuplexChannel> where TDuplexChannel : class, IDuplexChannel
    {
      public override EndpointAddress LocalAddress
      {
        get
        {
          IDuplexChannel currentChannel = (IDuplexChannel) this.Synchronizer.CurrentChannel;
          if (currentChannel == null)
            return (EndpointAddress) null;
          return currentChannel.LocalAddress;
        }
      }

      public override EndpointAddress RemoteAddress
      {
        get
        {
          IDuplexChannel currentChannel = (IDuplexChannel) this.Synchronizer.CurrentChannel;
          if (currentChannel == null)
            return (EndpointAddress) null;
          return currentChannel.RemoteAddress;
        }
      }

      public DuplexClientReliableChannelBinder(EndpointAddress to, Uri via, IChannelFactory<TDuplexChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode, ChannelParameterCollection channelParameters, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
        : base(to, via, factory, maskingMode, faultMode, channelParameters, defaultCloseTimeout, defaultSendTimeout)
      {
      }

      protected override IAsyncResult OnBeginSend(TDuplexChannel channel, Message message, TimeSpan timeout, AsyncCallback callback, object state)
      {
        return channel.BeginSend(message, timeout, callback, state);
      }

      protected override IAsyncResult OnBeginTryReceive(TDuplexChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
      {
        return channel.BeginTryReceive(timeout, callback, state);
      }

      protected override void OnEndSend(TDuplexChannel channel, IAsyncResult result)
      {
        channel.EndSend(result);
      }

      protected override bool OnEndTryReceive(TDuplexChannel channel, IAsyncResult result, out RequestContext requestContext)
      {
        Message message;
        bool flag = channel.EndTryReceive(result, out message);
        if (flag && message == null)
          this.OnReadNullMessage();
        requestContext = this.WrapMessage(message);
        return flag;
      }

      protected virtual void OnReadNullMessage()
      {
      }

      protected override void OnSend(TDuplexChannel channel, Message message, TimeSpan timeout)
      {
        channel.Send(message, timeout);
      }

      protected override bool OnTryReceive(TDuplexChannel channel, TimeSpan timeout, out RequestContext requestContext)
      {
        Message message;
        bool flag = channel.TryReceive(timeout, out message);
        if (flag && message == null)
          this.OnReadNullMessage();
        requestContext = this.WrapMessage(message);
        return flag;
      }
    }

    private sealed class DuplexClientReliableChannelBinder : ClientReliableChannelBinder<TChannel>.DuplexClientReliableChannelBinder<IDuplexChannel>
    {
      public override bool HasSession
      {
        get
        {
          return false;
        }
      }

      public DuplexClientReliableChannelBinder(EndpointAddress to, Uri via, IChannelFactory<IDuplexChannel> factory, MaskingMode maskingMode, ChannelParameterCollection channelParameters, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
        : base(to, via, factory, maskingMode, TolerateFaultsMode.Never, channelParameters, defaultCloseTimeout, defaultSendTimeout)
      {
      }

      public override ISession GetInnerSession()
      {
        return (ISession) null;
      }

      protected override bool HasSecuritySession(IDuplexChannel channel)
      {
        return false;
      }
    }

    private sealed class DuplexSessionClientReliableChannelBinder : ClientReliableChannelBinder<TChannel>.DuplexClientReliableChannelBinder<IDuplexSessionChannel>
    {
      public override bool HasSession
      {
        get
        {
          return true;
        }
      }

      public DuplexSessionClientReliableChannelBinder(EndpointAddress to, Uri via, IChannelFactory<IDuplexSessionChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode, ChannelParameterCollection channelParameters, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
        : base(to, via, factory, maskingMode, faultMode, channelParameters, defaultCloseTimeout, defaultSendTimeout)
      {
      }

      public override ISession GetInnerSession()
      {
        return (ISession) this.Synchronizer.CurrentChannel.Session;
      }

      protected override IAsyncResult BeginCloseChannel(IDuplexSessionChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
      {
        return ReliableChannelBinderHelper.BeginCloseDuplexSessionChannel((ReliableChannelBinder<IDuplexSessionChannel>) this, channel, timeout, callback, state);
      }

      protected override void CloseChannel(IDuplexSessionChannel channel, TimeSpan timeout)
      {
        ReliableChannelBinderHelper.CloseDuplexSessionChannel((ReliableChannelBinder<IDuplexSessionChannel>) this, channel, timeout);
      }

      protected override void EndCloseChannel(IDuplexSessionChannel channel, IAsyncResult result)
      {
        ReliableChannelBinderHelper.EndCloseDuplexSessionChannel(channel, result);
      }

      protected override bool HasSecuritySession(IDuplexSessionChannel channel)
      {
        return channel.Session is ISecuritySession;
      }

      protected override void OnReadNullMessage()
      {
        this.Synchronizer.OnReadEof();
      }
    }

    private abstract class RequestClientReliableChannelBinder<TRequestChannel> : ClientReliableChannelBinder<TRequestChannel> where TRequestChannel : class, IRequestChannel
    {
      private InputQueue<Message> inputMessages;

      public override EndpointAddress LocalAddress
      {
        get
        {
          return EndpointAddress.AnonymousAddress;
        }
      }

      public override EndpointAddress RemoteAddress
      {
        get
        {
          IRequestChannel currentChannel = (IRequestChannel) this.Synchronizer.CurrentChannel;
          if (currentChannel == null)
            return (EndpointAddress) null;
          return currentChannel.RemoteAddress;
        }
      }

      public RequestClientReliableChannelBinder(EndpointAddress to, Uri via, IChannelFactory<TRequestChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode, ChannelParameterCollection channelParameters, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
        : base(to, via, factory, maskingMode, faultMode, channelParameters, defaultCloseTimeout, defaultSendTimeout)
      {
      }

      public override IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
      {
        return this.GetInputMessages().BeginDequeue(timeout, callback, state);
      }

      public override bool EndTryReceive(IAsyncResult result, out RequestContext requestContext)
      {
        Message message;
        bool flag = this.GetInputMessages().EndDequeue(result, out message);
        requestContext = this.WrapMessage(message);
        return flag;
      }

      protected void EnqueueMessageIfNotNull(Message message)
      {
        if (message == null)
          return;
        this.GetInputMessages().EnqueueAndDispatch(message);
      }

      private InputQueue<Message> GetInputMessages()
      {
        lock (this.ThisLock)
        {
          if (this.State == CommunicationState.Created)
            throw Fx.AssertAndThrow("The method GetInputMessages() cannot be called when the binder is in the Created state.");
          if (this.State == CommunicationState.Opening)
            throw Fx.AssertAndThrow("The method GetInputMessages() cannot be called when the binder is in the Opening state.");
          if (this.inputMessages == null)
            this.inputMessages = TraceUtility.CreateInputQueue<Message>();
        }
        return this.inputMessages;
      }

      protected override IAsyncResult OnBeginRequest(TRequestChannel channel, Message message, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
      {
        return channel.BeginRequest(message, timeout, callback, state);
      }

      protected override IAsyncResult OnBeginSend(TRequestChannel channel, Message message, TimeSpan timeout, AsyncCallback callback, object state)
      {
        return channel.BeginRequest(message, timeout, callback, state);
      }

      protected override Message OnEndRequest(TRequestChannel channel, MaskingMode maskingMode, IAsyncResult result)
      {
        return channel.EndRequest(result);
      }

      protected override void OnEndSend(TRequestChannel channel, IAsyncResult result)
      {
        this.EnqueueMessageIfNotNull(channel.EndRequest(result));
      }

      protected override Message OnRequest(TRequestChannel channel, Message message, TimeSpan timeout, MaskingMode maskingMode)
      {
        return channel.Request(message, timeout);
      }

      protected override void OnSend(TRequestChannel channel, Message message, TimeSpan timeout)
      {
        message = channel.Request(message, timeout);
        this.EnqueueMessageIfNotNull(message);
      }

      protected override void OnShutdown()
      {
        if (this.inputMessages == null)
          return;
        this.inputMessages.Close();
      }

      public override bool TryReceive(TimeSpan timeout, out RequestContext requestContext)
      {
        Message message;
        bool flag = this.GetInputMessages().Dequeue(timeout, out message);
        requestContext = this.WrapMessage(message);
        return flag;
      }
    }

    private sealed class RequestAsyncResult : ReliableChannelBinder<TChannel>.OutputAsyncResult<ClientReliableChannelBinder<TChannel>>
    {
      private Message reply;

      public RequestAsyncResult(ClientReliableChannelBinder<TChannel> binder, AsyncCallback callback, object state)
        : base(binder, callback, state)
      {
      }

      protected override IAsyncResult BeginOutput(ClientReliableChannelBinder<TChannel> binder, TChannel channel, Message message, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
      {
        return binder.OnBeginRequest(channel, message, timeout, maskingMode, callback, state);
      }

      public static Message End(IAsyncResult result)
      {
        return AsyncResult.End<ClientReliableChannelBinder<TChannel>.RequestAsyncResult>(result).reply;
      }

      protected override void EndOutput(ClientReliableChannelBinder<TChannel> binder, TChannel channel, MaskingMode maskingMode, IAsyncResult result)
      {
        this.reply = binder.OnEndRequest(channel, maskingMode, result);
      }

      protected override string GetTimeoutString(TimeSpan timeout)
      {
        return SR.GetString("TimeoutOnRequest", new object[1]{ (object) timeout });
      }
    }

    private sealed class RequestClientReliableChannelBinder : ClientReliableChannelBinder<TChannel>.RequestClientReliableChannelBinder<IRequestChannel>
    {
      public override bool HasSession
      {
        get
        {
          return false;
        }
      }

      public RequestClientReliableChannelBinder(EndpointAddress to, Uri via, IChannelFactory<IRequestChannel> factory, MaskingMode maskingMode, ChannelParameterCollection channelParameters, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
        : base(to, via, factory, maskingMode, TolerateFaultsMode.Never, channelParameters, defaultCloseTimeout, defaultSendTimeout)
      {
      }

      public override ISession GetInnerSession()
      {
        return (ISession) null;
      }

      protected override bool HasSecuritySession(IRequestChannel channel)
      {
        return false;
      }
    }

    private sealed class RequestSessionClientReliableChannelBinder : ClientReliableChannelBinder<TChannel>.RequestClientReliableChannelBinder<IRequestSessionChannel>
    {
      public override bool HasSession
      {
        get
        {
          return true;
        }
      }

      public RequestSessionClientReliableChannelBinder(EndpointAddress to, Uri via, IChannelFactory<IRequestSessionChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode, ChannelParameterCollection channelParameters, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
        : base(to, via, factory, maskingMode, faultMode, channelParameters, defaultCloseTimeout, defaultSendTimeout)
      {
      }

      public override ISession GetInnerSession()
      {
        return (ISession) this.Synchronizer.CurrentChannel.Session;
      }

      protected override bool HasSecuritySession(IRequestSessionChannel channel)
      {
        return channel.Session is ISecuritySession;
      }
    }
  }
}
