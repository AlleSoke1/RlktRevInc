using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RlktRevInc
{
    internal class Program
    {
        private static string strDefaultVersion = "2408";
        private string version = Program.strDefaultVersion;
        private const string strConfigFile = ".\\RlktRevInc.ini";

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(
          string Section,
          string Key,
          string Default,
          StringBuilder RetVal,
          int Size,
          string FilePath);

        private static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            if (!File.Exists(strConfigFile))
            {
                Console.WriteLine(".\\RlktRevInc.ini Not found!");
            }
            else
            {
                StringBuilder RetVal1 = new StringBuilder();
                Program.GetPrivateProfileString("RLKT_REV", "RevisionHeaderPath", "", RetVal1, (int)byte.MaxValue, strConfigFile);
                StringBuilder RetVal2 = new StringBuilder();
                Program.GetPrivateProfileString("RLKT_REV", "VersionTextFilePath", "", RetVal2, (int)byte.MaxValue, strConfigFile);
                StringBuilder RetVal3 = new StringBuilder();
                Program.GetPrivateProfileString("RLKT_REV", "ProjectResourcePath", "", RetVal3, (int)byte.MaxValue, strConfigFile);
                int result = 0;
                if (!File.Exists(RetVal2.ToString()))
                {
                    File.WriteAllText(RetVal2.ToString(), Program.strDefaultVersion);
                }
                else
                {
                    string s = File.ReadAllText(RetVal2.ToString());
                    Console.WriteLine("Version inside file: " + s);
                    int.TryParse(s, out result);
                    Console.WriteLine(string.Format("Last Revision : {0} , Current Revision : {1}", (object)result, (object)(result + 1)));
                    ++result;
                    File.WriteAllText(RetVal2.ToString(), result.ToString());
                }
                string contents = "static char * szLastRevision = { \"" + result.ToString() + "\" };" + Environment.NewLine + "static int nLastRevision = " + result.ToString() + ";";
                File.WriteAllText(RetVal1.ToString(), contents);
                Program.lineChanger("FILEVERSION 1, 0, 0, " + result.ToString(), RetVal3.ToString(), 2);
            }
        }

        private static void lineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] contents = File.ReadAllLines(fileName);
            contents[line_to_edit - 1] = newText;
            File.WriteAllLines(fileName, contents);
        }
    }
}
