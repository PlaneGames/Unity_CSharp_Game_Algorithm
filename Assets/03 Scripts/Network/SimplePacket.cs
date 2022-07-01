using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;   //요게 바이너리 포매터임!
using System.Runtime.InteropServices;


public enum PacketType
{
    // For Server
    ChatRoomOpenReq = 1,
    ChatRoomJoinReq,
    ChatSendMsg,


    // For Client
    ChatRoomOpenComplete,
    ChatRoomOpenFailed,
    ChatReceiveMsg,
}

public interface IPacket
{

}

public class Packet
{
    public byte type;
    public IPacket data;

    public Packet(
        PacketType _type,
        IPacket _data
    )
    {
        type = (byte)_type;
        data = _data;
    }

    public static byte[] PacketToByte(Packet _packet)
    {
        // Packet Type.
        byte[] _packet_type = new byte[] { _packet.type };

        // Packet Data.
        int size = Marshal.SizeOf(_packet.data);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.StructureToPtr(_packet.data, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);

        // Packet Type + Data = Result.
        byte[] _res = new byte[_packet_type.Length + arr.Length];
        Array.Copy(_packet_type, 0, _res, 0, _packet_type.Length);
        Array.Copy(arr, 0, _res, _packet_type.Length, arr.Length);

        return _res;
    }

    public static byte[] PacketTypeToByte(PacketType _packet)
    {
        // Packet Type.
        byte[] _packet_type = new byte[] { (byte)_packet };

        return _packet_type;
    }

    public static PacketType GetPacketType(byte[] buffer)
    {
        return (PacketType)buffer[0];
    }

    public static Packet ToPacket<T>(byte[] buffer, PacketType _type) where T : IPacket, new()
    {
        Type _packet_t = typeof(T);

        int size = Marshal.SizeOf(_packet_t);
        if (size > buffer.Length)
        {
            throw new Exception();
        }
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(buffer, 1, ptr, size);

        IPacket _data = new T();
        _data = (T)Marshal.PtrToStructure(ptr, _packet_t);
        Packet _res = new Packet(_type, _data);
        Marshal.FreeHGlobal(ptr);

        return _res;
    }

    public static void Send(Packet _packet, Socket _socket)
    {
        if (_socket != null)
        {
            byte[] _res = Packet.PacketToByte(_packet);
            _socket.Send(_res, 0, _res.Length, SocketFlags.None);
        }
        else
        {
            Debug.LogError("Socket 접속 오류");
        }
    }
    
    public static void Send(PacketType _packet, Socket _socket)
    {
        if (_socket != null)
        {
            byte[] _res = Packet.PacketTypeToByte(_packet);
            _socket.Send(_res, 0, _res.Length, SocketFlags.None);
        }
        else
        {
            Debug.LogError("Socket 접속 오류");
        }
    }

}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Serializable]
public struct PacketChatRoomOpenReq : IPacket
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string room_name;

    [MarshalAs(UnmanagedType.I2)]
    public int room_pw;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Serializable]
public struct PacketChatRoomJoinReq : IPacket
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string room_name;

    [MarshalAs(UnmanagedType.I2)]
    public int room_pw;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Serializable]
public struct PacketChatSendMsgReq : IPacket
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string user_name;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 320)]
    public string talk_text;

    [MarshalAs(UnmanagedType.I4, SizeConst = 320)]
    public int UUID;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Serializable]
public struct PacketChatReceiveMsgReq : IPacket
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string user_name;
    
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    public string time;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 320)]
    public string talk_text;

    [MarshalAs(UnmanagedType.Bool)]
    public bool is_mine;

    [MarshalAs(UnmanagedType.I4, SizeConst = 320)]
    public int UUID;
}