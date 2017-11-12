
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;

namespace ST_2017.en
{
    public class en_115 : ST_Base
    {

        public en_115()
        {
            this.Id = 115;
            this.Name = "专利申请-本国申请人申请情况";
            this.type = "EN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());
            string sql = @"
select
	ady as 年代, 
	count(distinct an) 申请量
from 
	en
where 
	{0}
group by 
	p_c,ady
order by ady
";

            string exe_sql = string.Format(sql, GetFilter());
            dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_sql);
            ReadDateTable(dt);
            return true;
        }

        public override string GetFilter()
        {
            return $" en.p_c in({config.GuoJia}) and en.i_c in({ config.GuoJia})";
        }
    }
}
