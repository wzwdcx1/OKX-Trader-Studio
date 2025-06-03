using OKX_Studio.Models;
using OkxApi.ApiInfos;
using OkxApi.Common;
using OkxApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OKX_Studio.ViewModels
{
    public class MainViewModel : BaseNotifyPropertyChanged 
    {
        ObservableCollection<string> _spots;
        /// <summary>
        /// 所有币种
        /// </summary>
        public ObservableCollection<string> Spots
        {
            get => _spots;
            set
            {
                _spots = value;
                OnPropertyChanged(nameof(Spots));
            }
        }
        string _selectSpot;
        /// <summary>
        /// 选中的币种
        /// </summary>
        public string SelectSpot
        {
            get => _selectSpot;
            set
            {
                _selectSpot = value;
                OnPropertyChanged(nameof(SelectSpot));
            }
        }
        public MainViewModel()
        {
            InitializeCommands();
            //获取币种信息
            Spots = [];
            string spotInfoFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Datas\\ALLSPOT.txt";
            foreach (string spot in File.ReadLines(spotInfoFilePath))
            {
                Spots.Add(spot);
            }
        }
        #region 命令集
        public ICommand GetCommand { get; private set; }
        
        #endregion
        /// <summary>
        /// 命令集初始化
        /// </summary>
        private void InitializeCommands()
        {
            GetCommand = new RelayCommand(async (parameter) => await Get(parameter));
        }
        private async Task Get(object parameter)
        {
            string apiKey = "62b98274-635b-4a82-a547-295bba64e48b";
            string secretKey = "D459403842184295CC03DF0F80445CF6";
            string passphrase = "Wzwdyy12!";
            ApiKeys apiKeys = new(apiKey, secretKey, passphrase);
            //创建历史K线类
            KLine_History klineHis = new(apiKeys);
            DateTime startTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            DateTime endTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);


            List<OkxCandlestick> kLineDatas = await klineHis.GetCandlesticks(SelectSpot, startTime, endTime, "1m");
        }
    }
}
