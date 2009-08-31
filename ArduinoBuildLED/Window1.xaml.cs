using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ArduinoBuildLED
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private DateTime _lastMessageSentTime;

        private int _currentRed;
        private int _currentGreen;
        private int _currentBlue;

        private Arduino _arduino;
        private TfsController _tfsController;

        private System.Windows.Forms.NotifyIcon m_notifyIcon;

        public Window1()
        {
            InitializeComponent();

            // initialise code here
            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "The app has been minimized. TFS monitoring will continue and the lamp will continue to function.";
            m_notifyIcon.BalloonTipTitle = "Arduino Build LED";
            m_notifyIcon.Text = "Arduino Build LED";
            m_notifyIcon.Icon = new System.Drawing.Icon("timemachine.ico");
            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);
        }

        private void btnToggleRed_Click(object sender, RoutedEventArgs e)
        {
            if (_currentRed > 0)
            {
                _currentRed = 0;
            }
            else
            {
                _currentRed = 255;
            }

            _arduino.ChangeColor(_currentRed, 1, 1);
        }

        private void btnToggleGreen_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGreen > 0)
            {
                _currentGreen = 0;
            }
            else
            {
                _currentGreen = 255;
            }

            _arduino.ChangeColor(1, _currentGreen, 1);
        }

        private void btnToggleBlue_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBlue > 0)
            {
                _currentBlue = 0;
            }
            else
            {
                _currentBlue = 255;
            }

            _arduino.ChangeColor(1, 1, _currentBlue);
        }

        private void btnTurnAllOff_Click(object sender, RoutedEventArgs e)
        {
            _arduino.ChangeColor(0, 0, 0);
        }

        private void btnConnectArduino_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _arduino = new Arduino(txtPortName.Text, txtBaudRate.Text);

                _lastMessageSentTime = DateTime.Now;

                btnTurnAllOff.IsEnabled = true;
                btnTurnBlueOn.IsEnabled = true;
                btnTurnGreenOn.IsEnabled = true;
                btnTurnRedOn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                btnTurnAllOff.IsEnabled = false;
                btnTurnBlueOn.IsEnabled = false;
                btnTurnGreenOn.IsEnabled = false;
                btnTurnRedOn.IsEnabled = false;

                MessageBox.Show(ex.Message);
            }
        }

        private void btnConnectTFS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _tfsController = new TfsController(_arduino);
                _tfsController.TfsUrl = String.Format("http://{0}:8080", txtTFSServer.Text);
                _tfsController.TeamProject = txtTFSProject.Text;
                _tfsController.BuildDefinitionName = txtTFSBuildToMonitor.Text;

                _tfsController.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            _arduino.Dispose();

            this.Close();
        }

        private void sliderRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_arduino != null && _arduino.IsConnected && _lastMessageSentTime <= DateTime.Now.AddMilliseconds(-250))
            {
                _currentRed = (int)sliderRed.Value;

                _arduino.ChangeColor(_currentRed, 1, 1);
                _lastMessageSentTime = DateTime.Now;
            }
        }

        private void sliderGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_arduino != null && _arduino.IsConnected && _lastMessageSentTime <= DateTime.Now.AddMilliseconds(-250))
            {
                _currentGreen = (int)sliderGreen.Value;

                _arduino.ChangeColor(1, _currentGreen, 1);
                _lastMessageSentTime = DateTime.Now;
            }
        }

        private void sliderBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_arduino != null && _arduino.IsConnected && _lastMessageSentTime <= DateTime.Now.AddMilliseconds(-250))
            {
                _currentBlue = (int)sliderBlue.Value;

                _arduino.ChangeColor(1, 1, _currentBlue);
                _lastMessageSentTime = DateTime.Now;
            }
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_notifyIcon.Dispose();
            m_notifyIcon = null;
        }

        private WindowState m_storedWindowState = WindowState.Normal;
        private void OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                if (m_notifyIcon != null)
                    m_notifyIcon.ShowBalloonTip(2000);
            }
            else
                m_storedWindowState = WindowState;
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CheckTrayIcon();
        }

        void m_notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = m_storedWindowState;
        }
        void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        void ShowTrayIcon(bool show)
        {
            if (m_notifyIcon != null)
                m_notifyIcon.Visible = show;
        }
    }
}
