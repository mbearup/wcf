// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.SafeFreeCredentials
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace System.IdentityModel
{
  internal class SafeFreeCredentials : SafeHandle
  {
    private const string SECURITY = "security.Dll";
    internal SSPIHandle _handle;

    public override bool IsInvalid
    {
      get
      {
        if (!this.IsClosed)
          return this._handle.IsZero;
        return true;
      }
    }

    protected SafeFreeCredentials()
      : base(IntPtr.Zero, true)
    {
      this._handle = new SSPIHandle();
    }

    public static unsafe int AcquireCredentialsHandle(string package, CredentialUse intent, ref AuthIdentityEx authdata, out SafeFreeCredentials outCredential)
    {
#if FEATURE_CORECLR
      throw new NotImplementedException("AcquireCredentialsHandle is not supported on .NET Core. because SspiCli does not exist.");
#else
      int num = -1;
      outCredential = new SafeFreeCredentials();
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
      }
      finally
      {
        long timeStamp;
        num = SafeFreeCredentials.AcquireCredentialsHandleW((string) null, package, (int) intent, (void*) null, ref authdata, (void*) null, (void*) null, ref outCredential._handle, out timeStamp);
        if (num != 0)
          outCredential.SetHandleAsInvalid();
      }
      return num;
#endif
    }

    public static unsafe int AcquireDefaultCredential(string package, CredentialUse intent, ref AuthIdentityEx authIdentity, out SafeFreeCredentials outCredential)
    {
      int num = -1;
      outCredential = new SafeFreeCredentials();
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
      }
      finally
      {
        long timeStamp;
        num = SafeFreeCredentials.AcquireCredentialsHandleW((string) null, package, (int) intent, (void*) null, ref authIdentity, (void*) null, (void*) null, ref outCredential._handle, out timeStamp);
        if (num != 0)
          outCredential.SetHandleAsInvalid();
      }
      return num;
    }

    public static unsafe int AcquireCredentialsHandle(string package, CredentialUse intent, ref SecureCredential authdata, out SafeFreeCredentials outCredential)
    {
      int num1 = -1;
      IntPtr certContextArray = authdata.certContextArray;
      try
      {
        IntPtr num2 = new IntPtr((void*) &certContextArray);
        if (certContextArray != IntPtr.Zero)
          authdata.certContextArray = num2;
        outCredential = new SafeFreeCredentials();
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
        {
          long timeStamp;
          num1 = SafeFreeCredentials.AcquireCredentialsHandleW((string) null, package, (int) intent, (void*) null, ref authdata, (void*) null, (void*) null, ref outCredential._handle, out timeStamp);
          if (num1 != 0)
            outCredential.SetHandleAsInvalid();
        }
      }
      finally
      {
        authdata.certContextArray = certContextArray;
      }
      return num1;
    }

    public static unsafe int AcquireCredentialsHandle(string package, CredentialUse intent, ref IntPtr ppAuthIdentity, out SafeFreeCredentials outCredential)
    {
      int num = -1;
      outCredential = new SafeFreeCredentials();
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
      }
      finally
      {
        long timeStamp;
        num = SafeFreeCredentials.AcquireCredentialsHandleW((string) null, package, (int) intent, (void*) null, ppAuthIdentity, (void*) null, (void*) null, ref outCredential._handle, out timeStamp);
        if (num != 0)
          outCredential.SetHandleAsInvalid();
      }
      return num;
    }

    protected override bool ReleaseHandle()
    {
      return SafeFreeCredentials.FreeCredentialsHandle(ref this._handle) == 0;
    }

    [DllImport("SspiCli.Dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern unsafe int AcquireCredentialsHandleW([In] string principal, [In] string moduleName, [In] int usage, [In] void* logonID, [In] ref AuthIdentityEx authdata, [In] void* keyCallback, [In] void* keyArgument, ref SSPIHandle handlePtr, out long timeStamp);

    [DllImport("SspiCli.Dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern unsafe int AcquireCredentialsHandleW([In] string principal, [In] string moduleName, [In] int usage, [In] void* logonID, [In] IntPtr zero, [In] void* keyCallback, [In] void* keyArgument, ref SSPIHandle handlePtr, out long timeStamp);

    [DllImport("SspiCli.Dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern unsafe int AcquireCredentialsHandleW([In] string principal, [In] string moduleName, [In] int usage, [In] void* logonID, [In] ref SecureCredential authData, [In] void* keyCallback, [In] void* keyArgument, ref SSPIHandle handlePtr, out long timeStamp);

    [SuppressUnmanagedCodeSecurity]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [DllImport("SspiCli.Dll", SetLastError = true)]
    internal static extern int FreeCredentialsHandle(ref SSPIHandle handlePtr);
  }
}
