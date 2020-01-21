using UnityEngine;
using System.Collections;

namespace TransitionalObjects
{
    public class MovingTransition : BaseTransition
    {
        public enum MovementType { Absolute = 0, Local, Difference }//basically do we move from exactly one position to another, from one local position to another, or do we read the end point and move in relation
        public MovementType type = MovementType.Local;

        public bool deviateStart, deviateEnd, startAtCurrent, endAtCurrent;//use this to select between to random positions

        public Vector3 differenceStartPoint, startPoint, endPoint;
        public Vector3 minStart, maxStart, minEnd, maxEnd;

        public override void TriggerTransition(bool forceReset)
        {
            if(!forceReset)
                if(state != BaseTransition.TransitionState.Finished && state != BaseTransition.TransitionState.Waiting)
                    return;

            base.TriggerTransition(forceReset);

            differenceStartPoint = parent.transform.position;

            if(startAtCurrent)//if the start point should be the current position
            {
                switch(type)
                {
                    case MovementType.Absolute:
                        startPoint = transform.position;//then update it with the current position
                        break;

                    case MovementType.Local:
                        startPoint = parent.transform.localPosition;
                        break;
                }
            }
            else if(deviateStart)
                startPoint = Vector3.Lerp(minStart, maxStart, Random.value);//if we should deviate the start value then update it

            float value = Random.value;

            if(deviateEnd)
                endPoint = Vector3.Lerp(minEnd, maxEnd, value);
        }

        public override void TriggerFadeOut()
        {
            if(endAtCurrent)//if we need to update the end point to ensure we start smoothly
                switch(type)
                {
                    case MovementType.Absolute:
                        endPoint = transform.position;//then update it with the current position
                        break;

                    case MovementType.Local:
                        endPoint = parent.transform.localPosition;
                        break;
                }

            base.TriggerFadeOut();
        }

        protected override void Transition(float transitionPercentage)
        {
            switch(type)
            {
                case MovementType.Absolute:
                    parent.transform.position = startPoint + (endPoint - startPoint) * transitionPercentage;
                    break;

                case MovementType.Difference:
                    parent.transform.position = differenceStartPoint + (endPoint - startPoint) * transitionPercentage;//the important difference here is that this ignores aniamtions with no value. E.G if x = 0 then X wont move
                    break;

                case MovementType.Local:
                    parent.transform.localPosition = startPoint + (endPoint - startPoint) * transitionPercentage;
                    break;
            }
        }

        public override void Clone(BaseTransition other)
        {
            base.Clone(other);

            MovingTransition converted = (MovingTransition)other;

            type = converted.type;
            differenceStartPoint = converted.differenceStartPoint;
            startPoint = converted.startPoint;
            endPoint = converted.endPoint;
        }

        public override void TriggerTransitionWithoutDelay()
        {
            base.TriggerTransitionWithoutDelay();

            differenceStartPoint = parent.transform.position;
        }

        #region Editor Externals
#if(UNITY_EDITOR)
        /// <summary>
        /// This is mainly used by the editor when the type changes to update the start and end points accordingly
        /// </summary>
        public void SetType(MovementType newType)
        {
            MovementType previousType = type;
            Vector3 currentPosition = parent.transform.position;//store the old position

            type = previousType;
            ViewPosition(TransitionalObject.MovingDataType.StartPoint);//so view the position in the old type
            type = newType;//set to the new type
            UpdatePosition(TransitionalObject.MovingDataType.StartPoint);//and update position

            type = previousType;
            ViewPosition(TransitionalObject.MovingDataType.MaxStart);//so view the position in the old type
            type = newType;//set to the new type
            UpdatePosition(TransitionalObject.MovingDataType.MaxStart);//and update position

            type = previousType;
            ViewPosition(TransitionalObject.MovingDataType.EndPoint);//so view the position in the old type
            type = newType;//set to the new type
            UpdatePosition(TransitionalObject.MovingDataType.EndPoint);//and update position

            type = previousType;
            ViewPosition(TransitionalObject.MovingDataType.MaxEnd);//so view the position in the old type
            type = newType;//set to the new type
            UpdatePosition(TransitionalObject.MovingDataType.MaxEnd);//and update position

            parent.transform.position = currentPosition;//restore
        }

        /// <summary>
        /// Called by the editor to view either the start of end point
        /// </summary>
        public override void ViewPosition(TransitionalObject.MovingDataType movingType)
        {
            if(movingType == TransitionalObject.MovingDataType.StartPoint)
            {
                if(type == MovementType.Local)
                    parent.transform.localPosition = startPoint;
                else
                    parent.transform.position = startPoint;
            }
            else if(movingType == TransitionalObject.MovingDataType.MaxStart)
            {
                if(type == MovementType.Local)
                    parent.transform.localPosition = maxStart;
                else
                    parent.transform.position = maxStart;
            }
            else if(movingType == TransitionalObject.MovingDataType.MaxEnd)
            {
                if(type == MovementType.Local)
                    parent.transform.localPosition = maxEnd;
                else
                    parent.transform.position = maxEnd;
            }
            else
            {
                if(type == MovementType.Local)
                    parent.transform.localPosition = endPoint;
                else
                    parent.transform.position = endPoint;
            }
        }

        /// <summary>
        /// Called by the editor to update the start and end points based on the current position
        /// </summary>
        /// <param name="isStartPoint"></param>
        public override void UpdatePosition(TransitionalObject.MovingDataType movingType)
        {
            if(movingType == TransitionalObject.MovingDataType.StartPoint)
            {
                if(type == MovementType.Local)
                    startPoint = parent.transform.localPosition;
                else
                    startPoint = parent.transform.position;

                minStart = startPoint;
            }
            else if(movingType == TransitionalObject.MovingDataType.MaxStart)
            {
                if(type == MovementType.Local)
                    maxStart = parent.transform.localPosition;
                else
                    maxStart = parent.transform.position;
            }
            else if(movingType == TransitionalObject.MovingDataType.MaxEnd)
            {
                if(type == MovementType.Local)
                    maxEnd = parent.transform.localPosition;
                else
                    maxEnd = parent.transform.position;
            }
            else
            {
                if(type == MovementType.Local)
                    endPoint = parent.transform.localPosition;
                else
                    endPoint = parent.transform.position;

                minEnd = endPoint;
            }
        }
#endif
        #endregion
    }
}