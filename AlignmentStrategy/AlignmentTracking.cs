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

        public Status Step()
        {
            List<Point3D> currentNode = new List<Point3D>(fibers.Count);
            List<Point3D> lastNode = new List<Point3D>(fibers.Count);
            List<Point3D> nextNode = new List<Point3D>(fibers.Count);
            List<int> nextType = new List<int>(fibers.Count);
            for (int i = 0; i < fibers.Count; i++)
            {
                currentNode.Add(fibers[i].Node.Last<Point3D>());

                int fiberLength=fibers[i].Node.Count;
                if (fiberLength == 1)
                {
                    lastNode.Add(fibers[i].Node[0]);//first step
                }
                else
                {
                    lastNode.Add(fibers[i].Node[fiberLength - 2]);
                }

                int gridX = (int)(currentNode[i].X + 0.5);
                int gridY = (int)(currentNode[i].Y + 0.5);
                int gridZ = (int)(currentNode[i].Z + 0.5);
                if (gridX < 0 || gridX > 99 || gridY < 0 || gridY > 99 || gridZ < 0 || gridZ > 29)
                {
                    return Status.ReachEdge;
                }
                int type = voxels[gridX, gridY, gridZ].Type;
                nextType.Add(type);
                if (type == 0)
                {
                    return Status.NoFiber;
                }
                List<Point3D> nextNodeCandidates =new List<Point3D>(type);
                List<double> angle = new List<double>(type);
                for (int itype = 0; itype < type; itype++)
                {
                    nextNodeCandidates.Add(new Point3D
                    (
                        currentNode[i].X + voxels[gridX, gridY, gridZ].Directions[itype].X,
                        currentNode[i].Y + voxels[gridX, gridY, gridZ].Directions[itype].Y,
                        currentNode[i].Z + voxels[gridX, gridY, gridZ].Directions[itype].Z
                    ));
                    angle.Add(corner(lastNode[i], currentNode[i], nextNodeCandidates[itype]));
                }
                int minIndex = angle.IndexOf(angle.Min());
                nextNode.Add(nextNodeCandidates[minIndex]);

            }
            List<Point3D> vectors = new List<Point3D>(fibers.Count);
            for (int i = 0; i < fibers.Count; i++)
            {
                vectors.Add(new Point3D(nextNode[i].X - currentNode[i].X, nextNode[i].Y - currentNode[i].Y, nextNode[i].Z - currentNode[i].Z));
            }

            while (true)
            {
                if (vectors.Count < 2)
                    break;
                
                double[,] matrix = new double[vectors.Count, vectors.Count];
                for (int i = 0; i < vectors.Count; i++)
                {
                    for (int j = 0; j < vectors.Count; j++)
                    {
                        if (i != j)
                            matrix[i, j] = corner(vectors[i], vectors[j]);
                        else
                            matrix[i, j] = 20000;
                    }
                }
                double minElement = 10000;
                int minIndexA = -1;
                int minIndexB = -1;
                for (int i = 0; i < vectors.Count; i++)
                {
                    for (int j = 0; j < vectors.Count; j++)
                    {
                        if (matrix[i, j] < minElement)
                        {
                            minElement = matrix[i, j];
                            minIndexA = i;
                            minIndexB = j;
                        }
                    }
                }
                if (minElement < 0.2)
                {
                    vectors[minIndexA] = new Point3D(vectors[minIndexA].X + vectors[minIndexB].X, vectors[minIndexA].Y + vectors[minIndexB].Y, vectors[minIndexA].Z + vectors[minIndexB].Z);
                    vectors.RemoveAt(minIndexB);
                }
                else 
                    break;
            }

            double maxLength = 0;
            int maxIndex = -1;
            for (int i = 0; i < vectors.Count; i++)
            {
                if (getLength(vectors[i]) > maxLength)
                {
                    maxLength = getLength(vectors[i]);
                    maxIndex = i;
                }
            }
            Point3D forward = vectors[maxIndex];

            double e = Math.Sqrt(forward.X * forward.X + forward.Y * forward.Y + forward.Z * forward.Z);
            forward.X /= e;
            forward.Y /= e;
            forward.Z /= e;


            for (int i = 0; i < fibers.Count; i++)
            {
                //fibers[i].Node.Add(nextNode[i]);//Streamline
                fibers[i].Node.Add(new Point3D(currentNode[i].X + forward.X, currentNode[i].Y + forward.Y, currentNode[i].Z + forward.Z));//Streamline
            }

            return Status.Tracking;
        }

        private double corner(Point3D lastNode, Point3D currentNode, Point3D nextNode)
        {
            Point3D a = new Point3D(currentNode.X - lastNode.X, currentNode.Y - lastNode.Y, currentNode.Z - lastNode.Z);
            Point3D b = new Point3D(nextNode.X - currentNode.X, nextNode.Y - currentNode.Y, nextNode.Z - currentNode.Z);
            double numerator = a.X * b.X + a.Y * b.Y + a.Z * b.Z;
            double denominator = Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z) * Math.Sqrt(b.X * b.X + b.Y * b.Y + b.Z * b.Z);
            return Math.Acos(numerator / denominator);
        }

        private double corner(Point3D a, Point3D b)
        {
            double numerator = a.X * b.X + a.Y * b.Y + a.Z * b.Z;
            double denominator = Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z) * Math.Sqrt(b.X * b.X + b.Y * b.Y + b.Z * b.Z);
            return Math.Acos(numerator / denominator);
        }

        private double getLength(Point3D a)
        {
            return Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
        }
    }
}
