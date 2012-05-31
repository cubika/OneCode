/****************************** Module Header ******************************\
 模块:      Customer.cs
 项目:      CSWPFMasterDetailBinding
 版权 (c) Microsoft Corporation.
 
 此示例演示在WPF中主/细形式的数据绑定。
 
 This source is subject to the Microsoft Public License.
 See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 All other rights reserved.
 
 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace CSWPFMasterDetailBinding.Data
{
    class Customer : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private ObservableCollection<Order> _orders
            = new ObservableCollection<Order>();

        public int ID
        {
            get { return _id; }
            set { 
                _id = value;
                OnPropertyChanged("ID");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public ObservableCollection<Order> Orders
        {
            get { return _orders; }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
