// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.ChannelDemuxerBindingElement
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections.ObjectModel;

namespace System.ServiceModel.Channels
{
  internal class ChannelDemuxerBindingElement : BindingElement
  {
    private ChannelDemuxer demuxer;
    private ChannelDemuxerBindingElement.CachedBindingContextState cachedContextState;
    private bool cacheContextState;

    public TimeSpan PeekTimeout
    {
      get
      {
        return this.demuxer.PeekTimeout;
      }
      set
      {
        if (value < TimeSpan.Zero && value != ChannelDemuxer.UseDefaultReceiveTimeout)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.demuxer.PeekTimeout = value;
      }
    }

    public int MaxPendingSessions
    {
      get
      {
        return this.demuxer.MaxPendingSessions;
      }
      set
      {
        if (value < 1)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException(SR.GetString("ValueMustBeGreaterThanZero")));
        this.demuxer.MaxPendingSessions = value;
      }
    }

    public ChannelDemuxerBindingElement(bool cacheContextState)
    {
      this.cacheContextState = cacheContextState;
      if (cacheContextState)
        this.cachedContextState = new ChannelDemuxerBindingElement.CachedBindingContextState();
      this.demuxer = new ChannelDemuxer();
    }

    public ChannelDemuxerBindingElement(ChannelDemuxerBindingElement element)
    {
      this.demuxer = element.demuxer;
      this.cacheContextState = element.cacheContextState;
      this.cachedContextState = element.cachedContextState;
    }

    private void SubstituteCachedBindingContextParametersIfNeeded(BindingContext context)
    {
      if (!this.cacheContextState)
        return;
      if (!this.cachedContextState.IsStateCached)
      {
        foreach (object bindingParameter in (Collection<object>) context.BindingParameters)
          this.cachedContextState.CachedBindingParameters.Add(bindingParameter);
        this.cachedContextState.IsStateCached = true;
      }
      else
      {
        context.BindingParameters.Clear();
        foreach (object bindingParameter in (Collection<object>) this.cachedContextState.CachedBindingParameters)
          context.BindingParameters.Add(bindingParameter);
      }
    }

    public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
    {
      if (context == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
      this.SubstituteCachedBindingContextParametersIfNeeded(context);
      return context.BuildInnerChannelFactory<TChannel>();
    }

    public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
    {
      if (context == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
      ChannelDemuxerFilter filter = context.BindingParameters.Remove<ChannelDemuxerFilter>();
      this.SubstituteCachedBindingContextParametersIfNeeded(context);
      if (filter == null)
        return this.demuxer.BuildChannelListener<TChannel>(context);
      return this.demuxer.BuildChannelListener<TChannel>(context, filter);
    }

    public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
    {
      if (context == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
      return context.CanBuildInnerChannelFactory<TChannel>();
    }

    public override bool CanBuildChannelListener<TChannel>(BindingContext context)
    {
      if (context == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
      return context.CanBuildInnerChannelListener<TChannel>();
    }

    public override BindingElement Clone()
    {
      return (BindingElement) new ChannelDemuxerBindingElement(this);
    }

    public override T GetProperty<T>(BindingContext context)
    {
      if (this.cacheContextState && this.cachedContextState.IsStateCached)
      {
        for (int index = 0; index < this.cachedContextState.CachedBindingParameters.Count; ++index)
        {
          if (!context.BindingParameters.Contains(this.cachedContextState.CachedBindingParameters[index].GetType()))
            context.BindingParameters.Add(this.cachedContextState.CachedBindingParameters[index]);
        }
      }
      return context.GetInnerProperty<T>();
    }

    private class CachedBindingContextState
    {
      public bool IsStateCached;
      public BindingParameterCollection CachedBindingParameters;

      public CachedBindingContextState()
      {
        this.CachedBindingParameters = new BindingParameterCollection();
      }
    }
  }
}
