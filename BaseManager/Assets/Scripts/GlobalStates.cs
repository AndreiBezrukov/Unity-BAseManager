using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalStates : MonoBehaviour
{

    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;
    [SerializeField] private GameObject _gamePanel;

    private State _state = State.MAIN_MENU;
    public enum State 
    { 
        MAIN_MENU   = -1,
        IN_GAME     = 0,
        ON_PAUSE    = 1,
        WIN         = 2,
        LOSE        = 3
    }
    public State GetState()                 { return _state; }
    private void ChangeState(State state)   { _state = state; }
    public void SetState2InGame()           { ChangeState(State.IN_GAME); }
    public void SetState2MainMenu()         { ChangeState(State.MAIN_MENU); }
    public void SetState2OnPause()          { ChangeState(State.ON_PAUSE); }
    public void SetState2Win()              { ChangeState(State.WIN); }
    public void SetState2Lose()             { ChangeState(State.LOSE); }

    // Set On Click
    public void StartGame()     // Set On Click to Start_button
    {
        _menuPanel.SetActive    (false);

        _gamePanel.SetActive    (true);
        _pausePanel.SetActive   (true);

        SetState2OnPause();
        Debug.Log("Global states: Start Game");
    }
    public void PauseGame()     // Set On Click to Pause_button
    {
        _gamePanel.SetActive    (true);
        _pausePanel.SetActive   (true);  

        SetState2OnPause();
        Debug.Log("Global states: Pause Game");
    }
    public void ContinueGame()  // Set On Click to Continue_button
    {
        _pausePanel.SetActive   (false);

        SetState2InGame();
        Debug.Log("Global states: In Game");
    }
    public void EndGame()       // Set On Click to Exit_button
    {
        
        _gamePanel.SetActive    (false);
        _pausePanel.SetActive   (false);
        _losePanel.SetActive    (false);
        _winPanel.SetActive     (false);

        _menuPanel.SetActive    (true);

        _gamePanel.GetComponent<Game>().Reset(); // Reset to start from menu new game

        SetState2MainMenu();
        Debug.Log("Global states: Main Menu");
    }
    
    // Events
    public void Win()
    {
        _gamePanel.SetActive    (false);
        _winPanel.SetActive     (true);

        SetState2Win();
        Debug.Log("Global states: Win");
    }
    public void Lose()
    {
        _gamePanel.SetActive    (false);
        _losePanel.SetActive    (true);

        SetState2Lose();
        Debug.Log("Global states: Lose Game");
    }

    public void Init()
    {
        _gamePanel.SetActive(false);
        _pausePanel.SetActive(false);
        _winPanel.SetActive(false);
        _losePanel.SetActive(false);

        _menuPanel.SetActive(true);

        SetState2MainMenu();
        Debug.Log("Global states: Initialized Scene script");
    }

    void Start()
    {
        Init();
        Debug.Log("Global states: Start Scene script");
    }

    void Update()
    {
        
    }
}
