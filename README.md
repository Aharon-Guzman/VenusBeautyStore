# VenusBeautyStore — Sistema de Gestión para Salón de Belleza

Aplicación web full-stack para la gestión integral de un salón de belleza. Permite a clientes registrarse, consultar servicios y productos, y agendar citas en línea. El personal del salón gestiona la agenda, los servicios, los productos y los trabajadores desde un panel administrativo con acceso diferenciado por rol.

---

## Descripción

Plataforma multi-rol desarrollada con ASP.NET Core 8 y Entity Framework Core que cubre el flujo completo de un salón de belleza: desde el registro del cliente hasta la confirmación de su cita con múltiples servicios y productos incluidos. La autenticación y autorización están implementadas con ASP.NET Core Identity.

---

## Stack Tecnológico

- **Backend:** C# / ASP.NET Core 8, Razor Pages
- **ORM:** Entity Framework Core — Code-First con migraciones
- **Base de datos:** SQL Server
- **Autenticación:** ASP.NET Core Identity — roles, claims, gestión de sesión
- **Frontend:** HTML5, CSS3, JavaScript
- **Arquitectura:** Capas (PL / BLL / DAL)

---

## Roles del Sistema

| Rol | Acceso |
|---|---|
| **Cliente** | Registro, catálogo de servicios y productos, agendamiento de citas, historial personal |
| **Trabajador** | Gestión de agenda, visualización de citas del día, cambio de estado |
| **Administrador** | Acceso completo: servicios, productos, trabajadores, clientes y reportes |

---

## Funcionalidades Principales

### Para Clientes
- Registro e inicio de sesión con ASP.NET Core Identity
- Catálogo de servicios con imagen, descripción, duración y precio
- Catálogo de productos disponibles para reservar junto a la cita
- Agendamiento de citas con selección de fecha, hora, servicios y productos
- Historial de citas con estado y total

### Para Trabajadores / Administradores
- Vista de agenda con citas del día y sus detalles
- Gestión de servicios (nombre, descripción, duración, precio, imagen, activo/inactivo)
- Gestión de productos con stock y precio unitario
- Gestión de trabajadores con asignación de usuario Identity
- Gestión de clientes registrados

---

## Modelo de Datos

Una **Cita** puede contener múltiples servicios (`DetalleCitas`) y múltiples productos (`ReservaProductos`), con precio unitario registrado al momento de la reserva. Esto permite calcular el total de la cita y mantener historial de precios aunque los precios cambien después.

**Tablas principales:** AspNetUsers (Identity), Clientes, Trabajadores, Servicios, Producto, Citas, DetalleCitas, ReservaProductos.

---

## Configuración Local

```bash
# 1. Clonar el repositorio
git clone https://github.com/Aharon-Guzman/VenusBeautyStore.git

# 2. Actualizar cadena de conexión en appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR;Database=VenusBeautyDB;Trusted_Connection=True;"
}

# 3. Aplicar migraciones
dotnet ef database update

# 4. Ejecutar
dotnet run
```

---

## Estructura del Proyecto

```
VenusBeautyStore/
├── PL/          # Presentation Layer — Razor Pages, vistas
├── BLL/         # Business Logic Layer — servicios de negocio
├── DAL/         # Data Access Layer — DbContext, repositorios, migraciones EF Core
└── README.md
```

---

## Contexto Académico

Proyecto académico — Universidad Americana (UAM), Costa Rica · 2023

---

## Autor

**Aharón David Guzmán Guzmán**  
Ingeniería en Sistemas — Universidad Americana (UAM)  
[LinkedIn](https://www.linkedin.com/in/aharón-guzmán-81948136b) · [GitHub](https://github.com/Aharon-Guzman)
