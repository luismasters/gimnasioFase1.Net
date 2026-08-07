using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppGimn.Migrations
{
    /// <inheritdoc />
    public partial class IndiceUnicoDniCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============ 1. LIMPIAR DUPLICADOS EXISTENTES ============
            // Conserva el registro de menor Id por DNI, reasigna sus registros
            // relacionados (pagos, asistencias, evaluaciones, rutinas) y elimina los duplicados.

            migrationBuilder.Sql(@"
IF OBJECT_ID('tempdb..#DupClientes') IS NOT NULL DROP TABLE #DupClientes;
SELECT DNI, MIN(Id) AS MinId
INTO #DupClientes
FROM Clientes
WHERE DNI IS NOT NULL AND DNI <> ''
GROUP BY DNI
HAVING COUNT(*) > 1;

UPDATE p SET p.ClienteId = d.MinId
FROM Pagos p
INNER JOIN Clientes c ON c.Id = p.ClienteId
INNER JOIN #DupClientes d ON c.DNI = d.DNI
WHERE c.Id <> d.MinId;

UPDATE a SET a.ClienteId = d.MinId
FROM Asistencias a
INNER JOIN Clientes c ON c.Id = a.ClienteId
INNER JOIN #DupClientes d ON c.DNI = d.DNI
WHERE c.Id <> d.MinId;

UPDATE e SET e.ClienteId = d.MinId
FROM EvaluacionesFisicas e
INNER JOIN Clientes c ON c.Id = e.ClienteId
INNER JOIN #DupClientes d ON c.DNI = d.DNI
WHERE c.Id <> d.MinId;

UPDATE r SET r.ClienteId = d.MinId
FROM Rutinas r
INNER JOIN Clientes c ON c.Id = r.ClienteId
INNER JOIN #DupClientes d ON c.DNI = d.DNI
WHERE c.Id <> d.MinId;

DELETE c
FROM Clientes c
INNER JOIN #DupClientes d ON c.DNI = d.DNI
WHERE c.Id <> d.MinId;

DROP TABLE #DupClientes;
");

            migrationBuilder.Sql(@"
IF OBJECT_ID('tempdb..#DupEmpleados') IS NOT NULL DROP TABLE #DupEmpleados;
SELECT DNI, MIN(Id) AS MinId
INTO #DupEmpleados
FROM Empleados
WHERE DNI IS NOT NULL AND DNI <> ''
GROUP BY DNI
HAVING COUNT(*) > 1;

UPDATE r SET r.InstructorId = d.MinId
FROM Rutinas r
INNER JOIN Empleados e ON e.Id = r.InstructorId
INNER JOIN #DupEmpleados d ON e.DNI = d.DNI
WHERE e.Id <> d.MinId;

DELETE e
FROM Empleados e
INNER JOIN #DupEmpleados d ON e.DNI = d.DNI
WHERE e.Id <> d.MinId;

DROP TABLE #DupEmpleados;
");

            // ============ 2. CREAR ÍNDICES ÚNICOS ============
            migrationBuilder.CreateIndex(
                name: "IX_Clientes_DNI",
                table: "Clientes",
                column: "DNI",
                unique: true,
                filter: "[DNI] IS NOT NULL AND [DNI] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_DNI",
                table: "Empleados",
                column: "DNI",
                unique: true,
                filter: "[DNI] IS NOT NULL AND [DNI] <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clientes_DNI",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Empleados_DNI",
                table: "Empleados");
        }
    }
}