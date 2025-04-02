namespace PRUEBAS_LOGIN.Models
{
    public class VentaDTO
    {
        public int IdVenta { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ImporteDescuento { get; set; }
        public decimal ImporteIEPS { get; set; }
        public decimal ImporteIVA { get; set; }
        public decimal TotalVenta { get; set; }
        public string TipoPago { get; set; } // Este campo se puede asignar vacío o calcularlo según corresponda.
    }
}
