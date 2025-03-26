using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using PRUEBAS_LOGIN.Models;
using PRUEBAS_LOGIN.Permisos;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Common;

namespace PRUEBAS_LOGIN.Controllers
{
    [ValidarSesion]
    public class UsuariosController : Controller
    {
        private void CargarNombreUsuarioEnViewBag()
        {
            var usuarioJson = HttpContext.Session.GetString("usuario");
            if (!string.IsNullOrEmpty(usuarioJson))
            {
                var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);
                ViewBag.NombreUsuario = usuario?.Nombre;
            }
            else
            {
                ViewBag.NombreUsuario = "Usuario";
            }
        }

        [Obsolete]
        public IActionResult Usuarios()
        {
            CargarNombreUsuarioEnViewBag();

            List<Usuario> lista = new List<Usuario>();
            try
            {
                using (SqlConnection cn = new SqlConnection(Configuracion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_ListarUsuarios", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Usuario
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                Correo = dr["Correo"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                ApellidoPaterno = dr["ApellidoPaterno"].ToString(),
                                ApellidoMaterno = dr["ApellidoMaterno"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al listar usuarios: " + ex.Message;
                TempData["MensajeTipo"] = "danger";
            }
            return View(lista);
        }

        [HttpPost]
        [Obsolete]
        public IActionResult Nuevo(Usuario oUsuario)
        {
            CargarNombreUsuarioEnViewBag();

            if (oUsuario.Clave != oUsuario.ConfirmarClave)
            {
                TempData["Mensaje"] = "Las contraseñas no coinciden";
                TempData["MensajeTipo"] = "danger";
                return RedirectToAction("Usuarios");
            }

            // Encriptar la clave
            oUsuario.Clave = ConvertirClaveSha256(oUsuario.Clave);

            try
            {
                using (SqlConnection cn = new SqlConnection(Configuracion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Correo", oUsuario.Correo);
                    cmd.Parameters.AddWithValue("@Clave", oUsuario.Clave);
                    cmd.Parameters.AddWithValue("@Nombre", oUsuario.Nombre);
                    cmd.Parameters.AddWithValue("@ApellidoPaterno", oUsuario.ApellidoPaterno);
                    // Si ApellidoMaterno está vacío, se envía DBNull.Value
                    cmd.Parameters.AddWithValue("@ApellidoMaterno", string.IsNullOrWhiteSpace(oUsuario.ApellidoMaterno) ? (object)DBNull.Value : oUsuario.ApellidoMaterno);
                    // Ajusta este valor según la lógica de tu aplicación (por ejemplo, el usuario logueado o un valor predeterminado)
                    cmd.Parameters.AddWithValue("@UsuarioCreacion", 1);

                    // Parámetros de salida
                    cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    bool registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                    string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                    if (!registrado)
                    {
                        // Manejar el caso de correo duplicado
                        if (mensaje.Contains("correo") || mensaje.Contains("duplicado"))
                        {
                            mensaje = "El correo electrónico ya está registrado. Por favor use otro correo.";
                        }

                        TempData["Mensaje"] = mensaje;
                        TempData["MensajeTipo"] = "danger";
                        return RedirectToAction("Usuarios");
                    }

                    TempData["Mensaje"] = mensaje;
                    TempData["MensajeTipo"] = "success";
                    return RedirectToAction("Usuarios");
                }
            }
            catch (DbException dbEx) when (dbEx is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
            {
                TempData["Mensaje"] = "Error: El correo electrónico ya está registrado.";
                TempData["MensajeTipo"] = "danger";
                return RedirectToAction("Usuarios");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al registrar el usuario: " + ex.Message;
                TempData["MensajeTipo"] = "danger";
                return RedirectToAction("Usuarios");
            }
        }

        [HttpPost]
        [Obsolete]
        public IActionResult Editar(Usuario oUsuario)
        {
            CargarNombreUsuarioEnViewBag();

            try
            {
                using (SqlConnection cn = new SqlConnection(Configuracion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_ActualizarCorreoYNombre", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@Correo", oUsuario.Correo);
                    cmd.Parameters.AddWithValue("@Nombre", oUsuario.Nombre);
                    cmd.Parameters.AddWithValue("@ApellidoPaterno", oUsuario.ApellidoPaterno);
                    cmd.Parameters.AddWithValue("@ApellidoMaterno", string.IsNullOrWhiteSpace(oUsuario.ApellidoMaterno) ? (object)DBNull.Value : oUsuario.ApellidoMaterno);

                    // Parámetros de salida
                    cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    bool registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                    string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                    if (!registrado)
                    {
                        // Manejar específicamente el caso de correo duplicado
                        if (mensaje.Contains("correo") || mensaje.Contains("duplicado"))
                        {
                            mensaje = "El correo electrónico ya está registrado por otro usuario. Por favor use otro correo.";
                        }

                        TempData["Mensaje"] = mensaje;
                        TempData["MensajeTipo"] = "danger";
                        return RedirectToAction("Usuarios");
                    }

                    TempData["Mensaje"] = "Usuario actualizado correctamente";
                    TempData["MensajeTipo"] = "success";
                    return RedirectToAction("Usuarios");
                }
            }
            catch (DbException dbEx) when (dbEx is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
            {
                TempData["Mensaje"] = "El correo electrónico ya está registrado por un usuario, Porfavor intenta con otro.";
                TempData["MensajeTipo"] = "danger";
                return RedirectToAction("Usuarios");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al actualizar el usuario: " + ex.Message;
                TempData["MensajeTipo"] = "danger";
                return RedirectToAction("Usuarios");
            }
        }

        [HttpPost]
        [Obsolete]
        public IActionResult Eliminar(int IdUsuario)
        {
            CargarNombreUsuarioEnViewBag();

            try
            {
                using (SqlConnection cn = new SqlConnection(Configuracion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_EliminarUsuario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                    cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    bool registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                    string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                    TempData["Mensaje"] = mensaje;
                    TempData["MensajeTipo"] = registrado ? "success" : "danger";
                    return RedirectToAction("Usuarios");
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al eliminar el usuario: " + ex.Message;
                TempData["MensajeTipo"] = "danger";
                return RedirectToAction("Usuarios");
            }
        }

        [HttpPost]
        [Obsolete]
        public IActionResult ActualizarClave(int IdUsuario, string NuevaClave, string ConfirmarClave)
        {
            CargarNombreUsuarioEnViewBag();

            if (NuevaClave != ConfirmarClave)
            {
                TempData["Mensaje"] = "Las contraseñas no coinciden.";
                TempData["MensajeTipo"] = "danger";
                return RedirectToAction("Usuarios");
            }

            string claveEncriptada = ConvertirClaveSha256(NuevaClave);

            try
            {
                using (SqlConnection cn = new SqlConnection(Configuracion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_ActualizarClave", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                    cmd.Parameters.AddWithValue("@Clave", claveEncriptada);

                    SqlParameter mensajeParam = new SqlParameter("@Mensaje", SqlDbType.NVarChar, 100)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(mensajeParam);

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    string mensaje = mensajeParam.Value.ToString();
                    TempData["Mensaje"] = mensaje;
                    TempData["MensajeTipo"] = "success";
                    return RedirectToAction("Usuarios");
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al actualizar la contraseña: " + ex.Message;
                TempData["MensajeTipo"] = "danger";
                return RedirectToAction("Usuarios");
            }
        }

        private string ConvertirClaveSha256(string texto)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}