using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MHTriServer.Database.Models
{
    [Table("MHTri_Players")]
    [Index(nameof(UUID), IsUnique = true)]
    public class OfflinePlayer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(31)]
        [Column(TypeName = "varchar(31)")]
        /// Max length 32 (0x20) including \0
        public string UUID { get; set; }

        [ForeignKey("PlayerId")]
        public List<OfflineHunter> Hunters { get; set; } = new List<OfflineHunter>();

        public DateTime? LastLogin { get; set; }

        public OfflinePlayer() { }

        public OfflinePlayer(string uuid) => (UUID) = uuid;
    }
}
