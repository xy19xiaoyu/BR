
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using NPOI.XSSF.UserModel;

namespace ST_2017.en
{
    public class en_108 : ST_Base
    {

        public en_108()
        {
            this.Id = 108;
            this.Name = "中国-50行业专利申请量";
            this.type = "EN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());

            string sql = @"
	select
	 en_zt.zt,
	 '' as 行业, 
	 count(distinct an) 申请量
	from 
		en,
		en_zt
	where 
		en.pn = en_zt.pn 
        and en.i_c = 'CN'
		and {0}
	group by 
	en_zt.zt
";

            string exe_sql = string.Format(sql, GetFilter());
            dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_sql);
            foreach (DataRow row in dt.Rows)
            {
                row["行业"] = $"{row[0].ToString().PadLeft(2, '0')}.{Hys[row[0].ToString()]}";
            }
            dt.Columns.RemoveAt(0);
            return true;
        }

        public override string GetFilter()
        {
            return $" en.p_c in({config.GuoJia}) ";
        }
    }
}
