using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRDB.Extend
{
    public static class IPCExtend
    {
        //格式化IPC，去掉空格，补0变成11位
        public static String FormatIPC(this String ipc)
        {
            //新数据IPC加入版本号H04L  1/18(2006.01)
            if (ipc.Contains('('))
            {
                ipc = ipc.Substring(0, ipc.IndexOf('(')).Trim();
            }

            String tem = ipc.Replace("-", "");
            String[] split = tem.Split('/');
            if (split.Length == 2)//如果IPC中包含/
            {
                String start4 = split[0].Substring(0, 4);//开始4位不变
                String mid3 = split[0].Substring(4).Trim().PadLeft(3, '0');//中间不足3位则左补0
                String last4 = split[1].Trim();
                return start4 + mid3 + last4.PadRight(4, '0');
            }
            else if (tem.Length > 4 && tem.Length < 7)
            {
                String start4 = split[0].Substring(0, 4);//开始4位不变
                String mid3 = split[0].Substring(4).Trim().PadLeft(3, '0');//中间不足3位则左补0
                return start4 + mid3 + "0".PadRight(4, '0');
            }
            else

            {
                return tem.PadRight(11, '0');
            }
        }
    }
}
