// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.StandardTransformFactory
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Security.Cryptography;
using System.ServiceModel;

namespace System.IdentityModel
{
  internal class StandardTransformFactory : TransformFactory
  {
    private static StandardTransformFactory instance = new StandardTransformFactory();

    internal static StandardTransformFactory Instance
    {
      get
      {
        return StandardTransformFactory.instance;
      }
    }

    protected StandardTransformFactory()
    {
    }

    public override Transform CreateTransform(string transformAlgorithmUri)
    {
      if (transformAlgorithmUri == "http://www.w3.org/2001/10/xml-exc-c14n#")
        return (Transform) new ExclusiveCanonicalizationTransform();
      if (transformAlgorithmUri == "http://www.w3.org/2001/10/xml-exc-c14n#WithComments")
        return (Transform) new ExclusiveCanonicalizationTransform(false, true);
      if (transformAlgorithmUri == "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#STR-Transform")
      {
#if FEATURE_CORECLR
        throw new NotImplementedException("StrTransform is not supported in .NET Core");
#else
        return (Transform) new StrTransform();
#endif
      }
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new CryptographicException(SR.GetString("UnsupportedTransformAlgorithm")));
    }
  }
}
