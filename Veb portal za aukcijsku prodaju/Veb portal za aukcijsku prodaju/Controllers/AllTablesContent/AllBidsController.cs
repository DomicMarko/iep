using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Veb_portal_za_aukcijsku_prodaju.Models;

namespace Veb_portal_za_aukcijsku_prodaju.Controllers.AllTablesContent
{
    public class AllBidsController : Controller
    {
        private AukcijaEntities db = new AukcijaEntities();

        // GET: AllBids
        public ActionResult Index()
        {
            var bids = db.Bids.Include(b => b.Aukcija).Include(b => b.Korisnik);
            return View(bids.ToList());
        }

        // GET: AllBids/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bid bid = db.Bids.Find(id);
            if (bid == null)
            {
                return HttpNotFound();
            }
            return View(bid);
        }

        // GET: AllBids/Create
        public ActionResult Create()
        {
            ViewBag.AukcijaID = new SelectList(db.Aukcijas, "AukcijaID", "Proizvod");
            ViewBag.KorisnikID = new SelectList(db.Korisniks, "KorisnikID", "Ime");
            return View();
        }

        // POST: AllBids/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BidID,PonCena,Vreme,KorisnikID,AukcijaID")] Bid bid)
        {
            if (ModelState.IsValid)
            {
                db.Bids.Add(bid);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AukcijaID = new SelectList(db.Aukcijas, "AukcijaID", "Proizvod", bid.AukcijaID);
            ViewBag.KorisnikID = new SelectList(db.Korisniks, "KorisnikID", "Ime", bid.KorisnikID);
            return View(bid);
        }

        // GET: AllBids/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bid bid = db.Bids.Find(id);
            if (bid == null)
            {
                return HttpNotFound();
            }
            ViewBag.AukcijaID = new SelectList(db.Aukcijas, "AukcijaID", "Proizvod", bid.AukcijaID);
            ViewBag.KorisnikID = new SelectList(db.Korisniks, "KorisnikID", "Ime", bid.KorisnikID);
            return View(bid);
        }

        // POST: AllBids/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BidID,PonCena,Vreme,KorisnikID,AukcijaID")] Bid bid)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bid).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AukcijaID = new SelectList(db.Aukcijas, "AukcijaID", "Proizvod", bid.AukcijaID);
            ViewBag.KorisnikID = new SelectList(db.Korisniks, "KorisnikID", "Ime", bid.KorisnikID);
            return View(bid);
        }

        // GET: AllBids/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bid bid = db.Bids.Find(id);
            if (bid == null)
            {
                return HttpNotFound();
            }
            return View(bid);
        }

        // POST: AllBids/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bid bid = db.Bids.Find(id);
            db.Bids.Remove(bid);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
