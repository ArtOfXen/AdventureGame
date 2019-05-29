using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewspaperClippingScript : Selectable
{

    //public Image image;

    private Vector2 startingSize;
    private Vector2 highlightedSize;

    // Start is called before the first frame update
    protected override void Start()
    {
        //startingSize = new Vector2(image.GetComponent<RectTransform>().rect.width, image.GetComponent<RectTransform>().rect.height);
        startingSize = new Vector2(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height);
        calculateHighlightedSize();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsHighlighted())
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(highlightedSize.x, highlightedSize.y);
        }
        else
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(startingSize.x, startingSize.y);
        }
    }

    void calculateHighlightedSize()
    {
        float largestAxisFactor = 10f;
        float xFactor, yFactor;
        if (startingSize.x > startingSize.y)
        {
            xFactor = largestAxisFactor;
            yFactor = (startingSize.y / startingSize.x) * largestAxisFactor;
        }
        else
        {
            yFactor = largestAxisFactor;
            xFactor = (startingSize.x / startingSize.y) * largestAxisFactor;
        }

        highlightedSize = new Vector2(xFactor + startingSize.x, yFactor + startingSize.y);
    }

}
