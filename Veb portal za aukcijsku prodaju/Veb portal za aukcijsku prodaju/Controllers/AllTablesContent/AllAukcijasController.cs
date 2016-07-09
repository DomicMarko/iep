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
    public class AllAukcijasController : Controller
    {
        private AukcijaEntities db = new AukcijaEntities();

        // GET: AllAukcijas
        public ActionResult Index()
        {
            var aukcijas = db.Aukcijas.Include(a => a.Bid);
            return View(aukcijas.ToList());
        }

        // GET: AllAukcijas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aukcija aukcija = db.Aukcijas.Find(id);
            if (aukcija == null)
            {
                return HttpNotFound();
            }
            return View(aukcija);
        }

        // GET: AllAukcijas/Create
        public ActionResult Create()
        {
            ViewBag.BidID = new SelectList(db.Bids, "BidID", "BidID");
            return View();
        }

        // POST: AllAukcijas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AukcijaID,Proizvod,Trajanje,PocetnaCena,VremeKreiranja,VremeOtvaranja,VremeZatvaranja,Status,TrenutnaCena,Slika,BidID")] Aukcija aukcija)
        {
            if (ModelState.IsValid)
            {
                db.Aukcijas.Add(aukcija);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BidID = new SelectList(db.Bids, "BidID", "BidID", aukcija.BidID);
            return View(aukcija);
        }

        // GET: AllAukcijas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aukcija aukcija = db.Aukcijas.Find(id);
            if (aukcija == null)
            {
                return HttpNotFound();
            }
            ViewBag.BidID = new SelectList(db.Bids, "BidID", "BidID", aukcija.BidID);
            return View(aukcija);
        }

        // POST: AllAukcijas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AukcijaID,Proizvod,Trajanje,PocetnaCena,VremeKreiranja,VremeOtvaranja,VremeZatvaranja,Status,TrenutnaCena,Slika,BidID")] Aukcija aukcija)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aukcija).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BidID = new SelectList(db.Bids, "BidID", "BidID", aukcija.BidID);
            return View(aukcija);
        }

        // GET: AllAukcijas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aukcija aukcija = db.Aukcijas.Find(id);
            if (aukcija == null)
            {
                return HttpNotFound();
            }
            return View(aukcija);
        }

        // POST: AllAukcijas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Aukcija aukcija = db.Aukcijas.Find(id);
            db.Aukcijas.Remove(aukcija);
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
