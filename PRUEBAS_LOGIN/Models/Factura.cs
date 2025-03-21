// Archivo: Factura.cs
namespace PRUEBAS_LOGIN.Models
{
    public class Factura
    {
        public int IdFactura { get; set; }
        public DateTime FechaEmision { get; set; }
        public string NombreCliente { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
    }
}
