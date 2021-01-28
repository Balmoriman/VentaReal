using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WSVenta.Models;
using WSVenta.Models.Request;

namespace WSVenta.Services
{
    public class VentaService : IVentaService
    {
        public void Add(VentaRequest model)
        {
       
                using (VentaRealContext db = new VentaRealContext())
                {
                    //primero se guarda los elementos de la tabla padre 

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var venta = new Venta();
                            venta.Total = model.Conceptos.Sum(s => s.Cantidad * s.Importe);
                            venta.Fecha = DateTime.Now;
                            venta.IdCliente = model.IdCliente;
                            db.Venta.Add(venta);
                            db.SaveChanges();

                            //recorrido de todos los conectos uno or uno , guardado del detalle de la venta
                            foreach (var modelConcepto in model.Conceptos)
                            {
                                var concepto = new Models.Concepto();
                                concepto.Cantidad = modelConcepto.Cantidad;
                                concepto.IdProducto = modelConcepto.IdProducto;
                                concepto.PrecioUnitario = modelConcepto.PrecioUnitario;
                                concepto.IdVenta = venta.Id;
                                concepto.Importe = modelConcepto.Importe;
                                db.Concepto.Add(concepto);
                                db.SaveChanges();
                            }
                            transaction.Commit(); // si no se pone se va a bloquear la base de datos
                           
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw new Exception("ocurrio un error en la inserción"); 
                        }
                    }
                }
            
        }
    }
}
