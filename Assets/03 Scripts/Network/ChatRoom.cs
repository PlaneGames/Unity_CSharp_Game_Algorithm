using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;


public struct ChatRoomData
{
    public int room_code;
    public string room_name;
    public int room_pw;

    public ChatRoomData (
        int _code,
        string _name,
        int _pw
    )
    {
        room_code = _code;
        room_name = _name;
        room_pw = _pw;
    }
}

public class ChatRoom : MonoBehaviour
{
    // Room
    public static Dictionary<int, ChatRoomData> rooms;

    public static void Init()
    {
        rooms = new Dictionary<int, ChatRoomData>();
    }

    private static void GenChatRoomCode(Action<int, TroubleShooter> Result)
    {
        TroubleShooter _ts = new TroubleShooter();

        if (rooms != null)
        {
            int _new_code;
            while (true)
            {
                bool _complete = true;
                _new_code = UnityEngine.Random.Range(0, 1000);
                foreach (KeyValuePair<int, ChatRoomData> item in rooms)
                {
                    if (item.Key == _new_code)
                    {
                        _complete = false;
                        break;
                    }
                }
                if (_complete)
                    break;
            }

            _ts.Successed();
            Result(_new_code, _ts);
            return;
        }

        _ts.HasError(ErrorCode.ChatRoomNotInitialized);
        Result(-1, _ts);
        return;
    }

    public static void CreateChatRoom(string _room_name, int _room_pw, Action<TroubleShooter> Result)
    {
        GenChatRoomCode( ( _code, _Result ) => 
        {
            if (_Result.success)
            {
                if (_code >= 0)
                {
                    ChatRoomData _data = new ChatRoomData(_code, _room_name, _room_pw);
                    rooms.Add(_code, _data);
                    TroubleShooter ts = new TroubleShooter();
                    ts.Successed();
                    Result(ts);
                }
            }
        });
    }
}