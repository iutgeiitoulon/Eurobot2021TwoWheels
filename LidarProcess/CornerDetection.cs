using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace LidarProcessNS
{
    class CornerDetection
    {
        public static List<List<PointDExtended>> FindAllValidCrossingPoints(List<List<SegmentExtended>> list_of_family)
        {
            List<List<PointDExtended>> list_of_crossing_points = new List<List<PointDExtended>>();
            foreach (List<SegmentExtended> family in list_of_family)
            {
                List<int> list_of_case = Enumerable.Range(0, family.Count).ToList(); /// [0,1,2,3,...,n]
                List<List<int>> list_of_combinations_of_the_family = Toolbox.GetKCombs(list_of_case, 2).ToList().Select(x => x.ToList()).ToList(); /// [[0,1],[0,2],[0,3],[1,2],[1,3],[2,3],...]

                List<List<SegmentExtended>> list_of_parallel_combination = list_of_combinations_of_the_family.Select(
                    x => LineDetection.testIfSegmentArePerpendicular(family[x[0]], family[x[1]]) ? new List<SegmentExtended>() { family[x[0]], family[x[1]] } : null
                ).ToList();

                list_of_parallel_combination.RemoveAll(item => item == null);

                list_of_crossing_points.Add(list_of_parallel_combination.Select(x => Toolbox.GetCrossingPointBetweenSegment(x[0], x[1])).ToList());

            }

            list_of_crossing_points = list_of_crossing_points.Distinct().ToList();

            return list_of_crossing_points;
        }
    }
}
