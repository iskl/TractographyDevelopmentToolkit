using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf;

namespace AlignmentStrategy
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += this.CompositionTargetRendering;
            this.DataContext = new MainViewModel();
            setGridText(0);

        }

        private void slider_Level_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            grid.Center = new Point3D(50, 50, slider_Level.Value);
            setGridText((int)slider_Level.Value);
            textBlock_Level.Text = slider_Level.Value.ToString();
        }

        private void setGridText(int k)
        {
            this.overlay1.Children.Clear();
            for (int i = 0; i <= 100; i += 10)
            {
                for (int j = 0; j <= 100; j += 10)
                {
                    //var circle = new Ellipse { Width = 4, Height = 4, Fill = Brushes.Tomato };
                    var text = new TextBlock { Text = "(" + i + "," + j + "," + k + ")" };
                    //Overlay.SetPosition3D(circle, new Point3D(i, j, 0));
                    Overlay.SetPosition3D(text, new Point3D(i, j, k));
                    //this.overlay1.Children.Add(circle);
                    this.overlay1.Children.Add(text);
                }
            }
        }

        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            var matrix = Viewport3DHelper.GetTotalTransform(this.helixViewport3D_Main.Viewport);
            foreach (FrameworkElement element in this.overlay1.Children)
            {
                var position = Overlay.GetPosition3D(element);
                var position2D = matrix.Transform(position);
                Canvas.SetLeft(element, position2D.X - element.ActualWidth / 2);
                Canvas.SetTop(element, position2D.Y - element.ActualHeight / 2);
            }
        }

        private void showGridText_Click(object sender, RoutedEventArgs e)
        {
            if (showGridText.IsChecked == true)
            {
                overlay1.Visibility = Visibility.Visible;
            }
            else
            {
                overlay1.Visibility = Visibility.Hidden;
            }
        }

        private void button_Step_Click(object sender, RoutedEventArgs e)
        {
            int value= Convert.ToInt32(button_Step.Content);
            button_Step.Content = (value + 1).ToString();
        }
    }
}
