
public class MsgCall :MsgBase
{
    public MsgCall()
    {
        protoName = "MsgCall";
    }
    //˭�е�����
    public string id;
    public bool call;

    /// <summary>
    /// �е����Ľ��  0��ʾ�����е��� 1��ʾ������  2��ʾ���Ҷ���Ҫ����Ҫ�����·���
    /// </summary>
    public int result;
}
