using System.Collections.Generic;
using Cut_the_diet_game.Scripts.Level;
using Cut_the_diet_game.Scripts.Rope;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.Managers
{
    public class MultitouchManager : MonoBehaviour
    {
        public GameObject CutterPrefab;
        [SerializeField]private bool _isHoldingMouse;
        [SerializeField]private List<RopeCutter> _cutters;
        [SerializeField] private Bubble[] bubbles;
        private void Start()
        {
            try{ bubbles = FindObjectsOfType<Bubble>();} catch (System.Exception){}
        }

        void Update ()
        {
            ProcessMouse();
          //  ProcessTouches();
        }

        private void ProcessTouches()
        {
            foreach (var touch in Input.touches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        AddCutter(touch.position);
                        break;

                    case TouchPhase.Moved:
                        UpdateClosest(touch.position);
                        break;

                    case TouchPhase.Canceled:
                    case TouchPhase.Ended:
                        DestroyClosest(touch.position);
                        break;
                }
            }
        }

        private void ProcessMouse()
        {
          //  if (Input.touchCount > 0) return;

            if (Input.GetMouseButtonDown(0))
            {
                InBubble(Input.mousePosition);
                _isHoldingMouse = true;
                
                AddCutter(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0))
            {
                _isHoldingMouse = false;
                DestroyClosest(Input.mousePosition);
            }

            if (_isHoldingMouse)
            {
                _cutters[0].UpdatePosition(Input.mousePosition);
            }

        }

        private Vector2 pos;
        private void UpdateClosest(Vector2 pos)
        {
            this.pos = pos;
            var closest = GetClosest(pos);

            if (closest != null)
            {
                closest.UpdatePosition(pos);

            }
        }

        private RopeCutter GetClosest(Vector2 pos)
        {
            RopeCutter closest = null;
            float minDistance = float.MaxValue;

            foreach (var c in _cutters)
            {
                var cutterPos = RopeCutter.ScreenToWordPoint(pos);

                float dist = Vector2.Distance(cutterPos, c.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = c;
                }
            }

            return closest;
        }

        private void DestroyClosest(Vector2 pos)
        {

            var closest = GetClosest(pos);

            if (closest != null)
            {
                closest.GetComponent<TrailRenderer>().autodestruct = true; //когда след закончится, объект будет уничтожен
                closest.transform.position = closest.transform.position + Vector3.up * 0.001f; //лютый костыль. трейл не уничтожается параметром autodestruct, если за всю жзнь не был не разу отрисован. То есть можно щёлкнуть мышкой не двигая, и автодеструкт не сработает. Для этого сдвигаем трансформ немного, чтобы треил проинициализировался.
                _cutters.Remove(closest);
                Destroy(closest);
                //print("Cutter Destroyed!");
            }
        }

        private void AddCutter(Vector2 pos)
        {
            //print("Cutter Added!");
            var cutter = Instantiate(CutterPrefab, RopeCutter.ScreenToWordPoint(pos), Quaternion.identity);
            var cutScript = cutter.GetComponent<RopeCutter>();
            _cutters.Add(cutScript);
        }


        private void InBubble(Vector2 pos)
        {
            if (bubbles.Length==0)
            {
                return;
            }
           
           

            for (int i = 0; i < bubbles.Length; i++)
            {
                if (bubbles[i] != null)
                {
                    Vector2 boundsMin = Camera.main.WorldToScreenPoint(bubbles[i].gameObject.GetComponent<CircleCollider2D>().bounds.min);
                    Vector2 boundsMax = Camera.main.WorldToScreenPoint(bubbles[i].gameObject.GetComponent<CircleCollider2D>().bounds.max);
                    if (
                       boundsMin.x <= pos.x && boundsMax.x >= pos.x
                       && boundsMin.y <= pos.y && boundsMax.y >= pos.y
                      )
                    {
                        if (bubbles[i].Catched)
                        {
                            bubbles[i].Pop();
                        }
                    }
                }
            }
        }

    }
}
