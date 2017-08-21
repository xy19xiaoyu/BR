using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace xyExtensions
{
    public static class stringExtensions
    {
        public static Dictionary<string, Regex> regs = new Dictionary<string, Regex>();
        static stringExtensions()
        {
            regs.Add("大专院校", new Regex("(大学|学院|学校|设计院|中学|小学)"));
            regs.Add("工矿企业", new Regex("(公司|厂|会社|基地|集团|公旬|合作社)"));
            regs.Add("事业单位", new Regex("(医院|部队|解放军|局|政府)"));
            regs.Add("科研单位", new Regex("(科学院|工程院|研究|科研|研制|财团)"));
        }
        public static string Left(this string obj, int length)
        {
            if (obj.Length > length)
            {
                return obj.Substring(0, length);
            }
            else
            {
                return obj;
            }
        }
        public static string to_s(this List<string> list, char c = ',')
        {
            if (list == null) return "";
            string s = "";
            foreach (var x in list)
            {
                s += "'" + x + "'" + c;
            }
            return s.Trim(c);
        }
        public static string to_s(this List<int> list, char c = ',')
        {
            if (list == null) return "";
            string s = "";
            foreach (var x in list)
            {
                s += "'" + x.ToString() + "'" + c;
            }
            return s.Trim(c);
        }
        
        /// <summary>
        /// 申请号8位转12位
        /// </summary>
        /// <param name="_strApNo">申请号为8位或12位</param>
        /// <returns></returns>
        public static string ApNo8To12(this string obj)
        {
            if (obj == null)
            {
                throw new Exception("申请号格式错误！");
            }
            string strRetu = "";
            strRetu = obj.Trim();
            switch (strRetu.Length)
            {
                case 8:
                case 9:
                    if (strRetu.Substring(0, 1) == "0")
                    {
                        strRetu = string.Format("20{0}00{1}", obj.Substring(0, 3), obj.Substring(3, 5));
                    }
                    else
                    {
                        strRetu = string.Format("19{0}00{1}", obj.Substring(0, 3), obj.Substring(3, 5));
                    }
                    break;
                case 12:
                    break;
                case 13:
                    strRetu = strRetu.Substring(0, 12);
                    break;
                default:
                    throw new Exception("申请号格式错误！");
            }

            return strRetu;
        }

        public static string FormatDate(this string obj)
        {
            if (obj == null) return string.Empty;
            string date = obj.ToString().Trim();
            if (string.IsNullOrEmpty(date.Trim()))
            {
                return date;
            }
            else
            {
                if (obj.Length == 8) return obj;

                try
                {
                    DateTime dt = DateTime.Parse(date);
                    return dt.ToString("yyyyMMdd");
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        public static string GetYear(this string obj)
        {
            if (obj == null) return string.Empty;
            return obj.ToString().Trim().Substring(0, 4);
        }
        public static string MatchPaType(this string pa)
        {
            string type = "";
            bool isMatch = false;
            if (pa.Length <= 3)
            {
                type = "个人";
            }
            else
            {
                Dictionary<string, int> typeindex = new Dictionary<string, int>();
                foreach (var x in regs)
                {
                    Match mh = x.Value.Match(pa);
                    if (mh.Success)
                    {
                        type = x.Key;
                        isMatch = true;
                        typeindex.Add(x.Key, mh.Index);
                    }
                    else
                    {
                        typeindex.Add(x.Key, 0);
                    }
                }
                var RightIndex = (from y in typeindex
                                  orderby y.Value descending
                                  select y).First();
                type = RightIndex.Key;

                if (!isMatch)
                {
                    //带点儿的 英文名字 或者 4个字的人名，小日本名字
                    if (pa.IndexOf("·") > 0 || pa.Length == 4)
                    {
                        type = "个人";
                    }
                    else
                    {
                        type = "其它";
                    }
                }
            }
            return type;
        }

        public static long to_l(this string str)
        {
            long value = 0;
            long.TryParse(str, out value);
            return value;
        }
        public static int to_i(this double d)
        {
            return (int)d;
        }
        public static int to_i(this string val)
        {
            int value = 0;
            if (string.IsNullOrEmpty(val))
            {
                return value;
            }
            else
            {
                int.TryParse(val, out value);
            }
            return value;
        }
        public static int to_i(this object o)
        {
            return o.ToString().to_i();
        }
    }
}
