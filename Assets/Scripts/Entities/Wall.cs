using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFB.Map;


namespace UFB.Entities {
    
    public class Wall : MonoBehaviour
    {
        public TileSide Side { get; private set; } 

        void Start()
        {
            var tileSide = gameObject.name.Split("__");
            if (tileSide.Length < 2) throw new System.Exception("Wall name must be in the format of TileName__Side");
            var side = tileSide[1];

            switch(side)
            {
                case "Left":
                    Side = TileSide.Left;
                    break;
                case "Right":
                    Side = TileSide.Right;
                    break;
                case "Top":
                    Side = TileSide.Top;
                    break;
                case "Bottom":
                    Side = TileSide.Bottom;
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

}
}
