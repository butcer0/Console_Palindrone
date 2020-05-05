namespace ConsoleApp_Sandbox.Puzzles.Models
{
    public class Location
    {
        /// <summary>
        /// Location X
        /// </summary>
        public int X;
        /// <summary>
        /// Location Y
        /// </summary>
        public int Y;
        /// <summary>
        /// G + H
        /// </summary>
        /// <remarks>
        /// (i.e. Distance from start + Distance from destination 
        /// or cost thus far + estimated cost remaining)
        /// </remarks>
        public int F;
        /// <summary>
        /// Distance from start. Computed by adding each step incrementally.
        /// </summary>
        public int G;
        /// <summary>
        /// Distance to target. Computed mathematically.
        /// </summary>
        public int H;
        /// <summary>
        /// Parent node along path. Root node parent = null;
        /// </summary>
        public Location Parent;
    }
}