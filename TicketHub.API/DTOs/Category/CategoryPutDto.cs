using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketHub.API.DTOs.Category
{
    public class CategoryPutDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}