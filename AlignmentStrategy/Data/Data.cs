using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlignmentStrategy
{
    class Data
    {
        protected Voxel[, ,] voxels;

        public Voxel[, ,] Voxels
        {
            get { return voxels; }
            set { voxels = value; }
        }

        public Voxel this[int a, int b, int c]
        {
            get { return voxels[a, b, c]; }
            set { voxels[a, b, c] = value; }
        }
    }
}
