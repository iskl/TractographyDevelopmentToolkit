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


        private CrossData cross;
        private GeometryModel3D crossModel;
        private GeometryModel3D fiberModel;
        private GeometryModel3D directionModel;
        private AlignmentTracking tracking;
        private Model3D model;
        private double arrowDiameter=0.05;
        private bool showFibers = true;
        private bool showArrows = true;
        private bool showDirections = true;
        private String step="0";


        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            cross = new CrossData(60, 0.1);
            crossModel = new GeometryModel3D();
            fiberModel = new GeometryModel3D();
            directionModel = new GeometryModel3D();
            tracking = new AlignmentTracking(cross.Voxels, new Point3D(50,0,15));

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
                    int type = cross[i, j, k].Type;
                    for (int itype = 0; itype < type; itype++)
                    {
                        meshBuilder.AddArrow
                        (
                            new Point3D(i, j, k),
                            new Point3D
                            (
                                i + cross[i, j, k].Directions[itype].X,
                                j + cross[i, j, k].Directions[itype].Y,
                                k + cross[i, j, k].Directions[itype].Z
                            ),
                            arrowDiameter,
                            5
                        );
                    }//foritype
                }//forj
            }//fori

            crossModel = new GeometryModel3D
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
                int type = cross.Voxels[gridX, gridY, gridZ].Type;

                for (int itype = 0; itype < type; itype++)
                {
                    meshBuilder.AddArrow
                    (
                        new Point3D(endNode.X, endNode.Y, endNode.Z),
                        new Point3D
                        (
                            endNode.X + cross[gridX, gridY, gridZ].Directions[itype].X,
                            endNode.Y + cross[gridX, gridY, gridZ].Directions[itype].Y,
                            endNode.Z + cross[gridX, gridY, gridZ].Directions[itype].Z
                        ),
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

            tracking.Step();

            createFibers();

            createDirections();

        }

        public void UpdateModel()
        {
            var modelGroup = new Model3DGroup();

            if(showArrows) modelGroup.Children.Add(crossModel);
            
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