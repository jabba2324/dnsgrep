using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AWSroute53tool
{


    class CreateHostedZonesHelper
    {
        StreamReader csvReader;
        List<List<String>> table = new List<List<String>>();

        public CreateHostedZonesHelper(String fileDir)
        {
            csvReader = new StreamReader(fileDir);

            String bin;
            while((bin = csvReader.ReadLine()) != null)
            {
             table.Add(new List<String>(bin.Split(',')));
            }
            csvReader.Close();
        }

        public String getDomain(int x)
        {
            return table[x][0];
        }

        public String getUniqueId(int x)
        {
            return table[x][1];
        }

        public String getComment(int x)
        {
            return table[x][2];
        }

        public int getCsvLength()
        {
            return table.Count;
        }
     }
}
