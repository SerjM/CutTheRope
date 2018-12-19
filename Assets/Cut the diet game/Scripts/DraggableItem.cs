using UnityEngine;

namespace Cut_the_diet_game.Scripts
{
    public class DraggableItem : MonoBehaviour
    {
        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        private void Update()
        {
            if (!Application.isEditor) return;
            if (Input.GetMouseButton(1))
            {
                rb.velocity = Vector2.zero;
                var pos = (Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, 10f)));
                rb.position = Vector2.Lerp(rb.position, pos, 0.5f);
            }
        }
    }
}
