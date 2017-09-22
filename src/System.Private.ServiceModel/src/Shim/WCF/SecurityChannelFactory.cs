// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.SecurityChannelFactory`1
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.Runtime;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
  internal sealed class SecurityChannelFactory<TChannel> : LayeredChannelFactory<TChannel>
  {
    private ChannelBuilder channelBuilder;
    private SecurityProtocolFactory securityProtocolFactory;
    private SecuritySessionClientSettings<TChannel> sessionClientSettings;
    private bool sessionMode;
    private MessageVersion messageVersion;
    private ISecurityCapabilities securityCapabilities;

    public ChannelBuilder ChannelBuilder
    {
      get
      {
        return this.channelBuilder;
      }
    }

    public SecurityProtocolFactory SecurityProtocolFactory
    {
      get
      {
        return this.securityProtocolFactory;
      }
    }

    public SecuritySessionClientSettings<TChannel> SessionClientSettings
    {
      get
      {
        return this.sessionClientSettings;
      }
    }

    public bool SessionMode
    {
      get
      {
        return this.sessionMode;
      }
    }

    private bool SupportsDuplex
    {
      get
      {
        this.ThrowIfProtocolFactoryNotSet();
        return this.securityProtocolFactory.SupportsDuplex;
      }
    }

    private bool SupportsRequestReply
    {
      get
      {
        this.ThrowIfProtocolFactoryNotSet();
        return this.securityProtocolFactory.SupportsRequestReply;
      }
    }

    public MessageVersion MessageVersion
    {
      get
      {
        return this.messageVersion;
      }
    }

#region Fromwcf
    protected internal override Task OnOpenAsync(TimeSpan timeout)
    {
        this.OnOpen(timeout);
        return TaskHelpers.CompletedTask();
    }
#endregion

    public SecurityChannelFactory(ISecurityCapabilities securityCapabilities, BindingContext context, SecuritySessionClientSettings<TChannel> sessionClientSettings)
      : this(securityCapabilities, context, sessionClientSettings.ChannelBuilder, sessionClientSettings.CreateInnerChannelFactory())
    {
      this.sessionMode = true;
      this.sessionClientSettings = sessionClientSettings;
    }

    public SecurityChannelFactory(ISecurityCapabilities securityCapabilities, BindingContext context, ChannelBuilder channelBuilder, SecurityProtocolFactory protocolFactory)
      : this(securityCapabilities, context, channelBuilder, protocolFactory, (IChannelFactory) channelBuilder.BuildChannelFactory<TChannel>())
    {
    }

    public SecurityChannelFactory(ISecurityCapabilities securityCapabilities, BindingContext context, ChannelBuilder channelBuilder, SecurityProtocolFactory protocolFactory, IChannelFactory innerChannelFactory)
      : this(securityCapabilities, context, channelBuilder, innerChannelFactory)
    {
      this.securityProtocolFactory = protocolFactory;
    }

    private SecurityChannelFactory(ISecurityCapabilities securityCapabilities, BindingContext context, ChannelBuilder channelBuilder, IChannelFactory innerChannelFactory)
      : base((IDefaultCommunicationTimeouts) context.Binding, innerChannelFactory)
    {
      this.channelBuilder = channelBuilder;
      this.messageVersion = context.Binding.MessageVersion;
      this.securityCapabilities = securityCapabilities;
    }

    internal SecurityChannelFactory(Binding binding, SecurityProtocolFactory protocolFactory, IChannelFactory innerChannelFactory)
      : base((IDefaultCommunicationTimeouts) binding, innerChannelFactory)
    {
      this.securityProtocolFactory = protocolFactory;
    }

    private void CloseProtocolFactory(bool aborted, TimeSpan timeout)
    {
      if (this.securityProtocolFactory == null || this.SessionMode)
        return;
      this.securityProtocolFactory.Close(aborted, timeout);
      this.securityProtocolFactory = (SecurityProtocolFactory) null;
    }

    public override T GetProperty<T>()
    {
      if (this.SessionMode && typeof (T) == typeof (IChannelSecureConversationSessionSettings))
      {
        return (T) Convert.ChangeType(this.SessionClientSettings, typeof(T));
      }
      if (typeof (T) == typeof (ISecurityCapabilities))
        return (T) (object)this.securityCapabilities;
      return base.GetProperty<T>();
    }

    protected override void OnAbort()
    {
      base.OnAbort();
      this.CloseProtocolFactory(true, TimeSpan.Zero);
      if (this.sessionClientSettings == null)
        return;
      this.sessionClientSettings.Abort();
    }

    protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
    {
      List<OperationWithTimeoutBeginCallback> timeoutBeginCallbackList = new List<OperationWithTimeoutBeginCallback>();
      List<OperationEndCallback> operationEndCallbackList = new List<OperationEndCallback>();
      timeoutBeginCallbackList.Add(new OperationWithTimeoutBeginCallback(base.OnBeginClose));
      operationEndCallbackList.Add(new OperationEndCallback(base.OnEndClose));
      if (this.securityProtocolFactory != null && !this.SessionMode)
      {
        timeoutBeginCallbackList.Add(new OperationWithTimeoutBeginCallback(this.securityProtocolFactory.BeginClose));
        operationEndCallbackList.Add(new OperationEndCallback(this.securityProtocolFactory.EndClose));
      }
      if (this.sessionClientSettings != null)
      {
        timeoutBeginCallbackList.Add(new OperationWithTimeoutBeginCallback(this.sessionClientSettings.BeginClose));
        operationEndCallbackList.Add(new OperationEndCallback(this.sessionClientSettings.EndClose));
      }
      return OperationWithTimeoutComposer.BeginComposeAsyncOperations(timeout, timeoutBeginCallbackList.ToArray(), operationEndCallbackList.ToArray(), callback, state);
    }

    protected override void OnEndClose(IAsyncResult result)
    {
      OperationWithTimeoutComposer.EndComposeAsyncOperations(result);
    }

    protected override void OnClose(TimeSpan timeout)
    {
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      base.OnClose(timeout);
      this.CloseProtocolFactory(false, timeoutHelper.RemainingTime());
      if (this.sessionClientSettings == null)
        return;
      this.sessionClientSettings.Close(timeoutHelper.RemainingTime());
    }

    protected override TChannel OnCreateChannel(EndpointAddress address, Uri via)
    {
      this.ThrowIfDisposed();
      if (this.SessionMode)
      {
        return this.sessionClientSettings.OnCreateChannel(address, via);
      }
      if (typeof (TChannel) == typeof (IOutputChannel))
      {
        var ret = new SecurityChannelFactory<TChannel>.SecurityOutputChannel((ChannelManagerBase) this, this.securityProtocolFactory, ((IChannelFactory<IOutputChannel>) this.InnerChannelFactory).CreateChannel(address, via), address, via);
        return (TChannel) Convert.ChangeType(ret, typeof(TChannel));
      }
      if (typeof (TChannel) == typeof (IOutputSessionChannel))
      {
        
        var ret = new SecurityChannelFactory<TChannel>.SecurityOutputSessionChannel((ChannelManagerBase) this, this.securityProtocolFactory, ((IChannelFactory<IOutputSessionChannel>) this.InnerChannelFactory).CreateChannel(address, via), address, via);
        return (TChannel) Convert.ChangeType(ret, typeof(TChannel));
      }
      if (typeof (TChannel) == typeof (IDuplexChannel))
      {
        var ret = new SecurityChannelFactory<TChannel>.SecurityDuplexChannel((ChannelManagerBase) this, this.securityProtocolFactory, ((IChannelFactory<IDuplexChannel>) this.InnerChannelFactory).CreateChannel(address, via), address, via);
        return (TChannel) Convert.ChangeType(ret, typeof(TChannel));
      }
      if (typeof (TChannel) == typeof (IDuplexSessionChannel))
      {
        var ret = new SecurityChannelFactory<TChannel>.SecurityDuplexSessionChannel((ChannelManagerBase) this, this.securityProtocolFactory, ((IChannelFactory<IDuplexSessionChannel>) this.InnerChannelFactory).CreateChannel(address, via), address, via);
        return (TChannel) Convert.ChangeType(ret, typeof(TChannel));
      }
      if (typeof (TChannel) == typeof (IRequestChannel))
      {
        var ret = new SecurityChannelFactory<TChannel>.SecurityRequestChannel((ChannelManagerBase) this, this.securityProtocolFactory, ((IChannelFactory<IRequestChannel>) this.InnerChannelFactory).CreateChannel(address, via), address, via);
        return (TChannel)(object)ret;
      }
      var retFinal = new SecurityChannelFactory<TChannel>.SecurityRequestSessionChannel((ChannelManagerBase) this, this.securityProtocolFactory, ((IChannelFactory<IRequestSessionChannel>) this.InnerChannelFactory).CreateChannel(address, via), address, via);
      return (TChannel) Convert.ChangeType(retFinal, typeof(TChannel));
    }

    protected override void OnOpen(TimeSpan timeout)
    {
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      this.OnOpenCore(timeoutHelper.RemainingTime());
      base.OnOpen(timeoutHelper.RemainingTime());
      this.SetBufferManager();
    }

    private void SetBufferManager()
    {
      ITransportFactorySettings property = this.GetProperty<ITransportFactorySettings>();
      if (property == null)
        return;
      BufferManager bufferManager = property.BufferManager;
      if (bufferManager == null)
        return;
      if (this.SessionMode && this.SessionClientSettings != null && this.SessionClientSettings.SessionProtocolFactory != null)
      {
        this.SessionClientSettings.SessionProtocolFactory.StreamBufferManager = bufferManager;
      }
      else
      {
        this.ThrowIfProtocolFactoryNotSet();
        this.securityProtocolFactory.StreamBufferManager = bufferManager;
      }
    }

    protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("SecurityChannelFactory.OnBeginOpen not supported in .NET Core");
#else
      return (IAsyncResult) new OperationWithTimeoutAsyncResult(new OperationWithTimeoutCallback(base.OnOpen), timeout, callback, state);
#endif
    }

    protected override void OnEndOpen(IAsyncResult result)
    {
      OperationWithTimeoutAsyncResult.End(result);
    }

    private void OnOpenCore(TimeSpan timeout)
    {
      TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
      if (this.SessionMode)
      {
        this.SessionClientSettings.Open(this, this.InnerChannelFactory, this.ChannelBuilder, timeoutHelper.RemainingTime());
      }
      else
      {
        this.ThrowIfProtocolFactoryNotSet();
        this.securityProtocolFactory.Open(true, timeoutHelper.RemainingTime());
      }
    }

    private void ThrowIfDuplexNotSupported()
    {
      if (!this.SupportsDuplex)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SecurityProtocolFactoryDoesNotSupportDuplex", new object[1]{ (object) this.securityProtocolFactory })));
    }

    private void ThrowIfProtocolFactoryNotSet()
    {
      if (this.securityProtocolFactory == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SecurityProtocolFactoryShouldBeSetBeforeThisOperation")));
    }

    private void ThrowIfRequestReplyNotSupported()
    {
      if (!this.SupportsRequestReply)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SecurityProtocolFactoryDoesNotSupportRequestReply", new object[1]{ (object) this.securityProtocolFactory })));
    }

    private abstract class ClientSecurityChannel<UChannel> : SecurityChannel<UChannel> where UChannel : class, IChannel
    {
      private EndpointAddress to;
      private Uri via;
      private SecurityProtocolFactory securityProtocolFactory;
      private ChannelParameterCollection channelParameters;

      protected SecurityProtocolFactory SecurityProtocolFactory
      {
        get
        {
          return this.securityProtocolFactory;
        }
      }

      public EndpointAddress RemoteAddress
      {
        get
        {
          return this.to;
        }
      }

      public Uri Via
      {
        get
        {
          return this.via;
        }
      }

#region FromWCF
      protected internal override Task OnOpenAsync(TimeSpan timeout)
      {
        this.OnOpen(timeout);
        return TaskHelpers.CompletedTask();
      }
#endregion

      protected ClientSecurityChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory, UChannel innerChannel, EndpointAddress to, Uri via)
        : base(factory, innerChannel)
      {
        this.to = to;
        this.via = via;
        this.securityProtocolFactory = securityProtocolFactory;
        this.channelParameters = new ChannelParameterCollection((IChannel) this);
      }

      protected bool TryGetSecurityFaultException(Message faultMessage, out Exception faultException)
      {
        faultException = (Exception) null;
        if (!faultMessage.IsFault)
          return false;
        MessageFault fault = MessageFault.CreateFault(faultMessage, 16384);
        faultException = System.ServiceModel.Security.SecurityUtils.CreateSecurityFaultException(fault);
        return true;
      }

      protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
      {
        this.EnableChannelBindingSupport();
        return (IAsyncResult) new SecurityChannelFactory<TChannel>.ClientSecurityChannel<UChannel>.OpenAsyncResult(this, timeout, callback, state);
      }

      protected override void OnEndOpen(IAsyncResult result)
      {
        SecurityChannelFactory<TChannel>.ClientSecurityChannel<UChannel>.OpenAsyncResult.End(result);
      }

      protected override void OnOpen(TimeSpan timeout)
      {
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        this.EnableChannelBindingSupport();
        this.OnProtocolCreationComplete(this.SecurityProtocolFactory.CreateSecurityProtocol(this.to, this.Via, (object) null, typeof (TChannel) == typeof (IRequestChannel), timeoutHelper.RemainingTime()));
        this.SecurityProtocol.Open(timeoutHelper.RemainingTime());
        base.OnOpen(timeoutHelper.RemainingTime());
      }

      private void EnableChannelBindingSupport()
      {
        if (this.securityProtocolFactory != null && this.securityProtocolFactory.ExtendedProtectionPolicy != null && this.securityProtocolFactory.ExtendedProtectionPolicy.CustomChannelBinding != null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("ExtendedProtectionPolicyCustomChannelBindingNotSupported")));
        if (System.ServiceModel.Security.SecurityUtils.IsChannelBindingDisabled || !System.ServiceModel.Security.SecurityUtils.IsSecurityBindingSuitableForChannelBinding(this.SecurityProtocolFactory.SecurityBindingElement as TransportSecurityBindingElement) || (object) this.InnerChannel == null)
          return;
        IChannelBindingProvider property = this.InnerChannel.GetProperty<IChannelBindingProvider>();
        if (property == null)
          return;
        property.EnableChannelBindingSupport();
      }

      private void OnProtocolCreationComplete(SecurityProtocol securityProtocol)
      {
        this.SecurityProtocol = securityProtocol;
        this.SecurityProtocol.ChannelParameters = this.channelParameters;
      }

      public override T GetProperty<T>()
      {
        if (typeof (T) == typeof (ChannelParameterCollection))
          return (T) Convert.ChangeType(this.channelParameters, typeof(T));
        return base.GetProperty<T>();
      }

      private sealed class OpenAsyncResult : AsyncResult
      {
        private static readonly AsyncCallback openInnerChannelCallback = Fx.ThunkCallback(new AsyncCallback(SecurityChannelFactory<TChannel>.ClientSecurityChannel<UChannel>.OpenAsyncResult.OpenInnerChannelCallback));
        private static readonly AsyncCallback openSecurityProtocolCallback = Fx.ThunkCallback(new AsyncCallback(SecurityChannelFactory<TChannel>.ClientSecurityChannel<UChannel>.OpenAsyncResult.OpenSecurityProtocolCallback));
        private readonly SecurityChannelFactory<TChannel>.ClientSecurityChannel<UChannel> clientChannel;
        private TimeoutHelper timeoutHelper;

        public OpenAsyncResult(SecurityChannelFactory<TChannel>.ClientSecurityChannel<UChannel> clientChannel, TimeSpan timeout, AsyncCallback callback, object state)
          : base(callback, state)
        {
          this.timeoutHelper = new TimeoutHelper(timeout);
          this.clientChannel = clientChannel;
          if (!this.OnCreateSecurityProtocolComplete(this.clientChannel.SecurityProtocolFactory.CreateSecurityProtocol(this.clientChannel.to, this.clientChannel.Via, (object) null, typeof (TChannel) == typeof (IRequestChannel), this.timeoutHelper.RemainingTime())))
            return;
          this.Complete(true);
        }

        internal static void End(IAsyncResult result)
        {
          AsyncResult.End<SecurityChannelFactory<TChannel>.ClientSecurityChannel<UChannel>.OpenAsyncResult>(result);
        }

        private bool OnCreateSecurityProtocolComplete(SecurityProtocol securityProtocol)
        {
#if FEATURE_CORECLR
          throw new NotImplementedException("SecurityProtocol.EndOpen is not supported in .NET Core");
#else
          this.clientChannel.OnProtocolCreationComplete(securityProtocol);
          IAsyncResult result = securityProtocol.BeginOpen(this.timeoutHelper.RemainingTime(), SecurityChannelFactory<TChannel>.ClientSecurityChannel<UChannel>.OpenAsyncResult.openSecurityProtocolCallback, (object) this);
          if (!result.CompletedSynchronously)
            return false;
          securityProtocol.EndOpen(result);
          return this.OnSecurityProtocolOpenComplete();
#endif
        }

        private static void OpenSecurityProtocolCallback(IAsyncResult result)
        {
          if (result.CompletedSynchronously)
            return;
          SecurityChannelFactory<TChannel>.ClientSecurityChannel<UChannel>.OpenAsyncResult asyncState = result.AsyncState as SecurityChannelFactory<TChannel>.ClientSecurityChannel<UChannel>.OpenAsyncResult;
          if (asyncState == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR.GetString("InvalidAsyncResult"), "result"));
#if FEATURE_CORECLR
          throw new NotImplementedException("SecurityProtocol.EndOpen is not supported in .NET Core");
#else
          Exception exception = (Exception) null;
          bool flag;
          try
          {
            asyncState.clientChannel.SecurityProtocol.EndOpen(result);
            flag = asyncState.OnSecurityProtocolOpenComplete();
          }
          catch (Exception ex)
          {
            if (Fx.IsFatal(ex))
            {
              throw;
            }
            else
            {
              exception = ex;
              flag = true;
            }
          }
          if (!flag)
            return;
          asyncState.Complete(false, exception);
#endif
        }

        private bool OnSecurityProtocolOpenComplete()
        {
          IAsyncResult result = this.clientChannel.InnerChannel.BeginOpen(this.timeoutHelper.RemainingTime(), SecurityChannelFactory<TChannel>.ClientSecurityChannel<UChannel>.OpenAsyncResult.openInnerChannelCallback, (object) this);
          if (!result.CompletedSynchronously)
            return false;
          this.clientChannel.InnerChannel.EndOpen(result);
          return true;
        }

        private static void OpenInnerChannelCallback(IAsyncResult result)
        {
          if (result == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("result"));
          if (result.CompletedSynchronously)
            return;
          SecurityChannelFactory<TChannel>.ClientSecurityChannel<UChannel>.OpenAsyncResult asyncState = result.AsyncState as SecurityChannelFactory<TChannel>.ClientSecurityChannel<UChannel>.OpenAsyncResult;
          if (asyncState == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR.GetString("InvalidAsyncResult"), "result"));
          Exception exception = (Exception) null;
          try
          {
            asyncState.clientChannel.InnerChannel.EndOpen(result);
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

    private class SecurityOutputChannel : SecurityChannelFactory<TChannel>.ClientSecurityChannel<IOutputChannel>, IOutputChannel, IChannel, ICommunicationObject
    {
      public SecurityOutputChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory, IOutputChannel innerChannel, EndpointAddress to, Uri via)
        : base(factory, securityProtocolFactory, innerChannel, to, via)
      {
      }

      public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
      {
        return this.BeginSend(message, this.DefaultSendTimeout, callback, state);
      }

      public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
      {
        this.ThrowIfFaulted();
        this.ThrowIfDisposedOrNotOpen(message);
#if FEATURE_CORECLR
        throw new NotImplementedException("OutputChannelSendAsyncResult not supported in .NET Core");
#else
        return (IAsyncResult) new SecurityChannel<IOutputChannel>.OutputChannelSendAsyncResult(message, this.SecurityProtocol, this.InnerChannel, timeout, callback, state);
#endif
      }

      public void EndSend(IAsyncResult result)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("OutputChannelSendAsyncResult not supported in .NET Core");
#else
        SecurityChannel<IOutputChannel>.OutputChannelSendAsyncResult.End(result);
#endif
      }

      public void Send(Message message)
      {
        this.Send(message, this.DefaultSendTimeout);
      }

      public void Send(Message message, TimeSpan timeout)
      {
        this.ThrowIfFaulted();
        this.ThrowIfDisposedOrNotOpen(message);
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        this.SecurityProtocol.SecureOutgoingMessage(ref message, timeoutHelper.RemainingTime());
        this.InnerChannel.Send(message, timeoutHelper.RemainingTime());
      }
    }

    private sealed class SecurityOutputSessionChannel : SecurityChannelFactory<TChannel>.SecurityOutputChannel, IOutputSessionChannel, IOutputChannel, IChannel, ICommunicationObject, ISessionChannel<IOutputSession>
    {
      public IOutputSession Session
      {
        get
        {
          return ((ISessionChannel<IOutputSession>) this.InnerChannel).Session;
        }
      }

      public SecurityOutputSessionChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory, IOutputSessionChannel innerChannel, EndpointAddress to, Uri via)
        : base(factory, securityProtocolFactory, (IOutputChannel) innerChannel, to, via)
      {
      }
    }

    private class SecurityRequestChannel : SecurityChannelFactory<TChannel>.ClientSecurityChannel<IRequestChannel>, IRequestChannel, IChannel, ICommunicationObject
    {
      public SecurityRequestChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory, IRequestChannel innerChannel, EndpointAddress to, Uri via)
        : base(factory, securityProtocolFactory, innerChannel, to, via)
      {
      }

      public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state)
      {
        return this.BeginRequest(message, this.DefaultSendTimeout, callback, state);
      }

      public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
      {
        this.ThrowIfFaulted();
        this.ThrowIfDisposedOrNotOpen(message);
        return (IAsyncResult) new SecurityChannelFactory<TChannel>.RequestChannelSendAsyncResult(message, this.SecurityProtocol, this.InnerChannel, this, timeout, callback, state);
      }

      public Message EndRequest(IAsyncResult result)
      {
        return SecurityChannelFactory<TChannel>.RequestChannelSendAsyncResult.End(result);
      }

      public Message Request(Message message)
      {
        return this.Request(message, this.DefaultSendTimeout);
      }

      internal Message ProcessReply(Message reply, SecurityProtocolCorrelationState correlationState, TimeSpan timeout)
      {
        if (reply != null)
        {
          if (DiagnosticUtility.ShouldUseActivity)
          {
            ServiceModelActivity activity = TraceUtility.ExtractActivity(reply);
            if (activity != null && correlationState != null && (correlationState.Activity != null && activity.Id != correlationState.Activity.Id))
            {
              using (ServiceModelActivity.BoundOperation(activity))
              {
                if (FxTrace.Trace != null)
                  FxTrace.Trace.TraceTransfer(correlationState.Activity.Id);
                activity.Stop();
              }
            }
          }
          ServiceModelActivity activity1 = correlationState == null ? (ServiceModelActivity) null : correlationState.Activity;
          using (ServiceModelActivity.BoundOperation(activity1))
          {
            if (DiagnosticUtility.ShouldUseActivity)
              TraceUtility.SetActivity(reply, activity1);
            Message faultMessage = reply;
            Exception faultException = (Exception) null;
            try
            {
              this.SecurityProtocol.VerifyIncomingMessage(ref reply, timeout, correlationState);
            }
            catch (MessageSecurityException)
            {
              this.TryGetSecurityFaultException(faultMessage, out faultException);
              if (faultException == null)
                throw;
            }
            if (faultException != null)
            {
              this.Fault(faultException);
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(faultException);
            }
          }
        }
        return reply;
      }

      public Message Request(Message message, TimeSpan timeout)
      {
        this.ThrowIfFaulted();
        this.ThrowIfDisposedOrNotOpen(message);
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        SecurityProtocolCorrelationState correlationState = this.SecurityProtocol.SecureOutgoingMessage(ref message, timeoutHelper.RemainingTime(), (SecurityProtocolCorrelationState) null);
        return this.ProcessReply(this.InnerChannel.Request(message, timeoutHelper.RemainingTime()), correlationState, timeoutHelper.RemainingTime());
      }
    }

    private sealed class SecurityRequestSessionChannel : SecurityChannelFactory<TChannel>.SecurityRequestChannel, IRequestSessionChannel, IRequestChannel, IChannel, ICommunicationObject, ISessionChannel<IOutputSession>
    {
      public IOutputSession Session
      {
        get
        {
          return ((ISessionChannel<IOutputSession>) this.InnerChannel).Session;
        }
      }

      public SecurityRequestSessionChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory, IRequestSessionChannel innerChannel, EndpointAddress to, Uri via)
        : base(factory, securityProtocolFactory, (IRequestChannel) innerChannel, to, via)
      {
      }
    }

    private class SecurityDuplexChannel : SecurityChannelFactory<TChannel>.SecurityOutputChannel, IDuplexChannel, IInputChannel, IChannel, ICommunicationObject, IOutputChannel
    {
      internal IDuplexChannel InnerDuplexChannel
      {
        get
        {
          return (IDuplexChannel) this.InnerChannel;
        }
      }

      public EndpointAddress LocalAddress
      {
        get
        {
          return this.InnerDuplexChannel.LocalAddress;
        }
      }

      internal virtual bool AcceptUnsecuredFaults
      {
        get
        {
          return false;
        }
      }

      public SecurityDuplexChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory, IDuplexChannel innerChannel, EndpointAddress to, Uri via)
        : base(factory, securityProtocolFactory, (IOutputChannel) innerChannel, to, via)
      {
      }

      public Message Receive()
      {
        return this.Receive(this.DefaultReceiveTimeout);
      }

      public Message Receive(TimeSpan timeout)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("InputChannel not supported in .NET Core");
#else
        return InputChannel.HelpReceive((IInputChannel) this, timeout);
#endif
      }

      public IAsyncResult BeginReceive(AsyncCallback callback, object state)
      {
        return this.BeginReceive(this.DefaultReceiveTimeout, callback, state);
      }

      public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("InputChannel not supported in .NET Core");
#else
        return InputChannel.HelpBeginReceive((IInputChannel) this, timeout, callback, state);
#endif
      }

      public Message EndReceive(IAsyncResult result)
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("InputChannel not supported in .NET Core");
#else
        return InputChannel.HelpEndReceive(result);
#endif
      }

      public virtual IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
      {
        if (this.DoneReceivingInCurrentState())
          return (IAsyncResult) new DoneReceivingAsyncResult(callback, state);
        SecurityChannelFactory<TChannel>.ClientDuplexReceiveMessageAndVerifySecurityAsyncResult securityAsyncResult = new SecurityChannelFactory<TChannel>.ClientDuplexReceiveMessageAndVerifySecurityAsyncResult(this, this.InnerDuplexChannel, timeout, callback, state);
        securityAsyncResult.Start();
        return (IAsyncResult) securityAsyncResult;
      }

      public virtual bool EndTryReceive(IAsyncResult result, out Message message)
      {
        DoneReceivingAsyncResult result1 = result as DoneReceivingAsyncResult;
        if (result1 != null)
          return DoneReceivingAsyncResult.End(result1, out message);
        return ReceiveMessageAndVerifySecurityAsyncResultBase.End(result, out message);
      }

      internal Message ProcessMessage(Message message, TimeSpan timeout)
      {
        if (message == null)
          return (Message) null;
        Message faultMessage = message;
        Exception faultException = (Exception) null;
        try
        {
          this.SecurityProtocol.VerifyIncomingMessage(ref message, timeout);
        }
        catch (MessageSecurityException)
        {
          this.TryGetSecurityFaultException(faultMessage, out faultException);
          if (faultException == null)
            throw;
        }
        if (faultException != null)
        {
          if (this.AcceptUnsecuredFaults)
            this.Fault(faultException);
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(faultException);
        }
        return message;
      }

      public bool TryReceive(TimeSpan timeout, out Message message)
      {
        if (this.DoneReceivingInCurrentState())
        {
          message = (Message) null;
          return true;
        }
        TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        if (!this.InnerDuplexChannel.TryReceive(timeoutHelper.RemainingTime(), out message))
          return false;
        message = this.ProcessMessage(message, timeoutHelper.RemainingTime());
        return true;
      }

      public bool WaitForMessage(TimeSpan timeout)
      {
        return this.InnerDuplexChannel.WaitForMessage(timeout);
      }

      public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
      {
        return this.InnerDuplexChannel.BeginWaitForMessage(timeout, callback, state);
      }

      public bool EndWaitForMessage(IAsyncResult result)
      {
        return this.InnerDuplexChannel.EndWaitForMessage(result);
      }
    }

    private sealed class SecurityDuplexSessionChannel : SecurityChannelFactory<TChannel>.SecurityDuplexChannel, IDuplexSessionChannel, IDuplexChannel, IInputChannel, IChannel, ICommunicationObject, IOutputChannel, ISessionChannel<IDuplexSession>
    {
      public IDuplexSession Session
      {
        get
        {
          return ((ISessionChannel<IDuplexSession>) this.InnerChannel).Session;
        }
      }

      internal override bool AcceptUnsecuredFaults
      {
        get
        {
          return true;
        }
      }

      public SecurityDuplexSessionChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory, IDuplexSessionChannel innerChannel, EndpointAddress to, Uri via)
        : base(factory, securityProtocolFactory, (IDuplexChannel) innerChannel, to, via)
      {
      }
    }

    private sealed class RequestChannelSendAsyncResult : ApplySecurityAndSendAsyncResult<IRequestChannel>
    {
      private Message reply;
      private SecurityChannelFactory<TChannel>.SecurityRequestChannel securityChannel;

      public RequestChannelSendAsyncResult(Message message, SecurityProtocol protocol, IRequestChannel channel, SecurityChannelFactory<TChannel>.SecurityRequestChannel securityChannel, TimeSpan timeout, AsyncCallback callback, object state)
        : base(protocol, channel, timeout, callback, state)
      {
        this.securityChannel = securityChannel;
        this.Begin(message, (SecurityProtocolCorrelationState) null);
      }

      protected override IAsyncResult BeginSendCore(IRequestChannel channel, Message message, TimeSpan timeout, AsyncCallback callback, object state)
      {
        return channel.BeginRequest(message, timeout, callback, state);
      }

      internal static Message End(IAsyncResult result)
      {
        SecurityChannelFactory<TChannel>.RequestChannelSendAsyncResult channelSendAsyncResult = result as SecurityChannelFactory<TChannel>.RequestChannelSendAsyncResult;
        ApplySecurityAndSendAsyncResult<IRequestChannel>.OnEnd((ApplySecurityAndSendAsyncResult<IRequestChannel>) channelSendAsyncResult);
        return channelSendAsyncResult.reply;
      }

      protected override void EndSendCore(IRequestChannel channel, IAsyncResult result)
      {
        this.reply = channel.EndRequest(result);
      }

      protected override void OnSendCompleteCore(TimeSpan timeout)
      {
        this.reply = this.securityChannel.ProcessReply(this.reply, this.CorrelationState, timeout);
      }
    }

    private class ClientDuplexReceiveMessageAndVerifySecurityAsyncResult : ReceiveMessageAndVerifySecurityAsyncResultBase
    {
      private SecurityChannelFactory<TChannel>.SecurityDuplexChannel channel;

      public ClientDuplexReceiveMessageAndVerifySecurityAsyncResult(SecurityChannelFactory<TChannel>.SecurityDuplexChannel channel, IDuplexChannel innerChannel, TimeSpan timeout, AsyncCallback callback, object state)
        : base((IInputChannel) innerChannel, timeout, callback, state)
      {
        this.channel = channel;
      }

      protected override bool OnInnerReceiveDone(ref Message message, TimeSpan timeout)
      {
        message = this.channel.ProcessMessage(message, timeout);
        return true;
      }
    }
  }
}
