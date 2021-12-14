using System;
using System.Reflection;

namespace DalApi
{
    public class DalFactory
    {
        private static readonly Object LockObj = new Object();

        public static IDAL GetDal(DO.DalTypes type)
        {
            lock (LockObj)
            {
                if (type == DO.DalTypes.DalObj)
                    return DalObject.DalObject.GetInstance;
                if (type == DO.DalTypes.DalXml)
                    throw new ArgumentException("DalXml is not implemented yet");
                else
                    throw new ArgumentException("No type is match");
            }
        }
    }
}