using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// For password prompt
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Microsoft.VisualBasic;
//using CS160_Ginect;


namespace WpfApplication1
{

    public class Terminal
    {
        static internal String workingDirectory = @"C:\Users\Jessica\Ginect";
        static internal String password = "password";

        public Terminal()
        {
        }

        // Just for light testing
        static internal CmdReturn TestModularTerminal()
        {
            // Test GitLog()
            return GitLog();

            // Test git status
            //return ExecuteProcess(workingDirectory, "git status", false);

            // Test GitAddFilesToCommit()
            /*
            List<String> filesList = new List<string>();
            filesList.Add("jessica.txt");
            filesList.Add("jessica2.txt");
            return GitAddFilesToCommit(filesList);
            */

            // Test GitCommitWithMessage()
            // return GitCommitWithMessage("Another test commit");

            // Test GitTagLatestCommit()
            // return GitTagLatestCommit("testTag");

            // Test GitPush()
            // return GitPush();

        }

        /*
         * GitAddFilesToCommit()
         * 
         * This executes a 'git add <file1> <file2> .. <fileN>' command.
         * 
         */
        static internal CmdReturn GitAddFilesToCommit(List<String> filesList)
        {
            String filesStr = "";

            filesList.ForEach(delegate(String file)
            {
                filesStr += " ";
                filesStr += file;
            });

            return ExecuteProcess(workingDirectory, "git add" + filesStr, false);
        }


        static internal CmdReturn GitTagLatestCommit(String tagName)
        {
            String latestCommitID = GetLatestCommitID();
            return GitTag(tagName, latestCommitID);
        }

        /*
         * GetLatestCommitID()
         * 
         * This returns the latest commit ID of the local commits
         * made by the registered user. These commits have not been
         * pushed to the remote master yet.
         * 
         * Returns a string of the latest commit ID.
         * 
         */
        static private String GetLatestCommitID()
        {
            char[] delimiterChars = { ' ' };
            String latestCommit = null;

            String unpushedCommits = ExecuteCommand(workingDirectory, "git log origin/master..HEAD");
            unpushedCommits = ParseStdOut(unpushedCommits);

            using (StringReader reader = new StringReader(unpushedCommits))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (Regex.IsMatch(line, "^commit .+$"))
                    {
                        latestCommit = line;
                        break;
                    }
                }
            }

            String[] splitLatestCommit = latestCommit.Split(delimiterChars);
            return splitLatestCommit[1];
        }

        /*
         * GitTag()
         * 
         * This executes a 'git tag -f <tagname> <commitID>' command.
         * A 'git commit' must be made before this method.
         * 
         */
        static private CmdReturn GitTag(String tagName, String commitID)
        {
            return ExecuteProcess(workingDirectory, "git tag -f " + tagName + " " + commitID, false);
        }

        /*
         * GitCommitWithMessage()
         * 
         * This executes a 'git commit -m "My message"' command.
         * 
         */
        static internal CmdReturn GitCommitWithMessage(String message)
        {
            return ExecuteProcess(workingDirectory, "git commit -m \"" + message + "\"", false);
        }

        /*
         * GitPush()
         * 
         * This executes a 'git push' command. A 'git commit' must be made
         * before calling this method.
         * 
         */
        static internal CmdReturn GitPush()
        {
            return ExecuteProcess(workingDirectory, "git push", true);

            // TODO: Figure out if git push was executed correctly
        }

        // TODO: Complete this method. Return a list of files that have been modified
        // since the last commit
        static internal List<String> GitStatus()
        {
            List<String> modifiedFiles = new List<String>();
            String stdout = ExecuteCommand(workingDirectory, "git status");

            // parse stdout and add files to modifiedFiles

            return modifiedFiles;
        }

        static internal CmdReturn GitLog()
        {
            return ExecuteProcess(workingDirectory, "git log", false);
        }

        /*
         * ExecuteCommand()
         * 
         * This executes a command in CMD. The CMD terminal is not displayed.
         * Returns the standard output of the command.
         * 
         */
        static private String ExecuteCommand(String directory, String command)
        {
            Process process = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            // Make it so the terminal isn't displayed on the screen when executing commands
            startInfo.CreateNoWindow = true;

            // The cmd terminal
            startInfo.FileName = "cmd.exe";

            Directory.SetCurrentDirectory(directory);

            startInfo.Arguments = "/C " + command;
            process.StartInfo = startInfo;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();

            String stdout = ParseStdOut(process.StandardOutput.ReadToEnd());
            process.WaitForExit();
            process.Close();

            return stdout;
        }

        static private CmdReturn ExecuteProcess(String directory, String command, bool needToInputPassword)
        {
            Process process = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            if (needToInputPassword)
            {
                // The terminal is displayed, only used for sending the password to the terminal.
                // As in, can't find a way to hide the window AND send the password successfully.
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            }
            else
            {
                // Make it so the terminal isn't displayed on the screen when executing commands
                startInfo.CreateNoWindow = true;
            }

            // The cmd terminal
            startInfo.FileName = "cmd.exe";

            Directory.SetCurrentDirectory(directory);

            startInfo.Arguments = "/C " + command;
            process.StartInfo = startInfo;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;

            CmdReturn cmdReturn = new CmdReturn();
            process.Start();

            // For debugging purposes, do not delete!
            Process[] processlist = Process.GetProcesses();
            foreach (Process theprocess in processlist)
            {
                Console.WriteLine("Process: {0} ID: {1} MainWindowHandle: {2}", process.ProcessName, process.Id, process.MainWindowHandle);
            }

            if (needToInputPassword)
            {
                cmdReturn.inputSuccess = SendPasswordToStdin();
            }

            cmdReturn.stdout = ParseStdOut(process.StandardOutput.ReadToEnd());
            process.WaitForExit();
            cmdReturn.exitCode = process.ExitCode;
            process.Close();

            return cmdReturn;
        }

        static private CmdReturn.InputResult SendPasswordToStdin()
        {
            Process[] processes = Process.GetProcessesByName("cmd");
            int numProcesses = processes.Length;

            if (numProcesses > 0)
            {
                IntPtr mainWindowHandle = IntPtr.Zero;
                foreach (Process process in processes)
                {
                    if (!process.MainWindowHandle.Equals(IntPtr.Zero))
                    {
                        mainWindowHandle = process.MainWindowHandle;
                        break;
                    }
                }

                // Actual code + debugging, do not delete
                if (!SetForegroundWindow(mainWindowHandle))
                {
                    Debug.WriteLine("SET FOREGROUND WINDOW FAILED");
                    Debug.WriteLine("foreground window handle = " + GetForegroundWindow());
                }
                else
                {
                    Debug.WriteLine("SET FOREGROUND WINDOW SUCCESS");
                    Debug.WriteLine("foreground window handle = " + GetForegroundWindow());
                }

                SendKeys.SendWait(password + "{ENTER}");
                return CmdReturn.InputResult.SUCCESS;
            }
            // No processes found matching "cmd"
            else
            {
                return CmdReturn.InputResult.FAIL;
            }
        }

        static private int ExecuteCommandWithPassword(String directory, String command, String password)
        {
            Process process = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            // Make it so the terminal isn't displayed on the screen when executing commands
            //startInfo.CreateNoWindow = true;
            //startInfo.CreateNoWindow = false;
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;

            // The cmd terminal
            startInfo.FileName = "cmd.exe";

            Directory.SetCurrentDirectory(directory);

            startInfo.Arguments = "/C " + command;
            process.StartInfo = startInfo;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;

            //process.EnableRaisingEvents = true;


            process.Start();
            /*
            try
            {
                process.WaitForInputIdle();
            }

            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
             * */
            Process[] processlist = Process.GetProcesses();

            foreach (Process theprocess in processlist)
            {
                Console.WriteLine("Process: {0} ID: {1} MainWindowHandle: {2}", process.ProcessName, process.Id, process.MainWindowHandle);
            }

            //process.WaitForInputIdle();
            //System.Threading.Thread.Sleep(10000);
            //System.Console.WriteLine("process handle = " + process.MainWindowHandle);
            //System.Console.WriteLine("process main window title = " + process.MainWindowTitle);
            //Console.WriteLine("Try via WIN32: " + Microsoft.Win32.GetMainProcessWindow(process.Id));
            //process.Refresh();
            /*
            while (process.MainWindowHandle == IntPtr.Zero)
            {
                System.Console.WriteLine("process handle = " + process.MainWindowHandle);
                System.Console.WriteLine("process main window title = " + process.MainWindowTitle);

                process = Process.GetProcessById(process.Id);
                process.Refresh();
                System.Threading.Thread.Sleep(100);
            }
             * */


            //System.Console.WriteLine("process handle = " + process.MainWindowHandle);
            //System.Console.WriteLine("process main window title = " + process.MainWindowTitle);

            //System.Console.WriteLine("process handle = " + process.Handle);
            //SendKeyTestCmdExe(process.Handle);
            //SendKeyTestCmdExe(process.MainWindowHandle);
            SendKeyTestCmdExe();

            //System.Threading.Thread.Sleep(10000);
            //BinaryWriter stdin = new BinaryWriter(process.StandardInput.BaseStream);
            //stdin.Write(password);
            //process.StandardInput.Write(password + "\n");
            //Console.WriteLine("Done writing to standard input");

            //String stdout = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            int exitCode = process.ExitCode;
            //Debug.WriteLine(exitCode.ToString());
            process.Close();
            return exitCode;

            /*
            Debug.WriteLine("before end");
            if (process.HasExited)
            {
                Debug.WriteLine("process has exited");
            
            }
            else
            {
                Debug.WriteLine("process has not exited");
                return -10;
            }
            //return stdout;
             * */
        }

        /*
         * ParseStdOut()
         * 
         * Strips out the command prompt lines from stdout. The command prompt
         * starts with "C:". The rough format of standard output from executing Git
         * commands in CMD is:
         * 
         *      C:\current\directory>some stuff
         *      stdout line 1
         *      stdout line 2
         * 
         *      C:\current\directory>more stuff
         * 
         *      C:\current\directory>and more stuff
         * 
         * This method extracts and returns:
         * 
         *      stdout line 1
         *      stdout line 2
         *      
         */
        static private String ParseStdOut(String stdout)
        {
            String boundary = "^C:.*$";
            StringWriter writer = new StringWriter();
            int canWrite = 0;

            using (StringReader reader = new StringReader(stdout))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (Regex.IsMatch(line, boundary))
                    {
                        if (++canWrite > 1)
                            break;
                    }
                    else
                        writer.WriteLine(line);
                }
            }
            return writer.ToString();
        }





        /*
        // Get a handle to an application window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);

        [DllImport("USER32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("USER32.DLL", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
         * */

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("USER32.DLL", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("USER32.DLL")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static internal void SendKeyTestCmdExe(/* IntPtr cmdHandler */)
        {
            IntPtr windowHandle = IntPtr.Zero;

            Process[] processes = Process.GetProcessesByName("cmd");
            //Debug.WriteLine("all processes = " + processes.ToString());
            if (processes.Length > 0)
            {
                int last = processes.Length - 1;
                IntPtr mainWindowHandle = processes[last].MainWindowHandle;
                Debug.WriteLine("processes[last].ProcessName = " + processes[last].ProcessName);
                Debug.WriteLine("processes[last].Id = " + processes[last].Id);
                Debug.WriteLine("main window handle = " + mainWindowHandle);
                windowHandle = mainWindowHandle;

            }



            if (!SetForegroundWindow(windowHandle))
            {
                Debug.WriteLine("SET FOREGROUND WINDOW FAILED");
                Debug.WriteLine("foreground window handle = " + GetForegroundWindow());
            }
            else
            {
                Debug.WriteLine("SET FOREGROUND WINDOW SUCCESS");
                Debug.WriteLine("foreground window handle = " + GetForegroundWindow());
            }

            //System.Console.WriteLine("cmdHandler = " + cmdHandler);
            //SetForegroundWindow(cmdHandler);

            //System.Console.WriteLine(SetForegroundWindow(cmdHandler));

            //ShowWindow(cmdHandler, 1);
            //System.Console.WriteLine(ShowWindow(cmdHandler, 1));


            //System.Console.WriteLine("actual foregrounder handler = " + GetForegroundWindow().ToString());

            //Debug.Assert(cmdHandler == GetForegroundWindow());

            SendKeys.SendWait(password + "{ENTER}");
        }
    }
}
