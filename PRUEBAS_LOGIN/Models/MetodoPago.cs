using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRUEBAS_LOGIN.Models
{
    [Table("MetodosPago")]
    public class MetodoPago
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdMetodo { get; set; }

        [Required]
        [StringLength(3)]
        public string Clave { get; set; }

        [Required]
        [StringLength(100)]
        public string Descripcion { get; set; }

        [Required]
        public DateTime FechaInicioVigencia { get; set; }

        public DateTime? FechaFinVigencia { get; set; }

        [Required]
        public bool Activo { get; set; }

        [Required]
        public DateTime FechaUltimaActualizacion { get; set; }

        [Required]
        [StringLength(50)]
        public string UsuarioActualizacion { get; set; }
    }
}
