# Registro Mundial de Football con comprobante

Aplicación web académica para registrar personas en un juego del Mundial de Football. La persona selecciona la fecha, el partido, el asiento, completa sus datos y genera un comprobante que puede imprimir o guardar como PDF desde el navegador.

## Campos solicitados

- TipoDocumento
- Documento
- Nombres
- Apellidos
- FechaNacimiento
- Sexo
- Fecha del juego
- Partido
- Asiento

## Tecnologías

- ASP.NET Core 8
- Entity Framework Core
- SQLite
- HTML, CSS y JavaScript

## Cómo ejecutar

```bash
cd MundialRegistro
dotnet restore
dotnet run --urls http://localhost:5000
```

Luego abra el navegador en:

```text
http://localhost:5000
```

## Base de datos

La base de datos SQLite se crea automáticamente en:

```text
Data/mundial_registro.db
```

También se incluye el script de creación en:

```text
Database/schema.sql
```

## Comprobante PDF

Después de registrar una persona, aparece el comprobante. Presione **Imprimir / Guardar PDF** y seleccione la opción **Guardar como PDF** del navegador.

## Evidencias sugeridas para entregar

1. Captura de la pantalla principal.
2. Captura seleccionando fecha, partido y asiento.
3. Captura del formulario lleno.
4. Captura del comprobante generado.
5. PDF guardado desde el botón de impresión.
