# 📋 Hoja de Ruta y Fases de Desarrollo - AppGimn Enterprise

---

## 🎯 Estado Actual del Proyecto: FASE 1 COMPLETADA (100% Funcional)

### 🏋️‍♂️ Módulos Implementados en Fase 1:

1. **Base de Datos & Arquitectura Core (.NET 8 + EF Core):**
   - Entity Framework Core 8 con SQL Server LocalDB (`ApplicationDbContext`).
   - Seeder automatizado (`DbSeeder.cs`) con credenciales de desarrollo (`admin@gimnasio.com` / `Admin123!`), clientes y personal cargado para pruebas instantáneas.

2. **Gestión Completa de Clientes (CRUD + Filtros):**
   - Registro de datos personales, DNI, teléfono, email, fecha de inscripción, contacto de emergencia y ficha/observaciones médicas.
   - Búsqueda en tiempo real por DNI, Nombre o Apellido.
   - Control de estado (Cliente Activo / Inactivo).

3. **Gestión Completa de Empleados (CRUD + Filtros):**
   - Registro de personal, asignación de cargos (Gerente, Recepcionista, Entrenador, Mantenimiento), sueldos, fecha de ingreso y cálculo automático de antigüedad.
   - Filtro dinámico por cargo y búsqueda por texto.

4. **Autenticación y Roles (ASP.NET Core Identity):**
   - Registro e Inicio de Sesión personalizado.
   - Vinculación por DNI para asociar cuentas de usuario con registros de Clientes o Empleados.

5. **Landing Page SaaS & Portal Demo "Aura Fitness Club":**
   - Landing pública del software SaaS (`/Home/Index`).
   - Portal interactivo personalizado para el gimnasio ficticio **Aura Fitness Club** (`/Home/Demo`) con fotografías de instalaciones en alta resolución.

6. **Panel de Control Operativo (Dashboard & Estadísticas):**
   - Tarjetas métricas de Total Clientes, Empleados, Altas Recientes y Bajas.
   - Gráficos de distribución de personal y analítica histórica de inscripciones.

7. **Sistema de Diseño Unificado (Luxury Purple SaaS):**
   - Bootstrap 5, Bootstrap Icons, paleta Púrpura Imperial (`#8B3DFF`) y superficies oscuras elevadas (`#121212` / `#1E1E1E`). Guía documentada en `DESIGN_GUIDE.md`.

---

## 🚀 Propuesta de Organización para las Siguientes Fases

### 💳 FASE 2: Membresías, Cuotas y Control de Pagos (Siguiente Paso Recomendado)
- **Tipos de Membresía:** Configuración de planes (Mensual, Trimestral, Anual, Pase Libre, Musculación, VIP).
- **Control de Cuotas:** Registro de cobros, generación de comprobantes y fechas de vencimiento por cliente.
- **Estado Morosidad:** Indicadores visuales en pantalla de clientes (Al día / Vencido / Deudor) y bloqueo automático de acceso si la cuota está vencida.

### 🚪 FASE 3: Control de Acceso y Gestión de Clases Grupales
- **Módulo de Recepción / Molinete:** Pantalla de marcación rápida por DNI al ingresar al gimnasio con alerta sonora/visual de estado de cuota y apto médico.
- **Reserva de Clases:** Calendario de horarios para clases grupales (Spinning, Yoga, Crossfit) con cupos limitados por salón.

### 🏋️‍♀️ FASE 4: Fichas de Rutinas y Seguimiento Físico del Socio
- **Rutinas de Entrenamiento:** Creación de rutinas diarias asignadas por los entrenadores (Ejercicios, Series, Repeticiones, Cargas).
- **Evaluación Corporal:** Registro de evolución física (Peso, % Grasa, Pliegues, Perímetros musculares) con gráficos de progreso.

### 📊 FASE 5: Reportes Avanzados, Exportación y Automatizaciones
- **Exportación:** Generación de reportes de ingresos y padrón de socios en PDF / Excel.
- **Notificaciones Automáticas:** Recordatorios por Email/WhatsApp para avisar sobre vencimientos de cuotas o de aptos médicos.

---

## 📌 Organización Sugerida para Avanzar:
1. **Validación de Fase 1:** Confirmar si deseas ajustar o añadir algún campo específico en Clientes/Empleados antes de pasar a Pagos.
2. **Priorización de Fase 2:** Definir la estructura de precios y cuotas para comenzar a implementar la lógica de facturación y vencimientos.
