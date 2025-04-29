using UnityEngine;
using System;

[System.Serializable]
public class ObjectiveData
{
    // 0: collect, 1: talktosomebody, 2: enterlocation, 3: completepuzzle, 4: completeencounter, 5: use inventory item
    public byte objectiveType;
    public string objectId;
    public int amountNeeded;
    public int currentAmount;
}
