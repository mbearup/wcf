// Decompiled with JetBrains decompiler
// Type: System.Runtime.IOThreadTimer
// Assembly: System.ServiceModel.Internals, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0E931D74-5959-472E-BB1A-57A144CFBE8B
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.ServiceModel.Internals\v4.0_4.0.0.0__31bf3856ad364e35\System.ServiceModel.Internals.dll

using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Security;
using System.Threading;

namespace System.Runtime
{
  public class IOThreadTimer
  {
    public IOThreadTimer(Action<object> callback, object callbackState, bool isTypicallyCanceledShortlyAfterBeingSet)
    {
    }

    public bool Cancel()
    {
        throw new NotImplementedException("IOThreadTimer not implemented in .NET Core");
    }

    public void Set(TimeSpan timeFromNow)
    {
        throw new NotImplementedException("IOThreadTimer not implemented in .NET Core");
    }

    public void SetAt(long time)
    {
        throw new NotImplementedException("IOThreadTimer not implemented in .NET Core");
    }
  }
}
