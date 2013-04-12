using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace AlignmentStrategy
{
    class Voxel
    {
        private List<Point3D> directions=new List<Point3D>();
        private int type=0;

        public List<Point3D> Directions
        {
            get { return directions; }
            set { directions = value; }
        }

        public int Type
        {
            get { return type; }
            set { type = value; }
        }
    }
}
