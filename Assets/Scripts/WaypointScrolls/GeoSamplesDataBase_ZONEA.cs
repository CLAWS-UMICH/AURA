using TMPro;
using UnityEngine;
using MixedReality.Toolkit.UX.Experimental;
using System.Linq;
using System.Collections.Generic;
using GLTFast.Schema;
using Unity.XR.CoreUtils;
using Unity.VisualScripting;

namespace MixedReality.Toolkit.Examples.Demos
{
    public class GeoSamplesDataBase_ZONEA : MonoBehaviour
    {
        public GeoSampleController geoSampleController;
        public VirtualizedScrollRectList list;
        private float destScroll;
        private bool animate;

        private void Start()
        {
            Debug.Log("geoSamples database // ZONE A // Start method called.");

            // Update visible items based on waypoint properties
            list.OnVisible = (go, i) =>
            {
                Debug.Log($"OnVisible called for index {i}.");

                // access zone a total geosamples list
                if (i < 0 || i >= AstronautInstance.User.geosampleZones[0].TotalGeoSamples.samples.Count)
                {
                    Debug.LogWarning($"Index {i} is out of range for danger waypoint list.");
                    return;
                }

                GeoSample sample = AstronautInstance.User.geosampleZones[0].TotalGeoSamples.samples[i];
                string nameField = sample.name;
                string shapeField = sample.shape;
                bool isSignificant = sample.isSignificant;
                bool didFind = false;
                foreach (Transform icon in go.transform.GetChild(2).GetChild(0).GetChild(0))
                {
                    if (icon.gameObject.name == shapeField)
                    {
                        didFind = true;
                        if (isSignificant)
                        {
                            icon.gameObject.SetActive(true);
                            break;
                        }
                        else
                        {
                            icon.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                            icon.gameObject.SetActive(true);
                            break;
                        }
                    }
                    else if (icon.gameObject.name == "UIButtonFontIcon")
                    {
                        icon.gameObject.GetComponent<TextMeshPro>().text = sample.zone[^1] + (i + 1).ToString();
                    }
                }
                if (!didFind)
                {
                    if (isSignificant)
                    {
                        go.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                    }
                    else 
                    {
                        go.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().color = Color.white;
                        go.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
                    }
                }
            };
        }

        private void Update()
        {
            if (animate)
            {
                float newScroll = Mathf.Lerp(list.Scroll, destScroll, 8 * Time.deltaTime);
                list.Scroll = newScroll;

                if (Mathf.Abs(list.Scroll - destScroll) < 0.02f)
                {
                    list.Scroll = destScroll;
                    animate = false;
                }
            }
        }

        public void Next()
        {
            animate = true;
            destScroll = Mathf.Min(list.MaxScroll, Mathf.Floor(list.Scroll / list.RowsOrColumns) * list.RowsOrColumns + list.TotallyVisibleCount);
        }

        public void Prev()
        {
            animate = true;
            destScroll = Mathf.Max(0, Mathf.Floor(list.Scroll / list.RowsOrColumns) * list.RowsOrColumns - list.TotallyVisibleCount);
        }
    }
}
