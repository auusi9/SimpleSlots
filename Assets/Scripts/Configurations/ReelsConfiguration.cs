using Configurations;
using UnityEngine;

[CreateAssetMenu(fileName = "ReelsConfiguration", menuName = "ScriptableObjects/ReelsConfiguration", order = 1)]
public class ReelsConfiguration : ScriptableObject
{

    [SerializeField] private string _reelsSymbols;

    [SerializeField] private SymbolData[] symbolsData;

    public string ReelsSymbols => _reelsSymbols;

    public SymbolData[] SymbolsData => symbolsData;
}
