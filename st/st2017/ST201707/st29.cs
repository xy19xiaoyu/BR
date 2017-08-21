
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using xyExtensions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ST_2017.ST201707
{
    public class st29 : ST_Base
    {

        public st29()
        {
            this.Id = 29;
            this.Name = "29-医药制造业申请年-IPC大类";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            return true;
        }
        public override bool OutPut2Worksheet(XSSFWorkbook xbook)
        {

            string[] heads = new string[] { "IPC", "有效专利量" };
            Dictionary<string, string> hys = new Dictionary<string, string>();
            hys.Add("48", "27.医药制造业");

            Console.WriteLine("开始出表：{0}\t{1} ", Name, ztname);

            #region 生成表单逻辑

            ISheet sheet = xbook.CreateSheet(this.Name);
            IniStryle(xbook);


            #region 第一行
            int rowIndex = 0;
            XSSFRow head1 = sheet.CreateRow(rowIndex) as XSSFRow;
            head1.HeightInPoints = 21;
            int col_index = 1;
            foreach (var year in config.Years)
            {
                head1.CreateCell(col_index).SetCellValue(year);
                head1.CreateCell(col_index + 1).SetCellValue("");
                head1.GetCell(col_index).CellStyle = column_headStyle;
                head1.GetCell(col_index + 1).CellStyle = column_headStyle;
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIndex, rowIndex, col_index, col_index + 1));
                col_index += 2;
            }
            #endregion

            #region 第二行
            rowIndex = 1;
            XSSFRow head2 = sheet.CreateRow(rowIndex) as XSSFRow;
            head2.HeightInPoints = 21;
            col_index = 1;
            foreach (var year in config.Years)
            {
                for (int i = 0; i < heads.Length; i++)
                {
                    head2.CreateCell(col_index).SetCellValue(heads[i]);
                    head2.GetCell(col_index).CellStyle = column_headStyle;
                    sheet.SetColumnWidth(col_index, 16 * 256);
                    col_index++;
                }
            }
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 1, 0, 0));
            head1.CreateCell(0).SetCellValue("行业");
            head1.GetCell(0).CellStyle = column_headStyle;
            head2.CreateCell(0).SetCellValue("");
            head2.GetCell(0).CellStyle = column_headStyle;
            sheet.SetColumnWidth(0, 52 * 256);
            #endregion
            rowIndex = 2;
            foreach (var hy in hys)
            {
                #region 创建表格
                for (int i = 0; i < 10; i++)
                {
                    XSSFRow xls_row = sheet.CreateRow(rowIndex + i) as XSSFRow;
                    xls_row.CreateCell(0).SetCellValue(hy.Value.ToString());
                    xls_row.GetCell(0).CellStyle = valueStyle_left;

                    col_index = 1;
                    foreach (var year in config.Years)
                    {
                        foreach (var head in heads)
                        {
                            xls_row.CreateCell(col_index).SetCellValue("");
                            xls_row.GetCell(col_index).CellStyle = valueStyle;
                            sheet.SetColumnWidth(col_index, 16 * 256);
                            col_index += 1;
                        }
                    }
                }
                #endregion
                #region 合并单元格
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIndex, rowIndex + 9, 0, 0));
                #endregion
                col_index = 1;
                foreach (var year in config.Years)
                {
                    #region 获取行业某一年IPC有效专利
                    string top10ipc = string.Format(@"
                        select 
	                        top 10 
	                        cn_ipc.ipc3 ,
	                        count(distinct cn.an) 数量 
                        from 
	                        cn_zt,
	                        cn,
                            cn_ipc,
	                        cn_lg 
                        where 
                            cn_zt.an = cn.an     
	                        and cn_zt.an = cn_ipc.an  
	                        and cn_zt.an = cn_lg.an	                        
	                        and cn_zt.ztname ='{0}'
                            and cn.ady = '{1}' {2}
                            and cn_lg.ValidStatus in('有效','审中')         
                        group by cn_ipc.ipc3
                        order by 数量 desc", hy.Key, year, GetFilter5_1());

                    DataTable dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, top10ipc);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];
                        XSSFRow xls_row = sheet.GetRow(rowIndex + i) as XSSFRow;
                        xls_row.GetCell(col_index + 0).SetCellValue(row["ipc3"].ToString());
                        xls_row.GetCell(col_index + 1).SetCellValue(row["数量"].ToString());
                    }
                    col_index += 2;
                    #endregion
                }

                rowIndex += 10;
                Console.WriteLine(this.Name + "\t" + hy.Value);
            }
            Console.WriteLine("结束出表：{0}\t{1} ", Name, ztname);
            #endregion

            return true;
        }
        public override void Dispose()
        {
            dt = null;
        }
    }
}
