using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameToolkit
{
    public struct NineSliceData
    {
        public readonly int LeftPadding;
        public readonly int RightPadding;
        public readonly int TopPadding;
        public readonly int BottomPadding;

        public NineSliceData(int leftPadding, int rightPadding, int topPadding, int bottomPadding)
        {
            this.LeftPadding = leftPadding;
            this.RightPadding = rightPadding;
            this.TopPadding = topPadding;
            this.BottomPadding = bottomPadding;
        }
    }
}
