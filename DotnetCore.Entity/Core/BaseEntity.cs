using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
//抽象类目的是防止被实例化（美欧实例化必要）
namespace DotnetCore.Entity.Core
{
	/// <summary>
	/// DB表基底
	/// </summary>
	[Serializable]
	public abstract partial class BaseEntity
	{
		/// <summary>
		/// Id
		/// </summary>
		[DataMember]
		public long Id { get; set; }

		/// <summary>
		/// DB 資料版號
		/// </summary>
		/// 
		[Timestamp]
		public byte[] RowVersion { get; set; }

		/// <summary>
		/// 是否相等
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return Equals(obj as BaseEntity);
		}

		private static bool IsTransient(BaseEntity obj)
		{
			return obj != null && Equals(obj.Id, default(int));
		}

		private Type GetUnproxiedType()
		{
			return GetType();
		}

		/// <summary>
		/// 是否相等
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public virtual bool Equals(BaseEntity other)
		{
			if (other == null)
				return false;

			if (ReferenceEquals(this, other))
				return true;

			if (!IsTransient(this) &&
				!IsTransient(other) &&
				Equals(Id, other.Id))
			{
				var otherType = other.GetUnproxiedType();
				var thisType = GetUnproxiedType();
				return thisType.IsAssignableFrom(otherType) ||
						otherType.IsAssignableFrom(thisType);
			}

			return false;
		}

		/// <summary>
		/// 取出HashCode
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			if (Equals(Id, default(int)))
				return base.GetHashCode();
			return Id.GetHashCode();
		}

		/// <summary>
		/// 是否相等
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool operator ==(BaseEntity x, BaseEntity y)
		{
			return Equals(x, y);
		}

		/// <summary>
		/// 是否不相等
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool operator !=(BaseEntity x, BaseEntity y)
		{
			return !(x == y);
		}
	}
}
