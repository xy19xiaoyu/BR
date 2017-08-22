
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using NPOI.XSSF.UserModel;

namespace ST_2017.en
{
    public class en_110 : ST_Base
    {

        public en_110()
        {
            this.Id = 110;
            this.Name = "国外前5国-申请量";
            this.type = "EN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());

            string sql = @"
select * from (
	select
	     i_c as 国家,
	     ady as 申请年, 
	     count(distinct an) 申请量
	from 
		en
	where 
		{0}
	group by 
	    i_c,ady) as a1
PIVOT(sum(申请量) for 国家 in({1}) as table2
order by 申请年
";

            string exe_sql = string.Format(sql, GetFilter(), GetCountry());
            dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_sql);
            return true;
        }

        public override string GetFilter()
        {
            return $" p_c='UA' and  i_c  in ('DE','FR','CH','US','JP') ";
            //and i_c not in ('US','JP','','UA')
        }

        public string GetCountry()
        {
            return string.Format("[DE],[FR],[CH],[US],[JP]");
        }
    }
}
