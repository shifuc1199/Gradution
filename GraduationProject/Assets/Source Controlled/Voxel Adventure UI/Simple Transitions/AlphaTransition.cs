#define UsingUGUI
//#define UsingNGUI

using UnityEngine;
using System.Collections;

namespace TransitionalObjects
{
    [System.Serializable]
    public class AlphaTransition : BaseTransition
    {
        public bool startFaded;

        public override void Initialise()
        {
            if(startFaded)
            {
                currentTime = 0;//start at nothing
                Transition();//set the values
            }

            base.Initialise();
        }

        protected override void Transition(float transitionPercentage)
        {
#if(UsingNGUI)
            for(int i = 0; i < parent.affectedWidgets.Length; i++)
                parent.affectedWidgets[i].alpha = transitionPercentage;
#endif

#if(UsingUGUI)
            for(int i = 0; i < parent.affectedImages.Length; i++)
                parent.affectedImages[i].color = new Color(parent.affectedImages[i].color.r, parent.affectedImages[i].color.g, parent.affectedImages[i].color.b, transitionPercentage * parent.childrenMaxAlpha[parent.imageStartIndex + i]);

            for(int i = 0; i < parent.affectedCanvasGroups.Length; i++)
                parent.affectedCanvasGroups[i].alpha = transitionPercentage * parent.childrenMaxAlpha[parent.imageStartIndex + i];
#endif

            for(int i = 0; i < parent.affectedRenderers.Length; i++)
                if(parent.affectedRenderers[i].material.color != null)
                    parent.affectedRenderers[i].material.color = new Color(parent.affectedRenderers[i].material.color.r, parent.affectedRenderers[i].material.color.g, parent.affectedRenderers[i].material.color.b, transitionPercentage * parent.childrenMaxAlpha[i]);
        }

        public override void Clone(BaseTransition other)
        {
            base.Clone(other);

            startFaded = ((AlphaTransition)other).startFaded;
        }

        #region Editor Externals
#if(UNITY_EDITOR)
        /// <summary>
        /// Called by the editor to update the start and end points based on the current position
        /// </summary>
        /// <param name="isStartPoint"></param>
        public override void UpdatePosition(TransitionalObject.MovingDataType movingType)
        {
        }

        /// <summary>
        /// Called by the editor to view either the start of end point
        /// </summary>
        public override void ViewPosition(TransitionalObject.MovingDataType movingType)
        {
        }
#endif
        #endregion
    }
}