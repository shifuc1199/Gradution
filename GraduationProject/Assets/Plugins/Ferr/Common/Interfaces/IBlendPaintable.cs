using UnityEngine;
using System.Collections;

namespace Ferr {
	public interface IBlendPaintable {
	    void OnPainterSelected  (IBlendPaintType aPainter);
	    void OnPainterUnselected(IBlendPaintType aPainter);
	}
}