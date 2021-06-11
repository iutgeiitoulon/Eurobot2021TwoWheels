using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TracjectoryOptimization
{
    public class TrajectoryOptimization
    {
        /// <summary>
        /// Overall view of the algorithm:
        /// Initialize cities as depot, the cities and depots coordinates, ACO and MMTSP parameters.
        /// Distribute ants on current position of robot
        /// while the process has not met the termination criteria, do
        ///     for each ants, do
        ///             Transition probability process untill return to depot
        ///     end for
        ///     Compute the ant total distance
        ///     for each arc, do
        ///         Pheromone update
        ///     end for
        /// end while
        /// Return the best solution found
        /// </summary>
        public class AntColonyProgram
        {
            static Random random = new Random(0);
            static int alpha = 3; // impact strenght of pheromones
            static int beta = 2;  // impact strenght of distances between height
            static double rho = 0.01; // impact evaporation rate 
            static double Q = 2.0; // impact the increase of pheromones


            public static void AOCMethod(int numCities, int numDepots, int numCitiesBeforeDepot, int maxCitiesInATrail) //add list of cities + list of depots
            {
                int numAnts = 4;
                int maxTime = 1000;

                int numDepotsNecessary = NumberOfDepotsNecessary(numCities, numCitiesBeforeDepot, maxCitiesInATrail);
                int[][] dists = MakeGraphDistances(numCities, numDepotsNecessary); // list of coordinates : cities + depots
                int[][] ants = InitAnts(numAnts, numCities, numDepots, numCitiesBeforeDepot, maxCitiesInATrail, numDepotsNecessary);
                double[][] pheromones = InitPheromones(numCities, numDepots);

                int[] bestTrail = BestTrail(ants, dists);
                double bestLength = Length(bestTrail, dists);

                int time = 0;
                while (time < maxTime)
                {
                    UpdateAnts(ants, pheromones, dists, numCities, numDepots, numCitiesBeforeDepot, maxCitiesInATrail, numDepotsNecessary);
                    UpdatePheromones(pheromones, ants, dists);

                    int[] currentBestTrail = BestTrail(ants, dists);
                    double currentBestLength = Length(currentBestTrail, dists);
                    if (currentBestLength < bestLength)
                    {
                        bestLength = currentBestLength;
                        bestTrail = currentBestTrail;
                    }
                    ++time;
                }
            } // Main

            private static int[][] InitAnts(int numAnts, int numCities, int numDepots, int numCitiesBeforeDepot, int maxCitiesInATrail, int numDepotsNecessary)
            {
                int[][] ants = new int[numAnts][];
                int start = 0; // the current position of the robot is considered the root of the tree
                for (int k = 0; k < numAnts; k++)
                    ants[k] = RandomTrail(start, numCities, numDepots, numCitiesBeforeDepot, maxCitiesInATrail, numDepotsNecessary);
                return ants;
            }

            private static int[] RandomTrail(int start, int numCities, int numDepots, int numCitiesBeforeDepot, int maxCitiesInATrail, int numDepotsNecessary)
            {
                int[] trail = new int[numCities + numDepotsNecessary];
                bool[] visited = new bool[numCities];
                trail[0] = start;
                visited[start] = true;
                int r;

                for (int i = 0; i < numCities + numDepotsNecessary; i++)
                {
                    if (IsHaveToGoToDepot(i, maxCitiesInATrail, numCitiesBeforeDepot))
                        r = random.Next(numCities, numCities + numDepots);
                    else
                    {
                        do
                            r = random.Next(1, numCities);
                        while (visited[r] != true);
                        visited[r] = true;
                    }
                    trail[i] = r;
                }
                return trail;
            }

            private static double Length(int[] trail, int[][] dists)
            {
                double length = 0.0;
                for (int i = 0, max = trail.Length - 1; i < max; i++)
                    length += Distance(trail[i], trail[i + 1], dists);
                return length;
            }

            private static int[] BestTrail(int[][] ants, int[][] dists)
            {
                int bestAnt = 0;
                double bestDist = Length(ants[1], dists);
                double calculateDist = 0.0;

                for (int i = 1, max = ants.GetLength(1); i < max; i++)
                {
                    calculateDist = Length(ants[i], dists);
                    if (calculateDist < bestDist)
                    {
                        bestDist = calculateDist;
                        bestAnt = i;
                    }
                }
                return ants[bestAnt];
            }

            private static double[][] InitPheromones(int numCities, int numDepots)
            {
                double[][] pheromones = new double[numCities + numDepots][];
                for (int i = 0; i < numCities; i++)
                    pheromones[i] = new double[numCities + numDepots];
                for (int i = 0, max = pheromones.Length; i < max; i++)
                    for (int j = 0, max2 = pheromones[i].Length; j < max2; j++)
                        pheromones[i][j] = 0.01;
                return pheromones;
            }

            private static void UpdateAnts(int[][] ants, double[][] pheromones, int[][] dists, int numCities,
                int numDepots, int numCitiesBeforeDepot, int maxCitiesInATrail, int numDepotsNecessary)
            {
                for (int k = 0, max = ants.GetLength(1); k < max; k++)
                {
                    int start = 0; // the current position of the robot is considered the root of the tree
                    int[] newTrail = BuildTrail(k, start, pheromones, dists, numCities, numDepots, numCitiesBeforeDepot, maxCitiesInATrail, numDepotsNecessary);
                    ants[k] = newTrail;
                }
            }

            private static int[] BuildTrail(int k, int start, double[][] pheromones, int[][] dists, int numCities,
                int numDepots, int numCitiesBeforeDepot, int maxCitiesInATrail, int numDepotsNecessary)
            {
                int[] trail = new int[numCities + numDepotsNecessary];
                bool[] visited = new bool[numCities];
                trail[0] = start;
                visited[start] = true;
                for (int i = 0; i < numCities + numDepotsNecessary - 1; i++)
                {
                    int cityX = trail[i];
                    int next = NextCity(k, cityX, visited, pheromones, dists, numCities, i, numDepots, numCitiesBeforeDepot, maxCitiesInATrail);
                    trail[i + 1] = next;
                    visited[next] = true;
                }
                return trail;
            }

            private static int NextCity(int k, int cityX, bool[] visited, double[][] pheromones, int[][] dists, int numCities,
                int numHeightInTrail, int numDepots, int numCitiesBeforeDepot, int maxCitiesInATrail)
            {
                double[] probs;

                if (IsHaveToGoToDepot(numHeightInTrail, maxCitiesInATrail, numCitiesBeforeDepot))
                    probs = MoveProbsToDepot(k, cityX, pheromones, dists, numCities, numDepots);
                else
                    probs = MoveProbsToCity(k, cityX, visited, pheromones, dists, numCities);

                double[] cumul = new double[probs.Length + 1];
                for (int i = 0, max = probs.Length; i < max; i++)
                    cumul[i + 1] = cumul[i] + probs[i];

                double p = random.NextDouble();

                for (int i = 0, max = cumul.Length; i < max - 1; i++)
                    if (p >= cumul[i] && p < cumul[i + 1])
                    {
                        if (IsHaveToGoToDepot(numCities, maxCitiesInATrail, numCitiesBeforeDepot))
                            return i + numCities;
                        else
                            return i;
                    }
                throw new Exception("Failure to return valid city in NextCity");
            }

            private static double[] MoveProbsToCity(int k, int cityX, bool[] visited, double[][] pheromones, int[][] dists, int numCities)
            {
                double sum = 0.0;

                double[] tauEta = new double[numCities];
                for (int i = 0, max = tauEta.Length; i < max; i++)
                {
                    if (i == cityX)
                        tauEta[i] = 0.0; // Prob of moving to self is zero
                    else if (visited[i] == true)
                        tauEta[i] = 0.0; // Prob of moving to a visited node is zero
                    else
                    {
                        tauEta[i] = Math.Pow(pheromones[cityX][i], alpha) * Math.Pow((1.0 / Distance(cityX, i, dists)), beta);

                        if (tauEta[i] < 0.0001)
                            tauEta[i] = 0.0001;
                        else if (tauEta[i] > (double.MaxValue / (numCities * 100)))
                            tauEta[i] = double.MaxValue / (numCities * 100);
                    }
                    sum += tauEta[i];
                }

                double[] probs = new double[numCities];
                for (int i = 0, max = probs.Length; i < max; i++)
                    probs[i] = tauEta[i] / sum;
                return probs;
            }

            private static double[] MoveProbsToDepot(int k, int cityX, double[][] pheromones, int[][] dists, int numCities, int numDepots)
            {
                double sum = 0.0;

                double[] taueta = new double[numDepots];
                for (int i = numCities, max = taueta.Length; i < max + numCities; i++)
                {
                    taueta[i] = Math.Pow(pheromones[cityX][i], alpha) * Math.Pow((1.0 / Distance(cityX, i, dists)), beta);

                    if (taueta[i] < 0.0001)
                        taueta[i] = 0.0001;
                    else if (taueta[i] > (double.MaxValue / (numCities * 100)))
                        taueta[i] = double.MaxValue / (numCities * 100);

                    sum += taueta[i];
                }

                double[] probs = new double[numDepots];
                for (int i = 0, max = probs.Length; i < max; i++)
                    probs[i] = taueta[i] / sum;
                return probs;
            }

            private static void UpdatePheromones(double[][] pheromones, int[][] ants, int[][] dists)
            {
                double length = 0.0;
                double decrease = 0.0;
                double increase = 0.0;
                int currentCity = 0;
                int nextCity = 0;

                for (int k = 0, max = ants.GetLength(1); k < max; k++)
                {
                    for (int i = 0, max2 = ants.GetLength(2); i < max2 - 1; i++)
                    {
                        currentCity = ants[k][i];
                        nextCity = ants[k][i + 1];
                        decrease = (1.0 - rho) * pheromones[currentCity][nextCity];
                        if (EdgeInTrail(currentCity, nextCity, ants[k]) == true)
                            increase = (Q / length);
                        pheromones[currentCity][nextCity] = decrease + increase;
                    }
                }
            }

            private static bool EdgeInTrail(int nodeX, int nodeY, int[] trail)
            {
                bool isEdgeInTrail = false;
                for (int i = 0, max = trail.Length; i < max - 1; i++)
                    if (trail[i] == nodeX && trail[i + 1] == nodeY)
                        isEdgeInTrail = true;
                return isEdgeInTrail;
            }

            private static int[][] MakeGraphDistances(int numCities, int numDepotsNecessary)
            {
                int[][] dists = new int[numCities + numDepotsNecessary][];
                for (int i = numCities; i < numCities + numDepotsNecessary; i++)
                    dists[i] = new int[numCities + numDepotsNecessary];

                for (int i = 0; i < numCities + numDepotsNecessary; i++)
                    for (int j = i + 1; j < numCities + numDepotsNecessary; j++)
                    {
                        int d = 1; //Toolbox.Distance(xCup, yCup);
                        dists[i][j] = d; dists[j][i] = d;
                    }
                return dists;
            }

            private static double Distance(int cityX, int cityY, int[][] dists) => dists[cityX][cityY];

            private static int NumberOfDepotsNecessary(int numCities, int numCitiesBeforeDepot, int maxCitiesInATrail)
            {
                int numDepots = 1;
                numCities -= numCitiesBeforeDepot;
                numDepots += numCities / maxCitiesInATrail;
                if ((numCities % maxCitiesInATrail) != 0)
                    numDepots += 1;
                return numDepots;
            }

            private static bool IsHaveToGoToDepot(int numCities, int maxCitiesInATrail, int numCitiesBeforeDepot)
                => (numCities % maxCitiesInATrail) == numCitiesBeforeDepot;

        }

    }
}
