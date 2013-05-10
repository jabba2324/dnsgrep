using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AWSroute53tool
{
    class CreateRecordResourcesHelper
    {
        StreamReader csvReader;
        List<List<List<String>>> recordResources = new List<List<List<String>>>();


        public CreateRecordResourcesHelper(String recordResourcesDir)
        {
            csvReader = new StreamReader(recordResourcesDir);
            String bin;
            
            int count_x = 0;
            recordResources.Add(new List<List<String>>());
            while((bin = csvReader.ReadLine()) != null)
            {
                if (bin == ",,")
                {
                    count_x++;
                    recordResources.Add(new List<List<String>>());
                }
                else
                {
                    recordResources[count_x].Add(new List<String>(bin.Split(',')));
                }
            }
        }

        public List<String> getDomainName(int x)
        {
            List<String> bin = new List<String>();

            for (int count = 0;count < recordResources[x].Count;count++ )
            {
                bin.Add(recordResources[x][count][0]);
            }
            return bin;
        }

        public List<String> getRecordType(int x)
        {
            List<String> bin = new List<String>();

            for (int count = 0; count < recordResources[x].Count; count++)
            {
                bin.Add(recordResources[x][count][1]);
            }
            return bin;
        }

        public List<String> getResourceRecordValues(int x)
        {
            List<String> bin = new List<String>();

            for (int count = 0; count < recordResources[x].Count; count++)
            {
                bin.Add(recordResources[x][count][2]);
            }
            return bin;
        }

        public List<String> getLoadBalancerZones(int x)
        {
            List<String> bin = new List<String>();

            for (int count = 0; count < recordResources[x].Count; count++)
            {
                if((count % 2) != 0)
                {
                bin.Add(recordResources[x][count][3]);
                }
            }
            return bin;
        }

        public int getCsvLength()
        {
            return recordResources.Count;
        }
    }
}
