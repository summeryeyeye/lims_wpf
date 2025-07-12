using System.Diagnostics;

namespace Lims.ToolsForClient
{
    public static class BrowserHelper
    {
        public static void SearchByBaiduBaike(string keyword)
        {
            try
            {

                _ = Process.Start(new ProcessStartInfo("https://baike.baidu.com/item/" + keyword) { UseShellExecute=true});
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public static void SearchByFoodmate(string keyword)
        {
            try
            {

                _ = Process.Start(new ProcessStartInfo("http://down.foodmate.net/standard/search.php?corpstandard=2&fields=0&kw=" + keyword) { UseShellExecute = true });
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
