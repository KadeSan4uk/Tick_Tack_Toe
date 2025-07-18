using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<Button> _buttons;
    [SerializeField]
    private UIController _ui;
    [SerializeField]
    private BoardView _boardView;

    private InputController _input;
    private BoardController _board;
    private TurnManager _turnManager;
    private WinChecker _winChecker;

    private void Start()
    {
        _input = new InputController(_buttons);
        _input.OnCellClicked += OnCellClicked;

        _board = new BoardController(_buttons);
        _turnManager = new TurnManager();
        _winChecker = new WinChecker();

        _ui.SetRestartListener(RestartGame);
        RestartGame();
    }

    private void OnDestroy()
    {
        if (_input != null)
            _input.OnCellClicked -= OnCellClicked;
    }

    private void RestartGame()
    {
        _board.Reset();
        _turnManager.Reset();
        _boardView.HideAllLines();
        _ui.ShowCurrentPlayer(_turnManager.CurrentSymbol);
    }

    private void OnCellClicked(int index)
    {
        if (!_board.IsCellEmpty(index)) return;

        _board.SetCell(index, _turnManager.CurrentSymbol);
        string[,] boardState = _board.GetBoardState();

        if (_winChecker.IsGameOver(boardState, out string winner, out BoardView.WinLineType? winLine))
        {
            if (winner != null)
            {
                if (winLine.HasValue)
                    _boardView.ShowWinLine(winLine.Value);

                _ui.ShowResult($"Player {winner} wins!");
            }
            else
            {
                _ui.ShowResult("Draw!");
            }

            _board.DisableAll();
        }
        else
        {
            _turnManager.NextTurn();
            _ui.ShowCurrentPlayer(_turnManager.CurrentSymbol);
        }
    }
}
