using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HUD : MonoBehaviour
{
    [SerializeField] private Slider _jumpBar0 = null;
    [SerializeField] private Slider _jumpBar1 = null;

    private PlayerCharacter _playerCharacter = null;

    private List<Slider> _jumpBarArray = new List<Slider>();
    private void Start()
    {
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
        if(player)
        {
            _playerCharacter = player;
        }

        if( _jumpBar0 != null && _jumpBar1 != null) 
        {
            if(player)
            {
                MovementBehavior playerMovement = _playerCharacter.GetComponent<MovementBehavior>();
                if(playerMovement == null) { return; }

                SetJumpBar();

                UpdateJumpCount(playerMovement.MaxNumJumps, playerMovement.CurrentJumpNum);

                playerMovement.OnJump += UpdateJumpCount;
            }
        }
    }

    private void UpdateJumpCount(int maxJumpNum, int currentJumpNum)
    {
        if(currentJumpNum == 0)
        {
            for (int sliderIdx = 0; sliderIdx < _jumpBarArray.Count; sliderIdx++)
            {
                _jumpBarArray[sliderIdx].value = _jumpBarArray[sliderIdx].maxValue;
            }
            return;
        }

        _jumpBarArray[currentJumpNum - 1].value = 0;
    }

    private void SetJumpBar()
    {
        _jumpBarArray.Add(_jumpBar1);
        _jumpBarArray.Add(_jumpBar0);

        for(int sliderIdx = 0; sliderIdx < _jumpBarArray.Count; sliderIdx++)
        {
            _jumpBarArray[sliderIdx].value = _jumpBarArray[sliderIdx].maxValue;
        }
    }
}
