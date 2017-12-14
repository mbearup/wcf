// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Dispatcher.HeaderFilter
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
  internal abstract class HeaderFilter : MessageFilter
  {
    public override bool Match(MessageBuffer buffer)
    {
      if (buffer == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
      Message message = buffer.CreateMessage();
      try
      {
        return this.Match(message);
      }
      finally
      {
        message.Close();
      }
    }
  }
}
