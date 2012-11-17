using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ForkliftManager
{
    /*
     * Made By David Barka 17.11.2012
     * davidbarka@gmail.com
     */
    [Serializable()]
    class Card : System.Windows.Controls.Grid, ISerializable
    {
        private List<SolidColorBrush> cardColors = new List<SolidColorBrush>() { new SolidColorBrush(Colors.Tomato), new SolidColorBrush(Colors.Gold),new SolidColorBrush(Colors.SkyBlue),
                                                                                new SolidColorBrush(Colors.DarkSeaGreen), new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.CornflowerBlue) };
        private List<LinearGradientBrush> cardGradientColors = new List<LinearGradientBrush>() { new LinearGradientBrush(Colors.Red, Colors.Salmon, 60), new LinearGradientBrush(Colors.Goldenrod, Colors.Gold, 60), 
                                                                                             new LinearGradientBrush(Colors.RoyalBlue, Colors.SkyBlue, 60), new LinearGradientBrush(Colors.Green, Colors.DarkSeaGreen, 60) };
        public List<ServiceHistory> repHistorik = new List<ServiceHistory>();
        private TextBlock interNr { get; set; }
        private TextBlock serieNr { get; set; }
        private TextBlock plassering { get; set; }
        private TextBlock type { get; set; }
        private int yearReg { get; set; }
        private int monthReg { get; set; }
        private int ServiceYear { get; set; }
        private int ServiceMonth { get; set; }
        private int driftTimer { get; set; }
        private const int DANGER = 0, WARNING = 1, OK = 2, GOOD = 3, FRAME = 4, ACTIVEFRAME = 5;
        private Grid grid { get; set; }
        Grid CheckGrid { get; set; }
        Grid ServiceGrid { get; set; }
        private int cardWidth = 750;
        private int cardHeight = 400;
        private Rectangle frame { get; set; }
        private List<Card> listRef;
        private StackPanel stackRef;
        private bool isOpen = false;
        private ComboBox monthBox, yearBox;
        private DatePicker serviceDate;
        private Button serviceDone, yearCheck;
        private DoubleAnimation DueDateAnimation;
        private int Priority { get; set; }
        ScrollViewer serviceScroller;
        StackPanel serviceStack;
        private static int cardID = 0;
        public int ID { get; set; }
        private TextBox plassTextBox, serieTextBox;
        private bool editMode = false;

        public Card(string internr, string serienr, string plass, string type, int aar, int maande, List<Card> listRef, StackPanel stackRef)
        {
            ID = cardID++;
            this.listRef = listRef;
            this.stackRef = stackRef;
            Init(internr, serienr, plass, type, aar, maande);
            CardAppear();
            CheckAnnualInspection();
        }

        private void OpenAdvanceCard()
        {
            isOpen = true;
            DoubleAnimation da = new DoubleAnimation();
            da.From = 50;
            da.To = cardHeight;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            this.BeginAnimation(Panel.HeightProperty, da);
            frame.BeginAnimation(Rectangle.HeightProperty, da);
        }

        private void CloseAdvanceCard()
        {
            isOpen = false;
            DoubleAnimation da = new DoubleAnimation();
            da.From = cardHeight;
            da.To = 50;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            this.BeginAnimation(Panel.HeightProperty, da);
            frame.BeginAnimation(Rectangle.HeightProperty, da);
        }

        public void CardAppear()
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = 50;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            this.BeginAnimation(Panel.HeightProperty, da);

        }

        private void Init(string internr, string serienr, string plass, string type, int aar, int maande)
        {
            plassTextBox = new TextBox();
            serieTextBox = new TextBox();
            interNr = new TextBlock();
            interNr.Text = internr;
            serieNr = new TextBlock();
            serieNr.Text = serienr;
            plassering = new TextBlock();
            plassering.Text = plass;
            this.type = new TextBlock();
            this.type.Text = type;
            yearReg = aar;
            monthReg = maande;

            grid = new Grid();
            this.Children.Add(grid);

            frame = new Rectangle();
            frame.Width = cardWidth;
            frame.Height = 50;
            frame.StrokeThickness = 2;
            frame.Stroke = cardColors[FRAME];
            this.Children.Add(frame);

            this.Height = 50;
            this.Width = cardWidth;
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;

            //grid.ShowGridLines = true;

            GridSetup();
            ComboBoxSetup();

            interNr.FontSize = 22;
            interNr.FontWeight = FontWeights.Bold;
            Grid.SetColumn(interNr, 1);
            grid.Children.Add(interNr);

            this.type.FontSize = 14;
            this.type.FontWeight = FontWeights.SemiBold;
            Grid.SetRow(this.type, 1);
            Grid.SetColumn(this.type, 1);
            grid.Children.Add(this.type);

            plassering.FontSize = 14;
            plassering.FontWeight = FontWeights.SemiBold;
            Grid.SetRow(plassering, 1);
            Grid.SetColumn(plassering, 2);
            grid.Children.Add(plassering);

            serieNr.FontWeight = FontWeights.SemiBold;
            serieNr.FontSize = 14;
            Grid.SetRow(serieNr, 4);
            Grid.SetColumn(serieNr, 2);
            grid.Children.Add(serieNr);

            this.MouseEnter += new MouseEventHandler(mouseEnterCard);
            this.MouseLeave += new MouseEventHandler(mouseLeaveCard);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(clickedCard);
            this.MouseRightButtonDown += new MouseButtonEventHandler(EditCard);

            UpdateServiceList();
        }

        private void EditCard(object sender, MouseButtonEventArgs e)
        {
           
            if (editMode)
            {
                plassering.Text = plassTextBox.Text.ToUpper();
                serieNr.Text = serieTextBox.Text;
                editMode = false;
                Grid.SetRow(plassering, 1);
                Grid.SetColumn(plassering, 2);
                grid.Children.Remove(plassTextBox);
                grid.Children.Add(plassering);


                Grid.SetRow(serieNr, 4);
                Grid.SetColumn(serieNr, 2);
                grid.Children.Remove(serieTextBox);
                grid.Children.Add(serieNr);
                CheckAnnualInspection();
            }
            else
            {
                plassTextBox.Text = plassering.Text;
                serieTextBox.Text = serieNr.Text;
                editMode = true;
                Grid.SetRow(plassTextBox, 1);
                Grid.SetColumn(plassTextBox, 2);
                grid.Children.Remove(plassering);
                grid.Children.Add(plassTextBox);

                Grid.SetRow(serieTextBox, 4);
                Grid.SetColumn(serieTextBox, 2);
                grid.Children.Remove(serieNr);
                grid.Children.Add(serieTextBox);
            }
        }

        private void ComboBoxSetup()
        {
            string[] monthsName = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            serviceDone = new Button();
            serviceDone.Click += serviceDone_Click;
            serviceDone.Content = "Service gjort";
            serviceDone.FontSize = 14;
            serviceDone.FontWeight = FontWeights.SemiBold;
            yearCheck = new Button();
            yearCheck.Click += yearCheck_Click;
            yearCheck.Content = "Oppdater årskontroll";
            yearCheck.FontSize = 14;
            yearCheck.FontWeight = FontWeights.SemiBold;
            monthBox = new ComboBox();
            yearBox = new ComboBox();
            serviceDate = new DatePicker();
            serviceDate.Text = DateTime.Now.ToShortDateString();
            serviceDate.FontSize = 14;
            monthBox.FontSize = 14;
            yearBox.FontSize = 14;

            if (yearReg >= 2000)
            {
                yearBox.SelectedIndex = yearReg - 2000;
                monthBox.SelectedIndex = monthReg - 1;
            }
            for (int i = 0; i < monthsName.Length; i++)
            {
                monthBox.Items.Add(monthsName[i]);
            }
            for (int i = 0; i < 50; i++)
            {
                yearBox.Items.Add(2000 + i);
            }


            CheckGrid = new Grid();
            ServiceGrid = new Grid();

            AdvanceCardGridSetup();

            Grid.SetColumn(monthBox, 0);
            CheckGrid.Children.Add(monthBox);
            Grid.SetColumn(yearBox, 1);
            CheckGrid.Children.Add(yearBox);
            Grid.SetColumn(yearCheck, 2);
            CheckGrid.Children.Add(yearCheck);

            Grid.SetColumn(serviceDate, 0);
            ServiceGrid.Children.Add(serviceDate);
            Grid.SetColumn(serviceDone, 1);
            ServiceGrid.Children.Add(serviceDone);

        }

        private void serviceDone_Click(object sender, RoutedEventArgs e)
        {
            if (serviceDate.Text.Equals(""))serviceDate.Text = DateTime.Now.ToShortDateString();
            repHistorik.Add(new ServiceHistory(serviceDate.Text, ID));
            serviceStack.Children.Insert(0, repHistorik[repHistorik.Count-1]);
            
        }

        void yearCheck_Click(object sender, RoutedEventArgs e)
        {
            if (yearBox.SelectedIndex == yearReg - 2000 && monthBox.SelectedIndex == monthReg - 1)
            {
                yearReg++;
                yearBox.SelectedIndex++;
            }
            else if (yearBox.SelectedIndex > 0 && monthBox.SelectedIndex < 0)
            {
                yearReg = (int)yearBox.SelectedItem;
                monthReg = 1;
                monthBox.SelectedIndex = 1;
            }
            else if (monthBox.SelectedIndex > 0 && yearBox.SelectedIndex < 0)
            {
                yearReg = DateTime.Now.Year;
                yearBox.SelectedIndex = DateTime.Now.Year - 2000;
                monthReg = monthBox.SelectedIndex + 1;
            }
            else
            {
                yearReg = (int)yearBox.SelectedItem;
                monthReg = monthBox.SelectedIndex + 1;
            }
            CheckAnnualInspection();
        }

        private void AdvanceCardGridSetup()
        {
            ColumnDefinition checkCol1 = new ColumnDefinition();
            checkCol1.Width = new GridLength(100);
            ColumnDefinition checkCol2 = new ColumnDefinition();
            checkCol2.Width = new GridLength(60);
            ColumnDefinition checkCol3 = new ColumnDefinition();
            ColumnDefinition serviceCol1 = new ColumnDefinition();
            serviceCol1.Width = new GridLength(200);
            ColumnDefinition serviceCol2 = new ColumnDefinition();


            CheckGrid.ColumnDefinitions.Add(checkCol1);
            CheckGrid.ColumnDefinitions.Add(checkCol2);
            CheckGrid.ColumnDefinitions.Add(checkCol3);
            ServiceGrid.ColumnDefinitions.Add(serviceCol1);
            ServiceGrid.ColumnDefinitions.Add(serviceCol2);

            //ServiceGrid.ShowGridLines = true;
            //CheckGrid.ShowGridLines = true;

            Grid.SetColumn(CheckGrid, 1);
            Grid.SetRow(CheckGrid, 3);
            grid.Children.Add(CheckGrid);

            Grid.SetColumn(ServiceGrid, 2);
            Grid.SetRow(ServiceGrid, 3);
            grid.Children.Add(ServiceGrid);

            serviceScroller = new ScrollViewer();
            serviceStack = new StackPanel();
            serviceScroller.Content = serviceStack;
            Grid.SetColumn(serviceScroller, 2);
            Grid.SetRow(serviceScroller, 2);
            grid.Children.Add(serviceScroller);

            Button deleteButton = new Button();
            deleteButton.Content = "X";
            deleteButton.FontWeight = FontWeights.ExtraBold;
            deleteButton.Click +=new RoutedEventHandler(exitButton_MouseLeftButtonDown);
            Grid.SetColumn(deleteButton, 0);
            Grid.SetRow(deleteButton, 4);
            grid.Children.Add(deleteButton);
        }

        void exitButton_MouseLeftButtonDown(object sender, RoutedEventArgs e) 
        {
            DeleteCard();
        }

        private void GridSetup()
        {
            ColumnDefinition column0 = new ColumnDefinition();
            column0.Width = new GridLength(20);
            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = new GridLength(350);
            ColumnDefinition column2 = new ColumnDefinition();
            ColumnDefinition columnEnd = new ColumnDefinition();
            columnEnd.Width = new GridLength(5);

            grid.ColumnDefinitions.Add(column0);
            grid.ColumnDefinitions.Add(column1);
            grid.ColumnDefinitions.Add(column2);
            grid.ColumnDefinitions.Add(columnEnd);

            RowDefinition row1 = new RowDefinition();
            row1.Height = new GridLength(30);
            RowDefinition row2 = new RowDefinition();
            row2.Height = new GridLength(20);
            RowDefinition row3 = new RowDefinition();
            RowDefinition row4 = new RowDefinition();
            row4.Height = new GridLength(25);
            RowDefinition row5 = new RowDefinition();
            row5.Height = new GridLength(20);
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);
            grid.RowDefinitions.Add(row4);
            grid.RowDefinitions.Add(row5);
        }

        private void DeleteCard()
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = this.Height;
            da.To = 0;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            this.BeginAnimation(Panel.HeightProperty, da);
            frame.BeginAnimation(Panel.HeightProperty, da);
            isOpen = false;
            if (System.Windows.MessageBox.Show("Do you want to delete this card?", "Delete Card?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                listRef.Remove(this);
                UpdateList();
            }
            else
            {
                da.From = 0;
                da.To = 50;
                da.Duration = new Duration(TimeSpan.FromMilliseconds(300));
                this.BeginAnimation(Panel.HeightProperty, da);
                frame.BeginAnimation(Panel.HeightProperty, da);
            }
            CheckAnnualInspection();
        }

        private void clickedCard(object sender, MouseButtonEventArgs e)
        {
            if (!isOpen)
            {
                OpenAdvanceCard();
            }
            else if (isOpen)
            {
                CloseAdvanceCard();
            }
        }

        private void mouseLeaveCard(object sender, MouseEventArgs e)
        {
            frame.Stroke = cardColors[FRAME];
        }

        private void mouseEnterCard(object sender, MouseEventArgs e)
        {
            frame.Stroke = cardColors[ACTIVEFRAME];
        }

        #region Getters

        public string GetInterNr()
        {
            return interNr.Text;
        }

        public string GetSerieNr()
        {
            return serieNr.Text;
        }

        public string GetPlassering()
        {
            return plassering.Text;
        }

        public string GetTypeTruck()
        {
            return type.Text.ToUpper();
        }

        public int GetPriority()
        {
            return Priority;
        }

        public Panel GetCard()
        {
            return this;
        }

        public int GetDriftTimer()
        {
            return driftTimer;
        }

        #endregion

        public void CheckAnnualInspection()
        {
            int thisYear = DateTime.Now.Year;
            int thisMonth = DateTime.Now.Month;
            if (yearReg == 0)
            {
                this.Background = cardGradientColors[OK];//cardColors[OK];
                Priority = 4;
            }
            else if (thisYear > yearReg || thisYear >= yearReg && thisMonth > monthReg)
            {
                this.Background = cardGradientColors[DANGER];//cardColors[DANGER];
                Priority = 0;
            }
            else if (thisYear == yearReg && thisMonth == monthReg || thisYear == yearReg && thisMonth == monthReg)
            {
                this.Background = cardGradientColors[DANGER];//cardColors[DANGER];
                Priority = 1;
            }
            else if (thisYear == yearReg && thisMonth == (monthReg - 1) || thisYear == ServiceYear - 1 && ServiceMonth == 12 && ServiceYear != 0)
            {
                this.Background = cardGradientColors[WARNING];//cardColors[WARNING];
                Priority = 2;
            }
            else if (plassering.Text.Equals("") || serieNr.Text.Equals(""))
            {
                this.Background = cardGradientColors[OK];//cardColors[OK];
                Priority = 4;
            }
            else
            {
                this.Background = cardGradientColors[GOOD];//cardColors[GOOD];
                Priority = 3;
            }
            if (thisYear > yearReg && yearReg != 0 || thisYear >= yearReg && thisMonth > monthReg && yearReg != 0)
            {
                DueDateAnimation = new DoubleAnimation();
                DueDateAnimation.From = 1.0;
                DueDateAnimation.To = 0.4;
                DueDateAnimation.AutoReverse = true;
                DueDateAnimation.RepeatBehavior = RepeatBehavior.Forever;
                DueDateAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
                this.BeginAnimation(Panel.OpacityProperty, DueDateAnimation);
            }
            else
            {
                this.BeginAnimation(Panel.OpacityProperty, null);
            }
        }

        public void UpdateServiceList()
        {
            serviceStack.Children.Clear();
            for (int i = 0; i < repHistorik.Count; i++)
            {
                serviceStack.Children.Insert(0, repHistorik[i]);
            }
        }

        public Card(SerializationInfo info, StreamingContext context)
        {
            interNr = new TextBlock();
            interNr.Text = (string)info.GetValue("interNr", typeof(string));
            driftTimer = (int)info.GetValue("driftTimer", typeof(int));
            monthReg = (int)info.GetValue("monthReg", typeof(int));
            plassering = new TextBlock();
            plassering.Text = (string)info.GetValue("plassering", typeof(string));
            yearReg = (int)info.GetValue("yearReg", typeof(int));
            serieNr = new TextBlock();
            serieNr.Text = (string)info.GetValue("serieNr", typeof(string));
            type = new TextBlock();
            type.Text = (string)info.GetValue("type", typeof(string));
            listRef = (List<Card>)info.GetValue("listRef", typeof(List<Card>));
            ServiceYear = (int)info.GetValue("ServiceYear", typeof(int));
            ServiceMonth = (int)info.GetValue("ServiceMonth", typeof(int));
            ID = (int)info.GetValue("ID", typeof(int));
            cardID = (int)info.GetValue("cardID", typeof(int));
            //repHistorik = (List<ServiceHistory>)info.GetValue("repHistorik", typeof(List<ServiceHistory>));
            Init(interNr.Text, serieNr.Text, plassering.Text, type.Text, yearReg, monthReg);
            CheckAnnualInspection();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("interNr", interNr.Text);
            info.AddValue("driftTimer", driftTimer);
            info.AddValue("monthReg", monthReg);
            info.AddValue("plassering", plassering.Text);
            info.AddValue("serieNr", serieNr.Text);
            info.AddValue("yearReg", yearReg);
            info.AddValue("type", type.Text);
            info.AddValue("listRef", listRef);
            info.AddValue("ServiceYear", ServiceYear);
            info.AddValue("ServiceMonth", ServiceMonth);
            info.AddValue("ID", ID);
            info.AddValue("cardID", cardID);
            //info.AddValue("repHistorik", repHistorik);
        }

        // helper methods form main class

        private void UpdateList()
        {
            stackRef.Children.Clear();
            SortCardsOnName(listRef);
            SortCardsOnColor(listRef);
            for (int i = 0; i < listRef.Count; i++)
            {
                stackRef.Children.Add(listRef[i]);
            }
        }

        private void SortCardsOnColor(List<Card> cards)
        {
            cards.Sort(delegate(Card c1, Card c2)
            {
                return c1.GetPriority().CompareTo(c2.GetPriority());
            });
        }

        private void SortCardsOnName(List<Card> cards)
        {
            cards.Sort(delegate(Card c1, Card c2)
            {
                return c1.GetInterNr().CompareTo(c2.GetInterNr());
            });
        }

        public void SetStackRef(StackPanel newRef)
        {
            stackRef = newRef;
        }

        public void SetServiceList(List<ServiceHistory> listInn)
        {
            repHistorik = listInn;
        }

        public List<ServiceHistory> GetServiceList()
        {
            return repHistorik;
        }
    }
}
