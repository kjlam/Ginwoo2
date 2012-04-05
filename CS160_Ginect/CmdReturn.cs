using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CS160_Ginect
{
    
    
    class CmdReturn
    {
        internal InputResult inputSuccess;
        internal enum InputResult { SUCCESS, UNDEFINED, FAIL };

        internal int exitCode;
        internal String stdout;

        public CmdReturn()
        {
            inputSuccess = InputResult.UNDEFINED;
        }

        public String ToString()
        {
            StringWriter strWriter = new StringWriter();
            strWriter.WriteLine();
            strWriter.WriteLine("CMD RETURN");
            strWriter.WriteLine();

            // Display inputSuccess
            switch (inputSuccess)
            {
                case InputResult.SUCCESS:
                    strWriter.WriteLine("  inputSuccess = SUCCESS: password was inputted successfully.");
                    strWriter.WriteLine();
                    break;
                case InputResult.FAIL:
                    strWriter.WriteLine("  inputSuccess = FAIL: password was not inputted successfully.");
                    strWriter.WriteLine("    The problem may be that the password is incorrect or that");
                    strWriter.WriteLine("    the correct CMD terminal wasn't found.");
                    strWriter.WriteLine();
                    break;
                case InputResult.UNDEFINED:
                    strWriter.WriteLine("  inputSuccess = UNDEFINED: no password has been inputted.");
                    strWriter.WriteLine();
                    break;
                default:
                    strWriter.WriteLine("  inputSuccess error.");
                    strWriter.WriteLine();
                    break;
            }

            // Display exitCode
            strWriter.WriteLine("  exitCode = " + exitCode);
            strWriter.WriteLine();

            // Display stdout
            strWriter.WriteLine("  stdout =");
            strWriter.WriteLine(stdout);
            strWriter.WriteLine();

            return strWriter.ToString();
        }
    }
}
