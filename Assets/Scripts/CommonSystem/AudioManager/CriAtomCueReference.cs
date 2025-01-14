using CriWare.Assets;
using UnityEngine;


[System.Serializable]
public struct CriAtomCueReferenceStr
{
    [SerializeField] private CriAtomAcbAsset _acbAsset;
    [SerializeField] private string _cueId;

    public CriAtomAcbAsset AcbAsset => _acbAsset;
    public string CueId => _cueId;

    public CriAtomCueReferenceStr(CriAtomAcbAsset acbAsset, string cueId)
    {
        _acbAsset = acbAsset;
        _cueId = cueId;
    }
}