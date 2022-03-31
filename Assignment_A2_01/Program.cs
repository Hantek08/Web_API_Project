﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Assignment_A2_01.Models;
using Assignment_A2_01.Services;

namespace Assignment_A2_01
{
    class Program
    {
        static void Main(string[] args)
        {
            NewsService service = new NewsService();
            Task<NewsApiData> t1 = service.GetNewsAsync();

            Task.WaitAll(t1);
            if(t1?.Status == TaskStatus.RanToCompletion)
            {
                NewsApiData newsApiData = t1.Result;
                Console.WriteLine($"Top Headline:");
                //var TopHeadLineList = newsApiData.Articles.Select(a => a.Title).ToList();
                newsApiData.Articles.ForEach(a => Console.WriteLine(a.Title));
               
            }    
            else
            {
                Console.WriteLine("News service error.");
            }
        }
    }
}
