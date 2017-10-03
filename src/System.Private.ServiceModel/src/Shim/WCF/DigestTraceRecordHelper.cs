
using System.IO;
using System.ServiceModel;
using System.Security.Cryptography;

internal static class DigestTraceRecordHelper
{
  public static bool ShouldTraceDigest
  {
    get
    {
      return false;
    }
  }

  public static void TraceDigest(MemoryStream m, HashAlgorithm hash)
  {}
}
