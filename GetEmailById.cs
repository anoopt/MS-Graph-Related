private async static Task<EmailResponse> GetEmailUsingGraph(string token, string messageId, string emailId)
{
    HttpClient client = new HttpClient();
    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
    client.DefaultRequestHeaders.Add("Accept", "application/json");

    string path = $"https://graph.microsoft.com/v1.0/users/{emailId}/messages/{messageId}";

    HttpResponseMessage response = await client.GetAsync(path);
    if (response.IsSuccessStatusCode)
    {
        ErrorOccurred = false;
        ErrorMessage = string.Empty;
        var emailResponse = await response.Content.ReadAsAsync<EmailResponse>();
        return emailResponse;
    }
    else
    {
        ErrorOccurred = true;
        ErrorMessage = "Error in getting email  : " + await response.Content.ReadAsStringAsync();
        return null;
    }
}

//Email Response Data object structure

public class EmailResponse
{
    public DateTime receivedDateTime { get; set; }
    public DateTime sentDateTime { get; set; }
    public string subject { get; set; }
    public from from { get; set; }
    public List<toRecipients> toRecipients { get; set; }
    public body body { get; set; }
    public string bodyPreview { get; set; }
}

public class from
{
    public emailAddress emailAddress { get; set; }
}

public class toRecipients
{
    public emailAddress emailAddress { get; set; }
}

public class emailAddress
{
    public string name { get; set; }
    public string address { get; set; }
}

public class body
{
    public string contentType { get; set; }
    public string content { get; set; }
}