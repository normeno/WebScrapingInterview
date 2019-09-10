using System;
using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebScrapingInterview.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebScrapingInterview.Controllers
{
    [Route("api/[controller]")]
    public class VacasaController : Controller
    {
        private readonly HtmlWeb _web;
        private string _scrapingUrl;
        private HtmlNode _mainNode;

        public VacasaController()
        {
            _web = new HtmlWeb();
        }

        // GET api/vacasa
        [HttpGet]
        public ActionResult<List<VacasaRentals>> Get([FromQuery] VacasaGet qs)
        {
            SetScrapingUrl(qs.City.ToLower());
            var HtmlDoc = _web.Load(_scrapingUrl);
            var mainNode = HtmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'unit-listing')]");
            List<VacasaRentals> scrapingElements = ProcessScraping(mainNode, qs.City);

            if (qs.MinPrice > 0 || qs.MaxPrice > 0)
                scrapingElements = FilterByPrice(qs, scrapingElements);

            return scrapingElements;
        }

        // Remove elements by price
        private List<VacasaRentals> FilterByPrice(VacasaGet qs, List<VacasaRentals> scrapingElements)
        {
            for (int i = 0; i <= scrapingElements.Count - 1; i++)
            {
                if (qs.MinPrice > 0 && Decimal.Compare(qs.MinPrice, scrapingElements[i].MinPrice) < 0)
                {
                    scrapingElements.RemoveAt(i);
                }

                if (qs.MaxPrice > 0 && Decimal.Compare(qs.MaxPrice, scrapingElements[i].MaxPrice) < 0)
                {
                    scrapingElements.RemoveAt(i);
                }
            }

            return scrapingElements;
        }

        // Set the URL to generate scraping
        private void SetScrapingUrl(string city)
        {
            switch (city)
            {
                case "santiago":
                    _scrapingUrl = @"https://www.vacasa.com/vacation-rentals/chile/Santiago/Santiago/";
                    break;
                case "providencia":
                    _scrapingUrl = @"https://www.vacasa.com/vacation-rentals/chile/Providencia-Santiago/";
                    break;
                case "las condes":
                case "las+condes":
                case "las%20condes":
                    _scrapingUrl = @"https://www.vacasa.com/vacation-rentals/chile/Las-Condes-Santiago/";
                    break;
                default:
                    _scrapingUrl = null;
                    break;
            }
        }

        // Get elements by scraping or json
        private List<VacasaRentals> ProcessScraping(dynamic mainNode, string city)
        {
            List<VacasaRentals> scrapingElements = new List<VacasaRentals>();

            var dt = DateTime.Now;
            string currentDay = dt.Day.ToString();
            string currentHour = dt.Hour.ToString();
            string filePath = "wwwroot/json/" + city + "-" + currentDay + "-" + currentHour + ".json";

            if (System.IO.File.Exists(filePath))
            {
                string json = GetJson(filePath);
                scrapingElements = JsonConvert.DeserializeObject<List<VacasaRentals>>(json);
            }
            else
            {
                scrapingElements = SetScrapingElements(mainNode);
                StoreJson(city, scrapingElements, filePath);
            }

            return scrapingElements;
        }

        // Get Data from JSON
        private string GetJson(string filePath)
        {
            string data = System.IO.File.ReadAllText(filePath);
            return data;
        }

        // Store JSON from Scraping
        private void StoreJson(string city, List<VacasaRentals> scrapingElements, string filePath)
        {
            using (StreamWriter writer = System.IO.File.AppendText(filePath))
            {
                writer.WriteLine(JsonConvert.SerializeObject(scrapingElements));
            }
        }

        // Generate a list of rentals
        private List<VacasaRentals> SetScrapingElements(dynamic mainNode)
        {
            List<VacasaRentals> scrapingElements = new List<VacasaRentals>();

            foreach (var node in mainNode)
            {
                try
                {
                    string id = node.Attributes["data-mini"].Value;
                    string title = node.Element("h2").InnerText;
                    string price = node.SelectSingleNode("//a[contains(@class, 'type-body-small') and contains(@href, '/unit.php?UnitID=" + id + "')]").InnerText;
                    string image = node.SelectSingleNode("//a[contains(@class, 'type-heading-small') and contains(@href, '/unit.php?UnitID=" + id + "')]/img").Attributes["src"].Value;
                    string[] splitedPrice = price.Split("-");

                    scrapingElements.Add(new VacasaRentals()
                    {
                        ID = int.Parse(id),
                        Title = title,
                        MinPrice = Convert.ToDecimal(splitedPrice[0].Trim().Remove(0, 1)),
                        MaxPrice = Convert.ToDecimal(splitedPrice[1].Trim().Remove(0, 1)),
                        MoneySign = price.Trim().Substring(0, 1),
                        Image = image.Remove(0, 2)
                    });
                }
                catch (Exception ex)
                {
                    // TODO
                    string errMessage = ex.Message;
                }
            }

            return scrapingElements;
        }
    }
}