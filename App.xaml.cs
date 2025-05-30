using Autofac;
using OKX_Studio.ViewModels;
using System.Configuration;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace OKX_Studio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // 导入Win32 API用于激活窗口
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        private const int SW_RESTORE = 9;

        private static Mutex _mutex;

        public static Autofac.IContainer Container { get; private set; }
        //程序启动方法
        protected override void OnStartup(StartupEventArgs e)
        {
            // 全局异常处理
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var exception = (Exception)args.ExceptionObject;
                File.WriteAllText("crash.log", $"Unhandled Exception: {exception}");
            };

            try
            {

                const string mutexName = "OKX Studio"; // 使用唯一名称

                // 尝试创建互斥体
                bool createdNew;
                _mutex = new Mutex(true, mutexName, out createdNew);

                if (!createdNew)
                {
                    // 如果互斥体已存在，激活现有实例
                    ActivateOtherWindow();

                    // 关闭当前实例
                    Current.Shutdown();
                    return;
                }

                base.OnStartup(e);
                RenderOptions.ProcessRenderMode = RenderMode.Default;
                //创建容器的构造器
                var builder = new ContainerBuilder();

                //注册ViewModel为单例
                builder.RegisterType<MainViewModel>().SingleInstance();

                //生成容器
                Container = builder.Build();
                var viewModel = Container.Resolve<MainViewModel>();

                //启动主窗口
                var mainView = new MainWindow(viewModel);
                mainView.Show();
            }

            catch (Exception ex)
            {
                File.WriteAllText("startup_error.log", $"Startup Failed: {ex}");
                throw;
            }
        }
        /// <summary>
        /// 进程退出释放互斥体
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            _mutex?.ReleaseMutex();
            base.OnExit(e);
        }
        /// <summary>
        /// 
        /// </summary>
        private static void ActivateOtherWindow()
        {
            // 获取当前进程（实际是之前的实例）
            var currentProcess = System.Diagnostics.Process.GetCurrentProcess();

            // 查找同名的其他进程
            foreach (var process in System.Diagnostics.Process.GetProcessesByName(currentProcess.ProcessName))
            {
                if (process.Id == currentProcess.Id) continue;

                // 获取主窗口句柄
                IntPtr hWnd = process.MainWindowHandle;

                if (hWnd != IntPtr.Zero)
                {
                    // 恢复最小化的窗口
                    ShowWindowAsync(hWnd, SW_RESTORE);
                    // 激活窗口
                    SetForegroundWindow(hWnd);
                    break;
                }
            }
        }
    }

}
