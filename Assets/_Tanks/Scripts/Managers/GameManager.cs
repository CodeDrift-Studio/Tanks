using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Tanks.Complete.FirebaseSystem;

namespace Tanks.Complete
{
    public class GameManager : MonoBehaviour
    {
        // Which state the game is currently in
        public enum GameState
        {
            MainMenu,
            Game
        }

        // Data about the selected tanks passed from the menu to the GameManager
        public class PlayerData
        {
            public bool IsComputer;
            public Color TankColor;
            public GameObject UsedPrefab;
            public int ControlIndex;
        }
        
        public int m_NumRoundsToWin = 5;            // The number of rounds a single player has to win to win the game.
        public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
        public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases.
        public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases.

        [Header("Tanks Prefabs")]
        public GameObject m_Tank1Prefab;            // The Prefab used by the tank in Slot 1 of the Menu
        public GameObject m_Tank2Prefab;            // The Prefab used by the tank in Slot 2 of the Menu
        public GameObject m_Tank3Prefab;            // The Prefab used by the tank in Slot 3 of the Menu
        public GameObject m_Tank4Prefab;            // The Prefab used by the tank in Slot 4 of the Menu
        
        [FormerlySerializedAs("m_Tanks")] 
        public TankManager[] m_SpawnPoints;         // A collection of managers for enabling and disabling different aspects of the tanks.
        
        private GameState m_CurrentState;
        
        private int m_RoundNumber;                  // Which round the game is currently on.
        private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
        private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.
        private TankManager m_RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won.
        private TankManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.

        private PlayerData[] m_TankData;            // Data passed from the menu about each selected tank (at least 2, max 4)
        private float m_MatchStartTime;             // Time.time when the match started (scaled time, so pauses are excluded)
        private int m_PlayerCount = 0;             // The number of players (2 to 4), decided from the number of PlayerData passed by the menu
        private TextMeshProUGUI m_TitleText;        // The text used to display game message. Automatically found as part of the Menu prefab

        private void Start()
        {
            m_CurrentState = GameState.MainMenu;

            // Find the text used to display game info. Need to look at inactive object too, as the Menu prefab (which contains it) may be
            // disabled at the start when the user have a Title Screen which will enable the Menu.
            var textRef = FindAnyObjectByType<MessageTextReference>(FindObjectsInactive.Include);

            // If that text couldn't be found, we display an error and exit as it is required for the game manager to work
            if (textRef == null)
            {
                Debug.LogError("You need to add the Menus prefab in the scene to use the GameManager!");
                return;
            }

            m_TitleText = textRef.Text;
            m_TitleText.text = "";

            // The GameManager require 4 tanks prefabs, as the start menu have 4 fixed slot and need the 4 tanks to show there
            if (m_Tank1Prefab == null || m_Tank2Prefab == null || m_Tank3Prefab == null || m_Tank4Prefab == null)
            {
                Debug.LogError("You need to assign 4 tank prefab in the GameManager!");
            }
        }

        void GameStart()
        {
            // Create the delays so they only have to be made once.
            m_StartWait = new WaitForSeconds (m_StartDelay);
            m_EndWait = new WaitForSeconds (m_EndDelay);

            SpawnAllTanks();
            SetCameraTargets();

            // Once the tanks have been created and the camera is using them as targets, start the game.
            StartCoroutine (GameLoop ());
        }

        void ChangeGameState(GameState newState)
        {
            m_CurrentState = newState;
            FirebaseManager.SetGameState(newState.ToString(), m_RoundNumber);

            switch (m_CurrentState)
            {
                case GameState.Game:
                    GameStart();
                    break;
            }
        }

        // Called by the menu, passing along the data from the selection made by the player in the menu
        public void StartGame(PlayerData[] playerData)
        {
            m_TankData = playerData;
            m_PlayerCount = m_TankData.Length;

            int bots = 0;
            foreach (var data in m_TankData)
                if (data.IsComputer) bots++;
            m_MatchStartTime = Time.time;
            FirebaseManager.ReportMatchStarted(m_PlayerCount, m_PlayerCount - bots, bots, m_NumRoundsToWin,
                SceneManager.GetActiveScene().name);

            ChangeGameState(GameState.Game);
        }


        private void SpawnAllTanks()
        {
            // For all the tanks...
            for (int i = 0; i < m_PlayerCount; i++)
            {
                var playerData = m_TankData[i];
                
                // ... create them, set their player number and references needed for control.
                m_SpawnPoints[i].m_Instance =
                    Instantiate(playerData.UsedPrefab, m_SpawnPoints[i].m_SpawnPoint.position, m_SpawnPoints[i].m_SpawnPoint.rotation) as GameObject;

                //this guard against possible user error : if they created a prefab with Is Computer Control set to true
                //then all of those prefab would be bots. So we ensure it's to false (the IsComputer from player data
                //will re-enable this if needed when the game start)
                var mov = m_SpawnPoints[i].m_Instance.GetComponent<TankMovement>();
                mov.m_IsComputerControlled = false;
                
                m_SpawnPoints[i].m_PlayerNumber = i + 1;
                m_SpawnPoints[i].ControlIndex = playerData.ControlIndex;
                m_SpawnPoints[i].m_PlayerColor = playerData.TankColor;
                m_SpawnPoints[i].m_ComputerControlled = playerData.IsComputer;
            }

            AssignDisplayNames();

            //we delayed setup after all tanks are created as they expect to have access to all other tanks in the manager
            foreach (var tank in m_SpawnPoints)
            {
                if(tank.m_Instance == null)
                    continue;
                
                tank.Setup(this);
            }

            // "YOU" nameplate above the human tank only (after Setup, which caches the tank canvas layer)
            foreach (var tank in m_SpawnPoints)
            {
                if (tank.m_Instance != null && tank.m_IsLocalPlayer)
                    tank.CreateNameplate(m_TitleText != null ? m_TitleText.font : null);
            }
        }


        // Presentation-only naming. Gameplay identity (m_PlayerNumber, spawn index, ControlIndex, wins) is untouched.
        // The local player is the human tank on ControlIndex 1 (the one using the on-screen joystick), otherwise the
        // first human tank, wherever it is in the spawn order. AIs are numbered by spawn order, so the numbers are
        // fixed for the whole match. Any additional human keeps the old "PLAYER n" name.
        private void AssignDisplayNames()
        {
            int localIndex = -1;
            for (int i = 0; i < m_PlayerCount; i++)
            {
                if (m_SpawnPoints[i].m_ComputerControlled) continue;
                if (localIndex == -1) localIndex = i;
                if (m_SpawnPoints[i].ControlIndex == 1) { localIndex = i; break; }
            }

            int aiNumber = 0;
            for (int i = 0; i < m_PlayerCount; i++)
            {
                var tank = m_SpawnPoints[i];
                tank.m_IsLocalPlayer = i == localIndex;
                if (tank.m_IsLocalPlayer) tank.m_DisplayName = "YOU";
                else if (tank.m_ComputerControlled) tank.m_DisplayName = "AI " + (++aiNumber);
                else tank.m_DisplayName = "PLAYER " + tank.m_PlayerNumber;
            }
        }


        private void SetCameraTargets()
        {
            // Create a collection of transforms the same size as the number of tanks.
            Transform[] targets = new Transform[m_PlayerCount];

            // For each of these transforms...
            for (int i = 0; i < targets.Length; i++)
            {
                // ... set it to the appropriate tank transform.
                targets[i] = m_SpawnPoints[i].m_Instance.transform;
            }

            // These are the targets the camera should follow.
            m_CameraControl.m_Targets = targets;
        }


        // This is called from start and will run each phase of the game one after another.
        private IEnumerator GameLoop ()
        {
            // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
            yield return StartCoroutine (RoundStarting ());

            // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
            yield return StartCoroutine (RoundPlaying());

            // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
            yield return StartCoroutine (RoundEnding());

            // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
            if (m_GameWinner != null)
            {
                ReportMatchCompleted();

                // If there is a game winner, restart the level.
                SceneManager.LoadScene (0);
            }
            else
            {
                // If there isn't a winner yet, restart this coroutine so the loop continues.
                // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
                StartCoroutine (GameLoop ());
            }
        }


        private IEnumerator RoundStarting ()
        {
            // As soon as the round starts reset the tanks and make sure they can't move.
            ResetAllTanks ();
            DisableTankControl ();

            // Snap the camera's zoom and position to something appropriate for the reset tanks.
            m_CameraControl.SetStartPositionAndSize ();

            // Increment the round number and display text showing the players what round it is.
            m_RoundNumber++;
            m_TitleText.text = "ROUND " + m_RoundNumber;

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return m_StartWait;
        }


        private IEnumerator RoundPlaying ()
        {
            // As soon as the round begins playing let the players control the tanks.
            EnableTankControl ();

            // Clear the text from the screen.
            m_TitleText.text = string.Empty;

            // While there is not one tank left...
            while (!OneTankLeft())
            {
                // ... return on the next frame.
                yield return null;
            }
        }


        private IEnumerator RoundEnding ()
        {
            // Stop tanks from moving.
            DisableTankControl ();

            // Clear the winner from the previous round.
            m_RoundWinner = null;

            // See if there is a winner now the round is over.
            m_RoundWinner = GetRoundWinner ();

            // If there is a winner, increment their score.
            if (m_RoundWinner != null)
                m_RoundWinner.m_Wins++;

            // Now the winner's score has been incremented, see if someone has one the game.
            m_GameWinner = GetGameWinner ();

            FirebaseManager.ReportRoundCompleted(m_RoundNumber, ResultFor(m_RoundWinner));

            // The match is over: hide the on-screen mobile controls (no-op on non-mobile / when absent).
            // They are shown again by GameUIHandler.StartGame() when the next match begins.
            if (m_GameWinner != null && MobileUIControl.Instance != null)
                MobileUIControl.Instance.Hide();

            // Get a message based on the scores and whether or not there is a game winner and display it.
            string message = EndMessage ();
            m_TitleText.text = message;

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return m_EndWait;
        }


        // Analytics result from the human side: a bot winning counts as a loss, no winner is a draw.
        private static string ResultFor(TankManager winner)
        {
            if (winner == null) return FirebaseEventRegistry.ResultDraw;
            return winner.m_ComputerControlled ? FirebaseEventRegistry.ResultLoss : FirebaseEventRegistry.ResultWin;
        }

        private void ReportMatchCompleted()
        {
            int humanWins = 0;
            for (int i = 0; i < m_PlayerCount; i++)
                if (!m_SpawnPoints[i].m_ComputerControlled)
                    humanWins += m_SpawnPoints[i].m_Wins;

            FirebaseManager.ReportMatchCompleted(ResultFor(m_GameWinner), m_RoundNumber, humanWins, Time.time - m_MatchStartTime);
        }

        // This is used to check if there is one or fewer tanks remaining and thus the round should end.
        private bool OneTankLeft()
        {
            // Start the count of tanks left at zero.
            int numTanksLeft = 0;

            // Go through all the tanks...
            for (int i = 0; i < m_PlayerCount; i++)
            {
                // ... and if they are active, increment the counter.
                if (m_SpawnPoints[i].m_Instance.activeSelf)
                    numTanksLeft++;
            }

            // If there are one or fewer tanks remaining return true, otherwise return false.
            return numTanksLeft <= 1;
        }
        
        
        // This function is to find out if there is a winner of the round.
        // This function is called with the assumption that 1 or fewer tanks are currently active.
        private TankManager GetRoundWinner()
        {
            // Go through all the tanks...
            for (int i = 0; i < m_PlayerCount; i++)
            {
                // ... and if one of them is active, it is the winner so return it.
                if (m_SpawnPoints[i].m_Instance.activeSelf)
                    return m_SpawnPoints[i];
            }

            // If none of the tanks are active it is a draw so return null.
            return null;
        }


        // This function is to find out if there is a winner of the game.
        private TankManager GetGameWinner()
        {
            // Go through all the tanks...
            for (int i = 0; i < m_PlayerCount; i++)
            {
                // ... and if one of them has enough rounds to win the game, return it.
                if (m_SpawnPoints[i].m_Wins == m_NumRoundsToWin)
                    return m_SpawnPoints[i];
            }

            // If no tanks have enough rounds to win, return null.
            return null;
        }


        // Returns a string message to display at the end of each round.
        private string EndMessage()
        {
            // By default when a round ends there are no winners so the default end message is a draw.
            string message = "DRAW!";

            // If there is a winner then change the message to reflect that.
            if (m_RoundWinner != null)
                message = m_RoundWinner.m_ColoredPlayerText + WinVerb(m_RoundWinner) + " THE ROUND!";

            // Add some line breaks after the initial message.
            message += "\n\n\n\n";

            // Go through all the tanks and add each of their scores to the message.
            for (int i = 0; i < m_PlayerCount; i++)
            {
                int wins = m_SpawnPoints[i].m_Wins;
                message += m_SpawnPoints[i].m_ColoredPlayerText + ": " + wins + (wins == 1 ? " WIN\n" : " WINS\n");
            }

            // If there is a game winner, change the entire message to reflect that.
            if (m_GameWinner != null)
                message = m_GameWinner.m_ColoredPlayerText + WinVerb(m_GameWinner) + " THE GAME!";

            return message;
        }


        // "YOU WIN" / "AI 2 WINS" - grammatical agreement for the winner message.
        private static string WinVerb(TankManager winner)
        {
            return winner.m_IsLocalPlayer ? " WIN" : " WINS";
        }


        // This function is used to turn all the tanks back on and reset their positions and properties.
        private void ResetAllTanks()
        {
            for (int i = 0; i < m_PlayerCount; i++)
            {
                m_SpawnPoints[i].Reset();
            }
        }


        private void EnableTankControl()
        {
            for (int i = 0; i < m_PlayerCount; i++)
            {
                m_SpawnPoints[i].EnableControl();
            }
        }


        private void DisableTankControl()
        {
            for (int i = 0; i < m_PlayerCount; i++)
            {
                m_SpawnPoints[i].DisableControl();
            }
        }
    }
}