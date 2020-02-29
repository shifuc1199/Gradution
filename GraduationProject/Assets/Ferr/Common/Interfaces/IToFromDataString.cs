using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ferr {
	public interface IToFromDataString {
		string ToDataString();
		void FromDataString(string aData);
	}
}