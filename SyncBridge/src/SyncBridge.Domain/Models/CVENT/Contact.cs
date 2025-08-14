using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Models.CVENT;

public class Contact : CventCommonEntity
{
    [Key]
    public string? id { get; set; }

    // TODO: Add the necessary properties for the Contact class
}