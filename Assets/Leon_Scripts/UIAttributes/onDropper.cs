using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class onDropper : MonoBehaviour, IDropHandler
{

    private Image image;
    /// <summary>
    /// 获取射线需要碰撞的层
    /// </summary>
    private LayerMask canDragSphere;
    /// <summary>
    /// 直接从外部定义好层，简单理解
    /// </summary>
    public LayerMask canDragAttributes;
    //改变物体的模型
    private Mesh mesh;

    void OnEnable()
    {
        mesh = GetComponent<Mesh>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Mesh s = GetMesh(eventData);
        //if (s != null)
        //mesh = s;
        canDragSphere = LayerMask.NameToLayer("Sphere");
        canDragAttributes = LayerMask.NameToLayer("Attributes");
    }
    private Mesh GetMesh(PointerEventData eventData)
    {
        GameObject goSource = eventData.pointerDrag;
        if (goSource == null)
            return null;

        Mesh meshSource = eventData.pointerDrag.GetComponent<OnDragger>().mesh;
        Debug.Log("mesh:"+meshSource.name);
        if (meshSource == null)
            return null;
        return meshSource;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        List<RaycastHit> hitInfos = new List<RaycastHit>();
        Physics.RaycastAll(ray, Mathf.Infinity);
        foreach (var hitInfo in hitInfos)
        {
            Debug.Log("Name:" + hitInfo.collider.gameObject.name);
        }
    }
}
