using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenesis.Events {
    public class CameraRotateEvent : Event {
        public CameraRotationControls CameraRotationControls { get; }
        public MSPAOrthoController CamController { get; }
        public float Angle { get; }
        public CameraRotateEvent(ref CameraRotationControls cameraRotationControls, ref MSPAOrthoController camController, float angle) {
            CameraRotationControls = cameraRotationControls;
            CamController = camController;
            Angle = angle;
        }
        public override string ToString() => $"{GetType().Name}:\n\tCameraRotationControls:{CameraRotationControls}\n\tCamController:{CamController}\n\tAngle:{Angle}";
    }
}
