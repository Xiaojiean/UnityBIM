//-----------------------------------------------------------------------
// <copyright file="DetectedPlaneGenerator.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.Common
{
    using System.Collections.Generic;
    using GoogleARCore;
    using UnityEngine;

    /// <summary>
    /// Manages the visualization of detected planes in the scene.
    /// </summary>
    public class DetectedPlaneGenerator : MonoBehaviour
    {
        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject DetectedPlanePrefab;
        public GameObject planeDetectionPanel;
        public GameObject bottomMenuPanel;


        /// <summary>
        /// A list to hold new planes ARCore began tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<DetectedPlane> m_NewPlanes = new List<DetectedPlane>();
        private GlobalDataContainer container;

        /// <summary>
        /// The Unity Update method.
        /// </summary>

        public void Start()
        {
            FindDataContainer();
        }
        public void Update()
        {

            if (!container.RoomSelected)
            {
                return;
            }
            // Check that motion tracking is tracking.
            else if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }

            //Josua - The amount of detected planes of the last update, this is used to execute the OnPlaneDetectionFinished only on time, after the first detection of a plane
            var detectedPlanesOfLastUpdate = m_NewPlanes.Count;

            // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
            Session.GetTrackables<DetectedPlane>(m_NewPlanes, TrackableQueryFilter.New);

            //Josua - When in this update more than 0 planes were found and in the last update none, then initialize OnPlateDetectionFinished
            //This is important, otherwise the OnPlaneDetectionFinished would be called every update
            var detectedPlanesOfThisUpdate = m_NewPlanes.Count;
            if (detectedPlanesOfLastUpdate == 0 && detectedPlanesOfThisUpdate > 0)
            {
                OnPlaneDetectionFinished();
            }
            else if (detectedPlanesOfLastUpdate > 0 && detectedPlanesOfThisUpdate == 0)
            {
                OnPlanesLost();
            }


            for (int i = 0; i < m_NewPlanes.Count; i++)
            {
                // Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
                // the origin with an identity rotation since the mesh for our prefab is updated in Unity World
                // coordinates.
                GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
                planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(m_NewPlanes[i]);
            }
        }
        private void FindDataContainer()
        {
            GameObject containerObject = GameObject.Find(Constants.GlobalDataContainer);
            container = containerObject.GetComponent<GlobalDataContainer>();
        }

        public void OnPlaneDetectionFinished()
        {
            planeDetectionPanel.SetActive(false);
            bottomMenuPanel.SetActive(true);
        }

        public void OnPlanesLost()
        {
            planeDetectionPanel.SetActive(true);
            bottomMenuPanel.SetActive(false);
        }
    }
}
