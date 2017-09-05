// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.IReliableChannelBinder
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

namespace System.ServiceModel.Channels
{
  public interface IReliableChannelBinder
  {
    bool CanSendAsynchronously { get; }

    IChannel Channel { get; }

    bool Connected { get; }

    TimeSpan DefaultSendTimeout { get; }

    bool HasSession { get; }

    EndpointAddress LocalAddress { get; }

    EndpointAddress RemoteAddress { get; }

    CommunicationState State { get; }

    event BinderExceptionHandler Faulted;

    event BinderExceptionHandler OnException;

    void Abort();

    void Close(TimeSpan timeout);

    void Close(TimeSpan timeout, MaskingMode maskingMode);

    IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state);

    IAsyncResult BeginClose(TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state);

    void EndClose(IAsyncResult result);

    void Open(TimeSpan timeout);

    IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state);

    void EndOpen(IAsyncResult result);

    IAsyncResult BeginSend(Message message, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state);

    IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state);

    void EndSend(IAsyncResult result);

    void Send(Message message, TimeSpan timeout);

    void Send(Message message, TimeSpan timeout, MaskingMode maskingMode);

    bool TryReceive(TimeSpan timeout, out RequestContext requestContext);

    bool TryReceive(TimeSpan timeout, out RequestContext requestContext, MaskingMode maskingMode);

    IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state);

    IAsyncResult BeginTryReceive(TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state);

    bool EndTryReceive(IAsyncResult result, out RequestContext requestContext);

    ISession GetInnerSession();

    void HandleException(Exception e);

    bool IsHandleable(Exception e);

    void SetMaskingMode(RequestContext context, MaskingMode maskingMode);

    RequestContext WrapRequestContext(RequestContext context);
  }
}
