using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public GameObject missEffect;
    public GameObject normalEffect;
    public GameObject goodEffect;
    public GameObject perfectEffect;

    public KeyCode keyToPress;
    public bool canBePressed;

    [Header("Настройки длинной ноты")]
    public bool isLongNote;
    public float holdDuration; // В секундах
    public Transform visualBody; // Ссылка на дочерний спрайт хвоста (или сам объект)

    private float _activatorY;
    private bool _obtained = false;
    private bool _isHolding = false;
    private float _holdTimer = 0f;
    private float _tickScoreTimer = 0f;

    public void Setup(KeyCode key, float targetY, bool longNote, float durationSec, float fallSpeed)
    {
        keyToPress = key;
        _activatorY = targetY;
        isLongNote = longNote;
        holdDuration = durationSec;

        if (isLongNote && durationSec > 0f)
        {
            float lengthInUnits = durationSec * fallSpeed;
            if (visualBody != null)
            {
                Vector3 scale = visualBody.localScale;
                scale.y = lengthInUnits;
                visualBody.localScale = scale;
            }
            else
            {
                Vector3 scale = transform.localScale;
                scale.y = lengthInUnits;
                transform.localScale = scale;
            }
        }
    }

    void Update()
    {
        // Начало нажатия
        if (Input.GetKeyDown(keyToPress) && canBePressed && !_obtained)
        {
            _obtained = true;
            EvaluateHitAccuracy();

            if (isLongNote && holdDuration > 0f)
            {
                _isHolding = true;
                _holdTimer = holdDuration;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        // Удержание длинной ноты
        // Удержание длинной ноты с визуальным сокращением хвоста
        if (_isHolding)
        {
            // Фиксируем головку ноты ровно на линии кнопки нажатия
            transform.position = new Vector3(transform.position.x, _activatorY, transform.position.z);

            if (Input.GetKey(keyToPress))
            {
                _holdTimer -= Time.deltaTime;
                _tickScoreTimer += Time.deltaTime;

                // Плавно уменьшаем высоту хвоста пропорционально оставшемуся времени
                if (visualBody != null)
                {
                    Vector3 scale = visualBody.localScale;
                    scale.y = Mathf.Max(0f, _holdTimer * GameManager.instance.beatScroller.fallSpeed);
                    visualBody.localScale = scale;
                }

                if (_tickScoreTimer >= 0.1f)
                {
                    _tickScoreTimer = 0f;
                    GameManager.instance.AddHoldPoints(15);
                }

                if (_holdTimer <= 0f)
                {
                    _isHolding = false;
                    GameManager.instance.PerfectHit();
                    if (perfectEffect != null)
                        Instantiate(perfectEffect, new Vector3(transform.position.x, _activatorY, 0), perfectEffect.transform.rotation);
                    gameObject.SetActive(false);
                }
            }
            else if (Input.GetKeyUp(keyToPress))
            {
                _isHolding = false;
                if (_holdTimer > 0.15f)
                {
                    GameManager.instance.NoteMissed();
                    if (missEffect != null)
                        Instantiate(missEffect, new Vector3(transform.position.x, _activatorY, 0), missEffect.transform.rotation);
                }
                gameObject.SetActive(false);
            }
        }
    }

    private void EvaluateHitAccuracy()
    {
        float distance = Mathf.Abs(transform.position.y - _activatorY);

        if (distance > 0.35f)
        {
            GameManager.instance.NormalHit();
            if (normalEffect != null) Instantiate(normalEffect, transform.position, normalEffect.transform.rotation);
        }
        else if (distance > 0.15f)
        {
            GameManager.instance.GoodHit();
            if (goodEffect != null) Instantiate(goodEffect, transform.position, goodEffect.transform.rotation);
        }
        else
        {
            GameManager.instance.PerfectHit();
            if (perfectEffect != null) Instantiate(perfectEffect, transform.position, perfectEffect.transform.rotation);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Activator"))
        {
            canBePressed = true;
            _activatorY = other.transform.position.y;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (gameObject.activeSelf && other.CompareTag("Activator"))
        {
            canBePressed = false;

            if (!_obtained)
            {
                GameManager.instance.NoteMissed();
                if (missEffect != null)
                    Instantiate(missEffect, transform.position, missEffect.transform.rotation);
                gameObject.SetActive(false);
            }
        }
    }
}