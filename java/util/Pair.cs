namespace RotMGStats.RealmShark.NET.java.util
{
    /// <summary>
    /// Basic pair class
    /// </summary>
    /// <typeparam name="A">Type of objects to the left.</typeparam>
    /// <typeparam name="B">Type of objects to the right.</typeparam>
    public class Pair<A, B>
    {
        private readonly A fst;
        private readonly B snd;

        /// <summary>
        /// Constructor initializing the pair
        /// </summary>
        /// <param name="fst">Left object</param>
        /// <param name="snd">Right object</param>
        public Pair(A fst, B snd)
        {
            this.fst = fst;
            this.snd = snd;
        }

        /// <summary>
        /// Returns the left object
        /// </summary>
        /// <returns>The left object</returns>
        public A Left()
        {
            return fst;
        }

        /// <summary>
        /// Returns the right object
        /// </summary>
        /// <returns>The right object</returns>
        public B Right()
        {
            return snd;
        }

        /// <summary>
        /// toString of the pair
        /// </summary>
        public override string ToString()
        {
            return $"Pair[{fst},{snd}]";
        }
    }
}