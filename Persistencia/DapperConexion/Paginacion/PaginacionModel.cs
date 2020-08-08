using System.Collections.Generic;

namespace Persistencia.DapperConexion.Paginacion
{
    public class PaginacionModel
    {
        public List<IDictionary<string ,object>> ListaRecords {get;set;}
        //Lo que va retornar de una base de datos ---- [ {cursoId : "123213", "titulo" : "aspnet"}], [{"cursoId : "34332", "titulo" :"react"}]
        public int TotalRecords {get;set;}
        public int NumeroPaginas {get;set;}
    }
}