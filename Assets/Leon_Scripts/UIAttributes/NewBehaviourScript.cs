using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class NewBehaviourScript : MonoBehaviour,IDropHandler {

        /// <summary>
    /// 获取射线需要碰撞的层
    /// </summary>
    private LayerMask canDragSphere;
    /// <summary>
    /// 直接从外部定义好层，简单理解
    /// </summary>
    public LayerMask canDragAttributes;
    RaycastHit[] hitout;
    public bool Drop;
    private void Awake()
    {
        canDragSphere = LayerMask.NameToLayer("Sphere");
        canDragAttributes = LayerMask.NameToLayer("Attributes");
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        hitout = Physics.RaycastAll(ray, Mathf.Infinity, 1 << canDragAttributes);

        if (hitout.Length > 0)
        {
            for (int i = 0; i < hitout.Length; i++)
            {
                Debug.Log("   " + hitout[i].collider.name+ Drop);
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        print(name);
        Drop = true;
        GameObject dragGameObject = eventData.pointerDrag;
        Mesh mesh1 = dragGameObject.GetComponent<OnDragger>().mesh;
        if(hitout.Length>0)
        {

            for (int i = 0; i < hitout.Length; i++)
            {
                hitout[i].collider.gameObject.GetComponent<MeshFilter>().mesh = mesh1;
            }
        }
    }
}
