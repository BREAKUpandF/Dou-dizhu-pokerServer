
using System;
using System.Collections;
using System.Collections.Generic;

public class ByteArray
{
    const int DEFAULT_SIZE = 1024;

    /// <summary>
    /// 初始大小
    /// </summary>
    private int initSize;
  public byte[] bytes;
    /// <summary>
    /// 读写索引
    /// readIndex: 读数据的头，writeIndex: 写数据的头和读数据的尾
    /// </summary>
   
    public int readIndex;
    public int writeIndex;

    /// <summary>
    /// 数组容量
    /// </summary>
    private int capacity;

    /// <summary>
    /// 读写之间的长度
    /// </summary>
    public int Length { get { return writeIndex - readIndex; } }
    /// <summary>
    /// 数组的余量
    /// </summary>
    public int Remain { get { return capacity - writeIndex; } }

    /// <summary>
    /// 提供长度
    /// </summary>
    /// <param name="size"></param>
    public ByteArray(int size = DEFAULT_SIZE)
    {
        bytes = new byte[size];
        initSize = size;
        capacity = size;
        readIndex = 0;
        writeIndex = 0;
    }
    /// <summary>
    /// 提供字节数组
    /// </summary>
    /// <param name="bytes"></param>
    public ByteArray(byte[] bytes)
    {
        this.bytes = bytes;
        initSize = bytes.Length;
        capacity = bytes.Length;
        readIndex = 0;
        writeIndex = bytes.Length;
    }
    /// <summary>
    /// 覆盖前方已经被用过的数组，节省内存开销
    /// </summary>
    public void MoveBytes()
    {
        if(Length > 0)
        {
            Array.Copy(bytes, readIndex, bytes, 0, Length);
              
        }  
        writeIndex = Length;  
         readIndex = 0;
             
    }

    /// <summary>
    /// 扩容数组大小
    /// </summary>
    /// <param name="size"></param>
    public void ReSize(int size)
    {
        if (size < Length)
        {
            return;
        }
        if (size < initSize) return;

        capacity = size;
        byte[] newBytes = new byte[capacity];
        Array.Copy(bytes, readIndex, newBytes, 0, Length);
        bytes = newBytes;
    }

}
