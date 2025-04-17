using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRUEBAS_LOGIN.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Complementos
    {
        public Complementos33 Complementos33 { get; set; }
    }

    public class Complementos33
    {
        public List<Pagos20> Pagos20 { get; set; }
    }

    public class DoctoRelacionado
    {
        public ImpuestosDR ImpuestosDR { get; set; }
        public string IdDocumento { get; set; }
        public string Serie { get; set; }
        public string Folio { get; set; }
        public string MetodoDePagoDR { get; set; }
        public string MonedaDR { get; set; }
        public string EquivalenciaDR { get; set; }
        public string ImpPagado { get; set; }
        public string NumParcialidad { get; set; }
        public string ImpSaldoAnt { get; set; }
        public string ImpSaldoInsoluto { get; set; }
        public string ObjetoImpDR { get; set; }
    }

    public class ImpuestosDR
    {
        public List<TrasladosDR> TrasladosDR { get; set; }
        public object RetencionesDR { get; set; }
    }

    public class ImpuestosP
    {
        public List<TrasladosP> TrasladosP { get; set; }
        public object RetencionesP { get; set; }
    }

    public class Pago
    {
        public ImpuestosP ImpuestosP { get; set; }
        public DateTime FechaPago { get; set; }
        public string FormaDePagoP { get; set; }
        public string MonedaP { get; set; }
        public string TipoCambioP { get; set; }
        public string Monto { get; set; }
        public object NumOperacion { get; set; }
        public object RfcEmisorCtaOrd { get; set; }
        public object NomBancoOrdExt { get; set; }
        public object CtaOrdenante { get; set; }
        public object RfcEmisorCtaBen { get; set; }
        public object CtaBeneficiario { get; set; }
        public object TipoCadPago { get; set; }
        public object CertPago { get; set; }
        public object CadPago { get; set; }
        public object SelloPago { get; set; }
        public List<DoctoRelacionado> DoctoRelacionado { get; set; }
    }

    public class Pagos20
    {
        public List<Pago> Pago { get; set; }
        public Totales Totales { get; set; }
    }

    public class Partida
    {
        public double cantidad { get; set; }
        public object unidad { get; set; }
        public string descripcion { get; set; }
        public double valorUnitario { get; set; }
        public double importe { get; set; }
        public double descuento { get; set; }
        public string noIdentificacion { get; set; }
        public string comentario { get; set; }
        public string aduana { get; set; }
        public string pedimentonumero { get; set; }
        public string pedimentofecha { get; set; }
        public string cuentapredial { get; set; }
        public string extra1 { get; set; }
        public string extra2 { get; set; }
        public string extra3 { get; set; }
        public string extra4 { get; set; }
        public string extra5 { get; set; }
        public string ClaveProdServ { get; set; }
        public string ClaveUnidad { get; set; }
        public object imp1 { get; set; }
        public object imp2 { get; set; }
        public object imp3 { get; set; }
        public object ieps1 { get; set; }
        public object ieps2 { get; set; }
        public object ieps3 { get; set; }
        public object ret1 { get; set; }
        public object ret2 { get; set; }
        public object ret3 { get; set; }
        public string ObjetoImp { get; set; }
    }

    public class Root
    {
        public string Exportacion { get; set; }
        public string DomicilioFiscalReceptor { get; set; }
        public string RegimenFiscalReceptor { get; set; }
        public object Periodicidad { get; set; }
        public object Meses { get; set; }
        public object Anio { get; set; }
        public object cfdirelacionado { get; set; }
        public object implocal { get; set; }
        public object diasdecredito { get; set; }
        public object Confirmacion { get; set; }
        public string consecutivoManual { get; set; }
        public object FolioFiscalOrig { get; set; }
        public object SerieFolioFiscalOrig { get; set; }
        public object FechaFolioFiscalOrig { get; set; }
        public object MontoFolioFiscalOrig { get; set; }
        public object ordendecompra { get; set; }
        public object ordendeservicio { get; set; }
        public object extra_comm1 { get; set; }
        public object extra_comm2 { get; set; }
        public object extra_comm3 { get; set; }
        public object extra_comm4 { get; set; }
        public object extra_comm5 { get; set; }
        public string RFCemisor { get; set; }
        public string expedicion { get; set; }
        public string serieid { get; set; }
        public string RFCreceptor { get; set; }
        public string razonsocial { get; set; }
        public string numext { get; set; }
        public string numint { get; set; }
        public string calle { get; set; }
        public string colonia { get; set; }
        public string ciudad { get; set; }
        public string estado { get; set; }
        public string codigopostal { get; set; }
        public string pais { get; set; }
        public string telefono { get; set; }
        public string contacto { get; set; }
        public string email { get; set; }
        public string NumCtaPago { get; set; }
        public double subTotal { get; set; }
        public int TipoCambio { get; set; }
        public string Moneda { get; set; }
        public string tipoDeComprobante { get; set; }
        public string formaDePago { get; set; }
        public string metodoDePago { get; set; }
        public string RegimenFiscal { get; set; }
        public string UsoCFDI { get; set; }
        public string condicionesDePago { get; set; }
        public string observaciones { get; set; }
        public List<Partida> partidas { get; set; }
        public Complementos complementos { get; set; }
    }

    public class Totales
    {
        public string TotalRetencionesIVA { get; set; }
        public string TotalRetencionesISR { get; set; }
        public string TotalRetencionesIEPS { get; set; }
        public string TotalTrasladosBaseIVA16 { get; set; }
        public string TotalTrasladosImpuestoIVA16 { get; set; }
        public string TotalTrasladosBaseIVA8 { get; set; }
        public string TotalTrasladosImpuestoIVA8 { get; set; }
        public string TotalTrasladosBaseIVA0 { get; set; }
        public string TotalTrasladosImpuestoIVA0 { get; set; }
        public string TotalTrasladosBaseIVAExento { get; set; }
        public string MontoTotalPagos { get; set; }
    }

    public class TrasladosDR
    {
        public string BaseDR { get; set; }
        public string ImpuestoDR { get; set; }
        public string TipoFactorDR { get; set; }
        public string TasaOCuotaDR { get; set; }
        public string ImporteDR { get; set; }
    }

    public class TrasladosP
    {
        public string BaseP { get; set; }
        public string ImpuestoP { get; set; }
        public string TipoFactorP { get; set; }
        public string TasaOCuotaP { get; set; }
        public string ImporteP { get; set; }
    }


}
