using UnityEngine;

public class BeatScroller : MonoBehaviour
{
    [HideInInspector] public float fallSpeed = 5f;
    public bool hasStarted;

    void Update()
    {
        if (hasStarted)
        {
            transform.position -= new Vector3(0f, fallSpeed * Time.deltaTime, 0f);
        }
    }
}