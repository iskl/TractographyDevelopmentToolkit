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
            int index = 0;
            int j = 0;
            for (int k = 1; k >= -1; k--)
            {
                for (int i = -1; i <= 1; i++)
                {
                    fibers.Add(new Fiber());
                    fibers[index].Node.Add(startPoint.Add(i, j, k));
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

                int fiberLength = fibers[i].Node.Count;
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
                List<Point3D> nextNodeCandidates = new List<Point3D>(type);
                List<double> angle = new List<double>(type);
                for (int itype = 0; itype < type; itype++)
                {
                    nextNodeCandidates.Add
                    (
                        currentNode[i].Add(voxels[gridX, gridY, gridZ].Directions[itype])
                    );
                    angle.Add(corner(lastNode[i], currentNode[i], nextNodeCandidates[itype]));
                }
                int minIndex = angle.IndexOf(angle.Min());
                nextNode.Add(nextNodeCandidates[minIndex]);

            }
            List<Point3D> vectors = new List<Point3D>(fibers.Count);
            for (int i = 0; i < fibers.Count; i++)
            {
                vectors.Add(nextNode[i].Substract(currentNode[i]));
            }

            double threshold = 0.2;
            switch (nextType[5])
            {
                case 0:
                case 1:
                    threshold = 0.2;
                    break;
                case 2:
                default:
                    threshold = 0.1;
                    break;
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
                if (minElement < threshold)
                {
                    vectors[minIndexA] = vectors[minIndexA].Add(vectors[minIndexB]);
                    vectors.RemoveAt(minIndexB);
                }
                else
                    break;
            }

            double maxLength = 0;
            int maxIndex = -1;
            for (int i = 0; i < vectors.Count; i++)
            {
                if (vectors[i].GetLength() > maxLength)
                {
                    maxLength = vectors[i].GetLength();
                    maxIndex = i;
                }
            }
            Point3D forward = vectors[maxIndex];
            forward = forward.GetNormal();
            Point3D center = currentNode[5].Add(forward);

            for (int i = 0; i < fibers.Count; i++)
            {
                Point3D adjustedForward = new Point3D(0, 0, 0);
                //Streamline
                //fibers[i].Node.Add(nextNode[i]);

                //AlignmentTracking 1
                //fibers[i].Node.Add(currentNode[i].Add(forward));

                //AlignmentTracking 2
                double A = forward.X; double B = forward.Y; double C = forward.Z;
                double X = center.X; double Y = center.Y; double Z = center.Z;
                double D = (-1) * (A * X + B * Y + C * Z);//plane: Ax+By+Cz+D=0

                double lastDelta = 10000;
                while (true)
                {
                    adjustedForward = adjustedForward.Add(forward.Multiply(0.05));
                    double delta = Math.Abs(
                    (
                        currentNode[i].Add(adjustedForward).X * A
                        +
                        currentNode[i].Add(adjustedForward).Y * B
                        +
                        currentNode[i].Add(adjustedForward).Z * C
                        +
                        D
                    )
                    /
                    Math.Sqrt(A * A + B * B + C * C));
                    if (delta > lastDelta)
                        break;
                    lastDelta = delta;
                }
                fibers[i].Node.Add(currentNode[i].Add(adjustedForward));
            }

            return Status.Tracking;
        }

        private double corner(Point3D lastNode, Point3D currentNode, Point3D nextNode)
        {
            Point3D a = currentNode.Substract(lastNode);
            Point3D b = nextNode.Substract(currentNode);
            return corner(a, b);
        }

        private double corner(Point3D a, Point3D b)
        {
            double numerator = a.X * b.X + a.Y * b.Y + a.Z * b.Z;
            double denominator = Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z) * Math.Sqrt(b.X * b.X + b.Y * b.Y + b.Z * b.Z);
            return Math.Acos(numerator / denominator);
        }
    }
}
