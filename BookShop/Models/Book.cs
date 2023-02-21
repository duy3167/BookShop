using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int book_id { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int cate_id { get; set; }
        [ForeignKey("cate_id")]
        public Category? category { get; set; }

        [Required(ErrorMessage = "Supplier is required")]
        public int sup_id { get; set; }
        [ForeignKey("sup_id")]
        public Supplier? supplier { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string description { get; set; }
        [Required(ErrorMessage = "Author is required")]
        public string author { get; set; }

        [Required(ErrorMessage = "quantity is required")]
        public int quantity { get; set; }

        public string? image { get; set; }

        [Required(ErrorMessage = "Publishing year is required")]
        public string publishing_year { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public float price { get; set; }

        public int? status { get; set; }
    }
}
