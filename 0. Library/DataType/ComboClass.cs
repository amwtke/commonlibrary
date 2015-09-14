using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public class ComboClass<T>
	{
		public T Value;

		public ComboClass()
		{
		}

		public ComboClass(T val)
		{
			Value = val;
		}

		public override bool Equals(object obj)
		{
			ComboClass<T> rval = obj as ComboClass<T>;
			if (obj == null) {
				return false;
			} else {
				return this.Value.Equals(rval.Value);
			}
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}

	public class ComboClass<T1, T2>
	{
		public T1 V1;
		public T2 V2;

		public ComboClass()
		{
		}

		public ComboClass(T1 v1, T2 v2)
		{
			V1 = v1;
			V2 = v2;
		}

		public override bool Equals(object obj)
		{
			ComboClass<T1, T2> rval = obj as ComboClass<T1, T2>;
			if (obj == null) {
				return false;
			} else {
				return this.V1.Equals(rval.V1) && this.V2.Equals(rval.V2);
			}
		}

		public override int GetHashCode()
		{
			return V1.GetHashCode() ^ V2.GetHashCode();
		}
	}

	public class ComboClass<T1, T2, T3>
	{
		public T1 V1;
		public T2 V2;
		public T3 V3;

		public ComboClass()
		{
		}

		public ComboClass(T1 v1, T2 v2, T3 v3)
		{
			V1 = v1;
			V2 = v2;
			V3 = v3;
		}

		public override bool Equals(object obj)
		{
			ComboClass<T1, T2, T3> rval = obj as ComboClass<T1, T2, T3>;
			if (obj == null) {
				return false;
			} else {
				return this.V1.Equals(rval.V1) && 
					this.V2.Equals(rval.V2) && 
					this.V3.Equals(rval.V3);
			}
		}

		public override int GetHashCode()
		{
			return V1.GetHashCode() ^ V2.GetHashCode() ^ V3.GetHashCode();
		}
	}

	public class ComboClass<T1, T2, T3, T4>
	{
		public T1 V1;
		public T2 V2;
		public T3 V3;
		public T4 V4;

		public ComboClass()
		{
		}

		public ComboClass(T1 v1, T2 v2, T3 v3, T4 v4)
		{
			V1 = v1;
			V2 = v2;
			V3 = v3;
			V4 = v4;
		}

		public override bool Equals(object obj)
		{
			ComboClass<T1, T2, T3, T4> rval = obj as ComboClass<T1, T2, T3, T4>;
			if (obj == null) {
				return false;
			} else {
				return this.V1.Equals(rval.V1) &&
					this.V2.Equals(rval.V2) &&
					this.V3.Equals(rval.V3) &&
					this.V4.Equals(rval.V4);
			}
		}

		public override int GetHashCode()
		{
			return V1.GetHashCode() ^ V2.GetHashCode() ^ V3.GetHashCode() ^ V4.GetHashCode();
		}
	}
}
