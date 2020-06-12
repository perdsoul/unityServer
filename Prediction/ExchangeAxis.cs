using System;
using DbscanImplementation;
using UnityEngine;

namespace Resources.Scripts.Prediction
{
    public class ExchangeAxis
    {
        private double a = 12.1517;
        private double b = 0.298019;
        private double c = 12.5748;
        private double d = 0.2173;
        public Vector3 UnityPos_to_modelIndex(Vector3 position)
        {
            if (Launcher.instance.GetSceneName == "testModel")
            {
                a = 12.1517;
                b = 0.298019;
                c = 12.5748;
                d = 0.2173;
            }
            else if (Launcher.instance.GetSceneName == "cgm")
            {
                a = -322.959;
                b = -205.87;
                c = -25.4188;
                d = 4.62966;
            }
            Vector3 modelIndex = new Vector3
            (
                (float)Math.Ceiling( (position.x + a) / d ),
                (float)Math.Ceiling( (position.z + b) / d ),
                (float)Math.Ceiling( (position.y + c) / d )
            );
            return modelIndex;
        }

        public Vector3 ModelIndex_to_unityPos(float X,float Y,float Z)
        {
            if (Launcher.instance.GetSceneName == "testModel")
            {
                a = 12.1517;
                b = 0.298019;
                c = 12.5748;
                d = 0.2173;
            }
            else if (Launcher.instance.GetSceneName == "cgm")
            {
                a = -322.959;
                b = -205.87;
                c = -25.4188;
                d = 4.62966;
            }
            Vector3 position = new Vector3();
            position.x = (float)((X-0.5)*d-a);
            position.y = (float)((Z-0.5)*d-c);
            position.z = (float)((Y-0.5)*d-b);
            return position;
        }
    }
}