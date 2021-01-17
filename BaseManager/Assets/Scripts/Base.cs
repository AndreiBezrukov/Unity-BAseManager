using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviour
{
    // Game Object and UI elements
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private Transform  _workersPanel;  // Workers panel includes all workers appear in game
    [SerializeField] private Transform  _fightersPanel; // Fighters panel includes all fighters appear in game

    [SerializeField] private Button     _orderWorkerButton; 
    [SerializeField] private Button     _orderFighterButton;

    [SerializeField] private Text       _stateText;
    [SerializeField] private Text       _timerText;
    [SerializeField] private Text       _workersText;
    [SerializeField] private Text       _fightersText;
    [SerializeField] private Text       _resourcesText;
    [SerializeField] private GameObject THEGAME;

    // External scripts
    private GlobalStates    _THEGAMEScript;
    private Game            _gameScript;
    
    // Prefabs
    [SerializeField] private GameObject _fighterPrefab;
    [SerializeField] private GameObject _workerPrefab;

    // Lists of workers and fighters
    private List<GameObject> _fightersList;
    private List<GameObject> _workersList;

    // General Base parameters
    private State   _state;
    public int      resources;  // Players resources
    private float   _timer;     // timer of orders and updates
    private float   _delta;

    // Getters
    public float GetTimer() { return _timer; }
    public float GetDelta() { return _delta; }
    public State GetState() { return _state; }

    // Fighters parameters
    private Fighter.State _fighterStartState;

    private int     _maxFighters;        // Maximum fighters allowed
    private int     _curFightersCount;   // Count of fighters player has currently now (may be used to set start fighters count)
    private int     _fighterOrderCost;   // How much resources is required to order a fighter
    private float   _fighterOrderTime;   // How much time takes to order a fighter
    private int     _fighterUpdateCost;  // How much resources takes to update a fighter
    private float   _fighterUpdateTime;  // How much time takes to update a fighter
    private int     _fighterMaxHP;       // Maximum HP fighter can have after order before update
    private int     _fighterUpdateHP ;   // How much max hp will be increased
    private float   _fighterHealTime;    // How much time takes to heal a healHP of a fighter

    // Getters
    public Fighter.State    GetFighterStartState()      { return _fighterStartState; }
    public int              GetMaxFighters()            { return _maxFighters;      }
    public int              GetCurFightersCount()       { return _curFightersCount; }
    public int              GetFighterOrderCost()       { return _fighterOrderCost; }
    public float            GetFighterOrderTime()       { return _fighterOrderTime; }
    public int              GetFighterUpdateCost()      { return _fighterUpdateCost;}
    public float            GetFighterUpdateTime()      { return _fighterUpdateTime;}
    public int              GetFighterMaxHP()           { return _fighterMaxHP;     }
    public int              GetFighterUpdateHP()        { return _fighterUpdateHP ; }
    public float            GetFighterHealTime()        { return _fighterHealTime; }

    // Workers parameters
    private Worker.State _workerStartState;

    private int      _maxWorkers;           // Maximum workers allowed
    private int      _curWorkersCount;      // Count of workers player has currently now (may be used to set start workers count)
    private int      _workerOrderCost;      // How much resources is required to order a worker
    private float    _workerOrderTime;      // How much time takes to order a worker
    private int      _mineValue;            // How many resources worker mines
    private float    _mineTime;             // How much time takes for a worker to mine
    private int      _workersThreshHold;    // How many workers cause reduce of count resources being mined
    private int      _mineDecreaseValue;    // How much value of resources being mined is decreased 

    // Getters
    public Worker.State GetWorkerStartState()   {return _workerStartState; }
    public int      GetMaxWorkers       ()      {return _maxWorkers;       }
    public int      GetCurWorkersCount  ()      {return _curWorkersCount;  }
    public int      GetWorkerOrderCost  ()      {return _workerOrderCost;  }
    public float    GetWorkerOrderTime  ()      {return _workerOrderTime;  }
    public int      GetMineValue        ()      {return _mineValue;        }
    public float    GetMineTime         ()      {return _mineTime;         }
    public int      GetWorkersThreshHold()      {return _workersThreshHold;}
    public int      GetMineDecreaseValue()      {return _mineDecreaseValue; }

    public enum State
    {
        IDLE            = 0,
        ORDER_WORKER    = 1,
        ORDER_FIGHTER   = 2,
        UPDATE_FIGHTER  = 3
    }

    private void ChangeState(State state)   { _state = state; }
    public void SetState2Idle()             { ChangeState(State.IDLE); }
    public void SetState2OrderWorker()      { ChangeState(State.ORDER_WORKER); }
    public void SetState2OrderFighter()     { ChangeState(State.ORDER_FIGHTER); }
    public void SetState2UpdateFighter()    { ChangeState(State.UPDATE_FIGHTER); }

    // Orders
    public void OrderWorker()   // Set on Click to Workers_panel/OrderWorker_button
    {
        resources -= _workerOrderCost;
        _curWorkersCount++;
        _workersList.Add(Instantiate(_workerPrefab, _workersPanel));

        _timer = _workerOrderTime;

        SetState2OrderWorker();
        Debug.Log("Base: Ordered Worker " + _curWorkersCount.ToString());
        _gameScript.Action();
    }
    public void OrderFighter()  // Set on Click to Fighters_panel/OrderFighter_button
    {
        resources -= _fighterOrderCost;
        _curFightersCount++;
        _fightersList.Add(Instantiate(_fighterPrefab, _fightersPanel));

        _timer = _fighterOrderTime;

        SetState2OrderFighter();
        Debug.Log("Base: Ordered Fighter " + _curFightersCount.ToString());
        _gameScript.Action();
    }
    public void UpdateFighter() // For this - On Click set in Fighter prefab with StartUpdateFighter methods in script
    {
        resources -= _fighterUpdateCost;
        _timer = _fighterUpdateTime;
        SetState2UpdateFighter();
        Debug.Log("Base: Started update Fighter");
        _gameScript.Action();
    }

    private void ProcessOrder()
    {
        if (_timer <= 0)
        {
            switch (_state)
            {
                case State.ORDER_WORKER:
                    _workersList[_curWorkersCount-1].GetComponent<Worker>().SetState2Ready();
                    Debug.Log("Base: Worker " + _curWorkersCount.ToString() + "order finished");
                    if (_curWorkersCount > _workersThreshHold)
                    {
                        _mineValue -= _mineDecreaseValue;
                        Debug.Log("Base: Mine Value decreased");
                    }
                    break;
                case State.ORDER_FIGHTER:
                    _fightersList[_curFightersCount-1].GetComponent<Fighter>().SetState2Ready();
                    Debug.Log("Base: Fighter " + _curFightersCount.ToString() + "order finished");
                    break;
                case State.UPDATE_FIGHTER:
                    // find which fighter was updated:
                    for (int i = 0; i < _fightersList.Count; i++)
                        if (_fightersList[i].GetComponent<Fighter>().GetState() == Fighter.State.UPDATE)
                        {
                            _fightersList[i].GetComponent<Fighter>().FinishUpdateFighter();
                            Debug.Log("Base: finished update fighter " + i.ToString());
                            break;
                        }
                    break;
            }
            SetState2Idle();
            _timer=0;
        }
        else
            _timer -= _delta;
    }

    public void UpdateAvailable()
    {

        if (_THEGAMEScript.GetState() == GlobalStates.State.IN_GAME &&
            _gameScript.GetPhase() == Game.Phase.BASE &&
            _state == State.IDLE)
        {
            if (_curFightersCount > 0)
                if (resources >= _fighterUpdateCost)
                    for (int i = 0; i < _curFightersCount; i++)
                        _fightersList[i].GetComponent<Fighter>().InteractableUpdateButton(true);
                else
                    for (int i = 0; i < _curFightersCount; i++)
                        _fightersList[i].GetComponent<Fighter>().InteractableUpdateButton(false);
        }
        else
            for (int i = 0; i < _curFightersCount; i++)
                _fightersList[i].GetComponent<Fighter>().InteractableUpdateButton(false);
    }

    public void OrderAvailable()
    {
        if (_THEGAMEScript.GetState() == GlobalStates.State.IN_GAME &&
            _gameScript.GetPhase() == Game.Phase.BASE &&
            _state == State.IDLE)
        {
            if (_curWorkersCount < _maxWorkers &&
                resources >= _workerOrderCost)
                _orderWorkerButton.interactable = true;
            else
                _orderWorkerButton.interactable = false;
            if (_curFightersCount < _maxFighters &&
               resources >= _fighterOrderCost)
                _orderFighterButton.interactable = true;
            else
                _orderFighterButton.interactable = false;
        }
        else
        {
            _orderWorkerButton.interactable = false;
            _orderFighterButton.interactable = false;
        }
        // Same for update, because of need to check every fighter in the list
        UpdateAvailable();
    }

    public void UpdateLayoutInfo()
    {
        // Layout of base phase info
        _stateText.text = _state.ToString();
        if (_timer > 0)
            _timerText.gameObject.SetActive(true);
        else
            _timerText.gameObject.SetActive(false);
        _timerText.text         = _timer.ToString();
        _workersText.text       = _curWorkersCount  .ToString() + " / " + _maxWorkers.ToString() + "("+_workerOrderCost+")";
        _fightersText.text      = _curFightersCount .ToString() + " / " + _maxFighters.ToString()+ "("+_fighterOrderCost+")";
        _resourcesText.text     = resources         .ToString();

        // set buttons available
        OrderAvailable();
    }

    public Fighter SendFighter(int idx) 
    {
        if (_curFightersCount > 0 && (idx >=0 && idx < _curFightersCount))
            return _fightersList[idx].GetComponent<Fighter>();
        return new Fighter();
    }

    public void ReturnFighters(List<Fighter> fighters)
    {
        List<int> indexesToRemove= new List<int>();
        for (int i = 0; i < fighters.Count; i++)
        {
            if (fighters[i].GetCurrHP() <= 0)
            {
                indexesToRemove.Add(i);
            }
        }
        if (indexesToRemove.Count > 0)
            foreach (var i in indexesToRemove)
            {
                GameObject.Destroy(_fightersList[i]);
                _fightersList[i] = null;
                _fightersList.RemoveAt(i);
            }
        _curFightersCount=_fightersList.Count;

        //for (int i = 0; i < fighters.Count; i++)
        //{
        //    if (fighters[i].GetCurrHP() > 0)
        //    {
        //        _fightersList.Add(Instantiate(_fighterPrefab, _fightersPanel));
        //        _fightersList[i].GetComponent<Fighter>().GetFighter(fighters[i]);
        //    }
        //}
        

    }

    // Processes Over workers and fighters
    private void ProcessWorkers()
    {
        if (_curWorkersCount > 0)
            for (int i = 0; i < _curWorkersCount; i++)
            {
                // Position
                float y = _workerPrefab.GetComponent<RectTransform>().rect.height * i;
                _workersList[i].GetComponent<RectTransform>().localPosition = new Vector2(0, y);
                // Mineing
                if (_workersList[i].GetComponent<Worker>().GetState() == Worker.State.READY)
                    _workersList[i].GetComponent<Worker>().MineResources();
                // Layout
                _workersList[i].GetComponent<Worker>().UpdateLayoutInfo();
            }
    }
    public void ProcessFighters() 
    {
        if (_curFightersCount > 0)
            for (int i = 0; i < _curFightersCount; i++)
            {
                // Position
                float y = _fighterPrefab.GetComponent<RectTransform>().rect.height * i;
                _fightersList[i].GetComponent<RectTransform>().localPosition = new Vector2(0, y);

                // Healing
                if (_fightersList[i].GetComponent<Fighter>().GetState() == Fighter.State.READY)
                    _fightersList[i].GetComponent<Fighter>().HealHP();
                // Layout
                _fightersList[i].GetComponent<Fighter>().UpdateLayoutInfo();
            }
    }

    public void Init() // Use to init or reset script parameters
    {
        if (_fightersList.Count > 0)
            for (int i = _fightersList.Count-1; i >=0; i--)
            {
                GameObject.Destroy(_fightersList[i]);
                _fightersList[i] = null;
                _fightersList.RemoveAt(i);
            }
        if (_workersList.Count > 0)
            for (int i = _curWorkersCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(_workersList[i]);
                _workersList[i] = null;
                _workersList.RemoveAt(i);
            }

        _THEGAMEScript  = THEGAME.GetComponent<GlobalStates>();
        _gameScript     = _gamePanel.GetComponent<Game>();

        _state      = State.IDLE;
        resources   = 1000;
        _timer      = 0.0f;

        _fighterStartState  = Fighter.State.ORDERED;
        _maxFighters        = 5;
        _curFightersCount   = 0;
        _fighterOrderCost   = 120;
        _fighterOrderTime   = 10.0f;
        _fighterUpdateCost  = 70;
        _fighterUpdateTime  = 7.0f;
        _fighterMaxHP       = 20;
        _fighterUpdateHP    = 10;
        _fighterHealTime    = 3.0f;

        _workerStartState   = Worker.State.ORDERED;
        _maxWorkers         = 5;
        _curWorkersCount    = 0;
        _workerOrderCost    = 100;
        _workerOrderTime    = 5.0f;
        _mineValue          = 10;
        _mineTime           = 5.0f;
        _workersThreshHold  = 3;
        _mineDecreaseValue  = 1;

        Debug.Log("Base: Initialized Scene Script");
    }

    void Start()
    {
        _fightersList = new List<GameObject>();
        _workersList  = new List<GameObject>();
        Init();
        Debug.Log("Base: Start Scene Script");
    }

    void Update()
    {
        if (_THEGAMEScript.GetState() == GlobalStates.State.IN_GAME &&
            _gameScript.GetPhase() == Game.Phase.BASE)
        {
            _delta = _gameScript.GetDelta();
            if (_state != State.IDLE)
                ProcessOrder();
            ProcessWorkers();
            ProcessFighters();
            UpdateLayoutInfo();
        }
    }
}
