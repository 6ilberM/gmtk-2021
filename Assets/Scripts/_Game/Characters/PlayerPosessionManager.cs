using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//For Simplicity hard References per Level
public class PlayerPosessionManager : MonoBehaviour
{

    [SerializeField] private PlayerController[] playerControllers;
    [SerializeField] private LevelController levelController;

    [SerializeField] private InputReader inputReader;

    private int _currentCharacter = 0;

    private void Awake()
    {
        inputReader.swapEvent += OnSwap;
        levelController.OnGameStarted.AddListener(OnStart);
        levelController.OnGameEnded.AddListener(OnGameOver);

    }

    private void OnGameOver() { DisableAllPlayers(); }

    private void OnDestroy()
    {
        inputReader.swapEvent -= OnSwap;
        levelController.OnGameStarted.RemoveListener(OnStart);
        levelController.OnGameEnded.RemoveListener(OnGameOver);
    }

    private void OnStart()
    {
        _currentCharacter = 0;
        playerControllers[_currentCharacter].PosessCharacter(true);
    }

    private void DisableAllPlayers()
    {
        foreach (var controller in playerControllers)
        {
            controller.PosessCharacter(false);
        }
    }

    public void MoveToPositionAndSplit(Vector3 positionOffset)
    {
        for (int i = 0; i < playerControllers.Length; i++)
        {
            playerControllers[i].transform.position = positionOffset + (i == 0 ? -1 : 1) * Vector3.right * 4f * i;
        }

    }

    //Bad Practice but we just wanna get this shiz goin as fast as possible 
    /// <summary>
    /// Sequentially Swap Through the avaialable Characters
    /// </summary>
    private void OnSwap()
    {
        //Guard Statement... Do nothing Otherwise
        if (playerControllers.Length <= 1) { return; }
        playerControllers[_currentCharacter++].PosessCharacter(false);

        if (_currentCharacter >= playerControllers.Length) { _currentCharacter = 0; }
        playerControllers[_currentCharacter].PosessCharacter(true);

    }
}
