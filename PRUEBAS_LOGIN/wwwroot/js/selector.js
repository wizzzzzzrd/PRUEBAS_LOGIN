
    document.getElementById('buscarVentasBtn').addEventListener('click', async function () {
    const ticket = document.getElementById('buscarTicket').value.trim();
    if (!ticket) {
        alert("Por favor, ingrese un número de ticket.");
    return;
    }
    try {
      const response = await fetch('/Facturacion/BuscarVentas?ticket=' + encodeURIComponent(ticket));
    const data = await response.json();
    console.log('Datos recibidos:', data);

    const tbody = document.querySelector('#ventasTable tbody');
    tbody.innerHTML = ''; // Limpiar datos anteriores

    if (data.length === 0) {
        tbody.innerHTML = '<tr><td colspan="9">No se encontraron ventas.</td></tr>';
      } else {
        data.forEach(venta => {
            // Se verifica si la propiedad viene en camelCase o PascalCase
            const idVenta = venta.idVenta || venta.IdVenta || '';
            const subtotal = venta.subtotal || venta.Subtotal || '';
            const importeDescuento = venta.importeDescuento || venta.ImporteDescuento || '';
            const importeIEPS = venta.importeIEPS || venta.ImporteIEPS || '';
            const importeIVA = venta.importeIVA || venta.ImporteIVA || '';
            const totalVenta = venta.totalVenta || venta.TotalVenta || '';
            const tipoPago = venta.tipoPago || venta.TipoPago || '';

            const tr = document.createElement('tr');
            tr.innerHTML = `
            <td><input type="checkbox" class="form-check-input"></td>
            <td>${idVenta}</td>
            <td>${subtotal}</td>
            <td>${importeDescuento}</td>
            <td>${importeIEPS}</td>
            <td>${importeIVA}</td>
            <td>${totalVenta}</td>
            <td>${tipoPago}</td>
            <td>
              <button class="btn btn-sm btn-primary">Seleccionar</button>
            </td>
          `;
            tbody.appendChild(tr);
        });
      }
    } catch (error) {
        console.error('Error:', error);
    }
  });
