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
        bool JustPushed = true;

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
        private void ContinueButton_MouseEnter(object sender, MouseEventArgs e) 
        {
            ContinueButton.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/CommitButtonHover.png");
        }

        private void ContinueButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ContinueButton.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/CommitButton.png");
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
            //HelpMePleaseHelpHelp.Visibility = Visibility.Visible;
            //WC_HelpIcon.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/HelpIcon.png");
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
            LoadingScreen.Visibility = Visibility.Visible;
            waitTimer.Start();
        }

        private void LR_MenuLocal_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            switchToWorkingCopy();
            drawFileSystem();
        }

        private void RR_MenuRemote_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            switchToLocalRepository();
            if (JustPushed)
            {
                PushedText.Visibility = Visibility.Collapsed;
                NoAddedFilesText.Visibility = Visibility.Visible;
                TaggedText.Visibility = Visibility.Collapsed;
                CommitedText.Visibility = Visibility.Collapsed;
                AddedFilesText.Visibility = Visibility.Collapsed;
                JustPushed = false;
            }
            //todo rename to RR_Menu
            drawLocalRepository();

            
        }

        private void CommitButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //TODO: figure out wut message to put in git commit, maybe just number counter, resets status message in working 
            Terminal myTerminal = new Terminal();
            myTerminal.GitCommitWithMessage("Default Message");
            switchToLocalRepository();
            drawLocalRepository();
            AddedFilesText.Visibility = Visibility.Collapsed;
            NoAddedFilesText.Visibility = Visibility.Collapsed;
            PushedText.Visibility = Visibility.Collapsed;
            CommitedText.Visibility = Visibility.Visible;
        }

        private void LR_PushButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Terminal myTerminal = new Terminal();
            myTerminal.GitPush();
            waitTimer.Start();
            TaggedText.Visibility = Visibility.Collapsed;
            CommitedText.Visibility = Visibility.Collapsed;
            AddedFilesText.Visibility = Visibility.Collapsed;
            NoAddedFilesText.Visibility = Visibility.Collapsed;
            PushedText.Visibility = Visibility.Visible;
            JustPushed = true;

        }

        private void LR_TagButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //TODO: tag icon image needed, tag icon pops up, speech recognition starts
            showTagSection();
            tagIconActivated = true;

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
            TagRectangle.Visibility = Visibility.Visible;
            TagBlock1.Visibility = Visibility.Visible;
            TagNameTextBlock.Visibility = Visibility.Visible;
            TagLabel.Visibility = Visibility.Visible;
            TagLabel2.Visibility = Visibility.Visible;
        }

        private void hideTagSection()
        {
            TagRectangle.Visibility = Visibility.Collapsed;
            TagBlock1.Visibility = Visibility.Collapsed;
            TagNameTextBlock.Visibility = Visibility.Collapsed;
            TagLabel.Visibility = Visibility.Collapsed;
            TagLabel2.Visibility = Visibility.Collapsed;
            
        }

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


        /*
         * CursorInCommitBox()
         * 
         * Checks if cursor is in commitbox area, if so then drawcommitbox and add files to commit
        */
        private void CursorInCommitBoxZone()
        {
            if (System.Windows.Forms.Cursor.Position.X >= WC_CommitBox.Margin.Left && System.Windows.Forms.Cursor.Position.X <= (WC_CommitBox.Margin.Left + WC_CommitBox.Width))
            {
                if (System.Windows.Forms.Cursor.Position.Y <= WC_CommitBox.Margin.Top && System.Windows.Forms.Cursor.Position.Y >= (WC_CommitBox.Margin.Top - WC_CommitBox.Height))
                {
                    Console.WriteLine("cursor in commitbox zone X: " + System.Windows.Forms.Cursor.Position.X + " Y: " + System.Windows.Forms.Cursor.Position.Y);
                    drawCommitBox();
                    //TODO: make sure commit works
                    Terminal myTerminal = new Terminal();
                    myTerminal.GitAddFilesToCommit(selectedFileNames);
                    AddedFilesText.Visibility = Visibility.Visible;
                    NoAddedFilesText.Visibility = Visibility.Collapsed;
                    TaggedText.Visibility = Visibility.Collapsed;
                    CommitedText.Visibility = Visibility.Collapsed;
                    PushedText.Visibility = Visibility.Collapsed;
                    mouseLeftClick();
                    lassoFilesDragging = false;
                    finishDrag();
                }
            }
        }

        /*
         * CursorInTrashZone()
         * 
         * Checks if cursor is in trash area, if so then remove files from commit and remove files from directory
        */
        private void CursorInTrashZone()
        {
            if (System.Windows.Forms.Cursor.Position.X >= TrashCan.Margin.Left && System.Windows.Forms.Cursor.Position.X <= (TrashCan.Margin.Left + TrashCan.Width))
            {
                if (System.Windows.Forms.Cursor.Position.Y <= TrashCan.Margin.Top && System.Windows.Forms.Cursor.Position.Y >= (TrashCan.Margin.Top - TrashCan.Height))
                {
                    Console.WriteLine("cursor in Trash zone X: " + System.Windows.Forms.Cursor.Position.X + " Y: " + System.Windows.Forms.Cursor.Position.Y);
      
                    //TODO: make sure remove works
                    Terminal myTerminal = new Terminal(); 
                    myTerminal.GitRemoveFiles(selectedFileNames);

                    // TODO: git remove status message

                    //AddedFilesText.Visibility = Visibility.Visible;
                    //NoAddedFilesText.Visibility = Visibility.Collapsed;
                    //TaggedText.Visibility = Visibility.Collapsed;
                    //CommitedText.Visibility = Visibility.Collapsed;
                    //PushedText.Visibility = Visibility.Collapsed;
                    mouseLeftClick();
                    lassoFilesDragging = false;
                    finishDrag();
                    drawFileSystem();
                }
            }
        }

        private void HelpIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            //HelpMePleaseHelpHelp.Visibility = Visibility.Visible;

        }

        private void HelpMePleaseHelpHelp_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            HelpMePleaseHelpHelp.Visibility = Visibility.Collapsed;
        }

        private void ContinueButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string username = inputUsername.Text;
            string password = inputPassword.Password;
            string path = inputPath.Text;
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"temp.txt");
            file.WriteLine(path);
            file.WriteLine(username);
            file.WriteLine(password);
            file.Close();
            path = path.Replace("/", @"\\");
            string configpath = path + @"\\.git\\config";
            System.IO.StreamReader file2 = new System.IO.StreamReader(configpath);
            string text = file2.ReadToEnd();
            text = text.Replace("git@github.com", "https://" + username + ":" + password + "@github.com");
            file2.Close();
            file = new System.IO.StreamWriter(configpath);
            file.Write(text);
            file.Close();
            Config.Visibility = Visibility.Collapsed;
            NoAddedFilesText.Visibility = Visibility.Visible;
            switchToWorkingCopy();

            DIRECTORY = path;
            file.Close();

            MAX_FILE_DISPLAY_COUNT = 16;
            selectedFileNames = new List<string>();
            // get the files into the arraylist from the directory info
            DirectoryInfo directoryInfo = new DirectoryInfo(DIRECTORY);
            files = directoryInfo.GetFiles();
            fileCount = files.Length;
            icons = new Icon[fileCount];

            //deltaLeft = new double[1];
            //deltaTop = new double[1];

            //deltaLeft[0] = -1.0;
            //deltaTop[0] = -1.0;

            imagesFollowMouse = false;

            /*
            // inkCanvas is initially set to select mode that allows lassoing
            WC_inkCanvas.EditingMode = 
             * InkCanvasEditingMode.Select;
            */

            WC_inkCanvas.EditingMode = InkCanvasEditingMode.None;

            // initializethe images and textBlocks arraylist to contain the respective items
            images = new System.Windows.Controls.Image[fileCount];
            textBlocks = new System.Windows.Controls.TextBlock[fileCount];
            for (int i = 0; i < fileCount; i++)
            {
                images[i] = new System.Windows.Controls.Image();
                textBlocks[i] = new TextBlock();
            }

            // extract the icons from the files arraylist
            for (int i = 0; i < fileCount; i++)
            {
                icons[i] = System.Drawing.Icon.ExtractAssociatedIcon(files[i].FullName);
            }

            // draw the file system
            drawFileSystem();

        }
        private void Cursor_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void WC_Refresh_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            drawFileSystem();
        }

        private void LR_Refresh_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            drawLocalRepository();
        }

        private void RR_Refresh_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoadingScreen.Visibility = Visibility.Visible;
            waitTimer.Start();
        }

        private void RR_Refresh_MouseEnter(object sender, MouseEventArgs e)
        {
            RR_Refresh.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/refresh_blue-hover.png");
        }

         private void RR_Refresh_MouseLeave(object sender, MouseEventArgs e)
        {
            RR_Refresh.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/refresh_blue.png");
        }

        private void LR_Refresh_MouseEnter(object sender, MouseEventArgs e)
        {
            LR_Refresh.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/refresh_blue-hover.png");
        }

         private void LR_Refresh_MouseLeave(object sender, MouseEventArgs e)
        {
            LR_Refresh.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/refresh_blue.png");
        }

         private void WC_Refresh_MouseEnter(object sender, MouseEventArgs e)
        {
            WC_Refresh.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/refresh_blue-hover.png");
        }

         private void WC_Refresh_MouseLeave(object sender, MouseEventArgs e)
        {
            WC_Refresh.Source = LoadImage("pack://application:,,,/WpfApplication1;component/Images/refresh_blue.png");
        }





        
    }
}
