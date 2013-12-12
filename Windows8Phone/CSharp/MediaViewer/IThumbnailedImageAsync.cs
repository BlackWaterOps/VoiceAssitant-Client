/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Phone.Controls
{
    public interface IThumbnailedImageAsync
    {
        Task<Stream> GetThumbnailImage();
        Task<Stream> GetImage();
        DateTime DateTaken { get; }
    }
}