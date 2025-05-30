using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OKX_Studio.Models
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<bool> _canExecute;
        private readonly Action<object> _postExecute;

        /// <summary>
        /// 构造函数，接受执行方法和可执行判断方法，新增保存参数方法
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <param name="postExecute"></param>
        /// <exception cref="ArgumentException"></exception>
        public RelayCommand(Action<object> execute, Func<bool> canExecute = null, Action<object> postExecute = null)
        {
            _execute = execute ?? throw new ArgumentException(nameof(execute));
            _canExecute = canExecute;
            _postExecute = postExecute;
        }
        //判断命令是否可以执行
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }
        //执行命令
        public void Execute(object parameter)
        {
            _execute(parameter);
            _postExecute?.Invoke(parameter);
        }
        //事件，当可执行状态改变时触发
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
