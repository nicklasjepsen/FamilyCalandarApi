using SystemOut.CalandarApi;

namespace UnitTestProject
{
    public class CredentialProviderMock : ICredentialProvider
    {
        public string ValidId { get; set; }
        public CalendarCredential GetCredentials(string id)
        {
            if (id == ValidId)
                return new CalendarCredential
                {
                    Type = "ICS",
                };
            return null;
        }
    }
}
