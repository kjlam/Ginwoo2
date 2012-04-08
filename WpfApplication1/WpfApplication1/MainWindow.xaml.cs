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

        #region HoverImages
        private BitmapImage LoadImage(String s)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(s);
            logo.EndInit();

            return logo;
        }

        private void CommitButton_MouseEnter(object sender, MouseEventArgs e)
        {
            CommitButton.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/CommitButtonHover.png");
        }

        private void CommitButton_MouseLeave(object sender, MouseEventArgs e)
        {
            CommitButton.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/CommitButton.png");
        }

        private void LR_PushButton_MouseEnter(object sender, MouseEventArgs e)
        {
            LR_PushButton.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/PushButtonHover.png");
        }

        private void LR_PushButton_MouseLeave(object sender, MouseEventArgs e)
        {
            LR_PushButton.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/PushButton.png");
        }


        private void LR_TagButton_MouseEnter(object sender, MouseEventArgs e)
        {
            LR_TagButton.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/TagButtonHover.png");
        }

        private void LR_TagButton_MouseLeave(object sender, MouseEventArgs e)
        {
            LR_TagButton.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/TagButton.png");
        }

        private void WC_HelpIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            WC_HelpIcon.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/HelpIconHover.png");
        }

        private void WC_HelpIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            WC_HelpIcon.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/HelpIcon.png");
        }

        private void LR_HelpIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            LR_HelpIcon.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/HelpIconHover.png");
        }

        private void LR_HelpIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            LR_HelpIcon.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/HelpIcon.png");
        }

        private void RR_HelpIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            RR_HelpIcon.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/HelpIconHover.png");
        }

        private void RR_HelpIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            RR_HelpIcon.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/HelpIcon.png");
        }


        private void MenuWorking_MouseEnter_1(object sender, MouseEventArgs e)
        {
            MenuWorking.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/MenuHover.png");
        }


        private void MenuWorking_MouseLeave_1(object sender, MouseEventArgs e)
        {
            MenuWorking.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/MenuWorking.png");
        }



        private void RR_MenuRemote_MouseEnter(object sender, MouseEventArgs e)
        {
            RR_MenuRemote.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/MenuHover.png");
        }

        private void RR_MenuRemote_MouseLeave(object sender, MouseEventArgs e)
        {
            RR_MenuRemote.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/MenuRemote.png");
        }

        private void LR_MenuLocal_MouseEnter(object sender, MouseEventArgs e)
        {
            LR_MenuLocal.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/MenuHover.png");
        }

        private void LR_MenuLocal_MouseLeave(object sender, MouseEventArgs e)
        {
            LR_MenuLocal.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/MenuLocal.png");
        }



        private void LR_CommitBox_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        #endregion
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
            switchToLocalRepository();
            drawLocalRepository();
        }

        private void LR_MenuLocal_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            switchToRemoteRepository();
            drawRemoteRepository();
        }

        private void LR_MenuLocal_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            switchToWorkingCopy();
            drawFileSystem();
        }

        private void RR_MenuRemote_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            switchToLocalRepository();
            //todo rename to RR_Menu
            drawLocalRepository();
            
        }

        private void CommitButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //TODO: figure out wut message to put in git commit, maybe just number counter
            Terminal.GitCommitWithMessage("Default Message");
        }

        private void LR_PushButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Terminal.GitPush();
        }

        private void LR_TagButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //TODO: tag icon image needed, tag icon pops up, speech recognition starts
            showTagSection();
            tagIconActivated = true;
            Terminal.GitTagLatestCommit(tagName);
            hideTagSection();
        }

        private void switchToWorkingCopy()
        {
            WorkingCopy.Visibility = Visibility.Visible;
            LocalRepository.Visibility = Visibility.Collapsed;
            RemoteRepository.Visibility = Visibility.Collapsed;
        }
        private void switchToLocalRepository()
        {
            WorkingCopy.Visibility = Visibility.Collapsed;
            LocalRepository.Visibility = Visibility.Visible;
            RemoteRepository.Visibility = Visibility.Collapsed;
        }

        private void switchToRemoteRepository()
        {
            WorkingCopy.Visibility = Visibility.Collapsed;
            LocalRepository.Visibility = Visibility.Collapsed;
            RemoteRepository.Visibility = Visibility.Visible;
        }

        private void showTagSection()
        {
            TagBlock1.Visibility = Visibility.Visible;
            TagBlock2.Visibility = Visibility.Visible;
            TagLabel.Visibility = Visibility.Visible;
            TagLabel2.Visibility = Visibility.Visible;
        }

        private void hideTagSection()
        {
            TagBlock1.Visibility = Visibility.Collapsed;
            TagBlock2.Visibility = Visibility.Collapsed;
            TagLabel.Visibility = Visibility.Collapsed;
            TagLabel2.Visibility = Visibility.Collapsed;
            
        }

        //TODO: git pull figure out areas of directory system, switch to working mode
        /*
         * CursorInDirectoryArea()
         * 
         *Checks if cursor is in the directory area, if so return true, else return false
         */
        private bool CursorInDirectoryArea()
        {
            if (RHPos[0] > WC_Directory.Margin.Left && RHPos[0] < (WC_Directory.Margin.Left + WC_Directory.Width))
            {
                if (RHPos[1] < WC_Directory.Margin.Top && RHPos[1] > (WC_Directory.Margin.Top - WC_Directory.Height))
                    return true;
            }
                return false;

        }


        
    }
}
