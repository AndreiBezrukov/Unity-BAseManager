using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fighter : MonoBehaviour
{
    // Game Object and UI elements
    private Transform  THEGAME;
    private GameObject _fightersPanel; // Fighters panel includes all fighters appear in game
    private GameObject _basePanel;     // Base panel includes workers and fighters panels

    [SerializeField] private Text   _stateText;
    [SerializeField] private Text   _healthText;
    [SerializeField] private Text   _updateText;
    [SerializeField] private Text   _timerText;

    [SerializeField] private Button _updateButton;

    // External scripts
    private Base _baseScript;

    // Fighter's parameters
    private State   _state;     // Current fighter's state
    private float   _timer;     // used for heal;
    private int     _currHP;    // Current HP of fighter
    private int     _maxHP;     // Maximum HP of fighter
    private int     _update;    // Level of Update
    private int     _healHP;    // Heal HP value

    // Getters:
    public float    GetTimer()  { return _timer; }
    public int      GetCurrHP() { return _currHP; }
    public int      GetMaxHP()  { return _maxHP; }
    public int      GetUpdate() { return _update; }
    public int      GetHealHP() { return _healHP; }

    // State managment    
    public enum State
    {
        ORDERED = 0,
        READY   = 1,
        UPDATE  = 2
    }
    public State GetState()                 { return _state; }
    private void ChangeStateTo(State state) { _state = state; }
    public void SetState2Ready()            { ChangeStateTo(State.READY); }
    public void SetState2Update()           { ChangeStateTo(State.UPDATE); }

    // Processing fighter states
    public void StartUpdateFighter() // For Fighter_Prefab Set on Click on Update_button
    {
        _baseScript.UpdateFighter();
        SetState2Update();
        Debug.Log("Fighter: Start to update");
    }
    public void FinishUpdateFighter()
    {
        _update++;
        _maxHP  += _baseScript.GetFighterUpdateHP();
        _healHP = _update;
        SetState2Ready();
        Debug.Log("Fighter: Updated to " + _update.ToString());
    }
    public void HealHP()
    {
        if (_currHP < _maxHP)
            if (_timer <= 0)
            {
                _currHP += _healHP;
                if (_currHP > _maxHP)
                    _currHP = _maxHP;
                _timer = _baseScript.GetFighterHealTime();
            }
            else
                _timer -= _baseScript.GetDelta();
    }

    public void InteractableUpdateButton(bool Bool) 
    {
        _updateButton.interactable = Bool;
    }

    public bool FightEnemy(int dmg) 
    {
        if (_currHP > 0)
        {
            if (_state != State.READY)
            {
                _currHP = 0;
                Debug.Log("Fighter: fighter tryed to deal with enemy but was not ready to fight with enemy: " + _state.ToString() + " and died");
                return false;
            }
            else
                _currHP -= dmg;
            if (_currHP > 0)
                Debug.Log("Fighter: fighter get with enemy: " + _currHP.ToString()+"/"+_maxHP.ToString());
            else
                Debug.Log("Fighter: fighter get with enemy: " + _currHP.ToString()+"/"+_maxHP.ToString()+" and died");
            return true;
        }
        else
        {
            Debug.Log("Fighter: fighter already dead");
            return false;
        }
    }
    public string GetHP() 
    {
        string str = "(" + _currHP.ToString()+"/"+_maxHP.ToString()+")"; 
        return str;
    }

    public void UpdateLayoutInfo()
    {
        _stateText.text = _state.ToString();
        _healthText.text = _currHP.ToString() + " / " + _maxHP.ToString();
        if (_currHP < _maxHP)
            _timerText.gameObject.SetActive(true);
        else
            _timerText.gameObject.SetActive(false);
        _timerText.text = _timer.ToString();
        _updateText.text = _update.ToString();
    }

    public void Init()
    {
        THEGAME         = GameObject.Find("Game_canvas").GetComponent<Transform>();
        _basePanel      = GameObject.Find("Base_panel");
        _fightersPanel  = GameObject.Find("Fighters_panel");

        _baseScript = _basePanel.GetComponent<Base>();

        _state  = _baseScript.GetFighterStartState();
        _timer  = _baseScript.GetFighterHealTime();
        _maxHP  = _baseScript.GetFighterMaxHP();
        _currHP = _maxHP;
        _update = 1;
        _healHP = _update;

        Debug.Log("Fighter: Initialized Scene Script");
    }

    void Start()
    {
        Init();
        Debug.Log("Fighter: Start Scene Script");
    }

    void Update()
    {
 
    }

    public void GetFighter(Fighter fighter) // Use to copy here parameters from another copy of Fighter-script
    {
        // Game Object and UI elements
        _basePanel      = fighter._basePanel;
        _fightersPanel  = fighter._fightersPanel;

        _stateText      = fighter._stateText;
        _healthText     = fighter._healthText;
        _updateText     = fighter._updateText;
        _timerText      = fighter._timerText;

        // External scripts = fighter.;
        _baseScript         = fighter._baseScript;
                            
        // Fighter's paramet= fighter.;ers
        _state              = fighter._state;
        _timer              = fighter._timer;
        _currHP             = fighter._currHP;
        _maxHP              = fighter._maxHP;
        _update             = fighter._update;
        _healHP             = fighter._healHP;
    }
}
