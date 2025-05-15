using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.MonoGame.TP.PowerUps
{
    public class Elevacion : PowerUp
    {
        private float _elevacionExtra;

        public Elevacion(double tiempoActual, float elevacion) : base(tiempoActual)
        {
            this._elevacionExtra = elevacion;
        }

        public float getElevacionExtra()
        {
            return _elevacionExtra;
        }
    }
}
