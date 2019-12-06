using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDBootcamp32.Model
{
    [Table("tb_m_transaction")]
    public class Transaction
    {
        [Key]
        public int ID { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public int TotalPrice { get; set; }

        public Transaction()
        {
            this.TransactionDate = DateTimeOffset.Now.LocalDateTime;
        }

        public Transaction(int totalPrice)
        {
            this.TotalPrice = totalPrice;
        }
    }
}