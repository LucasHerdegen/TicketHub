using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.API.Models
{
    public class Ticket
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? UserEmail { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int EventId { get; set; }
        public Event? Event { get; set; }
    }
}