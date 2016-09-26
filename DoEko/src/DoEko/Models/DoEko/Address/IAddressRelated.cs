using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko.Addresses
{
    public interface IAddressRelated
    {
        //[Display(Description = "", Name = "Adres", ShortName = "Adres")]
        //[ForeignKey("Address")]
        int AddressId { get; set; }
        Address Address { get; set; }
    }
}
