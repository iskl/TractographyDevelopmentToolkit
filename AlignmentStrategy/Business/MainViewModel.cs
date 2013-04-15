namespace AlignmentStrategy
{
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;
    using System;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// Provides a ViewModel for the Main window.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        public Model3D Model
        {
            get { return model; }
            set
            {
                model = value;
                RaisePropertyChanged("Model");
            }
        }
        public bool ShowFibers
        {
            get { return showFibers; }
            set
            {
                showFibers = value;
                RaisePropertyChanged("ShowFibers");
                UpdateModel();
            }
        }

        public bool ShowArrows
        {
            get { return showArrows; }
            set
            {
                showArrows = value;
                RaisePropertyChanged("ShowArrows");
                UpdateModel();
            }
        }

        public bool ShowDirections
        {
            get { return showDirections; }
            set
            {
                showDirections = value;
                RaisePropertyChanged("ShowDirections");
                UpdateModel();
            }
        }
        
        public String Step
        {
            get { return step; }
            set
            {
                step = value;
                stepForward();
                RaisePropertyChanged("Step");
                UpdateModel();
            }
        }

        public double ArrowDiameter
        {
            get { return arrowDiameter; }
            set
            {
                arrowDiameter = value;
                createArrows();
                createDirections();
                RaisePropertyChanged("ArrowDiameter");
                UpdateModel();
            }
        }

        public String CurrentStatus
        {
            get
            {
                if(currentStatus == Status.NoFiber)
                    return "NoFiber";
                else if (currentStatus == Status.ReachEdge)
                    return "ReachEdge";
                else
                    return "Tracking";
            }
        }


        private Data data;
        private GeometryModel3D dataModel;
        private GeometryModel3D fiberModel;
        private GeometryModel3D directionModel;
        private AlignmentTracking tracking;
        private Model3D model;
        private double arrowDiameter=0.05;
        private bool showFibers = true;
        private bool showArrows = true;
        private bool showDirections = true;
        private String step="0";
        private Status currentStatus=Status.Tracking;


        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            //data = new CrossData(60, 0.15);
            data = new RingData(0.1);
            dataModel = new GeometryModel3D();
            fiberModel = new GeometryModel3D();
            directionModel = new GeometryModel3D();
            //tracking = new AlignmentTracking(data.Voxels, new Point3D(50,0,15));
            tracking = new AlignmentTracking(data.Voxels, new Point3D(25,50,15));

            createArrows();
            createDirections();

            UpdateModel();
        }

        private void createArrows()
        {
            //mesh arrows
            var meshBuilder = new MeshBuilder(false, false);

            int k = 15;
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    int type = data[i, j, k].Type;
                    for (int itype = 0; itype < type; itype++)
                    {
                        meshBuilder.AddArrow
                        (
                            new Point3D(i, j, k),
                            data[i, j, k].Directions[itype].Add(i,j,k),
                            arrowDiameter,
                            5
                        );
                    }//foritype
                }//forj
            }//fori

            dataModel = new GeometryModel3D
            {
                Geometry = meshBuilder.ToMesh(true),
                Material = MaterialHelper.CreateMaterial(Colors.Green),
                BackMaterial = MaterialHelper.CreateMaterial(Colors.Yellow)
            };
        }

        private void createFibers()
        {
            var meshBuilder = new MeshBuilder(true, true);
            for (int i = 0; i < tracking.Fibers.Count; i++)
            {
                meshBuilder.AddTube(tracking.Fibers[i].Node, 0.1, 12, false);
            }
            fiberModel = new GeometryModel3D
            {
                Geometry = meshBuilder.ToMesh(),
                Material = Materials.Hue,
                BackMaterial = Materials.Hue
            };
        }

        private void createDirections()
        {
            var meshBuilder = new MeshBuilder(false, false);
            for (int i = 0; i < tracking.Fibers.Count; i++)
            {
                Point3D endNode = tracking.Fibers[i].Node.Last<Point3D>();
                int gridX = (int)(endNode.X + 0.5);
                int gridY = (int)(endNode.Y + 0.5);
                int gridZ = (int)(endNode.Z + 0.5);
                if (gridX < 0 || gridX > 99 || gridY < 0 || gridY > 99 || gridZ < 0 || gridZ > 29)
                {
                    return;
                }
                int type = data.Voxels[gridX, gridY, gridZ].Type;

                for (int itype = 0; itype < type; itype++)
                {
                    meshBuilder.AddArrow
                    (
                        endNode,
                        endNode.Add(data[gridX, gridY, gridZ].Directions[itype]),
                        arrowDiameter,
                        5
                    );
                }
            }

            directionModel = new GeometryModel3D
            {
                Geometry = meshBuilder.ToMesh(true),
                Material = MaterialHelper.CreateMaterial(Colors.Blue),
                BackMaterial = MaterialHelper.CreateMaterial(Colors.Yellow)
            };
        }

        private void stepForward()
        {
            if (currentStatus == Status.Tracking)
            {
                currentStatus = tracking.Step();
                if (currentStatus == Status.Tracking)
                {
                    createFibers();
                    createDirections();
                }
            }
            RaisePropertyChanged("CurrentStatus");
        }

        public void UpdateModel()
        {
            var modelGroup = new Model3DGroup();

            if(showArrows) modelGroup.Children.Add(dataModel);
            
            if(showFibers) modelGroup.Children.Add(fiberModel);

            if (showDirections) modelGroup.Children.Add(directionModel);

            this.Model = modelGroup;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}