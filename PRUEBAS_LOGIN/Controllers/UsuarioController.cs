﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using PRUEBAS_LOGIN.Models;
using PRUEBAS_LOGIN.Permisos;

namespace PRUEBAS_LOGIN.Controllers
{
    [ValidarSesion]
    public class UsuarioController : Controller
    {
        // GET: Usuario/Perfil
        public IActionResult Perfil()
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
            ViewBag.NombreCompleto = $"{usuario.Nombre} {usuario.ApellidoPaterno} {usuario.ApellidoMaterno}".Trim();

            return View(usuario);
        }


        // POST: Usuario/ActualizarCorreoYNombre
        [HttpPost]  
        [Obsolete]

        public IActionResult ActualizarCorreoYNombre(Usuario model)
        {
            // Verificar que la sesión esté activa
            int? userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Acceso");
            }
            model.IdUsuario = userId.Value;

            // Validar que se capture el correo electrónico
            if (string.IsNullOrWhiteSpace(model.Correo))
            {
                TempData["TipoMensaje"] = "danger";
                TempData["Mensaje"] = "Debes capturar el correo electrónico.";
                return RedirectToAction("Perfil");
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(Configuracion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_ActualizarCorreoYNombre", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", model.IdUsuario);
                    cmd.Parameters.AddWithValue("@Correo", model.Correo);
                    cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                    cmd.Parameters.AddWithValue("@ApellidoPaterno", model.ApellidoPaterno);

                    // Si ApellidoMaterno es null o vacío, se envía DBNull.Value
                    cmd.Parameters.AddWithValue("@ApellidoMaterno",
                        string.IsNullOrWhiteSpace(model.ApellidoMaterno) ? (object)DBNull.Value : model.ApellidoMaterno);

                    // Parámetros de salida
                    cmd.Parameters.Add(new SqlParameter("@Registrado", SqlDbType.Bit) { Direction = ParameterDirection.Output });
                    cmd.Parameters.Add(new SqlParameter("@Mensaje", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output });

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    bool registrado = Convert.ToBoolean(cmd.Parameters["@Registrado"].Value);
                    string mensaje = cmd.Parameters["@Mensaje"].Value.ToString();

                    TempData["TipoMensaje"] = registrado ? "success" : "danger";
                    TempData["Mensaje"] = mensaje;

                    if (registrado)
                    {
                        // Actualizamos la sesión con la nueva información
                        HttpContext.Session.SetString("usuario", JsonConvert.SerializeObject(model));
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                // Error por clave duplicada
                TempData["TipoMensaje"] = "danger";
                TempData["Mensaje"] = "El correo electrónico ingresado ya existe, intenta con otro.";
            }
            catch (Exception ex)
            {
                TempData["TipoMensaje"] = "danger";
                TempData["Mensaje"] = "Error: " + ex.Message;
            }
            return RedirectToAction("Perfil");
        }

        // POST: Usuario/ActualizarClave
        [HttpPost]
        [Obsolete]
        public IActionResult ActualizarClave(int IdUsuario, string currentPassword, string newPassword, string renewPassword)
        {
            // Verificar que la sesión esté activa
            int? userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Acceso");
            }

            // Verificar que las nuevas contraseñas coincidan
            if (newPassword != renewPassword)
            {
                TempData["TipoMensaje"] = "warning";
                TempData["Mensaje"] = "Las contraseñas nuevas no coinciden.";
                return RedirectToAction("Perfil");
            }

            try
            {
                // Encriptar la nueva contraseña
                string claveEncriptada = ConvertirClaveSha256(newPassword);

                using (SqlConnection cn = new SqlConnection(Configuracion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_ActualizarClave", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", userId.Value);
                    cmd.Parameters.AddWithValue("@Clave", claveEncriptada);

                    SqlParameter mensajeParam = new SqlParameter("@Mensaje", SqlDbType.NVarChar, 100)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(mensajeParam);

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    string mensaje = mensajeParam.Value.ToString();
                    TempData["TipoMensaje"] = "success";
                    TempData["Mensaje"] = mensaje;
                }
            }
            catch (Exception ex)
            {
                TempData["TipoMensaje"] = "danger";
                TempData["Mensaje"] = "Error: " + ex.Message;
            }
            return RedirectToAction("Perfil");
        }

        // Método de encriptación (debe coincidir con el utilizado en el login)
        private string ConvertirClaveSha256(string Clave)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(Clave);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                var sb = new System.Text.StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
