using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using WebScrapingInterview.Models;

namespace WebScrapingInterview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly HtmlWeb _web;
        private string _scrapingUrl;
        private HtmlNode _mainNode;

        public ValuesController()
        {
            _web = new HtmlWeb();
            SetScrapingUrl();
        }

        // Set the URL to generate scraping
        private void SetScrapingUrl()
        {
            _scrapingUrl = @"https://www.vacasa.com/vacation-rentals/chile/Santiago/Santiago/";
        }

        private void SetMainNodeByClassName(string el, string className)
        {
            var HtmlDoc = _web.Load(_scrapingUrl);
            //_mainNode = HtmlDoc.DocumentNode.SelectNodes("//"+el+"[contains(@class, '"+className+"')]");
            _mainNode = HtmlDoc.DocumentNode.SelectSingleNode("//" + el + "[contains(@class, '" + className + "')]");
        }

        private void SetMainNodeBId(string el, string id)
        {
            var HtmlDoc = _web.Load(_scrapingUrl);
            _mainNode = HtmlDoc.DocumentNode.SelectSingleNode("//" + el + "[contains(@id, '" + id + "')]");
        }

        // GET api/values
        [HttpGet]
        public ActionResult<List<VacasaRentals>> Get()
        {
            List<VacasaRentals> scrapingElements = new List<VacasaRentals>();
            var HtmlDoc = _web.Load(_scrapingUrl);

            var mainNode = HtmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'unit-listing')]");

            foreach (var nNode in mainNode)
            {
                try
                {
                    string id = nNode.Attributes["data-mini"].Value;
                    string title = nNode.Element("h2").InnerText;
                    string price = nNode.SelectSingleNode("//a[contains(@class, 'type-body-small') and contains(@href, '/unit.php?UnitID="+id+"')]").InnerText;
                    string image = nNode.SelectSingleNode("//a[contains(@class, 'type-heading-small') and contains(@href, '/unit.php?UnitID=" + id+"')]/img").Attributes["src"].Value;
                    string[] splitedPrice = price.Split("-");

                    scrapingElements.Add(new VacasaRentals()
                    {
                        ID = int.Parse(id),
                        Title = title,
                        MinPrice = splitedPrice[0].Trim().Remove(0, 1),
                        MaxPrice = splitedPrice[1].Trim().Remove(0, 1),
                        MoneySign = price.Trim().Substring(0, 1), 
                        Image = image.Remove(0, 2)
                    });
                }
                catch(Exception ex)
                {
                    // TODO
                    string errMessage = ex.Message;
                }
            }

            return scrapingElements;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
