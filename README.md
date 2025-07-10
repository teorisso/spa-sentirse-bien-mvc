# SPA Sentirse Bien – Panel Administrativo MVC

**Trabajo Práctico Integrador 2025 - Componente ASP.NET MVC**

## 🎯 Descripción del Escenario

**Spa Sentirse Bien - Panel Administrativo** es una aplicación web MVC desarrollada con ASP.NET Core que proporciona a administradores y profesionales las herramientas necesarias para gestionar eficientemente el spa:

- **Administradores**: Control total del sistema, gestión de usuarios, servicios, turnos, pagos y estadísticas
- **Profesionales**: Acceso a agenda personal, consulta de turnos, registro de tratamientos realizados
- **Dashboard**: Estadísticas en tiempo real del rendimiento del spa
- **Gestión CRUD**: Operaciones completas sobre todas las entidades del sistema

Esta aplicación MVC consume la API ASP.NET Core y está diseñada específicamente para la gestión interna del spa.

## 🛠️ Herramientas Utilizadas

### Framework y Lenguaje
- **ASP.NET Core 9.0 MVC** - Framework web principal
- **C#** - Lenguaje de programación
- **Razor Pages** - Motor de vistas
- **Entity Framework Core** - ORM para acceso a datos

### Frontend
- **Bootstrap 5** - Framework CSS para diseño responsive
- **jQuery** - Biblioteca JavaScript
- **Chart.js** - Gráficos y estadísticas
- **FontAwesome** - Iconografía

### Base de Datos
- **MongoDB Atlas** - Base de datos NoSQL en la nube
- **MongoDB.Driver** - Driver oficial de MongoDB para .NET

### Seguridad
- **Cookie Authentication** - Autenticación basada en cookies
- **Authorization Policies** - Políticas de autorización por roles
- **Anti-Forgery Tokens** - Protección CSRF

### Utilidades
- **IHttpClientFactory** - Cliente HTTP para consumir API
- **Serilog** - Logging estructurado
- **AutoMapper** - Mapeo de objetos

## 📋 Cumplimiento de Requisitos TP-2025

### ✅ Requisitos Técnicos Implementados

1. **✅ Aplicación Web ASP.NET MVC**
   - Arquitectura MVC completa con controladores, vistas y modelos
   - Razor Pages para renderizado server-side
   - Patrón Repository para acceso a datos

2. **✅ Patrón MVC (Model-View-Controller)**
   - Separación clara de responsabilidades
   - Modelos para representación de datos
   - Vistas Razor para presentación
   - Controladores para lógica de negocio

3. **✅ Registro de nuevos usuarios**
   - Formulario de registro con validación
   - Campos: nombre, apellido, email, contraseña, rol
   - Validación server-side con DataAnnotations

4. **✅ Inicio de sesión con usuario y contraseña**
   - Sistema de login con cookies
   - Validación de credenciales
   - Sesiones persistentes

5. **✅ Contraseñas hasheadas con salt**
   - Integración con API que maneja BCrypt
   - Validación segura de contraseñas
   - Almacenamiento seguro en MongoDB

6. **✅ Recuperación de contraseña**
   - Formulario de recuperación
   - Envío de email con enlace único
   - Tokens temporales seguros

7. **✅ Listado paginado de elementos**
   - Servicios con paginación automática
   - Turnos con filtros avanzados
   - Usuarios con búsqueda y ordenamiento

8. **✅ Vista de detalle específica**
   - Detalle completo de servicios
   - Información expandida de turnos
   - Perfil detallado de usuarios

9. **✅ Crear elemento con dropdown/selector**
   - Formulario de servicios con selector de categoría
   - Creación de usuarios con dropdown de roles
   - Asignación de profesionales con selectores

10. **✅ Editar elemento existente**
    - Formularios de edición para todas las entidades
    - Validación de cambios
    - Actualización en tiempo real

11. **✅ Código QR generado desde backend**
    - Integración con API para generación de QR
    - Visualización de códigos QR en vistas
    - Descarga de imágenes QR

12. **✅ Funcionalidad exclusiva QR**
    - Panel de administración de QRs
    - Generación de códigos especiales
    - Historial de QRs utilizados

## 🚀 Instalación y Ejecución

### Requisitos Previos
```bash
# Verificar versión de .NET
dotnet --version  # Debe ser 9.0 o superior
```

### 1. Clonar el repositorio
```bash
git clone https://github.com/tu-usuario/spa-sentirse-bien-mvc.git
cd spa-sentirse-bien/spa-sentirse-bien-mvc
```

### 2. Restaurar dependencias
```bash
dotnet restore
```

### 3. Configurar variables de entorno
Crear archivo `.env` en la raíz del proyecto:

```env
# API Configuration
API_BASE_URL=http://localhost:5018/api
API_TIMEOUT=30

# MongoDB Configuration
ConnectionStrings__MongoDB=mongodb+srv://usuario:password@cluster.mongodb.net/
MongoDatabase=sentirseBien

# Authentication
Authentication__CookieName=SpaAdminAuth
Authentication__ExpireTimeSpan=02:00:00
Authentication__SlidingExpiration=true

# Logging
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft=Warning

# Application
Application__Name=Spa Sentirse Bien Admin
Application__Version=1.0.0
```

### 4. Ejecutar la aplicación
```bash
dotnet run
```

### 5. Verificar funcionamiento
- **Aplicación**: https://localhost:7001
- **Login**: https://localhost:7001/Account/Login
- **Dashboard**: https://localhost:7001/Home/Index
- **Servicios**: https://localhost:7001/Servicios

## 📱 Funcionalidades Principales

### Dashboard Administrativo
- **Estadísticas en tiempo real** del spa
- **Gráficos de rendimiento** por servicios
- **Resumen de turnos** del día
- **Métricas de pagos** y ingresos

### Gestión de Servicios
- **Listado paginado** con filtros
- **Crear servicio** con categorías
- **Editar servicio** existente
- **Eliminar servicio** con confirmación
- **Vista detalle** con información completa

### Gestión de Usuarios
- **CRUD completo** de usuarios
- **Asignación de roles** (cliente, profesional, admin)
- **Búsqueda y filtrado** avanzado
- **Historial de actividad** por usuario

### Gestión de Turnos
- **Calendario interactivo** de turnos
- **Asignación de profesionales** a servicios
- **Control de disponibilidad** en tiempo real
- **Reportes de turnos** por período

### Gestión de Pagos
- **Listado de transacciones** completo
- **Procesamiento de reembolsos**
- **Estadísticas de ingresos**
- **Reportes financieros** detallados

## 🎨 Estructura de Vistas

```
Views/
├── Shared/
│   ├── _Layout.cshtml           # Layout principal
│   ├── _ValidationScriptsPartial.cshtml
│   └── Error.cshtml             # Página de error
├── Home/
│   ├── Index.cshtml             # Dashboard principal
│   └── Privacy.cshtml           # Página de privacidad
├── Servicios/
│   ├── Index.cshtml             # Listado de servicios
│   ├── Create.cshtml            # Crear servicio
│   ├── Edit.cshtml              # Editar servicio
│   ├── Details.cshtml           # Detalle del servicio
│   └── Delete.cshtml            # Confirmar eliminación
├── Usuarios/
│   ├── Index.cshtml             # Listado de usuarios
│   ├── Create.cshtml            # Crear usuario
│   ├── Edit.cshtml              # Editar usuario
│   └── Details.cshtml           # Detalle del usuario
├── Turnos/
│   ├── Index.cshtml             # Gestión de turnos
│   └── Calendar.cshtml          # Vista de calendario
└── Account/
    ├── Login.cshtml             # Página de login
    └── AccessDenied.cshtml      # Acceso denegado
```

## 🔧 Estructura de Controladores

```
Controllers/
├── BaseController.cs            # Controlador base
├── HomeController.cs            # Dashboard y páginas principales
├── ServiciosController.cs       # CRUD de servicios
├── UsuariosController.cs        # Gestión de usuarios
├── TurnosController.cs          # Gestión de turnos
├── PagosController.cs           # Gestión de pagos
├── QRController.cs              # Generación y gestión de QR
└── AccountController.cs         # Autenticación y autorización
```

## 🔐 Seguridad Implementada

### Autenticación
- **Cookie Authentication** con expiración configurable
- **Login seguro** con validación de credenciales
- **Logout automático** por inactividad

### Autorización
- **Políticas por roles** (Admin, Profesional)
- **Filtros de autorización** en controladores
- **Protección de rutas** sensibles

### Validación
- **DataAnnotations** en modelos
- **Validación server-side** completa
- **Anti-forgery tokens** en formularios
- **Sanitización de entrada** de datos

## 📊 Características Responsive

### Diseño Adaptativo
- **Bootstrap 5** para responsive design
- **Menú colapsible** en dispositivos móviles
- **Tablas responsive** con scroll horizontal
- **Formularios optimizados** para touch

### Breakpoints Soportados
- **Mobile**: 576px y menor
- **Tablet**: 768px - 991px
- **Desktop**: 992px y mayor
- **Large Desktop**: 1200px y mayor

## 🎯 Integración con API

### Endpoints Consumidos
```csharp
// Autenticación
POST /api/auth/login
POST /api/auth/register

// Servicios
GET /api/services
POST /api/services
PUT /api/services/{id}
DELETE /api/services/{id}

// Usuarios
GET /api/users
POST /api/users
PUT /api/users/{id}
DELETE /api/users/{id}

// Turnos
GET /api/turnos
POST /api/turnos
PUT /api/turnos/{id}
DELETE /api/turnos/{id}

// Pagos
GET /api/payments
POST /api/payments/refund

// QR Codes
POST /api/qr/generate
GET /api/qr/history
```

## 🧪 Testing

```bash
# Ejecutar tests unitarios
dotnet test

# Tests con coverage
dotnet test --collect:"XPlat Code Coverage"

# Tests de integración
dotnet test --filter "Category=Integration"
```

## 📦 Scripts Disponibles

```bash
dotnet run           # Ejecutar en desarrollo
dotnet build         # Compilar proyecto
dotnet publish       # Publicar para producción
dotnet test          # Ejecutar tests
dotnet clean         # Limpiar artifacts
```

## 🎥 Demo y Capturas

Para ver el funcionamiento completo, revisar la carpeta `/videos` en el repositorio principal.

## 🤝 Contribución

Este proyecto es parte del TP Integrador 2025. Para contribuir:

1. Fork el repositorio
2. Crear feature branch (`git checkout -b feature/nueva-funcionalidad`)
3. Commit cambios (`git commit -am 'Agregar nueva funcionalidad'`)
4. Push branch (`git push origin feature/nueva-funcionalidad`)
5. Crear Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver archivo [LICENSE](LICENSE) para detalles.

---

**Desarrollado para el TP Integrador 2025**  
*Arquitectura: API ASP.NET Core + MVC + SPA Next.js* 