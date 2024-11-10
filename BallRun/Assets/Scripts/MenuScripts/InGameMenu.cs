using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputAsset;
    [SerializeField] private UnityEvent _openMenu;
    [SerializeField] private UnityEvent _closeMenu;

    private InputAction _openMenuAction;
    private bool _openedMenu = false;
    private void Awake()
    {
        if (_inputAsset == null) return;

        _openMenuAction = _inputAsset.FindActionMap("Menu").FindAction("OpenMenu");

        _openMenuAction.performed += OpenMenu;
        _closeMenu?.Invoke();
    }

    private void OpenMenu(InputAction.CallbackContext context)
    {
        if(!_openedMenu)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            _openMenu?.Invoke();
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _closeMenu?.Invoke();
            Time.timeScale = 1f;
        }

        _openedMenu = !_openedMenu;
    }

    public void CloseMenu()
    {
        _openedMenu = !_openedMenu;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        _closeMenu?.Invoke();
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
