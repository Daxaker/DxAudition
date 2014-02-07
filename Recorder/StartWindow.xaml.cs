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
using System.Windows.Shapes;
using NAudio;
using NAudio.Wave;
using System.IO;

namespace Recorder
{
    /// <summary>
    /// Логика взаимодействия для StartWindow.xaml
    /// </summary>
    public partial class StartWindow 
    {
        AudioProc audioProc = new AudioProc(System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString() + ".wav"));
        private int _deviceNumber;
        public StartWindow(int devNum)
        {
            InitializeComponent();
            button2.IsEnabled = false;
            button3.IsEnabled = false;
            _deviceNumber = devNum;
            audioProc.pitchDetectedEvent += audioProc_pitchDetectedEvent;
        }

        void audioProc_pitchDetectedEvent()
        {
            label1.Content = String.Format(@"{0}({1})", audioProc.Note, audioProc.Octave);
            label2.Content = String.Format(@"{0:0.00} Hz", audioProc.Pitch);
        }
        
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            audioProc.waveInStart(_deviceNumber);
            button1.IsEnabled = false;
            button2.IsEnabled = true;
            button3.IsEnabled = false;
            button4.IsEnabled = false;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            audioProc.waveInStop();
            button1.IsEnabled = true;
            button2.IsEnabled = false;
            button3.IsEnabled = true;
            button4.IsEnabled = true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            audioProc.SaveWaveFile();
            button3.IsEnabled = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.MainWindow.Show();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            File.Delete(audioProc.FileName);
        }

        
    }
}
