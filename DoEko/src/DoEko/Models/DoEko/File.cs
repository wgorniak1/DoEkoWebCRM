﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    public class File
    {
        [Key]
        public Guid Id { get; set; }
        [Display(AutoGenerateField = true,AutoGenerateFilter = true,Description = "Nazwa pliku",Name = "Nazwa pliku", Order = 1,Prompt = "Nazwa pliku", ShortName = "Nazwa")]
        public string Name { get; set; }
        public string Url { get; set; }
        public string ParentType { get; set; }
        public int? ProjectId { get; set; }
        public int? ContractId { get; set; }
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(AutoGenerateField = true, AutoGenerateFilter = true, Description = "Data ostatniej zmiany", Name = "Ostatnia zmiana", Order = 1, ShortName = "Zmieniono")]
        public DateTime ChangedAt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid ChangedBy { get; set; }
    }
}
