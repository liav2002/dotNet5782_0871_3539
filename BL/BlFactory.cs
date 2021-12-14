using System;
using System.Reflection;

namespace BlApi
{
    public class BlFactory
    {
        private static readonly Object LockObj = new Object();

        public static BlApi.IBL GetBl()
        {
            lock (LockObj)
            {
                return BO.BL.GetInstance;
            }
        }
    }
}