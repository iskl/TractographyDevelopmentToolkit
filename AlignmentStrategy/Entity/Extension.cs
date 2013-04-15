using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace AlignmentStrategy
{
    public static class Extension
    {
        public static Point3D Add(this Point3D p1, Point3D p2)
        {
            return new Point3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }

        public static Point3D Add(this Point3D p1, double x,double y,double z)
        {
            return new Point3D(p1.X + x, p1.Y + y, p1.Z + z);
        }

        public static Point3D Substract(this Point3D p1, Point3D p2)
        {
            return new Point3D(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }

        public static Point3D Multiply(this Point3D p1, double t)
        {
            return new Point3D(t*p1.X, t*p1.Y, t*p1.Z);
        }

        public static double GetLength(this Point3D a)
        {
            return Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
        }

        public static Point3D GetNormal(this Point3D a)
        {
            double e= Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
            a.X /= e;
            a.Y /= e;
            a.Z /= e;
            return a;
        }
    }
}
