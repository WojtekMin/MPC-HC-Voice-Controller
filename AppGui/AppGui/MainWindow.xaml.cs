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
using multimodal;

namespace AppGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MmiCommunication mmiC;
        private static MPCHomeCinema mpcHomeCinema;
        private Tts t = new Tts();
        public MainWindow()
        {
            InitializeComponent();

            mpcHomeCinema = new MPCHomeCinema("http://localhost:13579");

            mmiC = new MmiCommunication("localhost",8000, "User1", "GUI");
            mmiC.Message += MmiC_Message;
            mmiC.Start();

        }

        private async void MmiC_Message(object sender, MmiEventArgs e)
        {
            Console.WriteLine(e.Message);
            var doc = XDocument.Parse(e.Message);
            var com = doc.Descendants("command").FirstOrDefault().Value;
            dynamic json = JsonConvert.DeserializeObject(com);

            //double confidence = Double.Parse((string)json.recognized[0].ToString());
            String command = (string)json.recognized[0].ToString();
            String movie = (string)json.recognized[1].ToString();
            String number = (string)json.recognized[2].ToString();
            String time = (string)json.recognized[3].ToString();

            var info = await mpcHomeCinema.GetInfo();
            Console.WriteLine($"{info.FileName} is playing");
            Console.WriteLine($"{info.State} is its state");
            switch (command)
            {
                case "HELP":
                    if (movie == "EMP" && number == "EMP" && time == "EMP")
                    {
                        t.Speak("Hello. You can play or pause movie, change volume, mute or unmute, skip movie or play last one, skip or rewind given time stamp, or open new movie.");
                    }
                    else
                    {
                        t.Speak("This command is forbidden");
                    }
                    break;
                case "PLAY":
                    if (movie == "EMP" && number == "EMP" && time == "EMP")
                    {
                        await mpcHomeCinema.PlayAsync();
                    }
                    else
                    {
                        t.Speak("This command is forbidden");
                    }
                    break;
                case "PAUSE":
                    if (movie == "EMP" && number == "EMP" && time == "EMP")
                    {
                        await mpcHomeCinema.PauseAsync();
                    }
                    else
                    {
                        t.Speak("This command is forbidden");
                    }
                    break;
                case "NEXT":
                    if (movie == "EMP" && number == "EMP" && time == "EMP")
                    {
                        await mpcHomeCinema.NextAsync();
                    }
                    else
                    {
                        t.Speak("This command is forbidden");
                    }
                    break;
                case "BACK":
                    if (movie == "EMP" && number == "EMP" && time == "EMP")
                    {
                        await mpcHomeCinema.PrevAsync();
                    }
                    else
                    {
                        t.Speak("This command is forbidden");
                    }
                    break;
                case "MOVEFWD":
                    info = await mpcHomeCinema.GetInfo();
                    if (movie == "EMP" && number != "EMP" && time != "EMP")
                    {
                        await App.Current.Dispatcher.Invoke(async () =>
                        {
                            
                            if (time == "HOUR")
                            {
                                await mpcHomeCinema.SetPosition(new TimeSpan(info.Position.Hours + int.Parse(number), info.Position.Minutes, info.Position.Seconds));
                            }
                            else if (time == "MINUTE")
                            {
                                await mpcHomeCinema.SetPosition(new TimeSpan(info.Position.Hours, info.Position.Minutes + int.Parse(number), info.Position.Seconds));
                            }
                            else if (time == "SECOND")
                            {
                                await mpcHomeCinema.SetPosition(new TimeSpan(info.Position.Hours, info.Position.Minutes, info.Position.Seconds + int.Parse(number)));
                            }
                        });
                    }
                    else
                    {
                        t.Speak("This command is forbidden");
                    }
                    break;
                case "MOVEBWD":
                    info = await mpcHomeCinema.GetInfo();
                    if (movie == "EMP" && number != "EMP" && time != "EMP")
                    {
                        await App.Current.Dispatcher.Invoke(async () =>
                        {

                            if (time == "HOUR")
                            {
                                await mpcHomeCinema.SetPosition(new TimeSpan(info.Position.Hours - int.Parse(number), info.Position.Minutes, info.Position.Seconds));
                            }
                            else if (time == "MINUTE")
                            {
                                await mpcHomeCinema.SetPosition(new TimeSpan(info.Position.Hours, info.Position.Minutes - int.Parse(number), info.Position.Seconds));
                            }
                            else if (time == "SECOND")
                            {
                                await mpcHomeCinema.SetPosition(new TimeSpan(info.Position.Hours, info.Position.Minutes, info.Position.Seconds - int.Parse(number)));
                            }
                        });
                    }
                    else
                    {
                        t.Speak("This command is forbidden");
                    }
                    break;
                case "SETTIME":
                    if (movie == "EMP" && number != "EMP" && time != "EMP")
                    {
                        await App.Current.Dispatcher.Invoke(async () =>
                        {
                            if (time == "HOUR")
                            {
                                await mpcHomeCinema.SetPosition(new TimeSpan(int.Parse(number), 0, 0));
                            }
                            else if (time == "MINUTE")
                            {
                                await mpcHomeCinema.SetPosition(new TimeSpan(0, int.Parse(number), 0));
                            }
                            else if (time == "SECOND")
                            {
                                await mpcHomeCinema.SetPosition(new TimeSpan(0, 0, int.Parse(number)));
                            }
                        });
                    }
                    else
                    {
                        t.Speak("This command is forbidden");
                    }
                    break;
                case "MUTE":
                    if (movie == "EMP" && number == "EMP" && time == "EMP")
                    {
                        await mpcHomeCinema.MuteAsync();
                    }
                    else
                    {
                        t.Speak("This command is forbidden");
                    }
                    break;
                case "UNMUTE":
                    if (movie == "EMP" && number == "EMP" && time == "EMP")
                    {
                        await mpcHomeCinema.UnMuteAsync();
                    }
                    else
                    {
                        t.Speak("This command is forbidden");
                    }
                    break;
                case "SETVOL":
                    if (number == "EMP" && time != "EMP" && movie != "EMP")
                    {
                        t.Speak("This command is forbidden");
                    }
                    else
                    {
                        info = await mpcHomeCinema.GetInfo();
                        await App.Current.Dispatcher.Invoke(async () =>
                        {
                            await mpcHomeCinema.SetVolumeLevel(int.Parse(number));
                        });
                    }
                    break;
                case "VUP":
                    info = await mpcHomeCinema.GetInfo();
                    if (movie == "EMP" && number == "EMP" && time == "EMP")
                    {
                        if (info.VolumeLevel <= 80)
                        {
                            await mpcHomeCinema.SetVolumeLevel(info.VolumeLevel + 20);
                        }
                        else
                        {
                            t.Speak("You can only increase volume by 20 points with this command. If you want to do it with custom number of points, please use set volume command,");
                        }
                    }
                    else
                    {
                        t.Speak("This command is forbidden");
                    }
                    break;
                case "VDOWN":
                    info = await mpcHomeCinema.GetInfo();
                    if (movie == "EMP" && number == "EMP" && time == "EMP")
                    {
                        if (info.VolumeLevel >= 20)
                        {
                            await mpcHomeCinema.SetVolumeLevel(info.VolumeLevel - 20);
                        }
                        else
                        {
                            t.Speak("You can only decrease volume by 20 points with this command. If you want to do it with custom number of points, please use set volume command,");
                        }
                    }
                    else
                    {
                        t.Speak("This command is forbidden");
                    }
                    break;
                case "OPENFILE":
                    if (movie == "EMP" || number != "EMP" || time != "EMP")
                    {
                        t.Speak("This command is forbidden");
                    }
                    else
                    {
                        await App.Current.Dispatcher.Invoke(async () =>
                         {
                             switch (movie)
                             {
                                 case "ANIMALS":
                                     await mpcHomeCinema.OpenFileAsync("C:\\Users\\Public\\Videos\\SampleVideos\\Animals.wmv");
                                     break;
                                 case "ODYSSEY":
                                     await mpcHomeCinema.OpenFileAsync("C:\\Users\\Public\\Videos\\SampleVideos\\2001_ A SPACE ODYSSEY - Trailer.mp4");
                                     break;
                                 case "BLUEPLANET":
                                     await mpcHomeCinema.OpenFileAsync("C:\\Users\\Public\\Videos\\SampleVideos\\Blue Planet II  The Prequel.wmv");
                                     break;
                                 case "PLANETEARTH":
                                     await mpcHomeCinema.OpenFileAsync("C:\\Users\\Public\\Videos\\SampleVideos\\Planet Earth II Official Extended Trailer  BBC Earth.wmv");
                                     break;
                             }
                         });
                    }
                    break;
            }
        }
    }
}
