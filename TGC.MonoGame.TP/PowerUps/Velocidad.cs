using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.MonoGame.TP.PowerUps
{
    public class Velocidad : PowerUp
    {
        private int _multiplicador; // puede ser X2 X3 X5 ....

        public Velocidad(double tiempoActual, int multiplicador) : base(tiempoActual)
        {
            this._multiplicador = multiplicador;
        }

        public int getMultiplicador()
        {
            return _multiplicador;
        }

    }
}
