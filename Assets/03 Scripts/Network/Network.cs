using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Network : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class jdPacket
{
	public Int16 m_type { get; set; }
	public byte[] m_data 
	{ 
		get; set;
	}



	public jdPacket()
		{
	}

	public void SetData(byte[] data, int len)
		{
		m_data = new byte[len];
		Array.Copy(data, m_data, len);
	}

	public byte[] GetSendBytes()
	{
		byte[] type_bytes       = BitConverter.GetBytes(m_type);
		int header_size       	= (int)(m_data.Length);
		byte[] header_bytes     = BitConverter.GetBytes(header_size);
		byte[] send_bytes       = new byte[header_bytes.Length + type_bytes.Length + m_data.Length];

		//헤더 복사. 헤더 == 데이터의 크기
		Array.Copy(header_bytes, 0, send_bytes, 0 , header_bytes.Length);        
        
        	//타입 복사
		Array.Copy(type_bytes, 0, send_bytes, header_bytes.Length , type_bytes.Length);        
        
        	//데이터 복사
		Array.Copy(m_data, 0, send_bytes, header_bytes.Length + type_bytes.Length , m_data.Length);        

		return send_bytes;
	}
}