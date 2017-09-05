// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.SSPIHandle
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System.IdentityModel
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  internal struct SSPIHandle
  {
    private IntPtr HandleHi;
    private IntPtr HandleLo;

    public bool IsZero
    {
      get
      {
        if (this.HandleHi == IntPtr.Zero)
          return this.HandleLo == IntPtr.Zero;
        return false;
      }
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal void SetToInvalid()
    {
      this.HandleHi = IntPtr.Zero;
      this.HandleLo = IntPtr.Zero;
    }
  }
}
