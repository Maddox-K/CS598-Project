using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/New Dialogue Container")]
public class Dialogue : ScriptableObject
{
    public string speaker_name;

    [TextArea(3,10)]
    public string[] paragraphs;

    public ChoiceDialogue[] choices;
}
