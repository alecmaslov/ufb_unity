using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFB.Entities;
using System.Linq;
using UFB.Map;

namespace UFB.Development {
    
    public class BoardSpawner : MonoBehaviour
    {

        [Header("Entities")]
        [SerializeField] private GameObject _tilePrefab;

        [SerializeField] private string _mapName = "kraken";

        private GameBoard _gameBoard;

        void Start()
        {
            _gameBoard = GetComponent<GameBoard>();
            _gameBoard.SpawnBoard(_mapName);
        }
    }

}

