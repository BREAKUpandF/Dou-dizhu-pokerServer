
using System;
using System.Collections;
using System.Collections.Generic;

public class ByteArray
{
    const int DEFAULT_SIZE = 1024;

    /// <summary>
    /// ��ʼ��С
    /// </summary>
    private int initSize;
  public byte[] bytes;
    /// <summary>
    /// ��д����
    /// readIndex: �����ݵ�ͷ��writeIndex: д���ݵ�ͷ�Ͷ����ݵ�β
    /// </summary>
   
    public int readIndex;
    public int writeIndex;

    /// <summary>
    /// ��������
    /// </summary>
    private int capacity;

    /// <summary>
    /// ��д֮��ĳ���
    /// </summary>
    public int Length { get { return writeIndex - readIndex; } }
    /// <summary>
    /// ���������
    /// </summary>
    public int Remain { get { return capacity - writeIndex; } }

    /// <summary>
    /// �ṩ����
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
    /// �ṩ�ֽ�����
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
    /// ����ǰ���Ѿ����ù������飬��ʡ�ڴ濪��
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
    /// ���������С
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
