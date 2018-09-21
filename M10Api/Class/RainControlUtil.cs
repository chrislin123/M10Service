using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M10Api.Class
{
    public class RainControlUtil
    {


        static public decimal CalcRainAvg(List<decimal> DataList) {

            decimal dResult = 0;
            dResult = DataList.Where(s => s != -99).Sum(t => t);
            int iCount = DataList.Where(s => s != -99).Count();
            if (iCount != 0)
            {
                dResult = decimal.Round(dResult / iCount, 2);
            }

            return dResult;
        }




    }
}