
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using xyExtensions;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace ST_2017.ST201707
{
    public class st11 : ST_Base
    {

        public st11()
        {
            this.Id = 11;
            this.Name = "(11)有效发明专利量前15申请人";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            this.ztname = ztName;
            return true;
        }
        public override bool OutPut2Worksheet(NPOI.XSSF.UserModel.XSSFWorkbook xbook)
        {
            ISheet sheet = xbook.CreateSheet(this.Name);
            IniStryle(xbook);


            #region 取得列宽
            //取得列宽

            string[] heads = new string[] { "申请人", "申请量" };
            #endregion
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
            rowIndex++;
            foreach (var name in config.ZTNames)
            {
                for (int i = 0; i < 15; i++)
                {

                    XSSFRow val_row = sheet.CreateRow(rowIndex + i) as XSSFRow;
                    val_row.HeightInPoints = 21;
                    val_row.CreateCell(0).CellStyle = valueStyle_left;
                    for (int j = 0; j < config.Years.Count; j++)
                    {
                        val_row.CreateCell(1 + (j * 2)).CellStyle = valueStyle_left;
                        val_row.CreateCell(1 + (j * 2) + 1).CellStyle = valueStyle;
                    }
                }
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIndex, rowIndex + 14, 0, 0));
                sheet.GetRow(rowIndex).CreateCell(0).SetCellValue(Hys[name]);

                int column = 1;
                foreach (var year in config.Years)
                {
                    string sql = @"
select top 15 * from (
    select
        cn_pa.pa  as 申请人,	
        count(distinct cn_pa.an) as 申请量
    from 
        cn 
        join cn_pa on cn.an = cn_pa.an
        join cn_zt on cn.an = cn_zt.an
        join cn_lg on cn.an = cn_lg.an
        where
            cn_zt.ztname= '{0}' and cn.pdy <='{1}'  and cn_lg.dead_year >{1} and cn.type= '发明专利' {2}
        group by
             cn_pa.pa
    ) as a
order by 申请量 desc
";
                    string exe_sql = "";
                    exe_sql = string.Format(sql, name, year, GetFilter());
                    DataTable tmpdt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_sql);
                    for (int i = 0; i < tmpdt.Rows.Count; i++)
                    {
                        XSSFRow row = sheet.GetRow(rowIndex + i) as XSSFRow;
                        row.GetCell(column).SetCellValue(tmpdt.Rows[i]["申请人"].ToString());
                        row.GetCell(column + 1).SetCellValue(tmpdt.Rows[i]["申请量"].to_i());
                    }
                    column += 2;
                }
                rowIndex += 15;
                Console.WriteLine(Hys[name.ToString()]);
            }

            return true;

        }
        public override string GetFilter()
        {
            string where = "";
            if (!string.IsNullOrEmpty(config.GuoJia))
            {
                where += " and cn.guojia in(" + config.GuoJia + ")";
            }

            if (!string.IsNullOrEmpty(config.Sheng))
            {
                where += " and cn.sheng in(" + config.Sheng + ")";
            }
            if (!string.IsNullOrEmpty(config.Shi))
            {
                where += " and cn.Shi in (" + config.Shi + ")";
            }
            if (!string.IsNullOrEmpty(config.strQuXian))
            {
                where += " and cn.Xian in (" + config.strQuXian + ")";
            }
            if (!string.IsNullOrEmpty(config.strQuYu))
            {
                where += " and cn.QuYu in(" + config.strQuYu + ")";
            }

            if (!string.IsNullOrEmpty(config.zltype))
            {
                where += " and cn.type in(" + config.zltype + ")";
            }

            if (!string.IsNullOrEmpty(config.strPas))
            {
                where += " and cn_pa.pa in(" + config.strPas + ")";
            }
            return where;
        }

        public override void Dispose()
        {
            dt = null;
        }
    }
}
