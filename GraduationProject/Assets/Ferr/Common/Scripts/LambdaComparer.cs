using System;
using System.Collections.Generic;

namespace Ferr {
	/// <summary>
	/// A generic IComparer, primarily for sorting mesh segments by z value, to avoid overlap and Z issues.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class LambdaComparer<T> : IComparer<T> {
	    private readonly Func<T, T, int> func;
	    public LambdaComparer(Func<T, T, int> comparerFunc) {
	        this.func = comparerFunc;
	    }
	
	    public int Compare(T x, T y) {
	        return this.func(x, y);
	    }
	}
}