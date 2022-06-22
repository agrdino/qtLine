using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Prefab.Popup.NotiPopup;
using _Prefab.Popup.YesNoPopup;
using _Scripts.Handler;
using _Scripts.System;
using DG.Tweening;
using Scene.GameScene;
using UnityEngine;
using static qtHelper;
using Random = UnityEngine.Random;

public class GameManager : qtSingleton<GameManager>
{
    #region Controller
    
    #endregion

    #region ----- VARIABLE -----

    public int level
    {
        get;
        private set;
    }

    private float _startTime;
    public float playTime
    {
        get;
        private set;
    }
    
    private const int Col = 9;
    private const int Row = 9;
    
    private GameObject _board;

    private SquareHandler[,] _squareHandlers;

    public List<SquareHandler> squareForCheck
    {
        get;
        private set;
    }
    private List<SquareHandler> _ballQueue;
    private List<List<SquareHandler>> _row;
    private Dictionary<SquareHandler, bool> _path;

    private Stack<Step> _steps;

    private bool _isSetUp;

    public SquareHandler selectedBall
    {
        get;
        private set;
    }

    public int bonusBall
    {
        get;
        private set;
    }
    public int score { get; private set; }

    private Coroutine _initCoroutine;
    private Coroutine _moveCoroutine;
    
    #endregion

    public void StartGame()
    {
        _startTime = Time.time;
        bonusBall = -1;
        Initialize();
    }

    private void Update()
    {
        if (IsEnd)
        {
            return;
        }

        playTime = Time.time - _startTime;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            DataManager.Instance.SaveData(true);
        }
        else
        {
            DataManager.Instance.playerData.isSaving = false;
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            DataManager.Instance.SaveData(true);
        }
        else
        {
            DataManager.Instance.playerData.isSaving = false;
        }
    }

    #region ----- PUBLIC FUNCTION -----
    
    public void TargetSelect(SquareHandler target)
    {
        if (_isBonusBall)
        {
            CacheStep();
            _isBonusBall = false;
            if (target.ball != null)
            {
                target.ball.gameObject.SetActive(false);
            }
            var bonusBall = qtPooling.Instance.Spawn<BallHandler>(DataManager.Instance.Ball.name,
                DataManager.Instance.Ball, UIManager.Instance.currentScene.transform);
            bonusBall.Initialize(this.bonusBall, EBallState.Normal);
            target.ball = bonusBall;
            bonusBall.transform.position = target.transform.position;
            ScoreCalculate();
            ((GameScene)UIManager.Instance.currentScene).BonusBall(-1, false);
            this.bonusBall = -1;
            return;
        }
        if (selectedBall == null)
        {
            if (target.ball == null || target.ball.state == EBallState.Queue)
            {
                return;
            }
            
            selectedBall = target;
            selectedBall.ball.Select();
            return;
        }
        
        if (selectedBall != null && selectedBall.Equals(target))
        {
            selectedBall.ball.Select();
            selectedBall = null;
            return;
        }

        if (target.ball != null)
        {
            if (target.ball.state == EBallState.Normal)
            {
                selectedBall.ball.Select();
                selectedBall = target;
                selectedBall.ball.Select();
                return;
            }
            
            // var availableCount = squareForCheck.FindAll(square => !square.hasBall).Count;
            // if (availableCount != 0)
            // {
            //     SquareHandler square = null;
            //     do
            //     {
            //         square = _squareHandlers[Random.Range(0, Col), Random.Range(0, Row)];
            //     } while (square.hasBall);
            //
            //     var ball = target.ball;
            //     _ballQueue.Remove(target);
            //     _ballQueue.Add(square);
            //
            //     square.ball = ball;
            //     target.ball = null;
            //     ball.transform.position = square.transform.position;
            // }
            // else
            // {
            //     _ballQueue.ForEach(square => square.ball.Grow());
            //     var tempBall = target.ball;
            //     target.ball = selectedBall.ball;
            //     target.ball.transform.DOMove(selectedBall.transform.position, 0.025f);
            //     selectedBall.ball = tempBall;
            //     tempBall.transform.position = selectedBall.transform.position;
            //     if (isEnd)
            //     {
            //         DataManager.Instance.SaveData(false);
            //         IsEnd = true;
            //         ((LosePopup) UIManager.Instance.ShowPopup(qtScene.EPopup.Lose)).Initialize(RestartGame, delegate
            //         {
            //             ClearGame();
            //             UIManager.Instance.ShowScene(qtScene.EScene.MainMenu);
            //         });
            //     }
            //     else
            //     {
            //         NewTurn();
            //     }
            // }
        }

        if (selectedBall.ball.type == EBallType.Ghost)
        {
            CacheStep();
            var center = _squareHandlers[selectedBall.col, target.row];
            selectedBall.node = center;
            center.node = target;
            target.ball = selectedBall.ball;
            _moveCoroutine = StartCoroutine(Move(selectedBall));
            selectedBall.ball.Select();
            selectedBall.ball = null;
            selectedBall = null;
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
                CacheStep();
                if (target.ball != null)
                {
                    if (target.ball.state == EBallState.Queue)
                    {
                        var availableCount = squareForCheck.FindAll(square => !square.hasBall).Count;
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
                        else
                        {
                            _ballQueue.ForEach(square => square.ball.Grow());
                            var tempBall = target.ball;
                            target.ball = selectedBall.ball;
                            target.ball.transform.DOMove(selectedBall.transform.position, 0.025f);
                            selectedBall.ball = tempBall;
                            tempBall.transform.position = selectedBall.transform.position;
                            if (isEnd)
                            {
                                DataManager.Instance.SaveData(false);
                                IsEnd = true;
                                ((LosePopup) UIManager.Instance.ShowPopup(qtScene.EPopup.Lose)).Initialize(RestartGame, delegate
                                {
                                    ClearGame();
                                    UIManager.Instance.ShowScene(qtScene.EScene.MainMenu);
                                });
                            }
                            else
                            {
                                NewTurn();
                            }
                        }
                    }
                }

                target.ball = selectedBall.ball;
                _moveCoroutine = StartCoroutine(Move(selectedBall));
                selectedBall.ball.Select();
                selectedBall.ball = null;
                selectedBall = null;
            }
            else
            {
                Debug.LogError("Cant find path!");
                AudioManager.Instance.PlaySfx("wrong_sfx");
                selectedBall.ball.Select();
                selectedBall = null;
            }
        }
    }
    
    public void RestartGame()
    {
        ClearGame();
        StartGame();
    }

    public void Undo()
    {
        if (!DataManager.Instance.defaulLevel.undoAllow)
        {
            return;
        }
        if (_steps.Count == 0)
        {
            return;
        }

        var step = _steps.Pop();
        foreach (var square in _squareHandlers)
        {
            if (square.hasBall)
            {
                square.ball.gameObject.SetActive(false);
            }
            square.ball = null;
        }
        _ballQueue.Clear();
        score = step.score;
        step.balls.ForEach(ball =>
        {
            var newBall = qtPooling.Instance.Spawn<BallHandler>(DataManager.Instance.Ball.name, DataManager.Instance.Ball, UIManager.Instance.currentScene.transform);
            newBall.Initialize(ball.color, ball.state, ball.type);
            _squareHandlers[ball.col, ball.row].ball = newBall;
            if (ball.state == EBallState.Queue)
            {
                _ballQueue.Add(_squareHandlers[ball.col, ball.row]);
            }
            newBall.transform.position = _squareHandlers[ball.col, ball.row].transform.position;
        });
        
        ((GameScene) UIManager.Instance.currentScene).UpdateUI();
        ((GameScene) UIManager.Instance.currentScene).UpdateQueue(_ballQueue);
    }

    private bool _isBonusBall;
    public void BonusBall()
    {
        if (bonusBall == -1)
        {
            return;
        }
        
        if (selectedBall != null)
        {
            selectedBall.ball.Select();
            selectedBall = null;
        }

        _isBonusBall = !_isBonusBall;
    }
    
    public void ClearGame(bool isSave = false)
    {
        DataManager.Instance.SaveData(isSave);
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
        }
        
        if (_initCoroutine != null)
        {
            StopCoroutine(_initCoroutine);
        }
        foreach (var square in _squareHandlers)
        {
            if (square.hasBall)
            {
                square.ball.gameObject.SetActive(false);
            }
            square.ball = null;
        }

        _ballQueue.ForEach(square =>
        {
            square.ball = null;
        });
        _ballQueue.Clear();
        qtPooling.Instance.InActiveAll(DataManager.Instance.Ball.name);
    }

    #endregion

    #region ----- PRIVATE FUNCTION -----
    
    private void Initialize()
    {
        score = 0;
        level = 3;
        IsEnd = false;

        if (!_isSetUp)
        {
            SetUpBoard();
        }

        _steps.Clear();
        if (DataManager.Instance.playerData.isSaving)
        {
            _initCoroutine = StartCoroutine(ContinueGame());
            DataManager.Instance.playerData.isSaving = false;
            return;
        }
        else
        {
            _initCoroutine = StartCoroutine(InitBall());
        }
        ((GameScene) UIManager.Instance.currentScene).UpdateUI();
        ((GameScene) UIManager.Instance.currentScene).UpdateQueue(_ballQueue);
    }

    private void SetUpBoard()
    {
        _isSetUp = true;

        _board = FindObjectWithPath(UIManager.Instance.currentScene.gameObject, "Board");
        _squareHandlers ??= new SquareHandler[Col, Row];
        squareForCheck ??= new List<SquareHandler>();
        _ballQueue ??= new List<SquareHandler>();
        _row ??= new List<List<SquareHandler>>();
        _path ??= new Dictionary<SquareHandler, bool>();
        _steps ??= new Stack<Step>();

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if ((row + col) % 2 == 0)
                {
                    var tempSquare = qtPooling.Instance.Spawn<SquareHandler>(DataManager.Instance.SquareLight.name,
                        DataManager.Instance.SquareLight, _board.transform);
                    tempSquare.Initialize(col, row);
                    tempSquare.gameObject.SetActive(true);
                    _squareHandlers[col, row] = tempSquare;
                    squareForCheck.Add(tempSquare);
                    _path.Add(tempSquare, false);
                }
                else
                {
                    var tempSquare = qtPooling.Instance.Spawn<SquareHandler>(DataManager.Instance.SquareDark.name,
                        DataManager.Instance.SquareDark, _board.transform);
                    tempSquare.Initialize(col, row);
                    _squareHandlers[col, row] = tempSquare;
                    squareForCheck.Add(tempSquare);
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

    private IEnumerator ContinueGame()
    {
        var playerData = DataManager.Instance.playerData;
        score = playerData.score;
        _startTime = Time.time - playerData.playTime;
        yield return new WaitForSeconds(0.25f);
        foreach (var square in _squareHandlers)
        {
            if (square.hasBall)
            {
                square.ball.gameObject.SetActive(false);
            }
            square.ball = null;
        }
        playerData.balls.ForEach(ball =>
        {
            var newBall = qtPooling.Instance.Spawn<BallHandler>(DataManager.Instance.Ball.name, DataManager.Instance.Ball, UIManager.Instance.currentScene.transform);
            newBall.Initialize(ball.color, ball.state, ball.type);
            _squareHandlers[ball.col, ball.row].ball = newBall;
            if (ball.state == EBallState.Queue)
            {
                _ballQueue.Add(_squareHandlers[ball.col, ball.row]);
            }
            newBall.transform.position = _squareHandlers[ball.col, ball.row].transform.position;
        });
    }
    
    private IEnumerator InitBall()
    {
        var delayCoroutine = new WaitForSeconds(0.25f);
        yield return delayCoroutine;
        for (int i = 0; i < DataManager.Instance.defaulLevel.startBalls; i++)
        {
            SquareHandler square = null;
            do
            {
                square = _squareHandlers[Random.Range(0, Col), Random.Range(0, Row)];
            } while (square.hasBall);
            var randomColor = Random.Range(0, DataManager.Instance.colorBank.Length);
            
            var ball = qtPooling.Instance.Spawn<BallHandler>(DataManager.Instance.Ball.name, DataManager.Instance.Ball, UIManager.Instance.currentScene.transform);
            ball.Initialize(randomColor, EBallState.Normal);
            ball.transform.localScale = 0.3f * Vector3.one;
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
        var moveCoroutine = new WaitForSeconds(0.025f);
        while (temp.node != null)
        {
            ball.transform.DOMove(temp.node.transform.position, 0.025f);
            temp = temp.node;
            yield return moveCoroutine;
        }
        AudioManager.Instance.PlaySfx("complete_move_sfx");
        ball.transform.position = temp.transform.position;
        _ballQueue.ForEach(square =>
        {
            square.ball.Grow();
        });
        _ballQueue.Clear();
            
        ScoreCalculate();

        if (isEnd)
        {
            DataManager.Instance.SaveData(false);
            IsEnd = true;
            ((LosePopup) UIManager.Instance.ShowPopup(qtScene.EPopup.Lose)).Initialize(RestartGame, delegate
            {
                ClearGame();
                UIManager.Instance.ShowScene(qtScene.EScene.MainMenu);
            });
            yield break;
        }
        
        if (DataManager.Instance.defaulLevel.hasBonusBall)
        {
            if (Random.Range(0, 100) < DataManager.Instance.defaulLevel.ratioActiveBonusBall)
            {
                if (bonusBall == -1)
                {
                    bonusBall = Random.Range(0, level);
                    ((GameScene)UIManager.Instance.currentScene).BonusBall(bonusBall);
                }
            }
        }
        
        NewTurn();
    }

    private void CacheStep()
    {
        var step = new Step();
        step.score = score;
        foreach (var square in squareForCheck)
        {
            if (square.ball != null)
            {
                step.balls.Add(new Ball()
                {
                    col = square.col,
                    row = square.row,
                    color = square.ball.color,
                    state = square.ball.state,
                    type = square.ball.type
                });
            }
        }
        _steps.Push(step);
    }

    private void NewTurn()
    {
        var availableCount = squareForCheck.FindAll(square => !square.hasBall).Count;
        if (availableCount >= DataManager.Instance.defaulLevel.queueBalls)
        {
            availableCount = DataManager.Instance.defaulLevel.queueBalls;
        }
        for (int i = 0; i < availableCount; i++)
        {
            SquareHandler square = null;
            do
            {
                square = _squareHandlers[Random.Range(0, Col), Random.Range(0, Row)];
            } while (square.hasBall);
            
            var ball = qtPooling.Instance.Spawn<BallHandler>(DataManager.Instance.Ball.name, DataManager.Instance.Ball, UIManager.Instance.currentScene.transform);
            
            var randomColor = Random.Range(0, DataManager.Instance.colorBank.Length);
            int type = 0;
            if (level >= 5)
            {
                type = Random.Range(0, 2);
            }
            ball.Initialize(randomColor, EBallState.Queue, (EBallType)type);
            ball.transform.localScale = 0.3f * Vector3.one;

            ball.transform.position = square.transform.position;
            square.ball = ball;
            _ballQueue.Add(square);
        }

        ((GameScene)UIManager.Instance.currentScene).UpdateQueue(_ballQueue);
    }

    public bool IsEnd;
    private bool isEnd => squareForCheck.Find(square => !square.hasBall || square.ball.state == EBallState.Queue) == null;

    private void ScoreCalculate()
    {
        //Add bonus
        foreach (var row in _row)
        {
            score += ScoreCheck(row);
        }

        DataManager.Instance.highScore = score;

        var tempScore = squareForCheck.FindAll(square => square.isScore).Count;
        if (tempScore > 0)
        {
            AudioManager.Instance.PlaySfx("match_sfx");
            score += squareForCheck.FindAll(square => square.isScore).Count;
        }

        level = 3 + score / 50;
        level = Math.Clamp(level, 3, DataManager.Instance.colorBank.Length);
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
    
    private bool FindPath(SquareHandler startPosition, SquareHandler targetPosition)
    {
        if (startPosition.Equals(targetPosition))
        {
            startPosition.node = null;
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
            _path[startPosition] = true;
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
                    || _squareHandlers[center.col, center.row + 1].ball.state == EBallState.Queue
                    || _squareHandlers[center.col, center.row + 1].Equals(selectedBall))
                {
                    direction.Push(_squareHandlers[center.col, center.row + 1]);
                }
                
                //Right
                if (!_squareHandlers[center.col + 1, center.row].hasBall 
                    || _squareHandlers[center.col + 1, center.row].ball.state == EBallState.Queue
                    || _squareHandlers[center.col + 1, center.row].Equals(selectedBall))
                {
                    direction.Push(_squareHandlers[center.col + 1, center.row]);
                }
                return direction;
            }

            //Right
            if (!_squareHandlers[center.col + 1, center.row].hasBall ||
                _squareHandlers[center.col + 1, center.row].ball.state == EBallState.Queue
                || _squareHandlers[center.col + 1, center.row].Equals(selectedBall))
            {
                direction.Push(_squareHandlers[center.col + 1, center.row]);
            }  

            //Top
            if (!_squareHandlers[center.col, center.row - 1].hasBall 
                || _squareHandlers[center.col, center.row - 1].ball.state == EBallState.Queue
                || _squareHandlers[center.col, center.row - 1].Equals(selectedBall))
            {
                direction.Push(_squareHandlers[center.col, center.row - 1]);
            }        
        
            //Bot
            if (!_squareHandlers[center.col, center.row + 1].hasBall
                || _squareHandlers[center.col, center.row + 1].ball.state == EBallState.Queue
                || _squareHandlers[center.col, center.row + 1].Equals(selectedBall))
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
                    _squareHandlers[center.col, center.row - 1].ball.state == EBallState.Queue
                    || _squareHandlers[center.col, center.row - 1].Equals(selectedBall))
                {
                    direction.Push(_squareHandlers[center.col, center.row - 1]);
                }

                //Left
                if (!_squareHandlers[center.col - 1, center.row].hasBall ||
                    _squareHandlers[center.col - 1, center.row].ball.state == EBallState.Queue
                    || _squareHandlers[center.col - 1, center.row].Equals(selectedBall))
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
                    _squareHandlers[center.col, center.row + 1].ball.state == EBallState.Queue
                    || _squareHandlers[center.col, center.row + 1].Equals(selectedBall))
                {
                    direction.Push(_squareHandlers[center.col, center.row + 1]);
                }

                //Left
                if (!_squareHandlers[center.col - 1, center.row].hasBall ||
                    _squareHandlers[center.col - 1, center.row].ball.state == EBallState.Queue
                    || _squareHandlers[center.col - 1, center.row].Equals(selectedBall))
                {
                    direction.Push(_squareHandlers[center.col - 1, center.row]);
                }

                return direction;
            }
            
            //Left
            if (!_squareHandlers[center.col - 1, center.row].hasBall ||
                _squareHandlers[center.col - 1, center.row].ball.state == EBallState.Queue
                || _squareHandlers[center.col - 1, center.row].Equals(selectedBall))
            {
                direction.Push(_squareHandlers[center.col - 1, center.row]);
            }        
            
            //Top
            if (!_squareHandlers[center.col, center.row - 1].hasBall ||
                _squareHandlers[center.col, center.row - 1].ball.state == EBallState.Queue
                || _squareHandlers[center.col, center.row - 1].Equals(selectedBall))
            {
                direction.Push(_squareHandlers[center.col, center.row - 1]);
            }        
        
            //Bot
            if (!_squareHandlers[center.col, center.row + 1].hasBall ||
                _squareHandlers[center.col, center.row + 1].ball.state == EBallState.Queue
                || _squareHandlers[center.col, center.row + 1].Equals(selectedBall))
            {
                direction.Push(_squareHandlers[center.col, center.row + 1]);
            }        
            return direction;

        }
        
        //Top border
        if (center.row - 1 < 0)
        {
            //Right
            if (!_squareHandlers[center.col + 1, center.row].hasBall 
                || _squareHandlers[center.col + 1, center.row].ball.state == EBallState.Queue
                || _squareHandlers[center.col + 1, center.row].Equals(selectedBall))
            {
                direction.Push(_squareHandlers[center.col + 1, center.row]);
            }

            //Left
            if (!_squareHandlers[center.col - 1, center.row].hasBall 
                || _squareHandlers[center.col - 1, center.row].ball.state == EBallState.Queue
                || _squareHandlers[center.col - 1, center.row].Equals(selectedBall))
            {
                direction.Push(_squareHandlers[center.col - 1, center.row]);
            }

            //Bot
            if (!_squareHandlers[center.col, center.row + 1].hasBall 
                || _squareHandlers[center.col, center.row + 1].ball.state == EBallState.Queue
                || _squareHandlers[center.col, center.row + 1].Equals(selectedBall))
            {
                direction.Push(_squareHandlers[center.col, center.row + 1]);
            }

            return direction;
        }
        
        //Bot border
        if (center.row + 1 >= Row)
        {
            //Right
            if (!_squareHandlers[center.col + 1, center.row].hasBall 
                || _squareHandlers[center.col + 1, center.row].ball.state == EBallState.Queue
                || _squareHandlers[center.col + 1, center.row].Equals(selectedBall))
            {
                direction.Push(_squareHandlers[center.col + 1, center.row]);
            }  
        
            //Left
            if (!_squareHandlers[center.col - 1, center.row].hasBall 
                || _squareHandlers[center.col - 1, center.row].ball.state == EBallState.Queue
                || _squareHandlers[center.col - 1, center.row].Equals(selectedBall))
            {
                direction.Push(_squareHandlers[center.col - 1, center.row]);
            }        
        
            //Top
            if (!_squareHandlers[center.col, center.row - 1].hasBall
                || _squareHandlers[center.col, center.row - 1].ball.state == EBallState.Queue
                || _squareHandlers[center.col, center.row - 1].Equals(selectedBall))
            {
                direction.Push(_squareHandlers[center.col, center.row - 1]);
            }        
        
            return direction;
        }

        //Right
        if (!_squareHandlers[center.col + 1, center.row].hasBall 
            || _squareHandlers[center.col + 1, center.row].ball.state == EBallState.Queue
            || _squareHandlers[center.col + 1, center.row].Equals(selectedBall))
        {
            direction.Push(_squareHandlers[center.col + 1, center.row]);
        }  
        
        //Left
        if (!_squareHandlers[center.col - 1, center.row].hasBall 
            || _squareHandlers[center.col - 1, center.row].ball.state == EBallState.Queue
            || _squareHandlers[center.col - 1, center.row].Equals(selectedBall))
        {
            direction.Push(_squareHandlers[center.col - 1, center.row]);
        }        
        
        //Top
        if (!_squareHandlers[center.col, center.row - 1].hasBall 
            || _squareHandlers[center.col, center.row - 1].ball.state == EBallState.Queue
            || _squareHandlers[center.col, center.row - 1].Equals(selectedBall))
        {
            direction.Push(_squareHandlers[center.col, center.row - 1]);
        }        
        
        //Bot
        if (!_squareHandlers[center.col, center.row + 1].hasBall 
            || _squareHandlers[center.col, center.row + 1].ball.state == EBallState.Queue
            || _squareHandlers[center.col, center.row + 1].Equals(selectedBall))
        {
            direction.Push(_squareHandlers[center.col, center.row + 1]);
        }        
        return direction;
    }

    #endregion
}
