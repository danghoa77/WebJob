﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Job1670.Models
{
    [Table("JobApplication")]
    public class JobApplication
    {
        [Key]
        public int JobApplicationId { get; set; }

        [Required]
        public int JobId { get; set; }

        [ForeignKey("JobId")]
        public virtual Listing Listing { get; set; }

        [Required]
        public string JobSeekerId { get; set; }

        [ForeignKey("JobSeekerId")]
        public virtual JobSeeker JobSeeker { get; set; }


        [Column(TypeName = "nvarchar(max)")] // Đặt kiểu dữ liệu trong cơ sở dữ liệu là nvarchar(max) để hỗ trợ văn bản dài
        public string CoverLetter { get; set; }

        public string Status { get; set; }
    }
}
