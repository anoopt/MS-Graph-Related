//This method uses HTTP Client to send the request.
//MSAL library can also be used instead of this to get the Token.
//https://github.com/microsoftgraph/console-csharp-snippets-sample/blob/master/console-csharp-snippets-sample/AuthenticationHelper.cs

private async static Task<string> GetToken()
{
    string tokenToReturn = GetTokenFromCache();
    TimeSpan tokenTimeSpan = new TimeSpan(0, 0, 0);

    if (String.IsNullOrEmpty(tokenToReturn))
    {
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        //EnvironmentConfigurationManager is custom class which gets data from app config or app settings

        string grantType = EnvironmentConfigurationManager.GetSetting("AzureAdAppGrantType"); //client_credentials
        string clientId = EnvironmentConfigurationManager.GetSetting("AzureAdAppId");
        string clientSecret = EnvironmentConfigurationManager.GetSetting("AzureAdAppSecret");
        string scope = EnvironmentConfigurationManager.GetSetting("AzureAdAppScope"); //https://graph.microsoft.com/.default

        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", grantType),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("scope", scope)
        });

        
        string path = $"https://login.microsoftonline.com/{EnvironmentConfigurationManager.GetSetting("TenantId")}/oauth2/v2.0/token";

        HttpResponseMessage response = await client.PostAsync(path, formContent);
        if (response.IsSuccessStatusCode)
        {
            ErrorOccurred = false;
            ErrorMessage = string.Empty;
            var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();
            tokenToReturn = tokenResponse.access_token;
            if (!String.IsNullOrEmpty(tokenResponse.expires_in))
            {
                tokenTimeSpan = new TimeSpan(0, 0, Convert.ToInt32(tokenResponse.expires_in) - 10);
            }
        }
        else
        {
            ErrorOccurred = true;
            ErrorMessage = "Error in computing token : " + await response.Content.ReadAsStringAsync();
            tokenToReturn = string.Empty;
        }
        SaveTokenInCache(tokenToReturn, tokenTimeSpan);
    }
    return tokenToReturn;

}