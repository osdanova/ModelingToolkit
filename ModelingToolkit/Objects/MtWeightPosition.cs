using System.Numerics;

namespace ModelingToolkit.Objects
{
    public class MtWeightPosition
    {
        public int? JointIndex { get; set; }
        public Vector3? RelativePosition { get; set; }
        public float? Weight { get; set; }

        public MtWeightPosition()
        {
            JointIndex = null;
            RelativePosition = null;
            Weight = null;
        }

        public bool Equals(MtWeightPosition secondWeightPosition)
        {
            if (JointIndex != secondWeightPosition.JointIndex) {
                return false;
            }
            if (RelativePosition != secondWeightPosition.RelativePosition) {
                return false;
            }
            if(Weight != secondWeightPosition.Weight) {
                return false;
            }

            return true;
        }
    }
}
