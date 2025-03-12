using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using PRUEBAS_LOGIN.Models;
using PRUEBAS_LOGIN.Permisos; // Para el atributo [ValidarSesion]
using Newtonsoft.Json;
using System.Collections.Generic;

namespace PRUEBAS_LOGIN.Controllers
{
    [ValidarSesion]
    public class UsuariosController : Controller
    {
        // Método auxiliar para asignar el nombre del usuario al ViewBag desde la sesión
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

        // Acción para listar los usuarios en la vista "Usuarios"
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
                            Usuario oUsuario = new Usuario();
                            oUsuario.IdUsuario = Convert.ToInt32(dr["IdUsuario"]);
                            oUsuario.Correo = dr["Correo"].ToString();
                            oUsuario.Nombre = dr["Nombre"].ToString();
                            // La contraseña no se muestra por seguridad
                            lista.Add(oUsuario);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewData["MensajeError"] = "Error al listar usuarios: " + ex.Message;
            }
            return View(lista);
        }

        // GET: Usuarios/Nuevo
        public IActionResult Nuevo()
        {
            CargarNombreUsuarioEnViewBag();
            return View();
        }

        // POST: Usuarios/Nuevo
        [HttpPost]
        [Obsolete]
        public IActionResult Nuevo(Usuario oUsuario)
        {
            CargarNombreUsuarioEnViewBag();

            // Validar que las contraseñas coincidan
            if (oUsuario.Clave != oUsuario.ConfirmarClave)
            {
                TempData["Mensaje"] = "Las contraseñas no coinciden";
                TempData["MensajeTipo"] = "danger";
                return RedirectToAction("Usuarios", "Usuarios");
            }
            // Convertir la clave a SHA256
            oUsuario.Clave = ConvertirClaveSha256(oUsuario.Clave);
            bool registrado;
            string mensaje;

            try
            {
                using (SqlConnection cn = new SqlConnection(Configuracion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Correo", oUsuario.Correo);
                    cmd.Parameters.AddWithValue("@Clave", oUsuario.Clave);
                    cmd.Parameters.AddWithValue("@Nombre", oUsuario.Nombre);
                    cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                    mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al registrar el usuario: " + ex.Message;
                TempData["MensajeTipo"] = "danger";
                return RedirectToAction("Usuarios", "Usuarios");
            }

            if (!registrado)
            {
                // Por ejemplo, si el correo ya existe el SP devuelve "El correo ya está registrado."
                TempData["Mensaje"] = mensaje;
                TempData["MensajeTipo"] = "danger";
                return RedirectToAction("Usuarios", "Usuarios");
            }

            TempData["Mensaje"] = mensaje;
            TempData["MensajeTipo"] = "success";
            return RedirectToAction("Usuarios", "Usuarios");
        }

        // POST: Usuarios/Editar
        [HttpPost]
        [Obsolete]
        public IActionResult Editar(Usuario oUsuario)
        {
            CargarNombreUsuarioEnViewBag();

            bool registrado;
            string mensaje;

            try
            {
                using (SqlConnection cn = new SqlConnection(Configuracion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_ActualizarCorreoYNombre", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@Correo", oUsuario.Correo);
                    cmd.Parameters.AddWithValue("@Nombre", oUsuario.Nombre);
                    cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                    mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al actualizar el usuario: " + ex.Message;
                TempData["MensajeTipo"] = "danger";
                return RedirectToAction("Usuarios", "Usuarios");
            }

            TempData["Mensaje"] = mensaje;
            TempData["MensajeTipo"] = registrado ? "success" : "danger";
            return RedirectToAction("Usuarios", "Usuarios");
        }

        // POST: Usuarios/Eliminar
        [HttpPost]
        [Obsolete]
        public IActionResult Eliminar(int IdUsuario)
        {
            CargarNombreUsuarioEnViewBag();

            bool registrado;
            string mensaje;

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
                    registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                    mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al eliminar el usuario: " + ex.Message;
                TempData["MensajeTipo"] = "danger";
                return RedirectToAction("Usuarios", "Usuarios");
            }
            TempData["Mensaje"] = mensaje;
            TempData["MensajeTipo"] = registrado ? "success" : "danger";
            return RedirectToAction("Usuarios", "Usuarios");
        }

        // POST: Usuarios/ActualizarClave
        [HttpPost]
        [Obsolete]
        public IActionResult ActualizarClave(int IdUsuario, string NuevaClave, string ConfirmarClave)
        {
            CargarNombreUsuarioEnViewBag();

            if (NuevaClave != ConfirmarClave)
            {
                TempData["Mensaje"] = "Las contraseñas no coinciden.";
                TempData["MensajeTipo"] = "danger";
                return RedirectToAction("Usuarios", "Usuarios");
            }
            string claveEncriptada = ConvertirClaveSha256(NuevaClave);
            string mensaje = "";
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
                    mensaje = mensajeParam.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error al actualizar la contraseña: " + ex.Message;
            }
            TempData["Mensaje"] = mensaje;
            TempData["MensajeTipo"] = "success";
            return RedirectToAction("Usuarios", "Usuarios");
        }

        // Método para convertir una cadena a SHA256 (utilizado en el registro y actualización de contraseña)
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
