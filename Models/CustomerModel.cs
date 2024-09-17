using System.ComponentModel.DataAnnotations;
namespace Static_crud.Models
{
    public class CustomerModel
    {

        public int? CustomerID { get; set; }
        [Required]
        public string CustomerName   { get; set; }
        [Required]
        public string HomeAddress { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(10)]
        [MaxLength(10)]
        public string MobileNo { get; set; }
        [Required]
        public string GST_NO { get; set; }
        [Required]
        public string CityName { get; set; }
        [Required]
        [MinLength(6)]
        public string PinCode { get; set; }
        [Required]
        public decimal NetAmount { get; set; }
        [Required]
        public int UserID { get; set; }


    }
    public class CustomerDropDownModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }
}
