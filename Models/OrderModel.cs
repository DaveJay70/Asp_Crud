using System.ComponentModel.DataAnnotations;
namespace Static_crud.Models
{
    public class OrderModel
    {
        public int? OrderID { get; set; }
        [Required]
        public int? Ordernumber { get; set; }
        [Required]
        public DateTime OrderDate  { get; set; }
        [Required]
        public int CustomerID { get; set; }
        
        public string PaymentMode { get; set; }
        
        public decimal TotalAmount { get; set; }
        [Required]
        public string ShippingAddress { get; set; }
        [Required]
        public int UserID { get; set; }
    }

    public class OrderDropDownModel
    {
        public int OrderID { get; set; }

        public int Ordernumber { get; set; }

    }
}
