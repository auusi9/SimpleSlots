using System.Collections;
using TMPro;
using UnityEngine;

public class Balance : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _balanceText;
    [SerializeField] private TextMeshProUGUI _wonText;
    [SerializeField] private float _valueUpdateTime;
    [SerializeField] private int _balance = 200;
    [SerializeField] private int _bet = 1;

    private int _currentBalance = 0;
    private int _currentWon = 0;
    private int _won;
    
    private void Start()
    {
        _currentBalance = _balance;
        SetFinalValues();
    }
    
    public void AddBalance(int newBalance)
    {
        _balance += newBalance;
        _won = newBalance;
        StartCoroutine(UpdateBalance());
    }

    public void PlayGame()
    {
        StopAllCoroutines();
        _balance -= _bet * 3;
        _currentWon = 0;
        _won = 0;

        if (_balance < 0)
        {
            _balance += 200;
            Debug.Log("Adding balance");
        }
        
        SetFinalValues();
    }

    private IEnumerator UpdateBalance()
    {
        float time = 0f;

        while (time <= _valueUpdateTime)
        {
            _currentBalance = (int)Mathf.Lerp(_currentBalance, _balance, time/ _valueUpdateTime);

            _balanceText.SetText($"Balance: {_currentBalance}");
        
            _currentWon = (int)Mathf.Lerp(_currentWon, _won,  (time * 2)/ _valueUpdateTime);

            _wonText.SetText($"Won: {_currentWon}");
            yield return 0;
            time += Time.deltaTime;
        }
        
        SetFinalValues();
    }

    private void SetFinalValues()
    {
        _currentBalance = _balance;

        _balanceText.SetText($"Balance: {_currentBalance}");

        _currentWon = _won;

        _wonText.SetText($"Won: {_currentWon}");
    }
}
