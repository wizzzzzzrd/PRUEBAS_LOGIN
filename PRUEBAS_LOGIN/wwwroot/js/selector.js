// Función para actualizar los totales de facturación
function updateTotals() {
    const tbody = document.querySelector('#ticketsTable tbody');
    const rows = tbody.querySelectorAll('tr');

    let subtotalSinDesc = 0;
    let descuentoTotal = 0;
    let ivaTotal = 0;
    let iepsTotal = 0;

    rows.forEach(row => {
        const cells = row.querySelectorAll('td');
        // Asumiendo el siguiente orden:
        // 0: Ticket, 1: Subtotal, 2: Descuento, 3: IEPS, 4: IVA, 5: Total Venta, 6: Tipo de Pago, 7: Acciones
        const subtotal = parseFloat(cells[1].textContent) || 0;
        const descuento = parseFloat(cells[2].textContent) || 0;
        const ieps = parseFloat(cells[3].textContent) || 0;
        const iva = parseFloat(cells[4].textContent) || 0;

        subtotalSinDesc += subtotal;
        descuentoTotal += descuento;
        iepsTotal += ieps;
        ivaTotal += iva;
    });

    const subtotalConDesc = subtotalSinDesc - descuentoTotal;
    const totalConDesc = subtotalConDesc + ivaTotal + iepsTotal;

    document.getElementById('subtotalSinDesc').textContent = subtotalSinDesc.toFixed(2);
    document.getElementById('descuentoTotal').textContent = descuentoTotal.toFixed(2);
    document.getElementById('subtotalConDesc').textContent = subtotalConDesc.toFixed(2);
    document.getElementById('ivaTotal').textContent = ivaTotal.toFixed(2);
    document.getElementById('iepsTotal').textContent = iepsTotal.toFixed(2);
    document.getElementById('totalConDesc').textContent = totalConDesc.toFixed(2);
}

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

        if (!Array.isArray(data) || data.length === 0) {
            alert("No se encontró información para el ticket ingresado.");
        } else {
            data.forEach(venta => {
                // Extraer los datos respetando posibles diferencias en mayúsculas/minúsculas
                const idVenta = venta.idVenta || venta.IdVenta || ticket;
                const subtotal = venta.subtotal || venta.Subtotal || '0';
                const importeDescuento = venta.importeDescuento || venta.ImporteDescuento || '0';
                const importeIEPS = venta.importeIEPS || venta.ImporteIEPS || '0';
                const importeIVA = venta.importeIVA || venta.ImporteIVA || '0';
                const totalVenta = venta.totalVenta || venta.TotalVenta || '0';
                const tipoPago = venta.tipoPago || venta.TipoPago || '';

                // Índice para el binding MVC
                const index = tbody.querySelectorAll('tr').length;

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
                        <button type="button" class="btn btn-sm btn-danger quitarTicketBtn">
                            <i class="bi bi-x-circle"></i> Quitar
                        </button>
                    </td>
                `;

                // Campos ocultos para enviar al servidor en List<TicketViewModel> Tickets
                tr.innerHTML += `
                    <input type="hidden" name="Tickets[${index}].IdVenta" value="${idVenta}">
                    <input type="hidden" name="Tickets[${index}].Subtotal" value="${subtotal}">
                    <input type="hidden" name="Tickets[${index}].ImporteDescuento" value="${importeDescuento}">
                    <input type="hidden" name="Tickets[${index}].IEPS" value="${importeIEPS}">
                    <input type="hidden" name="Tickets[${index}].IVA" value="${importeIVA}">
                    <input type="hidden" name="Tickets[${index}].TotalVenta" value="${totalVenta}">
                    <input type="hidden" name="Tickets[${index}].TipoPago" value="${tipoPago}">
                `;

                tbody.appendChild(tr);
            });

            // Actualiza los totales después de agregar las filas
            updateTotals();
        }

        // Limpiar el campo de entrada
        document.getElementById('ticketInput').value = '';
    } catch (error) {
        console.error('Error al buscar el ticket:', error);
        alert('Ocurrió un error al consultar el ticket. Revisa la consola para más detalles.');
    }
});

// Delegación de eventos para remover una fila de la tabla
document.querySelector('#ticketsTable tbody').addEventListener('click', function (e) {
    if (e.target.closest('.quitarTicketBtn')) {
        const fila = e.target.closest('tr');
        fila.parentNode.removeChild(fila);
        // Actualizar totales después de quitar una fila
        updateTotals();
    }
});
    