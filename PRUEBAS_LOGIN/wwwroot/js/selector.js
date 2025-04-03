document.getElementById('agregarTicketBtn').addEventListener('click', async function () {
    const ticket = document.getElementById('ticketInput').value.trim();
    if (!ticket) {
        alert("Por favor, ingrese un número de ticket.");
        return;
    }

    try {
        // Consulta la información del ticket (ajusta la URL según corresponda)
        const response = await fetch('/Facturacion/BuscarVentas?ticket=' + encodeURIComponent(ticket));
        const data = await response.json();
        console.log('Datos recibidos:', data);

        const tbody = document.querySelector('#ticketsTable tbody');

        // Si no se encontraron registros, puedes notificar al usuario o agregar una fila informativa
        if (data.length === 0) {
            alert("No se encontró información para el ticket ingresado.");
        } else {
            // Si se devuelve un array, se agregan todos los registros (puede ser de 1 o más)
            data.forEach(venta => {
                // Extraer los datos respetando posibles diferencias en mayúsculas/minúsculas
                const idVenta = venta.idVenta || venta.IdVenta || ticket; // se puede usar el ticket ingresado como fallback
                const subtotal = venta.subtotal || venta.Subtotal || '';
                const importeDescuento = venta.importeDescuento || venta.ImporteDescuento || '';
                const importeIEPS = venta.importeIEPS || venta.ImporteIEPS || '';
                const importeIVA = venta.importeIVA || venta.ImporteIVA || '';
                const totalVenta = venta.totalVenta || venta.TotalVenta || '';
                const tipoPago = venta.tipoPago || venta.TipoPago || '';

                // Crear la nueva fila en la tabla
                const tr = document.createElement('tr');
                tr.innerHTML = `
          <td>${idVenta}</td>
          <td>${subtotal}</td>
          <td>${importeDescuento}</td>
          <td>${importeIEPS}</td>
          <td>${importeIVA}</td>
          <td>${totalVenta}</td>
          <td>${tipoPago}</td>
          <td>
            <button class="btn btn-sm btn-danger quitarTicketBtn">
              <i class="bi bi-x-circle"></i> Quitar
            </button>
          </td>
        `;
                tbody.appendChild(tr);
            });
        }

        // Opcional: Limpiar el campo de entrada después de agregar
        document.getElementById('ticketInput').value = '';
    } catch (error) {
        console.error('Error al buscar el ticket:', error);
    }
});

// Delegación de eventos para remover una fila de la tabla
document.querySelector('#ticketsTable tbody').addEventListener('click', function (e) {
    if (e.target.closest('.quitarTicketBtn')) {
        const fila = e.target.closest('tr');
        fila.parentNode.removeChild(fila);
    }
});
