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

    /*
     * Made By David Barka 17.11.2012
     * davidbarka@gmail.com
     */
    [Serializable()]
    class ServiceHistory : Grid, ISerializable
    {
        private string ServiceDate { get; set; }
        private string ServiceHours { get; set; }
        private Grid mainGrid;
        private TextBox input;
        private TextBlock hoursBlock;
        private bool isOpen = false;
        private Rectangle frame;
        private int fullCardHeight = 66;
        private int cardHeight = 24;
        public int cardID { get; set; }
        private Button deleteBtn;
        private List<ServiceHistory> serviceListeRef;

        public ServiceHistory(string date, int CardID, List<ServiceHistory> serviceListeRef)
        {
            ServiceDate = date;
            ServiceHours = "0";
            cardID = CardID;
            this.serviceListeRef = serviceListeRef;
            Init();
        }

        private void Init()
        {
            mainGrid = new Grid();
            ColumnDefinition col0 = new ColumnDefinition();
            col0.Width = new GridLength(20);
            ColumnDefinition col1 = new ColumnDefinition();
            col1.Width = new GridLength(100);
            ColumnDefinition col2 = new ColumnDefinition();
            //col2.Width = new GridLength(60);
            ColumnDefinition checkCol3 = new ColumnDefinition();
            mainGrid.ColumnDefinitions.Add(col0);
            mainGrid.ColumnDefinitions.Add(col1);
            mainGrid.ColumnDefinitions.Add(col2);
            RowDefinition row2 = new RowDefinition();
            row2.Height = new GridLength(24);
            mainGrid.RowDefinitions.Add(row2);
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(24);
            mainGrid.RowDefinitions.Add(row);
            RowDefinition row3 = new RowDefinition();
            mainGrid.RowDefinitions.Add(row3);
            this.Background = new SolidColorBrush(Colors.White);
            this.Width = 330;
            this.Height = cardHeight;
            frame = new Rectangle();
            frame.Width = 330;
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
            TextBlock inputLabel = new TextBlock();
            inputLabel.Text = ServiceDate;
            inputLabel.FontSize = 16;
            inputLabel.FontWeight = FontWeights.SemiBold;
            inputLabel.Foreground = new SolidColorBrush(Colors.Black);
            inputLabel.Text = "Oppdater:";
            Grid.SetColumn(inputLabel, 1);
            Grid.SetRow(inputLabel, 1);
            mainGrid.Children.Add(inputLabel);
            input = new TextBox();
            input.Width = 160;
            Grid.SetColumn(input, 2);
            Grid.SetRow(input, 1);
            mainGrid.Children.Add(input);
            input.KeyUp += input_KeyUp;
            //input.BorderBrush = null;
            input.FontSize = 16;
            deleteBtn = new Button();
            deleteBtn.Content = "X";
            deleteBtn.FontWeight = FontWeights.Bold;
            Grid.SetColumn(deleteBtn, 0);
            Grid.SetRow(deleteBtn, 2);
            mainGrid.Children.Add(deleteBtn);
            deleteBtn.Click +=new RoutedEventHandler(exitButton_MouseLeftButtonDown);
            this.MouseEnter += new MouseEventHandler(mouseEnterCard);
            this.MouseLeave += new MouseEventHandler(mouseLeaveCard);
        }

        private void mouseLeaveCard(object sender, MouseEventArgs e)
        {
            frame.StrokeThickness = 1;
            frame.Stroke = new SolidColorBrush(Colors.Gray);
        }

        private void mouseEnterCard(object sender, MouseEventArgs e)
        {
            frame.StrokeThickness = 2;
            frame.Stroke = new SolidColorBrush(Colors.SkyBlue);
        }

        private void exitButton_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        private void Delete()
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = this.Height;
            da.To = 0;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            this.BeginAnimation(Panel.HeightProperty, da);
            frame.BeginAnimation(Panel.HeightProperty, da);
            isOpen = false;
            if (System.Windows.MessageBox.Show("Vil du slette denne servicen?", "Slette service?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                serviceListeRef.Remove(this);
                //UpdateList();
            }
            else
            {
                da.From = 0;
                da.To = cardHeight;
                da.Duration = new Duration(TimeSpan.FromMilliseconds(300));
                this.BeginAnimation(Panel.HeightProperty, da);
                frame.BeginAnimation(Panel.HeightProperty, da);
            }
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
            if (input.Text != "")
            {
                ServiceHours = input.Text;
                input.Clear();
                updateServiceHours();
            }
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
                CloseAdvanceCard();
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
            cardID = (int)info.GetValue("cardID", typeof(int));
            serviceListeRef = (List<ServiceHistory>)info.GetValue("serviceListeRef", typeof(List<ServiceHistory>));
            Init();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ServiceYear", ServiceDate);
            info.AddValue("ServiceHours", ServiceHours);
            info.AddValue("cardID", cardID);
            info.AddValue("serviceListeRef", serviceListeRef);
        }

        public void SetServiceListRef(List<ServiceHistory> innput)
        {
            serviceListeRef = innput;
        }
    }
}
