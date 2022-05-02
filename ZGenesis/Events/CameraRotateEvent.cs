using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenesis.Events {
    public class CameraRotateEvent : Event {
        public MSPAOrthoController CamController { get; }
        public float Angle { get; }
        public CameraRotateEvent(MSPAOrthoController camController, float angle) {
            CamController = camController;
            Angle = -angle;
        }
        public override string ToString() => $"{GetType().Name}:\n\tCamController:{CamController}\n\tAngle:{Angle}";
    }
}
