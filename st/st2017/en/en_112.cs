
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
    public class en_112 : ST_Base
    {

        public en_112()
        {
            this.Id = 112;
            this.Name = "国外前5国-行业";
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
            heads.AddRange(config.GuoJias);

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
                #region 获取行业
                string pas = string.Format(@"
select * from (
	select
		en.i_c as 国家,
		COUNT(distinct en.an) as 申请量
	from
		en
	where 
		{0}
	group by
		en.i_c
) as a
PIVOT(sum(申请量) for 国家 in({0})) as table2", hy.Key, GetFilter(), GetCountry());
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
                for (int j = 0; j < heads.Count; j++)
                {
                    xls_row.GetCell(j + 1).SetCellValue(0);
                    xls_row.GetCell(j + 1).CellStyle = valueStyle;
                }
                #endregion
                #region 赋值
                int tmpi = 1;
                foreach (DataRow parow in dtpas.Rows)
                {
                    string ady = parow["国家"].ToString();
                    string count = parow["申请量"].ToString();
                    xls_row.GetCell(tmpi).SetCellValue(count);
                    tmpi++;
                }
                #endregion
                rowIndex++;
                Console.WriteLine(this.Name + "\t" + hy.Value);
            }
            Console.WriteLine("结束出表：{0} ", Name);
            return true;
        }

        public override string GetFilter()
        {
            return $" p_c in({config.GuoJia} and  i_c  in ({config.StrTop5Guojia})";
        }

        public string GetCountry()
        {            
            StringBuilder sb = new StringBuilder();
            foreach (var gj in config.GuoJias)
            {
                sb.Append($"[{gj}],");
            }            
            return sb.ToString(0, sb.Length - 1);
        }
    }
}
