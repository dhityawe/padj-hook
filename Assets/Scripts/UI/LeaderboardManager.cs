using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Server;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class LeaderboardManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnEnable()
        {
            // fetching leaderboard
            StartCoroutine(Leaderboard.GetLeaderboard());
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
