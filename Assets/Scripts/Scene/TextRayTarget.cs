using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Colliderの大きさを Textの大きさに合わせる。

[RequireComponent(typeof(BoxCollider2D), typeof(Text))]
public class TextRayTarget : MonoBehaviour
{
    Text text;
    BoxCollider2D bc;

    void Start()
    {
        text = GetComponent<Text>();
        bc = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        bc.size = new Vector2(text.preferredWidth, text.preferredHeight);
    }
}
