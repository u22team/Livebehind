using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontAnimation : MonoBehaviour
{
    int fontSizeDiff, fontSize;
    [SerializeField] int period = 10;
    [SerializeField] int size = 5;

    void Start()
    {
        fontSizeDiff = period;
        fontSize = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (fontSizeDiff == -period) fontSizeDiff = period;
        else fontSizeDiff -= 1;
        fontSize += fontSizeDiff;
        this.gameObject.transform.localScale = Vector3.one * (1 + (float)fontSize / size);
    }
}
