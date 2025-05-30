using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OKX_Studio.UserControls
{
    /// <summary>
    /// NonNegativeIntTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class NonNegativeIntTextBox : TextBox
    {
        public NonNegativeIntTextBox()
        {
            InitializeComponent();
            this.PreviewMouseDown += new MouseButtonEventHandler(OnPreviewMouseDown);
            this.GotFocus += new RoutedEventHandler(OnGotFocus);
            this.LostFocus += new RoutedEventHandler(OnLostFocus);
        }
        private string _lastValidText = "";

        // 按键过滤（仅允许数字和功能键）
        private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // 允许数字键（主键盘和小键盘）
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                e.Handled = false;
                return;
            }

            // 允许功能键
            var allowedKeys = new[] {
        Key.Back, Key.Delete, Key.Left,
        Key.Right, Key.Tab, Key.Enter
    };
            if (allowedKeys.Contains(e.Key))
            {
                e.Handled = false;
                return;
            }

            // 允许Ctrl+V（实际有效性由粘贴事件控制）
            if (e.Key == Key.V &&
               (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = false;
                return;
            }

            // 禁止其他所有按键（包括小数点）
            e.Handled = true;
        }

        // 实时输入验证
        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 直接禁止空格、负号和小数点
            if (e.Text.Any(c => char.IsWhiteSpace(c) || c == '-' || c == '.'))
            {
                e.Handled = true;
                return;
            }

            TextBox textBox = (TextBox)sender;
            string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);

            // 新正则表达式（纯数字校验）
            bool isValid = Regex.IsMatch(newText, @"^\d*$");
            e.Handled = !isValid;
        }

        // 最终验证
        private void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            // 防御性清理（空格、符号、小数点）
            if (textBox.Text.Any(c => !char.IsDigit(c)))
            {
                textBox.Text = new string(textBox.Text.Where(char.IsDigit).ToArray());
                textBox.CaretIndex = textBox.Text.Length;
                return;
            }

            // 允许空值
            if (string.IsNullOrEmpty(textBox.Text))
            {
                _lastValidText = "";
                return;
            }

            // 最终验证（非负整数）
            if (int.TryParse(textBox.Text, NumberStyles.None, CultureInfo.InvariantCulture, out int result) && result >= 0)
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

                // 必须满足：纯数字组成且是有效整数
                if (!Regex.IsMatch(pasteText, @"^\d+$") ||
                    !int.TryParse(pasteText, out int _))
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
