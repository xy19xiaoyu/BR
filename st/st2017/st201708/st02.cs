﻿
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
    public class st02 : ST_Base
    {

        public st02()
        {
            this.Id = 2;
            this.Name = "城市-有效发明专利-申请人类型";
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

            string[] heads = new string[] { "工矿企业", "科研院所", "个人" };

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
                head1.CreateCell(col_index + 2).SetCellValue("");
                head1.GetCell(col_index + 0).CellStyle = column_headStyle;
                head1.GetCell(col_index + 1).CellStyle = column_headStyle;
                head1.GetCell(col_index + 2).CellStyle = column_headStyle;

                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIndex, rowIndex, col_index, col_index + 2));
                col_index += 3;
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
                    sheet.SetColumnWidth(col_index, 12 * 256);
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
select * from (select
	cn_pa.pa_type as 申请人类型,
	count(distinct cn_pa.an) as 申请量
from 
	cn
    join cn_pa on cn.an = cn_pa.an  
    join cn_lg on cn.an = cn_lg.an
where {0}  and cn.type= '发明专利' and cn.pdy <='{1}'  and cn_lg.dead_year >{1}
group by cn_pa.pa_type) as a
PIVOT (sum(申请量) for 申请人类型 in([工矿企业],[科研院所],[个人])) as table2";
                List<int> values = new List<int>();
                foreach (var year in config.Years)
                {
                    string exe_sql = string.Format(sql, name.Value, year);
                    DataTable tmpdt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_sql);
                    if (tmpdt.Rows.Count > 0)
                    {
                        values.Add(tmpdt.Rows[0]["工矿企业"].ToString().to_i());
                        values.Add(tmpdt.Rows[0]["科研院所"].ToString().to_i());
                        values.Add(tmpdt.Rows[0]["个人"].ToString().to_i());
                    }
                    else
                    {
                        values.Add(0);
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
