using UnityEngine;

[CreateAssetMenu(fileName = "AssetSO", menuName = "Scriptable Objects/Assets")]
public class AssetSO : ScriptableObject
{
    public readonly new string name;
    public readonly Sprite icon;
}
