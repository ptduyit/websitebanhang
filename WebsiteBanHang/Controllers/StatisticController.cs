using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;
using System.Data.Sql;
using Microsoft.AspNetCore.Authorization;

namespace WebsiteBanHang.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "member,admin,employee")]
    public class StatisticController : ControllerBase
    {
        private readonly SaleDBContext _context;
        public StatisticController(SaleDBContext context)
        {
            _context = context;
        }

        // get  statistic by year
        [HttpGet("{year}")]
        public IActionResult getStatistic(int year)
        {
            //var import = _context.OrdersImportGoods.Where(p => p.OrderDate.Year == year).Sum(p => p.TotalPrice);
            //var export = _context.Orders.Where(p => p.OrderDate.Year == year).Sum(p => p.TotalPrice);
            List<StatistcOfMonth> statisticOfYear = new List<StatistcOfMonth>();
            for (int i = 1; i < 13; i++)
            {
                var import = _context.OrdersImportGoods.Where(p => p.OrderDate.Year == year).Where(q => q.OrderDate.Month == i).Sum(p => p.TotalPrice);
                var export = _context.Orders.Where(p => p.OrderDate.Year == year).Where(q => q.OrderDate.Month == i).Sum(p => p.TotalPrice);
                statisticOfYear.Add(new StatistcOfMonth()
                {
                    month = i,
                    import = import.Value,
                    export = export.Value
                });
            }
            return Ok(statisticOfYear);
        }
        [HttpGet]
        public IActionResult getYear()
        {
            List<decimal> years = new List<decimal>();
            var import = _context.OrdersImportGoods;
            var export = _context.Orders;
            foreach (var p in import)
            {
                if (years.Contains(p.OrderDate.Year) == false) {
                    years.Add(p.OrderDate.Year);
                }

            }
            foreach (var p in export)
            {
                if (years.Contains(p.OrderDate.Year) == false)
                {
                    years.Add(p.OrderDate.Year);
                }
            }

            return Ok(years);
        }

        [HttpGet("{year}")]
        public IActionResult getExportsOfYear(int year)
        {
            int[] result = new int[12];
            var allExport = _context.Orders.Where(p => p.OrderDate.Year == year);
            for(int i = 0; i < 12; i++)
            {
                result[i] = allExport.Where(p => p.OrderDate.Month == i+1).Count();
            }
            return Ok(result);
        }
        [HttpGet]
        public IActionResult getCretogyExportMonth(int month,int year)
        {
            var ord = _context.Orders.Where(pp => pp.OrderDate.Year == year).Where(qq => qq.OrderDate.Month == month);
            var result = (from a in ord join 
                          order in _context.OrderDetails  on a.OrderId equals order.OrderId
                          join
                          b in _context.Products on order.ProductId equals b.ProductId join
                          c in _context.ProductCategories on b.CategoryId equals c.CategoryId   
                          select new
                          {
                              c.CategoryId,
                              c.CategoryName,
                              order.OrderId,
                              order.ProductId,
                              order.Quantity,
                              order.UnitPrice,
                              total = order.Quantity* order.UnitPrice,
                              b.ProductName
                          }

                        );
            var x= result.GroupBy(l => l.CategoryId).
                Select(
                cl=> new {
                    cl.First().CategoryName,
                    cl.First().CategoryId,
                    cost=cl.Sum(tt=>tt.total)
                   
            });
            return Ok(x);
        }


        [HttpGet("{year}")]
        public IActionResult getStatisticOfyear(int year)
        {
            decimal[] result = new decimal[2];
            var export = _context.Orders.Where(p => p.OrderDate.Year == year);
            var import = _context.OrdersImportGoods.Where(p => p.OrderDate.Year == year);
            result[0] = (decimal)import.Sum(p => p.TotalPrice);
            result[1] = (decimal)export.Sum(p => p.TotalPrice);
         
            return Ok(result);
        }
        [HttpGet]
        public IActionResult getStatisticOnMonthOfYear(int year, int month)
        {
            decimal[] result = new decimal[2];
            var export = _context.Orders.Where(p => p.OrderDate.Year == year).Where(q=>q.OrderDate.Month==month);
            var import = _context.OrdersImportGoods.Where(p => p.OrderDate.Year == year).Where(q => q.OrderDate.Month == month);
            result[0] = (decimal)import.Sum(p => p.TotalPrice);
            result[1] = (decimal)export.Sum(p => p.TotalPrice);

            return Ok(result);
        }



































        //[HttpGet("{orderId}")]
        //public IActionResult GetOrderDetailsById([FromRoute] int IdProd, [FromRoute] int month, [FromRoute] int year)
        //{
        //}

        //    // lay tong so luong, gias sanr phaamr ban duco
        //    public async Task<IActionResult> getAllSold()
        //    {
        //        string sqlstr = "select Products.ProductId,Products.ProductName, sum(OrderDetails.Quantity) as TotalQuantity " +
        //           ", sum(OrderDetails.UnitPrice) as TotalPrice from Products, OrderDetails, Orders where Products.ProductId = " +
        //           "OrderDetails.ProductId and Orders.OrderId = OrderDetails.OrderId group by Products.ProductId,Products.ProductName";



        //        var a = (from o in _context.Orders
        //                 join d in _context.OrderDetails on o.OrderId equals d.OrderId
        //                 join p in _context.Products on d.ProductId equals p.ProductId
        //                 //where o.OrderDate > Convert.ToDateTime("1/2/2019")
        //                 //   && o.OrderDate < Convert.ToDateTime("12/12/2019")
        //                 group new { d.Quantity } by new { p.ProductId, p.ProductName, d.UnitPrice } into g
        //                 select new
        //                 {
        //                     ProductId = g.Key.ProductId,
        //                     ProductName = g.Key.ProductName,
        //                     UnitPrice = g.Key.UnitPrice,
        //                     TotalQuantity = g.Sum()
        //                 }
        //         );

        //        return Ok(a);
        //    }
    }
}