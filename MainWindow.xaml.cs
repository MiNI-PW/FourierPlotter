﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WPFLAB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window 
    {
        //To Remove Close Button -- WPF doesn't have such built in functionality
        // Prep stuff needed to remove close button on window
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Render);
        private Stopwatch stopwatch = new Stopwatch();
        private bool IsPaused { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();
            
        }


        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            //To Remove Close Button -- WPF doesn't have such built in functionality
            // Code to remove close box from window
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

            this.DataContext = new ObservableCollection<Circle> { new Circle { Radius = 200, Frequency = 1 } };
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Tick;
            
            //this.DataContext = Circles;
            ((INotifyCollectionChanged)circlesDataGrid.Items).CollectionChanged += Circles_CollectionChanged;

            

        }

        private void Circles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            //DrawCircles();
        }

        public async void Tick(object sender, EventArgs e)
        {
            if (stopwatch.Elapsed.TotalSeconds < 10)
            {
                ProgressBar.Value = stopwatch.ElapsedMilliseconds;
            }
            else
            {
                timer.Stop();
                stopwatch.Stop();
                stopwatch.Reset();
                DrawCircles();

            }

        }
        private void ExitMenuOption_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Start_Clicked(object sender, RoutedEventArgs e)
        {
            timer.Start();
            stopwatch.Start();
        }

        private void Pause_Clicked(object sender, RoutedEventArgs e)
        {
            stopwatch.Stop();
            timer.Stop();
        }

        private void Reset_Clicked(object sender, RoutedEventArgs e)
        {
            ProgressBar.Value = 0;
            stopwatch.Reset();
            timer.Stop();
            Plotter.Children.Clear();
        }

        private void MenuItem_OnChecked(object sender, RoutedEventArgs e)
        {
            if(circlesDataGrid != null)
            foreach (var item in circlesDataGrid.Items.SourceCollection)
            {
                Circle c = item as Circle;
                c.ellipse.Visibility = Visibility.Visible;
            }
        }

        private void MenuItem_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (circlesDataGrid != null)
            foreach (var item in circlesDataGrid.Items.SourceCollection)
            {
                Circle c = item as Circle;
                c.ellipse.Visibility = Visibility.Hidden;
            }
        }



        private void DrawCircles()
        {
            var previousCircle = (Circle)null;
            foreach (var item in circlesDataGrid.Items.SourceCollection)
            {
                var circleItem = item as Circle;
                if (previousCircle != null)
                {

                    circleItem.HorizontalRight = (int)(previousCircle.HorizontalRight + circleItem.ellipse.Width / 2);
                    Canvas.SetLeft(circleItem.ellipse, previousCircle.HorizontalRight - circleItem.ellipse.Width / 2);
                }
                else
                {
                    circleItem.HorizontalRight = (int)(Plotter.ActualWidth / 2 + circleItem.ellipse.Width / 2);
                    Canvas.SetLeft(circleItem.ellipse, Plotter.ActualWidth / 2 - circleItem.ellipse.Width / 2);
                }
                Canvas.SetTop(circleItem.ellipse, Plotter.ActualHeight / 2 - circleItem.ellipse.Height / 2);
                previousCircle = circleItem;
                Plotter.Children.Add(circleItem.ellipse);
            }
        }

    }
}
