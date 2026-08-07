using AppGimn.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppGimn.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();

            await context.Database.MigrateAsync();

            // ================= 1. SEMBRAR EMPLEADOS =================
            if (!await context.Empleados.AnyAsync())
            {
                var empleados = new List<Empleado>
                {
                    new Empleado
                    {
                        Nombre = "Administrador",
                        Apellido = "Sistema",
                        DNI = "00000000",
                        Cargo = "Gerente",
                        Telefono = "1100000000",
                        Email = "admin@gimnasio.com",
                        FechaIngreso = DateTime.Now.AddYears(-3),
                        Salario = 450000,
                        EstaActivo = true,
                        Observaciones = "Gerencia General & Administración Central"
                    },
                    new Empleado
                    {
                        Nombre = "Ana",
                        Apellido = "Martínez",
                        DNI = "44556677",
                        Cargo = "Recepcionista",
                        Telefono = "1155667788",
                        Email = "recepcion@gimnasio.com",
                        FechaIngreso = DateTime.Now.AddMonths(-8),
                        Salario = 220000,
                        EstaActivo = true,
                        Observaciones = "Turno Mañana (08:00 a 16:00 hs)"
                    },
                    new Empleado
                    {
                        Nombre = "Diego",
                        Apellido = "Silva",
                        DNI = "44998877",
                        Cargo = "Recepcionista",
                        Telefono = "1199881122",
                        Email = "diego.recepcion@gimnasio.com",
                        FechaIngreso = DateTime.Now.AddMonths(-4),
                        Salario = 220000,
                        EstaActivo = true,
                        Observaciones = "Turno Tarde/Noche (16:00 a 23:00 hs)"
                    },
                    new Empleado
                    {
                        Nombre = "Lucas",
                        Apellido = "Rodríguez",
                        DNI = "55667788",
                        Cargo = "Entrenador",
                        Telefono = "1166778899",
                        Email = "instructor@gimnasio.com",
                        FechaIngreso = DateTime.Now.AddYears(-2),
                        Salario = 280000,
                        EstaActivo = true,
                        Observaciones = "Head Coach - Musculación & Biomecánica"
                    },
                    new Empleado
                    {
                        Nombre = "Martina",
                        Apellido = "Herrera",
                        DNI = "55112233",
                        Cargo = "Entrenadora",
                        Telefono = "1122113344",
                        Email = "martina.herrera@gimnasio.com",
                        FechaIngreso = DateTime.Now.AddYears(-1),
                        Salario = 260000,
                        EstaActivo = true,
                        Observaciones = "Instructora de Pilates Reformer & Funcional"
                    },
                    new Empleado
                    {
                        Nombre = "Gonzalo",
                        Apellido = "Acosta",
                        DNI = "55334455",
                        Cargo = "Entrenador",
                        Telefono = "1133445566",
                        Email = "gonzalo.acosta@gimnasio.com",
                        FechaIngreso = DateTime.Now.AddMonths(-9),
                        Salario = 270000,
                        EstaActivo = true,
                        Observaciones = "Coach de HIIT & CrossFit"
                    }
                };

                await context.Empleados.AddRangeAsync(empleados);
                await context.SaveChangesAsync();
            }

            // ================= 2. SEMBRAR PLANES DE MEMBRESÍAS =================
            if (!await context.Membresias.AnyAsync())
            {
                var membresias = new List<Membresia>
                {
                    new Membresia
                    {
                        Nombre = "Pase Premium Aura 24/7",
                        Precio = 75000,
                        DuracionDias = 30,
                        Descripcion = "Acceso libre 24/7 a musculación, cardio y clases grupales ilimitadas con sauna y locker.",
                        EstaActivo = true
                    },
                    new Membresia
                    {
                        Nombre = "Pase Estándar Mensual",
                        Precio = 45000,
                        DuracionDias = 30,
                        Descripcion = "Acceso completo a la sala de musculación y área de cardio en horario regular.",
                        EstaActivo = true
                    },
                    new Membresia
                    {
                        Nombre = "Pase VIP Corporativo",
                        Precio = 120000,
                        DuracionDias = 30,
                        Descripcion = "Entrenador personal dedicado, evaluaciones bioimpedancia semanales y acceso al Spa.",
                        EstaActivo = true
                    },
                    new Membresia
                    {
                        Nombre = "Pase Estudiantil / Universitario",
                        Precio = 35000,
                        DuracionDias = 30,
                        Descripcion = "Descuento especial con certificado de alumno regular. Horario flexible.",
                        EstaActivo = true
                    },
                    new Membresia
                    {
                        Nombre = "Pase Anual Elite Aura",
                        Precio = 650000,
                        DuracionDias = 365,
                        Descripcion = "Membresía anual con 2 meses de regalo y kit oficial Aura Club.",
                        EstaActivo = true
                    }
                };

                await context.Membresias.AddRangeAsync(membresias);
                await context.SaveChangesAsync();
            }

            // ================= 3. SEMBRAR CLIENTES (+35 SOCIOS CON DIVERSOS ESTADOS) =================
            var dnisExistentes = new HashSet<string>(
                await context.Clientes.Select(c => c.DNI).ToListAsync());
                var nuevosClientes = new List<Cliente>
                {
                    new Cliente { Nombre = "Carlos", Apellido = "Gómez", DNI = "11223344", Email = "cliente@gimnasio.com", Telefono = "1144556677", FechaInscripcion = DateTime.Now.AddMonths(-6), ContactoEmergencia = "Roberto Gómez - 1144001122", ObservacionesMedicas = "Apto físico al día.", EstaActivo = true },
                    new Cliente { Nombre = "María", Apellido = "López", DNI = "22334455", Email = "maria.lopez@gmail.com", Telefono = "1199887766", FechaInscripcion = DateTime.Now.AddMonths(-3), ContactoEmergencia = "Fernando López - 1199002233", ObservacionesMedicas = "Sin observaciones.", EstaActivo = true },
                    new Cliente { Nombre = "Juan", Apellido = "Pérez", DNI = "33445566", Email = "juan.perez@gmail.com", Telefono = "1122334455", FechaInscripcion = DateTime.Now.AddMonths(-5), ContactoEmergencia = "Laura Pérez - 1122003344", ObservacionesMedicas = "Hipertensión leve controlada.", EstaActivo = true },
                    new Cliente { Nombre = "Sofía", Apellido = "Ramírez", DNI = "44112233", Email = "sofia.ramirez@gmail.com", Telefono = "1133221100", FechaInscripcion = DateTime.Now.AddMonths(-4), ContactoEmergencia = "Elena Ramírez - 1133009988", ObservacionesMedicas = "Apto de alto rendimiento.", EstaActivo = true },
                    new Cliente { Nombre = "Lucas", Apellido = "Fernández", DNI = "55223344", Email = "lucas.f@gmail.com", Telefono = "1155443322", FechaInscripcion = DateTime.Now.AddMonths(-2), ContactoEmergencia = "Esteban - 1155006677", ObservacionesMedicas = "Recuperación rodilla izquierda.", EstaActivo = true },
                    new Cliente { Nombre = "Valentina", Apellido = "Rossi", DNI = "66334455", Email = "valen.rossi@gmail.com", Telefono = "1166554433", FechaInscripcion = DateTime.Now.AddMonths(-7), ContactoEmergencia = "Tomás Rossi - 1166008899", ObservacionesMedicas = "Apto médico al día.", EstaActivo = true },
                    new Cliente { Nombre = "Mateo", Apellido = "Benítez", DNI = "77445566", Email = "mateo.b@gmail.com", Telefono = "1177665544", FechaInscripcion = DateTime.Now.AddMonths(-1), ContactoEmergencia = "Luciana Benítez - 1177001122", ObservacionesMedicas = "Sin observaciones.", EstaActivo = true },
                    new Cliente { Nombre = "Camila", Apellido = "Torres", DNI = "88556677", Email = "cami.torres@gmail.com", Telefono = "1188776655", FechaInscripcion = DateTime.Now.AddDays(-20), ContactoEmergencia = "Andrea Torres - 1188003344", ObservacionesMedicas = "Pase VIP.", EstaActivo = true },
                    new Cliente { Nombre = "Nicolás", Apellido = "Morales", DNI = "99667788", Email = "nico.morales@gmail.com", Telefono = "1199001122", FechaInscripcion = DateTime.Now.AddMonths(-8), ContactoEmergencia = "Patricia - 1199112233", ObservacionesMedicas = "Turno mañana.", EstaActivo = true },
                    new Cliente { Nombre = "Lucía", Apellido = "Castro", DNI = "12345678", Email = "lucia.castro@gmail.com", Telefono = "1123456789", FechaInscripcion = DateTime.Now.AddMonths(-5), ContactoEmergencia = "Miguel Castro - 1123004455", ObservacionesMedicas = "Pilates Reformer.", EstaActivo = true },

                    // 30 SOCIOS ADICIONALES CON VARIADOS ESTADOS
                    new Cliente { Nombre = "Gabriel", Apellido = "Fernández", DNI = "30111222", Email = "gabi.fernandez@gmail.com", Telefono = "1130111222", FechaInscripcion = DateTime.Now.AddMonths(-4), ContactoEmergencia = "Mamá - 1130000001", ObservacionesMedicas = "Excelente estado.", EstaActivo = true },
                    new Cliente { Nombre = "Florencia", Apellido = "Díaz", DNI = "30222333", Email = "flor.diaz@hotmail.com", Telefono = "1130222333", FechaInscripcion = DateTime.Now.AddMonths(-3), ContactoEmergencia = "Marido - 1130000002", ObservacionesMedicas = "Requiere estiramientos post rutina.", EstaActivo = true },
                    new Cliente { Nombre = "Esteban", Apellido = "Peralta", DNI = "30333444", Email = "esteban.peralta@gmail.com", Telefono = "1130333444", FechaInscripcion = DateTime.Now.AddMonths(-2), ContactoEmergencia = "Hermano - 1130000003", ObservacionesMedicas = "Al día.", EstaActivo = true },
                    new Cliente { Nombre = "Agustina", Apellido = "Bianchi", DNI = "30444555", Email = "agus.bianchi@gmail.com", Telefono = "1130444555", FechaInscripcion = DateTime.Now.AddMonths(-6), ContactoEmergencia = "Padre - 1130000004", ObservacionesMedicas = "Apto médico excelente.", EstaActivo = true },
                    new Cliente { Nombre = "Joaquín", Apellido = "Soria", DNI = "30555666", Email = "joaco.soria@gmail.com", Telefono = "1130555666", FechaInscripcion = DateTime.Now.AddMonths(-1), ContactoEmergencia = "Madre - 1130000005", ObservacionesMedicas = "Apto físico vence esta semana.", EstaActivo = true },
                    new Cliente { Nombre = "Micaela", Apellido = "Domínguez", DNI = "30666777", Email = "mica.dominguez@gmail.com", Telefono = "1130666777", FechaInscripcion = DateTime.Now.AddMonths(-5), ContactoEmergencia = "Tía - 1130000006", ObservacionesMedicas = "Plan Estudiantil.", EstaActivo = true },
                    new Cliente { Nombre = "Ramiro", Apellido = "Medina", DNI = "30777888", Email = "ramiro.medina@gmail.com", Telefono = "1130777888", FechaInscripcion = DateTime.Now.AddMonths(-9), ContactoEmergencia = "Esposa - 1130000007", ObservacionesMedicas = "Cuota vencida.", EstaActivo = true },
                    new Cliente { Nombre = "Delfina", Apellido = "Romero", DNI = "30888999", Email = "delfi.romero@gmail.com", Telefono = "1130888999", FechaInscripcion = DateTime.Now.AddYears(-1), ContactoEmergencia = "Padre - 1130000008", ObservacionesMedicas = "Pase Anual.", EstaActivo = true },
                    new Cliente { Nombre = "Santiago", Apellido = "Navarro", DNI = "31000111", Email = "santi.navarro@gmail.com", Telefono = "1131000111", FechaInscripcion = DateTime.Now.AddMonths(-10), ContactoEmergencia = "Hermana - 1130000009", ObservacionesMedicas = "Dado de baja por viaje.", EstaActivo = false },
                    new Cliente { Nombre = "Paula", Apellido = "Giménez", DNI = "31111222", Email = "pau.gimenez@gmail.com", Telefono = "1131111222", FechaInscripcion = DateTime.Now.AddMonths(-4), ContactoEmergencia = "Novio - 1130000010", ObservacionesMedicas = "Al día.", EstaActivo = true },
                    new Cliente { Nombre = "Bautista", Apellido = "Vega", DNI = "31222333", Email = "bauti.vega@gmail.com", Telefono = "1131222333", FechaInscripcion = DateTime.Now.AddMonths(-3), ContactoEmergencia = "Madre - 1130000011", ObservacionesMedicas = "Cuota pendiente.", EstaActivo = true },
                    new Cliente { Nombre = "Constanza", Apellido = "Rivas", DNI = "31333444", Email = "coty.rivas@gmail.com", Telefono = "1131333444", FechaInscripcion = DateTime.Now.AddMonths(-7), ContactoEmergencia = "Padre - 1130000012", ObservacionesMedicas = "Pase VIP Corporativo.", EstaActivo = true },
                    new Cliente { Nombre = "Felipe", Apellido = "Molina", DNI = "31444555", Email = "felipe.molina@gmail.com", Telefono = "1131444555", FechaInscripcion = DateTime.Now.AddMonths(-5), ContactoEmergencia = "Madre - 1130000013", ObservacionesMedicas = "Asma leve controlada.", EstaActivo = true },
                    new Cliente { Nombre = "Morena", Apellido = "Quiroga", DNI = "31555666", Email = "more.quiroga@gmail.com", Telefono = "1131555666", FechaInscripcion = DateTime.Now.AddMonths(-2), ContactoEmergencia = "Hermano - 1130000014", ObservacionesMedicas = "Sin afecciones.", EstaActivo = true },
                    new Cliente { Nombre = "Ignacio", Apellido = "Carrizo", DNI = "31666777", Email = "nacho.carrizo@gmail.com", Telefono = "1131666777", FechaInscripcion = DateTime.Now.AddMonths(-6), ContactoEmergencia = "Padre - 1130000015", ObservacionesMedicas = "Pendiente de cobro.", EstaActivo = true },
                    new Cliente { Nombre = "Lola", Apellido = "Ríos", DNI = "31777888", Email = "lola.rios@gmail.com", Telefono = "1131777888", FechaInscripcion = DateTime.Now.AddMonths(-8), ContactoEmergencia = "Madre - 1130000016", ObservacionesMedicas = "Al día en pagos.", EstaActivo = true },
                    new Cliente { Nombre = "Tomás", Apellido = "Paredes", DNI = "31888999", Email = "tomi.paredes@gmail.com", Telefono = "1131888999", FechaInscripcion = DateTime.Now.AddMonths(-1), ContactoEmergencia = "Hermana - 1130000017", ObservacionesMedicas = "Renueva este fin de semana.", EstaActivo = true },
                    new Cliente { Nombre = "Camila", Apellido = "Bustos", DNI = "32000111", Email = "cami.bustos@gmail.com", Telefono = "1132000111", FechaInscripcion = DateTime.Now.AddMonths(-4), ContactoEmergencia = "Esposo - 1130000018", ObservacionesMedicas = "Al día.", EstaActivo = true },
                    new Cliente { Nombre = "Facundo", Apellido = "Godoy", DNI = "32111222", Email = "facu.godoy@gmail.com", Telefono = "1132111222", FechaInscripcion = DateTime.Now.AddMonths(-11), ContactoEmergencia = "Amigo - 1130000019", ObservacionesMedicas = "Socio inactivo.", EstaActivo = false },
                    new Cliente { Nombre = "Lara", Apellido = "Silva", DNI = "32222333", Email = "lara.silva@gmail.com", Telefono = "1132222333", FechaInscripcion = DateTime.Now.AddMonths(-3), ContactoEmergencia = "Madre - 1130000020", ObservacionesMedicas = "Pase Estándar.", EstaActivo = true },
                    new Cliente { Nombre = "Sebastián", Apellido = "Ferreyra", DNI = "32333444", Email = "seba.ferreyra@gmail.com", Telefono = "1132333444", FechaInscripcion = DateTime.Now.AddMonths(-5), ContactoEmergencia = "Hermano - 1130000021", ObservacionesMedicas = "Cuota vencida.", EstaActivo = true },
                    new Cliente { Nombre = "Martina", Apellido = "Franco", DNI = "32444555", Email = "marti.franco@gmail.com", Telefono = "1132444555", FechaInscripcion = DateTime.Now.AddMonths(-2), ContactoEmergencia = "Padre - 1130000022", ObservacionesMedicas = "Molestia articular en muñeca.", EstaActivo = true },
                    new Cliente { Nombre = "Julián", Apellido = "Cabrera", DNI = "32555666", Email = "juli.cabrera@gmail.com", Telefono = "1132555666", FechaInscripcion = DateTime.Now.AddMonths(-7), ContactoEmergencia = "Esposa - 1130000023", ObservacionesMedicas = "Al día.", EstaActivo = true },
                    new Cliente { Nombre = "Zoe", Apellido = "Lucero", DNI = "32666777", Email = "zoe.lucero@gmail.com", Telefono = "1132666777", FechaInscripcion = DateTime.Now.AddMonths(-1), ContactoEmergencia = "Madre - 1130000024", ObservacionesMedicas = "Pase Estudiantil.", EstaActivo = true },
                    new Cliente { Nombre = "Manuel", Apellido = "Ojeda", DNI = "32777888", Email = "manu.ojeda@gmail.com", Telefono = "1132777888", FechaInscripcion = DateTime.Now.AddMonths(-9), ContactoEmergencia = "Padre - 1130000025", ObservacionesMedicas = "Cuota vencida hace 10 días.", EstaActivo = true },
                    new Cliente { Nombre = "Renata", Apellido = "Villalba", DNI = "32888999", Email = "rena.villalba@gmail.com", Telefono = "1132888999", FechaInscripcion = DateTime.Now.AddYears(-1), ContactoEmergencia = "Hermana - 1130000026", ObservacionesMedicas = "Pase Anual al día.", EstaActivo = true },
                    new Cliente { Nombre = "Benjamín", Apellido = "Arias", DNI = "33000111", Email = "benja.arias@gmail.com", Telefono = "1133000111", FechaInscripcion = DateTime.Now.AddMonths(-4), ContactoEmergencia = "Madre - 1130000027", ObservacionesMedicas = "Musculación.", EstaActivo = true },
                    new Cliente { Nombre = "Abril", Apellido = "Ponce", DNI = "33111222", Email = "abril.ponce@gmail.com", Telefono = "1133111222", FechaInscripcion = DateTime.Now.AddMonths(-3), ContactoEmergencia = "Padre - 1130000028", ObservacionesMedicas = "Renueva hoy.", EstaActivo = true },
                    new Cliente { Nombre = "Máximo", Apellido = "Correa", DNI = "33222333", Email = "maxi.correa@gmail.com", Telefono = "1133222333", FechaInscripcion = DateTime.Now.AddMonths(-12), ContactoEmergencia = "Hermano - 1130000029", ObservacionesMedicas = "Baja voluntaria.", EstaActivo = false },
                    new Cliente { Nombre = "Victoria", Apellido = "Luna", DNI = "33333444", Email = "vicky.luna@gmail.com", Telefono = "1133333444", FechaInscripcion = DateTime.Now.AddMonths(-6), ContactoEmergencia = "Madre - 1130000030", ObservacionesMedicas = "Pase VIP.", EstaActivo = true },
                    new Cliente { Nombre = "Iván", Apellido = "Santillán", DNI = "33444555", Email = "ivan.santillan@gmail.com", Telefono = "1133444555", FechaInscripcion = DateTime.Now.AddMonths(-5), ContactoEmergencia = "Padre - 1130000031", ObservacionesMedicas = "Escoliosis leve.", EstaActivo = true },
                    new Cliente { Nombre = "Paloma", Apellido = "Vera", DNI = "33555666", Email = "palo.vera@gmail.com", Telefono = "1133555666", FechaInscripcion = DateTime.Now.AddMonths(-2), ContactoEmergencia = "Novio - 1130000032", ObservacionesMedicas = "Al día.", EstaActivo = true }
                };

                // Asignar fecha de nacimiento y edad realista a cada socio nuevo (evita edades de 2000+ años)
                for (int i = 0; i < nuevosClientes.Count; i++)
                {
                    var c = nuevosClientes[i];
                    int anios = 18 + (i % 8) * 3;            // entre 18 y 39 años
                    int meses = i % 11;
                    int dias = (i * 5) % 27;
                    c.FechaNacimiento = DateTime.Today.AddYears(-anios).AddMonths(-meses).AddDays(-dias);
                }

                // Insertar únicamente socios cuyo DNI aún no exista (idempotente por DNI)
                var clientesFaltantes = nuevosClientes
                    .Where(c => !dnisExistentes.Contains(c.DNI))
                    .ToList();

                if (clientesFaltantes.Any())
                {
                    await context.Clientes.AddRangeAsync(clientesFaltantes);
                    await context.SaveChangesAsync();
                }

                // Reparar socios ya existentes que quedaron sin fecha de nacimiento (año 1)
                var clientesSinNacimiento = await context.Clientes
                    .Where(c => c.FechaNacimiento.Year <= 1900)
                    .ToListAsync();
                foreach (var c in clientesSinNacimiento)
                {
                    int edad = 25 + (c.Id % 12) * 2;
                    c.FechaNacimiento = DateTime.Today.AddYears(-edad).AddDays(-(c.Id % 27));
                }
                if (clientesSinNacimiento.Any())
                {
                    await context.SaveChangesAsync();
                }

            // ================= 4. SEMBRAR PAGOS PARA TODOS LOS SOCIOS =================
            var todosClientes = await context.Clientes.ToListAsync();
            var premiumPlan = await context.Membresias.FirstOrDefaultAsync(m => m.Nombre.Contains("Premium"));
            var estandarPlan = await context.Membresias.FirstOrDefaultAsync(m => m.Nombre.Contains("Estándar"));
            var vipPlan = await context.Membresias.FirstOrDefaultAsync(m => m.Nombre.Contains("VIP"));
            var estudianPlan = await context.Membresias.FirstOrDefaultAsync(m => m.Nombre.Contains("Estudiantil"));

            int recIndex = 1000;
            foreach (var c in todosClientes)
            {
                bool tienePago = await context.Pagos.AnyAsync(p => p.ClienteId == c.Id);
                if (!tienePago)
                {
                    var planSelected = c.DNI.EndsWith("4") || c.DNI.EndsWith("8") 
                        ? vipPlan 
                        : (c.DNI.EndsWith("7") ? estudianPlan : (c.Id % 2 == 0 ? premiumPlan : estandarPlan));

                    // Determinar estatus de cuota
                    bool esVencido = c.DNI == "22334455" || c.DNI == "30222333" || c.DNI == "30777888" || c.DNI == "31222333" || c.DNI == "31666777" || c.DNI == "32333444" || c.DNI == "32777888";

                    var pago = new Pago
                    {
                        ClienteId = c.Id,
                        MembresiaId = planSelected?.Id,
                        Monto = planSelected?.Precio ?? 45000,
                        FechaPago = esVencido ? DateTime.Now.AddDays(-35) : DateTime.Now.AddDays(-Random.Shared.Next(2, 22)),
                        FechaVencimiento = esVencido ? DateTime.Now.AddDays(-Random.Shared.Next(1, 10)) : DateTime.Now.AddDays(Random.Shared.Next(8, 30)),
                        MedioPago = c.Id % 4 == 0 ? "Efectivo" : (c.Id % 4 == 1 ? "MercadoPago" : (c.Id % 4 == 2 ? "Debito" : "Credito")),
                        ComprobanteNumero = $"REC-{recIndex++}",
                        RecepcionistaEmail = "recepcion@gimnasio.com"
                    };

                    await context.Pagos.AddAsync(pago);
                }
            }
            await context.SaveChangesAsync();

            // ================= 5. SEMBRAR EVALUACIONES FÍSICAS =================
            if (!await context.EvaluacionesFisicas.AnyAsync())
            {
                var carlos = await context.Clientes.FirstOrDefaultAsync(c => c.DNI == "11223344");
                var sofia = await context.Clientes.FirstOrDefaultAsync(c => c.DNI == "44112233");

                var evals = new List<EvaluacionFisica>();

                if (carlos != null)
                {
                    evals.Add(new EvaluacionFisica { ClienteId = carlos.Id, FechaEvaluacion = DateTime.Now.AddMonths(-3), PesoKg = 84.0, PorcentajeGrasa = 19.5, MasaMuscularKg = 38.5, ToraxCm = 98, CinturaCm = 89, BicepsCm = 36.0, Observaciones = "Evaluación diagnóstica inicial." });
                    evals.Add(new EvaluacionFisica { ClienteId = carlos.Id, FechaEvaluacion = DateTime.Now.AddMonths(-1), PesoKg = 80.5, PorcentajeGrasa = 16.0, MasaMuscularKg = 40.2, ToraxCm = 102, CinturaCm = 83, BicepsCm = 37.5, Observaciones = "Gran progreso en hipertrofia y reducción de grasa abdominal." });
                    evals.Add(new EvaluacionFisica { ClienteId = carlos.Id, FechaEvaluacion = DateTime.Now.AddDays(-4), PesoKg = 78.5, PorcentajeGrasa = 14.2, MasaMuscularKg = 41.8, ToraxCm = 104, CinturaCm = 81, BicepsCm = 38.5, Observaciones = "Excelente estado físico y masa muscular magra destacada." });
                }

                if (sofia != null)
                {
                    evals.Add(new EvaluacionFisica { ClienteId = sofia.Id, FechaEvaluacion = DateTime.Now.AddMonths(-2), PesoKg = 62.0, PorcentajeGrasa = 22.0, MasaMuscularKg = 26.5, ToraxCm = 88, CinturaCm = 68, BicepsCm = 27.0, Observaciones = "Objetivo: Tonificación y resistencia." });
                    evals.Add(new EvaluacionFisica { ClienteId = sofia.Id, FechaEvaluacion = DateTime.Now.AddDays(-10), PesoKg = 59.5, PorcentajeGrasa = 18.8, MasaMuscularKg = 28.1, ToraxCm = 90, CinturaCm = 65, BicepsCm = 28.2, Observaciones = "Reducción de grasa visceral y aumento de fuerza en tren inferior." });
                }

                await context.EvaluacionesFisicas.AddRangeAsync(evals);
                await context.SaveChangesAsync();
            }

            // ================= 6. SEMBRAR USUARIOS DE IDENTITY =================
            await CrearUsuarioSiNoExiste(userManager, "admin@gimnasio.com", "Admin123!", "00000000", esAdmin: true, esEmpleado: true, esCliente: false);
            await CrearUsuarioSiNoExiste(userManager, "recepcion@gimnasio.com", "Recep123!", "44556677", esAdmin: false, esEmpleado: true, esCliente: false);
            await CrearUsuarioSiNoExiste(userManager, "instructor@gimnasio.com", "Coach123!", "55667788", esAdmin: false, esEmpleado: true, esCliente: false);
            await CrearUsuarioSiNoExiste(userManager, "cliente@gimnasio.com", "Cliente123!", "11223344", esAdmin: false, esEmpleado: false, esCliente: true);
        }

        private static async Task CrearUsuarioSiNoExiste(UserManager<Usuario> userManager, string email, string password, string dni, bool esAdmin, bool esEmpleado, bool esCliente)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new Usuario
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    DNI = dni,
                    EsAdmin = esAdmin,
                    EsEmpleado = esEmpleado,
                    EsCliente = esCliente
                };
                await userManager.CreateAsync(user, password);
            }
        }
    }
}
