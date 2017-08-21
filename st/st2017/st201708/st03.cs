
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using xyExtensions;

namespace ST_2017.ST201708
{
    public class st03 : ST_Base
    {

        public st03()
        {
            this.Id = 3;
            this.Name = "城市-新公开专利";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}", this.config.FileName, this.Name, DateTime.Now.ToString());
            return true;
        }



        public override bool OutPut2Worksheet(NPOI.XSSF.UserModel.XSSFWorkbook xbook)
        {
            ISheet sheet = xbook.CreateSheet(this.Name);
            IniStryle(xbook);


            #region 取得列宽
            //取得列宽

            string[] heads = new string[] { "新公开发明专利量", "新公开专利量" };

            #endregion
            #region 第一行
            int rowIndex = 0;
            XSSFRow head1 = sheet.CreateRow(rowIndex) as XSSFRow;
            head1.HeightInPoints = 21;
            int col_index = 1;
            foreach (var year in config.Years)
            {
                head1.CreateCell(col_index + 0).SetCellValue(year);
                head1.CreateCell(col_index + 1).SetCellValue("");
                head1.GetCell(col_index + 0).CellStyle = column_headStyle;
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
                    sheet.SetColumnWidth(col_index, 18 * 256);
                    col_index++;
                }
            }
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 1, 0, 0));
            head1.CreateCell(0).SetCellValue("国地区省市");
            head1.GetCell(0).CellStyle = column_headStyle;
            head2.CreateCell(0).SetCellValue("");
            head2.GetCell(0).CellStyle = column_headStyle;
            sheet.SetColumnWidth(0, 52 * 256);
            #endregion
            rowIndex++;

            foreach (var name in GetFilters())
            {
                string sql = @"
select *from
(select 
	type as 专利类型,
	count(*) as 公开量
from cn 
	where {0} and pdy = '{1}' 
group by type) as a
PIVOT (sum(公开量) for 专利类型 in([发明专利],[实用新型])) as table2";
                List<int> values = new List<int>();
                foreach (var year in config.Years)
                {
                    string exe_sql = string.Format(sql, name.Value, year);
                    DataTable tmpdt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_sql);
                    if (tmpdt.Rows.Count > 0)
                    {
                        int fm = tmpdt.Rows[0]["发明专利"].ToString().to_i();
                        int xx = tmpdt.Rows[0]["实用新型"].ToString().to_i();
                        values.Add(fm);
                        values.Add(fm + xx);
                    }
                    else
                    {
                        values.Add(0);
                        values.Add(0);
                    }
                }

                #region 数据行
                XSSFRow val_row = sheet.CreateRow(rowIndex) as XSSFRow;
                val_row.HeightInPoints = 21;
                val_row.CreateCell(0).SetCellValue(name.Key);
                val_row.GetCell(0).CellStyle = valueStyle_left;

                for (int i = 0; i < values.Count; i++)
                {
                    val_row.CreateCell(i + 1).SetCellValue(values[i]);
                    val_row.GetCell(i + 1).CellStyle = valueStyle;
                }
                #endregion
                rowIndex++;
                Console.WriteLine(name.ToString());
            }

            return true;

        }


        public Dictionary<string, string> GetFilters()
        {
            Dictionary<string, string> dics = new Dictionary<string, string>();
            foreach (var guojia in config.GuoJias)
            {
                dics.Add(guojia, " cn.country ='" + guojia + "'");
            }

            foreach (var QuYu in config.QuYu)
            {
                dics.Add(QuYu.Key, " cn.sheng in(" + QuYu.Value + ")");
            }

            foreach (var sheng in config.Shengs)
            {
                dics.Add(sheng + "省", " cn.sheng ='" + sheng + "'");
            }

            foreach (var shi in config.Shis)
            {
                dics.Add(shi + "市", " cn.shi ='" + shi + "'");
            }
            return dics;

        }

    }
}
