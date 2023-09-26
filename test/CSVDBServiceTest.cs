using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

public class CSVDBServiceTest{
    
    [Fact]
    public async Task CsvDatabase_ReadFromCsvFileAsync()
    {
        // Create an HTTP client object
        //url can be different for each pc
        string baseURL = "http://localhost:5277";
        HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(baseURL);

        // Send an asynchronous HTTP GET request and automatically construct a Cheep object from the
        // JSON object in the body of the response
        HttpResponseMessage responseMessage = await client.GetAsync("cheeps");
        
        //assert
        Assert.Equal(200,(int)responseMessage.StatusCode);
        Cheep responseCheep = await responseMessage.Content.ReadFromJsonAsync<Cheep>();
    }

 [Fact]
    public async Task CsvDatabase_StoreInCsvFileAsync()
    {
        //arrange
        double timestamp = 1695034276;
        string author = "Henrik";
        string message = "boomba";
        Cheep testCheep = new Cheep(timestamp, author, message);

        //act
        // Create an HTTP client object
        // url can be different for each pc
        string baseURL = "http://localhost:5277";
        HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(baseURL);

        //send post request
        using HttpResponseMessage responseMessage = await client.PostAsJsonAsync("cheep",testCheep);
        Assert.Equal(201,(int)responseMessage.StatusCode);
        Cheep responseCheep = await responseMessage.Content.ReadFromJsonAsync<Cheep>();
        Assert.Equal(testCheep, responseCheep);
    }
}