using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemOut.CalandarApi
{
    public class CredentialProviderMock : ICredentialProvider
    {
        public CalendarCredentials GetCredentials(string id)
        {
            // TODO: Implement your own credential store
            return new CalendarCredentials
            {
                Username = "user",
                Password = "password",
                Domain = "Domain"
            };
        }
    }
}
