using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using M10Api.Models;

namespace M10Api.Controllers
{
  public class ProductController : ApiController
  {
    Product[] products = new Product[]
       {
              new Product { Id = 1, Name = "Tomato Soup1111", Category = "Groceries", Price = 1 },
              new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
              new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
       };

    public IEnumerable<Product> GetAllProducts()
    {


      return products;
    }
  }
}
