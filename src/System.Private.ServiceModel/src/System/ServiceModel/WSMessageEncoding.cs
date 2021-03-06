// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel
{
    public enum WSMessageEncoding
    {
        Text = 0,
        Mtom = 1
    }

    public static class WSMessageEncodingHelper
    {
        public static bool IsDefined(WSMessageEncoding value)
        {
            return value == WSMessageEncoding.Text;
        }
    }
}
