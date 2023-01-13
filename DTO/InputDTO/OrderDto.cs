namespace WebApiDemo1.DTO.InputDTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string OrderDate { get; set; }
        public int Amount { get; set; }
        public string ProductName { get; set; }
    }
}
