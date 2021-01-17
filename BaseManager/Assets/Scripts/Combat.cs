using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combat : MonoBehaviour
{
    // Game Object and UI elements
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private GameObject _basePanel;
    [SerializeField] private Text       _infoText;
    [SerializeField] private GameObject THEGAME;

    // External scripts
    private GlobalStates    _THEGAMEScript;
    private Game            _gameScript;
    private Base            _baseScript;

    private List<Fighter> _fighters;    // List of Fighters' scripts, used to recount their parameters and then return to base

    private int _enemyBaseDamage;       // How much dmg-points enemy does by default
    private int _DamageIncrease;        // How much increase dmg
    private int _roundsToIncrease;      // Increse dmg every rounds count
    private int _enemiesCount;          // How many enemies there are by default 
    private int _currEnemiesCount;      // How many enemies there are currently
    private int _enemiesCurrDamage;     // how much enemies dmg currently
    private int _currRound;             // current round

    public int GetEnemyBaseDamage()     { return _enemyBaseDamage; }
    public int GetDamageIncrease()      { return _DamageIncrease; }
    public int GetRoundsToIncrease()    { return _roundsToIncrease; }
    public int GetEnemiesCount()        { return _enemiesCount; }
    public int GetCurrEnemiesCount()    { return _currEnemiesCount; }
    public int GetEnemiesCurrDamage()   { return _enemiesCurrDamage; }
    public int GetCurrRound()           { return _currRound; }

    private void GetFighters()
    {
        _fighters.Clear();
        for (int i = 0; i < _baseScript.GetCurFightersCount(); i++)
            _fighters.Add(_baseScript.SendFighter(i));
    }

    public void StartCombat()
    {
        GetFighters();
        _currEnemiesCount = 0;
        _currRound = _gameScript.GetRound();
       
        if ((_currRound % _roundsToIncrease) == 0)
            _enemiesCurrDamage++;
        else
            _enemiesCount = _currRound + 1;
        _currEnemiesCount = _enemiesCount;

        Debug.Log("Combat: Combat Started");
    }

    public bool EndFight()
    {
        if (_currEnemiesCount <= 0)
            return true;
        else
            return false;
    }

    public void DoFight() // set on click to DoFight_button
    {
        int fightersAlive = _fighters.Count;
        for (int i = 0; i < _fighters.Count; i++)
        {
            if (_currEnemiesCount > 0)
                {
                if (_fighters[i].FightEnemy(_enemiesCurrDamage))
                    _currEnemiesCount--;
                }
        }
        for (int i = 0; i<_fighters.Count; i++)
            if (_fighters[i].GetCurrHP() <= 0)
                fightersAlive--;
        _baseScript.ReturnFighters(ReturnFighters2Base());
        _baseScript.ProcessFighters();
        if (fightersAlive <= 0 && _currEnemiesCount >= 1)
            THEGAME.GetComponent<GlobalStates>().Lose();
        Debug.Log("Combat: Fight happen, " +
            " | fighters alive " + fightersAlive.ToString() + "/" + _fighters.Count.ToString()+
            " | enemies left" + _currEnemiesCount.ToString() + "/" + _enemiesCount.ToString());
    }

    public List<Fighter> ReturnFighters2Base() { return _fighters; }

    public void Init()
    {
        _THEGAMEScript  = THEGAME.GetComponent<GlobalStates>();
        _gameScript     = _gamePanel.GetComponent<Game>();
        _baseScript     = _basePanel.GetComponent<Base>();

        _fighters = new List<Fighter>();

        _enemyBaseDamage    = 1;
        _DamageIncrease     = 1;
        _roundsToIncrease   = 5;
        _enemiesCount       = 1;
        _currEnemiesCount   = _enemiesCount;
        _enemiesCurrDamage  = _enemyBaseDamage;
        _currRound          = 0;

        Debug.Log("Combat: Initialized Scene Script");
    }

    void Start()
    {
        Init();
        Debug.Log("Combat: Start Scene Script");
    }
    void Update()
    {
        if(_gameScript.GetPhase()==Game.Phase.COMBAT)
            _infoText.text = "Enemies: " + _currEnemiesCount.ToString() + "| Damge: " + _enemiesCurrDamage.ToString();
    }
}
