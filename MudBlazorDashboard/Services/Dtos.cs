namespace MudBlazorDashboard.Services.Dtos
{
    public class MonthlyData
    {
        public int Month { get; set; }
        public decimal Total { get; set; }
    }

    public class ProductCategorySales
    {
        public string CategoryName { get; set; }
        public decimal Total { get; set; }
    }

    public class ProductSales
    {
        public string ProductName { get; set; }
        public decimal Total { get; set; }
    }

    public class CustomerSales
    {
        public string CustomerName { get; set; }
        public decimal Total { get; set; }
    }
}
