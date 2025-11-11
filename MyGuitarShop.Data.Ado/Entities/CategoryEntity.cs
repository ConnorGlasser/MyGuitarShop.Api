using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGuitarShop.Data.Ado.Entities
{
    public class CategoryEntity
    {
        public required int CategoryID { get; set; }

        [MaxLength(255)]
        public required string CategoryName { get; set; }
    }
}
