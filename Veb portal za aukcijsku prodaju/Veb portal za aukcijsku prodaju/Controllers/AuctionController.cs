using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Veb_portal_za_aukcijsku_prodaju.Models;

namespace Veb_portal_za_aukcijsku_prodaju.Controllers
{
    public class AuctionController : Controller
    {
        // GET: Auction
        public ActionResult Index(int id = -1)
        {
            if (id == -1) return RedirectToAction("Index", "Home");
            Aukcija aukcija = null;
            using (var context = new AukcijaEntities())
            {
                aukcija = context.Aukcijas.Find(id);
                if (aukcija == null)
                {
                    return HttpNotFound();
                }
            }
            return View(aukcija);
        }

        [HttpGet]
        public ActionResult ChangePrice(int? id, string newPrice)
        {
            try
            {
                double doubleValue = Convert.ToDouble(newPrice);
                Aukcija editAukcija;

                using (var context = new AukcijaEntities())
                {
                    editAukcija = context.Aukcijas.Find(id);
                }

                if (editAukcija != null)
                {
                    editAukcija.PocetnaCena = doubleValue;
                }

                using (var context = new AukcijaEntities())
                {
                    context.Entry(editAukcija).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to convert '{0}' to a Double.", newPrice);
            }
            catch (OverflowException)
            {
                Console.WriteLine("'{0}' is outside the range of a Double.", newPrice);
            }

            return RedirectToAction("Index", "Auction", new { id = id });

        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                Aukcija editAukcija;

                using (var context = new AukcijaEntities())
                {
                    editAukcija = context.Aukcijas.Find(id);
                }

                if (editAukcija != null)
                {
                    editAukcija.Status = "DELETED";
                }

                using (var context = new AukcijaEntities())
                {
                    context.Entry(editAukcija).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to delete auction");
            }

            return RedirectToAction("Index", "Admin", new { id = id });

        }

        [HttpGet]
        public ActionResult OpenAuction(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                Aukcija editAukcija;

                using (var context = new AukcijaEntities())
                {
                    editAukcija = context.Aukcijas.Find(id);
                }

                if (editAukcija != null)
                {
                    DateTime startTime = DateTime.Now;
                    DateTime endTime = startTime.AddSeconds((int)editAukcija.Trajanje);

                    editAukcija.Status = "OPEN";
                    editAukcija.VremeOtvaranja = startTime;
                    editAukcija.VremeZatvaranja = endTime;
                }


                using (var context = new AukcijaEntities())
                {
                    context.Entry(editAukcija).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to delete auction");
            }

            return RedirectToAction("Index", "Admin", new { id = id });

        }
    }
}