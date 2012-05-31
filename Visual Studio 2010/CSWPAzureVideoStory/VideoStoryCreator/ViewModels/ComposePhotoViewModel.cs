/********************************* 模块头 *********************************\
* 模块名: ComposePhotoViewModel.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 原版页面的ViewModel类.
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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using VideoStoryCreator.Models;
using VideoStoryCreator.Transitions;

namespace VideoStoryCreator.ViewModels
{
    public class ComposePhotoViewModel : PhotoViewModel
    {
        public ITransition Transition { get; set; }

        private UserControl _transitionDesigner;
        public UserControl TransitionDesigner
        {
            get
            {
                return this._transitionDesigner;
            }
            set
            {
                if (this._transitionDesigner != value)
                {
                    this._transitionDesigner = value;
                    this.NotifyPropertyChange("TransitionDesigner");
                }
            }
        }

        public List<string> AvailableTransitions
        {
            get
            {
                return TransitionFactory.AvailableTransitions;
            }
        }

        /// <summary>
        ///  复制view model.
        /// </summary>
        /// <returns>view model备份.</returns>
        public ComposePhotoViewModel CopyTo()
        {
            ComposePhotoViewModel copy = new ComposePhotoViewModel()
            {
                Name = this.Name,
                PhotoDuration = this.PhotoDuration,
                TransitionDuration = this.TransitionDuration,
                MediaStream = this.MediaStream
            };
            if (this.Transition != null)
            {
                copy.Transition = this.Transition.Clone();
            }
            return copy;
        }

        /// <summary>
        /// 从复制的ViewModel类改变值.
        /// </summary>
        public void CopyFrom(ComposePhotoViewModel source)
        {
            this.Name = source.Name;
            this.PhotoDuration = source.PhotoDuration;
            this.TransitionDuration = source.TransitionDuration;
            if (source.Transition != null)
            {
                this.Transition = source.Transition.Clone();
            }
            this.MediaStream = source.MediaStream;
        }

        /// <summary>
        /// 将实体类改变为ViewModel类.
        /// </summary>
        public static ComposePhotoViewModel CreateFromModel(Photo model)
        {
            ComposePhotoViewModel viewModel = new ComposePhotoViewModel()
            {
                Name = model.Name,
                MediaStream = model.ThumbnailStream,
                PhotoDuration = (int)(model.PhotoDuration.TotalSeconds)                    
            };
            if (model.Transition != null)
            {
                viewModel.Transition = model.Transition;
                viewModel.TransitionDuration = (int)(model.Transition.TransitionDuration.TotalSeconds);
            }
            return viewModel;
        }

        /// <summary>
        /// 更行view model的通信.
        /// </summary>
        public void UpdateModel()
        {
            Photo photoToModify = App.MediaCollection.Where(p => p.Name == this.Name).FirstOrDefault();
            if (photoToModify != null && this.Transition != null)
            {
                photoToModify.Transition = this.Transition;
                photoToModify.Transition.TransitionDuration = TimeSpan.FromSeconds(this.TransitionDuration);
                photoToModify.PhotoDuration = TimeSpan.FromSeconds(this.PhotoDuration);
            }
        }

        /// <summary>
        /// 如果ViewModel被删除, 同样需要删除通信类.
        /// </summary>
        public void RemoveModel()
        {
            Photo model = App.MediaCollection.Where(p => p.Name == this.Name).FirstOrDefault();
            if (model != null)
            {
                model.ThumbnailStream.Close();
                App.MediaCollection.Remove(model);
            }
        }
    }
}
