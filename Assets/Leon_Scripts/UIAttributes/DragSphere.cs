using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSphere : MonoBehaviour
{
    /// <summary>
    /// 将要拖动的物体
    /// </summary>
    private Transform dragGameObject;
    /// <summary>
    /// 获取射线需要碰撞的层
    /// </summary>
    private LayerMask canDragSphere;
    /// <summary>
    /// 直接从外部定义好层，简单理解
    /// </summary>
    public LayerMask canDragAttributes;
    /// <summary>
    /// 获得鼠标的位置和cube位置差
    /// </summary>
    private Vector3 offset;
    /// <summary>
    /// 是否点击到cube
    /// </summary>
    private bool isClickCube;
    /// <summary>
    /// 目标对象的屏幕坐标
    /// </summary>
    private Vector3 targetScreenPoint;
    /// <summary>
    /// 记录鼠标按下的位置方便被拖出的属性返回
    /// </summary>
    private Vector3 clickMousePosition;

    // Use this for initialization
    void Start()
    {
        canDragSphere =LayerMask.NameToLayer("Sphere");
        canDragAttributes = LayerMask.NameToLayer("Attributes");
    }

    // Update is called once per frame
    void Update()
    {
        MoveGameObject();
    }

   
    /// <summary>
    /// 移动获取的物体
    /// </summary>
    /// <returns></returns>
    void MoveGameObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CheckGameObject())
            {
                offset = dragGameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, targetScreenPoint.z));
            }
        }

        if (isClickCube)
        {
            //当前鼠标所在的屏幕坐标
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, targetScreenPoint.z);
            //把当前鼠标的屏幕坐标转换成世界坐标
            Vector3 curWorldPoint = Camera.main.ScreenToWorldPoint(curScreenPoint);
            dragGameObject.position = curWorldPoint + offset;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isClickCube = false;
        }
    }

    /// <summary>
    /// 检查是否点击到cube
    /// </summary>
    /// <returns></returns>
    bool CheckGameObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
       
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << canDragSphere | 1 << canDragAttributes))
        {
            isClickCube = true;
            //得到射线碰撞到的物体
            clickMousePosition = Input.mousePosition;
            //记录鼠标按下位置
            dragGameObject = hit.collider.gameObject.transform;
            Debug.Log(dragGameObject.name);
            targetScreenPoint = Camera.main.WorldToScreenPoint(dragGameObject.position);
            return true;
        }
        return false;
    }


}