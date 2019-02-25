using System.ComponentModel.DataAnnotations;

namespace LibraryData.Models
{
    public class Videos : LibraryAsset
    {
        [Required]
        public string Director { get; set; }
    }
}
