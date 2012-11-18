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
            UpdateStackPanelRef();
            UpdateServiceLists();
            BlurEffect blur = new BlurEffect();
            blur.Radius = 10;
            //this.Effect = blur;  
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
                cardStack.Children.Insert(0, cards[cards.Count - 1]);
                SaveCards();
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
                da.From = 206;
                da.To = 15;
                da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                SideMenu.BeginAnimation(ColumnDefinition.MinWidthProperty, da);

                RegNummer.Width = 15;
                SerieNr.Width = 15;
                plassering.Width = 15;
                Merknad.Width = 15;
                year.Width = 15;
                Type.Width = 15;
                monthGrid.Width = 15;
                label.Width = 15;
                aarskontroll.Width = 15;
                interLabel.Width = 15;
                serieLabel.Width = 15;
                plassLabel.Width = 15;
                dateCheck.Width = 15;
                AddBtn.Width = 15;

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

                showSideMenu.Content = ">>";
            }
            else
            {
                da.From = 30;
                da.To = 206;
                da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                SideMenu.BeginAnimation(ColumnDefinition.MinWidthProperty, da);

                RegNummer.Width = 186;
                SerieNr.Width = 186;
                plassering.Width = 186;
                Merknad.Width = 186;
                year.Width = 186;
                Type.Width = 186;
                monthGrid.Width = 186;
                label.Width = 128;
                aarskontroll.Width = 186;
                interLabel.Width = 186;
                serieLabel.Width = 186;
                plassLabel.Width = 186;
                dateCheck.Width = 186;
                AddBtn.Width = 97;

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
                isMenuShowing = false;
                isMenuShowing = true;
                showSideMenu.Content = "<<";
            }
        }
    }
}
