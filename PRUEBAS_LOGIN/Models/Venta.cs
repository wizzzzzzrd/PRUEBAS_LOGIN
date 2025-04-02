using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PRUEBAS_LOGIN.Models
{
    public class Venta
    {
        [Key]
        public int IdVenta { get; set; }

        public int NoSucursal { get; set; }

        public int IdCliente { get; set; }

        public int IdBeneficiario { get; set; }

        public int IdEnvioDir { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ImporteIVA { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ImporteIEPS { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ImporteDescuento { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal SubtotalConDescuento { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalVenta { get; set; }

        public short Estatus { get; set; }

        [Required]
        [StringLength(255)]
        [Column(TypeName = "varchar(255)")]
        public string UsuarioCreacion { get; set; }

        public DateTime FechaCreacion { get; set; }

        [Required]
        [StringLength(255)]
        [Column(TypeName = "varchar(255)")]
        public string UsuarioModificacion { get; set; }

        public DateTime FechaModificacion { get; set; }

        public int IdTerminal { get; set; }

        public int IdCorteCaja { get; set; }
    }
}
