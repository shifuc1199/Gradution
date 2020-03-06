using UnityEngine;
using System.Collections;

namespace Ferr {
	public interface ILerpable<T> {
		T Lerp(T aWith, float aLerp);
	}
}