
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using NPOI.XSSF.UserModel;
using xyExtensions;
using NPOI.SS.UserModel;

namespace ST_2017.en
{
    public class en_105 : ST_Base
    {

        public en_105()
        {
            this.Id = 105;
            this.Name = "行业-逐年专利申请总量";
            this.type = "EN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            return true;
        }
        public override bool OutPut2Worksheet(XSSFWorkbook xbook)
        {

            List<string> heads = new List<string> { "行业" };
            heads.AddRange(config.Years);

            Console.WriteLine("开始出表：{0} ", Name);


            ISheet sheet = xbook.CreateSheet(this.Name);
            IniStryle(xbook);

            #region 第一行
            int rowIndex = 0;
            XSSFRow head2 = sheet.CreateRow(rowIndex) as XSSFRow;
            head2.HeightInPoints = 21;
            int col_index = 0;
            for (int i = 0; i < heads.Count; i++)
            {
                head2.CreateCell(col_index).SetCellValue(heads[i]);
                head2.GetCell(col_index).CellStyle = column_headStyle;
                sheet.SetColumnWidth(col_index, 12 * 256);
                col_index++;
            }
            sheet.SetColumnWidth(0, 52 * 256);
            #endregion
            rowIndex = 1;
            foreach (var hy in Hys)
            {
                #region 获取行业申请人类型
                string pas = string.Format(@"
select
	en.ady as 申请年,
	COUNT(distinct en.an) as 申请量
from
	en,
	en_zt
where 
	en.pn = en_zt.pn
	and en_zt.zt in({0})
	and {1}
group by
	en.ady
order by 申请量 desc", hy.Key, GetFilter());
                DataTable dtpas = DBA.SqlDbAccess.GetDataTable(CommandType.Text, pas);
                #endregion
                Dictionary<string, string> values = new Dictionary<string, string>();
                foreach (var year in config.Years)
                {
                    values.Add(year, "0");
                }
                #region 创建表格
                XSSFRow xls_row = sheet.CreateRow(rowIndex) as XSSFRow;
                xls_row.CreateCell(0).SetCellValue(Hy(hy.Key));
                xls_row.GetCell(0).CellStyle = valueStyle_left;
                for (int j = 0; j < config.Years.Count; j++)
                {
                    xls_row.CreateCell(j + 1).SetCellValue(0);
                    xls_row.GetCell(j + 1).CellStyle = valueStyle;
                }
                #endregion
                #region 赋值
                foreach (DataRow parow in dtpas.Rows)
                {
                    string ady = parow["申请年"].ToString();
                    string count = parow["申请量"].ToString();
                    if (values.ContainsKey(ady))
                    {
                        values[ady] = count;
                    }
                }
                int tmpi = 1;
                foreach (var kv in values)
                {
                    xls_row.GetCell(tmpi).SetCellValue(kv.Value);
                    tmpi++;
                }
                #endregion
                Console.WriteLine(this.Name + "\t" + hy.Value);
                rowIndex++;
            }
            Console.WriteLine("结束出表：{0} ", Name);
            return true;
        }

        public override string GetFilter()
        {
            return $" en.p_c in({config.GuoJia}) ";
        }
    }
}
