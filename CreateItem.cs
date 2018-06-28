private async static Task<CreatedItem> CreateItemUsingGraph(string token, EmailResponse emailResponse, string reporterName)
{
    HttpClient client = new HttpClient();
    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
    client.DefaultRequestHeaders.Add("Accept", "application/json");

    ItemToAdd itemToAdd = new ItemToAdd();
    itemToAdd.fields = new fields();
    itemToAdd.fields.Title = "Mail reported by : " + reporterName;
    itemToAdd.fields.Subject = emailResponse.subject;
    itemToAdd.fields.Body = emailResponse.body.content;
    itemToAdd.fields.BodyPreview = emailResponse.bodyPreview;
    itemToAdd.fields.From = emailResponse.from.emailAddress.address;
    itemToAdd.fields.To = HelperMethods.ComputeToAddress(emailResponse.toRecipients);
    itemToAdd.fields.Received = emailResponse.receivedDateTime;
    itemToAdd.fields.Sent = emailResponse.sentDateTime;
    itemToAdd.fields.Internal = emailResponse.from.emailAddress.address.Contains(EnvironmentConfigurationManager.GetSetting("DomainName")) ? true : false;

    string path = $"https://graph.microsoft.com/v1.0/sites/{EnvironmentConfigurationManager.GetSetting("SharePointSiteId")}/lists/{EnvironmentConfigurationManager.GetSetting("SharePointListId")}/items";


    HttpResponseMessage response = await client.PostAsJsonAsync(path, itemToAdd);
    if (response.IsSuccessStatusCode)
    {
        ErrorOccurred = false;
        ErrorMessage = string.Empty;
        var responseItem = await response.Content.ReadAsAsync<CreatedItem>();
        return responseItem;
    }
    else
    {
        ErrorOccurred = true;
        ErrorMessage = "Error in creating item  : " + await response.Content.ReadAsStringAsync();
        return null;
    }
}