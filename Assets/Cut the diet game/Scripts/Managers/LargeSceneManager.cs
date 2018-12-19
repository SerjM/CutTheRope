using Cut_the_diet_game.Scripts.Level;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.Managers
{
    public enum CamerasStartPlace
    {
        Bottom,
        Upper
    }
    public class LargeSceneManager : MonoBehaviour
    {
        [SerializeField] private float _cameraNormalSpeed;
        [SerializeField] private float _cameraTurboSpeed;
        [SerializeField] private GameObject _bottomBg;
        [SerializeField] private GameObject _upperBg;
        [SerializeField] private CamerasStartPlace _startPlace;
        
        private GameObject[] _cameraPoints;
       
       




        private Camera _cam;
        private GameObject _food;
        private int _camerasDirection;
        private Vector3 _from;
        private Vector3 _to;
        private bool _readyToStart;

        private void Start()
        {
            _cam = Camera.main;
            _food = FindObjectOfType<Food>().gameObject;

            SetCamerasStartPlace(_startPlace);
            AllowMultitouchTo(false);
            _camerasDirection = GetCamerasDirectionAbout(_startPlace);
            SetBgsTransforms(out _from, out _to);
            CreateCameraPoints();
        }

        private void SetCamerasStartPlace(CamerasStartPlace startPlace)
        {
            if(startPlace== CamerasStartPlace.Bottom)
            {
                _cam.gameObject.transform.position = _bottomBg.transform.position - new Vector3(0, 0, 10);
            }
            else
            {
                _cam.gameObject.transform.position = _upperBg.transform.position - new Vector3(0, 0, 10);
            }
        }
        private void AllowMultitouchTo(bool aciveStatus)
        {
            FindObjectOfType<MultitouchManager>().GetComponent<MultitouchManager>().enabled = aciveStatus;
        }

        private int GetCamerasDirectionAbout(CamerasStartPlace startPlace)
        {
            if(startPlace== CamerasStartPlace.Bottom)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        private void SetBgsTransforms(out Vector3 from, out Vector3 to)
        {
            if(_startPlace== CamerasStartPlace.Bottom)
            {

            from = _bottomBg.transform.position-new Vector3(0,0,10);
            to = _upperBg.transform.position - new Vector3(0, 0, 10);
            }
            else
            {
                from = _upperBg.transform.position - new Vector3(0, 0, 10);
                to = _bottomBg.transform.position - new Vector3(0, 0, 10);
            }
        }

        private void Update()
        {
            SetCameraPointsPositions();
            if (!_readyToStart)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _cameraNormalSpeed = _cameraTurboSpeed;
                }
                if (_cam.gameObject.transform.position.y >= _bottomBg.transform.position.y && _cam.gameObject.transform.position.y <= _upperBg.transform.position.y)
                {
                    _cam.gameObject.transform.Translate(Vector3.up * _camerasDirection * _cameraNormalSpeed * Time.deltaTime);
                }
                else
                {
                    _readyToStart = true;
                    AllowMultitouchTo(true);
                    _cam.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    _cam.gameObject.transform.position = _to;
                }
            }
            else
            {
                if (_food == null)
                {
                    return;
                }
                if (_cameraPoints[2].transform.position.y <= _upperBg.transform.GetComponent<SpriteRenderer>().bounds.max.y &&
                    _cameraPoints[3].transform.position.y >= _bottomBg.transform.GetComponent<SpriteRenderer>().bounds.min.y
                    )
                {

                    if (_food.transform.position.y <= _cameraPoints[1].transform.position.y ||
                     _food.transform.position.y >= _cameraPoints[0].transform.position.y)
                    {

                        _cam.gameObject.transform.position = new Vector3(_cam.gameObject.transform.position.x,
                    Mathf.SmoothStep(_cam.gameObject.transform.position.y,Mathf.Round(_food.transform.position.y), 3 * Time.deltaTime), -10);

                    }
                }
                
            }
        }

        private void CreateCameraPoints()
        {
            _cameraPoints = new GameObject[4];
            for (int i = 0; i < _cameraPoints.Length; i++)
            {
                _cameraPoints[i] = new GameObject();
                _cameraPoints[i].transform.SetParent(Camera.main.transform, false);
                _cameraPoints[i].transform.SetAsLastSibling();
            }
            _cameraPoints[0].name = "upperPoint";
            _cameraPoints[1].name = "bottomPoint";
            _cameraPoints[2].name = "topUpperPoint";
            _cameraPoints[3].name = "bigBottomPoint";
        }
        private void SetCameraPointsPositions()
        {
            float width = Camera.main.pixelWidth;
            float height = Camera.main.pixelHeight;
            _cameraPoints[0].transform.position = Camera.main.ScreenToWorldPoint(new Vector2(width/2, height*3/4));
            _cameraPoints[1].transform.position = Camera.main.ScreenToWorldPoint(new Vector2(width/2, height*1/4));
            _cameraPoints[2].transform.position = Camera.main.ScreenToWorldPoint(new Vector2(width / 2, height));
            _cameraPoints[3].transform.position = Camera.main.ScreenToWorldPoint(new Vector2(width / 2,0));
        }
    }
}