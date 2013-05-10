using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Windows.Controls;
using System.IO;
using System.Xml.Linq;

namespace AWSroute53tool
{
    class AmazonWebServicesAPI
    {

        Logger logger;
        String requestBody;
        String date;
        StreamWriter streamWriter;

        public AmazonWebServicesAPI(Logger logger)
        {
            this.logger = logger;

        }

        private String createChanges(List<String> domainName, List<String> recordType, String TTL, List<String> resourceRecordValue, List<String> loadBalancerZones)
        {
            String changes = "<Changes>";
            int loadBalancerZoneCount = 0;
            for (int count = 0; count < domainName.Count; count++)
            {
                changes += "<Change>" +
                              "<Action>CREATE</Action>" +
                                  "<ResourceRecordSet>" +
                                      "<Name>" + domainName[count] + "</Name>" +
                                       "<Type>" + recordType[count] + "</Type>";

                if (recordType[count] == "A")
                {
                    changes += "<AliasTarget>" +
                                  "<HostedZoneId>" + loadBalancerZones[loadBalancerZoneCount] + "</HostedZoneId>" +
                                   "<DNSName>" + resourceRecordValue[count] + "</DNSName>" +
                                
                               "</AliasTarget>";
                    loadBalancerZoneCount++;
                }
                else
                {
                    changes += "<TTL>" + TTL + "</TTL>" +
                               "<ResourceRecords>" +
                                "<ResourceRecord><Value>" + resourceRecordValue[count] + "</Value></ResourceRecord>" +
                               "</ResourceRecords>";
                         
                  
                }
                changes += "</ResourceRecordSet>" +
                  "</Change>";
            }

               changes += "</Changes>";
            return changes;
        }

        public void createResourceRecordRequest(String hostedZone, List<String> domainName, List<String> recordType, String TTL, List<String> resourceRecordValues, List<String> loadBalancerZones, String auth, String date)
        {

            HttpWebRequest request;
            request = (HttpWebRequest)WebRequest.Create("https://route53.amazonaws.com/2011-05-05" + hostedZone + "/rrset");
            request.Method = "POST";

            request.Headers.Add("x-amzn-authorization", auth);
            request.Headers.Add("x-amz-date", date);
            request.ContentType = "text/plain";

            requestBody = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                        "<ChangeResourceRecordSetsRequest xmlns=\"https://route53.amazonaws.com/doc/2011-05-05/\">" +
                                            "<ChangeBatch>" +
                                                   "<Comment>This is a test request</Comment>" +
                                                       createChanges(domainName, recordType, TTL, resourceRecordValues, loadBalancerZones) +
                                             "</ChangeBatch>" +
                                        "</ChangeResourceRecordSetsRequest>").ToString();

            request.ContentLength = requestBody.Length;

            streamWriter = new StreamWriter(request.GetRequestStream());
            streamWriter.Write(requestBody);
            streamWriter.Close();

            getAwsResponse(request);

        }

        public void createHostedZoneRequest(String hostedZone, String callerRefrence, String comment, String auth, String date)
        {

            HttpWebRequest request;
            request = (HttpWebRequest)WebRequest.Create("https://route53.amazonaws.com/2011-05-05/hostedzone");
            request.Method = "POST";

            request.Headers.Add("x-amzn-authorization", auth);
            request.Headers.Add("x-amz-date", date);
            request.ContentType = "text/plain";

            requestBody = XDocument.Parse("<?xml version=\"1.0\" encoding= \"UTF-8\"?>" +
                                                    "<CreateHostedZoneRequest xmlns=\"https://route53.amazonaws.com/doc/2011-05-05/\">" +
                                                       "<Name>" + hostedZone + "</Name>" +
                                                           "<CallerReference>" + callerRefrence + "</CallerReference>" +
                                                                "<HostedZoneConfig>" +
                                                                   "<Comment>" + comment + "</Comment>" +
                                                                "</HostedZoneConfig>" +
                                                    "</CreateHostedZoneRequest>").ToString();
            request.ContentLength = requestBody.Length;

            streamWriter = new StreamWriter(request.GetRequestStream());
            streamWriter.Write(requestBody);
            streamWriter.Close();

            getAwsResponse(request);



        }

        public String requestDate()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://route53.amazonaws.com/date");
            
            WebResponse response = getAwsResponse(request);
 
                    String[] headers = response.Headers.GetValues(3);
                    date = headers[0];
                    response.Close();

            return date;
        }

        public List<String> requestHostedZoneIds(int hostedZonesCount, String auth, String date)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://route53.amazonaws.com/2011-05-05/hostedzone?maxitems=" + hostedZonesCount);

            request.Headers.Add("x-amzn-authorization", auth);
            request.Headers.Add("x-amz-date", date);

            WebResponse response = getAwsResponse(request);

            List<String> hostedZoneIds = new List<String>();

            XmlDocument responseBody = new XmlDocument();
            responseBody.LoadXml(new StreamReader(response.GetResponseStream()).ReadToEnd());

            foreach (XmlNode hostedZoneId in responseBody.GetElementsByTagName("Id"))
            {
                hostedZoneIds.Add(hostedZoneId.InnerText);
            }

            response.Close();
            return hostedZoneIds;
        }

        private WebResponse getAwsResponse(WebRequest request)
        {
            WebResponse response = null;

            try
            {
                response = request.GetResponse();

                logger.LOG("Tx \n" + request.Headers.ToString());
                logger.LOG(requestBody);

                logger.LOG("Rx \n" + response.Headers.ToString());
            }
            catch (WebException webEx)
            {
                try
                {
                    logger.LOG("Tx \n" + request.Headers.ToString());
                    logger.LOG(requestBody);

                    logger.LOG("Rx \n" + webEx.Response.Headers.ToString());
                    logger.LOG(webEx.Response.GetResponseStream());
                }
                catch (Exception Ex)
                {
                    logger.LOG(Ex.ToString());
                }
            }
            return response;

        }

    }
}
