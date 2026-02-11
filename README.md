# TicketHub API

**TicketHub** es una API RESTful escalable y de alto rendimiento construida con **.NET 9**, diseñada para simular la gestión y venta de entradas para eventos masivos. Este proyecto se centra en resolver desafíos de backend del mundo real, como la concurrencia en la compra de entradas, validaciones de stock complejas, paginación eficiente y caching.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12-239120?style=flat&logo=c-sharp)
![EF Core](https://img.shields.io/badge/EF%20Core-9.0-512BD4?style=flat&logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat&logo=microsoft-sql-server)
![Testing](https://img.shields.io/badge/Testing-xUnit%20%7C%20Moq-1081C2?style=flat)
![License](https://img.shields.io/badge/License-MIT-green)

---

## Características Principales

Este sistema va más allá de un CRUD básico, implementando patrones y prácticas de nivel profesional:

* **Gestión de Eventos y Categorías:** Creación y administración completa con relaciones relacionales.
* **Venta de Tickets con Concurrencia:** Lógica robusta para asegurar que no se sobre-venda la capacidad de un evento (Stock Control).
* **Seguridad:** Autenticación y Autorización basada en Roles (Admin/User) utilizando **ASP.NET Core Identity** y **JWT**.
* **Performance:**
    * **Paginación:** Implementada en endpoints de listado para optimizar la transferencia de datos.
    * **Output Caching:** Cacheo de respuestas en endpoints públicos para reducir la carga en la base de datos.
    * **Consultas Optimizadas:** Uso de `AsNoTracking` y proyecciones.
* **Calidad de Código (Testing):**
    * **Unit Tests:** Cobertura de la capa de Servicios y Controladores (aislando dependencias con Moq).
    * **Integration Tests:** Pruebas de flujo completo utilizando `WebApplicationFactory` y bases de datos en memoria (`InMemory Database`) para simular el entorno real sin afectar los datos de producción.
* **Clean Architecture:** Separación de responsabilidades (Controller -> Service -> Repository -> Database).
* **Validaciones:** Uso de **FluentValidation** para reglas de negocio y validación de modelos.
* **Documentación:** API documentada con **Swagger/OpenAPI**.

---

## Tecnologías y Librerías

* **Core:** ASP.NET Core Web API (.NET 9)
* **ORM:** Entity Framework Core 9
* **Base de Datos:** SQL Server
* **Mapeo:** AutoMapper
* **Validación:** FluentValidation
* **Testing:** xUnit, Moq, FluentAssertions, Microsoft.AspNetCore.Mvc.Testing

---

## Instalación y Ejecución

1.  **Clonar el repositorio:**
    ```bash
    git clone https://github.com/LucasHerdegen/TicketHub.git
    cd TicketHub.API
    ```

2.  **Configurar la Base de Datos:**
    Asegurate de tener SQL Server corriendo y actualiza la cadena de conexión en `appsettings.json` si es necesario. Luego, aplicá las migraciones:
    ```bash
    dotnet ef database update
    ```

3.  **Ejecutar la aplicación:**
    ```bash
    dotnet run
    ```
    La API estará disponible en `https://localhost:7001` (o el puerto configurado).

4.  **Ejecutar los Tests:**
    Para correr tanto los test unitarios como los de integración:
    ```bash
    dotnet test
    ```

---

## Diagrama de Entidad-Relación (DER)

A continuación se detalla el modelo de datos simplificado del sistema:

* Una **Categoría** puede tener múltiples Eventos asociados (Opcional).
* Un **Evento** pertenece obligatoriamente a una Categoría.
* Un **Evento** puede tener múltiples Tickets vendidos (Opcional).
* Un **Ticket** pertenece obligatoriamente a un Evento específico.

```mermaid
erDiagram
    CATEGORY ||--o{ EVENT : "contiene"
    EVENT ||--o{ TICKET : "genera"
    
    CATEGORY {
        int Id PK
        string Name
    }
    
    EVENT {
        int Id PK
        string Name
        string Description
        datetime Date
        decimal Price
        int Capacity
        int CategoryId FK
    }
    
    TICKET {
        int Id PK
        datetime PurchaseDate
        int EventId FK
        string UserId FK
    }
