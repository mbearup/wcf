namespace System.Runtime.Diagnostics
{
  public sealed class EtwProvider
  {
      
    public EtwProvider(Guid guid)
    {
        
    }
    
    private Action invokeControllerCallback;
    private bool end2EndActivityTracingEnabled;
    public bool IsEnabled()
    {
        return false;
    }
    
    public bool IsEnabled(byte level, long keywords)
    {
      return false;
    }

    internal Action ControllerCallBack
    {
      get
      {
        return this.invokeControllerCallback;
      }
      set
      {
        this.invokeControllerCallback = value;
      }
    }
    
    internal bool IsEnd2EndActivityTracingEnabled
    {
      get
      {
        return this.end2EndActivityTracingEnabled;
      }
    }
    
    internal void SetEnd2EndActivityTracingEnabled(bool isEnd2EndActivityTracingEnabled)
    {
       this.end2EndActivityTracingEnabled = isEnd2EndActivityTracingEnabled;
    }
    
    public unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, Guid value1, string value2, string value3)
    {
        return true;
    }
    
    public unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, params string[] values)
    {
        return true;
    }
    
    public unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, params object[] values)
    {
        return true;
    }
    
    public unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, params int[] values)
    {
        return true;
    }
    
    public unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, params long[] values)
    {
        return true;
    }
    
    public unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, Guid value1, long value2, long value3, params string[] values)
    {
        return true;
    }
    
    public unsafe bool WriteEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, string value1, long value2, string value3, string value4)
    {
        return true;
    }
    
    public void Dispose()
    {
        
    }
    
    public unsafe bool WriteTransferEvent(ref EventDescriptor eventDescriptor, EventTraceActivity eventTraceActivity, Guid relatedActivityId, int dataCount, IntPtr data)
    {
      return true;
    }
    
    /*public bool IsEventEnabled(ref EventDescriptor eventDescriptor)
    {
      return false;
    }*/
  }
}
