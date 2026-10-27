using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public Sprite defaultSprite;
    public Sprite pressedSprite;
    public KeyCode pressedKey;
    
    private SpriteRenderer _spriteRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pressedKey))
        {
            _spriteRenderer.sprite = pressedSprite;
        }

        if (Input.GetKeyUp(pressedKey))
        {
            _spriteRenderer.sprite = defaultSprite;
        }
    }
}
