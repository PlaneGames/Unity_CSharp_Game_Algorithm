using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;




public class Server : SingleTonMonobehaviour<Server>
{

    Socket serverSocket = null;
    ArrayList Connections = new ArrayList();
    ArrayList Buffer = new ArrayList();
    ArrayList ByteBuffers = new ArrayList();

    public const int PortNumb = 50001;


    private void Start()
    {
        Debug.Log("Server Start");
        this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, PortNumb);
        this.serverSocket.Bind(ipLocal);
        Debug.Log("Start Listening..");
        this.serverSocket.Listen(10);

        ChatRoom.Init();
    }

    void SocketClose()
    {
        //서버닫기
        if(this.serverSocket != null)
        {
            this.serverSocket.Close();
        }
        this.serverSocket = null;

        //클라이언트 끊기
        foreach(Socket client in this.Connections)
        {
            client.Close();
        }
        this.Connections.Clear();   
    }

    private void OnApplicationQuit()
    {
        SocketClose();
    }

    private void CheckNewUser()
    {
        ArrayList listenList = new ArrayList();
        listenList.Add(this.serverSocket);

        Socket.Select(listenList, null, null, 1000);

        for(int i = 0; i<listenList.Count; i++)
        {
            Socket newConnection = ((Socket)listenList[i]).Accept();
            this.Connections.Add(newConnection);
            this.ByteBuffers.Add(new ArrayList());
            Debug.Log("New Client Connected");
        }
    }

    private void ReceivePacket()
    {
        if(Connections.Count != 0)
        {
            ArrayList cloneConnections = new ArrayList(this.Connections);
            Socket.Select(cloneConnections, null, null, 1000);

            foreach(Socket client in cloneConnections)
            {
                byte[] buffer = new byte[512];
                client.Receive(buffer);
                ProgressPacket(Packet.GetPacketType(buffer), buffer, client);
            }
        }
    }

    private void ProgressPacket(PacketType _packet_t, byte[] _buffer, Socket _client)
    {
        Packet _packet = null;

        switch (_packet_t)
        {
        case PacketType.ChatRoomOpenReq:
            _packet = Packet.ToPacket<PacketChatRoomOpenReq>(_buffer, PacketType.ChatRoomOpenReq);
            PacketChatRoomOpenReq _ChatRoomOpenReq = (PacketChatRoomOpenReq)_packet.data;
            ChatRoom.CreateChatRoom(_ChatRoomOpenReq.room_name, _ChatRoomOpenReq.room_pw, ( Result ) => 
            {
                if (Result.success)
                {
                    Packet.Send(PacketType.ChatRoomOpenComplete, _client);
                    Debug.Log("\nType : Room Open!\nName : " + _ChatRoomOpenReq.room_name + "\nPW : " + _ChatRoomOpenReq.room_pw);
                }
            });
        break;            
        
        case PacketType.ChatRoomJoinReq:
            _packet = Packet.ToPacket<PacketChatRoomJoinReq>(_buffer, PacketType.ChatRoomJoinReq);
            PacketChatRoomJoinReq _ChatRoomJoinReq = (PacketChatRoomJoinReq)_packet.data;
            Debug.Log("\nType : Room Join!\nName : " + _ChatRoomJoinReq.room_name + "\nPW : " + _ChatRoomJoinReq.room_pw);
        break;

        default:
        return;
        }
    }

    private void Update()
    {
        CheckNewUser();
        ReceivePacket();
    }
}