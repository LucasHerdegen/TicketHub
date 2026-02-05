namespace TicketHub.API.DTOs.User
{
    public class TokenDto
    {
        public required string Token { get; set; }
        public required DateTime ValidTo { get; set; }
    }
}