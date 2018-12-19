using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Main_menu.Scripts
{
    public class GameMenuTabs : MonoBehaviour
    {
        public RectTransform[] Menus;
        public Image[] Tabs;

        public Sprite ActiveTabSprite;
        public Sprite InactiveTabSprite;

        public void Start()
        {
            OnTabClick(1);
        }

        public void OnTabClick(int tabN)
        {
            //табы нумеруются 0 1 2

            for (int i = 0; i < Menus.Length; i++)
            {
                Menus[i].DOAnchorPos(new Vector2(1000 * i - (1000 * tabN), 0f), 0.25f);

                if (tabN == i)
                {
                    Tabs[i].sprite = ActiveTabSprite;
                    Tabs[i].transform.SetAsLastSibling();
                    
                }
                else
                {
                    Tabs[i].sprite = InactiveTabSprite;
                }
            }
        }

    }
}
