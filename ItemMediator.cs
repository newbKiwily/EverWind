using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMediator : MonoBehaviour
{
    static public ItemMediator Instance;
    public PlayerStateContexter playerStateContexter;
    public Inventory inventory;
    public SerializedDictionary<string, InventoryItem> itemTable=new SerializedDictionary<string, InventoryItem>();
    public SerializedDictionary<int, GameObject> FieldItemTable = new SerializedDictionary<int, GameObject>();

    private void Awake()
    {
        if (null == Instance)
        {           
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    
    }

    public void mediation(string key)
    {
        var target = itemTable[key];
        Debug.Log("인벤토리 아이템 생성됌");
        inventory.putItem(target);
        //인벤토리로 전송
    }

    // 아이템을 비활성화하고 일정 시간 뒤에 다시 활성화하는 메서드
    public void itemRespawn(GameObject itemObject, float delay)
    {      
        itemObject.SetActive(false);
        StartCoroutine(RespawnAfterDelay(itemObject, delay));
    }

    // 오브젝트를 일정 시간 후에 다시 활성화하는 코루틴
    private IEnumerator RespawnAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(true);
        
        // FieldItem 컴포넌트를 가져와서 초기화
        FieldItem fieldItem = obj.GetComponent<FieldItem>();
        if (fieldItem != null)
        {
            fieldItem.initialize();
            var packet = PacketMethod.BuildResourceRespawnRequest(fieldItem.resourceId);
            NetworkClient.Instance.Send(packet);
        }
    }

    public InventoryItem getItemInfo(string itemname)
    {
        var item= itemTable[itemname];
        if(item==null)
            return null; 
        return item;
    }
}
