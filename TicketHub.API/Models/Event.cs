using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.API.Models
{
    public class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Capacity { get; set; }
        public DateTime Date { get; set; }
        [Column(TypeName = "decimal(18,2 )")]
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int SoldTickets { get; set; } = 0;

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}