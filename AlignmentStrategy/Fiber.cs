using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace AlignmentStrategy
{
    class Fiber
    {
        private List<Point3D> node=new List<Point3D>();

        public List<Point3D> Node
        {
            get { return node; }
            set { node = value; }
        }

        public Point3D this[int index]
        {
            get { return node[index]; }
            set { node[index] = value; }
        }
    }
}
