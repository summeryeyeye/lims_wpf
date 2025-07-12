using System.Drawing;
using System.Windows.Forms;

namespace Lims.WPF.Resources.UserControls
{
    public class DragForm : DevExpress.Utils.Win.TopFormBase
    {
        Bitmap m_buff;
        Graphics m_buffG;

        public DragForm(Rectangle _bound)
        {
            Text = "";
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            Size = _bound.Size;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;

            Opacity = 0.8d; //TopFormBase已经有默认值了；

            m_buff = new Bitmap(_bound.Width, _bound.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            m_buffG = Graphics.FromImage(m_buff);
            m_buffG.CopyFromScreen(_bound.Location, new Point(0, 0), _bound.Size);

            BackgroundImageLayout = ImageLayout.None;
            BackgroundImage = m_buff;

            Location = _bound.Location;

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // e.Graphics.DrawImage(m_buff, 0, 0);
        }
    }
}
