using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    public class MazeWall
    {
        public WallFace face;
        public MazeCell cell1;
        public MazeCell cell2;
        public bool isEdge;

        public GameObject wallObject;

        public bool hasBeenBuilt;

        public bool isCell1Visited()
        {
            return cell1.hasVisited;
        }

        public bool isCell2Visited()
        {
            if (cell2 == null)
                return false;

            return cell1.hasVisited;
        }


    }
}
