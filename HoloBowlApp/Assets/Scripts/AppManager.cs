using Assets.Scripts.Azure;
using Assets.Scripts.Models;
using HoloToolkit.Unity;
using UnityEngine;

namespace Assets.Scripts
{
    public enum AppStatus
    {
        Initial,
        PlacementRequired,
        Ready,
        Playing,
        ScoreSaving,
        ScoreBoard
    }

    public class AppManager : Singleton<AppManager>
    {
        public AppStatus AppStatus;
        public GameObject BowlingLanePreview;
        public GameObject BowlingLane;
        public GameObject ScoreSaving;
        public GameObject ScoreBoard;
        public Score CurrentScore;

        [HideInInspector]
        public AzureFunctionService AzureFunctionService;

        private void Start()
        {
            AzureFunctionService = GetComponent<AzureFunctionService>();
        }

        private void Update()
        {
            switch (AppStatus)
            {
                case AppStatus.Initial:
                    HoloBowlPlacementManager.Instance.GotTransform = false;

                    BowlingLanePreview.SetActive(true);
                    BowlingLane.SetActive(false);
                    AppStatus = AppStatus.PlacementRequired;
                    break;

                case AppStatus.PlacementRequired:
                    if (!HoloBowlPlacementManager.Instance.GotTransform) return;

                    BowlingLanePreview.SetActive(false);
                    BowlingLane.SetActive(true);
                    AppStatus = AppStatus.Playing;
                    break;

                case AppStatus.Playing:
                    BowlingLane.SetActive(false);
                    BowlingLane.SetActive(true);
                    AppStatus = AppStatus.Ready;
                    break;

                case AppStatus.ScoreSaving:
                case AppStatus.ScoreBoard:

                    if (ScoreSaving == null || ScoreBoard == null)
                    {
                        AppStatus = AppStatus.Initial;
                        break;
                    }

                    ScoreSaving.SetActive(AppStatus == AppStatus.ScoreSaving);
                    ScoreBoard.SetActive(AppStatus == AppStatus.ScoreBoard);

                    AppStatus = AppStatus.Ready;
                    break;
            }
        }
    }
}