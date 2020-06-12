using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CTSMarker
{
    public TcpSocket<XPacket> tcpSocket = null;
    public string sessionId = null;

    public CTSMarker(TcpSocket<XPacket> _tcpSocket, string _sessionId)
    {
        tcpSocket = _tcpSocket;
        sessionId = _sessionId;
    }
}

public class ConnectionMgr : MonoBehaviour
{
    Dictionary<CTSMarker, CloudSocket> mTcpToCloudSocketMap = new Dictionary<CTSMarker, CloudSocket>();
    Dictionary<CloudSocket, CTSMarker> mCloudToTcpSocketMap = new Dictionary<CloudSocket, CTSMarker>();

    List<CloudConnection> mCloudConnections = new List<CloudConnection>();

    Dictionary<CTSMarker, CloudConnection> mMarkerToCloudConnectionMap = new Dictionary<CTSMarker, CloudConnection>();

    public Dictionary<CloudSocket, CTSMarker> cloudToTcpSocketMap {
        get {
            return mCloudToTcpSocketMap;
        }
    }

    public void BuildConnection(CloudSocket cloudSocket, CTSMarker ctsMarker)
    {
        CloudConnection cloudConnection = new CloudConnection(cloudSocket, ctsMarker);
        mCloudConnections.Add(cloudConnection);

        mMarkerToCloudConnectionMap.Add(ctsMarker, cloudConnection);

        mTcpToCloudSocketMap.Add(ctsMarker, cloudSocket);
        mCloudToTcpSocketMap.Add(cloudSocket, ctsMarker);

        //1751578
        string recordPath = Application.streamingAssetsPath + "/" + Launcher.instance.GetSceneName + "/history.txt";
        Launcher.instance.history.getPath(recordPath);
        Launcher.instance.history.NewPath(ctsMarker.sessionId);
        //1751578
    }

    public void RemoveConnection(CTSMarker ctsMarker)
    {
        if (mMarkerToCloudConnectionMap.ContainsKey(ctsMarker))
        {
            mCloudConnections.Remove(mMarkerToCloudConnectionMap[ctsMarker]);

            mMarkerToCloudConnectionMap.Remove(ctsMarker);
            //1751578
            Launcher.instance.history.RemovePath(ctsMarker.sessionId);
            //1751578
        }
    }

    public void ProcessAttributeStream(CTSMarker ctsMarker, byte[] msgStream)
    {
        if (mTcpToCloudSocketMap.ContainsKey(ctsMarker))
        {
            //mTcpToCloudSocketMap[ctsMarker].EnqueueAttributeData(new AttributeData(msgStream));
            mTcpToCloudSocketMap[ctsMarker].UpdateList((ClientObjectAttribute)MsgNoteUtils.BytesToStruct(msgStream, typeof(ClientObjectAttribute)));
        }
    }

    void Update()
    {
        // Update and synchronize each connection
        for (int i = 0; i < mCloudConnections.Count; ++i)
        {
            mCloudConnections[i].Update();
        }

        //SyncrhonizeRadiance();

        // Cleanup disconnected connection
        GC();

        // Show some debug information
        ShowInfo();
    }


    void SyncrhonizeRadiance()
    {
        for (int i = 0; i < mCloudConnections.Count; ++i)
        {
            RadianceData radianceData = null;

            while ((radianceData = mCloudConnections[i].cloudSocket.DequeueRadianceData()) != null)
            {
                //Debug.Log("================================ Send data with: " + radianceData.Width + ", " + radianceData.Height);
                NetworkServer.SendRawData(mCloudToTcpSocketMap[mCloudConnections[i].cloudSocket], new XPacket((ushort)eMsgID.S2C_RadianceStream, radianceData.Width, radianceData.Height), radianceData.rawData);
            }
        }
    }

    void ShowInfo()
    {
        if (mCloudConnections.Count > 0)
        {
            Launcher.instance.stats.ShowStats("Remote client count: " + mCloudConnections.Count);
        }
    }

    void GC()
    {
        for (int i = 0; i < mCloudConnections.Count; ++i)
        {
            if (mCloudConnections[i].isDisconnected)
            {
                mCloudConnections.RemoveAt(i);

                break;
            }
        }
    }
}
