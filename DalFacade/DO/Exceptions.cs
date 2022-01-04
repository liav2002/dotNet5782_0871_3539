using System;
using System.Collections.Generic;
using System.Text;


namespace DO
{
    public class ItemNotFound : Exception
    {
        public ItemNotFound(string item) : base("ERROR: " + item + " not found.\n")
        {
        }
    }

    public class NonAvilableDrones : Exception
    {
        public NonAvilableDrones() : base(
            "Logistic Problem: There is no any avilable drone.\nmoving parcel to waiting list...\n")
        {
        }
    }

    public class DalTypeError : Exception
    {
        public DalTypeError() : base("ERROR: No type is match\n.")
        {
        }
    }

    public class XMLFileCreateFailed : Exception
    {
        public XMLFileCreateFailed(string filePath) : base($"ERROR: failed to create xml file: {filePath}.")
        {
        }
    }

    public class XMLFileLoadFailed : Exception
    {
        public XMLFileLoadFailed(string filePath) : base($"ERROR: failed to load xml file: {filePath}.")
        {
        }
    }
}