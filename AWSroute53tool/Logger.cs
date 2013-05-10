using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;
using System.Xml.Linq;

namespace AWSroute53tool
{
    class Logger
    {
        TextBox SystemLog;
        StreamReader itemReader;

        public Logger(TextBox SystemLog)
        {
            this.SystemLog = SystemLog;
        }
        
        public void LOG(String logItem)
        {
            SystemLog.Text = SystemLog.Text + logItem + "\n";
        }

        public void LOG(Stream logItemStream)
        {
            itemReader = new StreamReader(logItemStream);
            String logItem = itemReader.ReadToEnd();
            itemReader.Close();
            this.LOG(XDocument.Parse(logItem).ToString());
     
        }
    }
}
