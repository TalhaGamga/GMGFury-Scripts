using UnityEngine;

[CreateAssetMenu(fileName = "StrikingAttackData", menuName = "ScriptableObjects/Striking")]
class StrikingAttackSO : ScriptableObject
{
    [SerializeField] string dataName;

    public void CallData()
    {
        Debug.Log(dataName);
    }
}