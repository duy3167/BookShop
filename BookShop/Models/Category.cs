using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int cate_id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        public string name { get; set; }
        public int status { get; set; }

        public virtual ICollection<Book> books { get; set; }
    }
}
