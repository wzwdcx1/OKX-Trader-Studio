using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OKX_Studio.UserControls
{
    /// <summary>
    /// DoubleOnlyTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class DoubleTextBox : TextBox
    {
        public DoubleTextBox()
        {
            InitializeComponent();
            this.PreviewMouseDown += new MouseButtonEventHandler(OnPreviewMouseDown);
            this.GotFocus += new RoutedEventHandler(OnGotFocus);
            this.LostFocus += new RoutedEventHandler(OnLostFocus);
        }

        private string _lastValidText = string.Empty;

        // 实时输入验证（已移除科学计数法支持）
        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 禁止空格输入
            if (e.Text.Any(char.IsWhiteSpace))
            {
                e.Handled = true;
                return;
            }

            TextBox textBox = (TextBox)sender;
            string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);

            // 新正则表达式（不支持科学计数法）
            bool isValid = Regex.IsMatch(newText,
                @"^([-+]?(?:\d+[.,]?\d*|[.,]\d+)|[-+]?[.,]?)$");
            e.Handled = !isValid;
        }

        // 最终验证
        private void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            // 清理空格（防御性处理）
            if (textBox.Text.Any(char.IsWhiteSpace))
            {
                textBox.Text = textBox.Text.Replace(" ", "");
                textBox.CaretIndex = textBox.Text.Length;
                return;
            }

            // 允许中间状态
            if (textBox.Text == "-" ||
                textBox.Text == "." ||
                textBox.Text == "+" ||
                textBox.Text == "-." ||
                textBox.Text == "+.")
            {
                _lastValidText = textBox.Text;
                return;
            }

            // 空值处理
            if (string.IsNullOrEmpty(textBox.Text))
            {
                _lastValidText = "";
                return;
            }

            // 最终验证（不支持科学计数法）
            bool isNumeric = double.TryParse(textBox.Text,
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out double _);

            if (isNumeric)
            {
                _lastValidText = textBox.Text;
            }
            else
            {
                textBox.Text = _lastValidText;
                textBox.CaretIndex = Math.Min(_lastValidText.Length, textBox.Text.Length);
            }
        }

        // 粘贴处理
        private void NumericTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string pasteText = (string)e.DataObject.GetData(typeof(string));

                // 检查空格和格式
                if (pasteText.Any(char.IsWhiteSpace) ||
                    !Regex.IsMatch(pasteText, @"^[-+]?(\d+([.,]\d+)?|[.,]\d+)$"))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
        /// <summary>
        /// 选中全部内容  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.SelectAll();
            textBox.PreviewMouseDown -= new MouseButtonEventHandler(OnPreviewMouseDown);
        }
        void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
             TextBox tb = sender as TextBox;
             tb.Focus();
             e.Handled = true;
        }
        void OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.PreviewMouseDown += new MouseButtonEventHandler(OnPreviewMouseDown);
        }
    }
}
