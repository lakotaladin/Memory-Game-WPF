using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Igra_pamcenja
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Deklaracija klase za igru pamćenja
        memoryGame currentGame;
        // Niz dugmadi koji predstavljaju pločice u igri
        Button[] buttons = new Button[20];
        // Generator slučajnih brojeva
        Random rndGenerate = new Random();
        // Niz koji čuva brojeve na pločicama
        int[] profInputList = new int[20];

        // Metoda koja se poziva kada korisnik klikne na dugme "Reset"
        private void resetGame_Click(object sender, RoutedEventArgs e)
        {
            // Provera da li postoji aktivna igra
            if (currentGame != null)
            {
                // Resetovanje trenutne igre
                for (int i = 0; i < 20; i++)
                {
                    currentGame.Reset(buttons);
                }
                currentGame = null;
            }
        }

        // Metoda koja se poziva kada korisnik klikne na dugme "Start Game"
        private void startGame_Click(object sender, RoutedEventArgs e)
        {
            // Provera da li postoji aktivna igra
            if (currentGame == null)
            {
                // Generisanje slučajnih brojeva za pločice
                var test = Enumerable.Range(1, 20).OrderBy(x => rndGenerate.Next()).ToArray();
                for (int i = 0; i < 20; i++)
                {
                    // Inicijalizacija niz dugmadi i profInputList sa slučajno generisanim brojevima
                    buttons[i] = wrapPanel.Children[i] as Button;
                    profInputList[i] = (test[i] - 1) % 10 + 1;
                }
                // Kreiranje nove instance igre
                currentGame = new memoryGame(buttons, profInputList);
            }
        }

        // Metoda koja se poziva kada korisnik klikne na dugme na pločici tokom igre
        private void mtpButtonsOnclick(object sender, RoutedEventArgs e)
        {
            // Provera da li postoji aktivna igra
            if (currentGame != null)
            {
                // Dobijanje indeksa kliknutog dugmeta
                int buttonName = int.Parse((sender as Button).Name.Substring(6)) - 1;
                // Pozivanje odgovarajuće metode u klasi memoryGame radi dalje obrade
                currentGame.classOnclick(buttons, buttonName);
            }
        }

        // Unutrašnja klasa koja definiše logiku igre pamćenja
        public class memoryGame
        {
            // Niz koji čuva brojeve na pločicama
            int[] testarray = new int[20];
            // Brojač pogodaka
            int conditionCounter = 0;
            // Niz dugmadi koji predstavljaju pločice u igri
            private Button[] xBox;
            // Brojač kliknutih dugmadi
            int counter = 0;
            // Niz koji čuva indekse kliknutih dugmadi
            int[] pressed = new int[2];
            // Niz koji čuva informaciju da li je pločica otkrivena
            bool[] opened = new bool[20];

            // Konstruktor klase
            public memoryGame(Button[] box, int[] TArray)
            {
                // Inicijalizacija nizova i postavljanje brojeva na pločicama na upitnike
                xBox = box;
                testarray = TArray;

                for (int i = 0; i < 20; i++)
                {
                    // Postavljanje sadržaja dugmadi na upitnike
                    box[i].Content = "?";
                    // Postavljanje veličine fonta za upitnike
                    box[i].FontSize = 20;
                }
            }

            // Metoda koja se poziva kada korisnik klikne na dugme tokom igre
            public void classOnclick(Button[] box, int index)
            {
                // Provera uslova za dalju obradu
                if (counter == 2 || opened[index] || counter == 1 && pressed[0] == index)
                    return;

                // Prikazivanje broja koji odgovara kliknutom dugmetu na pločici
                xBox[index].Content = testarray[index].ToString();
                // Postavljanje debljine fonta na normalnu
                xBox[index].FontWeight = FontWeights.Normal;
                pressed[counter] = index;
                counter++;
                if (counter == 2)
                {
                    // Pozivanje metode za upoređivanje dva kliknuta dugmeta
                    buttonCompare(box, pressed[0], pressed[1]);
                }
            }

            // Metoda koja upoređuje dva kliknuta dugmeta
            public void buttonCompare(Button[] box, int check1, int check2)
            {
                // Provera da li su brojevi na dve pločice isti
                if (testarray[check1] == testarray[check2])
                {
                    // Povećavanje brojača pogodaka
                    conditionCounter++;
                    // Označavanje da su dve pločice pogodjene
                    opened[check1] = true;
                    opened[check2] = true;
                    if (conditionCounter == 10)
                    {
                        // Prikazivanje poruke o pobedi ako su sve pločice pogodjene
                        MessageBox.Show("Pobedio si!!!!");
                    }
                    counter = 0;
                }
                else
                {
                    // Pokretanje tajmera za vraćanje upitnika na pločicama nakon kratkog vremena
                    var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
                    timer.Start();
                    timer.Tick += (sender, args) =>
                    {
                        timer.Stop();
                        // Postavljanje sadržaja dugmadi na upitnike
                        xBox[check1].Content = "?";
                        xBox[check2].Content = "?";
                        counter = 0;
                    };
                }
            }

            // Metoda za resetovanje pločice
            public void Reset(Button[] box)
            {
                for (int i = 0; i < 20; i++)
                {
                    // Postavljanje sadržaja dugmadi na upitnike
                    xBox[i].Content = "?";
                }
            }
        }

    }
}