using Lims.Common.Dtos;
using Lims.ToolsForClient;
using System.Drawing;
using System.Windows.Forms;

namespace Lims.WPF.Views.Dialogs
{
    public partial class ReportDataPreview : DevExpress.XtraEditors.XtraForm
    {

        public ReportDataPreview(List<ItemDto> datas)
        {
            InitializeComponent();
            //List<ItemDto> datas = (List<ItemDto>)ItemDto.ReportSource;

            this.WindowState = FormWindowState.Normal;

            var timesFont = new Font("Times New Roman", 12F);
            var chnFont = new Font("楷体", 12F);


            this.Text = datas.FirstOrDefault().SampleCode;
            reportDataPreviewGrid.DataSource = datas.ToDataTable();
            reportDataPreviewView.OptionsView.ShowDetailButtons = false;

            foreach (DevExpress.XtraGrid.Columns.GridColumn col in reportDataPreviewView.Columns)
            {
                col.Visible = false;
                col.AppearanceCell.ForeColor = Color.FromArgb(0, 0, 0);
            }
            var columns = reportDataPreviewView.Columns;


            columns["ExecuteStandard"].Visible = true;
            columns["ExecuteStandard"].Caption = "执行标准";
            columns["ExecuteStandard"].AppearanceCell.Font = timesFont;
            columns["TestMethod"].Visible = true;
            columns["TestMethod"].Caption = "检测方法";
            columns["TestMethod"].AppearanceCell.Font = timesFont;

            columns["SingleConclusion"].Visible = true;
            columns["SingleConclusion"].Caption = "单项结论";

            columns["IndexRequest"].Visible = true;
            columns["IndexRequest"].Caption = "指标要求";
            columns["IndexRequest"].AppearanceCell.Font = timesFont;

            columns["TestResult"].Visible = true;
            columns["TestResult"].Caption = "检测结果";
            //AppearanceObjectEx cell=new AppearanceObjectEx();
            columns["TestResult"].AppearanceCell.Font = timesFont;

            columns["ReportUnit"].Visible = true;
            columns["ReportUnit"].Caption = "报告单位";
            //columns["ReportUnit"].AppearanceCell.Font = timesFont;

            columns["TestItem"].Visible = true;
            columns["TestItem"].Caption = "检测项目";
            //columns[""].AppearanceCell.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            reportDataPreviewView.Columns.ColumnByFieldName("TestItem").AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;



            columns["SampleCode"].Visible = true;
            columns["SampleCode"].Caption = "样品编号";







        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // 用双缓冲绘制窗口的所有子控件
                return cp;
            }
        }
    }

    public class ItemDtoForReportData : ItemDto
    {



    }
}