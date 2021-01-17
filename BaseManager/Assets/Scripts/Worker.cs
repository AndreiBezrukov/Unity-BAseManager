using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Worker : MonoBehaviour
{
    // Game Object and UI elements
    [SerializeField] private Text _StateText;
    [SerializeField] private Text _TimerText;

    private GameObject _basePanel;     // Base panel includes workers and fighters panels
    private GameObject _workersPanel;  // Workers panel includes all workers appear in game

    // External scripts
    private Base _baseScript;

    // Worker's parameters
    private State _state;
    private float _timer;

    // State managment
    public enum State 
    {
        ORDERED = 0,
        READY = 1
    }

    public State GetState()                 { return _state; }
    private void ChangeState(State state)   { _state = state; }
    public void SetState2Ready()            { ChangeState(State.READY); }

    public void MineResources()
    {
        if (_timer <= 0)
        {
            _basePanel.GetComponent<Base>().resources += _baseScript.GetMineValue();
            _timer = _baseScript.GetMineTime();
        }
        else
            _timer -= _baseScript.GetDelta();
    }


    public void UpdateLayoutInfo()
    {
        _StateText.text = _state.ToString();
        _TimerText.text = _timer.ToString() + " / " + _baseScript.GetMineTime().ToString();
    }

    public void Init()
    {
        _basePanel      = GameObject.Find("Game_canvas/Game_panel/Base_panel");
        _workersPanel   = GameObject.Find("Game_canvas/Game_panel/Base_panel/Workers_panel");

        _baseScript = _basePanel.GetComponent<Base>();

        _state      = _baseScript.GetWorkerStartState();
        _timer      = _baseScript.GetMineTime();

        Debug.Log("Worker: Initialized Scene Script");
    }

    void Start()
    {
        Init();
        Debug.Log("Worker: Start Scene Script");
    }

    void Update()
    {
    }
}
