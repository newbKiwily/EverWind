using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpUIManager : MonoBehaviour
{
    public Inventory inventory;
    public CraftUI craftUI;
    private GameObject currentUI;
    private bool isPopUpOn;
    public CameraMoving cameraMoving;
    private ItemTooltip itemTooltip;
    public DeadUI deadUI;
    
    private void Start()
    {
        isPopUpOn = false;
        inventory.gameObject.SetActive(false);
        craftUI.gameObject.SetActive(false);
        itemTooltip=this.GetComponent<ItemTooltip>();
        deadUI.gameObject.SetActive(false);
    }

    public void PopUpDeadUI()
    {
        deadUI.gameObject.SetActive(true);
        isPopUpOn = true;
        currentUI = deadUI.gameObject;
        return;
    }
    public void closeDeadUI()
    {
        deadUI.gameObject.SetActive(false);
        isPopUpOn = false;
        currentUI = null;
        return;
    }

    private void Update()
    {
        if (InputManager.Instance.GetInventoryKeyDown()&&isPopUpOn==false)
        {
            isPopUpOn = true;
            inventory.gameObject.SetActive(true);
            currentUI = inventory.gameObject;
            inventory.UpdateInventory();
            cameraMoving.onPreviewCam();
            return;
        }
        if(InputManager.Instance.GetCraftUIKeyDown()&&isPopUpOn==false)
        {
            isPopUpOn = true;
            craftUI.gameObject.SetActive(true);
            currentUI = craftUI.gameObject;
            craftUI.flushCraftZone();
            return;
        }
        if(InputManager.Instance.GetUICloseKeyDown())
        {
            currentUI.gameObject.SetActive(false);
            currentUI = null;
            isPopUpOn = false;
            itemTooltip.tooltipWindow.SetActive(false);
            cameraMoving.offPreviCam();
            return;
        }
        
    }

}
