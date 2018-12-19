using UnityEngine;

namespace Cut_the_diet_game.Scripts
{
    public class CameraColor : MonoBehaviour {

        private GameObject _back;

        private void Start()
        {
            _back = GameObject.Find("Back");
            SetColor();
        }

        private void SetColor()
        {
            Texture2D backgroundTex = _back.GetComponent<SpriteRenderer>().sprite.texture;
            Color pixelColor = backgroundTex.GetPixel(50, backgroundTex.height/2);
            gameObject.GetComponent<Camera>().backgroundColor = pixelColor;
        }
    }
}
