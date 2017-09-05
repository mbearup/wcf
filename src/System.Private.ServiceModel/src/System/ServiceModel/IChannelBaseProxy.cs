// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    /// <summary>
    /// An interface used by ChannelBase to override the ServiceChannel that would normally be returned by ClientBase. 
    /// </summary>
    public interface IChannelBaseProxy
    {
        ServiceChannel GetServiceChannel();
    }
}
