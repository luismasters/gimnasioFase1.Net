# 🎨 Guía de Diseño Visual y Componentes - AppGimn

Esta guía define las reglas de interfaz, accesibilidad, paleta de colores y componentes reutilizables para garantizar una **identidad visual consistente, limpia, sobria y profesional** en todo el proyecto **AppGimn**.

---

## 📐 1. Principios de Diseño

1. **Estilo Profesional y Sobrio:** Interfaz limpia estilo SaaS administrativo. Sin elementos estilo gaming ni luces neón.
2. **Framework Base:** **Bootstrap 5** + **Bootstrap Icons (`bi bi-*`)**.
3. **Accesibilidad (WCAG AA):** Alto contraste entre texto y fondos para garantizar legibilidad.
4. **Diseño Adaptable (Responsive):** Diseño Mobile-first compatible con smartphones, tablets y escritorio.

---

## 🎨 2. Paleta de Colores Oficial

Los colores están centralizados como variables CSS en [`wwwroot/css/site.css`](file:///C:/Users/luism/Desktop/proyectogymn/gimnasioFase1.Net/AppGimn/wwwroot/css/site.css):

| Elemento | Variable CSS | Código HEX | Uso |
| :--- | :--- | :--- | :--- |
| **Fondo Principal** | `--color-bg` | `#f8fafc` | Fondo global de la aplicación (Slate Light) |
| **Superficie / Tarjetas** | `--color-surface` | `#ffffff` | Contenedores y tarjetas principales |
| **Navegación Header** | `--color-nav-bg` | `#0f172a` | Fondo de la barra de navegación superior |
| **Color Primario** | `--color-primary` | `#2563eb` | Botones principales, acentos e hipervínculos |
| **Texto Principal** | `--color-text-main` | `#0f172a` | Encabezados y cuerpo de texto |
| **Texto Secundario** | `--color-text-muted` | `#64748b` | Subtítulos, labels y metadatos |
| **Estado Activo** | `--color-success-bg` | `#dcfce7` | Pastillas de estado activo (`#14532d` texto) |
| **Estado Inactivo** | `--color-danger-bg` | `#fee2e2` | Pastillas de estado inactivo (`#7f1d1d` texto) |
| **Bordes** | `--color-border` | `#e2e8f0` | Líneas divisoras y bordes de tarjetas |

---

## 🔤 3. Tipografía y Jerarquía

- **Familia Tipográfica:** `'Plus Jakarta Sans', sans-serif` (Google Fonts).
- **Jerarquía:**
  - `h1.display-5`: Títulos de banners de bienvenida o landing.
  - `h2.fw-bold`: Títulos principales de sección/vista (ej. *Gestión de Clientes*).
  - `h5.fw-bold`: Títulos dentro de tarjetas o modales.
  - `p.text-muted`: Subtítulos y descripciones explicativas.

---

## 🧩 4. Componentes Reutilizables

### A. Tarjetas (`.card-custom`)
Uso estándar para agrupar contenido, formularios o listados:
```html
<div class="card-custom p-4 mb-4">
    <h5 class="fw-bold text-dark">Título de la Tarjeta</h5>
    <p class="text-muted small">Descripción del contenido...</p>
</div>
```

### B. Pastillas de Estado (`.badge-status`)
Para indicar el estado activo/inactivo de clientes o empleados:
```html
<!-- Estado Activo -->
<span class="badge-status badge-status-active">
    <i class="bi bi-check-circle-fill"></i> Activo
</span>

<!-- Estado Inactivo -->
<span class="badge-status badge-status-inactive">
    <i class="bi bi-x-circle-fill"></i> Inactivo
</span>
```

### C. Botones Estándar
- **Acción Principal:** `<button class="btn btn-primary"><i class="bi bi-plus"></i> Guardar</button>`
- **Acción Secundario / Cancelar:** `<a class="btn btn-outline-custom">Cancelar</a>`

### D. Tablas Administrativas (`.table-app`)
Estilizado uniforme con encabezados en fondo claro y filas con hover sutil:
```html
<div class="card-custom overflow-hidden">
    <table class="table table-app mb-0">
        <thead>
            <tr>
                <th>Nombre</th>
                <th>Estado</th>
                <th class="text-end">Acciones</th>
            </tr>
        </thead>
        <tbody>
            ...
        </tbody>
    </table>
</div>
```

---

## 🔣 5. Librería de Iconos (Bootstrap Icons)

Se utilizan exclusivamente iconos de **Bootstrap Icons (`bi bi-*`)**:

- **Navegación e Inicio:** `bi-house-door`, `bi-speedometer2`, `bi-heart-pulse-fill`
- **Gestión de Clientes:** `bi-people`, `bi-person-plus`, `bi-person-lines-fill`
- **Gestión de Empleados:** `bi-person-badge`, `bi-person-workspace`
- **Acciones Tabla:** `bi-eye` (Ver), `bi-pencil` (Editar), `bi-trash` (Eliminar), `bi-search` (Buscar)

---

## 📱 6. Responsividad y Adaptabilidad

- Los formularios y filtros utilizan el sistema de grillas de Bootstrap 5 (`row g-3`, `col-md-*`, `col-lg-*`).
- Las tablas están contenidas dentro de `<div class="table-responsive">` para permitir desplazamiento horizontal suave en dispositivos móviles.
