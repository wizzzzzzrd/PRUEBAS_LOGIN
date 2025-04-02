using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRUEBAS_LOGIN.Models
{
    [Table("tbc_FW_TipoCobro")]
    public class FormaPago
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTipoPago { get; set; }

        [Required]
        [StringLength(20)]
        public string ClavePago { get; set; }

        [Required]
        [StringLength(100)]
        public string TipoPago { get; set; }

        [Required]
        public int Comision { get; set; }  // Valor predeterminado 0

        [Required]
        public int Estatus { get; set; }   // Valor predeterminado 0

        [Required]
        public int UsuarioCreacion { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        public int? UsuarioModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}
