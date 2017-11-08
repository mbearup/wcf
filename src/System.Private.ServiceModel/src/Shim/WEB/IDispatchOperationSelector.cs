// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Dispatcher.IDispatchOperationSelector
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
  /// <summary>Defines the contract that associates incoming messages with a local operation to customize service execution behavior.</summary>
  public interface IDispatchOperationSelector
  {
    /// <summary>Associates a local operation with the incoming method.</summary>
    /// <param name="message">The incoming <see cref="T:System.ServiceModel.Channels.Message" /> to be associated with an operation.</param>
    /// <returns>The name of the operation to be associated with the <paramref name="message" />.</returns>
    string SelectOperation(ref Message message);
  }
}
