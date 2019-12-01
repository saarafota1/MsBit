using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CashDask.Data;
using CashDask.Models;
using System.Collections;

namespace CashDask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly CashDaskContext _context;

        public PurchasesController(CashDaskContext context)
        {
            _context = context;
        }

        // GET: api/Purchases
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchases()
        {
            return await _context.Purchases.Include(a => a.products).ToListAsync();
        }

        // GET: api/Purchases/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Purchase>> GetPurchase(int id)
        {
            var purchase = await _context.Purchases.Include(a => a.products).FirstOrDefaultAsync(p => p.ID == id);

            if (purchase == null)
            {
                return NotFound();
            }

            return purchase;
        }

        // PUT: api/Purchases/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchase(int id, Purchase purchase)
        {
            if (id != purchase.ID)
            {
                return BadRequest();
            }

            _context.Entry(purchase).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Purchases
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Purchase>> PostPurchase(Purchase purchase)
        {
            _context.Purchases.Add(purchase);
            foreach (var product in purchase.products)
            {
                var temp_product = _context.Products.SingleOrDefault(o => o.ID == product.ID);
                temp_product.quantity--;
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPurchase", new { id = purchase.ID }, purchase);
        }

        [HttpPut("return/{id}")]
        public async Task<ActionResult<Purchase>> ReturnPurchase(int id)
        {
            var purchase = _context.Purchases.SingleOrDefault(o => o.ID == id);
            if( purchase != null)
            {
                TimeSpan time_diff_from_purchase = purchase.created - DateTime.Now;
                if (time_diff_from_purchase.Days < 30)
                {
                        var productes_returnd = new List<Product>();
                        var productes_that_cant_be_returnd = new List<Product>();
                        foreach (var product in purchase.products)
                        {
                            var temp_product = _context.Products.SingleOrDefault(o => o.ID == product.ID);
                            if (temp_product.type.can_return)
                            {
                                temp_product.quantity++;
                                productes_returnd.Add(temp_product);
                            }
                            else
                            {
                                productes_that_cant_be_returnd.Add(temp_product);
                            }

                        }
                        purchase._return = true;
                        purchase.return_date = DateTime.Now;

                        await _context.SaveChangesAsync();
                        if (time_diff_from_purchase.Days < 15)
                        {
                            return CreatedAtAction("Purchase Returnd", new { success = true, products = productes_returnd, cant_return = productes_that_cant_be_returnd, message = "Products Returnd and The Quantitys Changed,  Got Cash Less Then 15 Days From Purchase" });
                        }
                        else
                        {
                            return CreatedAtAction("Purchase Returnd", new { success = true, products = productes_returnd, cant_return = productes_that_cant_be_returnd, message = "Products Returnd and The Quantitys Changed, Got check  More Then 15 Days From Purchase And Less Then 30 Days" });
                        }
                }
                else
                {
                    return CreatedAtAction("Purchase Not Returnd", new { success = false, products = new { }, cant_return = new { }, message = "Products Not Returnd!, More Then 30 Days From Purchase" });

                }

            }

            return BadRequest();

        }
        // DELETE: api/Purchases/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Purchase>> DeletePurchase(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();

            return purchase;
        }

        private bool PurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.ID == id);
        }

        [HttpGet("{id}/products")]
        public async Task<ActionResult<Purchase>> GetPurchaseProducts(int id)
        {
            var purchase = await _context.Purchases.Include(a => a.products).FirstOrDefaultAsync(p => p.ID == id);

            if (purchase == null)
            {
                return NotFound();
            }

            return Ok(purchase.products);
        }

        [HttpGet("getStatistics")]
        public async Task<ActionResult<Purchase>> GetStatistics([FromBody]string month)
        {
            var isNumeric = int.TryParse(month, out int n);
            if( isNumeric)
            {
                int days = DateTime.DaysInMonth(DateTime.Now.Year, n);
                List<Report> result = new List<Report>();
                var purchases = await _context.Purchases.Where(o => o.created.Month == n).ToArrayAsync();

                for (int day = 1; day <= days; day++)
                {
                    result.Add(new Report{ day = day, purchases = 0, returns = 0 });
                    foreach (var purchas  in purchases)
                    {
                        if( purchas.created.Day == day)
                        {
                            if( purchas._return)
                            {
                                result[day].returns++;
                            }
                            else
                            {
                                result[day].purchases++;
                            }
                        }
                    }
                    
                }

                return Ok(result);
                
            }

            return BadRequest();
        }

    }
}
