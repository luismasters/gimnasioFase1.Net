namespace AppGimn.Models
{
    public class DashboardViewModel
    {
        public int TotalClientes { get; set; }
        public int TotalEmpleados { get; set; }
        public int ClientesInactivos { get; set; }
        public int EmpleadosInactivos { get; set; }
        public int ClientesRecientes { get; set; }
        public List<CargoCantidad> EmpleadosPorCargo { get; set; } = new();
    }

    public class CargoCantidad
    {
        public string Cargo { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class EstadisticasViewModel
    {
        public List<MesConteo> ClientesPorMes { get; set; } = new();
        public List<EmpleadoAntiguedad> EmpleadosPorAntiguedad { get; set; } = new();
    }

    public class MesConteo
    {
        public int Año { get; set; }
        public int Mes { get; set; }
        public int Cantidad { get; set; }
        public string NombreMes => new DateTime(Año, Mes, 1).ToString("MMM yyyy");
    }

    public class EmpleadoAntiguedad
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public int AntiguedadDias { get; set; }
        public double AntiguedadAños => Math.Round(AntiguedadDias / 365.0, 1);
    }
}
