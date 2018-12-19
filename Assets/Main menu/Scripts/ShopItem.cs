using UnityEngine;

namespace Main_menu.Scripts
{
    public class ShopItem : ScriptableObject
    {
        public string Head;
        public string Body;
        public int Price;
        public Sprite Image;
        public int NeededLevel;

        public bool IsBought;
    }
}
