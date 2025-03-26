using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

using PRUEBAS_LOGIN.Models;

using System.Data.SqlClient;
using System.Data;

using Newtonsoft.Json;


namespace PRUEBAS_LOGIN.Controllers
{
    public class AccesoController : Controller
    {


        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [Obsolete]
        public IActionResult Registrar(Usuario oUsuario)
        {
            bool registrado;
            string mensaje;

            // Validar que las contraseñas coincidan
            if (oUsuario.Clave != oUsuario.ConfirmarClave)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            // Encriptar la contraseña
            oUsuario.Clave = ConvertirClaveSha256(oUsuario.Clave);

            // Opcional: asignar valores por defecto a los apellidos en caso de que sean nulos o vacíos
            if (string.IsNullOrEmpty(oUsuario.ApellidoPaterno))
                oUsuario.ApellidoPaterno = "";
            if (string.IsNullOrEmpty(oUsuario.ApellidoMaterno))
                oUsuario.ApellidoMaterno = "";

            // Si no se define UsuarioCreacion, se puede asignar un valor por defecto (ej. 0)
            if (oUsuario.UsuarioCreacion <= 0)
                oUsuario.UsuarioCreacion = 0;

            using (SqlConnection cn = new SqlConnection(Configuracion.cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("@Clave", oUsuario.Clave);
                cmd.Parameters.AddWithValue("@Nombre", oUsuario.Nombre);
                cmd.Parameters.AddWithValue("@ApellidoPaterno", oUsuario.ApellidoPaterno);
                // Si ApellidoMaterno es null, se envía DBNull.Value
                cmd.Parameters.AddWithValue("@ApellidoMaterno",
                    string.IsNullOrEmpty(oUsuario.ApellidoMaterno) ? (object)DBNull.Value : oUsuario.ApellidoMaterno);
                cmd.Parameters.AddWithValue("@UsuarioCreacion", oUsuario.UsuarioCreacion);

                // Parámetros de salida
                cmd.Parameters.Add(new SqlParameter("@Registrado", SqlDbType.Bit) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@Mensaje", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Output });

                cn.Open();
                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["@Registrado"].Value);
                mensaje = cmd.Parameters["@Mensaje"].Value.ToString();
            }

            ViewData["Mensaje"] = mensaje;

            if (registrado)
            {
                return RedirectToAction("Login", "Acceso");
            }
            else
            {
                return View();
            }
        }
    
        // Método para iniciar sesión
        [HttpPost]
        [Obsolete]
        public IActionResult Login(Usuario oUsuario)
        {
            try
            {
                // Convertir la clave a SHA256
                oUsuario.Clave = ConvertirClaveSha256(oUsuario.Clave);

                using (SqlConnection cn = new SqlConnection(Configuracion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Correo", oUsuario.Correo);
                    cmd.Parameters.AddWithValue("@Clave", oUsuario.Clave);

                    cn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) // Si hay datos
                        {
                            int resultado = reader.GetInt32(0); // IdUsuario
                            string nombre = reader.IsDBNull(1) ? null : reader.GetString(1); // Nombre
                                                                                             // Se asume que el SP devuelve ApellidoPaterno en la columna 2 y ApellidoMaterno en la columna 3
                            string apellidoPaterno = reader.IsDBNull(2) ? null : reader.GetString(2);
                            string apellidoMaterno = reader.IsDBNull(3) ? null : reader.GetString(3);

                            if (resultado > 0)
                            {
                                // Guardar datos completos en la sesión
                                HttpContext.Session.SetInt32("userId", resultado);
                                var usuarioCompleto = new Usuario
                                {
                                    IdUsuario = resultado,
                                    Nombre = nombre,
                                    ApellidoPaterno = apellidoPaterno,
                                    ApellidoMaterno = apellidoMaterno,
                                    Correo = oUsuario.Correo // u otros campos que consideres necesarios
                                };
                                HttpContext.Session.SetString("usuario", JsonConvert.SerializeObject(usuarioCompleto));

                                // Redirigir correctamente a Home
                                return RedirectToAction("Index", "Home");
                            }
                            else if (resultado == -1)
                            {
                                ViewData["Mensaje"] = "La clave es incorrecta. Intenta nuevamente.";
                            }
                            else if (resultado == -2)
                            {
                                ViewData["Mensaje"] = "Correo electrónico incorrecto. Por favor, revisa tu correo.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewData["Mensaje"] = "Ocurrió un error al iniciar sesión: " + ex.Message;
            }

            return View();
        }
        // Método para convertir una cadena a SHA256 (renombrado para evitar conflictos)
        private string ConvertirClaveSha256(string texto)
        {
            using (System.Security.Cryptography.SHA256 sha256Hash = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(texto));
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }
}