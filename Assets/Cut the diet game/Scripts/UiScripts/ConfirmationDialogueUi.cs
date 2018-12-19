using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Cut_the_diet_game.Scripts.UiScripts
{
    public class ConfirmationDialogueUi : MonoBehaviour
    {
        [SerializeField] private Text HeadText;
        [SerializeField] private Text BodyText;

        [SerializeField] private Button ButtonY;
        [SerializeField] private Button ButtonN;

        public void Hide()
        {
            //TODO animate
            Destroy(gameObject);
        }

        public void SetButtonYEvent(UnityAction call)
        {
            ButtonY.onClick.AddListener(call);
        }

    }
}
