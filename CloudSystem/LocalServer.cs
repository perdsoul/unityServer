using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

public class LocalServer
{
    CloudSocket mCloudSocket = null;

    CTSMarker mCTSMarker = null;

    RadianceCollector mRadianceCollector = null;

    int mWebClientExchangeCode = 4000;

    HashSet<string> ComponentTransformittedSet = new HashSet<string>();

    JObject ReuseDataInfoObj;

    //同步次数
    int times = 1;

    //初始加载文件序号
    int index = 1;

    string reuseDataPath = Application.streamingAssetsPath + "/" + Launcher.instance.GetSceneName + "/reuseInfo.json";
    //string reuseDataPath = "Assets/StreamingAssets/" + Launcher.instance.GetSceneName + "/reuseInfo.json";

    string firstLoadDataPath = Application.streamingAssetsPath + "/" + Launcher.instance.GetSceneName + "/firstLoad_";
    //string firstLoadDataPath = "Assets/StreamingAssets/" + Launcher.instance.GetSceneName + "/firstLoad_";


    public LocalServer(CloudSocket cloudSocket, CTSMarker ctsMarker)
    {
        mCloudSocket = cloudSocket;

        mCTSMarker = ctsMarker;

        switch (Launcher.instance.GIMode)
        {
            case eGiMode.Enlighten:
                mRadianceCollector = new EnlightenRadianceCollector();
                break;
            case eGiMode.RSM:
                break;
            case eGiMode.LPV:
                break;
            case eGiMode.Visible:
                mRadianceCollector = new VisibleRadianceCollector();
                break;
        }

        mRadianceCollector.Setup();
        GetReuseDataFile();
    }


    public void GetReuseDataFile() {
        StreamReader sr = new StreamReader(reuseDataPath);
        string json = sr.ReadToEnd();
        ReuseDataInfoObj = JObject.Parse(json);
    }


    Utils.int2 ExtractScreenSize(ClientObjectAttribute clientData)
    {
        int height = (int)clientData.Param % mWebClientExchangeCode;
        int weight = (int)clientData.Param / mWebClientExchangeCode;

        return new Utils.int2(weight, height);
    }


    public void Update()
    {
        if (mRadianceCollector != null)
        {
            if (mCloudSocket.AttributeDataListLength > 0)
            {
                Debug.Log("Start Syn View Point");
                ClientObjectAttribute clientObjAttribute = mCloudSocket.DequeueAttributeData();
                
                int height = (int)clientObjAttribute.Param % mWebClientExchangeCode;
                int width = (int)clientObjAttribute.Param / mWebClientExchangeCode;

                //Debug.Log("Param:" + (int)clientObjAttribute.Param + "  ExchangeCode:" + mWebClientExchangeCode);
                //Debug.Log("Width:" + width +"  Height:"+ height);

                if (times == 1 && File.Exists(firstLoadDataPath + index + ".bin"))
                {
                    while (File.Exists(firstLoadDataPath + index + ".bin"))
                    {
                        byte[] data = File.ReadAllBytes(firstLoadDataPath + index + ".bin");
                        //mCloudSocket.EnqueueRadianceData(new RadianceData((ushort)clientObjAttribute.LightPosZ, (ushort)width, (ushort)height, data));
                        NetworkServer.SendRawData(mCTSMarker, new XPacket((ushort)eMsgID.S2C_RadianceStream, (ushort)width, (ushort)height), data);
                        index++;
                    }
                }
                else if (times == 1)
                {
                    mRadianceCollector.Collect(width, height, clientObjAttribute);

                    mRadianceCollector.SendRadianceData(mCTSMarker, (ushort)clientObjAttribute.LightPosZ, (ushort)width, (ushort)height, ComponentTransformittedSet, ReuseDataInfoObj, true);
                }
                else
                {
                    mRadianceCollector.Collect(width, height, clientObjAttribute);

                    mRadianceCollector.SendRadianceData(mCTSMarker, (ushort)clientObjAttribute.LightPosZ, (ushort)width, (ushort)height, ComponentTransformittedSet, ReuseDataInfoObj, false);
                }

                times++;
            }
        }
    }
}
