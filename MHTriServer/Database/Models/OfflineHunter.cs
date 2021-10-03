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
        public string HunterName { get; set; }

        [Required]
        public uint UnknownField4 { get; set; }

        [Required]
        public uint UnknownField5 { get; set; }

        [Required]
        public uint UnknownField6 { get; set; }

        [Required]
        public uint Rank { get; set; }

        [MaxLength(31)]
        [Column(TypeName = "varchar(31)")]
        public string UnknownField8 { get; set; }
    }
}
