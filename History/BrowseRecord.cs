﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using Resources.Scripts.Prediction;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Resources.Scripts.History
{
    public class BrowseRecord
    {
        private Vector3 _positon;
        private Vector3 _rotation;
        private DateTime _time;

        public BrowseRecord()
        {
            this._positon = new Vector3();
            this._rotation = new Vector3();
            this._time = new DateTime();
        }
        public BrowseRecord(Vector3 _pos, Vector3 _rot)
        {
            this._positon = _pos;
            this._rotation = _rot;
            this._time = System.DateTime.Now;
        }
        public BrowseRecord(Vector3 _pos, Vector3 _rot, DateTime _ti)
        {
            this._positon = _pos;
            this._rotation = _rot;
            this._time = _ti;
        }

        public Vector3 pos
        {
            get
            {
                return _positon;
            }
        }
        public Vector3 rot
        {
            get
            {
                return _rotation;
            }
        }
        public DateTime date
        {
            get
            {
                return _time;
            }
        }

        public string Output(string _pathname)
        {
            string _record;
            _record =_pathname+"=" +_positon.ToString().Replace("(", "").Replace(")", "") + "_" +
                _rotation.ToString().Replace("(", "").Replace(")", "") + "_" + _time.ToString();
            return _record;
        }
    }


    
}
