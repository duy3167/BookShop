using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int order_detail_id { get; set; }
        public int order_id { get; set; }
        [ForeignKey("order_id")]
        public Order order { get; set; }

        public int book_id { get; set; }
        [ForeignKey("book_id")]
        public Book book { get; set; }

        public int quantity { get; set; }
    }
}
