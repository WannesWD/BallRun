using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerCharacterEffectManager : MonoBehaviour
{
    //[SerializeField] 
    //GameObject _speedLineVFXTemplate = null;
    [SerializeField]
    private VisualEffect _speedLineVFX = null;

    [SerializeField]
    private Color _doubleJumpColor = Color.blue;

    [SerializeField]
    private Material _attachedMaterial = null;


    const string COLOR_PARAMETER = "_BaseColor";

    private Color _originalColor;
    private GameObject speedlineEffect;
    private void Start()
    {
        if(_attachedMaterial == null)
        {
            Renderer renderer = transform.GetComponentInChildren<Renderer>();
            if (renderer)
            {
                _attachedMaterial = renderer.material;
            }
        }

        _originalColor = _attachedMaterial.color;
    }
    // Update is called once per frame

    public void DrawSpeeLines(bool draw)
    {
        if (_speedLineVFX == null) return;

        if (draw)
            _speedLineVFX.Play();
        else
            _speedLineVFX.Stop();
    }

    public void SetDoubleJumpColor(bool doubleJump)
    {
        if (doubleJump) 
        {
            _attachedMaterial.SetColor(COLOR_PARAMETER, _doubleJumpColor);
        }
        else
        {
            _attachedMaterial.SetColor(COLOR_PARAMETER, _originalColor);
        }
    }

    void OnDestroy()
    {
        if (_attachedMaterial == null) return;

        _attachedMaterial.SetColor(COLOR_PARAMETER, _originalColor);
    }
}
