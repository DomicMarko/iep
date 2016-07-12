using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Veb_portal_za_aukcijsku_prodaju.Models;
using PagedList;

namespace Veb_portal_za_aukcijsku_prodaju.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Index(int id = -1)
        {
            if (id == -1) return RedirectToAction("Index", "Home");            

            Narudzbina narudzbina = null;
            using (var context = new AukcijaEntities())
            {
                narudzbina = context.Narudzbinas.Find(id);

                if (narudzbina == null)
                {
                    return HttpNotFound();
                }
            }
            return View(narudzbina);
        }

        public ActionResult AllOrders(int? id, string sortOrder, string currentFilter, string searchString, string minP, string maxP, string AuctionStatus, int? page)
        {
            bool onlyMin, onlyMax, minMax;
            onlyMin = onlyMax = minMax = false;

            if (!String.IsNullOrEmpty(minP)) onlyMin = true;
            if (!String.IsNullOrEmpty(maxP)) onlyMax = true;
            if (onlyMin && onlyMax)
            {
                onlyMin = onlyMax = false;
                minMax = true;
            }

            using (var context = new AukcijaEntities())
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                IEnumerable<Veb_portal_za_aukcijsku_prodaju.Models.Narudzbina> narudzbine = context.Narudzbinas.Where(n => n.KorisnikID == id);

                if (!String.IsNullOrEmpty(searchString))                                    
                    narudzbine = narudzbine.Where(s => s.Status.Contains(searchString));
                                                

                switch (sortOrder)
                {
                    case "name_desc":
                        narudzbine = narudzbine.OrderByDescending(s => s.Status);
                        break;
                    case "Date":
                        narudzbine = narudzbine.OrderBy(s => s.DatumPravljenja);
                        break;
                    case "date_desc":
                        narudzbine = narudzbine.OrderByDescending(s => s.DatumPravljenja);
                        break;
                    default:  // Name ascending 
                        narudzbine = narudzbine.OrderBy(s => s.Status);
                        break;
                }               

                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(narudzbine.ToPagedList(pageNumber, pageSize));
            }
        }
    }    
}