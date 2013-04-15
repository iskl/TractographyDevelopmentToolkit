using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace AlignmentStrategy
{
    class CrossData : Data
    {
        public CrossData(double angle, double error)
        {
            voxels = new Voxel[100, 100, 30];
            angle = angle / 180.0 * Math.PI;
            double k1 = Math.Tan(angle);

            Random rdm = new Random();

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    for (int k = 0; k < 30; k++)
                    {
                        voxels[i, j, k] = new Voxel();
                        if
                        (
                            j <= k1 * (i - 50) + 50 + 5 / Math.Cos(angle)
                            &&
                            j >= k1 * (i - 50) + 50 - 5 / Math.Cos(angle)
                        )
                        {
                            voxels[i, j, k].Type++;

                            double x = Math.Cos(angle) + ((double)rdm.Next(1, 200) / 100.0 - 1.0) * error;
                            double y = Math.Sin(angle) + ((double)rdm.Next(1, 200) / 100.0 - 1.0) * error;
                            double z = 0 + ((double)rdm.Next(1, 200) / 100.0 - 1.0) * error;
                            double e = Math.Sqrt(x * x + y * y + z * z);

                            voxels[i, j, k].Directions.Add(new Point3D(x / e, y / e, z / e));
                        }

                        if (i <= 50 + 5 && i >= 50 - 5)
                        {
                            voxels[i, j, k].Type++;

                            double x = 0 + ((double)rdm.Next(1, 200) / 100.0 - 1.0) * error;
                            double y = 1 + ((double)rdm.Next(1, 200) / 100.0 - 1.0) * error;
                            double z = 0 + ((double)rdm.Next(1, 200) / 100.0 - 1.0) * error;
                            double e = Math.Sqrt(x * x + y * y + z * z);

                            voxels[i, j, k].Directions.Add(new Point3D(x / e, y / e, z / e));
                        }
                    }
                }
            }
        }
    }
}
