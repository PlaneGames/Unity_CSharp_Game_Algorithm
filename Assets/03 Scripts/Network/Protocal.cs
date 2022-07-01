using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

using System.Runtime.InteropServices;


[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class Protocal : MonoBehaviour
{
    
    public enum PACKET_REQUEST_TYPE
    {
        SEND_RECEIVE = 0,
        SEND,
        RECEIVE,
    }

    public const int SEND_RECEIVE = 0;
    public const int SEND = 0;
    public const int RECEIVE = 0;

    public readonly string index;
    public readonly PACKET_REQUEST_TYPE req_type;
    
    static string S;
    [MarshalAs(UnmanagedType.Bool)] static bool B;
    [MarshalAs(UnmanagedType.I1)] static byte I1;
    [MarshalAs(UnmanagedType.I2)] static short I2;
    [MarshalAs(UnmanagedType.I4)] static int I4;
    [MarshalAs(UnmanagedType.I8)] static long I8;

    static ArrayList data;


    protected Protocal(string _idx, PACKET_REQUEST_TYPE _type)
    {
        index = _idx;
        req_type = _type;
        // byte의 사이즈는 동적으로 할당이 가능하도록 List 등을 활용하면 좋을듯.
        data = new ArrayList();
    }

    public static void AddI1(byte _data)
    {
        I1 = _data;
        short _len = (short)S.Length;
        data.Add(_len);
        data.Add(I1);
    }

    public static void AddI2(short _data)
    {
        I2 = _data;
        short _len = (short)S.Length;
        data.Add(_len);
        data.Add(I2);
    }

    public static void AddI4(int _data)
    {
        I4 = _data;
        short _len = (short)S.Length;
        data.Add(_len);
        data.Add(I4);
    }

    public static void AddStr(string _data)
    {
        S = _data;
        short _len = (short)S.Length;
        data.Add(_len);
        data.Add(S);
    }

    public static byte[] DataToArray()
    {
        // Need Short To Byte Array Logic.
        return data.ToArray(typeof(byte[])) as byte[];
    }

    public static void SendData()
    {
        if (Client.Instance.clientSocket != null)
        {
            byte[] _res = DataToArray();
            Client.Instance.clientSocket.Send(_res, 0, _res.Length, SocketFlags.None);
            data.Clear();
            data = null;
        }
        else
        {
            Debug.LogError("Socket 접속 오류");
        }
    }

    public static readonly Protocal CHAT_ROOM_OPEN          = new Protocal("0", SEND);

}
