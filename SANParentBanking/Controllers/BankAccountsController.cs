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
    public class BankAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BankAccounts
        public ActionResult Index()
        {
            int is_owner = db.BankAccounts
                                    .Where(x => x.Owner == User.Identity.Name).Count();
            if (is_owner > 0)
            {
                var Accounts = db.BankAccounts
                                        .Where(x => x.Owner == User.Identity.Name);
                return View(Accounts.ToList());
            }
            else
            {
                var Accounts = db.BankAccounts
                        .Where(x => x.Recipient == User.Identity.Name);
                return View(Accounts.ToList());

            }
        }

        // GET: BankAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Create
        [Authorize(Roles = "Owner")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Owner")]
        public ActionResult Create([Bind(Include = "Id,Account_ID,Owner,Recipient,Name,Open_Date,Interest_Rate")] BankAccount bankAccount)
        {
            //check if owner and recipient have the same email id 
            int OwnEqlRec = db.BankAccounts.Where(x => bankAccount.Owner == bankAccount.Recipient).Count();

            //check if recipient already exists in the DB 
            int RecipientAlExists = db.BankAccounts.Where(x => x.Recipient == bankAccount.Recipient).Count();

            //check if recipient entered is an existing owner 
            int OwnExists = db.BankAccounts.Where(x => x.Owner == bankAccount.Recipient).Count();

            //check if Owner entered is an existing Recipient 
            int RecExists = db.BankAccounts.Where(x => x.Recipient == bankAccount.Owner).Count();

            if (OwnEqlRec > 0)
            {
                ModelState.AddModelError("Owner", "Owner and Recipient cannot be the same person");
            }

            if (RecipientAlExists > 0)
            {
                ModelState.AddModelError("Recipient", "Recipient already exists in the DB");
            }
            
            if (OwnExists > 0)
            {
                ModelState.AddModelError("Recipient", "Recipient entered is Owner of another existing account");
            }

            if (RecExists > 0)
            {
                ModelState.AddModelError("Owner", "Owner entered is Recipient of another existing account");
            }

            if (ModelState.IsValid)
            {
                db.BankAccounts.Add(bankAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bankAccount);
        }
        // GET: BankAccounts/Edit/5
        [Authorize(Roles = "Owner")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public ActionResult Edit([Bind(Include = "Id,Account_ID,Owner, Recipient,Name,Open_Date, Interest_Rate")] BankAccount bankAccount)
        {
            //check if owner and recipient have the same email id
            int OwnEqlRec = db.BankAccounts.Where(x => x.Owner == bankAccount.Recipient).Count();

            //check if owner and recipient have the same email id
            int RecOwn = db.BankAccounts.Where(x => x.Recipient == bankAccount.Owner).Count();

            //check if recipient already exists in the DB
            int RecipientAlExists = db.BankAccounts.Where(x => x.Recipient == bankAccount.Recipient).Count();

            //check if recipient entered is owner of an account
            int OwnExists = db.BankAccounts.Where(x => x.Owner == bankAccount.Recipient).Count();

            //Validation 1
            if (OwnEqlRec > 0 || RecOwn > 0)
            {
                if (RecOwn > 0)
                {
                    ModelState.AddModelError(" Owner", "Owner name matches with a Recipient in the DB");
                }

                else
                {
                    ModelState.AddModelError(" Recipient", "Recipient name matches with an Owner in the DB");
                }
            }

            // Validation 2
            if (RecipientAlExists > 0)
            {
                ModelState.AddModelError(" Recipient", "This Recipient already exists in the DB");
            }

            // Validation 3
            if (OwnExists > 0)
            {
                ModelState.AddModelError(" Recipient", "Recipient is already an Owner of another account");
            }

            if (ModelState.IsValid)
            {
                db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bankAccount);
        }
        // GET: BankAccounts/Delete/5
        [Authorize(Roles = "Owner")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [Authorize(Roles = "Owner")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            db.BankAccounts.Remove(bankAccount);
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
