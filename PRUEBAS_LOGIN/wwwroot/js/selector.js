document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('addTicketsBtn').addEventListener('click', function () {
        var pendingTbody = document.getElementById('pendingTable').getElementsByTagName('tbody')[0];
        var ticketsTbody = document.getElementById('ticketsTable').getElementsByTagName('tbody')[0];

        // Convertimos a array para iterar sin problemas aunque se modifique el DOM
        var rows = Array.from(pendingTbody.getElementsByTagName('tr'));

        rows.forEach(function (row) {
            var checkbox = row.querySelector('input[type="checkbox"]');

            if (checkbox && checkbox.checked) {
                // Extraer datos de la fila pendiente
                var cells = row.getElementsByTagName('td');
                var ticketValue = cells[1].innerText;
                var totalValue = cells[2].innerText;

                // Crear nueva fila para la tabla de tickets
                var newRow = document.createElement('tr');

                newRow.innerHTML = `
                    <td>${ticketValue}</td>
                    <td>${totalValue}</td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td>${totalValue}</td>
                    <td></td>
                    <td><button class="btn btn-danger btn-sm delete-ticket">Eliminar</button></td>
                `;

                // Agregar la nueva fila a la tabla de tickets
                ticketsTbody.appendChild(newRow);

                // Eliminar la fila de la tabla de pendientes
                row.remove();
            }
        });

        // Asignar evento a los botones "Eliminar"
        asignarEventosEliminar();
    });

    function asignarEventosEliminar() {
        document.querySelectorAll('.delete-ticket').forEach(button => {
            button.removeEventListener('click', eliminarTicket); // Evita eventos duplicados
            button.addEventListener('click', eliminarTicket);
        });
    }

    function eliminarTicket(event) {
        var row = event.target.closest('tr');
        var ticketValue = row.cells[0].innerText;
        var totalValue = row.cells[1].innerText;
        var pendingTbody = document.getElementById('pendingTable').getElementsByTagName('tbody')[0];

        // Crear una nueva fila para devolver a pendientes
        var pendingRow = document.createElement('tr');
        pendingRow.innerHTML = `
            <td><input type="checkbox" class="form-check-input"></td>
            <td>${ticketValue}</td>
            <td>${totalValue}</td>
        `;

        // Agregar la fila a la tabla de pendientes
        pendingTbody.appendChild(pendingRow);

        // Remover la fila de la tabla de tickets
        row.remove();
    }
});
