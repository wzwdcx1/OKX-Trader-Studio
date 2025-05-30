using OKX_Studio.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //获取币种信息
            Spots = [];
            string spotInfoFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Datas\\ALLSPOT.txt";
            foreach (string spot in File.ReadLines(spotInfoFilePath))
            {
                Spots.Add(spot);
            }
        }

    }
}
