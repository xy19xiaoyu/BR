using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.XSSF.UserModel;

namespace ST_2017.Interface
{
    public interface IState : IDisposable
    {

        /// <summary>
        /// ID
        /// </summary>
        int Id
        {
            get;
        }

        /// <summary>
        /// 表名
        /// </summary>
        string Name
        {
            get;
            set;
        }
        string type
        {
            get;
        }
        /// <summary>
        /// 描述
        /// </summary>
        string DES
        {
            get;

        }
        string Sheng
        {
            get;
            set;
        }
        string Shi
        {
            get;
            set;
        }
        string GuoJia
        {
            get;
            set;
        }
        string city
        {
            get;
            set;
        }
        bool Sate(string ztName);
        bool Sate(string ztName, string city);
        string zltype
        {
            get;
            set;
        }
        List<string> lstPa
        {
            get;
            set;
        }
        string Pa
        {
            get;
            set;
        }
        /// <summary>
        /// 将统计结果输出指ExcelApp的一个Sheet中
        /// </summary>
        /// <param name="xApp"></param>
        /// <returns></returns>
        bool OutPut2Worksheet();
    }
}
