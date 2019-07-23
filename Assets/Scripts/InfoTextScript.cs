using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoTextScript : MonoBehaviour
{
    private Text text;

    private bool isTextShowing;
    private float showTextStartTime;
    private const float showTextDuration = 5f;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<Text>();
        isTextShowing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTextShowing)
        {
            if (Time.time - showTextStartTime >= showTextDuration)
            {
                text.text = "";
                isTextShowing = false;
            }
        }
    }

    public void updateInfoText(string newText)
    {
        text.text = newText;
        isTextShowing = true;
        showTextStartTime = Time.time;
    }
}
