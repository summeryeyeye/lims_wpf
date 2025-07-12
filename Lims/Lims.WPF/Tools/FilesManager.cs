using DevExpress.Xpf.Core;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace Lims.WPF.Tools
{
    public static class FilesManager
    {
        public static string[] GetFileSystemEntries(string dir, string? regexPattern = null, bool recurse = false, bool throwEx = false)
        {
            List<string> lst = new List<string>();

            try
            {
                foreach (string item in Directory.GetFileSystemEntries(dir))
                {
                    try
                    {
                        if (regexPattern == null || Regex.IsMatch(Path.GetFileName(item), regexPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline))
                        {
                            lst.Add(item);
                        }

                        //递归
                        if (recurse && (File.GetAttributes(item) & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            lst.AddRange(GetFileSystemEntries(item, regexPattern, true));
                        }
                    }
                    catch { if (throwEx) { throw; } }
                }
            }
            catch { if (throwEx) { throw; } }

            return lst.ToArray();
        }

        public static void OpenFile(string filePath)
        {
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }
        public static void OpenFolder(string filePath)
        {
            Process.Start("Explorer", "/select," + filePath);
        }
        public static void DeleteFile(string filePath)
        {
            if (DXMessageBox.Show("确定删除该标准文件", "删除", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) == MessageBoxResult.OK)
            {
                try
                {
                    FileInfo file = new FileInfo(filePath);
                    file.Delete();
                }
                catch (Exception e)
                {

                    throw new Exception(e.Message);
                }

            }

        }
    }
}