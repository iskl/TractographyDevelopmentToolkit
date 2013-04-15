using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace AlignmentStrategy
{
    class RingData : Data
    {
        public RingData(double error)
        {
            voxels = new Voxel[100, 100, 30];

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
                            ((double)i - 50.0) * ((double)i - 50.0) + ((double)j - 50.0) * ((double)j - 50.0) < 30.0 * 30.0
                            &&
                            ((double)i - 50.0) * ((double)i - 50.0) + ((double)j - 50.0) * ((double)j - 50.0) > 20.0 * 20.0
                        )
                        {
                            voxels[i, j, k].Type++;

                            double k1 = Math.Abs(((double)j - 50.0) / ((double)i - 50.0));

                            double E;
                            double angle;
                            if (i - 50 < 0 && j - 50 > 0)
                            {
                                E = 1;
                                angle = Math.PI / 2 - Math.Atan(k1);
                            }
                            else if (i - 50 > 0 && j - 50 > 0)
                            {
                                E = 1;
                                angle = (-1) * (Math.PI / 2 - Math.Atan(k1));
                            }
                            else if (i - 50 > 0 && j - 50 < 0)
                            {
                                E = -1;
                                angle = Math.PI / 2 - Math.Atan(k1);
                            }
                            else if (i - 50 < 0 && j - 50 < 0)
                            {
                                E = -1;
                                angle = (-1) * (Math.PI / 2 - Math.Atan(k1));
                            }
                            else if (i - 50 == 0 && j - 50 > 0)
                            {
                                E = 1;
                                angle = 0;
                            }
                            else if (i - 50 == 0 && j - 50 < 0)
                            {
                                E = -1;
                                angle = 0;
                            }
                            else if (i - 50 > 0 && j - 50 == 0)
                            {
                                E = 1;
                                angle = (-1) * (Math.PI / 2);
                            }
                            else//if (i - 50 < 0 && j - 50 == 0)
                            {
                                E = 1;
                                angle = Math.PI / 2;
                            }



                            double x = E * Math.Cos(angle) + ((double)rdm.Next(1, 200) / 100.0 - 1.0) * error;
                            double y = E * Math.Sin(angle) + ((double)rdm.Next(1, 200) / 100.0 - 1.0) * error;
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
