using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Net.Sockets;


public class Protocol
{
    
    ///<summary> 패킷 전송후, 수신 대기 </summary>
    public const byte REQUEST = 0;
    ///<summary> 패킷 전송 </summary>
    public const byte SEND = 1;
    ///<summary> 패킷 수신 </summary>
    public const byte RESPONSE = 2;

    public readonly ushort msg_idx;                    // 0 ~ 65,535 -> 2 Byte
    public readonly byte req_type;

    static ArrayList data;


    protected Protocol(ushort _msg_idx, byte _type)
    {
        msg_idx = _msg_idx;
        req_type = _type;
        data = new ArrayList();

        // Protocol Header
        byte[] _bytes_msg_idx = BitConverter.GetBytes(_msg_idx);
        data.AddRange(_bytes_msg_idx);
        data.Add(_type);

        //Add(_msg_idx);
        //Add(_type);

        // HeaderArch : [ TotalLen(2B) | MessageIndex(2B) | RequestType(1B) ]
        // DataArch : [ KeyLen(1B) | KeyString(KeyLen) | DataLen(2B) | Data(DataLen) ]
    }

    void AddDataList(byte[] _bytes)
    {
        byte[] _len = new byte[2];
        Array.Copy(BitConverter.GetBytes(_bytes.Length), _len, 2);
        data.AddRange(_len);
        data.AddRange(_bytes);
    }

    void AddKeyList(string _key)
    {
        byte[] _bytes_key = Encoding.UTF8.GetBytes(_key);
        data.Add((byte)_bytes_key.Length);
        data.AddRange(_bytes_key);
    }

    public Protocol Add(string _key, byte _data)
    {
        AddKeyList(_key);
        data.Add((byte)1);
        data.Add((byte)0);
        data.Add(_data);
        return this;
    }

    public Protocol Add(string _key, short _data)
    {
        AddKeyList(_key);
        byte[] _bytes = BitConverter.GetBytes(_data);
        AddDataList(_bytes);
        return this;
    }

    public Protocol Add(string _key, ushort _data)
    {
        AddKeyList(_key);
        byte[] _bytes = BitConverter.GetBytes(_data);
        AddDataList(_bytes);
        return this;
    }

    public Protocol Add(string _key, int _data)
    {
        AddKeyList(_key);
        byte[] _bytes = BitConverter.GetBytes(_data);
        AddDataList(_bytes);
        return this;
    }
    
    public Protocol Add(string _key, uint _data)
    {
        AddKeyList(_key);
        byte[] _bytes = BitConverter.GetBytes(_data);
        AddDataList(_bytes);
        return this;
    }

    public Protocol Add(string _key, long _data)
    {
        AddKeyList(_key);
        byte[] _bytes = BitConverter.GetBytes(_data);
        AddDataList(_bytes);
        return this;
    }

    public Protocol Add(string _key, ulong _data)
    {
        AddKeyList(_key);
        byte[] _bytes = BitConverter.GetBytes(_data);
        AddDataList(_bytes);
        return this;
    }

    public Protocol Add(string _key, string _data)
    {
        AddKeyList(_key);
        byte[] _bytes = Encoding.UTF8.GetBytes(_data);
        AddDataList(_bytes);
        return this;
    }

    public byte[] DataToArray()
    {
        return data.OfType<byte>().ToArray();
    }

    public void SendData()
    {
        byte[] _bytes = DataToArray();
        ResponseData(_bytes);

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

    void ResponseData(byte[] _bytes)
    {
        ushort _pak_type = (ushort)BitConverter.ToInt16(_bytes, 0);
        byte _req_type = _bytes[2];
        Debug.Log(_pak_type + " : " + _req_type);

        string _key = Encoding.UTF8.GetString(_bytes, 4, _bytes[3]);
        ushort _data_len = (ushort)BitConverter.ToInt16(_bytes, 4 + _bytes[3]);
        byte[] _data = new byte[_data_len];
        Array.Copy(_bytes, (int)(4 + _bytes[3] + _data_len), _data, 0, (int)_data_len);
        //_key = ;

        Debug.Log(_bytes[3] + " : " + _key + " : " + _data_len);
        for (int i = 0; i < _data.Length; i ++)
        {
            Debug.Log(_data[i]);
        }

    }

    public static Protocol CHAT_ROOM_OPEN          = new Protocol(0, RESPONSE);

}