using System;
using System.Runtime.InteropServices; 
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
using System.Windows.Forms;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.Drawing;
using System.Threading;
using Coding4Fun.Kinect.Wpf;
using System.Diagnostics;


namespace WpfApplication1
{
    public class GestureText : INotifyPropertyChanged
    {
        string gesture = "Gesture: ";
        string rightPos = "RH: ";
        string leftPos = "LH: ";
        string headPos = "RH: ";
        public event PropertyChangedEventHandler PropertyChanged;

        public string Gesture
        {
            get { return gesture; }
            set
            {
                gesture = value;
                OnPropertyChanged("Gesture");
            }
        }

        public string RightPos
        {
            get { return rightPos; }
            set
            {
                rightPos = value;
                OnPropertyChanged("RightPos");
            }
        }

        public string LeftPos
        {
            get { return leftPos; }
            set
            {
                leftPos = value;
                OnPropertyChanged("LeftPos");
            }
        }

        public string HeadPos
        {
            get { return headPos; }
            set
            {
                headPos = value;
                OnPropertyChanged("HeadPos");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public partial class MainWindow : Window
    {
        
        GestureText textBox = new GestureText();

        DispatcherTimer waitTimer = new System.Windows.Threading.DispatcherTimer();
        private DispatcherTimer readyTimer;
        DispatcherTimer selectTimer = new System.Windows.Threading.DispatcherTimer();
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private const double AngleChangeSmoothingFactor = 0.35;
        private const string AcceptedSpeechPrefix = "Accepted_";
        private const string RejectedSpeechPrefix = "Rejected_";
        private KinectSensor kinect;
        private double angle;
        private SpeechRecognitionEngine speechRecognizer;
        bool closing = false;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        List<float[]> storedSkeletonValues = new List<float[]>();
        bool push = false;
        bool pushable = true;
        bool back = false;
        bool actionWait = false;
        double[] RHPos = new double[2];
        float[] RHSensitivity = new float[2]{0.3f,0.3f};
        bool selectActivated = false;
        bool tagIconActivated = false;
        private string tagName = "";
        private bool lassoFilesDragging = false;

    #region mouseEmulation
    [DllImport("user32.dll")]
    static extern IntPtr GetMessageExtraInfo();

    [DllImport("user32.dll", SetLastError = true)]
    static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    [Flags]
    public enum MouseEventFlags
    {
        LEFTDOWN = 0x00000002,
        LEFTUP = 0x00000004,
        MIDDLEDOWN = 0x00000020,
        MIDDLEUP = 0x00000040,
        MOVE = 0x00000001,
        ABSOLUTE = 0x00008000,
        RIGHTDOWN = 0x00000008,
        RIGHTUP = 0x00000010
    }

    /// <summary>
    /// The event type contained in the union field
    /// </summary>
    enum SendInputEventType : int
    {
        /// <summary>
        /// Contains Mouse event data
        /// </summary>
        InputMouse
    }

    /// <summary>
    /// The mouse data structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct MouseInputData
    {
        /// <summary>
        /// The x value, if ABSOLUTE is passed in the flag then this is an actual X and Y value
        /// otherwise it is a delta from the last position
        /// </summary>
        public int dx;
        /// <summary>
        /// The y value, if ABSOLUTE is passed in the flag then this is an actual X and Y value
        /// otherwise it is a delta from the last position
        /// </summary>
        public int dy;
        /// <summary>
        /// Wheel event data, X buttons
        /// </summary>
        public uint mouseData;
        /// <summary>
        /// ORable field with the various flags about buttons and nature of event
        /// </summary>
        public MouseEventFlags dwFlags;
        /// <summary>
        /// The timestamp for the event, if zero then the system will provide
        /// </summary>
        public uint time;
        /// <summary>
        /// Additional data obtained by calling app via GetMessageExtraInfo
        /// </summary>
        public IntPtr dwExtraInfo;
    }



    /// <summary>
    /// The Data passed to SendInput in an array.
    /// </summary>
    /// <remarks>Contains a union field type specifies what it contains </remarks>
    [StructLayout(LayoutKind.Sequential)]
    struct INPUT
    {
        /// <summary>
        /// The actual data type contained in the union Field
        /// </summary>
        public SendInputEventType type;
        public MouseInputData mi;
    }

        #endregion



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
        }



        void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            KinectSensor old = (KinectSensor)e.OldValue;

            StopKinect(old);

            KinectSensor sensor = (KinectSensor)e.NewValue;
            this.kinect = sensor;

            if (sensor == null && speechRecognizer == null)
            {
                return;
            }




            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.3f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 0.5f
            };
             sensor.SkeletonStream.Enable(parameters);

            //sensor.SkeletonStream.Enable();

            this.speechRecognizer = this.CreateSpeechRecognizer();



            this.readyTimer = new DispatcherTimer();
            this.readyTimer.Tick += this.ReadyTimerTick;
            this.readyTimer.Interval = new TimeSpan(0, 0, 4);
            this.readyTimer.Start();



            sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);
            sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            try
            {
                sensor.Start();
            }
            catch (System.IO.IOException)
            {
                kinectSensorChooser1.AppConflictOccurred();
            }
        }

        private void ReadyTimerTick(object sender, EventArgs e)
        {
            this.Start();
            //statusLabel.Content = "Ready to recognize speech!";
            this.readyTimer.Stop();
            this.readyTimer = null;
        }

        private void Start()
        {
            var audioSource = this.kinect.AudioSource;
            audioSource.BeamAngleMode = BeamAngleMode.Adaptive;

            // This should be off by default, but just to be explicit, this MUST be set to false.
            audioSource.AutomaticGainControlEnabled = false;

            var kinectStream = audioSource.Start();
            this.speechRecognizer.SetInputToAudioStream(
                kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            // Keep recognizing speech until window closes
            this.speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
        }


        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (closing)
            {
                return;
            }

            //Get a skeleton
            Skeleton first = GetFirstSkeleton(e);

            if (first == null)
            {
                return;
            }

            //set scaled position
            //ScalePosition(headImage, first.Joints[JointType.Head]);
            //ScalePosition(leftEllipse, first.Joints[JointType.HandLeft]);
            ScalePosition(Cursor, first.Joints[JointType.HandRight]);
            textBox.Gesture = "Gesture: ";
            using (DepthImageFrame depth = e.OpenDepthImageFrame())
            {
                if (depth == null ||
                    kinectSensorChooser1.Kinect == null)
                {
                    return;
                }
                float[] skeletonValues = new float[9];
                DepthImagePoint leftSkeletalPoint2 = depth.MapFromSkeletonPoint(first.Joints[JointType.HandLeft].Position);
                DepthImagePoint rightSkeletalPoint2 = depth.MapFromSkeletonPoint(first.Joints[JointType.HandRight].Position);
                DepthImagePoint headDepthPoint = depth.MapFromSkeletonPoint(first.Joints[JointType.Head].Position);
                skeletonValues[0] = leftSkeletalPoint2.X;
                skeletonValues[1] = leftSkeletalPoint2.Y;
                skeletonValues[2] = leftSkeletalPoint2.Depth;
                skeletonValues[3] = rightSkeletalPoint2.X;
                skeletonValues[4] = rightSkeletalPoint2.Y;
                skeletonValues[5] = rightSkeletalPoint2.Depth;
                skeletonValues[6] = headDepthPoint.X;
                skeletonValues[7] = headDepthPoint.Y;
                skeletonValues[8] = headDepthPoint.Depth;
                storedSkeletonValues.Add(skeletonValues);
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)RHPos[0], (int)RHPos[1]);
                if (!actionWait)
                {
                    CheckSwipe(e);
                    CheckStatic(e);
                    if (lassoFilesDragging)
                    {
                        CursorInCommitBoxZone();
                    }
                }
            }
            GetCameraPoint(first, e);
            /*if (selectActivated)
            {
                FollowPointer();
            }
             * */

            FollowPointer();
        }

        void SelectTimer_Root(object sender, EventArgs e)
        {
            //TODO: Do somethign as user has started lasso
            actionWait = false;

            if (selectTimer.Interval == new TimeSpan(0, 0, 1))
            {
                
                selectTimer.Stop();
            }
        }

        #region Speech recognizer setup
        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        private SpeechRecognitionEngine CreateSpeechRecognizer()
        {
            #region Initialization
            RecognizerInfo ri = GetKinectRecognizer();
            if (ri == null)
            {
                System.Windows.MessageBox.Show(
                    @"There was a problem initializing Speech Recognition.
Ensure you have the Microsoft Speech SDK installed.",
                    "Failed to load Speech SDK",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                this.Close();
                return null;
            }

            SpeechRecognitionEngine sre;
            try
            {
                sre = new SpeechRecognitionEngine(ri.Id);
            }
            catch
            {
                System.Windows.MessageBox.Show(
                    @"There was a problem initializing Speech Recognition.
Ensure you have the Microsoft Speech SDK installed and configured.",
                    "Failed to load Speech SDK",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                this.Close();
                return null;
            }
            #endregion

            //TODO: build more robust grammar, maybe just spell out letters
            #region Build grammar
            var wordChoices = new Choices();
            wordChoices.Add("hello world");
            wordChoices.Add("a");
            wordChoices.Add("b");
            wordChoices.Add("c");
            wordChoices.Add("d");
            wordChoices.Add("e");
            wordChoices.Add("f");
            wordChoices.Add("g");
            wordChoices.Add("h");
            wordChoices.Add("i");
            wordChoices.Add("j");
            wordChoices.Add("k");
            wordChoices.Add("l");
            wordChoices.Add("m");
            wordChoices.Add("n");
            wordChoices.Add("o");
            wordChoices.Add("p");
            wordChoices.Add("q");
            wordChoices.Add("r");
            wordChoices.Add("s");
            wordChoices.Add("t");
            wordChoices.Add("u");
            wordChoices.Add("V");
            wordChoices.Add("W");
            wordChoices.Add("X");
            wordChoices.Add("Y");
            wordChoices.Add("Z");
            wordChoices.Add("cancel");
            wordChoices.Add("reset");
            wordChoices.Add("done");
            /*var colorChoices = new Choices();
            colorChoices.Add("red");
            colorChoices.Add("green");
            colorChoices.Add("blue");

            var shapeChoices = new Choices(new string[] {
                "square", "circle"
            });*/

            var gb = new GrammarBuilder { Culture = ri.Culture };
            gb.Append(new SemanticResultKey("Words", wordChoices));



            /*
            gb.Append(new SemanticResultKey("Colors", colorChoices));
            gb.Append(new SemanticResultKey("Shapes", shapeChoices));
            gb.Append("please", 0, 3);
             */

            // gb.Append(new SemanticResultKey("Color", colorChoices);
            // gb.Append(new SemanticResultKey("Shapes", shapeCoices);

            // Create the actual Grammar instance, and then load it into the speech recognizer.
            var g = new Grammar(gb);

            sre.LoadGrammar(g);
            #endregion

            #region Hook up events
            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
            sre.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(sre_SpeechRecognitionRejected);
            /*
            sre.SpeechHypothesized += this.SreSpeechHypothesized;
            sre.SpeechRecognitionRejected += this.SreSpeechRecognitionRejected;
            */
            #endregion

            return sre;
        }
        #endregion

        #region Speech recognition events
        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence < 0.5)
            {
                this.RejectSpeech(e.Result);
                return;
            }

            // e.Result.Semantics["Color"].Value

            //switch (e.Result.Text.ToUpperInvariant())
            //{
            //    case "RED":
            //        brush = this.redBrush;
            //        break;
            //    case "GREEN":
            //        brush = this.greenBrush;
            //        break;
            //    case "BLUE":
            //        brush = this.blueBrush;
            //        break;
            //    default:
            //        brush = this.blackBrush;
            //        break;
            //}

            string[] output = e.Result.Text.Split();

            //switch (output[0].ToUpperInvariant())
            //TODO: actions that will be performed for each grammar, need tagbox so i can make it invisible and not hit testvisible when tagicon not activated
            string result = e.Result.Semantics["Words"].Value.ToString().ToUpperInvariant();
            if (tagIconActivated)
            {
                switch (result)
                {
                    case "CANCEL":
                        textBox.Gesture += "";
                        tagName = "";
                        tagIconActivated = false;
                        hideTagSection();
                        //call function to get rid of the tagBox
                        break;
                    case "RESET":
                        textBox.Gesture += "";
                        tagName = "";
                        break;
                    case "DONE":
                        //call git tag function with the tagName string
                        Terminal.GitTagLatestCommit(tagName);
                        TaggedText.Visibility = Visibility.Visible;
                        CommitedText.Visibility = Visibility.Collapsed;
                        hideTagSection();
                        tagIconActivated = false;
                        break;
                    case "HELLO WORLD":
                        textBox.Gesture += "HELLO World";
                        break;
                    case "A":
                    case "B":
                    case "C":
                    case "D":
                    case "E":
                    case "F":
                    case "G":
                    case "H":
                    case "I":
                    case "J":
                    case "K":
                    case "L":
                    case "M":
                    case "N":
                    case "O":
                    case "P":
                    case "Q":
                    case "R":
                    case "S":
                    case "T":
                    case "U":
                    case "V":
                    case "W":
                    case "X":
                    case "Y":
                    case "Z":
                        string letter = result.ToLowerInvariant();
                        textBox.Gesture += letter;
                        tagName += letter;
                        Console.WriteLine(tagName + "\n");
                        TagNameTextBlock.Text = tagName;
                        break;
                    default:
                        break;
                }
            }
            //switch (output[1].ToUpperInvariant())

            var audioSource = this.kinect.AudioSource;
            this.angle = audioSource.SoundSourceAngle;

            string status = "Recognized: " + e.Result.Text + " " + e.Result.Confidence + " | Angle: " + this.angle + " | Confidence: " + audioSource.SoundSourceAngleConfidence;

            //   Dispatcher.BeginInvoke(new Action(() => { shapeFunc(brush, this.angle); }), DispatcherPriority.Normal);
        }

        private void RejectSpeech(RecognitionResult result)
        {
            string status = "Rejected: " + (result == null ? string.Empty : result.Text + " " + result.Confidence);
        }

        void sre_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            this.RejectSpeech(e.Result);
        }
        #endregion
        void CheckSwipe(AllFramesReadyEventArgs e)
        {
            if (storedSkeletonValues.Count >= 45)
            {
                //arrays: index 0 is x values, index 1 is y values, and index 2 is z values
                float[] LHCounter = new float[3] { 0, 0, 0 };
                float[] RHCounter = new float[3] { 0, 0, 0 };
                float[] LHThreshold = new float[3] { 25, 25, 75 };
                float[] RHThreshold = new float[3] { 25, 25, 20 };
                int skeletonListCount = storedSkeletonValues.Count;
                bool posZChange = true;

                for (int i = 30; i > 1; i--)
                {
                    float[] firstSkeleton = storedSkeletonValues[skeletonListCount - i];
                    float[] secondSkeleton = storedSkeletonValues[skeletonListCount - i + 1];
                    float[] leftDifference = new float[3];
                    float[] rightDifference = new float[3];
                    for (int j = 0; j < 3; j++)
                    {
                        leftDifference[j] = firstSkeleton[j] - secondSkeleton[j];
                        rightDifference[j] = firstSkeleton[j + 3] - secondSkeleton[j + 3];
                    }
                    //RHPos[0] = firstSkeleton[3];
                    //RHPos[1] = firstSkeleton[4];

                    for (int k = 0; k < 3; k++)
                    {
                        if (k == 2)
                        {
                            if (posZChange && leftDifference[k] < 0)
                            {
                                posZChange = false;
                                LHCounter[k] = 0;
                            }
                            else if (!posZChange && leftDifference[k] > 0)
                            {
                                posZChange = true;
                                LHCounter[k] = 0;
                            }
                        }
                        RHCounter[k] += rightDifference[k];
                        //if right hand moved too far away, reset possible push recognition; otherwise, increase the left hand counters
                        if (RHCounter[k] > RHThreshold[k])
                        {
                            LHCounter[0] = 0;
                            LHCounter[1] = 0;
                            LHCounter[2] = 0;
                        }
                        else
                        {
                            LHCounter[k] += leftDifference[k];
                        }
                    }
                    if (LHCounter[0] < LHThreshold[0] && LHCounter[1] < LHThreshold[1])
                    {
                        actionWait = true;
                        selectTimer.Start();
                        if (LHCounter[2] > LHThreshold[2])
                        {
                            //TODO: push registered
                            textBox.Gesture += "Push Registered";
                            KinectPush();
                        }
                        else if (LHCounter[2] < -LHThreshold[2])
                        {
                            //TODO: pull registered
                            textBox.Gesture += "Pull Registered";
                            if (CursorInDirectoryArea())
                            {
                                //TODO: git pull would be caled her, but currently no function, and switch to working mode
                                //switchToWorkingMode();
                            }
                            else
                            {
                                KinectPull();
                            }
                        }
                    }
                    else
                    {
                        LHCounter[0] = 0;
                        LHCounter[1] = 0;
                        LHCounter[2] = 0;
                    }

                }
            }
        }

        void CheckStatic(AllFramesReadyEventArgs e)
        {
            int numFrames = 60;
            if (storedSkeletonValues.Count > numFrames)
            {
                //selectThreshold: how far the left and right hands can be and still register as a select
                float[] selectThreshold = new float[3]{50,50,50};
                for (int i = numFrames-1; i > 0; i--)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        float leftValue = storedSkeletonValues[storedSkeletonValues.Count - i][j];
                        float rightValue = storedSkeletonValues[storedSkeletonValues.Count - i][j+3];
                        float difference = Math.Abs(leftValue - rightValue);
                        if (difference > selectThreshold[j])
                        {
                            return;
                        }
                    }
                }
                //TODO: Select action (check if hand positions within directory area
                textBox.Gesture += "SELECTED";
                actionWait = true;
                selectTimer.Start();
                //lasso completed when select is activated again (hands touch again for the interval)
                //lasso completed, so change cursor senitivity back to original
                if (selectActivated)
                {
                    selectActivated = false;
                    lassoFilesDragging = true;
                    RHSensitivity[0] = 0.3f;
                    RHSensitivity[1] = 0.3f;
                    mouseLeftClick();
                    getSelectedFiles();
                    WC_inkCanvas.EditingMode = InkCanvasEditingMode.None;
                    //System.Windows.Controls.Image draggingImage  = getSelectedFiles();
                    System.Windows.Controls.Image draggingImage = findNearestImage(RHPos[0], RHPos[1]);
                    System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)InkCanvas.GetLeft(draggingImage), (int)InkCanvas.GetTop(draggingImage));
                    mouseLeftDown();
                    //TODO: Fix Dragging
                }
                //lasso start
                //Decrease sensitivity to improve accuracy of lassoing, and move the cursor to the closest image of the mouse to move the set of images
                else if (!selectActivated)
                {
                    RHSensitivity[0] = 0.6f;
                    RHSensitivity[1] = 0.6f;
                    WC_inkCanvas.EditingMode = InkCanvasEditingMode.Select;
                    selectActivated = true;
                    mouseLeftDown();
                }
            }
        }

        static void KinectPush()
        {
            mouseLeftClick();
        }

        static void KinectPull()
        {
            mouseRightClick();   
        }

        static void mouseLeftClick()
        {
            INPUT Input = new INPUT();
            Input.type = SendInputEventType.InputMouse;
            Input.mi.dwFlags = MouseEventFlags.LEFTDOWN |MouseEventFlags.LEFTUP;
            SendInput(1, ref Input, Marshal.SizeOf(new INPUT()));
        }

        static void mouseRightClick()
        {
            INPUT Input = new INPUT();
            Input.type = SendInputEventType.InputMouse;
            Input.mi.dwFlags = MouseEventFlags.RIGHTDOWN | MouseEventFlags.RIGHTUP;
            SendInput(1, ref Input, Marshal.SizeOf(new INPUT()));
        }
        static void mouseLeftDown()
        {
              INPUT    Input= new INPUT();
              Input.type      = SendInputEventType.InputMouse;
              Input.mi.dwFlags  = MouseEventFlags.LEFTDOWN;
              SendInput(1,ref Input,Marshal.SizeOf(new INPUT()));
        }


        void FollowPointer()
        {

            System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)RHPos[0], (int)RHPos[1]);
             // SetCursorPos((int)(RHPos[0]), (int)(RHPos[1]));
            /*b
              double fx = RHPos[0];
              double fy = RHPos[1];
              INPUT  Input= new INPUT();
              Input.type = SendInputEventType.InputMouse;;
              Input.mi.dwFlags = MouseEventFlags.MOVE | MouseEventFlags.ABSOLUTE;
              Input.mi.dx = (int)fx;
              Input.mi.dy = (int)fy;
              SendInput(1,ref Input,Marshal.SizeOf(new INPUT()));
             */
        }

        void GetCameraPoint(Skeleton first, AllFramesReadyEventArgs e)
        {

            using (DepthImageFrame depth = e.OpenDepthImageFrame())
            {
                if (depth == null ||
                    kinectSensorChooser1.Kinect == null)
                {
                    return;
                }


                //Map a joint location to a point on the depth map
                //head
                DepthImagePoint headDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.Head].Position);
                //left hand
                DepthImagePoint leftDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.HandLeft].Position);
                //right hand
                DepthImagePoint rightDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.HandRight].Position);


                //Map a depth point to a point on the color image
                //head
                ColorImagePoint headColorPoint =
                    depth.MapToColorImagePoint(headDepthPoint.X, headDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //left hand
                ColorImagePoint leftColorPoint =
                    depth.MapToColorImagePoint(leftDepthPoint.X, leftDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //right hand
                ColorImagePoint rightColorPoint =
                    depth.MapToColorImagePoint(rightDepthPoint.X, rightDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //Set location
                //CameraPosition(headImage, headColorPoint);
                //CameraPosition(leftEllipse, leftColorPoint);
                //CameraPosition(rightEllipse, rightColorPoint);
            }
        }


        Skeleton GetFirstSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }


                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                //get the first tracked skeleton
                Skeleton first = (from s in allSkeletons
                                  where s.TrackingState == SkeletonTrackingState.Tracked
                                  select s).FirstOrDefault();

                return first;

            }
        }

        private void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    //stop sensor 
                    sensor.Stop();

                    //stop audio if not null
                    if (sensor.AudioSource != null)
                    {
                        sensor.AudioSource.Stop();
                    }


                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closing = true;
            StopKinect(kinectSensorChooser1.Kinect);
        }

        private void CameraPosition(FrameworkElement element, ColorImagePoint point)
        {
            //Divide by 2 for width and height so point is right in the middle 
            // instead of in top/left corner
            //Canvas.SetLeft(element, point.X - element.Width / 2);
            //Canvas.SetTop(element, point.Y - element.Height / 2);
            element.Margin = new Thickness (point.X, point.Y, 0, 0);
        }

        private void ArrowPosition(FrameworkElement element, float pointX, float pointY)
        {
            Canvas.SetLeft(element, pointX - element.Width / 2);
            Canvas.SetTop(element, pointY + element.Height);
        }
        

        private void ScalePosition(FrameworkElement element, Joint joint)
        {
            //convert the value to X/Y
            //Joint scaledJoint = joint.ScaleTo(1280, 720); 
             
            //convert & scale (.3 = means 1/3 of joint distance)
            Joint scaledJoint = joint.ScaleTo(1366, 768, RHSensitivity[0], RHSensitivity[1]);
            Canvas.SetLeft(element, scaledJoint.Position.X-element.Width/2 - 150);
            Canvas.SetTop(element, scaledJoint.Position.Y-element.Height/2 - 300);
            textBox.RightPos = (int)scaledJoint.Position.X + " " + (int)scaledJoint.Position.Y + "\n";
            RHPos[0] = scaledJoint.Position.X;
            RHPos[1] = scaledJoint.Position.Y;
        }

    }
}
