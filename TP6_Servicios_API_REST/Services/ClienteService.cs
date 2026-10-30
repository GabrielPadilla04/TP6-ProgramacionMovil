using TP6_Servicios_API_REST.Data;
using TP6_Servicios_API_REST.Excepciones;
using TP6_Servicios_API_REST.Models;
using Microsoft.EntityFrameworkCore;
using TP6_Shared.DTOs.Cliente.Request;
using TP6_Shared.DTOs.Cliente.Response;

namespace TP6_Servicios_API_REST.Services
{
    public class ClienteService
    {
        private readonly AppDbContext _context;

        public ClienteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClienteResponse>> ObtenerTodos(int page, int pageSize)
        {
            try
            {
                return await _context.Clientes
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new ClienteResponse(c.Id, c.Nombre, c.Documento, c.Telefono, c.Email))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener los clientes: {ex.Message}");
            }
        }

        public async Task<ClienteResponse> ObtenerPorId(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);

                if (cliente == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el cliente con ID {id}.");
                }

                return new ClienteResponse(cliente.Id, cliente.Nombre, cliente.Documento, cliente.Telefono, cliente.Email);
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener el cliente: {ex.Message}");
            }
        }

        public async Task<ClienteResponse> Crear(CrearClienteRequest dto)
        {
            try
            {
                // Validación: No pueden haber dos clientes con el mismo Documento
                bool existeDoc = await _context.Clientes.AnyAsync(c => c.Documento == dto.Documento);
                if (existeDoc)
                {
                    throw new RecursoExistenteException($"Ya existe un cliente registrado con el documento '{dto.Documento}'.");
                }

                Cliente nuevoCliente = new Cliente
                {
                    Nombre = dto.Nombre,
                    Documento = dto.Documento,
                    Telefono = dto.Telefono,
                    Email = dto.Email
                };

                _context.Clientes.Add(nuevoCliente);
                await _context.SaveChangesAsync();

                return new ClienteResponse(nuevoCliente.Id, nuevoCliente.Nombre, nuevoCliente.Documento, nuevoCliente.Telefono, nuevoCliente.Email);
            }
            catch (RecursoExistenteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al crear el cliente: {ex.Message}");
            }
        }

        public async Task Actualizar(int id, ActualizarClienteRequest dto)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);

                if (cliente == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el cliente con ID {id}.");
                }

                // Validar si el Documento que mandaron ya lo tiene OTRO cliente distinto
                bool docOcupado = await _context.Clientes
                    .AnyAsync(c => c.Documento == dto.Documento && c.Id != id);

                if (docOcupado)
                {
                    throw new RecursoExistenteException($"El documento '{dto.Documento}' ya pertenece a otro cliente.");
                }

                cliente.Nombre = dto.Nombre;
                cliente.Documento = dto.Documento;
                cliente.Telefono = dto.Telefono;
                cliente.Email = dto.Email;

                _context.Clientes.Update(cliente);
                await _context.SaveChangesAsync();
            }
            catch (RecursoNoExisteException) { throw; }
            catch (RecursoExistenteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al actualizar el cliente: {ex.Message}");
            }
        }

        public async Task Eliminar(int id)
        {
            try
            {
                // Buscamos el cliente directamente (sin Include)
                var cliente = await _context.Clientes.FindAsync(id);

                if (cliente == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el cliente con ID {id}.");
                }

                // VALIDACIÓN: Consultamos directamente a la tabla Salidas si existe alguna con este ClienteId
                var cantidadSalidas = await _context.Salidas.CountAsync(s => s.ClienteId == id);

                if (cantidadSalidas > 0)
                {
                    throw new DatosLlegaronErradosException($"No se puede eliminar el cliente '{cliente.Nombre}' porque tiene un historial de {cantidadSalidas} salida(s) registrada(s).");
                }

                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
            }
            catch (RecursoNoExisteException) { throw; }
            catch (DatosLlegaronErradosException) { throw; } // Atrapamos la validación
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al eliminar el cliente: {ex.Message}");
            }
        }
    }
}