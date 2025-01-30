﻿using System.ComponentModel.DataAnnotations;

namespace CgmscHO_API.DTO
{
    public class NearExpBatchDTO
    {
       
        public string? ITEMCODE { get; set; }
        public string? ITEMNAME { get; set; }
        public string? STRENGTH1 { get; set; }
        public string? BATCHNO { get; set; }
        public DateTime? EXPDATE { get; set; }
        public string? EXPTIMELINE { get; set; }
        public Int64? FACSTOCK { get; set; }
        [Key]
        public int INWNO { get; set; }
    }
}
