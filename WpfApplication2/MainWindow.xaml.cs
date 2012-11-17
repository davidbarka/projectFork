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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button[] monthsBtns;
        private int selectedMonth = DateTime.Now.Month;
        private List<Card> cards;
        private SaveFile save;
        private int listCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            save = new SaveFile();
            cards = new List<Card>();
            Init();
            cards = save.Open();
            UpdateList();
            UpdateStackPanelRef();
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
            save.Save(cards);
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

    }
}
