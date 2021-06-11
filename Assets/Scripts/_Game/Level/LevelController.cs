using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelController : Singleton<LevelController>
{
    public UnityEvent OnGameStarted;
    public UnityEvent OnGameEnded;

    public void StartGame() { OnGameStarted.Invoke(); }

    public void EndGame() { OnGameStarted.Invoke(); }

}
