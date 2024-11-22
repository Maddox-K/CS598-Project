using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_DataPersistence
{
    void LoadData(GameData data);

    void SaveData(GameData data);
}