// AForge Machine Learning Library
// AForge.NET framework
//
// Copyright � Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.MachineLearning
{
    using System;

    /// <summary>
    /// Epsilon greedy exploration policy.
    /// </summary>
    /// 
    /// <remarks><para>The class implements epsilon greedy exploration policy. Acording to the policy,
    /// the best action is chosen with probability <b>1-epsilon</b>. Otherwise,
    /// with probability <b>epsilon</b>, any other action, except the best one, is
    /// chosen randomly.</para>
    /// <para>According to the policy, the epsilon value is known also as exploration rate.</para>
    /// </remarks>
    /// 
    public class EpsilonGreedyExploration : IExplorationPolicy
    {
        // exploration rate
        private double epsilon;

        // random number generator
        private Random rand = new Random( (int) DateTime.Now.Ticks );

        /// <summary>
        /// Epsilon value (exploration rate).
        /// </summary>
        /// 
        public double Epsilon
        {
            get { return epsilon; }
            set { epsilon = Math.Max( 0.0, Math.Min( 1.0, value ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EpsilonGreedyExploration"/> class.
        /// </summary>
        /// 
        /// <param name="epsilon">Epsilon value (exploration rate).</param>
        /// 
        public EpsilonGreedyExploration( double epsilon )
        {
            Epsilon = epsilon;
        }

        /// <summary>
        /// Choose an action.
        /// </summary>
        /// 
        /// <param name="actionEstimates">Action estimates.</param>
        /// 
        /// <returns>Returns the next action.</returns>
        /// 
        /// <remarks>The method chooses an action depending on the provided estimates. The
        /// estimates can be any sort of estimate, which values usefulness of the action
        /// (expected summary reward, discounted reward, etc).</remarks>
        /// 
        public int ChooseAction( double[] actionEstimates )
        {
            // actions count
            int actionsCount = actionEstimates.Length;

            // find the best action (greedy)
            double maxReward = actionEstimates[0];
            int greedyAction = 0;

            for ( int i = 1; i < actionsCount; i++ )
            {
                if ( actionEstimates[i] > maxReward )
                {
                    maxReward = actionEstimates[i];
                    greedyAction = i;
                }
            }

            // try to do exploration
            if ( rand.NextDouble( ) < epsilon )
            {
                int randomAction = rand.Next( actionsCount - 1 );

                if ( randomAction >= greedyAction )
                    randomAction++;

                return randomAction;
            }

            return greedyAction;
        }
    }
}
