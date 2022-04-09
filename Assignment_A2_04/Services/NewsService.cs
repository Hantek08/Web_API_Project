#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Assignment_A2_04.Models;
using Assignment_A2_04.ModelsSampleData;
using System.Collections.Generic;

namespace Assignment_A2_04.Services
{
    public class NewsService
    {
        public EventHandler<string> NewsApiAvailable;
        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "d318329c40734776a014f9d9513e14ae";


        public async Task<News> GetNewsAsync(NewsCategory category)
        {

//#if UseNewsApiSample      
//            NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync(category);

//#else

            // your code to get live data

            News news = null;
            //NewsCategory newsCategory = category;
            NewsCacheKey key = new NewsCacheKey(category);

            if (!key.CacheExist)
            {
                //https://newsapi.org/docs/endpoints/top-headlines
                var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}&apiKey={apiKey}";
                news = await ReadNewsApiAsync(uri);
                News.Serialize(news,key.FileName);
                OnNewsApiAvailable($"News in category is available: {category}");

            }
            else
            {
                news = News.Deserialize(key.FileName);
                OnNewsApiAvailable($"XML Cached news in category is available: {category}");

            }
            return news;
        }


        //This method is responsible to raise the event
        protected virtual void OnNewsApiAvailable(string s)
        {
            NewsApiAvailable?.Invoke(this, s);
        }

        private NewsItem GetNewsArticle(Article ndArticle)
        {

            NewsItem newsItemArticle = new NewsItem();
            newsItemArticle.DateTime = ndArticle.PublishedAt;
            newsItemArticle.Title = ndArticle.Title;
            newsItemArticle.Description = ndArticle.Description;

            return newsItemArticle;
        }


        private async Task<News> ReadNewsApiAsync(string uri)
        {
            // part of your read web api code here
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            NewsApiData nd = await response.Content.ReadFromJsonAsync<NewsApiData>();

            //Your Code to convert WeatherApiData to Forecast using Linq.
            News news = new News();
            news.Articles = new List<NewsItem>();

            nd.Articles.ForEach(a => news.Articles.Add(GetNewsArticle(a)));


            return news;
        }
    }
}
