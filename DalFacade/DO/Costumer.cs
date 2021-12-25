﻿using System;


namespace DO
{
    public class Costumer
    {
        private int _id;
        private string _name;
        private string _phone;
        private Location _location;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name
        {
            get => _name;
            set
            {
                SysLog.SysLog.GetInstance().ChangeCostumerName(_id, value);
                this._name = value;
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                SysLog.SysLog.GetInstance().ChangeCostumerPhone(_id, value);
                this._phone = value;
            }
        }

        public Location Location
        {
            get => _location;
            set => _location = value;
        }


        public override string ToString()
        {
            return string.Format("the id is: {0}\nthe name is: {1}\nthe phone is: {2}\n" +
                                 "the location is: {3}\n"
                , Id, Name, Phone, Location);
        }

        public Costumer(int id, string name, string phone, Location location)
        {
            SysLog.SysLog.GetInstance().AddCostumer(id);
            this._id = id;
            this._name = name;
            this._phone = phone;
            this._location = location;
        }
    }
}