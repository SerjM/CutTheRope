using UnityEngine;
using UnityEngine.Events;

namespace Cut_the_diet_game.Scripts.UiScripts
{
    public class DialogManager : MonoBehaviour
    {
        public static DialogManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        public GameObject DialogueYNPrefab;

        public void Show(UnityAction call)
        {
            var dialogueGo = Instantiate(DialogueYNPrefab);
            var dialogueScript = dialogueGo.GetComponent<ConfirmationDialogueUi>();
            dialogueScript.SetButtonYEvent(call);
        }
    }
}
