using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData
{
    public class moveResult
    {
        private long traversal;
        private long moved;

        #region Constructors
        public moveResult()
        {
            traversal = 0;
            moved = 0;
        }
        #endregion

        #region Properties
        public long traversalCounter
        {
            get { return traversal; }
        }

        public long movedCounter
        {
            get { return moved; }
        }

        public string resultText => "Traversed " + traversalCounter + " rows and moved " + movedCounter + " rows";
        #endregion

        #region Counter Mangement
        public void resetTraversalCounter()
        {
            traversal = 0;
        }

        public void resetMovedCounter()
        {
            moved = 0;
        }

        public void resetCounters()
        {
            resetTraversalCounter();
            resetMovedCounter();
        }

        public void incrementMovedCounter()
        {
            moved++;
        }

        public void incrementTraversalCounter()
        {
            traversal++;
        }
        #endregion

    }
}
