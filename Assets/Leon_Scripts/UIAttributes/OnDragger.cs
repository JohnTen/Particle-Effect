using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class OnDragger : MonoBehaviour,IDragHandler, IBeginDragHandler, IEndDragHandler
{

    private Image image;
    public GameObject go;
    /// <summary>
    /// 每个图片代表不同的Mesh
    /// </summary>
    public Mesh mesh;

    void OnEnable()
    {
        image = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (image.sprite == null)
        {
            Debug.LogError("Current component of 'Image' have none 'Sprite'.");
            return;
        }

        go = new GameObject("Draging");
        go.transform.SetParent(eventData.pointerDrag.transform.parent);

        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;

        Image goImg = go.AddComponent<Image>();
        goImg.sprite = image.sprite;
        goImg.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (go == null)
            return;

        go.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(go);
        go = null;
    }
}
