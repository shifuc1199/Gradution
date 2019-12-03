using UnityEngine;
using System.Collections;

namespace Ferr {
	public class InspectorName : PropertyAttribute {
	    public string mName;
	    public InspectorName(string aName) {
	        mName = aName;
	    }
	}
}