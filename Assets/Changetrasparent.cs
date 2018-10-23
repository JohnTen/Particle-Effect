using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Changetrasparent : MonoBehaviour {

    private RectTransform circleTransform;
    private Color circleImage;
    [SerializeField]
    private float changeFrequency;
    [SerializeField]
    private float changeFrequency2;
    private float transparent;

    private void Start()
    {
        circleTransform = this.GetComponent<RectTransform>();
        circleImage = this.GetComponent<Image>().color;
    }

    private void Update()
    {
        float x=0, y=0;
        ChangeTransparent(transparent);
        ChangScale(x,y);
        if(x>=1f)
        {
            x = 0;
        }
        if (y >= 1f)
        {
            y = 0;
        }
        if(transparent >=255)
        {
            transparent = 0;
        }
    }
    private void ChangeTransparent(float transparent)
    {
        transparent = Mathf.Lerp(0, 1, Time.deltaTime / changeFrequency);
        circleImage = new Color(1, 1, 1, transparent);
    }
    private void ChangScale(float x, float y)
    {


        x = Mathf.Lerp(0, 1, Time.deltaTime / changeFrequency2);
        y = Mathf.Lerp(0, 1, Time.deltaTime / changeFrequency2);

        Vector3 vec1 = circleTransform.localScale;

        vec1.x = x;
        vec1.y = y;

        circleTransform.localScale = vec1;

    }
}
