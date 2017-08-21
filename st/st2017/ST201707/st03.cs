
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;

namespace ST_2017.ST201707
{
    public class st03 : ST_Base
    {

        public st03()
        {
            this.Id = 3;
            this.Name = "(03)专利许可";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());
            string joinzt = " join cn_zt on cn_zt.an = cn.an";
            string sql = @"
with tmp as (
select
	cn.ady as 年代,
    cn.type as 专利类型,
	count(zl_xk.an) as 专利许可量
from 
	years left join cn on years.y = cn.ady 
	{1}   
    join zl_xk on cn.an = zl_xk.an    
where 1=1 {0}
group by cn.ady,cn.type)
select 年代,ISNULL([发明专利],0) as [发明专利],ISNULL([实用新型],0) as [实用新型]  from tmp
PIVOT (sum(专利许可量) for 专利类型 in([发明专利],[实用新型])) as table2
order by 年代
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
            dt = StateManager.AddSumRow(dt);
            return true;
        }

    }
}
