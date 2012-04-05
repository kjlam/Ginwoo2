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
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.Threading;
using Coding4Fun.Kinect.Wpf;

namespace WpfApplication1{

public class MouseSimulateForm : Form
{
    private Point _buttonPos = Point.Empty;
    public MouseSimulateForm()
    {
        Button bt = new Button { Text = "SetPos", Location = new Point(20, 20) };
        this.Controls.Add(bt);
        bt.Click += new EventHandler(bt_Click);

        bt = new Button { Text = "Test", Location = new Point(20, 60) };
        this.Controls.Add(bt);
        bt.Click += new EventHandler(test_Click);
    }

    void bt_Click(object sender, EventArgs e)
    {
        if (_buttonPos == Point.Empty)
            _buttonPos = Control.MousePosition;
        else
            MessageBox.Show("SB");
    }

    void test_Click(object sender, EventArgs e)
    {
        //Move the mouse to the button position
        SetCursorPos(_buttonPos.X, _buttonPos.Y);

        //Perform button click.
        INPUT structInput = new INPUT();
        structInput.type = SendInputEventType.InputMouse;
        structInput.mkhi.mi.dwFlags = MouseEventFlags.ABSOLUTE | MouseEventFlags.LEFTDOWN | MouseEventFlags.LEFTUP;
        structInput.mkhi.mi.dx = _buttonPos.X;
        structInput.mkhi.mi.dy = _buttonPos.Y;
        uint i = SendInput(1, ref structInput, Marshal.SizeOf(new INPUT()));
    }

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
        InputMouse,
        /// <summary>
        /// Contains Keyboard event data
        /// </summary>
        InputKeyboard,
        /// <summary>
        /// Contains Hardware event data
        /// </summary>
        InputHardware
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

    [StructLayout(LayoutKind.Sequential)]
    struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct HARDWAREINPUT
    {
        public int uMsg;
        public short wParamL;
        public short wParamH;
    }

    /// <summary>
    /// Captures the union of the three three structures.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    struct MouseKeybdhardwareInputUnion
    {
        /// <summary>
        /// The Mouse Input Data
        /// </summary>
        [FieldOffset(0)]
        public MouseInputData mi;

        /// <summary>
        /// The Keyboard input data
        /// </summary>
        [FieldOffset(0)]
        public KEYBDINPUT ki;

        /// <summary>
        /// The hardware input data
        /// </summary>
        [FieldOffset(0)]
        public HARDWAREINPUT hi;
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
        public MouseKeybdhardwareInputUnion mkhi;
    }
}

}