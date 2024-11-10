using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ValuableItemHealth : MonoBehaviour
{
    [SerializeField] private Slider _hpBar = null;
    [SerializeField] private int _startHealth = 5;
    [SerializeField] private Rigidbody _rigidBody = null;
    [SerializeField] private SphereCollider _sphereCollider = null;
    [SerializeField] private Color _flickerColor = Color.white;
    [SerializeField] private float _flickerDuration = 1.0f;

    private Material _attachedMaterial;

    private bool _wasThrown = false;
    private int _currentHealth = 0;
    private const float GROUND_CHECK_DISTANCE = 1f;
    private const string GROUND_LAYER = "Ground";
    private const string COLOR_PARAMETER = "_BaseColor";

    public bool wasThrown
    {
        get { return _wasThrown; }
        set { _wasThrown = value; }
    }
    public int currentHealth
    {
        get { return _currentHealth; }
    }

    public int startHealth
    {
        get { return _startHealth; }
    }
    private void Start()
    {
        _currentHealth = _startHealth;

        //set item material so it can be adjusted later
        Renderer renderer = transform.GetComponentInChildren<Renderer>();
        if (renderer)
        {
            _attachedMaterial = renderer.material;
        }
        _hpBar.maxValue = _startHealth;
    }
    void Update()
    {
        if(_wasThrown)
        {
            HandleFallBehavior();
        }
    }

    void HandleFallBehavior()
    {
        if(_rigidBody == null || _sphereCollider == null) { return; }
        
        RaycastHit hitInfo = new RaycastHit();

        //check for ground collision
        if (Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down, out hitInfo,
                 _sphereCollider.radius + GROUND_CHECK_DISTANCE, LayerMask.GetMask(GROUND_LAYER)))
        {
            if(hitInfo.normal == Vector3.up) 
            {
                _wasThrown = false;
                _currentHealth--;
                StartCoroutine(HandleColorFlicker());
                _hpBar.value = _currentHealth;

                if (_currentHealth <= 0)
                    Kill();
            }
        }
    }

    private IEnumerator HandleColorFlicker()
    {
        Color startColor = _attachedMaterial.GetColor(COLOR_PARAMETER);
        float time = _flickerDuration;
        float normalizedTime;
        while (time > 0)
        {
            time -= Time.deltaTime;
            normalizedTime = Mathf.Clamp01(time / _flickerDuration);

            var currentColor = startColor;
            var targetColor = _flickerColor;

            var lerpTime = 1.0f - ((normalizedTime - 0.5f) * 2.0f);
            if (normalizedTime < 0.5f)
            {
                currentColor = _flickerColor;
                targetColor = startColor;

                lerpTime = 1.0f - (normalizedTime * 2.0f);
            }

            var finalColor = Color.Lerp(currentColor, targetColor, lerpTime);
            _attachedMaterial.SetColor(COLOR_PARAMETER, finalColor);
            yield return new WaitForEndOfFrame();
        }
    }

    void Kill()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
