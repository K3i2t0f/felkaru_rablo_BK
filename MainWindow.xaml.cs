using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace felkaru_rablo_BK
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random();

        int reelSymbolCount = 10;
       

        int balance = 100;
        int spinCost = 10;

        List<string> symbols = new List<string>()
        {
            //ide jonnek a kepek
        };

        public MainWindow()
        {
            InitializeComponent();

            UpdateBalance();

            SetStartImages();
        }

        void UpdateBalance()
        {
            BalanceText.Text = balance.ToString();
        }

        void SetImage(Image imageControl, string path)
        {
            imageControl.Source =
                new BitmapImage(new Uri(path, UriKind.Relative));
        }

        void SetStartImages()
        {
            SetReel(Reel1Top, Reel1Middle, Reel1Bottom);
            SetReel(Reel2Top, Reel2Middle, Reel2Bottom);
            SetReel(Reel3Top, Reel3Middle, Reel3Bottom);
        }

        string SetReel(Image top, Image middle, Image bottom)
        {
            int middleIndex = random.Next(reelSymbolCount);

            int topIndex =
                (middleIndex - 1 + reelSymbolCount) % reelSymbolCount;

            int bottomIndex =
                (middleIndex + 1) % reelSymbolCount;

            SetImage(top, symbols[topIndex]);
            SetImage(middle, symbols[middleIndex]);
            SetImage(bottom, symbols[bottomIndex]);

            return symbols[middleIndex];
        }

        async Task<string> SpinReel(
            Image top,
            Image middle,
            Image bottom,
            int spinTime)
        {
            string finalSymbol = "";

            int loops = spinTime / 80;

            for (int i = 0; i < loops; i++)
            {
                finalSymbol = SetReel(top, middle, bottom);

                await Task.Delay(80);
            }

            return finalSymbol;
        }

        private async void SpinButton_Click(object sender, RoutedEventArgs e)
        {
            if (balance < spinCost)
            {
                ResultText.Text = "❌ Nincs elég kredited!";
                return;
            }

            SpinButton.IsEnabled = false;

            balance -= spinCost;

            UpdateBalance();

            ResultText.Text = "🎰 Pörgetés...";

            // Tárcsák pörgetése külön időkkel

            Task<string> reel1Task = SpinReel(
                Reel1Top,
                Reel1Middle,
                Reel1Bottom,
                1500);

            Task<string> reel2Task = SpinReel(
                Reel2Top,
                Reel2Middle,
                Reel2Bottom,
                2200);

            Task<string> reel3Task = SpinReel(
                Reel3Top,
                Reel3Middle,
                Reel3Bottom,
                3000);

            string reel1 = await reel1Task;
            string reel2 = await reel2Task;
            string reel3 = await reel3Task;

            // NYERÉS ELLENŐRZÉS

            if (reel1 == reel2 && reel2 == reel3)
            {
                balance += 50;

                ResultText.Text = "🔥 JACKPOT! +50 kredit";
            }
            else if (
                reel1 == reel2 ||
                reel1 == reel3 ||
                reel2 == reel3)
            {
                balance += 20;

                ResultText.Text = "✨ Nyertél! +20 kredit";
            }
            else
            {
                ResultText.Text = "💀 Vesztettél!";
            }

            UpdateBalance();

            SpinButton.IsEnabled = true;
        }
    }
}