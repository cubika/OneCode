/********************************* 模块头 *********************************\
* 模块名: Photo.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* photo实体类.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.IO;
using System.Windows.Media.Imaging;
using VideoStoryCreator.Transitions;

namespace VideoStoryCreator.Models
{
    public class Photo
    {
        public string Name { get; set; }
        public Stream ThumbnailStream { get; set; }
        public WriteableBitmap ResizedImage { get; set; }
        public Stream ResizedImageStream { get; set; }
        public ITransition Transition { get; set; }
        public TimeSpan PhotoDuration { get; set; }

 
        public override bool Equals(object obj)
        {
            if (obj is Photo)
            {
                return this.Name.Equals(((Photo)obj).Name);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}
