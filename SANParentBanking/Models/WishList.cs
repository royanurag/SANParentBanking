using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SANParentBanking.Models
{
    public class WishList
    {
        public int Id { get; set; }
        [DisplayName("Date Added")]
        public DateTime Date_Added { get; set; }
        [Required(ErrorMessage = "Please enter cost of the product")]
        public double Cost { get; set; }
        [Required(ErrorMessage = "Please provide a Description")]
        public String Description { get; set; }
        [Url(ErrorMessage = "Please enter a valid Web Link")]
        public String WebLink { get; set; }
        public Boolean Purchased { get; set; }

        public double amount_required = 0;

        public virtual int AccountId { get; set; }
        public virtual BankAccount Account { get; set; }


        public double CalculateAmountRequired()
        {
            amount_required = Account.transactionsAmount - Cost;
            return (Math.Abs(amount_required));
        }
    }
}