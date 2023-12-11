using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ElaticSearch.WEB.ViewModels
{
    public class BlogCreateViewModel
    {
        [Display(Name = "Blog Title")]
        public required string Title { get; set; } = null!;


        [Display(Name = "Blog Content")]
        public required string Content { get; set; } = null!;

        [Display(Name = "Blog Tag")]
        public string Tags { get; set; } = null!;
        
    }
}
