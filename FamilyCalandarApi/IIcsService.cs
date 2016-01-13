using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemOut.CalandarApi
{
    public interface IIcsService
    {
        string[] GetIcsContent(string path);
    }
}
