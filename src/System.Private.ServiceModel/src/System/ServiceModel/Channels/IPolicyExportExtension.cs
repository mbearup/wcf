// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Description.IPolicyExportExtension
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

namespace System.ServiceModel.Description
{
  /// <summary>Implement <see cref="T:System.ServiceModel.Description.IPolicyExportExtension" /> to insert custom binding policy assertions in the Web Services Description Language (WSDL) information.</summary>
  public interface IPolicyExportExtension
  {
    /// <summary>Implement to include for exporting a custom policy assertion about bindings.</summary>
    /// <param name="exporter">The <see cref="T:System.ServiceModel.Description.MetadataExporter" /> that you can use to modify the exporting process.</param>
    /// <param name="context">The <see cref="T:System.ServiceModel.Description.PolicyConversionContext" /> that you can use to insert your custom policy assertion.</param>

    // MetadataExporter and PolicyConversionContext not supported in .NET Core
    // void ExportPolicy(MetadataExporter exporter, PolicyConversionContext context);
  }
}
