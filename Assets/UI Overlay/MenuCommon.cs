using System;
using System.Threading;
using Cinemachine;
using UnityEngine;

/// <summary>
/// Operations shared by menus
/// </summary>
public class MenuCommon : MonoBehaviour
{
    private Func<bool> close;
    public void Initialize(Func<bool> _close)
    {
        close = _close;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M))
        {
            close();
        }
    }

    public void EnableDialogMode()
    {
        overlayCursor.showCursor = true;
        cinemachineFreeLook.enabled = false;
        Player.GetComponent<InteractionChecker>().enabled = false;
        Player.GetComponent<FallToGround>().enabled = false;
        Player.GetComponent<CharacterController>().enabled = false;
        Player.GetComponent<ThirdPersonMovement>().enabled = false;
        Player.GetComponent<AnimationExample>().enabled = false;
    }

    public void DisableDialogMode()
    {
        overlayCursor.showCursor = false;
        cinemachineFreeLook.enabled = true;
        Player.GetComponent<InteractionChecker>().enabled = true;
        Player.GetComponent<FallToGround>().enabled = true;
        Player.GetComponent<CharacterController>().enabled = true;
        Player.GetComponent<ThirdPersonMovement>().enabled = true;
        Player.GetComponent<AnimationExample>().enabled = true;
    }

    public void PlayOpenSound()
    {
        soundEffectBoard.MenuOpen.Play();
    }

    public void PlayCloseSound()
    {
        soundEffectBoard.MenuCancel.Play();
    }

    public void PlayMoveSound()
    {
        soundEffectBoard.MenuMove.Play();
    }

    public void PlaySelectSound()
    {
        soundEffectBoard.MenuSelect.Play();
    }

    private SoundEffectBoard _soundEffectBoard;
    private SoundEffectBoard soundEffectBoard
    {
        get
        {
            if (_soundEffectBoard == null)
            {
                _soundEffectBoard = FindObjectOfType<SoundEffectBoard>();
            }

            return _soundEffectBoard;
        }
    }

    private OverlayCursor _overlayCursor;
    private OverlayCursor overlayCursor
    {
        get
        {
            if (_overlayCursor == null)
            {
                _overlayCursor = FindObjectOfType<OverlayCursor>();
            }

            return _overlayCursor;
        }
    }

    private CinemachineFreeLook _cinemachineFreeLook;
    private CinemachineFreeLook cinemachineFreeLook
    {
        get
        {
            if (_cinemachineFreeLook == null)
            {
                _cinemachineFreeLook = FindObjectOfType<CinemachineFreeLook>();
            }

            return _cinemachineFreeLook;
        }
    }

    private GameObject _player;

    public GameObject Player
    {
        get
        {
            if (_player == null)
            {
                _player = FindObjectOfType<CharacterController>().gameObject;
            }

            return _player;
        }
    }
}
