// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.ChannelDemuxer
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.Generic;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Channels
{
  internal class ChannelDemuxer
  {
    public static readonly TimeSpan UseDefaultReceiveTimeout = TimeSpan.MinValue;
    private TypedChannelDemuxer inputDemuxer;
    private TypedChannelDemuxer replyDemuxer;
    private Dictionary<System.Type, TypedChannelDemuxer> typeDemuxers;
    private TimeSpan peekTimeout;
    private int maxPendingSessions;

    public TimeSpan PeekTimeout
    {
      get
      {
        return this.peekTimeout;
      }
      set
      {
        this.peekTimeout = value;
      }
    }

    public int MaxPendingSessions
    {
      get
      {
        return this.maxPendingSessions;
      }
      set
      {
        this.maxPendingSessions = value;
      }
    }

    public ChannelDemuxer()
    {
      this.peekTimeout = ChannelDemuxer.UseDefaultReceiveTimeout;
      this.maxPendingSessions = 10;
      this.typeDemuxers = new Dictionary<System.Type, TypedChannelDemuxer>();
    }

    public IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context) where TChannel : class, IChannel
    {
#if FEATURE_CORECLR
          throw new NotImplementedException("MatchAllMessageFilter is not supported in .NET Core");
#else
      return this.BuildChannelListener<TChannel>(context, new ChannelDemuxerFilter((MessageFilter) new MatchAllMessageFilter(), 0));
#endif
    }

    public IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context, ChannelDemuxerFilter filter) where TChannel : class, IChannel
    {
      return this.GetTypedDemuxer(typeof (TChannel), context).BuildChannelListener<TChannel>(filter);
    }

    private TypedChannelDemuxer CreateTypedDemuxer(System.Type channelType, BindingContext context)
    {
#if !FEATURE_CORECLR
// Demuxer types not supported in .NET Core
      if (channelType == typeof (IDuplexChannel))
        return (TypedChannelDemuxer) new DuplexChannelDemuxer(context);
      if (channelType == typeof (IInputSessionChannel))
        return (TypedChannelDemuxer) new InputSessionChannelDemuxer(context, this.peekTimeout, this.maxPendingSessions);
      if (channelType == typeof (IReplySessionChannel))
        return (TypedChannelDemuxer) new ReplySessionChannelDemuxer(context, this.peekTimeout, this.maxPendingSessions);
      if (channelType == typeof (IDuplexSessionChannel))
        return (TypedChannelDemuxer) new DuplexSessionChannelDemuxer(context, this.peekTimeout, this.maxPendingSessions);
#endif
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
      
    }

    private TypedChannelDemuxer GetTypedDemuxer(System.Type channelType, BindingContext context)
    {
      TypedChannelDemuxer typedChannelDemuxer = (TypedChannelDemuxer) null;
      bool flag = false;
      if (channelType == typeof (IInputChannel))
      {
        if (this.inputDemuxer == null)
        {
#if FEATURE_CORECLR
          throw new NotImplementedException("ReplyChannelDemuxer is not supported in .NET Core");
#else
          this.inputDemuxer = !context.CanBuildInnerChannelListener<IReplyChannel>() ? (TypedChannelDemuxer) new InputChannelDemuxer(context) : (this.replyDemuxer = (TypedChannelDemuxer) new ReplyChannelDemuxer(context));
          flag = true;
#endif
        }
        typedChannelDemuxer = this.inputDemuxer;
      }
      else if (channelType == typeof (IReplyChannel))
      {
        if (this.replyDemuxer == null)
        {
#if FEATURE_CORECLR
          throw new NotImplementedException("ReplyChannelDemuxer is not supported in .NET Core");
#else
          this.inputDemuxer = this.replyDemuxer = (TypedChannelDemuxer) new ReplyChannelDemuxer(context);
          flag = true;
#endif
        }
        typedChannelDemuxer = this.replyDemuxer;
      }
      else if (!this.typeDemuxers.TryGetValue(channelType, out typedChannelDemuxer))
      {
        typedChannelDemuxer = this.CreateTypedDemuxer(channelType, context);
        this.typeDemuxers.Add(channelType, typedChannelDemuxer);
        flag = true;
      }
      if (!flag)
        context.RemainingBindingElements.Clear();
      return typedChannelDemuxer;
    }
  }
}
