using GuardianTalesGuildRaidUpdater.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace GuardianTalesGuildRaidUpdater.Views
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

        private async void btnGetList_Click(object sender, RoutedEventArgs e)
        {
            Uri googledrive_about = new Uri("https://www.googleapis.com/drive/v3/about");
            HttpRequestMessage httpRequest = new()
            {
                Method = HttpMethod.Get,
                RequestUri = googledrive_about,
            };

            HttpClient httpClient = new HttpClient();
            var response = await httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode) return;

            string result = await response.Content.ReadAsStringAsync();

            Console.WriteLine(result);
        }
    }
}
