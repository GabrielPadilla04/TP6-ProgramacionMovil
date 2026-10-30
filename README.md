# Catálogo Móvil & Carrito de Compras - Trabajo Práctico N°6 (ITES)

Sistema móvil integral desarrollado con ASP.NET Core Web API en el backend y **.NET MAUI Blazor Hybrid** en el frontend (orientado y probado en dispositivos Android). El sistema incluye un catálogo de productos, carrito de compras con simulación de pagos y administración de stock.

## Integrantes del Equipo
- Gabriel Padilla
- Valentín Montes

## Prerrequisitos
Antes de comenzar, asegúrate de tener instalado en tu computadora:
- **.NET 10 SDK**.
- Visual Studio 2026 con la carga de trabajo de **Desarrollo de .NET MAUI** instalada (o entorno equivalente con emulador Android / dispositivo físico).
- MariaDB corriendo localmente o en un servidor accesible.
- Un gestor de bases de datos (DBeaver, MySQL Workbench, etc.).

##  Paso a Paso para Ejecutar el Sistema

### Paso 1: Clonar el repositorio
```bash
git clone [https://github.com/GabrielPadilla04/TP6-ProgramacionMovil](https://github.com/GabrielPadilla04/TP6-ProgramacionMovil)
cd TP6-ProgramacionMovil
```
### Paso 2: Configurar la Base de Datos MariaDB
Creá una base de datos vacía (con el nombre tp6_db) en tu gestor y entrá a la carpeta del backend (API desarrollada en el TP5). Buscá el archivo de configuración `secrets.json`.
Asegurate de que la cadena de conexión apunte correctamente a tu servidor local de MariaDB con tu usuario y contraseña.


### Paso 3: Aplicar las Migraciones (Entity Framework Core)
Para crear automáticamente todas las tablas normalizadas (hasta la 3FN) en tu base de datos, ubicate en la terminal dentro de la carpeta del proyecto backend.
```bash
cd TP6_SERVICIOS_API_REST
dotnet ef database update
```

### Paso 4: Ejecutar el Backend (ASP.NET Web API)
Desde la misma carpeta del backend, iniciá el servidor de la API:
```bash
dotnet run
```
La terminal indicará en qué puertos está corriendo para que el frontend pueda consumirla.

### Paso 5: Ejecutar el Frontend Móvil (Blazor Hybrid)
Abrí otra pestaña o ventana de la terminal, navegá hasta la carpeta del proyecto MAUI Blazor e inícialo seleccionando el emulador o dispositivo Android:
```bash
cd TP6_FrontEnd
dotnet build -t:Run -f net10.0-android
```

---

## Stack Tecnológico
- **Backend:** ASP.NET Core Web API (.NET 10), Entity Framework Core.
- **Base de Datos:** MariaDB (Normalizada al menos hasta la 3FN).
- **Frontend:** .NET MAUI Blazor Hybrid (orientado a Android) con componentes UI interactivos.
- **Generación y Envío de Reportes:** QuestPDF (para reportes de stock, entradas, salidas) y SMTP para envío por correo.
- **Logging y Auditoría:** Middleware global personalizado con persistencia en Base de Datos para registro (log) de errores.
- **Seguridad:** Autenticación y Autorización con Tokens JWT y control de acceso/gestión de roles.

## Características Principales
1. **Catálogo de Productos:** Interfaz estilo Mercado Libre para visualizar productos, prestando especial atención a la disponibilidad y stock actualizado.
2. **Carrito de Compras y Pago:** Flujo completo que permite registrar las ventas de un negocio, con vía para facilitar la compra y pago (simulado).
3. **Control de Inventario (Entradas/Salidas):** Registro de entradas de mercadería que facilita el control exhaustivo sobre los stocks.
4. **Reportes PDF y Correo:** Mecanismo de generación de reportes en formato PDF para su consulta (stock, entradas, salidas, etc.)### Paso 2: Configurar la Base de Datos MariaDB
Creá una base de datos vacía en tu gestor y entrá a la carpeta del backend (API desarrollada en el TP5). Buscá el archivo de configuración `appsettings.json`.
Asegurate de que la cadena de conexión apunte correctamente a tu servidor local de MariaDB con tu usuario y contraseña.


Diagrama ER
<img width="6538" height="5165" alt="Inventory Sales and Purchasing Data Model-2026-10-30-191449" src="https://github.com/user-attachments/assets/bbeec18a-bce0-4ce8-924f-3d3a05eb833f" />
