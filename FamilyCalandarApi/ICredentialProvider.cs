namespace SystemOut.CalandarApi
{
    interface ICredentialProvider
    {
        CalendarCredentials GetCredentials(string id);
    }
}