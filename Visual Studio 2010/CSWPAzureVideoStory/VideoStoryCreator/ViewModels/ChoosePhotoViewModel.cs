/********************************* 模块头 *********************************\
* 模块名: ChoosePhotoViewModel.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 选中图片的ViewModel类.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


namespace VideoStoryCreator.ViewModels
{
    public class ChoosePhotoViewModel : PhotoViewModel
    {
        public bool IsSelected { get; set; }
    }
}
