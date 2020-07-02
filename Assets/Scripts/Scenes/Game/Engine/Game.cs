﻿using CardGame.Events;
using CardGame.Players;
using CardGame.Utils;
using Force.DeepCloner;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame
{
    public enum GameState
    {
        NotStarted,
        InProgress,
        Ended
    }

    [Serializable]
    public class GameSettings
    {
        public int MaxTimePerRound = 35;
        public int InitialCardCount = 3;
        public int CardIncrementsPerTurn = 1;
        public int MaxUnitsOnField = 4; 
    }

    public class Game : IDisposable
    {
        protected List<Player> _players;
        protected int _currentPlayerTurn, _roundCount, _turnCount;
        protected InterceptorService _interceptorService;
        protected GameState _gameState;
        protected Randomizer _randomizer;
        protected GameSettings _gameSettings;


        public Action EventGameStart;
        public Action EventGameEnd;

        public Game(string seed, GameSettings settings = null)
        {
            _interceptorService = new InterceptorService();
            _gameState = GameState.NotStarted;
            _gameSettings = settings != null ? settings : new GameSettings();

            _roundCount = 1;
            _turnCount = 0;
            _players = new List<Player>();
            _randomizer = new Randomizer(seed);
            ClearEvents();
        }

        public InterceptorService InterceptorService => _interceptorService;
        public Player CurrentPlayerTurn => _players[_currentPlayerTurn];
        public GameState GameState => _gameState;
        public Randomizer Randomizer => _randomizer;
        public List<Player> Players => _players;
        public GameSettings Settings => _gameSettings;
        public bool IsStarted => _gameState == GameState.InProgress;

        public void ClearEvents()
        {
            EventGameEnd = () => { };
            EventGameStart = () => { };
        }

        public void AddPlayer(Player player)
        {
            _players.Add(player);
        }

        public void SetTurn(int playerIndex)
        {
            _currentPlayerTurn = playerIndex;
            CurrentPlayerTurn.StartTurn();
        }

        public Player GetOpponent(Player player)
        {
            int playerIndex = _players.IndexOf(player);
            return _players[(++(playerIndex) % _players.Count)];
        }

        public Player GetPlayer(string instanceID)
        {
            foreach (Player player in _players)
            {
                if (player.InstanceID == instanceID) return player;
            }
            return null;
        }

        public bool NextTurn()
        {
            if (_gameState == GameState.Ended) return false;
            _turnCount++;
            if (_turnCount % _players.Count == 0)
            {
                _roundCount = (_turnCount / _players.Count) + 1;
                Debug.Log("Round: " + _roundCount);
            }
            _currentPlayerTurn = ++_currentPlayerTurn % _players.Count;
            SetTurn(_currentPlayerTurn);
            return true;
        }

        public void ReadyPlayers()
        {
            foreach (Player player in _players)
            {
                player.Ready();
            }
        }

        public void StartGame()
        {
            if (_gameState != GameState.NotStarted || _players.Count <= 0) return;


            _gameState = GameState.InProgress;

            _turnCount = 0;

            EventGameStart();

            RollTurn();
        }

        public void EndGame(bool forceEnd = false)
        {
            if (_gameState == GameState.Ended) return;

            _interceptorService.ClearInterceptors();

            Player winner = null;
            int deathCount = 0;
            foreach (Player player in _players)
            {
                if (player.Health <= 0)
                {
                    deathCount++;
                    if (winner == null)
                    {
                        //determine winner
                        winner = GetOpponent(player);
                    }
                }
                player.EndTurn(true);
            }

            if (!forceEnd)
            {
                bool draw = deathCount >= _players.Count;

                if (!draw)
                {
                    Debug.Log($"Game ended! Winner: {winner.Name}, total rounds: {_roundCount}");
                }
                else
                {
                    Debug.Log("Game is a draw!");
                }
            }

            _gameState = GameState.Ended;
            EventGameEnd();
        }

        public Game ToCopy(bool simulated = true)
        {
            Game copy = this.DeepClone();
            copy.ClearEvents();
            copy.InterceptorService.IsSimulated = simulated;

            return copy;
        }

        private void RollTurn()
        {
            SetTurn(_randomizer.RandomInt(0, _players.Count));
        }

        public void Dispose()
        {
            _interceptorService = new InterceptorService();
            ClearEvents();
            EndGame(true);
        }
    }
}
