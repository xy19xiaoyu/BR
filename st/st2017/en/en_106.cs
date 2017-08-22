
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;

namespace ST_2017.en
{
    public class en_106 : ST_Base
    {

        public en_106()
        {
            this.Id = 106;
            this.Name = "中国-专利申请情况";
            this.type = "EN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());
            string sql = @"
select 
    convert(varchar,ady) as 申请年,
	COUNT(*) as 申请量
from
 en 
where 
	i_c = 'CN' 
	and {0}
group by ady
";

            string exe_sql = string.Format(sql, GetFilter());
            dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_sql);
            dt = AddSumRow(dt);
            return true;
        }

        public override string GetFilter()
        {
            return $" en.p_c in({config.GuoJias}) ";
        }
    }
}
