# 🏡 GV Propiedades — Sitio Web Inmobiliario

> Sitio web institucional y sistema de gestión para **Gattini & Viola Propiedades**, inmobiliaria familiar de Saladillo, Buenos Aires, Argentina. Especializada en propiedades urbanas y rurales.

🌐 **Sitio en producción:** [gattiniviola.com](https://www.gattiniviola.com)

---

## 📋 Descripción

Aplicación web completa desarrollada en **ASP.NET Core 8 con Razor Pages**, que incluye:

- Catálogo público de propiedades urbanas y rurales con filtros avanzados
- Panel de administración protegido por autenticación
- Gestión de imágenes con conversión automática a **WebP**
- Slider de inicio configurable con orden personalizable
- Formulario de contacto integrado con **Web3Forms**
- Integración con videos de **YouTube**
- Optimizado para SEO con **Google Tag Manager**

---

## 🛠️ Stack Tecnológico

| Capa | Tecnología |
|------|-----------|
| Framework | ASP.NET Core 8 — Razor Pages |
| Base de datos | SQL Server + Entity Framework Core |
| Autenticación | Cookie Authentication + BCrypt |
| Procesamiento de imágenes | ImageSharp (conversión a WebP) |
| Frontend | Bootstrap 5, Owl Carousel, AOS, Fancybox |
| Contenedor | Docker |
| Formularios | Web3Forms API |

---

## ✨ Funcionalidades

### Sitio Público
- **Página de inicio** con slider de imágenes de fondo configurable y propiedades destacadas
- **División Campos** — Listado, búsqueda y filtros de propiedades rurales (aptitud, superficie, ubicación, precio)
- **División Urbano** — Listado, búsqueda y filtros de propiedades urbanas (tipo, dormitorios, ambientes, baños, precio)
- **Ficha de propiedad** — Galería de imágenes (con Fancybox), video de YouTube, descripción, mapa de Google Maps, formulario de contacto y enlace directo a WhatsApp
- **Página de contacto** con formulario y datos de oficina
- **Páginas informativas:** Sobre nosotros, Servicios
- **Botón flotante de WhatsApp**

### Panel de Administración (`/Admin`)
- Login con email y contraseña (BCrypt)
- **Dashboard** con acceso rápido a gestión
- **Gestión de Propiedades Urbanas** — Crear, editar y eliminar, con carga de imágenes múltiple
- **Gestión de Campos** — Crear, editar y eliminar propiedades rurales
- **Gestión del Slider** — Subir nuevas imágenes y reordenar con drag & drop
- **Configuración de cuenta** — Cambiar nombre, email y contraseña
- Conversión automática de todas las imágenes subidas a formato **WebP**

---

## 📁 Estructura del Proyecto

```
GV/
├── Models/
│   ├── Propiedad.cs          # Clase base abstracta
│   ├── PropiedadUrbana.cs    # Hereda de Propiedad
│   ├── PropiedadCampo.cs     # Hereda de Propiedad
│   ├── Imagen.cs
│   ├── Video.cs
│   └── Usuario.cs
├── Pages/
│   ├── Index.cshtml           # Página de inicio
│   ├── PropertiesUrbano.cshtml
│   ├── PropertiesCampos.cshtml
│   ├── Property.cshtml        # Detalle de propiedad
│   ├── Contact.cshtml
│   ├── About.cshtml
│   ├── Services.cshtml
│   └── Admin/
│       ├── Login.cshtml
│       ├── Dashboard.cshtml
│       ├── GestionPropiedades.cshtml
│       ├── GestionCampos.cshtml
│       ├── GestionInicio.cshtml   # Gestión del slider
│       ├── CrearPropiedad.cshtml
│       ├── EditarPropiedad.cshtml
│       ├── CrearCampo.cshtml
│       ├── EditarCampo.cshtml
│       └── Configuracion.cshtml
├── Services/
│   └── ImageConversionService.cs  # Conversión WebP con ImageSharp
├── Data/
│   └── AppDbContext.cs
├── slider-order.json          # Orden del slider (persistido en disco)
├── appsettings.json
└── Dockerfile
```

---

## ⚙️ Configuración

### 1. Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local o en hosting)

### 2. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/gv-propiedades.git
cd gv-propiedades
```

### 3. Configurar la cadena de conexión

Editá `appsettings.json` con tus datos de SQL Server:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=InmobiliariaDB;Trusted_Connection=True;"
}
```

### 4. Aplicar migraciones

```bash
dotnet ef database update
```

### 5. Correr el proyecto

```bash
dotnet run
```

El sitio estará disponible en `https://localhost:5001`

---

## 🐳 Docker

El proyecto incluye un `Dockerfile` multistage listo para producción:

```bash
# Construir imagen
docker build -t gv-propiedades .

# Correr contenedor
docker run -p 8080:8080 gv-propiedades
```

---

## 🔐 Primer Usuario Administrador

Para crear el primer usuario administrador, podés insertar directamente en la base de datos con un hash de BCrypt, o agregar una ruta de inicialización temporal. El sistema de login utiliza `BCrypt.Net` para verificar contraseñas.

---

## 🗂️ Modelo de Datos

```
Propiedad (abstracta)
├── Id, Titulo, Descripcion, Division
├── Ubicacion, Precio, FechaPublicacion, EsDestacada
├── Imagenes (1:N)
└── Videos (1:N)

PropiedadUrbana : Propiedad
└── Tipo, Ambientes, Dormitorios, Baños,
    SuperficieTotal, SuperficieCubierta,
    Cochera, Patio, Aire_acond, Seguridad, Pileta, Direccion

PropiedadCampo : Propiedad
└── Aptitud, Hectareas
```

---

## 📸 Gestión de Imágenes

Todas las imágenes subidas se convierten automáticamente a **WebP** (calidad 80) usando **ImageSharp**, reduciendo el tamaño de los archivos y mejorando la performance del sitio. Las imágenes se almacenan en `wwwroot/uploads/imagenes/`.

Las imágenes del slider de inicio se guardan en `wwwroot/images/fondos/slider/` y su orden se persiste en `slider-order.json`.

---

## 📬 Formulario de Contacto

El formulario usa [Web3Forms](https://web3forms.com/) para enviar emails sin backend propio. El `access_key` está configurado directamente en las vistas. Para usar tu propia cuenta, reemplazá el valor en `Contact.cshtml` y `Property.cshtml`.

---

## 🔧 Variables de entorno recomendadas para producción

| Variable | Descripción |
|----------|-------------|
| `ConnectionStrings__DefaultConnection` | Cadena de conexión a SQL Server |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ASPNETCORE_URLS` | URL donde escucha la app |

---

## 📄 Licencia

Proyecto privado — © 2025 Gattini & Viola Propiedades. Todos los derechos reservados.

---

*Desarrollado para Gattini & Viola Propiedades — Saladillo, Buenos Aires, Argentina*
