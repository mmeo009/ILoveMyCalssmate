using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{//���� ���� ���� ���� ������ �ִ� Ŭ����
    public int slotId;      //���� ��ȣ (Slot Ŭ���� ���� ��)
    public int itemId;      //������ ��ȣ
    public int slotX;
    public int slotY;

    public void InitDummy(int slotId, int itemId, int slotX, int slotY)
    {//�μ��� ���� ������ Class�ʿ� �Է�
        this.slotId = slotId;
        this.itemId = itemId;
        this.slotX = slotX;
        this.slotY = slotY;
    }
}
