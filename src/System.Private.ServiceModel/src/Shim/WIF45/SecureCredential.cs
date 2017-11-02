// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.SecureCredential
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Security.Cryptography.X509Certificates;

namespace System.IdentityModel
{
  public struct SecureCredential
  {
    public const int CurrentVersion = 4;
    public int version;
    public int cCreds;
    public IntPtr certContextArray;
    private IntPtr rootStore;
    public int cMappers;
    private IntPtr phMappers;
    public int cSupportedAlgs;
    private IntPtr palgSupportedAlgs;
    public SchProtocols grbitEnabledProtocols;
    public int dwMinimumCipherStrength;
    public int dwMaximumCipherStrength;
    public int dwSessionLifespan;
    public SecureCredential.Flags dwFlags;
    public int reserved;

    public SecureCredential(int version, X509Certificate2 certificate, SecureCredential.Flags flags, SchProtocols protocols)
    {
      this.rootStore = this.phMappers = this.palgSupportedAlgs = this.certContextArray = IntPtr.Zero;
      this.cCreds = this.cMappers = this.cSupportedAlgs = 0;
      this.dwMinimumCipherStrength = this.dwMaximumCipherStrength = 0;
      this.dwSessionLifespan = this.reserved = 0;
      this.version = version;
      this.dwFlags = flags;
      this.grbitEnabledProtocols = protocols;
      if (certificate == null)
        return;
      this.certContextArray = certificate.Handle;
      this.cCreds = 1;
    }

    [System.Flags]
    public enum Flags
    {
      Zero = 0,
      NoSystemMapper = 2,
      NoNameCheck = 4,
      ValidateManual = 8,
      NoDefaultCred = 16,
      ValidateAuto = 32,
    }
  }
}
