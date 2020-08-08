using System;

namespace Aplicacion.Cursos
{
    public class PrecioDto
    {
         public Guid PrecioId {get;set;}        
        public decimal PrecioActual {get;set;}        
        public decimal PrecioPromocion {get;set;}
        public Guid CursoId {get;set;}        
    }
}