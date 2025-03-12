using System;
using System.ComponentModel.DataAnnotations;

namespace PRUEBAS_LOGIN.Models
{
    public class Fiscales
    {
        [Key]
        public int IdFiscal { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(510)]
        public string Calle { get; set; }

        [Required]
        [StringLength(100)]
        public string NoExterior { get; set; }

        [StringLength(100)]
        public string NoInterior { get; set; }

        [Required]
        [StringLength(20)]
        public string CodigoPostal { get; set; }

        [Required]
        [StringLength(510)]
        public string Colonia { get; set; }

        [Required]
        [StringLength(510)]
        public string DelMunicipio { get; set; }

        [Required]
        [StringLength(510)]
        public string Ciudad { get; set; }

        [Required]
        [StringLength(510)]
        public string Estado { get; set; }

        [Required]
        [StringLength(510)]
        public string Pais { get; set; }

        [Required]
        [StringLength(100)]
        public string TipoPersona { get; set; }

        [Required]
        [StringLength(100)]
        public string RFC { get; set; }

        [Required]
        [StringLength(510)]
        public string RazonSocial { get; set; }

        [Required]
        [StringLength(100)]
        public string CFDI { get; set; }



        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
