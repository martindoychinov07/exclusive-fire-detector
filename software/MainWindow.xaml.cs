using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace APM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly SolidColorBrush WaitingColor = new SolidColorBrush(Color.FromRgb(0xFF, 0xA5, 0x00));
        private static readonly SolidColorBrush ConnectedColor = new SolidColorBrush(Color.FromRgb(0x00, 0xFA, 0x9A));
        private static readonly SolidColorBrush SafeColor = new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50));
        private static readonly SolidColorBrush WarningColor = new SolidColorBrush(Color.FromRgb(0xFF, 0xA5, 0x00));
        private static readonly SolidColorBrush DangerColor = new SolidColorBrush(Color.FromRgb(0xDC, 0x14, 0x3C));

        // Timers
        private DispatcherTimer _connectionTimer;
        private DispatcherTimer _objectDetectionTimer;
        private DispatcherTimer _objectResetTimer;
        private DispatcherTimer _flameDetectionTimer;
        private DispatcherTimer _flameResetTimer;

        public MainWindow()
        {
            InitializeComponent();
            SetInitialState();
            SetupTimers();
        }

        private void SetInitialState()
        {
            SetDeviceStatus("waiting");
            FlameDetectorText.Text = "No Flame Detected";
            FlameDetectorText.Foreground = SafeColor;
            ObjectDetectorText.Text = "No Object Detected";
            ObjectDetectorText.Foreground = SafeColor;
            GasDetectorText.Text = "No Gas Detected";
            GasDetectorText.Foreground = SafeColor;
        }

        private void SetupTimers()
        {
            // Device connection (10 seconds)
            _connectionTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(20) };
            _connectionTimer.Tick += (s, e) => {
                SetDeviceStatus("connected");
                _connectionTimer.Stop();
            };
            _connectionTimer.Start();

            // Object detection (14.53 seconds)
            _objectDetectionTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(24.53) };
            _objectDetectionTimer.Tick += (s, e) => {
                ObjectDetectorText.Text = "⚠️ Object Detected!";
                ObjectDetectorText.Foreground = WarningColor;
                _objectDetectionTimer.Stop();

                // Start reset timer (3 seconds)
                _objectResetTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
                _objectResetTimer.Tick += (s2, e2) => {
                    ObjectDetectorText.Text = "No Object Detected";
                    ObjectDetectorText.Foreground = SafeColor;
                    _objectResetTimer.Stop();
                };
                _objectResetTimer.Start();
            };
            _objectDetectionTimer.Start();

            // Flame detection (31.28 seconds)
            _flameDetectionTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(41.28) };
            _flameDetectionTimer.Tick += (s, e) => {
                FlameDetectorText.Text = "🔥 Flame Detected!";
                FlameDetectorText.Foreground = DangerColor;
                _flameDetectionTimer.Stop();

                // Start reset timer (4.35 seconds)
                _flameResetTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5.35) };
                _flameResetTimer.Tick += (s2, e2) => {
                    FlameDetectorText.Text = "No Flame Detected";
                    FlameDetectorText.Foreground = SafeColor;
                    _flameResetTimer.Stop();
                };
                _flameResetTimer.Start();
            };
            _flameDetectionTimer.Start();
        }

        private void SetDeviceStatus(string status)
        {
            switch (status.ToLower())
            {
                case "connected":
                    DeviceStatusTextBlock.Text = "DEVICE CONNECTED";
                    DeviceStatusTextBlock.Foreground = ConnectedColor;
                    break;
                case "error":
                    DeviceStatusTextBlock.Text = "CONNECTION ERROR";
                    DeviceStatusTextBlock.Foreground = Brushes.Red;
                    break;
                default:
                    DeviceStatusTextBlock.Text = "WAITING FOR CONNECTION...";
                    DeviceStatusTextBlock.Foreground = WaitingColor;
                    break;
            }
        }
    }
}