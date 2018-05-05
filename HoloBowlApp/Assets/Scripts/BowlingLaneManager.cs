using System.Collections;
using Assets.Scripts.Models;
using HoloToolkit.Unity;
using UnityEngine;

namespace Assets.Scripts
{
    public class BowlingLaneManager : MonoBehaviour
    {
        public int Rounds = 2;
        public int Shots = 2;
        public GameObject BallPrefab;
        public GameObject PinsPrefab;
        public TextMesh InfoText;

        private int _currentRounds;
        private int _currentShots;
        private GameObject _currentBall;
        private GameObject _currentPins;
        private TextToSpeech _textToSpeech;
        private int _totalScore;
        private string _originalText;


        void Start()
        {
            InfoText.text = "Let's go. You have " + Rounds + " rounds of " + Shots + " shots";
            _originalText = InfoText.text;
        }

        private void OnEnable()
        {
            _textToSpeech = GetComponent<TextToSpeech>();

            if (_textToSpeech != null && _textToSpeech.isActiveAndEnabled)
                _textToSpeech.SpeakSsml(InfoText.text);

            _currentRounds = Rounds;
            _currentShots = Shots;

            if (!string.IsNullOrEmpty(_originalText))
                InfoText.text = _originalText;

            _setBall();
            _setPins();
        }

        private void _setBall()
        {
            if (_currentBall != null)
                Destroy(_currentBall);

            _currentBall = Instantiate(BallPrefab, transform);
            _currentBall.GetComponent<BallManager>().OnStopDragg = OnStopDragg;
        }

        private void _setPins()
        {
            if (_currentPins != null)
                Destroy(_currentPins);

            _currentPins = Instantiate(PinsPrefab, transform);
        }

        private void OnStopDragg()
        {
            StartCoroutine(WaitAndCount());
            _currentBall.GetComponent<BallManager>().IsDraggable = false;
        }

        private IEnumerator WaitAndCount()
        {
            yield return new WaitForSeconds(4);

            var count = _countHittedPins();

            if (count == _currentPins.transform.childCount)
                _setPins();

            InfoText.text = "You hit " + count + " pins";

            if (_textToSpeech != null)
                _textToSpeech.SpeakSsml(InfoText.text);

            _totalScore += count;

            // TODO refactorizar esto
            _currentShots--;

            if (_currentShots == 0)
            {
                _setPins();
                _currentRounds--;
                _currentShots = 2;
            }

            if (_currentRounds == 0)
            {
                if (AppManager.Instance != null)
                {
                    AppManager.Instance.CurrentScore = new Score
                    {
                        PlatformId = "HoloLens",
                        PlayerScore = _totalScore
                    };

                    AppManager.Instance.AppStatus = AppStatus.ScoreSaving;
                };

                InfoText.text = string.Empty;
                _totalScore = 0;
                yield break;
            }

            _setBall();
        }

        private int _countHittedPins()
        {
            var count = 0;

            foreach (Transform pin in _currentPins.transform)
            {
                if (pin.localEulerAngles.x < 355 || pin.localEulerAngles.y < 355)
                {
                    Destroy(pin.gameObject);
                    count++;
                }
            }
            return count;
        }
    }
}
