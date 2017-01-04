using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SANParentBanking.Models;

namespace SANParentBanking.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.Account);
            transactions = transactions.OrderBy(t => t.Transaction_Date);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        [Authorize(Roles = "Owner")]
        public ActionResult Create()
        {

            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "Owner");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public ActionResult Create([Bind(Include = "Id,Transaction_Date,Amount,Note,AccountId")] Transaction transaction)
        {
            double amount = 0;
            double principle = 0;
            
                BankAccount account = db.BankAccounts.FirstOrDefault(a => a.Id == transaction.AccountId && (a.Recipient == User.Identity.Name || a.Owner == User.Identity.Name));


            var transactions = db.Transactions.Include(t => t.Account);
            foreach (var item in transactions)
            {
                amount = item.Amount;
                principle = principle + amount;

            }

            if(transaction.Amount<0)
                {
                double abs_amount = Math.Abs(transaction.Amount);
                    if (principle < abs_amount)
                    {
                        ModelState.AddModelError("Amount", "Amount debited cannot be greater than account balance");
                    }
            }
            if (ModelState.IsValid)
            {

                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            

            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "Owner", transaction.AccountId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        [Authorize(Roles = "Owner")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "Owner", transaction.AccountId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public ActionResult Edit([Bind(Include = "Id,Transaction_Date,Amount,Note,AccountId")] Transaction transaction)
        {
            double amount = 0;
            double principle = 0;
            var transactions = db.Transactions.Include(t => t.Account);
            foreach (var item in transactions)
            {
                amount = item.Amount;
                principle = principle + amount;

            }

            if (transaction.Amount < 0)
            {
                double abs_amount = Math.Abs(transaction.Amount);
                if (principle < abs_amount)
                {
                    ModelState.AddModelError("Amount", "Amount debited cannot be greater than account balance");
                }
            }

            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.BankAccounts, "Id", "Owner", transaction.AccountId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        [Authorize(Roles = "Owner")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public ActionResult DeleteConfirmed(int id)
        {
            double amount = 0;
            double principle = 0;
            var transactions = db.Transactions.Include(t => t.Account);
            foreach (var item in transactions)
            {
                amount = item.Amount;
                principle = principle + amount;

            }
            if (principle > 0.00)
            {
                ModelState.AddModelError("Amount", "Account has balance and cannot be deleted.");
            }

            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
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
