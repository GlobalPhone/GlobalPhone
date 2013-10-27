using System;
using System.Collections.Generic;

namespace GlobalPhone
{
    public class Utils
    {
        public static TRet map_detect<T, TRet>(IEnumerable<T> collection, Func<T, TRet> func) where TRet:class
        {
            foreach (var value in collection)
            {
                TRet result ;
                if ((result=func(value))!=null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}