using System.Collections.Generic;

namespace ModelingToolkit.Objects
{
    // A triangle strip. Every 3 indices it's a face. The last vertex is always the new one, the other 2 alternate order. Eg: 012 213 234 435
    public class MtTriangleStrip
    {
        public bool IsClockwise { get; set; }
        public List<MtVertex> Vertices { get; set; }
        public List<int> VertexIds { get; set; }

        public MtTriangleStrip()
        {
            IsClockwise= false;
            Vertices = new List<MtVertex>();
            VertexIds= new List<int>();
        }

        // Adds the given face to the tristrip. If the face can't be added it returns false.
        public bool AddFace(MtFace newFace)
        {
            if (Vertices.Count == 0)
            {
                Vertices.Add(newFace.Vertices[0]);
                Vertices.Add(newFace.Vertices[1]);
                Vertices.Add(newFace.Vertices[2]);
                VertexIds.Add(0);
                VertexIds.Add(1);
                VertexIds.Add(2);
                return true;
            }

            // Vertex matches. Value1 = newFace's index. Value2 = strip VertexId
            (int, int)? match1 = null;
            (int, int)? match2 = null;

            // Iterate over face vertices
            for(int i = 0; i < 3; i++) 
            {
                if (match2 != null) break;

                // Iterate over strip's previous vertices
                for (int j = VertexIds.Count - 1; j >= VertexIds.Count - 3; j--)
                {
                    // Index already matched
                    if (match1 != null && match1.Value.Item2 == j) {
                        continue;
                    }

                    if (newFace.Vertices[i].Equals(Vertices[VertexIds[j]]))
                    {
                        if(match1 == null) {
                            match1 = (i,j);
                        }
                        else {
                            match2 = (i, j);
                            break;
                        }
                    }
                }
            }

            // No matches found, can't add to this strip
            if(match1 == null || match2 == null) {
                return false;
            }

            // Add matched vertices
            if(VertexIds.Count % 2 == 0)
            {
                VertexIds.Add(VertexIds[match1.Value.Item2]);
                VertexIds.Add(VertexIds[match2.Value.Item2]);
            }
            else
            {
                VertexIds.Add(VertexIds[match2.Value.Item2]);
                VertexIds.Add(VertexIds[match1.Value.Item2]);
            }

            // Add new vertex
            for (int i = 0; i < 3; i++)
            {
                if(i != match1.Value.Item1 && i != match2.Value.Item1)
                {
                    VertexIds.Add(Vertices.Count);
                    Vertices.Add(newFace.Vertices[i]);
                    break;
                }
            }

            return true;
        }

        public void FlipOrder()
        {
            for(int i = 0; i < VertexIds.Count; i += 3)
            {
                int temp = VertexIds[i];
                VertexIds[i] = VertexIds[i+1];
                VertexIds[i + 1] = temp;
            }
        }
    }
}
