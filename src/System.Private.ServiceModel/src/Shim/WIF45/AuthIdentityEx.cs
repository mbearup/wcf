// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.AuthIdentityEx
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Runtime.InteropServices;

namespace System.IdentityModel
{
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
  public struct AuthIdentityEx
  {
    private static readonly int WinNTAuthIdentityVersion = 512;
    internal int Version;
    internal int Length;
    internal string UserName;
    internal int UserNameLength;
    internal string Domain;
    internal int DomainLength;
    internal string Password;
    internal int PasswordLength;
    internal int Flags;
    internal string PackageList;
    internal int PackageListLength;

    internal AuthIdentityEx(string userName, string password, string domain, params string[] additionalPackages)
    {
      this.Version = AuthIdentityEx.WinNTAuthIdentityVersion;
      this.Length = Marshal.SizeOf(typeof (AuthIdentityEx));
      this.UserName = userName;
      this.UserNameLength = userName == null ? 0 : userName.Length;
      this.Password = password;
      this.PasswordLength = password == null ? 0 : password.Length;
      this.Domain = domain;
      this.DomainLength = domain == null ? 0 : domain.Length;
      this.Flags = 2;
      if (additionalPackages == null)
      {
        this.PackageList = (string) null;
        this.PackageListLength = 0;
      }
      else
      {
        this.PackageList = string.Join(",", additionalPackages);
        this.PackageListLength = this.PackageList.Length;
      }
    }
  }
}
