// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.WebHttpSecurityModeHelper
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

namespace System.ServiceModel
{
  internal static class WebHttpSecurityModeHelper
  {
    internal static bool IsDefined(WebHttpSecurityMode value)
    {
      if (value != WebHttpSecurityMode.None && value != WebHttpSecurityMode.Transport)
        return value == WebHttpSecurityMode.TransportCredentialOnly;
      return true;
    }
  }
}
