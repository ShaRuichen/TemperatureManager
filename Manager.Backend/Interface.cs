using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.Types.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Collection.Generic;
using System.Math;
namespace Manager
{
   
        //查看某天
        //当天所有数据
        public Dictionary<string,int> lookupforday(string year,string month,string day)
        {
            Dictionary<string,int> t;//("时：分",温度）
            var tem=Sql.Execute("SELECT temperature,hour,minute FROM data WHERE year=@0, month=@1,day=@2",year,month,day);
            foreach(DataRow n in tem)
            {
                string time = Concat(n[1],":" ,n[2]);
                t.Add(time,Convert.ToInt32(n[0]));
            }
            return t;
        }

        //查看某月
        //每天的平均温度
        public Dictionary<string,int> lookupformonth(string year, string month)
        {
            Dictionary<string,int> t;
            int mon = Convert.ToInt32(month);
            int days = dayformonth(year,mon);
            for (int i = 0; i < days; i++)
            {
                var tem = Sql.Execute("SELECT temperature FROM data WHERE year=@0, month=@1,day=@2", year, month, i);
                int min = 60;
                int max = -30;
                int sum = 0;
                foreach (DataRow n in tem)
                {
                    int thistem = Convert.ToInt32(n[0]);
                    sum += thistem;
                    if(min> thistem){
                        min = thistem;
                    }
                    if (max < thistem)
                    {
                        max = thistem;
                    }
                }
                int i_ = i + 1;
                string time = Concat(i_.ToString(),"日");
                t.Add(Concat(time,"最低温度"),min);
                t.Add(Concat(time, "平均温度"),sum / 96);
                t.Add(Concat(time, "最高温度"), max);
            }
            return t;
            //每三个数据是一天，第一个是最低温，第二个是平均温，第三个是最高温
        }

        //查看某年
        public Dictionary<string,int> lookupforyear(string year)
        {
            Dictionary<string,int> result;
            for(int i = 0; i <= 12; i++)
            {
                int sum = 0;
                int min = 60;
                int max = -30;
                Dictionary <string,int> t = lookupformonth(year, i.ToString());
                int days = dayformonth(year,i);
                for(int j = 0;j < days; j++)
                {
                    int j_ = j + 1;
                    sum += t[Concat(j.ToString(), "日平均温度")];
                    int dmin = t[Concat(j.ToString(), "日最低温度")];
                    int dmax = t[Concat(j.ToString(), "日最高温度")];
                    if (max < dmax)
                        max = dmax;
                    if (min > dmin)
                        min = dmin;
                }
                int i_ = i + 1;
                result.Add(Concat(i_.ToString,"月最低温度"),min);
                result.Add(Concat(i_.ToString, "月平均温度"), sum /days);
                result.Add(Concat(i_.ToString, "月最高温度"), max);
            }
            return result;
        }


        //预测某天
        public Dictionary<string,int> forecast(string month, string day)
        {
            double[] result;
            //把当天数据取出来放进去
            double[] y = new double[96];
            double[] x = new double[96];
            var tem = Sql.Execute("SELECT temperature FROM data WHERE  month=@0,day=@1", month, day);
            int con = 0;
            foreach (DataRow n in tem)
            {
                y[con % 96] += Convert.ToInt32(n[0]);
                con++;
            }
            for (int i = 0; i < 96; i++)
            {
                y[i] = y[i] / 3;
                x[i] = i * 15;
            }
            result = MultiLine(x, y, con, 5);
            Dictionary<string, int> t;
            for (int i = 0; i < 96; i++) {
                int temp = 0;
                for (int j = 0; i < 5; j++) {
                    tem += result[j]*Math.Pow(i * 15, j);
                }
                int hour = i *15/ 60;
                int time = (i * 15) % 60;
                t.Add(Concat(hour.ToString(),":",time.ToString()),temp);
            }
            return t;
        }
        private int dayformonth(string year,int mon)
        {
            int days;
            if ((mon % 2 == 1 && mon <= 7) || (mon % 2 == 0 && mon >= 8))
            {
                days = 31;
            }
            else if (mon == 2)
            {
                if (Convert.ToInt32(year) % 4 == 0)
                {
                    days = 29;
                }
                else
                    days = 28;
            }
            else
            {
                days = 30;
            }
            return days;
        }
    
}