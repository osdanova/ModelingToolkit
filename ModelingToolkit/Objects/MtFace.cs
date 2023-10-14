using System.Collections.Generic;

namespace ModelingToolkit.Objects
{
    public class MtFace
    {
        public List<int> VertexIndices { get; set; }
        public List<MtVertex> Vertices { get; set; } // Used for TriStrips
        public bool Clockwise { get; set; }

        public MtFace()
        {
            VertexIndices = new List<int>();
            Vertices = new List<MtVertex>();
            Clockwise = true;
        }

        public override string ToString()
        {
            string output = "[";
            foreach (int i in VertexIndices) { output += " " + i; }
            output += " ]";
            return output + " Clockwise: " + Clockwise;
        }
    }
}
