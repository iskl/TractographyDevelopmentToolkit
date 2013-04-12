using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace AlignmentStrategy
{
    class AlignmentTracking
    {
        private List<Fiber> fibers;
        private Voxel[, ,] voxels;

        public List<Fiber> Fibers
        {
            get { return fibers; }
            set { fibers = value; }
        }

        public AlignmentTracking(Voxel[, ,] v, Point3D startPoint)
        {
            voxels = v;
            fibers = new List<Fiber>();
            int index=0;
            int j=0;
            for(int k=1;k>=-1;k--)
            {
                for(int i=-1;i<=1;i++)
                {
                    fibers.Add(new Fiber());
                    fibers[index].Node.Add(new Point3D(startPoint.X+i,startPoint.Y+j,startPoint.Z+k));
                    index++;
                }
            }
        }

        public void Step()
        {
            for (int i = 0; i < fibers.Count; i++)
            {
                Point3D endNode = fibers[i].Node.Last<Point3D>();
                fibers[i].Node.Add(new Point3D(endNode.X, endNode.Y + 1, endNode.Z));
            }
        }
    }
}
