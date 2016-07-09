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
    public class AllNarudzbinasController : Controller
    {
        private AukcijaEntities db = new AukcijaEntities();

        // GET: AllNarudzbinas
        public ActionResult Index()
        {
            var narudzbinas = db.Narudzbinas.Include(n => n.Korisnik);
            return View(narudzbinas.ToList());
        }

        // GET: AllNarudzbinas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Narudzbina narudzbina = db.Narudzbinas.Find(id);
            if (narudzbina == null)
            {
                return HttpNotFound();
            }
            return View(narudzbina);
        }

        // GET: AllNarudzbinas/Create
        public ActionResult Create()
        {
            ViewBag.KorisnikID = new SelectList(db.Korisniks, "KorisnikID", "Ime");
            return View();
        }

        // POST: AllNarudzbinas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NarudzbinaID,CenaPaketa,BrojTokena,Status,DatumPravljenja,KorisnikID")] Narudzbina narudzbina)
        {
            if (ModelState.IsValid)
            {
                db.Narudzbinas.Add(narudzbina);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.KorisnikID = new SelectList(db.Korisniks, "KorisnikID", "Ime", narudzbina.KorisnikID);
            return View(narudzbina);
        }

        // GET: AllNarudzbinas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Narudzbina narudzbina = db.Narudzbinas.Find(id);
            if (narudzbina == null)
            {
                return HttpNotFound();
            }
            ViewBag.KorisnikID = new SelectList(db.Korisniks, "KorisnikID", "Ime", narudzbina.KorisnikID);
            return View(narudzbina);
        }

        // POST: AllNarudzbinas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NarudzbinaID,CenaPaketa,BrojTokena,Status,DatumPravljenja,KorisnikID")] Narudzbina narudzbina)
        {
            if (ModelState.IsValid)
            {
                db.Entry(narudzbina).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.KorisnikID = new SelectList(db.Korisniks, "KorisnikID", "Ime", narudzbina.KorisnikID);
            return View(narudzbina);
        }

        // GET: AllNarudzbinas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Narudzbina narudzbina = db.Narudzbinas.Find(id);
            if (narudzbina == null)
            {
                return HttpNotFound();
            }
            return View(narudzbina);
        }

        // POST: AllNarudzbinas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Narudzbina narudzbina = db.Narudzbinas.Find(id);
            db.Narudzbinas.Remove(narudzbina);
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
