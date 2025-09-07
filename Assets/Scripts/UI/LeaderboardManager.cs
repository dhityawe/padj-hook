using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Server;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class LeaderboardManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private List<LeaderboardEntryUI> leaderboardEntries;
        [SerializeField] private List<TextMeshProUGUI> _textFields; // Keep for backward compatibility
        
        [Header("Settings")]
        [SerializeField] private bool autoRefreshOnEnable = true;
        [SerializeField] private float refreshInterval = 30f; // Refresh every 30 seconds
        
        private Coroutine refreshCoroutine;
        
        void Start()
        {
            // Initialize all entries with default values
            InitializeEntries();
        }

        private void OnEnable()
        {
            if (autoRefreshOnEnable)
            {
                // fetching leaderboard
                StartCoroutine(Leaderboard.GetLeaderboard(OnLeaderboardSuccess, OnLeaderboardError));
                
                // Start auto-refresh
                if (refreshCoroutine != null)
                {
                    StopCoroutine(refreshCoroutine);
                }
                refreshCoroutine = StartCoroutine(AutoRefreshCoroutine());
            }
        }
        
        private void OnDisable()
        {
            // Stop auto-refresh
            if (refreshCoroutine != null)
            {
                StopCoroutine(refreshCoroutine);
                refreshCoroutine = null;
            }
        }
        
        private void InitializeEntries()
        {
            if (leaderboardEntries != null)
            {
                foreach (var entry in leaderboardEntries)
                {
                    if (entry != null)
                    {
                        entry.SetDefaultValues();
                    }
                }
            }
        }
        
        private void OnLeaderboardSuccess(List<LeaderboardEntry> entries)
        {
            if (entries == null || entries.Count == 0)
            {
                Debug.LogWarning("Leaderboard data is empty");
                SetAllEntriesToDefault();
                return;
            }
            
            // Update UI entries
            for (int i = 0; i < leaderboardEntries.Count; i++)
            {
                if (leaderboardEntries[i] != null)
                {
                    if (i < entries.Count)
                    {
                        leaderboardEntries[i].SetEntry(entries[i]);
                    }
                    else
                    {
                        leaderboardEntries[i].SetDefaultValues();
                    }
                }
            }
            
            Debug.Log($"Successfully updated leaderboard with {entries.Count} entries");
        }
        
        private void OnLeaderboardError(string errorMessage)
        {
            Debug.LogError($"Leaderboard error: {errorMessage}");
            SetAllEntriesToDefault();
        }
        
        private void SetAllEntriesToDefault()
        {
            if (leaderboardEntries != null)
            {
                foreach (var entry in leaderboardEntries)
                {
                    if (entry != null)
                    {
                        entry.SetDefaultValues();
                    }
                }
            }
        }
        
        private IEnumerator AutoRefreshCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(refreshInterval);
                StartCoroutine(Leaderboard.GetLeaderboard(OnLeaderboardSuccess, OnLeaderboardError));
            }
        }
        
        // Public method to manually refresh leaderboard
        public void RefreshLeaderboard()
        {
            StartCoroutine(Leaderboard.GetLeaderboard(OnLeaderboardSuccess, OnLeaderboardError));
        }
        
        // Public method to clear all entries
        public void ClearLeaderboard()
        {
            SetAllEntriesToDefault();
        }
    }
}
