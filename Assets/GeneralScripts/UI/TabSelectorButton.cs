// Based On tutorial by Game Dev Guide https://www.youtube.com/watch?v=211t6r12XPQ&t=64s
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Unity.VectorGraphics;

[RequireComponent(typeof(SVGImage))]
public class TabSelectorButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private TabGroup tabGroup;

    [Header("Callbacks")]
    [SerializeField] private UnityEvent onTabSelected;
    [SerializeField] private UnityEvent onTabDeselected;

    [HideInInspector] public SVGImage background;

    private void Awake()
    {
        background = GetComponent<SVGImage>();
        tabGroup.Subscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit();
    }

    public void Select()
    {
        onTabSelected?.Invoke();
    }

    public void Deselect()
    {
        onTabDeselected?.Invoke();
    }
}
