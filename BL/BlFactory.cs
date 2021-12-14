using System;
using System.Reflection;

namespace BlApi
{
    public class BlFactory
    {
        public static BlApi.IBL GetBl()
        {
            return BO.BL.GetInstance;
        }
    }
}