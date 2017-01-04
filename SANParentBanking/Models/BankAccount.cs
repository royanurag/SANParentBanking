using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SANParentBanking.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        [DisplayName("Account ID")]
        public int AccountId { get; set; }
        [EmailAddress(ErrorMessage = "Please enter valid Email Address for Owner")]
        public string Owner { get; set; }
        [EmailAddress(ErrorMessage = "Please enter valid Email Address for Recipient")]
        public string Recipient { get; set; }
        [Required(ErrorMessage = "Please enter a Name for the Account")]
        public string Name { get; set; }
        [DisplayName("Open Date")]
        [Editable(false,AllowInitialValue =true)]
        public DateTime OpenDate { get; set; }
        [CustomValidation(typeof(BankAccount), "ValidateInterestRate")]
        [DisplayName("Interest Rate")]
        public float Interest_Rate { get; set; }

        public virtual List<WishList> WishLists { get; set; }        //Get list of Wishlist associated for this account
        public virtual List<Transaction> Transactions { get; set; }      //Get list of Transaction associated for this account

        public double interestAmountPct { get; set; }
        public double principalAmountPct { get; set; }
        public double transactionsAmount { get; set; }

        public BankAccount()
        {
            OpenDate = DateTime.Now;
        }
        public static ValidationResult ValidateInterestRate(float Interest_Rate, ValidationContext context)
        {
            if (Interest_Rate<0)
            {
                return new ValidationResult("Interest Rate cannot be 0% or below");
            }
            else if (Interest_Rate > 100)
            {
                return new ValidationResult("Interest Rate cannot be 100% or above");
            }
            return ValidationResult.Success;
        }


        public double CurrentBalanceWithoutInterest()
        {
            // sum total of all the transactions
            double total = Transactions.Sum(x => x.Amount);
            return Math.Round(total,2);
        }

        public double CurrentBalanceWithInterest()
        {
            double interest = CalculateInterest();
            double bal = CurrentBalanceWithoutInterest();
            double amount = interest + bal;
            return (Math.Round(amount,2));
        }

        public double CalculateInterest()
        {
            double Principle = 0.0;
            double interest_earned = 0.0;
            double amount_w_int = 0.0;
            double amount_deposited = 0.0;
            double Interest_apply_days = 0.0;
            double timesPerYear = 12.0;
            foreach(var item in Transactions)
                {
                amount_deposited = item.Amount;
                
                Principle = Principle + amount_deposited;
                if (amount_deposited > 0)
                {
                    Interest_apply_days = ((DateTime.Now - item.Transaction_Date).TotalDays) / 365;
                    double body = 1 + (Interest_Rate / (timesPerYear*100));
                    double exponent = timesPerYear * Interest_apply_days;
                    amount_w_int = amount_w_int + (amount_deposited * Math.Pow(body, exponent));
                }
            }
            interest_earned = amount_w_int - Principle;
            interestAmountPct = (interest_earned / amount_w_int)*100;
            principalAmountPct = (100 - interestAmountPct)*100;
            transactionsAmount= amount_w_int;
          
            return (Math.Round(interest_earned,2));
        }

        public double CalculateInterestEndOfYear()
        {
            double Principle = 0.0;
            double interest_earned = 0.0;
            double amount_w_int = 0.0;
            double amount_deposited = 0.0;
            double Interest_apply_days = 0.0;
            double timesPerYear = 12.0;
            DateTime C_date= DateTime.Now;
            foreach (var item in Transactions)
            {
                amount_deposited = item.Amount;

                Principle = Principle + amount_deposited;
                if (amount_deposited > 0)
                {
                    var lastDayOfTheYear = new DateTime(C_date.Year, 12, 31);
                    Interest_apply_days = ((lastDayOfTheYear - item.Transaction_Date).TotalDays) / 365;
                    double body = 1 + (Interest_Rate / (timesPerYear * 100));
                    double exponent = timesPerYear * Interest_apply_days;
                    amount_w_int = amount_w_int + (amount_deposited * Math.Pow(body, exponent));
                }
            }
            interest_earned = amount_w_int - Principle;
            interestAmountPct = (interest_earned / amount_w_int) * 100;
            principalAmountPct = (100 - interestAmountPct) * 100;
            transactionsAmount = amount_w_int;
            return (Math.Round(interest_earned,2));
            //return (Math.Round(interest_earned,2));
        }




    }
}