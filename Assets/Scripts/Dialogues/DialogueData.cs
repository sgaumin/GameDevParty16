using DG.Tweening;
using Tools.Utils;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "DialogueData", order = 1)]
public class DialogueData : ScriptableObject
{
	public string name;
	[TextArea(3, 5)]
    public string[] dialogue;
	[TextArea(3, 5)]
	public string[] dialogueAttaque;
}