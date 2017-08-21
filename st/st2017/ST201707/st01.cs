
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;

namespace ST_2017.ST201707
{
    public class st01 : ST_Base
    {

        public st01()
        {
            this.Id = 1;
            this.Name = "(01)申请量";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());
            string joinzt = " join cn_zt on cn_zt.an = cn.an";
            string sql = @"
select
	years.y as 年代,
	count(cn.an) as 申请量
from 
	years left join cn on years.y = cn.ady    
    {1}
where 1=1 {0}
group by years.y
order by years.y
";
            if (string.IsNullOrEmpty(config.ztname))
            {
                sql = string.Format(sql, GetFilter(), "");
            }
            else
            {
                sql = string.Format(sql, GetFilter(), joinzt);
            }


            dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, sql);
            dt = StateManager.ReadDateTable(dt);
            dt = StateManager.AddSumRow(dt);
            return true;
        }

    }
}
