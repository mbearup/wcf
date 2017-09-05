// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SecurityProtocolCorrelationState
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.IdentityModel.Tokens;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Security
{
  internal class SecurityProtocolCorrelationState
  {
    private SecurityToken token;
    private SignatureConfirmations signatureConfirmations;
    private ServiceModelActivity activity;

    public SecurityToken Token
    {
      get
      {
        return this.token;
      }
    }

    internal SignatureConfirmations SignatureConfirmations
    {
      get
      {
        return this.signatureConfirmations;
      }
      set
      {
        this.signatureConfirmations = value;
      }
    }

    internal ServiceModelActivity Activity
    {
      get
      {
        return this.activity;
      }
    }

    public SecurityProtocolCorrelationState(SecurityToken token)
    {
      this.token = token;
      this.activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.Current : (ServiceModelActivity) null;
    }
  }
}
