using UnityEngine;
using System.Collections;

namespace Ferr {
	public interface IProceduralMesh {
		Mesh       MeshData  {get;}
		MeshFilter MeshFilter{get;}
		void Build(bool aFullBuild);
	}
}