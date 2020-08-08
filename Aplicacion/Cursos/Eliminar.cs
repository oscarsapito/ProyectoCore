using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Eliminar
    {
        public class Ejecuta : IRequest {
            public Guid Id {get;set;}
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;
            public Manejador(CursosOnlineContext context){
                    _context = context;
                }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var instructoresDB = _context.CursoInstructor.Where(x=>x.CursoId == request.Id);
                foreach(var instructor in instructoresDB){
                    _context.CursoInstructor.Remove(instructor);
                }

                var comentariosDB = _context.Comentario.Where(x=>x.CursoId == request.Id);
                foreach(var cmt in comentariosDB){
                    _context.Comentario.Remove(cmt);
                }

                var precioDB = _context.Precio.Where(x=>x.CursoId == request.Id).FirstOrDefault();
                if(precioDB != null){
                    _context.Precio.Remove(precioDB);
                }


                                
                var curso = await _context.Curso.FindAsync(request.Id);
                if(curso == null){
                    //throw new Exception ("No se puede eliminar curso");
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound,new {mensaje = "No se encontro el curso"});//objeto json llamado mensaje,pero podria ser cualquie variable
                }
                _context.Remove(curso);

                var resultado = await _context.SaveChangesAsync();

                if(resultado > 0){
                    return Unit.Value;
                }

                throw new Exception("No se pudieron guardar los cambios");
            }
        }

    }
}