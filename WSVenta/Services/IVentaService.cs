using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WSVenta.Models.Request;

namespace WSVenta.Services
{
    public interface IVentaService
    {
        //para inyecion de dependencias
        public void Add(VentaRequest model);     
    }
}
