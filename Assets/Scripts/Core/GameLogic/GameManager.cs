using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<Button> _buttons;
    [SerializeField]
    private UIController _uiController;
    [SerializeField]
    private BoardView _boardView;
    [SerializeField]
    private Image _emptyImage;
    [SerializeField]
    private TMP_Text _playerName;
    [SerializeField]
    private TMP_Text _aiRivalName;
    [SerializeField]
    private PlayerSettings _playerSettings;
    [SerializeField]
    private AiRivalSettings _aiRivalSettings;
    [SerializeField]
    private Button _backToLobbyButton;
    [SerializeField]
    private Image _sceneFaderImage;

    private float _fadeDuration = 1.5f;

    private Sprite _playerSprite;
    private Sprite _aiRivalSprite;
    private Sprite _emptySprite;

    private InputController _input;
    private BoardController _board;
    private TurnManager _turnManager;
    private WinChecker _winChecker;

    private const string LobbyScene = "Lobby";

    private void Start()
    {
        SetSpriteReferences();
        GetGameLogic();
        RestartGame();
        GetInput();

        _uiController.SetRestartListener(RestartGame);
        _backToLobbyButton.onClick.AddListener(() => LoadLobbyScene());
    }

    private void OnDestroy()
    {
        if (_input != null)
            _input.OnCellClicked -= OnCellClicked;
    }

    private void GetGameLogic()
    {
        _board = new BoardController(_buttons, _emptySprite);
        _turnManager = new TurnManager(_playerSprite, _aiRivalSprite, _playerName, _aiRivalName);
        _winChecker = new WinChecker();
    }

    private void GetInput()
    {
        _input = new InputController(_buttons);
        _input.OnCellClicked += OnCellClicked;
    }

    private void SetSpriteReferences()
    {
        _playerSprite = _playerSettings.playerSprite;
        _aiRivalSprite = _aiRivalSettings.aiSprite;
        _emptySprite = _emptyImage.sprite;
    }

    private void RestartGame()
    {
        SceneFader();
        _board.Reset();
        _turnManager.Reset();
        _boardView.HideAllLines();
        _uiController.ShowCurrentPlayer(_turnManager.CurrentName());
    }

    private void SceneFader()
    {
        _sceneFaderImage.gameObject.SetActive(true);
        _sceneFaderImage.canvasRenderer.SetAlpha(1f);
        _sceneFaderImage.CrossFadeAlpha(0f, _fadeDuration, false);
    }

    private void OnCellClicked(int index)
    {
        if (!_board.IsCellEmpty(index)) return;

        _board.SetCell(index, _turnManager.currentSprite);
        Sprite[,] boardState = _board.GetBoardState();

        if (_winChecker.IsGameOver(boardState, out Sprite winner, out BoardView.WinLineType? winLine))
        {
            if (winner != null)
            {
                if (winLine.HasValue)
                    _boardView.ShowWinLine(winLine.Value);

                string winnerName = GetNameBySprite(winner);
                _uiController.ShowResult($" \n{winnerName} wins!");
            }
            else
            {
                _uiController.ShowResult("Draw!");
            }

            _board.DisableAll();
        }
        else
        {
            _turnManager.NextTurn();
            _uiController.ShowCurrentPlayer(_turnManager.CurrentName());
        }
    }

    private string GetNameBySprite(Sprite sprite)
    {
        if (sprite == _playerSprite) return _playerName.text;
        if (sprite == _aiRivalSprite) return _aiRivalName.text;
        return "Unknown";
    }

    public void LoadLobbyScene()
    {
        SceneManager.LoadScene(LobbyScene);
    }
}
