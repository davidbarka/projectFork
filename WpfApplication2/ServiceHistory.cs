using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Runtime.Serialization;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ForkliftManager
{
    [Serializable]
    class ServiceHistory : Grid, ISerializable
    {
        private string ServiceDate { get; set; }
        private string ServiceHours { get; set; }
        private Grid mainGrid;
       // private TextBox input;
        private TextBlock hoursBlock;
        private bool isOpen = false;
        private Rectangle frame;
        private int fullCardHeight = 50;
        private int cardHeight = 24;

        public ServiceHistory(string date)
        {
            ServiceDate = date;
            ServiceHours = "0";
            Init();
        }

        private void Init()
        {
            mainGrid = new Grid();
            ColumnDefinition col0 = new ColumnDefinition();
            col0.Width = new GridLength(10);
            ColumnDefinition col1 = new ColumnDefinition();
            col1.Width = new GridLength(100);
            ColumnDefinition col2 = new ColumnDefinition();
            //col2.Width = new GridLength(60);
            ColumnDefinition checkCol3 = new ColumnDefinition();
            mainGrid.ColumnDefinitions.Add(col0);
            mainGrid.ColumnDefinitions.Add(col1);
            mainGrid.ColumnDefinitions.Add(col2);
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(24);
            mainGrid.RowDefinitions.Add(row);
            this.Background = new SolidColorBrush(Colors.White);
            this.Width = 300;
            this.Height = cardHeight;
            frame = new Rectangle();
            frame.Width = 300;
            frame.Height = cardHeight;
            frame.StrokeThickness = 1;
            frame.Stroke = new SolidColorBrush(Colors.Gray);
            this.Children.Add(frame);
            this.Children.Add(mainGrid);
            TextBlock serviceDate = new TextBlock();
            serviceDate.Text = ServiceDate;
            serviceDate.FontSize = 16;
            serviceDate.FontWeight = FontWeights.SemiBold;
            serviceDate.Foreground = new SolidColorBrush(Colors.Black);
            Grid.SetColumn(serviceDate, 1);
            mainGrid.Children.Add(serviceDate);
            hoursBlock = new TextBlock();
            hoursBlock.Text = "Driftstimer: " + ServiceHours;
            hoursBlock.FontSize = 16;
            hoursBlock.FontWeight = FontWeights.SemiBold;
            hoursBlock.Foreground = new SolidColorBrush(Colors.Black);
            Grid.SetColumn(hoursBlock, 2);
            mainGrid.Children.Add(hoursBlock);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(clickedCard);
            //input = new TextBox();
            //input.IsEnabled = false;
           // Grid.SetColumn(input, 2);
            //mainGrid.Children.Add(input);
            //input.KeyUp += input_KeyUp;
            //input.BorderBrush = null;
            //input.FontSize = 16;
        }

        private void clickedCard(object sender, MouseButtonEventArgs e)
        {
            if (!isOpen)
            {
                OpenAdvanceCard();
            }
            else CloseAdvanceCard();
        }

        private void OpenAdvanceCard()
        {
            isOpen = true;
            DoubleAnimation da = new DoubleAnimation();
            da.From = cardHeight;
            da.To = fullCardHeight;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            this.BeginAnimation(Panel.HeightProperty, da);
            frame.BeginAnimation(Rectangle.HeightProperty, da);
        }

        private void CloseAdvanceCard()
        {
            isOpen = false;
            DoubleAnimation da = new DoubleAnimation();
            da.From = fullCardHeight;
            da.To = cardHeight;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            this.BeginAnimation(Panel.HeightProperty, da);
            frame.BeginAnimation(Rectangle.HeightProperty, da);
        }
        void input_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Return))
            {
                //ServiceHours = input.Text;
                //input.IsEnabled = false;
                //input.Clear();
                updateServiceHours();
            }
        }

        private void updateServiceHours()
        {
            hoursBlock.Text = "Driftstimer: " + ServiceHours;
        }


         public ServiceHistory(SerializationInfo info, StreamingContext context)
        {
            ServiceDate = (string)info.GetValue("ServiceYear", typeof(string));
            ServiceHours = (string)info.GetValue("ServiceHours", typeof(string));
            Init();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ServiceYear", ServiceDate);
            info.AddValue("ServiceHours", ServiceHours);
        }
    }
}
