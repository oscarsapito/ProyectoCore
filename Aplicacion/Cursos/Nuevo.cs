using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Nuevo              //Nuevo Curso
    {
        public class Ejecuta : IRequest {
            //[Required(ErrorMessage="Por favor ingrese el titulo del curso")]
            public string Titulo {get;set;}
            public string Descripcion {get;set;}
            public DateTime? FechaPublicacion {get;set;}
            public List<Guid> ListaInstructor {get;set;}
            public decimal Precio {get;set;}
            public decimal PrecioPromocion {get;set;}

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
                Guid _cursoId = Guid.NewGuid();
                var curso = new Curso{
                    CursoId = _cursoId,
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    FechaPublicacion = request.FechaPublicacion,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.Curso.Add(curso);

                if(request.ListaInstructor != null){                    
                    foreach(var id in request.ListaInstructor){
                        var cursoInstructor = new CursoInstructor{
                            CursoId = _cursoId,
                            InstructorId = id
                        };
                        _context.CursoInstructor.Add(cursoInstructor);
                    }
                }

                /*Agregar logica para insertar un precio del curso*/
                var precioEntidad = new Precio{
                    CursoId = _cursoId,
                    PrecioActual = request.Precio,
                    PrecioPromocion = request.PrecioPromocion,
                    PrecioId = Guid.NewGuid()
                };
                
                _context.Precio.Add(precioEntidad);
                var valor = await _context.SaveChangesAsync();
                if(valor >0){
                    return Unit.Value;
                }

                throw new Exception("No se pudo insertar el curso");

            }
        }
    }
}