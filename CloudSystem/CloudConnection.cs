using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CloudConnection
{
    LocalServer mLocalServer = null;

    RemoteClient mRemoteClient = null;

    CloudSocket mCloudSocket = null;

    CTSMarker mCTSMarker = null;

    public CloudConnection(CloudSocket cloudSocket, CTSMarker cTSMarker)
    {
        mCloudSocket = cloudSocket;

        mCTSMarker = cTSMarker;

        mLocalServer = new LocalServer(cloudSocket, cTSMarker);

        mRemoteClient = new RemoteClient(cloudSocket);
    }

    public CloudSocket cloudSocket
    {
        get
        {
            return mCloudSocket;
        }
    }

    public CTSMarker ctsMarker {
        get {
            return mCTSMarker;
        }
    }

    public bool isDisconnected
    {
        get
        {
            return false;
        }
    }

    public void Update()
    {
        mLocalServer.Update();
    }
}
