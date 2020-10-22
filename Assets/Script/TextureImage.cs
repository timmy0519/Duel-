using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextureImage : MonoBehaviour
{

    public Texture2D myImg; // 從外部拖拉自己喜歡的圖片進來

    void Awake()
    {
        Sprite s = Sprite.Create(myImg, new Rect(0, 0, myImg.width, myImg.height), Vector2.zero);
        GetComponent<Image>().sprite = s;
    }
}