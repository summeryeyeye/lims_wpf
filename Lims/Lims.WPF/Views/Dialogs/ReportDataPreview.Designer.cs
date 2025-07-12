using Lims.WPF.Resources.UserControls;

namespace Lims.WPF.Views.Dialogs
{
    partial class ReportDataPreview
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            reportDataPreviewGrid = new MyGridControl();
            reportDataPreviewView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)reportDataPreviewGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)reportDataPreviewView).BeginInit();
            SuspendLayout();
            // 
            // reportDataPreviewGrid
            // 
            reportDataPreviewGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataPreviewGrid.Location = new System.Drawing.Point(0, 0);
            reportDataPreviewGrid.MainView = reportDataPreviewView;
            reportDataPreviewGrid.Name = "reportDataPreviewGrid";
            reportDataPreviewGrid.Size = new System.Drawing.Size(1020, 640);
            reportDataPreviewGrid.TabIndex = 0;
            reportDataPreviewGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { reportDataPreviewView });
            // 
            // reportDataPreviewView
            // 
            reportDataPreviewView.Appearance.Row.Font = new System.Drawing.Font("楷体", 12F);
            reportDataPreviewView.Appearance.Row.Options.UseFont = true;
            reportDataPreviewView.Appearance.Row.Options.UseTextOptions = true;
            reportDataPreviewView.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            reportDataPreviewView.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            reportDataPreviewView.GridControl = reportDataPreviewGrid;
            reportDataPreviewView.IndicatorWidth = 45;
            reportDataPreviewView.Name = "reportDataPreviewView";
            reportDataPreviewView.OptionsBehavior.Editable = false;
            reportDataPreviewView.OptionsBehavior.ReadOnly = true;
            reportDataPreviewView.OptionsClipboard.AllowCopy = DevExpress.Utils.DefaultBoolean.True;
            reportDataPreviewView.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
            reportDataPreviewView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
            reportDataPreviewView.OptionsSelection.MultiSelect = true;
            reportDataPreviewView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            reportDataPreviewView.OptionsView.ShowGroupPanel = false;
            // 
            // ReportDataPreview
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1020, 640);
            Controls.Add(reportDataPreviewGrid);
            Name = "ReportDataPreview";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "XtraForm1";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)reportDataPreviewGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)reportDataPreviewView).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private DevExpress.XtraGrid.Views.Grid.GridView reportDataPreviewView;
        private MyGridControl reportDataPreviewGrid;
    }
}