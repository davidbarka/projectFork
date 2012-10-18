using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Runtime.Serialization;

namespace ForkliftManager
{
    [Serializable]
    class ServiceHistory : Grid, ISerializable
    {
        private string ServiceYear { get; set; }
        private string ServiceMonth { get; set; }
        private int ServiceHours { get; set; }
        private Grid mainGrid;

        public ServiceHistory(string year, string month, int hours)
        {
            ServiceYear = year;
            ServiceMonth = month;
            ServiceHours = hours;
            Init();
        }

        private void Init()
        {
            mainGrid = new Grid();
            ColumnDefinition col1 = new ColumnDefinition();
            col1.Width = new GridLength(100);
            ColumnDefinition col2 = new ColumnDefinition();
            col2.Width = new GridLength(60);
            ColumnDefinition checkCol3 = new ColumnDefinition();
            mainGrid.ColumnDefinitions.Add(col1);
            mainGrid.ColumnDefinitions.Add(col2);
            this.Background = new SolidColorBrush(Colors.White);
            this.Width = 300;
            this.Height = 24;
            Rectangle serviceCard = new Rectangle();
            serviceCard.Width = 300;
            serviceCard.Height = 24;
            serviceCard.StrokeThickness = 1;
            serviceCard.Stroke = new SolidColorBrush(Colors.Gray);
            this.Children.Add(serviceCard);
            this.Children.Add(mainGrid);
            TextBlock serviceDate = new TextBlock();
            serviceDate.Text = ServiceYear + "/" + ServiceMonth;
            serviceDate.Foreground = new SolidColorBrush(Colors.Black);
            Grid.SetColumn(serviceDate, 0);
            mainGrid.Children.Add(serviceDate);
            TextBlock hoursBlock = new TextBlock();
            hoursBlock.Text = ServiceHours.ToString();
            hoursBlock.Foreground = new SolidColorBrush(Colors.Black);
            Grid.SetColumn(hoursBlock, 1);
            mainGrid.Children.Add(hoursBlock);
        }


         public ServiceHistory(SerializationInfo info, StreamingContext context)
        {
            ServiceYear = (string)info.GetValue("ServiceYear", typeof(string));
            ServiceMonth = (string)info.GetValue("ServiceMonth", typeof(string));
            ServiceHours = (int)info.GetValue("ServiceHours", typeof(int));
            Init();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ServiceYear", ServiceYear);
            info.AddValue("ServiceMonth", ServiceMonth);
            info.AddValue("ServiceHours", ServiceHours);
        }
    }
}
