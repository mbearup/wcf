// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Description.IWsdlExportExtension
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

namespace System.ServiceModel.Description
{
  /// <summary>Defines endpoint or contract behaviors that can export custom metadata.</summary>
  public interface IWsdlExportExtension
  {
    /// <summary>Writes custom Web Services Description Language (WSDL) elements into the generated WSDL for a contract.</summary>
    /// <param name="exporter">The <see cref="T:System.ServiceModel.Description.WsdlExporter" /> that exports the contract information.</param>
    /// <param name="context">Provides mappings from exported WSDL elements to the contract description.</param>
    void ExportContract(WsdlExporter exporter, WsdlContractConversionContext context);

    /// <summary>Writes custom Web Services Description Language (WSDL) elements into the generated WSDL for an endpoint.</summary>
    /// <param name="exporter">The <see cref="T:System.ServiceModel.Description.WsdlExporter" /> that exports the endpoint information.</param>
    /// <param name="context">Provides mappings from exported WSDL elements to the endpoint description.</param>
    void ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context);
  }
}
