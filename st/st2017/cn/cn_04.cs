
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;

namespace ST_2017.cn
{
    public class cn_04 : ST_Base
    {

        public cn_04()
        {
            this.Id = 4;
            this.Name = "主要申请人";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());
            string sql = @"
select
	 cn_pa.pa as 申请人,
	 city as 国家,
	 count(*)  申请量
from 
	cn,
	cn_pa
where 
	cn.an = cn_pa.an
group by 
	cn_pa.pa, city
order by 国家,申请量 desc";

            dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, sql);
            return true;
        }

    }
}
