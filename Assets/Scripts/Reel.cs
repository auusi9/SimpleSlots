using System;
using System.Collections;
using System.Collections.Generic;
using Configurations;
using UnityEngine;

public class Reel : MonoBehaviour
{
    [SerializeField] private GameObject symbolDataPrefab;
    [SerializeField] private float _reelSpeed = 1f;

    private Symbol[] _symbols;
    private int _currentPosition = 0;
    private int[] _reel;
    private float _symbolHeight;

    public event EventHandler OnScrollFinished;
    
    public void Create(int[] reelsSymbolsReel, SymbolData[] symbolsData)
    {
        _symbolHeight = symbolsData[0].Sprite.rect.height;
        _symbols = new Symbol[reelsSymbolsReel.Length];
        _reel = reelsSymbolsReel;
        
        for (int i = 0; i < _reel.Length; i++)
        {
           CreateSymbol(i, symbolsData[_reel[i] - 1]);
        }

        SetPositions();
    }
    
    public void ShowPrizeMarkers(List<WonPrize> prizes)
    {
        for (int i = 0; i < prizes.Count; i++)
        {
            int prizeIndex = _currentPosition + prizes[i].Y;
            if (prizeIndex >= _symbols.Length)
            {
                prizeIndex -= _symbols.Length;
            }
            
            _symbols[prizeIndex].ActivateMarker(prizes[i].Size);
        }
    }

    public void RemoveMarkers()
    {
        for (int i = 0; i < _symbols.Length; i++)
        {
            _symbols[i].DesactivateMarker();
        }
    }
    
    public void InfiniteScroll()
    {
        StartCoroutine(ScrollReel());
    }
    
    public void MoveTo(int position)
    {
        StopAllCoroutines();
        StartCoroutine(ScrollTo(position));
    }

    private void SetPositions()
    {
        _currentPosition = 0;
        for (int i = 0; i < _symbols.Length; i++)
        {
            _symbols[i].SetPosition(Vector2.up * GetPosition(i) * _symbolHeight);
        }
    }

    private void CreateSymbol(int index, SymbolData symbolData)
    {
        GameObject symbolGo = Instantiate(symbolDataPrefab, transform);
        symbolGo.name = "Symbol" + index;

        Symbol newSymbol = symbolGo.GetComponent<Symbol>();

        if (newSymbol == null)
        {
            Debug.LogError("Missing component Symbol in " + symbolDataPrefab.name + " prefab");
            return;
        }
        
        newSymbol.OnCreate(symbolData.Sprite);
        _symbols[index] = newSymbol;
    }

    IEnumerator ScrollReel()
    {
        float time = 0;

        while (true)
        {
            MoveSymbols(_reelSpeed);

            yield return 0;
            time += Time.deltaTime;
        }
    } 
    
    IEnumerator ScrollTo(int position)
    {
        while (_currentPosition != position)
        {
            float distance = _symbols[position].RectTransform.anchoredPosition.y;
            float alternative = _reelSpeed * ((distance / (3f * _symbolHeight)) + 0.25f); 
            MoveSymbols(distance > 3f * _symbolHeight || distance < 0f ? _reelSpeed : alternative);
            yield return 0;
        }

        float time = 0f;

        while (time <= 0.2f)
        {       
            for (int i = 0; i < _symbols.Length; i++)
            {
                Vector2 finalPos = Vector2.up * GetPosition(i) * _symbolHeight;
                _symbols[i].SetPosition(Vector2.Lerp(_symbols[i].RectTransform.anchoredPosition, finalPos, time/0.2f));
            }
               
            yield return 0;
            time += Time.deltaTime;
        }

        ScrollFinished();
    }

    private void MoveSymbols(float speed)
    {
        for (int i = 0; i < _symbols.Length; i++)
        {
            _symbols[i].AddPosition(GetCurrentSpeed(speed));

            if (IsSymbolBelowReelWindow(i))
            {
                Vector2 newPosition = Vector2.up * (_symbols.Length - 3) * _symbolHeight;
                newPosition.y -= (-3 * _symbolHeight) - _symbols[i].RectTransform.anchoredPosition.y;

                _symbols[i].SetPosition(newPosition);
                _currentPosition--;

                if (_currentPosition < 0)
                {
                    _currentPosition = _symbols.Length - 1;
                }
            }
        }
    }

    private bool IsSymbolBelowReelWindow(int i)
    {
        return _symbols[i].RectTransform.anchoredPosition.y <= (-3) * _symbolHeight;
    }

    private Vector2 GetCurrentSpeed(float speed)
    {
        return -Vector2.up * speed * Time.deltaTime;
    }

    private int GetPosition(int index)
    {
        int initialIndex = index;
        if (index < _currentPosition)
        {
            index += _symbols.Length;    
        }
        
        if (index - _currentPosition > 2)
        {
            index = initialIndex;

            if (index > _currentPosition)
            {
                index -= _symbols.Length;
            }
        }

        return _currentPosition - index;
    }
    
    private void ScrollFinished()
    {
        EventHandler handler = OnScrollFinished;

        if (handler != null)
        {
            handler(this, null);
        }
    }
}
