using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDBootcamp32.Model
{
    [Table("tb_t_transactiondetail")]
    public class TransactionDetail
    {
        [Key]
        public int ID { get; set; }
        public int Quantity { get; set; }
        public int SubTotal { get; set; }

        public Transaction Transactions { get; set; }
        public Items Items { get; set; }

        public TransactionDetail() { }
        public TransactionDetail(Transaction transaction, Items item, int quantity, int subTotal)
        {
            this.Transactions = transaction;
            this.Items = item;
            this.Quantity = quantity;
            this.SubTotal = SubTotal;
        }
    }
}
