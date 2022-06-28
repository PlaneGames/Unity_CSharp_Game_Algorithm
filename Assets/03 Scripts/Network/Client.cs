using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;


public class Client : SingleTonMonobehaviour<Client>
{

    public string serverIp = "127.0.0.1";
    Socket clientSocket = null;


    void Start()
    {
        this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress serverIPAdress = IPAddress.Parse(this.serverIp);
        IPEndPoint serverEndPoint = new IPEndPoint(serverIPAdress, Server.PortNumb);

        try
        {
            Debug.Log("Connecting to Server");
            this.clientSocket.Connect(serverEndPoint);
        }
        catch (SocketException e)
        {
            Debug.Log("Connection Failed:" + e.Message);
        }
    }

    private void OnApplicationQuit()
    {
        if (this.clientSocket != null)
        {
            this.clientSocket.Close();
            this.clientSocket = null;
        }
    }

    public static void Send(Packet _packet)
    {
        if (Client.Instance.clientSocket == null)
        {
            return;
        }

        byte[] _res = Packet.PacketToByte(_packet);
        Client.Instance.clientSocket.Send(_res, 0, _res.Length, SocketFlags.None);
    }

    public static void ReqChatRoomOpen(string _room_name, int _room_pw)
    {
        PacketChatRoomOpenReq _data = new PacketChatRoomOpenReq();
        _data.room_name = _room_name;
        _data.room_pw = _room_pw;
        Packet _packet = new Packet(PacketType.ChatRoomOpenReq, _data);
        Client.Send(_packet);
    }
    
    public static void ReqChatRoomJoin(string _room_name, int _room_pw)
    {
        PacketChatRoomJoinReq _data = new PacketChatRoomJoinReq();
        _data.room_name = _room_name;
        _data.room_pw = _room_pw;
        Packet _packet = new Packet(PacketType.ChatRoomJoinReq, _data);
        Client.Send(_packet);
    }

}