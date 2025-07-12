//using System.Diagnostics;
//using System.Text.RegularExpressions;

//namespace Lims.ToolsForClient
//{
//    public static class FilesManager
//    {
//        public static string[] GetFileSystemEntries(string dir, string regexPattern = null, bool recurse = false, bool throwEx = false)
//        {
//            List<string> lst = new List<string>();

//            try
//            {
//                foreach (string item in Directory.GetFileSystemEntries(dir))
//                {
//                    try
//                    {
//                        if (regexPattern == null || Regex.IsMatch(Path.GetFileName(item), regexPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline))
//                        {
//                            lst.Add(item);
//                        }

//                        //递归
//                        if (recurse && (File.GetAttributes(item) & FileAttributes.Directory) == FileAttributes.Directory)
//                        {
//                            lst.AddRange(GetFileSystemEntries(item, regexPattern, true));
//                        }
//                    }
//                    catch { if (throwEx) { throw; } }
//                }
//            }
//            catch { if (throwEx) { throw; } }

//            return lst.ToArray();
//        }

//        public static void OpenFile(string filePath)
//        {
//            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
//        }
//        public static void OpenFolder(string filePath)
//        {
//            Process.Start("Explorer", "/select," + filePath);
//        }
//        public static void DeleteFile(string filePath)
//        {

//            var file = new FileInfo(filePath);
//            file.Delete();
//        }
//    }
//}