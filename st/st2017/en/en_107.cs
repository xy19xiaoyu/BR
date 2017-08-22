
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;

namespace ST_2017.en
{
    public class en_107 : ST_Base
    {

        public en_107()
        {
            this.Id = 107;
            this.Name = "中国-申请专利";
            this.type = "EN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());
            string sql = @"
select 
	pn as 公开号,
	an as 申请号,
	ti as 专利名称
from
	en 
where 
	i_c = 'CN' 
	and {0}
";

            string exe_sql = string.Format(sql, GetFilter());
            dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_sql);
            return true;
        }

        public override string GetFilter()
        {
            return $" en.p_c in({config.GuoJias}) ";
        }
    }
}
