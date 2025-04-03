using UnityEngine;

namespace XRMultiplayer
{
    public class SeatBillboard : MonoBehaviour
    {
        public void RotateBillboard(int seatID)
        {
            transform.localRotation = Quaternion.Euler(0, SeatIDToAngle(seatID), 0);
        }

        float SeatIDToAngle(int seatID)
        {
            switch (seatID)
            {
                case 0:
                    return 0;
                case 1:
                    return 180;
                case 2:
                    return 270;
                case 3:
                    return 90;
                default:
                    return 0;
            }
        }
    }
}
