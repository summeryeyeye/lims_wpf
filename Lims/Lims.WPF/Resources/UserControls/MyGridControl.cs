using DevExpress.XtraGrid.Views.Grid;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Lims.WPF.Resources.UserControls
{
    public class MyGridControl : DevExpress.XtraGrid.GridControl
    {
        GridView gv = null;
        Point m_mouseDownLocation;
        int m_dragHandle;
        DragForm m_dragRowShadow;

        public MyGridControl()
        {
            Load += MyGridControl_Load;
        }
        private void MyGridControl_Load(object sender, EventArgs e)
        {
            if (gv == null)
            {
                gv = (GridView)MainView;
                gv.CustomDrawRowIndicator += gridView_CustomDrawRowIndicator;
                gv.IndicatorWidth = 45;
            }
        }
        protected override void OnMouseDown(MouseEventArgs ev)
        {
            if (ev.Button == MouseButtons.Left)
            {
                var _hit = gv.CalcHitInfo(ev.Location);
                if (_hit.RowHandle >= 0)
                {
                    m_dragHandle = _hit.RowHandle;
                    m_mouseDownLocation = ev.Location;
                }
                else
                {
                    m_dragHandle = -1;
                }
            }
            base.OnMouseDown(ev);
        }
        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
        protected override void OnMouseMove(MouseEventArgs ev)
        {
            if (ev.Button == MouseButtons.Left && m_dragHandle >= 0)
            {
                if (m_dragRowShadow == null)
                {
                    double _x2 = Math.Pow(ev.Location.X - m_mouseDownLocation.X, 2);
                    double _y2 = Math.Pow(ev.Location.Y - m_mouseDownLocation.Y, 2);
                    double _d2 = Math.Sqrt(_x2 + _y2);
                    if (_d2 > 3)
                    {
                        //执行拖拽；
                        BeginDrag(m_dragHandle);
                    }
                }
                else
                {
                    m_dragRowShadow.Location = new Point(m_dragRowShadow.Location.X, PointToScreen(ev.Location).Y);
                }
            }
            base.OnMouseMove(ev);
        }

        protected override void OnMouseUp(MouseEventArgs ev)
        {
            if (m_dragRowShadow != null)
            {
                var _hit = gv.CalcHitInfo(ev.Location);
                EndDrag(_hit.RowHandle);
            }
            base.OnMouseUp(ev);
        }

        private void BeginDrag(int _handle)
        {
            var _info = (DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo)gv.GetViewInfo();

            Rectangle _bound = _info.GetGridRowInfo(_handle).Bounds;
            _bound.Location = PointToScreen(_bound.Location);
            m_dragRowShadow = new DragForm(_bound);
            m_dragRowShadow.Show();
        }

        private void EndDrag(int _handle)
        {
            if (m_dragRowShadow != null)
            {
                m_dragRowShadow.Close();
                m_dragRowShadow.Dispose();
                m_dragRowShadow = null;

                int _rowIndex = gv.GetDataSourceRowIndex(m_dragHandle);

                DataRow _row = ((DataTable)DataSource).Rows[_rowIndex];
                object[] _values = _row.ItemArray;

                base.BeginUpdate();

                //移除目标行；
                ((DataTable)DataSource).Rows.RemoveAt(m_dragHandle);
                _row = ((DataTable)DataSource).NewRow();
                _row.ItemArray = _values;
                if (_handle >= 0)
                {
                    //插入指定位置；
                    ((DataTable)DataSource).Rows.InsertAt(_row, _handle);
                    gv.FocusedRowHandle = _handle;
                }
                else
                {
                    //添加；
                    ((DataTable)DataSource).Rows.Add(_row);
                    gv.FocusedRowHandle = gv.RowCount - 1;
                }
                //DataTable dt = this.DataSource as DataTable;
                //JosnConfigHelper.WriteConfig(dt);//修改本地数据
                base.EndUpdate();
            }
        }

    }
}
