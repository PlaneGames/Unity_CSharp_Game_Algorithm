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

    //클라이언트로부터 받은패킷 클래스를 담아 놓는다.
    ArrayList Buffer = new ArrayList();

    ArrayList ByteBuffers = new ArrayList();

    public const int PortNumb = 12345;

    private void Start()
    {
        Debug.Log("Server Start");
        this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, PortNumb);   //IPAddress.Any : 모든 네트워크로부터 들을 준비하겠다.
        //바인딩
        this.serverSocket.Bind(ipLocal);    //클라이언트로부터 받은 소켓을 로컬의 엔드포인트로 연결 하겠다.
        //리스닝
        Debug.Log("Start Listening..");
        this.serverSocket.Listen(100);  // backlog : 클라이언트의 최대 수
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

    private void Update()
    {
        ArrayList listenList = new ArrayList();
        listenList.Add(this.serverSocket);

        Socket.Select(listenList, null, null, 1000);
       
        //<연결요청>
        //받은 연결요청이 있다면 리슨리스트는 0이 아니다.
        for(int i = 0; i<listenList.Count; i++)
        {
            //Accept
            Socket newConnection = ((Socket)listenList[i]).Accept();
            //클라이언트 소켓을 저장
            this.Connections.Add(newConnection);
            //
            this.ByteBuffers.Add(new ArrayList());
            Debug.Log("New Client Connected");
        }

        //서버와 연결된 클라이언트들이 하나라도 있다면
        if(Connections.Count != 0)
        {
            ArrayList cloneConnections = new ArrayList(this.Connections);
            Socket.Select(cloneConnections, null, null, 1000);
            foreach(Socket client in cloneConnections)
            {
                byte[] receivedBytes = new byte[512];
                ArrayList buffer = (ArrayList)this.ByteBuffers[cloneConnections.IndexOf(client)];

                //클라이언트로부터 전송된 데이터 담기
                int read = client.Receive(receivedBytes);
                for(int i = 0; i<read; i++)
                {
                    buffer.Add(receivedBytes[i]);
                }

                while(buffer.Count > 0)
                {
                    //패킷의 첫번째의 정보는 전체 데이터의 크기임 그걸 가져옴.
                    int packetDataLength = (byte)buffer[0];
                    if(packetDataLength < buffer.Count)
                    {
                        ArrayList thisPacketBytes = new ArrayList(buffer);
                        //버퍼의 뒷부분 잘라내기
                        thisPacketBytes.RemoveRange(packetDataLength, thisPacketBytes.Count - (packetDataLength + 1));
                        //버퍼의 가장 첫부분 잘라내기
                        thisPacketBytes.RemoveRange(0, 1);
                        buffer.RemoveRange(0, packetDataLength + 1);

                        byte[] readBytes = (byte[])thisPacketBytes.ToArray(typeof(byte));

                        SimplePacket readpacket = SimplePacket.FromByteArray(readBytes);
                        this.Buffer.Add(readpacket);

                        Debug.LogWarning("Packet Receive From Client IP :[" + client.RemoteEndPoint.ToString() + "]"
                            + readpacket.mouseX.ToString() + " / " + readpacket.mouseY.ToString());
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}