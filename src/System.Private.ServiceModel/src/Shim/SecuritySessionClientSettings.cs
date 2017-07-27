// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SecuritySessionClientSettings
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Globalization;

namespace System.ServiceModel.Security
{
  internal class SecuritySessionClientSettings
  {
    internal static readonly TimeSpan defaultKeyRenewalInterval = TimeSpan.Parse("10:00:00", (IFormatProvider) CultureInfo.InvariantCulture);
    internal static readonly TimeSpan defaultKeyRolloverInterval = TimeSpan.Parse("00:05:00", (IFormatProvider) CultureInfo.InvariantCulture);
    internal const string defaultKeyRenewalIntervalString = "10:00:00";
    internal const string defaultKeyRolloverIntervalString = "00:05:00";
    internal const bool defaultTolerateTransportFailures = true;
  }
}
