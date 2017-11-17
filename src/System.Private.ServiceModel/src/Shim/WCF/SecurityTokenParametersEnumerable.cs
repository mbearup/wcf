// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SecurityTokenParametersEnumerable
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Collections;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
  internal class SecurityTokenParametersEnumerable : IEnumerable<SecurityTokenParameters>, IEnumerable
  {
    private SecurityBindingElement sbe;
    private bool clientTokensOnly;

    public SecurityTokenParametersEnumerable(SecurityBindingElement sbe)
      : this(sbe, false)
    {
    }

    public SecurityTokenParametersEnumerable(SecurityBindingElement sbe, bool clientTokensOnly)
    {
      if (sbe == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("sbe");
      this.sbe = sbe;
      this.clientTokensOnly = clientTokensOnly;
    }

    public IEnumerator<SecurityTokenParameters> GetEnumerator()
    {
      if (this.sbe is SymmetricSecurityBindingElement)
      {
        SymmetricSecurityBindingElement sbe = (SymmetricSecurityBindingElement) this.sbe;
        if (sbe.ProtectionTokenParameters != null && (!this.clientTokensOnly || !sbe.ProtectionTokenParameters.HasAsymmetricKey))
          yield return sbe.ProtectionTokenParameters;
      }
      else
      {  
#if FEATURE_CORECLR
          CompatibilityShim.Log("Skipping AsymmetricSecurityBindingElement");
#else
          if (this.sbe is AsymmetricSecurityBindingElement)
          {
            AsymmetricSecurityBindingElement asbe = (AsymmetricSecurityBindingElement) this.sbe;
            if (asbe.InitiatorTokenParameters != null)
              yield return asbe.InitiatorTokenParameters;
            if (asbe.RecipientTokenParameters != null && !this.clientTokensOnly)
              yield return asbe.RecipientTokenParameters;
            asbe = (AsymmetricSecurityBindingElement) null;
          }
#endif
      }
      foreach (SecurityTokenParameters securityTokenParameters in this.sbe.EndpointSupportingTokenParameters.Endorsing)
      {
        if (securityTokenParameters != null)
          yield return securityTokenParameters;
      }
      foreach (SecurityTokenParameters securityTokenParameters in this.sbe.EndpointSupportingTokenParameters.SignedEncrypted)
      {
        if (securityTokenParameters != null)
          yield return securityTokenParameters;
      }
      foreach (SecurityTokenParameters securityTokenParameters in this.sbe.EndpointSupportingTokenParameters.SignedEndorsing)
      {
        if (securityTokenParameters != null)
          yield return securityTokenParameters;
      }
      foreach (SecurityTokenParameters securityTokenParameters in this.sbe.EndpointSupportingTokenParameters.Signed)
      {
        if (securityTokenParameters != null)
          yield return securityTokenParameters;
      }
#if FEATURE_CORECLR
      CompatibilityShim.Log("Skipping SecurityBindingElement.OperationSupportingTokenParameters");
#else
      foreach (SupportingTokenParameters supportingTokenParameters in (IEnumerable<SupportingTokenParameters>) this.sbe.OperationSupportingTokenParameters.Values)
      {
        SupportingTokenParameters str = supportingTokenParameters;
        if (str != null)
        {
          foreach (SecurityTokenParameters securityTokenParameters in str.Endorsing)
          {
            if (securityTokenParameters != null)
              yield return securityTokenParameters;
          }
          foreach (SecurityTokenParameters securityTokenParameters in str.SignedEncrypted)
          {
            if (securityTokenParameters != null)
              yield return securityTokenParameters;
          }
          foreach (SecurityTokenParameters securityTokenParameters in str.SignedEndorsing)
          {
            if (securityTokenParameters != null)
              yield return securityTokenParameters;
          }
          foreach (SecurityTokenParameters securityTokenParameters in str.Signed)
          {
            if (securityTokenParameters != null)
              yield return securityTokenParameters;
          }
        }
        str = (SupportingTokenParameters) null;
      }
#endif
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotImplementedException());
    }
  }
}
