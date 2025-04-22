using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using PRUEBAS_LOGIN.Models;
using PRUEBAS_LOGIN.Permisos;
using System.Collections.Generic;

namespace PRUEBAS_LOGIN.Controllers
{
    [ValidarSesion]
    public class FacturacionController : Controller
    {
        // GET: Facturacion/Fiscales
        [HttpGet]
        [Obsolete]
        public IActionResult Fiscales()
        {
            // Verifica la sesión
            var usuarioJson = HttpContext.Session.GetString("usuario");
            if (string.IsNullOrEmpty(usuarioJson))
            {
                return RedirectToAction("Login", "Acceso");
            }
            var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);
            if (usuario == null)
            {
                return RedirectToAction("Login", "Acceso");
            }
            ViewBag.NombreUsuario = usuario.Nombre;
            ViewBag.IdUsuario = usuario.IdUsuario;

            // Inicializa el modelo de Datos Fiscales
            Fiscales model = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Configuracion.cadena))
                {
                    cn.Open();

                    // Consulta DatosFiscales
                    string queryFiscales = "SELECT * FROM dbo.DatosFiscales WHERE IdUsuario = @IdUsuario";
                    using (SqlCommand cmd = new SqlCommand(queryFiscales, cn))
                    {
                        cmd.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                model = new Fiscales
                                {
                                    IdFiscal = Convert.ToInt32(dr["IdFiscal"]),
                                    IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                    Calle = dr["Calle"].ToString(),
                                    NoExterior = dr["NoExterior"].ToString(),
                                    NoInterior = dr["NoInterior"] == DBNull.Value ? null : dr["NoInterior"].ToString(),
                                    CodigoPostal = dr["CodigoPostal"].ToString(),
                                    Colonia = dr["Colonia"].ToString(),
                                    DelMunicipio = dr["DelMunicipio"].ToString(),
                                    Ciudad = dr["Ciudad"].ToString(),
                                    Estado = dr["Estado"].ToString(),
                                    Pais = dr["Pais"].ToString(),
                                    TipoPersona = dr["TipoPersona"].ToString(),
                                    RFC = dr["RFC"].ToString(),
                                    RazonSocial = dr["RazonSocial"].ToString(),
                                    CFDI = dr["CFDI"].ToString(),
                                    FechaCreacion = dr["FechaCreacion"] as DateTime?,
                                    FechaModificacion = dr["FechaModificacion"] as DateTime?
                                };
                            }
                        }
                    }

                    // Consulta UsoCFDI
                    string queryUsoCFDI = "SELECT IdUsoCFDI, ClaveUsoCFDI, Descripcion FROM dbo.UsoCFDI";
                    using (SqlCommand cmd = new SqlCommand(queryUsoCFDI, cn))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            var usosCFDI = new List<UsoCFDI>();
                            while (dr.Read())
                            {
                                usosCFDI.Add(new UsoCFDI
                                {
                                    IdUsoCFDI = Convert.ToInt32(dr["IdUsoCFDI"]),
                                    ClaveUsoCFDI = dr["ClaveUsoCFDI"].ToString(),
                                    Descripcion = dr["Descripcion"].ToString()
                                });
                            }
                            ViewBag.UsosCFDI = usosCFDI;
                        }
                    }

                    // Consulta RegimenFiscal
                    string queryRegimenFiscal = "SELECT ClaveRegimenFiscal, Descripcion, TipoPersona FROM dbo.RegimenFiscal";
                    using (SqlCommand cmd = new SqlCommand(queryRegimenFiscal, cn))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            var regimenesFiscales = new List<RegimenFiscal>();
                            while (dr.Read())
                            {
                                regimenesFiscales.Add(new RegimenFiscal
                                {
                                    ClaveRegimenFiscal = dr["ClaveRegimenFiscal"].ToString(),
                                    Descripcion = dr["Descripcion"].ToString(),
                                    TipoPersona = dr["TipoPersona"].ToString()
                                });
                            }
                            ViewBag.RegimenesFiscales = regimenesFiscales;
                        }
                    }

                    // Consulta de Métodos de Pago
                    string queryMetodosPago = "SELECT IdMetodo, Clave, Descripcion FROM MetodosPago WHERE Activo = 1";
                    using (SqlCommand cmd = new SqlCommand(queryMetodosPago, cn))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            var metodosPago = new List<MetodoPago>();
                            while (dr.Read())
                            {
                                metodosPago.Add(new MetodoPago
                                {
                                    IdMetodo = Convert.ToInt32(dr["IdMetodo"]),
                                    Clave = dr["Clave"].ToString(),
                                    Descripcion = dr["Descripcion"].ToString()
                                });
                            }
                            ViewBag.MetodosPago = metodosPago;
                        }
                    }

                    // Consulta de Formas de Pago
                    string queryFormasPago = "SELECT IdTipoPago, ClavePago, TipoPago FROM tbc_FW_TipoCobro";
                    using (SqlCommand cmd = new SqlCommand(queryFormasPago, cn))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            var formasPago = new List<FormaPago>();
                            while (dr.Read())
                            {
                                formasPago.Add(new FormaPago
                                {
                                    IdTipoPago = Convert.ToInt32(dr["IdTipoPago"]),
                                    ClavePago = dr["ClavePago"].ToString(),
                                    TipoPago = dr["TipoPago"].ToString()
                                });
                            }
                            ViewBag.FormasPago = formasPago;
                        }
                    }

                    // Consulta de EmisorFactura
                    // Se asume que se recuperará el primer registro. Si existen múltiples registros, se debe ajustar la lógica
                    string queryEmisor = "SELECT TOP 1 * FROM dbo.EmisorFactura";
                    using (SqlCommand cmd = new SqlCommand(queryEmisor, cn))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                // Se recuperan todos los datos para el formulario
                                EmisorFactura emisor = new EmisorFactura
                                {
                                    EmisorFacturaId = Convert.ToInt32(dr["EmisorFacturaId"]),
                                    NombreRazonSocial = dr["NombreRazonSocial"].ToString(),
                                    NombreSucursal = dr["NombreSucursal"].ToString(),
                                    CURP = dr["CURP"].ToString(),
                                    Correo = dr["Correo"].ToString(),
                                    RFC = dr["RFC"].ToString(),
                                    RegimenFiscal = dr["RegimenFiscal"].ToString(),
                                    CorreoSucursal = dr["CorreoSucursal"].ToString(),
                                    NumeroSucursal = dr["NumeroSucursal"] != DBNull.Value ? Convert.ToInt32(dr["NumeroSucursal"]) : (int?)null,
                                    RFCVentasAlMenudeo = dr["RFCVentasAlMenudeo"].ToString(),
                                    CorreoVentas = dr["CorreoVentas"].ToString(),
                                    Telefono = dr["Telefono"].ToString(),
                                    TipoDistribucion = dr["TipoDistribucion"].ToString(),
                                    LugarExpedicion = dr["LugarExpedicion"].ToString(),
                                    SerieFactura = dr["SerieFactura"].ToString(),
                                    Calle = dr["Calle"].ToString(),
                                    NumeroExterior = dr["NumeroExterior"].ToString(),
                                    NumeroInterior = dr["NumeroInterior"] == DBNull.Value ? null : dr["NumeroInterior"].ToString(),
                                    CodigoPostal = dr["CodigoPostal"] != DBNull.Value ? Convert.ToInt32(dr["CodigoPostal"]) : (int?)null,
                                    Localidad = dr["Localidad"].ToString(),
                                    Colonia = dr["Colonia"].ToString(),
                                    DelegacionMunicipio = dr["DelegacionMunicipio"].ToString(),
                                    Estado = dr["Estado"].ToString(),
                                    Pais = dr["Pais"].ToString(),
                                    Referencia = dr["Referencia"].ToString()
                                };
                                ViewBag.Emisor = emisor;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["TipoMensaje"] = "danger";
                TempData["Mensaje"] = "Error al obtener datos fiscales: " + ex.Message;
            }

            ViewBag.HasFiscalData = (model != null);
            return View(model);
        }

        // POST: Facturacion/Fiscales (actualización de datos fiscales)
        [HttpPost]
        [Obsolete]
        public IActionResult Fiscales(Fiscales model)
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
            {
                TempData["TipoMensaje"] = "danger";
                TempData["Mensaje"] = "No se encontró sesión de usuario.";
                return RedirectToAction("Login", "Acceso");
            }
            model.IdUsuario = userId.Value;
            model.FechaModificacion = DateTime.Now;

            if (Request.Form.ContainsKey("municipio"))
                model.DelMunicipio = Request.Form["municipio"];
            if (Request.Form.ContainsKey("codigoPostal"))
                model.CodigoPostal = Request.Form["codigoPostal"];
            if (Request.Form.ContainsKey("colonia"))
                model.Colonia = Request.Form["colonia"];
            if (Request.Form.ContainsKey("ciudad"))
                model.Ciudad = Request.Form["ciudad"];
            if (Request.Form.ContainsKey("estado"))
                model.Estado = Request.Form["estado"];
            if (Request.Form.ContainsKey("pais"))
                model.Pais = Request.Form["pais"];
            if (Request.Form.ContainsKey("rfc"))
                model.RFC = Request.Form["rfc"];
            if (Request.Form.ContainsKey("razonSocial"))
                model.RazonSocial = Request.Form["razonSocial"];
            if (Request.Form.ContainsKey("usoCFDI"))
                model.CFDI = Request.Form["usoCFDI"];

            model.FechaCreacion = DateTime.Now;
            model.FechaModificacion = DateTime.Now;

            if (string.IsNullOrWhiteSpace(model.Calle))
            {
                TempData["TipoMensaje"] = "danger";
                TempData["Mensaje"] = "El campo Calle es obligatorio.";
                return RedirectToAction("Fiscales");
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(Configuracion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarFiscal", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", model.IdUsuario);
                    cmd.Parameters.AddWithValue("@Calle", model.Calle);
                    cmd.Parameters.AddWithValue("@NoExterior", model.NoExterior);
                    cmd.Parameters.AddWithValue("@NoInterior", string.IsNullOrWhiteSpace(model.NoInterior) ? (object)DBNull.Value : model.NoInterior);
                    cmd.Parameters.AddWithValue("@CodigoPostal", model.CodigoPostal);
                    cmd.Parameters.AddWithValue("@Colonia", model.Colonia);
                    cmd.Parameters.AddWithValue("@DelMunicipio", model.DelMunicipio);
                    cmd.Parameters.AddWithValue("@Ciudad", model.Ciudad);
                    cmd.Parameters.AddWithValue("@Estado", model.Estado);
                    cmd.Parameters.AddWithValue("@Pais", model.Pais);
                    cmd.Parameters.AddWithValue("@TipoPersona", model.TipoPersona);
                    cmd.Parameters.AddWithValue("@RFC", model.RFC);
                    cmd.Parameters.AddWithValue("@RazonSocial", model.RazonSocial);
                    cmd.Parameters.AddWithValue("@CFDI", model.CFDI);
                    cmd.Parameters.AddWithValue("@FechaCreacion", model.FechaCreacion);
                    cmd.Parameters.AddWithValue("@FechaModificacion", model.FechaModificacion);

                    SqlParameter registradoParam = new SqlParameter("@Registrado", System.Data.SqlDbType.Bit)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    cmd.Parameters.Add(registradoParam);

                    SqlParameter mensajeParam = new SqlParameter("@Mensaje", System.Data.SqlDbType.VarChar, 100)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    cmd.Parameters.Add(mensajeParam);

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    bool registrado = Convert.ToBoolean(registradoParam.Value);
                    string mensaje = mensajeParam.Value.ToString();

                    TempData["TipoMensaje"] = registrado ? "success" : "danger";
                    TempData["Mensaje"] = mensaje;
                }
            }
            catch (Exception ex)
            {
                TempData["TipoMensaje"] = "danger";
                TempData["Mensaje"] = "Error al registrar: " + ex.Message;
                TempData["Debug"] = "StackTrace: " + ex.StackTrace;
            }
            return RedirectToAction("Fiscales");
        }

        public IActionResult Facturas()
        {
            var usuarioJson = HttpContext.Session.GetString("usuario");
            if (string.IsNullOrEmpty(usuarioJson))
            {
                return RedirectToAction("Login", "Acceso");
            }
            var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);
            if (usuario == null)
            {
                return RedirectToAction("Login", "Acceso");
            }
            ViewBag.NombreUsuario = usuario.Nombre;
            return View();
        }

        // Método para buscar ventas (tickets) por IdVenta (número de ticket)
        [HttpGet]
        public IActionResult BuscarVentas(int ticket)
        {
            var ventas = new List<VentaDTO>();
            try
            {
                using (SqlConnection cn = new SqlConnection(Configuracion.cadena))
                {
                    cn.Open();
                    // Llamamos al procedimiento almacenado sp_BuscarVenta
                    using (SqlCommand cmd = new SqlCommand("sp_BuscarVenta", cn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ticket", ticket);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                ventas.Add(new VentaDTO
                                {
                                    IdVenta = Convert.ToInt32(dr["IdVenta"]),
                                    Subtotal = Convert.ToDecimal(dr["Subtotal"]),
                                    ImporteDescuento = Convert.ToDecimal(dr["ImporteDescuento"]),
                                    ImporteIEPS = Convert.ToDecimal(dr["ImporteIEPS"]),
                                    ImporteIVA = Convert.ToDecimal(dr["ImporteIVA"]),
                                    TotalVenta = Convert.ToDecimal(dr["TotalVenta"]),
                                    TipoPago = dr["TipoPago"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            return Json(ventas);
        }

        // POST: Facturacion/ActualizarFiscal (actualizar datos fiscales existentes)
        [HttpPost]
        [Obsolete]
        public IActionResult ActualizarFiscal(Fiscales model)
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
            {
                TempData["TipoMensaje"] = "danger";
                TempData["Mensaje"] = "No se encontró sesión de usuario.";
                return RedirectToAction("Login", "Acceso");
            }
            model.IdUsuario = userId.Value;
            model.FechaModificacion = DateTime.Now;

            if (Request.Form.ContainsKey("usoCFDI"))
                model.CFDI = Request.Form["usoCFDI"];
            else
            {
                TempData["TipoMensaje"] = "danger";
                TempData["Mensaje"] = "El campo Uso CFDI es obligatorio.";
                return RedirectToAction("Fiscales");
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(Configuracion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_ActualizarFiscal", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdFiscal", model.IdFiscal);
                    cmd.Parameters.AddWithValue("@IdUsuario", model.IdUsuario);
                    cmd.Parameters.AddWithValue("@Calle", model.Calle);
                    cmd.Parameters.AddWithValue("@NoExterior", model.NoExterior);
                    cmd.Parameters.AddWithValue("@NoInterior", string.IsNullOrWhiteSpace(model.NoInterior) ? (object)DBNull.Value : model.NoInterior);
                    cmd.Parameters.AddWithValue("@CodigoPostal", model.CodigoPostal);
                    cmd.Parameters.AddWithValue("@Colonia", model.Colonia);
                    cmd.Parameters.AddWithValue("@DelMunicipio", model.DelMunicipio);
                    cmd.Parameters.AddWithValue("@Ciudad", model.Ciudad);
                    cmd.Parameters.AddWithValue("@Estado", model.Estado);
                    cmd.Parameters.AddWithValue("@Pais", model.Pais);
                    cmd.Parameters.AddWithValue("@TipoPersona", model.TipoPersona);
                    cmd.Parameters.AddWithValue("@RFC", model.RFC);
                    cmd.Parameters.AddWithValue("@RazonSocial", model.RazonSocial);
                    cmd.Parameters.AddWithValue("@CFDI", model.CFDI);
                    cmd.Parameters.AddWithValue("@FechaModificacion", model.FechaModificacion);

                    SqlParameter actualizadoParam = new SqlParameter("@Actualizado", System.Data.SqlDbType.Bit)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    cmd.Parameters.Add(actualizadoParam);

                    SqlParameter mensajeParam = new SqlParameter("@Mensaje", System.Data.SqlDbType.VarChar, 100)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    cmd.Parameters.Add(mensajeParam);

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    bool actualizado = Convert.ToBoolean(actualizadoParam.Value);
                    string mensaje = mensajeParam.Value.ToString();

                    TempData["TipoMensaje"] = actualizado ? "success" : "danger";
                    TempData["Mensaje"] = mensaje;
                }
            }
            catch (Exception ex)
            {
                TempData["TipoMensaje"] = "danger";
                TempData["Mensaje"] = "Error al actualizar: " + ex.Message;
                TempData["Debug"] = "StackTrace: " + ex.StackTrace;
            }
            return RedirectToAction("Fiscales");
        }
    }
}
