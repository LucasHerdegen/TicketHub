namespace TicketHub.API.DTOs
{
    public class TokenDto
    {
        public required string Token { get; set; }
        public required DateTime ValidTo { get; set; }
    }
}