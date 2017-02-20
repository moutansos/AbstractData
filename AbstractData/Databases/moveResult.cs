using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractData.Databases
{
    class moveResult
    {
        private long traversal;
        private long moved;

        #region Constructors
        public moveResult()
        {
            traversalCounter = 0;
            movedCounter = 0;
        }
        #endregion

        #region Properties
        public long traversalCounter
        {
            get { return traversal; }
            set
            {
                //Only alow increment
                if (value == traversal + 1)
                {
                    traversal = value;
                }
                else
                {
                    throw new ArgumentException("Invalid assignment to reslut class");
                }
            }
        }

        public long movedCounter
        {
            get { return traversal; }
            set
            {
                //Only alow increment
                if (value == moved + 1)
                {
                    moved = value;
                }
                else
                {
                    throw new ArgumentException("Invalid assignment to reslut class");
                }
            }
        }
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
        #endregion

    }
}
