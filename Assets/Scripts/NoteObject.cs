using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public GameObject missEffect;
    public GameObject normalEffect;
    public GameObject goodEffect;
    public GameObject perfectEffect;

    public KeyCode keyToPress;
    
    public bool canBePressed;
    
    private bool _obtained = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keyToPress))
        {
            if (canBePressed)
            {
                _obtained = true;
                
                gameObject.SetActive(false);
                
                //GameManager.instance.NoteHit();
                
                if (Mathf.Abs(transform.position.y) > 0.25f)
                {
                    GameManager.instance.NormalHit();
                    Instantiate(normalEffect, transform.position, normalEffect.transform.rotation);
                    Debug.Log("Normal Hit");
                }
                else if (Mathf.Abs(transform.position.x) > 0.1f)
                {
                    GameManager.instance.GoodHit();
                    Instantiate(goodEffect, transform.position, goodEffect.transform.rotation);
                    Debug.Log("Good Hit");
                }
                else
                {
                    GameManager.instance.PerfectHit();
                    Instantiate(perfectEffect, transform.position, perfectEffect.transform.rotation);
                    Debug.Log("Perfect Hit");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Activator"))
        {
            canBePressed = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (gameObject.activeSelf)
        {
            if (other.CompareTag("Activator"))
            {
                canBePressed = false;

                if (!_obtained) GameManager.instance.NoteMissed();

                Instantiate(missEffect, transform.position, missEffect.transform.rotation);
            }
        }
    }
}
