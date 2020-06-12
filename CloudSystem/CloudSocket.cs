using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Resources.Scripts.DBscan;

public class RadianceData
{
    public UInt16 Header;
    public UInt16 Width;
    public UInt16 Height;
    
    byte[] mRawData;

    public RadianceData(UInt16 width , UInt16 height , byte[] rawData)
    {
        Width = width;
        Height = height;
        mRawData = rawData;
    }

    public RadianceData(UInt16 header, UInt16 width, UInt16 height, byte[] rawData)
    {
        Header = header;
        Width = width;
        Height = height;
        mRawData = rawData;
    }

    public byte[] packedData
    {
        get
        {
            byte[] headerBytes = BitConverter.GetBytes(Header);

            byte[] newData = new byte[headerBytes.Length + mRawData.Length];

            //Debug.Log("======================================== header length: " + headerBytes.Length + ", #$34F: " + Header);

            Array.Copy(headerBytes, newData, headerBytes.Length);
            Array.Copy(mRawData, 0, newData, headerBytes.Length, mRawData.Length);

            return newData;
        }
    }

    public byte[] rawData
    {
        get
        {
            return mRawData;
        }
    }
}

public class AttributeData
{
    public byte[] RawData;

    public AttributeData(byte[] rawData)
    {
        RawData = rawData;
    }
}


public class CloudSocket
{
    Stack<ClientObjectAttribute> mAttributeDataList = new Stack<ClientObjectAttribute>();
    List<ClientObjectAttribute> mList = new List<ClientObjectAttribute>();
    List<ClientObjectAttribute> mHistoryList = new List<ClientObjectAttribute>();
    Stack<RadianceData> mRadianceDataList = new Stack<RadianceData>();

    float deltaTime = 0.5f;
    int index = 0;
    public bool IsEqual(ClientObjectAttribute clientObjectAttributeA,ClientObjectAttribute clientObjectAttributeB)
    {
        if (clientObjectAttributeA.Param != clientObjectAttributeB.Param)
            return false;
        if (clientObjectAttributeA.CameraPosX != clientObjectAttributeB.CameraPosX)
            return false;
        if (clientObjectAttributeA.CameraPosY != clientObjectAttributeB.CameraPosY)
            return false;
        if (clientObjectAttributeA.CameraPosZ != clientObjectAttributeB.CameraPosZ)
            return false;
        if (clientObjectAttributeA.CameraRotX != clientObjectAttributeB.CameraRotX)
            return false;
        if (clientObjectAttributeA.CameraRotY != clientObjectAttributeB.CameraRotY)
            return false;
        if (clientObjectAttributeA.CameraRotZ != clientObjectAttributeB.CameraRotZ)
            return false;
        return true;
    }

    public double DistanceBtw(ClientObjectAttribute clientObjectAttributeA, ClientObjectAttribute clientObjectAttributeB)
    {
        return Math.Sqrt(
            Math.Pow((clientObjectAttributeA.CameraPosX - clientObjectAttributeB.CameraPosX), 2) +
            Math.Pow((clientObjectAttributeA.CameraPosY - clientObjectAttributeB.CameraPosY), 2) +
            Math.Pow((clientObjectAttributeA.CameraPosZ - clientObjectAttributeB.CameraPosZ), 2));
    }

    public ClientObjectAttribute PredictNextNode(ClientObjectAttribute clientObjectAttributeA, ClientObjectAttribute clientObjectAttributeB) {
        ClientObjectAttribute result = new ClientObjectAttribute();
        double distance = DistanceBtw(clientObjectAttributeA, clientObjectAttributeB);

        result.Param = clientObjectAttributeA.Param;
        result.CameraPosX = (float)(clientObjectAttributeA.CameraPosX + distance * clientObjectAttributeA.CameraRotX);
        result.CameraPosY = (float)(clientObjectAttributeA.CameraPosY + distance * clientObjectAttributeA.CameraRotY);
        result.CameraPosZ = (float)(clientObjectAttributeA.CameraPosZ + distance * clientObjectAttributeA.CameraRotZ);
        result.CameraRotX = clientObjectAttributeA.CameraRotX;
        result.CameraRotY = clientObjectAttributeA.CameraRotY;
        result.CameraRotZ = clientObjectAttributeA.CameraRotZ;
        return result;
    }


    public int AttributeDataListLength
    {
        get {
            return mAttributeDataList.Count;
        }
    }

    public RadianceData DequeueRadianceData()
    {
        if (mRadianceDataList.Count > 0)
        {
            return mRadianceDataList.Pop();
        }

        return null;
    }

    public ClientObjectAttribute DequeueAttributeData()
    {
        return mAttributeDataList.Pop();
    }

    public void EnqueueRadianceData(RadianceData dataBytes)
    {
        mRadianceDataList.Push(dataBytes);
    }

    public void EnqueueAttributeData(ClientObjectAttribute clientObjectAttribute)
    {
        //if (mAttributeDataList.Count >= 3) {
        //    mAttributeDataList.Dequeue();
        //}
        mAttributeDataList.Push(clientObjectAttribute);
        Debug.Log("Update Viewpoint List");
    }

    private double Compare(ClientObjectAttribute clientObjectAttribute, ClientObjectAttribute predictObjectAttribute)
    {
        double score;
        double a = clientObjectAttribute.CameraPosX - predictObjectAttribute.CameraPosX;
        double b = clientObjectAttribute.CameraPosY - predictObjectAttribute.CameraPosY;
        double c = clientObjectAttribute.CameraPosZ - predictObjectAttribute.CameraPosZ;
        double d = clientObjectAttribute.CameraRotX - predictObjectAttribute.CameraRotX;
        double e = clientObjectAttribute.CameraRotY - predictObjectAttribute.CameraRotY;
        double f = clientObjectAttribute.CameraRotZ - predictObjectAttribute.CameraRotZ;
        score = a * a + b * b + c * c + d * d + e * e + f * f;
        return score;

    }

    private double _count;
    private bool _notPredicted = true;
    private ClientObjectAttribute predictObjectAttribute;
    public void UpdateList(ClientObjectAttribute clientObjectAttribute)
    {
        ConnectionMgr connectionMgr = Launcher.instance.connectionMgr;
        CTSMarker ctsMarker = connectionMgr.cloudToTcpSocketMap[this];
        Launcher.instance.history.UpdateCamera(ctsMarker.sessionId, clientObjectAttribute);
        //clientObjectAttribute.LightPosZ = 66;

        if (mList.Count <= 0)
        {
            EnqueueAttributeData(clientObjectAttribute);
            mList.Add(clientObjectAttribute);
            _count = 0;

        }
        else
        {
            if (!IsEqual(clientObjectAttribute, mList[mList.Count - 1]))
            {
                EnqueueAttributeData(clientObjectAttribute);
                mList.Add(clientObjectAttribute);
                _count = 0;
                Debug.Log(clientObjectAttribute.CameraPosX + "  " + clientObjectAttribute.CameraPosY + "  " +
                          clientObjectAttribute.CameraPosZ);
                Debug.Log(clientObjectAttribute.CameraRotX + "  " + clientObjectAttribute.CameraRotY + "  " +
                          clientObjectAttribute.CameraRotZ);
                if (!_notPredicted)
                {
                    //做预测的反馈
                    double score = Compare(clientObjectAttribute, predictObjectAttribute);
                    //                    Debug.Log(clientObjectAttribute.CameraPosX+"  "+clientObjectAttribute.CameraPosY+"  "+
                    //                              clientObjectAttribute.CameraPosZ);
                    //                    Debug.Log(clientObjectAttribute.CameraRotX+"  "+clientObjectAttribute.CameraRotY+"  "+
                    //                              clientObjectAttribute.CameraRotZ);
                    //                    Debug.Log("偏差值： "+(score/128).ToString("0.00"));
                    //                    Debug.Log(predictObjectAttribute.CameraPosX+"  "+predictObjectAttribute.CameraPosY+"  "+
                    //                              predictObjectAttribute.CameraPosZ);
                    //                    Debug.Log(predictObjectAttribute.CameraRotX+"  "+predictObjectAttribute.CameraRotY+"  "+
                    //                              predictObjectAttribute.CameraRotZ);
                    _notPredicted = true;
                }
            }
            //1751578
            else
            { _count++; }

            if (_count > 5 && _notPredicted)
            {
                Debug.Log("!!!!!!!!!!!!!Predict!!!!!!!!!!!!!");
                Record a = Launcher.instance.history.getPredict(ctsMarker.sessionId, clientObjectAttribute);
                predictObjectAttribute.Param = clientObjectAttribute.Param;
                predictObjectAttribute.CameraPosX = (float)a.posX;
                predictObjectAttribute.CameraPosY = (float)a.posY;
                predictObjectAttribute.CameraPosZ = (float)a.posZ;
                predictObjectAttribute.CameraRotX = (float)a.rotX;
                predictObjectAttribute.CameraRotY = (float)a.rotY;
                predictObjectAttribute.CameraRotZ = (float)a.rotZ;
                //测试数据


                //predictObjectAttribute.CameraPosX = (float)-9.745914;
                //predictObjectAttribute.CameraPosY = (float)13.74586;
                //predictObjectAttribute.CameraPosZ = (float) 6.888044;
                //predictObjectAttribute.CameraRotX = (float)0.3567473;
                //predictObjectAttribute.CameraRotY = (float)-0.7427009;
                //predictObjectAttribute.CameraRotZ = (float)-0.5666805;
                //测试结束
                predictObjectAttribute.LightPosZ = 66;
                EnqueueAttributeData(predictObjectAttribute);
                _notPredicted = false;
                Debug.Log("!!!!!!!!!!!!!Finish!!!!!!!!!!!!!");
            }
            //1751578
        }
    }


    //public void UpdateList(ClientObjectAttribute clientObjectAttribute) {
    //    //first node
    //    if (mList.Count <= 0) {
    //        EnqueueAttributeData(clientObjectAttribute);
    //        mList.Add(clientObjectAttribute);
    //    }
    //    else {
    //        //different from the previous node
    //        if (!IsEqual(clientObjectAttribute, mList[mList.Count - 1])) {
    //            EnqueueAttributeData(clientObjectAttribute);
    //            mList.Add(clientObjectAttribute);
    //        }
    //        //enough 2 nodes, predict next node
    //        else if (mList.Count >= 2)
    //        {
    //            ClientObjectAttribute nextNode = PredictNextNode(clientObjectAttribute, mList[mList.Count - 2]);
    //            EnqueueAttributeData(nextNode);
    //            mList.Add(nextNode);
    //        }
    //    }
    //}
}
