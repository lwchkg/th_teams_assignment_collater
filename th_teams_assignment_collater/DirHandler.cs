using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;

namespace th_teams_assignment_collater
{
    class DirHandler
    {
        static readonly string[] FolderSuffix = {
            " - Student Work",
            " - 學生功課"
        };

        public static List<string> GetFolderList()
        {
            var output = new List<string>();
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            foreach (string orgDir in Directory.GetDirectories(basePath))
            {
                try
                {
                    foreach (string repoDir in Directory.GetDirectories(orgDir))
                    {
                        if (FolderSuffix.Any(s => repoDir.EndsWith(s)))
                            output.Add(repoDir);
                    }
                }
                catch (Exception) { /* Do nothing */ }
            }

            return output;
        }

        public static Dictionary<string, string> GetFileList(string from)
        {
            var fileList = new Dictionary<string, string>();

            try
            {
                foreach (string submittedFilesDir in Directory.GetDirectories(Path.Combine(from)))
                {
                    try
                    {
                        foreach (string userDir in Directory.GetDirectories(submittedFilesDir))
                        {
                            string userDirFN = Path.GetFileName(userDir);
                            try
                            {
                                foreach (string homeworkDir in Directory.GetDirectories(userDir))
                                {
                                    string homeworkDirFN = Path.GetFileName(homeworkDir);
                                    try
                                    {
                                        foreach (string versionDir in Directory.GetDirectories(homeworkDir))
                                        {
                                            string versionDirFN = Path.GetFileName(versionDir);
                                            try
                                            {
                                                foreach (string file in Directory.GetFiles(versionDir))
                                                {
                                                    string fileFN = Path.GetFileName(file);
                                                    string outputFN = $"{homeworkDirFN}\\{userDirFN} - {versionDirFN} - {fileFN}";
                                                    fileList[file] = outputFN;
                                                }
                                            }
                                            catch (Exception) { /* Do nothing. */ }
                                        }
                                    }
                                    catch (Exception) { /* Do nothing. */ }
                                }
                            }
                            catch (Exception) { /* Do nothing. */ }
                        }
                    }
                    catch (Exception) { /* Do nothing. */ }
                }
            }
            catch (Exception) { /* Do nothing. */ }

            return fileList;
        }

        private static void EnsureDirExist(string dir)
        {
            if (!Directory.Exists(dir))
            {
                EnsureDirExist(Path.GetDirectoryName(dir));
                Directory.CreateDirectory(dir);
            }
        }

        private static void CopyFile(string from, string to)
        {
            EnsureDirExist(Path.GetDirectoryName(to));
            File.Copy(from, to);
        }

        public static bool DoCollate(string from, string to, int maxRetries, Action<string> FailCallback)
        {
            bool success = true;

            var fileList = GetFileList(from);
            foreach (var item in fileList)
            {
                for (int i = 1; i <= maxRetries; ++i)
                {
                    try
                    {
                        CopyFile(item.Key, Path.Combine(to, item.Value));
                    }
                    catch (Exception)
                    {
                        FailCallback.Invoke($"Retry {i}: copy \"{item.Key}\" to \"{item.Value}\"");
                        if (i == maxRetries)
                            success = false;
                    }
                }
            }
            return success;
        }
    }
}
