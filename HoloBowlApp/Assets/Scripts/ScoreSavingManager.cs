using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Assets.Scripts
{
    public class ScoreSavingManager : MonoBehaviour, IInputClickHandler
    {
        public string AvailableCharacters = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";
        public GameObject[] Letters;
        public TextMesh ScoreTitle;

#if UNITY_UWP
        private async void _save()
        {
            var score = AppManager.Instance.CurrentScore;

            score.PlayerName = string.Empty;

            foreach (var letter in Letters)
                score.PlayerName += letter.transform.Find("Letter").GetComponentInChildren<TextMesh>().text;

            if( AppManager.Instance.AzureFunctionService != null)
                await AppManager.Instance.AzureFunctionService.AddScore(score);
            AppManager.Instance.AppStatus = AppStatus.ScoreBoard;
        }
#endif

#if !UNITY_UWP
        private void _save()
        {
            var score = AppManager.Instance.CurrentScore;

            score.PlayerName = string.Empty;

            foreach (var letter in Letters)
                score.PlayerName += letter.transform.Find("Letter").GetComponentInChildren<TextMesh>().text;

            AppManager.Instance.AppStatus = AppStatus.ScoreBoard;
        }
#endif

        public void OnEnable()
        {
            ScoreTitle.text = "You scored " + AppManager.Instance.CurrentScore.PlayerScore;
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {

            // guardar
            if (eventData.selectedObject.name == "SaveButton")
                _save();

            // retroceder o aumentar letra
            if (eventData.selectedObject.name != "Back" && eventData.selectedObject.name != "Forward") return;

            var textMesh = eventData.selectedObject.transform.parent.transform.Find("Letter").GetComponentInChildren<TextMesh>();
            var index = AvailableCharacters.IndexOf(textMesh.text, StringComparison.Ordinal);
            var newIndex = index;

            switch (eventData.selectedObject.name)
            {
                case "Back":
                    newIndex = index == 0 ? AvailableCharacters.Length - 1 : index - 1;
                    break;
                case "Forward":
                    newIndex = index == AvailableCharacters.Length - 1 ? 0 : index + 1;
                    break;
            }

            textMesh.text = AvailableCharacters[newIndex].ToString();
        }
    }
}
