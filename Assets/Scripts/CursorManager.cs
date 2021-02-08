using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    // 커스텀 마우스 커서
    [SerializeField] Texture2D cursorImg;

    // Start is called before the first frame update 
    void Start()
    {
        Cursor.SetCursor(cursorImg, Vector2.zero, CursorMode.ForceSoftware);
    }
}
