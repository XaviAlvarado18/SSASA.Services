# SSASA.Services - Backend SOAP Web Service ⚙️

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-blue)
![SOAP](https://img.shields.io/badge/Service-SOAP%20ASMX-orange)
![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-red)

Este repositorio contiene la **Capa de Servicios y Lógica de Negocio** del ecosistema SSASA. Actúa como el motor central que gestiona la persistencia de datos y las reglas transaccionales a través de un servicio web **ASMX (SOAP)**.

## 🏗️ Arquitectura del Servidor

El proyecto sigue un patrón de diseño desacoplado para garantizar la escalabilidad:

* **Service Layer (`EmployeeService.asmx`)**: Expone los métodos de contrato para que la UI consuma los datos de forma segura.
* **Data Access Layer (`Data/DatabaseLogic.cs`)**: Gestiona la comunicación directa con SQL Server utilizando ADO.NET y procedimientos almacenados para máxima eficiencia.
* **Models**: Define los objetos de transferencia de datos (DTO) como `Employee` y `Department` que viajan a través de la red.

## 🛠️ Métodos del Servicio (API Reference)

El servicio expone las siguientes capacidades principales:

* `SaveEmployee`: Registra o actualiza personal, incluyendo validaciones de seguridad para evitar duplicidad de DPI.
* `GetAllEmployees`: Retorna el listado completo procesando cálculos de antigüedad directamente desde la base de datos.
* `DeleteEmployee`: Realiza la eliminación de registros por identificador único.
* `GetAllDepartments`: Devuelve la colección completa de departamentos para el llenado de catálogos.
* `GetDashboardStats`: Provee métricas agregadas (KPIs) para el Dashboard de la aplicación web.

## 🗄️ Lógica de Base de Datos

El backend implementa reglas críticas mediante **Stored Procedures**:

1.  **Validación de Unicidad**: Control estricto de restricciones `UNIQUE KEY` para identificadores legales (DPI).
2.  **Integridad Referencial**: Manejo de llaves foráneas entre Empleados y Departamentos.
3.  **Desactivación en Cascada**: Implementación de lógica que inactiva automáticamente a los empleados cuando su departamento de origen es deshabilitado.

## 🚀
