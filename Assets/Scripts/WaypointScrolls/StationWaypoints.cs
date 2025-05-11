using TMPro;
using UnityEngine;
using MixedReality.Toolkit.UX.Experimental;
using System.Linq;
using System.Collections.Generic;

namespace MixedReality.Toolkit.Examples.Demos
{
    public class StationWaypoints : MonoBehaviour
    {
        public NavigationController navigationController;
        public VirtualizedScrollRectList list;
        private float destScroll;
        private bool animate;

        private void Start()
        {
            Debug.Log("Station Start method called.");

            // Update visible items based on waypoint properties
            list.OnVisible = (go, i) =>
            {
                Debug.Log($"OnVisible called for index {i}.");

                // Get the waypoint ID from the index
                if (i < 0 || i >= navigationController.StationWaypointList.Count)
                {
                    Debug.LogWarning($"Index {i} is out of range for danger waypoint list.");
                    return;
                }

                Waypoint waypoint = navigationController.StationWaypointList[i];
                Debug.Log($"Waypoint retrieved: Name = {waypoint.Name}, ID = {waypoint.Id}");

                // Update the name and icon field
                foreach (var text in go.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    if (text.gameObject.name == "Text")
                    {
                        text.text = $"{waypoint.Name}";
                        Debug.Log($"Updated Text field with waypoint name: {waypoint.Name}");
                    }
                    if (text.gameObject.name == "UIButtonFontIcon")
                    {
                        text.text = $"{waypoint.Name[0]}";
                        Debug.Log($"Updated UIButtonFontIcon field with first letter of waypoint name: {waypoint.Name[0]}");
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
