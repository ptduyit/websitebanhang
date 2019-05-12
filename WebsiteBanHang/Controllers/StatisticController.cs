﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;
using System.Data.Sql;

namespace WebsiteBanHang.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
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