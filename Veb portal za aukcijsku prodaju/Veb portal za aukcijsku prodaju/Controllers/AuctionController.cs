using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Veb_portal_za_aukcijsku_prodaju.Hubs;
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

                var bids = context.Bids
                    .Where(n => n.AukcijaID == id)
                    .OrderByDescending(n => n.PonCena)
                    .Take(10);

                foreach(var bid in bids)
                {
                    Korisnik korisnik = context.Korisniks.Find(bid.KorisnikID);
                    bid.KorisnikImePrezime = korisnik.Ime + " " + korisnik.Prezime;
                }

                aukcija.Top10Bids = bids;

                aukcija.Top10Bids = aukcija.Top10Bids.ToList();

                if ((aukcija.VremeZatvaranja != null) && (!aukcija.VremeZatvaranja.Equals("")) && (aukcija.Status.Equals("OPEN")))
                    aukcija.PreostaloVreme = ((DateTime)aukcija.VremeZatvaranja - DateTime.Now).TotalSeconds;
                else
                    if ((aukcija.VremeOtvaranja != null) && (!aukcija.VremeOtvaranja.Equals("")) && (!aukcija.Status.Equals("OPEN")))
                        //auk.PreostaloVreme = ((DateTime)auk.VremeZatvaranja - (DateTime)auk.VremeOtvaranja).TotalSeconds;
                        aukcija.PreostaloVreme = -1;
                    else
                        aukcija.PreostaloVreme = (double)aukcija.Trajanje;
            }
            return View(aukcija);
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
                
    }
}