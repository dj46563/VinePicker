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
using VinePicker.ViewModels;

namespace VinePicker.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            IndexViewModel model = new IndexViewModel();
            model.LeftVine = new VineViewModel()
            {
                Vine = DataAccess.DataAccess.GetVine(),
                Position = Position.Left
            };
            model.RightVine = new VineViewModel()
            {
                Vine = DataAccess.DataAccess.GetSimilarVine(model.LeftVine.Vine.VineId, model.LeftVine.Vine.Rating),
                Position = Position.Right
            };

            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

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

        public ActionResult AddVine(Vine model)
        {
            Vine vine = RetrieveVine(model.Permalink, model.Submitter);
            DataAccess.DataAccess.AddVine(vine);

            return View("Index");
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

        public Vine RetrieveVine(string permalink, string submitter = null)
        {
            Vine vine = new Vine()
            {
                Permalink = permalink,
                Submitter = submitter
            };

            // Get the vine code
            int startIndex = vine.Permalink.IndexOf("v/") + 2;
            int endIndex = vine.Permalink.Length;
            for (int i = startIndex; i < vine.Permalink.Length; i++)
            {
                char character = vine.Permalink[i];
                if (!Char.IsLetterOrDigit(character))
                {
                    endIndex = i;
                    break;
                }
            }
            string videoCode = vine.Permalink.Substring(startIndex, endIndex - startIndex);

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
                vine.Description = objects.First["description"].ToString();
                vine.VideoUrl = objects.First["videoUrl"].ToString();
                vine.Created = DateTime.Parse(objects.First["created"].ToString().Substring(0, 10));
                vine.Loops = FormatNumber(Int32.Parse(objects.First["loops"].ToString()));
                vine.Likes = FormatNumber(Int32.Parse(objects.First["likes"].ToString()));
                vine.Username = objects.First["username"].ToString();
                vine.Rating = 0;
            }

            return vine;
        }

        public PartialViewResult GetVine(Vine data)
        {
            VineViewModel viewModel = new VineViewModel()
            {
                Vine = RetrieveVine(data.Permalink, data.Submitter),
                Position = Position.None
            };
            return PartialView("_VineView", viewModel);
        }
    }
}
