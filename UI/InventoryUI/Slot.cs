using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected Button button;

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClick);
    }

    protected virtual void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClick);
    }

    protected abstract void OnClick();

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltip.Instance.HideTooltip();
    }
}
