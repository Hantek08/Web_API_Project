#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Threading.Tasks;

using Assignment_A2_01.Models;
using Assignment_A2_01.ModelsSampleData;
namespace Assignment_A2_01.Services
{
    public class NewsService
    {
        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "d318329c40734776a014f9d9513e14ae";
        public async Task<NewsApiData> GetNewsAsync()
        {

        // if: UseNewsApiSample      
            /*NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync("sports");
            
            NewsApiData news =  new NewsApiData();
            //news.Status = nd.Status;
            news.Articles = new List<Article>();
            foreach (var item in nd.Articles)
            {
                news.Articles.Add(item);
            }
            */
            //https://newsapi.org/docs/endpoints/top-headlines
             var uri = $"https://newsapi.org/v2/top-headlines?country=se&category=sports&apiKey={apiKey}";
            //Read the response from the webApi 
            HttpResponseMessage respons = await httpClient.GetAsync(uri);
            respons.EnsureSuccessStatusCode();
            NewsApiData nd = await respons.Content.ReadFromJsonAsync<NewsApiData>();
            //Your code here to read live data
            NewsApiData news = new NewsApiData();
            news.Articles = new List<Article>();
            
            nd.Articles.ForEach(a => news.Articles.Add(a));


            return nd;
        }




    }
}
