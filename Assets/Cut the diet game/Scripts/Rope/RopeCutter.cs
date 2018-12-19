using System.Linq;
using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.Rope
{
    public class RopeCutter : MonoBehaviour
    {
        public bool DrawDebugRays = true;
        private Vector3 _lastFramePosition;
        private Rigidbody2D _rigidbody;
        private Camera _mainCamera;
        private Vector2 _direction;


        public string[] CutTags = {"Rope"};

    private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _mainCamera = Camera.main;
        }

        public void UpdatePosition(Vector2 pos)
        {
            _lastFramePosition = transform.position;
        
            var newPos = _mainCamera.ScreenToWorldPoint(
                new Vector3(pos.x, pos.y, 
                    -_mainCamera.transform.position.z));
            _rigidbody.position = newPos;

            var direction = newPos - _lastFramePosition; //Направление в котором прошли за последний кадр
            _direction = direction;
            //CastRay(direction);
        }

        private void LateUpdate()
        {
            CastRay(_direction);
        }

        private void CastRay(Vector3 direction)
        {
            
            RaycastHit2D hit = Physics2D.Raycast(_lastFramePosition, direction, direction.magnitude, LayerMask.GetMask(CutTags));
            
            //Draw debug rays
            if (DrawDebugRays && hit.transform != null && CutTags.Contains(hit.transform.tag))
            {
                //Destroy(hit.transform.gameObject);
                var rope = hit.transform.GetComponentInParent<Rope>();
                if (!rope.IsCut)
                {
                    rope.Cut(hit.point);
                    AudioManager.PlaySoundEffect("Cut");
                }

                Debug.DrawRay(_lastFramePosition, direction, Color.green, 5f);
            }
            else
            {
                Debug.DrawRay(_lastFramePosition, direction, Color.red, 1f);
            }
        }

        public static Vector3 ScreenToWordPoint(Vector2 pos)
        {
            return Camera.main.ScreenToWorldPoint(
                new Vector3(pos.x, pos.y, 
                    -Camera.main.transform.position.z));
        }



    }
}
