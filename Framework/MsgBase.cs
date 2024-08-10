using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using Newtonsoft.Json;

public class MsgBase
{
   public string protoName="";

    /// <summary>
    /// 编码信息
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static byte[] EncodeMsg(MsgBase msg)
    {
        string s=JsonConvert.SerializeObject(msg);
        return Encoding.UTF8.GetBytes(s);
    }
  /// <summary>
  /// 解码信息
  /// </summary>

    public static MsgBase DecodeMsg(string protoName, byte[] bytes,int offset,int count)
    {
        string s=Encoding.UTF8.GetString(bytes,offset,count);
        return JsonConvert.DeserializeObject(s,Type.GetType(protoName))as MsgBase;
    }
    
    ///<summary
    // 编码协议名字
    ///</summary>
    public static byte[] EncodeName(MsgBase msg)
    {
        byte[] nameBytes  = Encoding.UTF8.GetBytes(msg.protoName);
        short len=(short)nameBytes.Length;
        byte[] bytes = new byte[len+2];
        bytes[0] = (byte)(len%256);
        bytes[1] = (byte)(len/256);
        Array.Copy(nameBytes,0,bytes,2,len);
        return bytes;
    }
    //解码协议名字
    public static string DecodeName(byte[] bytes,int offset,out int count)
    {
        count = 0;
        if (offset + 2 > bytes.Length) return "";
        short len = (short)((short)bytes[offset ] +(short)bytes[offset + 1] * 256);
        Console.WriteLine("len:"+len+"---"+bytes.Length);
        Console.WriteLine(UTF8Encoding.UTF8.GetString(bytes));

        if (len <= 0)
        {
            return "";
        }
        count = 2 + len;
        return Encoding.UTF8.GetString(bytes, offset + 2, len);
    }
    
}
