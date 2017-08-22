using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xyExtensions;
using Newtonsoft.Json;
namespace ST_2017
{
    [JsonObject(MemberSerialization.OptOut)]
    public class StatConfig
    {
        public List<string> Top5Guojia { get; set; }
        public List<string> Years { get; set; }
        public string Type { get; set; }
        public string FileName { get; set; }
        public string Dir { get; set; }
        public List<string> ZTNames { get; set; }
        public List<string> GuoJias { get; set; }
        public List<string> Shengs { get; set; }
        public List<string> Shis { get; set; }
        public List<string> ZLTypes { get; set; }
        public List<int> Tables { get; set; }
        public List<string> Xian { get; set; }
        public Dictionary<string, string> QuYu { get; set; }
        public List<string> Pas { get; set; }

       

        private string _stryears;
        public string strYears
        {
            get
            {
                if (string.IsNullOrEmpty(_stryears))
                {
                    _stryears = Years.to_s();
                }
                return _stryears;

            }
        }

        private string _zltype;
        [JsonIgnore]
        public string zltype
        {
            get
            {
                if (string.IsNullOrEmpty(_zltype))
                {
                    _zltype = ZLTypes.to_s();
                }
                return _zltype;
            }
        }

        private string _city;
        [JsonIgnore]
        public string city
        {
            get
            {
                if (string.IsNullOrEmpty(_city))
                {
                    _city = Shis.to_s();
                }
                return _city;
            }
        }



        private string _Sheng;
        [JsonIgnore]
        public string Sheng
        {
            get
            {
                if (string.IsNullOrEmpty(_Sheng))
                {
                    _Sheng = Shengs.to_s();
                }
                return _Sheng;
            }
        }
        private string _Shi;
        [JsonIgnore]
        public string Shi
        {
            get
            {
                if (string.IsNullOrEmpty(_Shi))
                {
                    _Shi = Shis.to_s();
                }
                return _Shi;
            }
        }
        private string _GuoJia;
        [JsonIgnore]
        public string GuoJia
        {
            get
            {
                if (string.IsNullOrEmpty(_GuoJia))
                {
                    _GuoJia = GuoJias.to_s();
                }
                return _GuoJia;
            }
        }
        private string _ztname;
        [JsonIgnore]
        public string ztname
        {
            get
            {
                if (string.IsNullOrEmpty(_ztname))
                {
                    _ztname = ZTNames.to_s();
                }
                return _ztname;
            }
        }

        private string _QuXian;
        [JsonIgnore]
        public string strQuXian
        {
            get
            {
                if (string.IsNullOrEmpty(_QuXian))
                {
                    _QuXian = Xian.to_s();
                }
                return _QuXian;
            }
        }
        private string _QuYu;
        [JsonIgnore]
        public string strQuYu
        {
            get
            {
                if (string.IsNullOrEmpty(_QuYu))
                {
                    _QuYu = Xian.to_s();
                }
                return _QuYu;
            }
        }
        private string _strPas;
        [JsonIgnore]
        public string strPas
        {
            get
            {
                if (string.IsNullOrEmpty(_strPas))
                {
                    _strPas = Pas.to_s();
                }
                return _strPas;
            }
        }
        private string _strTop5Guojia;

        public string StrTop5Guojia
        {
            get
            {
                if (string.IsNullOrEmpty(_strTop5Guojia))
                {
                    _strTop5Guojia = Top5Guojia.to_s();
                }
                return _strTop5Guojia;
            }

        }
    }
}
