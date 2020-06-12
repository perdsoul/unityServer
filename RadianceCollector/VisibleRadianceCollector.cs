using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class VisibleRadianceCollector : RadianceCollector
{
    protected override void Render(int width, int height)
    {
        Screen.SetResolution(width, height, false);

        //mSyncedCamera.targetTexture = mRenderTexture;
        mSyncedCamera.targetTexture = renderTexture;

        mSyncedCamera.Render();

        //RenderTexture.active = mRenderTexture;

        //OutputRt(mRenderTexture);

        //RenderTexture.active = null;

        Screen.SetResolution(Launcher.instance.consoleWidth, Launcher.instance.consoleHeight, false);
    }

    string ColorToHex(Color color)
    {
        Color32 c = color;
        string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", c.r, c.g, c.b, c.a);
        return hex;
    }

    int ColorToInt(Color color)
    {
        int intColor = int.Parse(ColorToHex(color), System.Globalization.NumberStyles.HexNumber);

        return intColor;
    }

    public override byte[] GetRadianceDataInPng(HashSet<string> ComponentTransformittedSet)
    {
        if (Launcher.instance.radianceMgr.mIsPrepaed == false)
        {
            return null;
        }

        RenderTexture.active = mRenderTexture;

        Texture2D png = new Texture2D(mRenderTexture.width, mRenderTexture.height, TextureFormat.RGB24, false);
        png.ReadPixels(new Rect(0, 0, mRenderTexture.width, mRenderTexture.height), 0, 0);

        Color32[] visiblePixels = png.GetPixels32();


        ///*
        string visibleNames = "";
        Dictionary<string, int> visAccumulatorDic = new Dictionary<string, int>();
        ///*
        Launcher.instance.radianceMgr.ResetAccumulator();

        //Debug.Log("====================================1: " + visiblePixels.Length + ", " + Launcher.instance.radianceMgr.mVisibleAccumulator.Length);

        //int maxId = 0;
        //for (int i = 0; i < visiblePixels.Length; ++i)
        //{
        //    Debug.Log(visiblePixels[i]);
        //    int partId = Launcher.instance.radianceMgr.mVisibleGrid[visiblePixels[i].r, visiblePixels[i].g, visiblePixels[i].b];
        //    string name = Launcher.instance.radianceMgr.mVisibleNames[visiblePixels[i].r, visiblePixels[i].g, visiblePixels[i].b];
        //    if (partId > 0)
        //    {
        //        Launcher.instance.radianceMgr.mVisibleAccumulator[partId - 1]++;
        //    }

        //    maxId = Mathf.Max(maxId, partId);
        //    visibleNames += name;
        //}

        //Debug.Log(visibleNames);

        ////Debug.Log("====================================2: " + maxId + ", " + Launcher.instance.radianceMgr.mVisibleAccumulator.Length);

        //for (int i = 0; i < Launcher.instance.radianceMgr.mVisibleAccumulator.Length; ++i)
        //{
        //    if (Launcher.instance.radianceMgr.mVisibleAccumulator[i] > 0)
        //    {
        //        visibleIds += i + "\n";
        //        //visibleNames += Launcher.instance.radianceMgr.mVisibleAccumulator[i].allNameInOne + "\n";
        //    }
        //}
        ////*/

        //RenderTexture.active = null;
        //Debug.Log("All Parts: " + Launcher.instance.radianceMgr.mVisibleAccumulator.Length);
        for (int i = 0; i < visiblePixels.Length; ++i)
        {
            string name = Launcher.instance.radianceMgr.mVisibleNames[visiblePixels[i].r, visiblePixels[i].g, visiblePixels[i].b];
            if (name == null)
            {
                continue;
            }
            if (!ComponentTransformittedSet.Contains(name))
            {
                visAccumulatorDic.Add(name, 1);
                visibleNames += name + "/";
                ComponentTransformittedSet.Add(name);
            }
        }
        return System.Text.Encoding.UTF8.GetBytes(visibleNames);
    }

    //public override void SendRadianceData(CTSMarker ctsmarker, ushort header, ushort width, ushort height, HashSet<string> ComponentTransformittedSet, JObject ReuseDataInfoObject, bool firstLoadStore)
    //{

    //    if (Launcher.instance.radianceMgr.mIsPrepaed == false)
    //    {
    //        return;
    //    }

    //    RenderTexture.active = mRenderTexture;

    //    Texture2D png = new Texture2D(mRenderTexture.width, mRenderTexture.height, TextureFormat.RGB24, false);
    //    png.ReadPixels(new Rect(0, 0, mRenderTexture.width, mRenderTexture.height), 0, 0);

    //    Color32[] visiblePixels = png.GetPixels32();

    //    /*
    //     write into pic
    //     */
    //    //byte[] bytes = png.EncodeToPNG();
    //    //File.WriteAllBytes(Application.streamingAssetsPath + "/Image" + ".png", bytes);
    //    //Debug.Log("save to png!!");

    //    ///*
    //    string visibleIds = "";
    //    string visibleNames = "";
    //    //统计像素频率
    //    Dictionary<string, int> visAccumulatorDic = new Dictionary<string, int>();
    //    ///*
    //    Launcher.instance.radianceMgr.ResetAccumulator();

    //    byte[] allByteData = new byte[0];
    //    string lengthList = "";
    //    int count = 0;
    //    int index = 1;

    //    string firstLoadStorePath = Application.streamingAssetsPath + "/" + Launcher.instance.GetSceneName + "/firstLoad_";
    //    //string firstLoadStorePath = "Assets/StreamingAssets/" + Launcher.instance.GetSceneName + "/firstLoad_";

    //    for (int i = 0; i < visiblePixels.Length; i++)
    //    {
    //        //send data per 100
    //        if (count >= 10 || i >= visiblePixels.Length - 1)
    //        {
    //            Debug.Log("Start Sending Components");
    //            //package doesn't end
    //            if (i < visiblePixels.Length - 1)
    //                lengthList += "0";
    //            //package ends
    //            else
    //                lengthList += "1";
    //            //send data
    //            lengthList += "//";
    //            byte[] byteLength = System.Text.Encoding.UTF8.GetBytes(lengthList);
    //            //Debug.Log(byteLength.Length);
    //            //Debug.Log("count:" + count);
    //            byte[] byteNeedToSend = new byte[300 + allByteData.Length];
    //            System.Buffer.BlockCopy(byteLength, 0, byteNeedToSend, 0, byteLength.Length);
    //            System.Buffer.BlockCopy(allByteData, 0, byteNeedToSend, 300, allByteData.Length);

    //            if (firstLoadStore)
    //            {
    //                File.WriteAllBytes(firstLoadStorePath + index + ".bin", byteNeedToSend);
    //                index++;
    //            }

    //            //socket.EnqueueRadianceData(new RadianceData(header, width, height, byteNeedToSend));
    //            NetworkServer.SendRawData(ctsmarker, new XPacket((ushort)eMsgID.S2C_RadianceStream, width, height), byteNeedToSend);
    //            //reset
    //            count = 0;
    //            allByteData = new byte[0];
    //            lengthList = "";
    //        }
    //        string name = Launcher.instance.radianceMgr.mVisibleNames[visiblePixels[i].r, visiblePixels[i].g, visiblePixels[i].b];

    //        if (name == null)
    //        {
    //            continue;
    //        }
    //        //替换重用构件名
    //        string[] nameArr = name.Split('/');
    //        string reuseName = (string)ReuseDataInfoObject[nameArr[nameArr.Length - 1]];
    //        if (reuseName != null)
    //        {
    //            name = name.Replace(nameArr[nameArr.Length - 1], reuseName);
    //        }

    //        if (!ComponentTransformittedSet.Contains(name))
    //        {
    //            byte[] temp = GetGlbData(name);
    //            //Debug.Log("send glb:" + name);
    //            if (temp.Length == 0)
    //            {
    //                Debug.Log("can't find component: " + name);
    //                continue;
    //            }
    //            //visibleNames += name + "/";
    //            lengthList += temp.Length.ToString() + "/";
    //            count++;
    //            if (allByteData.Length == 0)
    //            {
    //                allByteData = temp;
    //            }
    //            else
    //            {
    //                byte[] newData = new byte[allByteData.Length + temp.Length];
    //                System.Buffer.BlockCopy(allByteData, 0, newData, 0, allByteData.Length);
    //                System.Buffer.BlockCopy(temp, 0, newData, allByteData.Length, temp.Length);
    //                allByteData = newData;
    //            }
    //            ComponentTransformittedSet.Add(name);
    //        }
    //    }
    //}

    //1751578

    public override void SendRadianceData(CTSMarker ctsmarker, ushort header, ushort width, ushort height, HashSet<string> ComponentTransformittedSet, JObject ReuseDataInfoObject, bool firstLoadStore)
    {

        Debug.Log(header + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        if (Launcher.instance.radianceMgr.mIsPrepaed == false)
        {
            return;
        }

        RenderTexture.active = mRenderTexture;

        Texture2D png = new Texture2D(mRenderTexture.width, mRenderTexture.height, TextureFormat.RGB24, false);
        png.ReadPixels(new Rect(0, 0, mRenderTexture.width, mRenderTexture.height), 0, 0);

        Color32[] visiblePixels = png.GetPixels32();

        ///*
        string visibleIds = "";
        string visibleNames = "";
        //统计像素频率
        Dictionary<string, int> visAccumulatorDic = new Dictionary<string, int>();
        ///*
        Launcher.instance.radianceMgr.ResetAccumulator();

        byte[] allByteData = new byte[0];
        string lengthList = "";
        int count = 0;
        int index = 1;

        string firstLoadStorePath = Application.streamingAssetsPath + "/" + Launcher.instance.GetSceneName + "/firstLoad_";
        //string firstLoadStorePath = "Assets/StreamingAssets/" + Launcher.instance.GetSceneName + "/firstLoad_";

        for (int i = 0; i < visiblePixels.Length; i++)
        {
            //send data per 100
            if (count >= 10 || i >= visiblePixels.Length)
            {
                Debug.Log("Start Sending Components");

                //send data
                if (header == 66)
                {
                    lengthList += "1";
                    Debug.Log("++++++++++++++++++++++++++++");
                }
                else
                {
                    lengthList += "0";
                }
                lengthList += "//";
                byte[] byteLength = System.Text.Encoding.UTF8.GetBytes(lengthList);
                //Debug.Log(byteLength.Length);
                //Debug.Log("count:" + count);
                byte[] byteNeedToSend = new byte[300 + allByteData.Length];
                System.Buffer.BlockCopy(byteLength, 0, byteNeedToSend, 0, byteLength.Length);
                System.Buffer.BlockCopy(allByteData, 0, byteNeedToSend, 300, allByteData.Length);

                if (firstLoadStore)
                {
                    File.WriteAllBytes(firstLoadStorePath + index + ".bin", byteNeedToSend);
                    index++;
                }

                //socket.EnqueueRadianceData(new RadianceData(header, width, height, byteNeedToSend));
                NetworkServer.SendRawData(ctsmarker, new XPacket((ushort)eMsgID.S2C_RadianceStream, width, height), byteNeedToSend);
                //reset
                count = 0;
                allByteData = new byte[0];
                lengthList = "";
            }
            string name = Launcher.instance.radianceMgr.mVisibleNames[visiblePixels[i].r, visiblePixels[i].g, visiblePixels[i].b];

            if (name == null)
            {
                continue;
            }
            //替换重用构件名
            string[] nameArr = name.Split('/');
            string reuseName = (string)ReuseDataInfoObject[nameArr[nameArr.Length - 1]];
            if (reuseName != null)
            {
                name = name.Replace(nameArr[nameArr.Length - 1], reuseName);
            }

            if (!ComponentTransformittedSet.Contains(name))
            {
                byte[] temp = GetGlbData(name);
                //Debug.Log("send glb:" + name);
                if (temp.Length == 0)
                {
                    Debug.Log("can't find component: " + name);
                    continue;
                }
                //visibleNames += name + "/";
                lengthList += temp.Length.ToString() + "/";
                count++;
                if (allByteData.Length == 0)
                {
                    allByteData = temp;
                }
                else
                {
                    byte[] newData = new byte[allByteData.Length + temp.Length];
                    System.Buffer.BlockCopy(allByteData, 0, newData, 0, allByteData.Length);
                    System.Buffer.BlockCopy(temp, 0, newData, allByteData.Length, temp.Length);
                    allByteData = newData;
                }
                ComponentTransformittedSet.Add(name);
            }
        }
    }

    public override HashSet<string> getCenterComponents(int width, int height, ClientObjectAttribute clientObjAttribute)
    {
        HashSet<string> result = new HashSet<string>();
        if (Launcher.instance.radianceMgr.mIsPrepaed == false)
        {
            return null;
        }
        RenderTexture.active = mRenderTexture;

        Texture2D png = new Texture2D(mRenderTexture.width, mRenderTexture.height, TextureFormat.RGB24, false);
        png.ReadPixels(new Rect(mRenderTexture.width * 0.33f, mRenderTexture.height * 0.33f, mRenderTexture.width * 0.33f, mRenderTexture.height * 0.33f), 0, 0);
        /* System.IO.File.WriteAllBytes(@"e:\test.png", png.EncodeToPNG());
         Texture2D png2 = new Texture2D(mRenderTexture.width, mRenderTexture.height, TextureFormat.RGB24, false);
         png2.ReadPixels(new Rect(0, 0, mRenderTexture.width, mRenderTexture.height), 0, 0);
         System.IO.File.WriteAllBytes(@"e:\test2.png", png2.EncodeToPNG());*/

        Color32[] visiblePixels = png.GetPixels32();
        for (int i = 0; i < visiblePixels.Length; i++)
        {
            string name = Launcher.instance.radianceMgr.mVisibleNames[visiblePixels[i].r, visiblePixels[i].g, visiblePixels[i].b];

            if (name == null)
            {
                continue;
            }
            //替换重用构件名
            string[] nameArr = name.Split('/');
            if (!result.Contains(name))
            {
                result.Add(name);
            }
        }
        Debug.Log("Saved");
        return result;
    }
    //1751578


    //public override void SendRadianceData(CTSMarker ctsmarker, ushort header, ushort width, ushort height, HashSet<string> ComponentTransformittedSet, JObject ReuseDataInfoObject, bool firstLoadStore)
    //{
    //    if (Launcher.instance.radianceMgr.mIsPrepaed == false)
    //    {
    //        return;
    //    }

    //    RenderTexture.active = mRenderTexture;

    //    Texture2D png = new Texture2D(mRenderTexture.width, mRenderTexture.height, TextureFormat.RGB24, false);
    //    png.ReadPixels(new Rect(0, 0, mRenderTexture.width, mRenderTexture.height), 0, 0);

    //    Color32[] visiblePixels = png.GetPixels32();

    //    ///*
    //    string visibleIds = "";
    //    string visibleNames = "";
    //    //统计像素频率
    //    Dictionary<string, int> visAccumulatorDic = new Dictionary<string, int>();
    //    ///*
    //    Launcher.instance.radianceMgr.ResetAccumulator();

    //    string firstLoadStorePath = Application.streamingAssetsPath + "/" + Launcher.instance.GetSceneName + "/firstLoad_";
    //    //string firstLoadStorePath = "Assets/StreamingAssets/" + Launcher.instance.GetSceneName + "/firstLoad_";
    //    for (int i = 0; i < visiblePixels.Length; ++i)
    //    {
    //        string name = Launcher.instance.radianceMgr.mVisibleNames[visiblePixels[i].r, visiblePixels[i].g, visiblePixels[i].b];
    //        if (name == null)
    //        {
    //            continue;
    //        }

    //        ////替换重用构件名
    //        if (ReuseDataInfoObject[name] != null)
    //        {
    //            name = (string)ReuseDataInfoObject[name];
    //        }

    //        if (!ComponentTransformittedSet.Contains(name))
    //        {
    //            byte[] temp = GetGlbData(name);
    //            Debug.Log("send glb:" + name);
    //            if (temp.Length == 0)
    //            {
    //                Debug.Log("can't find component: " + name);
    //                continue;
    //            }
    //            //socket.EnqueueRadianceData(new RadianceData(header, width, height, temp));
    //            NetworkServer.SendRawData(ctsmarker, new XPacket((ushort)eMsgID.S2C_RadianceStream, width, height), temp);
    //            ComponentTransformittedSet.Add(name);
    //        }

    //    }
    //}

    //public override void SendRadianceData(CTSMarker ctsmarker, ushort header, ushort width, ushort height, HashSet<string> ComponentTransformittedSet, JObject ReuseDataInfoObject, bool firstLoadStore)
    //{
    //    if (Launcher.instance.radianceMgr.mIsPrepaed == false)
    //    {
    //        return;
    //    }

    //    RenderTexture.active = mRenderTexture;

    //    Texture2D png = new Texture2D(mRenderTexture.width, mRenderTexture.height, TextureFormat.RGB24, false);
    //    png.ReadPixels(new Rect(0, 0, mRenderTexture.width, mRenderTexture.height), 0, 0);

    //    Color32[] visiblePixels = png.GetPixels32();

    //    ///*
    //    string visibleIds = "";
    //    string visibleNames = "";
    //    //统计像素频率
    //    Dictionary<string, int> visAccumulatorDic = new Dictionary<string, int>();
    //    ///*
    //    Launcher.instance.radianceMgr.ResetAccumulator();

    //    byte[] allByteData = new byte[0];
    //    string lengthList = "";
    //    int count = 0;
    //    int i = 0;
    //    int index = 1;
    //    int flag = 1;
    //    int midPos = visiblePixels.Length / 2 - 1;

    //    //string firstLoadStorePath = Application.streamingAssetsPath + "/" + Launcher.instance.GetSceneName + "/firstLoad_";
    //    string firstLoadStorePath = "Assets/StreamingAssets/" + Launcher.instance.GetSceneName + "/firstLoad_";

    //    while (i <= visiblePixels.Length / 2)
    //    {
    //        int pointer = midPos + flag * i;
    //        if (pointer > 0)
    //        {
    //            //send data per 100
    //            if (count >= 30 || i >= visiblePixels.Length / 2)
    //            {
    //                //send data
    //                lengthList += "//";
    //                byte[] byteLength = System.Text.Encoding.UTF8.GetBytes(lengthList);
    //                Debug.Log(byteLength.Length);
    //                Debug.Log("count:" + count);
    //                byte[] byteNeedToSend = new byte[500 + allByteData.Length];
    //                System.Buffer.BlockCopy(byteLength, 0, byteNeedToSend, 0, byteLength.Length);
    //                System.Buffer.BlockCopy(allByteData, 0, byteNeedToSend, 500, allByteData.Length);

    //                if (firstLoadStore)
    //                {
    //                    File.WriteAllBytes(firstLoadStorePath + index + ".bin", byteNeedToSend);
    //                    index++;
    //                }

    //                //socket.EnqueueRadianceData(new RadianceData(header, width, height, byteNeedToSend));
    //                NetworkServer.SendRawData(ctsmarker, new XPacket((ushort)eMsgID.S2C_RadianceStream, width, height), byteNeedToSend);
    //                //reset
    //                count = 0;
    //                allByteData = new byte[0];
    //                lengthList = "";
    //            }
    //            string name = Launcher.instance.radianceMgr.mVisibleNames[visiblePixels[pointer].r, visiblePixels[pointer].g, visiblePixels[pointer].b];

    //            if (flag < 0)
    //            {
    //                flag = 1;
    //                i++;
    //            }
    //            else
    //                flag = -1;

    //            if (name == null)
    //            {
    //                continue;
    //            }

    //            //替换重用构件名
    //            string[] nameArr = name.Split('/');
    //            string reuseName = (string)ReuseDataInfoObject[nameArr[nameArr.Length - 1]];
    //            if (reuseName != null)
    //            {
    //                name = name.Replace(nameArr[nameArr.Length - 1], reuseName);
    //            }

    //            if (!ComponentTransformittedSet.Contains(name))
    //            {
    //                byte[] temp = GetGlbData(name);
    //                Debug.Log("send glb:" + name);
    //                if (temp.Length == 0)
    //                {
    //                    Debug.Log("can't find component: " + name);
    //                    continue;
    //                }
    //                //visibleNames += name + "/";
    //                lengthList += temp.Length.ToString() + "/";
    //                count++;
    //                if (allByteData.Length == 0)
    //                {
    //                    allByteData = temp;
    //                }
    //                else
    //                {
    //                    byte[] newData = new byte[allByteData.Length + temp.Length];
    //                    System.Buffer.BlockCopy(allByteData, 0, newData, 0, allByteData.Length);
    //                    System.Buffer.BlockCopy(temp, 0, newData, allByteData.Length, temp.Length);
    //                    allByteData = newData;
    //                }
    //                ComponentTransformittedSet.Add(name);
    //            }
    //        }
    //    }
    //}


    public byte[] GetGlbData(string name)
    {
        string fileName = Application.streamingAssetsPath + "/" + name + ".glb";
        //string fileName = "Assets/StreamingAssets/" + name + ".glb";

        if (File.Exists(fileName))
        {
            return File.ReadAllBytes(fileName);
        }
        else
        {
            return new byte[0];
        }
    }
}
