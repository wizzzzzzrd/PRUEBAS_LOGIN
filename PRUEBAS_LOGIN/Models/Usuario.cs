﻿using System.ComponentModel.DataAnnotations;

namespace PRUEBAS_LOGIN.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; } // Cambié "idUsuario" a "IdUsuario" para mantener la consistencia

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria.")]
        public string Clave { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }  // VARCHAR(100)
        public string ApellidoMaterno { get; set; }  // VARCHAR(100) (NULL permitido)
        public int Estatus { get; set; }             // INT (NOT NULL, DEFAULT 1)
        public int UsuarioCreacion { get; set; }     // INT (NOT NULL)

        public DateTime FechaCreacion { get; set; }  // DATETIME (NOT NULL, DEFAULT GETDATE())

        public string ConfirmarClave { get; set; }

    }
}
