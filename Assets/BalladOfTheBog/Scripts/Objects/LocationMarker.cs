using System.Collections.Generic;
using UnityEngine;

public class LocationMarker : MonoBehaviour
{
    [SerializeField] private bool _usedForQuest;
    [SerializeField] private bool _usedForCutscene;
    [SerializeField] private string _locationName;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (_usedForQuest)
            {
                QuestEvents.OnLocationVisited?.Invoke(_locationName);
            }
            if (_usedForCutscene)
            {
                Dictionary<string, bool> cutscenes = GameManager.instance.gameData.cutScenes;
                if (!cutscenes.ContainsKey(_locationName) || (cutscenes.ContainsKey(_locationName) && !cutscenes[_locationName]))
                {
                    GameManager.instance.gameData.cutScenes.Add(_locationName, true);
                    CutSceneEvents.InvokeLocationEntered(_locationName);
                }
            }   
        }
    }
}
