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
    public Socket clientSocket = null;
    public int UUID;


    void Start()
    {
        this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress serverIPAdress = IPAddress.Parse(this.serverIp);
        IPEndPoint serverEndPoint = new IPEndPoint(serverIPAdress, Server.PortNumb);

        try
        {
            Debug.Log("Connecting to Server");
            this.clientSocket.Connect(serverEndPoint);
            UUID = UnityEngine.Random.Range(10000000, 99999999); //! UUID는 서버에서 발급해야함.
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

    public static void ReqChatRoomOpen(string _room_name, int _room_pw)
    {
        PacketChatRoomOpenReq _data = new PacketChatRoomOpenReq();
        _data.room_name = _room_name;
        _data.room_pw = _room_pw;
        Packet _packet = new Packet(PacketType.ChatRoomOpenReq, _data);
        Packet.Send(_packet, Client.Instance.clientSocket);
    }
    
    public static void ReqChatRoomJoin(string _room_name, int _room_pw)
    { 
        PacketChatRoomJoinReq _data = new PacketChatRoomJoinReq();
        _data.room_name = _room_name;
        _data.room_pw = _room_pw;
        Packet _packet = new Packet(PacketType.ChatRoomJoinReq, _data);
        Packet.Send(_packet, Client.Instance.clientSocket);
    }    

    public static void ReqChatSendMsg(string _name, string _talk)
    { 
        PacketChatSendMsgReq _data = new PacketChatSendMsgReq();
        _data.user_name = _name;
        _data.talk_text = _talk;
        _data.UUID = Client.Instance.UUID;
        Packet _packet = new Packet(PacketType.ChatSendMsg, _data);
        Packet.Send(_packet, Client.Instance.clientSocket);
    }

    private void Update()
    {
        ReceivePacket();
    }

    private void ReceivePacket()
    {
        if(Client.Instance.clientSocket != null && Client.Instance.clientSocket.Available != 0)
        {
            byte[] buffer = new byte[512];
            Client.Instance.clientSocket.Receive(buffer);
            ProgressPacket(Packet.GetPacketType(buffer), buffer, Client.Instance.clientSocket);
        }
    }

    private void ProgressPacket(PacketType _packet_t, byte[] _buffer, Socket _client)
    {
        Packet _packet = null;

        switch (_packet_t)
        {
        case PacketType.ChatRoomOpenComplete:
            UI_PopupMgr.GetPopup<UIP_ChatRoom>();
        break;
        
        case PacketType.ChatReceiveMsg:
            _packet = Packet.ToPacket<PacketChatReceiveMsgReq>(_buffer, PacketType.ChatReceiveMsg);
            PacketChatReceiveMsgReq _ChatReceiveMsg = (PacketChatReceiveMsgReq)_packet.data;
            var _obj = GameObject.Find("@ PopupChatRoom");
            var _comp = _obj.GetComponent<UIP_ChatRoom>();

            if (_ChatReceiveMsg.is_mine)
            {
                _comp.CreateMyTalkBalloon(_ChatReceiveMsg.UUID, _ChatReceiveMsg.time, _ChatReceiveMsg.talk_text);
            }               
            else
            {
                _comp.CreateOtherTalkBalloon(_ChatReceiveMsg.UUID, _ChatReceiveMsg.user_name, _ChatReceiveMsg.time, _ChatReceiveMsg.talk_text);
            }
        break;            

        default:
        return;
        }
    }

}