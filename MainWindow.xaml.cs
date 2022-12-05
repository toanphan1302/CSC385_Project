using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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

namespace CSC385_Project
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

        private List<string> PrepData()
        {
            List<string> output = new List<string>();
            resultsWindow.Text = "";
            output.Add("https://www.google.com/");
            output.Add("https://www.youtube.com/");
            output.Add("https://www.cnn.com/");
            output.Add("https://www.stmartin.edu/");
            output.Add("https://www.twitter.com/");
            output.Add("https://www.facebook.com/");
            return output;
        }

        private async Task RunDownLoadAsync()
        {
            var websites = PrepData();

            foreach (var site in websites)
            {
                var results = await Task.Run(() => DownloadWebsite(site));
                ReportWebsiteInfo(results);
            }
        }

        private async Task RunDownLoadParallelAsync()
        {
            var websites = PrepData();
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

            foreach (var site in websites)
            {
                tasks.Add(Task.Run(() => DownloadWebsite(site)));
            }
            var results = await Task.WhenAll(tasks);
            foreach(var item in results)
            {
                ReportWebsiteInfo(item);
            }
        }

        private void RunDownLoadSync()
        {
            var websites = PrepData();

            foreach(var site in websites)
            {
                var results = DownloadWebsite(site);
                ReportWebsiteInfo(results);
            }
        }

        private WebsiteDataModel DownloadWebsite(string websiteURL)
        {
            var output = new WebsiteDataModel();
            var client = new WebClient();
            output.WebsiteUrl = websiteURL;
            output.WebsiteData = client.DownloadString(websiteURL);

            return output;
        }
        private void ReportWebsiteInfo(WebsiteDataModel data)
        {
            resultsWindow.Text += $"{data.WebsiteUrl} downloaded: {data.WebsiteData.Length} charaters long \n";
        }


        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = Stopwatch.StartNew();
            RunDownLoadSync();
            watch.Stop();
            var time = watch.ElapsedMilliseconds;
            resultsWindow.Text += $"Total execution time: {time}";
        }

        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = Stopwatch.StartNew();
            await RunDownLoadAsync();
            watch.Stop();
            var time = watch.ElapsedMilliseconds;
            resultsWindow.Text += $"Total execution time: {time}";
        }

        private async void executeAsyncParallel_Click(object sender, RoutedEventArgs e)
        {
            var watch = Stopwatch.StartNew();
            await RunDownLoadParallelAsync();
            watch.Stop();
            var time = watch.ElapsedMilliseconds;
            resultsWindow.Text += $"Total execution time: {time}";
        }
    }

    public class WebsiteDataModel
    {
        public string WebsiteUrl { get; set; } = "";
        public string WebsiteData { get; set; } = "";
    }
}
