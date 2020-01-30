using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Poco;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class WonPrize
{
    public int Y;
    public int Size;

    public WonPrize(int y, int size)
    {
        Y = y;
        Size = size;
    }
}

public class ReelsManager : MonoBehaviour
{
    [SerializeField] private ReelsConfiguration _reelsConfiguration;
    [SerializeField] private Reel[] _reels;
    [SerializeField] private Button _spinButton;
    [SerializeField] private Toggle _autoToggle;
    [SerializeField] private float _timeBetweenReels = 0.2f;
    [SerializeField] private Balance _balance;
    
    private ReelsSymbols _reelsSymbols;
    private int _currentReel = 0;
    private int[] _result;
    
    private void Start()
    {
        _reelsSymbols = GetJsonData<ReelsSymbols>.GetJsonInfo(_reelsConfiguration.ReelsSymbols);

        GenerateReels();
        
        _spinButton.onClick.AddListener(SpinClick);
    }

    private void OnDestroy()
    {
        _spinButton.onClick.RemoveListener(SpinClick);
    }

    private void GenerateReels()
    {
        for (int i = 0; i < _reels.Length; i++)
        {
            _reels[i].Create(_reelsSymbols.Reels[i], _reelsConfiguration.SymbolsData);
        }
    }
    
    private void SpinClick()
    {
        _spinButton.interactable = false;
        
        for (int i = 0; i < _reels.Length; i++)
        {
            _reels[i].RemoveMarkers();
        }
        
        _result = new int[_reelsSymbols.Reels.Length];

        for (int i = 0; i < _result.Length; i++)
        {
            _result[i] = Random.Range(0, _reelsSymbols.Reels[i].Length);
        }
        
        _currentReel = 0;
        StartCoroutine(ScrollReels());
        _balance.PlayGame();
    }

    private IEnumerator ScrollReels()
    {
        for (int i = 0; i < _reels.Length; i++)
        {
            _reels[i].InfiniteScroll();
            yield return new WaitForSecondsRealtime(_timeBetweenReels);
        }
        
        yield return new WaitForSecondsRealtime(Random.Range(2f - _timeBetweenReels * _reels.Length, 4f - _timeBetweenReels * _reels.Length));

        _reels[_currentReel].MoveTo(_result[_currentReel]);
        _reels[_currentReel].OnScrollFinished += OnReelStoppedMoving;
    }

    private void OnReelStoppedMoving(object sender, EventArgs e)
    {
        _reels[_currentReel].OnScrollFinished -= OnReelStoppedMoving;

        if (LastReelFinishedMoving())
        {
            CheckForPrizes();
            
            if (IsAutoPlayEnabled())
            {
                StartCoroutine(SpinDelay());
                return;
            }
            
            _spinButton.interactable = true;
            return;
        }

        _currentReel++;
        _reels[_currentReel].MoveTo(_result[_currentReel]);
        _reels[_currentReel].OnScrollFinished += OnReelStoppedMoving;
    }

    private bool IsAutoPlayEnabled()
    {
        return _autoToggle.isOn;
    }

    private IEnumerator SpinDelay()
    {
        yield return new WaitForSeconds(1f);
        SpinClick();
    }

    private bool LastReelFinishedMoving()
    {
        return _currentReel >= _reels.Length - 1;
    }

    private void CheckForPrizes()
    {
        int[][] finalWindow = new int[3][];
        BuildFinalWindow(ref finalWindow);

        Prize[][] prizes = _reelsSymbols.Prizes;

        List<WonPrize>[] foundPrizes = new List<WonPrize>[5];
        int prize = 0;
        
        for (int i = 0; i < finalWindow.Length; i++)
        {
            for (int k = 0; k < prizes.Length; k++)
            {
                for (int j = 0; j < finalWindow[i].Length; j++)
                {
                    if (PrizeFound(foundPrizes, j, prizes, k, finalWindow, i, ref prize))
                    {
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < _reels.Length; i++)
        {
            _reels[i].ShowPrizeMarkers(foundPrizes[i]);
        }
        
        _balance.AddBalance(prize);
    }

    private static bool PrizeFound(List<WonPrize>[] foundPrizes, int j, Prize[][] prizes, int k, int[][] finalWindow, int i,
        ref int prize)
    {
        if (foundPrizes[j] == null)
        {
            foundPrizes[j] = new List<WonPrize>();
        }

        bool _validPrize = false;

        for (int m = 0; m < prizes[k].Length; m++)
        {
            for (int l = 0; l < prizes[k][m].Combination.Length; l++)
            {
                if (j + l < finalWindow[i].Length && prizes[k][m].Combination[l] == finalWindow[i][j + l])
                {
                    _validPrize = true;
                }
                else
                {
                    _validPrize = false;
                    break;
                }
            }

            if (_validPrize)
            {
                foundPrizes[j].Add(new WonPrize(i, prizes[k][m].Combination.Length));
                prize += prizes[k][m].PrizeValue;
                return true;
            }
        }

        return false;
    }

    private void BuildFinalWindow(ref int[][] finalWindow)
    {
        string log = "Final Window: \n";
        for (int i = 0; i < 3; i++)
        {
            finalWindow[i] = new int[5];

            for (int j = 0; j < 5; j++)
            {
                int winIndex = _result[j] + i;
                if (winIndex >= _reelsSymbols.Reels[j].Length)
                {
                    winIndex = winIndex - _reelsSymbols.Reels[j].Length;
                }

                finalWindow[i][j] = _reelsSymbols.Reels[j][winIndex];
                log += " " + finalWindow[i][j] + ",";
            }

            log += " \n";
        }

        Debug.Log(log);
    }
}
