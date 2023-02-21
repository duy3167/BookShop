using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int order_id { get; set; }

        public int user_id { get; set; }
        [ForeignKey("user_id")]

        public User user { get; set; }

        public DateTime order_date { get; set; }
        public DateTime delivery_date { get; set; }
        public float total { get; set; }

        public int status { get; set; }
    }
}
