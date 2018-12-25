using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VinePicker.Models;
using ScrapySharp.Network;
using ScrapySharp.Extensions;
using Newtonsoft.Json.Linq;

namespace VinePicker.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Add()
        {
            return View(new Vine());
        }

        private static string FormatNumber(long num)
        {
            // Ensure number has max 3 significant digits (no rounding up can happen)
            long i = (long)Math.Pow(10, (int)Math.Max(0, Math.Log10(num) - 2));
            num = num / i * i;

            if (num >= 1000000000)
                return (num / 1000000000D).ToString("0.##") + "B";
            if (num >= 1000000)
                return (num / 1000000D).ToString("0.##") + "M";
            if (num >= 1000)
                return (num / 1000D).ToString("0.##") + "K";

            return num.ToString("#,0");
        }

        public PartialViewResult GetVine(Vine data)
        {
            // Get the vine code
            int startIndex = data.Permalink.IndexOf("v/") + 2;
            int endIndex = data.Permalink.Length;
            for (int i = startIndex; i < data.Permalink.Length; i++)
            {
                char character = data.Permalink[i];
                if (!Char.IsLetterOrDigit(character))
                    endIndex = i - 1;
            }
            string videoCode = data.Permalink.Substring(startIndex, endIndex - startIndex);

            ScrapingBrowser browser = new ScrapingBrowser();
            WebResource jsonResource = null;
            var task = Task.Run(() =>
                {
                    jsonResource =
                        browser.DownloadWebResource(new Uri("https://archive.vine.co/posts/" + videoCode + ".json"));
                });
            bool isSuccess = task.Wait(TimeSpan.FromMilliseconds(2000));
            if (isSuccess)
            {
                JArray objects = JArray.Parse("[" + jsonResource.GetTextContent() + "]");
                data.Description = objects.First["description"].ToString();
                data.VideoUrl = objects.First["videoUrl"].ToString();
                data.Created = DateTime.Parse(objects.First["created"].ToString().Substring(0, 10));
                data.Loops = FormatNumber(Int32.Parse(objects.First["loops"].ToString()));
                data.Likes = FormatNumber(Int32.Parse(objects.First["likes"].ToString()));
                data.Username = objects.First["username"].ToString();
                data.Rating = 0;
            }

            return PartialView("_VineView", data);
        }
    }
}
