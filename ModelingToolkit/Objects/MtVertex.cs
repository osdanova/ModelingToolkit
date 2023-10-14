using System.Collections.Generic;
using System.Numerics;

namespace ModelingToolkit.Objects
{
    public class MtVertex
    {
        public Vector3? AbsolutePosition { get; set; }
        public List<MtWeightPosition> Weights { get; set; }
        public Vector3? TextureCoordinates { get; set; } // UV
        public Vector4? Color { get; set; } // RGBA
        public Vector3? Normal { get; set; }
        public bool HasWeights { get => Weights != null && Weights.Count > 0; }
        public bool HasTextureCoordinates { get => TextureCoordinates != null; }
        public bool HasColor { get => Color != null; }
        public bool HasNormal { get => Normal != null; }

        public MtVertex()
        {
            AbsolutePosition = null;
            Weights = new List<MtWeightPosition> ();
            TextureCoordinates = null;
            Color = null;
            Normal = null;
        }

        public override string ToString()
        {
            string output = AbsolutePosition + "";
            if (TextureCoordinates != null) {
                output += " [UV: "+TextureCoordinates.Value.X+ ";"+TextureCoordinates.Value.Y+"]";
            }
            return output + " [Weights: "+Weights.Count+"] [HasColor: "+(Color!=null)+ "] [HasNormal: "+(Normal!=null)+"]";
        }

        public bool Equals(MtVertex secondVertex)
        {
            if(secondVertex == null)
            {
                return false;
            }

            if (HasWeights)
            {
                if (Weights.Count != secondVertex.Weights.Count)
                {
                    return false;
                }
                else
                {
                    for (int i = 0; i < Weights.Count; i++)
                    {
                        if (!Weights[i].Equals(secondVertex.Weights[i]))
                        {
                            return false;
                        }
                    }
                }
            }
            else if (AbsolutePosition != secondVertex.AbsolutePosition)
            {
                return false;
            }

            if (TextureCoordinates != secondVertex.TextureCoordinates)
            {
                return false;
            }

            if (Color != secondVertex.Color)
            {
                return false;
            }

            if (Normal != secondVertex.Normal)
            {
                return false;
            }

            return true;
        }
    }
}
