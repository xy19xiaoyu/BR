
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;

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
    cn_biblio.ti as 标题,
    cn_biblio.abs as 摘要,
    cn_pa.pa as 申请人
from 
    cn,
    cn_biblio,
    cn_pa
where 
    cn.sn = cn_biblio.sn
    and cn.an = cn_pa.an
    and cn_pa.sort = 0
order by country";

            dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, sql);
            return true;
        }

    }
}
