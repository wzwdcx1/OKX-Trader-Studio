using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX_Studio.Models
{
    public class Common
    {

    }
    /// <summary>
    /// 实现INotifyPropertyChanged接口的基类
    /// </summary>
    public class BaseNotifyPropertyChanged : INotifyPropertyChanged
    {
        //实现INotifyPropertyChanged接口的标准方式，用于通知UI控件属性的变化
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
