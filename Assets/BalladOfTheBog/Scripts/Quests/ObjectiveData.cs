using UnityEngine;
using System;

[System.Serializable]
public class ObjectiveData
{
    // 0: collect, 1: talktosomebody, 2: enterlocation, 3: completepuzzle, 4: completeencounter
    public byte objectiveType;
    public string objectId;
    public int amountNeeded;
    public int currentAmount;
}
