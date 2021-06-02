using Momentum.Pm.Api.PortalApi.DataContracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Lidar
{
    /// <summary>Permet de créer une liste de <see cref="LidarDevice"/> construisant une carte à plusieurs.</summary>
    public class LidarFusion
    {
        #region Properties & Constructor

        /// <summary>Position du référentiel dans lequel sont placés les points Lidar.</summary>
        public PositionInfo ReferentialPosition { get; set; }
        
        public List<LidarPoint> Map = new List<LidarPoint>();

        /// <summary>Résolu lorsque la carte des Lidars réunis a été construite.</summary>
        public event MapAvailableEventHandler MapAvailable;

        public LidarFusion(IEnumerable<LidarDevice> lidars, PositionInfo referentialPosition)
        {
            Lidars = lidars;
            ReferentialPosition = referentialPosition;
        }

        #endregion
        #region LidarCollection

        private ObservableCollection<LidarDevice> _lidars;

        /// <summary>Liste des <see cref="LidarDevice"/> fonctionnant ensemble.</summary>
        public IEnumerable<LidarDevice> Lidars
        {
            get => _lidars;
            set
            {
                _lidars = new ObservableCollection<LidarDevice>(value);
                _lidars.CollectionChanged += Lidars_CollectionChanged;
                foreach (LidarDevice lidar in _lidars)
                    if (lidar != null)
                    {
                        // Supprime l'event s'il a déjà été attribué
                        lidar.PointsAvailable -= Lidar_PointsAvailable;
                        lidar.PointsAvailable += Lidar_PointsAvailable;
                    }
            }
        }

        private void Lidars_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (LidarDevice lidarDevice in e.NewItems)
                if (lidarDevice != null)
                {
                    // Supprime l'event s'il a déjà été attribué
                    lidarDevice.PointsAvailable -= Lidar_PointsAvailable;
                    lidarDevice.PointsAvailable += Lidar_PointsAvailable;
                }

            foreach (LidarDevice lidarDevice in e.OldItems)
                if (lidarDevice != null)
                    // Supprime l'event précédemment attribué
                    lidarDevice.PointsAvailable -= Lidar_PointsAvailable;
        }

        #endregion
        #region LidarDecoding

        private int _lidarCounter = 0;
        private List<LidarPoint> _mapTemporaryPointList = new List<LidarPoint>();

        private void Lidar_PointsAvailable(object sender, LidarPointsReadyEventArgs e)
        {
            List<LidarPoint> sourcePoints = e.LidarPoints;

            // Ajoute les points trouvés à la liste temporaire
            lock (sourcePoints)
            lock (_mapTemporaryPointList)
            {
                _mapTemporaryPointList.AddRange(sourcePoints);
            }

            // Met à jour la liste publique avec la temporaire si on a reçu les n cartes des Lidar
            if (++_lidarCounter == _lidars.Count)
            {
                lock (Map)
                lock (_mapTemporaryPointList)
                {
                        // Trie les points selon leur angle
                        Map = _mapTemporaryPointList.OrderBy(o => o.Angle).ToList();
                }

                // Remet à zéro les informations
                _lidarCounter = 0;
                lock (_mapTemporaryPointList) _mapTemporaryPointList = new List<LidarPoint>();

                MapAvailable?.Invoke(this, new MapAvailableEventArgs(Map));
            }
        }

        #endregion
    }

    public delegate void MapAvailableEventHandler(object sender, MapAvailableEventArgs e);

    public class MapAvailableEventArgs : EventArgs
    {
        /// <summary>Liste des points décrivant la carte construite.</summary>
        public readonly List<LidarPoint> Map;

        /// <summary>Crée une instance de <see cref="MapAvailableEventArgs"/>.</summary>
        public MapAvailableEventArgs(List<LidarPoint> map)
            => Map = map;
    }
}
