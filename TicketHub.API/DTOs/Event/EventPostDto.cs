namespace TicketHub.API.DTOs.Event
{
    public class EventPostDto
    {
        public string? Name { get; set; }
        public int Capacity { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }
}