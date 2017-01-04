using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SANParentBanking.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Transaction Date")]
        [CustomValidation(typeof(Transaction), "TransactionDateValidation")]
        public DateTime Transaction_Date { get; set; }
        [CustomValidation(typeof(Transaction), "AmountValidation")]
        public double Amount { get; set; }
        [Required(ErrorMessage = "Please provide a Note for the transaction.")]
        public string Note { get; set; }

        public virtual int AccountId { get; set; }
        public virtual BankAccount Account { get; set; }

        public static ValidationResult AmountValidation(double Amount, ValidationContext context)
        {
            if (Amount == 0.00)
            {
                return new ValidationResult("Amount can not be 0.00");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult TransactionDateValidation(DateTime Transaction_Date, ValidationContext context)
        {
            if (Transaction_Date > DateTime.Now)
            {
                return new ValidationResult("Transaction Date cannot be a future date");
            }
            else if (Transaction_Date.Year < DateTime.Now.Year)
            {
                return new ValidationResult("Transaction Date cannot be before current year");
            }
            return ValidationResult.Success;
        }
    }
}
