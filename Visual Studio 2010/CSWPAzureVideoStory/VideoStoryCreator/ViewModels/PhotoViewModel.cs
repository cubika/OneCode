/********************************* 模块头 *********************************\
* 模块名: PhotoViewModel.cs
* 项目名: VideoStoryCreator
* 版权(c) Microsoft Corporation.
* 
* 给予通信类的基类.
* 几种ViewMode关联到phone继承的类.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace VideoStoryCreator.ViewModels
{
    public class PhotoViewModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get { return this._name; }
            set
            {
                if (this._name != value)
                {
                    this._name = value;
                    this.NotifyPropertyChange("Name");
                }
            }
        }

        // MediaStream不必支持提示变更事件.
        public Stream MediaStream { get; set; }

        private int _transitionDuration = 2;
        public int TransitionDuration 
        {
            get { return this._transitionDuration; }
            set
            {
                if (this._transitionDuration != value)
                {
                    this._transitionDuration = value;
                    this.NotifyPropertyChange("TransitionDuration");
                }
            }
        }

        private int _photoDuration = 5;
        public int PhotoDuration
        {
            get { return this._photoDuration; }
            set
            {
                if (this._photoDuration != value)
                {
                    this._photoDuration = value;
                    this.NotifyPropertyChange("PhotoDuration");
                }
            }
        }

        private BitmapImage _imageSource;
        public BitmapImage ImageSource
        {
            get
            {
                if (this.MediaStream == null)
                {
                    return null;
                }

                if (this._imageSource == null)
                {
                    this._imageSource = new BitmapImage();
                    this._imageSource.SetSource(this.MediaStream);
                }

                return this._imageSource;
            }
        }

        /// <summary>
        /// 使用相片的名称进行比较.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is PhotoViewModel)
            {
                return this.Name.Equals(((PhotoViewModel)obj).Name);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChange(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this.PropertyChanged, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
