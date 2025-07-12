using System.Diagnostics;

namespace Lims.ToolsForClient
{
    public class PrintUtil
    {
        public static bool Print(string pathStr)
        {
            try
            {
                if (File.Exists(pathStr) == false)
                    return false;

                var pr = new Process
                {
                    StartInfo =
                    {
                        FileName = pathStr,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Verb = "Print",
                    UseShellExecute=true,
                    }
                };
                System.Diagnostics.Process.Start(pr.StartInfo);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}