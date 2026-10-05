# Despliegue: front en Vercel + API y base de datos de la oficina

## Cómo queda armado

```
Navegador ──HTTPS──▶ Vercel (front Angular, sitio estático)
    │
    └──HTTPS──▶ https://api.tudominio.com ──(túnel de Cloudflare)──▶ máquina de la oficina
                                                                       │  Docker: API .NET (puerto 8080, solo localhost)
                                                                       └──▶ SQL Server 192.168.1.17:1433 (red interna)
```

**Por qué así:** Vercel solo sirve el front. No ejecuta .NET y, además, `192.168.1.17` es una IP privada que no
existe desde internet. La API tiene que correr **dentro de la red de la oficina**, donde sí ve la base de datos, y
publicarse por HTTPS mediante un túnel. **El puerto 1433 de SQL Server no se abre a internet en ningún caso.**

---

## Mientras se compra el dominio: la API desde la PC de la oficina con Tailscale Funnel

Sin dominio propio se puede desplegar igual: la PC (tiene Tailscale) publica la API con una URL HTTPS **estable**
(no cambia al reiniciar), y el front va en Vercel con su dirección gratuita `*.vercel.app`.

1. **API en Docker** (ya probado: modo producción, llega a la base real, Swagger apagado, solo escucha en localhost):
   en `.env` poner `API_PUERTO=8090` (el 8080 de esa PC está ocupado por otros programas) y
   ```bash
   docker compose up -d --build
   ```
2. **Publicarla con Funnel.** El Funnel de esta PC ya usa el puerto 443 para otro servicio (`127.0.0.1:8077`), así que
   se usa el **8443** (Funnel solo admite 443, 8443 y 10000):
   ```bash
   tailscale funnel --bg --https=8443 http://127.0.0.1:8090
   tailscale funnel status
   ```
   La API queda en `https://desktop-rqr1dib.tail4a0d10.ts.net:8443`.
   ⚠️ No usar `tailscale funnel reset` para quitarlo: borra **toda** la configuración, incluido el servicio del 443.
3. **Front en Vercel con la CLI.** El repositorio del front pertenece a otra cuenta de GitHub, y la CLI evita depender
   de la integración con Git. Desde la carpeta del front:
   ```bash
   npx vercel login
   npx vercel link
   npx vercel env add API_URL production     # valor: https://desktop-rqr1dib.tail4a0d10.ts.net:8443
   npx vercel deploy --prod
   ```
4. **Cerrar el círculo.** Con la URL `*.vercel.app` que entregue Vercel, editar `.env` de la API
   (`Cors__Origins__0` y `Recuperacion__FrontendResetUrl`) y aplicar con `docker compose up -d`.

Limitaciones de esta etapa:
- La PC debe estar **encendida, en la red de la oficina**, con Docker y Tailscale funcionando.
- En modo producción los correos (invitaciones y recuperación de contraseña) **solo salen si se configura `Email__*`**;
  sin SMTP no hay forma de entregar los enlaces. Mientras tanto, las cuentas se crean con contraseña inicial.
- Funnel no garantiza ancho de banda; sirve para esta etapa, no para producción definitiva.
- Antes de publicar, cambiar la contraseña de `admin.general`: la temporal circuló por chat.

Cuando llegue el dominio se pasa al túnel con nombre (sección 2) y se actualiza `API_URL` en Vercel.

## 0. Antes de empezar (una sola vez)

1. **Cambiar la contraseña de la base de datos.** La anterior estuvo en el historial de git y hay que darla por
   comprometida. Crear de paso un usuario SQL propio de la aplicación con permisos mínimos:

   ```sql
   CREATE LOGIN buscador_app WITH PASSWORD = 'UNA-CLAVE-LARGA-Y-NUEVA';
   USE Buscador;
   CREATE USER buscador_app FOR LOGIN buscador_app;
   ALTER ROLE db_datareader ADD MEMBER buscador_app;
   ALTER ROLE db_datawriter ADD MEMBER buscador_app;
   ```

2. **Ejecutar los scripts en la base REAL**, en SSMS y en este orden (todos se pueden repetir sin problema):
   `database/000_instalar_todo.sql` → crear/asignar el primer ADMIN GENERAL (ver `database/002…`, paso 5) →
   tokens iniciales (paso 6) → `database/004_menu_operador.sql`.
3. **Cambiar la contraseña temporal de `admin.general`** y pedir al proveedor un token de RENIEC nuevo
   (el actual circuló por chat).

## 1. La API en una máquina de la oficina

Requisitos: Docker instalado y que la máquina alcance `192.168.1.17:1433`.

```bash
git clone https://github.com/irubio06dev-svg/internal-search-backend.git
cd internal-search-backend
cp .env.example .env        # completar TODOS los valores (ver la tabla de abajo)
docker compose up -d --build
curl http://localhost:8080/system/usuarios      # debe responder 401 (la API está viva y pide sesión)
```

Si en lugar de `401` falla, mirar `docker compose logs api`:
- *"No se configuró ConnectionStrings…"* / *"Jwt:Key…"* → falta algo en `.env`.
- Errores de SQL → la máquina no alcanza la base o el usuario/clave son incorrectos.

## 2. Publicar la API por HTTPS (túnel de Cloudflare)

Se necesita una cuenta de Cloudflare con el dominio que se quiera usar (gratis).

1. Cloudflare → **Zero Trust → Networks → Tunnels → Create a tunnel** (tipo *Cloudflared*). Copiar el **token**.
2. En *Public Hostname*: `api.tudominio.com` → Service **HTTP** `api:8080`.
3. Pegar el token en `.env` como `TUNNEL_TOKEN` y levantar el túnel:
   ```bash
   docker compose --profile tunel up -d
   ```
4. Probar desde cualquier lado: `https://api.tudominio.com/system/usuarios` → `401`.

Alternativas equivalentes: Tailscale Funnel, o un servidor con IP pública y Caddy/nginx con certificado.
Lo importante es que la API quede en **HTTPS** (el sitio de Vercel es HTTPS y el navegador bloquea llamadas HTTP).

## 3. El front en Vercel

1. Vercel → **Add New → Project** → importar el repositorio `internal-search-frontend`.
2. La configuración ya está en `vercel.json` (build, carpeta de salida y rutas). No cambiar nada más.
3. **Settings → Environment Variables**: crear `API_URL` = `https://api.tudominio.com` (Production y Preview).
   El build falla a propósito si la URL no empieza con `https://`.
4. **Settings → General → Node.js Version**: 22.x o superior.
5. **Deploy.** Anotar la URL que entrega (ej. `https://tu-proyecto.vercel.app`).

## 4. Cerrar el círculo (CORS y correos)

Con la URL real de Vercel, editar `.env` de la API y reiniciar:

```
Cors__Origins__0=https://tu-proyecto.vercel.app
Recuperacion__FrontendResetUrl=https://tu-proyecto.vercel.app/auth/reset-password
```
```bash
docker compose up -d
```

Cada dominio desde el que se use el front (dominio propio, etc.) va en una línea `Cors__Origins__N` adicional.
Las URLs de *preview* de Vercel no están permitidas salvo que se agreguen.

## 5. Verificación final

- [ ] `https://tu-proyecto.vercel.app` abre el login (con el logo).
- [ ] Entrar con `admin.general`: arriba dice **Tokens ilimitados** y aparece **Administración** en el menú.
- [ ] Recargar con F5 estando en *Usuarios y tokens* **no** manda a la portada.
- [ ] Crear un usuario de prueba: llega el correo de invitación (requiere `Email__*`).
- [ ] Con ese usuario, una consulta por DNI descuenta 1 token.
- [ ] *Auditoría* muestra el login y la consulta.
- [ ] Desactivar el usuario de prueba al terminar.

## Variables de entorno de la API

| Variable | Obligatoria | Para qué |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | Sí | Conexión a la base real |
| `Jwt__Key` | Sí | Clave de firma (32+ caracteres aleatorios) |
| `Jwt__Issuer`, `Jwt__Audience` | Sí | Valores propios; no dejar los genéricos |
| `Cors__Origins__0` | Sí | URL exacta del front en Vercel |
| `Recuperacion__FrontendResetUrl` | Sí | Enlace de los correos de recuperación |
| `Email__Host/Port/EnableSsl/User/Password/From` | Para correos | SMTP; sin esto no salen invitaciones ni recuperaciones |
| `Reniec__Token` | Para RENIEC | Token del proveedor |
| `Proxy__ConfiarEnCabeceras` | Sí detrás del túnel | Ya lo fija `docker-compose.yml`. Sin esto todos los usuarios compartirían una IP y el límite de intentos los bloquearía a la vez |
| `TUNNEL_TOKEN` | Con túnel | Token de Cloudflare |

## Cosas a tener presentes

- **Datos personales en internet.** La API devuelve datos de DNI y RENIEC. Además del login y los tokens, conviene
  limitar quién llega: *Cloudflare Access* (login corporativo delante de la API o del front) o lista de IPs permitidas.
- **Plan de Vercel.** El plan *Hobby* es solo para uso personal y no comercial; una herramienta de la empresa
  corresponde al plan *Pro* (o publicar el front en otro hosting estático).
- **Secretos.** `.env` no se versiona (está en `.gitignore`). Nunca poner claves en `appsettings.json` ni en el repositorio.
- **Actualizar la API.** `git pull && docker compose up -d --build` en la máquina de la oficina.
- **Si la máquina de la oficina se apaga**, el sistema deja de funcionar (el front sigue en Vercel pero sin API).
  Para algo más robusto, la API puede ir a un servidor siempre encendido con acceso a la base por VPN.
