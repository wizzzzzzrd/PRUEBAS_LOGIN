using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Data;
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
            // Verifica que exista la sesión
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
            // Asigna también el IdUsuario para la validación en la vista
            ViewBag.IdUsuario = usuario.IdUsuario;

            // Inicializa el modelo
            Fiscales model = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Configuracion.cadena))
                {
                    string queryFiscales = "SELECT * FROM dbo.DatosFiscales WHERE IdUsuario = @IdUsuario";
                    using (SqlCommand cmd = new SqlCommand(queryFiscales, cn))
                    {
                        cmd.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
                        cn.Open();
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

        [HttpPost]
        [Obsolete]
        public IActionResult Fiscales(Fiscales model)
        {
            // Recupera el IdUsuario de la sesión para vincular la información fiscal al usuario correspondiente
            int? userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
            {
                TempData["TipoMensaje"] = "danger";
                TempData["Mensaje"] = "No se encontró sesión de usuario.";
                return RedirectToAction("Login", "Acceso");
            }
            model.IdUsuario = userId.Value;

            // Mapeo de campos con nombres diferentes en el formulario
            if (Request.Form.ContainsKey("municipio"))
            {
                model.DelMunicipio = Request.Form["municipio"];
            }
            if (Request.Form.ContainsKey("codigoPostal"))
            {
                model.CodigoPostal = Request.Form["codigoPostal"];
            }
            if (Request.Form.ContainsKey("colonia"))
            {
                model.Colonia = Request.Form["colonia"];
            }
            if (Request.Form.ContainsKey("ciudad"))
            {
                model.Ciudad = Request.Form["ciudad"];
            }
            if (Request.Form.ContainsKey("estado"))
            {
                model.Estado = Request.Form["estado"];
            }
            if (Request.Form.ContainsKey("pais"))
            {
                model.Pais = Request.Form["pais"];
            }
            if (Request.Form.ContainsKey("rfc"))
            {
                model.RFC = Request.Form["rfc"];
            }
            if (Request.Form.ContainsKey("razonSocial"))
            {
                model.RazonSocial = Request.Form["razonSocial"];
            }
            if (Request.Form.ContainsKey("usoCFDI"))
            {
                model.CFDI = Request.Form["usoCFDI"];
            }
         
            // Asignar las fechas actuales
            model.FechaCreacion = DateTime.Now;
            model.FechaModificacion = DateTime.Now;

            // Validación básica: el campo Calle es obligatorio
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
                    cmd.CommandType = CommandType.StoredProcedure;

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
                    cmd.Parameters.AddWithValue("@CFDI", model.CFDI); // Asegúrate de incluir este parámetro
                    cmd.Parameters.AddWithValue("@FechaCreacion", model.FechaCreacion);
                    cmd.Parameters.AddWithValue("@FechaModificacion", model.FechaModificacion);

                    // Parámetros de salida para conocer el resultado del registro
                    SqlParameter registradoParam = new SqlParameter("@Registrado", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(registradoParam);

                    SqlParameter mensajeParam = new SqlParameter("@Mensaje", SqlDbType.VarChar, 100)
                    {
                        Direction = ParameterDirection.Output
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
                // Información detallada para depuración
                TempData["TipoMensaje"] = "danger";
                TempData["Mensaje"] = "Error al registrar: " + ex.Message;
                TempData["Debug"] = "StackTrace: " + ex.StackTrace;
            }
            return RedirectToAction("Fiscales");
        }
        public IActionResult Facturas()
        {
            // Obtener el usuario de la sesión
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

            // Asignar el nombre de usuario al ViewBag
            ViewBag.NombreUsuario = usuario.Nombre;

            // Si en el futuro deseas pasar también la lista de facturas a la vista,
            // aquí puedes cargar los datos de facturación (por ejemplo, dummy o reales).
            return View();
        }


        // POST: Facturacion/ActualizarFiscal (para actualizar datos fiscales existentes)
        [HttpPost]
        [Obsolete]
        public IActionResult ActualizarFiscal(Fiscales model)
        {
            // Verifica la sesión
            int? userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
            {
                TempData["TipoMensaje"] = "danger";
                TempData["Mensaje"] = "No se encontró sesión de usuario.";
                return RedirectToAction("Login", "Acceso");
            }
            model.IdUsuario = userId.Value;
            model.FechaModificacion = DateTime.Now;

            // Asigna el valor de CFDI desde el formulario
            if (Request.Form.ContainsKey("usoCFDI"))
            {
                model.CFDI = Request.Form["usoCFDI"];
            }
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
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros del procedimiento almacenado
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
                    cmd.Parameters.AddWithValue("@CFDI", model.CFDI); // Asegúrate de incluir este parámetro
                    cmd.Parameters.AddWithValue("@FechaModificacion", model.FechaModificacion);

                    // Parámetros de salida
                    SqlParameter actualizadoParam = new SqlParameter("@Actualizado", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(actualizadoParam);

                    SqlParameter mensajeParam = new SqlParameter("@Mensaje", SqlDbType.VarChar, 100)
                    {
                        Direction = ParameterDirection.Output
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