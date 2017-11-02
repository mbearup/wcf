using System.IdentityModel.Selectors;

// A shim to enable some WIF3.5 classes, specifically WSSecurityTokenSerializer, to be passed back to WCF

namespace System.ServiceModel
{
  public static class CompatibilityShim
  {
    public static SecurityTokenSerializer Serializer
    {
      get; set;
    }
  }
}
