using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace AWSroute53tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Logger logger;
        AmazonWebServicesAPI aws;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        public String generateAuth(String accessKey, String secretKey, String date)
        {
            byte[] secretArray = Encoding.UTF8.GetBytes(secretKey);
            byte[] dateArray = Encoding.UTF8.GetBytes(date);


            HMACSHA1 hash = new HMACSHA1(secretArray);
            hash.Initialize();
            return "AWS3-HTTPS AWSAccessKeyId=" + accessKey + ",Algorithm=HMACSHA1,Signature=" + Convert.ToBase64String(hash.ComputeHash(dateArray));
        }

        private void CreateHostedZone_Click(object sender, RoutedEventArgs e)
        {
            logger = new Logger(SystemLog);
            aws = new AmazonWebServicesAPI(logger);
            CreateHostedZonesHelper hostedZonesTable = new CreateHostedZonesHelper("hostedZones.csv");
            String secretKey = this.accessKey.Text;
            String accessKey = this.secretKey.Text;
            String date = aws.requestDate();
            String auth = generateAuth(accessKey, secretKey, date);

            for (int count = 0; count < hostedZonesTable.getCsvLength(); count++)
            {
                aws.createHostedZoneRequest(hostedZonesTable.getDomain(count), hostedZonesTable.getUniqueId(count), hostedZonesTable.getComment(count), auth, date);
                Thread.Sleep(50);
            }

        }

        private void CreateRecordResources_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Int32.Parse(ttl.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid TTL");
                return;
            }
            try
            {
                Int32.Parse(this.offset.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid Offset");
                return;
            }

            String secretKey = this.accessKey.Text;
            String accessKey = this.secretKey.Text;
            int offset = Int32.Parse(this.offset.Text);
            logger = new Logger(SystemLog);
            aws = new AmazonWebServicesAPI(logger);
            CreateRecordResourcesHelper resourceRecordsHelper = new CreateRecordResourcesHelper("recordResources.csv");

            String date = aws.requestDate();
            String auth = generateAuth(accessKey, secretKey, date);
            List<String> hostedZones = aws.requestHostedZoneIds(resourceRecordsHelper.getCsvLength() + offset, auth, date);

            for (int count = 0; count < resourceRecordsHelper.getCsvLength(); count++)
            {
                aws.createResourceRecordRequest(hostedZones[count + offset], resourceRecordsHelper.getDomainName(count), resourceRecordsHelper.getRecordType(count), ttl.Text, resourceRecordsHelper.getResourceRecordValues(count), resourceRecordsHelper.getLoadBalancerZones(count), auth, date);
                Thread.Sleep(50);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }
}
