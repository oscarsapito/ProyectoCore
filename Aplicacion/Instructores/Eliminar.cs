using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Persistencia.DapperConexion.Instructor;

namespace Aplicacion.Instructores
{
    public class Eliminar
    {
        public class Ejecuta : IRequest
        {
            public Guid Id {get;set;}
        }        

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly IInstructor _instructorRepisotory;
            public Manejador(IInstructor instructorRepository)
            {
                _instructorRepisotory = instructorRepository;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var resultado = await _instructorRepisotory.Elimina(request.Id);
                if(resultado >0)
                {
                    return Unit.Value;
                }
                throw new Exception("No se puedo eliminar la data del instructor");
            }
        }

    }
}