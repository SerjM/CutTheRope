using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Main_menu.Scripts
{
    public class ShopItemUi : MonoBehaviour
    {

        public Text Head;
        public Text Body;
        public Text Price;
        public Image Image;

        private ShopItem _item;

        public void Draw(ShopItem item)
        {
            Head.text = item.Head;
            Body.text = item.Body;
            Price.text = item.Price.ToString();
            Image.sprite = item.Image;
        }

        public void OnClick()
        {
            ProgressManager.Buy(_item);
        }

    }
}
