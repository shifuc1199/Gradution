#define StoreVersion

using UnityEngine;
using System.Collections;

namespace TransitionalObjects
{
    public class RotatingTransition : BaseTransition
    {
        public bool reverseNegativeRotations;
        public Vector3 startPoint, endPoint;

        protected override void Transition(float transitionPercentage)
        {
            if(reverseNegativeRotations)
            {
#if StoreVersion
                Vector3 difference = FixRotations(endPoint - startPoint);//basically if any values are < 0 this inverts them properly
#else
            Vector3 difference = K2Maths.FixRotations(endPoint.localRotation.eulerAngles - startPoint.localRotation.eulerAngles);//basically if any values are < 0 this inverts them properly
#endif

                parent.transform.localEulerAngles = startPoint + difference * transitionPercentage;
            }
            else
                parent.transform.localEulerAngles = startPoint + (endPoint - startPoint) * transitionPercentage;
        }

#if StoreVersion
        public static Vector3 FixRotations(Vector3 eulerInput)
        {
            if(eulerInput.x < 0)// || eulerInput.x > 180)
                eulerInput.x = 360 + eulerInput.x;

            if(eulerInput.y < 0)// || eulerInput.y > 180)
                eulerInput.y = 360 + eulerInput.y;

            if(eulerInput.z < 0)// || eulerInput.z > 180)
                eulerInput.z = 360 + eulerInput.z;

            return eulerInput;
        }
#endif

        public override void Clone(BaseTransition other)
        {
            base.Clone(other);

            RotatingTransition converted = (RotatingTransition)other;

            reverseNegativeRotations = converted.reverseNegativeRotations;
            startPoint = converted.startPoint;
            endPoint = converted.endPoint;
        }

        #region Editor Externals
#if(UNITY_EDITOR)
        /// <summary>
        /// Called by the editor to view either the start of end point
        /// </summary>
        public override void ViewPosition(TransitionalObject.MovingDataType movingType)
        {
            if(movingType == TransitionalObject.MovingDataType.StartPoint)
                parent.transform.localEulerAngles = startPoint;
            else
                parent.transform.localEulerAngles = endPoint;
        }

        /// <summary>
        /// Called by the editor to update the start and end points based on the current position
        /// </summary>
        /// <param name="isStartPoint"></param>
        public override void UpdatePosition(TransitionalObject.MovingDataType movingType)
        {
            if(movingType == TransitionalObject.MovingDataType.StartPoint)
                startPoint = parent.transform.rotation.eulerAngles;
            else
                endPoint = parent.transform.rotation.eulerAngles;
        }
#endif
        #endregion
    }
}