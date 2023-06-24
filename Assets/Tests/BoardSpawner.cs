using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFB.Entities;
using System.Linq;
using UFB.Map;

namespace UFB.Development {
    
    public class BoardSpawner : MonoBehaviour
    {
        [SerializeField] private string _mapName = "kraken";
     
        void Start()
        {
            var gameBoard = GetComponent<GameBoard>();
            gameBoard.SpawnBoard(_mapName);
        }
    }

}

