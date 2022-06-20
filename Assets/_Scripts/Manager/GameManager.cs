using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Prefab.Popup.NotiPopup;
using _Scripts.Handler;
using _Scripts.System;
using DG.Tweening;
using Scene.GameScene;
using UnityEngine;
using static qtHelper;

public class GameManager : qtSingleton<GameManager>
{
    #region Controller
    
    #endregion

    #region ----- VARIABLE -----

    
    private const int Col = 9;
    private const int Row = 9;
    
    private GameObject _board;

    private SquareHandler[,] _squareHandlers;
    private List<SquareHandler> _squareForCheck;
    private List<SquareHandler> _ballQueue;
    private List<List<SquareHandler>> _row;
    private Dictionary<SquareHandler, bool> _path;

    private bool _isSetUp;

    public SquareHandler selectedBall
    {
        get;
        private set;
    }
    public int score { get; private set; }
    
    #endregion

    public void StartGame()
    {
        Initialize();
    }

    #region ----- PUBLIC FUNCTION -----
    
    public void TargetSelect(SquareHandler target)
    {
        if (selectedBall == null)
        {
            if (target.ball == null || target.ball.state == EBallState.Queue)
            {
                return;
            }
            
            selectedBall = target;
            return;
        }
        
        if (selectedBall != null && selectedBall.Equals(target))
        {
            selectedBall = null;
            return;
        }

        if (target.ball != null)
        {
            if (target.ball.state == EBallState.Normal)
            {
                selectedBall = target;
                return;
            }
            
            var availableCount = _squareForCheck.FindAll(square => !square.hasBall).Count;
            if (availableCount != 0)
            {
                SquareHandler square = null;
                do
                {
                    square = _squareHandlers[Random.Range(0, Col), Random.Range(0, Row)];
                } while (square.hasBall);

                var ball = target.ball;
                _ballQueue.Remove(target);
                _ballQueue.Add(square);
            
                square.ball = ball;
                target.ball = null;
                ball.transform.position = square.transform.position;
            }
        }

        if (selectedBall.ball.type == EBallType.Ghost)
        {
            
        }
        else
        {
            foreach (var key in _path.Keys.ToList())
            {
                key.node = null;
                _path[key] = false;
            }
            // StartCoroutine(FindPath(selectedBall, target));
            if (FindPath(selectedBall, target))
            {
                Debug.LogWarning("Move");
                target.ball = selectedBall.ball;
                StartCoroutine(Move(selectedBall));
                selectedBall.ball = null;
                selectedBall = null;
            
                _ballQueue.ForEach(square =>
                {
                    square.ball.Grow();
                });
                _ballQueue.Clear();
            
                ScoreCalculate();
            
                if (isEnd)
                {
                    ((NotiPopup) UIManager.Instance.ShowPopup(qtScene.EPopup.Noti)).Initialize("You lose");
                }
            
                NewTurn();
            }
            else
            {
                Debug.LogError("Cant find path!");
                selectedBall = null;
            }
        }
    }
    

    #endregion

    #region ----- PRIVATE FUNCTION -----
    
    private void Initialize()
    {
        score = 0;

        if (!_isSetUp)
        {
            SetUpBoard();
        }
        
        //Todo: Reset board
        //_squareHandlers.Clear();
        
        //Todo: Spawn ball
        _ballQueue.ForEach(ball => ball.gameObject.SetActive(false));
        _ballQueue.Clear();

        StartCoroutine(InitBall());
    }

    private void SetUpBoard()
    {
        _isSetUp = true;

        _board = FindObjectWithPath(UIManager.Instance.currentScene.gameObject, "Board");
        _squareHandlers ??= new SquareHandler[Col, Row];
        _squareForCheck ??= new List<SquareHandler>();
        _ballQueue ??= new List<SquareHandler>();
        _row ??= new List<List<SquareHandler>>();
        _path ??= new Dictionary<SquareHandler, bool>();

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if ((row + col) % 2 == 0)
                {
                    var tempSquare = qtPooling.Instance.Spawn(DataManager.Instance.SquareLight.name,
                        DataManager.Instance.SquareLight, _board.transform).GetComponent<SquareHandler>();
                    tempSquare.Initialize(col, row);
                    tempSquare.gameObject.SetActive(true);
                    _squareHandlers[col, row] = tempSquare;
                    _squareForCheck.Add(tempSquare);
                    _path.Add(tempSquare, false);
                }
                else
                {
                    var tempSquare = qtPooling.Instance.Spawn(DataManager.Instance.SquareDark.name,
                        DataManager.Instance.SquareDark, _board.transform).GetComponent<SquareHandler>();
                    tempSquare.Initialize(col, row);
                    _squareHandlers[col, row] = tempSquare;
                    _squareForCheck.Add(tempSquare);
                    _path.Add(tempSquare, false);
                }
            }
        }
        
        //Get row
        var tempList = new List<SquareHandler>();
        //col
        for (int i = 0; i < Row; i++)
        {
            tempList = new List<SquareHandler>();
            for (int j = 0; j < Col; j++)
            {
                tempList.Add(_squareHandlers[j, i]);
            }
            _row.Add(tempList);
        }

        //row
        for (int i = 0; i < Col; i++)
        {
            tempList = new List<SquareHandler>();
            for (int j = 0; j < Row; j++)
            {
                tempList.Add(_squareHandlers[i, j]);
            }
            
            _row.Add(tempList);
        }

        //diagonal l-r
        for (int i = 2; i < Col; i++)
        {
            tempList = new List<SquareHandler>();
            for (int j = 0; j < i; j++)
            {
                tempList.Add(_squareHandlers[i-j, j]);
            }

            _row.Add(tempList);
        }
        
        //diagonal l-r
        for (int i = 1; i < Col - 2; i++)
        {
            tempList = new List<SquareHandler>();
            for (int j = 0; j < Row - i; j++)
            {
                tempList.Add(_squareHandlers[i+j, Col - 1 - j]);
            }
            
            _row.Add(tempList);
        }
        
        //diagonal r-l
        for (int i = Col - 3; i >= 1; i--)
        {
            tempList = new List<SquareHandler>();
            for (int j = 0; j < Col - i; j++)
            {
                tempList.Add(_squareHandlers[i+j, j]);
            }

            _row.Add(tempList);
        }
        
        //diagonal r-l
        for (int i = 2; i < Col - 1; i++)
        {
            tempList = new List<SquareHandler>();
            for (int j = 0; j < i; j++)
            {
                tempList.Add(_squareHandlers[i-j, Row - 1 - j]);
            }
            
            _row.Add(tempList);
        }
    }

    private IEnumerator InitBall()
    {
        var delayCoroutine = new WaitForSeconds(0.25f);
        yield return delayCoroutine;
        for (int i = 0; i < 8; i++)
        {
            SquareHandler square = null;
            do
            {
                square = _squareHandlers[Random.Range(0, Col), Random.Range(0, Row)];
            } while (square.hasBall);
            var randomColor = Random.Range(0, DataManager.Instance.colorBank.Length);
            
            var ball = qtPooling.Instance.Spawn(DataManager.Instance.Ball.name, DataManager.Instance.Ball, UIManager.Instance.currentScene.transform).GetComponent<BallHandler>();
            ball.transform.localScale = 0.3f * Vector3.one;
            ball.Initialize(randomColor, EBallState.Normal);
            ball.Grow();

            ball.transform.position = square.transform.position;
            square.ball = ball;
        }
        NewTurn();
    }

    private IEnumerator Move(SquareHandler startPosition)
    {
        var temp = startPosition;
        var ball = startPosition.ball;
        var moveCoroutine = new WaitForSeconds(0.01f);
        while (temp.node != null)
        {
            ball.transform.DOMove(temp.node.transform.position, 0.01f);
            temp = temp.node;
            yield return moveCoroutine;
        }

        ball.transform.position = temp.transform.position;
    }

    private void NewTurn()
    {
        var availableCount = _squareForCheck.FindAll(square => !square.hasBall).Count;
        if (availableCount >= 3)
        {
            availableCount = 3;
        }
        for (int i = 0; i < availableCount; i++)
        {
            SquareHandler square = null;
            do
            {
                square = _squareHandlers[Random.Range(0, Col), Random.Range(0, Row)];
            } while (square.hasBall);
            
            var ball = qtPooling.Instance.Spawn(DataManager.Instance.Ball.name, DataManager.Instance.Ball, UIManager.Instance.currentScene.transform).GetComponent<BallHandler>();
            
            var randomColor = Random.Range(0, DataManager.Instance.colorBank.Length);
            ball.Initialize(randomColor, EBallState.Queue);
            ball.transform.localScale = 0.3f * Vector3.one;

            ball.transform.position = square.transform.position;
            square.ball = ball;
            _ballQueue.Add(square);
        }

    }

    private bool isEnd => _squareForCheck.Find(square => !square.hasBall || square.ball.state == EBallState.Queue) == null;

    private void ScoreCalculate()
    {
        foreach (var row in _row)
        {
            score += ScoreCheck(row);
        }

        score += _squareForCheck.FindAll(square => square.isScore).Count;

        foreach (var squareHandler in _squareHandlers)
        {
            if (squareHandler.isScore)
            {
                squareHandler.Score();
            }
        }

        ((GameScene) UIManager.Instance.currentScene).UpdateUI();
    }

    private int ScoreCheck(List<SquareHandler> checkList)
    {
        int bonus = 0;
        bool isBonus = false;
        var scoreList = new List<SquareHandler>();
        int countBall = 1;
        scoreList.Clear();
        SquareHandler root = checkList[0];
        scoreList.Add(root);
        for (int j = 1; j < checkList.Count; j++)
        {
            if (!checkList[j].hasBall 
                || !root.hasBall 
                || checkList[j].ball.color != root.ball.color
                || root.ball.state == EBallState.Queue
                || checkList[j].ball.state == EBallState.Queue)
            {
                countBall = 1;
                root = checkList[j];
                scoreList.Clear();
                scoreList.Add(root);
            }
            else
            {
                countBall++;
                root = checkList[j];
                scoreList.Add(root);
                if (countBall >= 3)
                {
                    Debug.Log(scoreList.Count);
                    scoreList.ForEach(square => square.isScore = true);
                }

                if (countBall >= 5 && !isBonus)
                {
                    isBonus = true;
                    bonus++;
                }
            }
        }

        return bonus;
    }
    

    private int count;
    private bool FindPath(SquareHandler startPosition, SquareHandler targetPosition)
    {
        count++;
        if (startPosition.Equals(targetPosition))
        {
            return true;
        }
        
        if (AvailableDirection(targetPosition).Count == 0)
        {
            //Cant move to target
            return false;
        }
        
        var availableMove = AvailableDirection(startPosition);
        if (availableMove.Count == 0)
        {
            //Start cant move
            return false;
        }
        
        while (availableMove.Count > 0)
        {
            if (count > 1000)
            {
                break;
            }
            startPosition.node = availableMove.Pop();
            if (_path[startPosition.node])
            {
                continue;
            }
            _path[startPosition.node] = true;
            if(FindPath(startPosition.node, targetPosition))
            {
                return true;
            }
        }

        return false;
    }

    private Stack<SquareHandler> AvailableDirection(SquareHandler center)
    {
        Stack<SquareHandler> direction = new Stack<SquareHandler>();

        //left border
        if (center.col - 1 < 0)
        {
            //Left-bot corner
            if (center.row + 1 >= Row)
            {
                //Top
                if (!_squareHandlers[center.col, center.row - 1].hasBall 
                    || _squareHandlers[center.col, center.row - 1].ball.state == EBallState.Queue)
                {
                    direction.Push(_squareHandlers[center.col, center.row - 1]);
                }
                
                //Right
                if (!_squareHandlers[center.col + 1, center.row].hasBall 
                    || _squareHandlers[center.col + 1, center.row].ball.state == EBallState.Queue)
                {
                    direction.Push(_squareHandlers[center.col + 1, center.row]);
                }
                return direction;
            }

            //Left-top corner
            if (center.row - 1 < 0)
            {
                //Bot
                if (!_squareHandlers[center.col, center.row + 1].hasBall 
                    || _squareHandlers[center.col, center.row + 1].ball.state == EBallState.Queue)
                {
                    direction.Push(_squareHandlers[center.col, center.row + 1]);
                }
                
                //Right
                if (!_squareHandlers[center.col + 1, center.row].hasBall 
                    || _squareHandlers[center.col + 1, center.row].ball.state == EBallState.Queue)
                {
                    direction.Push(_squareHandlers[center.col + 1, center.row]);
                }
                return direction;
            }

            //Right
            if (!_squareHandlers[center.col + 1, center.row].hasBall ||
                _squareHandlers[center.col + 1, center.row].ball.state == EBallState.Queue)
            {
                direction.Push(_squareHandlers[center.col + 1, center.row]);
            }  

            //Top
            if (!_squareHandlers[center.col, center.row - 1].hasBall ||
                _squareHandlers[center.col, center.row - 1].ball.state == EBallState.Queue)
            {
                direction.Push(_squareHandlers[center.col, center.row - 1]);
            }        
        
            //Bot
            if (!_squareHandlers[center.col, center.row + 1].hasBall ||
                _squareHandlers[center.col, center.row + 1].ball.state == EBallState.Queue)
            {
                direction.Push(_squareHandlers[center.col, center.row + 1]);
            }        
            return direction;
        }
        
        //Right border
        if (center.col + 1 >= Col)
        {
            //Right-bot corner
            if (center.row + 1 >= Row)
            {
                //Top
                if (!_squareHandlers[center.col, center.row - 1].hasBall ||
                    _squareHandlers[center.col, center.row - 1].ball.state == EBallState.Queue)
                {
                    direction.Push(_squareHandlers[center.col, center.row - 1]);
                }

                //Left
                if (!_squareHandlers[center.col - 1, center.row].hasBall ||
                    _squareHandlers[center.col - 1, center.row].ball.state == EBallState.Queue)
                {
                    direction.Push(_squareHandlers[center.col - 1, center.row]);
                }

                return direction;
            }

            //Right-top corner
            if (center.row - 1 < 0)
            {
                //Bot
                if (!_squareHandlers[center.col, center.row + 1].hasBall ||
                    _squareHandlers[center.col, center.row + 1].ball.state == EBallState.Queue)
                {
                    direction.Push(_squareHandlers[center.col, center.row + 1]);
                }

                //Left
                if (!_squareHandlers[center.col - 1, center.row].hasBall ||
                    _squareHandlers[center.col - 1, center.row].ball.state == EBallState.Queue)
                {
                    direction.Push(_squareHandlers[center.col - 1, center.row]);
                }

                return direction;
            }
            
            //Left
            if (!_squareHandlers[center.col - 1, center.row].hasBall ||
                _squareHandlers[center.col - 1, center.row].ball.state == EBallState.Queue)
            {
                direction.Push(_squareHandlers[center.col - 1, center.row]);
            }        
            
            //Top
            if (!_squareHandlers[center.col, center.row - 1].hasBall ||
                _squareHandlers[center.col, center.row - 1].ball.state == EBallState.Queue)
            {
                direction.Push(_squareHandlers[center.col, center.row - 1]);
            }        
        
            //Bot
            if (!_squareHandlers[center.col, center.row + 1].hasBall ||
                _squareHandlers[center.col, center.row + 1].ball.state == EBallState.Queue)
            {
                direction.Push(_squareHandlers[center.col, center.row + 1]);
            }        
            return direction;

        }
        
        //Top border
        if (center.row - 1 < 0)
        {
            //Right
            if (!_squareHandlers[center.col + 1, center.row].hasBall ||
                _squareHandlers[center.col + 1, center.row].ball.state == EBallState.Queue)
            {
                direction.Push(_squareHandlers[center.col + 1, center.row]);
            }

            //Left
            if (!_squareHandlers[center.col - 1, center.row].hasBall ||
                _squareHandlers[center.col - 1, center.row].ball.state == EBallState.Queue)
            {
                direction.Push(_squareHandlers[center.col - 1, center.row]);
            }

            //Bot
            if (!_squareHandlers[center.col, center.row + 1].hasBall ||
                _squareHandlers[center.col, center.row + 1].ball.state == EBallState.Queue)
            {
                direction.Push(_squareHandlers[center.col, center.row + 1]);
            }

            return direction;
        }
        
        //Bot border
        if (center.row + 1 >= Row)
        {
            //Right
            if (!_squareHandlers[center.col + 1, center.row].hasBall ||
                _squareHandlers[center.col + 1, center.row].ball.state == EBallState.Queue)
            {
                direction.Push(_squareHandlers[center.col + 1, center.row]);
            }  
        
            //Left
            if (!_squareHandlers[center.col - 1, center.row].hasBall ||
                _squareHandlers[center.col - 1, center.row].ball.state == EBallState.Queue)
            {
                direction.Push(_squareHandlers[center.col - 1, center.row]);
            }        
        
            //Top
            if (!_squareHandlers[center.col, center.row - 1].hasBall ||
                _squareHandlers[center.col, center.row - 1].ball.state == EBallState.Queue)
            {
                direction.Push(_squareHandlers[center.col, center.row - 1]);
            }        
        
            return direction;
        }

        //Right
        if (!_squareHandlers[center.col + 1, center.row].hasBall ||
            _squareHandlers[center.col + 1, center.row].ball.state == EBallState.Queue)
        {
            direction.Push(_squareHandlers[center.col + 1, center.row]);
        }  
        
        //Left
        if (!_squareHandlers[center.col - 1, center.row].hasBall ||
            _squareHandlers[center.col - 1, center.row].ball.state == EBallState.Queue)
        {
            direction.Push(_squareHandlers[center.col - 1, center.row]);
        }        
        
        //Top
        if (!_squareHandlers[center.col, center.row - 1].hasBall ||
            _squareHandlers[center.col, center.row - 1].ball.state == EBallState.Queue)
        {
            direction.Push(_squareHandlers[center.col, center.row - 1]);
        }        
        
        //Bot
        if (!_squareHandlers[center.col, center.row + 1].hasBall ||
            _squareHandlers[center.col, center.row + 1].ball.state == EBallState.Queue)
        {
            direction.Push(_squareHandlers[center.col, center.row + 1]);
        }        
        return direction;
    }

    #endregion
}
