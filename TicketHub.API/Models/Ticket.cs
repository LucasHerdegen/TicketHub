using System.ComponentModel.DataAnnotations.Schema;

namespace TicketHub.API.Models
{
    public class Ticket
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime PucharseDate { get; set; } = DateTime.Now;
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
    }
}