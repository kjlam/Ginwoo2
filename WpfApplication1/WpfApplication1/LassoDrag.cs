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
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Deployment;


namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        String DIRECTORY;                                   // Local directory path in STRING
        int MAX_FILE_DISPLAY_COUNT;                         // Maximum number of files displayed

        FileInfo[] files;                                   // Array to store information of files
        Icon[] icons;                                       // Array to store .ico icon images of files
        System.Windows.Controls.Image[] images;             // Array to store all the WPF image elements for the files icon to be displayed
        System.Windows.Controls.TextBlock[] textBlocks;     // Array to store all the WPF textblock elements for the file name to be displayed
        FileInfo[] selectedFiles;                                   // Files that are selected by the lasso
        int fileCount;                                              // Number of files that are displayed.
        System.Windows.Controls.Image[] selectedImages;             // Selected Images
        System.Windows.Controls.TextBlock[] selectedTextBlocks;     // Selected Textblocks
        List<String> selectedFileNames;
        private System.Windows.Point mouseClick;            // x y coordinate for the mouse pointer click, used in dragging files
        double baseLeft;                                    // the base left coordinate for the image
        double baseTop;                                     // the base right coordinate for the iamge

        double[] deltaLeft;                                 // base left - (minus) respective image left coordinate
        double[] deltaTop;                                  // base left - (minus) respective image left coordinate


        bool imagesFollowMouse;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = textBox;
            selectTimer.Tick += new EventHandler(SelectTimer_Root);
            selectTimer.Interval = new TimeSpan(0, 0, 3);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            waitTimer.Interval = new TimeSpan(0, 0, 1);
            DIRECTORY = @"C:\\Users\\Benj\\Desktop\\CS 160\\Ginect";
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
            WC_inkCanvas.EditingMode = InkCanvasEditingMode.Select;
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

        // METHOD: this method obtains all the selected files by the lasso, and places it in the selectedFiles array
        private void getSelectedFiles()
        {

            //Console.WriteLine("SELECTED");

            // if there are some selected images from before,
            if (selectedImages != null)
            {
                // first start by removing all the previously selected images
                for (int i = 0; i < selectedImages.Count(); i++)
                {
                    WC_inkCanvas.Children.Remove(selectedImages[i]);
                }

            }

            if (selectedTextBlocks != null)
            {
                for (int i = 0; i < selectedTextBlocks.Count(); i++)
                {
                    WC_inkCanvas.Children.Remove(selectedTextBlocks[i]);
                }

            }

            // get all the selected elements into the arraylist
            System.Collections.ObjectModel.ReadOnlyCollection<UIElement> selectedElements = WC_inkCanvas.GetSelectedElements();
            int index = 0;


            // this double loop counts all the images selected
            for (int i = 0; i < selectedElements.Count; i++)
            {
                for (int j = 0; j < fileCount; j++)
                {
                    if (images[j].Equals(selectedElements[i]))
                    {
                        index++;
                    }
                }
            }

            // using the index value, instantiate selectedFiles and selectedImages
            selectedFiles = new FileInfo[index];
            selectedImages = new System.Windows.Controls.Image[index];
            selectedTextBlocks = new TextBlock[index];

            // reset the counter to 0
            index = 0;

            int IMAGE_WIDTH = 40;                       // Width of image?
            int IMAGE_HEIGHT = 40;                      // Height of image?
            double IMAGE_OPACITY_VALUE = 0.5;            // opacity value 1 is full

            // for all the selected elements,
            for (int i = 0; i < selectedElements.Count; i++)
            {

                // and for all the files displayed in the filesystem
                for (int j = 0; j < fileCount; j++)
                {

                    // if the images match up
                    if (images[j].Equals(selectedElements[i]))
                    {

                        // create a fileInfo object and insert it to the selectedFiles arraylist
                        FileInfo insert = files[j];
                        selectedFiles[index] = insert;

                        // get the coordinates of the original image
                        double left = InkCanvas.GetLeft(images[j]);
                        double top = InkCanvas.GetTop(images[j]);

                        //Console.WriteLine(left + " " + top);

                        selectedImages[index] = new System.Windows.Controls.Image();
                        selectedTextBlocks[index] = new TextBlock();

                        // do complicated shit to get the image into bImag
                        MemoryStream ms = new MemoryStream();
                        System.Drawing.Bitmap dImg = (System.Drawing.Icon.ExtractAssociatedIcon(files[j].FullName)).ToBitmap();
                        dImg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        System.Windows.Media.Imaging.BitmapImage bImg = new System.Windows.Media.Imaging.BitmapImage();
                        bImg.BeginInit();
                        bImg.StreamSource = new MemoryStream(ms.ToArray());
                        bImg.EndInit();

                        // set the new image and display it
                        selectedImages[index].Source = bImg;
                        selectedImages[index].Height = IMAGE_HEIGHT;
                        selectedImages[index].Width = IMAGE_WIDTH;

                        selectedImages[index].Opacity = IMAGE_OPACITY_VALUE;

                        //selectedImages[index].PreviewMouseDown += new MouseButtonEventHandler(myimg_MouseDown);
                        //selectedImages[index].PreviewMouseMove += new MouseEventHandler(myimg_MouseMove);
                        //selectedImages[index].PreviewMouseUp += new MouseButtonEventHandler(myimg_MouseUp);

                        selectedFileNames.Add(files[j].Name);

                        selectedTextBlocks[index].Text = files[j].Name;
                        selectedTextBlocks[index].Foreground = new SolidColorBrush(Colors.White);
                        selectedTextBlocks[index].Background = new SolidColorBrush(Colors.Black);
                        selectedTextBlocks[index].Width = IMAGE_WIDTH * 2;
                        selectedTextBlocks[index].TextAlignment = TextAlignment.Center;
                        selectedTextBlocks[index].Opacity = IMAGE_OPACITY_VALUE;


                        System.Windows.Controls.InkCanvas.SetTop(selectedImages[index], top);
                        System.Windows.Controls.InkCanvas.SetLeft(selectedImages[index], left);

                        System.Windows.Controls.InkCanvas.SetTop(selectedTextBlocks[index], top + IMAGE_HEIGHT + 10);
                        System.Windows.Controls.InkCanvas.SetLeft(selectedTextBlocks[index], left - 20);

                        WC_inkCanvas.Children.Add(selectedImages[index]);
                        WC_inkCanvas.Children.Add(selectedTextBlocks[index]);


                        index++;
                    }
                }
            }

            Console.WriteLine("The count of all the selectedfiles is " + selectedFiles.Count());
        }

        private void drag()
        {
            if (imagesFollowMouse == true)
            {
                double CursorLeft = System.Windows.Forms.Cursor.Position.X;
                double CursorTop = System.Windows.Forms.Cursor.Position.Y;

                for (int i = 0; i < selectedImages.Count(); i++)
                {
                    selectedImages[i].SetValue(InkCanvas.LeftProperty, (CursorLeft - (baseLeft - deltaLeft[i])));
                    selectedImages[i].SetValue(InkCanvas.TopProperty, (CursorTop - (baseTop - deltaTop[i])));

                    double SOFUKCINGCOLD = InkCanvas.GetLeft(selectedImages[i]);
                    double IHATETHISPROJ = InkCanvas.GetTop(selectedImages[i]);

                    selectedTextBlocks[i].SetValue(InkCanvas.LeftProperty, SOFUKCINGCOLD - 20);
                    selectedTextBlocks[i].SetValue(InkCanvas.TopProperty, IHATETHISPROJ + 40 + 10);
                }
            }
        }

        private void startDrag()
        {

            Console.WriteLine("Start Drag has been called");

            WC_inkCanvas.EditingMode = InkCanvasEditingMode.None;

            System.Windows.Controls.Image nearestImage = findNearestImage(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);

            //Console.WriteLine("AHHHHHHHHH");

            //((System.Windows.Controls.Image)sender).CaptureMouse();

            //baseTop = InkCanvas.GetTop(((System.Windows.Controls.Image)sender));
            //baseLeft = InkCanvas.GetLeft(((System.Windows.Controls.Image)sender));
            //Console.WriteLine(nearestImage.Name);

            baseTop = InkCanvas.GetTop(nearestImage);
            baseLeft = InkCanvas.GetLeft(nearestImage);

            deltaLeft = new double[selectedImages.Count()];
            deltaTop = new double[selectedImages.Count()];

            for (int i = 0; i < selectedImages.Count(); i++)
            {
                deltaLeft[i] = InkCanvas.GetLeft(selectedImages[i]);
                deltaTop[i] = InkCanvas.GetTop(selectedImages[i]);
            }

            imagesFollowMouse = true;
        }

        private void finishDrag()
        {
            imagesFollowMouse = false;
        }

        // METHOD: this method draws the File System area. It first clears everything on the ink canvas
        private void drawFileSystem()
        {

            // First, Clear everything on the inkCanvas
            WC_inkCanvas.Children.Clear();

            // TOGGLE THESE VALUES FOR DISPLAYING:
            int IMAGE_WIDTH = 40;                       // Width of image?
            int IMAGE_HEIGHT = 40;                      // Height of image?
            int INITIAL_TOP_MARGIN = 100;               // initial top margin of files displayed
            int INITIAL_LEFT_MARGIN = 100;               // initial left margin
            int ROW = 6;                                // How many rows to be displayed?
            int COLUMN = 5;                             // How many columns to be displayed?
            int HORIZONTAL_SPACING = 120;               // horizontal spacing between each icon
            int VERTICAL_SPACING = 100;                 // vertical spacing between each icon

            // for all the files
            for (int i = 0; i < fileCount; i++)
            {
                // do complicated shit to get the image into bImag
                MemoryStream ms = new MemoryStream();
                System.Drawing.Bitmap dImg = icons[i].ToBitmap();
                dImg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                System.Windows.Media.Imaging.BitmapImage bImg = new System.Windows.Media.Imaging.BitmapImage();
                bImg.BeginInit();
                bImg.StreamSource = new MemoryStream(ms.ToArray());
                bImg.EndInit();

                // settings for images
                images[i].Source = bImg;
                images[i].Height = IMAGE_HEIGHT;
                images[i].Width = IMAGE_WIDTH;

                // settings for textblocks
                textBlocks[i].Text = files[i].Name;
                textBlocks[i].Foreground = new SolidColorBrush(Colors.White);
                textBlocks[i].Background = new SolidColorBrush(Colors.Black);
                textBlocks[i].Width = IMAGE_WIDTH * 2;
                textBlocks[i].TextAlignment = TextAlignment.Center;

                // place the image and the textblocks on the inkCanvas
                System.Windows.Controls.InkCanvas.SetTop(images[i], (i / COLUMN) * HORIZONTAL_SPACING + INITIAL_TOP_MARGIN);
                System.Windows.Controls.InkCanvas.SetTop(textBlocks[i], (i / COLUMN) * HORIZONTAL_SPACING + INITIAL_TOP_MARGIN + IMAGE_HEIGHT + 10);

                System.Windows.Controls.InkCanvas.SetLeft(images[i], (i % (ROW - 1)) * VERTICAL_SPACING + INITIAL_LEFT_MARGIN);
                System.Windows.Controls.InkCanvas.SetLeft(textBlocks[i], (i % (ROW - 1)) * VERTICAL_SPACING + INITIAL_LEFT_MARGIN - 20);

                // Add IT! AWW YEAAA
                WC_inkCanvas.Children.Add(images[i]);
                WC_inkCanvas.Children.Add(textBlocks[i]);
            }

        }


        // METHOD: this method draws the commit box area. It first redraws the file system before drawing
        private void drawCommitBox()
        {

            drawFileSystem();

            // TOGGLE THESE VALUES FOR DISPLAYING:
            int IMAGE_WIDTH = 40;                       // Width of image?
            int IMAGE_HEIGHT = 40;                      // Height of image?
            int INITIAL_TOP_MARGIN = 500;               // initial top margin of files displayed
            int INITIAL_LEFT_MARGIN = 700;               // initial left margin
            int ROW = 6;                                // How many rows to be displayed?
            int COLUMN = 5;                             // How many columns to be displayed?
            int HORIZONTAL_SPACING = 120;               // horizontal spacing between each icon
            int VERTICAL_SPACING = 100;                 // vertical spacing between each icon

            int count = selectedFiles.Count();

            Icon[] selectedFilesIcons = new Icon[count];
            System.Windows.Controls.Image[] selectedFileImages = new System.Windows.Controls.Image[count];
            System.Windows.Controls.TextBlock[] selectedFileTextBlocks = new System.Windows.Controls.TextBlock[count];


            for (int i = 0; i < count; i++)
            {
                selectedFileImages[i] = new System.Windows.Controls.Image();
                selectedFileTextBlocks[i] = new TextBlock();
                selectedFilesIcons[i] = System.Drawing.Icon.ExtractAssociatedIcon(selectedFiles[i].FullName);
            }


            // for all the files
            for (int i = 0; i < count; i++)
            {
                // do complicated shit to get the image into bImag
                MemoryStream ms = new MemoryStream();
                System.Drawing.Bitmap dImg = selectedFilesIcons[i].ToBitmap();
                dImg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                System.Windows.Media.Imaging.BitmapImage bImg = new System.Windows.Media.Imaging.BitmapImage();
                bImg.BeginInit();
                bImg.StreamSource = new MemoryStream(ms.ToArray());
                bImg.EndInit();

                // settings for images
                selectedFileImages[i].Source = bImg;
                selectedFileImages[i].Height = IMAGE_HEIGHT;
                selectedFileImages[i].Width = IMAGE_WIDTH;

                // settings for textblocks

                Console.WriteLine(selectedFiles[i].Name);

                selectedFileTextBlocks[i].Text = selectedFiles[i].Name;
                selectedFileTextBlocks[i].Foreground = new SolidColorBrush(Colors.White);
                selectedFileTextBlocks[i].Background = new SolidColorBrush(Colors.Black);
                selectedFileTextBlocks[i].Width = IMAGE_WIDTH * 2;
                selectedFileTextBlocks[i].TextAlignment = TextAlignment.Center;

                // place the image and the textblocks on the inkCanvas
                System.Windows.Controls.InkCanvas.SetTop(selectedFileImages[i], (i / COLUMN) * HORIZONTAL_SPACING + INITIAL_TOP_MARGIN);
                System.Windows.Controls.InkCanvas.SetTop(selectedFileTextBlocks[i], (i / COLUMN) * HORIZONTAL_SPACING + INITIAL_TOP_MARGIN + IMAGE_HEIGHT + 10);

                System.Windows.Controls.InkCanvas.SetLeft(selectedFileImages[i], (i % (ROW - 1)) * VERTICAL_SPACING + INITIAL_LEFT_MARGIN);
                System.Windows.Controls.InkCanvas.SetLeft(selectedFileTextBlocks[i], (i % (ROW - 1)) * VERTICAL_SPACING + INITIAL_LEFT_MARGIN - 20);

                // Add IT! AWW YEAAA
                WC_inkCanvas.Children.Add(selectedFileImages[i]);
                WC_inkCanvas.Children.Add(selectedFileTextBlocks[i]);
            }

        }

        // METHOD
        private void setupDrag()
        {


        }

        //TODO: instead of having mouse drag, just have the images follow the mouse/cursor around , not sure if myImg_MouseUp needed now 
        void myimg_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((System.Windows.Controls.Image)sender).ReleaseMouseCapture();

        }

        void myimg_MouseMove(object sender, MouseEventArgs e)
        {

           // if (((System.Windows.Controls.Image)sender).IsMouseCaptured)
            //{
                //System.Windows.Point mouseCurrent = e.GetPosition(null);
                //double Left = mouseCurrent.X;
                //double Top = mouseCurrent.Y;

            if ((deltaTop[0] != -1) && (deltaLeft[0] != -1))
            {

                double CursorLeft = System.Windows.Forms.Cursor.Position.X;
                double CursorTop = System.Windows.Forms.Cursor.Position.Y;

                for (int i = 0; i < selectedImages.Count(); i++)
                {

                    selectedImages[i].SetValue(InkCanvas.LeftProperty, (CursorLeft - (baseLeft - deltaLeft[i])));
                    selectedImages[i].SetValue(InkCanvas.TopProperty, (CursorTop - (baseTop - deltaTop[i])));

                    double SOFUKCINGCOLD = InkCanvas.GetLeft(selectedImages[i]);
                    double IHATETHISPROJ = InkCanvas.GetTop(selectedImages[i]);

                    selectedTextBlocks[i].SetValue(InkCanvas.LeftProperty, SOFUKCINGCOLD - 20);
                    selectedTextBlocks[i].SetValue(InkCanvas.TopProperty, IHATETHISPROJ + 40 + 10);
                }

            }
           // }
        }


        //void myimg_MouseDown()
        void myimg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //mouseClick = e.GetPosition(null);

            //Console.WriteLine("HELLO");
            System.Windows.Controls.Image nearestImage = findNearestImage(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);

            //Console.WriteLine("AHHHHHHHHH");

            //((System.Windows.Controls.Image)sender).CaptureMouse();

            //baseTop = InkCanvas.GetTop(((System.Windows.Controls.Image)sender));
            //baseLeft = InkCanvas.GetLeft(((System.Windows.Controls.Image)sender));
            Console.WriteLine(nearestImage.Name);
            
            baseTop = InkCanvas.GetTop(nearestImage);
            baseLeft = InkCanvas.GetLeft(nearestImage);


            deltaLeft = new double[selectedImages.Count()];
            deltaTop = new double[selectedImages.Count()];

            for (int i = 0; i < selectedImages.Count(); i++)
            {
                deltaLeft[i] = InkCanvas.GetLeft(selectedImages[i]);
                deltaTop[i] = InkCanvas.GetTop(selectedImages[i]);
            }
        }

        //TODO: delete old list of files when calling method again;
        private void drawLocalRepository()
        {
            CmdReturn localRepositoryList = Terminal.GitGetLocalRepoFiles();
            //TODO: uncomment to enable git file list 
            List<String> lrList = localRepositoryList.fileList;
           // Console.WriteLine(lrList[0]);
            LR_inkCanvas.Children.Clear();

            
            /*
             * List<String> lrList = new List<string>();

            lrList.Add("one.cs");
            lrList.Add("two.pdf");
            lrList.Add("three.doc");
            lrList.Add("four.txt");
            lrList.Add("five.psp");
            lrList.Add("six.ppt");
            lrList.Add("seven.docx");
            lrList.Add("eight.txt");
            */
            

            int count = lrList.Count;

            System.Windows.Controls.Image[] images = new System.Windows.Controls.Image[count];
            System.Windows.Controls.TextBlock[] textBlocks = new TextBlock[count];

            for (int i = 0; i < count; i++)
            {
                images[i] = new System.Windows.Controls.Image();
                textBlocks[i] = new TextBlock();
            }
            



            // TOGGLE THESE VALUES FOR DISPLAYING:
            int IMAGE_WIDTH = 40;                       // Width of image?
            int IMAGE_HEIGHT = 40;                      // Height of image?
            int INITIAL_TOP_MARGIN = 100;               // initial top margin of files displayed
            int INITIAL_LEFT_MARGIN = 100;               // initial left margin
            int ROW = 6;                                // How many rows to be displayed?
            int COLUMN = 5;                             // How many columns to be displayed?
            int HORIZONTAL_SPACING = 120;               // horizontal spacing between each icon
            int VERTICAL_SPACING = 100;                 // vertical spacing between each icon

            // for all the files
            for (int i = 0; i < count; i++)
            {

                String filename = lrList[i];
                FileInfo temp = new FileInfo(filename);
                String extension = temp.Extension.ToString();

                //Console.WriteLine(filename + " " + extension);

                Icon icon = IconReader.GetFileIcon(extension, IconReader.IconSize.Large, false);

                // do complicated shit to get the image into bImag
                MemoryStream ms = new MemoryStream();
                System.Drawing.Bitmap dImg = icon.ToBitmap();
                dImg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                System.Windows.Media.Imaging.BitmapImage bImg = new System.Windows.Media.Imaging.BitmapImage();
                bImg.BeginInit();
                bImg.StreamSource = new MemoryStream(ms.ToArray());
                bImg.EndInit();

                // settings for images
                images[i].Source = bImg;
                images[i].Height = IMAGE_HEIGHT;
                images[i].Width = IMAGE_WIDTH;

                // settings for textblocks
                textBlocks[i].Text = filename;
                textBlocks[i].Foreground = new SolidColorBrush(Colors.White);
                textBlocks[i].Background = new SolidColorBrush(Colors.Black);
                textBlocks[i].Width = IMAGE_WIDTH * 2;
                textBlocks[i].TextAlignment = TextAlignment.Center;

                // place the image and the textblocks on the inkCanvas
                System.Windows.Controls.InkCanvas.SetTop(images[i], (i / COLUMN) * HORIZONTAL_SPACING + INITIAL_TOP_MARGIN);
                System.Windows.Controls.InkCanvas.SetTop(textBlocks[i], (i / COLUMN) * HORIZONTAL_SPACING + INITIAL_TOP_MARGIN + IMAGE_HEIGHT + 10);

                System.Windows.Controls.InkCanvas.SetLeft(images[i], (i % (ROW - 1)) * VERTICAL_SPACING + INITIAL_LEFT_MARGIN);
                System.Windows.Controls.InkCanvas.SetLeft(textBlocks[i], (i % (ROW - 1)) * VERTICAL_SPACING + INITIAL_LEFT_MARGIN - 20);

                // Add IT! AWW YEAAA
                LR_inkCanvas.Children.Add(images[i]);
                LR_inkCanvas.Children.Add(textBlocks[i]);
            }

        }

        private void drawRemoteRepository()
        {
            CmdReturn remoteRepositoryList = Terminal.GitGetRemoteRepoFiles();
            List<String> rrList = remoteRepositoryList.fileList;
            RR_inkCanvas.Children.Clear();

            //List<String> rrList = new List<string>();


            /*
            rrList.Add("one.cs");
            rrList.Add("two.pdf");
            rrList.Add("three.doc");
            rrList.Add("four.txt");
            rrList.Add("five.psp");
            rrList.Add("six.ppt");
            rrList.Add("seven.docx");
            rrList.Add("eight.txt");

             * 
             * */

            int count = rrList.Count;

            System.Windows.Controls.Image[] images = new System.Windows.Controls.Image[count];
            System.Windows.Controls.TextBlock[] textBlocks = new TextBlock[count];

            for (int i = 0; i < count; i++)
            {
                images[i] = new System.Windows.Controls.Image();
                textBlocks[i] = new TextBlock();
            }




            // TOGGLE THESE VALUES FOR DISPLAYING:
            int IMAGE_WIDTH = 40;                       // Width of image?
            int IMAGE_HEIGHT = 40;                      // Height of image?
            int INITIAL_TOP_MARGIN = 100;               // initial top margin of files displayed
            int INITIAL_LEFT_MARGIN = 100;               // initial left margin
            int ROW = 6;                                // How many rows to be displayed?
            int COLUMN = 5;                             // How many columns to be displayed?
            int HORIZONTAL_SPACING = 120;               // horizontal spacing between each icon
            int VERTICAL_SPACING = 100;                 // vertical spacing between each icon

            // for all the files
            for (int i = 0; i < count; i++)
            {

                String filename = rrList[i];
                FileInfo temp = new FileInfo(filename);
                String extension = temp.Extension.ToString();

                //Console.WriteLine(filename + " " + extension);

                Icon icon = IconReader.GetFileIcon(extension, IconReader.IconSize.Large, false);

                // do complicated shit to get the image into bImag
                MemoryStream ms = new MemoryStream();
                System.Drawing.Bitmap dImg = icon.ToBitmap();
                dImg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                System.Windows.Media.Imaging.BitmapImage bImg = new System.Windows.Media.Imaging.BitmapImage();
                bImg.BeginInit();
                bImg.StreamSource = new MemoryStream(ms.ToArray());
                bImg.EndInit();

                // settings for images
                images[i].Source = bImg;
                images[i].Height = IMAGE_HEIGHT;
                images[i].Width = IMAGE_WIDTH;

                // settings for textblocks
                textBlocks[i].Text = filename;
                textBlocks[i].Foreground = new SolidColorBrush(Colors.White);
                textBlocks[i].Background = new SolidColorBrush(Colors.Black);
                textBlocks[i].Width = IMAGE_WIDTH * 2;
                textBlocks[i].TextAlignment = TextAlignment.Center;

                // place the image and the textblocks on the inkCanvas
                System.Windows.Controls.InkCanvas.SetTop(images[i], (i / COLUMN) * HORIZONTAL_SPACING + INITIAL_TOP_MARGIN);
                System.Windows.Controls.InkCanvas.SetTop(textBlocks[i], (i / COLUMN) * HORIZONTAL_SPACING + INITIAL_TOP_MARGIN + IMAGE_HEIGHT + 10);

                System.Windows.Controls.InkCanvas.SetLeft(images[i], (i % (ROW - 1)) * VERTICAL_SPACING + INITIAL_LEFT_MARGIN);
                System.Windows.Controls.InkCanvas.SetLeft(textBlocks[i], (i % (ROW - 1)) * VERTICAL_SPACING + INITIAL_LEFT_MARGIN - 20);

                // Add IT! AWW YEAAA
                RR_inkCanvas.Children.Add(images[i]);
                RR_inkCanvas.Children.Add(textBlocks[i]);

            }
        }

        /* METHOD: Parameters:  x coordinate
         *                      y coordinate 
         * 
         * Returns: The nearest image from the specified coordiate
         */
        private System.Windows.Controls.Image findNearestImage(double x, double y)
        {
            System.Windows.Controls.Image output = new System.Windows.Controls.Image();
            double currentBest = -1;

            for (int i = 0; i < selectedImages.Count(); i++)
            {
                double left = InkCanvas.GetLeft(selectedImages[i]);
                double top = InkCanvas.GetTop(selectedImages[i]);

                double distance = Math.Sqrt(Math.Pow(left - x, 2) + Math.Pow(top - y, 2));

                if (currentBest == -1)
                {
                    currentBest = distance;
                    output = selectedImages[i];
                }
                else if (distance < currentBest)
                {
                    currentBest = distance;
                    output = selectedImages[i];
                }
            }
            return output;
        }


        private void button4_Click(object sender, RoutedEventArgs e)
        {
            getSelectedFiles();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            WC_inkCanvas.EditingMode = InkCanvasEditingMode.None;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            WC_inkCanvas.EditingMode = InkCanvasEditingMode.Select;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            drawCommitBox();
        }


 


    }

}