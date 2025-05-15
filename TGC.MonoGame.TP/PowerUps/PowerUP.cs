using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.MonoGame.TP.Modelos;

namespace TGC.MonoGame.TP.PowerUps
{
    public abstract class PowerUp
    {
        protected static float duracion = 10f; //10 segundos para cualquier power up
        protected double tiempoFinal; //en segundos
        public bool NoVigente { get; set; } = false;

        public PowerUp(double tiempoActual)
        {
            tiempoFinal = tiempoActual + duracion;
        }

        //public abstract float implementar();

        public bool estaVigente(double tiempoActual)
        {
            return tiempoActual <= tiempoFinal;
        }

    }
}
