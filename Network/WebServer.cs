using UnityEngine;
using System.Collections;
using System.Threading;
using System;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using WebSocketSharp.Server;
using WebSocketSharp;
using System.Text;

class AccumDataBuffer
{
    WebSocketServiceHost m_ServiceSession = null;

    string m_SessionID = string.Empty;

    List<byte[]> m_DataToSendList = new List<byte[]>();

    bool m_IsSending = false;

    public AccumDataBuffer(WebSocketServiceHost serviceSession , string sessionId)
    {
        m_ServiceSession = serviceSession;

        m_SessionID = sessionId;
    }

    bool needToSend
    {
        get
        {
            return m_DataToSendList.Count > 0 && m_IsSending == false;
        }

    }
    public void PushData(byte[] rawData)
    {
        m_DataToSendList.Add(rawData);
    }

    public void SendAsyncCompleted(bool isCompleted)
    {
        m_IsSending = false;
    }

    public void Update()
    {
        if (needToSend)
        {
            m_IsSending = true;

            //重复发送
            //m_ServiceSession.Sessions.SendToAsync(m_SessionID, m_DataToSendList[m_DataToSendList.Count - 1], SendAsyncCompleted);

            m_DataToSendList.Clear();
        }
    }
}

class WebServer : BaseServer
{
    WebSocketServer m_WebServer = null;

    WebSocketServiceHost m_LcrsService = null;

    Dictionary<string, AccumDataBuffer> m_SessionDataBufferMap = new Dictionary<string, AccumDataBuffer>();

    public override void Startup(string strIpAddress, int port)
    {
        m_WebServer = new WebSocketServer(IPAddress.Parse(strIpAddress), port);

        m_WebServer.Start();

        m_WebServer.AddWebSocketService<LcrsService>("/Lcrs");

        m_WebServer.WebSocketServices.TryGetServiceHost("/Lcrs", out m_LcrsService);

        //Debug.LogError("Startup");
    }

    public override void Update()
    {
        foreach (AccumDataBuffer accumData in m_SessionDataBufferMap.Values)
        {
            accumData.Update();
        }
    }

    public override void SendRawData(CTSMarker ctsMarker, XPacket msgNote, byte[] protoBytes)
    {
        if (m_LcrsService != null)
        {
            //Debug.Log("==================================================== Send data with: " + protoBytes.Length);

            if (m_SessionDataBufferMap.ContainsKey(ctsMarker.sessionId) == false)
            {
                m_SessionDataBufferMap.Add(ctsMarker.sessionId, new AccumDataBuffer(m_LcrsService, ctsMarker.sessionId));
            }

            m_SessionDataBufferMap[ctsMarker.sessionId].PushData(protoBytes);

            //m_LcrsService.Sessions.SendTo(ctsMarker.sessionId, protoBytes);

            m_LcrsService.Sessions.SendToAsync(ctsMarker.sessionId, protoBytes, SendAsyncCompleted);

            // Debug: save the file
            //string strSaveFile = Application.dataPath + "/rt_" + System.DateTime.Now.Minute + "_" + System.DateTime.Now.Second + "_web.png";
            //FileStream fs = File.Open(strSaveFile, FileMode.Create);
            //fs.Write(protoBytes, 0, protoBytes.Length);
            //fs.Close();
        }
    }

    void SendAsyncCompleted(bool isCompleted)
    {

    }

    public override void Close()
    {
        if (m_WebServer != null)
        {
            m_WebServer.Stop();
        }
    }
}