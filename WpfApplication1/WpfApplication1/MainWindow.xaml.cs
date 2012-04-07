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
using System.Drawing;
using System.IO;

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

        private void WC_CommitBox_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        //Menu navigation: LeftClick represents push, RightClick represents pull

        private void MenuWorking_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WorkingCopy.Visibility = Visibility.Collapsed;
            LocalRepository.Visibility = Visibility.Visible;
            drawLocalRepository();
        }

        private void LR_MenuLocal_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LocalRepository.Visibility = Visibility.Collapsed;
            RemoteRepository.Visibility = Visibility.Visible;
            drawRemoteRepository();
        }

        private void LR_MenuLocal_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            LocalRepository.Visibility = Visibility.Collapsed;
            WorkingCopy.Visibility = Visibility.Visible;
            drawFileSystem();
        }

        private void image6_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            RemoteRepository.Visibility = Visibility.Collapsed;
            LocalRepository.Visibility = Visibility.Visible;
            //todo rename to RR_Menu
            drawLocalRepository();
            
        }

        private void CommitButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //TODO: figure out wut message to put in git commit, maybe just number counter
            Terminal.GitCommitWithMessage("Trial");
        }

        private void LR_PushButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Terminal.GitPush();
        }

        private void LR_TagButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //TODO: tag icon image needed, tag icon pops up, speech recognition starts
            //Terminal.GitTagLatestCommit(tagName);
        }

        //TODO: git pull figure out area of fie system and pull



    }
}
