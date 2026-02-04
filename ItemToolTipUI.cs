using UnityEngine;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance { get; private set; }

    public GameObject tooltipWindow;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        HideTooltip();
    }
    public void ShowTooltip(string title, string description)
    {
        tooltipWindow.SetActive(true);
        titleText.text = title;
        descriptionText.text = description;

        // 마우스 위치로 툴팁 이동 (필요 시)
        tooltipWindow.transform.position = Input.mousePosition;
    }

    public void HideTooltip()
    {
        tooltipWindow.SetActive(false);
    }

    private void Update()
    {
        // 툴팁이 켜져 있다면 마우스를 따라다니게 함
        if (tooltipWindow.activeSelf)
        {
            tooltipWindow.transform.position = Input.mousePosition;
        }
    }
}