using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point start, dest;
        Color strokeColor = Colors.Red;
        Color fillColor = Colors.Yellow;
        Brush currentStrokeBrush;
        Brush currentFillBrush;
        int strokeThickness = 3;
        string shapeType = "Line";
        string actionType = "Draw";
        public MainWindow()
        {
            InitializeComponent();
            strokeColorPicker.SelectedColor = strokeColor;
            fillColorPicker.SelectedColor = fillColor;
        }

        private void myCanvs_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(actionType == "Draw")
            {
                switch (shapeType)
                {
                    case "Line":
                        var line = myCanvs.Children.OfType<Line>().LastOrDefault();
                        UpdateShape(line, currentStrokeBrush, strokeThickness);
                        break;
                    case "Rectangle":
                        var rect = myCanvs.Children.OfType<Rectangle>().LastOrDefault();
                        UpdateShape(rect, currentStrokeBrush, currentFillBrush, strokeThickness);
                        break;
                    case "Ellipse":
                        var ellipse = myCanvs.Children.OfType<Ellipse>().LastOrDefault();
                        UpdateShape(ellipse, currentStrokeBrush, currentFillBrush, strokeThickness);
                        break;
                    case "Polyline":
                        var polyline = myCanvs.Children.OfType<Polyline>().LastOrDefault();
                        UpdateShape(polyline, currentStrokeBrush, currentFillBrush, strokeThickness);
                        break;
                }
                myCanvs.Cursor = Cursors.Arrow;
            }

            DisplayStatus();
        }

        private void UpdateShape(Shape shape, Brush stroke, int thickness)
        {
            shape.Stroke = stroke;
            shape.StrokeThickness = thickness;
        }

        private void UpdateShape(Shape shape, Brush stroke, Brush fill, int thickness)
        {
            shape.Stroke = stroke;
            shape.Fill = fill;
            shape.StrokeThickness = thickness;
        }


        private void DrawEllipse(Brush stroke, Brush fill, int thickness)
        {
            double origin_X = Math.Min(start.X, dest.X);
            double origin_Y = Math.Min(start.Y, dest.Y);
            double width = Math.Abs(dest.X - start.X);
            double height = Math.Abs(dest.Y - start.Y);
            Ellipse myEllipse = new Ellipse()
            {
                Stroke = currentStrokeBrush,
                Width = width,
                Height = height,
                Fill = new SolidColorBrush(Colors.Wheat),
                StrokeThickness = 3.0
            };
            myEllipse.SetValue(Canvas.LeftProperty, origin_X);
            myEllipse.SetValue(Canvas.TopProperty, origin_Y);
            myCanvs.Children.Add(myEllipse);
        }
        private void DrawRectangle(Brush stroke, Brush fill, int thickness)
        {
            Rectangle rect = new Rectangle()
            {
                Stroke = stroke,
                Fill = fill,
                StrokeThickness = thickness
            };
            UpdateShape(rect);
            myCanvs.Children.Add(rect);
        }

        private void UpdateShape(Shape shape)
        {
            Point origin;
            origin.X = Math.Min(start.X, dest.X);
            origin.Y = Math.Min(start.Y, dest.Y);
            double width = Math.Abs(dest.X - start.X);
            double height = Math.Abs(dest.Y - start.Y);

            shape.Width = width;
            shape.Height = height;
            shape.SetValue(Canvas.LeftProperty, origin.X);
            shape.SetValue(Canvas.TopProperty, origin.Y);

        }

        private void DrawLine(Brush stroke, int thickness)
        {
            //Line myLine = new Line();
            //myLine.Stroke = currentStrokeBrush;
            //myLine.X1 = start.X;
            //myLine.Y1 = start.Y;
            //myLine.X2 = dest.X;
            //myLine.Y2 = dest.Y;
            //myLine.StrokeThickness = 3.0;

            Line line = new Line()
            {
                X1 = start.X,
                Y1 = start.Y,
                X2 = dest.X,
                Y2 = dest.Y,
                Stroke = stroke,
                StrokeThickness = thickness
            };

            myCanvs.Children.Add(line);
        }

        private void myCanvs_MouseMove(object sender, MouseEventArgs e)
        {
            dest = e.GetPosition(myCanvs);

            if(actionType == "Erase")
            {
                Shape eraseShape = e.OriginalSource as Shape;
                myCanvs.Children.Remove(eraseShape);
                if(myCanvs.Children.Count == 0)myCanvs.Cursor = Cursors.Arrow;
            }
            else if(e.LeftButton == MouseButtonState.Pressed)
            {
                    switch (shapeType)
                    {
                        case "Line":
                            var line = myCanvs.Children.OfType<Line>().LastOrDefault();
                            line.X2 = dest.X;
                            line.Y2 = dest.Y;
                            break;
                        case "Rectangle":
                            var rect = myCanvs.Children.OfType<Rectangle>().LastOrDefault();
                            UpdateShape(rect);
                            break;
                        case "Ellipse":
                            var ellipse = myCanvs.Children.OfType<Ellipse>().LastOrDefault();
                            UpdateShape(ellipse);
                            break;
                        case "Polyline":
                            var polyline = myCanvs.Children.OfType<Polyline>().LastOrDefault();
                            PointCollection myPointCollection = polyline.Points;
                            myPointCollection.Add(dest);
                            break;
                }
            }

            DisplayStatus();
        }

        private void strokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            strokeColor = (Color)e.NewValue;
            currentStrokeBrush = new SolidColorBrush(strokeColor);
        }

        private void fillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            fillColor = (Color)e.NewValue;
            currentFillBrush = new SolidColorBrush(fillColor);
        }

        private void thicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strokeThickness = (int)e.NewValue;

        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton targetRadioButton = sender as RadioButton;
            shapeType = targetRadioButton.Tag.ToString();
            actionType = "Draw";
        }

        private void myCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            start = e.GetPosition(myCanvs);
            if (actionType == "Draw")
            {
                myCanvs.Cursor = Cursors.Cross;
                switch (shapeType)
                {
                    case "Line":
                        DrawLine(Brushes.Gray, 1);
                        break;
                    case "Rectangle":
                        DrawRectangle(Brushes.Gray, Brushes.LightGray, 1);
                        break;
                    case "Ellipse":
                        DrawEllipse(Brushes.Gray, Brushes.LightGray, 1);
                        break;
                    case "Polyline":
                        DrawPolyline(Brushes.Gray, Brushes.LightGray, 1);
                        break;
                }
            }
            DisplayStatus();
        }

        private void DrawPolyline(Brush stroke, Brush fill, int thickness)
        {
            PointCollection myPointCollection = new PointCollection();
            myPointCollection.Add(start);

            Polyline polyline = new Polyline()
            {
                Stroke = stroke,
                Fill = fill,
                StrokeThickness = thickness,
                Points = myPointCollection
            };
            myCanvs.Children.Add(polyline);
        }

        private void EarseButton_Click(object sender, RoutedEventArgs e)
        {
            actionType = "Erase";
            if(myCanvs.Children.Count != 0)
            {
                myCanvs.Cursor = Cursors.Hand;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            myCanvs.Children.Clear();
        }

        private void saveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int w = Convert.ToInt32(myCanvs.RenderSize.Width);
            int h = Convert.ToInt32(myCanvs.RenderSize.Height);

            RenderTargetBitmap rtb = new RenderTargetBitmap(w, h, 64d, 64d, PixelFormats.Default);
            rtb.Render(myCanvs);

            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));
        
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "儲存畫布";
            saveFileDialog.DefaultExt = "*.png";
            saveFileDialog.Filter = "png檔案(*.png)|*.png|所有檔案(*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                string path = saveFileDialog.FileName; ;
                using (var fs = File.Create(path))
                {
                    png.Save(fs);
                }
            }
        }

        private void DisplayStatus()
        {
            int lineCount = myCanvs.Children.OfType<Line>().Count();
            int rectangleCount = myCanvs.Children.OfType<Rectangle>().Count();
            int ellipseCount = myCanvs.Children.OfType<Ellipse>().Count();
            int polylineCount = myCanvs.Children.OfType<Polyline>().Count();
            coordinateLabel.Content = $"座標點({Math.Round(start.X)},{Math.Round(start.Y)} : {Math.Round(dest.X)},{Math.Round(dest.Y)})";
            shapeLabel.Content = $"Line:{lineCount},Rectangle:{rectangleCount},Ellipse:{ellipseCount},Polyline:{polylineCount}";
        }
    }
}
