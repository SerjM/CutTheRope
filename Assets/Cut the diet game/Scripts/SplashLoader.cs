using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Cut_the_diet_game.Scripts
{
    public class SplashLoader : MonoBehaviour {

        [SerializeField] private Image _logo;
        [SerializeField] private float _startDirection;
        [SerializeField] private float _endDirection;
        [SerializeField] private Ease _startEase;
        [SerializeField] private Ease _endEase;
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);
            var secuance = DOTween.Sequence();
            secuance.Append(_logo.DOColor(new Color(1,1,1,1), _startDirection)).SetEase(_startEase);
            secuance.OnComplete(() =>
            {
                CloseOperation();
            });

        }
        private void CloseOperation()
        {
            var secuance = DOTween.Sequence();
            secuance.Append(_logo.DOColor(new Color(1, 1, 1, 0), _endDirection)).SetEase(_endEase);
            secuance.OnComplete(() =>
            {
                GoToMenu();
            });
        }

        private void GoToMenu()
        {
            SceneManager.LoadScene("Menu");
        }
   
    }
}
