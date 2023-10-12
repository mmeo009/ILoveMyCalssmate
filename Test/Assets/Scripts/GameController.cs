using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Slot[] slots;                                //���� ��Ʈ�ѷ������� Slot �迭�� ����

    private Vector3 _target;
    private ItemInfo carryingItem;                      //��� �ִ� ������ ���� �� ����

    private Dictionary<int, Slot> slotDictionary;       //Slot id, Slot class �����ϱ� ���� �ڷᱸ��


    private void Start()
    {
        slotDictionary = new Dictionary<int, Slot>();   //�ʱ�ȭ

        for (int i = 0; i < slots.Length; i++)
        {                                               //�� ������ ID�� �����ϰ� ��ųʸ��� �߰�
            slots[i].id = i;
            slotDictionary.Add(i, slots[i]);
        }

        PlaceItem(1, 1);
        PlaceItem(3, 1);
        PlaceItem(3, 2, 3);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlaceRandomItem();
        }

        if (Input.GetMouseButtonDown(0)) //���콺 ���� ��
        {
            SendRayCast();
        }

        if (Input.GetMouseButton(0) && carryingItem)    //��� �̵���ų ��
        {
            OnItemSelected();
        }

        if (Input.GetMouseButtonUp(0))  //���콺 ��ư�� ������
        {
            SendRayCast();
        }
    }
    void SendRayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var slot = hit.transform.GetComponent<Slot>();          //Raycast�� ���� ���� Slotĭ
            if (slot.state == Slot.SLOTSTATE.FULL && carryingItem == null)
            {
                string itemPath = "Prefabs/Item_Grabbed_" + slot.itemObject.id.ToString("000");
                var itemGo = (GameObject)Instantiate(Resources.Load(itemPath));     //������ ����

                itemGo.transform.SetParent(this.transform);
                itemGo.transform.localPosition = Vector3.zero;
                itemGo.transform.localScale = Vector3.one * 2;

                carryingItem = itemGo.GetComponent<ItemInfo>();         //���� ���� �Է�
                carryingItem.InitDummy(slot.id, slot.itemObject.id,slot.x,slot.y);

                slot.ItemGrabbed();
            }
            else if (slot.state == Slot.SLOTSTATE.EMPTY && carryingItem != null)
            {//�� ���Կ� ������ ��ġ
                if ((carryingItem.slotX == slot.x || carryingItem.slotY == slot.y) && !(carryingItem.slotX == slot.x && carryingItem.slotY == slot.y))
                {
                    slot.CreateItem(carryingItem.itemId);       //��� �ִ°� ���� ��ġ�� ����
                    Destroy(carryingItem.gameObject);           //��� �ִ°� �ı�
                }
                else
                {
                    OnItemCarryFail();  //������ ��ġ ����
                }
            }
            else if (slot.state == Slot.SLOTSTATE.FULL && carryingItem != null)
            {//Checking �� ����
                if (slot.itemObject.id == carryingItem.itemId)
                {
                    OnItemMergedWithTarget(slot.id);    //���� �Լ� ȣ��
                }
                else
                {
                    OnItemCarryFail();  //������ ��ġ ����
                }
            }
        }
        else
        {
            if (!carryingItem) return;
            OnItemCarryFail();  //������ ��ġ ����
        }

    }


    void OnItemSelected()
    {   //�������� �����ϰ� ���콺 ��ġ�� �̵� 
        _target = Camera.main.ScreenToWorldPoint(Input.mousePosition);  //��ǥ��ȯ
        _target.z = 0;
        var delta = 10 * Time.deltaTime;
        delta *= Vector3.Distance(transform.position, _target);
        carryingItem.transform.position = Vector3.MoveTowards(carryingItem.transform.position, _target, delta);
    }

    void OnItemMergedWithTarget(int targetSlotId)
    {
        var slot = GetSlotById(targetSlotId);
        Destroy(slot.itemObject.gameObject);            //slot�� �ִ� ��ü �ı�
        slot.CreateItem(carryingItem.itemId + 1);       //���Կ� ���� ��ȣ ��ü ����
        Destroy(carryingItem.gameObject);               //��� �ִ� ��ü �ı�
    }

    void OnItemCarryFail()
    {//������ ��ġ ���� �� ����
        var slot = GetSlotById(carryingItem.slotId);        //���� ��ġ Ȯ��
        slot.CreateItem(carryingItem.itemId);               //�ش� ���Կ� �ٽ� ����
        Destroy(carryingItem.gameObject);                   //��� �ִ� ��ü �ı�
    }
    void PlaceRandomItem()
    {//������ ���Կ� ������ ��ġ
        if (AllSlotsOccupied())
        {
            return;
        }
        var rand = UnityEngine.Random.Range(0, slots.Length); //����Ƽ �����Լ��� �����ͼ� 0 ~ �迭 ũ�� ���� ��
        var slot = GetSlotById(rand);
        while (slot.state == Slot.SLOTSTATE.FULL)
        {
            rand = UnityEngine.Random.Range(0, slots.Length);
            slot = GetSlotById(rand);
        }
        slot.CreateItem(0);
    }
    public void PlaceItem(int x, int y, int itemId = 0)
    {
        for(int i = 0; i < slotDictionary.Count; i++)
        {
            var slot = GetSlotById(i);
            if(slot.x == x && slot.y == y)
            {
                if(itemId != 1)
                {
                    slot.CreateItem(itemId);
                }
                else
                {
                    slot.CreateItem(0);
                }
            }
        }
    }


    bool AllSlotsOccupied()
    {//��� ������ ä���� �ִ��� Ȯ��
        foreach (var slot in slots)                       //foreach���� ���ؼ� Slots �迭�� �˻���
        {
            if (slot.state == Slot.SLOTSTATE.EMPTY)       //����ִ��� Ȯ��
            {
                return false;
            }
        }
        return true;
    }
    Slot GetSlotById(int id)
    {//���� ID�� ��ųʸ����� Slot Ŭ������ ����
        return slotDictionary[id];
    }


}