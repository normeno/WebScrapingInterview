using System;
namespace WebScrapingInterview.Models
{
    public class VacasaRentals
    {
        public VacasaRentals()
        {
        }

        public int ID { get; set; }
        public string Title { get; set; }
        public string MinPrice { get; set; }
        public string MaxPrice { get; set; }
        public string MoneySign { get; set; }
        public string Image { get; set; }
    }
}
