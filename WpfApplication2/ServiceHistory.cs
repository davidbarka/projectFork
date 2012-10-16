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

        public ServiceHistory(string year, string month, int hours)
        {
            ServiceYear = year;
            ServiceMonth = month;
            ServiceHours = hours;
            Init();
        }

        private void Init()
        {
            ColumnDefinition col1 = new ColumnDefinition();
            col1.Width = new GridLength(100);
            ColumnDefinition col2 = new ColumnDefinition();
            col2.Width = new GridLength(60);
            ColumnDefinition checkCol3 = new ColumnDefinition();
            this.ColumnDefinitions.Add(col1);
            this.ColumnDefinitions.Add(col2);
            Rectangle serviceCard = new Rectangle();
            serviceCard.Width = 300;
            serviceCard.Height = 24;
            serviceCard.StrokeThickness = 1;
            serviceCard.Stroke = new SolidColorBrush(Colors.Black);
            this.Background = new SolidColorBrush(Colors.White);
            this.Width = 300;
            this.Height = 24;
            this.Children.Add(serviceCard);
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
