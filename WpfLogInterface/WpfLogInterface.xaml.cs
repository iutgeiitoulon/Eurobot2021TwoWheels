using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfLogInterfaceNs
{
    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    public partial class WpfLogInterface : UserControl
    {
        private bool logrecorder = false;
        private bool logreplay = false;
        private bool pause_play_replay = false;

        public WpfLogInterface()
        {
            InitializeComponent();
        }

        private void OnLogRecorderClick(object sender, RoutedEventArgs e)
        {
            logrecorder = !logrecorder;

            OnLogRecorderSwitchEvent?.Invoke(this, logrecorder);
        }

        private void OnLogReplayClick(object sender, RoutedEventArgs e)
        {
            logreplay = !logreplay;
            OnLogReplayEvent?.Invoke(this, logreplay);
        }

        private void On_Pause_Play_Click(object sender, RoutedEventArgs e)
        {
            pause_play_replay = !pause_play_replay;

            if (pause_play_replay)
            {
                Pause_Play_Btn.Content = "▶";
            }
            else
            {
                Pause_Play_Btn.Content = "⏸";
            }

            OnPausePlaySwitchEvent?.Invoke(this, pause_play_replay);
        }

        private void On_Skip_Click(object sender, RoutedEventArgs e)
        {
            OnSkipBtnEvent?.Invoke(this, new EventArgs());
        }

        private void On_Back_Click(object sender, RoutedEventArgs e)
        {
            OnBackBtnEvent?.Invoke(this, new EventArgs());
        }

        #region Events

        public event EventHandler<bool> OnLogRecorderSwitchEvent;
        public event EventHandler<bool> OnLogReplayEvent;

        public event EventHandler<bool> OnPausePlaySwitchEvent;

        public event EventHandler<EventArgs> OnSkipBtnEvent;
        public event EventHandler<EventArgs> OnBackBtnEvent;


        #endregion
    }
}
