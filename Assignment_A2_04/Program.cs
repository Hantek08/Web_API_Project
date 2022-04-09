﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Assignment_A2_04.Models;
using Assignment_A2_04.Services;

namespace Assignment_A2_04
{
    class Program
    {
        static void Main(string[] args)
        {
            NewsService service = new NewsService();

            service.NewsApiAvailable += ReportNewsDataAvailable;
            Task<News> t1 = null, t2 = null;
            Exception exception = null;

            try
            {
                for (NewsCategory i = NewsCategory.business; i < NewsCategory.technology + 1; i++)
                {
                    t1 = service.GetNewsAsync(i);

                }
                t1.Wait();

                for (NewsCategory i = NewsCategory.business; i < NewsCategory.technology + 1; i++)
                {
                    t2 = service.GetNewsAsync(i);

                }
                t2.Wait();
            }
            catch (Exception ex)
            {

                exception = ex;
            }
            Console.WriteLine("---------------------------");
            for (NewsCategory i = NewsCategory.business; i < NewsCategory.technology + 1; i++)
            {

                Console.WriteLine($"News in Category {i}");
                if (t1?.Status == TaskStatus.RanToCompletion)
                {

                    News news = null;
                    news = t1.Result;
                    
                  

                    news.Articles.ForEach(a => Console.WriteLine($" - {a.DateTime.ToString("yyyy-MM-dd HH:mm")}\t: {a.Title}"));

                }
                else
                {
                    Console.WriteLine($"Geolocation News service error.");
                }

            }


        }
        static void ReportNewsDataAvailable(object sender, string message)
        {
            Console.WriteLine($"Event message from news service: {message}");
        }
    }
}