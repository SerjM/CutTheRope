using Cut_the_diet_game.Scripts.Level;
using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;

namespace Cut_the_diet_game.Scripts
{
    public class BoxEmotionControl : MonoBehaviour {

        [SerializeField] private Sprite[] _sadFaces;
        [SerializeField] private Sprite[] _eatFaces;
        [SerializeField] private Sprite[] _iddleFaces;
        [SerializeField] private Sprite[] _nearFaces;
        [SerializeField] private SpriteRenderer _face;

        private float _foodAndBoxFirstDistance;
        private GameObject _food;
        private GameObject _foodFollower;
        private float[] _distancePieces;
        private Sprite _eatFace;
        private Sprite _sadFace;

        private void Start()
        {
            _food = FindObjectOfType<Food>().gameObject;
            _foodFollower = new GameObject("foodFollower");
            _foodFollower.transform.position = _food.transform.position;
            _foodAndBoxFirstDistance = GetBoxToFoodDistance();

            //  SetIddleFace();
            CutTheDistance();
            Test();
            SetIddleFace();
            SetEatFace();
            SetSadFace();
        }
        private float GetBoxToFoodDistance()
        {
            return Mathf.Abs( Vector2.Distance(gameObject.transform.position, _foodFollower.transform.position));
        }

        private void SetIddleFace()
        {
            _face.sprite = _iddleFaces[Random.Range(0, _iddleFaces.Length)];
        }

        private void SetEatFace()
        {
            _eatFace = _eatFaces[Random.Range(0, _eatFaces.Length)];

        }

        private void SetSadFace()
        {
            _sadFace= _sadFaces[Random.Range(0, _sadFaces.Length)];
        }
    

        private void Update()
        {
            if (_food != null)
            {
                _foodFollower.transform.position = _food.transform.position;
            }
            ChangeEmotion();
        }

        private void CutTheDistance()
        {
            _distancePieces = new float[_nearFaces.Length];
            float _pieceLength = _foodAndBoxFirstDistance / (_nearFaces.Length+1);

            for (int i =0;i< _distancePieces.Length; i++)
            {
                _distancePieces[i] = _foodAndBoxFirstDistance- (_pieceLength * (i+1));
            }
        }
        private void ChangeEmotion()
        {
            if(Mathf.Round(GetBoxToFoodDistance()) == 0)
            {
                _face.sprite = _eatFace;
                return;
            }
            if (FindObjectOfType<LevelManager>().IsLose)
            {
                _face.sprite = _sadFace;
                return;
            }


            for (int i = 0; i < _distancePieces.Length-1; i++)
            {
                if(GetBoxToFoodDistance()<=_distancePieces[i] && GetBoxToFoodDistance() >= _distancePieces[i + 1])
                {
                    _face.sprite = _nearFaces[i];
                }
                if(GetBoxToFoodDistance() <= _distancePieces[_distancePieces.Length - 1])
                {
                    _face.sprite = _nearFaces[_nearFaces.Length-1];
                }
            }
        }

        private void Test()
        {
            Debug.Log(_foodAndBoxFirstDistance);
            for (int i = 0; i < _distancePieces.Length; i++)
            {
                Debug.Log("distancePiece[" + i + "]= " + _distancePieces[i]);
            }

        }

    }
}
