using Microsoft.EntityFrameworkCore;
using MudBlazorDashboard.Data;
using MudBlazorDashboard.Services.Dtos;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazorDashboard.Services
{
    public class ReportService
    {
        public async Task<decimal> GetTotalSalesOfTheDay()
        {
            List<decimal?> qryResult = new List<decimal?>();

            using (var context = new NorthwindContext())
            {
                var dateStart = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                var dateEnd = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");                

                qryResult = await context.Database.SqlQuery<decimal?>($"""
                    SELECT sum([Order Details].UnitPrice * [Order Details].Quantity) as Total FROM Orders JOIN [Order Details] on Orders.OrderID = [Order Details].OrderID 
                    WHERE Orders.OrderDate BETWEEN datetime({dateStart}) AND datetime({dateEnd})
                """).ToListAsync();                
            }

            await Task.Delay(new Random().Next(2000, 5000));

            var value = qryResult.FirstOrDefault();            

            return value == null ? 0 : value.Value;
        }
        
        public async Task<int> GetTotalOrdersOfTheDay()
        {
            List<int> result;

            using (var context = new NorthwindContext())
            {
                var dateStart = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                var dateEnd = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");

                result = await context.Database.SqlQuery<int>($"""
                    SELECT count(*) FROM Orders WHERE Orders.OrderDate BETWEEN datetime({dateStart}) AND datetime({dateEnd});
                """).ToListAsync();
            }

            await Task.Delay(new Random().Next(2000, 5000));

            return result.FirstOrDefault();
        }

        public async Task<int> GetTotalShippedOrdersOfTheDay()
        {
            List<int> result;

            using (var context = new NorthwindContext())
            {
                var dateStart = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                var dateEnd = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");

                result = await context.Database.SqlQuery<int>($"""
                    SELECT count(*) FROM Orders WHERE Orders.ShippedDate BETWEEN datetime({dateStart}) AND datetime({dateEnd});
                """).ToListAsync();
            }

            await Task.Delay(new Random().Next(2000, 5000));

            return result.FirstOrDefault();
        }

        public async Task<List<MonthlyData>> GetMonthlySalesYtd()
        {
            List<MonthlyData> result = new List<MonthlyData>();

            using (var context = new NorthwindContext())
            {
                var year = DateTime.Now.Year.ToString();

                result = await context.Database.SqlQuery<MonthlyData>($"""
                    SELECT strftime('%m', Orders.OrderDate) as Month, sum([Order Details].UnitPrice * [Order Details].Quantity) as Total
                    FROM Orders JOIN [Order Details] on Orders.OrderID = [Order Details].OrderID
                    WHERE strftime('%Y', Orders.OrderDate) = {year}
                    GROUP BY strftime('%m', Orders.OrderDate);
                """).ToListAsync();
            }

            await Task.Delay(new Random().Next(2000, 5000));

            return result;
        }

        public async Task<List<MonthlyData>> GetMonthlyOrdersYtd()
        {
            List<MonthlyData> result = new List<MonthlyData>();

            using (var context = new NorthwindContext())
            {
                var start = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);
                var end = DateTime.Now;

                result = await context.Database.SqlQuery<MonthlyData>($"""
                    SELECT strftime('%m', Orders.OrderDate) as Month, count(*) as Total
                    FROM Orders
                    WHERE Orders.OrderDate BETWEEN {start} AND {end}
                    GROUP BY strftime('%m', Orders.OrderDate)
                """).ToListAsync();
            }

            await Task.Delay(new Random().Next(2000, 5000));

            return result;
        }

        public async Task<List<MonthlyData>> GetMonthlyOrdersShippedYtd()
        {
            List<MonthlyData> result = new List<MonthlyData>();

            using (var context = new NorthwindContext())
            {
                var start = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);
                var end = DateTime.Now;

                result = await context.Database.SqlQuery<MonthlyData>($"""
                    SELECT strftime('%m', Orders.ShippedDate) as Month, count(*) as Total
                    FROM Orders
                    WHERE Orders.ShippedDate BETWEEN {start} AND {end}
                    GROUP BY strftime('%m', Orders.ShippedDate)
                """).ToListAsync();
            }

            await Task.Delay(new Random().Next(2000, 5000));

            return result;
        }

        public async Task<List<ProductCategorySales>> GetCurrentMonthlySalesByProductCategory()
        {
            var result = new List<ProductCategorySales>();

            using (var context = new NorthwindContext())
            {
                var year = DateTime.Now.ToString("yyyy");
                var month = DateTime.Now.ToString("MM");
                
                result = await context.Database.SqlQuery<ProductCategorySales>($"""
                    SELECT Categories.CategoryName, sum([Order Details].UnitPrice * [Order Details].Quantity) as Total
                    FROM Categories 
                    JOIN Products on Categories.CategoryID = Products.CategoryID 
                    JOIN [Order Details] on Products.ProductID = [Order Details].ProductID 
                    JOIN Orders on [Order Details].OrderID = Orders.OrderID
                    WHERE strftime('%Y', Orders.OrderDate) = {year} AND strftime('%m', Orders.OrderDate) = {month}
                    GROUP BY Categories.CategoryName
                """).ToListAsync();
            }

            await Task.Delay(new Random().Next(2000, 5000));

            return result;
        }

        public async Task<List<ProductSales>> GetTop10ProductsOfMonth()
        {
            var result = new List<ProductSales>();

            using (var context = new NorthwindContext())
            {
                var year = DateTime.Now.ToString("yyyy");
                var month = DateTime.Now.ToString("MM");

                result = await context.Database.SqlQuery<ProductSales>($"""
                    SELECT Products.ProductName, sum([Order Details].UnitPrice * [Order Details].Quantity) as Total
                    FROM Orders JOIN [Order Details] on Orders.OrderID = [Order Details].OrderID JOIN Products on [Order Details].ProductID = Products.ProductID
                    WHERE strftime('%Y', Orders.OrderDate) = {year} AND strftime('%m', Orders.OrderDate) = {month}
                    GROUP BY [Order Details].ProductID
                    ORDER BY Total DESC
                    LIMIT 10
                """).ToListAsync();
            }

            await Task.Delay(new Random().Next(2000, 5000));

            return result;
        }

        public async Task<List<CustomerSales>> GetTop10CustomersOfMonth()
        {
            var result = new List<CustomerSales>();

            using (var context = new NorthwindContext())
            {
                var year = DateTime.Now.ToString("yyyy");
                var month = DateTime.Now.ToString("MM");

                result = await context.Database.SqlQuery<CustomerSales>($"""
                    SELECT Customers.CompanyName as CustomerName, sum([Order Details].UnitPrice * [Order Details].Quantity) as Total
                    FROM Customers JOIN Orders on Customers.CustomerID = Orders.CustomerID JOIN [Order Details] on Orders.OrderID = [Order Details].OrderID
                    WHERE strftime('%Y', Orders.OrderDate) = {year} AND strftime('%m', Orders.OrderDate) = {month}
                    GROUP BY Customers.CustomerID
                    ORDER BY Total DESC
                    LIMIT 10
                """).ToListAsync();
            }

            await Task.Delay(new Random().Next(4000, 6000));

            return result;
        }
    }
}
