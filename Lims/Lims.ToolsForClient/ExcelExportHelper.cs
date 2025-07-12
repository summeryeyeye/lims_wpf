using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;

namespace Lims.ToolsForClient
{
    public class ExcelExportHelper
    {
        //委托
        public delegate void ExportResult(bool res);

        public event ExportResult ExportResultEvent;

        /// <summary>
                /// 要导出的Excel对象
                /// </summary>
        private HSSFWorkbook workbook = null;

        /// <summary>
        /// 要导出的Excel对象属性
        /// </summary>
        private HSSFWorkbook Workbook
        {
            get
            {
                if (workbook == null)
                {
                    workbook = new HSSFWorkbook();
                }
                return workbook;
            }
            set
            {
                workbook = value;
            }
        }

        ///// <summary>
        //        /// 设置Excel文件基本属性
        //        /// </summary>
        //        /// <param name="ep">属性</param>
        //public void SetExcelProperty(ExcelProperty ep)
        //{
        //    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
        //    dsi.Company = ep.Company;//填加xls文件公司信息
        //    Workbook.DocumentSummaryInformation = dsi;

        //    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
        //    si.Author = ep.Author; //填加xls文件作者信息
        //    si.ApplicationName = ep.ApplicationName; //填加xls文件创建程序信息
        //    si.Comments = ep.Comments; //填加xls文件备注
        //    si.Title = ep.Title; //填加xls文件标题信息
        //    si.Subject = ep.Subject;//填加文件主题信息
        //    si.CreateDateTime = DateTimeOffset.Now;
        //    Workbook.SummaryInformation = si;
        //}

        /// <summary>
                /// 泛型列表List导出到Excel文件
                /// </summary>
                /// <param name="list">源List表</param>
                /// <param name="strHeaderText">标题信息</param>
                /// <param name="strFileName">保存路径</param>
                /// <param name="titles">列名</param>
        [Obsolete]
        public void ExportToFile<T>(List<T> list, string strHeaderText, string strFileName, string[] titles = null)
        {
            try
            {
                //转换数据源
                DataTable dtSource = ListToDataTable(list, titles);
                //开始导出
                Export(dtSource, strHeaderText, strFileName);
                System.GC.Collect();
                ExportResultEvent?.Invoke(true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //if (LogHelper != null)
                //    LogHelper.Error(string.Format("ExportToFile error:{0}", ex));
                //ExportResultEvent?.Invoke(false);
            }
        }

        /// <summary>
                /// DataTable导出到Excel文件
                /// </summary>
                /// <param name="dtSource">源DataTable</param>
                /// <param name="strHeaderText">标题信息</param>
                /// <param name="strFileName">保存路径</param>
        [Obsolete]
        public void Export(DataTable dtSource, string strHeaderText, string strFileName)
        {
            using (MemoryStream ms = Export(dtSource, strHeaderText))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }

        /// <summary>
                /// DataTable导出到Excel的MemoryStream
                /// </summary>
                /// <param name="dtSource">源DataTable</param>
                /// <param name="strHeaderText">标题信息</param>
        [Obsolete]
        private MemoryStream Export(DataTable dtSource, string strHeaderText)
        {
            ISheet sheet = Workbook.CreateSheet();
            ICellStyle dateStyle = Workbook.CreateCellStyle();
            IDataFormat format = Workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = Workbook.CreateSheet();
                    }

                    #region 表头及样式
                    {
                        IRow headerRow = sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        ICellStyle headStyle = Workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = Workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;
                        //CellRangeAddress四个参数为：起始行，结束行，起始列，结束列
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                    }
                    #endregion 表头及样式

                    #region 列头及样式
                    {
                        IRow headerRow = sheet.CreateRow(1);
                        ICellStyle headStyle = Workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = Workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                        }
                    }
                    #endregion 列头及样式

                    rowIndex = 2;
                }
                #endregion

                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    ICell newCell = dataRow.CreateCell(column.Ordinal);
                    string drValue = row[column].ToString();
                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型
                            newCell.SetCellValue(drValue);
                            break;

                        case "System.DateTime"://日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);
                            newCell.CellStyle = dateStyle;//格式化显示
                            break;
                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;

                        case "System.Int16"://整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;

                        case "System.Decimal"://浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;

                        case "System.DBNull"://空值处理
                            newCell.SetCellValue("");
                            break;

                        default:
                            newCell.SetCellValue("");
                            break;
                    }
                }
                #endregion

                rowIndex++;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                Workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }

        /// <summary>
                /// 泛型列表List转换为DataTable
                /// </summary>
                /// <typeparam name="T">泛型实体</typeparam>
                /// <param name="list">要转换的列表</param>
                /// <param name="titles">标题</param>
                /// <returns></returns>
        public DataTable ListToDataTable<T>(List<T> list, string[] titles)
        {
            DataTable dt = new DataTable();
            Type listType = typeof(T);
            PropertyInfo[] properties = listType.GetProperties();
            //标题行
            if (titles != null && properties.Length == titles.Length)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo property = properties[i];
                    dt.Columns.Add(new DataColumn(titles[i], property.PropertyType));
                }
            }
            else
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo property = properties[i];
                    dt.Columns.Add(new DataColumn(property.Name, property.PropertyType));
                }
            }
            //内容行
            foreach (T item in list)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dr[i] = properties[i].GetValue(item, null);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }



        public static void ExportListToExcelXlsx<T>(List<T> data,string tableName, string filePath)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                var properties = typeof(T).GetProperties();
                worksheet.Cells[1, 1].Value = tableName;
                // 表头
                for (int i = 0; i < properties.Length; i++)
                    worksheet.Cells[2, i + 1].Value = properties[i].Name;

                // 数据
                for (int row = 0; row < data.Count; row++)
                    for (int col = 0; col < properties.Length; col++)
                        worksheet.Cells[row + 3, col + 1].Value = properties[col].GetValue(data[row], null);

                package.SaveAs(new FileInfo(filePath));
            }
        }



    }
}