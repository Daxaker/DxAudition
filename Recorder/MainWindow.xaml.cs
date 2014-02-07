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
using NAudio;
using NAudio.Wave;
using System.IO;
using Microsoft.Win32;

namespace Recorder
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            for (int waveInDevice = 0; waveInDevice < WaveIn.DeviceCount; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                listBox1.Items.Add(String.Format("Device {0}: {1}", waveInDevice, deviceInfo.ProductName));
            }
            listBox1.SelectedIndex = 1;   
                  
        }
        
        private void OK_Click(object sender, RoutedEventArgs e)
        {
           ;
           Window wnd = new StartWindow(listBox1.SelectedIndex);
           wnd.Show();
           this.Hide();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        
    }
}
