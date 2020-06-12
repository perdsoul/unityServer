﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using Resources.Scripts.Prediction;
using Resources.Scripts.DBscan;


namespace Resources.Scripts.History
{
    public class Path
    {
        private Vector3 mLastPos;
        private string pathName;
        private int duration = 0;
        private Record mCamera;
        private Vector3 mSpeed;

        public Record camera
        {
            get
            {
                return mCamera;
            }
        }
        public Vector3 cameraPos
        {
            get
            {
                return new Vector3((float)mCamera.posX, (float)mCamera.posY, (float)mCamera.posZ);
            }
        }
        public Vector3 cameraRot
        {
            get
            {
                return new Vector3((float)mCamera.rotX, (float)mCamera.rotY, (float)mCamera.rotZ);
            }
        }
        public Vector3 speed
        {
            get
            {
                return mSpeed;
            }
            set
            {
                mSpeed = value;
            }
        }
        public Vector3 lastPos
        {
            get
            {
                return mLastPos;
            }
        }


        public void UpdateCamera(ClientObjectAttribute clientObjectAttribute)
        {
            camera.posX = clientObjectAttribute.CameraPosX;
            camera.posY = clientObjectAttribute.CameraPosY;
            camera.posZ = clientObjectAttribute.CameraPosZ;
            camera.rotX = clientObjectAttribute.CameraRotX;
            camera.rotY = clientObjectAttribute.CameraRotY;
            camera.rotZ = clientObjectAttribute.CameraRotZ;
            Vector3 _cameraPos = new Vector3(clientObjectAttribute.CameraPosX, clientObjectAttribute.CameraPosY, clientObjectAttribute.CameraPosZ);
            if (lastPos != _cameraPos)//运动时更新速度以及预测开关
            {
                speed = _cameraPos - lastPos;
                //Debug.Log(speed.ToString());
            }
        }

        public Path(int _pathNum)
        {
            pathName = "Path" + _pathNum.ToString();
            mCamera=new Record(new Vector3(0,0,0),new Vector3(0,0,0) );
        }
        public string name
        {
            get
            {
                return pathName;
            }
        }
        public bool isOvertime(Vector3 newPos)
        {
            if (mLastPos != newPos)
            {
                duration = 0;
                mLastPos = newPos;
                return false;
            }
            else
            {
                if (duration < 2)
                {
                    duration++;
                    return false;
                }
                else
                {
                    duration++;
                    return true;
                }
            }
        }
        public bool isStop()
        {
            if (duration >= 5)
            {
                return true;
            }
            return false;
        }
    }
}
