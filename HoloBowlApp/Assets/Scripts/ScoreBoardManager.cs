using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;
using HoloToolkit.Unity.Collections;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Assets.Scripts
{
    public class ScoreBoardManager : MonoBehaviour, IInputClickHandler
    {
        public GameObject ScoreListPrefab;
        public GameObject ScorePrefab;
        private GameObject _currentScoreList;

        private IEnumerable<Score> _scores;
        private bool _shouldRender;

#if UNITY_UWP
        private async void OnEnable()
        {
            if (AppManager.Instance.AzureFunctionService != null)
                _scores = await AppManager.Instance.AzureFunctionService.GetScores();
            _shouldRender = true;
        }
#endif

#if UNITY_EDITOR
        private void OnEnable()
        {
            _scores = new List<Score> { AppManager.Instance.CurrentScore };
            _shouldRender = true;
        }
#endif

        private void Update()
        {
            if (_shouldRender)
                _setScores(_scores);
        }

        private void _setScores(IEnumerable<Score> scores)
        {
            _shouldRender = false;

            if (_currentScoreList != null)
                Destroy(_currentScoreList);

            _currentScoreList = Instantiate(ScoreListPrefab, gameObject.transform);

            if (scores != null)
                foreach (var score in scores.OrderByDescending(o => o.PlayerScore).Take(10))
                {
                    var newScore = Instantiate(ScorePrefab, _currentScoreList.transform);
                    newScore.transform.Find("PlayerName").GetComponent<TextMesh>().text = score.PlayerName;
                    newScore.transform.Find("PlayerScore").GetComponent<TextMesh>().text = score.PlayerScore.ToString();
                }

            for (var i = _currentScoreList.transform.childCount; i <= 9; i++)
                Instantiate(ScorePrefab, _currentScoreList.transform);

            _currentScoreList.GetComponent<ObjectCollection>().UpdateCollection();
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (eventData.selectedObject.name == "PlayAgainButton")
            {
                AppManager.Instance.AppStatus = AppStatus.Initial;
                gameObject.SetActive(false);
            }
        }
    }
}
