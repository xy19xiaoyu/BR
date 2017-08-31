
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ST_2017.cn
{
    public class cn_03 : ST_Base
    {

        public cn_03()
        {
            this.Id = 3;
            this.Name = "申请专利";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());
            string sql = @"
select 
	cn.an 申请号,
	cn.type 专利类型,
	cn.ad 申请日,
	country 来源国家,
    cn_pa.pa as 申请人,
    cn_biblio.ti as 标题,
    cn_biblio.abs as 摘要
    
from 
    cn,
    cn_biblio,
    cn_pa
where 
    cn.sn = cn_biblio.sn
    and cn.an = cn_pa.an
    and cn_pa.sort = 0
order by cn.country,cn.ady";

            dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, sql);
            return true;
        }
        public override bool MergeCell(XSSFWorkbook xbook)
        {
            ISheet sheet = xbook.GetSheet(this.Name);
            sheet.SetColumnWidth(4, 50 * 256);
            sheet.SetColumnWidth(5, 100 * 256);
            sheet.SetColumnWidth(5, 120 * 256);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                XSSFRow xls_row = sheet.GetRow(i + 1) as XSSFRow;
                xls_row.GetCell(4).CellStyle = valueStyle_left;
                xls_row.GetCell(5).CellStyle = valueStyle_left;
                xls_row.GetCell(6).CellStyle = valueStyle_left;
            }

            return true;
        }
    }
}
