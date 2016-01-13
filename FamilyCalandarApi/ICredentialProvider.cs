namespace SystemOut.CalandarApi
{
    public interface ICredentialProvider
    {
        CalendarCredential GetCredentials(string id);
    }
}