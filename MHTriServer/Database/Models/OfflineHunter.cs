using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MHTriServer.Database.Models
{
    [Table("MHTri_Hunters")]
    public class OfflineHunter
    {
        public int Id { get; set; }

        [Required]
        public int PlayerId { get; set; }

        [Required]
        [MaxLength(7)]
        [Column(TypeName = "varchar(7)")]
        /// Max length 8 including \0
        public string SaveID { get; set; }

        [Required]
        [MaxLength(31)]
        [Column(TypeName = "varchar(31)")]
        /// Max length 32 (0x20) including \0
        public string CharacterName { get; set; }
    }
}
