namespace WebScrapingInterview.Models
{
    // Scraping params
    public class VacasaRentals
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string MoneySign { get; set; }
        public string Image { get; set; }
    }

    // Query String params
    public class VacasaGet
    {
        public string City { get; set; } = "las condes";
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
    }
}
