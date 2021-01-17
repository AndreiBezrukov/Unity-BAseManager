using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    // Game Object and UI elements
    [SerializeField] public GameObject THEGAME;

    [SerializeField] private Text       _phaseText;
    [SerializeField] private Text       _phaseTimerText;
    [SerializeField] private Text       _gameTimeText;
    [SerializeField] private Text       _IDLEText;

    [SerializeField] private GameObject _combatPanel;
    [SerializeField] private Button     _doFightButton;
    
    [SerializeField] private GameObject _basePanel;
    [SerializeField] private Button     _orderFighterButton;
    [SerializeField] private Button     _orderWorkerButton;

    private GlobalStates    _THEGAMEScript;
    private Combat          _combatScript;
    private Base            _baseScript;

    private Phase   _phase;             // Start game phase
    private long    GameTime;           // Global game time 
    private float   _tempTimer;   
    private float   _delta;   
    public  float   _basePhaseTime;     // Time which base phase lasts
    private float   _basePhaseTimer;    // Timer to end Base Phase and go to Combat
    private int     _round;             // Round COMBAT -> BASE counter
              
    // Getters:
    public float    GetBasePhaseTimer() { return _basePhaseTimer; }
    public int      GetRound()          { return _round; }
    public float    GetDelta()          { return _delta; }
    // Phase managment
    public enum Phase
    {
        BASE    = 1,
        COMBAT  = -1
    }
    public Phase GetPhase()                 { return _phase; }
    private void ChangePhsae(Phase phase)   { _phase = phase; }
    public void SetPhase2Base()             { ChangePhsae(Phase.BASE); }
    public void SetPhase2Combat()           { ChangePhsae(Phase.COMBAT); }

    public void SwitchPhase() // Used to automatically switch betwen BASE and COMBAT states
    {
        if (_phase == Phase.BASE)
        {
            SetPhase2Combat();

            _orderWorkerButton  .interactable = false;
            _orderFighterButton .interactable = false;

            _doFightButton      .interactable = true;

            _combatScript.StartCombat();
        }
        else if (_phase == Phase.COMBAT)
        {
            SetPhase2Base();

            _basePhaseTimer = _basePhaseTime;
            _round++;

            _doFightButton      .interactable = false;

            _baseScript.OrderAvailable();
        }
        Debug.Log("Game: Switch Phase " + _phase.ToString() +" round "+ _round.ToString());
    }
    // Processing game phases
    private void CountGameTime() 
    {
        _delta = Time.deltaTime;
        _tempTimer += _delta;
        if (_tempTimer >= 1)
        {
            GameTime++;
            _tempTimer = 0;
        }
    }
    private void ProcessBase() // Count Base phase Time; when it elapses - reset timer and switch to combat
    {
        _basePhaseTimer -= _delta;
        if (_basePhaseTimer <= 0)
        {
            Debug.Log("Game: Base Phase time elapsed");
            _basePhaseTimer = _basePhaseTime;
            SwitchPhase();
        }
    }
    private void ProcessCombat() // Count enemies and do fight against; if no enemies - check if win; otherswise - switch to base
    {
        if (_combatScript.EndFight())
        {
            Debug.Log("Game: Combat Phase is over");
            CheckWin();
        //    _baseScript.ReturnFighters(_combatScript.ReturnFighters2Base());
            SwitchPhase();
        }
    }

    private void UpdateLayoutInfo()
    {
        _phaseText.text         = _phase            .ToString();
        _phaseTimerText.text    = _basePhaseTimer   .ToString();
        _gameTimeText.text      = GameTime          .ToString();
        _IDLEText.text          = _idleRounds       .ToString();
    }

    public void Init()
    {
        _THEGAMEScript  =   THEGAME.GetComponent<GlobalStates>();
        _combatScript   =   _combatPanel.GetComponent<Combat>();
        _baseScript     =   _basePanel.GetComponent<Base>();

        _doFightButton.GetComponent<Button>().interactable = false;

        GameTime        = 0;
        _tempTimer      = 0.0f;
        _delta          = 0.0f;
        _phase          = Phase.BASE;
        _basePhaseTime  = 20;
        _basePhaseTimer = _basePhaseTime;
        _round          = 0;
        _idleRounds     = 0;
        _isRoundIdle    = true;
        Debug.Log("Game: Initialized Scene Script");
    }

    void Start()
    {
        Init();
        Debug.Log("Game: Start Scene Script");
    }

    void Update()
    {
        if (_THEGAMEScript.GetState() == GlobalStates.State.IN_GAME)
        {
            CountGameTime();
            switch (_phase)
            {
                case Phase.BASE:
                    ProcessBase();
                    break;
                case Phase.COMBAT:
                    ProcessCombat();
                    break;
            }
            UpdateLayoutInfo();
            _baseScript.UpdateLayoutInfo();
        }
    }

    public void Reset() // Use to init and reset script
    {
        _basePanel.GetComponent<Base>().Init();
        _combatPanel.GetComponent<Combat>().Init();

        Init();

        Debug.Log("Game: Reset Scene Script");
    }

    ///////////////////////////////////////////////////////////////////////
    public int      idleRoundsToWin = 10;
    private int     _idleRounds     = 0;
    private bool    _isRoundIdle    = true;

    public void Action() 
    {
        _isRoundIdle = false;
        Debug.Log("Game: Action happened");
    }
    private void CheckWin()
    {
        Debug.Log("Game: Check Win");
        if (_isRoundIdle == true)
            _idleRounds++;
        else
        {
            _isRoundIdle = true;
            _idleRounds = 0;
        }
        Debug.Log("Game: No Action Happen " + _idleRounds.ToString() + "in a row");
        if (_idleRounds >= idleRoundsToWin)
            _THEGAMEScript.Win();
    }

}
