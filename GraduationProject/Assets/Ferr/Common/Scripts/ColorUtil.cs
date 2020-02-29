using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ferr {
    public static class ColorUtil {
        static float sqrt3 = (float)Math.Sqrt(3);

        #region Cylindrical Color Spaces
        public static Color   HSL  (float aHue, float aSaturation, float aLuminance) {
            // HSL is a circle of colors, divided into six slices
	        float hue = ((aHue % 1f) * 360.0f) / 60.0f;

            // Calculate the (XY sort of) location of the color within the color slice
	        float chroma = (1 - Mathf.Abs(2 * aLuminance - 1)) * aSaturation;
	        float x      = chroma * (1 - Mathf.Abs(hue % 2f - 1f));

            // and pick out the color based on the hue region
            float r = 0, g = 0, b = 0;
            if (hue < 1) {
                r = chroma;
                g = x;
            } else if (hue < 2) {
                r = x;
                g = chroma;
            } else if (hue < 3) {
                g = chroma;
                b = x;
            } else if (hue < 4) {
                g = x;
                b = chroma;
            } else if (hue < 5) {
                r = x;
                b = chroma;
            } else {
                r = chroma;
                b = x;
            }

            // and factor in the lightness
            float valueMatch = aLuminance - 0.5f*chroma;

	        return new Color(r + valueMatch, g + valueMatch, b + valueMatch, 1);
        }
        public static Vector3 ToHSV(float aR,   float aG,          float aB) {
	        float max = Mathf.Max(aR, Mathf.Max(aG, aB));
	        float min = Mathf.Min(aR, Mathf.Min(aG, aB));

	        float hue        = (float)Mathf.Atan2(2 * aR - aG - aB, sqrt3 * (aG - aB));
            float saturation = (max == 0) ? 0 : 1 - (1 * min / max);
            float value      = max;

            return new Vector3(hue, saturation, value);
        }
        public static Color   HSV  (float aHue, float aSaturation, float aValue) {
            // HSV is a hexagon of color, divide the hue into one of the six sectors
            float hue    = ((aHue % 1) * 360.0f) / 60.0f;

            // Calculate the (XY sort of) location of the color within the hexagon slice
            float chroma = aValue * aSaturation;
	        float x      = chroma * (1 - Mathf.Abs(hue % 2 - 1));

            // and pick out the color based on the hue region
            float r=0,g=0,b=0;
            if (hue < 1) {
                r = chroma;
                g = x;
            } else if (hue < 2) {
                r = x;
                g = chroma;
            } else if (hue < 3) {
                g = chroma;
                b = x;
            } else if (hue < 4) {
                g = x;
                b = chroma;
            } else if (hue < 5) {
                r = x;
                b = chroma;
            } else {
                r = chroma;
                b = x;
            }

            // and factor in the value/saturation
            float valueMatch = aValue - chroma;

            return new Color(r + valueMatch, g + valueMatch, b + valueMatch);
        }
        public static Color   HCL  (float aHue, float aChroma,     float aLuma) {
            // HCL is a color cube, divided up into 6 faces
            float hue = ((aHue % 1) * 360.0f) / 60.0f;

            // Calculate the XY location of the color within the face
	        float x = aChroma * (1 - Mathf.Abs(hue % 2 - 1));

            // and pick out the color based on the hue region
            float r = 0, g = 0, b = 0;
            if (hue < 1) {
                r = aChroma;
                g = x;
            } else if (hue < 2) {
                r = x;
                g = aChroma;
            } else if (hue < 3) {
                g = aChroma;
                b = x;
            } else if (hue < 4) {
                g = x;
                b = aChroma;
            } else if (hue < 5) {
                r = x;
                b = aChroma;
            } else {
                r = aChroma;
                b = x;
            }

            // and factor in the luma
            float lumaMatch = aLuma - (0.3f * r + 0.59f * g + 0.11f * b);

            return new Color(r + lumaMatch, g + lumaMatch, b + lumaMatch);
        }
        public static Color GetColorBand(Color[] aColorBand, float aValue) {
            aValue = aValue % 1.0f;
	        Color col = Color.white;
            if (aColorBand != null) {
                int index1 = (int)(aValue * (aColorBand.Length));
	            int index2 = (int)Mathf.Min((aValue * (aColorBand.Length)) + 1, aColorBand.Length - 1);
	            index1 = Mathf.Max(0, index1);
	            index1 = Mathf.Min(aColorBand.Length - 1, index1);

                Color col1 = aColorBand[index1];
                Color col2 = aColorBand[index2];

                float lerpVal = aValue - (index1 * (1f / ((float)aColorBand.Length)));
                col = Color.Lerp(col1, col2, lerpVal / (1f / ((float)aColorBand.Length)));
            }

            return col;
        }
        #endregion
	    
	    #region Hex Conversions
	    public static Color FromHex(string aHex) {
		    if (aHex.Length != 8) return Color.red;
		    return new Color( 
			    Convert.ToInt32(""+aHex[0]+aHex[1])/255f,
			    Convert.ToInt32(""+aHex[2]+aHex[3])/255f,
			    Convert.ToInt32(""+aHex[4]+aHex[5])/255f,
			    Convert.ToInt32(""+aHex[6]+aHex[7])/255f );
	    }
	    public static string ToHex(Color aColor) {
		    return string.Format("{0:X}{1:X}{2:X}{3:X}", 
		    (int)(aColor.r * 255),
		    (int)(aColor.g * 255),
		    (int)(aColor.b * 255),
		    (int)(aColor.a * 255));
	    }
	    #endregion
    }
}
