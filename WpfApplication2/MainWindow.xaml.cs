using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;

namespace ForkliftManager
{
    /*
     * Made By David Barka 17.11.2012
     * davidbarka@gmail.com
     */
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button[] monthsBtns;
        private int selectedMonth = DateTime.Now.Month;
        private List<Card> cards;
        private List<ServiceHistory> ServiceList;
        private SaveFile save;
        private int listCount = 0;
        private bool isMenuShowing = true;
        private bool smallView = false;

        public MainWindow()
        {
            InitializeComponent();
            save = new SaveFile();
            cards = new List<Card>();
            ServiceList = new List<ServiceHistory>();
            Init();
            cards = save.Open();
            ServiceList = save.OpenService();
            UpdateList();
            SetAntallTrucks();
            UpdateStackPanelRef();
            UpdateAntallTrucksRef();
            UpdateServiceLists();
            showSideMenu_Click(null,null);
        }

        private void UpdateAntallTrucksRef()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].SetAntallTrucksRef(antallTrucks);
            }
        }

        private void UpdateStackPanelRef()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].SetStackRef(cardStack);
            }
        }

        private void UpdateList()
        {
            cardStack.Children.Clear();
            SortCardsOnName(cards);
            SortCardsOnColor(cards);
            for (int i = 0; i < cards.Count; i++)
            {
                cardStack.Children.Add(cards[i]);
                cards[i].CardAppear();
            }
            listCount = cards.Count;
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

        private void SaveCards()
        {
            GetServiceList();
            save.Save(cards);
            save.Save(ServiceList);
        }

        private void GetServiceList()
        {
            ServiceList.Clear();
            for (int i = 0; i < cards.Count; i++)
            {
                for (int j = 0; j < cards[i].repHistorik.Count; j++)
                {
                    ServiceList.Add(cards[i].repHistorik[j]);
                }
            }
        }

        private void UpdateServiceLists()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                for (int j = 0; j < ServiceList.Count; j++)
                {
                    if (ServiceList[j].cardID == cards[i].ID)
	                {
                        cards[i].repHistorik.Add(ServiceList[j]);
	                }
                }
                cards[i].UpdateServiceList();
            }
        }

        private void Init()
        {
            for (int i = 0; i < 50; i++)
            {
                int temp = (2000 + i);
                year.Items.Add(temp);
            }
            int currentYear = (DateTime.Now.Year - 2000);
            year.SelectedIndex = currentYear;
            string[] monthsName = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            monthsBtns = new Button[12];
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    monthsBtns[index] = new Button();
                    monthsBtns[index].Content = monthsName[index];
                    Grid.SetColumn(monthsBtns[index], j);
                    Grid.SetRow(monthsBtns[index], i);
                    monthGrid.Children.Add(monthsBtns[index]);
                    monthsBtns[index].Click += new RoutedEventHandler(buttonClick);
                    index++;
                }
            }

            monthsBtns[DateTime.Now.Month - 1].IsEnabled = false;
            SetButtonImg(false);
            SetAntallTrucks();
        }

        public void SetAntallTrucks()
        {
            antallTrucks.Text = cards.Count.ToString();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            string internr, serienr = "", plass = "", type;
            int aar =0, maande =0;
            if (RegNummer.GetLineText(0).Equals(""))
            {
                RegNummer.Focus();
            }
            else
            {
                internr = RegNummer.GetLineText(0);
                internr = internr.ToUpper();
                RegNummer.Clear();
                serienr = SerieNr.GetLineText(0);
                serienr = serienr.ToUpper();
                SerieNr.Clear();
                plass = plassering.GetLineText(0);
                plass = plass.ToUpper();
                plassering.Clear();
                if (!(bool)dateCheck.IsChecked)
                {
                    aar = (int)year.SelectedItem;
                    maande = GetSelectedMonth();
                }
                type = (string)Type.SelectedItem;
                cards.Add(new Card(internr, serienr, plass, type, aar, maande, cards, cardStack));
                cards[cards.Count - 1].SetAntallTrucksRef(antallTrucks);
                cardStack.Children.Insert(0, cards[cards.Count - 1]);
                SaveCards();
                SetAntallTrucks();
            }
        }
 


        private void buttonClick(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < monthsBtns.Length; i++)
            {
                monthsBtns[i].IsEnabled = true;
                if (monthsBtns[i].Equals(sender))
                {
                    monthsBtns[i].IsEnabled = false;
                    selectedMonth = i + 1;
                }  
            }
        }

        private int GetSelectedMonth()
        {
            for (int i = 0; i < monthsBtns.Length; i++)
            {
                if (!monthsBtns[i].IsEnabled)
                {
                    return i + 1;
                }
            }
            return 1;
        }

        private void search_KeyUp(object sender, KeyEventArgs e)
        {
            string comp = search.Text;
            comp = comp.ToUpper();
            List<Card> temp = new List<Card>();
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].GetInterNr().Contains(comp) || cards[i].GetSerieNr().Contains(comp) ||
                    cards[i].GetPlassering().Contains(comp) || cards[i].GetTypeTruck().Contains(comp))
                {
                    temp.Add(cards[i]);
                }
            }

            cardStack.Children.Clear();
            for (int i = temp.Count-1; i > -1; i--)
            {
                cardStack.Children.Insert(0, temp[i]);
                temp[i].CardAppear();
            }
            if (comp.Equals(""))
            {
                UpdateList();
            }
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveCards();
        }

        private void showSideMenu_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation();
            if (isMenuShowing)
            {
                HideAnimation(RegNummer, 186, 15);
                HideAnimation(SerieNr, 186, 15);
                HideAnimation(plassering, 186, 15);
                HideAnimation(Merknad, 186, 15);
                HideAnimation(year, 186, 15);
                HideAnimation(Type, 186, 15);
                HideAnimation(aarskontroll, 186, 15);
                HideAnimation(interLabel, 186, 15);
                HideAnimation(serieLabel, 186, 15);
                HideAnimation(plassLabel, 186, 15);
                HideAnimation(dateCheck, 186, 15);
                HideAnimation(AddBtn, 97, 15);
                HideAnimation(label, 128, 15);
                DoubleAnimation da2 = new DoubleAnimation(186, 15, TimeSpan.FromMilliseconds(200));
                monthGrid.BeginAnimation(Grid.WidthProperty, da2);

                RegNummer.Opacity = 0;
                SerieNr.Opacity = 0;
                plassering.Opacity = 0;
                Merknad.Opacity = 0;
                year.Opacity = 0;
                Type.Opacity = 0;
                monthGrid.Opacity = 0;
                label.Opacity = 0;
                aarskontroll.Opacity = 0;
                interLabel.Opacity = 0;
                serieLabel.Opacity = 0;
                plassLabel.Opacity = 0;
                dateCheck.Opacity = 0;
                AddBtn.Opacity = 0;
                isMenuShowing = false;
                da.From = 206;
                da.To = 15;
                da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                SideMenu.BeginAnimation(ColumnDefinition.MinWidthProperty, da);
                SetButtonImg(true);
            }
            else
            {
                da.From = 30;
                da.To = 206;
                da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                SideMenu.BeginAnimation(ColumnDefinition.MinWidthProperty, da);

                HideAnimation(RegNummer, 15, 186);
                HideAnimation(SerieNr, 15, 186);
                HideAnimation(plassering, 15, 186);
                HideAnimation(Merknad, 15, 186);
                HideAnimation(year, 15, 186);
                HideAnimation(Type, 15, 186);
                HideAnimation(aarskontroll, 15, 186);
                HideAnimation(interLabel, 15, 186);
                HideAnimation(serieLabel, 15, 186);
                HideAnimation(plassLabel, 15, 186);
                HideAnimation(dateCheck, 15, 186);
                HideAnimation(AddBtn, 15, 97);
                HideAnimation(label, 15, 128);
                DoubleAnimation da2 = new DoubleAnimation(15, 186, TimeSpan.FromMilliseconds(200));
                monthGrid.BeginAnimation(Grid.WidthProperty, da2);

                RegNummer.Opacity = 100;
                SerieNr.Opacity = 100;
                plassering.Opacity = 100;
                Merknad.Opacity = 100;
                year.Opacity = 100;
                Type.Opacity = 100;
                monthGrid.Opacity = 100;
                label.Opacity = 100;
                aarskontroll.Opacity = 100;
                interLabel.Opacity = 100;
                serieLabel.Opacity = 100;
                plassLabel.Opacity = 100;
                dateCheck.Opacity = 100;
                AddBtn.Opacity = 100;

                isMenuShowing = true;
                SetButtonImg(false);
            }
        }

        private void HideAnimation(Control AnimationObject, double start, double end)
        {
            DoubleAnimation da = new DoubleAnimation(start, end, TimeSpan.FromMilliseconds(200));
            AnimationObject.BeginAnimation(Control.WidthProperty, da);
        }

        private void SetButtonImg(bool isRight)
        {
            Uri uri;
            if (isRight)
            {
                uri = new Uri("pack://application:,,/Bilder/right.png");
            }
            else
            {
                uri = new Uri("pack://application:,,/Bilder/left.png");
            }
            BitmapImage bitmap = new BitmapImage(uri);

            Image img = new Image();
            img.Source = bitmap;
            img.Stretch = Stretch.Uniform;

            showSideMenu.Content = img;
        }

        private void closeAll_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].CloseAllCardsExternal();
            }
        }

        private void view_Click(object sender, RoutedEventArgs e)
        {
            if (!smallView)
            {
                smallView = true;
                for (int i = 0; i < cards.Count; i++)
                {
                    cards[i].SetHeight(35);
                }
            }
        }
    }
}
