using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OKX_Studio.UserControls
{
    /// <summary>
    /// NonNegativeDoubleTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class NonNegativeDoubleTextBox : TextBox
    {
        public NonNegativeDoubleTextBox()
        {
            InitializeComponent();
            this.PreviewMouseDown += new MouseButtonEventHandler(OnPreviewMouseDown);
            this.GotFocus += new RoutedEventHandler(OnGotFocus);
            this.LostFocus += new RoutedEventHandler(OnLostFocus);
        }
        private string _lastValidText = string.Empty;

        // 实时输入验证（禁止负号）
        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 禁止空格和负号
            if (e.Text.Any(c => char.IsWhiteSpace(c) || c == '-'))
            {
                e.Handled = true;
                return;
            }

            TextBox textBox = (TextBox)sender;
            string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);

            // 新正则表达式（仅非负数值）
            bool isValid = Regex.IsMatch(newText,
                @"^(?:\d+[.,]?\d*|[.,]\d+|[.,]?)$");
            e.Handled = !isValid;
        }

        // 最终验证
        private void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            // 清理空格和负号（防御性处理）
            if (textBox.Text.Any(c => char.IsWhiteSpace(c) || c == '-'))
            {
                textBox.Text = new string(textBox.Text
                    .Replace(" ", "")
                    .Replace("-", "")
                    .ToArray());
                textBox.CaretIndex = textBox.Text.Length;
                return;
            }

            // 允许中间状态
            if (textBox.Text == "." || string.IsNullOrEmpty(textBox.Text))
            {
                _lastValidText = textBox.Text;
                return;
            }

            // 最终验证（强制非负）
            bool isNumeric = double.TryParse(textBox.Text,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out double result) && result >= 0;

            if (isNumeric || textBox.Text == ".")
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

                // 检查非法字符和负值
                if (pasteText.Any(c => char.IsWhiteSpace(c) || c == '-') ||
                    !Regex.IsMatch(pasteText, @"^(\d+([.,]\d+)?|[.,]\d+)$") ||
                    !double.TryParse(pasteText, NumberStyles.AllowDecimalPoint,
                        CultureInfo.InvariantCulture, out double val) ||
                    val < 0)
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }


        // 修复后的按键过滤逻辑
        private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // 允许所有数字键（主键盘和数字小键盘）
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                e.Handled = false;
                return;
            }

            // 允许小数点（兼容不同文化设置）
            if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
            {
                e.Handled = false;
                return;
            }

            // 允许功能键
            var allowedKeys = new[] 
            {
                    Key.Back,
                    Key.Delete,
                    Key.Left,
                    Key.Right,
                    Key.Tab,
                    Key.Enter
            };
            if (allowedKeys.Contains(e.Key))
            {
                e.Handled = false;
                return;
            }

            // 允许Ctrl+V（实际粘贴由Pasting事件控制）
            if (e.Key == Key.V &&
               (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = false;
                return;
            }

            // 其他按键一律阻止
            e.Handled = true;
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
