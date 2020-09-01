using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientGoldManager : MonoBehaviour
{
    private Vector3 _goldPosition;
    [SerializeField] private GameObject _gold;

    [SerializeField] private GameObject _leaderBoard;
    [SerializeField] private GameObject _gameEndScreen;


    [SerializeField] private List<LeaderboardElement> _leaderboardElements;

    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private NetworkGameManager _networkGameManager;
    [SerializeField] private List<PlayerManager.Player> _sortedPlayers;
    public GameObject grid;


    [Serializable]
    class LeaderboardElement
    {
        public Image clientRepresentation;
        public TextMeshProUGUI points;
        public TextMeshProUGUI playerName;

        public LeaderboardElement(Image clientRepresentation, TextMeshProUGUI points, TextMeshProUGUI playerName)
        {
            this.clientRepresentation = clientRepresentation;
            this.points = points;
            this.playerName = playerName;
        }
    }


    public void UpdateGoldPosition(int x, int z)
    {
        if (grid.GetComponent<ClientGrid>().game_start)
        {
            GameObject goldParticle = Instantiate(Resources.Load("Prefabs/Goldparticles")) as GameObject;
            _networkGameManager.soundsPlayer.PlayGoldSound();
            goldParticle.transform.parent = grid.transform;
            goldParticle.transform.localPosition = _goldPosition;
        }

        _goldPosition = new Vector3(grid.transform.position.x + x, grid.transform.position.y + 0.85f, grid.transform.position.z + z);
        _gold.transform.position = _goldPosition;
    }


    public void LeaderboardSetup()
    {
        SortPlayersByScore();
        PopulateLeaderboard();
    }

    private void SortPlayersByScore()
    {
        var temp = _playerManager.players;

        _sortedPlayers = temp.OrderByDescending(player => player.score).ToList();
    }

    private void PopulateLeaderboard()
    {
        for (int i = 0; i <= _leaderBoard.transform.childCount; i++)
        {
            if (i < _sortedPlayers.Count)
            {
                // Leaderboard GO
                //    child(0) - image that is used to represent the client - clientRepresentation
                //        child(0) - container of content
                //            child(0) - points/score - pointsText
                //            child(1) - playerID/name - playerName
                _leaderboardElements.Add(new LeaderboardElement(_leaderBoard.transform.GetChild(i).gameObject.GetComponent<Image>(),
                    _leaderBoard.transform.GetChild(i).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>(),
                    _leaderBoard.transform.GetChild(i).transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>()));
            }

            else if (i > _sortedPlayers.Count)
            {
                _leaderBoard.transform.GetChild(i - 1).gameObject.SetActive(false);
            }
        }


        for (int i = 0; i < _leaderboardElements.Count; i++)
        {
            _leaderboardElements[i].playerName.text = _sortedPlayers[i].name;
            _leaderboardElements[i].points.text = _sortedPlayers[i].score.ToString();
        }

        ShowClient();
    }

    public void UpdateLeaderboard()
    {
        SortPlayersByScore();
        for (int i = 0; i < _leaderboardElements.Count; i++)
        {
            _leaderboardElements[i].points.text = _sortedPlayers[i].score.ToString();
            _leaderboardElements[i].playerName.text = _sortedPlayers[i].name;
        }

        ShowClient();
    }

    private void ShowClient()
    {
        for (int i = 0; i < _leaderboardElements.Count; i++)
        {
            _leaderboardElements[i].clientRepresentation.enabled = false;
        }

        var id = _sortedPlayers.Find(player => player.playerID == _playerManager.ClientId).name.ToString();
        _leaderboardElements.Find(element => element.playerName.text == id).clientRepresentation.enabled = true;
    }

    [ContextMenu("test LEaderobard")]
    public void DisplayFinalLeaderboard()
    {
        RectTransform rectTransform = _leaderBoard.GetComponent<RectTransform>();
        _leaderBoard.transform.parent = _gameEndScreen.transform;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(0f, 0f);
        rectTransform.localScale = new Vector3(2f, 2f, 2f);
        _gameEndScreen.SetActive(true);
    }

    public void AssignPlayerManager(PlayerManager playerManager)
    {
        _playerManager = playerManager;
    }
}