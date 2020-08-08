using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Dominio;
using Persistencia;
using FluentValidation;
using Aplicacion.ManejadorError;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace Aplicacion.Cursos
{
    public class Editar
    {
        public class Ejecuta : IRequest {

            public Guid CursoId {get;set;}
            public string Titulo {get;set;}
            public string Descripcion {get;set;}
            public DateTime? FechaPublicacion {get;set;}
            public List<Guid> ListaInstructor {get;set;}

            public decimal? Precio {get;set;}
            public decimal? PrecioPromocion {get;set;}

        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta>{
            public EjecutaValidacion(){
                RuleFor(x => x.Titulo).NotEmpty();
                RuleFor(x => x.Descripcion).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {

            private readonly CursosOnlineContext _context;
            public Manejador(CursosOnlineContext context){
                _context = context;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var curso = await _context.Curso.FindAsync(request.CursoId);
                if(curso == null){
                    //throw new Exception ("No se puede eliminar curso");
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound,new {mensaje = "No se encontro el curso"});//objeto json llamado mensaje,pero podria ser cualquie variable
                }
                curso.Titulo =request.Titulo ?? curso.Titulo;
                curso.Descripcion = request.Descripcion ?? curso.Descripcion;
                curso.FechaPublicacion = request.FechaPublicacion ?? curso.FechaPublicacion;
                curso.FechaCreacion = DateTime.UtcNow;
                
                /*Actializar el precio del curso*/
                var precioEntidad = _context.Precio.Where(x=>x.CursoId == curso.CursoId).FirstOrDefault();
                if(precioEntidad != null){
                    precioEntidad.PrecioPromocion = request.PrecioPromocion ?? precioEntidad.PrecioPromocion;
                    precioEntidad.PrecioActual = request.Precio ?? precioEntidad.PrecioActual;
                }else{
                    precioEntidad = new Precio{
                        PrecioId = Guid.NewGuid(),
                        PrecioActual = request.Precio ?? 0,
                        PrecioPromocion = request.PrecioPromocion ?? 0,
                        CursoId = curso.CursoId
                    };
                    await _context.Precio.AddAsync(precioEntidad);
                }



                if(request.ListaInstructor != null){
                    if(request.ListaInstructor.Count >0){
                        //Eliminar los instructores actuales del curso en la base de datos
                        var instructoreBD = _context.CursoInstructor.Where(x=> x.CursoId == request.CursoId).ToList();
                        foreach(var instructorEliminar in instructoreBD){
                            _context.CursoInstructor.Remove(instructorEliminar);
                        }
                        /*Fin del procedimiento para eliminar instructores*/

                        /*Procedimiento para agregar instructores que provienen del cliente*/
                        foreach(var id in request.ListaInstructor){
                            var nuevoInstructor = new CursoInstructor{
                                CursoId = request.CursoId,
                                InstructorId = id
                            };
                            _context.CursoInstructor.Add(nuevoInstructor);
                        }
                        /*Fin de procedimiento*/
                    }
                }


                var resultado = await _context.SaveChangesAsync();

                if(resultado>0)
                    return Unit.Value;
                throw new Exception("No se guardaron los cambios en el curso");

            }
        }
    }
}