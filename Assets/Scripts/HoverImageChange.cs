using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverImageChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite originalSprite;
    public Sprite hoverSprite;
    private Image imageComponent;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        if (originalSprite == null && imageComponent != null)
        {
            originalSprite = imageComponent.sprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.instance.PlayHover();
        if (imageComponent != null && hoverSprite != null)
        {
            imageComponent.sprite = hoverSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (imageComponent != null && originalSprite != null)
        {
            imageComponent.sprite = originalSprite;
        }
    }
}
