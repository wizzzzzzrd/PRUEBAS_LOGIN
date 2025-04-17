using System.ComponentModel.DataAnnotations;

public class EmisorFactura
{
    [Key]
    public int EmisorFacturaId { get; set; }  // Llave primaria

    [Required]
    [MaxLength(100)]
    public string NombreRazonSocial { get; set; }  // Nombre o Razon Social

    [MaxLength(100)]
    public string NombreSucursal { get; set; }     // Nombre Sucursal

    [MaxLength(18)]
    public string CURP { get; set; }               // CURP

    [MaxLength(100)]
    public string Correo { get; set; }             // Correo (correo principal)

    [MaxLength(13)]
    public string RFC { get; set; }                // RFC

    [MaxLength(50)]
    public string RegimenFiscal { get; set; }      // Regimen Fiscal

    [MaxLength(100)]
    public string CorreoSucursal { get; set; }     // Correo de sucursal

    public int? NumeroSucursal { get; set; }       // Numero Sucursal

    [MaxLength(13)]
    public string RFCVentasAlMenudeo { get; set; }   // RFC Ventas al Menudeo

    [MaxLength(100)]
    public string CorreoVentas { get; set; }       // Correo de ventas

    [MaxLength(20)]
    public string Telefono { get; set; }           // Telefono

    [MaxLength(50)]
    public string TipoDistribucion { get; set; }   // Tipo Distribucion

    [MaxLength(100)]
    public string LugarExpedicion { get; set; }      // Lugar de Expedicion

    [MaxLength(50)]
    public string SerieFactura { get; set; }         // Serie Factura

    [MaxLength(100)]
    public string Calle { get; set; }              // Calle

    [MaxLength(20)]
    public string NumeroExterior { get; set; }     // Numero Exterior

    [MaxLength(20)]
    public string NumeroInterior { get; set; }     // Numero Interior

    public int? CodigoPostal { get; set; }         // Codigo Postal (almacenado como entero)

    [MaxLength(100)]
    public string Localidad { get; set; }          // Localidad

    [MaxLength(100)]
    public string Colonia { get; set; }            // Colonia

    [MaxLength(100)]
    public string DelegacionMunicipio { get; set; } // Delegacion Municipio

    [MaxLength(100)]
    public string Estado { get; set; }             // Estado

    [MaxLength(100)]
    public string Pais { get; set; }               // Pais

    [MaxLength(255)]
    public string Referencia { get; set; }         // Referencia
}
