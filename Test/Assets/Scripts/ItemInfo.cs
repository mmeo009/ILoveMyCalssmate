using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{//잡은 물건 정보 값을 가지고 있는 클래스
    public int slotId;      //슬롯 번호 (Slot 클래스 생성 후)
    public int itemId;      //아이템 번호
    public int slotX;
    public int slotY;

    public void InitDummy(int slotId, int itemId, int slotX, int slotY)
    {//인수로 받은 값들을 Class쪽에 입력
        this.slotId = slotId;
        this.itemId = itemId;
        this.slotX = slotX;
        this.slotY = slotY;
    }
}
