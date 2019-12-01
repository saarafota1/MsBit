using CashDask.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashDask.Data
{
    public class InitialData
    {
        public static void Initialize(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<CashDaskContext>();
                context.Database.EnsureCreated();
                //context.Database.Migrate();

                // Look for any ProductType
                if (context.ProductType == null || !context.ProductType.Any())
                {
                    var product_types = GetProductsType().ToArray();
                    context.ProductType.AddRange(product_types);
                    context.SaveChanges();
                }

                // Look for any Products
                if (context.Products == null || !context.Products.Any())
                {
                    var products = getProducts(context).ToArray();
                    context.Products.AddRange(products);
                    context.SaveChanges();
                }

                // Look for any Purchases
                if (context.Purchases == null || !context.Purchases.Any())
                {
                    var purches = GetPurchases(context).ToArray();
                    context.Purchases.AddRange(purches);
                    context.SaveChanges();
                }

            }
        }
        public static List<Product> getProducts(CashDaskContext db)
        {
            List<Product> products = new List<Product>()
            {
                new Product {name="product1",price = 450,quantity=30,type= db.ProductType.SingleOrDefault(a => a.name == "shoes") },
                new Product {name="product2",price = 200,quantity=50,type= db.ProductType.SingleOrDefault(a => a.name == "underwear") },
                new Product {name="product3",price = 50,quantity=20,type= db.ProductType.SingleOrDefault(a => a.name == "shirts") },
                new Product {name="product4",price = 150,quantity=15,type= db.ProductType.SingleOrDefault(a => a.name == "underwear") }
            };
            return products;
        }

        public static List<Purchase> GetPurchases(CashDaskContext db)
        {
            List<Purchase> purchases = new List<Purchase>()
            {
                new Purchase {
                    created=new DateTime(2019, 9, 9, 10, 0, 0),
                    products=new List<Product>(db.Products.Take(2)),
                    _return=false
                },
                new Purchase {
                    created=new DateTime(2019, 9, 9, 11, 5, 0),
                    products=new List<Product>(db.Products.Take(2)),
                    _return=false
                },
            };
            return purchases;
        }
        public static List<ProductType> GetProductsType()
        {
            List<ProductType> product_types = new List<ProductType>()
            {
                new ProductType {name="underwear ",can_return=false },
                new ProductType {name="shirts",can_return=true},
                new ProductType {name="pantes",can_return=true},
                new ProductType {name="shoes",can_return=true}
            };
            return product_types;
        }
    }
}
