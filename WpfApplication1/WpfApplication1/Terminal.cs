using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;


using System.Runtime.InteropServices;


namespace WpfApplication1
{

    public class Terminal
    {
        static internal String workingDirectory;

        public Terminal()
        {
            System.IO.StreamReader file = new System.IO.StreamReader(@"temp.txt");
            workingDirectory = @file.ReadLine();
        }

        // Just for light testing
        static internal CmdReturn TestModularTerminal()
        {
            // Test GitLog()
            //return GitLog();

            // Test git status
            //return GitStatus();

            // Test GitAddFilesToCommit()
            /*
            List<String> filesList = new List<string>();
            filesList.Add("jessica.txt");
            filesList.Add("jessica2.txt");
            return GitAddFilesToCommit(filesList);
            */

            // Test GitCommitWithMessage()
            //return GitCommitWithMessage("Another test commit");

            // Test GitTagLatestCommit()
            //return GitTagLatestCommit("testTag");

            // Test GitPush()
            //return GitPush();

            // Test GitGetLocalRepoFiles()
            //return GitGetLocalRepoFiles();

            // Test GitGetRemoteRepoFiles()
            return GitGetRemoteRepoFiles();
        }

        static internal CmdReturn GitGetLocalRepoFiles()
        {
            CmdReturn cmdReturn = ExecuteProcess(workingDirectory, "git ls-tree --full-tree -r HEAD", false);
            cmdReturn.fileList = ParseGetRepoFilesStdout(cmdReturn.stdout);
            return cmdReturn;
        }

        static internal CmdReturn GitGetRemoteRepoFiles()
        {
            ExecuteProcess(workingDirectory, "git fetch", true);
            CmdReturn cmdReturn = ExecuteProcess(workingDirectory, "git ls-tree --full-tree -r origin", false);
            cmdReturn.fileList = ParseGetRepoFilesStdout(cmdReturn.stdout);
            return cmdReturn;
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

            String unpushedCommits = ExecuteProcess(workingDirectory, "git log origin/master..HEAD", false).stdout;
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
        }

        /*
        * GitPull()
        * 
        * This executes a 'git pull' command. A 'git commit' must be made
        * before calling this method.
        * 
        */
        static internal CmdReturn GitPull()
        {
            return ExecuteProcess(workingDirectory, "git pull", true);
        }

        

        static internal CmdReturn GitStatus()
        {
            CmdReturn cmdReturn = ExecuteProcess(workingDirectory, "git status", false);
            cmdReturn.fileList = ParseGitStatusStdout(cmdReturn.stdout);
            return cmdReturn;
        }

        static internal CmdReturn GitLog()
        {
            return ExecuteProcess(workingDirectory, "git log", false);
        }

        static private CmdReturn ExecuteProcess(String directory, String command, bool needToInputPassword)
        {
            Process process = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            /*if (needToInputPassword)
            {
                // The terminal is displayed, only used for sending the password to the terminal.
                // As in, can't find a way to hide the window AND send the password successfully.
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            }
            else
            {*/
                // Make it so the terminal isn't displayed on the screen when executing commands
                startInfo.CreateNoWindow = true;
            //}

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
                //todo: uncomment this
                //Console.WriteLine("Process: {0} ID: {1} MainWindowHandle: {2}", process.ProcessName, process.Id, process.MainWindowHandle);
            }

            /*if (needToInputPassword)
            {
                cmdReturn.inputSuccess = SendPasswordToStdin();
            }*/

            cmdReturn.stdout = ParseStdOut(process.StandardOutput.ReadToEnd());
            process.WaitForExit();
            cmdReturn.exitCode = process.ExitCode;
            process.Close();

            return cmdReturn;
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
         * C:\Users\Jessica\Ginect>git status
# On branch master
# Changes not staged for commit:
#   (use "git add/rm <file>..." to update what will be committed)
#   (use "git checkout -- <file>..." to discard changes in working directory)
#
#       deleted:    jessica.txt
#       modified:   jessica2.txt
#
# Untracked files:
#   (use "git add <file>..." to include in what will be committed)
#
#       jessica3.txt
no changes added to commit (use "git add" and/or "git commit -a")
         * */
        static private List<String> ParseGitStatusStdout(String stdout)
        {
            List<String> fileList = new List<String>();

            // Instantiate the regular expression object.
            String deleted = @"^.*deleted:\s*(\S*)\s*$";
            String modified = @"^.*modified:\s*(\S*)\s*$";
            String added = @"^#\s+(\S*)\s*$";
            String boundaryUntrackedFiles = @"^#.*Untracked\sfiles:.*$";

            Regex regexDeleted = new Regex(deleted);
            Regex regexModified = new Regex(modified);
            Regex regexAdded = new Regex(added);

            bool getUntrackedFiles = false;

            using (StringReader reader = new StringReader(stdout))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Match m = regexDeleted.Match(line);
                    if (m.Success)
                    {
                        Group g = m.Groups[1];
                        if (g.Success)
                        {
                            fileList.Add(g.Value);
                        }
                    }

                    m = regexModified.Match(line);
                    if (m.Success)
                    {
                        Group g = m.Groups[1];
                        if (g.Success)
                        {
                            fileList.Add(g.Value);
                        }
                    }

                    if (getUntrackedFiles)
                    {
                        m = regexAdded.Match(line);
                        if (m.Success)
                        {
                            Group g = m.Groups[1];
                            if (g.Success)
                            {
                                fileList.Add(g.Value);
                            }
                        }
                    }

                    if (Regex.IsMatch(line, boundaryUntrackedFiles))
                    {
                        getUntrackedFiles = true;
                    }
                }
            }
            return fileList;
        }

        /*
         * 160000 commit 21b748f4bf92e961c54d9ed88ea7ad4274ced939  Ginect
100644 blob e69de29bb2d1d6434b8b29ae775ad8c2e48c5391    README
100644 blob e69de29bb2d1d6434b8b29ae775ad8c2e48c5391    RJW.txt
100644 blob 274078a236dedec1236aa71c496883317dab9272    jessica.txt
100644 blob bd077e1ada14f475ade4f6e1426bb3768295e26b    jessica2.txt
100644 blob f1f0540097947db8f9864eb310b79b9b8bb39219    jessica3.txt
100644 blob 2a7efc2606b912cbac3bb9f3f1015acbec5d2658    jessica4.txt
100644 blob e69de29bb2d1d6434b8b29ae775ad8c2e48c5391    klam.txt
100644 blob e69de29bb2d1d6434b8b29ae775ad8c2e48c5391    push.txt
         * */
        static private List<String> ParseGetRepoFilesStdout(String stdout)
        {
            List<String> fileList = new List<String>();

            Regex regex = new Regex(@"^\S+\s+(\S+)\s+\S+\s+(\S+)\s*$");

            using (StringReader reader = new StringReader(stdout))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Match m = regex.Match(line);
                    if (m.Success)
                    {
                        Group g1 = m.Groups[1];
                        Group g2 = m.Groups[2];
                        if (g1.Success && g2.Success && g1.Value != "commit")
                        {
                            fileList.Add(g2.Value);
                        }
                    }
                }
            }
            return fileList;
        }

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("USER32.DLL", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("USER32.DLL")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
