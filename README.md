# Introduction 
Template de Clean arquitecture basada en la arquitectura echa por [Ardalis](https://github.com/ardalis/CleanArchitecture) y recomendada por Microsoft. 
https://github.com/ardalis/CleanArchitecture

# Caracteristicas principales
1.	Se utilizan Guard Classes para validaciones generales.
2.	Se utiliza AutoFac para el IoC.
3.	Se usan Specifications para la comunicación con la base de datos.
4.	Emisión de eventos mediante cola y ejecucíón con Mediator.

# Como funciona
TODO: Agregar documentación de Template. 

#Contribución
Para ayudar a hacer este template mejor, por favor comunicarse con el autor del mismo y discutir ideas.

# Enlaces de interés
- [Ardalis Clean arquitecture](https://github.com/ardalis/CleanArchitecture)
- [Ardalis Specifications](https://github.com/ardalis/Specification)
- [Ardalis Guard Classes](https://github.com/ardalis/GuardClauses)
- [Autofac](https://autofac.org/)
- [Outbox Pattern](https://itnext.io/the-outbox-pattern-in-event-driven-asp-net-core-microservice-architectures-10b8d9923885)

# NuBaultBank Core - Análisis de Seguridad OWASP

## Tabla de Cumplimiento de Estándares OWASP

| ID | Estándar OWASP | Descripción | ¿Cumple? | Razón/Observación |
|----|----------------|-------------|-----------|-------------------|
| 1 | Autenticación Segura | Implementación de mecanismos seguros de autenticación con tokens JWT | ✅ | Se implementa autenticación JWT con Bearer token y encriptación BCrypt para contraseñas |
| 2 | Protección contra Inyección SQL | Prevención de inyecciones SQL usando ORM y parametrización | ✅ | Uso de Entity Framework Core con repositorios tipados |
| 3 | Gestión de Sesiones | Manejo seguro de sesiones de usuario | ✅ | Implementación de tokens JWT con tiempo de expiración |
| 4 | Encriptación de Datos Sensibles | Encriptación de información sensible como contraseñas | ✅ | Uso de BCrypt para hash de contraseñas |
| 5 | Control de Acceso | Implementación de autorización basada en roles | ❌ | No se observa implementación de roles o permisos granulares |
| 6 | Validación de Entrada | Validación de datos de entrada en endpoints | ❌ | Falta validación exhaustiva en DTOs y modelos |
| 7 | Protección XSS | Prevención de ataques Cross-Site Scripting | ❌ | No se observa sanitización de datos de entrada/salida |
| 8 | CORS Seguro | Configuración segura de CORS | ⚠️ | CORS configurado pero permite todos los orígenes |
| 9 | Rate Limiting | Limitación de tasa de peticiones | ❌ | No implementado |
| 10 | Logging Seguro | Registro de eventos de seguridad | ✅ | Implementación de ILogService para eventos críticos |
| 11 | HTTPS Forzado | Redirección forzada a HTTPS | ✅ | Implementado en Program.cs |
| 12 | Headers de Seguridad | Headers HTTP de seguridad | ❌ | No se configuran headers de seguridad adicionales |
| 13 | Gestión de Errores | Manejo seguro de errores | ✅ | Uso de Result pattern para manejo de errores |
| 14 | Auditoría de Transacciones | Registro de transacciones financieras | ✅ | Logging de operaciones financieras implementado |
| 15 | Timeout de Sesión | Expiración de sesiones inactivas | ❌ | No implementado timeout de sesiones |
| 16 | Política de Contraseñas | Requisitos mínimos de contraseñas | ❌ | No hay validación de fortaleza de contraseñas |
| 17 | 2FA | Autenticación de dos factores | ❌ | No implementado |
| 18 | Cifrado en Tránsito | Datos cifrados en transmisión | ✅ | HTTPS implementado |
| 19 | Cifrado en Reposo | Datos cifrados en almacenamiento | ❌ | No se observa cifrado de datos en BD |
| 20 | Gestión de Dependencias | Control de dependencias seguras | ✅ | Uso de Directory.Packages.props |
| 21 | Protección CSRF | Tokens anti-CSRF | ❌ | No implementado |
| 22 | Seguridad en Transferencias | Validación de transferencias bancarias | ✅ | Validaciones implementadas en TransferEndpoints |
| 23 | Monitoreo de Actividad | Registro de actividades sospechosas | ⚠️ | Logging básico sin detección de anomalías |
| 24 | Bloqueo de Cuentas | Bloqueo tras intentos fallidos | ❌ | No implementado |
| 25 | Notificaciones de Seguridad | Alertas de actividad sospechosa | ❌ | No implementado sistema de notificaciones |
| 26 | Validación de Beneficiarios | Verificación de beneficiarios | ✅ | Implementado en BeneficiaryEndpoints |
| 27 | Segregación de Datos | Aislamiento de datos entre usuarios | ✅ | Implementado a nivel de modelo de datos |
| 28 | Gestión de Tokens | Manejo seguro de tokens JWT | ✅ | Implementación correcta en AuthService |
| 29 | Sanitización de Datos | Limpieza de datos de entrada/salida | ❌ | No implementada sanitización |
| 30 | Protección DoS | Defensa contra ataques DoS | ❌ | No implementado |
| 31 | Seguridad en API | Documentación y versionado de API | ✅ | Swagger implementado con seguridad |
| 32 | Auditoría de Código | Revisión de código seguro | ⚠️ | No se observan herramientas de análisis estático |
| 33 | Gestión de Secretos | Manejo seguro de secretos | ❌ | No se observa gestor de secretos |
| 34 | Logging de Accesos | Registro de intentos de acceso | ✅ | Implementado en AuthEndpoints |
| 35 | Validación de Transacciones | Verificación de transacciones | ✅ | Implementado en TransferEndpoints |
| 36 | Protección de Endpoints | Autorización en endpoints | ✅ | Uso de [Authorize] attribute |
| 37 | Seguridad en Préstamos | Validación de solicitudes de préstamos | ✅ | Implementado en LoanEndpoints |
| 38 | Integridad de Datos | Verificación de integridad | ⚠️ | Básica sin checksums |
| 39 | Gestión de Sesión Concurrente | Control de sesiones simultáneas | ❌ | No implementado |
| 40 | Validación de Documentos | Verificación de documentos de identidad | ❌ | No implementada |
| 41 | Protección de Metadata | Ocultamiento de información sensible | ⚠️ | Headers exponen información |
| 42 | Seguridad en Depósitos | Validación de depósitos | ✅ | Implementado en AccountEndpoints |
| 43 | Control de Versiones | Gestión segura de versiones | ✅ | Implementado en API |
| 44 | Protección de Rutas | Seguridad en rutas de API | ✅ | Rutas protegidas con autorización |
| 45 | Validación de Estados | Control de estados de transacciones | ✅ | Implementado en modelos |
| 46 | Seguridad en Logs | Protección de logs sensibles | ⚠️ | Sin encriptación de logs |
| 47 | Protección de Configuración | Seguridad en archivos de configuración | ❌ | Configuración no securizada |
| 48 | Validación de Roles | Control de acceso basado en roles | ❌ | No implementado |
| 49 | Seguridad en Caché | Protección de datos en caché | ❌ | No implementado |
| 50 | Protección de Sesión | Seguridad en manejo de sesiones | ⚠️ | Básica sin renovación de tokens |

## Leyenda
- ✅ Cumple completamente
- ⚠️ Cumple parcialmente
- ❌ No cumple

## Recomendaciones de Mejora

1. Implementar autenticación de dos factores (2FA)
2. Agregar sistema de roles y permisos
3. Implementar rate limiting
4. Mejorar la configuración de CORS
5. Agregar validación de fortaleza de contraseñas
6. Implementar protección contra CSRF
7. Agregar sistema de notificaciones de seguridad
8. Implementar bloqueo de cuentas tras intentos fallidos
9. Agregar sanitización de datos
10. Implementar gestión segura de secretos
