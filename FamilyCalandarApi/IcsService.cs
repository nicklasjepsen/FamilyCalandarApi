using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SystemOut.CalandarApi
{
    internal class IcsService : IIcsService
    {
        public string[] GetIcsContent(string path)
        {
            var wc = new WebClient();
            var data = wc.DownloadString(path);
            return data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }
    }
}
