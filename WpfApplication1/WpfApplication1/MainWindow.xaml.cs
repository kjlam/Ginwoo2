using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.Threading;
using Coding4Fun.Kinect.Wpf;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        private void image5_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void image7_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void image3_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void inkCanvas_Gesture(object sender, InkCanvasGestureEventArgs e)
        {

        }

        private void inkCanvas_Gesture_1(object sender, InkCanvasGestureEventArgs e)
        {

        }

        private void CommitButton_MouseEnter(object sender, MouseEventArgs e)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("pack://application:,,,/WpfApplication1;component/Images/CommitButtonHover.png");
            logo.EndInit();

            CommitButton.Source = logo;
        }

        private void CommitButton_MouseLeave(object sender, MouseEventArgs e)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("pack://application:,,,/WpfApplication1;component/Images/CommitButton.png");
            logo.EndInit();

            CommitButton.Source = logo;
        }

        private void LR_CommitBox_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void image6_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void MenuWorking_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void MenuWorking_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void MenuWorking_MouseLeave(object sender, MouseEventArgs e)
        {

        }

    }
}
