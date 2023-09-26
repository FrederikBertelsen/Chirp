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
        HttpResponseMessage responseMessage = await client.GetAsync("cheeps/3");
        
        //assert
        Assert.Equal(200,(int)responseMessage.StatusCode);
        List<Cheep> responseCheeps = await responseMessage.Content.ReadFromJsonAsync<List<Cheep>>();
        Assert.Equal(responseCheeps.Count(),3);
        Assert.Equal(responseCheeps[0], new Cheep(1690891760,"ropf","Hello, BDSA students!"));

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