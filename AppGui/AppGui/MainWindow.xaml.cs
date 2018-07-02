using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using mmisharp;
using Newtonsoft.Json;
using MPC_HC.Domain;
using MPC_HC.Domain.Helpers;
using MPC_HC.Domain.Interfaces;
using MPC_HC.Domain.Services;

namespace AppGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MmiCommunication mmiC;
        private static MPCHomeCinema mpcHomeCinema;
        public MainWindow()
        {
            InitializeComponent();

            mpcHomeCinema = new MPCHomeCinema("http://localhost:13579");

            mmiC = new MmiCommunication("localhost",8000, "User1", "GUI");
            mmiC.Message += MmiC_MessageAsync;
            mmiC.Start();

        }

        private async void MmiC_MessageAsync(object sender, MmiEventArgs e)
        {
            Console.WriteLine(e.Message);
            var doc = XDocument.Parse(e.Message);
            var com = doc.Descendants("command").FirstOrDefault().Value;
            dynamic json = JsonConvert.DeserializeObject(com);

            Shape _s = null;
            var info = await mpcHomeCinema.GetInfo();
            Console.WriteLine($"{info.FileName} is playing");
            Console.WriteLine($"{info.State} is its state");
            switch ((string)json.recognized[0].ToString())
            {
                case "PLAY": 
                    await mpcHomeCinema.PlayAsync();
                    break;
                case "PAUSE":
                    await mpcHomeCinema.PauseAsync();
                    break;
                case "NEXT":
                    await mpcHomeCinema.NextAsync();
                    break;
                case "BACK":
                    await mpcHomeCinema.PrevAsync();
                    break;
                case "MUTE":
                    await mpcHomeCinema.MuteAsync();
                    break;
                case "UNMUTE":
                    await mpcHomeCinema.UnMuteAsync();
                    break;
                case "VUP":
                    info = await mpcHomeCinema.GetInfo();
                    if (info.VolumeLevel <= 80)
                    {
                        await mpcHomeCinema.SetVolumeLevel(info.VolumeLevel + 20);
                    }
                    break;
                case "VDOWN":
                    info = await mpcHomeCinema.GetInfo();
                    if (info.VolumeLevel >= 20)
                    {
                        await mpcHomeCinema.SetVolumeLevel(info.VolumeLevel - 20);
                    }
                    break;
            }

            /*App.Current.Dispatcher.Invoke(() =>
            {
                switch ((string)json.recognized[1].ToString())
                {
                    case "GREEN":
                        _s.Fill = Brushes.Green;
                        var mpcHomeCinema = new MPCHomeCinema("http://localhost:13579");
                        var result = mpcHomeCinema.PlayAsync();
                        break;
                    case "BLUE":
                        _s.Fill = Brushes.Blue;
                        break;
                    case "RED":
                        _s.Fill = Brushes.Red;
                        break;
                }
            });*/
            


        }
    }
}
