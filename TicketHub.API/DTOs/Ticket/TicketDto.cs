namespace TicketHub.API.DTOs.Ticket
{
    public class TicketDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime PucharseDate { get; set; }
        public int EventId { get; set; }
        public string? EventName { get; set; }
    }
}