using UnityEngine;
using System.Collections;
using System.Threading;
using System;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class XPacket : MsgPrefix
{
    public UInt16 MsgID;
    public UInt16 ScreenWidth;
    public UInt16 ScreenHeight;

    public XPacket(UInt16 msgID)
    {
        MsgID = msgID;
    }

    public XPacket(UInt16 msgID , UInt16 width , UInt16 height)
    {
        MsgID = msgID;
        ScreenWidth = width;
        ScreenHeight = height;
    }
}

public class MsgNoteUtils
{
    public static byte[] StructToBytes(object structObj)
    {
        int size = Marshal.SizeOf(structObj);

        byte[] bytes = new byte[size];
        IntPtr structPtr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(structObj, structPtr, false);
        Marshal.Copy(structPtr, bytes, 0, size);
        Marshal.FreeHGlobal(structPtr);
        return bytes;
    }

    public static object BytesToStruct(byte[] bytes, Type type)
    {
        int size = Marshal.SizeOf(type);

        if (size > bytes.Length)
        {
            return null;
        }

        IntPtr structPtr = Marshal.AllocHGlobal(size);
        Marshal.Copy(bytes, 0, structPtr, size);
        object obj = Marshal.PtrToStructure(structPtr, type);
        Marshal.FreeHGlobal(structPtr);
        return obj;
    }
}

public enum eMsgID
{
    Common,
    C2S_AttributeStream,
    S2C_RadianceStream,
}

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct ClientObjectAttribute
{
    //public ushort ScreenWidth;
    //public ushort ScreenHeight;

    public float Param;

    // Camera position
    public float CameraPosX;
    public float CameraPosY;
    public float CameraPosZ;

    // Camera rotation
    public float CameraRotX;
    public float CameraRotY;
    public float CameraRotZ;

    // Light position
    public float LightPosX;
    public float LightPosY;
    public float LightPosZ;

    // Light rotation
    public float LightRotX;
    public float LightRotY;
    public float LightRotZ;
}